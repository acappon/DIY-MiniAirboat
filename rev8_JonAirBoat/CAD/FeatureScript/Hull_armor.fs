FeatureScript 3112;
import(path : "onshape/std/geometry.fs", version : "3112.0");

// Hull armor for the Lowe 1448 reference mesh:
//   - 1/4" UHMW bottom strips (4 fore-aft strips, staggered seams on ribs)
//   - #10-24 flat-head screws, fender washers + nylock nuts through the rib bottom flanges
//   - Formed aluminum chine angles covering the outer UHMW edges
//   - Aluminum bow skin wrapping over the bow and down past the UHMW leading edge
// Hull frame: X forward from transom, Z up from the flat bottom, centerline at Y = YC.

const YC = -36.25;

// Bottom centerline profile (x, z): flat bottom, then three planar bow-rise panels.
const ZB = [[-10, 0], [101, 0], [112, 1.25], [128, 3.75], [158.17, 9.28]];

// Chine half-width at the bottom surface.
const HW0 = [[0, 24.03], [30, 24.2], [60, 24.38], [78, 24.47], [90, 24.44], [100, 24.49], [105, 24.41],
        [110, 24.37], [115, 24.27], [120, 24.18], [125, 24.09], [130, 23.95], [135, 23.78], [140, 23.62],
        [145, 23.44], [150, 23.27], [155, 23.11], [158.2, 23.0]];

const RIBS = [37.85, 54.75, 70.75, 95.25, 108.25, 122.25];
const INNER_SEAM = 54.75;
const OUTER_SEAM = 95.25;
const INNER_BOLTS = [3.125, 9.125];
const OUTER_BOLTS = [15.875, 20.875];

function interp(t is array, x is number) returns number
{
    if (x <= t[0][0])
        return t[0][1];
    for (var i = 1; i < size(t); i += 1)
    {
        if (x <= t[i][0])
        {
            const f = (x - t[i - 1][0]) / (t[i][0] - t[i - 1][0]);
            return t[i - 1][1] + f * (t[i][1] - t[i - 1][1]);
        }
    }
    return t[size(t) - 1][1];
}

function slopeAt(x is number) returns number
{
    for (var i = 1; i < size(ZB); i += 1)
    {
        if (x <= ZB[i][0])
            return (ZB[i][1] - ZB[i - 1][1]) / (ZB[i][0] - ZB[i - 1][0]);
    }
    return 0;
}

// Offset a 2D polyline by d along its left normal, mitering the corners.
function offsetPoly(pts is array, d is number) returns array
{
    const n = size(pts);
    var res = [];
    for (var i = 0; i < n; i += 1)
    {
        var nPrev = undefined;
        var nNext = undefined;
        if (i > 0)
        {
            const dx = pts[i][0] - pts[i - 1][0];
            const dz = pts[i][1] - pts[i - 1][1];
            const L = sqrt(dx * dx + dz * dz);
            nPrev = [-dz / L, dx / L];
        }
        if (i < n - 1)
        {
            const dx = pts[i + 1][0] - pts[i][0];
            const dz = pts[i + 1][1] - pts[i][1];
            const L = sqrt(dx * dx + dz * dz);
            nNext = [-dz / L, dx / L];
        }
        if (nPrev == undefined)
            nPrev = nNext;
        if (nNext == undefined)
            nNext = nPrev;
        var m = [nPrev[0] + nNext[0], nPrev[1] + nNext[1]];
        const ml = sqrt(m[0] * m[0] + m[1] * m[1]);
        m = [m[0] / ml, m[1] / ml];
        const c = m[0] * nNext[0] + m[1] * nNext[1];
        res = append(res, [pts[i][0] + d * m[0] / c, pts[i][1] + d * m[1] / c]);
    }
    return res;
}

function reversed(a is array) returns array
{
    var r = [];
    for (var i = size(a) - 1; i >= 0; i -= 1)
        r = append(r, a[i]);
    return r;
}

function concat(a is array, b is array) returns array
{
    var r = a;
    for (var v in b)
        r = append(r, v);
    return r;
}

// Sketch a closed polygon on a plane and extrude it; returns the new body.
function polyExtrude(context is Context, id is Id, pts is array, pl is Plane, dEnd is number, dStart is number) returns Query
{
    const sk = newSketchOnPlane(context, id + "sk", { "sketchPlane" : pl });
    var p = [];
    for (var q in pts)
        p = append(p, vector(q[0], q[1]) * inch);
    p = append(p, p[0]);
    skPolyline(sk, "poly", { "points" : p });
    skSolve(sk);
    opExtrude(context, id + "ex", {
                "entities" : qSketchRegion(id + "sk"),
                "direction" : pl.normal,
                "endBound" : BoundingType.BLIND,
                "endDepth" : dEnd * inch,
                "startBound" : BoundingType.BLIND,
                "startDepth" : dStart * inch
            });
    opDeleteBodies(context, id + "del", { "entities" : qCreatedBy(id + "sk", EntityType.BODY) });
    return qCreatedBy(id + "ex", EntityType.BODY);
}

// Plane whose sketch coordinates are (X, Z), through the hull centerline.
function xzPlane() returns Plane
{
    return plane(vector(0, YC, 0) * inch, vector(0, -1, 0), vector(1, 0, 0));
}

// Plane whose sketch coordinates are (X, Y), below the hull.
function xyPlane() returns Plane
{
    return plane(vector(0, 0, -6) * inch, vector(0, 0, 1), vector(1, 0, 0));
}

function nameAndColor(context is Context, q is Query, name is string, c is Color)
{
    setProperty(context, { "entities" : q, "propertyType" : PropertyType.NAME, "value" : name });
    setProperty(context, { "entities" : q, "propertyType" : PropertyType.APPEARANCE, "value" : c });
}

// #10-24 x 5/8 truss-head screw + flat washer + nylock through a trim part and the hull skin.
// P: point on the hull surface (inches), n: unit outward normal, tOuter: trim thickness,
// ex: explode distance (inches) - screw moves out, washer and nut move in.
// Returns the hole tool (an unexploded cylinder through the trim part) to subtract later.
function trimFastener(context is Context, id is Id, P is Vector, n is Vector, tOuter is number, ex is number, c is Color) returns Query
{
    const base = P + tOuter * n;
    const out = (2 * ex) * n;
    fCylinder(context, id + "head", { "bottomCenter" : (base + out) * inch, "topCenter" : (base + 0.13 * n + out) * inch, "radius" : 0.246 * inch });
    fCylinder(context, id + "shank", { "bottomCenter" : (base + 0.01 * n + out) * inch, "topCenter" : (base - 0.625 * n + out) * inch, "radius" : 0.095 * inch });
    opBoolean(context, id + "u", {
                "tools" : qUnion([qCreatedBy(id + "head", EntityType.BODY), qCreatedBy(id + "shank", EntityType.BODY)]),
                "operationType" : BooleanOperationType.UNION
            });
    nameAndColor(context, qUnion([qCreatedBy(id + "head", EntityType.BODY), qCreatedBy(id + "shank", EntityType.BODY), qCreatedBy(id + "u", EntityType.BODY)]),
            "Screw #10-24 x 5/8 truss head 18-8 SS", c);
    fCylinder(context, id + "washer", { "bottomCenter" : (P - ex * n) * inch, "topCenter" : (P - (0.05 + ex) * n) * inch, "radius" : 0.219 * inch });
    nameAndColor(context, qCreatedBy(id + "washer", EntityType.BODY), "Flat washer #10 18-8 SS", c);
    fCylinder(context, id + "nut", { "bottomCenter" : (P - (0.05 + 1.6 * ex) * n) * inch, "topCenter" : (P - (0.29 + 1.6 * ex) * n) * inch, "radius" : 0.2165 * inch });
    nameAndColor(context, qCreatedBy(id + "nut", EntityType.BODY), "Nylock nut #10-24 18-8 SS", c);
    fCylinder(context, id + "hole", { "bottomCenter" : (base + 0.05 * n) * inch, "topCenter" : (P - 0.05 * n) * inch, "radius" : 0.1 * inch });
    return qCreatedBy(id + "hole", EntityType.BODY);
}

annotation { "Feature Type Name" : "Hull armor" }
export const hullArmor = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        annotation { "Name" : "UHMW thickness" }
        isLength(definition.uhmwT, { (inch) : [0.0625, 0.25, 1] } as LengthBoundSpec);

        annotation { "Name" : "UHMW forward end (X)" }
        isLength(definition.uhmwEnd, { (inch) : [102, 135, 155] } as LengthBoundSpec);

        annotation { "Name" : "Bow skin overlap past UHMW" }
        isLength(definition.overlap, { (inch) : [0.5, 3, 12] } as LengthBoundSpec);

        annotation { "Name" : "Bow skin thickness" }
        isLength(definition.skinT, { (inch) : [0.02, 0.063, 0.25] } as LengthBoundSpec);

        annotation { "Name" : "Include bolts", "Default" : true }
        definition.includeBolts is boolean;

        annotation { "Name" : "Include chine angles", "Default" : true }
        definition.includeTrim is boolean;

        annotation { "Name" : "Include bow skin and deck ports", "Default" : true }
        definition.includeBowSkin is boolean;

        annotation { "Name" : "Explode distance (renders only)" }
        isLength(definition.explode, { (inch) : [0, 0, 48] } as LengthBoundSpec);

        annotation { "Name" : "Explode trim fasteners (renders only)" }
        isLength(definition.explodeTrim, { (inch) : [0, 0, 48] } as LengthBoundSpec);

        annotation { "Name" : "Hide hull skin (renders only)", "Default" : false }
        definition.hideHullSkin is boolean;
    }
    {
        const T = definition.uhmwT / inch;
        const xEnd = definition.uhmwEnd / inch;
        const lap = definition.overlap / inch;
        const tSkin = definition.skinT / inch;
        const uhmwColor = color(0.12, 0.12, 0.12);
        const aluColor = color(0.78, 0.8, 0.83);
        const boltColor = color(0.55, 0.57, 0.6);

        // ---------- UHMW strips ----------
        var bottom = [[-1, 0], [101, 0], [112, 1.25], [128, 3.75], [158.17, 9.28]];
        const slabProfile = concat(bottom, reversed(offsetPoly(bottom, -T)));
        var strips = [];
        var k = 0;
        for (var s in [1, -1])
        {
            const side = s == 1 ? "Port" : "Stbd";
            // [u1, u2 (or -1 for chine edge), xa, xb, label]
            const pieces = [
                    [0.125, 12.125, 0, INNER_SEAM, "inner aft"],
                    [0.125, 12.125, INNER_SEAM, xEnd, "inner fwd"],
                    [12.375, -1, 0, OUTER_SEAM, "outer aft"],
                    [12.375, -1, OUTER_SEAM, xEnd, "outer fwd"]];
            for (var pc in pieces)
            {
                const pid = id + ("strip" ~ k);
                k += 1;
                const slab = polyExtrude(context, pid + "slab", slabProfile, xzPlane(), 27, 27);
                var plan = [];
                if (pc[1] > 0)
                {
                    plan = [[pc[2], YC + s * pc[0]], [pc[3], YC + s * pc[0]], [pc[3], YC + s * pc[1]], [pc[2], YC + s * pc[1]]];
                }
                else
                {
                    var outer = [];
                    var xs = [pc[2]];
                    for (var h in HW0)
                    {
                        if (h[0] > pc[2] && h[0] < pc[3])
                            xs = append(xs, h[0]);
                    }
                    xs = append(xs, pc[3]);
                    for (var x in xs)
                        outer = append(outer, [x, YC + s * min(24.375, interp(HW0, x) - 0.13)]);
                    plan = concat([[pc[2], YC + s * pc[0]], [pc[3], YC + s * pc[0]]], reversed(outer));
                }
                const tool = polyExtrude(context, pid + "plan", plan, xyPlane(), 30, 0);
                opBoolean(context, pid + "int", {
                            "tools" : qUnion([slab, tool]),
                            "operationType" : BooleanOperationType.INTERSECTION
                        });
                const body = qCreatedBy(pid, EntityType.BODY);
                nameAndColor(context, body, "UHMW strip 1/4 - " ~ side ~ " " ~ pc[4], uhmwColor);
                strips = append(strips, body);
            }
        }

        // ---------- Bolts and nuts through the rib flanges ----------
        if (definition.includeBolts)
        {
            var spots = []; // [x, u, s]
            for (var s in [1, -1])
            {
                for (var rx in RIBS)
                {
                    for (var u in INNER_BOLTS)
                    {
                        if (rx == INNER_SEAM)
                        {
                            spots = append(spots, [rx - 1, u, s]);
                            spots = append(spots, [rx + 1, u, s]);
                        }
                        else
                            spots = append(spots, [rx, u, s]);
                    }
                    for (var u in OUTER_BOLTS)
                    {
                        if (rx == OUTER_SEAM)
                        {
                            spots = append(spots, [rx - 1, u, s]);
                            spots = append(spots, [rx + 1, u, s]);
                        }
                        else
                            spots = append(spots, [rx, u, s]);
                    }
                }
            }
            var bolts = [];
            var washers = [];
            var nuts = [];
            for (var i = 0; i < size(spots); i += 1)
            {
                const sp = spots[i];
                const m = slopeAt(sp[0]);
                const nl = sqrt(1 + m * m);
                const n = vector(-m / nl, 0, 1 / nl);
                const S = vector(sp[0], YC + sp[2] * sp[1], interp(ZB, sp[0]));
                const B = S - T * n;
                const bid = id + ("bolt" ~ i);
                // #10 82-degree flat head: 0.385 dia x 0.116 tall, 0.19 shank.
                fCone(context, bid + "head", {
                            "bottomCenter" : B * inch,
                            "topCenter" : (B + 0.116 * n) * inch,
                            "bottomRadius" : 0.1925 * inch,
                            "topRadius" : 0.095 * inch
                        });
                fCylinder(context, bid + "shank", {
                            "bottomCenter" : (B + 0.1 * n) * inch,
                            "topCenter" : (S + 0.62 * n) * inch,
                            "radius" : 0.095 * inch
                        });
                opBoolean(context, bid + "u", {
                            "tools" : qUnion([qCreatedBy(bid + "head", EntityType.BODY), qCreatedBy(bid + "shank", EntityType.BODY)]),
                            "operationType" : BooleanOperationType.UNION
                        });
                const bq = qCreatedBy(bid, EntityType.BODY);
                nameAndColor(context, bq, "Screw #10-24 x 1 flat head 82 deg 18-8 SS", boltColor);
                bolts = append(bolts, bq);
                // Fender washer on the rib flange, then the nylock nut.
                const wid = id + ("washer" ~ i);
                fCylinder(context, wid, {
                            "bottomCenter" : (S + 0.18 * n) * inch,
                            "topCenter" : (S + 0.23 * n) * inch,
                            "radius" : 0.5 * inch
                        });
                nameAndColor(context, qCreatedBy(wid, EntityType.BODY), "Fender washer #10 x 1 OD 18-8 SS", boltColor);
                washers = append(washers, qCreatedBy(wid, EntityType.BODY));
                const nid = id + ("nut" ~ i);
                fCylinder(context, nid, {
                            "bottomCenter" : (S + 0.23 * n) * inch,
                            "topCenter" : (S + 0.47 * n) * inch,
                            "radius" : 0.2165 * inch
                        });
                const nq = qCreatedBy(nid, EntityType.BODY);
                nameAndColor(context, nq, "Nylock nut #10-24 18-8 SS", boltColor);
                nuts = append(nuts, nq);
            }
            opBoolean(context, id + "holes", {
                        "tools" : qUnion(bolts),
                        "targets" : qUnion(strips),
                        "operationType" : BooleanOperationType.SUBTRACTION,
                        "keepTools" : true
                    });
            // Exploded view: screws drop out below the strips, washers and nuts lift off the ribs.
            const E = definition.explode;
            if (E > 0 * inch)
            {
                opTransform(context, id + "explodeBolts", { "bodies" : qUnion(bolts), "transform" : transform(vector(0 * inch, 0 * inch, -2.2 * E)) });
                opTransform(context, id + "explodeWashers", { "bodies" : qUnion(washers), "transform" : transform(vector(0 * inch, 0 * inch, 1.0 * E)) });
                opTransform(context, id + "explodeNuts", { "bodies" : qUnion(nuts), "transform" : transform(vector(0 * inch, 0 * inch, 1.6 * E)) });
            }
        }
        if (definition.explode > 0 * inch)
            opTransform(context, id + "explodeStrips", { "bodies" : qUnion(strips), "transform" : transform(vector(0 * inch, 0 * inch, -definition.explode)) });

        if (definition.hideHullSkin)
        {
            var skins = [];
            // The skin is the one mesh sheet spanning the whole boat; ribs and fittings are smaller sheets.
            for (var b in evaluateQuery(context, qBodyType(qEverything(EntityType.BODY), BodyType.SHEET)))
            {
                const bb = evBox3d(context, { "topology" : b });
                if (bb.maxCorner[0] - bb.minCorner[0] > 150 * inch && bb.maxCorner[2] - bb.minCorner[2] > 15 * inch)
                    skins = append(skins, b);
            }
            if (size(skins) == 0)
                throw regenError("Hull skin not found");
            opDeleteBodies(context, id + "hideSkin", { "entities" : qUnion(skins) });
        }


        const exT = definition.explodeTrim / inch;

        // ---------- Chine angles ----------
        if (definition.includeTrim)
        {
            const uc0 = interp(HW0, 0) - 0.129;
            const d = [0.458, 0.889];
            const nOut = [0.889, -0.458];
            const angleEnd = xEnd - lap - 0.25;
            var pathX = [];
            for (var x = 0; x < 100; x += 10)
                pathX = append(pathX, x);
            pathX = concat(pathX, [100, 101, 103, 106, 109, 112, 115, 120, 125, 128, angleEnd]);
            for (var s in [1, -1])
            {
                const side = s == 1 ? "Port" : "Stbd";
                const aid = id + ("angle" ~ (s == 1 ? "P" : "S"));
                // Profile (u, z) about the inside corner.
                const P1 = [uc0, -T];
                const P2 = [uc0 + 1.5 * d[0], -T + 1.5 * d[1]];
                const P3 = [P2[0] + 0.125 * nOut[0], P2[1] + 0.125 * nOut[1]];
                const P4 = [uc0 + 0.076, -T - 0.125];
                const P5 = [uc0 - 1.4, -T - 0.125];
                const P6 = [uc0 - 1.4, -T];
                var prof = [];
                for (var q in [P1, P2, P3, P4, P5, P6])
                    prof = append(prof, [YC + s * q[0], q[1]]);
                const pl = plane(vector(0, 0, 0) * inch, vector(1, 0, 0), vector(0, 1, 0));
                const sk = newSketchOnPlane(context, aid + "sk", { "sketchPlane" : pl });
                var p = [];
                for (var q in prof)
                    p = append(p, vector(q[0], q[1]) * inch);
                p = append(p, p[0]);
                skPolyline(sk, "poly", { "points" : p });
                skSolve(sk);
                var pathPts = [];
                for (var x in pathX)
                    pathPts = append(pathPts, vector(x, YC + s * (interp(HW0, x) - 0.129), interp(ZB, x) - T) * inch);
                opFitSpline(context, aid + "path", { "points" : pathPts });
                opSweep(context, aid + "sweep", {
                            "profiles" : qSketchRegion(aid + "sk"),
                            "path" : qCreatedBy(aid + "path", EntityType.EDGE)
                        });
                opDeleteBodies(context, aid + "del", { "entities" : qUnion([qCreatedBy(aid + "sk", EntityType.BODY), qCreatedBy(aid + "path", EntityType.BODY)]) });
                const angle = qCreatedBy(aid + "sweep", EntityType.BODY);
                nameAndColor(context, angle, "Chine angle 1-1/2 x 1-1/2 x 1/8 alum (formed 117 deg) - " ~ side, aluColor);

                // Side-leg bolts every 6 in, 0.85 in up the leg from the inside corner.
                if (definition.includeBolts)
                {
                    const n = vector(0, s * nOut[0], nOut[1]);
                    var holes = [];
                    var j = 0;
                    for (var x = 3; x <= angleEnd - 2; x += 6)
                    {
                        const P = vector(x, YC + s * (interp(HW0, x) - 0.129 + 0.85 * d[0]), interp(ZB, x) - T + 0.85 * d[1]);
                        holes = append(holes, trimFastener(context, aid + ("f" ~ j), P, n, 0.125, exT, boltColor));
                        j += 1;
                    }
                    opBoolean(context, aid + "holes", { "tools" : qUnion(holes), "targets" : angle, "operationType" : BooleanOperationType.SUBTRACTION });
                }
            }
        }

        if (!definition.includeBowSkin)
            return;

        // ---------- Bow skin ----------
        const faceX = function(z) { return 158.17 + 0.1875 * (z - 9.25); };
        const slopeC = (9.28 - 3.75) / (158.17 - 128);
        const ncl = sqrt(1 + slopeC * slopeC);
        const offC = [slopeC / ncl * T, -1 / ncl * T];
        const zC = function(x) { return 3.75 + slopeC * (x - 128); };
        const xJog = xEnd + 2;
        const xAft = xEnd - lap;
        const wrapPath = [
                [faceX(16.6) - 0.01, 16.6],
                [159.25, 18.6],
                [159.25, 19.55],
                [160.33, 19.55],
                [160.33, 18.45],
                [faceX(16.0), 16.0],
                [158.17, 9.28],
                [xJog, zC(xJog)],
                [xEnd + 0.6 + offC[0], zC(xEnd + 0.6) + offC[1]],
                [xAft + offC[0], zC(xAft) + offC[1]]];
        const wrapProfile = concat(wrapPath, reversed(offsetPoly(wrapPath, tSkin)));
        const wslab = polyExtrude(context, id + "bow" + "wrap", wrapProfile, xzPlane(), 27, 27);
        var wplanP = [];
        for (var h in [[xAft - 1, interp(HW0, xAft - 1)], [135, 23.78], [140, 23.62], [145, 23.44], [150, 23.27], [155, 23.11], [158.0, 23.02], [158.3, 23.6], [161.5, 23.6]])
            wplanP = append(wplanP, [h[0], YC + h[1]]);
        var wplanS = [];
        for (var q in wplanP)
            wplanS = append(wplanS, [q[0], 2 * YC - q[1]]);
        const wtool = polyExtrude(context, id + "bow" + "wplan", concat(wplanP, reversed(wplanS)), xyPlane(), 30, 0);
        opBoolean(context, id + "bow" + "wint", {
                    "tools" : qUnion([wslab, wtool]),
                    "operationType" : BooleanOperationType.INTERSECTION
                });
        const skin = qCreatedBy(id + "bow", EntityType.BODY);
        nameAndColor(context, skin, "Bow skin 0.063 5052 aluminum", aluColor);

        // Bow skin bolts: around the bow face and across the bow rise forward of the UHMW (never through it).
        if (definition.includeBolts)
        {
            var spots = []; // [P, n]
            const nFace = vector(1, 0, -0.1875) / sqrt(1 + 0.1875 * 0.1875);
            const nRise = vector(slopeC, 0, -1) / ncl;
            const across = [-18, -12, -6, 0, 6, 12, 18];
            for (var u in [-23, 23])
                for (var z in [10.5, 12.5, 14.5])
                    spots = append(spots, [vector(faceX(z), YC + u, z), nFace]);
            for (var z in [10.2, 15.4])
                for (var u in across)
                    spots = append(spots, [vector(faceX(z), YC + u, z), nFace]);
            for (var u in [-22, 22])
                for (var x in [139, 145, 151, 156])
                    spots = append(spots, [vector(x, YC + u, zC(x)), nRise]);
            for (var x in [xJog + 1.5, 152])
                for (var u in across)
                    spots = append(spots, [vector(x, YC + u, zC(x)), nRise]);
            var holes = [];
            for (var j = 0; j < size(spots); j += 1)
                holes = append(holes, trimFastener(context, id + ("bowf" ~ j), spots[j][0], spots[j][1], tSkin, exT, boltColor));
            opBoolean(context, id + "bowholes", { "tools" : qUnion(holes), "targets" : skin, "operationType" : BooleanOperationType.SUBTRACTION });
        }

        // Two 6 in screw-out inspection ports in the front deck, for reaching the bow-skin nuts.
        for (var u in [-10, 10])
        {
            const pid = id + ("port" ~ (u > 0 ? "P" : "S"));
            fCylinder(context, pid, {
                        "bottomCenter" : vector(150, YC + u, 16.55) * inch,
                        "topCenter" : vector(150, YC + u, 16.9) * inch,
                        "radius" : 4 * inch
                    });
            nameAndColor(context, qCreatedBy(pid, EntityType.BODY), "Deck inspection port 6 in", color(0.95, 0.95, 0.95));
        }
    });
