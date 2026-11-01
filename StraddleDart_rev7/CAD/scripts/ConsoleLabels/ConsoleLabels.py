# Makes the dash switches and sockets light gray so they show against the black panel, and puts
# white labels on the panel below them (not the key): level, one word per line, in a column under
# each item. Letters are drawn as bars in a simple block font, the same way GaugeDials draws its
# figures, so they stay level and readable on the tilted panel. Run it in the Layout design.
import adsk.core, adsk.fusion, math, traceback

# Body -> label lines below it (None: gray only, no label).
ITEMS = {
    'kill_switch_lanyard': ['KILL'],
    'kill_switch_lanyard_clip': None,
    'key_switch': None,
    'trouble_light_red': ['CHECK', 'ENGINE'],  # stays red; label only
    'rocker_nav_off_anchor': ['NAV', 'LIGHTS'],
    'rotary_switch_light_bar_off_lo_med_hi': ['LIGHT', 'BAR'],
    'rotary_switch_searchlight_off_lo_med_hi': ['SEARCH', 'LIGHT'],
    'usb_a_charger_5v_3a': ['USB'],
    'usb_a_charger_5v_3a_rubber_flap': None,
    'socket_12v_cigarette': ['12V/20A'],
    'socket_12v_cigarette_rubber_flap': None,
}
KEEP_COLOUR = {'trouble_light_red'}
PANEL = 'dash_panel_aluminium'
VISOR = 'dash_hood_visor_5052_0-063'

CAP = 0.35        # cm, letter height
GAP = 1.60        # cm from the lowest switch centre down to the middle of the first label line
LINE = 1.6        # line spacing, in letter heights
RAISE = 0.03      # cm the letters stand proud of the panel
SKETCH = 'console_labels_level'
OLD_SKETCH = 'console_labels'  # from the first version of this script

# Block font on a 4 x 6 grid: each glyph is a list of polylines.
GLYPHS = {
    'A': [[(0, 0), (0, 5), (1, 6), (3, 6), (4, 5), (4, 0)], [(0, 3), (4, 3)]],
    'B': [[(0, 0), (0, 6), (3, 6), (4, 5), (4, 4), (3, 3), (0, 3)], [(3, 3), (4, 2), (4, 1), (3, 0), (0, 0)]],
    'C': [[(4, 6), (1, 6), (0, 5), (0, 1), (1, 0), (4, 0)]],
    'E': [[(4, 6), (0, 6), (0, 0), (4, 0)], [(0, 3), (3, 3)]],
    'G': [[(4, 5), (3, 6), (1, 6), (0, 5), (0, 1), (1, 0), (3, 0), (4, 1), (4, 3), (2, 3)]],
    'H': [[(0, 0), (0, 6)], [(4, 0), (4, 6)], [(0, 3), (4, 3)]],
    'I': [[(2, 0), (2, 6)], [(1, 6), (3, 6)], [(1, 0), (3, 0)]],
    'K': [[(0, 0), (0, 6)], [(4, 6), (0, 2.4)], [(1.4, 3.6), (4, 0)]],
    'L': [[(0, 6), (0, 0), (4, 0)]],
    'N': [[(0, 0), (0, 6), (4, 0), (4, 6)]],
    'R': [[(0, 0), (0, 6), (3, 6), (4, 5), (4, 4), (3, 3), (0, 3)], [(2, 3), (4, 0)]],
    'S': [[(4, 5), (3, 6), (1, 6), (0, 5), (0, 4), (1, 3), (3, 3), (4, 2), (4, 1), (3, 0), (1, 0), (0, 1)]],
    'T': [[(0, 6), (4, 6)], [(2, 6), (2, 0)]],
    'U': [[(0, 6), (0, 1), (1, 0), (3, 0), (4, 1), (4, 6)]],
    'V': [[(0, 6), (2, 0), (4, 6)]],
    '0': [[(1, 0), (0, 1), (0, 5), (1, 6), (3, 6), (4, 5), (4, 1), (3, 0), (1, 0)]],
    '1': [[(1, 5), (2, 6), (2, 0)], [(1, 0), (3, 0)]],
    '2': [[(0, 5), (1, 6), (3, 6), (4, 5), (4, 4), (0, 0), (4, 0)]],
    '/': [[(0.5, 0), (3.5, 6)]],
}
STROKE = 0.95     # bar width, grid units
ADVANCE = 5.6     # letter pitch, grid units

PAINTS = {'gray': (['Paint - Enamel Glossy (Light Gray)', 'Paint - Enamel Glossy (Light Grey)',
                    'Paint - Enamel Glossy (Grey)', 'Paint - Enamel Glossy (Gray)'], (190, 192, 194), ('gray', 'grey')),
          'white': (['Paint - Enamel Glossy (White)'], (245, 245, 245), ('white',))}


def by_name(appearances, name):
    try:
        return appearances.itemByName(name)
    except Exception:
        return None


def find_paint(app, design, key):
    names, rgb, words = PAINTS[key]
    for n in names:
        a = by_name(design.appearances, n)
        if a:
            return a
    libs = [lib for lib in app.materialLibraries]
    for n in names:
        for lib in libs:
            a = by_name(lib.appearances, n)
            if a:
                return design.appearances.addByCopy(a, n)
    for lib in libs:
        try:
            for a in lib.appearances:
                n = a.name.lower()
                if any(w in n for w in words) and 'paint' in n and 'gloss' in n and 'dark' not in n:
                    return design.appearances.addByCopy(a, a.name)
        except Exception:
            pass
    if design.appearances.count:
        a = design.appearances.addByCopy(design.appearances.item(0), names[0])
        for p in a.appearanceProperties:
            c = adsk.core.ColorProperty.cast(p)
            if c:
                try:
                    c.value = adsk.core.Color.create(rgb[0], rgb[1], rgb[2], 255)
                except Exception:
                    pass
        return a
    return None


def find_body(design, name):
    for comp in design.allComponents:
        for b in comp.bRepBodies:
            if b.name == name:
                return comp, b
    return None, None


def mid(bb):
    return adsk.core.Point3D.create((bb.minPoint.x + bb.maxPoint.x) / 2, (bb.minPoint.y + bb.maxPoint.y) / 2,
                                    (bb.minPoint.z + bb.maxPoint.z) / 2)


def normal_of(face):
    ok, n = face.evaluator.getNormalAtPoint(face.pointOnFace)
    return n


def front_face(body, toward):
    # The panel's front is its biggest flat face that points toward the driver.
    flats = [f for f in body.faces if adsk.core.Plane.cast(f.geometry)]
    big = max(f.area for f in flats)
    flats = [f for f in flats if f.area >= 0.5 * big]

    def score(f):
        v = f.centroid.vectorTo(toward)
        v.normalize()
        return normal_of(f).dotProduct(v)
    return max(flats, key=score)


def up_in(design, comp):
    # Model Y is up in these designs; carry it into the component's own space.
    up = adsk.core.Vector3D.create(0, 1, 0)
    occs = design.rootComponent.allOccurrencesByComponent(comp)
    if occs.count:
        m = occs.item(0).transform2.copy()
        m.invert()
        up.transformBy(m)
    return up


class Frame:
    """2D frame on the panel in sketch space: origin at the face centre, u level to the right, v up."""

    def __init__(self, sketch, face, up):
        c = face.centroid
        n = normal_of(face)
        up = up.copy()
        k = up.dotProduct(n)
        up = adsk.core.Vector3D.create(up.x - k * n.x, up.y - k * n.y, up.z - k * n.z)
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
        self.c, self.n, self.up3, self.right3 = c, n, up, right
        self.o = s0
        self.u = to_sketch(right)
        self.v = to_sketch(up)

    def xy(self, p):
        # Panel coordinates of a point in component space.
        d = self.c.vectorTo(p)
        return d.dotProduct(self.right3), d.dotProduct(self.up3)

    def pt(self, x, y):
        return adsk.core.Point3D.create(self.o.x + x * self.u.x + y * self.v.x,
                                        self.o.y + x * self.u.y + y * self.v.y, 0)


def bar(sketch, frame, x0, y0, x1, y1, w, quads):
    # A rectangle of width w from (x0, y0) to (x1, y1), with square ends that fill the corners.
    dx, dy = x1 - x0, y1 - y0
    L = math.hypot(dx, dy)
    ex, ey = dx / L * w / 2, dy / L * w / 2
    nx, ny = -ey, ex
    xa, ya, xb, yb = x0 - ex, y0 - ey, x1 + ex, y1 + ey
    pts = [frame.pt(x, y) for x, y in [(xa + nx, ya + ny), (xb + nx, yb + ny), (xb - nx, yb - ny), (xa - nx, ya - ny)]]
    lines = sketch.sketchCurves.sketchLines
    first = lines.addByTwoPoints(pts[0], pts[1])
    last = first
    for p in pts[2:]:
        last = lines.addByTwoPoints(last.endSketchPoint, p)
    lines.addByTwoPoints(last.endSketchPoint, first.startSketchPoint)
    quads.append([(p.x, p.y) for p in pts])


def word(sketch, frame, text, cx, cy, quads):
    # One line of text centred on (cx, cy) in panel coordinates.
    s = CAP / 6
    width = (len(text) - 1) * ADVANCE + 4
    x0 = cx - width * s / 2
    y0 = cy - 3 * s
    for i, ch in enumerate(text):
        for line in GLYPHS.get(ch, []):
            for (ax, ay), (bx, by) in zip(line, line[1:]):
                bar(sketch, frame, x0 + (i * ADVANCE + ax) * s, y0 + ay * s,
                    x0 + (i * ADVANCE + bx) * s, y0 + by * s, STROKE * s, quads)


def inside(quads, x, y):
    for q in quads:
        sign = 0
        for (ax, ay), (bx, by) in zip(q, q[1:] + q[:1]):
            c = (bx - ax) * (y - ay) - (by - ay) * (x - ax)
            if abs(c) < 1e-12:
                continue
            s = 1 if c > 0 else -1
            if sign and s != sign:
                break
            sign = s
        else:
            return True
    return False


def remove_old(pcomp):
    # Only the features and sketches this script made, found by their names.
    gone = 0
    for feat in [f for f in pcomp.features.extrudeFeatures if f.name.startswith('label_')]:
        if all(b.name.startswith('label_') for b in feat.bodies):
            feat.deleteMe()
            gone += 1
    for name in (OLD_SKETCH, SKETCH):
        sk = pcomp.sketches.itemByName(name)
        if sk:
            sk.deleteMe()
            gone += 1
    return gone


def run(context):
    app = adsk.core.Application.get()
    ui = app.userInterface
    try:
        design = adsk.fusion.Design.cast(app.activeProduct)
        gray, white = find_paint(app, design, 'gray'), find_paint(app, design, 'white')
        lines = []

        # 1. Light gray switches and sockets
        found = {}
        for name in ITEMS:
            comp, body = find_body(design, name)
            if not body:
                lines.append(f'{name}: not found')
                continue
            found[name] = (comp, body)
            if name in KEEP_COLOUR:
                continue
            try:
                body.appearance = gray
                for f in body.faces:
                    f.appearance = gray
            except Exception:
                lines.append(f'{name}: could not repaint (referenced design?)')
        lines.insert(0, f'Light gray: {len([n for n in found if n not in KEEP_COLOUR])} bodies ({gray.name})')

        # 2. White labels on the panel
        pcomp, panel = find_body(design, PANEL)
        if not panel:
            lines.append(f'{PANEL} not found, so no labels')
            ui.messageBox('\n'.join(lines))
            return
        if pcomp.sketches.itemByName(OLD_SKETCH) or pcomp.sketches.itemByName(SKETCH):
            r = ui.messageBox('Labels from an earlier run are on the panel. Remove them and draw the new ones?\n\n'
                              'Only the label sketch and the label_ features this script made are removed.',
                              'Console labels', adsk.core.MessageBoxButtonTypes.YesNoButtonType)
            if r != adsk.core.DialogResults.DialogYes:
                lines.append('Labels left as they were.')
                ui.messageBox('\n'.join(lines))
                return
            lines.append(f'Removed {remove_old(pcomp)} old label items.')

        _, visor = find_body(design, VISOR)
        toward = mid(visor.boundingBox) if visor else app.activeViewport.camera.eye
        face = front_face(panel, toward)
        sk = pcomp.sketches.add(face)
        sk.name = SKETCH
        fr = Frame(sk, face, up_in(design, pcomp))

        labelled = [(name, words) for name, words in ITEMS.items() if words and name in found]
        pos = {name: fr.xy(mid(found[name][1].boundingBox)) for name, _ in labelled}
        row = min(y for _, y in pos.values()) - GAP
        quads = []
        sk.isComputeDeferred = True
        for name, words in labelled:
            x, _ = pos[name]
            for i, w in enumerate(words):
                word(sk, fr, w, x, row - i * LINE * CAP, quads)
        sk.isComputeDeferred = False

        # Extrude the letter regions only: skip the panel outline and the holes inside A, B, R, 0.
        profs = list(sk.profiles)

        def picks(to_sketch):
            out = []
            for p in profs:
                if p.areaProperties().area > 4 * CAP * CAP:
                    continue
                c = p.areaProperties().centroid
                if to_sketch:
                    c = sk.modelToSketchSpace(c)
                if inside(quads, c.x, c.y):
                    out.append(p)
            return out
        a, b = picks(False), picks(True)
        chosen = a if len(a) >= len(b) else b
        coll = adsk.core.ObjectCollection.create()
        for p in chosen:
            coll.add(p)
        ext = pcomp.features.extrudeFeatures

        def make(dist):
            inp = ext.createInput(coll, adsk.fusion.FeatureOperations.NewBodyFeatureOperation)
            inp.setDistanceExtent(False, adsk.core.ValueInput.createByReal(dist))
            return ext.add(inp)
        feat = make(RAISE)
        cm = feat.bodies.item(0).getPhysicalProperties().centerOfMass
        if face.centroid.vectorTo(cm).dotProduct(fr.n) < 0:
            feat.deleteMe()
            feat = make(-RAISE)
        feat.name = 'label_console'
        for i, body in enumerate(feat.bodies):
            body.name = f'label_console_{i + 1}'
            body.appearance = white
        lines.append(f'White labels: {len(labelled)} items, {feat.bodies.count} letter bodies '
                     f'({len(chosen)} of {len(profs)} sketch regions)')

        msg = '\n'.join(lines)
        app.log('ConsoleLabels: ' + msg)
        ui.messageBox(msg)
    except Exception:
        app.log('ConsoleLabels failed:\n' + traceback.format_exc())
        ui.messageBox('Failed:\n' + traceback.format_exc())
