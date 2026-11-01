"""Re-check of the proposed side foam strips (Finding 1), with the corrected swamped GM.
Each variant adds two urethane strips (2 lb/ft3) along the hull sides and re-floats the swamped
boat; reports GM, list, largest righting arm toward the list (starboard) and the moment it makes."""
import sys, os
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
import numpy as np
import mass, swamped as S


def run(label, strips):
    extra = []
    for x0, x1, y0, y1, z0, z1 in strips:
        for sgn in (1, -1):
            v = (x1 - x0) * (y1 - y0) * (z1 - z0) / 1000.0           # cm3
            lb = v * 0.032 / mass.LB
            ylo, yhi = sorted((sgn * y0, sgn * y1))
            extra.append(dict(name='side foam', lb=lb, vol=v, lo=[x0, ylo, z0], hi=[x1, yhi, z1], skin=False))
    ps = S.parts(True, True, extra)
    items = mass.lightship()
    Wb, Gb = mass.cg(items)
    W = Wb + sum(p['lb'] for p in extra)
    G = (np.array(Gb) * Wb + sum(np.array([(p['lo'][k] + p['hi'][k]) / 2 for k in range(3)]) * p['lb'] for p in extra)) / W
    R = S.prepare(ps)
    lift = S.max_lift(R)
    T, tau, _ = S.float_at(R, W, G)
    wl = T + tau * (S.XS - S.XM)
    fb = (S.SHEER - wl).min()
    gz = {}
    for deg in (-10, -5, -2, 2, 5, 8, 10, 12, 15, 20, 25):
        p = np.radians(deg)
        _, _, c = S.float_at(R, W, G, p, tau)
        gz[deg] = (G[1] * np.cos(p) - G[2] * np.sin(p)) - (c[1] * np.cos(p) - c[2] * np.sin(p))
    gm = (gz[2] - gz[-2]) / (2 * np.sin(np.radians(2)))
    lst = -np.degrees(np.arctan2((gz[2] + gz[-2]) / 2, gm))
    mx = max(v for d, v in gz.items() if d > 0)
    at = [d for d, v in gz.items() if v == mx][0]
    print('%-46s +%4.1f lb  reserve %4.0f lb  fb %.1f in  GM %4.1f in  list %.1f deg  GZmax %.2f in at %d deg  RM %3.0f lb-ft'
          % (label, sum(p['lb'] for p in extra), lift - W, fb / 25.4, gm / 25.4, lst, mx / 25.4, at, W * mx / 25.4 / 12))


if __name__ == '__main__':
    run('no strips (as built)', [])
    run('strips x 51-110 in, z 110-230 (original idea)', [(1290, 2790, 760, 860, 110, 230)])
    run('strips x 51-94.5 in (stop at the console)', [(1290, 2400, 760, 860, 110, 230)])
    run('strips x 51-94.5 in, raised z 200-320', [(1290, 2400, 760, 860, 200, 320)])
    run('strips x 51-94.5 in, 6 in tall z 110-260', [(1290, 2400, 760, 860, 110, 262)])
    run('strips 6 x 6 in high under the gunwale z 250-400', [(1290, 2400, 760, 912, 250, 400)])
    run('gunwale strips + console cabinet foam (stbd)', [(1290, 2400, 760, 912, 250, 400)])
