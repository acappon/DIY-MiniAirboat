# Gives the three dash gauges (tach, water temp, fuel) a white appearance so they read against
# the black dash panel, at night and in the renders. Run it in the design that holds the gauge
# bodies: Utilities > Scripts and Add-Ins > + (add this folder) > Run.
import adsk.core, adsk.fusion, traceback

GAUGES = ('tachometer_electric', 'water_temp_gauge', 'fuel_gauge')
WHITE = 'Paint - Enamel Glossy (White)'


def find_white(app, design):
    have = design.appearances.itemByName(WHITE)
    if have:
        return have
    for lib in app.materialLibraries:
        a = lib.appearances.itemByName(WHITE)
        if a:
            return design.appearances.addByCopy(a, WHITE)
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
        ui.messageBox(msg)
    except Exception:
        ui.messageBox('Failed:\n' + traceback.format_exc())
