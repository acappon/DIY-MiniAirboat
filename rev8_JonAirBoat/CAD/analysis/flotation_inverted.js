// Capsized flotation of the minimal 1448: boat upside down, driver swimming, full tank, 200 lb of gear strapped behind the driver.
// Sizes the bow wedge under the front shelf and the blocks beside the prop cage so the boat floats level, like a raft.
// Run: node flotation_inverted.js [json]
//
// Physics: the hull is open, so it floods and adds no buoyancy of its own. No credit is taken for air trapped under it, or for the
// sealed dry box. An item under water weighs W(1 - 1/SG), an item above water weighs W. The boat only has a real float height where
// foam pierces the surface: if all the foam is under water there is no waterplane to hold it level.
// Boat axes: X forward, Y to port, Z up from the hull bottom when upright. Upside down, water covers everything with Z above Zw,
// and the hull bottom stands Zw inches clear of the water.
'use strict';
const WATER = 62.4, FOAM_PCF = 2, FT3 = 1728;

// [name, lb, X, Y, Zlow, Zhigh, SG]. Weights and positions are from minimal_1448.js; SG is the flooded item as a whole.
const ALU = 2.7, STEEL = 7.8;
const ITEMS = [
  ['Hull bottom, ribs and lower sides', 110, 58, 0, 0, 2, ALU],
  ['Hull upper sides, benches, decks, transom', 138, 65, 0, 2, 19, ALU],
  ['Stand, shoes, doublers, clips, pads', 49.1, 18, 0, 3, 56, ALU],
  ['Prop cage and mesh', 45, 4, 0, 18, 88, ALU],
  ['Twin rudders and linkage', 8.5, -6, 0, 20, 50, ALU],
  ['Stick, cables, pedal, heel rests', 8.5, 70, 8, 1, 30, 3],
  ['Battery', 4.5, 83.6, 0, 4, 9, 2.5],
  // Seat module, v1.0.1.3. The closed-cell seat pad and the compartment foam are counted as foam below, not here.
  ['Seat struts (carbon) and clips', 5.6, 81.5, 0, 9, 11, 1.8],
  ['Dry box, lid, brackets, no credit for the sealed air', 3.9, 81.5, 0, 3.75, 11, ALU],
  ['Storage compartment walls and lids', 4.6, 81.5, 0, 1.5, 11, ALU],
  ['Seat base sandwich and vinyl', 3.4, 84, 0, 11.25, 14.1, 0.45],
  // Seatback: 18 x 15 in, 1/4 ply with 1-1/2 in closed-cell foam in front and 1/2 in behind, 2-1/4 in in all: 608 in^3 (0.35 ft^3) for 3 lb,
  // so SG about 0.14: it is a float, not a near-neutral part (was SG 0.5, which counted only 3 lb of its lift).
  ['Folding seatback', 3, 75, 0, 14.5, 29, 3 / (18 * 15 * 2.25 / 1728 * 62.4)],
  ['Seat module small parts', 0.4, 81.5, 0, 3, 11, ALU],
  // Oarlocks, risers and rests. The oars are operator gear, part of the gear load below (and they float).
  ['Oarlocks, risers, rests', 4, 73.5, 0, 19.5, 27, ALU],
  ['Fuel gauge sender, gauge, straps, line', 2, 110, -10, 4, 26, 1.5],
  ['PFDs and dock lines', 6, 85, 0, 3, 12, 1],
  ['Fire extinguisher', 4, 99.5, -25, 13, 27, 3],
  ['Air horn', 0.5, 105, -14, 24, 26, 2],
  ['Flag and whip', 2, -4, 0, 20, 132, 1.8],
  ['Navigation lights', 1.5, 60, 0, 17, 50, 1.5],
  ['UHMW bottom sheet and fasteners', 35.6, 77, 0, 1.2, 0, 0.94],
  ['Trim tab and turnbuckle', 3, -2, -19, 0, 3, ALU],
  ['Tailpipe, shields, pipe guard', 7, 12, 14, 16, 45, STEEL],
  ['Cage struts and cross-straps', 2.5, 25, 0, 20, 78, ALU],
  ['Dash', 4.4, 103, -13, 26, 30, 2],
  ['Phone holder', 0.8, 100, 0, 26, 30, 2],
  ['Console frame and pod', 2.9, 105, -18, 2, 30, ALU],
  ['Harness, switch, solenoid, conduit', 12.4, 62, 0, 3, 8, 3],
  ['Belt guard', 3, 13, 0, 22, 52, ALU],
  ['Radiator brackets, overflow bottle', 3, 25, 0, 30, 56, ALU],
  ['Brackets, rivnuts, bolts', 5, 40, 0, 2, 20, STEEL],
  ['Bilge pump and hose', 2, 20, 0, 1, 3, 2],
  ['LED light bar', 6, 8, 0, 88, 91, 2.5],
  ['Searchlight', 8, 6, 0, 88, 96, 2.5],
  ['800R engine, bare', 79, 20, 0, 15, 36, 4],
  ['Exhaust', 20, 16, 12, 30, 56, STEEL],
  ['Radiator, flooded', 16.5, 16, -10, 34, 56, ALU],
  ['Belt, pulleys, shaft, bearings, plates', 36.3, 14, 0, 22, 56, 4],
  ['Oil tank, ECU, engine harness', 10, 17, -13, 17, 27, 1.1],
  ['Engine mounts', 5, 20, 0, 14, 20, STEEL],
  ['Prop', 15, 4, 0, 19, 85, 1.6],
];
const fuelItem = sg => ['Fuel, 6 gal, in a sealed tank', 45, 143, 0, 16.5, 25, sg];
const gearItem = (sg, x) => ['Gear strapped behind the driver', 200, x, 0, 1, 13, sg];

// Hull inner half width at (x, z), from rays on the reference mesh.
const hw = (x, z) => x >= 133 ? 27 - 0.17 * (x - 135) - 0.5 * (15.5 - z) : Math.min(24.4 + 0.5 * z, x < 3 ? 27 : 99);
const zb = x => 5 + (x - 135) * 0.19;      // hull bottom under the shelf
const DECK = 16.5, SHELF_AFT = 133, SHELF_FWD = 156;

// Foam as 1 in voxels: [x, y, z, in^3].
function wedgeVox(Xa) {
  const v = [], d = 1;
  for (let x = Xa + d / 2; x < SHELF_FWD; x += d) for (let z = zb(x) + d / 2; z < DECK; z += d) {
    const w = hw(x, z);
    for (let y = -w + d / 2; y < w; y += d) v.push([x, y, z, d * d * d]);
  }
  return v;
}
// Aft corner block: beside the stand, under the cage. X 2.5 to 13, from the shoes out to the hull side, floor to gunwale.
// The port one stops at Z 10, well below the tailpipe, which leaves at about Y 28, Z 16.5.
function cornerVox(s, ztop) {
  const v = [], d = 1;
  for (let x = 3; x < 13; x += d) for (let z = 2; z < ztop; z += d) {
    const w = hw(x, z) - 0.5;
    for (let y = 10.5; y < w; y += d) v.push([x, s * y, z, 1]);
  }
  return v;
}
// Side block: strapped outside the hull beside the cage, clear of the prop inflow, reaching down to Z 8 so it pierces the surface
// upside down. Upright it stays above the static waterline, about Z 6 aft. Length and height set here, width solved.
const UP = { x0: -1, x1: 29, z0: 8, z1: 24, yIn: 34.5 };
function upperVox(s, width) {
  const v = [], d = 1, n = Math.max(0, width);
  if (n <= 0) return v;
  const nY = Math.ceil(n), dy = n / nY;
  for (let x = UP.x0 + d / 2; x < UP.x1; x += d) for (let z = UP.z0 + d / 2; z < UP.z1; z += d)
    for (let i = 0; i < nY; i++) v.push([x, s * (UP.yIn + (i + 0.5) * dy), z, dy]);
  return v;
}
function benchVox(fill) {
  if (fill <= 0) return [];
  const v = [];
  for (const [x0, x1, z0, z1] of [[13, 33, 2.4, 13.7], [75.7, 89.5, 1, 12.5]])
    for (let x = x0 + 0.5; x < x1; x += 1) for (let z = z0 + 0.5; z < z1; z += 1) {
      const w = 24.5 + 0.5 * z;
      for (let y = -w + 0.5; y < w; y += 1) v.push([x, y, z, fill]);
    }
  return v;
}

function statics(vox, Zw) {
  let lift = 0, mx = 0, my = 0, vol = 0;
  for (const [x, y, z, a] of vox) { const n = ((z >= Zw ? WATER : 0) - FOAM_PCF) * a / FT3; lift += n; mx += n * x; my += n * y; vol += a; }
  return { lift, mx, my, vol };
}
function itemsNet(items, Zw) {
  let W = 0, Mx = 0, My = 0;
  for (const it of items) {
    const [, w, x, y, z0, z1, sg] = it;
    const f = z1 > z0 ? Math.min(1, Math.max(0, (z1 - Zw) / (z1 - z0))) : (z0 >= Zw ? 1 : 0);
    const n = w * (1 - f / sg); W += n; Mx += n * x; My += n * y;
  }
  return { W, Mx, My };
}

// Solve the wedge length and the two upper block widths for force, pitch and roll balance at float height Zw.
function design({ Zw, gearSG = 1e9, fuelSG = 0.74, fill = 0, xGear = 57, extra = [], cornerS = 17, cornerP = 10 }) {
  const items = [...ITEMS, fuelItem(fuelSG), gearItem(gearSG, xGear), ...extra];
  const fixed = [...cornerVox(1, cornerP), ...cornerVox(-1, cornerS), ...benchVox(fill)];
  const net = itemsNet(items, Zw), base = statics(fixed, Zw);
  const upLift = (WATER - FOAM_PCF) / FT3 * (UP.x1 - UP.x0) * (UP.z1 - UP.z0);     // lb per inch of width
  const xUp = (UP.x0 + UP.x1) / 2;
  const trial = Xa => {
    const w = statics(wedgeVox(Xa), Zw);
    const need = net.W - base.lift - w.lift;   // lift the upper blocks must give
    return { w, need, gap: net.Mx - base.mx - w.mx - need * xUp };
  };
  // Search the wedge length: fine steps, 1 in voxels; take the closest pitch balance.
  let best = null;
  for (let Xa = SHELF_AFT; Xa < SHELF_FWD - 1; Xa += 1) { const t = trial(Xa); if (!best || Math.abs(t.gap) < Math.abs(best.gap)) best = { ...t, Xa }; }
  const need = Math.max(0, best.need), mean = need / upLift / 2;
  // Roll: unequal widths; lift arm is yIn + width / 2.
  let wP = mean, wS = mean;
  for (let k = 0; k < 40; k++) {
    const yP = UP.yIn + wP / 2, yS = UP.yIn + wS / 2;
    const gapM = net.My - base.my - upLift * (wP * yP - wS * yS);
    const d = gapM / (upLift * (yP + yS));
    wP += d; wS -= d;
  }
  const vox = [...wedgeVox(best.Xa), ...fixed, ...upperVox(1, wP), ...upperVox(-1, wS)];
  return { Zw, items, vox, Xa: best.Xa, gap: best.gap, wP, wS, net: net.W, wedge: best.w.vol / FT3, corners: statics(fixed.filter(() => true), Zw).vol / FT3, upper: (wP + wS) * (UP.x1 - UP.x0) * (UP.z1 - UP.z0) / FT3,
    foamNew: (best.w.vol + statics([...cornerVox(1, cornerP), ...cornerVox(-1, cornerS)], Zw).vol) / FT3 + (wP + wS) * (UP.x1 - UP.x0) * (UP.z1 - UP.z0) / FT3 };
}

// Rigid-body float at heel and pitch: buoyancy from foam voxels and item volumes, weight in air at G.
function points(d) {
  const P = [];
  for (const [x, y, z, a] of d.vox) P.push([x, y, z, a * WATER / FT3]);
  for (const [, w, x, y, z0, z1, sg] of d.items) if (sg < 1e8) {
    const b = w / sg;                              // lift of the item when under water, lb
    for (let i = 0; i < 4; i++) P.push([x, y, z0 + (i + 0.5) / 4 * (z1 - z0), b / 4]);
  }
  return P;
}
function weightCG(d) {
  let W = 0, mx = 0, my = 0, mz = 0;
  for (const [, w, x, y, z0, z1] of d.items) { W += w; mx += w * x; my += w * y; mz += w * (z0 + z1) / 2; }
  for (const [x, y, z, a] of d.vox) { const m = a * FOAM_PCF / FT3; W += m; mx += m * x; my += m * y; mz += m * z; }
  return { W, x: mx / W, y: my / W, z: mz / W };
}
// Righting arm in inches for a heel about the X axis (phi) or pitch about Y (theta). Positive pushes back to level.
function arm(d, phiDeg, thetaDeg) {
  const P = points(d), G = weightCG(d), phi = phiDeg * Math.PI / 180, th = thetaDeg * Math.PI / 180;
  const depth = p => (p[2] - G.z) * Math.cos(phi) * Math.cos(th) + (p[1] - G.y) * Math.sin(phi) + (p[0] - G.x) * Math.sin(th);
  let lo = -80, hi = 80;
  const buoy = c => P.reduce((s, p) => s + (depth(p) >= c ? p[3] : 0), 0);
  for (let k = 0; k < 40; k++) { const m = (lo + hi) / 2; buoy(m) > G.W ? lo = m : hi = m; }
  const c = (lo + hi) / 2;
  let b = 0, my = 0, mx = 0, mz = 0;
  for (const p of P) if (depth(p) >= c) { b += p[3]; my += p[3] * (p[1] - G.y); mx += p[3] * (p[0] - G.x); mz += p[3] * (p[2] - G.z); }
  const By = my / b, Bx = mx / b, Bz = mz / b;
  if (phiDeg) return (By * Math.cos(phi) - Bz * Math.sin(phi)) * (phiDeg > 0 ? 1 : -1);
  return (Bx * Math.cos(th) - Bz * Math.sin(th)) * (thetaDeg > 0 ? 1 : -1);
}

function report(t, o) {
  const d = design(o);
  const G = weightCG(d);
  const heel = [5, 10, 20, 30, -10, -20].map(a => +arm(d, a, 0).toFixed(1));
  const pitch = [3, 6, -3, -6].map(a => +arm(d, 0, a).toFixed(1));
  const out = { case: t, Zw: d.Zw, netLb: Math.round(d.net), wedgeFromX: d.Xa, wedgeFt3: +d.wedge.toFixed(2), upperPwidthIn: +d.wP.toFixed(1), upperSwidthIn: +d.wS.toFixed(1), upperFt3: +d.upper.toFixed(2),
    newFoamFt3: +d.foamNew.toFixed(2), newFoamLb: +(d.foamNew * FOAM_PCF).toFixed(1), gapInLb: Math.round(d.gap), Wair: Math.round(G.W), zG: +G.z.toFixed(1), heel, pitch };
  return out;
}

if (require.main === module) {
  const sum = [...ITEMS, fuelItem(.74)].reduce((s, i) => s + i[1], 0);
  console.log('Dry items, lb:', sum.toFixed(1), '(hull 248 + equipment 463.8 = 711.8)');
  console.log('Wedge capacity, ft3:', (wedgeVox(SHELF_AFT).reduce((s, v) => s + v[3], 0) / FT3).toFixed(2));
  console.log('Corner blocks, ft3 P/S:', (cornerVox(1, 10).length / FT3).toFixed(2), (cornerVox(-1, 17).length / FT3).toFixed(2));
  const rows = [];
  const NC = { cornerP: 0, cornerS: 0 };   // At 9 in the corner blocks sit mostly above water, so the design case leaves them out.
  const cases = [['A design: benches empty, gear no buoyancy', { Zw: 9, ...NC }], ['B benches 50% foam', { Zw: 4, fill: 0.5 }], ['C benches full', { Zw: 4, fill: 1 }], ['D gear SG 2', { Zw: 9, gearSG: 2, ...NC }], ['E gear SG 1.2', { Zw: 9, gearSG: 1.2, ...NC }],
    ['F tank flooded', { Zw: 9, fuelSG: 1, ...NC }], ['G gear at X 40', { Zw: 9, xGear: 40, ...NC }], ['H gear at X 70', { Zw: 9, xGear: 70, ...NC }]];
  for (const [t, o] of cases) rows.push(report(t, o));
  for (const r of rows) console.log(JSON.stringify(r));
}
// Chosen design, v1.0.1.3 (2026-11-10): rear bench inspected, full and sound; the middle bench is gone. Its flotation is
// replaced by the closed-cell seat pad, foam in the bottom of the two seat storage compartments and two footwell side
// blocks (Minimal_cockpit.fs), with the small bow wedge from X 154 and two equal corner blocks to Z 10.
// Reports float height, list and pitch.
const hullHalf = (x, z) => 24.4 + 0.5 * z - Math.max(0, x - 75) * 0.045;   // as in Minimal_cockpit.fs
function seatFoamVox() {
  const v = [];
  const add = (x0, x1, z0, z1, inside) => {
    for (let x = x0 + 0.5; x < x1; x++) for (let z = z0 + 0.5; z < z1; z++) for (let y = -31.5; y < 32; y++) if (inside(x, y, z)) v.push([x, y, z, 1]);
  };
  add(76.5, 91.5, 11.625, 14.125, (x, y) => Math.abs(y) < 19);                                                // seat pad
  add(77.6, 85.4, 1.5, 5, (x, y, z) => Math.abs(y) > 5.1 && Math.abs(y) < hullHalf(85.5, z) - 0.3);   // compartment bottoms
  for (const [sd, x0, x1] of [[1, 98, 113], [1, 113, 128], [-1, 98, 106], [-1, 110, 119], [-1, 119, 128]])  // footwell side blocks
    add(x0, x1, 1.5, 13, (x, y, z) => y * sd > 0 && Math.abs(y) < hullHalf(x1, z) - 0.3 && Math.abs(y) > hullHalf(x1, z) - 6.3);
  return v;
}
function chosen() {
  const items = [...ITEMS, fuelItem(0.74), gearItem(1e9, 57)];
  const B = [];
  for (let x = 13.5; x < 33; x++) for (let z = 2.9; z < 13.7; z++) {
    const w = 24.5 + 0.5 * z;
    for (let y = -w + 0.5; y < w; y++) B.push([x, y, z, 1]);
  }
  const vox = [...B, ...seatFoamVox(), ...wedgeVox(154), ...cornerVox(1, 10), ...cornerVox(-1, 10)];
  const d = { items, vox }, G = weightCG(d), P = points(d);
  let lo = -5, hi = 30;
  for (let k = 0; k < 40; k++) { const m = (lo + hi) / 2; P.reduce((s, p) => s + (p[2] >= m ? p[3] : 0), 0) > G.W ? lo = m : hi = m; }
  const hp = arm(d, 2, 0), hn = arm(d, -2, 0), pp = arm(d, 0, 2), pn = arm(d, 0, -2);
  return { newFoamFt3: +((vox.length - B.length) / FT3).toFixed(2), benchFt3: +(B.length / FT3).toFixed(2), bottomClearIn: +((lo + hi) / 2).toFixed(1), listDeg: +(2 * (hn - hp) / (hn + hp)).toFixed(1), pitchDeg: +(2 * (pn - pp) / (pn + pp)).toFixed(1),
    reserveLb: Math.round(P.reduce((s, p) => s + p[3], 0) - G.W), heel: [5, 10, 20, 30].map(a => +arm(d, a, 0).toFixed(1)), pitch: [3, 6].map(a => +arm(d, 0, a).toFixed(1)) };
}
if (require.main === module) console.log('CHOSEN', JSON.stringify(chosen()));
module.exports = { design, report, weightCG, arm, UP, hw, zb, chosen };
