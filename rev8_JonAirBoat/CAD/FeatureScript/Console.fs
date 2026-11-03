FeatureScript 3112;
import(path : "onshape/std/geometry.fs", version : "3112.0");

// Driver console (rev8): table, dash and support frame on the driver's starboard side, in the airboat
// assembly frame: X forward from the transom, Y = 0 on the centerline (port +), Z up from the inside of
// the hull floor. Its own Part Studio so it can be changed without touching the rest of the boat.
//   - Table top 1/2 in marine HDPE (King StarBoard or equal) at armrest height, with an outboard fiddle
//     rail. A small tray forward of the dash.
//   - Dash panel 3/8 in HDPE, leaning back toward the driver, braced by two 1/8 in aluminum gussets.
//     Instruments: tachometer/hour meter, water temperature, fuel level, voltmeter, oil-pressure and
//     charge warning lights, key switch (OFF/ON/START), lanyard kill switch, 6-gang rocker switch panel
//     and a 12 V / USB outlet. Each is a named body so the wiring can be routed to it later.
//   - Frame: 1 x 1 x 1/8 in 6061 angle bolted with 1/4-20 (slotted-angle style): four legs, two top
//     rails, two cross arms that carry the table out to the gunwale, knee braces and side diagonals.
//   - Four 1/4 in foot plates, each bolted with two 5/16-18 into rivnuts in the console mount rails
//     (Seating and interior mounts studio, Y -12 and -24). That is the only interface to the hull.

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

// Extrude an L-shaped angle profile (legs a, thickness t) along `axis` from corner point P.
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

// Closed polygon in the YZ plane ([y, z] pairs) at X = x0, extruded `len` in +X.
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

// Flat bar of width w and thickness t from point a to point b; the bar's width lies in the plane spanned
// by (b - a) and `inPlane`, its thickness along their normal.
function flatBar(context is Context, id is Id, a is Vector, b is Vector, inPlane is Vector, w is number, t is number)
{
    const ax = normalize(b - a);
    const n = normalize(cross(ax, inPlane));
    const sk = newSketchOnPlane(context, id + "sk", { "sketchPlane" : plane(a * inch, n, ax) });
    const L = norm(b - a);
    // sketch x = ax, sketch y = n x ax (the bar width direction)
    skPolyline(sk, "poly", { "points" : [vector(0, -w / 2) * inch, vector(L, -w / 2) * inch, vector(L, w / 2) * inch, vector(0, w / 2) * inch, vector(0, -w / 2) * inch] });
    skSolve(sk);
    opExtrude(context, id + "ex", { "entities" : qSketchRegion(id + "sk"), "direction" : n, "endBound" : BoundingType.BLIND, "endDepth" : t * inch });
    opDeleteBodies(context, id + "del", { "entities" : qCreatedBy(id + "sk", EntityType.BODY) });
}

annotation { "Feature Type Name" : "Driver console" }
export const driverConsole = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        annotation { "Name" : "Table aft edge (X)" }
        isLength(definition.x0, { (inch) : [60, 101, 150] } as LengthBoundSpec);

        annotation { "Name" : "Table length (X)" }
        isLength(definition.len, { (inch) : [12, 21, 36] } as LengthBoundSpec);

        annotation { "Name" : "Table inboard edge (distance to starboard of the centerline)" }
        isLength(definition.yIn, { (inch) : [6, 10.5, 20] } as LengthBoundSpec);

        annotation { "Name" : "Table outboard edge (distance to starboard of the centerline)" }
        isLength(definition.yOut, { (inch) : [20, 33.5, 37] } as LengthBoundSpec);

        annotation { "Name" : "Table top height (Z)" }
        isLength(definition.tableZ, { (inch) : [18, 25, 32] } as LengthBoundSpec);

        annotation { "Name" : "Dash set back from the table front" }
        isLength(definition.dashBack, { (inch) : [0, 3, 8] } as LengthBoundSpec);

        annotation { "Name" : "Dash panel height (along the panel)" }
        isLength(definition.dashH, { (inch) : [4, 8.5, 14] } as LengthBoundSpec);

        annotation { "Name" : "Dash lean back toward the driver" }
        isAngle(definition.dashLean, { (degree) : [0, 20, 45] } as AngleBoundSpec);

        annotation { "Name" : "Mount rail Y, inboard (starboard of the centerline)" }
        isLength(definition.railIn, { (inch) : [6, 12, 20] } as LengthBoundSpec);

        annotation { "Name" : "Mount rail Y, outboard (starboard of the centerline)" }
        isLength(definition.railOut, { (inch) : [16, 24, 25] } as LengthBoundSpec);
    }
    {
        const x0 = definition.x0 / inch;
        const x1 = x0 + definition.len / inch;
        const yI = -definition.yIn / inch;
        const yO = -definition.yOut / inch;
        const tz = definition.tableZ / inch;
        const tT = 0.5;                          // table thickness
        const tb = tz - tT;                      // table underside
        const rI = -definition.railIn / inch;
        const rO = -definition.railOut / inch;
        const lean = definition.dashLean;
        const dH = definition.dashH / inch;
        const dT = 0.375;
        const a = 1;                             // angle leg
        const t = 0.125;
        const alu = color(0.75, 0.77, 0.8);
        const plateC = color(0.62, 0.66, 0.72);
        const hdpe = color(0.93, 0.93, 0.9);
        const black = color(0.08, 0.08, 0.09);
        const zFoot = 1;                         // top of the mount rails
        const zLeg0 = zFoot + 0.25;
        const zArm = tb - a;                     // cross arms and top rails: Z zArm..tb

        // Dash geometry: aft face starts on the table `dashBack` aft of the front edge and leans back.
        const u = vector(-sin(lean), 0, cos(lean));          // up the panel
        const n = vector(cos(lean), 0, sin(lean));           // panel normal, forward
        const xd = x1 - definition.dashBack / inch;
        const P0 = vector(xd, 0, tz);                        // aft face, bottom edge (Y set per point)
        const dashY0 = yO + 0.85;          // clear of the outboard fiddle rail
        const dashY1 = yI;
        // Face point at (y, v up the panel), on the aft face.
        const face = function(y, v) { return vector(P0[0], y, P0[2]) + u * v; };

        // Instruments [name, y, v, bezel radius, can radius, can depth, kind].
        const inst = [["Tachometer / hour meter 3-3/8 in", yI - 4.5, 4.6, 1.85, 1.6875, 2.5, "gauge"],
                      ["Water temperature gauge 2-1/16 in", yI - 9.1, 5.6, 1.18, 1.03, 2.2, "gauge"],
                      ["Fuel level gauge 2-1/16 in", yI - 12.5, 5.6, 1.18, 1.03, 2.2, "gauge"],
                      ["Voltmeter 2-1/16 in", yI - 15.9, 5.6, 1.18, 1.03, 2.2, "gauge"],
                      ["Oil pressure warning light", yI - 19.0, 6.3, 0.4, 0.3, 1.5, "light"],
                      ["Charge (alternator) warning light", yI - 19.0, 4.9, 0.4, 0.3, 1.5, "light"],
                      ["Key switch OFF / ON / START", yI - 9.1, 2.3, 0.65, 0.5, 2.0, "switch"],
                      ["Lanyard kill switch (engine stop)", yI - 2.1, 1.7, 0.75, 0.6, 1.6, "switch"],
                      ["12 V and USB outlet", yI - 20.4, 2.3, 0.75, 0.6, 2.2, "outlet"]];

        // Rectangle on the dash in panel coordinates: y across, v up the panel, d through it (0 = aft face).
        const pRect = function(rid, y0, y1, v0, v1, d0, d1)
            {
                const sk = newSketchOnPlane(context, rid + "sk", { "sketchPlane" : plane((P0 + n * d0) * inch, n, vector(0, 1, 0)) });
                skPolyline(sk, "poly", { "points" : [vector(y0, v0) * inch, vector(y1, v0) * inch, vector(y1, v1) * inch, vector(y0, v1) * inch, vector(y0, v0) * inch] });
                skSolve(sk);
                opExtrude(context, rid + "ex", { "entities" : qSketchRegion(rid + "sk"), "direction" : n, "endBound" : BoundingType.BLIND, "endDepth" : (d1 - d0) * inch });
                opDeleteBodies(context, rid + "del", { "entities" : qCreatedBy(rid + "sk", EntityType.BODY) });
            };
        // 6-gang rocker switch panel window [y0, y1, v0, v1].
        const sw = [yI - 17.7, yI - 11.8, 1.25, 3.25];

        // ---------- Tools first: instrument holes through the dash panel ----------
        var holes = [];
        for (var k = 0; k < size(inst); k += 1)
        {
            const hid = id + ("hole" ~ k);
            const c0 = face(inst[k][1], inst[k][2]);
            mkCyl(context, hid, c0 - n * 0.1, c0 + n * (dT + 0.1), inst[k][4]);
            holes = append(holes, qCreatedBy(hid, EntityType.BODY));
        }
        pRect(id + "swCut", sw[0], sw[1], sw[2], sw[3], -0.1, dT + 0.1);
        holes = append(holes, qCreatedBy(id + "swCut", EntityType.BODY));

        // ---------- Dash panel ----------
        pRect(id + "dash", dashY0, dashY1, 0, dH, 0, dT);
        opBoolean(context, id + "dash" + "holes", { "targets" : qCreatedBy(id + "dash", EntityType.BODY), "tools" : qUnion(holes), "operationType" : BooleanOperationType.SUBTRACTION });
        paintBody(context, qCreatedBy(id + "dash", EntityType.BODY), "Dash panel 3/8 marine HDPE (StarBoard), instrument holes", hdpe);

        // ---------- Instruments (bezel on the aft face, body through the hole) ----------
        for (var m = 0; m < size(inst); m += 1)
        {
            const iid = id + ("inst" ~ m);
            const c1 = face(inst[m][1], inst[m][2]);
            mkCyl(context, iid + "bezel", c1 - n * 0.2, c1, inst[m][3]);
            mkCyl(context, iid + "can", c1, c1 + n * (dT + inst[m][5]), inst[m][4] - 0.01);
            opBoolean(context, iid + "join", { "tools" : qCreatedBy(iid, EntityType.BODY), "operationType" : BooleanOperationType.UNION });
            paintBody(context, qCreatedBy(iid, EntityType.BODY), inst[m][0] ~ " (bought)", black);
        }
        {
            const sid = id + "switches";
            pRect(sid + "bezel", sw[0] - 0.3, sw[1] + 0.3, sw[2] - 0.25, sw[3] + 0.25, -0.15, 0);
            pRect(sid + "body", sw[0] + 0.01, sw[1] - 0.01, sw[2] + 0.01, sw[3] - 0.01, 0, dT + 2);
            opBoolean(context, sid + "join", { "tools" : qCreatedBy(sid, EntityType.BODY), "operationType" : BooleanOperationType.UNION });
            paintBody(context, qCreatedBy(sid, EntityType.BODY), "Rocker switch panel 6-gang with fuses: nav lights, anchor light, LED bar, searchlight, bilge pump, spare (bought)", black);
        }

        // ---------- Table, fiddle rails ----------
        mkBox(context, id + "table", x0, x1, yO, yI, tb, tz);
        paintBody(context, qCreatedBy(id + "table", EntityType.BODY), "Console table top 1/2 marine HDPE (StarBoard)", hdpe);
        mkBox(context, id + "fiddleOut", x0, x1 - 0.75, yO, yO + 0.75, tz, tz + 1);
        paintBody(context, qCreatedBy(id + "fiddleOut", EntityType.BODY), "Table fiddle rail 3/4 x 1 HDPE - outboard", hdpe);
        mkBox(context, id + "fiddleFwd", x1 - 0.75, x1, yO, yI, tz, tz + 1);
        paintBody(context, qCreatedBy(id + "fiddleFwd", EntityType.BODY), "Table fiddle rail 3/4 x 1 HDPE - forward (tray lip)", hdpe);

        // ---------- Dash gussets (forward of the panel, on the table) ----------
        const F = function(v) { return P0 + u * v + n * dT; };
        for (var gy in [dashY0 + 0.15, yI - 0.75])
        {
            const gid = id + ("gusset" ~ (gy < -20 ? "Out" : "In"));
            const f0 = F(0);
            const f1 = F(dH - 1.5);
            xzPrism(context, gid, gy, gy + 0.125, [[f0[0], tz], [x1 - 0.85, tz], [f1[0], f1[2]], [f0[0], f0[2]]]);
            paintBody(context, qCreatedBy(gid, EntityType.BODY), "Dash gusset 1/8 6061 plate (angle clips to the table and panel) - " ~ (gy < -20 ? "outboard" : "inboard"), alu);
        }

        // ---------- Frame: 1 x 1 x 1/8 6061 angle ----------
        const xa = x0 + 1;            // aft cross arm and legs (flanges point forward)
        const xf = x1 - 1;            // forward cross arm and legs (flanges point aft)
        const down = vector(0, 0, -1);
        angleStock(context, id + "armAft", vector(xa, yI - 0.5, tb), vector(0, -1, 0), vector(1, 0, 0), down, a, t, (yI - 0.5) - (yO + 0.5));
        paintBody(context, qCreatedBy(id + "armAft", EntityType.BODY), "Console cross arm 1x1x1/8 6061 angle - aft", alu);
        angleStock(context, id + "armFwd", vector(xf, yI - 0.5, tb), vector(0, -1, 0), vector(-1, 0, 0), down, a, t, (yI - 0.5) - (yO + 0.5));
        paintBody(context, qCreatedBy(id + "armFwd", EntityType.BODY), "Console cross arm 1x1x1/8 6061 angle - forward", alu);
        angleStock(context, id + "railIn", vector(xa + a, rI + 0.5, tb), vector(1, 0, 0), vector(0, -1, 0), down, a, t, (xf - a) - (xa + a));
        paintBody(context, qCreatedBy(id + "railIn", EntityType.BODY), "Console top rail 1x1x1/8 6061 angle - inboard", alu);
        angleStock(context, id + "railOut", vector(xa + a, rO - 0.5, tb), vector(1, 0, 0), vector(0, 1, 0), down, a, t, (xf - a) - (xa + a));
        paintBody(context, qCreatedBy(id + "railOut", EntityType.BODY), "Console top rail 1x1x1/8 6061 angle - outboard", alu);
        const legs = [[xa, rI + 0.5, 1, -1, "aft inboard", "aftIn"], [xf, rI + 0.5, -1, -1, "fwd inboard", "fwdIn"], [xa, rO - 0.5, 1, 1, "aft outboard", "aftOut"], [xf, rO - 0.5, -1, 1, "fwd outboard", "fwdOut"]];
        for (var lg in legs)
        {
            const lid = id + ("leg" ~ lg[5]);
            angleStock(context, lid, vector(lg[0], lg[1], zLeg0), vector(0, 0, 1), vector(lg[2], 0, 0), vector(0, lg[3], 0), a, t, zArm - zLeg0);
            paintBody(context, qCreatedBy(lid, EntityType.BODY), "Console leg 1x1x1/8 6061 angle - " ~ lg[4], alu);
            const fid = id + ("foot" ~ lg[5]);
            const cx = lg[0] + lg[2] * a / 2;
            const cy = lg[1] + lg[3] * a / 2;
            mkBox(context, fid, cx - 1.5, cx + 1.5, cy - 1.5, cy + 1.5, zFoot, zLeg0);
            paintBody(context, qCreatedBy(fid, EntityType.BODY), "Console foot plate 3 x 3 x 1/4 6061, 2 x 5/16-18 into rail rivnuts - " ~ lg[4], plateC);
        }
        // Knee braces under the outboard cantilever, beside the cross arm vertical flanges.
        const zUp = vector(0, 0, 1);
        flatBar(context, id + "kneeAft", vector(xa + 2 * t, rO - 1.1, 18), vector(xa + 2 * t, yO + 1.5, tb - 0.75), zUp, 1, t);
        paintBody(context, qCreatedBy(id + "kneeAft", EntityType.BODY), "Console knee brace 1 x 1/8 6061 flat bar - aft", alu);
        flatBar(context, id + "kneeFwd", vector(xf - t, rO - 1.1, 18), vector(xf - t, yO + 1.5, tb - 0.75), zUp, 1, t);
        paintBody(context, qCreatedBy(id + "kneeFwd", EntityType.BODY), "Console knee brace 1 x 1/8 6061 flat bar - forward", alu);
        // Side diagonals against the leg fore-aft flanges (stop the frame racking fore and aft).
        flatBar(context, id + "diagIn", vector(xa + 0.6, rI + 0.5 - t, zLeg0 + 1.5), vector(xf - 0.6, rI + 0.5 - t, zArm - 0.8), zUp, 1, t);
        paintBody(context, qCreatedBy(id + "diagIn", EntityType.BODY), "Console diagonal 1 x 1/8 6061 flat bar - inboard", alu);
        flatBar(context, id + "diagOut", vector(xf - 0.6, rO - 0.5 + t, zLeg0 + 1.5), vector(xa + 0.6, rO - 0.5 + t, zArm - 0.8), zUp, 1, t);
        paintBody(context, qCreatedBy(id + "diagOut", EntityType.BODY), "Console diagonal 1 x 1/8 6061 flat bar - outboard", alu);
    });
