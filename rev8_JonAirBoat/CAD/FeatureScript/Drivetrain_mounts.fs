FeatureScript 3112;
import(path : "onshape/std/geometry.fs", version : "3112.0");

// Drivetrain mounts for a flat-bottom jonboat hull, in the airboat assembly frame:
// X forward from the transom, Y = 0 on the centerline, Z up from the hull bottom.
//   - Two engine beds: 2 x 6 x 1/8 in 6061 rectangular tube on edge, with welded 1-1/2 in angle
//     flanges, notched over the ribs, sitting on 1/8 in 5052 bottom doublers
//   - Six 1/4 in mount plates (A-frame aft/forward feet, brace feet) and four engine-mount plates,
//     bolted into 3/8-16 rivnuts set in the bed tops
//   - Two cage supports: 2 x 2 x 1/4 in angle post along the hull side, bottom foot near the chine,
//     knee brace to the transom, 1/4 in top plate with nylocks underneath for the front cage foot pads,
//     and a vertical aft post on the knee carrying a second top plate for the aft foot pads

// The hull side is described by two parameters: the inside half-width at the floor and the side flare.
// Rib positions are the 1448's; check them on the real hull.
const RIBS = [37.85, 54.75];
// Rev7 A-frame and brace foot footprints [x0, x1]; all span 13-16 in off the centerline.
const FEET = [[10, 17, "A-frame aft"], [19, 26, "A-frame forward"], [45, 53, "brace"]];
// Rev7 rubber engine mounts [x0, x1]: their bases reach from Y 7.6 to 16.3, inboard of the beds.
const ENGINE = [[27.25, 34.75, "aft"], [38.25, 44, "forward"]];

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

function doUnite(context is Context, id is Id, q is Query)
{
    opBoolean(context, id, { "tools" : q, "operationType" : BooleanOperationType.UNION });
}

function doCut(context is Context, id is Id, targets is Query, tools is Query)
{
    opBoolean(context, id, { "targets" : targets, "tools" : tools, "operationType" : BooleanOperationType.SUBTRACTION });
}

// Extrude an L-shaped angle profile (legs a, thickness t) along `axis` from point P.
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
    opExtrude(context, id + "ex", {
                "entities" : qSketchRegion(id + "sk"),
                "direction" : axis,
                "endBound" : BoundingType.BLIND,
                "endDepth" : len * inch
            });
    opDeleteBodies(context, id + "del", { "entities" : qCreatedBy(id + "sk", EntityType.BODY) });
}

annotation { "Feature Type Name" : "Drivetrain mounts" }
export const drivetrainMounts = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        annotation { "Name" : "Bed centerline offset (each side)" }
        isLength(definition.bedY, { (inch) : [6, 14.5, 22] } as LengthBoundSpec);

        annotation { "Name" : "Bed aft end (X)" }
        isLength(definition.bedX0, { (inch) : [0, 2, 20] } as LengthBoundSpec);

        annotation { "Name" : "Bed forward end (X)" }
        isLength(definition.bedX1, { (inch) : [40, 62, 100] } as LengthBoundSpec);

        annotation { "Name" : "Cage top plate height (Z)" }
        isLength(definition.cageZ, { (inch) : [10, 16.9, 24] } as LengthBoundSpec);

        annotation { "Name" : "Hull inside half-width at the floor" }
        isLength(definition.hullHalf, { (inch) : [18, 24.2, 36] } as LengthBoundSpec);

        annotation { "Name" : "Hull side flare (half-width gain per unit height)" }
        isReal(definition.sideFlare, { (unitless) : [0, 0.515, 1.5] } as RealBoundSpec);

        annotation { "Name" : "Cage feet moved inboard from rev7 (each side)" }
        isLength(definition.cageDelta, { (inch) : [0, 2.75, 8] } as LengthBoundSpec);

        annotation { "Name" : "Transom inside face (X)" }
        isLength(definition.transomInside, { (inch) : [-5, 0.1, 5] } as LengthBoundSpec);
    }
    {
        const dC = definition.cageDelta / inch;
        const by = definition.bedY / inch;
        const x0 = definition.bedX0 / inch;
        const x1 = definition.bedX1 / inch;
        const dT = 0.125;   // doubler
        const H = 6;        // tube height
        const W = 2;        // tube width
        const wall = 0.125;
        const pT = 0.25;    // plate
        const topZ = dT + H;
        const alu = color(0.75, 0.77, 0.8);
        const plateC = color(0.62, 0.66, 0.72);
        const dblC = color(0.85, 0.86, 0.88);
        const nutC = color(0.45, 0.47, 0.5);

        for (var s in [1, -1])
        {
            const side = s > 0 ? "Port" : "Stbd";
            const bid = id + ("bed" ~ side);
            const yc = s * by;

            // Cutting tools first: an id prefix has to be used in one contiguous run of operations,
            // so every part's own operations below must not be interrupted by tool creation.
            mkBox(context, bid + "inner", x0 + wall, x1 - wall, yc - W / 2 + wall, yc + W / 2 - wall, dT + wall, dT + H - wall);
            var notches = [];
            for (var r = 0; r < size(RIBS); r += 1)
            {
                mkBox(context, bid + ("notch" ~ r), RIBS[r] - 1.5, RIBS[r] + 1.5, yc - 3, yc + 3, -0.1, dT + 1.5);
                notches = append(notches, qCreatedBy(bid + ("notch" ~ r), EntityType.BODY));
            }
            // Flange bolt holes every 6 in per side (3 in staggered), clear of the ribs.
            var holes = [];
            var j = 0;
            for (var hx = x0 + 3; hx <= x1 - 2; hx += 3)
            {
                var nearRib = false;
                for (var rx in RIBS)
                    if (abs(hx - rx) < 2.5)
                        nearRib = true;
                if (!nearRib)
                {
                    const side2 = (j % 2 == 0) ? 1 : -1;
                    mkCyl(context, bid + ("hole" ~ j), vector(hx, yc + side2 * (W / 2 + 0.9), -0.5), vector(hx, yc + side2 * (W / 2 + 0.9), dT + 1), 0.14);
                    holes = append(holes, qCreatedBy(bid + ("hole" ~ j), EntityType.BODY));
                }
                j += 1;
            }
            // Plate bolts into the bed top: the bed is a closed tube, so 3/8-16 rivnuts (plus-nuts) are set
            // in its top wall under every plate bolt.
            var rv = [];
            var rvX = [];
            for (var fe in FEET)
                rvX = concatenateArrays([rvX, [fe[0] + 1, fe[1] - 1]]);
            for (var en in ENGINE)
                rvX = concatenateArrays([rvX, [(en[0] + en[1]) / 2 - 2, (en[0] + en[1]) / 2 + 2]]);
            for (var q = 0; q < size(rvX); q += 1)
            {
                const rid = bid + ("rv" ~ q);
                mkCyl(context, rid, vector(rvX[q], yc, dT + H - wall - 0.1), vector(rvX[q], yc, topZ + pT + 0.1), 0.265);
                rv = append(rv, qCreatedBy(rid, EntityType.BODY));
            }

            // Bed: tube (closed ends act as end caps) welded to both bottom flanges, notched, drilled.
            const bb = bid + "b";
            mkBox(context, bb + "outer", x0, x1, yc - W / 2, yc + W / 2, dT, dT + H);
            doCut(context, bb + "hollow", qCreatedBy(bb + "outer", EntityType.BODY), qCreatedBy(bid + "inner", EntityType.BODY));
            for (var f in [1, -1])
            {
                const fid = bb + ("fl" ~ (f > 0 ? "A" : "B"));
                mkBox(context, fid + "h", x0, x1, yc + f * W / 2, yc + f * (W / 2 + 1.5), dT, dT + 0.125);
                mkBox(context, fid + "v", x0, x1, yc + f * W / 2, yc + f * (W / 2 + 0.125), dT, dT + 1.5);
            }
            doUnite(context, bb + "weld", qCreatedBy(bb, EntityType.BODY));
            doCut(context, bb + "notches", qCreatedBy(bb, EntityType.BODY), qUnion(notches));
            opBoolean(context, bb + "drill", { "targets" : qCreatedBy(bb, EntityType.BODY), "tools" : qUnion(holes), "operationType" : BooleanOperationType.SUBTRACTION, "keepTools" : true });
            opBoolean(context, bb + "rivnut", { "targets" : qCreatedBy(bb, EntityType.BODY), "tools" : qUnion(rv), "operationType" : BooleanOperationType.SUBTRACTION, "keepTools" : true });
            paintBody(context, qCreatedBy(bb, EntityType.BODY), "Engine bed 2x6x1/8 6061 with flanges (welded) - " ~ side, alu);

            // Bottom doublers between the ribs, drilled to match the flanges.
            const segs = [[x0, RIBS[0] - 1.5], [RIBS[0] + 1.5, RIBS[1] - 1.5], [RIBS[1] + 1.5, x1]];
            for (var k = 0; k < size(segs); k += 1)
            {
                const did = bid + ("dbl" ~ k);
                mkBox(context, did + "box", segs[k][0], segs[k][1], yc - 3, yc + 3, 0, dT);
                opBoolean(context, did + "drill", { "targets" : qCreatedBy(did, EntityType.BODY), "tools" : qUnion(holes), "operationType" : BooleanOperationType.SUBTRACTION, "keepTools" : true });
                paintBody(context, qCreatedBy(did, EntityType.BODY), "Bottom doubler 1/8 5052 - " ~ side ~ " " ~ (k + 1), dblC);
            }
            opDeleteBodies(context, bid + "holesDel", { "entities" : qUnion(holes) });

            // Mount plates for the A-frame and brace feet, bolted down into the bed-top rivnuts.
            // The starboard exhaust hangers also bolt to the A-frame plates (two 5/16 bolts each).
            for (var m = 0; m < size(FEET); m += 1)
            {
                const ft = FEET[m];
                const pid = bid + ("plate" ~ m);
                mkBox(context, pid + "box", ft[0] - 1, ft[1] + 1, s * 12, s * 17, topZ, topZ + pT);
                opBoolean(context, pid + "drill", { "targets" : qCreatedBy(pid, EntityType.BODY), "tools" : qUnion(rv), "operationType" : BooleanOperationType.SUBTRACTION, "keepTools" : true });
                paintBody(context, qCreatedBy(pid, EntityType.BODY), "Mount plate 1/4 6061 - " ~ ft[2] ~ " foot - " ~ side, plateC);
            }

            // Engine-mount plates: bolted to the bed top (rivnuts) and reaching inboard under the rubber
            // engine mounts, whose studs take nylocks underneath the overhang.
            for (var em = 0; em < size(ENGINE); em += 1)
            {
                const ep = ENGINE[em];
                const xc = (ep[0] + ep[1]) / 2;
                var sh = [];
                for (var px in [xc - 2, xc + 2])
                {
                    const hid = bid + ("eh" ~ em ~ round(px * 10));
                    mkCyl(context, hid, vector(px, s * 9.5, topZ - 0.1), vector(px, s * 9.5, topZ + pT + 0.1), 0.265);
                    sh = append(sh, qCreatedBy(hid, EntityType.BODY));
                    const nid = bid + ("enut" ~ em ~ round(px * 10));
                    mkCyl(context, nid, vector(px, s * 9.5, topZ - 0.45), vector(px, s * 9.5, topZ), 0.4);
                    paintBody(context, qCreatedBy(nid, EntityType.BODY), "Nylock nut 1/2-13 + washer (engine mount stud)", nutC);
                }
                const pid = bid + ("eplate" ~ em);
                mkBox(context, pid + "box", ep[0], ep[1], s * 7.25, s * 17, topZ, topZ + pT);
                opBoolean(context, pid + "drill", { "targets" : qCreatedBy(pid, EntityType.BODY), "tools" : qUnion(concatenateArrays([sh, rv])), "operationType" : BooleanOperationType.SUBTRACTION, "keepTools" : true });
                paintBody(context, qCreatedBy(pid, EntityType.BODY), "Engine mount plate 1/4 6061 - " ~ ep[2] ~ " - " ~ side, plateC);
                opDeleteBodies(context, pid + "holesDel", { "entities" : qUnion(sh) });
            }
            opDeleteBodies(context, bid + "rvDel", { "entities" : qUnion(rv) });

            // ---- Cage support ----
            const cid = id + ("cage" ~ side);
            const cx = 11;                       // post spans X 11-13
            const hw = definition.hullHalf / inch;
            const flare = definition.sideFlare;
            const u = function(z) { return hw + flare * z; };
            const zTop = definition.cageZ / inch - pT;
            const axis = normalize(vector(0, s * flare, 1));
            const nIn = normalize(vector(0, -s * 1, flare));
            // The post top is cut flush with the underside of the top plate (about 29 deg to the post).
            mkBox(context, cid + "postTrim", cx - 1, cx + 4, s * (hw - 6), s * (hw + 20), zTop, zTop + 8);
            // Hole-saw reliefs in the post's inboard leg, around the outboard row of plate nuts.
            var relief = [qCreatedBy(cid + "postTrim", EntityType.BODY)];
            for (var rx2 in [10.75, 13.25])
            {
                const rid = cid + ("relief" ~ round(rx2 * 10));
                mkCyl(context, rid, vector(rx2, s * (34.45 - dC), zTop - 0.6), vector(rx2, s * (34.45 - dC), zTop + 0.1), 0.45);
                relief = append(relief, qCreatedBy(rid, EntityType.BODY));
            }
            const P0 = vector(cx, s * (u(pT) - 0.2), pT);
            const len = (zTop + 1 - pT) / axis[2];
            angleStock(context, cid + "post", P0, axis, vector(1, 0, 0), nIn, 2, 0.25, len);
            doCut(context, cid + "post" + "trim", qCreatedBy(cid + "post", EntityType.BODY), qUnion(relief));
            paintBody(context, qCreatedBy(cid + "post", EntityType.BODY), "Cage support post 2x2x1/4 6061 angle - " ~ side, alu);

            mkBox(context, cid + "foot", 9, 15, s * (hw - 3.5), s * hw, 0, pT);
            paintBody(context, qCreatedBy(cid + "foot", EntityType.BODY), "Cage support bottom foot 1/4 6061 - " ~ side, plateC);

            // Plate edges stop just inside the flared side at the plate underside (its lowest, narrowest point).
            const uTop = u(zTop) - 0.05;
            var th = [];
            // Hole pattern matches the narrowed rev7 cage foot pads (bolts 2.5 in apart each way).
            for (var tx in [10.75, 13.25])
            {
                for (var hu in [31.95 - dC, 34.45 - dC])
                {
                    const hid = cid + ("th" ~ round(tx * 10) ~ round(hu * 10));
                    mkCyl(context, hid, vector(tx, s * hu, zTop - 0.1), vector(tx, s * hu, zTop + pT + 0.1), 0.203);
                    th = append(th, qCreatedBy(hid, EntityType.BODY));
                    const nid = cid + ("nut" ~ round(tx * 10) ~ round(hu * 10));
                    mkCyl(context, nid, vector(tx, s * hu, zTop - 0.33), vector(tx, s * hu, zTop), 0.32);
                    paintBody(context, qCreatedBy(nid, EntityType.BODY), "Nylock nut 3/8-16 + washer (under plate)", nutC);
                }
            }
            mkBox(context, cid + "top" + "box", 9.5, 14.5, s * (31.0 - dC), s * uTop, zTop, zTop + pT);
            doCut(context, cid + "top" + "drill", qCreatedBy(cid + "top", EntityType.BODY), qUnion(th));
            paintBody(context, qCreatedBy(cid + "top", EntityType.BODY), "Cage support top plate 1/4 6061 - " ~ side, plateC);

            // Knee brace: horizontal angle from the post aft to a clip plate bolted to the inside of the transom.
            const zk = 10;
            const uk = u(zk) - 2.4;
            const xT = definition.transomInside / inch;
            angleStock(context, cid + "knee", vector(xT + pT, s * uk, zk), vector(1, 0, 0), vector(0, s, 0), vector(0, 0, -1), 2, 0.25, cx - 0.1 - xT - pT);
            paintBody(context, qCreatedBy(cid + "knee", EntityType.BODY), "Cage support knee 2x2x1/4 6061 angle - " ~ side, alu);
            // Knee-to-post clip: one leg on top of the knee, one against the post's aft face (two 5/16 bolts each).
            angleStock(context, cid + "kneeClip", vector(cx, s * uk, zk), vector(0, s, 0), vector(-1, 0, 0), vector(0, 0, 1), 2, 0.25, 2);
            paintBody(context, qCreatedBy(cid + "kneeClip", EntityType.BODY), "Cage support knee clip 2x2x1/4 6061 angle - " ~ side, plateC);
            // Clip stays inside the flared side at its lowest edge.
            mkBox(context, cid + "clip", xT, xT + pT, s * (uk - 2.5), s * (u(zk - 3) - 0.1), zk - 3, zk + 1);
            paintBody(context, qCreatedBy(cid + "clip", EntityType.BODY), "Cage support transom clip 1/4 6061 - " ~ side, plateC);

            // Aft cage feet: a vertical angle post standing on the knee, under a second top plate.
            const zAft = definition.cageZ / inch + 1.4 - pT; // rev7 aft pads sit 1.4 in above the front pads
            angleStock(context, cid + "aftPost", vector(2.05, s * (uk + 2), zk), vector(0, 0, 1), vector(1, 0, 0), vector(0, -s, 0), 2, 0.25, zAft - zk);
            paintBody(context, qCreatedBy(cid + "aftPost", EntityType.BODY), "Cage support aft post 2x2x1/4 6061 angle - " ~ side, alu);
            var ah = [];
            for (var ax in [1.8, 4.3])
            {
                for (var au in [31.95 - dC, 34.45 - dC])
                {
                    const hid = cid + ("ath" ~ round(ax * 10) ~ round(au * 10));
                    mkCyl(context, hid, vector(ax, s * au, zAft - 0.1), vector(ax, s * au, zAft + pT + 0.1), 0.203);
                    ah = append(ah, qCreatedBy(hid, EntityType.BODY));
                    const nid = cid + ("anut" ~ round(ax * 10) ~ round(au * 10));
                    mkCyl(context, nid, vector(ax, s * au, zAft - 0.33), vector(ax, s * au, zAft), 0.32);
                    paintBody(context, qCreatedBy(nid, EntityType.BODY), "Nylock nut 3/8-16 + washer (under plate)", nutC);
                }
            }
            mkBox(context, cid + "aftTop" + "box", 0.55, 5.55, s * uk, s * (u(zAft) - 0.05), zAft, zAft + pT);
            doCut(context, cid + "aftTop" + "drill", qCreatedBy(cid + "aftTop", EntityType.BODY), qUnion(ah));
            paintBody(context, qCreatedBy(cid + "aftTop", EntityType.BODY), "Cage support aft top plate 1/4 6061 - " ~ side, plateC);
        }
    });
