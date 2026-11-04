// First check of the steering and throttle linkage in CAD/FeatureScript/Minimal_controls.fs.
// Run: node controls_800r.js
// Static loads. It does not cover cable friction, which is added as a round figure, or vibration.

const RHO = 0.00238;       // slug/ft^3
const THRUST = 497;        // lbf at full power
const DISC_FT2 = Math.PI * (33 / 12) ** 2;
const LIMIT = 7000;        // psi, the same limit as stand_800r.js

const induced = Math.sqrt(THRUST / (2 * RHO * DISC_FT2));
const wake = 2 * induced;  // ft/s
const q = 0.5 * RHO * wake * wake;
const chord = 8, span = 30;
const area = chord * span / 144;
const CL_MAX = 1.2;
const side = CL_MAX * q * area;
console.log(`Prop wake ${wake.toFixed(0)} ft/s (${(wake * 0.682).toFixed(0)} mph), ${q.toFixed(1)} psf`);
console.log(`Rudder ${area.toFixed(2)} ft^2, ${side.toFixed(0)} lbf each at CL ${CL_MAX}`);

// Hinge at 25% chord. Near the stall the center of pressure moves back to about 40%.
const hingeArm = 0.15 * chord;
const hinge = 2 * side * hingeArm;   // in-lb, both rudders
const horn = 4.3;                    // cable arm, in
const lever = 8;                     // cable point above the stick pivot, in
const grip = 26;                     // grip above the pivot, in
const cable = hinge / horn;
const FRICTION = 5;                  // lbf at the grip, round figure for 14 ft of cable
console.log(`Hinge moment ${hinge.toFixed(0)} in-lb, cable ${cable.toFixed(0)} lbf, grip ${(cable * lever / grip + FRICTION).toFixed(0)} lbf with friction`);

const deg = (r) => r * 180 / Math.PI;
const rudderMax = 25;
const travel = horn * Math.sin(rudderMax * Math.PI / 180);
const stickDeg = deg(Math.asin(travel / lever));
console.log(`At ${rudderMax} deg of rudder: cable travel +/-${travel.toFixed(2)} in, stick +/-${stickDeg.toFixed(0)} deg, grip +/-${(grip * Math.sin(stickDeg * Math.PI / 180)).toFixed(1)} in`);

// Shaft between its bearings at Z 18.7 and Z 52, with the full side load at mid span.
const tube = (d, t) => {
  const I = Math.PI / 64 * (d ** 4 - (d - 2 * t) ** 4);
  return { S: I / (d / 2), lbPerIn: Math.PI * (d - t) * t * 0.098 };
};
const shaftSpan = 52 - 18.7;
for (const [d, t] of [[0.75, 0.065], [1, 0.065], [1, 0.125]]) {
  const s = tube(d, t);
  const psi = side * shaftSpan / 4 / s.S;
  console.log(`Shaft ${d} x ${t}: ${psi.toFixed(0)} psi, limit ${LIMIT}, factor ${(LIMIT / psi).toFixed(1)}, ${(s.lbPerIn * 34.7).toFixed(2)} lb each`);
}

// Pedal: the crank pulls the cable by its arm through the pedal's swing.
const crank = 3.4, pedalDeg = 20;
console.log(`Pedal ${pedalDeg} deg of swing gives ${(crank * Math.sin(pedalDeg * Math.PI / 180)).toFixed(2)} in of cable travel`);

const weights = [
  ['Rudders, 8 x 30 in, two skins of 0.040 with ribs, 2', 4.4],
  ['Rudder shafts 1 x 0.125, 2', 2 * tube(1, 0.125).lbPerIn * 34.7],
  ['Upper arms 1 x 0.065, 23 in, 2', 2 * tube(1, 0.065).lbPerIn * 23],
  ['Lower brackets, bearings, horns, cable arm', 1.2],
  ['Tie bar, rod ends, cable end rod and clamp', 0.6],
];
const stick = [
  ['Stick, grip, base and cheeks', 1.2],
  ['Rudder cable, about 14 ft at 0.15 lb/ft', 2.1],
  ['Pedal, crank, hinge rod, base, stop, spring', 0.9],
  ['Heel rests', 0.3],
  ['Throttle cable, about 16 ft at 0.1 lb/ft', 1.6],
];
for (const [label, list, allowance] of [['Rudders and linkage', weights, 12], ['Stick, pedal and cables', stick, 10]]) {
  const lb = list.reduce((t, [, w]) => t + w, 0);
  console.log(`${label}: ${lb.toFixed(1)} lb itemized against the ${allowance} lb allowance`);
}
