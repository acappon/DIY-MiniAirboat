"""Bottom plating and stringer check after ISO 12215-5:2008 (small craft, design pressures and
scantlings, simplified method for motor craft). Equations as published in the 2008 edition; the
full text was not available while writing this, so a reviewer must confirm each one against a
licensed copy (see the notes in the structural report).

Units follow the standard: m, kg, knots, kN/m2 (kPa), N/mm2 (MPa), mm. Writes iso_bottom.json."""
import json
import math
import os

HERE = os.path.dirname(os.path.abspath(__file__))
IN = 0.0254

# --- craft data (rev 7) ------------------------------------------------------------------------
M_EMPTY_LB = 1247.0 + 52                 # CAD weight model, empty, + the 0.125 bottom and three frames
M_LDC = (M_EMPTY_LB + 508 + 6.6 * 6.1) * 0.4536   # loaded: + persons/gear capacity + full fuel, kg
LWL = 126 * IN                           # loaded waterline: flat bottom to x 120 in + the start of the roll-up
BC = 72 * IN                             # chine beam: 66 in flat + 3 in each 45 deg chine
BETA = 10.0                              # flat bottom; the standard says not less than 10 deg
V = 39 / 1.1508                          # declared top speed, knots
KDC = {'C (inshore)': 0.6, 'D (sheltered waters)': 0.4}

# 5052-H32 riveted (no welds in the bottom): unwelded properties
SIG_U, SIG_Y = 215.0, 160.0              # MPa minimum
SD_PLATE = min(0.6 * SIG_U, 0.9 * SIG_Y)  # plating design stress
SD_STIFF = min(0.7 * SIG_U, 0.9 * SIG_Y)  # stiffener design stress

# bottom panels: clear spans between hat-stringer flanges (hats 2.76 in wide) and frame flanges (1.5 in)
HAVE_T = 0.125                           # as built (was 0.090)
PANELS = {  # as built: frames at 0, 12, 24, 36, 48, 60, 72, 84, 96, 111.5 and 120 in
    'outer bay, x 96-111.5 in (14 in clear)': dict(b=16.5 - 2.76, l=111.5 - 1.5 - 96, x=104),
    'inner bay, x 96-111.5 in': dict(b=14.8 - 2.76, l=111.5 - 1.5 - 96, x=104),
    'outer bay, x 48-96 in (frames 12 in apart)': dict(b=12 - 1.5, l=16.5 - 2.76, x=90),
    'outer bay, x 0-48 in (frames 12 in apart)': dict(b=12 - 1.5, l=16.5 - 2.76, x=30),
}


def ncg_eq1(m, L=LWL, B=BC, beta=BETA, v=V):
    return 0.32 * (L / (10 * B) + 0.084) * (50 - beta) * v ** 2 * B ** 2 / m


def ncg_eq2(m, v=V):
    return 0.5 * v / m ** 0.17


def kL(x, n):
    n3 = min(n, 3.0)
    r = x / LWL
    return 1.0 if r >= 0.6 else min(1.0, (1 - 0.167 * n3) / 0.6 * r + 0.167 * n3)


def k2(ar):
    return min(0.5, (0.271 * ar ** 2 + 0.910 * ar - 0.554) / (ar ** 2 - 0.313 * ar + 1.351))


def kar_plate(b_mm, l_mm, m):
    AD = min(b_mm * l_mm, 2.5 * b_mm ** 2) * 1e-6
    kR = 1.5 - 3e-4 * b_mm                    # planing craft, plating
    return max(0.25, min(1.0, kR * 0.1 * m ** 0.15 / AD ** 0.3))


def bottom_pressure(kdc, n, kar, kl, m=M_LDC):
    base_p = 0.1 * m / (LWL * BC) * (1 + kdc ** 0.5 * n)
    base_d = 2.4 * m ** 0.33 + 20
    pmin = 0.45 * m ** 0.33 + 0.9 * LWL * kdc
    return max(base_p * kar * kl, base_d * kar * kdc * kl, pmin), base_p


def plate_t(b_mm, P, ar):
    return b_mm * math.sqrt(P * k2(ar) / (1000 * SD_PLATE))


def hat_sm(t_hat, h=2.5, top=1.0, flange=0.88, t_skin=0.090):
    """Section modulus (in3, at the hat top) of a hat stringer with 20 t of attached bottom plating."""
    parts = [(top * t_hat, h - t_hat / 2), (2 * (h - t_hat) * t_hat, (h - t_hat) / 2 + 0.0),
             (2 * flange * t_hat, t_hat / 2), (20 * t_skin * t_skin, -t_skin / 2)]
    A = sum(a for a, _ in parts)
    yb = sum(a * y for a, y in parts) / A
    I = top * t_hat ** 3 / 12 + top * t_hat * (h - t_hat / 2 - yb) ** 2
    I += 2 * t_hat * (h - t_hat) ** 3 / 12 + 2 * (h - t_hat) * t_hat * ((h - t_hat) / 2 - yb) ** 2
    I += 2 * flange * t_hat ** 3 / 12 + 2 * flange * t_hat * (t_hat / 2 - yb) ** 2
    I += 20 * t_skin * t_skin ** 3 / 12 + 20 * t_skin * t_skin * (-t_skin / 2 - yb) ** 2
    return I / (h - yb)


def stringer_check(kdc, n, s_in, lu_in, t_hat):
    s, lu = s_in * IN, lu_in * IN
    AD = min(s * lu, 0.33 * lu ** 2)
    kR = 1 - 2e-4 * lu * 1000
    kar = max(0.25, min(1.0, kR * 0.1 * M_LDC ** 0.15 / AD ** 0.3))
    P, _ = bottom_pressure(kdc, n, kar, 1.0)
    sm_req = 83.33 * P * s * lu ** 2 / SD_STIFF          # cm3 (kCS = 1)
    return dict(P=P, kar=kar, sm_req_in3=sm_req / 16.387, sm_have_in3=hat_sm(t_hat))


def main():
    m = M_LDC
    n1, n2 = ncg_eq1(m), ncg_eq2(m)
    out = dict(have_t_in=HAVE_T, inputs=dict(m_LDC_kg=m, m_LDC_lb=m / 0.4536, LWL_m=LWL, BC_m=BC, beta=BETA, V_kn=V, sd_plate=SD_PLATE, sd_stiff=SD_STIFF),
               ncg=dict(eq1=n1, eq2=n2), cases=[])
    print('m_LDC %.0f kg (%.0f lb), LWL %.2f m, BC %.2f m, V %.1f kn' % (m, m / 0.4536, LWL, BC, V))
    print('nCG eq (1) %.1f, eq (2) %.1f' % (n1, n2))
    for cat, kdc in KDC.items():
        for nlabel, n in (('eq (2)', n2), ('limited to 3.0 (Table: cruising, planing)', 3.0)):
            for pname, p in PANELS.items():
                b, l = sorted((p['b'] * 25.4, p['l'] * 25.4))
                ar = l / b
                kar = kar_plate(b, l, m)
                kl = kL(p['x'] * IN, n)
                P, base = bottom_pressure(kdc, n, kar, kl)
                t = plate_t(b, P, ar)
                row = dict(category=cat, ncg_basis=nlabel, ncg=n, panel=pname, b_mm=b, l_mm=l, kAR=kar, kL=kl, P_kPa=P,
                           P_psi=P * 0.14504, t_req_mm=t, t_req_in=t / 25.4,
                           t_req_with_frames_12=None)
                out['cases'].append(row)
                print('%-22s nCG %-4.1f %-46s P %5.1f kPa (%4.1f psi)  t_req %.3f in' % (cat, n, pname, P, P * 0.14504, t / 25.4))
    # options: thicker bottom, or intermediate frames so every bay is 12 in
    opts = []
    for cat, kdc in KDC.items():
        for n in (n2, 3.0):
            b, l = sorted(((12 - 1.5) * 25.4, (16.5 - 2.76) * 25.4))
            kar = kar_plate(b, l, m)
            P, _ = bottom_pressure(kdc, n, kar, 1.0)
            opts.append(dict(category=cat, ncg=n, option='frames at 12 in throughout (add 60, 84, 108 in)', P_kPa=P,
                             t_req_in=plate_t(b, P, l / b) / 25.4))
    out['options'] = opts
    for o in opts:
        print('option: %-22s nCG %.1f %s -> P %.1f kPa, t_req %.3f in' % (o['category'], o['ncg'], o['option'], o['P_kPa'], o['t_req_in']))
    # stringers: bed stringer (0.080), spacing ~15.6 in, span 24 in between frames (fwd bays)
    st = []
    for cat, kdc in KDC.items():
        for n in (n2, 3.0):
            for t_hat, nm in ((0.080, 'bed hat 0.080'), (0.063, 'outer hat 0.063')):
                r = stringer_check(kdc, n, 15.6, 24.0, t_hat)
                r.update(category=cat, ncg=n, stringer=nm)
                st.append(r)
                print('stringer %-16s %-22s nCG %.1f: P %.1f kPa, SM req %.3f in3, have %.3f in3' % (nm, cat, n, r['P'], r['sm_req_in3'], r['sm_have_in3']))
    out['stringers'] = st
    json.dump(out, open(os.path.join(HERE, 'iso_bottom.json'), 'w'), indent=1)


if __name__ == '__main__':
    main()
