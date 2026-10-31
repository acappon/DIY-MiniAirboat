"""Hydrostatics and intact stability of the rev 7 airboat in fresh water.

The hull envelope comes from data/sections.json: cuts through the skin, UHMW strips, chine
guards and nose cap every 25 mm, exported by fusion_cut_sections.py. Each station becomes
a half-breadth table b(z); the hull below the sheer is rasterised on a y-z grid and the water
plane z = T + tau (x - XM) - y tan(phi) is applied to it (model mm; heel phi > 0 = starboard down).
"""
import json, os, sys
import numpy as np
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
import mass

HERE = os.path.dirname(os.path.abspath(__file__))
RHO = 1.000e-6 * 1000 / mass.LB   # fresh water, lb per mm3 (1000 kg/m3)
DX, DZ, DY = 25.0, 4.0, 8.0
XM = 1905.0                        # trim reference, mid-length of the hull


def envelope():
    """Stations x, z grid, half-breadth b[x, z] and sheer height per station."""
    sec = json.load(open(os.path.join(HERE, 'data', 'sections.json')))
    xs = np.array(sorted(float(k) for k in sec))
    zs = np.arange(-10.0 + DZ / 2, 470.0, DZ)
    b = np.zeros((len(xs), len(zs)))
    sheer = np.zeros(len(xs))
    for i, x in enumerate(xs):
        for name, pts in sec['%.0f' % x]:
            p = np.array(pts)
            if name == 'hull_skin_aluminum':
                sheer[i] = max(sheer[i], p[:, 1].max())
            y0, z0, y1, z1 = p[:-1, 0], p[:-1, 1], p[1:, 0], p[1:, 1]
            for j, z in enumerate(zs):
                m = (z0 - z) * (z1 - z) <= 0
                if not m.any():
                    continue
                dz = np.where(np.abs(z1 - z0) < 1e-9, 1e-9, z1 - z0)
                t = np.clip((z - z0[m]) / dz[m], 0, 1)
                y = np.abs(y0[m] + t * (y1[m] - y0[m]))
                b[i, j] = max(b[i, j], y.max())
        b[i, zs > sheer[i]] = 0  # the envelope ends at the sheer
    return xs, zs, b, sheer


XS, ZS, B, SHEER = envelope()
YS = np.arange(-950 + DY / 2, 950, DY)
INSIDE = np.abs(YS)[None, None, :] <= B[:, :, None]          # [x, z, y] hull cells
CX, CZ, CY = np.meshgrid(XS, ZS, YS, indexing='ij')
CELL = DX * DZ * DY


def displaced(T, tau, phi):
    """Volume (mm3) and centroid of the hull envelope below the water plane (cells cut by the plane
    count by the fraction below it, so volume varies smoothly with draft)."""
    wl = T + tau * (CX - XM) - CY * np.tan(phi)
    f = np.clip((wl - (CZ - DZ / 2)) / DZ, 0, 1) * INSIDE
    s = f.sum()
    if s == 0:
        return 0.0, np.zeros(3)
    return s * CELL, np.array([(f * CX).sum() / s, (f * CY).sum() / s, (f * CZ).sum() / s])


def draft_for(W, tau, phi):
    """Draft at XM that floats W lb (bisection; volume rises with draft)."""
    lo, hi = -400.0, 900.0
    for _ in range(40):
        T = (lo + hi) / 2
        if displaced(T, tau, phi)[0] * RHO > W:
            hi = T
        else:
            lo = T
    return (lo + hi) / 2


def equilibrium(W, G, phi=0.0, tau=None):
    """Draft T at XM and trim tau (dz/dx) for weight W (lb) at G; free trim unless tau is given."""
    if tau is not None:
        T = draft_for(W, tau, phi)
        return T, tau, displaced(T, tau, phi)[1]
    def err(t):
        T = draft_for(W, t, phi)
        return displaced(T, t, phi)[1][0] - G[0], T
    t0, t1 = 0.0, -0.02
    e0, _ = err(t0)
    e1, T = err(t1)
    for _ in range(30):
        if abs(e1) < 0.5 or e1 == e0:
            break
        t0, t1, e0 = t1, t1 - e1 * (t1 - t0) / (e1 - e0), e1
        e1, T = err(t1)
    return T, t1, displaced(T, t1, phi)[1]


def gz(W, G, T, tau, phis):
    """Righting arm (mm) at each heel angle, trim held at the upright value."""
    out = []
    for p in phis:
        Tp, _, c = equilibrium(W, G, p, tau)
        hB = c[1] * np.cos(p) - c[2] * np.sin(p)
        hG = G[1] * np.cos(p) - G[2] * np.sin(p)
        out.append((p, hG - hB, Tp))
    return out


def waterplane_gm(T, tau, G, W):
    """Metacentric height from the waterplane: GM = KB + I_T / V - KG (upright)."""
    wl = T + tau * (XS - XM)
    bw = np.array([np.interp(w, ZS, B[i]) for i, w in enumerate(wl)])
    IT = (2.0 / 3.0) * (bw ** 3).sum() * DX
    v, c = displaced(T, tau, 0.0)
    return c[2] + IT / v - G[2], c


def freeboard(T, tau, phi=0.0):
    """Lowest freeboard to the sheer (mm) along the low side, and where it is."""
    wl = T + tau * (XS - XM) + np.max(B, axis=1) * np.tan(phi)
    fb = SHEER - wl
    ok = SHEER > 0
    i = np.argmin(np.where(ok, fb, 1e9))
    return fb[i], XS[i]


CASES = ['Empty, no fuel', 'Solo, 1 gal fuel', 'Solo, full fuel', 'Solo + 100 lb cargo, full fuel',
         'Solo + passenger, full fuel']


def report():
    rows = []
    for name in CASES:
        items = mass.load_case(name.lower().replace('+ passenger', 'passenger'))
        W, G = mass.cg(items)
        G = np.array(G)
        T, tau, c = equilibrium(W, G)
        gm, _ = waterplane_gm(T, tau, G, W)
        fb, fbx = freeboard(T, tau)
        d_tr = T + tau * (0 - XM)
        d_bow = T + tau * (XS[-1] - XM)
        # static list from the off-centre CG: small-angle tan(list) = TCG / GM
        lst = np.degrees(np.arctan2(-G[1], gm))
        # heel at which the low sheer reaches the water (upright trim)
        for deg in np.arange(0.5, 40, 0.25):
            if freeboard(T, tau, np.radians(deg))[0] <= 0:
                break
        rows.append(dict(case=name, W=W, LCG=G[0], TCG=G[1], VCG=G[2], T=T, trim=np.degrees(np.arctan(-tau)),
                         d_transom=d_tr, d_bow=d_bow, fb=fb, fbx=fbx, GM=gm, list=lst, flood=deg))
    return rows


if __name__ == '__main__':
    print('hull: %d stations, sheer %0.f..%0.f mm, max half-breadth %.0f mm' % (len(XS), SHEER[SHEER > 0].min(), SHEER.max(), B.max()))
    v, c = displaced(1000, 0, 0)
    print('envelope volume to the sheer %.2f ft3, LCB %.0f' % (v / 28316846.6, c[0]))
    for r in report():
        print('%-34s W %6.0f lb LCG %5.1f in VCG %4.1f in | draft transom %4.1f in bow %4.1f in trim %+5.2f deg (bow up +) | '
              'freeboard %4.1f in at x %3.0f in | GM %5.1f in | list %+4.1f deg | sheer wets at %4.1f deg' % (
              r['case'], r['W'], r['LCG'] / 25.4, r['VCG'] / 25.4, r['d_transom'] / 25.4, r['d_bow'] / 25.4, r['trim'],
              r['fb'] / 25.4, r['fbx'] / 25.4, r['GM'] / 25.4, r['list'], r['flood']))
