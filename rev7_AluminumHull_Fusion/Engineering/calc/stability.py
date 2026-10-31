"""Intact stability: righting-arm curves and the heel from everyday heeling moments.
Writes results.json with everything the report quotes (hydrostatics, swamped cases, GZ)."""
import json, os, sys
import numpy as np
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
import mass, hydro, swamped

HERE = os.path.dirname(os.path.abspath(__file__))
ANGLES = [0, 2, 4, 6, 8, 10, 12, 14, 16, 20, 25]

# Side area of the boat above water for beam wind, m2 and centroid height above the hull bottom (mm):
# cage drum (64 in prop disc seen edge-on with screens) 0.9 m2 at 1335; two rudders 2 x 0.40 m2 at 1150;
# radiator, engine, bench, people about 0.5 m2 at 700; hull topsides 3.8 m x 0.25 m at 330.
WIND_AREAS = [(0.9, 1335), (0.8, 1150), (0.5, 700), (0.95, 330)]


def gz_curve(W, G, tau):
    out = []
    for deg in ANGLES:
        p = np.radians(deg)
        T, _, c = hydro.equilibrium(W, G, p, tau)
        arm = (G[1] * np.cos(p) - G[2] * np.sin(p)) - (c[1] * np.cos(p) - c[2] * np.sin(p))
        out.append([deg, arm, hydro.freeboard(T, tau, p)[0]])
    return out


def heel_for_moment(curve, W, M):
    """Heel (deg) where W * GZ equals the heeling moment M (lb-mm), from the GZ curve."""
    for (d0, g0, _), (d1, g1, _) in zip(curve, curve[1:]):
        if W * g1 >= M:
            return d0 + (d1 - d0) * (M - W * g0) / (W * (g1 - g0))
    return None


def wind_moment(knots, T):
    q = 0.5 * 1.2 * (knots * 0.5144) ** 2          # Pa
    Fz = sum(a * q * 1.2 * (z - T / 2) / 1000 for a, z in WIND_AREAS)   # N m, lever to half draft
    return Fz / 4.448 / 0.0254 * 25.4               # lb-mm


def main():
    res = dict(cases=[], swamped=[], notes={})
    for r in hydro.report():
        res['cases'].append({k: float(v) if not isinstance(v, str) else v for k, v in r.items()})
    for name in ('Solo, full fuel', 'Solo + passenger, full fuel'):
        items = mass.load_case(name.lower().replace('+ passenger', 'passenger'))
        W, G = mass.cg(items)
        G = np.array(G)
        G_up = G.copy()
        G_up[1] = 0.0                    # curve for a centred CG; the list is reported separately
        T, tau, _ = hydro.equilibrium(W, G_up)
        curve = gz_curve(W, G_up, tau)
        # a 200 lb person standing on the floor at the side (boarding, hauling a line), CG 36 in up
        m_person = 200 * (850 - 0) * np.cos(0)
        heel_person = heel_for_moment(curve, W + 200, m_person)
        heel_wind = heel_for_moment(curve, W, wind_moment(30, T))
        res['cases'].append(dict(case=name + ' (GZ)', W=W, GZ=curve, heel_person_at_rail=heel_person,
                                 heel_wind30=heel_wind))
        print(name, 'GZ in:', ' '.join('%d:%.1f' % (d, g / 25.4) for d, g, _ in curve),
              '| 200 lb at rail: %s deg | 30 kn beam wind: %.1f deg' % (
                  'n/a' if heel_person is None else '%.1f' % heel_person, heel_wind))
    for c in swamped.CASES:
        r = swamped.analyse(c.get('cooler'), c.get('bay'), c.get('extra_lb', 0), c['label'])
        res['swamped'].append({k: (float(v) if isinstance(v, (float, np.floating)) else v) for k, v in r.items()})
    json.dump(res, open(os.path.join(HERE, 'results.json'), 'w'), indent=1, default=float)


if __name__ == '__main__':
    main()
