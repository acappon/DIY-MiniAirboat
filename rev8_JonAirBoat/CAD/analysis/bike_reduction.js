// Engine stand and belt reduction for the minimal 1448 airboat with a GSX-R1000 K5-K8-class engine.
// Picks the gearbox gear and belt ratio for the prop rpm, checks belt speed and pull, prop-shaft stress and the
// thrust loads the stand carries. Inches, pounds, rpm. Every engine figure is to be confirmed on the real engine.
// Usage: node bike_reduction.js
'use strict';

// ---------------- Engine (GSX-R1000 K5, to confirm in the service manual) ----------------
const PRIMARY = 1.553;                       // crank to gearbox input; about 1.55, NOT CONFIRMED
const GEARS = { 3: 1.714, 4: 1.500, 5: 1.360, 6: 1.269 };   // K5 gearbox ratios
const ENGINE_RPM = 9500;                     // continuous operating point (peak torque 8,500, peak power 11,500)
const HP = 140;                              // at the gearbox output at 9,500 rpm (about 80 lb-ft x 9,500 / 5,252, less losses)
const BIKE_SPROCKET_R = 0.625 / Math.sin(Math.PI / 17) / 2;   // 17T #530 front sprocket pitch radius
const BIKE_PEAK_TQ = 80;                     // lb-ft at the crank

// ---------------- Prop ----------------
const PROP_D = 66;                           // in, 3-blade
const PROP_RPM_TARGET = 2600;
const THRUST = 580;                          // lbf static (estimate, minimal_1448.js)
const PROP_LB = 20;
const SOUND = 1125;                          // ft/s
const tip = rpm => Math.PI * PROP_D / 12 * rpm / 60;

// ---------------- Belt: synchronous, pulleys [teeth], pitch mm ----------------
const BELTS = [
  { name: '14MGT 30T / 47T', pitch: 14, zs: 30, zl: 47 },
  { name: '8MGT 45T / 71T', pitch: 8, zs: 45, zl: 71 },
];
const pd = (z, p) => z * p / Math.PI / 25.4;   // pitch diameter, in

const out = { gears: [], belts: [], shaft: [], stand: {} };
for (const [g, r] of Object.entries(GEARS)) {
  const cs = ENGINE_RPM / (PRIMARY * r);
  out.gears.push({ gear: +g, countershaftRpm: Math.round(cs), countershaftTorqueLbFt: Math.round(HP * 5252 / cs),
    beltRatioFor2600: +(cs / PROP_RPM_TARGET).toFixed(3) });
}
// The bike's own worst countershaft pull: peak torque in 1st gear on the 17T sprocket.
const bikePull = BIKE_PEAK_TQ * 12 * PRIMARY * 2.562 / BIKE_SPROCKET_R;

const GEAR = 4;
const csRpm = ENGINE_RPM / (PRIMARY * GEARS[GEAR]);
for (const b of BELTS) {
  const ds = pd(b.zs, b.pitch), dl = pd(b.zl, b.pitch);
  const propRpm = csRpm * b.zs / b.zl;
  const v = Math.PI * ds * csRpm / 12;                // ft/min
  const te = 33000 * HP / v;                          // effective tension, lbf
  out.belts.push({ belt: b.name, smallPD: +ds.toFixed(2), largePD: +dl.toFixed(2), propRpm: Math.round(propRpm),
    tipMach: +(tip(propRpm) / SOUND).toFixed(2), beltFpm: Math.round(v), beltMps: +(v * 0.00508).toFixed(1),
    effTensionLbf: Math.round(te), shaftLoadLbf: Math.round(1.6 * te), bikeChainPullLbf: Math.round(bikePull) });
}

// ---------------- Prop shaft: 4140 pre-hardened, belt pull overhung, prop thrust ----------------
const sel = out.belts[0];
const torqueIn = HP * 5252 / sel.propRpm * 12;
for (const d of [1.25, 1.375, 1.5]) {
  const k = Math.PI * d ** 3;
  const tau = 16 * torqueIn / k;
  const sigma = 32 * (sel.shaftLoadLbf * 2.0) / k + 4 * THRUST / (Math.PI * d * d);   // pulley 2 in outboard of its bearing
  out.shaft.push({ diaIn: d, torsionPsi: Math.round(tau), bendingPsi: Math.round(sigma),
    vonMisesPsi: Math.round(Math.sqrt(sigma ** 2 + 3 * tau ** 2)), factorVs30ksiFatigue: +(30000 / Math.sqrt(sigma ** 2 + 3 * tau ** 2)).toFixed(1) });
}

// ---------------- Stand: thrust at the prop axis, reacted at the feet ----------------
const PROP_Z = 52, BASE = 36;                  // prop axis height above the bottom; stand foot spread fore and aft
out.stand = { thrustLbf: THRUST, propAxisZ: PROP_Z, overturnInLb: THRUST * PROP_Z, footBase: BASE,
  footLoadEachSideLbf: Math.round(THRUST * PROP_Z / BASE / 2), propTorqueLbFt: Math.round(torqueIn / 12) };

// ---------------- Prop-shaft bearings ----------------
// Straddle mount: aft bearing X 8, belt pulley X 13, forward (thrust) bearing X 20, prop at X 5.
// Forward: 4-bolt tapered-roller flange unit (Dodge F4B-E-108R class, C about 7,070 lbf radial dynamic).
// Aft: UCF208-24 ball flange unit (C about 6,542 lbf), radial only.
const XA = 8, XP = 13, XF = 20, XPROP = 5, SPAN = XF - XA;
const beltLoad = sel.shaftLoadLbf;
const PROP_I = 20 / 32.2 * 2.75 ** 2 / 3;          // slug ft^2, blades as rods from the hub
const gyro = PROP_I * (sel.propRpm * Math.PI / 30) * (30 * Math.PI / 180) * 12;   // in-lb at a 30 deg/s yaw rate
const life = (C, P, n) => Math.pow(C / P, 10 / 3) * 1e6 / (60 * n);
const ballLife = (C, P, n) => Math.pow(C / P, 3) * 1e6 / (60 * n);
const frAft = beltLoad * (XF - XP) / SPAN + gyro / SPAN + PROP_LB * (XF - XPROP) / SPAN;
const frFwd = beltLoad * (XP - XA) / SPAN + gyro / SPAN;
out.bearings = { gyroInLb: Math.round(gyro), frAftLbf: Math.round(frAft), frFwdLbf: Math.round(frFwd), cases: [] };
// Tapered roller equivalent load P = 0.4 Fr + 1.5 Fa (generic; confirm with the Dodge catalog method).
for (const [lbl, fa] of [['static thrust (hole shot)', THRUST], ['1.5 x static thrust (design)', 1.5 * THRUST], ['cruise thrust about 320 lbf', 320]]) {
  const P = 0.4 * frFwd + 1.5 * fa;
  out.bearings.cases.push({ case: lbl, thrustLbf: Math.round(fa), fwdEquivLoadLbf: Math.round(P), fwdL10Hours: Math.round(life(7070, P, sel.propRpm)) });
}
out.bearings.aftL10Hours = Math.round(ballLife(6542, frAft, sel.propRpm));
// Forward bearing plate: thrust at the center of a plate spanning 10 in between the stand side frames, 6 in deep.
for (const t of [0.375, 0.5]) {
  const M = 1.5 * THRUST * 10 / 4, S = 6 * t * t / 6;
  out.bearings['plateStressPsi_' + t] = Math.round(M / S);
}

// ---------------- Weight ----------------
const W = { 'Engine with gearbox, oil and coolant': 145, 'Belt, pulleys, prop shaft, bearings, tensioner': 39,
  'Radiator and hoses': 12, 'Exhaust': 10, 'Engine stand (6061 tube and plates)': 41, 'Short beds and doublers': 15 };
out.weight = { ...W, total: Object.values(W).reduce((a, b) => a + b, 0) };
console.log(JSON.stringify(out, null, 1));
