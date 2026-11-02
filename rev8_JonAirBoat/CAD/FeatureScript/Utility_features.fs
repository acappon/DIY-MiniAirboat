FeatureScript 3112;
import(path : "onshape/std/geometry.fs", version : "3112.0");

// Moves every solid part in the Part Studio by the same offset. Used to seat the imported rev7
// drivetrain on the engine-bed mount plates without editing the import itself.
annotation { "Feature Type Name" : "Translate all parts" }
export const translateAllParts = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        annotation { "Name" : "X offset" }
        isLength(definition.dx, { (inch) : [-100, 0, 100] } as LengthBoundSpec);

        annotation { "Name" : "Y offset" }
        isLength(definition.dy, { (inch) : [-100, 0, 100] } as LengthBoundSpec);

        annotation { "Name" : "Z offset" }
        isLength(definition.dz, { (inch) : [-100, 0, 100] } as LengthBoundSpec);
    }
    {
        opTransform(context, id + "move", {
                    "bodies" : qAllModifiableSolidBodies(),
                    "transform" : transform(vector(definition.dx, definition.dy, definition.dz))
                });
    });

// Narrows the rev7 cage base so the cage feet land inside the Lowe 1448 hull sides.
// Foot pads, isolators and foot bolts move inboard by `delta` on each side. Legs, base rails and
// side diagonals are rebuilt as 1 in x 0.065 in tubes whose ends move inboard by `delta` at the
// feet, tapering to no change at the drum hoops (Z topZ), so the cage drum itself does not move.
annotation { "Feature Type Name" : "Narrow cage base" }
export const narrowCageBase = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        annotation { "Name" : "Move feet inboard (each side)" }
        isLength(definition.delta, { (inch) : [0, 2.75, 8] } as LengthBoundSpec);

        annotation { "Name" : "Height where legs meet the drum (no shift)" }
        isLength(definition.topZ, { (inch) : [20, 38.1, 60] } as LengthBoundSpec);

        annotation { "Name" : "Height below which the full shift applies" }
        isLength(definition.lowZ, { (inch) : [10, 20, 40] } as LengthBoundSpec);
    }
    {
        const D = definition.delta / inch;
        const zTop = definition.topZ / inch;
        const zLow = definition.lowZ / inch;
        const shift = function(z) { return D * min(1, max(0, (zTop - z) / (zTop - zLow))); };

        var feet = { "P" : [], "S" : [] };
        var tubes = [];
        // Bodies are picked by position and shape, not by name: imported part names are part
        // metadata and are not visible while the Part Studio regenerates.
        for (var b in evaluateQuery(context, qBodyType(qEverything(EntityType.BODY), BodyType.SOLID)))
        {
            const bb = evBox3d(context, { "topology" : b });
            const lo = bb.minCorner / inch;
            const hi = bb.maxCorner / inch;
            const c = (lo + hi) / 2;
            const side = c[1] > 0 ? "P" : "S";
            var hasTube = false;
            for (var tf in evaluateQuery(context, qGeometry(qOwnedByBody(b, EntityType.FACE), GeometryType.CYLINDER)))
                if (abs(evSurfaceDefinition(context, { "face" : tf }).radius / inch - 0.5) < 0.01)
                    hasTube = true;
            const inSideFrame = abs(abs(c[1]) - 33.2) < 0.3 && hi[1] - lo[1] < 1.2 && lo[2] > 16 && hi[2] < 39 && lo[0] > -3.5 && hi[0] < 13.5;
            const inFootZone = lo[2] > 15.5 && hi[2] < 20 && abs(c[1]) > 30 && abs(c[1]) < 36 && lo[0] > -0.5 && hi[0] < 15.5 && hi[1] - lo[1] < 4.5;
            if (!(hasTube && inSideFrame) && inFootZone)
            {
                feet[side] = append(feet[side], b);
            }
            else if (hasTube && inSideFrame)
            {
                const n = "cage base tube";
                var rOut = 0;
                var rIn = 1e9;
                var d = undefined;
                var aMax = 0;
                for (var f in evaluateQuery(context, qGeometry(qOwnedByBody(b, EntityType.FACE), GeometryType.CYLINDER)))
                {
                    const cyl = evSurfaceDefinition(context, { "face" : f });
                    const r = cyl.radius / inch;
                    rOut = max(rOut, r);
                    rIn = min(rIn, r);
                    // The tube axis comes from the largest cylinder face; small faces are copes.
                    const a = evArea(context, { "entities" : f }) / inch / inch;
                    if (a > aMax)
                    {
                        aMax = a;
                        d = cyl.coordSystem.zAxis;
                    }
                }
                // Endpoints from the bounding box along the dominant axis direction.
                var k = 0;
                for (var ci in [1, 2])
                    if (abs(d[ci]) > abs(d[k]))
                        k = ci;
                const ext = (bb.maxCorner[k] - bb.minCorner[k]) / inch;
                const len = (ext - 2 * rOut * sqrt(max(0, 1 - d[k] * d[k]))) / abs(d[k]);
                const A = c - d * len / 2;
                const B = c + d * len / 2;
                tubes = append(tubes, {
                            "name" : n, "color" : try silent(getProperty(context, { "entity" : b, "propertyType" : PropertyType.APPEARANCE })),
                            "A" : A, "B" : B, "rOut" : rOut, "rIn" : (rIn < rOut ? rIn : rOut - 0.065), "s" : (c[1] > 0 ? 1 : -1), "body" : b
                        });
            }
        }

        if (size(feet["P"]) == 0 || size(feet["S"]) == 0 || size(tubes) == 0)
            throw regenError("Cage feet or base tubes not found (feet " ~ size(feet["P"]) ~ "/" ~ size(feet["S"]) ~ ", tubes " ~ size(tubes) ~ ")");

        opTransform(context, id + "feetP", { "bodies" : qUnion(feet["P"]), "transform" : transform(vector(0, -D, 0) * inch) });
        opTransform(context, id + "feetS", { "bodies" : qUnion(feet["S"]), "transform" : transform(vector(0, D, 0) * inch) });

        var olds = [];
        for (var tb in tubes)
            olds = append(olds, tb.body);
        opDeleteBodies(context, id + "oldTubes", { "entities" : qUnion(olds) });

        for (var i = 0; i < size(tubes); i += 1)
        {
            const t = tubes[i];
            const A = t.A - vector(0, t.s * shift(t.A[2]), 0);
            const B = t.B - vector(0, t.s * shift(t.B[2]), 0);
            const tin = id + ("bore" ~ i);
            fCylinder(context, tin, { "bottomCenter" : (A - normalize(B - A) * 0.1) * inch, "topCenter" : (B + normalize(B - A) * 0.1) * inch, "radius" : t.rIn * inch });
            const tid = id + ("tube" ~ i);
            fCylinder(context, tid + "outer", { "bottomCenter" : A * inch, "topCenter" : B * inch, "radius" : t.rOut * inch });
            opBoolean(context, tid + "bore", { "targets" : qCreatedBy(tid, EntityType.BODY), "tools" : qCreatedBy(tin, EntityType.BODY), "operationType" : BooleanOperationType.SUBTRACTION });
            setProperty(context, { "entities" : qCreatedBy(tid, EntityType.BODY), "propertyType" : PropertyType.NAME, "value" : t.name ~ " (narrowed base)" });
            // Rev7 cage tubes are powder-coated near-black.
            const col = t.color == undefined ? color(0.098, 0.098, 0.098) : color(t.color.red, t.color.green, t.color.blue);
            try silent(setProperty(context, { "entities" : qCreatedBy(tid, EntityType.BODY), "propertyType" : PropertyType.APPEARANCE, "value" : col }));
        }
    });
