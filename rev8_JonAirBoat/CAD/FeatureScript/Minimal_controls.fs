FeatureScript 3112;
import(path : "onshape/std/geometry.fs", version : "3112.0");

// Steering linkage and throttle linkage of the minimal 1448, in the airboat assembly frame: X forward
// from the transom, Y + to port, Z up from the hull bottom (CAD/analysis/controls_800r.js).
//   - Rudder shafts, lower brackets on the aft hoop, upper arms to the hoop, flange bearings
//   - Horns, tie bar, and the port outboard arm that takes the push-pull cable: stick forward turns left
//   - Pedal hinge rod and crank between the driver's feet, cable stop on the rib at X 108
//   - Cable bracket at the engine, placed until the real throttle body is measured
// The rudders and the cables are in Minimal layout and Minimal cockpit.

const RUDDER_Y = 14;
const SHAFT_X = -4;     // 25% of the 8 in chord back from the leading edge at X -2
const HORN_Z = 19.625;
const CABLE_Y = 18;
const CRANK_Y = 0.33;
const HINGE = [103.8, 0.8];
const CRANK_R = 3.4;

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

annotation { "Feature Type Name" : "Minimal controls" }
export const minimalControls = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
    }
    {
        const alu = color(0.75, 0.77, 0.8);
        const steel = color(0.45, 0.47, 0.5);
        const dark = color(0.15, 0.15, 0.17);

        // ---- Rudder shafts and their supports ----
        for (var s in [1, -1])
        {
            const side = s > 0 ? "Port" : "Stbd";
            const y = s * RUDDER_Y;
            mkCyl(context, id + ("shaft" ~ side), vector(SHAFT_X, y, 18.3), vector(SHAFT_X, y, 53), 0.5);
            paintBody(context, qCreatedBy(id + ("shaft" ~ side), EntityType.BODY), "Rudder shaft 1 x 0.125 6061 - " ~ side, alu);
            mkBox(context, id + ("lower" ~ side), SHAFT_X - 0.9, 0.4, y - 0.75, y + 0.75, 18.55, 18.8);
            paintBody(context, qCreatedBy(id + ("lower" ~ side), EntityType.BODY), "Lower rudder bracket 1/4 6061, clamped to the aft hoop - " ~ side, steel);
            mkCyl(context, id + ("upper" ~ side), vector(SHAFT_X, y, 52), vector(0.8, s * 35.4, 52), 0.5);
            paintBody(context, qCreatedBy(id + ("upper" ~ side), EntityType.BODY), "Upper rudder arm 1 x 0.065 6061, to the aft hoop - " ~ side, alu);
            mkCyl(context, id + ("bearing" ~ side), vector(SHAFT_X, y, 51.4), vector(SHAFT_X, y, 52.6), 0.9);
            paintBody(context, qCreatedBy(id + ("bearing" ~ side), EntityType.BODY), "Upper rudder bearing - " ~ side, dark);
            mkBox(context, id + ("horn" ~ side), SHAFT_X - 0.6, 0.2, y - 0.5, y + 0.5, HORN_Z - 0.125, HORN_Z + 0.125);
            paintBody(context, qCreatedBy(id + ("horn" ~ side), EntityType.BODY), "Rudder horn 1/4 6061 - " ~ side, steel);
        }

        // ---- Tie bar and cable arm ----
        mkCyl(context, id + "tieBar", vector(0, -RUDDER_Y - 0.5, HORN_Z), vector(0, RUDDER_Y + 0.5, HORN_Z), 0.25);
        paintBody(context, qCreatedBy(id + "tieBar", EntityType.BODY), "Rudder tie bar 1/2 x 0.065 with rod ends", alu);
        mkBox(context, id + "cableArm", SHAFT_X - 0.5, SHAFT_X + 0.5, RUDDER_Y, CABLE_Y + 0.3, HORN_Z - 0.125, HORN_Z + 0.125);
        paintBody(context, qCreatedBy(id + "cableArm", EntityType.BODY), "Port outboard cable arm 1/4 6061", steel);
        mkCyl(context, id + "cableRod", vector(3, CABLE_Y, HORN_Z), vector(SHAFT_X, CABLE_Y, HORN_Z), 0.125);
        paintBody(context, qCreatedBy(id + "cableRod", EntityType.BODY), "Rudder cable end rod", steel);
        mkBox(context, id + "cableClamp", 1.7, 3.2, CABLE_Y - 0.6, CABLE_Y + 0.6, 19.2, 20.8);
        paintBody(context, qCreatedBy(id + "cableClamp", EntityType.BODY), "Rudder cable clamp on the aft hoop", steel);

        // ---- Pedal linkage ----
        mkCyl(context, id + "hingeRod", vector(HINGE[0], -4.5, HINGE[1]), vector(HINGE[0], CRANK_Y + 0.1, HINGE[1]), 0.19);
        paintBody(context, qCreatedBy(id + "hingeRod", EntityType.BODY), "Pedal hinge rod 3/8", steel);
        mkBox(context, id + "crank", HINGE[0] - 0.3, HINGE[0] + 0.3, CRANK_Y - 0.0625, CRANK_Y + 0.0625, HINGE[1], HINGE[1] + CRANK_R + 0.2);
        paintBody(context, qCreatedBy(id + "crank", EntityType.BODY), "Pedal crank 1/8 6061", alu);
        mkCyl(context, id + "throttleCore", vector(HINGE[0], CRANK_Y, HINGE[1] + CRANK_R), vector(108.5, CRANK_Y, HINGE[1] + CRANK_R), 0.06);
        paintBody(context, qCreatedBy(id + "throttleCore", EntityType.BODY), "Throttle cable core", steel);
        mkBox(context, id + "cableStop", 108.5, 109.1, -0.4, 1, 2.3, 4.8);
        paintBody(context, qCreatedBy(id + "cableStop", EntityType.BODY), "Throttle cable stop on the rib", alu);
        mkCyl(context, id + "spring", vector(106.5, -2.5, 2.6), vector(107.4, -2.5, 2.3), 0.25);
        paintBody(context, qCreatedBy(id + "spring", EntityType.BODY), "Pedal return spring envelope", color(0.8, 0.7, 0.2));
        mkBox(context, id + "engineBracket", 26, 28, -7.6, -7.4, 29.5, 32);
        paintBody(context, qCreatedBy(id + "engineBracket", EntityType.BODY), "Throttle cable bracket at the engine", alu);
    });
