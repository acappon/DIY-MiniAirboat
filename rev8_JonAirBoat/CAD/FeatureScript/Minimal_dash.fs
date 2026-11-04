FeatureScript 3112;
import(path : "onshape/std/geometry.fs", version : "3112.0");

// Dash panel of the minimal 1448, in the airboat assembly frame: X forward from the transom, Y + to port,
// Z up from the hull bottom. The pod sits on the console arm in Minimal cockpit.
//   - Top row, left to right as the driver sees it: USB charging port, gas, tach, temperature
//   - Bottom row: lanyard kill switch, searchlight joystick, searchlight switch, lights switch
//     (off, running, LED bar), key switch, 12 V / 20 A outlet
//   - Marine tablet holder on the left side of the pod
// Sizes are placeholders until the real parts are bought.

const POD_X = 100;      // aft edge of the pod at its bottom
const POD_Z = 24;       // same as CONSOLE_Z in Minimal cockpit
const POD_Y = [-21.25, -4.75];
const FACE_LEN = 6.82;
const TOP_T = 4.75;     // row heights along the face, from its bottom edge
const LOW_T = 1.7;
const LABEL_H = 0.26;

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

// A square-on block on the tablet mount plane: u and v along the screen, w out of it.
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

        const B = vector(POD_X, 0, POD_Z + 1.2);     // bottom edge of the face, on the centerline
        const T = normalize(vector(2.6, 0, 6.3));    // up the face
        const N = normalize(vector(-6.3, 0, 2.6));   // out toward the driver
        const at = function(y, t) { return B + vector(0, y, 0) + T * t; };

        const px = POD_X;
        const pz = POD_Z;
        mkPolyXZ(context, id + "pod", [[px, pz + 0.5], [px + 5.3, pz + 0.5], [px + 5.3, pz + 4], [px + 3.2, pz + 7.5], [px + 2.6, pz + 7.5], [px, pz + 1.2], [px, pz + 0.5]], POD_Y[0], POD_Y[1]);

        // Labels are cut into the pod so they stay part of it.
        const labels = [["USB", -6.6, TOP_T + 1.75], ["GAS", -9.6, TOP_T + 1.75], ["TACH", -13, TOP_T + 1.75], ["TEMP", -16.4, TOP_T + 1.75],
            ["KILL", -6.2, 0.2], ["SEARCH", -8.9, 0.2], ["ON/OFF", -11.6, 0.2], ["LIGHTS", -14.3, 0.2], ["KEY", -17, 0.2], ["12V", -19.7, 0.2]];
        const sk = newSketchOnPlane(context, id + "labelSk", { "sketchPlane" : plane(B * inch, N, vector(0, -1, 0)) });
        for (var i = 0; i < size(labels); i += 1)
        {
            const lb = labels[i];
            const w = length(lb[0]) * 0.2;
            skText(sk, "text" ~ i, { "text" : lb[0], "fontName" : "OpenSans-Bold.ttf",
                        "firstCorner" : vector(-lb[1] - w / 2, lb[2]) * inch,
                        "secondCorner" : vector(-lb[1] + w / 2, lb[2] + LABEL_H) * inch });
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
        const gauges = [["gas", -9.6, 1.05, "Gas gauge, analog 2-1/16"], ["tach", -13, 1.6, "Tachometer, analog 3-3/8"], ["temp", -16.4, 1.05, "Temperature gauge, analog 2-1/16"]];
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

        // ---- Bottom row ----
        const kill = at(-6.2, LOW_T);
        mkCyl(context, id + "killBase", kill, kill + N * 0.3, 0.75);
        paintBody(context, qCreatedBy(id + "killBase", EntityType.BODY), "Lanyard kill switch", black);
        mkCyl(context, id + "killCap", kill + N * 0.3, kill + N * 0.65, 0.42);
        paintBody(context, qCreatedBy(id + "killCap", EntityType.BODY), "Lanyard kill switch cap and clip", red);

        const stick = at(-8.9, LOW_T);
        mkCyl(context, id + "joyBase", stick, stick + N * 0.2, 0.8);
        paintBody(context, qCreatedBy(id + "joyBase", EntityType.BODY), "Searchlight joystick base", black);
        mkCyl(context, id + "joyStem", stick + N * 0.2, stick + N * 1, 0.13);
        paintBody(context, qCreatedBy(id + "joyStem", EntityType.BODY), "Searchlight joystick stem", metal);
        opSphere(context, id + "joyKnob", { "center" : (stick + N * 1.1) * inch, "radius" : 0.3 * inch });
        paintBody(context, qCreatedBy(id + "joyKnob", EntityType.BODY), "Searchlight joystick knob", black);

        const toggles = [["seek", -11.6, "Searchlight on/off switch", 0.35], ["lights", -14.3, "Lights switch: off, running, LED bar", 0.42]];
        for (var t in toggles)
        {
            const c = at(t[1], LOW_T);
            mkCyl(context, id + (t[0] ~ "Nut"), c, c + N * 0.12, t[3]);
            paintBody(context, qCreatedBy(id + (t[0] ~ "Nut"), EntityType.BODY), t[2] ~ " bezel", metal);
            mkCyl(context, id + (t[0] ~ "Lever"), c + N * 0.12, c + N * 0.75 + T * 0.18, 0.09);
            paintBody(context, qCreatedBy(id + (t[0] ~ "Lever"), EntityType.BODY), t[2] ~ " lever", black);
        }

        const key = at(-17, LOW_T);
        mkCyl(context, id + "keyBezel", key, key + N * 0.2, 0.6);
        paintBody(context, qCreatedBy(id + "keyBezel", EntityType.BODY), "Key switch", metal);
        mkCyl(context, id + "keyHead", key + N * 0.2 + vector(0, -0.32, 0), key + N * 0.2 + vector(0, 0.32, 0), 0.13);
        paintBody(context, qCreatedBy(id + "keyHead", EntityType.BODY), "Key", black);

        const outlet = at(-19.7, LOW_T);
        mkCyl(context, id + "outlet", outlet, outlet + N * 0.15, 0.7);
        paintBody(context, qCreatedBy(id + "outlet", EntityType.BODY), "12 V 20 A outlet", black);
        mkCyl(context, id + "outletCap", outlet + N * 0.15, outlet + N * 0.25, 0.5);
        paintBody(context, qCreatedBy(id + "outletCap", EntityType.BODY), "12 V outlet cap", red);

        // ---- Tablet holder on the left side of the pod ----
        const armA = vector(px + 2, POD_Y[1], pz + 3.2);
        const armB = vector(px + 0.6, POD_Y[1] + 4.2, pz + 3.8);
        mkCyl(context, id + "mountBase", armA + vector(0, -0.02, 0), armA + vector(0, 0.2, 0), 1.25);
        paintBody(context, qCreatedBy(id + "mountBase", EntityType.BODY), "Tablet holder base plate, through-bolted", black);
        mkCyl(context, id + "mountArm", armA, armB, 0.55);
        paintBody(context, qCreatedBy(id + "mountArm", EntityType.BODY), "Tablet holder arm", black);
        opSphere(context, id + "mountBallA", { "center" : armA * inch, "radius" : 0.75 * inch });
        paintBody(context, qCreatedBy(id + "mountBallA", EntityType.BODY), "Tablet holder ball", black);
        opSphere(context, id + "mountBallB", { "center" : armB * inch, "radius" : 0.75 * inch });
        paintBody(context, qCreatedBy(id + "mountBallB", EntityType.BODY), "Tablet holder ball", black);
        const screen = armB + vector(0, 1.3, 0) + N * 0.9;  // clear of the kill switch from the driver's eye
        const u = vector(0, -1, 0);
        mkPlate(context, id + "cradle", screen, u, T, N, 9, 6.2, 0.5);
        paintBody(context, qCreatedBy(id + "cradle", EntityType.BODY), "Tablet cradle, marine, 8 x 5.5 in tablet", black);
        mkPlate(context, id + "tablet", screen + N * 0.5, u, T, N, 8, 5.4, 0.03);
        paintBody(context, qCreatedBy(id + "tablet", EntityType.BODY), "Tablet screen", color(0.12, 0.2, 0.32));
    });
