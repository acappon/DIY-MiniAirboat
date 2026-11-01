# Body survey of the split assembly. Paths are rewritten to the old single-design subassembly names
# (hull_r4-2_aluminum, drivetrain_r5_G13, ...) so calc/mass.py's grouping and rules work unchanged.
import json, re
OUT = r'C:\Users\acappon\AppData\Local\Temp\fmcp\eng\bodies.json'
MAP = {'Hull': 'hull_r4-2_aluminum:1', 'Drivetrain': 'drivetrain_r5_G13:1', 'Flotation': 'flotation_r5_aft72:1',
       'Cage and rudders': 'cage_rudders_r2:1', 'Layout': 'layout_r6:1', 'Render people': 'layout_r6:1+people_for_renders:1'}


def run(_context: str):
    assert APP.activeDocument.name.startswith('Airboat rev7 - Assembly'), APP.activeDocument.name
    rows = []
    acc = adsk.fusion.CalculationAccuracy.LowCalculationAccuracy

    def fix(path):
        head, _, rest = path.partition('+')
        m = re.match(r'Airboat rev7 - (.+?)(?: v\d+)?:\d+$', head)
        new = MAP.get(m.group(1), head) if m else head
        return new + ('+' + rest if rest else '')

    def walk(occs):
        for o in occs:
            if not o.isLightBulbOn:
                continue
            for b in o.bRepBodies:
                if not b.isLightBulbOn or not b.isSolid:
                    continue
                pp = b.getPhysicalProperties(acc)
                c = pp.centerOfMass
                bb = b.boundingBox
                rows.append(dict(path=fix(o.fullPathName), body=b.name, vol=pp.volume, mass=pp.mass, mat=b.material.name if b.material else '',
                                 c=[c.x * 10, -c.z * 10, c.y * 10],
                                 lo=[bb.minPoint.x * 10, -bb.maxPoint.z * 10, bb.minPoint.y * 10],
                                 hi=[bb.maxPoint.x * 10, -bb.minPoint.z * 10, bb.maxPoint.y * 10]))
            walk(o.childOccurrences)
    walk(ROOT.occurrences)
    json.dump(rows, open(OUT, 'w'), indent=0)
    heads = {}
    for r in rows:
        k = r['path'].split('+')[0]
        heads[k] = heads.get(k, 0) + 1
    print(len(rows), 'bodies', heads)
