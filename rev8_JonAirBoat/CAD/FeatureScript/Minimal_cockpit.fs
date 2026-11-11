FeatureScript 3112;
import(path : "onshape/std/geometry.fs", version : "3112.0");

// Cockpit of the minimal 1448, in the airboat assembly frame: X forward from the transom, Y + to port,
// Z up from the hull bottom. The middle bench is gone; the driver sits on the seat module (Crew mannequins).
//   - Seat module where the middle bench was: two struts hull side to hull side at the old bench-top level, a small
//     electrical dry box hung between them (battery, fuse block, relays), a dry storage compartment each side with
//     flotation foam in its bottom and an aft-hinged lid, a closed-cell foam seat across all three, bolted down over the
//     dry box lid, and a folding seatback hinged just above the foam on brackets bolted to the aft strut
//   - Steering stick to port on the rib at X 95.25, with a push-pull cable along the port side and across the rear bench
//   - Throttle pedal with two half-round heel rests, with a push-pull cable ahead of the toes and along the starboard side
//   - Console frame to starboard, clear above the knees of two people side by side (the pod is in Minimal dash)
//   - Fuel tank on the front deck step with its gauge sender, straps and fuel line
//   - Floor conduit on the centerline from the dry box aft wall to the engine
//   - Sled oil tank upright on the starboard side frame of the stand, away from the exhaust; envelope until it is measured
//   - Oarlocks for rowing facing aft on the folded seatback: closed rings in top-mount sockets on 4 in risers on the rails
//     at X 66, 7 ft oars shipped in the locks along inside each rail, blades forward, on a strapped rest at X 118, clear
//     above the console arm (CAD/analysis/oars.js has the rowing and stowing clearances)
//   - Capsize flotation foam: bow wedge under the shelf, two aft corner blocks, the seat compartment bottoms and two
//     footwell side blocks, sized in CAD/analysis/flotation_inverted.js; the closed-cell seat pad counts too
// Sizes are placeholders until the real parts are bought, and the cable and line routes are provisional.
// The hull sides taper in from about X 75: keep runs along them about 1 in inboard of the skin (raycast the reference mesh).

const DECK_Z = 16.55;   // top of the front deck step, from the reference mesh
const FLOOR_Z = 0.1;
const PEDAL_Y = -2.5;   // inboard, so a passenger's left foot stays beside it
const STICK = [95.25, 21];
const CONSOLE_X = 102.5;
const CONSOLE_Z = 24;   // underside; a seated knee tops out at about Z 19
const STRUT_X = [76.5, 85.5];   // aft faces of the two 1 x 2 seat struts; the dry box sits between them
const STRUT_Z = [9, 11];        // old bench-top level
const BOX = [77.5, 85.5, 5, 3.75, 11]; // dry box X0, X1, half width, Z0, Z1 (outside)
const SEAT = [76.5, 91.5, 19, 11.625, 14.125]; // seat pad X0, X1, half width, Z0, Z1
const OARLOCK = [66, 34.5, 19.5, 4]; // X, |Y| of the pin, rail top Z, riser height
const OAR_Z = 25.5;                 // shaft center in the ring, shipped and rowing
const PIVOT = [75.9, 14.6];     // seatback hinge axis, just above the foam

// Inside half width of the hull skin, from rays on the reference mesh: sides flare out about 0.5 in per inch of
// height and taper in forward of about X 75.
function hullHalf(x is number, z is number) returns number
{
    return 24.4 + 0.5 * z - max(0, x - 75) * 0.045;
}

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

// Round pipe through a list of points, one cylinder per run.
function mkPipe(context is Context, id is Id, pts is array, r is number)
{
    for (var i = 0; i < size(pts) - 1; i += 1)
        mkCyl(context, id + ("seg" ~ i), pts[i], pts[i + 1], r);
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

// Half-round bar across the boat, flat side down on the floor.
function mkHalfRound(context is Context, id is Id, cx is number, y0 is number, y1 is number, r is number)
{
    var pts = [];
    const n = 8;
    for (var i = 0; i <= n; i += 1)
    {
        const a = PI * i / n;
        pts = append(pts, [cx + r * cos(a * radian), FLOOR_Z + r * sin(a * radian)]);
    }
    mkPolyXZ(context, id, append(pts, pts[0]), y0, y1);
}

annotation { "Feature Type Name" : "Minimal cockpit" }
export const minimalCockpit = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
    }
    {
        const alu = color(0.75, 0.77, 0.8);
        const dark = color(0.15, 0.15, 0.17);
        const caseC = color(0.95, 0.75, 0.15, 0.35);
        const tankC = color(0.8, 0.12, 0.1, 0.55);
        const lineC = color(0.1, 0.1, 0.1);

        // ---- Seat module: replaces the middle bench (v1.0.1.3) ----
        const strutC = color(0.72, 0.74, 0.78);
        const seatC = color(0.85, 0.85, 0.8);
        const foamC0 = color(0.18, 0.56, 0.51, 0.6);
        // Two struts across the boat at the fore and aft edges of the dry box: the thwart the bench used to be.
        for (var st in [[STRUT_X[0], "aft"], [STRUT_X[1], "forward"]])
        {
            const yEnd = hullHalf(st[0] + 0.5, STRUT_Z[0] + 1) - 0.4;
            const sid = id + ("strut" ~ st[1]);
            mkBox(context, sid, st[0], st[0] + 1, -yEnd, yEnd, STRUT_Z[0], STRUT_Z[1]);
            paintBody(context, qCreatedBy(sid, EntityType.BODY), "Seat strut 1 x 2 x 1/8 6061 tube, hull side to hull side - " ~ st[1], strutC);
            for (var sd in [1, -1])
            {
                const cid = id + ("strutClip" ~ st[1] ~ (sd > 0 ? "P" : "S"));
                const y0 = yEnd - 1.75;
                const y1 = yEnd + 0.25;
                const y2 = yEnd + 0.375;
                const z0 = STRUT_Z[0] - 0.125;
                mkPoly(context, cid, [[sd * y0, z0], [sd * y2, z0], [sd * y2, STRUT_Z[1]], [sd * y1, STRUT_Z[1]], [sd * y1, STRUT_Z[0]], [sd * y0, STRUT_Z[0]], [sd * y0, z0]], st[0], st[0] + 1);
                paintBody(context, qCreatedBy(cid, EntityType.BODY), "Strut end clip 2 x 2 x 1/8 6061 angle, opened to the side flare, bolted through the hull side - " ~ st[1] ~ (sd > 0 ? " port" : " starboard"), strutC);
            }
        }
        // Electrical dry box: 0.063 5052, riveted and sealed corners, hung from the struts; gasketed lid bolted into rivnuts.
        mkBox(context, id + "boxOut", BOX[0], BOX[1], -BOX[2], BOX[2], BOX[3], BOX[4]);
        mkBox(context, id + "boxIn", BOX[0] + 0.063, BOX[1] - 0.063, -BOX[2] + 0.063, BOX[2] - 0.063, BOX[3] + 0.063, BOX[4] + 1);
        opBoolean(context, id + "boxCut", { "targets" : qCreatedBy(id + "boxOut", EntityType.BODY), "tools" : qCreatedBy(id + "boxIn", EntityType.BODY), "operationType" : BooleanOperationType.SUBTRACTION });
        paintBody(context, qCreatedBy(id + "boxOut", EntityType.BODY), "Electrical dry box 0.063 5052, 8 x 10 x 7-1/4, riveted and sealed, hung from the struts", caseC);
        mkBox(context, id + "boxLid", BOX[0] - 0.25, BOX[1] + 0.25, -BOX[2] - 0.25, BOX[2] + 0.25, BOX[4], BOX[4] + 0.08);
        paintBody(context, qCreatedBy(id + "boxLid", EntityType.BODY), "Dry box lid 0.080 5052, closed-cell gasket, 8 #10-24 bolts into rivnuts", strutC);
        mkBox(context, id + "battery", 81.9, 85.3, -4.2, 2.7, 3.95, 9.05);
        paintBody(context, qCreatedBy(id + "battery", EntityType.BODY), "Lithium battery envelope", dark);
        mkBox(context, id + "fuseBlock", 82.1, 85.1, -2.5, 2.5, 9.3, 10.8);
        paintBody(context, qCreatedBy(id + "fuseBlock", EntityType.BODY), "Power distribution fuse block, on a bracket over the battery, fuses facing the lid", color(0.2, 0.35, 0.7));
        mkBox(context, id + "relays", 78, 81.5, 1, 4.7, 3.9, 5.4);
        paintBody(context, qCreatedBy(id + "relays", EntityType.BODY), "Relays K1-K5 on a strip", color(0.3, 0.3, 0.35));
        // Battery switch on a plate under the forward strut, port of the box, reachable without unbolting the seat.
        mkBox(context, id + "switchPlate", STRUT_X[1] + 1, STRUT_X[1] + 1.125, 6.5, 9.5, 6, STRUT_Z[0]);
        paintBody(context, qCreatedBy(id + "switchPlate", EntityType.BODY), "Battery switch plate 1/8 6061, bolted to the forward strut", strutC);
        mkCyl(context, id + "batterySwitch", vector(STRUT_X[1] + 1.125, 8, 7.5), vector(STRUT_X[1] + 3.1, 8, 7.5), 1.5);
        paintBody(context, qCreatedBy(id + "batterySwitch", EntityType.BODY), "Battery switch, on/off, knob forward", color(0.8, 0.1, 0.1));
        // Dry storage each side: 0.032 5052 walls riveted under the struts, stopped 1-1/2 in above the floor so bilge water
        // runs under; closed-cell foam in the bottom (flotation); aft-hinged lid outboard of the seat.
        for (var sd in [1, -1])
        {
            const sn = sd > 0 ? "port" : "starboard";
            for (var w in [[BOX[0], "aft"], [BOX[1] - 0.032, "forward"]])
            {
                const wid = id + ("compWall" ~ w[1] ~ (sd > 0 ? "P" : "S"));
                const yo = hullHalf(w[0], 1.5) - 0.3;
                const yt = hullHalf(w[0], STRUT_Z[0]) - 0.3;
                mkPoly(context, wid, [[sd * BOX[2], 1.5], [sd * yo, 1.5], [sd * yt, STRUT_Z[0]], [sd * BOX[2], STRUT_Z[0]], [sd * BOX[2], 1.5]], w[0], w[0] + 0.032);
                paintBody(context, qCreatedBy(wid, EntityType.BODY), "Storage compartment wall 0.032 5052 - " ~ w[1] ~ " " ~ sn, strutC);
            }
            const fid = id + ("compFoam" ~ (sd > 0 ? "P" : "S"));
            const y15 = hullHalf(BOX[1], 1.5) - 0.3;
            const y5 = hullHalf(BOX[1], 5) - 0.3;
            mkPoly(context, fid, [[sd * (BOX[2] + 0.1), 1.5], [sd * y15, 1.5], [sd * y5, 5], [sd * (BOX[2] + 0.1), 5], [sd * (BOX[2] + 0.1), 1.5]], BOX[0] + 0.1, BOX[1] - 0.1);
            paintBody(context, qCreatedBy(fid, EntityType.BODY), "Flotation foam - seat compartment bottom " ~ sn ~ ", closed-cell polyethylene, the storage floor", foamC0);
            const lid = id + ("compLid" ~ (sd > 0 ? "P" : "S"));
            mkBox(context, lid, STRUT_X[0], STRUT_X[1] + 1, sd * (SEAT[2] + 0.5), sd * (hullHalf(STRUT_X[1], STRUT_Z[1]) - 1), STRUT_Z[1], STRUT_Z[1] + 0.063);
            paintBody(context, qCreatedBy(lid, EntityType.BODY), "Storage lid 0.063 5052, piano hinge on the aft edge, gasketed, two latches - " ~ sn, strutC);
        }
        // Seat: 3/8 in marine plywood base (stiff enough to stand on its front edge) on closed-cell strips on the struts, bolted down through the struts over the dry box
        // lid, with a 2-1/2 in closed-cell polyethylene pad (flotation) in marine vinyl. Room for the driver and a passenger.
        mkBox(context, id + "seatBase", SEAT[0], SEAT[1], -SEAT[2], SEAT[2], STRUT_Z[1] + 0.25, SEAT[3]);
        paintBody(context, qCreatedBy(id + "seatBase", EntityType.BODY), "Seat base 3/8 marine plywood, epoxy sealed, bolted to the struts", color(0.78, 0.66, 0.45));
        mkBox(context, id + "seatPad", SEAT[0], SEAT[1], -SEAT[2], SEAT[2], SEAT[3], SEAT[4]);
        paintBody(context, qCreatedBy(id + "seatPad", EntityType.BODY), "Seat pad 38 x 15 x 2-1/2 closed-cell polyethylene, marine vinyl cover", seatC);
        // Folding seatback: hinged just above the foam on two brackets bolted to the aft strut, 12 degree recline against
        // a stop, folds flat forward onto the seat. Padded on the back too, so you can sit facing aft on it (engine off).
        for (var sb in [1, -1])
        {
            const bid = id + ("backBracket" ~ (sb > 0 ? "P" : "S"));
            mkBox(context, bid, STRUT_X[0] - 1.25, STRUT_X[0], sb * 9.4, sb * 9.525, STRUT_Z[0], PIVOT[1] + 0.5);
            paintBody(context, qCreatedBy(bid, EntityType.BODY), "Seatback hinge bracket 1/8 6061 with recline stop, bolted to the aft strut - " ~ (sb > 0 ? "port" : "starboard"), strutC);
        }
        mkCyl(context, id + "backPivot", vector(PIVOT[0], -9.7, PIVOT[1]), vector(PIVOT[0], 9.7, PIVOT[1]), 0.1875);
        paintBody(context, qCreatedBy(id + "backPivot", EntityType.BODY), "Seatback pivot bolt 3/8 stainless with nylon washers", color(0.85, 0.85, 0.88));
        {
            const u = vector(-sin(12 * degree), cos(12 * degree));
            const n = vector(cos(12 * degree), sin(12 * degree));
            const b = vector(PIVOT[0], PIVOT[1]);
            var pts = [];
            for (var q in [[-0.75, 0], [1.5, 0], [1.5, 15], [-0.75, 15], [-0.75, 0]])
            {
                const p = b + n * q[0] + u * q[1];
                pts = append(pts, [p[0], p[1]]);
            }
            mkPolyXZ(context, id + "seatback", pts, -9.25, 9.25);
            paintBody(context, qCreatedBy(id + "seatback", EntityType.BODY), "Folding seatback 18 x 15: 1/4 marine plywood, 1-1/2 in closed-cell front pad, 1/2 in rear pad, marine vinyl", seatC);
        }

        // ---- Steering stick ----
        mkBox(context, id + "stickBase", STICK[0] - 1.5, STICK[0] + 1.5, STICK[1] - 2, STICK[1] + 2, 1.5, 1.625);
        paintBody(context, qCreatedBy(id + "stickBase", EntityType.BODY), "Stick base plate on the rib", alu);
        for (var s in [1, -1])
        {
            const cid = id + (s > 0 ? "cheekP" : "cheekS");
            mkBox(context, cid, STICK[0] - 1, STICK[0] + 1, STICK[1] + s * 1.3 - 0.0625, STICK[1] + s * 1.3 + 0.0625, 1.625, 4.5);
            paintBody(context, qCreatedBy(cid, EntityType.BODY), "Stick pivot cheek 1/8 6061", alu);
        }
        mkCyl(context, id + "stick", vector(STICK[0], STICK[1], 3.5), vector(STICK[0], STICK[1], 27.5), 0.75);
        paintBody(context, qCreatedBy(id + "stick", EntityType.BODY), "Steering stick 1-1/2 x 0.120 6061", alu);
        mkCyl(context, id + "grip", vector(STICK[0], STICK[1], 27.5), vector(STICK[0], STICK[1], 31.5), 0.9);
        paintBody(context, qCreatedBy(id + "grip", EntityType.BODY), "Stick grip", dark);
        mkPipe(context, id + "rudderCable", [vector(STICK[0], 21.6, 11.5), vector(90, 29.5, 13.5), vector(75, 30.8, 14.6), vector(40, 30.8, 15), vector(30, 24, 15.2), vector(20, 18.5, 16.5), vector(3, 18, 19.6)], 0.2);
        paintBody(context, qCreatedBy(id + "rudderCable", EntityType.BODY), "Rudder push-pull cable", lineC);

        // ---- Throttle pedal and heel rests ----
        mkHalfRound(context, id + "heelA", 100.9, PEDAL_Y - 3, PEDAL_Y + 3, 0.5);
        paintBody(context, qCreatedBy(id + "heelA", EntityType.BODY), "Heel rest half-round, aft", alu);
        mkHalfRound(context, id + "heelB", 102.7, PEDAL_Y - 3, PEDAL_Y + 3, 0.5);
        paintBody(context, qCreatedBy(id + "heelB", EntityType.BODY), "Heel rest half-round, forward", alu);
        mkBox(context, id + "pedalBase", 103.3, 104.3, PEDAL_Y - 2, PEDAL_Y + 2, FLOOR_Z, 0.8);
        paintBody(context, qCreatedBy(id + "pedalBase", EntityType.BODY), "Pedal hinge base", alu);
        mkPolyXZ(context, id + "pedal", [[103.8, 0.8], [110, 5.2], [109.89, 5.35], [103.69, 0.95], [103.8, 0.8]], PEDAL_Y - 1.75, PEDAL_Y + 1.75);
        paintBody(context, qCreatedBy(id + "pedal", EntityType.BODY), "Throttle pedal 3/16 6061", alu);
        mkPipe(context, id + "throttleCable", [vector(109, 0.33, 4.2), vector(113, -1, 4.4), vector(116, -6, 4.5), vector(118, -17, 5), vector(118, -20, 14), vector(112, -28.4, 14.4), vector(100, -29.6, 14.6), vector(89, -30.2, 14.8), vector(75, -30.8, 15), vector(22, -31.2, 15), vector(27, -12, 26), vector(27, -7.5, 31)], 0.2);
        paintBody(context, qCreatedBy(id + "throttleCable", EntityType.BODY), "Throttle push-pull cable", lineC);

        // ---- Console, starboard: open underneath, on a rib foot and the gunwale ----
        mkBox(context, id + "consoleFoot", 106.75, 109.25, -23, -19, 2.3, 2.425);
        paintBody(context, qCreatedBy(id + "consoleFoot", EntityType.BODY), "Console foot plate on the rib", alu);
        mkCyl(context, id + "consolePost", vector(108, -21, 2.4), vector(CONSOLE_X, -21, CONSOLE_Z), 0.5);
        paintBody(context, qCreatedBy(id + "consolePost", EntityType.BODY), "Console post 1 x 0.095 6061", alu);
        mkPipe(context, id + "consoleArm", [vector(CONSOLE_X, -5, CONSOLE_Z), vector(CONSOLE_X, -21, CONSOLE_Z), vector(CONSOLE_X, -33.4, 19.3)], 0.5);
        paintBody(context, qCreatedBy(id + "consoleArm", EntityType.BODY), "Console arm 1 x 0.095 6061, to the gunwale", alu);
        mkCyl(context, id + "extinguisher", vector(99.5, -25, 13.1), vector(99.5, -25, 27.1), 1.75);
        paintBody(context, qCreatedBy(id + "extinguisher", EntityType.BODY), "Fire extinguisher B-I envelope", color(0.8, 0.1, 0.1));

        // ---- Fuel tank on the front deck step ----
        mkBox(context, id + "tank", 134, 152.25, -6.3, 6.3, DECK_Z, 24.95);
        paintBody(context, qCreatedBy(id + "tank", EntityType.BODY), "Fuel tank 6 gal envelope", tankC);
        mkCyl(context, id + "filler", vector(148, 3, 24.95), vector(148, 3, 26), 1.5);
        paintBody(context, qCreatedBy(id + "filler", EntityType.BODY), "Tank filler cap", dark);
        mkCyl(context, id + "pump", vector(138, -3, 24.95), vector(138, -3, 25.6), 2);
        paintBody(context, qCreatedBy(id + "pump", EntityType.BODY), "Sled fuel pump module flange", dark);
        mkCyl(context, id + "senderHead", vector(143, 3, 24.95), vector(143, 3, 25.4), 1.25);
        paintBody(context, qCreatedBy(id + "senderHead", EntityType.BODY), "Fuel gauge sender, top mount", color(0.9, 0.8, 0.2));
        mkCyl(context, id + "senderTube", vector(143, 3, 17.2), vector(143, 3, 24.95), 0.25);
        paintBody(context, qCreatedBy(id + "senderTube", EntityType.BODY), "Fuel gauge sender tube", color(0.9, 0.8, 0.2));
        const strap = [[-6.4, DECK_Z], [-6.4, 25.05], [6.4, 25.05], [6.4, DECK_Z], [6.3, DECK_Z], [6.3, 24.95], [-6.3, 24.95], [-6.3, DECK_Z], [-6.4, DECK_Z]];
        for (var sx in [140.5, 146])
        {
            const sid = id + ("strap" ~ round(sx));
            mkPoly(context, sid, strap, sx, sx + 1);
            paintBody(context, qCreatedBy(sid, EntityType.BODY), "Tank tie-down strap", dark);
        }
        mkPipe(context, id + "fuelLine", [vector(138, -3, 25.6), vector(134, -8, 26), vector(131, -12, 18), vector(125, -27.4, 16), vector(105, -29.6, 15.8), vector(22, -31.2, 15.8), vector(26, -12, 24), vector(26, -7.5, 30)], 0.2);
        paintBody(context, qCreatedBy(id + "fuelLine", EntityType.BODY), "Fuel line", color(0.1, 0.25, 0.1));

        // ---- Harness: gland in the dry box aft wall, then corrugated conduit on rib standoffs ----
        const conduitC = color(0.2, 0.2, 0.22, 0.45);
        mkPipe(context, id + "harnessConduit", [vector(BOX[0], 0, 5.3), vector(75.3, 0, 5), vector(72, 0, 4), vector(40, 0, 4), vector(34.6, -4, 4.2), vector(34.6, -10.5, 4.8)], 1);
        paintBody(context, qCreatedBy(id + "harnessConduit", EntityType.BODY), "Harness conduit, 2 in corrugated self-draining, UL 94 V-2", conduitC);

        // ---- Sled oil tank, upright, bracketed to the starboard side frame ----
        mkBox(context, id + "oilTank", 12.6, 22.4, -17, -10.5, 17, 26);
        paintBody(context, qCreatedBy(id + "oilTank", EntityType.BODY), "Envelope - sled oil tank", color(0.85, 0.7, 0.3, 0.55));
        mkCyl(context, id + "oilFiller", vector(17.5, -13.75, 26), vector(17.5, -13.75, 26.8), 1);
        paintBody(context, qCreatedBy(id + "oilFiller", EntityType.BODY), "Oil tank filler cap", dark);

        // ---- Oars: oarlocks on 4 in risers at X 66, oars shipped in the locks along inside each rail ----
        // The rower sits facing aft on the folded seatback (top about Z 16.4), so the pins are about 9 in above the seat and
        // the hands pass beside the knees. Shipped, each oar swings blade forward in its ring and lies on a rest at X 118,
        // inside the rail and about 4 in above the console arm where it meets the starboard gunwale.
        const oarC = color(0.75, 0.77, 0.8);
        const bladeC = color(0.1, 0.1, 0.1);
        const ringC = color(0.6, 0.6, 0.62);
        for (var os in [1, -1])
        {
            const sn = os > 0 ? "port" : "starboard";
            const oy = function(x) { return os * (OARLOCK[1] - (x - OARLOCK[0]) * 3 / 52); };
            const rz = OARLOCK[2] + 0.125;
            mkBox(context, id + ("oarDoubler" ~ sn), OARLOCK[0] - 2, OARLOCK[0] + 2, os * (OARLOCK[1] - 0.75), os * (OARLOCK[1] + 0.75), OARLOCK[2], rz);
            paintBody(context, qCreatedBy(id + ("oarDoubler" ~ sn), EntityType.BODY), "Oarlock doubler 1/8 6061 - " ~ sn, alu);
            mkBox(context, id + ("oarRiser" ~ sn), OARLOCK[0] - 1.5, OARLOCK[0] + 1.5, os * (OARLOCK[1] - 0.75), os * (OARLOCK[1] + 0.75), rz, rz + OARLOCK[3]);
            paintBody(context, qCreatedBy(id + ("oarRiser" ~ sn), EntityType.BODY), "Oarlock riser 1-1/2 x 3 x 4 6061, pocketed - " ~ sn, alu);
            const sz = rz + OARLOCK[3];
            mkCyl(context, id + ("oarSocket" ~ sn), vector(OARLOCK[0], os * OARLOCK[1], sz), vector(OARLOCK[0], os * OARLOCK[1], sz + 0.15), 0.9);
            paintBody(context, qCreatedBy(id + ("oarSocket" ~ sn), EntityType.BODY), "Oarlock socket, top mount - " ~ sn, ringC);
            mkCyl(context, id + ("oarPin" ~ sn), vector(OARLOCK[0], os * OARLOCK[1], sz + 0.15), vector(OARLOCK[0], os * OARLOCK[1], OAR_Z - 1.05), 0.25);
            paintBody(context, qCreatedBy(id + ("oarPin" ~ sn), EntityType.BODY), "Oarlock shank - " ~ sn, ringC);
            var ringPts = [];
            for (var k = 0; k <= 8; k += 1)
                ringPts = append(ringPts, vector(OARLOCK[0], os * OARLOCK[1] + 1.1 * cos(k * 45 * degree), OAR_Z + 1.1 * sin(k * 45 * degree)));
            mkPipe(context, id + ("oarRing" ~ sn), ringPts, 0.15);
            paintBody(context, qCreatedBy(id + ("oarRing" ~ sn), EntityType.BODY), "Oarlock, closed ring - " ~ sn, ringC);

            // Forward rest: an angle clip on the rail with a padded cradle and a rubber strap over the shaft.
            const rx = 118;
            mkBox(context, id + ("oarRestPost" ~ sn), rx - 0.5, rx + 0.5, os * 32.2, os * 32.325, OARLOCK[2] - 1.5, OAR_Z - 0.8);
            paintBody(context, qCreatedBy(id + ("oarRestPost" ~ sn), EntityType.BODY), "Oar rest clip 1/8 6061 on the rail - " ~ sn, alu);
            mkBox(context, id + ("oarRest" ~ sn), rx - 0.75, rx + 0.75, os * 30.2, os * 32.325, OAR_Z - 1, OAR_Z - 0.8);
            paintBody(context, qCreatedBy(id + ("oarRest" ~ sn), EntityType.BODY), "Oar rest cradle, padded, with rubber strap - " ~ sn, dark);

            // Shipped oar, 84 in: grip 42-47, shaft to the blade at 104, blade 104-126 lying flat; stop just aft of the ring.
            mkCyl(context, id + ("oarGrip" ~ sn), vector(42, oy(42), OAR_Z), vector(47, oy(47), OAR_Z), 0.65);
            paintBody(context, qCreatedBy(id + ("oarGrip" ~ sn), EntityType.BODY), "Oar 7 ft, grip - " ~ sn, bladeC);
            mkCyl(context, id + ("oarShaft" ~ sn), vector(47, oy(47), OAR_Z), vector(104, oy(104), OAR_Z), 0.8);
            paintBody(context, qCreatedBy(id + ("oarShaft" ~ sn), EntityType.BODY), "Oar 7 ft, aluminum shaft - " ~ sn, oarC);
            mkCyl(context, id + ("oarStop" ~ sn), vector(64.2, oy(64.2), OAR_Z), vector(64.8, oy(64.8), OAR_Z), 1.05);
            paintBody(context, qCreatedBy(id + ("oarStop" ~ sn), EntityType.BODY), "Oar rubber stop, 24 in inboard - " ~ sn, bladeC);
            const yb = oy(115);
            mkBox(context, id + ("oarBlade" ~ sn), 104, 126, yb - 3, yb + 3, OAR_Z - 0.3, OAR_Z + 0.3);
            paintBody(context, qCreatedBy(id + ("oarBlade" ~ sn), EntityType.BODY), "Oar 7 ft, blade 6 x 22 - " ~ sn, bladeC);
        }

        // ---- Capsize flotation foam, added to the foam in the rear bench. CAD/analysis/flotation_inverted.js has the sizing. ----
        // Footwell side blocks, 6 in thick against the hull side, Z 1.5 to 13, forward of the stick; the starboard one stops
        // short of the console foot. Each run is cut to the hull width at its forward end.
        const foamC1 = color(0.18, 0.56, 0.51, 0.6);
        for (var fb in [[1, 98, 113], [1, 113, 128], [-1, 98, 106], [-1, 110, 119], [-1, 119, 128]])
        {
            const bid = id + ("foamSide" ~ (fb[0] > 0 ? "P" : "S") ~ fb[1]);
            const o0 = hullHalf(fb[2], 1.5) - 0.3;
            const o1 = hullHalf(fb[2], 13) - 0.3;
            mkPoly(context, bid, [[fb[0] * (o0 - 6), 1.5], [fb[0] * o0, 1.5], [fb[0] * o1, 13], [fb[0] * (o1 - 6), 13], [fb[0] * (o0 - 6), 1.5]], fb[1], fb[2]);
            paintBody(context, qCreatedBy(bid, EntityType.BODY), "Flotation foam - footwell side block " ~ (fb[0] > 0 ? "port" : "starboard") ~ " X " ~ fb[1] ~ "-" ~ fb[2], foamC1);
        }
        const foamC = color(0.18, 0.56, 0.51, 0.6);
        mkPolyXZ(context, id + "foamWedge", [[154, 8.62], [156, 9], [156, DECK_Z], [154, DECK_Z], [154, 8.62]], -21.8, 21.8);
        paintBody(context, qCreatedBy(id + "foamWedge", EntityType.BODY), "Flotation foam - bow wedge under the shelf", foamC);
        for (var s in [1, -1])
        {
            const cid = id + (s > 0 ? "foamAftP" : "foamAftS");
            mkPoly(context, cid, [[s * 10.5, 2], [s * 24.9, 2], [s * 28.9, 10], [s * 10.5, 10], [s * 10.5, 2]], 3, 13);
            paintBody(context, qCreatedBy(cid, EntityType.BODY), "Flotation foam - aft corner block " ~ (s > 0 ? "port" : "starboard"), foamC);
        }
    });
