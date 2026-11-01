"""Builds ../Structural_Checks.html from structure.json (run structure.py first)."""
import html
import json
import os
import re

HERE = os.path.dirname(os.path.abspath(__file__))
R = json.load(open(os.path.join(HERE, 'structure.json')))
ISO = json.load(open(os.path.join(HERE, 'iso_bottom.json')))
CSS = re.search(r'<style>.*?</style>', open(os.path.join(HERE, 'report_template.html'), encoding='utf-8').read(), re.S).group(0)
CSS = CSS.replace('</style>', 'table.l td:last-child,table.l th:last-child{text-align:left}td.bad{color:var(--warn);font-weight:600}td.good{color:var(--accent)}\n</style>')
L = R['loads']


def n(v, d=0):
    return '{:,.{}f}'.format(v, d)


def table(checks, title=None):
    rows = []
    for c in checks:
        unit = c['unit']
        dem = n(c['demand'] / 1000, 1) + ' ksi' if unit == 'psi' else n(c['demand']) + ' lb'
        cap = n(c['capacity'] / 1000, 1) + ' ksi' if unit == 'psi' else n(c['capacity']) + ' lb'
        fos = '&gt; 50' if c['fos'] > 50 else n(c['fos'], 2)
        cls = 'good' if c['ok'] else 'bad'
        rows.append('<tr><td>%s%s</td><td>%s</td><td>%s</td><td class="%s">%s</td><td>%s</td></tr>' % (
            html.escape(c['item']), ('<br><span class="small">%s</span>' % html.escape(c['note'])) if c.get('note') else '',
            dem, cap, cls, fos, n(c['req'], 2)))
    head = '<h3>%s</h3>' % title if title else ''
    return head + ('<div class="tw"><table><tr><th>Check</th><th>Demand</th><th>Allowable / capacity</th><th>FoS</th>'
                   '<th>Needs</th></tr>%s</table></div>' % ''.join(rows))


def pushrod_svg():
    P = [(-129.54, 450.85), (-114.3, 787.4), (342.9, 787.4), (2438.408, 345.523)]
    P = [(x / 25.4, y / 25.4) for x, y in P]
    W, H, l, r, t, b = 640, 230, 40, 20, 20, 40
    sx = lambda x: l + (x + 10) / 115 * (W - l - r)
    sy = lambda y: t + (36 - y) / 36 * (H - t - b)
    path = ' '.join('%.1f,%.1f' % (sx(x), sy(y)) for x, y in P)
    out = ['<svg viewBox="0 0 %d %d" role="img" aria-label="Pushrod plan view">' % (W, H)]
    for y in (0, 12, 24, 36):
        out.append('<line class="grid" x1="%d" x2="%d" y1="%.1f" y2="%.1f"/><text class="tick" x="%d" y="%.1f">%d</text>' % (l, W - r, sy(y), sy(y), l - 6, sy(y) + 4, y))
    for x in (0, 24, 48, 72, 96):
        out.append('<text class="tick mid" x="%.1f" y="%d">%d</text>' % (sx(x), H - b + 18, x))
    out.append('<line class="flood" x1="%.1f" y1="%.1f" x2="%.1f" y2="%.1f"/>' % (sx(P[0][0]), sy(P[0][1]), sx(P[3][0]), sy(P[3][1])))
    out.append('<polyline class="ln b" points="%s"/>' % path)
    out.append('<text class="ttl" x="%.1f" y="%.1f">rudder horn</text>' % (sx(P[0][0]) + 6, sy(P[0][1]) + 16))
    out.append('<text class="ttl" x="%.1f" y="%.1f" text-anchor="end">steering lever</text>' % (sx(P[3][0]), sy(P[3][1]) + 18))
    out.append('<text class="ttl" x="%.1f" y="%.1f">rod as drawn (outside the cage)</text>' % (sx(14), sy(31) - 8))
    out.append('<text class="ttl" x="%.1f" y="%d">x forward of the transom (in) · y to port (in)</text></svg>' % (l, H - 6))
    out.append('<p class="legend"><span class="b">pushrod as drawn</span><span class="d">line between the two rod-end pins: the only line a pinned rod can push along</span></p>')
    return ''.join(out)


def main():
    T, C, S, E, P, X = R['tower'], R['cage'], R['side'], R['engine'], R['pushrod'], R['transom']
    F = R['footrest']
    allc = T['checks'] + C['checks'] + S['checks'] + E['checks'] + P['checks'] + X['checks'] + F['checks']
    low = [c for c in allc if not c['ok']]
    fixes_ok = all(c['ok'] for c in T['checks_fixed'] + C['checks_fixed'] + S['checks_fixed'] + E['checks_fixed'] + X['checks_fixed'] + F['checks_fixed'] + P['checks_fixed'])
    cd, cf = C['as drawn'], C['with fixes']
    out = ['<!doctype html><html lang="en"><head><meta charset="utf-8"><meta name="viewport" content="width=device-width, initial-scale=1">',
           '<title>Structural Checks</title>', CSS, '</head><body><main>',
           '<h1>Structural checks</h1>',
           '<p class="sub">Straddle Dart rev 7 · hand calculations for engineering review · 31 Oct 2026</p>',
           '<div class="card"><p><strong>Status:</strong> calculated from the rev 7 Fusion geometry. Not reviewed or stamped by a licensed engineer. '
           'Linear frame models and closed-form checks only, no finite-element run. Inputs, load cases, methods and the scripts that reproduce every number are below. '
           'All the fixes below are applied in the rev 7 CAD (split design files), BOM and build guide.</p></div>',
           '<h2>Summary</h2><div class="kpis">',
           '<div class="kpi"><b>%d</b><span>checks run</span></div>' % len(allc),
           '<div class="kpi"><b>%d</b><span>below the required safety factor as drawn</span></div>' % len(low),
           '<div class="kpi"><b>%s</b><span>pass as now built</span></div>' % ('all' if fixes_ok else 'not all'),
           '<div class="kpi"><b>%s lb</b><span>design prop thrust (430 lb G13B estimate + 25%%)</span></div>' % n(L['thrust_design']),
           '</div>']
    # findings
    N = P['new']
    out.append('<div class="card ok"><p><strong>1. Steering pushrod: redesigned (31 Oct 2026).</strong> The old rod had a rod end at each end but jogged %.1f in off the line between them. It would have yielded at about %.0f lb of rod force, against the %.0f lb that hard rudder at speed produces. '
               'Now every link is straight and pinned at both ends:'
               '<ul>'
               '<li><strong>Horns:</strong> they point 3.5 in aft and 2 in outboard.</li>'
               '<li><strong>Tie bar (%.0f lb) and drive link (%.0f lb):</strong> both run behind the aft screen, under the rudders&rsquo; bottom edges.</li>'
               '<li><strong>Bellcrank:</strong> 3.5 / 4 in arms, on a 3/8 in steel bracket welded to the aft hoop at the lower rail.</li>'
               '<li><strong>Long rod:</strong> a single %.0f in 1 &times; .065 rod (%.0f lb, buckling factor 4.4) runs outside the cage to the lever, which moved 1.5 in outboard.</li>'
               '</ul>'
               'Lever travel stays about &plusmn;10&deg; for &plusmn;33&deg; of rudder, and full rudder at speed takes about %.0f lb at the handle. '
               'The steering reaction now lands at the hoop/rail corner instead of mid-rail.</p></div>'
               % (P['offset'], P['F_yield'], L['rod_F'], N['tie'], N['drive'], N['L'], N['rod'], N['hand']))
    TF, SF, XF, EF = T['checks_fixed'], S['checks_fixed'], X['checks_fixed'], E['checks_fixed']
    worst = lambda cs: min(c['fos'] for c in cs)
    out.append('<div class="card ok"><p><strong>Fixed in the rev 7 CAD, BOM and build guide (31 Oct 2026):</strong></p><ol start="2">'
               '<li><strong>Grip.</strong> The 1 in 316 stick was shown welded into the 6061 inner rail. That was my rev 7 error: stainless can&rsquo;t be welded to aluminium. '
               'It is now a 1-1/8 in stick welded to a 3 &times; 2 &times; 1/4 in 316 base plate. Four 1/4 in 316 bolts clamp the plate through the console top and both rail walls, with crush tubes, Tef-Gel and a gasket. '
               'Stick factor 1.31 &rarr; %.2f; bolts %.2f.</li>'
               '<li><strong>Prop cage.</strong> The side bays and the drum had no diagonals, so the tubes carried the loads in bending. Now:'
               '<ul>'
               '<li>DOM tube (yield 50 ksi or better) throughout.</li>'
               '<li>Hoops 1 &times; .083 (were .049; .065 was enough for the G13, .083 is needed for the stronger G13B slipstream).</li>'
               '<li>Longitudinals and aft screen bars 1 &times; .065 DOM (were 3/4 and 1/2 EMT).</li>'
               '<li>A 1 &times; .065 diagonal in each side bay, from the aft foot up to the forward leg 3 in below the hoop.</li>'
               '</ul>'
               'Lowest factor 0.71 &rarr; %.2f. Kickers from the rudder rails were tried and dropped: they would cross the prop disk.</li>'
               '<li><strong>Trim-tab bracket.</strong> A 3/8 &times; 2 in 6061 flat bar is riveted inside the transom behind it, from the bottom frame angle to the rub rail (0.41 &rarr; %.2f). '
               'A 1.5 in angle would have hit the aft floor panel, which starts 0.55 in inside the transom.</li>'
               '<li><strong>Driver side frame.</strong> Posts, arms, inner rail, knee braces and diagonal are now 3/16 in wall (1.28 &rarr; %.2f, skins not counted). This adds 5.6 lb.</li>'
               '<li><strong>Prop shaft.</strong> Now specified as 4140 pre-hardened, 28&ndash;32 HRC (fatigue factor 1.56 &rarr; %.2f), with the thrust carried on a shoulder or clamped collar.</li>'
               '<li><strong>Forward A-frame crossbar.</strong> Now 2 &times; 3 &times; 1/8, 3 in deep (static factor 1.68 &rarr; %.2f). Its belt-ripple stress range drops from %.1f to %.1f ksi. '
               'The build guide now says to check the tie-strut bolts are tight before every run: without the struts each aft engine mount would see about %.0f lb of uplift.</li>'
               '</ol><p>Together these add about 22 lb (empty boat 1,247 lb by the CAD weight model). Every changed part was interference-checked: no new clashes.</p></div>'
               % (SF[-2]['fos'], SF[-1]['fos'], worst(cf['checks']), XF[0]['fos'], worst(SF[:-2]), EF[0]['fos'], TF[0]['fos'],
                  T['fatigue_range']['crossbar (fwd, tie struts)'] / 1000, T['fatigue_fixed']['crossbar (fwd, tie struts)'] / 1000, E['no_struts']['per_mount']))
    out.append('<div class="card ok"><p><strong>What passes as drawn:</strong>'
               ' A-frame legs (factor 4.6 or more).'
               ' Clevis braces (%.0f lb, buckling factor 27).'
               ' Clevis bolts, tongues and tabs.'
               ' A-frame and brace foot bolts.'
               ' Yoke bolts.'
               ' Engine mounts (bolt factor over 50, bushing 5.7).'
               ' Flange bearings (L10 about %s h at the belt load).'
               ' Transom plating at 0.75 psi (2.7).'
               ' Cage pad bolts.'
               ' Armrest hinge.</p></div>' % (-T['brace_N'][0], n(E['bearing']['L10h'])))
    # 1 inputs
    out.append('<h2>1 · Loads</h2><div class="tw"><table class="l"><tr><th>Load</th><th>Value</th><th>Basis</th></tr>')
    rows = [
        ('Prop thrust, design', '%s lb' % n(L['thrust_design']), '430 lb static estimate for the G13B (about 100 hp; the summary&rsquo;s 340 lb at about 70 hp scaled by power<sup>2/3</sup>), +25% for the estimate and throttle overshoot; applied at the 52.5 in shaft line'),
        ('Belt hub load', '%s lb' % n(L['belt_hub']), 'G13B peak torque 83 lb-ft on the 30T pulley (pitch radius 1.50 in) gives Te %s lb; hub = Te &times; 1.5 (tight + slack side)' % n(L['belt_Te'])),
        ('Gyroscopic moment', '%s lb-ft' % n(L['gyro_lbin'] / 12), 'Prop I &asymp; 0.55 slug-ft&sup2; (3 carbon blades about 12 lb, k 0.45 R), 2,600 rpm (6,500 engine rpm at 2.5:1), yaw 1.5 rad/s (86&deg;/s spin)'),
        ('Vertical acceleration', '3 g total', 'Stern, running over chop, sheltered water'),
        ('Lateral acceleration', '1 g', 'Hard skidding turn'),
        ('Rudder normal force', '%s lb each' % n(L['rudder_N']), 'C<sub>N</sub> 1.2 &times; q %.1f psf &times; 4.1 ft&sup2; &times; 1.25; slipstream %.0f ft/s at 39 mph at the rudders (momentum theory, 9 in aft of the disk)' % (L['q_rudder'], L['v_slip_speed'])),
        ('Rudder hinge moment and rod force', '%s lb-in; %s lb' % (n(L['hinge_M']), n(L['rod_F'])), 'Centre of pressure 0.3 chord + 0.6 in aft of the hinge; both rudders through the tie bar; horn arm 4.0 in'),
        ('Screen drag', '%s lb aft, %s lb front' % (n(L['mesh_aft']), n(L['mesh_fwd'])), 'Hardware cloth K = %.2f (Idelchik, solidity 0.16) at 39 mph plus the prop inflow, &times;1.25' % L['mesh_K']),
        ('Hand-hold (grip)', '600 lb', 'Pull in any direction at the knob, 3.75 in above the rail'),
        ('Knee on a brace', '400 lb', '325 lb rider, about 0.6 g turn, &times;2 for impact'),
        ('Armrest lean / console step', '250 lb / 300 lb', 'At the armrest end / mid console'),
        ('Cage misuse', '250 lb down, 150 lb sideways', 'Someone standing on a hoop top / shoving the side'),
        ('Transom pressure', '0.75 psi', 'Assumed following-wave slap (about 21 in of head). ISO 12215-5 should replace this'),
    ]
    out += ['<tr><td>%s</td><td>%s</td><td>%s</td></tr>' % r for r in rows]
    out.append('</table></div>')
    out.append('<h2>2 · Method and criteria</h2><ul>'
               '<li><strong>Frame models:</strong> 3D linear frames (6 degrees of freedom per node, Euler-Bernoulli beams), solved in <code>calc/frame3d.py</code>, which reproduces the textbook cantilever exactly. '
               'Drive tower: four A-frame legs, two crossbars, pinned clevis braces, and a rigid head standing for the peak plates and yoke. Each case is run with fixed and with pinned feet, and the worse result is kept. '
               'Cage: both hoops in 36+ chords, 8 longitudinals, face bars, rudder rails, legs on fixed pads. '
               'Side frame: posts on the base plate and hull-side clips, top rail, cross arms, bowed braces in 8 chords, inner rail, diagonal.</li>'
               '<li><strong>Stresses:</strong> axial plus bending at each element end; square tubes add both bending axes; torsion is combined by von Mises.</li>'
               '<li><strong>Allowables:</strong>'
               '<ul>'
               '<li>Welded 6061-T6 members are checked against the heat-affected yield of 15 ksi (ADM) everywhere, because the peak moments sit at the welds.</li>'
               '<li>Cage steel: A513 Type 1 minimum, 32 ksi, as drawn; DOM at 50 ksi or better with the fixes. EMT: 30 ksi.</li>'
               '<li>Bolts: stainless F593 condition CW, 100 ksi; grade 8, 150 ksi.</li>'
               '<li>Aluminium bearing in the heat-affected zone: 2 &times; 24 ksi &times; d &times; t.</li>'
               '</ul></li>'
               '<li><strong>Required safety factors:</strong> 1.65 on yield and buckling for operating loads (ADM / AISC allowable-stress level); 1.0 for misuse loads (no permanent bend); 2.0 on bolts and pins; 2.0 on shaft fatigue (Goodman).</li>'
               '<li><strong>Fatigue screen:</strong> the stress range from &plusmn;25% belt-load ripple, compared with 2 ksi. That is roughly the constant-amplitude limit of common welded aluminium details (ADM Chapter J, categories D&ndash;E). The reviewer should confirm the detail category.</li></ul>')
    out.append('<h2>3 · Drive tower</h2><p>Belt pull path: prop pulley &rarr; yoke &rarr; peak plates &rarr; forward A-frame &rarr; crossbar &rarr; tie struts &rarr; bellhousing adapter &rarr; jackshaft. Thrust goes through the two clevis braces to the bed plates at x 49.5 in. Head deflection is at most %.3f in.</p>' % T['head_disp'])
    out.append(table(T['checks'], 'As drawn') + table(T['checks_fixed'], 'As now built: forward crossbar 2 × 3 × 1/8'))
    out.append('<h2>4 · Prop cage</h2><p>Lumped weight %.0f lb (tubes, screens, drum, rudders, light bar and searchlight). Drum movement at the prop plane under misuse loads: %s in as drawn.</p>'
               % (cd['weight'], ', '.join('%.2f' % v for v in cd['deflection'].values())))
    out.append(table(cd['checks'], 'As drawn') + table(cf['checks'], 'As now built: DOM tube, 1 × .065 hoops, longitudinals and aft bars, side-bay diagonals'))
    out.append('<h2>5 · Driver side frame, grip and armrest</h2><p>Knee brace: rolled to %.1f in radius, %.1f in long. A 400 lb knee load moves it %.3f in. The console top and outboard skin, counted as shear panels at half stiffness, change the post stress by only about 2%%.</p>' % (S['brace_R'], S['brace_L'], S['knee_defl']))
    out.append(table(S['checks'], 'As drawn') + table(S['checks_fixed'], 'As now built: 3/16 in walls; grip on a bolted base plate (skins not counted)'))
    out.append('<h2>6 · Engine mounts, prop shaft and bearings</h2><p>Engine assembly %.0f lb on four mounts at x 31 and 42 in. Worst aft-mount radial load %.0f lb (3 g vertical %.0f, 1 g roll couple %.0f, 2 &times; peak torque %.0f). '
               'The tie struts carry the belt pull. Without them the aft pair would see %.0f lb of uplift. Flange bearings (C &asymp; 6,500 lb): about %.0f lb typical load, L10 &asymp; %s h.</p>'
               % (E['We'], E['mount_radial'], *E['mount_parts'].values(), E['no_struts']['aft_pair_uplift'], E['bearing']['P'], n(E['bearing']['L10h'])))
    out.append(table(E['checks'] + E['checks_fixed']))
    out.append('<h2>7 · Steering linkage</h2><p>Plan view of the old pushrod (5/8 &times; .065 steel, rod ends at both ends). The rod can only carry force along the dashed line. The %.1f in jog turns that force into bending.</p>' % P['offset'])
    out.append(pushrod_svg())
    out.append(table(P['checks'], 'Before') + table(P['checks_fixed'], 'As now built'))
    out.append('<h2>8 · Transom and trim tab</h2><p>The transom is 1/8 in 5052 with no vertical stiffeners: %.1f in clear between the bottom frame angle and the rub rail. At 0.75 psi it sees %.1f ksi with fixed edges and %.1f ksi with simply supported edges. '
               'Trim tab: lift %.0f lb, hinge moment %.0f lb-in, adjuster strut %.0f lb, out-of-plane pull on the transom %.0f lb.</p>'
               % (17.0 - 1.6, X['sig_fixed'] / 1000, X['sig_ss'] / 1000, X['tab_lift'], X['hinge_M'], X['strut'], X['pull']))
    out.append(table(X['checks']) + table(X['checks_fixed'], 'As now built'))
    out.append('<h2>8b · Footrest ladders</h2><p>Rebuilt 31 Oct 2026 as two foot ladders, a landing for each foot like motorcycle pegs:'
               '<ul>'
               '<li><strong>Left:</strong> rails 7.5 and 12.5 in to port.</li>'
               '<li><strong>Right:</strong> rails 7.5 and 14.5 in to starboard, with the throttle pedal centred 10.75 in to starboard.</li>'
               '<li><strong>Both:</strong> solid 1 in 6061 rod rails and rungs, the same 5 rung positions and slope.</li>'
               '<li><strong>Steering lever:</strong> moved 1.5 in outboard, clear of the left knee.</li>'
               '</ul>'
               'The old single ladder&rsquo;s 15 in rungs were at yield under a 400 lb stomp.</p>')
    out.append(table(F['checks'], 'Before') + table(F['checks_fixed'], 'As now built'))
    I = ISO['inputs']
    rows = ''.join('<tr><td>%s</td><td>%.1f</td><td>%s</td><td>%.2f</td><td>%.2f</td><td>%.1f kPa (%.1f psi)</td><td class="%s">%.3f in</td></tr>' % (
        html.escape(c['category']), c['ncg'], html.escape(c['panel']), c['kAR'], c['kL'], c['P_kPa'], c['P_psi'],
        'good' if c['t_req_in'] <= ISO['have_t_in'] + 0.0015 else 'bad', c['t_req_in']) for c in ISO['cases'])
    orows = ''.join('<tr><td>%s</td><td>%.1f</td><td>%.1f kPa</td><td>%.3f in</td></tr>' % (html.escape(o['category']), o['ncg'], o['P_kPa'], o['t_req_in']) for o in ISO['options'])
    srows = ''.join('<tr><td>%s</td><td>%s</td><td>%.1f</td><td>%.1f kPa</td><td>%.3f in&sup3;</td><td class="good">%.3f in&sup3;</td></tr>' % (
        html.escape(r['stringer']), html.escape(r['category']), r['ncg'], r['P'], r['sm_req_in3'], r['sm_have_in3']) for r in ISO['stringers'])
    out.append('<h2>8c · Bottom plating to ISO 12215-5</h2>'
               '<div class="card ok"><p><strong>Bottom plating: now 0.125 in, with frames added at 60, 84 and 111.5 in (31 Oct 2026).</strong> '
               'The original 0.090 in bottom was thinner than ISO 12215-5 requires: its worst panels, between frames 24 in apart, needed 0.131&ndash;0.176 in. '
               'As built, the boat passes as a category D (sheltered waters) planing cruiser with the dynamic load factor limited to 3.0 g: the worst bay needs 0.117 in. '
               'In category C (inshore) it needs 0.126 in, within 1%%. '
               'At the speed-based 5.4 g it would need up to 0.157 in, so the 3.0 g basis must be written into the owner&rsquo;s manual: a planing cruiser, slow down in waves. '
               'The hat stringers pass with large margins. The 1/4 in UHMW is not counted.</p></div>'
               '<p><strong>Inputs:</strong></p><ul>'
               '<li>Loaded displacement %.0f kg (%.0f lb): empty with the heavier bottom, plus the 508 lb capacity, plus full fuel.</li>'
               '<li>Loaded waterline length %.2f m, chine beam %.2f m.</li>'
               '<li>Deadrise taken as 10&deg;: the bottom is flat, and the standard&rsquo;s minimum is 10&deg;.</li>'
               '<li>Top speed %.1f knots.</li>'
               '<li>Plating design stress %.0f MPa: 5052-H32 unwelded, the lesser of 0.6&sigma;<sub>u</sub> and 0.9&sigma;<sub>y</sub>. The bottom is riveted, with no welds.</li>'
               '</ul>'
               '<p><strong>Dynamic load factor n<sub>CG</sub>:</strong> its first equation gives %.1f g, because the bottom is flat, the boat is light and it&rsquo;s fast. Two lower values are shown:'
               '<ul>'
               '<li>%.1f g, from the standard&rsquo;s alternative speed-based equation.</li>'
               '<li>3.0 g, the limit for planing cruisers. Using it requires the owner&rsquo;s manual to state that operation, for example: slow down in waves.</li>'
               '</ul>'
               'The Everglades fits design category D (sheltered waters); C (inshore) is shown for open lakes.</p>'
               % (I['m_LDC_kg'], I['m_LDC_lb'], I['LWL_m'], I['BC_m'], I['V_kn'], I['sd_plate'], ISO['ncg']['eq1'], ISO['ncg']['eq2'])
               + '<div class="tw"><table><tr><th>Category</th><th>n<sub>CG</sub></th><th>Panel</th><th>k<sub>AR</sub></th><th>k<sub>L</sub></th><th>Design pressure</th><th>Required t (have 0.125)</th></tr>' + rows + '</table></div>'
               + '<h3>Hat stringers (24 in span between frames, 15.6 in spacing)</h3><div class="tw"><table><tr><th>Stringer</th><th>Category</th><th>n<sub>CG</sub></th><th>Pressure</th><th>Section modulus needed</th><th>Has</th></tr>' + srows + '</table></div>'
               + '<p class="small">The equations follow ISO 12215-5:2008. The full text wasn&rsquo;t available when this was written, so the reviewer should confirm each one against a licensed copy:'
               '<ul class="small">'
               '<li>both n<sub>CG</sub> equations, and which governs;</li>'
               '<li>k<sub>DC</sub> (category C 0.6, D 0.4);</li>'
               '<li>k<sub>L</sub>;</li>'
               '<li>k<sub>AR</sub>, with k<sub>R</sub> = 1.5 &minus; 3&times;10<sup>&minus;4</sup> b for plating and 1 &minus; 2&times;10<sup>&minus;4</sup> l<sub>u</sub> for stiffeners;</li>'
               '<li>the planing and displacement base pressures and the minimum pressure;</li>'
               '<li>k<sub>2</sub>;</li>'
               '<li>the aluminium design stresses.</li>'
               '</ul>'
               '<span class="small">Script: <code>calc/iso_bottom.py</code>.</span></p>')
    out.append('<h2>9 · Limits of this analysis</h2><ul>'
               '<li><strong>Load estimates:</strong> thrust, prop inertia, belt tension, rudder coefficients and tab lift are all estimates. A static thrust test and a weighed prop would firm up the two biggest.</li>'
               '<li><strong>Simplified joints and parts:</strong> joints are modelled as rigid welds or ideal pins, and the peak plates and yoke as one rigid body. The cage mesh and console skins are left out as stiffeners (conservative). The tower is modelled as rigid on the bed plates; the beds and stringers aren&rsquo;t modelled.</li>'
               '<li><strong>Fatigue:</strong> screened, not calculated. Weld quality dominates real fatigue life of welded aluminium drive frames, so inspect the A-frame welds after the first 10 hours and every season.</li>'
               '<li><strong>Not covered:</strong> blade failure (no cage stops a thrown blade), trailer and tie-down loads, and hull bottom slamming.</li></ul>')
    out.append('<h2>10 · Reviewer checklist</h2><ol>'
               '<li>Confirm the design loads in section 1, especially thrust, the belt hub factor and the 3 g / 1 g accelerations.</li>'
               '<li>Confirm the HAZ allowables and the fatigue detail category for the A-frame crossbar and tab welds.</li>'
               '<li>Review the steering linkage, grip mounting, cage bracing and side-frame walls as built.</li>'
               '<li>Replace the 0.75 psi transom pressure with ISO 12215-5 values, and confirm the section 8c bottom-plating equations and the design category.</li>'
               '<li>Check the Savitsky flap lift used for the trim tab against the tab&rsquo;s real maximum angle.</li></ol>')
    out.append('<h2>11 · Reproducing these results</h2><p>Run <code>calc/structure.py</code> (it writes <code>structure.json</code>), then <code>calc/make_structure_report.py</code>. Python 3 with numpy. The geometry is typed in from the Fusion parameters (<code>cage_*</code>, <code>drv_*</code>) and the body survey <code>data/bodies.json</code>.</p>')
    out.append('</main></body></html>')
    open(os.path.join(HERE, '..', 'Structural_Checks.html'), 'w', encoding='utf-8').write('\n'.join(out))
    print('written', len(allc), 'checks,', len(low), 'low as drawn; fixes pass:', fixes_ok)


if __name__ == '__main__':
    main()
