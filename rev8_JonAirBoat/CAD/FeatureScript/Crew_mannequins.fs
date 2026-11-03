FeatureScript 3112;
import(path : "onshape/std/geometry.fs", version : "3112.0");

// Simple seated crew mannequins (about 50th-percentile adult, 5'9") for renders and ergonomics checks.
// Built from cylinders and spheres in the airboat assembly frame: X forward from the transom,
// Y = 0 on the centerline, Z up from the inside of the hull floor. Each figure sits facing +X with its
// hip point on the seat surface. Seating is tandem on the centerline (rev8): the passenger on the
// storage box in front of the engine, the driver on the bucket seat over the battery dry box.

function cyl(context is Context, id is Id, a is Vector, b is Vector, r is number)
{
    fCylinder(context, id, { "bottomCenter" : a * inch, "topCenter" : b * inch, "radius" : r * inch });
}

function sph(context is Context, id is Id, c is Vector, r is number)
{
    opSphere(context, id, { "center" : c * inch, "radius" : r * inch });
}

function paint(context is Context, q is Query, name is string, c is Color)
{
    setProperty(context, { "entities" : q, "propertyType" : PropertyType.NAME, "value" : name });
    setProperty(context, { "entities" : q, "propertyType" : PropertyType.APPEARANCE, "value" : c });
}

// Seated figure with its hip point at H. The shins drop toward the floor at `floorZ` (feet flat when
// they reach it; at most an 18.5 in knee-to-ankle length, so on a high seat the feet hang a little).
function seatedFigure(context is Context, id is Id, H is Vector, floorZ is number, role is string, vest is Color)
{
    const skin = color(0.87, 0.69, 0.55);
    const pants = color(0.2, 0.27, 0.4);
    const shoes = color(0.15, 0.12, 0.1);
    for (var s in [1, -1])
    {
        const sid = id + (s > 0 ? "L" : "R");
        const hip = H + vector(0, s * 4, 3);
        const knee = H + vector(17, s * 4.5, 4);
        const drop = min(knee[2] - floorZ - 2.6, 18.25);
        const ankle = knee + vector(3, 0, -drop);
        cyl(context, sid + "thigh", hip, knee, 3);
        sph(context, sid + "knee", knee, 2.6);
        cyl(context, sid + "shin", knee, ankle, 2.2);
        cyl(context, sid + "foot", ankle + vector(-1.5, 0, -0.8), ankle + vector(8, 0, -0.8), 1.8);
        paint(context, qUnion([qCreatedBy(sid + "thigh", EntityType.BODY), qCreatedBy(sid + "knee", EntityType.BODY), qCreatedBy(sid + "shin", EntityType.BODY)]), role ~ " - leg", pants);
        paint(context, qCreatedBy(sid + "foot", EntityType.BODY), role ~ " - shoe", shoes);

        const shoulder = H + vector(-2, s * 8, 24);
        const elbow = H + vector(5, s * 9.5, 12.5);
        const hand = H + vector(15, s * 7.5, 15);
        cyl(context, sid + "upperArm", shoulder, elbow, 2);
        sph(context, sid + "elbow", elbow, 1.9);
        cyl(context, sid + "forearm", elbow, hand, 1.7);
        sph(context, sid + "hand", hand, 1.9);
        paint(context, qUnion([qCreatedBy(sid + "upperArm", EntityType.BODY), qCreatedBy(sid + "elbow", EntityType.BODY), qCreatedBy(sid + "forearm", EntityType.BODY), qCreatedBy(sid + "hand", EntityType.BODY)]), role ~ " - arm", skin);
    }
    cyl(context, id + "pelvis", H + vector(0, -5.5, 3), H + vector(0, 5.5, 3), 3.4);
    paint(context, qCreatedBy(id + "pelvis", EntityType.BODY), role ~ " - pelvis", pants);
    cyl(context, id + "torso", H + vector(0, 0, 2), H + vector(-2, 0, 25), 6.8);
    paint(context, qCreatedBy(id + "torso", EntityType.BODY), role ~ " - torso (life vest)", vest);
    cyl(context, id + "neck", H + vector(-2, 0, 24), H + vector(-1.5, 0, 28.5), 2.2);
    sph(context, id + "head", H + vector(-1, 0, 32.5), 4.4);
    paint(context, qUnion([qCreatedBy(id + "neck", EntityType.BODY), qCreatedBy(id + "head", EntityType.BODY)]), role ~ " - head", skin);
    // Hearing protection: airboat props are loud.
    cyl(context, id + "muffs", H + vector(-1, -5, 32.5), H + vector(-1, 5, 32.5), 2.1);
    paint(context, qCreatedBy(id + "muffs", EntityType.BODY), role ~ " - ear muffs", color(0.1, 0.1, 0.1));
}

annotation { "Feature Type Name" : "Crew mannequins" }
export const crewMannequins = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        annotation { "Name" : "Driver hip point X (bucket seat)" }
        isLength(definition.driverX, { (inch) : [0, 103, 150] } as LengthBoundSpec);

        annotation { "Name" : "Driver hip point Z (seat cushion top)" }
        isLength(definition.driverZ, { (inch) : [0, 17, 40] } as LengthBoundSpec);

        annotation { "Name" : "Passenger hip point X (storage box)" }
        isLength(definition.passengerX, { (inch) : [0, 68, 150] } as LengthBoundSpec);

        annotation { "Name" : "Passenger hip point Z (box cushion top)" }
        isLength(definition.passengerZ, { (inch) : [0, 18, 40] } as LengthBoundSpec);

        annotation { "Name" : "Floor height (Z) for the feet" }
        isLength(definition.floorZ, { (inch) : [-2, 0, 10] } as LengthBoundSpec);
    }
    {
        const f = definition.floorZ / inch;
        seatedFigure(context, id + "driver", vector(definition.driverX / inch, 0, definition.driverZ / inch), f, "Driver mannequin", color(0.95, 0.45, 0.05));
        seatedFigure(context, id + "passenger", vector(definition.passengerX / inch, 0, definition.passengerZ / inch), f, "Passenger mannequin", color(0.95, 0.85, 0.1));
    });
