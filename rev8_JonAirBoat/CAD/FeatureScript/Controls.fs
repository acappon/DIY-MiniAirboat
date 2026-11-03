FeatureScript 3112;
import(path : "onshape/std/geometry.fs", version : "3112.0");

// Driver controls (rev8): steering stick and foot-pedal throttle, each driving a Teleflex (SeaStar)
// push-pull cable, in the airboat assembly frame: X forward from the transom, Y = 0 on the centerline
// (port +), Z up from the inside of the hull floor.
//   - Steering: stick on the port side of the driver seat, pivoting fore-aft on a clevis bracket on its
//     own mount rail. A lug 4.4 in above the pivot drives a 43C-type push-pull cable (3 in travel) that
//     runs aft through the storage box and along the old rod line to the cage bellcrank pin B.
//   - Throttle: pedal hinged at the heel on a footrest platform for both feet, standing 30 deg up at idle
//     and pressed through 25 deg to a stop bolt (the steep stroke is the trick T-H Marine's Hot Foot
//     uses). A down-arm clamped to the pedal shaft, outside the platform's starboard wall, has cable
//     holes at 3, 3.5 and 4 in (about 1.3, 1.5 and 1.7 in of cable travel) and drives a 33C-type
//     push-pull cable aft along the starboard side of the dry box, through the storage box, to the
//     engine throttle. The arm leans 12.5 deg forward at idle so it passes vertical mid-stroke.
// Cable centerlines are fitted splines through the waypoints below; the path wires are kept (named) so
// lengths and bend radii can be measured. Cables pass the storage box through grommets in its panels.

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

function doCut(context is Context, id is Id, targets is Query, tools is Query)
{
    opBoolean(context, id, { "targets" : targets, "tools" : tools, "operationType" : BooleanOperationType.SUBTRACTION });
}

function doUnite(context is Context, id is Id, q is Query)
{
    opBoolean(context, id, { "tools" : q, "operationType" : BooleanOperationType.UNION });
}

// 1 x 2 x 1/8 rectangular tube lying flat, X from x0 to x1, centered on yc (same as the seating rails).
function mountRail(context is Context, id is Id, x0 is number, x1 is number, yc is number, name is string)
{
    mkBox(context, id + "in", x0 - 1, x1 + 1, yc - 0.875, yc + 0.875, 0.125, 0.875);
    mkBox(context, id + "out", x0, x1, yc - 1, yc + 1, 0, 1);
    doCut(context, id + "cut", qCreatedBy(id + "out", EntityType.BODY), qCreatedBy(id + "in", EntityType.BODY));
    paintBody(context, qCreatedBy(id + "out", EntityType.BODY), name, color(0.72, 0.74, 0.78));
}

// Closed polygon in the XZ plane ([x, z] pairs), extruded from Y = y0 to Y = y1.
function xzPrism(context is Context, id is Id, y0 is number, y1 is number, pts is array)
{
    const sk = newSketchOnPlane(context, id + "sk", { "sketchPlane" : plane(vector(0, y0, 0) * inch, vector(0, 1, 0), vector(1, 0, 0)) });
    var p = [];
    for (var q in pts)
        p = append(p, vector(q[0], -q[1]) * inch);   // sketch y axis = normal x X = -Z here
    p = append(p, p[0]);
    skPolyline(sk, "poly", { "points" : p });
    skSolve(sk);
    opExtrude(context, id + "ex", { "entities" : qSketchRegion(id + "sk"), "direction" : vector(0, 1, 0), "endBound" : BoundingType.BLIND, "endDepth" : (y1 - y0) * inch });
    opDeleteBodies(context, id + "del", { "entities" : qCreatedBy(id + "sk", EntityType.BODY) });
}

// Cable conduit: circle of radius r swept along a spline through pts. The path wire is kept and named.
function cable(context is Context, id is Id, pts is array, r is number, name is string, c is Color)
{
    var p = [];
    for (var q in pts)
        p = append(p, q * inch);
    opFitSpline(context, id + "path", { "points" : p });
    const edge = qCreatedBy(id + "path", EntityType.EDGE);
    const tl = evEdgeTangentLine(context, { "edge" : edge, "parameter" : 0 });
    const sk = newSketchOnPlane(context, id + "sk", { "sketchPlane" : plane(tl.origin, tl.direction) });
    skCircle(sk, "c", { "center" : vector(0, 0) * inch, "radius" : r * inch });
    skSolve(sk);
    opSweep(context, id + "sweep", { "profiles" : qSketchRegion(id + "sk"), "path" : edge });
    opDeleteBodies(context, id + "del", { "entities" : qCreatedBy(id + "sk", EntityType.BODY) });
    paintBody(context, qCreatedBy(id + "sweep", EntityType.BODY), name, c);
    setProperty(context, { "entities" : qCreatedBy(id + "path", EntityType.BODY), "propertyType" : PropertyType.NAME, "value" : "Centerline - " ~ name });
}

annotation { "Feature Type Name" : "Driver controls (stick, throttle pedal, push-pull cables)" }
export const driverControls = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        annotation { "Name" : "Stick pivot X" }
        isLength(definition.stickX, { (inch) : [80, 110, 130] } as LengthBoundSpec);

        annotation { "Name" : "Stick Y (port of the centerline)" }
        isLength(definition.stickY, { (inch) : [10, 13.5, 20] } as LengthBoundSpec);

        annotation { "Name" : "Throttle pedal hinge X" }
        isLength(definition.pedalX, { (inch) : [100, 121, 140] } as LengthBoundSpec);
    }
    {
        const sx = definition.stickX / inch;
        const sy = definition.stickY / inch;
        const px = definition.pedalX / inch;
        const alu = color(0.75, 0.77, 0.8);
        const plateC = color(0.62, 0.66, 0.72);
        const steel = color(0.35, 0.36, 0.38);
        const black = color(0.08, 0.08, 0.09);
        const red = color(0.75, 0.12, 0.1);
        const ss = color(0.82, 0.83, 0.85);

        // ---------------- Tools first ----------------
        const pz = 5.5;                                        // stick pivot height
        // Steering cable, aft end: core rod line from bellcrank pin B (cage studio) along the old rod line.
        const pinB = vector(-3.6, 32, 28.93);
        const d = normalize(vector(0.98, -0.17, -0.1));
        const P = function(tt) { return pinB + d * tt; };
        mkCyl(context, id + "tabHole", P(5.5), P(7.5), 0.2);

        // ---------------- Mount rails (same section as the seating rails) ----------------
        mountRail(context, id + "railStick", sx - 10, sx + 8, sy, "Interior mount rail 1x2x1/8 6061 - steering stick");
        mountRail(context, id + "railFootP", px - 2, px + 12.5, 5, "Interior mount rail 1x2x1/8 6061 - footrest, Port");
        mountRail(context, id + "railFootS", px - 2, px + 12.5, -5, "Interior mount rail 1x2x1/8 6061 - footrest, Stbd");

        // ---------------- Steering stick ----------------
        mkBox(context, id + "stickBase", sx - 8.5, sx + 6, sy - 1.5, sy + 1.5, 1, 1.25);
        paintBody(context, qCreatedBy(id + "stickBase", EntityType.BODY), "Stick base plate 1/4 6061, 4 x 5/16-18 into rail rivnuts", plateC);
        for (var ck in [[sy - 1.1, sy - 0.85, "In"], [sy + 0.85, sy + 1.1, "Out"]])
        {
            const hid = id + ("pivotHole" ~ ck[2]);
            mkCyl(context, hid, vector(sx, ck[0] - 0.1, pz), vector(sx, ck[1] + 0.1, pz), 0.26);
            const cid = id + ("cheek" ~ ck[2]);
            mkBox(context, cid, sx - 2, sx + 2, ck[0], ck[1], 1.25, pz + 1.5);
            doCut(context, cid + "hole", qCreatedBy(cid, EntityType.BODY), qCreatedBy(hid, EntityType.BODY));
            paintBody(context, qCreatedBy(cid, EntityType.BODY), "Stick clevis cheek 1/4 6061, welded or bolted to the base - " ~ (ck[2] == "Out" ? "outboard" : "inboard"), plateC);
        }
        mkCyl(context, id + "pivotBolt", vector(sx, sy - 1.35, pz), vector(sx, sy + 1.35, pz), 0.25);
        paintBody(context, qCreatedBy(id + "pivotBolt", EntityType.BODY), "Stick pivot bolt 1/2-13 grade 8 with nylock, in bronze or nylon bushings", steel);
        {
            const kid = id + "stick";
            const top = vector(sx + 2, sy, 33);
            mkCyl(context, kid + "sleeveIn", vector(sx, sy - 1, pz), vector(sx, sy + 1, pz), 0.26);
            mkCyl(context, kid + "sleeveOut", vector(sx, sy - 0.8, pz), vector(sx, sy + 0.8, pz), 0.6);
            doCut(context, kid + "bore", qCreatedBy(kid + "sleeveOut", EntityType.BODY), qCreatedBy(kid + "sleeveIn", EntityType.BODY));
            mkCyl(context, kid + "tube", vector(sx, sy, pz + 0.3), top, 0.5625);
            mkCyl(context, kid + "grip", top + vector(0, -3, 0.4), top + vector(0, 3, 0.4), 0.55);
            mkBox(context, kid + "lug", sx - 0.6, sx + 0.1, sy - 0.125, sy + 0.125, pz + 3.9, pz + 4.9);
            doUnite(context, kid + "join", qCreatedBy(kid, EntityType.BODY));
            paintBody(context, qCreatedBy(kid, EntityType.BODY), "Steering stick weldment: 1-1/8 x 0.120 DOM steel tube, pivot sleeve, cable lug, T-grip (MIG welded)", black);
        }
        // Steering cable, stick end: anchor upright, hub, core rod to the lug.
        const sAz = pz + 4.4;
        mkBox(context, id + "sAnchor", sx - 8.25, sx - 8, sy - 0.75, sy + 0.75, 1.25, sAz + 0.7);
        paintBody(context, qCreatedBy(id + "sAnchor", EntityType.BODY), "Steering cable anchor upright 1/4 6061 with cable clamp, on the stick base", plateC);
        mkCyl(context, id + "sHub", vector(sx - 8, sy, sAz), vector(sx - 6.5, sy, sAz), 0.45);
        paintBody(context, qCreatedBy(id + "sHub", EntityType.BODY), "Steering cable end hub and clamp groove (43C type), stick end", red);
        mkCyl(context, id + "sRod", vector(sx - 6.5, sy, sAz), vector(sx - 0.62, sy, sAz), 0.125);
        paintBody(context, qCreatedBy(id + "sRod", EntityType.BODY), "Steering cable core rod 1/4-28 with clevis on the stick lug", ss);

        // Steering cable, bellcrank end: rod end on pin B, core rod, hub and its clamp tab.
        mkCyl(context, id + "bRod", P(0.8), P(7.2), 0.125);
        paintBody(context, qCreatedBy(id + "bRod", EntityType.BODY), "Steering cable core rod 1/4-28 with rod-end eye on bellcrank pin B (replaces the rev7 rod)", ss);
        mkCyl(context, id + "bHub", P(7.2), P(8.7), 0.45);
        paintBody(context, qCreatedBy(id + "bHub", EntityType.BODY), "Steering cable end hub and clamp groove (43C type), bellcrank end", red);
        // Clamp tab outside the drum screen (screen radius about 37 in from the shaft axis; tab 38 in and out).
        // On the boat it is welded to a short arm off the aft hoop next to the bellcrank, fitted in place.
        mkBox(context, id + "bBracket", 3.0, 3.25, 29.9, 31.6, 27.4, 28.9);
        doCut(context, id + "bBracket" + "hole", qCreatedBy(id + "bBracket", EntityType.BODY), qCreatedBy(id + "tabHole", EntityType.BODY));
        paintBody(context, qCreatedBy(id + "bBracket", EntityType.BODY), "Steering cable clamp tab 1/4 steel, welded to a short arm off the aft cage hoop at fit-up", steel);
        // Steering conduit: hub at the bellcrank end -> old rod line -> storage box grommets -> stick hub.
        cable(context, id + "steerCable", [P(8.7), P(25), P(40), P(51), vector(56, 22.8, 18.5), vector(63.15, 23, 13), vector(71, 21.6, 12.4), vector(78.85, 20, 12), vector(87, 17.5, 11.2), vector(95, 14.8, sAz + 0.3), vector(sx - 9.8, sy, sAz), vector(sx - 8.25, sy, sAz)],
              0.25, "Steering push-pull cable, Teleflex/SeaStar 43C type (4300CC), 3 in travel", black);

        // ---------------- Footrest and throttle pedal ----------------
        const fx0 = px - 2.5;
        const fx1 = px + 13;
        const fTop = 4.5;
        mkBox(context, id + "footIn", fx0 + 0.125, fx1 - 0.125, -8.875, 8.875, 0, fTop - 0.125);
        mkBox(context, id + "footrest", fx0, fx1, -9, 9, 1, fTop);
        doCut(context, id + "footrest" + "cut", qCreatedBy(id + "footrest", EntityType.BODY), qCreatedBy(id + "footIn", EntityType.BODY));
        paintBody(context, qCreatedBy(id + "footrest", EntityType.BODY), "Footrest platform 1/8 6061 diamond plate, bent box (both feet)", alu);
        const hz = fTop + 0.35;                                // pedal shaft height
        const ang = 30 * degree;                               // pedal angle at idle; full throttle is 5 deg
        const lean = 12.5 * degree;                            // arm lean forward of vertical at idle (half the 25 deg stroke)
        const armHole = 3.5;                                   // cable hole used in the model (holes at 3, 3.5, 4 in)
        const ad = [sin(lean), -cos(lean)];                    // down the arm
        const an = [cos(lean), sin(lean)];                     // across the arm
        {
            const pid = id + "pedal";
            const A = [px, hz];
            const B = [px + 10 * cos(ang), hz + 10 * sin(ang)];
            const C = [B[0] - 0.25 * sin(ang), B[1] + 0.25 * cos(ang)];
            const D = [px - 0.25 * sin(ang), hz + 0.25 * cos(ang)];
            xzPrism(context, pid + "plate", -6.5, -2.5, [A, B, C, D]);
            mkCyl(context, pid + "shaft", vector(px, -2.5, hz), vector(px, -10.25, hz), 0.25);
            var arm = [];
            for (var e in [[-0.3, -0.4], [-0.3, 0.4], [4.4, 0.4], [4.4, -0.4]])
                arm = append(arm, [px + e[0] * ad[0] + e[1] * an[0], hz + e[0] * ad[1] + e[1] * an[1]]);
            xzPrism(context, pid + "arm", -10.25, -10.0, arm);
            for (var h in [[3, "hole30"], [3.5, "hole35"], [4, "hole40"]])
            {
                const hid = pid + h[1];
                mkCyl(context, hid, vector(px + h[0] * ad[0], -10.5, hz + h[0] * ad[1]), vector(px + h[0] * ad[0], -9.75, hz + h[0] * ad[1]), 0.1);
                doCut(context, hid + "cut", qCreatedBy(pid + "arm", EntityType.BODY), qCreatedBy(hid, EntityType.BODY));
            }
            doUnite(context, pid + "join", qCreatedBy(pid, EntityType.BODY));
            paintBody(context, qCreatedBy(pid, EntityType.BODY), "Throttle pedal: 1/4 6061 plate with grip tape on 1/2 in shaft, down-arm clamped with a collar and through-bolt (cable holes at 3, 3.5, 4 in), torsion return spring with a choice of anchor holes", black);
        }
        // Full-throttle stop under the pedal toe: set so the pedal stops at 5 deg, before the cable or throttle body bottoms.
        mkCyl(context, id + "tStop", vector(px + 7.97, -4.5, fTop), vector(px + 7.97, -4.5, hz + 8 * sin(5 * degree)), 0.125);
        paintBody(context, qCreatedBy(id + "tStop", EntityType.BODY), "Throttle full-stop bolt 1/4-20 x 1.5 with jam nut, threaded into a rivnut in the footrest top (adjust for full throttle)", ss);
        for (var pb in [[-6.4, -5.6, "inboard"], [-8.8, -8.0, "outboard"]])
        {
            const bid = id + ("pillow" ~ (pb[2] == "inboard" ? "In" : "Out"));
            mkBox(context, bid, px - 0.5, px + 0.5, pb[0], pb[1], fTop, hz - 0.25);
            paintBody(context, qCreatedBy(bid, EntityType.BODY), "Pedal shaft pillow block, nylon, bolted through the footrest top - " ~ pb[2], color(0.9, 0.9, 0.86));
        }
        // Throttle cable, pedal end: anchor plate on the footrest starboard wall, hub, core rod to the arm.
        const tz = hz + armHole * ad[1];                       // cable line through the arm hole at idle
        const tcx = px + armHole * ad[0];
        const ty = -10.4;
        {
            const aid = id + "tAnchor";
            mkBox(context, aid + "plate", px - 8.5, px + 3, -9.125, -9.0, tz - 0.6, 3.5);
            mkBox(context, aid + "tab", px - 8.5, px - 8.25, -10.9, -9.125, tz - 0.6, tz + 0.6);
            doUnite(context, aid + "join", qUnion([qCreatedBy(aid + "plate", EntityType.BODY), qCreatedBy(aid + "tab", EntityType.BODY)]));
            paintBody(context, qCreatedBy(aid, EntityType.BODY), "Throttle cable anchor bracket 1/8 6061, bolted to the footrest starboard wall", plateC);
        }
        mkCyl(context, id + "tHub", vector(px - 8.25, ty, tz), vector(px - 6.75, ty, tz), 0.4);
        paintBody(context, qCreatedBy(id + "tHub", EntityType.BODY), "Throttle cable end hub and clamp groove (33C type), pedal end", red);
        mkCyl(context, id + "tRod", vector(px - 6.75, ty, tz), vector(tcx - 0.45, ty, tz), 0.125);
        paintBody(context, qCreatedBy(id + "tRod", EntityType.BODY), "Throttle cable core rod 10-32 with clevis on the pedal arm", ss);
        // Throttle cable, engine end (throttle body position is approximate: confirm on the engine). The
        // end hub and core rod line up with the conduit's arrival direction; the bracket holds the hub there.
        const eA = vector(46.5, -11.5, 23.3);
        cable(context, id + "throttleCable", [vector(px - 8.5, ty, tz), vector(105, -10.1, 2.4), vector(97, -9.9, 3.6), vector(88, -9.9, 6.5), vector(78.85, -10, 9), vector(71, -10, 9), vector(63.15, -10, 9), vector(56, -10.4, 12.5), vector(50, -11.1, 18.5), eA],
              0.21, "Throttle push-pull cable, Teleflex/SeaStar 33C type (3300CC), 3 in travel", black);
        const eT = evEdgeTangentLine(context, { "edge" : qCreatedBy(id + "throttleCable" + "path", EntityType.EDGE), "parameter" : 1 }).direction;
        mkCyl(context, id + "eHub", eA, eA + eT * 1.5, 0.4);
        paintBody(context, qCreatedBy(id + "eHub", EntityType.BODY), "Throttle cable end hub (33C type), engine end", red);
        mkCyl(context, id + "eRod", eA + eT * 1.5, eA + eT * 6, 0.125);
        paintBody(context, qCreatedBy(id + "eRod", EntityType.BODY), "Throttle cable core rod 10-32 with clevis to the throttle lever (position approximate)", ss);
        const eM = eA + eT * 0.75;
        mkBox(context, id + "eBracket", eM[0] - 0.9, eM[0] + 0.9, -12.45, -12.2, eM[2] - 1.0, eM[2] + 1.0);
        paintBody(context, qCreatedBy(id + "eBracket", EntityType.BODY), "Throttle cable anchor bracket at the throttle body (bolts to the intake; position approximate)", plateC);
    });
