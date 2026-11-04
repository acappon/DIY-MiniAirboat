"""Structural hand calculations for the rev 7 airboat: drive tower (A-frames, braces, tie struts),
prop cage, driver side frame, engine mounts, prop shaft, steering pushrod, transom and trim tab.

Units: lb, in, psi. Model frame as the CAD: x forward from the transom, y to port, z up from the
hull bottom. Geometry comes from the rev 7 Fusion model (data/bodies.json survey and the cage_* /
drv_* parameters); loads are derived in LOADS below. Writes structure.json for the report."""
import json
import math
import os
import sys

import numpy as np

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from frame3d import Frame, round_tube, rect_tube, rigid, stress

HERE = os.path.dirname(os.path.abspath(__file__))
MM = 1 / 25.4
G_LB_IN3 = {'steel': 0.2836, 'al': 0.0975}

# --- materials (psi). Aluminium Design Manual (ADM 2020) minimums; welded = HAZ values for 6061-T6
# with 4043/5356 filler. Cage steel is unspecified ERW/DOM tube, so the A513 Type 1 minimum is used.
MAT = {
    'al6061': dict(name='6061-T6', E=10.1e6, Fy=35000, Fu=38000, Fyw=15000, Fuw=24000),
    'al5052': dict(name='5052-H32', E=10.2e6, Fy=23000, Fu=31000, Fyw=9500, Fuw=25000),
    'steel': dict(name='steel tube (A513 Type 1 min.)', E=29.0e6, Fy=32000, Fu=45000, Fyw=32000, Fuw=45000),
    'emt': dict(name='EMT (ANSI C80.3, assumed 30 ksi)', E=29.0e6, Fy=30000, Fu=42000, Fyw=30000, Fuw=42000),
}
REQ_MEMBER = 1.65   # ADM / AISC ASD safety factor on yield and buckling
REQ_CONN = 2.0      # bolts and pins against nominal capacity
GRIP_HURST = 450    # lb, hand-hold rating at the Hurst T-handle

# bolts: (tensile stress area, root area, Fu)
BOLT = {
    '3/8 SS (F593 CW)': (0.0775, 0.0678, 100000),
    '3/8 grade 8': (0.0775, 0.0678, 150000),
    '1/2 grade 8': (0.1419, 0.1257, 150000),
    '5/16 grade 2': (0.0524, 0.0454, 74000),
    '1/4 316 (F593 CW)': (0.0318, 0.0269, 100000),
}


def bolt_fos(kind, T, V, planes=1):
    """Factor of safety for a bolt with tension T and shear V (per bolt), threads in the shear plane."""
    At, Ar, Fu = BOLT[kind]
    Tc, Vc = Fu * At, 0.6 * Fu * Ar * planes
    u = math.hypot(T / Tc, V / Vc)
    return 1 / u if u else float('inf'), Tc, Vc


def column_Fcr(mat, kl_r):
    """Inelastic (Johnson) / elastic (Euler) column buckling stress, unwelded yield."""
    E, Fy = mat['E'], mat['Fy']
    lc = math.pi * math.sqrt(2 * E / Fy)
    return Fy * (1 - (kl_r / lc) ** 2 / 2) if kl_r < lc else math.pi ** 2 * E / kl_r ** 2


def check(name, demand, capacity, required, unit='psi', note=''):
    fos = capacity / demand if demand > 1e-9 else float('inf')
    return dict(item=name, demand=demand, capacity=capacity, fos=fos, req=required, ok=fos >= required, unit=unit, note=note)


# --- loads ---------------------------------------------------------------------------------------
RHO_AIR = 0.002377          # slug/ft3
RHO_W = 1.94                # slug/ft3, fresh water
PROP_D = 64.0               # in
THRUST = 430.0              # lb static, G13B (~100 hp): 340 lb at ~70 hp scaled by power^(2/3); confirm on a scale
THRUST_D = 1.25 * THRUST    # +25% for the estimate and throttle overshoot
TQ_PEAK = 83.0              # lb-ft, G13B peak torque (1.3 L DOHC 16-valve, typical figure; check the engine bought)
R30 = 30 * 8 / math.pi / 2 * MM   # 30T HTD 8M pitch radius, in
TE = TQ_PEAK * 12 / R30     # belt effective tension at peak torque
BELT = 1.5 * TE             # hub load: tight + slack side (Gates: Tt = 1.25 Te, Ts = 0.25 Te)
PROP_RPM = 6500 / 2.5       # G13B peak power at 6,500 rpm, 2.5:1 belt
OMEGA = PROP_RPM * 2 * math.pi / 60
I_PROP = 0.55               # slug-ft2: 3 carbon blades ~12 lb at k = 0.45 R, plus the HPL hub
YAW_RATE = 1.5              # rad/s, hard spin turn (about 86 deg/s)
GYRO = I_PROP * OMEGA * YAW_RATE * 12     # lb-in
NZ_BUMP = 3.0               # g, vertical at the stern running over chop (incl. gravity)
NY_TURN = 1.0               # g, lateral in a hard skidding turn


def slipstream():
    A = math.pi * (PROP_D / 24) ** 2
    vi0 = math.sqrt(THRUST / (2 * RHO_AIR * A))           # induced velocity at the disk, static
    V = 41 * 1.4667                                       # top speed, ft/s (G13B)
    T39 = THRUST * (1 - 0.085 * 4.1)                      # thrust falls ~8.5% per 10 mph (summary); ~41 mph top speed
    vi = (-V + math.sqrt(V ** 2 + 2 * T39 / (RHO_AIR * A))) / 2
    k = 1 + (9 / 32) / math.sqrt(1 + (9 / 32) ** 2)       # rudder leading edge 9 in aft of the disk
    v_static, v_speed = k * vi0, V + k * vi
    return dict(A_ft2=A, vi0=vi0, vi=vi, V=V, v_static=v_static, v_speed=v_speed, q=0.5 * RHO_AIR * max(v_static, v_speed) ** 2)


SS = slipstream()
RUDDER_S = 13.4 * 44 / 144          # ft2 each
RUDDER_CN = 1.2                     # low-aspect flat plate near stall
RUDDER_N = 1.25 * RUDDER_CN * SS['q'] * RUDDER_S   # +25% for gusts and swirl
RUDDER_DELTA = math.radians(33)
RUDDER_CP = 0.3 * 13.4 + 0.6        # in aft of the hinge axis
HINGE_M = RUDDER_N * RUDDER_CP      # per rudder
HORN_RY = 101.6 * MM                # horn arm across the pushrod line (y)
ROD_F = 2 * HINGE_M / HORN_RY       # pushrod force, both rudders through the tie bar
SOLIDITY = 1 - (1 - 0.041 / 0.5) ** 2                 # 1/2 in 19 ga hardware cloth
MESH_K = 1.3 * SOLIDITY + (SOLIDITY / (1 - SOLIDITY)) ** 2   # Idelchik, round-wire screen
SCREEN = math.pi * (36.2 / 12) ** 2            # ft2, each screen
# at top speed every screen sees the boat speed plus the prop's induced flow (front 0.8 vi, aft 1.21 vi)
MESH_FWD = 1.25 * MESH_K * 0.5 * RHO_AIR * (SS['V'] + 0.80 * SS['vi']) ** 2 * SCREEN
MESH_AFT = 1.25 * MESH_K * 0.5 * RHO_AIR * (SS['V'] + 1.21 * SS['vi']) ** 2 * SCREEN

LOADS = dict(thrust=THRUST, thrust_design=THRUST_D, torque_peak_lbft=TQ_PEAK, belt_Te=TE, belt_hub=BELT,
             prop_rpm=PROP_RPM, gyro_lbin=GYRO, yaw_rate=YAW_RATE, I_prop=I_PROP, nz=NZ_BUMP, ny=NY_TURN,
             v_slip_static=SS['v_static'], v_slip_speed=SS['v_speed'], q_rudder=SS['q'], rudder_N=RUDDER_N,
             hinge_M=HINGE_M, rod_F=ROD_F, mesh_aft=MESH_AFT, mesh_fwd=MESH_FWD, mesh_K=MESH_K)


def envelope(frame, cases, groups=None):
    """Run load cases; worst stress and axial force per member group."""
    out = {}
    res_all = {}
    for cname, loads in cases.items():
        r = frame.solve(loads)
        res_all[cname] = r
        for m in r['members']:
            g = m['e']['group']
            if groups and g not in groups:
                continue
            s, N = stress(m)
            d = out.setdefault(g, dict(sigma=0, case='', Nmin=0, Nmax=0, L=0, sec=m['e']['sec'], mat=m['e']['mat']))
            if s > d['sigma']:
                d['sigma'], d['case'] = s, cname
            d['Nmin'], d['Nmax'] = min(d['Nmin'], N), max(d['Nmax'], N)
    return out, res_all


# ==================================================================================================
# 1. Drive tower: two A-frames, the rigid yoke/peak-plate head, clevis braces and the tie struts
# ==================================================================================================
def tower(feet='fixed', xbar_f=None):
    f = Frame()
    al = MAT['al6061']
    leg, xbar, brace = rect_tube(2, 2, 0.125), rect_tube(1.5, 1.5, 0.125), rect_tube(1.5, 1.5, 0.125)
    zf, zt, zc, zj = 150 * MM, 1245 * MM, 711 * MM, 1130 * MM
    yb, yt = 376.6 * MM, 302.4 * MM

    def ly(z):
        return yb + (yt - yb) * (z - zf) / (zt - zf)
    zs = 1334 * MM
    head = f.node('H', (18.0, 0, zs))
    for tag, x in (('a', 343 * MM), ('f', 571.5 * MM)):
        for s, sg in (('P', 1), ('S', -1)):
            f.node('foot%s%s' % (tag, s), (x, sg * ly(zf), zf))
            f.node('xb%s%s' % (tag, s), (x, sg * ly(zc), zc))
            f.node('top%s%s' % (tag, s), (x, sg * ly(zj), zj))
            f.chain('A-frame legs (%s)' % ('aft' if tag == 'a' else 'fwd'),
                    ['foot%s%s' % (tag, s), 'xb%s%s' % (tag, s), 'top%s%s' % (tag, s)], leg, al, up=(1, 0, 0))
            f.member('head', 'top%s%s' % (tag, s), 'H', rigid(), al)
            if feet == 'fixed':
                f.support('foot%s%s' % (tag, s))
            else:
                f.support('foot%s%s' % (tag, s), 'xyz')
        # crossbar, with the tie-strut tabs on the forward one at y = +-6.5 in
        if tag == 'f':
            f.node('tsP', (x, 6.5, zc))
            f.node('tsS', (x, -6.5, zc))
            f.chain('crossbar (fwd, tie struts)', ['xbfS', 'tsS', 'tsP', 'xbfP'], xbar_f or xbar, al, up=(0, 0, 1))
        else:
            f.member('crossbar (aft)', 'xbaS', 'xbaP', xbar, al, up=(0, 0, 1))
    # braces: clevis tabs on the forward leg faces near the top, to feet on the bed plates at x 49.5
    for s, sg in (('P', 1), ('S', -1)):
        f.node('tab%s' % s, (619 * MM, sg * 310 * MM, zj))
        f.member('head', 'topf%s' % s, 'tab%s' % s, rigid(), al)
        f.node('bfoot%s' % s, (1257 * MM, sg * 376 * MM, 180 * MM))
        f.member('braces (clevis, axial)', 'tab%s' % s, 'bfoot%s' % s, brace, al, truss=True)
        f.support('bfoot%s' % s, 'xyz')
    # mass points on the head (lb): prop + flange, shaft, bearings, yoke + pulley + jack screw,
    # peak plates, radiator with coolant and its cradle, plus the legs/crossbars/braces lumped
    masses = {'prop': ((5.5, 0, zs), 21.8), 'shaft': ((13.3, 0, zs), 9.8), 'brg': ((18.0, 0, zs), 9.0),
              'yoke': ((18.0, 0, zs), 22.8), 'plates': ((18.0, 0, 52.0), 20.0), 'rad': ((17.6, 0, 70.0), 25.0),
              'frame': ((18.0, 0, 30.0), 28.0)}
    return f, masses


def tower_cases(masses):
    W = sum(w for _, w in masses.values())

    H = np.array((18.0, 0, 1334 * MM))

    def base(nz=1.0, ny=0.0, thrust=THRUST_D, belt=BELT, gyro=0.0):
        F, M = np.zeros(3), np.array([0, gyro, 0.0])
        for k, (p, w) in masses.items():
            fk = np.array([thrust if k == 'prop' else 0, -ny * w, -nz * w])
            F += fk
            M += np.cross(np.array(p) - H, fk)
        F[2] -= belt                     # belt pull on the prop pulley, directly under the head node
        L = {'H': (*F, *M)}
        L['tsP'] = (0, 0, belt / 2, 0, 0, 0)
        L['tsS'] = (0, 0, belt / 2, 0, 0, 0)
        return L
    return W, {
        'T1 full power, hard turn (gyro +)': base(ny=NY_TURN, gyro=GYRO),
        'T2 full power, hard turn (gyro -)': base(ny=-NY_TURN, gyro=-GYRO),
        'T3 full power over chop (3 g)': base(nz=NZ_BUMP),
        'T4 idle, 3 g, no thrust or belt pull': base(nz=NZ_BUMP, thrust=0, belt=0),
    }


def run_tower(xbar_f=None):
    groups = {}
    feet_react = {}
    brace_N = [0, 0]
    disp = 0
    for feet in ('fixed', 'pinned'):
        f, masses = tower(feet, xbar_f)
        W, cases = tower_cases(masses)
        env, res = envelope(f, cases)
        for g, d in env.items():
            if g == 'head':
                continue
            o = groups.setdefault(g, dict(d, feet=feet))
            if d['sigma'] > o['sigma']:
                groups[g] = dict(d, feet=feet)
            o = groups[g]
            o['Nmin'], o['Nmax'] = min(o['Nmin'], d['Nmin']), max(o['Nmax'], d['Nmax'])
        for cname, r in res.items():
            disp = max(disp, float(np.linalg.norm(r['disp']['H'][:3])))
            for nm, R in r['react'].items():
                c = feet_react.setdefault(nm.rstrip('PS'), dict(uplift=-1e9, shear=0, My=0, Mx=0, down=0))
                c['uplift'], c['down'] = max(c['uplift'], -R[2]), max(c['down'], R[2])
                c['shear'] = max(c['shear'], math.hypot(R[0], R[1]))
                c['My'], c['Mx'] = max(c['My'], abs(R[4])), max(c['Mx'], abs(R[3]))
    checks = []
    al = MAT['al6061']
    for g, d in groups.items():
        s = d['sec']
        if 'braces' in g:
            N = min(d['Nmin'], 0)
            L = math.dist((619 * MM, 310 * MM, 1130 * MM), (1257 * MM, 376 * MM, 180 * MM))
            r = math.sqrt(s['Iy'] / s['A'])
            Fcr = column_Fcr(al, L / r)
            checks.append(check('Brace buckling (pinned, L %.1f in, KL/r %.0f)' % (L, L / r), -N, Fcr * s['A'], REQ_MEMBER, 'lb'))
            checks.append(check('Brace at the welded tongues (HAZ)', -N / s['A'], al['Fyw'], REQ_MEMBER))
            brace_N = [d['Nmin'], d['Nmax']]
            continue
        checks.append(check('%s, worst end (welded, %s feet)' % (g, d['feet']), d['sigma'], al['Fyw'], REQ_MEMBER,
                            note=d['case']))
        if 'legs' in g and d['Nmin'] < 0:
            L = (1130 - 150) * MM
            r = math.sqrt(s['Iy'] / s['A'])
            checks.append(check('%s buckling (K 1, L %.1f in)' % (g, L), -d['Nmin'], column_Fcr(al, L / r) * s['A'], REQ_MEMBER, 'lb'))
    # A-frame feet: 2 x 3/8 SS at +-2.25 in; the foot plate is 3 in wide (Mx by prying on 1.5 in)
    for key, nb, arm in (('foota', 2, 4.5), ('footf', 2, 4.5)):
        R = feet_react[key]
        T = max(R['uplift'], 0) / nb + R['My'] / arm + R['Mx'] / (nb * 1.5)
        V = R['shear'] / nb
        fos, Tc, Vc = bolt_fos('3/8 SS (F593 CW)', T, V)
        checks.append(dict(item='A-frame %s foot bolts (2 x 3/8 SS): T %.0f lb, V %.0f lb per bolt' % ('aft' if key == 'foota' else 'fwd', T, V),
                           demand=T, capacity=Tc, fos=fos, req=REQ_CONN, ok=fos >= REQ_CONN, unit='lb', note=''))
    Rb = feet_react['bfoot']
    T = max(Rb['uplift'], 0) / 2
    V = Rb['shear'] / 2
    fos, Tc, Vc = bolt_fos('3/8 SS (F593 CW)', T, V)
    checks.append(dict(item='Brace foot bolts (2 x 3/8 SS): T %.0f lb, V %.0f lb per bolt' % (T, V), demand=V, capacity=Vc,
                       fos=fos, req=REQ_CONN, ok=fos >= REQ_CONN, unit='lb', note=''))
    # clevis: 3/8 grade 8 in double shear; 1/4 tabs and tongue bearing in the HAZ (ADM: 2 Ftu d t)
    Nb = max(abs(brace_N[0]), abs(brace_N[1]))
    fos, _, Vc = bolt_fos('3/8 grade 8', 0, Nb, planes=2)
    checks.append(check('Brace clevis bolt, 3/8 grade 8, double shear', Nb, Vc, REQ_CONN, 'lb'))
    checks.append(check('Brace tongue bearing, 1/4 in 6061 at the weld (HAZ)', Nb, 2 * al['Fuw'] * 0.375 * 0.25, REQ_CONN, 'lb'))
    # tie struts: 1 x 1 x 1/8, 4.7 in long, in compression; 5/16 bolt double shear through the tube
    Pts = BELT / 2
    s = rect_tube(1, 1, 0.125)
    checks.append(check('Tie strut, 1 x 1 x 1/8 (compression, short)', Pts / s['A'], al['Fyw'], REQ_MEMBER))
    fos, _, Vc = bolt_fos('5/16 grade 2', 0, Pts, planes=2)
    checks.append(check('Tie strut bolt, 5/16 (grade 2 assumed), double shear', Pts, Vc, REQ_CONN, 'lb'))
    checks.append(check('Tie strut tab bearing, 1/4 in 6061 at the weld (HAZ)', Pts, 2 * al['Fuw'] * 0.3125 * 0.25, REQ_CONN, 'lb'))
    # yoke-to-peak-plate bolts: per plate, one bearing's share in single shear over 3 x 1/2 grade 8
    Vplate = math.hypot(THRUST_D, BELT / 2 + GYRO / (163 * MM) + NZ_BUMP * 31 / 2)
    fos, _, Vc = bolt_fos('1/2 grade 8', 0, Vplate / 3)
    checks.append(check('Yoke pivot/slot bolts, 3 x 1/2 grade 8 per peak plate', Vplate / 3, Vc, REQ_CONN, 'lb'))
    # fatigue screen: belt load ripple +-25% (4-cylinder firing pulses through the flex coupling)
    f, masses = tower('fixed', xbar_f)
    unit = {'H': (0, 0, -1, 0, 0, 0), 'tsP': (0, 0, 0.5, 0, 0, 0), 'tsS': (0, 0, 0.5, 0, 0, 0)}
    env, _ = envelope(f, {'unit belt': unit})
    fat = {g: d['sigma'] * 0.5 * BELT for g, d in env.items() if g != 'head' and 'braces' not in g}
    return dict(checks=checks, groups={g: dict(sigma=d['sigma'], case=d['case'], feet=d['feet'], Nmin=d['Nmin'], Nmax=d['Nmax'])
                                       for g, d in groups.items()},
                feet=feet_react, brace_N=brace_N, head_disp=disp, fatigue_range=fat, head_weight=sum(w for _, w in masses.values()))


# ==================================================================================================
# 2. Prop cage: two 72.4 in hoops, 8 EMT longitudinals, face bars, rudder rails, legs on 4 pads
# ==================================================================================================
CR, CZ, XA, XF = 36.2, 52.5, -2.0, 11.6


def cage():
    f = Frame()
    st, emt = MAT['steel'], MAT['emt']
    hoop, t065 = round_tube(1.0, 0.049), round_tube(1.0, 0.065)
    e34, e12 = round_tube(0.922, 0.049), round_tube(0.706, 0.042)
    angs = set(range(0, 360, 10))
    special = {}
    for k in range(8):
        special['L%d' % k] = 22.5 + 45 * k
    for nm, z in (('r1', 28.0), ('r2', 77.5)):
        a = math.degrees(math.asin((z - CZ) / CR))
        special[nm + 'P'], special[nm + 'S'] = a % 360, (180 - a) % 360
    for nm, y in (('fb', 15.0), ('ab', 21.0), ('lb', 12.0)):
        a = math.degrees(math.acos(y / CR))
        special[nm + 'PT'], special[nm + 'ST'] = a, 180 - a
        special[nm + 'PB'], special[nm + 'SB'] = 360 - a, 180 + a
    special['top'], special['bot'], special['side'] = 90.0, 270.0, 0.0
    for a in special.values():
        angs = {x for x in angs if abs((x - a + 180) % 360 - 180) > 3}
    allang = sorted(set(round(a, 3) for a in list(angs) + list(special.values())))
    inv = {round(v, 3): k for k, v in special.items()}

    def pt(x, a):
        r = math.radians(a)
        return (x, CR * math.cos(r), CZ + CR * math.sin(r))
    for hx, h in ((XA, 'A'), (XF, 'F')):
        names = []
        for a in allang:
            nm = '%s%s' % (h, inv.get(a, '%.1f' % a))
            f.node(nm, pt(hx, a))
            names.append(nm)
        f.chain('hoops 1 x .049', names + names[:1], hoop, st, up=(1, 0, 0))
    for k in range(8):
        f.member('longitudinals 3/4 EMT', 'AL%d' % k, 'FL%d' % k, e34, emt)
    # front bars (1/2 EMT) top to bottom at y +-15 on the forward hoop
    for s in 'PS':
        f.member('front bars 1/2 EMT', 'Ffb%sT' % s, 'Ffb%sB' % s, e12, emt)
    # rudder rails (1 x .065) across the aft hoop at z 28 and 77.5; aft bars (1/2 EMT) at y +-21
    for nm, z in (('r1', 28.0), ('r2', 77.5)):
        chain = ['A%sS' % nm]
        for y in (-21.0, -13.0, 13.0, 21.0):
            n = '%s_%g' % (nm, y)
            f.node(n, (XA, y, z))
            chain.append(n)
        chain.append('A%sP' % nm)
        f.chain('rudder rails 1 x .065', chain, t065, st, up=(1, 0, 0))
    for s, y in (('P', 21.0), ('S', -21.0)):
        f.chain('aft bars 1/2 EMT', ['Aab%sB' % s, 'r1_%g' % y, 'r2_%g' % y, 'Aab%sT' % s], e12, emt)
    # legs from the lower longitudinal node (22.5 deg below horizontal) to the pads; base rails
    for s, sg, lk in (('P', 1, 'L7'), ('S', -1, 'L4')):
        f.node('padA' + s, (77.47 * MM, sg * 33.2, 474 * MM))
        f.node('padF' + s, (304.8 * MM, sg * 33.2, 443 * MM))
        f.member('legs 1 x .065', 'A' + lk, 'padA' + s, t065, st)
        f.member('legs 1 x .065', 'F' + lk, 'padF' + s, t065, st)
        f.member('base rails 1 x .065', 'padA' + s, 'padF' + s, t065, st)
        f.support('padA' + s)
        f.support('padF' + s)
    return f


def cage_cases(f, new_steer=False):
    # self weight of the tubes, lumped to nodes
    W = {}
    total = 0
    for e in f.els:
        p, q = f.nodes[e['a']], f.nodes[e['b']]
        w = e['sec']['A'] * np.linalg.norm(q - p) * G_LB_IN3['steel']
        total += w
        for i in (e['a'], e['b']):
            W[i] = W.get(i, 0) + w / 2
    hoopA = [i for nm, i in f.names.items() if nm.startswith('A')]
    hoopF = [i for nm, i in f.names.items() if nm.startswith('F')]
    for idx, w in ((hoopA, 6.3 + 2.4), (hoopF, 6.3 + 2.4)):   # screens + half the drum mesh
        for i in idx:
            W[i] = W.get(i, 0) + w / len(idx)
    for nm in ('r1_13', 'r1_-13'):                              # rudders hang on the lower brackets
        W[f.names[nm]] = W.get(f.names[nm], 0) + 9.0
    for nm in ('FlbPT', 'FlbST'):                               # light bar + searchlight on the front hoop
        W[f.names[nm]] = W.get(f.names[nm], 0) + 10.0
    total_all = sum(W.values())
    inv = {i: nm for nm, i in f.names.items()}

    def grav(nz, ny=0.0):
        return {inv[i]: (0, -ny * w, -nz * w, 0, 0, 0) for i, w in W.items()}

    def add(L, nm, v):
        L[nm] = tuple(np.add(L.get(nm, (0,) * 6), v))

    def rudders(L, sgn):
        Fy = sgn * RUDDER_N * math.cos(RUDDER_DELTA) / 2
        Fx = -RUDDER_N * math.sin(RUDDER_DELTA) / 2
        for nm in ('r1_13', 'r1_-13', 'r2_13', 'r2_-13'):
            add(L, nm, (Fx, Fy, 0, 0, 0, 0))
        if new_steer:      # drive link pulls the port horn; bellcrank on the hoop at the lower rail end
            link = 2 * HINGE_M / 3.5
            add(L, 'r1_13', (0, -sgn * link, 0, 0, 0, 0))
            add(L, 'Ar1P', (sgn * ROD_F, sgn * link, 0, 0, 0, 0))
        else:
            add(L, 'r1_13', (sgn * ROD_F, 0, 0, 0, 0, 0))       # old pushrod reaction at the port horn
        for idx, Ftot in ((hoopA, MESH_AFT), (hoopF, MESH_FWD)):
            for i in idx:
                add(L, inv[i], (-Ftot / len(idx), 0, 0, 0, 0, 0))
        return L
    cases = {
        'C1 full rudder, over chop (3 g)': rudders(grav(NZ_BUMP), 1),
        'C2 opposite rudder, over chop (3 g)': rudders(grav(NZ_BUMP), -1),
        'C3 full rudder, hard turn (1 g lateral)': rudders(grav(1.0, NY_TURN), 1),
    }
    m4 = grav(1.0)
    add(m4, 'Atop', (0, 0, -250, 0, 0, 0))
    cases['C4 misuse: 250 lb on top of the aft hoop'] = m4
    m5 = grav(1.0)
    add(m5, 'Aside', (0, -150, 0, 0, 0, 0))
    cases['C5 misuse: 150 lb push on the side of the aft hoop'] = m5
    m6 = grav(1.0)
    add(m6, 'Ftop', (0, 0, -250, 0, 0, 0))
    cases['C6 misuse: 250 lb on top of the forward hoop'] = m6
    return cases, total_all


REQ_MISUSE = 1.0    # misuse loads (someone standing on or shoving the cage): no permanent bend
DOM = dict(MAT['steel'], name='DOM 1020 tube, yield 50 ksi or better (proposed)', Fy=50000, Fyw=50000)


def cage_fixed():
    """Proposed fixes: DOM tube throughout, hoops 1 x .083 (was .049; .065 was enough for the G13), 1 x .065 longitudinals and aft
    bars (were 3/4 and 1/2 EMT), and a diagonal in each side bay (aft pad to the forward leg top).
    Forward kickers from the rudder rails were tried and dropped: they cross the prop disk."""
    f = cage()
    t065 = round_tube(1.0, 0.065)
    for e in f.els:
        g = e['group']
        if e['mat'] is MAT['steel']:
            e['mat'] = DOM
        if g.startswith('hoops'):
            e.update(sec=round_tube(1.0, 0.083), group='hoops 1 x .083 DOM (was .049; .083 for the G13B)')
        if g.startswith('longitudinals'):
            e.update(sec=t065, mat=DOM, group='longitudinals 1 x .065 DOM (was 3/4 EMT)')
        if g.startswith('aft bars'):
            e.update(sec=t065, mat=DOM, group='aft bars 1 x .065 DOM (was 1/2 EMT)')
    for s, lk in (('P', 'L7'), ('S', 'L4')):
        f.member('side-bay diagonals 1 x .065 (new)', 'padA' + s, 'F' + lk, t065, DOM)
    return f


def run_cage():
    out = {}
    for tag, f in (('as drawn', cage()), ('with fixes', cage_fixed())):
        cases, weight = cage_cases(f, new_steer=(tag == 'with fixes'))
        oper = {k: v for k, v in cases.items() if k[:2] in ('C1', 'C2', 'C3')}
        mis = {k: v for k, v in cases.items() if k not in oper}
        env, _ = envelope(f, oper)
        envm, res = envelope(f, mis)
        checks = []
        for g, d in env.items():
            if g.startswith('base rails'):
                continue           # runs between two fixed pads: carries nothing in this model
            checks.append(check('%s, operating (%s)' % (g, d['case'].split(' ')[0]), d['sigma'], d['mat']['Fy'], REQ_MEMBER, note=d['case']))
        for g, d in envm.items():
            if g.startswith('base rails'):
                continue
            checks.append(check('%s, misuse (%s)' % (g, d['case'].split(' ')[0]), d['sigma'], d['mat']['Fy'], REQ_MISUSE, note=d['case']))
        s = round_tube(1.0, 0.065)
        Lleg = math.dist((XA, 33.2, CZ - CR * math.sin(math.radians(22.5))), (77.47 * MM, 33.2, 474 * MM))
        Nleg = -min(env['legs 1 x .065']['Nmin'], envm['legs 1 x .065']['Nmin'])
        checks.append(check('Cage legs buckling (K 1, L %.1f in)' % Lleg, Nleg,
                            column_Fcr(MAT['steel'], Lleg / math.sqrt(s['Iy'] / s['A'])) * s['A'], REQ_MEMBER, 'lb'))
        # inward movement of the drum at the prop plane under the misuse loads (tip clearance 4.65 in)
        defl = {}
        for cname, (a, b, d) in (('C4 misuse: 250 lb on top of the aft hoop', ('Atop', 'Ftop', (0, 0, -1))),
                                 ('C6 misuse: 250 lb on top of the forward hoop', ('Atop', 'Ftop', (0, 0, -1))),
                                 ('C5 misuse: 150 lb push on the side of the aft hoop', ('Aside', 'Fside', (0, -1, 0)))):
            r = res[cname]
            ua, ub = r['disp'][a][:3], r['disp'][b][:3]
            u = ua + (ub - ua) * (5.0 - XA) / (XF - XA)
            defl[cname] = float(np.dot(u, d))
        # pad reactions -> 4 x 3/8 SS bolts on a 2.5 in square
        pads = {}
        _, resall = envelope(f, cases)
        for cname, r in resall.items():
            for nm, R in r['react'].items():
                T = max(-R[2], 0) / 4 + math.hypot(R[3], R[4]) / (2 * 2.5)
                V = math.hypot(R[0], R[1]) / 4 + abs(R[5]) / (4 * 1.77)
                p = pads.setdefault(nm[:4], dict(T=0, V=0, F=0))
                p['T'], p['V'] = max(p['T'], T), max(p['V'], V)
                p['F'] = max(p['F'], float(math.hypot(*R[:3])))
        for nm, p in pads.items():
            fos, Tc, Vc = bolt_fos('3/8 SS (F593 CW)', p['T'], p['V'])
            checks.append(dict(item='Cage %s pad bolts (4 x 3/8 SS): T %.0f, V %.0f lb per bolt' % ('aft' if nm == 'padA' else 'fwd', p['T'], p['V']),
                               demand=p['T'], capacity=Tc, fos=fos, req=REQ_CONN, ok=fos >= REQ_CONN, unit='lb', note=''))
        out[tag] = dict(checks=checks, deflection=defl, pads=pads, weight=weight)
    out['checks'] = out['as drawn']['checks']
    out['checks_fixed'] = out['with fixes']['checks']
    return out


# ==================================================================================================
# 3. Driver side frame (starboard): posts, top rail, cross arms, bowed knee braces, inner rail
# ==================================================================================================
def side_frame():
    f = Frame()
    al = MAT['al6061']
    sq2, r23, br = rect_tube(2, 2, 0.125), rect_tube(2, 3, 0.125), round_tube(1.5, 0.125)
    deck, top, zarm = 3.03, 26.0, 25.0
    xs = (97.0, 108.0)
    for i, x in enumerate(xs):
        p = 'p%d' % i
        zs = [deck, deck + 3, deck + 4 if i == 0 else None, deck + 7.5, deck + 10, 24.0 if i == 1 else None, zarm]
        names = []
        for z in sorted(z for z in zs if z is not None):
            f.node('%s_%.2f' % (p, z), (x, -31.0, z))
            names.append('%s_%.2f' % (p, z))
        f.chain('posts 2 x 2', names, sq2, al, up=(1, 0, 0))
        f.support('%s_%.2f' % (p, deck), 'xyz')
        f.support('%s_%.2f' % (p, deck + 3), 'xyz')
        f.support('%s_%.2f' % (p, deck + 7.5), 'xyz')
        # cross arm to the inner rail, with a node over the brace top
        f.node('a%d_b' % i, (x, -15.5, zarm))
        f.node('a%d_r' % i, (x, -12.0, zarm))
        f.chain('cross arms 2 x 2', ['%s_%.2f' % (p, zarm), 'a%d_b' % i, 'a%d_r' % i], sq2, al, up=(0, 0, 1))
        # bowed knee brace, 8 chords on the 3-point arc
        A, B = np.array([-31.0, deck + 10]), np.array([-15.5, 24.0])
        c = np.linalg.norm(B - A)
        n = np.array([(A[1] - B[1]) / c, (B[0] - A[0]) / c])
        if n[0] > 0:
            n = -n                       # outboard is -y on the starboard side
        M = (A + B) / 2 + 2.5 * n
        # circle through A, M, B
        ax, ay, bx, by, cx, cy = *A, *M, *B
        d = 2 * (ax * (by - cy) + bx * (cy - ay) + cx * (ay - by))
        ux = ((ax ** 2 + ay ** 2) * (by - cy) + (bx ** 2 + by ** 2) * (cy - ay) + (cx ** 2 + cy ** 2) * (ay - by)) / d
        uy = ((ax ** 2 + ay ** 2) * (cx - bx) + (bx ** 2 + by ** 2) * (ax - cx) + (cx ** 2 + cy ** 2) * (bx - ax)) / d
        R = math.hypot(ax - ux, ay - uy)
        t0, t1, tm = (math.atan2(p_[1] - uy, p_[0] - ux) for p_ in (A, B, M))

        def between(a, b, m):
            span = (b - a) % (2 * math.pi)
            return span if (m - a) % (2 * math.pi) < span else span - 2 * math.pi
        span = between(t0, t1, tm)
        bn = []
        for k in range(9):
            t = t0 + span * k / 8
            nm = '%s_%.2f' % (p, deck + 10) if k == 0 else 'kb%d_%d' % (i, k)
            f.node(nm, (x, ux + R * math.cos(t), uy + R * math.sin(t)))
            bn.append(nm)
        f.chain('knee braces 1.5 x .125 (bowed)', bn, br, al, up=(1, 0, 0))
        f.member('knee braces 1.5 x .125 (bowed)', bn[-1], 'a%d_b' % i, rigid(), al)
        f.R_brace, f.L_brace = R, abs(span) * R
    f.chain('top rail 2 x 2', ['p0_%.2f' % zarm, 'p1_%.2f' % zarm], sq2, al, up=(0, 0, 1))
    f.chain('side diagonal 1.5 x .125', ['p0_%.2f' % (deck + 4), 'p1_24.00'], br, al)
    # inner rail (2 x 3), aft end at the aft arm's aft face (hinge bracket), grip at x 100.8
    for nm, x in (('ir96', 96.0), ('irgrip', 100.8), ('irmid', 102.5), ('ir109', 109.0)):
        f.node(nm, (x, -12.0, zarm))
    f.chain('inner rail 2 x 3', ['ir96', 'a0_r', 'irgrip', 'irmid', 'a1_r', 'ir109'], r23, al, up=(0, 0, 1))
    f.node('pin', (95.2, -12.0, 27.85))
    f.member('armrest U-bracket', 'ir96', 'pin', rigid(), al)
    return f


def panel_diagonal(f, group, a, b, t, w, h, eff=0.5):
    """Riveted skin as an equivalent diagonal: same in-plane shear stiffness (G t w / h) as the panel
    w wide (along the load) and h deep, derated to eff for rivet slip at the clip slots."""
    d = math.hypot(w, h)
    G, E = 3.8e6, MAT['al6061']['E']
    A = eff * G * t * d ** 3 / (E * w * h)
    f.member(group, a, b, dict(kind='round', A=A, Iy=1e-6, Iz=1e-6, J=1e-6, Sy=1e9, Sz=1e9, c=1), MAT['al6061'], truss=True)
    return A


def run_side_frame():
    grip_h = 29.5 - 25.75      # knob centre above the weld at the rail top
    cases = {
        'S1 grip pulled inboard 600 lb': {'irgrip': (0, 600, 0, -600 * grip_h, 0, 0)},
        'S2 grip pushed outboard 600 lb': {'irgrip': (0, -600, 0, 600 * grip_h, 0, 0)},
        'S3 grip pulled aft 600 lb': {'irgrip': (-600, 0, 0, 0, -600 * grip_h, 0)},
        'S4 knee on the aft brace 400 lb outboard': {'kb0_4': (0, -400, 0, 0, 0, 0)},
        'S5 knee on the forward brace 400 lb outboard': {'kb1_4': (0, -400, 0, 0, 0, 0)},
        'S6 leaning on the armrest end 250 lb': {'pin': (0, 0, -250, 0, -250 * (95.2 - 83.1), 0)},
        'S7 standing on the console 300 lb': {'irmid': (0, 0, -300, 0, 0, 0)},
    }
    al = MAT['al6061']
    out = {}
    for tag in ('frame only', 'with skins', 'with fixes'):
        f = side_frame()
        if tag == 'with skins':
            out['A_top'] = panel_diagonal(f, 'console top 1/8 (as shear panel)', 'p0_25.00', 'a1_r', 0.125, 11, 19)
            out['A_skin'] = panel_diagonal(f, 'outboard skin 0.063 (as shear panel)', 'p0_7.03', 'p1_25.00', 0.063, 11, 18)
        if tag == 'with fixes':          # 3/16 walls on the posts and knee braces; skins not counted
            for e in f.els:
                if e['group'].startswith('posts'):
                    e.update(sec=rect_tube(2, 2, 0.1875), group='posts 2 x 2 x 3/16')
                if e['group'].startswith('knee') and e['sec']['kind'] == 'round':
                    e.update(sec=round_tube(1.5, 0.1875), group='knee braces 1.5 x .188 (bowed)')
        env, res = envelope(f, cases)
        checks = []
        for g, d in env.items():
            if g == 'armrest U-bracket' or 'shear panel' in g:
                continue
            checks.append(check('%s (%s)' % (g, d['case'].split(' ')[0]), d['sigma'], al['Fyw'], REQ_MEMBER, note=d['case']))
        out[tag] = checks
        if tag == 'frame only':
            kn = res['S4 knee on the aft brace 400 lb outboard']['disp']['kb0_4']
            out['knee_defl'] = float(np.linalg.norm(kn[:3]))
            out['brace_R'], out['brace_L'] = f.R_brace, f.L_brace
    extra = []
    # grip stick: 316 rod, knob 3.75 in above the rail top; annealed 316 yield 30 ksi (it is welded)
    S = math.pi / 32
    extra.append(check('Grip stick, 1 in 316 rod welded into the rail, 600 lb at the knob', 600 * grip_h / S, 30000, REQ_MEMBER,
                       note='bending at the base; ultimate 75 ksi'))
    h2 = 29.5 - 26.375         # built: stick welded to a 1/4 plate on the console top
    S2 = math.pi * 1.125 ** 3 / 32
    grip_fixed = [check('Grip stick, 1-1/8 in 316 on the bolted base plate (built), 600 lb at the knob', 600 * h2 / S2, 30000, REQ_MEMBER,
                        note='bending at the base plate weld; ultimate 75 ksi')]
    # 4 x 1/4 316 bolts on a 2.2 x 1.2 in pattern: 600 lb x 3.1 in overturning, worst about the short side
    Tb = 600 * (h2 + 0.25) / (2 * 1.2)
    fos, Tc, Vc = bolt_fos('1/4 316 (F593 CW)', Tb, 600 / 4)
    grip_fixed.append(dict(item='Grip base-plate bolts, 4 x 1/4 316: T %.0f lb, V %.0f lb per bolt' % (Tb, 150), demand=Tb, capacity=Tc,
                           fos=fos, req=REQ_CONN, ok=fos >= REQ_CONN, unit='lb', note='through the console top and both rail walls, crush tubes'))
    # Hurst 1530040 T-handle (cast aluminium) straight on an M16 x 1.5 stud turned on the stick end (no adapter),
    # 316 nylon-insert lock nut under the neck, 1/4 316 cross pin 0.4 in above the shoulder. The stick is
    # cold-drawn 316 (A276 Cond B, 65 ksi for 1-1/8 bar). A shift knob, so the hand-hold is rated GRIP_HURST lb
    # at the handle (the stick, plate and frame keep 600 lb). Bar centre 0.75 in above the shoulder; hand up to
    # 1.5 in off the bar centre. The lock nut's friction is not counted.
    P_t, a_t, e_t = GRIP_HURST, 0.75, 1.5
    D_stud, d3 = 16 / 25.4, 0.5575            # M16 x 1.5: major and minor diameter, in
    d_pin, z_pin = 0.25, 0.4
    Fy_b = 65000
    S_stud = math.pi * d3 ** 3 / 32
    grip_fixed.append(check('Hurst T-handle stud, M16 x 1.5 on the 316 stick, %d lb at the bar' % P_t, P_t * a_t / S_stud, Fy_b,
                            REQ_MEMBER, note='bending at the shoulder; A276 Cond B 316'))
    S_net = S_stud - d_pin * d3 ** 2 / 6       # minus the cross-pin hole
    grip_fixed.append(check('Hurst T-handle stud at the 1/4 pin hole', P_t * (a_t - z_pin) / S_net, Fy_b, REQ_MEMBER,
                            note='net section, %.1f in above the shoulder' % z_pin))
    A_pin = math.pi * d_pin ** 2 / 4
    F_pin = P_t * e_t / D_stud                 # shear per plane = twist / stud diameter
    grip_fixed.append(check('Hurst T-handle cross pin, 1/4 316, %d lb pull at the bar end' % P_t, F_pin, 0.6 * 75000 * A_pin,
                            REQ_CONN, 'lb', note='twist about the stick'))
    grip_fixed.append(check('Hurst T-handle pin bearing in the cast boss', F_pin, 1.5 * 30000 * d_pin * 0.25, REQ_CONN, 'lb',
                            note='0.25 in wall assumed (measure the neck); cast aluminium Fu about 30 ksi'))
    Fstop = 250 * 12.1 / 4.85
    Fpin = math.hypot(250, Fstop)
    extra.append(check('Armrest hinge pin, 1/2 316, double shear', Fpin, 2 * 0.6 * 75000 * 0.196, REQ_CONN, 'lb'))
    extra.append(check('Armrest lugs, 2 x 1/4 6061 bearing (HAZ)', Fpin, 2 * 2 * al['Fuw'] * 0.5 * 0.25, REQ_CONN, 'lb'))
    out.update(checks=out['frame only'] + extra, checks_fixed=out['with fixes'] + grip_fixed, stop=Fstop, pin=Fpin)
    return out


# ==================================================================================================
# 4. Engine mounts, prop shaft and bearings
# ==================================================================================================
def run_engine():
    We = 179 + 15 + 7 + 11 + 1.3 + 4.4 + 5.7   # dressed G13 + ~15 lb for the G13B twin-cam head, oil, adapter, coupling, jackshaft, 30T pulley
    xcg, zcg = 35.5, 17.0
    xa, xf, ym, zm = 31.0, 42.0, 212 * MM, 343 * MM
    track = 2 * ym
    fa = (xf - xcg) / (xf - xa)
    out = {}
    vert = NZ_BUMP * We
    out['vertical, 3 g'] = vert * fa / 2
    lat = NY_TURN * We
    out['lateral roll couple, 1 g'] = lat * (zcg - zm) / track / 2
    tq = 2 * TQ_PEAK * 12
    out['torque reaction, 2 x peak'] = tq / track / 2
    radial = sum(out.values())
    shear_lat = lat / 4
    Fr = math.hypot(radial, shear_lat)
    checks = []
    fos, _, Vc = bolt_fos('1/2 grade 8', 0, Fr, planes=2)
    checks.append(check('Engine mount through-bolt, 1/2 grade 8 (aft mount, worst)', Fr, Vc, REQ_CONN, 'lb'))
    checks.append(check('Poly bushing bearing on the bolt (1/2 x 3 in), 90A urethane ~1,000 psi', Fr / 1.5, 1000, REQ_CONN))
    # what happens without the tie struts: belt pull lands on the jackshaft, aft of both mounts
    xj = 18.0
    up_aft = (BELT * (xf - xj) - We * (xf - xcg)) / (xf - xa)
    out_nostrut = dict(aft_pair_uplift=up_aft, per_mount=up_aft / 2)
    # prop shaft, 1-1/2 in 4140, keyed: bending at the aft bearing from the overhung prop and the
    # gyroscopic moment (rotating, so fully reversed), torsion from 2.5 x peak torque
    d = 1.5
    S, Zp = math.pi * d ** 3 / 32, math.pi * d ** 3 / 16
    Mb = NZ_BUMP * 21.8 * (14.78 - 5.5) + GYRO
    Tq = TQ_PEAK * 2.5 * 12
    sa, tm = Mb / S, Tq / Zp
    Fu = 95000                       # 4140 annealed; pre-hardened is ~140 ksi
    Se = 0.5 * Fu * 0.8 * 0.85 / 2.0   # surface, size, keyway Kf 2
    fos_shaft = 1 / (sa / Se + math.sqrt(3) * tm / Fu)
    checks.append(dict(item='Prop shaft at the aft bearing, 4140 annealed (rotating bending %.1f ksi, torsion %.1f ksi)' % (sa / 1000, tm / 1000),
                       demand=sa, capacity=Se, fos=fos_shaft, req=2.0, ok=fos_shaft >= 2.0, unit='psi',
                       note='Goodman, keyway Kf 2'))
    Fu2 = 140000
    Se2 = 0.5 * Fu2 * 0.8 * 0.85 / 2.0
    fos2 = 1 / (sa / Se2 + math.sqrt(3) * tm / Fu2)
    fixed = [dict(item='Prop shaft at the aft bearing, 4140 pre-hardened (28-32 HRC)', demand=sa, capacity=Se2, fos=fos2,
                  req=2.0, ok=fos2 >= 2.0, unit='psi', note='Goodman, keyway Kf 2')]
    # bearings: UCF208-24 class, C ~ 6,500 lb; typical cruise load ~ half the belt pull + prop weight share
    P = BELT / 2 + 21.8 * (21.2 - 5.5) / (21.2 - 14.78)
    L10h = (6500 / P) ** 3 * 1e6 / (PROP_RPM * 60)
    return dict(We=We, mount_parts=out, mount_radial=Fr, no_struts=out_nostrut, checks=checks, checks_fixed=fixed,
                shaft=dict(Mb=Mb, sa=sa, tm=tm, Se=Se, fos=fos_shaft), bearing=dict(P=P, L10h=L10h))


# ==================================================================================================
# 5. Steering pushrod (5/8 x .065 steel) as drawn: rod ends at the horn and the lever, two bends
# ==================================================================================================
def run_pushrod():
    P = np.array([(-129.54, 450.85, 762.63), (-114.3, 787.4, 762.63), (342.9, 787.4, 762.0),
                  (2438.408, 345.523, 444.394)]) * MM
    line = P[-1] - P[0]
    u = line / np.linalg.norm(line)
    off = max(np.linalg.norm((p - P[0]) - np.dot(p - P[0], u) * u) for p in P[1:-1])
    s = round_tube(0.625, 0.065)
    M = ROD_F * off
    sig = M / s['Sy'] + ROD_F / s['A']
    F_yield = MAT['steel']['Fy'] / (off / s['Sy'] + 1 / s['A'])
    # if straightened (or with a bellcrank at the outboard corner), the long leg is a column
    Lstraight = np.linalg.norm(P[-1] - P[1])
    Pcr = math.pi ** 2 * MAT['steel']['E'] * s['Iy'] / Lstraight ** 2
    s1 = round_tube(1.0, 0.065)
    Pcr1 = math.pi ** 2 * MAT['steel']['E'] * s1['Iy'] / Lstraight ** 2
    checks = [check('Steering pushrod as drawn: bending at the dog-leg (%.1f in off the pin line)' % off, sig,
                    MAT['steel']['Fy'], REQ_MEMBER),
              check('Pushrod straight long leg, 5/8 x .065, buckling (L %.0f in)' % Lstraight, ROD_F, Pcr, 3.0, 'lb',
                    note='3.0 for a slender steering link'),
              check('Pushrod long leg as 1 x .065, buckling', ROD_F, Pcr1, 3.0, 'lb')]
    # as built (2026-10-31): aft horns (3.5 in arm across the boat), tie bar and drive link behind the
    # aft screen, bellcrank 3.5 / 4 in on a 3/8 x 1.5 steel bracket off the aft hoop, straight 1 x .065 rod
    arm, armB = 3.5, 4.0
    tie, drv = HINGE_M / arm, 2 * HINGE_M / arm
    rod = 2 * HINGE_M / armB
    A = np.array((-91.44, 812.8, 712.0)) * MM
    B = np.array((2438.408, 383.623, 444.394)) * MM
    Lr = float(np.linalg.norm(B - A)) - 1.2
    st = MAT['steel']
    s58 = round_tube(0.625, 0.065)
    fixed = [check('Long rod, 1 x .065 steel, buckling (L %.0f in, pinned)' % Lr, rod, math.pi ** 2 * st['E'] * s1['Iy'] / Lr ** 2, 3.0, 'lb'),
             check('Drive link, 5/8 x .065, buckling (13 in)', drv, math.pi ** 2 * st['E'] * s58['Iy'] / 13 ** 2, 3.0, 'lb'),
             check('Tie bar, 5/8 x .065, buckling (26 in)', tie, math.pi ** 2 * st['E'] * s58['Iy'] / 26 ** 2, 3.0, 'lb')]
    Fp = math.hypot(drv, rod)
    Sb = 1.5 * 0.375 ** 2 / 6
    fixed.append(check('Bellcrank bracket, 3/8 x 1.5 steel, %.0f lb at 1.6 in off the hoop' % Fp, Fp * 1.6 / Sb, 36000, REQ_MEMBER, note='A36'))
    Sa = 0.25 * 1.0 ** 2 / 6          # in-plane (edgewise) bending of the 1/4 x 1 arm
    fixed.append(check('Bellcrank arm, 1/4 x 1 steel, %.0f lb at 3.5 in' % drv, drv * 3.5 / Sa, 36000, REQ_MEMBER, note='A36, edgewise at the hub; the eye sits just under the plate'))
    fos, _, Vc = bolt_fos('3/8 SS (F593 CW)', 0, Fp)
    fixed.append(check('Bellcrank pivot and rod-end pins, 3/8 SS, single shear', Fp, Vc, REQ_CONN, 'lb'))
    hand = rod * 13.1 / 34.7
    return dict(offset=off, sigma=sig, F_yield=F_yield, L=Lstraight, Pcr=Pcr, Pcr1=Pcr1, checks=checks, checks_fixed=fixed,
                hinge_M_yield=F_yield * HORN_RY / 2, new=dict(tie=tie, drive=drv, rod=rod, L=Lr, hand=hand))


# ==================================================================================================
# 6. Transom plate and trim tab
# ==================================================================================================
def run_transom():
    t, b = 0.125, 17.0 - 1.6            # 1/8 5052, clear span bottom angle to rub rail
    m = MAT['al5052']
    p = 0.75                            # psi, assumed following-wave slap (~21 in head); ISO 12215-5 to confirm
    sig_fixed = 0.5 * p * (b / t) ** 2
    sig_ss = 0.75 * p * (b / t) ** 2
    checks = [check('Transom 1/8 5052 between the bottom angle and rub rail, 0.75 psi (simply supported)', sig_ss, m['Fy'], REQ_MEMBER)]
    # trim tab (18 x 6 in, starboard): Savitsky flap lift at 39 mph, 5 deg, x 1.3
    V = 39 * 1.4667
    beam = 66 / 12
    dCL = 0.046 * (6 / 66) * 5 * (18 / 66)
    L = 1.3 * dCL * 0.5 * RHO_W * V ** 2 * beam ** 2
    Mh = L * 0.4 * 6
    arm = abs(-2 * 6.3) / math.hypot(2, 6.3)
    Fs = Mh / arm
    Fx = Fs * 2 / math.hypot(2, 6.3)     # out-of-plane pull on the transom at the bracket
    a, Ls = 6.3 - 1.6, b
    Mstrip = Fx * a * (Ls - a) / Ls
    S6 = 6 * t ** 2 / 6
    checks.append(check('Trim-tab bracket on the bare 1/8 transom (6 in strip)', Mstrip / S6, m['Fy'], REQ_MEMBER))
    S_bar = 2 * 0.375 ** 2 / 6 + 6 * t ** 2 / 6   # 3/8 x 2 flat bar flat on the plate + the strip (non-composite)
    fixed = [check('Same with the 3/8 x 2 in 6061 flat bar riveted behind the bracket (built)', Mstrip / S_bar, MAT['al6061']['Fy'], REQ_MEMBER,
                   note='a 1.5 in angle would hit the aft floor panel 0.55 in inside the transom')]
    return dict(p=p, sig_fixed=sig_fixed, sig_ss=sig_ss, tab_lift=L, hinge_M=Mh, strut=Fs, pull=Fx, checks=checks, checks_fixed=fixed)


def run_footrest():
    """Two foot ladders (2026-10-31): solid 1 in 6061 rod rails and rungs, welded. A 400 lb stomp
    (325 lb rider bracing) at mid-rung; the rung spans between rail centres, each rail spans its
    floor foot to the bow-shelf top (about 17 in along the slope) and takes half of it."""
    al = MAT['al6061']
    S = math.pi / 32
    P = 400.0
    L_rail = (120.0 - 0.5 - 104.5) / math.cos(math.radians(22.7))
    checks = []
    for name, span in (('right (throttle) ladder rung, 7.0 in span', 7.0), ('left ladder rung, 5.0 in span', 5.0)):
        checks.append(check('Footrest %s, 1 in solid rod, 400 lb at mid' % name, P * span / 4 / S, al['Fyw'], REQ_MEMBER))
    checks.append(check('Footrest rail, 1 in solid rod, half the 400 lb at mid-length (%.1f in)' % L_rail, P / 2 * L_rail / 4 / S, al['Fyw'], REQ_MEMBER))
    old = [check('Old single ladder rung, 15 in span, 400 lb at mid', P * 15 / 4 / S, al['Fyw'], REQ_MEMBER)]
    return dict(checks=old, checks_fixed=checks)


def main():
    out = dict(loads=LOADS, req=dict(member=REQ_MEMBER, conn=REQ_CONN), materials=MAT)
    out['tower'] = run_tower()
    tf = run_tower(rect_tube(2, 3, 0.125))
    out['tower']['checks_fixed'] = [dict(c, item=c['item'].replace('crossbar (fwd, tie struts)', 'crossbar (fwd) as 2 x 3 x 1/8, 3 in deep'))
                                    for c in tf['checks'] if 'crossbar (fwd' in c['item']]
    out['tower']['fatigue_fixed'] = tf['fatigue_range']
    out['cage'] = run_cage()
    out['side'] = run_side_frame()
    out['engine'] = run_engine()
    out['pushrod'] = run_pushrod()
    out['transom'] = run_transom()
    out['footrest'] = run_footrest()
    for k in ('tower', 'cage', 'side', 'engine', 'pushrod', 'transom', 'footrest'):
        out[k].setdefault('checks_fixed', [])
        print('==', k)
        for c in out[k]['checks'] + out[k].get('checks_fixed', []):
            print('  %-95s %10.0f %10.0f  FoS %5.2f %s' % (c['item'][:95], c['demand'], c['capacity'], c['fos'], '' if c['ok'] else '<-- LOW'))
    json.dump(out, open(os.path.join(HERE, 'structure.json'), 'w'), indent=1,
              default=lambda o: o.tolist() if isinstance(o, (np.ndarray, np.generic)) else str(o))
    return out


if __name__ == '__main__':
    main()
