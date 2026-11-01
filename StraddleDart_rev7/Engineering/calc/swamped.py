"""Swamped flotation: the hull is full of water, so only the parts themselves (foam, wood,
metal, sealed air spaces) displace water. Lost-buoyancy method: each body keeps its weight and
its own displaced volume; the boat floats where the submerged part of those volumes carries
the total weight.

Each body is treated as its bounding box filled evenly with its displaced volume, cut into
slices so trim and heel are followed across wide blocks. The hull skin is the exception: most
of its metal is the bottom plating, so 75% of it sits in the bottom 10 mm.
"""
import os, re, sys
import numpy as np
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
import mass
from hydro import XS, SHEER, B, XM

RHO = 1.0 / mass.LB                       # lb per cm3 of fresh water
N = 6                                     # slices per body in x and y

# Displaced volume of lumped (placeholder / bought) items: effective density of the solid
# material in g/cm3, or 'sealed' to use the full CAD volume (closed, watertight items).
DISPLACE = [
    (r'ENGINE_G13|starter', 3.5),         # aluminium block and head, steel crank; water fills the rest
    (r'radiator_Civic', 1.6),             # floods; metal only
    (r'PROP_WarpDrive', 1.6),
    (r'muffler', 7.85),
    (r'belt_HTD8M', 1.2),
    (r'FUEL_TANK', 0.95),                 # empty and vented: plastic only (conservative)
    (r'battery_group31', 'sealed'),
    (r'bench_cushion|backrest_removable|armrest_pad', 1.0),  # vinyl over foam that soaks up water
    (r'cage_light_bar', 2.7), (r'searchlight', 2.0), (r'safety_gear', 1.5), (r'BILGE_PUMP', 1.5),
    (r'engine_front_guard_mesh', 7.85), (r'deck_plate_6in', 1.0), (r'sump_grate_FRP', 1.8),
    (r'driver_dash', 1.5), (r'power_distribution', 1.5),
    (r'RUDDER_.*honeycomb', 2.7),         # assume the cells flood (conservative)
    (r'MESH_', 7.85),
]
DX_RE = [(re.compile(p), d) for p, d in DISPLACE]

# Sealed bench spaces (cm3, x, z extents mm). Inside: y +-152, z 90..394 mm.
BENCH_COOLER = dict(name='bench cooler air (lid latched, plug in)', vol=77.5 * 30.4 * 30.4, x=(1435, 2210))
BENCH_BAY = dict(name='battery bay air (lid bolted)', vol=40.7 * 30.4 * 30.4 - 13617 - 289, x=(2222, 2629))


def parts(cooler=True, bay=True, extra=()):
    """Weight items with displaced volumes: list of dict(lb, vol cm3, lo, hi, skin)."""
    out = []
    for i in mass.lightship():
        key = i['path'] + '|' + i['body']
        if i['rule'] == 'allowance':
            v = i['lb'] * mass.LB / 2.7
        else:
            v = None
            for rx, d in DX_RE:
                if rx.search(key):
                    v = i['vol'] if d == 'sealed' else i['lb'] * mass.LB / d
                    break
            if v is None:
                v = i['vol']                # density-rule bodies: the CAD volume is the material
        out.append(dict(name=key, lb=i['lb'], vol=v, lo=i['lo'], hi=i['hi'], skin='hull_skin' in key))
    for sp, on in ((BENCH_COOLER, cooler), (BENCH_BAY, bay)):
        if on:
            out.append(dict(name=sp['name'], lb=0.0, vol=sp['vol'], lo=[sp['x'][0], -152, 90],
                            hi=[sp['x'][1], 152, 394], skin=False))
    out.extend(extra)
    return out


def _slices(p):
    lo, hi = np.array(p['lo'], float), np.array(p['hi'], float)
    hi = np.maximum(hi, lo + 1.0)
    fx = (np.arange(N) + 0.5) / N
    xs = lo[0] + fx * (hi[0] - lo[0])
    ys = lo[1] + fx * (hi[1] - lo[1])
    X, Y = np.meshgrid(xs, ys, indexing='ij')
    return X.ravel(), Y.ravel(), lo[2], hi[2]


def prepare(ps):
    rows = []
    for p in ps:
        X, Y, z0, z1 = _slices(p)
        v = p['vol'] / X.size
        if p['skin']:   # bottom plating at the bottom, the rest spread up the sides
            rows += [(x, y, z0, z0 + 10, 0.75 * v) for x, y in zip(X, Y)]
            rows += [(x, y, z0, z1, 0.25 * v) for x, y in zip(X, Y)]
        else:
            rows += [(x, y, z0, z1, v) for x, y in zip(X, Y)]
    return np.array(rows)


def buoyancy(R, T, tau, phi):
    """Lift (lb) and centre of buoyancy of the submerged part of every slice."""
    wl = T + tau * (R[:, 0] - XM) - R[:, 1] * np.tan(phi)
    f = np.clip((wl - R[:, 2]) / (R[:, 3] - R[:, 2]), 0, 1)
    v = f * R[:, 4]
    s = v.sum()
    zc = R[:, 2] + f * (R[:, 3] - R[:, 2]) / 2
    return s * RHO, np.array([(v * R[:, 0]).sum(), (v * R[:, 1]).sum(), (v * zc).sum()]) / max(s, 1e-9)


def float_at(R, W, G, phi=0.0, tau=None):
    def draft(t):
        lo, hi = -500.0, 2000.0
        for _ in range(50):
            T = (lo + hi) / 2
            if buoyancy(R, T, t, phi)[0] > W:
                hi = T
            else:
                lo = T
        return (lo + hi) / 2
    if tau is not None:
        T = draft(tau)
        return T, tau, buoyancy(R, T, tau, phi)[1]
    t0, t1 = 0.0, -0.03
    def e(t):
        T = draft(t)
        return buoyancy(R, T, t, phi)[1][0] - G[0], T
    e0, _ = e(t0)
    e1, T = e(t1)
    for _ in range(40):
        if abs(e1) < 0.5 or e1 == e0:
            break
        t0, t1, e0 = t1, t1 - e1 * (t1 - t0) / (e1 - e0), e1
        e1, T = e(t1)
    return T, t1, buoyancy(R, T, t1, phi)[1]


def max_lift(R):
    return R[:, 4].sum() * RHO


def analyse(cooler=True, bay=True, extra_lb=0.0, label=''):
    extra = []
    if extra_lb:   # dense gear (e.g. tools, anchor, ammo) on the floor amidships: no lift
        extra.append(dict(name='dense gear', lb=extra_lb, vol=0.0, lo=[1500, -200, 80], hi=[2100, 200, 150], skin=False))
    ps = parts(cooler, bay, extra)
    W = sum(p['lb'] for p in ps)
    G = np.array([sum(p['lb'] * (p['lo'][k] + p['hi'][k]) / 2 for p in ps) / W for k in range(3)])
    # use the true CG from the weight model for the boat itself
    items = mass.lightship()
    Wb, Gb = mass.cg(items)
    G = (np.array(Gb) * Wb + sum(np.array([(p['lo'][k] + p['hi'][k]) / 2 for k in range(3)]) * p['lb']
                                  for p in ps if p['name'] == 'dense gear')) / W if extra_lb else np.array(Gb)
    R = prepare(ps)
    lift = max_lift(R)
    T, tau, cb = float_at(R, W, G)
    wl = T + tau * (XS - XM)
    fb = SHEER - wl
    i = np.argmin(fb)
    # swamped righting arm curve (trim held)
    gzs = []
    for deg in (2, 5, 10, 15, 20, 25, 30):
        p = np.radians(deg)
        Tp, _, c = float_at(R, W, G, p, tau)
        gzs.append((deg, (G[1] * np.cos(p) - G[2] * np.sin(p)) - (c[1] * np.cos(p) - c[2] * np.sin(p))))
    # GM from the slope across upright (+-2 deg): the boat's small off-centre CG makes GZ(0) non-zero,
    # so GZ(2)/sin(2) alone would mix the list moment into GM
    pm = np.radians(-2)
    _, _, cm = float_at(R, W, G, pm, tau)
    gzm = (G[1] * np.cos(pm) - G[2] * np.sin(pm)) - (cm[1] * np.cos(pm) - cm[2] * np.sin(pm))
    gm = (gzs[0][1] - gzm) / (2 * np.sin(np.radians(2)))
    gz0 = (gzs[0][1] + gzm) / 2                      # GZ at upright (list moment / W)
    list_deg = -np.degrees(np.arctan2(gz0, gm))       # + = starboard down
    return dict(label=label, W=W, lift=lift, reserve=lift - W, T=T, trim=np.degrees(np.arctan(-tau)),
                fb_min=fb[i], fb_x=XS[i], fb_transom=fb[0], fb_bow=fb[np.argmin(np.abs(XS - 3048))],
                GM=gm, GZ=gzs, list=list_deg, GZ_port=gzm)


CASES = [
    dict(label='Swamped, no crew or fuel, bench sealed (design case)', cooler=True, bay=True),
    dict(label='Swamped, cooler lid open (battery bay sealed)', cooler=False, bay=True),
    dict(label='Swamped, whole bench flooded', cooler=False, bay=False),
    dict(label='Swamped, bench sealed, + 100 lb dense gear', cooler=True, bay=True, extra_lb=100),
]


if __name__ == '__main__':
    for c in CASES:
        r = analyse(c.get('cooler'), c.get('bay'), c.get('extra_lb', 0), c['label'])
        print('%-55s W %5.0f lb, lift if fully under %5.0f lb, reserve %+5.0f lb | trim %+5.1f deg | '
              'freeboard min %+5.1f in at x %3.0f in, transom %+5.1f, x120 %+5.1f | GM %5.1f in | GZ %s' % (
              r['label'], r['W'], r['lift'], r['reserve'], r['trim'], r['fb_min'] / 25.4, r['fb_x'] / 25.4,
              r['fb_transom'] / 25.4, r['fb_bow'] / 25.4, r['GM'] / 25.4,
              ' '.join('%d:%.1f' % (d, g / 25.4) for d, g in r['GZ'])))
