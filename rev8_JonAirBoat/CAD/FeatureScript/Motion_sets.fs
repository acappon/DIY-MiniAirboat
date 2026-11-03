FeatureScript 3112;
import(path : "onshape/std/geometry.fs", version : "3112.0");

// Motion sets: splits a Part Studio into rigid moving groups so Assembly 1 can mate them.
// Added as the last feature of the drivetrain, cage, controls and seating Part Studios. Its "Set"
// parameter is driven by the studio's configuration input #set:
//   0 = every part (the studio as designed; Part Studio default)
//   1 = the fixed parts only (inserted fixed into the assembly)
//   2.. = one moving group each (inserted free and mated)
// Each group also carries the mate connectors on its joint axes; the fixed set carries the matching
// connectors on the fixed side, at the same coordinates.
//
// Parts are picked by selectors: a string is a regular expression on the part name; ["c", p] is the
// part whose bounding-box centre is at p (within 0.05 in); ["in", a, b] is any part whose centre lies
// in the box a..b. Part names cannot be read while a Part Studio regenerates, so every set here uses
// centres (taken from the studios as designed); re-check them if a moving part is resized or moved.

export enum MotionStudio
{
    annotation { "Name" : "Drivetrain (prop, belt drive)" }
    DRIVETRAIN,
    annotation { "Name" : "Cage and rudders" }
    CAGE,
    annotation { "Name" : "Controls (stick, throttle, cables)" }
    CONTROLS,
    annotation { "Name" : "Seating and interior mounts" }
    SEATING
}

// groups: selector list per moving set (set 2 is groups[0]). exclude: parts left out of every set but 0.
// connectors: [id, set, owner selector, frame]; frame is ["pt", origin, zAxis, xAxis] or ["rod", rod selector]
// (the rod's own axis, centred on the rod).
function studioSets(studio) returns map
{
    // Compared by name: the parameter value can carry the enum type of an older Feature Studio version.
    const st = toString(studio);
    const X = vector(1, 0, 0);
    const Y = vector(0, 1, 0);
    const Z = vector(0, 0, 1);
    if (st == "DRIVETRAIN")
    {
        const shaft = ["c", vector(13.3, 0, 53.2)];
        const jack = ["c", vector(19.7, 0, 15.7)];
        const idler = ["c", vector(18, -3.583, 21.7)];
        return {
            "groups" : [
                // 2: prop, prop flange, prop shaft, 75T pulley
                [["c", vector(5, 0, 60.48)], ["c", vector(7.25, 0, 53.2)], shaft, ["c", vector(18, 0, 53.2)]],
                // 3: jackshaft, 30T pulley, flex coupling, flywheel (crank side)
                [jack, ["c", vector(18, 0, 15.7)], ["c", vector(23.25, 0, 15.7)], ["c", vector(24.5, 0, 15.7)]],
                // 4: belt idler
                [idler]],
            "exclude" : [],
            "connectors" : [
                ["propAxis", 2, shaft, ["pt", vector(10, 0, 53.2), X, Z]],
                ["propAxis", 1, ["c", vector(21.213, 0, 53.2)], ["pt", vector(10, 0, 53.2), X, Z]],          // fwd bearing flange
                ["driveAxis", 3, jack, ["pt", vector(20, 0, 15.7), X, Z]],
                ["driveAxis", 1, ["c", vector(21.05, 0, 15.7)], ["pt", vector(20, 0, 15.7), X, Z]],          // jackshaft bearing housing
                ["idlerAxis", 4, idler, ["pt", vector(18.25, -3.583, 21.7), X, Z]],
                ["idlerAxis", 1, ["c", vector(18.9635, -3.583, 21.7)], ["pt", vector(18.25, -3.583, 21.7), X, Z]]]   // idler bracket
        };
    }
    if (st == "CAGE")
    {
        const tubeP = ["c", vector(-3.6, 13, 53.45)];
        const tubeS = ["c", vector(-3.6, -13, 53.45)];
        const pinP = ["c", vector(-7.0995, 14.9995, 29.493)];
        const pinS = ["c", vector(-7.0995, -10.9995, 29.8275)];
        const tie = ["c", vector(-7.1, 2, 29.4005)];
        const link = ["c", vector(-7.1, 21.5, 28.7315)];
        const crank = ["c", vector(-5.2875, 29.9375, 28.9265)];
        const pinA = ["c", vector(-7.0995, 27.9995, 28.927)];
        return {
            "groups" : [
                // 2: port rudder (honeycomb, edge channels, hinge tube, pivot plugs, horn) and its horn pin.
                // The box stops short of the hinge brackets, thrust washers and pivot bolts, which stay fixed.
                [["in", vector(-19, 12.4, 29.7), vector(-3.05, 15.4, 77.4)], pinP],
                // 3: starboard rudder and horn pin
                [["in", vector(-19, -13.6, 29.7), vector(-3.05, -10.5, 77.4)]],
                [tie],                                                    // 4 tie bar
                [link],                                                   // 5 drive link
                [crank, pinA, ["c", vector(-3.5995, 31.9995, 28.927)]]], // 6 bellcrank, pins A and B
            // rev7 steering rod and rod end, replaced by the 43C cable (Controls studio)
            "exclude" : [["c", vector(46.779, 23.4535, 23.4025)], ["c", vector(-3.156, 32, 28.6665)]],
            "connectors" : [
                ["rudderP", 2, tubeP, ["pt", vector(-3.6, 13, 30.1), Z, X]],
                ["rudderP", 1, ["c", vector(-3.3, 13, 29.225)], ["pt", vector(-3.6, 13, 30.1), Z, X]],      // lower hinge bracket
                ["rudderS", 3, tubeS, ["pt", vector(-3.6, -13, 30.1), Z, X]],
                ["rudderS", 1, ["c", vector(-3.3, -13, 29.225)], ["pt", vector(-3.6, -13, 30.1), Z, X]],
                ["bellcrank", 6, crank, ["pt", vector(-3.6, 28, 28.9), Z, X]],
                ["bellcrank", 1, ["c", vector(-3.6, 28, 29.1335)], ["pt", vector(-3.6, 28, 28.9), Z, X]],   // pivot bolt
                ["tieP", 4, tie, ["pt", vector(-7.1, 15, 29.4), Z, X]],
                ["tieP", 2, pinP, ["pt", vector(-7.1, 15, 29.4), Z, X]],
                ["tieS", 4, tie, ["pt", vector(-7.1, -11, 29.4), Z, X]],
                ["tieS", 3, pinS, ["pt", vector(-7.1, -11, 29.4), Z, X]],
                ["linkA", 5, link, ["pt", vector(-7.1, 28, 28.73), Z, X]],
                ["linkA", 6, pinA, ["pt", vector(-7.1, 28, 28.73), Z, X]],
                ["linkHorn", 5, link, ["pt", vector(-7.1, 15, 28.73), Z, X]],
                ["linkHorn", 2, pinP, ["pt", vector(-7.1, 15, 28.73), Z, X]]]
        };
    }
    if (st == "CONTROLS")
    {
        const stick = ["c", vector(110.9805, 13.5, 19.425)];
        const pedal = ["c", vector(125.1024, -6.375, 5.2671)];
        const sRod = ["c", vector(106.44, 13.5, 9.9)];
        const bRod = ["c", vector(0.3214, 31.3198, 28.5299)];
        const tRod = ["c", vector(117.7788, -10.4, 1.433)];
        const eRod = ["c", vector(44.3863, -11.7281, 26.3891)];
        return {
            "groups" : [[stick], [pedal], [sRod], [bRod], [tRod], [eRod]],   // 2 stick, 3 pedal, 4-7 cable core rods
            "exclude" : [],
            "connectors" : [
                ["stick", 2, stick, ["pt", vector(110, 13.5, 5.5), Y, X]],
                ["stick", 1, ["c", vector(110, 13.5, 5.5)], ["pt", vector(110, 13.5, 5.5), Y, X]],            // stick pivot bolt
                ["pedal", 3, pedal, ["pt", vector(121, -6, 4.85), Y, X]],
                ["pedal", 1, ["c", vector(121, -6, 4.55)], ["pt", vector(121, -6, 4.85), Y, X]],              // inboard pillow block
                ["steerRodStick", 4, sRod, ["rod", sRod]],
                ["steerRodStick", 1, ["c", vector(102.75, 13.5, 9.9)], ["rod", sRod]],                         // hub, stick end
                ["steerRodBell", 5, bRod, ["rod", bRod]],
                ["steerRodBell", 1, ["c", vector(4.1937, 30.648, 28.1347)], ["rod", bRod]],                    // hub, bellcrank end
                ["throttleRodPedal", 6, tRod, ["rod", tRod]],
                ["throttleRodPedal", 1, ["c", vector(113.5, -10.4, 1.433)], ["rod", tRod]],                        // hub, pedal end
                ["throttleRodEngine", 7, eRod, ["rod", eRod]],
                ["throttleRodEngine", 1, ["c", vector(46.0773, -11.5456, 23.9178)], ["rod", eRod]]]            // hub, engine end
        };
    }
    const seat = ["c", vector(103.2, 0, 24.8125)];
    const lid = ["c", vector(71, 0, 16.1875)];
    return {
        "groups" : [
            [seat],                                            // 2 seat on its slide tracks
            [lid, ["c", vector(71, 0, 17.1875)]]],             // 3 storage box lid and cushion
        "exclude" : [],
        "connectors" : [
            ["seatSlide", 2, seat, ["pt", vector(103, 6.5, 13.625), X, Z]],
            ["seatSlide", 1, ["c", vector(104, 6.5, 13.375)], ["pt", vector(103, 6.5, 13.625), X, Z]],      // port track
            ["lidHinge", 3, lid, ["pt", vector(63, 0, 16), Y, X]],
            ["lidHinge", 1, ["c", vector(63.75, 0, 15.25)], ["pt", vector(63, 0, 16), Y, X]]]                // aft top rail
    };
}

function perp(z is Vector) returns Vector
{
    return abs(z[2]) < 0.9 ? normalize(cross(vector(0, 0, 1), z)) : normalize(cross(vector(0, 1, 0), z));
}

function selects(sel, name is string, c is Vector) returns boolean
{
    if (sel is string)
        return name != "" && match(name, sel).hasMatch;
    if (sel[0] == "c")
        return norm(c - sel[1]) < 0.05;
    for (var a = 0; a < 3; a += 1)
        if (c[a] < sel[1][a] || c[a] > sel[2][a])
            return false;
    return true;
}

function anySelects(sels is array, name is string, c is Vector) returns boolean
{
    for (var sel in sels)
        if (selects(sel, name, c))
            return true;
    return false;
}

function firstSelected(bodies is array, names is array, centres is array, sel)
{
    for (var i = 0; i < size(bodies); i += 1)
        if (selects(sel, names[i], centres[i]))
            return bodies[i];
    return undefined;
}

function applySet(context is Context, id is Id, studio, k)
{
    const S = studioSets(studio);
    if (k > size(S.groups) + 1)
        throw regenError("This studio has sets 0 to " ~ (size(S.groups) + 1));
    const bodies = evaluateQuery(context, qAllModifiableSolidBodies());
    var names = [];
    var centres = [];
    for (var bd in bodies)
    {
        var nm = "";
        try silent
        {
            nm = getProperty(context, { "entity" : bd, "propertyType" : PropertyType.NAME });
        }
        names = append(names, nm);
        const bx = evBox3d(context, { "topology" : bd, "tight" : true });
        centres = append(centres, (bx.minCorner + bx.maxCorner) / 2 / inch);
    }

    // Which bodies stay in this set.
    var keepIdx = [];
    var drop = [];
    for (var bi = 0; bi < size(bodies); bi += 1)
    {
        var g = 1;
        for (var j = 0; j < size(S.groups); j += 1)
            if (anySelects(S.groups[j], names[bi], centres[bi]))
                g = j + 2;
        if (g == k && !anySelects(S.exclude, names[bi], centres[bi]))
            keepIdx = append(keepIdx, bi);
        else
            drop = append(drop, bodies[bi]);
    }
    var kept = [];
    var keptNames = [];
    var keptCentres = [];
    for (var ki in keepIdx)
    {
        kept = append(kept, bodies[ki]);
        keptNames = append(keptNames, names[ki]);
        keptCentres = append(keptCentres, centres[ki]);
    }
    if (size(kept) == 0)
        throw regenError("Motion sets: set " ~ k ~ " selects no parts");

    // Mate connectors for this set (frames are worked out while every body still exists).
    for (var c in S.connectors)
    {
        if (c[1] != k)
            continue;
        const owner = firstSelected(kept, keptNames, keptCentres, c[2]);
        if (owner == undefined)
            throw regenError("Motion sets: no owner part for connector " ~ c[0] ~ " in set " ~ k);
        var origin;
        var zA;
        var xA;
        if (c[3][0] == "pt")
        {
            origin = c[3][1];
            zA = c[3][2];
            xA = c[3][3];
        }
        else
        {
            const rod = firstSelected(bodies, names, centres, c[3][1]);
            if (rod == undefined)
                throw regenError("Motion sets: no rod for connector " ~ c[0]);
            var best = undefined;
            var bestA = 0;
            for (var f in evaluateQuery(context, qGeometry(qOwnedByBody(rod, EntityType.FACE), GeometryType.CYLINDER)))
            {
                const a = evArea(context, { "entities" : f }) / inch ^ 2;
                if (a > bestA)
                {
                    bestA = a;
                    best = evSurfaceDefinition(context, { "face" : f });
                }
            }
            const rb = evBox3d(context, { "topology" : rod, "tight" : true });
            const mid = (rb.minCorner + rb.maxCorner) / 2 / inch;
            const o = best.coordSystem.origin / inch;
            zA = best.coordSystem.zAxis;
            origin = o + zA * dot(mid - o, zA);
            xA = perp(zA);
        }
        opMateConnector(context, id + c[0], { "coordSystem" : coordSystem(origin * inch, xA, zA), "owner" : owner });
        try silent
        {
            setProperty(context, { "entities" : qCreatedBy(id + c[0], EntityType.BODY), "propertyType" : PropertyType.NAME, "value" : c[0] });
        }
    }
    if (size(drop) > 0)
        opDeleteBodies(context, id + "drop", { "entities" : qUnion(drop) });
}

annotation { "Feature Type Name" : "Motion sets" }
export const motionSets = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        annotation { "Name" : "Studio" }
        definition.studio is MotionStudio;

        annotation { "Name" : "Set (0 all, 1 fixed, 2+ moving group)" }
        isInteger(definition.set, { (unitless) : [0, 0, 20] } as IntegerBoundSpec);
    }
    {
        if (definition.set == 0)
            return;
        try
        {
            applySet(context, id, definition.studio, definition.set);
        }
        catch (e)
        {
            // Name the first part after the failure so a parts listing shows it (the regen error text is
            // not readable through the API).
            setProperty(context, { "entities" : qNthElement(qAllModifiableSolidBodies(), 0), "propertyType" : PropertyType.NAME, "value" : "MOTION SETS ERROR: " ~ toString(e) });
        }
    });
