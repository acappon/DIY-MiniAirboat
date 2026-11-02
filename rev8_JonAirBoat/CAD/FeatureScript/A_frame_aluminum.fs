FeatureScript 3112;
import(path : "onshape/std/geometry.fs", version : "3112.0");

// Bolted 6061-T6 aluminum A-frame that replaces the rev7 welded steel A-frame in the drivetrain
// Part Studio (place it after "Raise onto engine-bed mount plates"). Same frame as the airboat
// assembly: X forward from the transom, Y + to port, Z up from the hull floor.
//   - Peak plates: 1/2 in, made from the steel plates' outline and holes; the inside faces (where the
//     redrive bearing flanges and tension yoke bolt on) stay put, the extra thickness goes outward
//   - Legs: 2-1/2 x 2-1/2 x 3/16 in square tube on the steel legs' centerlines; the top end is slotted
//     to straddle the peak plate (clevis) and through-bolted with two 3/8 in bolts
//   - Feet: 2 x 2 x 1/4 in angle clips on the front and back faces of each leg, held down by the
//     existing 3/8-16 bolts into the mount-plate captive nuts
//   - Crossbars: 2 x 2 x 3/16 in tube between the legs, on angle clips
//   - Braces: 2 x 2 x 3/16 in tube, angle-clip brackets on the forward leg and on the brace mount plate
// The steel parts are found by position and size (imported part names are not visible during
// regeneration) and deleted.

// Bounding boxes [min, max] of the steel A-frame parts after the 0.7 in drivetrain raise.
const STEEL = [
    [[12.5, 10.9, 6.62], [14.5, 15.81, 49.7]], [[12.5, -15.81, 6.62], [14.5, -10.9, 49.7]],
    [[21.5, 10.9, 6.62], [23.5, 15.81, 49.7]], [[21.5, -15.81, 6.62], [23.5, -10.9, 49.7]],
    [[12.5, 10.9, 49.7], [14.5, 12.9, 49.82]], [[12.5, -12.9, 49.7], [14.5, -10.9, 49.82]],
    [[21.5, 10.9, 49.7], [23.5, 12.9, 49.82]], [[21.5, -12.9, 49.7], [23.5, -10.9, 49.82]],
    [[13.31, -13, 45.2], [13.69, 13, 61.7]], [[22.31, -13, 45.2], [22.69, 13, 61.7]],
    [[12.75, -12.37, 27.95], [14.25, 12.37, 29.45]], [[21.5, -12.34, 27.96], [23.5, 12.34, 30.96]],
    [[10.25, 13.31, 6.37], [16.75, 16.31, 6.62]], [[10.25, -16.31, 6.37], [16.75, -13.31, 6.62]],
    [[19.25, 13.31, 6.37], [25.75, 16.31, 6.62]], [[19.25, -16.31, 6.37], [25.75, -13.31, 6.62]],
    [[11.75, 14.63, 6.62], [12.5, 14.88, 8.38]], [[14.5, 14.63, 6.62], [15.25, 14.88, 8.38]],
    [[11.75, -14.88, 6.62], [12.5, -14.63, 8.38]], [[14.5, -14.88, 6.62], [15.25, -14.63, 8.38]],
    [[20.75, 14.63, 6.62], [21.5, 14.88, 8.38]], [[23.5, 14.63, 6.62], [24.25, 14.88, 8.38]],
    [[20.75, -14.88, 6.62], [21.5, -14.63, 8.38]], [[23.5, -14.88, 6.62], [24.25, -14.63, 8.38]],
    [[24.57, 11.5, 8.44], [49.43, 15.51, 44.63]], [[24.57, -15.51, 8.44], [49.43, -11.5, 44.63]],
    [[45.5, 13.31, 6.37], [53.5, 16.31, 6.62]], [[45.5, -16.31, 6.37], [53.5, -13.31, 6.62]],
    [[48.25, 14.96, 6.62], [50.75, 15.21, 8.62]], [[48.25, 14.41, 6.62], [50.75, 14.65, 8.62]],
    [[48.25, -15.21, 6.62], [50.75, -14.96, 8.62]], [[48.25, -14.65, 6.62], [50.75, -14.4, 8.62]],
    [[23.5, 12.36, 44.2], [25.25, 12.61, 46.2]], [[23.5, 11.8, 44.2], [25.25, 12.05, 46.2]],
    [[23.5, -12.61, 44.2], [25.25, -12.36, 46.2]], [[23.5, -12.05, 44.2], [25.25, -11.8, 46.2]],
    [[47.35, 14.68, 6.83], [50.54, 14.93, 10.58]], [[23.46, 12.08, 42.5], [26.65, 12.33, 46.24]],
    [[47.35, -14.93, 6.83], [50.54, -14.68, 10.58]], [[23.46, -12.33, 42.5], [26.65, -12.08, 46.24]],
    [[49.31, 14.06, 7.69], [49.69, 15.56, 8.06]], [[24.31, 11.45, 45.01], [24.69, 12.95, 45.39]],
    [[49.31, -15.56, 7.69], [49.69, -14.06, 8.06]], [[24.31, -12.95, 45.01], [24.69, -11.45, 45.39]]
];
const PLATE_T = 0.5;       // aluminum peak plate
const LEG_W = 2.5;         // leg tube size
const LEG_T = 0.1875;
const BAR_W = 2;           // crossbar and brace tube size
const BAR_T = 0.1875;
const MOUNT_TOP = 6.375;   // top of the engine-bed mount plates
const LEG_TOP = 49.7;

function boxMatch(bb is Box3d, ref is array) returns boolean
{
    const lo = bb.minCorner / inch;
    const hi = bb.maxCorner / inch;
    for (var i = 0; i < 3; i += 1)
        if (abs(lo[i] - ref[0][i]) > 0.03 || abs(hi[i] - ref[1][i]) > 0.03)
            return false;
    return true;
}

// Square tube of width w and wall t from A to B; e1 sets the orientation of one pair of faces.
function squareTube(context is Context, id is Id, A is Vector, B is Vector, e1 is Vector, w is number, t is number)
{
    const axis = normalize(B - A);
    const len = norm(B - A);
    for (var k in [["o", w], ["i", w - 2 * t]])
    {
        const sid = id + (k[0] ~ "sk");
        const sk = newSketchOnPlane(context, sid, { "sketchPlane" : plane(A * inch, axis, e1) });
        skRectangle(sk, "r", { "firstCorner" : vector(-k[1] / 2, -k[1] / 2) * inch, "secondCorner" : vector(k[1] / 2, k[1] / 2) * inch });
        skSolve(sk);
        opExtrude(context, id + (k[0] ~ "ex"), { "entities" : qSketchRegion(sid), "direction" : axis,
                    "startBound" : BoundingType.BLIND, "startDepth" : (k[0] == "i" ? 0.1 : 0) * inch,
                    "endBound" : BoundingType.BLIND, "endDepth" : (len + (k[0] == "i" ? 0.1 : 0)) * inch });
        opDeleteBodies(context, id + (k[0] ~ "del"), { "entities" : qCreatedBy(sid, EntityType.BODY) });
    }
    opBoolean(context, id + "bore", { "targets" : qCreatedBy(id + "oex", EntityType.BODY), "tools" : qCreatedBy(id + "iex", EntityType.BODY), "operationType" : BooleanOperationType.SUBTRACTION });
}

// Angle (legs a x a, thickness t) of length len along `along`, corner at P, legs along e1 and e2.
function angleClip(context is Context, id is Id, P is Vector, along is Vector, e1 is Vector, e2 is Vector, a is number, t is number, len is number)
{
    const sk = newSketchOnPlane(context, id + "sk", { "sketchPlane" : plane(P * inch, along, e1) });
    const k = dot(cross(along, e1), e2) > 0 ? 1 : -1;
    var pts = [];
    for (var q in [[0, 0], [a, 0], [a, t], [t, t], [t, a], [0, a], [0, 0]])
        pts = append(pts, vector(q[0], k * q[1]) * inch);
    skPolyline(sk, "poly", { "points" : pts });
    skSolve(sk);
    opExtrude(context, id + "ex", { "entities" : qSketchRegion(id + "sk"), "direction" : along, "endBound" : BoundingType.BLIND, "endDepth" : len * inch });
    opDeleteBodies(context, id + "del", { "entities" : qCreatedBy(id + "sk", EntityType.BODY) });
}

function label(context is Context, q is Query, n is string)
{
    setProperty(context, { "entities" : q, "propertyType" : PropertyType.NAME, "value" : n });
    setProperty(context, { "entities" : q, "propertyType" : PropertyType.APPEARANCE, "value" : color(0.78, 0.8, 0.83) });
}

annotation { "Feature Type Name" : "Aluminum A-frame (bolted)" }
export const aluminumAFrame = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
    }
    {
        // ---- Find the steel parts ----
        var steel = [];
        var steelPlates = [];
        for (var b in evaluateQuery(context, qBodyType(qEverything(EntityType.BODY), BodyType.SOLID)))
        {
            const bb = evBox3d(context, { "topology" : b });
            for (var i = 0; i < size(STEEL); i += 1)
            {
                if (boxMatch(bb, STEEL[i]))
                {
                    steel = append(steel, b);
                    if (i == 8 || i == 9)
                        steelPlates = append(steelPlates, b);
                }
            }
        }
        if (size(steel) != size(STEEL))
            throw regenError("Found " ~ size(steel) ~ " of " ~ size(STEEL) ~ " steel A-frame parts; check the drivetrain position.");

        // ---- Peak plates: extrude the steel plates' inside faces outward to 1/2 in ----
        const plateX = [13.69, 22.31];          // inside faces (redrive side)
        const outward = [-1, 1];
        var newPlates = [];
        for (var p = 0; p < 2; p += 1)
        {
            var face = undefined;
            for (var pl in steelPlates)
                for (var f in evaluateQuery(context, qGeometry(qOwnedByBody(pl, EntityType.FACE), GeometryType.PLANE)))
                {
                    const pd = evPlane(context, { "face" : f });
                    if (abs(pd.origin[0] / inch - plateX[p]) < 0.01 && abs(abs(pd.normal[0]) - 1) < 1e-6 && evArea(context, { "entities" : f }) > 100 * inch ^ 2)
                        face = f;
                }
            const pid = id + ("plate" ~ p);
            opExtrude(context, pid + "ex", { "entities" : face, "direction" : vector(outward[p], 0, 0), "endBound" : BoundingType.BLIND, "endDepth" : PLATE_T * inch });
            label(context, qCreatedBy(pid, EntityType.BODY), "A-frame peak plate 1/2 6061 - " ~ (p == 0 ? "aft" : "fwd"));
            newPlates = append(newPlates, qCreatedBy(pid, EntityType.BODY));
        }
        opDeleteBodies(context, id + "steelDel", { "entities" : qUnion(steel) });

        // ---- Legs, slotted over the peak plates ----
        const legX = [plateX[0] - PLATE_T / 2, plateX[1] + PLATE_T / 2];
        for (var p = 0; p < 2; p += 1)
        {
            for (var s in [1, -1])
            {
                const lid = id + ("leg" ~ p ~ (s > 0 ? "P" : "S"));
                // The leg leans about 4 deg, so its foot is cut at that angle to sit flat on the mount plate.
                const trimId = id + ("footCut" ~ p ~ (s > 0 ? "P" : "S"));
                fCuboid(context, trimId, { "corner1" : vector(legX[p] - 3, s * 10, MOUNT_TOP - 2) * inch, "corner2" : vector(legX[p] + 3, s * 19, MOUNT_TOP) * inch });
                // Steel leg centerline: Y 14.81 at the foot to 11.9 at the top
                const A = vector(legX[p], s * (14.81 - 2.91 * (MOUNT_TOP - 6.62) / 43.08), MOUNT_TOP - 0.2);
                const B = vector(legX[p], s * 11.9, LEG_TOP);
                squareTube(context, lid, A, B, vector(-1, 0, 0), LEG_W, LEG_T);
                opBoolean(context, lid + "slot", { "targets" : qCreatedBy(lid, EntityType.BODY), "tools" : newPlates[p], "operationType" : BooleanOperationType.SUBTRACTION, "keepTools" : true });
                opBoolean(context, lid + "foot", { "targets" : qCreatedBy(lid, EntityType.BODY), "tools" : qCreatedBy(trimId, EntityType.BODY), "operationType" : BooleanOperationType.SUBTRACTION });
                label(context, qCreatedBy(lid, EntityType.BODY), "A-frame leg 2.5x2.5x3/16 6061 - " ~ (p == 0 ? "aft " : "fwd ") ~ (s > 0 ? "port" : "stbd"));

                // Foot clips on the aft and forward faces, over the mount-plate bolts
                for (var e in [-1, 1])
                {
                    const cid = id + ("clip" ~ p ~ (s > 0 ? "P" : "S") ~ (e > 0 ? "f" : "a"));
                    const cx = legX[p] + e * LEG_W / 2;
                    angleClip(context, cid, vector(cx, s * 13.56, MOUNT_TOP), vector(0, s, 0), vector(e, 0, 0), vector(0, 0, 1), 2, 0.25, 2.5);
                    label(context, qCreatedBy(cid, EntityType.BODY), "A-frame foot clip 2x2x1/4 6061 angle");
                }
            }
            // Crossbar between the leg inside faces, at the steel crossbar height
            const zc = p == 0 ? 28.7 : 29.46;
            const yIn = 14.81 - 2.91 * (zc - 6.62) / 43.08 - LEG_W / 2 - 0.05;
            const bid = id + ("bar" ~ p);
            squareTube(context, bid, vector(legX[p], -yIn, zc), vector(legX[p], yIn, zc), vector(1, 0, 0), BAR_W, BAR_T);
            label(context, qCreatedBy(bid, EntityType.BODY), "A-frame crossbar 2x2x3/16 6061 - " ~ (p == 0 ? "aft" : "fwd"));
        }

        // ---- Braces from the forward legs to the brace mount plates ----
        for (var s in [1, -1])
        {
            const sid = id + ("brace" ~ (s > 0 ? "P" : "S"));
            const top = vector(legX[1] + LEG_W / 2 + 1.2, s * 12.0, 44.0);
            const bot = vector(48.6, s * 14.6, 9.3);
            const ax = normalize(bot - top);
            squareTube(context, sid, top, bot, normalize(cross(ax, vector(0, 1, 0))), BAR_W, BAR_T);
            label(context, qCreatedBy(sid, EntityType.BODY), "A-frame brace 2x2x3/16 6061 - " ~ (s > 0 ? "port" : "stbd"));

            // Top bracket: angle on the forward leg's front face, outboard of the brace
            const tbid = id + ("braceTop" ~ (s > 0 ? "P" : "S"));
            angleClip(context, tbid, vector(legX[1] + LEG_W / 2, s * (12.0 + BAR_W / 2), 42.5), vector(0, 0, 1), vector(1, 0, 0), vector(0, s, 0), 2, 0.25, 3);
            label(context, qCreatedBy(tbid, EntityType.BODY), "A-frame brace top bracket 2x2x1/4 6061 angle");

            // Bottom shoe: angle bolted to the brace mount plate, upright leg outboard of the brace
            const bsid = id + ("braceShoe" ~ (s > 0 ? "P" : "S"));
            angleClip(context, bsid, vector(45.5, s * (14.6 + BAR_W / 2 + 0.25), MOUNT_TOP), vector(1, 0, 0), vector(0, -s, 0), vector(0, 0, 1), 2, 0.25, 8);
            label(context, qCreatedBy(bsid, EntityType.BODY), "A-frame brace shoe 2x2x1/4 6061 angle");
        }
    });
