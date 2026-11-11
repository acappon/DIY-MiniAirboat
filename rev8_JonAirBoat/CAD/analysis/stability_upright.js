// Upright stability of the minimal 1448: three load cases, heel curves, heeling moments, pitch under thrust and running limits.
// Run: node stability_upright.js
// Hull: the analytic 1448 form from minimal_1448.js. Sections are a flat bottom with flared sides to the gunwale at Z 19, and the transom
// top at Z 15. Each heel is solved by station, with the hull at its level trim, holding displacement constant. Free surface: none, dry bilge.
'use strict';
const log = console.log, tab = console.table; console.log = () => {}; console.table = () => {};
const m = require('./minimal_1448.js'); console.log = log; console.table = tab;
const { h, OUTFIT, ENGINES, hullItem, mass, floatAt, top, humpMargin } = m;

const eng = ENGINES.find(e => e[0] === 'etec800');
const EQUIP = [...OUTFIT, ['Engine system', eng[3], 20, 0, 22], ['Prop', eng[4], 4, 0, 42]];
const DRIVER = ['Driver', 200, 83.5, 0, 27];
const thrust = eng[5], Vp = eng[6];
const SHEER = 19, TRANSOM = 15;

// Hull offsets.
const zb = x => x <= h.flatEnd ? 0 : h.bowZ * Math.pow((x - h.flatEnd) / (h.bowX - h.flatEnd), 1.5);
const hw = x => x <= h.flatEnd ? h.bottom / 2 : h.bottom / 2 - 6 * Math.pow((x - h.flatEnd) / (h.bowX - h.flatEnd), 2);
const flare = h.flare;
const sheerZ = x => x < 6 ? TRANSOM + (SHEER - TRANSOM) * x / 6 : SHEER;

// Immersed area and centroid of one section, heeled phi (+ = port down), water line through (0, d) in the section frame.
// y is positive to port. Numerical strip over Y in the upright frame.
function section(x, d, phi, trimT) {
  const b0 = hw(x), z0 = zb(x), top = sheerZ(x), tan = Math.tan(phi);
  const n = 48; let A = 0, My = 0, Mz = 0;
  const wMax = b0 + flare * (top - z0);
  for (let i = 0; i < n; i++) {
    const y = -wMax + (i + 0.5) * 2 * wMax / n, dy = 2 * wMax / n;
    const lo = Math.max(z0, z0 + (Math.abs(y) - b0) / flare);      // hull surface at this y
    const wl = d + y * tan - x * trimT;                            // water surface; port (+y) goes down so water rises there
    const hi = Math.min(wl, top);
    if (hi > lo) { const a = (hi - lo) * dy; A += a; My += a * y; Mz += a * (hi + lo) / 2; }
  }
  return { A, My, Mz };
}
function hullAt(d, phi, trimT) {
  let V = 0, My = 0, Mz = 0, Mx = 0;
  for (let x = 1; x < h.bowX; x += 2) { const s = section(x, d, phi, trimT); V += s.A * 2; My += s.My * 2; Mz += s.Mz * 2; Mx += s.A * 2 * x; }
  return { V, Y: My / V, Z: Mz / V, X: Mx / V };
}
const RHO = 62.4 / 1728;
function solveD(W, phi, trimT) {
  const Vt = W / RHO; let lo = -30, hi = 45;
  for (let k = 0; k < 36; k++) { const c = (lo + hi) / 2; hullAt(c, phi, trimT).V < Vt ? lo = c : hi = c; }
  return (lo + hi) / 2;
}
function arm(W, TCG, KG, deg, t) {
  const phi = deg * Math.PI / 180, d = solveD(W, phi, t), b = hullAt(d, phi, t);
  return { r: (b.Y - TCG) * Math.cos(phi) + (b.Z - KG) * Math.sin(phi), d };
}
// Lowest freeboard round the hull at heel phi: gunwale at the sheer and transom corners.
function floodMargin(d, phiDeg, t) {
  const tan = Math.tan(phiDeg * Math.PI / 180);
  let min = 99, where = '';
  for (let x = 0; x <= h.bowX; x += 4) {
    const top = sheerZ(x), y = hw(x) + flare * (top - zb(x));
    const fb = top - (d + y * tan - x * t);
    if (fb < min) { min = fb; where = 'X ' + x; }
  }
  return { min, where };
}
// Righting arm: positive restores for a port heel, using arm(+deg), and the mirror for starboard.
function caseRun(name, crew) {
  const items = [hullItem, ...EQUIP, ...crew];
  const mp = mass(items);
  const fl = floatAt(h, mp.W, mp.LCG);
  const t = Math.tan(fl.trim * Math.PI / 180);
  const Tst = fl.Tst;
  const hy = fl.hy;
  const BMt = hy.IT / hy.V, GM = hy.VCB + BMt - mp.VCG;
  return { name, items, mp, fl, t, Tst, GM, BMt };
}

const GEAR_Y = 150;
const tied = [['Gear tied down behind the driver', 150, 60, 0, 10]];
// Scattered loose: six bundles on the floor and benches.
const scatter = [['Loose 1', 30, 110, 12, 6], ['Loose 2', 25, 95, -14, 6], ['Loose 3', 30, 60, 14, 6], ['Loose 4', 25, 50, -16, 6], ['Loose 5', 20, 82, 18, 16], ['Loose 6', 20, 25, -18, 17]];
// Loose gear after it slides. Floor half width about 23 in.
const slidPort = scatter.map(s => [s[0], s[1], s[2], 23, 6]);
const slidBow = scatter.map(s => [s[0], s[1], 118, s[3] / 2, 8]);
const slidAft = scatter.map(s => [s[0], s[1], 42, s[3], 6]);

const cases = [
  ['1. Driver only', [DRIVER]],
  ['2. Driver + 150 lb tied down', [DRIVER, ...tied]],
  ['3a. Driver + 150 lb loose, scattered', [DRIVER, ...scatter]],
  ['3b. Loose gear slid to one side', [DRIVER, ...slidPort]],
  ['3c. Loose gear slid forward', [DRIVER, ...slidBow]],
  ['3d. Loose gear slid aft', [DRIVER, ...slidAft]],
];

const propT = 244 * 12;                     // in-lb, prop reaction at full power
const q30 = 0.00256 * 30 * 30;              // psf at 30 mph
const windA = 18, windArm = 34;             // ft^2 and in of the side area above water, against the waterline
const wind = q30 * windA * windArm;
const out = [];
for (const [name, crew] of cases) {
  const c = caseRun(name, crew);
  const { mp, t, Tst } = c, W = mp.W, TCG = mp.TCG, KG = mp.VCG;
  const curve = [5, 10, 15, 20, 25, 30, 35].map(d => { const a = arm(W, TCG, KG, d, t); const b = arm(W, TCG, KG, -d, t); return { d, port: a.r, stbd: -b.r, fbP: floodMargin(a.d, d, t) }; });
  let flood = null; for (let d = 1; d < 50; d += 0.5) { const a = arm(W, TCG, KG, d, t); if (floodMargin(a.d, d, t).min <= 0) { flood = d; break; } }
  // Static list from TCG alone.
  const listDeg = Math.atan2(TCG, c.GM) * 180 / Math.PI;
  const heelOf = M => Math.atan2(M + W * TCG, W * c.GM) * 180 / Math.PI;
  const boarding = 200 * 34;                // 200 lb stepping on the gunwale, weight added at the side
  const W2 = W + 200, TCG2 = (W * TCG + 200 * 34) / W2, KG2 = (W * KG + 200 * 19) / W2;
  const fl2 = floatAt(h, W2, (W * mp.LCG + 200 * 82) / W2);
  const t2 = Math.tan(fl2.trim * Math.PI / 180);
  let boardHeel = null, boardFb = null;
  for (let d = 0; d < 40; d += 0.25) { const a = arm(W2, TCG2, KG2, d, t2); if (a.r >= 0) { boardHeel = d; boardFb = +floodMargin(a.d, d, t2).min.toFixed(1); break; } }
  // Worst steady stack: loose gear list, prop torque, 30 mph wind and the driver leaning 15 in, all the same way.
  const stack = heelOf(propT + wind + 200 * 15);
  // Thrust pitch: bow down moment, tied at the dock, thrust line Z 52.
  const M = thrust * (52 - Tst);
  const LCG2 = mp.LCG + M / W;
  const flp = floatAt(h, W, LCG2);
  const bowFb = SHEER - (flp.Tst - 150 * Math.tan(flp.trim * Math.PI / 180));
  // Running: rollover threshold for a chine trip, half bottom beam against CG height, and top speed.
  const sideG = (h.bottom / 2 - Math.abs(TCG)) / (KG - 0);
  const CdA = crew.length > 1 ? 12 : 9;
  const tp = top(h, mp, { T0: thrust, Vp }, CdA);
  const hump = humpMargin(h, mp, { T0: thrust, Vp }, CdA);
  const r = { case: name, W: Math.round(W), LCG: +mp.LCG.toFixed(1), TCG: +TCG.toFixed(1), KG: +KG.toFixed(1), draft: +Tst.toFixed(2), trim: +c.fl.trim.toFixed(2), transomFb: +(TRANSOM - Tst).toFixed(1), GM: +c.GM.toFixed(1),
    list: +listDeg.toFixed(1), propHeel: +heelOf(propT).toFixed(1), windHeel: +heelOf(wind).toFixed(1), floodDeg: flood, boardHeel, boardFb, stackHeel: +stack.toFixed(1), thrustTrim: +flp.trim.toFixed(1), bowFbThrust: +bowFb.toFixed(1), chineTripG: +sideG.toFixed(2), topMph: tp.mph, planing: tp.planing, topTrim: +tp.s.tau.toFixed(1), hump: Math.round(hump),
    curve: curve.map(x => [x.d, +x.port.toFixed(1), +x.stbd.toFixed(1)]) };
  out.push(r);
  console.log(JSON.stringify(r));
}
console.log('propT', propT, 'wind in-lb', Math.round(wind));
module.exports = { out };
