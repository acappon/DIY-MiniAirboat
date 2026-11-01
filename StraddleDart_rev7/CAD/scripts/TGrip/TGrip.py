# Swaps the grip's white ball knob for a T-handle: a polished aluminium bar across the boat on top
# of the stick, like a Hurst T-handle (no logo). The ball is hidden, not deleted. Run it in the
# Layout design.
import adsk.core, adsk.fusion, math, traceback

STICK = 'shifter_stick_chrome'
BALL = 'shifter_knob_white'
BAR = 'shifter_T_handle_6061'
LENGTH = 4.5 * 2.54   # cm, across the boat
DIA = 1.25 * 2.54     # cm
END_ROUND = 0.6       # cm fillet on the bar ends
SINK = 0.4            # cm the stick goes into the bar past its centre line

ALU = ['Aluminum - Polished', 'Aluminum - Satin', 'Aluminum']


def by_name(appearances, name):
    try:
        return appearances.itemByName(name)
    except Exception:
        return None


def find_alu(app, design):
    for n in ALU:
        a = by_name(design.appearances, n)
        if a:
            return a
    libs = [lib for lib in app.materialLibraries]
    for n in ALU:
        for lib in libs:
            a = by_name(lib.appearances, n)
            if a:
                return design.appearances.addByCopy(a, n)
    for lib in libs:
        try:
            for a in lib.appearances:
                n = a.name.lower()
                if 'alumin' in n and 'polish' in n:
                    return design.appearances.addByCopy(a, a.name)
        except Exception:
            pass
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


def in_comp(design, comp, v):
    # A world direction carried into the component's own space.
    v = v.copy()
    occs = design.rootComponent.allOccurrencesByComponent(comp)
    if occs.count:
        m = occs.item(0).transform2.copy()
        m.invert()
        v.transformBy(m)
    v.normalize()
    return v


def stick_tip(stick, ball_c):
    # The stick's round flat end nearest the ball: its centre, and the direction out of the stick.
    best = None
    for f in stick.faces:
        if not adsk.core.Plane.cast(f.geometry):
            continue
        if not any(adsk.core.Circle3D.cast(e.geometry) for e in f.edges):
            continue
        d = f.centroid.distanceTo(ball_c)
        if best is None or d < best[0]:
            best = (d, f)
    if not best:
        return None, None
    f = best[1]
    ok, n = f.evaluator.getNormalAtPoint(f.pointOnFace)
    return f.centroid, n


def run(context):
    app = adsk.core.Application.get()
    ui = app.userInterface
    try:
        design = adsk.fusion.Design.cast(app.activeProduct)
        comp, stick = find_body(design, STICK)
        _, ball = find_body(design, BALL)
        if not stick:
            ui.messageBox(f'{STICK} not found. Open the Layout design and run it there.')
            return
        if find_body(design, BAR)[1]:
            ui.messageBox('The T-handle is already there.')
            return
        ball_c = mid(ball.boundingBox) if ball else mid(stick.boundingBox)
        tip, out = stick_tip(stick, ball_c)
        if not tip:
            ui.messageBox('Could not find the top end of the stick.')
            return
        r = DIA / 2
        c = tip.copy()
        c.translateBy(adsk.core.Vector3D.create(out.x * (r - SINK), out.y * (r - SINK), out.z * (r - SINK)))
        across = in_comp(design, comp, adsk.core.Vector3D.create(0, 0, 1))  # model Z is across the boat
        # Keep the bar square to the stick.
        k = across.dotProduct(out)
        across = adsk.core.Vector3D.create(across.x - k * out.x, across.y - k * out.y, across.z - k * out.z)
        across.normalize()
        h = LENGTH / 2
        p1 = adsk.core.Point3D.create(c.x - h * across.x, c.y - h * across.y, c.z - h * across.z)
        p2 = adsk.core.Point3D.create(c.x + h * across.x, c.y + h * across.y, c.z + h * across.z)

        tmp = adsk.fusion.TemporaryBRepManager.get().createCylinderOrCone(p1, r, p2, r)
        if design.designType == adsk.fusion.DesignTypes.ParametricDesignType:
            base = comp.features.baseFeatures.add()
            base.startEdit()
            body = comp.bRepBodies.add(tmp, base)
            base.finishEdit()
            base.name = 'T_handle'
            body = base.bodies.item(0)
        else:
            body = comp.bRepBodies.add(tmp)
        body.name = BAR

        note = ''
        try:
            edges = adsk.core.ObjectCollection.create()
            for e in body.edges:
                if adsk.core.Circle3D.cast(e.geometry):
                    edges.add(e)
            fi = comp.features.filletFeatures.createInput()
            fi.addConstantRadiusEdgeSet(edges, adsk.core.ValueInput.createByReal(END_ROUND), True)
            comp.features.filletFeatures.add(fi).name = 'T_handle_ends'
            body = find_body(design, BAR)[1]
        except Exception:
            note = '\nEnd rounds failed, so the bar ends are square.'

        alu = find_alu(app, design)
        if alu:
            body.appearance = alu
        if ball:
            ball.isLightBulbOn = False

        # Model Y is up: report how the hand-hold height changed for the structural check.
        up = in_comp(design, comp, adsk.core.Vector3D.create(0, 1, 0))
        dz = ball_c.vectorTo(c).dotProduct(up) * 10
        msg = (f'T-handle added: {LENGTH / 2.54:.2f} in across the boat, {DIA / 2.54:.2f} in thick, '
               f'{alu.name if alu else "default appearance"}.\n'
               f'White ball knob hidden (not deleted).\n'
               f'Bar centre is {dz:+.0f} mm ({dz / 25.4:+.2f} in) from the old ball centre, measured upward.{note}')
        app.log('TGrip: ' + msg)
        ui.messageBox(msg)
    except Exception:
        app.log('TGrip failed:\n' + traceback.format_exc())
        ui.messageBox('Failed:\n' + traceback.format_exc())
