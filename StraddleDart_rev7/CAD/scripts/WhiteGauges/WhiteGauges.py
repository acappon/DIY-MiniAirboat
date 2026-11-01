# Gives the three dash gauges (tach, water temp, fuel) a white appearance so they read against
# the black dash panel, at night and in the renders. Run it in the design that holds the gauge
# bodies: Utilities > Scripts and Add-Ins > + (add this folder) > Run.
import adsk.core, adsk.fusion, traceback

GAUGES = ('tachometer_electric', 'water_temp_gauge', 'fuel_gauge')
WHITE = 'Paint - Enamel Glossy (White)'


def by_name(appearances, name):
    # itemByName raises instead of returning None when the name isn't there.
    try:
        return appearances.itemByName(name)
    except Exception:
        return None


def find_white(app, design):
    have = by_name(design.appearances, WHITE)
    if have:
        return have
    libs = [lib for lib in app.materialLibraries]
    for lib in libs:
        a = by_name(lib.appearances, WHITE)
        if a:
            return design.appearances.addByCopy(a, WHITE)
    # Library names vary between Fusion versions: take any glossy white paint.
    for lib in libs:
        try:
            for a in lib.appearances:
                n = a.name.lower()
                if 'white' in n and 'paint' in n and 'gloss' in n:
                    return design.appearances.addByCopy(a, WHITE)
        except Exception:
            pass
    # Last resort: copy any appearance in the design and turn its colours white.
    if design.appearances.count:
        a = design.appearances.addByCopy(design.appearances.item(0), WHITE)
        for p in a.appearanceProperties:
            c = adsk.core.ColorProperty.cast(p)
            if c:
                try:
                    c.value = adsk.core.Color.create(245, 245, 245, 255)
                except Exception:
                    pass
        return a
    return None


def run(context):
    app = adsk.core.Application.get()
    ui = app.userInterface
    try:
        design = adsk.fusion.Design.cast(app.activeProduct)
        white = find_white(app, design)
        if not white:
            ui.messageBox(f'Could not find "{WHITE}" in the appearance libraries.')
            return
        done, refs = [], []
        for comp in design.allComponents:
            for b in comp.bRepBodies:
                if b.name not in GAUGES:
                    continue
                try:
                    b.appearance = white
                    # Face appearances override the body's, so paint those too.
                    for f in b.faces:
                        f.appearance = white
                    done.append(b.name)
                except Exception:
                    refs.append(f'{b.name} (in {comp.name})')
        msg = f'White: {", ".join(done) or "none"}'
        if refs:
            msg += ('\n\nThese are in a referenced design and could not be changed here. '
                    'Open that design, run the script there, save, then update the reference:\n' + '\n'.join(refs))
        missing = set(GAUGES) - set(done) - {r.split(' ')[0] for r in refs}
        if missing:
            msg += '\n\nNot found in this design: ' + ', '.join(sorted(missing))
        # An appearance on an occurrence overrides its bodies' own, so report any on the way to a
        # gauge, and select the gauges so you can see which shapes on the dash they really are.
        sel = ui.activeSelections
        sel.clear()
        notes = []
        root = design.rootComponent
        for b in root.bRepBodies:
            if b.name in GAUGES:
                sel.add(b)
        for occ in root.allOccurrences:
            for b in occ.component.bRepBodies:
                if b.name not in GAUGES:
                    continue
                sel.add(b.createForAssemblyContext(occ))
                o = occ
                while o:
                    if o.appearance:
                        notes.append(f'{o.fullPathName} has appearance "{o.appearance.name}"')
                    o = o.assemblyContext
                if not b.isVisible:
                    notes.append(f'{b.name} in {occ.fullPathName} is hidden')
        if notes:
            msg += '\n\nOverrides that can hide the white:\n' + '\n'.join(sorted(set(notes)))
        msg += '\n\nThe gauge bodies are now selected (highlighted) in the window.'
        app.log('WhiteGauges: ' + msg)
        ui.messageBox(msg)
    except Exception:
        app.log('WhiteGauges failed:\n' + traceback.format_exc())
        ui.messageBox('Failed:\n' + traceback.format_exc())
