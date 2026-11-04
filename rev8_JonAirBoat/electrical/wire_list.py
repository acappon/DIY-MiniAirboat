"""Master wire list for the minimal 1448. Everything electrical comes from this file:
  python wire_list.py   writes Wire_List.csv, Schematic.html and ../CAD/FeatureScript/Minimal_wiring.fs

Frame: X forward from the transom, Y + to port, Z up from the hull bottom (inches).
Rules (user, 2026-11-03): chassis ground is used only inside the engine. 1/0 AWG black from battery
negative to the engine, 1/0 AWG red with a fusible link from battery positive to the starter solenoid.
Everything else is a red/black pair of one AWG from the hub to the device. Red is power, black is return,
every other color is a signal or a sensor. 12 AWG with a 20 A fuse to the 12 V outlet, 18 AWG with a 10 A
fuse elsewhere unless the voltage drop below says otherwise.
"""
import csv
import html
import math
import os

HERE = os.path.dirname(os.path.abspath(__file__))
VOLTS = 13.8

# AWG: ohms per foot, lb per foot with insulation, CAD radius
AWG = {'1/0': (0.0001, 0.34, 0.20), '6': (0.0004, 0.085, 0.12), '12': (0.0016, 0.022, 0.065), '18': (0.0064, 0.008, 0.045)}
COLORS = {'red': '#d11919', 'black': '#111111', 'orange': '#f07a0a', 'purple': '#7d3fb0', 'blue': '#1f6fd1', 'violet': '#c04fd8',
          'yellow': '#e6c300', 'gray': '#808890', 'green': '#199c3c', 'tan': '#c49a5c', 'brown': '#7a4a22', 'pink': '#ff86b5',
          'white': '#e8e8e8', 'white/blue': '#a9c8ee', 'white/green': '#a9e0ba', 'K yellow': '#d6b800'}

P = {  # device terminals
    'BAT+': (86, 2, 8), 'BAT-': (86, -3, 8), 'HUB': (85.5, 0, 9), 'K': (88, 3.6, 6),
    'SOL': (31, -10.6, 15.2), 'GND': (29, -7.4, 19), 'J1': (31, -10.6, 18.2),
    'LED': (8, 0, 88.7), 'SEARCH': (5, 3, 88.7), 'STERN': (9, 11, 88.7), 'BOWNAV': (157, 0, 17.2),
    'BILGE': (8, 3, 1.1), 'PUMP': (138, -3, 25.7), 'SENDER': (143, 3, 25.5),
    'CTS': (28, -6.5, 29), 'EGT': (25.5, 9.3, 31),
    'TB1': (103.5, -19.6, 27.2), 'TB2': (103.5, -18.4, 27.2),
}
POD_B = (100, 0, 25.2)
T_ = (0.381, 0, 0.924)
N_ = (-0.924, 0, 0.381)


def back(y, t):
    """Point behind the dash face for a device at height t along the face."""
    return tuple(POD_B[i] + (T_[i] * t) - N_[i] * 0.5 + ((y if i == 1 else 0)) for i in range(3))


D = {'OUTLET': back(-19.7, 1.7), 'USB': back(-6.6, 4.75), 'GAS': back(-9.6, 4.75), 'TACH': back(-13, 4.75), 'TEMP': back(-16.4, 4.75),
     'EGTG': back(-19.4, 4.75), 'KEY': back(-17, 1.7), 'ROT': back(-14.3, 1.7), 'SW': back(-11.6, 1.7), 'JOY': back(-8.9, 1.7), 'DESS': back(-6.2, 1.7)}
P.update(D)

ROUTES = {
    'box': [],
    'eng': [(82.4, 0, 6), (75.3, 0, 5), (72, 0, 3.2), (40, 0, 3.2), (34.6, -4, 3.6), (34.6, -10.5, 4.5), (34, -10.5, 13)],
    'cage': [(82.4, 0, 6), (75.3, 0, 5), (72, 0, 3.2), (40, 0, 3.2), (34.6, -4, 3.6), (34.6, -10.5, 4.5), (34, -10.5, 13), (30, -16, 14.6), (18, -29, 14.8), (15, -31.4, 16), (11.7, -31.4, 33)]
    + [(11.7, 36.8 * math.cos(math.radians(a)), 52 + 36.8 * math.sin(math.radians(a))) for a in (-150, -170, 170, 150, 130, 110, 95)],
    'bilge': [(82.4, 0, 6), (75.3, 0, 5), (72, 0, 3.2), (40, 0, 3.2), (34, -1, 2.1), (13, -1, 1.5)],
    'dash': [(89.7, -2, 10.5), (90, -20, 10), (96, -21.8, 10), (102.8, -21.8, 23.6), (103.2, -20, 26)],
    'bow': [(89.7, 2, 10.5), (90.5, 24, 12), (92, 27, 13.6), (120, 27, 15), (128, 24, 16.2), (131.3, 10, 17), (134, 7, 17)],
}

# Last points before a device, in order out from the device, to clear the rail, engine and tank.
WAY = {'GND': [(30, -10.6, 17.2)], 'CTS': [(30.5, -10.2, 20)], 'EGT': [(30.5, 6, 34), (30.5, -6.5, 34), (30.5, -10.2, 20)],
       'PUMP': [(134, 7, 26)], 'SENDER': [(134, 7, 26)], 'BOWNAV': [(153, 7, 17)]}

# id, name, from, to, AWG, color, amps, route, connector at from, connector at to, sheet note
W = []


def w(id_, name, a, b, awg, color, amps, route, ca, cb, note=''):
    W.append(dict(id=id_, name=name, a=a, b=b, awg=awg, color=color, amps=amps, route=route, ca=ca, cb=cb, note=note))


w('W01', 'Battery positive to starter solenoid', 'BAT+', 'SOL', '1/0', 'red', 150, 'eng', 'ring', 'ring', 'Fusible link within 7 in of the battery. Regulator output lands on the solenoid stud.')
w('W02', 'Battery negative to engine ground', 'BAT-', 'GND', '1/0', 'black', 150, 'eng', 'ring', 'ring', 'The only tie between the battery and chassis ground.')
w('W03', 'Battery positive to hub', 'BAT+', 'HUB', '6', 'red', 45, 'box', 'ring', 'ring', 'Through MF1, 60 A, at the battery.')
w('W04', 'Battery negative to hub', 'BAT-', 'HUB', '6', 'black', 45, 'box', 'ring', 'ring')
w('W05', 'F1 to 12 V outlet', 'HUB', 'OUTLET', '12', 'red', 20, 'dash', 'fork', 'spade', 'F1 20 A, always on')
w('W06', '12 V outlet return', 'OUTLET', 'HUB', '12', 'black', 20, 'dash', 'spade', 'ring')
w('W07', 'K3 to LED bar', 'K', 'LED', '12', 'red', 10.3, 'cage', 'spade', 'dt', 'F2 15 A through K3. 18 AWG would drop too much.')
w('W08', 'LED bar return', 'LED', 'HUB', '12', 'black', 10.3, 'cage', 'dt', 'ring')
w('W09', 'K4 to searchlight', 'K', 'SEARCH', '18', 'red', 3.7, 'cage', 'spade', 'dt', 'F3 10 A through K4')
w('W10', 'Searchlight return', 'SEARCH', 'HUB', '18', 'black', 3.7, 'cage', 'dt', 'ring')
w('W11', 'K2 to stern light', 'K', 'STERN', '18', 'red', 0.5, 'cage', 'spade', 'dt', 'F4 10 A through K2')
w('W12', 'Stern light return', 'STERN', 'HUB', '18', 'black', 0.5, 'cage', 'dt', 'ring')
w('W13', 'K2 to bow light', 'K', 'BOWNAV', '18', 'red', 0.5, 'bow', 'spade', 'dt', 'F4 10 A through K2')
w('W14', 'Bow light return', 'BOWNAV', 'HUB', '18', 'black', 0.5, 'bow', 'dt', 'ring')
w('W15', 'F5 to dash switch bus', 'HUB', 'TB1', '18', 'red', 6, 'dash', 'fork', 'ring', 'F5 10 A, always on. Feeds the key and both light switches.')
w('W16', 'K1 to dash accessory bus', 'K', 'TB2', '18', 'red', 3.5, 'dash', 'spade', 'ring', 'F6 10 A through K1. Gauges and USB, key on.')
w('W17', 'Dash accessory return', 'TB2', 'HUB', '18', 'black', 3.5, 'dash', 'ring', 'ring')
w('W18', 'F7 to bilge pump', 'HUB', 'BILGE', '18', 'red', 2, 'bilge', 'fork', 'dt', 'F7 10 A, always on, automatic pump')
w('W19', 'Bilge pump return', 'BILGE', 'HUB', '18', 'black', 2, 'bilge', 'dt', 'ring')
w('W20', 'K5 to fuel pump', 'K', 'PUMP', '12', 'red', 6, 'bow', 'spade', 'dt', 'F8 15 A through K5. Fuel pump current is an estimate.')
w('W21', 'Fuel pump return', 'PUMP', 'HUB', '12', 'black', 6, 'bow', 'dt', 'ring')
w('W22', 'Switch bus to key', 'TB1', 'KEY', '18', 'red', 6, 'pod', 'ring', 'spade')
w('W23', 'Switch bus to lights switch', 'TB1', 'ROT', '18', 'red', 1, 'pod', 'ring', 'spade')
w('W24', 'Switch bus to searchlight switch', 'TB1', 'SW', '18', 'red', 1, 'pod', 'ring', 'spade')
w('W25', 'Accessory bus to USB', 'TB2', 'USB', '18', 'red', 1.5, 'pod', 'ring', 'spade')
w('W26', 'USB return', 'USB', 'TB2', '18', 'black', 1.5, 'pod', 'spade', 'ring')
for i, (dev, name) in enumerate([('GAS', 'gas gauge'), ('TACH', 'tach'), ('TEMP', 'temperature gauge'), ('EGTG', 'exhaust gauge')]):
    w('W%02d' % (27 + 2 * i), 'Accessory bus to ' + name, 'TB2', dev, '18', 'red', 0.3, 'pod', 'ring', 'ring')
    w('W%02d' % (28 + 2 * i), name.capitalize() + ' return', dev, 'TB2', '18', 'black', 0.3, 'pod', 'ring', 'ring')
w('W35', 'Key ON to K1 coil and engine', 'KEY', 'J1', '18', 'orange', 0.5, 'keyeng', 'spade', 'dt', 'Ignition on signal. Branches to the K1 coil in the dry box.')
w('W36', 'Key START to solenoid', 'KEY', 'SOL', '18', 'purple', 5, 'keyeng', 'spade', 'ring', 'Start signal. Solenoid coil returns through the engine.')
w('W37', 'Lights switch RUN to K2 coil', 'ROT', 'K', '18', 'blue', 0.2, 'dashr', 'spade', 'spade', 'On in both RUN and LED positions')
w('W38', 'Lights switch LED to K3 coil', 'ROT', 'K', '18', 'violet', 0.2, 'dashr', 'spade', 'spade')
w('W39', 'Searchlight switch to K4 coil', 'SW', 'K', '18', 'yellow', 0.2, 'dashr', 'spade', 'spade')
w('W40', 'Engine pump signal to K5 coil', 'J1', 'K', '18', 'pink', 0.2, 'engr', 'dt', 'spade', 'Confirm the ECM drives the pump on and off.')
w('W41', 'Tach signal', 'J1', 'TACH', '18', 'gray', 0.05, 'engdash', 'dt', 'ring', 'Confirm the signal from the service manual.')
w('W42', 'Temperature sender signal', 'CTS', 'TEMP', '18', 'green', 0.05, 'engdash', 'dt', 'ring', 'Separate two-wire sender. The ECM keeps its own sensor.')
w('W43', 'Temperature sender return', 'CTS', 'TEMP', '18', 'brown', 0.05, 'engdash', 'dt', 'ring')
w('W44', 'Gas sender signal', 'SENDER', 'GAS', '18', 'tan', 0.05, 'bowdash', 'dt', 'ring')
w('W45', 'Gas sender return', 'SENDER', 'GAS', '18', 'brown', 0.05, 'bowdash', 'dt', 'ring')
w('W46', 'Exhaust probe, type K extension', 'EGT', 'EGTG', '18', 'K yellow', 0.0, 'engdash', 'tc', 'tc', 'Thermocouple pair. Its red conductor is type K negative, not power.')
for i, c in enumerate(['white', 'white/blue', 'white/green']):
    w('W%02d' % (47 + i), 'DESS post to ECM, wire %d' % (i + 1), 'DESS', 'J1', '18', c, 0.05, 'keyeng', 'dt', 'dt', 'Sled DESS post used as the lanyard kill. Colors to be matched to the sled harness.')
w('W50', 'Searchlight remote cable', 'JOY', 'SEARCH', '18', 'gray', 0.1, 'joy', 'oem', 'oem', 'Supplied with the searchlight. Length to be checked.')

for k, (name, amps, awg) in enumerate([('K1 accessory', 3.5, '18'), ('K2 navigation', 1, '18'), ('K3 LED bar', 10.3, '12'), ('K4 searchlight', 3.7, '18'), ('K5 fuel pump', 6, '12')]):
    w('W%02d' % (51 + k), 'Hub fuse to relay ' + name, 'HUB', 'K', awg, 'red', amps, 'box', 'fork', 'spade')
w('W56', 'Relay coil returns (5, daisy chain)', 'K', 'HUB', '18', 'black', 1, 'box', 'spade', 'ring')

ROUTE_ALIAS = {
    'pod': lambda a, b: [],
    'keyeng': lambda a, b: list(reversed(ROUTES['dash'])) + [(84, -1, 11)] + ROUTES['eng'],
    'dashr': lambda a, b: list(reversed(ROUTES['dash'])),
    'engr': lambda a, b: list(reversed(ROUTES['eng'])),
    'engdash': lambda a, b: list(reversed(ROUTES['eng'])) + [(84, -1, 11)] + ROUTES['dash'],
    'bowdash': lambda a, b: list(reversed(ROUTES['bow'])) + ROUTES['dash'],
    'joy': lambda a, b: list(reversed(ROUTES['dash'])) + [(84, -1, 11)] + ROUTES['cage'],
}


def path(x, slot):
    a, b = P[x['a']], P[x['b']]
    r = x['route']
    pts = ROUTE_ALIAS[r](a, b) if r in ROUTE_ALIAS else list(ROUTES[r])
    if a[0] < 60 and r in ('eng', 'cage', 'bilge', 'bow', 'dash'):
        pts = list(reversed(pts))
    if r in ('bow', 'dash') and a[0] > 95:
        pts = list(reversed(ROUTES[r]))
    if r == 'cage' and a[0] < 60:
        pts = list(reversed(ROUTES[r]))
    if r == 'bilge' and a[0] < 60:
        pts = list(reversed(ROUTES[r]))
    if r == 'eng' and a[0] < 60:
        pts = list(reversed(ROUTES[r]))
    # Fan each wire out in the harness so the bundle reads as separate wires.
    n = 5
    dy = (slot % n - 2) * 0.1
    dz = ((slot // n) % n - 2) * 0.1
    mid = [(p[0], p[1] + dy, p[2] + dz) for p in pts]
    x['head'] = [a] + WAY.get(x['a'], [])
    x['tail'] = list(reversed(WAY.get(x['b'], []))) + [b]
    x['base'] = tuple(pts)
    x['off'] = (dy, dz)
    return x['head'] + mid + x['tail']


def dist(a, b):
    return math.sqrt(sum((a[i] - b[i]) ** 2 for i in range(3)))


for i, x in enumerate(W):
    x['pts'] = path(x, i)
    x['in'] = sum(dist(x['pts'][k], x['pts'][k + 1]) for k in range(len(x['pts']) - 1)) + 6
    x['ft'] = x['in'] / 12
    ohm = AWG[x['awg']][0]
    x['lb'] = x['ft'] * AWG[x['awg']][1]
    # Round-trip drop: the pair is the same length and size.
    x['drop'] = 2 * x['ft'] * ohm * x['amps']
    x['pct'] = 100 * x['drop'] / VOLTS

FUSES = [('MF1', 60, 'Main feed, at the battery', 'W03'), ('F1', 20, '12 V outlet', 'W05'), ('F2', 15, 'LED bar (K3)', 'W07'), ('F3', 10, 'Searchlight (K4)', 'W09'),
         ('F4', 10, 'Navigation lights (K2)', 'W11, W13'), ('F5', 10, 'Dash switch bus: key, lights, searchlight', 'W15'), ('F6', 10, 'Accessory bus: gauges, USB (K1)', 'W16'),
         ('F7', 10, 'Bilge pump', 'W18'), ('F8', 15, 'Fuel pump (K5)', 'W20')]
CONN = {'ring': 'Ring terminal, sealed', 'fork': 'Fuse block screw, ring', 'spade': '1/4 in female spade, sealed', 'dt': 'Deutsch DT', 'tc': 'Type K mini plug', 'oem': 'As supplied'}


def write_csv():
    with open(os.path.join(HERE, 'Wire_List.csv'), 'w', newline='', encoding='utf-8') as f:
        c = csv.writer(f)
        c.writerow(['Wire', 'Function', 'From', 'To', 'AWG', 'Color', 'Amps', 'Length ft', 'Drop V (pair)', 'Drop %', 'End A', 'End B', 'Note'])
        for x in W:
            c.writerow([x['id'], x['name'], x['a'], x['b'], x['awg'], x['color'], x['amps'], round(x['ft'], 1), round(x['drop'], 2), round(x['pct'], 1), CONN[x['ca']], CONN[x['cb']], x['note']])


# ---- Schematic: one HTML page per sheet, 17 x 11 in landscape ----
SW = 1540
SH = 980


def line(x1, y1, x2, y2, c, wd=2.4):
    return f'<line x1="{x1}" y1="{y1}" x2="{x2}" y2="{y2}" stroke="{COLORS[c]}" stroke-width="{wd}"/>'


def poly(pts, c, wd=2.4):
    return f'<polyline fill="none" stroke="{COLORS[c]}" stroke-width="{wd}" points="' + ' '.join(f'{p[0]},{p[1]}' for p in pts) + '"/>'


def text(x, y, s, size=13, anchor='start', weight='normal', fill='#111'):
    return f'<text x="{x}" y="{y}" font-size="{size}" text-anchor="{anchor}" font-weight="{weight}" fill="{fill}">{html.escape(s)}</text>'


def box(x, y, w_, h, label='', sub='', fill='#f4f5f7'):
    s = f'<rect x="{x}" y="{y}" width="{w_}" height="{h}" fill="{fill}" stroke="#222" stroke-width="1.4" rx="3"/>'
    if label:
        s += text(x + w_ / 2, y + h / 2 + (0 if sub else 5), label, 13, 'middle', 'bold')
    if sub:
        s += text(x + w_ / 2, y + h / 2 + 15, sub, 11, 'middle')
    return s


def tag(x, y, wire, size=''):
    t = wire + (' ' + size if size else '')
    wd = 7.2 * len(t) + 8
    return f'<rect x="{x - wd / 2}" y="{y - 10}" width="{wd}" height="16" fill="#fff" stroke="#999" stroke-width=".7"/>' + text(x, y + 3, t, 11, 'middle')


def titleblock(n, title):
    return (f'<rect x="10" y="10" width="{SW - 20}" height="{SH - 20}" fill="none" stroke="#222" stroke-width="2"/>'
            + text(30, 42, f'Minimal 1448 airboat electrical schematic: {title}', 22, 'start', 'bold')
            + text(SW - 30, 42, f'Sheet {n} of 4   v1.0.0.4 draft', 14, 'end')
            + text(30, 62, 'Generated from electrical/wire_list.py. Red is power, black is return, other colors are signals or sensors. Chassis ground only inside the engine.', 13))


def sheet1():
    s = titleblock(1, 'power distribution')
    s += box(60, 110, 150, 110, 'BATTERY', '12 V lithium, dry box', '#fff4cf')
    s += text(70, 135, '+', 18, 'start', 'bold') + text(190, 135, '-', 18, 'start', 'bold')
    # Starter and engine
    s += line(95, 110, 95, 88, 'red', 5) + line(95, 88, 860, 88, 'red', 5) + tag(300, 88, 'W01', '1/0') + box(380, 78, 96, 20, 'fusible link', '', '#ffe2e2')
    s += box(860, 76, 170, 56, 'STARTER SOLENOID', 'B+ stud, engine', '#e9eefc')
    s += line(1030, 104, 1110, 104, 'gray') + box(1110, 76, 180, 56, 'Engine charging', 'regulator output, confirm', '#e9eefc')
    s += line(175, 220, 175, 248, 'black', 5) + line(175, 248, 860, 248, 'black', 5) + tag(420, 248, 'W02', '1/0')
    s += box(860, 226, 170, 44, 'ENGINE GROUND STUD', '', '#e9eefc') + text(1040, 252, 'Chassis ground starts here. Used only inside the engine.', 12)
    s += line(860, 132, 945, 132, 'gray', 0.1)
    # Hub
    s += box(60, 300, 170, 640, '', '', '#eef6ee') + text(145, 322, 'POWER HUB', 14, 'middle', 'bold') + text(145, 338, 'fuse block, negative bus', 11, 'middle')
    s += poly([(120, 220), (120, 262), (40, 262), (40, 360), (60, 360)], 'red', 3.5) + tag(84, 262, 'W03', '6') + box(26, 300, 28, 20, '', '', '#ffe2e2') + text(30, 335, 'MF1', 11, 'start') + text(16, 350, '60 A', 11, 'start')
    s += poly([(150, 220), (150, 276), (250, 276), (250, 924), (230, 924)], 'black', 3.5) + tag(250, 300, 'W04', '6')
    rows = [('F1', '20 A', None, 'W05', '12', 'W06', '12 V OUTLET', 'always on'), ('F2', '15 A', 'K3', 'W07', '12', 'W08', 'LED BAR', '10.3 A, cage top'),
            ('F3', '10 A', 'K4', 'W09', '18', 'W10', 'SEARCHLIGHT', '3.7 A, cage top'), ('F4', '10 A', 'K2', 'W11', '18', 'W12', 'STERN LIGHT', 'all-round white'),
            ('F4', '10 A', 'K2', 'W13', '18', 'W14', 'BOW LIGHT', 'red and green'), ('F5', '10 A', None, 'W15', '18', '', 'DASH SWITCH BUS', 'TB1: key, lights, searchlight'),
            ('F6', '10 A', 'K1', 'W16', '18', 'W17', 'DASH ACCESSORY BUS', 'TB2: gauges, USB'), ('F7', '10 A', None, 'W18', '18', 'W19', 'BILGE PUMP', 'automatic'),
            ('F8', '15 A', 'K5', 'W20', '12', 'W21', 'FUEL PUMP', 'sled module in tank')]
    y = 372
    for f, a, k, wr, awg, wb, dev, sub in rows:
        s += box(232, y - 12, 66, 24, f'{f} {a}', '', '#ffe2e2')
        s += line(298, y, 420, y, 'red', 3 if awg == '12' else 2)
        if k:
            s += box(420, y - 14, 70, 28, k + ' 30-87', '', '#fff')
        else:
            s += line(420, y, 490, y, 'red', 3 if awg == '12' else 2)
        s += line(490, y, 1090, y, 'red', 3 if awg == '12' else 2) + tag(760, y, wr, awg)
        s += box(1090, y - 22, 300, 44, dev, sub, '#e9eefc')
        if wb:
            yb = y + 16
            s += poly([(1090, yb), (950, yb), (950, y + 28), (262, y + 28), (262, 924), (230, 924)], 'black', 3 if awg == '12' else 2) + tag(1010, yb, wb, awg)
        y += 60
    s += text(1090, y + 2, 'Relay contacts sit in the dry box. Coils are on sheet 2.', 12)
    return s


def sheet2():
    s = titleblock(2, 'switching and signals')
    # Bus
    s += box(60, 110, 150, 44, 'TB1 switch bus', 'from F5 (W15)', '#fde6e6')
    s += line(135, 154, 135, 760, 'red', 3)
    y = 200
    # Key
    s += box(330, y - 30, 220, 150, '', '', '#fff') + text(440, y - 10, 'KEY SWITCH', 14, 'middle', 'bold') + text(440, y + 6, 'OFF 12:00, ON 1:30, START 3:00 (spring back)', 10, 'middle')
    s += line(135, y + 30, 330, y + 30, 'red') + tag(230, y + 30, 'W22', '18') + text(340, y + 34, 'B', 13)
    s += text(535, y + 34, 'ON', 12, 'end') + text(535, y + 84, 'START', 12, 'end')
    s += line(550, y + 30, 1000, y + 30, 'orange') + tag(770, y + 30, 'W35', '18')
    s += box(1000, y + 8, 110, 44, 'K1 coil', 'accessory', '#fff') + line(1110, y + 30, 1250, y + 30, 'black') + text(1260, y + 34, 'Coil return W56 to hub', 12)
    s += poly([(920, y + 30), (920, y - 18), (1000, y - 18)], 'orange') + box(1000, y - 40, 170, 44, 'ENGINE J1', 'ignition on, confirm', '#e9eefc')
    s += line(550, y + 80, 1000, y + 80, 'purple') + tag(770, y + 80, 'W36', '18') + box(1000, y + 58, 170, 44, 'SOLENOID coil S', 'returns inside engine', '#e9eefc')
    y = 400
    s += box(330, y - 30, 220, 150, '', '', '#fff') + text(440, y - 10, 'LIGHTS SWITCH', 14, 'middle', 'bold') + text(440, y + 6, 'rotary: OFF, RUN, LED', 11, 'middle')
    s += line(135, y + 30, 330, y + 30, 'red') + tag(230, y + 30, 'W23', '18') + text(340, y + 34, 'COM', 13)
    s += text(535, y + 34, 'RUN+LED', 12, 'end') + text(535, y + 84, 'LED', 12, 'end')
    s += line(550, y + 30, 1000, y + 30, 'blue') + tag(770, y + 30, 'W37', '18') + box(1000, y + 8, 110, 44, 'K2 coil', 'navigation', '#fff') + line(1110, y + 30, 1250, y + 30, 'black') + text(1260, y + 34, 'W56', 12)
    s += line(550, y + 80, 1000, y + 80, 'violet') + tag(770, y + 80, 'W38', '18') + box(1000, y + 58, 110, 44, 'K3 coil', 'LED bar', '#fff') + line(1110, y + 80, 1250, y + 80, 'black') + text(1260, y + 84, 'W56', 12)
    y = 590
    s += box(330, y - 22, 220, 60, 'SEARCHLIGHT SWITCH', 'on/off', '#fff')
    s += line(135, y + 8, 330, y + 8, 'red') + tag(230, y + 8, 'W24', '18') + line(550, y + 8, 1000, y + 8, 'yellow') + tag(770, y + 8, 'W39', '18')
    s += box(1000, y - 14, 110, 44, 'K4 coil', 'searchlight', '#fff') + line(1110, y + 8, 1250, y + 8, 'black') + text(1260, y + 12, 'W56', 12)
    y = 680
    s += box(330, y - 22, 220, 60, 'ENGINE J1', 'pump output, confirm', '#e9eefc') + line(550, y + 8, 1000, y + 8, 'pink') + tag(770, y + 8, 'W40', '18')
    s += box(1000, y - 14, 110, 44, 'K5 coil', 'fuel pump', '#fff') + line(1110, y + 8, 1250, y + 8, 'black') + text(1260, y + 12, 'W56', 12)
    y = 770
    s += box(330, y - 22, 220, 60, 'LANYARD KILL', 'sled DESS post on the dash', '#fff')
    for i, (c, wr) in enumerate([('white', 'W47'), ('white/blue', 'W48'), ('white/green', 'W49')]):
        s += line(550, y - 6 + i * 14, 1000, y - 6 + i * 14, c, 3) + tag(640 + i * 90, y - 6 + i * 14, wr, '18')
    s += box(1000, y - 22, 170, 60, 'ENGINE J1 to ECM', 'confirm pinout', '#e9eefc')
    y = 860
    s += box(330, y - 22, 220, 60, 'SEARCHLIGHT JOYSTICK', 'supplied remote', '#fff') + line(550, y + 8, 1000, y + 8, 'gray', 3) + tag(770, y + 8, 'W50', 'cable') + box(1000, y - 14, 170, 44, 'SEARCHLIGHT', 'control input', '#e9eefc')
    return s


def sheet3():
    s = titleblock(3, 'gauges and sensors')
    s += box(60, 110, 170, 44, 'TB2 accessory bus', 'from K1 (W16, W17)', '#fde6e6')
    s += line(120, 154, 120, 840, 'red', 3) + line(170, 154, 170, 840, 'black', 3)
    rows = [('GAS GAUGE', 'W27', 'W28', [('tan', 'W44', 'Gas sender in tank signal'), ('brown', 'W45', 'Gas sender return')], 'GAS SENDER', 'tank, front deck'),
            ('TACH', 'W29', 'W30', [('gray', 'W41', 'Tach signal, confirm source')], 'ENGINE J1', 'tach output'),
            ('TEMPERATURE GAUGE', 'W31', 'W32', [('green', 'W42', 'Sender signal'), ('brown', 'W43', 'Sender return')], 'TEMP SENDER', 'two-wire, on coolant hose'),
            ('EXHAUST GAUGE', 'W33', 'W34', [('K yellow', 'W46', 'Type K extension')], 'EGT PROBE', 'in the tuned pipe')]
    y = 210
    for dev, r, b, sig, far, sub in rows:
        s += line(120, y, 430, y, 'red') + tag(280, y, r, '18') + line(170, y + 24, 430, y + 24, 'black') + tag(320, y + 24, b, '18')
        s += box(430, y - 26, 200, 76, dev, 'analog, panel lamp on + feed')
        for i, (c, wr, lab) in enumerate(sig):
            yy = y + i * 24
            s += line(630, yy, 1040, yy, c, 3) + tag(760, yy, wr, '18') + text(830, yy - 6, lab, 11)
        s += box(1040, y - 26, 220, 76, far, sub, '#e9eefc')
        y += 130
    s += line(120, y, 430, y, 'red') + tag(280, y, 'W25', '18') + line(170, y + 24, 430, y + 24, 'black') + tag(320, y + 24, 'W26', '18')
    s += box(430, y - 26, 200, 76, 'USB CHARGING PORT', 'dual, 5 V, rubber flap')
    return s


def sheet4():
    s = titleblock(4, 'wire and fuse lists')
    cols = [(24, 'Wire'), (66, 'Function'), (300, 'AWG'), (340, 'Color'), (420, 'Amps'), (462, 'Ft'), (500, 'Drop')]
    half = 28
    for page, xoff in ((0, 0), (1, 760)):
        for x, h in cols:
            s += text(x + xoff, 92, h, 11, 'start', 'bold')
        y = 108
        for r in W[page * half:(page + 1) * half]:
            s += f'<rect x="{xoff + 342 - 16}" y="{y - 9}" width="11" height="10" fill="{COLORS[r["color"]]}" stroke="#555" stroke-width=".5"/>'
            s += text(24 + xoff, y, r['id'], 11) + text(66 + xoff, y, r['name'][:40], 11) + text(300 + xoff, y, r['awg'], 11) + text(340 + xoff, y, r['color'], 11)
            s += text(420 + xoff, y, str(r['amps']), 11) + text(462 + xoff, y, f"{r['ft']:.1f}", 11)
            pair = r['color'] == 'red' and r['awg'] in ('12', '18')
            s += text(500 + xoff, y, f"{r['pct']:.1f}%" if pair else '', 11)
            y += 15.6
    y = 560
    s += text(24, y, 'Fuses', 14, 'start', 'bold')
    y += 20
    for f, a, what, wr in FUSES:
        s += text(24, y, f'{f}  {a} A  {what}  ({wr})', 12)
        y += 17
    y = 780
    s += text(24, y, 'Connectors', 14, 'start', 'bold')
    y += 20
    for t in ['Battery, solenoid, ground stud, fuse block: sealed ring terminals, adhesive heat shrink.', 'Relays and dash switches: sealed 1/4 in female spades.', 'Devices outside the dry box: Deutsch DT, 2-pin for a device pair. Engine J1: Deutsch DT 12-pin on the stand.', 'Gauges: ring terminals on the gauge studs. Exhaust probe: type K mini plug.']:
        s += text(24, y, t, 12)
        y += 17
    x = 800
    y = 600
    s += text(x, y, 'To confirm from the 800R service manual', 14, 'start', 'bold')
    y += 20
    for t in ['Engine J1 pinout: ignition on, tach, fuel pump output, DESS post.', 'Whether the ECM runs the pump on and off, which K5 assumes, or varies it.', 'Fuel pump current. 6 A is an estimate.', 'Charging output and where the regulator lands.', 'Solenoid coil current. 5 A is an estimate.', 'Whether the kept DESS post works as the lanyard kill.']:
        s += text(x, y, t, 12)
        y += 17
    return s


def write_html():
    pages = ''.join(f'<section><svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 {SW} {SH}" width="100%" font-family="Arial, Helvetica, sans-serif">{f()}</svg></section>' for f in (sheet1, sheet2, sheet3, sheet4))
    doc = f'''<!doctype html><html><head><meta charset="utf-8"><title>Minimal 1448 electrical schematic</title>
<style>@page {{ size: 17in 11in; margin: 0.15in; }} body {{ margin: 0; background: #fff; }} section {{ page-break-after: always; }} section:last-child {{ page-break-after: auto; }} svg {{ display: block; }}</style></head><body>{pages}</body></html>'''
    with open(os.path.join(HERE, 'Schematic.html'), 'w', encoding='utf-8') as f:
        f.write(doc)


def write_fs():
    routes = []
    wires = []
    fmt = lambda pts: '[' + ','.join('[%.1f,%.1f,%.1f]' % tuple(q) for q in pts) + ']'
    for x in W:
        if x['base'] not in routes:
            routes.append(x['base'])
        c = COLORS[x['color']]
        rgb = ','.join('%.2f' % (int(c[i:i + 2], 16) / 255) for i in (1, 3, 5))
        wires.append('["%s %s %s AWG %s",%s,[%s],%s,%d,%.1f,%.1f,%s]' % (x['id'], x['name'], x['awg'], x['color'], AWG[x['awg']][2], rgb, fmt(x['head']), routes.index(x['base']), x['off'][0], x['off'][1], fmt(x['tail'])))
    route_txt = ',\n'.join(fmt(r) for r in routes)
    wire_txt = ',\n'.join(wires)
    ends = {}
    for x in W:
        for key, kind in ((x['a'], x['ca']), (x['b'], x['cb'])):
            if key not in ('HUB', 'K', 'BAT+', 'BAT-'):
                ends.setdefault((key, kind), P[key])
    conns = ',\n'.join('    ["%s connector, %s", [%.1f, %.1f, %.1f], %s]' % (k[0], CONN[k[1]], v[0], v[1], v[2], {'dt': '0.32', 'ring': '0.24', 'spade': '0.16', 'tc': '0.2', 'oem': '0.22'}.get(k[1], '0.2')) for k, v in ends.items())
    src = f'''FeatureScript 3112;
import(path : "onshape/std/geometry.fs", version : "3112.0");

// Wiring of the minimal 1448, generated by electrical/wire_list.py. Do not edit by hand.
// Frame: X forward from the transom, Y + to port, Z up from the hull bottom. Each wire is one body,
// named with its wire number; each connector is a marker at its end.

// Harness routes, shared by the wires that follow them.
const ROUTES = [
{route_txt}
];

// name, radius, color, points from the first end, route, harness offset in Y and Z, points to the last end.
const WIRES = [
{wire_txt}
];

const CONNECTORS = [
{conns}
];

const DEVICES = [
    ["K1 to K5 relays", [88, 3.6, 6], 0.7],
    ["Starter solenoid", [31, -10.6, 15.2], 0.75],
    ["Engine J1, Deutsch DT 12-pin", [31, -10.6, 18.2], 0.5],
    ["Bow light", [157, 0, 17.6], 0.7],
    ["All-round stern light", [9, 11, 89.4], 0.7],
    ["Bilge pump", [8, 3, 1.1], 1.1],
    ["Temperature sender", [28, -6.5, 29], 0.4],
    ["Exhaust probe", [25.5, 9.3, 31], 0.3]
];

function mkCyl(context is Context, id is Id, a is Vector, b is Vector, r is number)
{{
    fCylinder(context, id, {{ "bottomCenter" : a * inch, "topCenter" : b * inch, "radius" : r * inch }});
}}

function paintBody(context is Context, q is Query, name is string, c is Color)
{{
    setProperty(context, {{ "entities" : q, "propertyType" : PropertyType.NAME, "value" : name }});
    setProperty(context, {{ "entities" : q, "propertyType" : PropertyType.APPEARANCE, "value" : c }});
}}

annotation {{ "Feature Type Name" : "Minimal wiring" }}
export const minimalWiring = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {{
    }}
    {{
        for (var i = 0; i < size(WIRES); i += 1)
        {{
            const w = WIRES[i];
            const wid = id + ("w" ~ i);
            var pts = w[3];
            for (var r in ROUTES[w[4]])
                pts = append(pts, [r[0], r[1] + w[5], r[2] + w[6]]);
            pts = concatenateArrays([pts, w[7]]);
            var joins = [];
            for (var k = 0; k < size(pts) - 1; k += 1)
            {{
                const a = vector(pts[k]);
                const b = vector(pts[k + 1]);
                if (norm(b - a) < 0.02)
                    continue;
                const d = normalize(b - a) * 0.02;
                const sid = wid + ("s" ~ k);
                mkCyl(context, sid, a - d, b + d, w[1]);
                opSphere(context, wid + ("j" ~ k), {{ "center" : b * inch, "radius" : w[1] * 1.001 * inch }});
                joins = append(joins, qCreatedBy(sid, EntityType.BODY));
                joins = append(joins, qCreatedBy(wid + ("j" ~ k), EntityType.BODY));
            }}
            opBoolean(context, wid + "u", {{ "tools" : qUnion(joins), "operationType" : BooleanOperationType.UNION }});
            paintBody(context, qCreatedBy(wid, EntityType.BODY), w[0], color(w[2][0], w[2][1], w[2][2]));
        }}
        for (var i = 0; i < size(CONNECTORS); i += 1)
        {{
            const c = CONNECTORS[i];
            opSphere(context, id + ("c" ~ i), {{ "center" : vector(c[1]) * inch, "radius" : c[2] * inch }});
            paintBody(context, qCreatedBy(id + ("c" ~ i), EntityType.BODY), c[0], color(0.12, 0.12, 0.14));
        }}
        for (var i = 0; i < size(DEVICES); i += 1)
        {{
            const c = DEVICES[i];
            fCuboid(context, id + ("d" ~ i), {{ "corner1" : (vector(c[1]) - vector(c[2], c[2], c[2])) * inch, "corner2" : (vector(c[1]) + vector(c[2], c[2], c[2])) * inch }});
            paintBody(context, qCreatedBy(id + ("d" ~ i), EntityType.BODY), c[0], color(0.3, 0.32, 0.36));
        }}
    }});
'''
    with open(os.path.join(HERE, '..', 'CAD', 'FeatureScript', 'Minimal_wiring.fs'), 'w', encoding='utf-8', newline='\n') as f:
        f.write(src)
    return len(src)


if __name__ == '__main__':
    write_csv()
    write_html()
    n = write_fs()
    print('wires', len(W), 'fs bytes', n)
    tot = {}
    for x in W:
        tot[x['awg']] = tot.get(x['awg'], 0) + x['ft']
    print({k: round(v, 1) for k, v in tot.items()})
    print('wire lb', round(sum(x['lb'] for x in W), 1), ' 1/0 lb', round(sum(x['lb'] for x in W if x['awg'] == '1/0'), 1))
    for x in W:
        if x['color'] == 'red' and x['awg'] in ('12', '18') and x['id'] <= 'W21':
            print(x['id'], x['name'], x['awg'], x['amps'], 'A', round(x['ft'], 1), 'ft', round(x['drop'], 2), 'V', round(x['pct'], 1), '%')
    # What each pair would do at 18 AWG, for the sizing call.
    for x in W:
        if x['id'] in ('W07', 'W20'):
            print(x['id'], 'at 18 AWG', round(2 * x['ft'] * AWG['18'][0] * x['amps'], 2), 'V', round(100 * 2 * x['ft'] * AWG['18'][0] * x['amps'] / VOLTS, 1), '%')
