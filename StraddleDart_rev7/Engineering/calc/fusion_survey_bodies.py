import json
OUT = r'C:\Users\acappon\AppData\Local\Temp\fmcp\eng\bodies.json'
def run(_context: str):
    rows = []
    acc = adsk.fusion.CalculationAccuracy.LowCalculationAccuracy
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
                mat = b.material.name if b.material else ''
                rows.append(dict(path=o.fullPathName, body=b.name, vol=pp.volume, mass=pp.mass, mat=mat,
                                 c=[c.x * 10, -c.z * 10, c.y * 10],
                                 lo=[bb.minPoint.x * 10, -bb.maxPoint.z * 10, bb.minPoint.y * 10],
                                 hi=[bb.maxPoint.x * 10, -bb.minPoint.z * 10, bb.maxPoint.y * 10]))
            walk(o.childOccurrences)
    for b in ROOT.bRepBodies:
        pass
    walk(ROOT.occurrences)
    json.dump(rows, open(OUT, 'w'), indent=0)
    print(len(rows), 'bodies; total CAD mass %.0f kg' % sum(r['mass'] for r in rows))
