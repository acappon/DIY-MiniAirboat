FeatureScript 3112;
import(path : "onshape/std/geometry.fs", version : "3112.0");

// Provisional layout of the minimal 1448 with the Rotax 800R E-TEC, in the airboat assembly frame:
// X forward from the transom, Y + to port, Z up from the hull bottom (reports/Drivetrain_800R_ETEC.md).
//   - Reference hull section, transom, rear bench and rib, three 3/16 in UHMW wear strips
//   - Engine, pipe, silencer, second-muffler space, tailpipe and radiator as envelopes
//   - 8MGT 32T/94T belt, prop shaft, bearings, bearing plates, 66 in prop disc
//   - Bolted stand with cross-straps on four separate shoes (solid placeholders for the tube sizes in their part names)
//   - Hull mounts: shoe doublers, transom and bench clips, cage foot pads
//   - Exhaust heat shields
//   - Smaller cage, rudders, cage-top lights and the Florida flag
// Engine, pipe and radiator sizes are placeholders until the real parts are measured. The exhaust
// is assumed on the port side and the engine roll angle is 0. The cage is a proposal. The reference
// transom is square at X 0; the real one rakes from about X 1.5 at Z 2 to about X -1.7 at Z 14.

const CRANK_Z = 22.72;       // raised 0.72 in so the stock 8MGT-2000 belt fits at 29.28 in centers
const PROP_Z = 52;
const BELT_X = [12, 14];        // 2 in envelope: 36 mm belt, about 1.4 in, between pulley flanges
const R_SMALL = 1.605;          // 32T 8MGT pitch radius
const R_LARGE = 4.71;           // 94T 8MGT pitch radius
const PROP_R = 33;              // 66 in
const SWEEP_X = [4, 6];         // blade sweep, assumed
const HUB_R = 3.5;
const STAND_Y = 8.5;            // side frame centerlines
const CAGE_R = 36;              // 2 in or more outside the tips, plus the tube
const CAGE_X = [1, 11];
const CAGE_FOOT = [[1, 17.975], [14.5, 13.825]]; // [X, Z] of the aft (corner cap) and forward (bench top) feet
const CAGE_Y = 30.5;
const BENCH = [13, 33, 2.4, 13.7]; // rear bench X and Z range, from the reference mesh
const RIB_X = [36, 37, 38.5, 39.5]; // rib hat at the stand's forward foot, from the reference mesh
const RIB_TOP = 1.52;
const TRANSOM_Z = 15;
const HW_BOTTOM = 24.03;
const SIDE_FLARE = 0.515;
const BOTTOM = [[0, 0], [101, 0], [112, 1.25], [128, 3.75]]; // flat bottom and bow rise, same as Hull armor
const STRIP_T = 0.1875;

function mkBox(context is Context, id is Id, x0 is number, x1 is number, y0 is number, y1 is number, z0 is number, z1 is number)
{
    fCuboid(context, id, { "corner1" : vector(min(x0, x1), min(y0, y1), min(z0, z1)) * inch, "corner2" : vector(max(x0, x1), max(y0, y1), max(z0, z1)) * inch });
}

function mkCyl(context is Context, id is Id, a is Vector, b is Vector, r is number)
{
    fCylinder(context, id, { "bottomCenter" : a * inch, "topCenter" : b * inch, "radius" : r * inch });
}

function paintBody(context is Context, q is Query, name is string, c is Color)
{
    setProperty(context, { "entities" : q, "propertyType" : PropertyType.NAME, "value" : name });
    setProperty(context, { "entities" : q, "propertyType" : PropertyType.APPEARANCE, "value" : c });
}

// Square bar of width w from point a to point b.
function mkBar(context is Context, id is Id, a is Vector, b is Vector, w is number)
{
    const d = normalize(b - a);
    mkFlat(context, id, a, b, w, w, abs(d[1]) > 0.9 ? vector(1, 0, 0) : vector(0, 1, 0));
}

// Flat bar from point a to point b, w wide along xDir and t thick.
function mkFlat(context is Context, id is Id, a is Vector, b is Vector, w is number, t is number, xDir is Vector)
{
    const d = normalize(b - a);
    const sk = newSketchOnPlane(context, id + "sk", { "sketchPlane" : plane(a * inch, d, xDir) });
    skRectangle(sk, "rect", { "firstCorner" : vector(-w / 2, -t / 2) * inch, "secondCorner" : vector(w / 2, t / 2) * inch });
    skSolve(sk);
    opExtrude(context, id + "ex", {
                "entities" : qSketchRegion(id + "sk"),
                "direction" : d,
                "endBound" : BoundingType.BLIND,
                "endDepth" : norm(b - a) * inch
            });
    opDeleteBodies(context, id + "del", { "entities" : qCreatedBy(id + "sk", EntityType.BODY) });
}

// Closed Y-Z polygon extruded along X from x0 to x1.
function mkPoly(context is Context, id is Id, pts is array, x0 is number, x1 is number)
{
    const sk = newSketchOnPlane(context, id + "sk", { "sketchPlane" : plane(vector(x0, 0, 0) * inch, vector(1, 0, 0), vector(0, 1, 0)) });
    var p = [];
    for (var q in pts)
        p = append(p, vector(q[0], q[1]) * inch);
    skPolyline(sk, "poly", { "points" : p });
    skSolve(sk);
    opExtrude(context, id + "ex", {
                "entities" : qSketchRegion(id + "sk"),
                "direction" : vector(1, 0, 0),
                "endBound" : BoundingType.BLIND,
                "endDepth" : (x1 - x0) * inch
            });
    opDeleteBodies(context, id + "del", { "entities" : qCreatedBy(id + "sk", EntityType.BODY) });
}

// Round pipe through a list of points, one cylinder per run.
function mkPipe(context is Context, id is Id, pts is array, r is number)
{
    for (var i = 0; i < size(pts) - 1; i += 1)
        mkCyl(context, id + ("seg" ~ i), pts[i], pts[i + 1], r);
}

// Rounded rectangle in the Y-Z plane, as polygon points.
function roundRect(y0 is number, y1 is number, z0 is number, z1 is number, r is number) returns array
{
    var pts = [];
    const corners = [[y1 - r, z1 - r, 0], [y0 + r, z1 - r, 90], [y0 + r, z0 + r, 180], [y1 - r, z0 + r, 270]];
    for (var c in corners)
        for (var i = 0; i <= 6; i += 1)
        {
            const a = (c[2] + 15 * i) * degree;
            pts = append(pts, [c[0] + r * cos(a), c[1] + r * sin(a)]);
        }
    return append(pts, pts[0]);
}

// Closed X-Z polygon extruded along Y from y0 to y1.
function mkPolyXZ(context is Context, id is Id, pts is array, y0 is number, y1 is number)
{
    const sk = newSketchOnPlane(context, id + "sk", { "sketchPlane" : plane(vector(0, y0, 0) * inch, vector(0, 1, 0), vector(1, 0, 0)) });
    var p = [];
    for (var q in pts)
        p = append(p, vector(q[0], -q[1]) * inch); // sketch y is -Z on this plane
    skPolyline(sk, "poly", { "points" : p });
    skSolve(sk);
    opExtrude(context, id + "ex", {
                "entities" : qSketchRegion(id + "sk"),
                "direction" : vector(0, 1, 0),
                "endBound" : BoundingType.BLIND,
                "endDepth" : (y1 - y0) * inch
            });
    opDeleteBodies(context, id + "del", { "entities" : qCreatedBy(id + "sk", EntityType.BODY) });
}

// Outline around the two pulleys in the Y-Z plane, offset out by `off`.
function beltOutline(off is number) returns array
{
    const rA = R_SMALL + off;
    const rB = R_LARGE + off;
    const a = asin((R_LARGE - R_SMALL) / (PROP_Z - CRANK_Z)) / radian;
    const n = 12;
    var pts = [];
    for (var i = 0; i <= n; i += 1)
    {
        const t = -a + (PI + 2 * a) * i / n;
        pts = append(pts, [rB * cos(t * radian), PROP_Z + rB * sin(t * radian)]);
    }
    for (var i = 0; i <= n; i += 1)
    {
        const t = PI + a + (PI - 2 * a) * i / n;
        pts = append(pts, [rA * cos(t * radian), CRANK_Z + rA * sin(t * radian)]);
    }
    return append(pts, pts[0]);
}

annotation { "Feature Type Name" : "Minimal 800R layout" }
export const minimal800RLayout = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        annotation { "Name" : "Include reference hull", "Default" : true }
        definition.showHull is boolean;

        annotation { "Name" : "Include cage, rudders, lights and flag", "Default" : true }
        definition.showCage is boolean;
    }
    {
        const envC = color(0.9, 0.55, 0.2);
        const hotC = color(0.75, 0.3, 0.2);
        const reserveC = color(0.95, 0.8, 0.3, 0.45);
        const steelC = color(0.45, 0.47, 0.5);
        const alu = color(0.75, 0.77, 0.8);
        const plateC = color(0.62, 0.66, 0.72);
        const beltC = color(0.15, 0.15, 0.17);
        const propC = color(0.3, 0.5, 0.9, 0.3);
        const hullC = color(0.8, 0.82, 0.78, 0.35);
        const cageC = color(0.25, 0.6, 0.35);
        const orange = color(1, 0.4, 0);

        // ---- Reference hull ----
        if (definition.showHull)
        {
            const wT = HW_BOTTOM + SIDE_FLARE * TRANSOM_Z;
            const t = 0.125;
            mkPoly(context, id + "hullSec", [[-HW_BOTTOM, 0], [HW_BOTTOM, 0], [wT, TRANSOM_Z], [wT - t, TRANSOM_Z], [HW_BOTTOM - t, t], [-HW_BOTTOM + t, t], [-wT + t, TRANSOM_Z], [-wT, TRANSOM_Z], [-HW_BOTTOM, 0]], 0, 100);
            paintBody(context, qCreatedBy(id + "hullSec", EntityType.BODY), "Reference - hull section at the transom", hullC);
            mkPoly(context, id + "transom", [[-HW_BOTTOM, 0], [HW_BOTTOM, 0], [wT, TRANSOM_Z], [-wT, TRANSOM_Z], [-HW_BOTTOM, 0]], -0.125, 0);
            paintBody(context, qCreatedBy(id + "transom", EntityType.BODY), "Reference - transom", hullC);
            const wB0 = HW_BOTTOM + SIDE_FLARE * BENCH[2];
            const wB1 = HW_BOTTOM + SIDE_FLARE * BENCH[3];
            mkPoly(context, id + "bench", [[-wB0, BENCH[2]], [wB0, BENCH[2]], [wB1, BENCH[3]], [-wB1, BENCH[3]], [-wB0, BENCH[2]]], BENCH[0], BENCH[1]);
            paintBody(context, qCreatedBy(id + "bench", EntityType.BODY), "Reference - rear bench", hullC);
            mkPolyXZ(context, id + "rib", [[RIB_X[0], 0], [RIB_X[3], 0], [RIB_X[3], 0.1], [RIB_X[2], 0.1], [RIB_X[2], RIB_TOP], [RIB_X[1], RIB_TOP], [RIB_X[1], 0.1], [RIB_X[0], 0.1], [RIB_X[0], 0]], -HW_BOTTOM, HW_BOTTOM);
            paintBody(context, qCreatedBy(id + "rib", EntityType.BODY), "Reference - rib at X 37.75", hullC);
        }

        // ---- UHMW wear strips: centerline and both chines, 3/16 x 12 in ----
        const stripC = color(0.12, 0.12, 0.12);
        var stripPts = BOTTOM;
        for (var i = size(BOTTOM) - 1; i >= 0; i -= 1)
            stripPts = append(stripPts, [BOTTOM[i][0], BOTTOM[i][1] - STRIP_T]);
        stripPts = append(stripPts, BOTTOM[0]);
        for (var st in [[-6, 6, "centerline"], [12.03, 24.03, "port chine"], [-24.03, -12.03, "stbd chine"]])
        {
            const sid = id + ("strip" ~ round(st[0] + 30));
            mkPolyXZ(context, sid, stripPts, st[0], st[1]);
            paintBody(context, qCreatedBy(sid, EntityType.BODY), "UHMW strip 3/16 - " ~ st[2], stripC);
        }

        // ---- Engine, crank along X, cylinders up, PTO aft ----
        mkBox(context, id + "case", 14, 30, -7, 7, CRANK_Z - 6, CRANK_Z + 6);
        paintBody(context, qCreatedBy(id + "case", EntityType.BODY), "Envelope - 800R crankcase", envC);
        mkBox(context, id + "cyl", 16, 28, -5.5, 5.5, CRANK_Z + 6, CRANK_Z + 18);
        paintBody(context, qCreatedBy(id + "cyl", EntityType.BODY), "Envelope - 800R cylinders", envC);

        // ---- Belt and pulleys ----
        mkPoly(context, id + "beltCore", beltOutline(0), BELT_X[0] - 0.1, BELT_X[1] + 0.1);
        const bl = id + "belt";
        mkPoly(context, bl + "outer", beltOutline(0.12), BELT_X[0], BELT_X[1]);
        opBoolean(context, bl + "hollow", { "targets" : qCreatedBy(bl + "outer", EntityType.BODY), "tools" : qCreatedBy(id + "beltCore", EntityType.BODY), "operationType" : BooleanOperationType.SUBTRACTION });
        paintBody(context, qCreatedBy(bl, EntityType.BODY), "Belt 8MGT-2000-36, shown in its 2 in pulley envelope", beltC);
        mkCyl(context, id + "pSmall", vector(BELT_X[0], 0, CRANK_Z), vector(BELT_X[1], 0, CRANK_Z), R_SMALL);
        paintBody(context, qCreatedBy(id + "pSmall", EntityType.BODY), "Pulley 32T on the crank PTO", steelC);
        mkCyl(context, id + "pLarge", vector(BELT_X[0], 0, PROP_Z), vector(BELT_X[1], 0, PROP_Z), R_LARGE);
        paintBody(context, qCreatedBy(id + "pLarge", EntityType.BODY), "Pulley 94T", steelC);

        // ---- Prop shaft, bearings, bearing plates ----
        mkCyl(context, id + "shaft", vector(SWEEP_X[0] + 3, 0, PROP_Z), vector(22, 0, PROP_Z), 0.75);
        paintBody(context, qCreatedBy(id + "shaft", EntityType.BODY), "Prop shaft 1-1/2 4140", steelC);
        for (var b in [[7, 9, 2.55, "Aft bearing UCF208-24"], [18.5, 21.5, 2.75, "Forward bearing tapered roller 4-bolt"]])
        {
            const bid = id + ("brg" ~ round(b[0]));
            mkBox(context, bid, b[0], b[1], -b[2], b[2], PROP_Z - b[2], PROP_Z + b[2]);
            paintBody(context, qCreatedBy(bid, EntityType.BODY), b[3], plateC);
        }
        // Two petals each side of the bearing, a 1/2 in rib between them, a 1 in border, and solid ends for the
        // tensioning slots (CAD/analysis/bearing_plate.js).
        for (var bp in [[8, 7.5, 0.375, "3/8"], [20, 7.75, 0.5, "1/2"]])
        {
            const pid = id + ("bplate" ~ bp[0]);
            mkBox(context, pid, bp[0], bp[0] + bp[2], -bp[1], bp[1], PROP_Z - 3, PROP_Z + 3);
            const zone = bp[1] - 1.5 - 3.1;
            const w = (zone - 0.5) / 2;
            var tools = [];
            for (var s in [1, -1])
                for (var k = 0; k < 2; k += 1)
                {
                    const a = 3.1 + k * (w + 0.5);
                    const cid = id + ("petal" ~ bp[0] ~ (s > 0 ? "P" : "S") ~ k);
                    mkPoly(context, cid, roundRect(s > 0 ? a : -a - w, s > 0 ? a + w : -a, PROP_Z - 2, PROP_Z + 2, 0.5), bp[0] - 0.1, bp[0] + bp[2] + 0.1);
                    tools = append(tools, qCreatedBy(cid, EntityType.BODY));
                }
            opBoolean(context, id + ("bcut" ~ bp[0]), { "targets" : qCreatedBy(pid, EntityType.BODY), "tools" : qUnion(tools), "operationType" : BooleanOperationType.SUBTRACTION });
            paintBody(context, qCreatedBy(pid, EntityType.BODY), "Bearing plate " ~ bp[3] ~ " 6061 with petal cutouts at X " ~ bp[0], plateC);
        }

        // ---- Prop ----
        mkCyl(context, id + "hubCore", vector(SWEEP_X[0] - 0.1, 0, PROP_Z), vector(SWEEP_X[1] + 0.1, 0, PROP_Z), HUB_R);
        const dd = id + "disc";
        mkCyl(context, dd + "c", vector(SWEEP_X[0], 0, PROP_Z), vector(SWEEP_X[1], 0, PROP_Z), PROP_R);
        opBoolean(context, dd + "bore", { "targets" : qCreatedBy(dd + "c", EntityType.BODY), "tools" : qCreatedBy(id + "hubCore", EntityType.BODY), "operationType" : BooleanOperationType.SUBTRACTION });
        paintBody(context, qCreatedBy(dd, EntityType.BODY), "Prop 66 in blade sweep", propC);
        mkCyl(context, id + "hub", vector(SWEEP_X[0] - 1, 0, PROP_Z), vector(SWEEP_X[1] + 1, 0, PROP_Z), HUB_R);
        paintBody(context, qCreatedBy(id + "hub", EntityType.BODY), "Prop hub and flange", steelC);

        // ---- Stand and hull mounts (CAD/analysis/stand_800r.js) ----
        // No bed runs under the rear bench: the rail closes the side frame above it and the feet stand on separate shoes.
        const bedTop = 3.125;
        const topZ = 56;
        const railZ = 14.9; // between the bench top (0.2 in below) and the forward hoop (0.25 in above)
        const railEnd = 21 + 19 * (topZ - railZ) / (topZ - bedTop);
        const frames = [
            ["postAft", [9, bedTop], [9, topZ + 1], 2],
            ["postFwd", [21, railZ], [21, topZ + 0.75], 1.5],
            ["diag", [21, topZ], [40, bedTop], 1.5],
            ["top", [9, topZ], [21, topZ], 1.5],
            ["rail", [9, railZ], [railEnd, railZ], 2]
        ];
        for (var s in [1, -1])
        {
            const side = s > 0 ? "Port" : "Stbd";
            const yc = s * STAND_Y;
            mkBox(context, id + ("shoeAft" ~ side), 2, 12.75, yc - 1, yc + 1, 0.125, bedTop);
            paintBody(context, qCreatedBy(id + ("shoeAft" ~ side), EntityType.BODY), "Aft shoe 2x3x1/8 6061 - " ~ side, alu);
            const nA = RIB_X[0] - 0.15;
            const nB = RIB_X[3] + 0.15;
            mkPolyXZ(context, id + ("shoeFwd" ~ side), [[34, 0.125], [nA, 0.125], [nA, RIB_TOP + 0.08], [nB, RIB_TOP + 0.08], [nB, 0.125], [44, 0.125], [44, bedTop], [34, bedTop], [34, 0.125]], yc - 1, yc + 1);
            paintBody(context, qCreatedBy(id + ("shoeFwd" ~ side), EntityType.BODY), "Forward shoe 2x3x1/8 6061, notched over the rib - " ~ side, alu);
            mkBox(context, id + ("dblAft" ~ side), 2.25, 12.75, yc - 2.5, yc + 2.5, 0, 0.125);
            paintBody(context, qCreatedBy(id + ("dblAft" ~ side), EntityType.BODY), "Bottom doubler 1/8 5052 - aft " ~ side, plateC);
            mkBox(context, id + ("dblFwdA" ~ side), 33.5, nA, yc - 3, yc + 3, 0, 0.125);
            paintBody(context, qCreatedBy(id + ("dblFwdA" ~ side), EntityType.BODY), "Bottom doubler 1/8 5052 - forward A " ~ side, plateC);
            mkBox(context, id + ("dblFwdB" ~ side), nB, 44.5, yc - 3, yc + 3, 0, 0.125);
            paintBody(context, qCreatedBy(id + ("dblFwdB" ~ side), EntityType.BODY), "Bottom doubler 1/8 5052 - forward B " ~ side, plateC);
            // 2 x 2 x 1/8 angle clips, 3 in wide: the aft shoe hangs between the transom and the rear bench's aft panel,
            // so uplift at the aft post goes into those panels and not into the bottom skin.
            mkPolyXZ(context, id + ("clipT" ~ side), [[1.1, bedTop], [3.1, bedTop], [3.1, bedTop + 0.125], [1.225, bedTop + 0.125], [1.225, bedTop + 2], [1.1, bedTop + 2], [1.1, bedTop]], yc - 1.5, yc + 1.5);
            paintBody(context, qCreatedBy(id + ("clipT" ~ side), EntityType.BODY), "Transom clip 2x2x1/8 angle - " ~ side, plateC);
            mkPolyXZ(context, id + ("clipB" ~ side), [[11, bedTop], [13, bedTop], [13, bedTop + 2], [12.875, bedTop + 2], [12.875, bedTop + 0.125], [11, bedTop + 0.125], [11, bedTop]], yc - 1.5, yc + 1.5);
            paintBody(context, qCreatedBy(id + ("clipB" ~ side), EntityType.BODY), "Bench clip 2x2x1/8 angle - " ~ side, plateC);
            for (var f in frames)
            {
                const fid = id + (f[0] ~ side);
                mkBar(context, fid, vector(f[1][0], yc, f[1][1]), vector(f[2][0], yc, f[2][1]), f[3]);
                paintBody(context, qCreatedBy(fid, EntityType.BODY), "Stand " ~ f[0] ~ " " ~ (f[3] == 2 ? "2x2x1/8" : "1-1/2x1-1/2x1/8") ~ " 6061 - " ~ side, alu);
            }
        }
        const ties = [[9, railZ, 7.5], [9, topZ, 7.5], [21, topZ, 7.75]];
        for (var i = 0; i < size(ties); i += 1)
        {
            const tid = id + ("tie" ~ i);
            mkBar(context, tid, vector(ties[i][0], -ties[i][2], ties[i][1]), vector(ties[i][0], ties[i][2], ties[i][1]), 1.5);
            paintBody(context, qCreatedBy(tid, EntityType.BODY), "Stand tie 1-1/2x1-1/2x1/8 6061 - " ~ (i + 1), alu);
        }
        // Tension straps, an X on the forward face of the diagonals below the cylinders, for side-to-side bracing.
        const nDiag = normalize(vector(topZ - bedTop, 0, 19));
        const diagPt = function(t, y) { return vector(21 + 19 * t, y, topZ - (topZ - bedTop) * t); };
        for (var sp in [[-1, 0.81, "A"], [1, 0.94, "B"]])
        {
            const a = diagPt(0.36, sp[0] * STAND_Y) + nDiag * sp[1];
            const b = diagPt(0.9, -sp[0] * STAND_Y) + nDiag * sp[1];
            const spid = id + ("strap" ~ sp[2]);
            mkFlat(context, spid, a, b, 1, 0.125, normalize(cross(normalize(b - a), nDiag)));
            paintBody(context, qCreatedBy(spid, EntityType.BODY), "Stand cross-strap 1x1/8 6061 - " ~ sp[2], plateC);
        }

        // ---- Exhaust, port side ----
        mkPipe(context, id + "pipe", [vector(25.5, 7, 30), vector(25.5, 14, 32), vector(29, 14, 38), vector(29, 14, 52), vector(17, 14, 54)], 2.5);
        paintBody(context, qCreatedBy(id + "pipe", EntityType.BODY), "Envelope - stock tuned pipe", hotC);
        mkCyl(context, id + "silencer", vector(15, 15.5, 45), vector(26, 15.5, 45), 3.5);
        paintBody(context, qCreatedBy(id + "silencer", EntityType.BODY), "Envelope - stock silencer", hotC);

        // Heat shields, 0.040 in aluminum sheet on standoffs: one between the exhaust and the stand,
        // belt and bearings, and one each side of the pipe where it passes through the side frame.
        const shieldC = color(0.85, 0.87, 0.9);
        mkBox(context, id + "shieldPort", 11, 31, 10.45, 10.49, 35, 57.5);
        paintBody(context, qCreatedBy(id + "shieldPort", EntityType.BODY), "Heat shield - stand and belt side", shieldC);
        mkBox(context, id + "shieldPost", 22.3, 22.34, 7.5, 10.4, 26, 36);
        paintBody(context, qCreatedBy(id + "shieldPost", EntityType.BODY), "Heat shield - forward post", shieldC);
        mkPolyXZ(context, id + "shieldDiag", [[30.53, 26], [26.94, 36], [26.98, 36], [30.57, 26], [30.53, 26]], 7.5, 10.4);
        paintBody(context, qCreatedBy(id + "shieldDiag", EntityType.BODY), "Heat shield - diagonal", shieldC);

        mkCyl(context, id + "muffler", vector(18, 0, 45.5), vector(36, 0, 45.5), 3);
        paintBody(context, qCreatedBy(id + "muffler", EntityType.BODY), "Reserved - second muffler", reserveC);
        // Runs outside the hoops and inside the cage legs.
        mkPipe(context, id + "tail", [vector(15, 15.5, 45), vector(15, 26, 26), vector(13, 27.5, 23), vector(-1, 28, 16.5)], 1);
        paintBody(context, qCreatedBy(id + "tail", EntityType.BODY), "Tailpipe to the port transom corner", hotC);

        // ---- Radiator, starboard, just ahead of the prop where the inflow is strong, clear of the exhaust ----
        mkBox(context, id + "radiator", 15, 17, -28, -10, 34, 56);
        paintBody(context, qCreatedBy(id + "radiator", EntityType.BODY), "Envelope - radiator 18 x 22 x 2", color(0.3, 0.45, 0.7));

        // ---- Cage, rudders, lights, flag ----
        if (definition.showCage)
        {
            for (var cx in CAGE_X)
            {
                const hid = id + ("hoop" ~ cx);
                mkCyl(context, id + ("hoopCore" ~ cx), vector(cx - 0.7, 0, PROP_Z), vector(cx + 0.7, 0, PROP_Z), CAGE_R - 0.625);
                mkCyl(context, hid + "c", vector(cx - 0.625, 0, PROP_Z), vector(cx + 0.625, 0, PROP_Z), CAGE_R + 0.625);
                opBoolean(context, hid + "bore", { "targets" : qCreatedBy(hid + "c", EntityType.BODY), "tools" : qCreatedBy(id + ("hoopCore" ~ cx), EntityType.BODY), "operationType" : BooleanOperationType.SUBTRACTION });
                paintBody(context, qCreatedBy(hid, EntityType.BODY), "Cage hoop at X " ~ cx, cageC);
                const ft = cx == CAGE_X[0] ? CAGE_FOOT[0] : CAGE_FOOT[1];
                for (var s in [1, -1])
                {
                    const lid = id + ("leg" ~ cx ~ (s > 0 ? "P" : "S"));
                    mkBar(context, lid, vector(ft[0], s * CAGE_Y, ft[1]), vector(cx, s * CAGE_Y, PROP_Z - sqrt(CAGE_R * CAGE_R - CAGE_Y * CAGE_Y)), 1.25);
                    paintBody(context, qCreatedBy(lid, EntityType.BODY), "Cage leg at X " ~ cx, cageC);
                }
            }
            // Foot pads, 1/8 in 6061 with a matching backing plate: aft legs on the transom corner caps,
            // forward legs on the bench top next to the hull side.
            for (var s in [1, -1])
            {
                const side = s > 0 ? "Port" : "Stbd";
                mkBox(context, id + ("padAft" ~ side), -1, 3, s * 29.75, s * 32.75, CAGE_FOOT[0][1] - 0.125, CAGE_FOOT[0][1]);
                paintBody(context, qCreatedBy(id + ("padAft" ~ side), EntityType.BODY), "Cage foot pad, corner cap - " ~ side, plateC);
                mkBox(context, id + ("padFwd" ~ side), 13.25, 18.25, s * 27.5, s * 31.5, BENCH[3], CAGE_FOOT[1][1]);
                paintBody(context, qCreatedBy(id + ("padFwd" ~ side), EntityType.BODY), "Cage foot pad, bench top - " ~ side, plateC);
            }
            // Struts from the forward hoop to the top of the stand: the hoops alone stand on a 10 in base.
            for (var s in [1, -1])
            {
                const uid = id + ("strut" ~ (s > 0 ? "P" : "S"));
                mkCyl(context, uid, vector(CAGE_X[1], s * 25.46, 77.46), vector(21, s * STAND_Y, 56.75), 0.5);
                paintBody(context, qCreatedBy(uid, EntityType.BODY), "Cage strut 1x0.065 6061 - " ~ (s > 0 ? "Port" : "Stbd"), cageC);
            }
            for (var k = 0; k < 8; k += 1)
            {
                const t = (22.5 + 45 * k) * degree;
                const sid = id + ("str" ~ k);
                mkCyl(context, sid, vector(CAGE_X[0], CAGE_R * cos(t), PROP_Z + CAGE_R * sin(t)), vector(CAGE_X[1], CAGE_R * cos(t), PROP_Z + CAGE_R * sin(t)), 0.375);
                paintBody(context, qCreatedBy(sid, EntityType.BODY), "Cage stringer " ~ (k + 1), cageC);
            }
            for (var s in [1, -1])
            {
                const rid = id + ("rudder" ~ (s > 0 ? "P" : "S"));
                mkBox(context, rid, -10, -2, s * 14 - 0.25, s * 14 + 0.25, 21, 51);
                paintBody(context, qCreatedBy(rid, EntityType.BODY), "Envelope - rudder", plateC);
            }
            const topCage = PROP_Z + CAGE_R + 0.625;
            mkBox(context, id + "lightBar", 7, 9, -10, 10, topCage, topCage + 3);
            paintBody(context, qCreatedBy(id + "lightBar", EntityType.BODY), "Envelope - LED light bar 20 in", steelC);
            mkBox(context, id + "searchlight", 3, 7, -4, 4, topCage, topCage + 8);
            paintBody(context, qCreatedBy(id + "searchlight", EntityType.BODY), "Envelope - remote searchlight", steelC);
            mkCyl(context, id + "whip", vector(-0.5, 0, topCage - 1), vector(-0.5, 0, 132), 0.25);
            paintBody(context, qCreatedBy(id + "whip", EntityType.BODY), "Flag whip", steelC);
            mkBox(context, id + "flag", -12.5, -0.5, -0.05, 0.05, 120, 130);
            paintBody(context, qCreatedBy(id + "flag", EntityType.BODY), "Florida flag 10 x 12 orange", orange);
        }
    });
