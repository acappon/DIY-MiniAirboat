"""Performance estimate (time to plane, top speed, cruise power, fuel and range), rebuilt from the
design summary's method and calibrated to its published numbers for the original 1,280 lb boat:
- thrust 340 lb static, falling 8.5% per 10 mph
- hump drag 17% of weight; planing drag of the 66 in flat bottom at about 2.8 deg trim
  (Savitsky prismatic lift with fixed trim, ITTC friction), times a calibration factor
- air drag CdA 8 ft2
- prop efficiency 0.38 at cruise; fuel 0.59 lb/hp-h (3.2 gph at 33 hp in the summary)
Writes performance.json."""
import json
import math
import os

HERE = os.path.dirname(os.path.abspath(__file__))
G, RHO_W, RHO_A, NU = 32.174, 1.94, 0.002377, 1.0e-5
B = 66 / 12                 # planing beam, ft
TAU = 2.8                   # running trim, deg
CDA = 8.0                   # ft2
T0 = 340.0                  # static thrust, lb
ETA_P, BSFC, FUEL_LB_GAL, TANK = 0.38, 0.59, 6.1, 6.6
MPH = 1.4667


def thrust(v_mph):
    return T0 * (1 - 0.085 * v_mph / 10)


def planing_drag(W, v):
    """Savitsky-type prismatic planing surface at fixed trim (v in ft/s)."""
    q = 0.5 * RHO_W * v * v
    cl = W / (q * B * B)
    cv = v / math.sqrt(G * B)
    lo, hi = 0.05, 12.0                      # wetted length / beam
    for _ in range(80):
        lam = (lo + hi) / 2
        cl0 = TAU ** 1.1 * (0.012 * lam ** 0.5 + 0.0055 * lam ** 2.5 / cv ** 2)
        lo, hi = (lam, hi) if cl0 < cl else (lo, lam)
    re = v * lam * B / NU
    cf = 0.075 / (math.log10(re) - 2) ** 2 + 0.0004
    return W * math.tan(math.radians(TAU)) + q * lam * B * B * cf / math.cos(math.radians(TAU)), lam


def drag(W, v_mph, k):
    """Hull + air drag. Planing (v >= vp): the curve the design summary's own numbers imply,
    hydro = 138.6 lb x (W/1280)^e x (v/30)^1.32 (e = 0.43 from its solo and two-up points; e = 1 is the
    conservative bound). Below planing: rises as v^2 to 17% of weight at the hump (7.6 mph, where the
    summary's 100 lb / 65 lb thrust margins put it), then falls linearly onto the planing curve at vp."""
    v = v_mph * MPH
    air = 0.5 * RHO_A * v * v * CDA
    plane = lambda vv: 138.6 * (W / 1280) ** k['e'] * (vv / 30) ** 1.32
    vh, vp = 7.6, k['vp']
    if v_mph <= vh:
        hyd = 0.17 * W * (v_mph / vh) ** 2
    elif v_mph < vp:
        f = (v_mph - vh) / (vp - vh)
        hyd = 0.17 * W * (1 - f) + plane(vp) * f
    else:
        hyd = plane(v_mph)
    return hyd + air


def time_to(W, v_end, k, dt=0.01):
    m, v, t = W / G, 0.0, 0.0
    margin = 1e9
    while v < v_end * MPH:
        vm = v / MPH
        net = thrust(vm) - drag(W, vm, k)
        margin = min(margin, net)
        if net <= 0:
            return float('inf'), margin
        v += net / (m * 1.10) * dt              # +10% added water mass
        t += dt
    return t, margin


def top_speed(W, k):
    v = 15.0
    while thrust(v) > drag(W, v, k) and v < 70:
        v += 0.05
    return v


def cruise(W, v_mph, k):
    d = drag(W, v_mph, k)
    hp = d * v_mph * MPH / 550 / ETA_P
    gph = hp * BSFC / FUEL_LB_GAL
    return hp, gph, TANK / gph * v_mph


def speed_at_hp(W, hp, k):
    v = 20.0
    while cruise(W, v, k)[0] < hp and v < 60:
        v += 0.05
    return v


def calibrate(e):
    # vp so that the solo 1,280 lb boat reaches 15 mph in 4.0 s (summary)
    k = dict(e=e, vp=15.0)
    lo, hi = 9.0, 30.0
    for _ in range(40):
        k['vp'] = (lo + hi) / 2
        t = time_to(1280, 15, k)[0]
        lo, hi = (k['vp'], hi) if t < 4.0 else (lo, k['vp'])
    return k


def main():
    cases = {  # name: (static thrust, solo weight, two-aboard weight)
        'Original design (summary, G13 ~70 hp)': (340, 1280, 1480),
        'Rev 7 with the G13 (~70 hp)': (340, 1305 + 240, 1305 + 440),
        'Rev 7 as built with the G13B (~100 hp)': (430, 1305 + 19 + 240, 1305 + 19 + 440),
    }
    out = dict(models={})
    for model, e in (('summary model (drag ~ W^0.43)', 0.43), ('conservative (drag ~ W)', 1.0)):
        globals()['T0'] = 340.0
        k = calibrate(e)
        print('== %s: planing reached at %.1f mph' % (model, k['vp']))
        res = {}
        for name, (t0, w1, w2) in cases.items():
            globals()['T0'] = t0
            rows = {}
            for label, W in (('solo, full fuel', w1), ('two aboard, full fuel', w2)):
                t, margin = time_to(W, 15, k)
                vmax = top_speed(W, k)
                hp30, gph30, rng30 = cruise(W, 30, k)
                v33 = speed_at_hp(W, 33, k)
                hp33, gph33, rng33 = cruise(W, v33, k)
                rows[label] = dict(W=W, t_plane=t, hump_margin=margin, v_top=vmax, hp_30=hp30, gph_30=gph30, range_30=rng30,
                                   v_at_33hp=v33, gph_33hp=gph33, range_33hp=rng33)
                print('%-44s %-22s W %5.0f  plane %4.1f s  hump margin %4.0f lb  top %4.1f mph | 30 mph: %4.1f hp %3.1f gph %3.0f mi | 33 hp: %4.1f mph %3.0f mi'
                      % (name, label, W, t, margin, vmax, hp30, gph30, rng30, v33, rng33))
            res[name] = rows
        out['models'][model] = dict(k=k, cases=res)
    json.dump(out, open(os.path.join(HERE, 'performance.json'), 'w'), indent=1)


if __name__ == '__main__':
    main()
