# Adds simple dial markings to the three white dash gauges so each reads as what it is in the
# renders: tach 0-8 with a red zone, fuel E-F, water temp C-H, each with a red needle. Numbers and
# letters are seven-segment bars, which stay upright on the tilted dash without sketch text.
# Run it in the design that holds the gauge bodies (the Layout design), after WhiteGauges.
import adsk.core, adsk.fusion, math, traceback

MARK_H = 0.05    # cm proud of the dial face for ticks and figures
NEEDLE_H = 0.10  # cm for the needle and hub

# Seven-segment figures: segments a (top), b (top right), c (bottom right), d (bottom),
# e (bottom left), f (top left), g (middle).
SEGS = {'0': 'abcdef', '1': 'bc', '2': 'abged', '3': 'abgcd', '4': 'fgbc', '5': 'afgcd',
        '6': 'afgedc', '7': 'abc', '8': 'abcdefg', 'C': 'afed', 'E': 'afged', 'F': 'afge', 'H': 'bcefg'}

# Angles are degrees clockwise from straight up, seen from the driver.
DIALS = {
    'tachometer_electric': dict(sweep=(-135, 135), majors=9, minors=1, red_from=6.5,
                                labels=[(str(i), i) for i in range(9)], label_r=0.62, label_h=0.15,
                                needle=2.5),
    'fuel_gauge': dict(sweep=(-60, 60), majors=5, minors=0, red_upto=0,
                       labels=[('E', 0), ('F', 4)], label_r=0.50, label_h=0.22, needle=3.0),
    'water_temp_gauge': dict(sweep=(-60, 60), majors=5, minors=0, red_from=4,
                             labels=[('C', 0), ('H', 4)], label_r=0.50, label_h=0.22, needle=1.8),
}

PAINTS = {'black': ('Paint - Enamel Glossy (Black)', (20, 20, 20)),
          'red': ('Paint - Enamel Glossy (Red)', (200, 20, 20))}


def by_name(appearances, name):
    try:
        return appearances.itemByName(name)
    except Exception:
        return None


def find_paint(app, design, colour):
    name, rgb = PAINTS[colour]
    have = by_name(design.appearances, name)
    if have:
        return have
    libs = [lib for lib in app.materialLibraries]
    for lib in libs:
        a = by_name(lib.appearances, name)
        if a:
            return design.appearances.addByCopy(a, name)
    for lib in libs:
        try:
            for a in lib.appearances:
                n = a.name.lower()
                if colour in n and 'paint' in n and 'gloss' in n:
                    return design.appearances.addByCopy(a, name)
        except Exception:
            pass
    if design.appearances.count:
        a = design.appearances.addByCopy(design.appearances.item(0), name)
        for p in a.appearanceProperties:
            c = adsk.core.ColorProperty.cast(p)
            if c:
                try:
                    c.value = adsk.core.Color.create(rgb[0], rgb[1], rgb[2], 255)
                except Exception:
                    pass
        return a
    return None


def centre_of(face):
    return face.centroid


def normal_of(face):
    ok, n = face.evaluator.getNormalAtPoint(face.pointOnFace)
    return n


def radius_of(face):
    r = 0
    for e in face.edges:
        c = adsk.core.Circle3D.cast(e.geometry)
        if c:
            r = max(r, c.radius)
    return r or math.sqrt(face.area / math.pi)


def front_face(body, toward):
    # The dial is the big round flat face that points toward the driver.
    flats = [f for f in body.faces if adsk.core.Plane.cast(f.geometry) and
             any(adsk.core.Circle3D.cast(e.geometry) for e in f.edges)]
    if not flats:
        return None
    big = max(f.area for f in flats)
    flats = [f for f in flats if f.area >= 0.6 * big]

    def score(f):
        v = centre_of(f).vectorTo(toward)
        v.normalize()
        return normal_of(f).dotProduct(v)
    return max(flats, key=score)


def find_body(design, name):
    for comp in design.allComponents:
        for b in comp.bRepBodies:
            if b.name == name:
                return comp, b
    return None, None


class Frame:
    """2D frame on the dial in sketch space: origin at the centre, u to the right, v up."""

    def __init__(self, sketch, face, up):
        c = centre_of(face)
        n = normal_of(face)
        up = up.copy()
        k = up.dotProduct(n)
        up = adsk.core.Vector3D.create(up.x - k * n.x, up.y - k * n.y, up.z - k * n.z)
        if up.length < 1e-6:
            up = adsk.core.Vector3D.create(1, 0, 0)
        up.normalize()
        right = up.crossProduct(n)
        s0 = sketch.modelToSketchSpace(c)

        def to_sketch(vec):
            p = c.copy()
            p.translateBy(vec)
            q = sketch.modelToSketchSpace(p)
            d = adsk.core.Vector3D.create(q.x - s0.x, q.y - s0.y, 0)
            d.normalize()
            return d
        self.o = s0
        self.u = to_sketch(right)
        self.v = to_sketch(up)

    def pt(self, x, y):
        return adsk.core.Point3D.create(self.o.x + x * self.u.x + y * self.v.x,
                                        self.o.y + x * self.u.y + y * self.v.y, 0)


def polygon(sketch, frame, pts):
    lines = sketch.sketchCurves.sketchLines
    sp = [frame.pt(x, y) for x, y in pts]
    first = lines.addByTwoPoints(sp[0], sp[1])
    last = first
    for p in sp[2:]:
        last = lines.addByTwoPoints(last.endSketchPoint, p)
    lines.addByTwoPoints(last.endSketchPoint, first.startSketchPoint)


def bar(sketch, frame, x0, y0, x1, y1, w):
    # A rectangle of width w from (x0, y0) to (x1, y1).
    dx, dy = x1 - x0, y1 - y0
    L = math.hypot(dx, dy)
    nx, ny = -dy / L * w / 2, dx / L * w / 2
    polygon(sketch, frame, [(x0 + nx, y0 + ny), (x1 + nx, y1 + ny), (x1 - nx, y1 - ny), (x0 - nx, y0 - ny)])


def radial(a_deg, r):
    a = math.radians(a_deg)
    return r * math.sin(a), r * math.cos(a)


def figure(sketch, frame, ch, cx, cy, h):
    W, t = 0.55 * h, 0.16 * h
    g = 0.45 * t  # gap so the bars never touch
    hl, vl = W / 2 - g, h / 4 - g
    seg = {'a': (0, h / 2, 'h'), 'g': (0, 0, 'h'), 'd': (0, -h / 2, 'h'),
           'b': (W / 2, h / 4, 'v'), 'c': (W / 2, -h / 4, 'v'),
           'f': (-W / 2, h / 4, 'v'), 'e': (-W / 2, -h / 4, 'v')}
    for s in SEGS[ch]:
        x, y, o = seg[s]
        if o == 'h':
            bar(sketch, frame, cx + x - hl, cy + y, cx + x + hl, cy + y, t)
        else:
            bar(sketch, frame, cx + x, cy + y - vl, cx + x, cy + y + vl, t)


def text(sketch, frame, s, cx, cy, h):
    step = 0.75 * h
    x = cx - step * (len(s) - 1) / 2
    for ch in s:
        figure(sketch, frame, ch, x, cy, h)
        x += step


def extrude(comp, sketch, face, height, appearance, name, max_area):
    profs = adsk.core.ObjectCollection.create()
    for p in sketch.profiles:
        if p.areaProperties().area < max_area:
            profs.add(p)
    if not profs.count:
        return 0
    ext = comp.features.extrudeFeatures

    def make(h):
        inp = ext.createInput(profs, adsk.fusion.FeatureOperations.NewBodyFeatureOperation)
        inp.setDistanceExtent(False, adsk.core.ValueInput.createByReal(h))
        return ext.add(inp)
    feat = make(height)
    # Make sure the marks stand out of the face, not into the gauge.
    c, n = centre_of(face), normal_of(face)
    mid = feat.bodies.item(0).getPhysicalProperties().centerOfMass
    if c.vectorTo(mid).dotProduct(n) < 0:
        feat.deleteMe()
        feat = make(-height)
    feat.name = name
    for i, b in enumerate(feat.bodies):
        b.name = f'{name}_{i + 1}'
        b.appearance = appearance
    return feat.bodies.count


def up_in(design, comp):
    # Model Y is up in these designs; carry it into the component's own space.
    up = adsk.core.Vector3D.create(0, 1, 0)
    occs = design.rootComponent.allOccurrencesByComponent(comp)
    if occs.count:
        m = occs.item(0).transform2.copy()
        m.invert()
        up.transformBy(m)
    return up


def dial(comp, face, spec, black, red, up):
    r = radius_of(face) * 0.92
    a0, a1 = spec['sweep']
    n = spec['majors'] - 1
    ang = lambda val: a0 + (a1 - a0) * val / n
    is_red = lambda val: val >= spec.get('red_from', 1e9) - 1e-9 or val <= spec.get('red_upto', -1e9) + 1e-9
    max_area = 0.3 * math.pi * r * r

    sk_b = comp.sketches.add(face)
    sk_r = comp.sketches.add(face)
    sk_b.name, sk_r.name = f'dial_{spec["key"]}_marks', f'dial_{spec["key"]}_red'
    fb, fr = Frame(sk_b, face, up), Frame(sk_r, face, up)
    sk_b.isComputeDeferred = sk_r.isComputeDeferred = True

    steps = n * (spec['minors'] + 1)
    for i in range(steps + 1):
        val = i / (spec['minors'] + 1)
        major = i % (spec['minors'] + 1) == 0
        a = ang(val)
        x0, y0 = radial(a, r * (0.80 if major else 0.87))
        x1, y1 = radial(a, r * 0.97)
        sk, fr_ = (sk_r, fr) if is_red(val) else (sk_b, fb)
        bar(sk, fr_, x0, y0, x1, y1, r * (0.035 if major else 0.018))
    for s, val in spec['labels']:
        cx, cy = radial(ang(val), r * spec['label_r'])
        text(sk_b, fb, s, cx, cy, r * spec['label_h'])

    # Needle from just behind the pivot to near the ticks, tapering, with a round hub.
    a = math.radians(ang(spec['needle']))
    dx, dy = math.sin(a), math.cos(a)
    px, py = -dy, dx
    tail, tip, wb, wt = -0.15 * r, 0.76 * r, 0.035 * r, 0.008 * r
    polygon(sk_r, fr, [(tail * dx + wb * px, tail * dy + wb * py), (tip * dx + wt * px, tip * dy + wt * py),
                       (tip * dx - wt * px, tip * dy - wt * py), (tail * dx - wb * px, tail * dy - wb * py)])
    sk_r.sketchCurves.sketchCircles.addByCenterRadius(fr.pt(0, 0), 0.08 * r)

    sk_b.isComputeDeferred = sk_r.isComputeDeferred = False
    nb = extrude(comp, sk_b, face, MARK_H, black, f'dial_{spec["key"]}_marks', max_area)
    nr = extrude(comp, sk_r, face, NEEDLE_H, red, f'dial_{spec["key"]}_needle', max_area)
    return nb + nr


def run(context):
    app = adsk.core.Application.get()
    ui = app.userInterface
    try:
        design = adsk.fusion.Design.cast(app.activeProduct)
        black, red = find_paint(app, design, 'black'), find_paint(app, design, 'red')
        if not (black and red):
            ui.messageBox('Could not find or make black and red paint appearances.')
            return
        _, visor = find_body(design, 'dash_hood_visor_5052_0-063')
        if visor:
            bb = visor.boundingBox
            toward = adsk.core.Point3D.create((bb.minPoint.x + bb.maxPoint.x) / 2, (bb.minPoint.y + bb.maxPoint.y) / 2,
                                              (bb.minPoint.z + bb.maxPoint.z) / 2)
        else:
            toward = app.activeViewport.camera.eye
        lines = []
        for key, spec in DIALS.items():
            spec = dict(spec, key=key)
            comp, body = find_body(design, key)
            if not body:
                lines.append(f'{key}: not found in this design')
                continue
            if comp.sketches.itemByName(f'dial_{key}_marks'):
                lines.append(f'{key}: already has dial markings (undo them to run again)')
                continue
            face = front_face(body, toward)
            if not face:
                lines.append(f'{key}: no round flat face found')
                continue
            try:
                n = dial(comp, face, spec, black, red, up_in(design, comp))
                lines.append(f'{key}: {n} bodies added, dial radius {radius_of(face) * 10:.0f} mm')
            except Exception:
                lines.append(f'{key}: failed\n{traceback.format_exc()}')
        msg = '\n'.join(lines)
        app.log('GaugeDials: ' + msg)
        ui.messageBox(msg)
    except Exception:
        app.log('GaugeDials failed:\n' + traceback.format_exc())
        ui.messageBox('Failed:\n' + traceback.format_exc())
