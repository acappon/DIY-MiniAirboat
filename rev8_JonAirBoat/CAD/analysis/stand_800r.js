// First check of the 800R stand in CAD/FeatureScript/Minimal_layout.fs: weight, and hand-calc member
// loads at full power. Run: node stand_800r.js
// Static loads with a simple truss and beam model. It does not cover engine vibration, joints, or the
// gusset and bolt design; the 7 ksi limit is about 14 ksi of 6061-T6 fatigue strength over a hole factor of 2.

const RHO = 0.098; // lb/in^3, 6061
const E = 10e6;    // psi
const LIMIT = 7000;

function tube(b, h, t) {
  const A = b * h - (b - 2 * t) * (h - 2 * t);
  const I = (b * h ** 3 - (b - 2 * t) * (h - 2 * t) ** 3) / 12;
  return { A, I, r: Math.sqrt(I / A), S: I / (h / 2), lbPerIn: A * RHO };
}

const T22 = tube(2, 2, 0.125);
const T15 = tube(1.5, 1.5, 0.125);
const BED = tube(2, 3, 0.125);
const GUSSETS = 4;

const bedTop = 3.125;
const postAft = 57 - bedTop;
const postFwd = 43;
const diag = Math.hypot(19, 56 - bedTop);
const zRail = 14;
const railEnd = 21 + 19 * (56 - zRail) / (56 - bedTop);
const rail = railEnd - 9;
const bed = 35;

const members = [
  ['Aft post 2 x 2 x 1/8', 2, postAft, T22],
  ['Engine rail 2 x 2 x 1/8', 2, rail, T22],
  ['Forward post 1-1/2 x 1-1/2 x 1/8', 2, postFwd, T15],
  ['Diagonal 1-1/2 x 1-1/2 x 1/8', 2, diag, T15],
  ['Top rail 1-1/2 x 1-1/2 x 1/8', 2, 12, T15],
  ['Ties 1-1/2 x 1-1/2 x 1/8', 3, 15, T15],
  ['Bed 2 x 3 x 1/8', 2, bed, BED],
];
let lb = GUSSETS;
for (const [name, n, len, s] of members) {
  const w = n * len * s.lbPerIn;
  lb += w;
  console.log(`${name}: ${n} x ${len.toFixed(1)} in = ${w.toFixed(1)} lb`);
}
console.log(`Gussets (estimate): ${GUSSETS} lb\nStand and beds: ${lb.toFixed(1)} lb against the 45 lb target\n`);

// Loads at full power (Drivetrain_800R_ETEC.md Sections 3, 4, 7), per side frame.
const thrust = 497 / 2;
const belt = 993;
const beltFwd = belt * 5 / 12 / 2;   // pulley at X 13 between bearings at X 8 and X 20
const beltAft = belt * 7 / 12 / 2;
const torqueCouple = 244 * 12 / 17;  // side frames 17 in apart

// Crank pull at X 13, engine mounts assumed at X 17 and X 27: the aft mount takes the most.
const mountUp = belt * (27 - 13) / (27 - 17) / 2;
const span = 12, a = 8, b = span - a; // rail between the posts
const railM = mountUp * a * b / span;

const diagLoad = thrust / (19 / diag) + torqueCouple / ((56 - bedTop) / diag);
const eulerP = (s, L) => Math.PI ** 2 * E * s.I / L ** 2;

const check = (name, load, cap, unit) =>
  console.log(`${name}: ${load.toFixed(0)} ${unit}, limit ${cap.toFixed(0)} ${unit}, factor ${(cap / load).toFixed(1)}`);

check('Engine rail bending', railM / T22.S, LIMIT, 'psi');
check('Diagonal buckling', diagLoad, eulerP(T15, diag), 'lbf');
check('Forward post buckling', beltFwd + torqueCouple, eulerP(T15, postFwd), 'lbf');
check('Aft post buckling', beltAft + torqueCouple, eulerP(T22, postAft), 'lbf');

// Side-to-side natural frequency of the top of the stand, for each bracing stage. Joint fixity is the
// big unknown, so each stage runs half-fixed and rigid joints. Measure it with a tap test on the real stand.
const G = 386;
const hz = (k, lb) => Math.sqrt(k * G / lb) / (2 * Math.PI);
const propRpm = 7600 / 2.94;
const crankAt = (f) => (f * 60 * 2.94).toFixed(0);

// The strap X runs from 36% to 90% of the way down the diagonals, below the cylinders.
const strapLen = Math.hypot(0.54 * 19, 17, 0.54 * (56 - bedTop));
const strapK = E * 0.125 / strapLen * (17 / strapLen) ** 2;  // the strap in tension only
const upperDiag = 0.36 * diag;                               // bends above the X

console.log('\nStand side-to-side (prop once-per-rev is 43 Hz at full power):');
for (const [label, fix] of [['half-fixed joints', 6], ['rigid joints', 12]]) {
  const bare = 2 * fix * E * T22.I / 42 ** 3 + 2 * fix * E * T15.I / diag ** 3;
  const topFrame = 2 * fix * E * T15.I / 12 ** 3;         // carries the aft posts back to the strapped plane
  const upper = 2 * fix * E * T15.I / upperDiag ** 3;
  const braced = 1 / (1 / strapK + 1 / upper + 1 / topFrame) + bare;
  for (const [stage, k, lb] of [['no straps, cage on hull', bare, 60], ['straps, cage strutted on', braced, 93]]) {
    const f = hz(k, lb);
    console.log(`${label}, ${stage}: ${k.toFixed(0)} lb/in, ${f.toFixed(0)} Hz, prop matches at about ${crankAt(f)} engine rpm`);
  }
}
