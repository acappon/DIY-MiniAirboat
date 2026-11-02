FeatureScript 3112;
import(path : "onshape/std/geometry.fs", version : "3112.0");

// Parametric flat-bottom jonboat hull, a placeholder until a real hull is measured.
// Frame: X forward from the outside of the transom, Y = 0 on the centerline (port +), Z up from the
// outside of the bottom. The bottom is flat to `flatEnd`, then rises to `bowRise` at the bow while it
// narrows to `bowBottom`. Sides flare out evenly from the chine to the gunwale.
// Defaults are a Lowe L1652MT from dealer specs (length, bottom width, beam, transom height); the side
// height, flat length, bow rise and bow width are assumptions to be replaced by measurements.

const STATIONS = [0, 0.2, 0.4, 0.6, 0.8, 1]; // bow-rise stations as fractions of the rise length

function bottomZ(def is map, x is number) returns number
{
    const xf = def.flatEnd / inch;
    const xb = def.oal / inch;
    return x <= xf ? 0 : def.bowRise / inch * ((x - xf) / (xb - xf)) ^ 1.5;
}

function bottomHalf(def is map, x is number) returns number
{
    const xf = def.flatEnd / inch;
    const xb = def.oal / inch;
    const w0 = def.bottomWidth / inch / 2;
    const w1 = def.bowBottom / inch / 2;
    return x <= xf ? w0 : w0 - (w0 - w1) * ((x - xf) / (xb - xf)) ^ 2;
}

// Section outline at station x, offset inward by `off` (0 for the outside skin), top at `zTop`.
function section(def is map, x is number, off is number, zTop is number) returns array
{
    const flare = (def.beam / inch - def.bottomWidth / inch) / 2 / (def.sideHeight / inch);
    const zb = bottomZ(def, x) + off;
    const hb = bottomHalf(def, x) - off;
    const ht = hb + flare * (zTop - zb);
    return [[-hb, zb], [hb, zb], [ht, zTop], [-ht, zTop], [-hb, zb]];
}

function loftSolid(context is Context, id is Id, def is map, xs is array, off is number, zTop is number)
{
    var regions = [];
    for (var i = 0; i < size(xs); i += 1)
    {
        const sid = id + ("sk" ~ i);
        const sk = newSketchOnPlane(context, sid, { "sketchPlane" : plane(vector(xs[i], 0, 0) * inch, vector(1, 0, 0), vector(0, 1, 0)) });
        var pts = [];
        for (var p in section(def, xs[i], off, zTop))
            pts = append(pts, vector(p[0], p[1]) * inch);
        skPolyline(sk, "poly", { "points" : pts });
        skSolve(sk);
        regions = append(regions, qSketchRegion(sid));
    }
    // Loft the flat run and the bow rise separately: one loft through all stations overshoots
    // below the flat bottom where the rise starts.
    opLoft(context, id + "loftFlat", { "profileSubqueries" : [regions[0], regions[1]], "bodyType" : ToolBodyType.SOLID });
    opLoft(context, id + "loftBow", { "profileSubqueries" : subArray(regions, 1, size(regions)), "bodyType" : ToolBodyType.SOLID });
    opBoolean(context, id + "join", { "tools" : qUnion([qCreatedBy(id + "loftFlat", EntityType.BODY), qCreatedBy(id + "loftBow", EntityType.BODY)]), "operationType" : BooleanOperationType.UNION });
    for (var i = 0; i < size(xs); i += 1)
        opDeleteBodies(context, id + ("del" ~ i), { "entities" : qCreatedBy(id + ("sk" ~ i), EntityType.BODY) });
}

annotation { "Feature Type Name" : "Jon hull (parametric)" }
export const jonHull = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        annotation { "Name" : "Overall length" }
        isLength(definition.oal, { (inch) : [120, 189.5, 260] } as LengthBoundSpec);
        annotation { "Name" : "Bottom width (chine to chine)" }
        isLength(definition.bottomWidth, { (inch) : [36, 52, 72] } as LengthBoundSpec);
        annotation { "Name" : "Beam at the gunwale" }
        isLength(definition.beam, { (inch) : [48, 75, 100] } as LengthBoundSpec);
        annotation { "Name" : "Side height (bottom to gunwale)" }
        isLength(definition.sideHeight, { (inch) : [12, 21, 30] } as LengthBoundSpec);
        annotation { "Name" : "Transom height" }
        isLength(definition.transomHeight, { (inch) : [12, 20, 30] } as LengthBoundSpec);
        annotation { "Name" : "Flat bottom ends at (X)" }
        isLength(definition.flatEnd, { (inch) : [60, 121, 200] } as LengthBoundSpec);
        annotation { "Name" : "Bottom height at the bow" }
        isLength(definition.bowRise, { (inch) : [2, 10.5, 20] } as LengthBoundSpec);
        annotation { "Name" : "Bottom width at the bow" }
        isLength(definition.bowBottom, { (inch) : [20, 40, 72] } as LengthBoundSpec);
        annotation { "Name" : "Skin thickness" }
        isLength(definition.skin, { (inch) : [0.05, 0.1, 0.25] } as LengthBoundSpec);
        annotation { "Name" : "Rear bench X range start" }
        isLength(definition.rearBench0, { (inch) : [0, 12, 100] } as LengthBoundSpec);
        annotation { "Name" : "Rear bench X range end" }
        isLength(definition.rearBench1, { (inch) : [0, 32, 100] } as LengthBoundSpec);
        annotation { "Name" : "Front bench X range start" }
        isLength(definition.frontBench0, { (inch) : [60, 140, 250] } as LengthBoundSpec);
        annotation { "Name" : "Front bench X range end" }
        isLength(definition.frontBench1, { (inch) : [60, 158, 250] } as LengthBoundSpec);
        annotation { "Name" : "Bench top height" }
        isLength(definition.benchTop, { (inch) : [6, 14, 24] } as LengthBoundSpec);
        annotation { "Name" : "Transom notch width (0 for none)" }
        isLength(definition.notchWidth, { (inch) : [0, 0, 70] } as LengthBoundSpec);
        annotation { "Name" : "Transom notch bottom height" }
        isLength(definition.notchHeight, { (inch) : [6, 15.85, 30] } as LengthBoundSpec);
        annotation { "Name" : "Rear bench cut-out, inner edge of port box (0 = full bench)" }
        isLength(definition.rearGapPort, { (inch) : [0, 0, 30] } as LengthBoundSpec);
        annotation { "Name" : "Rear bench cut-out, inner edge of starboard box (0 = full bench)" }
        isLength(definition.rearGapStbd, { (inch) : [0, 0, 30] } as LengthBoundSpec);
    }
    {
        const xf = definition.flatEnd / inch;
        const xb = definition.oal / inch;
        const t = definition.skin / inch;
        const G = definition.sideHeight / inch;
        const flare = (definition.beam / inch - definition.bottomWidth / inch) / 2 / G;

        var xsOut = [0, xf];
        var xsIn = [t, xf];
        for (var k = 1; k < size(STATIONS); k += 1)
        {
            const x = xf + (xb - xf) * STATIONS[k];
            xsOut = append(xsOut, x);
            xsIn = append(xsIn, k == size(STATIONS) - 1 ? x - t : x);
        }

        // Tools first (cavity, transom cut), then the outer solid and its booleans under its own prefix,
        // so each prefix is used in one contiguous run and the result stays queryable by that prefix.
        loftSolid(context, id + "cavity", definition, xsIn, t, G + 1);
        // Transom top is lower than the sides.
        fCuboid(context, id + "transomCut", { "corner1" : vector(-1, -60, definition.transomHeight / inch) * inch, "corner2" : vector(t + 0.01, 60, G + 1) * inch });
        // Optional center notch cut down into the transom (clearance for the prop cage and exhaust).
        const nw = definition.notchWidth / inch;
        if (nw > 0)
            fCuboid(context, id + "notchCut", { "corner1" : vector(-1, -nw / 2, definition.notchHeight / inch) * inch, "corner2" : vector(t + 0.01, nw / 2, G + 1) * inch });
        loftSolid(context, id + "outer", definition, xsOut, 0, G);
        opBoolean(context, id + "outer" + "hollow", { "targets" : qCreatedBy(id + "outer", EntityType.BODY), "tools" : qCreatedBy(id + "cavity", EntityType.BODY), "operationType" : BooleanOperationType.SUBTRACTION });
        opBoolean(context, id + "outer" + "trim", { "targets" : qCreatedBy(id + "outer", EntityType.BODY), "tools" : qCreatedBy(id + "transomCut", EntityType.BODY), "operationType" : BooleanOperationType.SUBTRACTION });
        if (nw > 0)
            opBoolean(context, id + "outer" + "notch", { "targets" : qCreatedBy(id + "outer", EntityType.BODY), "tools" : qCreatedBy(id + "notchCut", EntityType.BODY), "operationType" : BooleanOperationType.SUBTRACTION });
        setProperty(context, { "entities" : qCreatedBy(id + "outer", EntityType.BODY), "propertyType" : PropertyType.NAME, "value" : "Hull shell (parametric 1652 placeholder)" });
        setProperty(context, { "entities" : qCreatedBy(id + "outer", EntityType.BODY), "propertyType" : PropertyType.APPEARANCE, "value" : color(0.86, 0.45, 0.1) });

        // Benches: closed aluminum boxes (foam-filled on the real boat), fitted to the inside of the sides.
        // The rear bench center can be cut out (drivetrain bay), leaving a foam-filled box at each side.
        const gapP = definition.rearGapPort / inch;
        const gapS = definition.rearGapStbd / inch;
        const splitRear = gapP > 0 || gapS > 0;
        if (splitRear)
            fCuboid(context, id + "rearGapCut", { "corner1" : vector(definition.rearBench0 / inch - 1, -gapS, -1) * inch, "corner2" : vector(definition.rearBench1 / inch + 1, gapP, G + 1) * inch });
        const benches = [["rearBench", definition.rearBench0 / inch, definition.rearBench1 / inch, splitRear ? "Rear side box (bench end, foam-filled)" : "Rear bench (factory, foam-filled)"],
                         ["frontBench", definition.frontBench0 / inch, definition.frontBench1 / inch, "Front bench (factory, foam-filled)"]];
        for (var bn in benches)
        {
            const bid = id + bn[0];
            const x1 = bn[2];
            const zTop = definition.benchTop / inch;
            const zBot = zTop - 6;
            const hb = bottomHalf(definition, x1) - t;
            const zb = bottomZ(definition, x1) + t;
            const hBot = hb + flare * (zBot - zb) - 0.05;
            const hTop = hb + flare * (zTop - zb) - 0.05;
            const sk = newSketchOnPlane(context, bid + "sk", { "sketchPlane" : plane(vector(bn[1], 0, 0) * inch, vector(1, 0, 0), vector(0, 1, 0)) });
            skPolyline(sk, "poly", { "points" : [vector(-hBot, zBot) * inch, vector(hBot, zBot) * inch, vector(hTop, zTop) * inch, vector(-hTop, zTop) * inch, vector(-hBot, zBot) * inch] });
            skSolve(sk);
            opExtrude(context, bid + "ex", { "entities" : qSketchRegion(bid + "sk"), "direction" : vector(1, 0, 0), "endBound" : BoundingType.BLIND, "endDepth" : (x1 - bn[1]) * inch });
            opDeleteBodies(context, bid + "del", { "entities" : qCreatedBy(bid + "sk", EntityType.BODY) });
            if (splitRear && bn[0] == "rearBench")
                opBoolean(context, bid + "gap", { "targets" : qCreatedBy(bid, EntityType.BODY), "tools" : qCreatedBy(id + "rearGapCut", EntityType.BODY), "operationType" : BooleanOperationType.SUBTRACTION });
            setProperty(context, { "entities" : qCreatedBy(bid, EntityType.BODY), "propertyType" : PropertyType.NAME, "value" : bn[3] });
            setProperty(context, { "entities" : qCreatedBy(bid, EntityType.BODY), "propertyType" : PropertyType.APPEARANCE, "value" : color(0.8, 0.42, 0.1) });
        }
    });
