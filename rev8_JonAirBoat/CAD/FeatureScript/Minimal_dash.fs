FeatureScript 3112;
import(path : "onshape/std/geometry.fs", version : "3112.0");

// Dash panel of the minimal 1448, in the airboat assembly frame: X forward from the transom, Y + to port,
// Z up from the hull bottom. The pod sits on the console arm in Minimal cockpit.
//   - Top row, left to right as the driver sees it: USB charging port, gas, tach, temperature, exhaust temperature
//   - Bottom row: lanyard kill switch with coiled lanyard, searchlight joystick, searchlight switch,
//     rotary lights switch (off, running, LED bar), key switch (off, on, start), 12 V / 20 A outlet
//   - The USB port and 12 V outlet have rubber weather flaps, shown partway open
//   - Adjustable marine holder on the left side of the pod, shown with a phone in landscape; it opens for a tablet
//   - On a shelf on top of the pod: round bubble level for list and trim, and the maximum load label beside it
// Sizes are placeholders until the real parts are bought.

const POD_X = 100;      // aft edge of the pod at its bottom
const POD_Z = 24;       // same as CONSOLE_Z in Minimal cockpit
const POD_Y = [-21.25, -4.75];
const FACE_LEN = 6.82;
const TOP_T = 4.75;     // row heights along the face, from its bottom edge
const LOW_T = 1.7;
const LABEL_H = 0.26;
const LIGHTS_Y = -14.3;
const KEY_Y = -17;
const FLAP_DEG = 55;    // how far the weather flaps are shown open
const SHELF_Y = [-14.7, -4.9];
// Text of the load label and its letter height. The numbers come from CAD/analysis/minimal_1448.js.
const LOAD_LABEL = [["MAXIMUM LOAD 395 LB", 0.33], ["PEOPLE + GEAR + FUEL", 0.24], ["FULL TANK 45 LB (EMPTY 8 LB) LEAVES 350 LB", 0.19],
    ["KEEPS YOU WITHIN THE 825 LB CAPACITY PLATE", 0.19], ["SHIFT GEAR TO CENTER BUBBLE. TIE DOWN", 0.19]];

function mkCyl(context is Context, id is Id, a is Vector, b is Vector, r is number)
{
    fCylinder(context, id, { "bottomCenter" : a * inch, "topCenter" : b * inch, "radius" : r * inch });
}

function paintBody(context is Context, q is Query, name is string, c is Color)
{
    setProperty(context, { "entities" : q, "propertyType" : PropertyType.NAME, "value" : name });
    setProperty(context, { "entities" : q, "propertyType" : PropertyType.APPEARANCE, "value" : c });
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

// Rubber flap hinged on the right edge of a round port, swung out from the face by FLAP_DEG.
function mkFlap(context is Context, id is Id, c is Vector, T is Vector, N is Vector, r is number)
{
    const left = vector(0, 1, 0);
    const open = left * cos(FLAP_DEG * degree) + N * sin(FLAP_DEG * degree);
    const mid = c - left * r + open * r;
    const n = normalize(cross(open, T));
    mkCyl(context, id, mid - n * 0.04, mid + n * 0.04, r);
}

// A square-on block on the phone mount plane: u and v along the screen, w out of it.
function mkPlate(context is Context, id is Id, c is Vector, u is Vector, v is Vector, w is Vector, a is number, b is number, t is number)
{
    const sk = newSketchOnPlane(context, id + "sk", { "sketchPlane" : plane(c * inch, w, u) });
    skRectangle(sk, "rect", { "firstCorner" : vector(-a / 2, -b / 2) * inch, "secondCorner" : vector(a / 2, b / 2) * inch });
    skSolve(sk);
    opExtrude(context, id + "ex", {
                "entities" : qSketchRegion(id + "sk"),
                "direction" : w,
                "endBound" : BoundingType.BLIND,
                "endDepth" : t * inch
            });
    opDeleteBodies(context, id + "del", { "entities" : qCreatedBy(id + "sk", EntityType.BODY) });
}

annotation { "Feature Type Name" : "Minimal dash" }
export const minimalDash = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
    }
    {
        const podC = color(0.55, 0.58, 0.62);
        const black = color(0.1, 0.1, 0.12);
        const red = color(0.8, 0.1, 0.1);
        const metal = color(0.72, 0.74, 0.76);
        const rubber = color(0.28, 0.29, 0.3);

        const B = vector(POD_X, 0, POD_Z + 1.2);     // bottom edge of the face, on the centerline
        const T = normalize(vector(2.6, 0, 6.3));    // up the face
        const N = normalize(vector(-6.3, 0, 2.6));   // out toward the driver
        const at = function(y, t) { return B + vector(0, y, 0) + T * t; };

        const px = POD_X;
        const pz = POD_Z;
        mkPolyXZ(context, id + "pod", [[px, pz + 0.5], [px + 5.3, pz + 0.5], [px + 5.3, pz + 4], [px + 3.2, pz + 7.5], [px + 2.6, pz + 7.5], [px, pz + 1.2], [px, pz + 0.5]], POD_Y[0], POD_Y[1]);

        // Labels are cut into the pod so they stay part of it.
        const labels = [["USB", -6.6, TOP_T + 1.75, LABEL_H], ["GAS", -9.6, TOP_T + 1.75, LABEL_H], ["TACH", -13, TOP_T + 1.75, LABEL_H], ["TEMP", -16.4, TOP_T + 1.75, LABEL_H], ["EXHAUST", -19.4, TOP_T + 1.75, LABEL_H],
            ["KILL", -6.2, LOW_T + 0.95, LABEL_H], ["SEARCH", -8.9, 0.2, LABEL_H], ["ON/OFF", -11.6, 0.2, LABEL_H], ["LIGHTS", LIGHTS_Y, 0.2, LABEL_H], ["KEY", KEY_Y, 0.2, LABEL_H], ["12V", -19.7, 0.2, LABEL_H],
            ["OFF", LIGHTS_Y + 1.15, LOW_T + 0.65, 0.17], ["RUN", LIGHTS_Y, LOW_T + 0.98, 0.17], ["LED", LIGHTS_Y - 1.15, LOW_T + 0.65, 0.17],
            ["OFF", KEY_Y, LOW_T + 0.72, 0.17], ["ON", KEY_Y - 0.62, LOW_T + 0.52, 0.17], ["START", KEY_Y - 1.05, LOW_T - 0.08, 0.17]];
        const sk = newSketchOnPlane(context, id + "labelSk", { "sketchPlane" : plane(B * inch, N, vector(0, -1, 0)) });
        for (var i = 0; i < size(labels); i += 1)
        {
            const lb = labels[i];
            const w = length(lb[0]) * lb[3] * 0.77;
            skText(sk, "text" ~ i, { "text" : lb[0], "fontName" : "OpenSans-Bold.ttf",
                        "firstCorner" : vector(-lb[1] - w / 2, lb[2]) * inch,
                        "secondCorner" : vector(-lb[1] + w / 2, lb[2] + lb[3]) * inch });
        }
        skSolve(sk);
        opExtrude(context, id + "labelEx", {
                    "entities" : qSketchRegion(id + "labelSk"),
                    "direction" : -N,
                    "endBound" : BoundingType.BLIND,
                    "endDepth" : 0.03 * inch
                });
        opBoolean(context, id + "labelCut", { "targets" : qCreatedBy(id + "pod", EntityType.BODY), "tools" : qCreatedBy(id + "labelEx", EntityType.BODY), "operationType" : BooleanOperationType.SUBTRACTION });
        opDeleteBodies(context, id + "labelDel", { "entities" : qCreatedBy(id + "labelSk", EntityType.BODY) });
        paintBody(context, qCreatedBy(id + "pod", EntityType.BODY), "Dash pod 0.063 aluminum sheet", podC);

        // ---- Top row ----
        const gauges = [["gas", -9.6, 1.05, "Gas gauge, analog 2-1/16"], ["tach", -13, 1.6, "Tachometer, analog 3-3/8"], ["temp", -16.4, 1.05, "Temperature gauge, analog 2-1/16"], ["egt", -19.4, 1.05, "Exhaust temperature gauge, analog 2-1/16"]];
        for (var g in gauges)
        {
            const c = at(g[1], TOP_T);
            const gid = id + g[0];
            mkCyl(context, gid + "Bezel", c, c + N * 0.3, g[2]);
            paintBody(context, qCreatedBy(gid + "Bezel", EntityType.BODY), g[3] ~ " bezel", metal);
            mkCyl(context, gid + "Dial", c + N * 0.3, c + N * 0.32, g[2] - 0.14);
            paintBody(context, qCreatedBy(gid + "Dial", EntityType.BODY), g[3] ~ " dial", black);
            mkCyl(context, gid + "Needle", c + N * 0.33, c + N * 0.33 + (T * 0.7 + vector(0, -0.45, 0)) * (g[2] - 0.2) / 0.83 + N * 0.01, 0.035);
            paintBody(context, qCreatedBy(gid + "Needle", EntityType.BODY), g[3] ~ " needle", color(0.95, 0.35, 0.1));
        }
        const usb = at(-6.6, TOP_T);
        mkCyl(context, id + "usb", usb, usb + N * 0.12, 0.6);
        paintBody(context, qCreatedBy(id + "usb", EntityType.BODY), "USB charging port", black);
        mkCyl(context, id + "usbA", usb + N * 0.12 + vector(0, 0.22, 0), usb + N * 0.14 + vector(0, 0.22, 0), 0.14);
        paintBody(context, qCreatedBy(id + "usbA", EntityType.BODY), "USB socket A", metal);
        mkCyl(context, id + "usbB", usb + N * 0.12 + vector(0, -0.22, 0), usb + N * 0.14 + vector(0, -0.22, 0), 0.14);
        paintBody(context, qCreatedBy(id + "usbB", EntityType.BODY), "USB socket B", metal);
        mkFlap(context, id + "usbFlap", usb + N * 0.13, T, N, 0.62);
        paintBody(context, qCreatedBy(id + "usbFlap", EntityType.BODY), "USB rubber weather flap", rubber);

        // ---- Bottom row ----
        // Lanyard kill switch: a plunger held out by a red fork on a coiled lanyard. Pull the fork and the engine stops.
        const kill = at(-6.2, LOW_T);
        mkCyl(context, id + "killBase", kill, kill + N * 0.3, 0.75);
        paintBody(context, qCreatedBy(id + "killBase", EntityType.BODY), "Lanyard kill switch body", black);
        mkCyl(context, id + "killCap", kill + N * 0.3, kill + N * 0.75, 0.2);
        mkCyl(context, id + "killHead", kill + N * 0.75, kill + N * 0.88, 0.42);
        opBoolean(context, id + "killPlunger", { "targets" : qCreatedBy(id + "killCap", EntityType.BODY), "tools" : qCreatedBy(id + "killHead", EntityType.BODY), "operationType" : BooleanOperationType.UNION, "targetsAndToolsNeedGrouping" : true });
        paintBody(context, qCreatedBy(id + "killCap", EntityType.BODY), "Lanyard kill switch plunger", black);

        // Fork around the plunger, then the coil, hanging straight down.
        const forkC = kill + N * 0.6;
        const forkDown = normalize(vector(-T[0], 0, -T[2]));
        mkPlate(context, id + "lanyard", forkC + forkDown * 0.25, vector(0, -1, 0), T, N, 0.75, 0.9, 0.08);
        var coil = [];
        const coilTop = forkC + forkDown * 0.7 + N * 0.15;
        const turns = 9;
        const steps = turns * 8;
        for (var i = 0; i <= steps; i += 1)
        {
            const a = 2 * PI * i / 8;
            coil = append(coil, coilTop + vector(0.26 * cos(a * radian), 0.26 * sin(a * radian), -i * 0.065));
        }
        var coilBodies = [];
        for (var i = 0; i < steps; i += 1)
        {
            const sid = id + ("coil" ~ i);
            const d = coil[i + 1] - coil[i];
            mkCyl(context, sid, coil[i] - d * 0.08, coil[i + 1] + d * 0.08, 0.05);
            coilBodies = append(coilBodies, qCreatedBy(sid, EntityType.BODY));
        }
        mkCyl(context, id + "coilLead", forkC + forkDown * 0.65 + N * 0.04, coil[0] + (coil[0] - coil[1]) * 0.1, 0.05);
        opBoolean(context, id + "lanyardJoin", { "targets" : qCreatedBy(id + "lanyard", EntityType.BODY), "tools" : qUnion(append(coilBodies, qCreatedBy(id + "coilLead", EntityType.BODY))), "operationType" : BooleanOperationType.UNION, "targetsAndToolsNeedGrouping" : true });
        paintBody(context, qCreatedBy(id + "lanyard", EntityType.BODY), "Kill switch fork and coiled lanyard", red);
        const clipTop = coil[steps];
        mkCyl(context, id + "lanyardClip", clipTop, clipTop + vector(0, 0, -0.9), 0.14);
        paintBody(context, qCreatedBy(id + "lanyardClip", EntityType.BODY), "Lanyard snap clip, to the driver", metal);

        const stick = at(-8.9, LOW_T);
        mkCyl(context, id + "joyBase", stick, stick + N * 0.2, 0.8);
        paintBody(context, qCreatedBy(id + "joyBase", EntityType.BODY), "Searchlight joystick base", black);
        mkCyl(context, id + "joyStem", stick + N * 0.2, stick + N * 1, 0.13);
        paintBody(context, qCreatedBy(id + "joyStem", EntityType.BODY), "Searchlight joystick stem", metal);
        opSphere(context, id + "joyKnob", { "center" : (stick + N * 1.1) * inch, "radius" : 0.3 * inch });
        paintBody(context, qCreatedBy(id + "joyKnob", EntityType.BODY), "Searchlight joystick knob", black);

        const seek = at(-11.6, LOW_T);
        mkCyl(context, id + "seekNut", seek, seek + N * 0.12, 0.35);
        paintBody(context, qCreatedBy(id + "seekNut", EntityType.BODY), "Searchlight on/off switch bezel", metal);
        mkCyl(context, id + "seekLever", seek + N * 0.12, seek + N * 0.75 + T * 0.18, 0.09);
        paintBody(context, qCreatedBy(id + "seekLever", EntityType.BODY), "Searchlight on/off switch lever", black);

        // Rotary lights switch, three positions: off, running, LED bar. The pointer is shown at off.
        const lit = at(LIGHTS_Y, LOW_T);
        mkCyl(context, id + "lightsNut", lit, lit + N * 0.08, 0.7);
        paintBody(context, qCreatedBy(id + "lightsNut", EntityType.BODY), "Rotary lights switch plate", metal);
        mkCyl(context, id + "lightsLever", lit + N * 0.08, lit + N * 0.55, 0.5);
        paintBody(context, qCreatedBy(id + "lightsLever", EntityType.BODY), "Rotary lights switch knob", black);
        const offDir = normalize(T + vector(0, 1, 0));
        mkCyl(context, id + "lightsPointer", lit + N * 0.56, lit + N * 0.56 + offDir * 0.62, 0.06);
        paintBody(context, qCreatedBy(id + "lightsPointer", EntityType.BODY), "Rotary lights switch pointer", color(0.95, 0.95, 0.9));

        // Key switch: a vertical slot at off (12 o'clock), on at 1:30, start at 3:00.
        const key = at(KEY_Y, LOW_T);
        mkCyl(context, id + "keyBezel", key, key + N * 0.2, 0.6);
        paintBody(context, qCreatedBy(id + "keyBezel", EntityType.BODY), "Key switch", metal);
        mkPlate(context, id + "keySlot", key + N * 0.2, vector(0, -1, 0), T, N, 0.13, 0.66, 0.015);
        paintBody(context, qCreatedBy(id + "keySlot", EntityType.BODY), "Key slot", black);

        const outlet = at(-19.7, LOW_T);
        mkCyl(context, id + "outlet", outlet, outlet + N * 0.15, 0.7);
        paintBody(context, qCreatedBy(id + "outlet", EntityType.BODY), "12 V 20 A outlet", black);
        mkCyl(context, id + "outletCap", outlet + N * 0.15, outlet + N * 0.18, 0.47);
        paintBody(context, qCreatedBy(id + "outletCap", EntityType.BODY), "12 V socket ring", metal);
        mkCyl(context, id + "outletBore", outlet + N * 0.18, outlet + N * 0.19, 0.34);
        paintBody(context, qCreatedBy(id + "outletBore", EntityType.BODY), "12 V socket", black);
        mkFlap(context, id + "outletFlap", outlet + N * 0.16, T, N, 0.72);
        paintBody(context, qCreatedBy(id + "outletFlap", EntityType.BODY), "12 V rubber weather flap", rubber);

        // ---- Level shelf on top of the pod ----
        // A level must lie flat, so it can't go on the sloped face. Bent 0.063 sheet (drawn thicker), on the pod ridge and its forward corner.
        const shelfZ = pz + 7.6;
        mkPolyXZ(context, id + "shelf", [[px + 2.6, shelfZ], [px + 5.4, shelfZ], [px + 5.4, pz + 4], [px + 5.3, pz + 4], [px + 5.3, pz + 7.5], [px + 2.6, pz + 7.5], [px + 2.6, shelfZ]], SHELF_Y[0], SHELF_Y[1]);
        paintBody(context, qCreatedBy(id + "shelf", EntityType.BODY), "Level shelf, 0.063 aluminum sheet", podC);

        // Two-axis round bubble level, 2-3/4 in, shimmed at setup so a centered bubble is the design trim.
        const lvl = vector(px + 4, -6.4, shelfZ);
        mkCyl(context, id + "lvlBody", lvl, lvl + vector(0, 0, 0.4), 1.375);
        paintBody(context, qCreatedBy(id + "lvlBody", EntityType.BODY), "Round bubble level, body", black);
        mkCyl(context, id + "lvlVial", lvl + vector(0, 0, 0.4), lvl + vector(0, 0, 0.42), 1.2);
        paintBody(context, qCreatedBy(id + "lvlVial", EntityType.BODY), "Round bubble level, vial", color(0.75, 0.9, 0.35));
        mkCyl(context, id + "lvlRing", lvl + vector(0, 0, 0.42), lvl + vector(0, 0, 0.43), 0.55);
        paintBody(context, qCreatedBy(id + "lvlRing", EntityType.BODY), "Round bubble level, center ring", black);
        mkCyl(context, id + "lvlDisk", lvl + vector(0, 0, 0.43), lvl + vector(0, 0, 0.44), 0.5);
        paintBody(context, qCreatedBy(id + "lvlDisk", EntityType.BODY), "Round bubble level, center", color(0.75, 0.9, 0.35));
        mkCyl(context, id + "lvlBubble", lvl + vector(0, 0, 0.44), lvl + vector(0, 0, 0.45), 0.34);
        paintBody(context, qCreatedBy(id + "lvlBubble", EntityType.BODY), "Round bubble level, bubble", color(0.97, 0.98, 0.9));

        // Maximum load label. Reads from the driver's seat: up the label is forward, the driver's left is port.
        const lc = vector(px + 4, -11.4, shelfZ);
        fCuboid(context, id + "tag", { "corner1" : (lc + vector(-1.25, -3.25, 0)) * inch, "corner2" : (lc + vector(1.25, 3.25, 0.02)) * inch });
        paintBody(context, qCreatedBy(id + "tag", EntityType.BODY), "Maximum load label", color(0.97, 0.97, 0.94));
        const tagSk = newSketchOnPlane(context, id + "tagSk", { "sketchPlane" : plane((lc + vector(0, 0, 0.02)) * inch, vector(0, 0, 1), vector(0, -1, 0)) });
        var ty = 1.02;
        for (var i = 0; i < size(LOAD_LABEL); i += 1)
        {
            const ln = LOAD_LABEL[i];
            ty -= ln[1];
            const w = length(ln[0]) * ln[1] * 0.77;
            skText(tagSk, "line" ~ i, { "text" : ln[0], "fontName" : "OpenSans-Bold.ttf",
                        "firstCorner" : vector(-w / 2, ty) * inch, "secondCorner" : vector(w / 2, ty + ln[1]) * inch });
            ty -= 0.16;
        }
        skSolve(tagSk);
        opExtrude(context, id + "tagText", {
                    "entities" : qSketchRegion(id + "tagSk", true),
                    "direction" : vector(0, 0, 1),
                    "endBound" : BoundingType.BLIND,
                    "endDepth" : 0.01 * inch
                });
        opDeleteBodies(context, id + "tagDel", { "entities" : qCreatedBy(id + "tagSk", EntityType.BODY) });
        paintBody(context, qCreatedBy(id + "tagText", EntityType.BODY), "Maximum load label text", black);

        // ---- Adjustable holder on the left side of the pod ----
        const armA = vector(px + 2, POD_Y[1], pz + 3.2);
        const armB = vector(px + 0.6, POD_Y[1] + 4.2, pz + 3.8);
        mkCyl(context, id + "mountBase", armA + vector(0, -0.02, 0), armA + vector(0, 0.2, 0), 1.25);
        paintBody(context, qCreatedBy(id + "mountBase", EntityType.BODY), "Holder base plate, through-bolted", black);
        mkCyl(context, id + "mountArm", armA, armB, 0.55);
        paintBody(context, qCreatedBy(id + "mountArm", EntityType.BODY), "Holder arm, locks by its knob", black);
        opSphere(context, id + "mountBallA", { "center" : armA * inch, "radius" : 0.75 * inch });
        paintBody(context, qCreatedBy(id + "mountBallA", EntityType.BODY), "Holder ball", black);
        opSphere(context, id + "mountBallB", { "center" : armB * inch, "radius" : 0.75 * inch });
        paintBody(context, qCreatedBy(id + "mountBallB", EntityType.BODY), "Holder ball", black);
        const screen = armB + vector(0, 1.3, 0) + N * 0.9;  // clear of the kill switch from the driver's eye
        const u = vector(0, -1, 0);
        const phoneL = 6.4;   // landscape; the clamp slides out along the rail for a tablet up to 8 x 5.5 in
        const phoneH = 3.1;
        mkPlate(context, id + "cradle", screen, u, T, N, 8.6, 1.1, 0.3);
        mkPlate(context, id + "clampL", screen + u * (phoneL / 2 + 0.15), u, T, N, 0.3, phoneH + 0.5, 0.75);
        mkPlate(context, id + "clampR", screen - u * (phoneL / 2 + 0.15), u, T, N, 0.3, phoneH + 0.5, 0.75);
        mkPlate(context, id + "clampFoot", screen - T * (phoneH / 2 + 0.15), u, T, N, 2.4, 0.3, 0.75);
        mkPlate(context, id + "clampStem", screen - T * 0.9, u, T, N, 1.1, 2, 0.3);
        opBoolean(context, id + "cradleJoin", { "targets" : qCreatedBy(id + "cradle", EntityType.BODY), "tools" : qUnion([qCreatedBy(id + "clampL", EntityType.BODY), qCreatedBy(id + "clampR", EntityType.BODY), qCreatedBy(id + "clampFoot", EntityType.BODY), qCreatedBy(id + "clampStem", EntityType.BODY)]), "operationType" : BooleanOperationType.UNION, "targetsAndToolsNeedGrouping" : true });
        paintBody(context, qCreatedBy(id + "cradle", EntityType.BODY), "Adjustable clamp cradle, phone or tablet", black);
        mkPlate(context, id + "tablet", screen + N * 0.3, u, T, N, phoneL, phoneH, 0.32);
        paintBody(context, qCreatedBy(id + "tablet", EntityType.BODY), "Phone in landscape", color(0.12, 0.2, 0.32));
    });
