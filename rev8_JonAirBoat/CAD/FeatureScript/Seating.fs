FeatureScript 3112;
import(path : "onshape/std/geometry.fs", version : "3112.0");

// Tandem seating and the interior hull mounts for a flat-bottom jonboat (rev8, 1652 placeholder hull),
// in the airboat assembly frame: X forward from the transom, Y = 0 on the centerline (port +),
// Z up from the inside of the hull floor.
//   - Interior mount rails: 1 x 2 x 1/8 in 6061 rectangular tube lying flat on the floor, bolted down
//     through every rib they cross. Everything inside the cockpit (dry box, storage box, grab bar,
//     console) bolts to these rails with 5/16-18 rivnuts, so nothing is fastened through the bottom skin.
//   - Passenger storage box: a full-width bay in front of the engine. Its 1-1/2 x 1-1/2 x 1/8 angle frame
//     is the thwart that replaces the removed middle bench, tied to the gunwale at each corner. Fore and
//     aft panels 0.050 5052; hinged 3/8 in marine plywood lid with a vinyl-covered foam cushion; flotation
//     foam under a 1/4 in plywood false floor.
//   - Backrest / grab bar: inverted U of 1 in x 0.083 6061 tube with a pad, stayed down to the rails.
//   - Driver: Group 31 LiFePO4 battery and power hub in a gasketed 0.063 5052 dry box (removable aft
//     panel), slide track and bucket seat on its 1/8 in top plate.
// The console (driver's starboard side) is its own Part Studio and bolts to the console rails here.

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

// Extrude an L-shaped angle profile (legs a, thickness t) along `axis` from corner point P.
// e1 and e2 are the leg directions (unit, perpendicular to axis and each other).
function angleStock(context is Context, id is Id, P is Vector, axis is Vector, e1 is Vector, e2 is Vector, a is number, t is number, len is number)
{
    const pl = plane(P * inch, axis, e1);
    const ySk = cross(axis, e1);
    const k = dot(ySk, e2) > 0 ? 1 : -1;
    const sk = newSketchOnPlane(context, id + "sk", { "sketchPlane" : pl });
    const pts = [[0, 0], [a, 0], [a, t], [t, t], [t, a], [0, a], [0, 0]];
    var p = [];
    for (var q in pts)
        p = append(p, vector(q[0], k * q[1]) * inch);
    skPolyline(sk, "poly", { "points" : p });
    skSolve(sk);
    opExtrude(context, id + "ex", { "entities" : qSketchRegion(id + "sk"), "direction" : axis, "endBound" : BoundingType.BLIND, "endDepth" : len * inch });
    opDeleteBodies(context, id + "del", { "entities" : qCreatedBy(id + "sk", EntityType.BODY) });
}

// Closed polygon in the YZ plane at X = x0, extruded `len` in +X. pts are [y, z] pairs.
function yzPrism(context is Context, id is Id, x0 is number, len is number, pts is array)
{
    const sk = newSketchOnPlane(context, id + "sk", { "sketchPlane" : plane(vector(x0, 0, 0) * inch, vector(1, 0, 0), vector(0, 1, 0)) });
    var p = [];
    for (var q in pts)
        p = append(p, vector(q[0], q[1]) * inch);
    p = append(p, p[0]);
    skPolyline(sk, "poly", { "points" : p });
    skSolve(sk);
    opExtrude(context, id + "ex", { "entities" : qSketchRegion(id + "sk"), "direction" : vector(1, 0, 0), "endBound" : BoundingType.BLIND, "endDepth" : len * inch });
    opDeleteBodies(context, id + "del", { "entities" : qCreatedBy(id + "sk", EntityType.BODY) });
}

// Closed polygon in the XZ plane, extruded from Y = y0 to Y = y1. pts are [x, z] pairs.
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

// 1 x 2 x 1/8 rectangular tube lying flat (2 in wide in Y, 1 in tall), X from x0 to x1, centered on yc.
function mountRail(context is Context, id is Id, x0 is number, x1 is number, yc is number, name is string)
{
    mkBox(context, id + "in", x0 - 1, x1 + 1, yc - 0.875, yc + 0.875, 0.125, 0.875);
    mkBox(context, id + "out", x0, x1, yc - 1, yc + 1, 0, 1);
    doCut(context, id + "cut", qCreatedBy(id + "out", EntityType.BODY), qCreatedBy(id + "in", EntityType.BODY));
    paintBody(context, qCreatedBy(id + "out", EntityType.BODY), name, color(0.72, 0.74, 0.78));
}

annotation { "Feature Type Name" : "Seating and interior mounts" }
export const seatingInterior = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        annotation { "Name" : "Hull inside half-width at the floor" }
        isLength(definition.hullHalf, { (inch) : [18, 25.9, 36] } as LengthBoundSpec);

        annotation { "Name" : "Hull side flare (half-width gain per unit height)" }
        isReal(definition.sideFlare, { (unitless) : [0, 0.5476, 1.5] } as RealBoundSpec);

        annotation { "Name" : "Gunwale height (inside, Z)" }
        isLength(definition.gunwaleZ, { (inch) : [12, 20.9, 30] } as LengthBoundSpec);

        annotation { "Name" : "Storage box aft face (X)" }
        isLength(definition.boxX0, { (inch) : [40, 63, 120] } as LengthBoundSpec);

        annotation { "Name" : "Storage box length (X)" }
        isLength(definition.boxLen, { (inch) : [10, 16, 30] } as LengthBoundSpec);

        annotation { "Name" : "Storage box frame top (Z)" }
        isLength(definition.boxTop, { (inch) : [8, 16, 22] } as LengthBoundSpec);

        annotation { "Name" : "Driver hip point X" }
        isLength(definition.driverX, { (inch) : [60, 103, 150] } as LengthBoundSpec);

        annotation { "Name" : "Battery center offset to port (Y)" }
        isLength(definition.battY, { (inch) : [0, 6.85, 10] } as LengthBoundSpec);
    }
    {
        const hb0 = definition.hullHalf / inch;
        const fl = definition.sideFlare;
        const hw = function(z) { return hb0 + fl * z; };   // inside half-width of the hull at height z
        const gz = definition.gunwaleZ / inch;
        const bx0 = definition.boxX0 / inch;
        const bx1 = bx0 + definition.boxLen / inch;
        const bz = definition.boxTop / inch;
        const dx = definition.driverX / inch;
        const alu = color(0.75, 0.77, 0.8);
        const dark = color(0.18, 0.19, 0.21);
        const vinyl = color(0.9, 0.9, 0.86);
        const ply = color(0.78, 0.66, 0.45);

        // ---------------- Interior mount rails (hull mounts) ----------------
        const barX = bx0 - 5;                // grab-bar uprights
        for (var sr in [1, -1])
        {
            const side = sr > 0 ? "Port" : "Stbd";
            mountRail(context, id + ("boxRail" ~ side), barX - 9.5, bx1 + 2, sr * 20, "Interior mount rail 1x2x1/8 6061 - storage box / grab bar, " ~ side);
        }
        // Dry box rails under the box side walls; console rails under the console legs (Y -12, -24).
        const dbx0 = dx - 6;
        const dbx1 = dx + 9;
        const dby0 = -8;
        const dby1 = 10.5;
        mountRail(context, id + "dryRailP", dbx0 - 3, dbx1 + 6, dby1 - 1, "Interior mount rail 1x2x1/8 6061 - dry box, Port");
        mountRail(context, id + "dryRailS", dbx0 - 3, dbx1 + 6, dby0 + 1, "Interior mount rail 1x2x1/8 6061 - dry box, Stbd");
        mountRail(context, id + "conRailIn", dx - 5, dx + 23, -12, "Interior mount rail 1x2x1/8 6061 - console, inboard");
        mountRail(context, id + "conRailOut", dx - 5, dx + 23, -24, "Interior mount rail 1x2x1/8 6061 - console, outboard");

        // ---------------- Passenger storage box (thwart) ----------------
        const A = 1.5;
        const T = 0.125;
        const c = 0.25;                       // clearance to the hull side
        const z0 = 1;                         // on the rails
        // Bottom and top cross rails (angle, along Y), aft and forward.
        for (var er in [[bx0, 1, "aft"], [bx1, -1, "fwd"]])
        {
            const ex = er[0];
            const inw = er[1];                 // +1: legs point forward into the box
            const yb = hw(z0) - c;
            const yt = hw(bz - A) - c - A - 0.25;  // ends clear of the corner posts
            angleStock(context, id + ("botRail" ~ er[2]), vector(ex, -yb, z0), vector(0, 1, 0), vector(inw, 0, 0), vector(0, 0, 1), A, T, 2 * yb);
            paintBody(context, qCreatedBy(id + ("botRail" ~ er[2]), EntityType.BODY), "Storage box frame bottom rail 1-1/2 x 1-1/2 x 1/8 6061 angle - " ~ er[2], alu);
            angleStock(context, id + ("topRail" ~ er[2]), vector(ex, -yt, bz), vector(0, 1, 0), vector(inw, 0, 0), vector(0, 0, -1), A, T, 2 * yt);
            paintBody(context, qCreatedBy(id + ("topRail" ~ er[2]), EntityType.BODY), "Storage box thwart top rail 1-1/2 x 1-1/2 x 1/8 6061 angle - " ~ er[2], alu);
        }
        // Corner posts follow the hull flare from the bottom rail up to the gunwale, where they bolt
        // through the hull side: they tie the thwart to both sides like the removed bench did.
        for (var sp in [1, -1])
        {
            for (var ep in [[bx0, 1, "aft"], [bx1, -1, "fwd"]])
            {
                const side = sp > 0 ? "Port" : "Stbd";
                const pid = id + ("post" ~ side ~ ep[2]);
                const zA = z0 + A;
                const zB = gz - 0.1;
                const yA = hw(zA) - c;
                const yB = hw(zB) - c;
                const ax = normalize(vector(0, sp * (yB - yA), zB - zA));
                const outN = normalize(vector(0, sp * (zB - zA), -(yB - yA)));   // toward the hull side
                angleStock(context, pid, vector(ep[0], sp * yA, zA), ax, vector(ep[1], 0, 0), -outN, A, T, norm(vector(0, yB - yA, zB - zA)));
                paintBody(context, qCreatedBy(pid, EntityType.BODY), "Storage box corner post / thwart knee 1-1/2 x 1-1/2 x 1/8 6061 angle, bolted through the hull side at the gunwale - " ~ side ~ " " ~ ep[2], alu);
            }
        }
        // Fore and aft panels, riveted to the frame (inside the angles), trapezoid following the hull.
        // Each has two 1 in grommet holes for the push-pull cables (Controls studio): steering to port,
        // throttle to starboard.
        for (var en in [[bx0 + T, "aft", [[23, 13], [-10, 9]]], [bx1 - T - 0.05, "fwd", [[20, 12], [-10, 9]]]])
        {
            const yb = hw(z0 + T) - c - T - 0.05;
            const yt = hw(bz - T) - c - T - 0.05;
            var grommets = [];
            for (var gi = 0; gi < size(en[2]); gi += 1)
            {
                const gid = id + ("grommet" ~ en[1] ~ gi);
                mkCyl(context, gid, vector(en[0] - 0.5, en[2][gi][0], en[2][gi][1]), vector(en[0] + 0.55, en[2][gi][0], en[2][gi][1]), 0.5);
                grommets = append(grommets, qCreatedBy(gid, EntityType.BODY));
            }
            yzPrism(context, id + ("panel" ~ en[1]), en[0], 0.05, [[-yb, z0 + T], [yb, z0 + T], [yt, bz - T], [-yt, bz - T]]);
            doCut(context, id + ("panel" ~ en[1]) + "holes", qCreatedBy(id + ("panel" ~ en[1]), EntityType.BODY), qUnion(grommets));
            paintBody(context, qCreatedBy(id + ("panel" ~ en[1]), EntityType.BODY), "Storage box panel 0.050 5052 - " ~ en[1], color(0.82, 0.83, 0.85));
        }
        // Flotation foam on the floor, under a 1/4 in plywood false floor.
        {
            const fx0 = bx0 + A + 0.1;
            const fx1 = bx1 - A - 0.1;
            const fz0 = z0 + 0.01;          // on the mount rails, so bilge water runs under it
            const fz1 = 6;
            yzPrism(context, id + "foam", fx0, fx1 - fx0, [[-(hw(fz0) - c - 0.3), fz0], [hw(fz0) - c - 0.3, fz0], [hw(fz1) - c - 0.3, fz1], [-(hw(fz1) - c - 0.3), fz1]]);
            paintBody(context, qCreatedBy(id + "foam", EntityType.BODY), "Flotation foam, closed-cell polyethylene, fuel-resistant (under the false floor)", color(0.95, 0.95, 0.9));
            yzPrism(context, id + "falseFloor", fx0, fx1 - fx0, [[-(hw(fz1) - c - 0.3), fz1], [hw(fz1) - c - 0.3, fz1], [hw(fz1 + 0.25) - c - 0.3, fz1 + 0.25], [-(hw(fz1 + 0.25) - c - 0.3), fz1 + 0.25]]);
            paintBody(context, qCreatedBy(id + "falseFloor", EntityType.BODY), "Storage box false floor 1/4 marine plywood (removable)", ply);
        }
        // Lid (hinged on the aft top rail) and cushion.
        {
            const yl = hw(bz) - c - A - 0.25;  // clear of the corner posts
            yzPrism(context, id + "lid", bx0, bx1 - bx0, [[-yl, bz], [yl, bz], [yl, bz + 0.375], [-yl, bz + 0.375]]);
            paintBody(context, qCreatedBy(id + "lid", EntityType.BODY), "Storage box lid 3/8 marine plywood (piano hinge on the aft rail, 2 latches)", ply);
            yzPrism(context, id + "cushion", bx0 + 0.5, bx1 - bx0 - 1, [[-(yl - 1), bz + 0.375], [yl - 1, bz + 0.375], [yl - 1, bz + 2], [-(yl - 1), bz + 2]]);
            paintBody(context, qCreatedBy(id + "cushion", EntityType.BODY), "Passenger cushion, closed-cell foam 1-5/8 in, marine vinyl cover", vinyl);
        }

        // ---------------- Backrest / grab bar ----------------
        const barTop = 34;
        const bw = 20;
        const r = 0.5;
        const barId = id + "grabBar";
        mkCyl(context, barId + "up1", vector(barX, bw, 1.25), vector(barX, bw, barTop), r);
        mkCyl(context, barId + "up2", vector(barX, -bw, 1.25), vector(barX, -bw, barTop), r);
        mkCyl(context, barId + "cross", vector(barX, -bw, barTop), vector(barX, bw, barTop), r);
        opSphere(context, barId + "b1", { "center" : vector(barX, bw, barTop) * inch, "radius" : r * inch });
        opSphere(context, barId + "b2", { "center" : vector(barX, -bw, barTop) * inch, "radius" : r * inch });
        doUnite(context, barId + "join", qCreatedBy(barId, EntityType.BODY));
        paintBody(context, qCreatedBy(barId, EntityType.BODY), "Backrest / grab bar, 1 in OD x 0.083 6061 tube, bent inverted U", alu);
        for (var sb in [1, -1])
        {
            const side = sb > 0 ? "Port" : "Stbd";
            const st = id + ("stay" ~ side);
            mkCyl(context, st, vector(barX - 0.9, sb * bw, 22), vector(barX - 8.5, sb * bw, 1.65), 0.375);
            paintBody(context, qCreatedBy(st, EntityType.BODY), "Grab bar stay 3/4 in OD x 0.065 6061 tube, flattened ends - " ~ side, alu);
            const ft = id + ("barFoot" ~ side);
            mkBox(context, ft, barX - 10, barX + 1.5, sb * bw - 1, sb * bw + 1, 1, 1.25);
            paintBody(context, qCreatedBy(ft, EntityType.BODY), "Grab bar foot plate 1/4 6061 (upright and stay bolt to it, rivnuts in the rail) - " ~ side, color(0.62, 0.66, 0.72));
        }
        mkBox(context, id + "backPad", barX + r, barX + r + 1.25, -8, 8, 22, 33);
        paintBody(context, qCreatedBy(id + "backPad", EntityType.BODY), "Passenger backrest pad, 3/8 plywood back, foam, marine vinyl", vinyl);

        // ---------------- Driver: dry box, battery, power hub, track, seat ----------------
        const dz0 = 1;
        const dz1 = 13;
        const wT = 0.063;
        mkBox(context, id + "dryIn", dbx0 + wT, dbx1 - wT, dby0 + wT, dby1 - wT, dz0 + wT, dz1 + 1);
        mkBox(context, id + "dryAftCut", dbx0 - 1, dbx0 + wT + 0.01, dby0 + 1, dby1 - 1, dz0 + 1, dz1 - 1);
        mkBox(context, id + "dryOut", dbx0, dbx1, dby0, dby1, dz0, dz1);
        doCut(context, id + "dryOut" + "cut", qCreatedBy(id + "dryOut", EntityType.BODY), qUnion([qCreatedBy(id + "dryIn", EntityType.BODY), qCreatedBy(id + "dryAftCut", EntityType.BODY)]));
        paintBody(context, qCreatedBy(id + "dryOut", EntityType.BODY), "Battery dry box 0.063 5052 (riveted corners, sealed), aft opening for the access panel", alu);
        mkBox(context, id + "dryPanel", dbx0 - 0.125, dbx0 - 0.005, dby0 + 0.5, dby1 - 0.5, dz0 + 0.5, dz1 - 0.5);
        paintBody(context, qCreatedBy(id + "dryPanel", EntityType.BODY), "Dry box access panel 1/8 6061, gasketed, 8 x #10-24 into rivnuts", color(0.62, 0.66, 0.72));
        mkBox(context, id + "dryTop", dbx0, dbx1, dby0, dby1, dz1, dz1 + 0.125);
        paintBody(context, qCreatedBy(id + "dryTop", EntityType.BODY), "Dry box top / seat base 1/8 6061, gasketed", color(0.62, 0.66, 0.72));
        const by0 = definition.battY / inch - 3.4;
        mkBox(context, id + "battery", dbx0 + 1, dbx0 + 14, by0, by0 + 6.8, dz0 + wT + 0.01, dz0 + wT + 9.01);
        paintBody(context, qCreatedBy(id + "battery", EntityType.BODY), "Battery Group 31 LiFePO4 12 V dual-purpose, BMS (bought, about 27 lb)", dark);
        mkBox(context, id + "powerHub", dbx0 + 2, dbx0 + 11, dby0 + wT + 0.01, dby0 + wT + 2.5, dz0 + 4, dz0 + 9);
        paintBody(context, qCreatedBy(id + "powerHub", EntityType.BODY), "Power hub: battery switch, main fuse, fuse block, bus bars (bought, placeholder)", color(0.8, 0.15, 0.12));
        for (var sq in [1, -1])
        {
            const tid = id + ("track" ~ (sq > 0 ? "P" : "S"));
            mkBox(context, tid, dx - 6, dx + 8, sq * 6.5 - 0.6, sq * 6.5 + 0.6, dz1 + 0.125, dz1 + 0.625);
            paintBody(context, qCreatedBy(tid, EntityType.BODY), "Seat slide track, stainless, 6 in travel (bought) - " ~ (sq > 0 ? "Port" : "Stbd"), dark);
        }
        const seatId = id + "seat";
        const pz = dz1 + 0.625;
        mkBox(context, seatId + "pan", dx - 10, dx + 12, -9.5, 9.5, pz, 16.9);
        xzPrism(context, seatId + "back", -9.5, 9.5, [[dx - 6.9, 16.9], [dx - 8.6, 36], [dx - 11.6, 36], [dx - 10, 16.9]]);
        doUnite(context, seatId + "join", qCreatedBy(seatId, EntityType.BODY));
        paintBody(context, qCreatedBy(seatId, EntityType.BODY), "Driver bucket seat, marine low-back, vinyl (bought, about 12 lb)", vinyl);
    });
