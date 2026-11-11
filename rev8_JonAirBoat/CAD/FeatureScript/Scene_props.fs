FeatureScript 3112;
import(path : "onshape/std/geometry.fs", version : "3112.0");

// Props for the v1.0.1.3 concept scenes, in the airboat assembly frame: X forward from the transom, Y + to port,
// Z up from the hull bottom. Not part of the boat or its BOM. Every body is named with a scene prefix so a render
// can switch a set on or off in the assembly:
//   "Rowing - "  operator facing aft on the folded seatback, the seatback folded flat, oars out in the locks
//                (pins at X 66, Y +/-34.5, Z 25.5; 24 in inboard; shaft dipping 22 degrees to the water)
//   "Gear - "    cooler, duffel bag, two life jackets and a snake tong, tied down aft of the seat
// The oars, dock lines and gear are operator gear, not boat equipment (minimal_1448.js).

const PIN = [66, 34.5, 25.5];
const DIP = 22;     // shaft angle below horizontal at the blade
const SEAT_TOP = 16.4;

function pCyl(context is Context, id is Id, a is Vector, b is Vector, r is number)
{
    fCylinder(context, id, { "bottomCenter" : a * inch, "topCenter" : b * inch, "radius" : r * inch });
}

function pSph(context is Context, id is Id, c is Vector, r is number)
{
    opSphere(context, id, { "center" : c * inch, "radius" : r * inch });
}

function pBox(context is Context, id is Id, x0 is number, x1 is number, y0 is number, y1 is number, z0 is number, z1 is number)
{
    fCuboid(context, id, { "corner1" : vector(min(x0, x1), min(y0, y1), min(z0, z1)) * inch, "corner2" : vector(max(x0, x1), max(y0, y1), max(z0, z1)) * inch });
}

function pPaint(context is Context, q is Query, name is string, c is Color)
{
    setProperty(context, { "entities" : q, "propertyType" : PropertyType.NAME, "value" : name });
    setProperty(context, { "entities" : q, "propertyType" : PropertyType.APPEARANCE, "value" : c });
}

// Flat plate centered at c, spanning a along u and b along v, t thick along w = u x v.
function pPlate(context is Context, id is Id, c is Vector, u is Vector, v is Vector, a is number, b is number, t is number)
{
    const w = normalize(cross(u, v));
    const sk = newSketchOnPlane(context, id + "sk", { "sketchPlane" : plane((c - w * t / 2) * inch, w, normalize(u)) });
    skRectangle(sk, "rect", { "firstCorner" : vector(-a / 2, -b / 2) * inch, "secondCorner" : vector(a / 2, b / 2) * inch });
    skSolve(sk);
    opExtrude(context, id + "ex", { "entities" : qSketchRegion(id + "sk"), "direction" : w, "endBound" : BoundingType.BLIND, "endDepth" : t * inch });
    opDeleteBodies(context, id + "del", { "entities" : qCreatedBy(id + "sk", EntityType.BODY) });
}

// Seated rower facing aft (-X), hip point H, hands on the oar handles at hL and hR.
function rower(context is Context, id is Id, H is Vector, hL is Vector, hR is Vector)
{
    const skin = color(0.87, 0.69, 0.55);
    const pants = color(0.2, 0.27, 0.4);
    const shoes = color(0.15, 0.12, 0.1);
    const vest = color(0.95, 0.45, 0.05);
    for (var s in [1, -1])
    {
        const sid = id + (s > 0 ? "L" : "R");
        const hip = H + vector(0, s * 4, 3);
        const knee = H + vector(-16, s * 6, 7);
        const ankle = H + vector(-22, s * 6, -11.5);
        pCyl(context, sid + "thigh", hip, knee, 3);
        pSph(context, sid + "knee", knee, 2.6);
        pCyl(context, sid + "shin", knee, ankle, 2.2);
        pCyl(context, sid + "foot", ankle + vector(1.5, 0, -0.8), ankle + vector(-8, 0, -0.8), 1.8);
        pPaint(context, qUnion([qCreatedBy(sid + "thigh", EntityType.BODY), qCreatedBy(sid + "knee", EntityType.BODY), qCreatedBy(sid + "shin", EntityType.BODY)]), "Rowing - operator leg", pants);
        pPaint(context, qCreatedBy(sid + "foot", EntityType.BODY), "Rowing - operator shoe", shoes);
        const shoulder = H + vector(0, s * 8, 23);
        const hand = s > 0 ? hL : hR;
        const elbow = (shoulder + hand) / 2 + vector(1, s * 3, -5);
        pCyl(context, sid + "upperArm", shoulder, elbow, 2);
        pSph(context, sid + "elbow", elbow, 1.9);
        pCyl(context, sid + "forearm", elbow, hand, 1.7);
        pSph(context, sid + "hand", hand, 1.9);
        pPaint(context, qUnion([qCreatedBy(sid + "upperArm", EntityType.BODY), qCreatedBy(sid + "elbow", EntityType.BODY), qCreatedBy(sid + "forearm", EntityType.BODY), qCreatedBy(sid + "hand", EntityType.BODY)]), "Rowing - operator arm", skin);
    }
    pCyl(context, id + "pelvis", H + vector(0, -5.5, 3), H + vector(0, 5.5, 3), 3.4);
    pPaint(context, qCreatedBy(id + "pelvis", EntityType.BODY), "Rowing - operator pelvis", pants);
    pCyl(context, id + "torso", H + vector(0, 0, 2), H + vector(-1, 0, 25), 6.8);
    pPaint(context, qCreatedBy(id + "torso", EntityType.BODY), "Rowing - operator torso (life vest)", vest);
    pCyl(context, id + "neck", H + vector(-1, 0, 24), H + vector(-1.5, 0, 28.5), 2.2);
    pSph(context, id + "head", H + vector(-2, 0, 32.5), 4.4);
    pPaint(context, qUnion([qCreatedBy(id + "neck", EntityType.BODY), qCreatedBy(id + "head", EntityType.BODY)]), "Rowing - operator head", skin);
    // A headlamp for the night scene.
    pCyl(context, id + "lamp", H + vector(-6.2, 0, 33.5), H + vector(-7.2, 0, 33.5), 0.9);
    pPaint(context, qCreatedBy(id + "lamp", EntityType.BODY), "Rowing - operator headlamp", color(0.95, 0.95, 0.7));
}

// One 7 ft oar out through its lock: 24 in inboard, 60 in outboard, blade squared (face fore and aft).
function oar(context is Context, id is Id, s is number) returns Vector
{
    const P = vector(PIN[0], s * PIN[1], PIN[2]);
    const d = vector(0, s * cos(DIP * degree), -sin(DIP * degree));
    const handle = P - d * 24;
    pCyl(context, id + "grip", handle, handle + d * 5, 0.65);
    pPaint(context, qCreatedBy(id + "grip", EntityType.BODY), "Rowing - oar grip", color(0.1, 0.1, 0.1));
    pCyl(context, id + "shaft", handle + d * 5, P + d * 38, 0.8);
    pPaint(context, qCreatedBy(id + "shaft", EntityType.BODY), "Rowing - oar shaft, 7 ft", color(0.75, 0.77, 0.8));
    const bw = normalize(vector(0, s * sin(DIP * degree), cos(DIP * degree)));
    pPlate(context, id + "blade", P + d * 49, d, bw, 22, 6, 0.6);
    pPaint(context, qCreatedBy(id + "blade", EntityType.BODY), "Rowing - oar blade", color(0.1, 0.1, 0.1));
    return handle + d * 1.5;
}

annotation { "Feature Type Name" : "Scene props" }
export const sceneProps = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
    }
    {
        // ---- Rowing set ----
        // The seatback folded flat forward onto the seat (hinge X 75.9), its back pad up: the rowing seat.
        pBox(context, id + "foldedBack", 76.2, 91.2, -9.25, 9.25, 14.15, SEAT_TOP);
        pPaint(context, qCreatedBy(id + "foldedBack", EntityType.BODY), "Rowing - seatback folded flat", color(0.85, 0.85, 0.8));
        const hL = oar(context, id + "oarP", 1);
        const hR = oar(context, id + "oarS", -1);
        rower(context, id + "rower", vector(81, 0, SEAT_TOP), hL, hR);

        // ---- Gear set, tied down aft of the seat, clear of the rower's feet ----
        pBox(context, id + "cooler", 38, 52, -15, -1, 2, 15);
        pPaint(context, qCreatedBy(id + "cooler", EntityType.BODY), "Gear - cooler 48 qt", color(0.95, 0.95, 0.93));
        pBox(context, id + "coolerLid", 37.6, 52.4, -15.4, -0.6, 15, 16.2);
        pPaint(context, qCreatedBy(id + "coolerLid", EntityType.BODY), "Gear - cooler lid", color(0.15, 0.35, 0.7));
        pCyl(context, id + "duffel", vector(39, 9, 8), vector(57, 9, 8), 6);
        pPaint(context, qCreatedBy(id + "duffel", EntityType.BODY), "Gear - duffel bag", color(0.25, 0.32, 0.2));
        pBox(context, id + "pfd1", 39, 51, -14, -2, 16.2, 18.7);
        pPaint(context, qCreatedBy(id + "pfd1", EntityType.BODY), "Gear - life jacket", color(1, 0.55, 0.05));
        pBox(context, id + "pfd2", 41, 53, 4, 14, 14, 16.5);
        pPaint(context, qCreatedBy(id + "pfd2", EntityType.BODY), "Gear - life jacket", color(1, 0.55, 0.05));
        pCyl(context, id + "strap1", vector(45, -15.6, 9), vector(45, 15.6, 9), 0.3);
        pPaint(context, qCreatedBy(id + "strap1", EntityType.BODY), "Gear - tie-down strap", color(0.85, 0.75, 0.1));

        // Snake tong: 52 in aluminum pole, pistol grip at the low end, jaws at the top, leaning along the port side.
        const t0 = vector(42, 20, 3);
        const t1 = vector(88, 20.5, 18);
        const td = normalize(t1 - t0);
        pCyl(context, id + "tongPole", t0, t1, 0.5);
        pPaint(context, qCreatedBy(id + "tongPole", EntityType.BODY), "Gear - snake tong pole, 52 in", color(0.8, 0.82, 0.85));
        pCyl(context, id + "tongGrip", t0 + td * 1, t0 + td * 6, 0.75);
        pPaint(context, qCreatedBy(id + "tongGrip", EntityType.BODY), "Gear - snake tong grip", color(0.1, 0.1, 0.1));
        pCyl(context, id + "tongTrigger", t0 + td * 4, t0 + td * 4 + vector(0, 0, -3), 0.3);
        pPaint(context, qCreatedBy(id + "tongTrigger", EntityType.BODY), "Gear - snake tong trigger", color(0.1, 0.1, 0.1));
        pCyl(context, id + "tongJawA", t1, t1 + td * 4 + vector(0, 0, 1.2), 0.35);
        pCyl(context, id + "tongJawB", t1, t1 + td * 4 + vector(0, 0, -1.2), 0.35);
        pPaint(context, qUnion([qCreatedBy(id + "tongJawA", EntityType.BODY), qCreatedBy(id + "tongJawB", EntityType.BODY)]), "Gear - snake tong jaws", color(0.1, 0.1, 0.1));
    });
