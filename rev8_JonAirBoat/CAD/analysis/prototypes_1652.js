// Prototype A / B / C comparison on the 1652 hull: dock trim, running trim, top speed and the thrust margin
// at the hump, for solo, driver + 1 and driver + 2.
// Hull hydrostatics and planing come from compare_hulls.js (its 1652 entry, at the CAD placeholder length).
// Equipment weights are the v1.0.0.3 CAD weights; trim_stability.js still models the 1448 hull.
// Prototype C (G13B turbo) is an estimate: about 140 hp at low boost, static thrust scaled from B by
// power^(2/3) (momentum theory, same 64 in prop), about 25 lb of turbo, intercooler and piping.
// Usage: node prototypes_1652.js
'use strict';
const fs = require('fs');
const path = require('path');

// Reuse compare_hulls.js without its report loop.
const src = fs.readFileSync(path.join(__dirname, 'compare_hulls.js'), 'utf8');
const m = {};
new Function('m', src.slice(0, src.indexOf('const out = [];')).replace("'use strict';", '') +
  '; Object.assign(m, { HULLS, floatAt, savitsky, mass, top, humpMargin });')(m);
const hull = { ...m.HULLS['1652'], bowX: 189.5 };

// [name, lb, X, Y, Z]
const BASE = [
  ['Hull 1652, middle bench out, rear bench center out', 438, 72, 0, 7],
  ['UHMW strips, chine angles, bow skin (scaled to 1652)', 110, 85, 0, 1.5],
  ['Engine beds, mounts, plates (CAD + connection fixes)', 59, 32, 0, 4],
  ['Cage supports', 15.5, 8, 0, 12],
  ['Engine G13 SOHC, dressed', 185, 38, 0.5, 18],
  ['Redrive, belt, pulleys', 55, 18, 0, 34],
  ['A-frame, bolted aluminum (CAD)', 81, 18, 0, 35],
  ['Prop shaft, hub, bearings', 15, 12, 0, 53],
  ['Prop, 3-blade', 20, 5, 0, 53],
  ['Radiator, coolant', 25, 17.5, 0, 66],
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
// extraLb: weight over Prototype A (engine, prop, turbo system), placed at the engine.
const PROTOS = {
  A: { name: 'G13 SOHC, 3-blade', hp: 70, T0: 330, rpm: 2400, extraLb: 0 },
  B: { name: 'G13B DOHC, 4-blade', hp: 100, T0: 440, rpm: 2300, extraLb: 17 },
  C: { name: 'G13B turbo (low boost), 4-blade', hp: 140, T0: Math.round(440 * Math.pow(140 / 100, 2 / 3)), rpm: 2300, extraLb: 42 },
};
const VP = 85;   // speed (mph) at which thrust falls to zero; linear fall-off as in trim_stability.js

const rows = [];
for (const [pk, p] of Object.entries(PROTOS)) {
  for (const [lk, l] of Object.entries(LOADS)) {
    const items = [...BASE, ['Prototype extra weight', p.extraLb, 36, 0, 24], ...l.crew];
    const mp = m.mass(items);
    const fl = m.floatAt(hull, mp.W, mp.LCG);
    const thr = { T0: p.T0, Vp: VP };
    const t = m.top(hull, mp, thr, l.CdA);
    rows.push({ proto: pk, hp: p.hp, staticThrust: p.T0, load: lk, weightLb: Math.round(mp.W),
      dockTrim: +fl.trim.toFixed(2), topMph: t.mph, topTrim: +t.s.tau.toFixed(2),
      humpMarginLbf: Math.round(m.humpMargin(hull, mp, thr, l.CdA)), propTorqueFtLb: Math.round(p.hp * 5252 / p.rpm) });
  }
}
console.table(rows);
