import json
OUT = r'C:\Users\acappon\AppData\Local\Temp\fmcp\eng\sections.json'
NAMES = ('hull_skin_aluminum', 'uhmw_strip_1', 'uhmw_strip_2', 'uhmw_strip_3', 'uhmw_strip_4',
         'chine_guard_angle_P', 'chine_guard_angle_S', 'nose_cap')
def run(_context: str):
    tbm = adsk.fusion.TemporaryBRepManager.get()
    hull = find_occ(ROOT, 'hull_r4-2_aluminum')
    bodies = []
    for o in hull.childOccurrences:
        for b in o.bRepBodies:
            if b.name in NAMES:
                bodies.append((b.name, tbm.copy(b)))
    print('bodies', [n for n, _ in bodies])
    bb = bodies[0][1].boundingBox
    print('skin copy bbox (world cm)', bb.minPoint.asArray(), bb.maxPoint.asArray())
    out = {}
    x = 5.0
    while x < 3810:
        pl = adsk.core.Plane.create(P3(x / 10, 0, 0), adsk.core.Vector3D.create(1, 0, 0))
        polys = []
        for n, b in bodies:
            try:
                w = tbm.planeIntersection(b, pl)
            except Exception:
                w = None
            if not w:
                continue
            for e in w.edges:
                ev = e.evaluator; _, p0, p1 = ev.getParameterExtents(); ok, pts = ev.getStrokes(p0, p1, 0.05)
                if ok and len(pts) > 1:
                    polys.append([n, [[-p.z * 10, p.y * 10] for p in pts]])
        out['%.0f' % x] = polys
        x += 25.0
    json.dump(out, open(OUT, 'w'))
    print('stations', len(out))
