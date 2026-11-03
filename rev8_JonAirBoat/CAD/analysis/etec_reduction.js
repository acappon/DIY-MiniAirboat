// Belt reduction and prop shaft for the minimal 1448 airboat with a Rotax 800R E-TEC snowmobile two-stroke.
// The belt runs straight off the crank PTO (no gearbox, no clutch). Picks the belt and ratio for the prop rpm,
// checks belt speed and pull, the overhung load on the crank, prop-shaft stress and the thrust bearings.
// Inches, pounds, rpm. Every engine figure is to be confirmed on the real engine.
// Usage: node etec_reduction.js
'use strict';

// ---------------- Engine (Rotax 800R E-TEC, 799.5 cc, 82 x 75.7 mm; BRP lists 7,900 rpm maximum) ----------------
const ENGINE_RPM = 7600;                     // continuous operating point, below the 7,900 rpm maximum
const HP = 120;                              // used at the prop shaft (about 75-80% of the rated power; BRP doesn't publish it)

// ---------------- Prop (same Warp Drive 66 in 3-blade as before) ----------------
const PROP_D = 66;
const PROP_RPM_TARGET = 2600;
const THRUST = 497;                          // lbf static at 120 hp (minimal_1448.js thrust rule)
const PROP_LB = 15;
const SOUND = 1125;                          // ft/s
const tip = rpm => Math.PI * PROP_D / 12 * rpm / 60;

// ---------------- Belt: synchronous, one stage, pulleys [teeth], pitch mm ----------------
// Keep the belt at or under about 6,500 ft/min (33 m/s) unless the pulleys are dynamically balanced.
const BELTS = [
  { name: '8MGT 32T / 94T (chosen)', pitch: 8, zs: 32, zl: 94 },
  { name: '8MGT 36T / 106T', pitch: 8, zs: 36, zl: 106 },
  { name: '14MGT 28T / 82T', pitch: 14, zs: 28, zl: 82 },
  { name: '8MGT 32T / 104T (4-blade option, about 2,350 rpm)', pitch: 8, zs: 32, zl: 104 },
];
const pd = (z, p) => z * p / Math.PI / 25.4;   // pitch diameter, in

const out = { ratioFor2600: +(ENGINE_RPM / PROP_RPM_TARGET).toFixed(2), crankTorqueLbFt: Math.round(HP * 5252 / ENGINE_RPM), belts: [], shaft: [] };
for (const b of BELTS) {
  const ds = pd(b.zs, b.pitch), dl = pd(b.zl, b.pitch);
  const propRpm = ENGINE_RPM * b.zs / b.zl;
  const v = Math.PI * ds * ENGINE_RPM / 12;           // ft/min
  const te = 33000 * HP / v;                          // effective tension, lbf
  out.belts.push({ belt: b.name, smallPD: +ds.toFixed(2), largePD: +dl.toFixed(2), propRpm: Math.round(propRpm),
    tipMach: +(tip(propRpm) / SOUND).toFixed(2), beltFpm: Math.round(v), beltMps: +(v * 0.00508).toFixed(1),
    effTensionLbf: Math.round(te), shaftLoadLbf: Math.round(1.6 * te) });
}
const sel = out.belts[0];

// ---------------- Crank PTO: the small pulley overhangs the PTO main bearing ----------------
// The sled's drive clutch already hangs here; the belt pull is new. Moment at the PTO bearing with the pulley
// centre about 2.5 in out. If the service manual can't confirm this, add an outboard support bearing.
out.pto = { overhangIn: 2.5, sideLoadLbf: sel.shaftLoadLbf, momentInLb: Math.round(sel.shaftLoadLbf * 2.5) };

// ---------------- Prop shaft: 4140 pre-hardened, same straddle layout as the bike design ----------------
const torqueIn = HP * 5252 / sel.propRpm * 12;
for (const d of [1.25, 1.375, 1.5]) {
  const k = Math.PI * d ** 3;
  const tau = 16 * torqueIn / k;
  const sigma = 32 * (sel.shaftLoadLbf * 2.0) / k + 4 * THRUST / (Math.PI * d * d);   // conservative: pulley 2 in outboard
  const vm = Math.sqrt(sigma ** 2 + 3 * tau ** 2);
  out.shaft.push({ diaIn: d, vonMisesPsi: Math.round(vm), factorVs30ksiFatigue: +(30000 / vm).toFixed(1) });
}
out.propTorqueLbFt = Math.round(torqueIn / 12);

// ---------------- Prop-shaft bearings (unchanged parts: Dodge F4B-E-108R class forward, UCF208-24 aft) ----------------
const XA = 8, XP = 13, XF = 20, XPROP = 5, SPAN = XF - XA;
const PROP_I = PROP_LB / 32.2 * 2.75 ** 2 / 3;
const gyro = PROP_I * (sel.propRpm * Math.PI / 30) * (30 * Math.PI / 180) * 12;
const life = (C, P, n) => Math.pow(C / P, 10 / 3) * 1e6 / (60 * n);
const ballLife = (C, P, n) => Math.pow(C / P, 3) * 1e6 / (60 * n);
const frAft = sel.shaftLoadLbf * (XF - XP) / SPAN + gyro / SPAN + PROP_LB * (XF - XPROP) / SPAN;
const frFwd = sel.shaftLoadLbf * (XP - XA) / SPAN + gyro / SPAN;
out.bearings = { gyroInLb: Math.round(gyro), frAftLbf: Math.round(frAft), frFwdLbf: Math.round(frFwd),
  fwdL10HoursAtStaticThrust: Math.round(life(7070, 0.4 * frFwd + 1.5 * THRUST, sel.propRpm)),
  aftL10Hours: Math.round(ballLife(6542, frAft, sel.propRpm)) };

// ---------------- Stand loads ----------------
const PROP_Z = 52, BASE = 36;
out.stand = { overturnInLb: THRUST * PROP_Z, footLoadEachSideLbf: Math.round(THRUST * PROP_Z / BASE / 2) };

// ---------------- Fuel (direct-injection two-stroke, about 0.55 lb/hp/h; estimate) ----------------
const gph = hp => +(hp * 0.55 / 6.1).toFixed(1);
out.fuel = { fullPowerGph: gph(HP), cruiseGph60hp: gph(60), minutesOn6galFull: Math.round(6 / gph(HP) * 60), hoursOn6galCruise: +(6 / gph(60)).toFixed(1) };

// ---------------- Weight (engine bare weight unverified) ----------------
const W = { 'Engine, bare (reported, unverified)': 79, 'Tuned pipe and stock silencer': 20, 'Radiator, hoses and coolant': 15,
  'Belt, pulleys, prop shaft, bearings, collars, plates': 39, 'Oil tank and oil, ECU, harness': 10, 'Engine mounts': 5 };
out.weight = { ...W, engineSystem: Object.values(W).reduce((a, b) => a + b, 0), standAndBedsTarget: 45 };
console.log(JSON.stringify(out, null, 1));
