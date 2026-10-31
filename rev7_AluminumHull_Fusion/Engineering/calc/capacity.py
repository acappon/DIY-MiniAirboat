"""Capacity label numbers by the US Coast Guard method for inboard boats (33 CFR 183.33 and
183.39), applied voluntarily: a home-built boat for the builder's own use is not a
manufacturer's product, but the method gives an honest, defensible load limit.

Also checks the 33 CFR 183 Subpart F basic-flotation load (2/15 of the persons capacity plus
25% of the dead weight, as submerged weight, with the two largest air chambers left out).
"""
import os, sys
import numpy as np
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
import mass, hydro, swamped

PERSON_AREAS = [(2290.0, 'driver'), (1620.0, 'passenger')]
SEAT_Z = 470.0          # 18.5 in cushion top
EDGE_Y = -850.0         # outboard edge of the passenger area, inside the starboard rub rail


def boat_weight_items():
    """Boat weight per 183.33: hull, deck, permanent appurtenances; the 6.6 gal tank is portable
    and the BOM counts it empty, so no fuel is included (183.39 test uses the same)."""
    return mass.lightship()


def machinery_lb(items):
    keys = ('drivetrain', 'battery_group31', 'steering', 'throttle', 'shifter', 'power_distribution', 'main_harness')
    return sum(i['lb'] for i in items if any(k in i['path'] for k in keys))


def max_displacement():
    """Water displaced at maximum level immersion: transversely level, no trim, water at the
    lowest point of ingress (the lowest sheer)."""
    zmin = hydro.SHEER[hydro.SHEER > 0].min()
    v, _ = hydro.displaced(zmin, 0.0, 0.0)
    return v * hydro.RHO, zmin


def offset_state(items, A):
    """Heel, freeboard and trim with A lb on the starboard edge at seat height."""
    W0, G0 = mass.cg(items)
    W = W0 + A
    Gx = (W0 * G0[0] + sum(A / 2 * x for x, _ in PERSON_AREAS)) / W
    G = np.array([Gx, (W0 * G0[1] + A * EDGE_Y) / W, (W0 * G0[2] + A * SEAT_Z) / W])
    T, tau, _ = hydro.equilibrium(W, np.array([Gx, 0.0, G[2]]))
    lo, hi = 0.0, np.radians(30)
    for _ in range(16):   # heel where the righting arm balances the offset weight
        p = (lo + hi) / 2
        Tp, _, c = hydro.equilibrium(W, G, p, tau)
        arm = (G[1] * np.cos(p) - G[2] * np.sin(p)) - (c[1] * np.cos(p) - c[2] * np.sin(p))
        lo, hi = (p, hi) if arm < 0 else (lo, p)
    fb, x = hydro.freeboard(Tp, tau, p)
    return np.degrees(p), fb, x


def offset_test(items):
    """183.39(a)(2): weight along one outboard edge at seat height, shared between the two
    passenger areas, until water reaches the sheer. Returns A (lb), heel (deg)."""
    lo, hi = 0.0, 2000.0
    for _ in range(9):
        A = (lo + hi) / 2
        heel, fb, _ = offset_state(items, A)
        print('   offset %.0f lb: heel %.1f deg, freeboard %.2f in' % (A, heel, fb / 25.4), flush=True)
        lo, hi = (A, hi) if fb > 0 else (lo, A)
    return lo, offset_state(items, lo)[0]


if __name__ == '__main__':
    items = boat_weight_items()
    Wb, _ = mass.cg(items)
    M = machinery_lb(items)
    D, zmin = max_displacement()
    W1 = D / 5 - Wb / 5 - 4 * M / 5
    W2 = (D - Wb) / 7
    Wmax = max(W1, W2)
    print('boat weight %.0f lb, machinery %.0f lb, max displacement %.0f lb (water at %.1f in)' % (Wb, M, D, zmin / 25.4))
    print('max weight capacity: formula 1 %.0f lb, formula 2 %.0f lb -> %.0f lb' % (W1, W2, Wmax))
    A, heel = offset_test(items)
    P_test = A / 0.6
    P = min(Wmax, P_test) if P_test < 550 else Wmax
    print('offset test: A = %.0f lb at %.1f deg heel -> %.0f lb; persons capacity %.0f lb' % (A, heel, P_test, P))
    print('persons (whole number) = round((%.0f + 32) / 141) = %d' % (P, int((P + 32) / 141 + 0.5)))
    dead = Wmax - P
    sub = 2 / 15 * P + 0.25 * dead
    r = swamped.analyse(cooler=False, bay=False, extra_lb=sub, label='Subpart F load, bench air chambers excluded')
    print('Subpart F: submerged load %.0f lb -> reserve %+.0f lb, freeboard min %+.1f in, GM %.1f in' % (
        sub, r['reserve'], r['fb_min'] / 25.4, r['GM'] / 25.4))
