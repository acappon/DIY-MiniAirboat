FeatureScript 3112;
import(path : "onshape/std/geometry.fs", version : "3112.0");

// Cockpit of the minimal 1448, in the airboat assembly frame: X forward from the transom, Y + to port,
// Z up from the hull bottom. The driver sits on the middle bench (Crew mannequins).
//   - Dry box set into the middle bench: lithium battery and power distribution fuse block
//   - Steering stick to port on the rib at X 95.25, with a push-pull cable along the port side and across the rear bench
//   - Throttle pedal with two half-round heel rests, with a push-pull cable ahead of the toes and along the starboard side
//   - Console frame to starboard, clear above the knees of two people side by side (the pod is in Minimal dash)
//   - Fuel tank on the front deck step with its gauge sender, straps and fuel line
//   - Harness sleeve through the middle bench foam, and floor conduit on the centerline to the engine
//   - Sled oil tank upright on the starboard side frame of the stand, away from the exhaust; envelope until it is measured
//   - Capsize flotation foam: bow wedge under the shelf and two aft corner blocks, sized in CAD/analysis/flotation_inverted.js
// Sizes are placeholders until the real parts are bought, and the cable and line routes are provisional.
// The hull sides taper in from about X 75: keep runs along them about 1 in inboard of the skin (raycast the reference mesh).

const DECK_Z = 16.55;   // top of the front deck step, from the reference mesh
const FLOOR_Z = 0.1;
const PEDAL_Y = -2.5;   // inboard, so a passenger's left foot stays beside it
const STICK = [95.25, 21];
const CONSOLE_X = 102.5;
const CONSOLE_Z = 24;   // underside; a seated knee tops out at about Z 19

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

        // ---- Dry box, lid forward, set into the bench through its forward panel ----
        mkBox(context, id + "dryBox", 82.6, 89.5, -5.3, 5.3, 2, 11.7);
        paintBody(context, qCreatedBy(id + "dryBox", EntityType.BODY), "Dry box, waterproof case 10.6 x 9.7 x 6.9", caseC);
        mkBox(context, id + "battery", 84, 87.4, -4.4, 2.5, 2.6, 7.7);
        paintBody(context, qCreatedBy(id + "battery", EntityType.BODY), "Lithium battery envelope", dark);
        mkBox(context, id + "fuseBlock", 84, 87, -2.5, 2.5, 8.2, 9.7);
        paintBody(context, qCreatedBy(id + "fuseBlock", EntityType.BODY), "Power distribution fuse block", color(0.2, 0.35, 0.7));

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
        mkPipe(context, id + "throttleCable", [vector(109, 0.33, 4.2), vector(113, -1, 4.4), vector(116, -6, 4.5), vector(118, -22, 6), vector(112, -28.4, 14.4), vector(100, -29.6, 14.6), vector(89, -30.2, 14.8), vector(75, -30.8, 15), vector(22, -31.2, 15), vector(27, -12, 26), vector(27, -7.5, 31)], 0.2);
        paintBody(context, qCreatedBy(id + "throttleCable", EntityType.BODY), "Throttle push-pull cable", lineC);

        // ---- Console, starboard: open underneath, on a rib foot and the gunwale ----
        mkBox(context, id + "consoleFoot", 106.75, 109.25, -23, -19, 2.3, 2.425);
        paintBody(context, qCreatedBy(id + "consoleFoot", EntityType.BODY), "Console foot plate on the rib", alu);
        mkCyl(context, id + "consolePost", vector(108, -21, 2.4), vector(CONSOLE_X, -21, CONSOLE_Z), 0.5);
        paintBody(context, qCreatedBy(id + "consolePost", EntityType.BODY), "Console post 1 x 0.095 6061", alu);
        mkPipe(context, id + "consoleArm", [vector(CONSOLE_X, -5, CONSOLE_Z), vector(CONSOLE_X, -21, CONSOLE_Z), vector(CONSOLE_X, -33.4, 19.3)], 0.5);
        paintBody(context, qCreatedBy(id + "consoleArm", EntityType.BODY), "Console arm 1 x 0.095 6061, to the gunwale", alu);
        mkCyl(context, id + "extinguisher", vector(99, -26, 8), vector(99, -26, 22), 1.75);
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

        // ---- Harness: rigid sleeve through the bench foam, then corrugated conduit on rib standoffs ----
        const conduitC = color(0.2, 0.2, 0.22, 0.45);
        mkCyl(context, id + "harnessSleeve", vector(82.6, 0, 6.03), vector(75.3, 0, 5), 1.125);
        paintBody(context, qCreatedBy(id + "harnessSleeve", EntityType.BODY), "Harness sleeve 2-1/4 x 0.065 6061 through the bench foam", alu);
        mkPipe(context, id + "harnessConduit", [vector(75.3, 0, 5), vector(72, 0, 4), vector(40, 0, 4), vector(34.6, -4, 4.2), vector(34.6, -10.5, 4.8)], 1);
        paintBody(context, qCreatedBy(id + "harnessConduit", EntityType.BODY), "Harness conduit, 2 in corrugated self-draining, UL 94 V-2", conduitC);

        // ---- Sled oil tank, upright, bracketed to the starboard side frame ----
        mkBox(context, id + "oilTank", 12.6, 22.4, -17, -10.5, 17, 26);
        paintBody(context, qCreatedBy(id + "oilTank", EntityType.BODY), "Envelope - sled oil tank", color(0.85, 0.7, 0.3, 0.55));
        mkCyl(context, id + "oilFiller", vector(17.5, -13.75, 26), vector(17.5, -13.75, 26.8), 1);
        paintBody(context, qCreatedBy(id + "oilFiller", EntityType.BODY), "Oil tank filler cap", dark);

        // ---- Capsize flotation foam, added to the foam in the two benches. CAD/analysis/flotation_inverted.js has the sizing. ----
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
