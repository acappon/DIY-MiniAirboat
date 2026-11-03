// Performance of the rev8 airboat on an 1860-class hull with a Toyota 2GR-FE 3.5 L V6: equipment weight and
// payload under the capacity plate, dock trim, running trim, top speed and the thrust margin at the hump.
// Hull hydrostatics and planing come from compare_hulls.js (its 1860 entry, scaled from dealer specs).
// Equipment weights are the v1.0.0.3 CAD weights, with the engine, redrive and radiator resized for the 2GR
// (estimates). Static thrust is scaled from 440 lbf at 100 hp by power^(2/3) on the current 64 in prop; the
// prop and redrive are not yet sized for the 2GR, so treat speeds as +/-25% until the static test.
// Usage: node performance_1860.js
'use strict';
const fs = require('fs');
const path = require('path');

// Reuse compare_hulls.js without its report loop.
const src = fs.readFileSync(path.join(__dirname, 'compare_hulls.js'), 'utf8');
const m = {};
new Function('m', src.slice(0, src.indexOf('const out = [];')).replace("'use strict';", '') +
  '; Object.assign(m, { HULLS, floatAt, savitsky, mass, top, humpMargin });')(m);
const hull = m.HULLS['1860'];

// [name, lb, X, Y, Z]
const BASE = [
  ['Hull 1860, middle bench out, rear bench center out', 640, 83, 0, 7],
  ['UHMW strips, chine angles, bow skin (scaled to 1860)', 147, 85, 0, 1.5],
  ['Engine beds, mounts, plates (CAD + connection fixes)', 59, 32, 0, 4],
  ['Cage supports', 15.5, 8, 0, 12],
  ['Engine Toyota 2GR-FE 3.5 V6, dressed (estimate)', 370, 38, 0.5, 18],
  ['Redrive, belt, pulleys (resized, estimate)', 75, 18, 0, 34],
  ['A-frame, bolted aluminum (CAD)', 81, 18, 0, 35],
  ['Prop shaft, hub, bearings', 15, 12, 0, 53],
  ['Prop', 27, 5, 0, 53],
  ['Radiator, coolant (resized, estimate)', 35, 17.5, 0, 66],
  ['Exhaust', 20, 26, 9, 22],
  ['Prop cage, black steel', 85, 5, 0, 52],
  ['Rudders, bellcrank', 25, -7, 0, 45],
  ['Controls (CAD)', 22, 108, 5.7, 7.3],
  ['Battery, Group 31 LiFePO4', 28, 104.5, 6.85, 5.6],
  ['Dry box, power hub, wiring', 19.1, 104.5, 1, 7],
  ['Interior mount rails', 11.9, 88, -4, 0.5],
  ['Lights', 10, 30, 0, 80],
  ['Driver seat and tracks', 15, 104, 0, 18],
  ['Storage box / thwart', 45.7, 71, 0, 9],
  ['Grab bar', 7.5, 55, 0, 18],
  ['Console', 22.9, 112, -22, 22],
  ['Trim tab', 4, -2, -19, 1],
  ['Safety gear', 25, 64, 0, 8],
  ['Fuel 6.5 gal + tank (plan position)', 47, 135, 0, 6],
];
const DRIVER = ['Driver', 200, 104, 0, 26];
const LOADS = {
  solo: { crew: [DRIVER], CdA: 12 },
  plus1: { crew: [DRIVER, ['Passenger', 200, 69, 0, 27]], CdA: 15 },
  plus2: { crew: [DRIVER, ['Passenger', 200, 69, 12, 27], ['Passenger', 200, 69, -12, 27]], CdA: 18 },
};
const PLATE = hull.plate;
const HP = 270, RPM_PROP = 2300, VP = 85;                // VP: speed (mph) at which thrust falls to zero
const T0 = Math.round(440 * Math.pow(HP / 100, 2 / 3));
const eqLb = BASE.reduce((s, b) => s + b[1], 0) - hull.hullLb;
console.log('Equipment ' + Math.round(eqLb) + ' lb; plate ' + PLATE + ' lb; payload under the plate ' + Math.round(PLATE - eqLb) +
  ' lb; static thrust ~' + T0 + ' lbf; prop torque ~' + Math.round(HP * 5252 / RPM_PROP) + ' ft-lb');
LOADS.soloGear = { crew: [DRIVER, ['Gear on the storage box', 100, 64, 0, 14]], CdA: 12 };
const rows = [];
for (const [lk, l] of Object.entries(LOADS)) {
  for (const fuel of [1, 0.25]) {
    const items = BASE.map(b => b[0].startsWith('Fuel') ? [b[0], 7 + (b[1] - 7) * fuel, b[2], b[3], b[4]] : b).concat(l.crew);
    const mp = m.mass(items);
    const fl = m.floatAt(hull, mp.W, mp.LCG);
    const thr = { T0, Vp: VP };
    const t = m.top(hull, mp, thr, l.CdA);
    rows.push({ load: lk, fuel, weightLb: Math.round(mp.W), dockTrim: +fl.trim.toFixed(2), transomFreeboard: +(hull.transom - fl.Tst).toFixed(1),
      topMph: t.mph, topTrim: +t.s.tau.toFixed(2), humpMarginLbf: Math.round(m.humpMargin(hull, mp, thr, l.CdA)) });
  }
}
console.table(rows);
