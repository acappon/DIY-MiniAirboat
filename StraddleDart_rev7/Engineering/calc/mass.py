"""Weight and centre of gravity of the rev 7 airboat from the Fusion model.

Every visible solid body in data/bodies.json (exported by fusion_survey_bodies.py) gets a
weight: placeholders and bought items get a lumped weight from the BOM, everything else
volume x density by material. Coordinates are model mm: x forward from the transom,
y to port, z up from the hull bottom.
"""
import json, os, re

HERE = os.path.dirname(os.path.abspath(__file__))
LB = 453.592  # g per lb

# (regex on 'path|body', lumped lb for the matched group or None, density g/cm3)
# First match wins. A lump is shared among all matching bodies by volume.
RULES = [
    (r'ENGINE_G13B?_placeholder', 179 + 7 - 23 - 7 + 15, None),  # BOM dressed 179 lb incl. flywheel + starter, + 3.5 qt oil; +15 lb G13B twin-cam head (estimate)
    (r'starter_placeholder', 7, None),
    (r'radiator_Civic_approx', 20, None),          # incl. coolant (BOM)
    (r'PROP_WarpDrive', 18, None),
    (r'muffler_envelope', 10, None),
    (r'belt_HTD8M', 3, None),
    (r'FUEL_TANK', 4.5, None),                     # empty; fuel is a load item
    (r'battery_group31', 68, None),
    (r'bench_cushion', 4, None),
    (r'backrest_removable', 5, None),              # each component matched separately below
    (r'armrest_pad', 1, None),
    (r'cage_light_bar', 10, None),
    (r'searchlight_control', 1, None),
    (r'searchlight_cables', 2, None),
    (r'\+searchlight:', 9, None),
    (r'safety_gear', 6, None),
    (r'BILGE_PUMP', 2.5, None),
    (r'engine_front_guard_mesh', 5, None),
    (r'deck_plate_6in', 0.45, None),              # each (BOM 3.6 lb for 8)
    (r'power_distribution', 1, None),
    (r'sump_grate_FRP', 2.5, None),
    (r'driver_dash_hood:1\|(?!dash_hood|dash_panel)', 3.0, None),   # gauges, switches, sockets (catalog)
    (r'knee_brace_pad', None, 0.08),                # closed-cell foam + coated Cordura cover
    (r'phone_cradle|phone_swivel', 0.4, None),
    (r'RUDDER_.*honeycomb', None, 0.354),         # 11.9 lb for both panels (BOM)
    (r'MESH_', None, 1.03),                       # 17.3 lb of hardware cloth on 0.04 in sheets
    (r'FOAM_|foam_bow|FOAM_bow', None, 0.032),     # 2 lb/ft3 urethane
    (r'uhmw|UHMW', None, 0.94),
    (r'hdpe|HDPE|isolator|grommet', None, 0.95),
    (r'poly_bushing|flex_coupling|rubber|gasket', None, 1.2),
    (r'ply|plywood|bench_box|bow_shelf_1-2|bulkhead|battery_bay_lid|cooler_lid', None, 0.60),
    (r'harness|wire_|cable|_1-0|loom|conduit', None, 3.0),  # copper + insulation
    (r'cage_hoop|cage_legs|cage_base|cage_side_diagonal|grip_base_plate|cage_foot_pads|EMT|shaft_guard|rudder_rails|bolt|steel|_SS|'
     r'engine_bracket|prop_shaft|jackshaft_1|flywheel|pulley_HTD8M_30T|bearing_flange|jack_screw|'
     r'exhaust|tailpipe|radiator_top_stays|steering_pushrod|rudder_tie_bar|grip|shifter_stick|ubolt', None, 7.85),
    (r'.', None, 2.70),                            # aluminium by default
]
RX = [(re.compile(p), lump, rho) for p, lump, rho in RULES]

GROUPS = [  # report groups (first match on path)
    ('Hull', r'^hull_r4-2'), ('Drivetrain', r'^drivetrain'), ('Cage and rudders', r'^cage_rudders'),
    ('Flotation, floor, sump', r'^flotation'), ('Seating, controls, electrical, lights', r'^layout'),
]

# Not modelled in CAD: rivets, sealant, paint, fasteners, wiring sundries (BOM 'N' lines), placed
# at the hull's own centre of gravity.
NOT_IN_CAD_LB = 4.5 + 4 + 3 + 1 + 3 + 1 + 5 + 2 + 2   # rivets 2,250; primer only (hull left bare, 2026-10-31)

# Load items: lb, (x, y, z) mm
GAS_LB_PER_GAL = 6.1
PERSON_LB = 200.0
DRIVER = (2290.0, 0.0, 675.0)      # seated 8 in above the 18.5 in cushion, 8 in ahead of the backrest
PASSENGER = (1620.0, 0.0, 675.0)


def bodies():
    return json.load(open(os.path.join(HERE, 'data', 'bodies.json')))


def weigh(rows=None):
    rows = rows or bodies()
    # lumps are shared by volume within the occurrence path that matched
    rule_of, lump_vol = [], {}
    for r in rows:
        key = r['path'] + '|' + r['body']
        for i, (rx, lump, rho) in enumerate(RX):
            if rx.search(key):
                grp = (i, r['path'].split('+')[1] if lump is not None and '+' in r['path'] else '')
                rule_of.append((i, grp))
                if lump is not None:
                    lump_vol[grp] = lump_vol.get(grp, 0) + r['vol']
                break
    items = []
    for r, (i, grp) in zip(rows, rule_of):
        _, lump, rho = RX[i]
        lb = lump * r['vol'] / lump_vol[grp] if lump is not None else r['vol'] * rho / LB
        items.append(dict(r, lb=lb, rule=RULES[i][0]))
    return items


def cg(items):
    w = sum(i['lb'] for i in items)
    return w, [sum(i['lb'] * i['c'][k] for i in items) / w for k in range(3)]


def group_of(item):
    for g, p in GROUPS:
        if re.search(p, item['path']):
            return g
    return 'Other'


def lightship():
    """Empty boat (no fuel, crew or gear) as one weight item list incl. the not-in-CAD allowance."""
    items = weigh()
    hull = [i for i in items if group_of(i) == 'Hull']
    _, hc = cg(hull)
    items.append(dict(path='not_in_CAD', body='rivets, sealant, paint, fasteners', lb=NOT_IN_CAD_LB, c=hc,
                      lo=hc, hi=hc, vol=0, rule='allowance'))
    return items


def load_case(name):
    """Weight items for a named load case."""
    items = lightship()
    tank = [i for i in items if 'FUEL_TANK' in i['path']][0]
    def add(n, lb, c):
        items.append(dict(path='load', body=n, lb=lb, c=list(c), lo=list(c), hi=list(c), vol=0, rule='load'))
    if 'full fuel' in name:
        add('fuel 6.6 gal', 6.6 * GAS_LB_PER_GAL, tank['c'])
    if '1 gal' in name:
        add('fuel 1 gal', GAS_LB_PER_GAL, (tank['c'][0], tank['c'][1], tank['lo'][2] + 20))
    if 'driver' in name or 'solo' in name:
        add('driver', PERSON_LB, DRIVER)
    if 'passenger' in name:
        add('passenger', PERSON_LB, PASSENGER)
    if 'cargo' in name:
        add('cargo 100 lb on the bow shelf', 100, (tank['c'][0] - 150, -250, tank['lo'][2] + 100))
    return items


if __name__ == '__main__':
    items = weigh()
    tot = {}
    for i in items:
        g = group_of(i)
        tot.setdefault(g, []).append(i)
    for g, its in tot.items():
        w, c = cg(its)
        print('%-40s %7.1f lb  LCG %6.1f in  VCG %5.1f in' % (g, w, c[0] / 25.4, c[2] / 25.4))
    w, c = cg(lightship())
    print('%-40s %7.1f lb  LCG %6.1f in  TCG %5.2f in  VCG %5.1f in' % ('Empty boat incl. allowance', w, c[0] / 25.4, c[1] / 25.4, c[2] / 25.4))
    big = sorted(items, key=lambda i: -i['lb'])[:25]
    for i in big:
        print('  %6.1f lb  %-45s %s' % (i['lb'], (i['path'].split('+')[-1])[:45], i['rule'][:30]))
