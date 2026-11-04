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
const zRail = 14.9;              // between the rear bench top (0.2 in below) and the forward hoop (0.25 in above)
const postFwd = 56.75 - zRail;
const diag = Math.hypot(19, 56 - bedTop);
const railEnd = 21 + 19 * (56 - zRail) / (56 - bedTop);
const rail = railEnd - 9;
const shoeAft = 10.75;           // X 2 to 12.75, between the transom and the rear bench
const shoeFwd = 10;              // X 34 to 44, over the rib at X 37.75

const members = [
  ['Aft post 2 x 2 x 1/8', 2, postAft, T22],
  ['Engine rail 2 x 2 x 1/8', 2, rail, T22],
  ['Forward post 1-1/2 x 1-1/2 x 1/8', 2, postFwd, T15],
  ['Diagonal 1-1/2 x 1-1/2 x 1/8', 2, diag, T15],
  ['Top rail 1-1/2 x 1-1/2 x 1/8', 2, 12, T15],
  ['Ties 1-1/2 x 1-1/2 x 1/8', 3, 15, T15],
  ['Aft shoe 2 x 3 x 1/8', 2, shoeAft, BED],
  ['Forward shoe 2 x 3 x 1/8', 2, shoeFwd, BED],
];
let lb = GUSSETS;
for (const [name, n, len, s] of members) {
  const w = n * len * s.lbPerIn;
  lb += w;
  console.log(`${name}: ${n} x ${len.toFixed(1)} in = ${w.toFixed(1)} lb`);
}
console.log(`Gussets (estimate): ${GUSSETS} lb\nStand and shoes: ${lb.toFixed(1)} lb against the 45 lb target\n`);

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

// ---- Hull mounts: side frame as a 2D frame on two feet, with no bed between them ----
// Direct stiffness, X and Z in inches. The aft post is held by the aft shoe; the diagonal is pinned at the forward shoe.
function solveFrame(nodes, bars, fixed, loads) {
  const n = nodes.length * 3, K = Array.from({ length: n }, () => new Float64Array(n));
  const F = new Float64Array(n);
  const local = (L, s) => {
    const a = E * s.A / L, b = E * s.I, k = Array.from({ length: 6 }, () => new Array(6).fill(0));
    k[0][0] = k[3][3] = a; k[0][3] = k[3][0] = -a;
    const c = 12 * b / L ** 3, d = 6 * b / L ** 2, e = 4 * b / L, f = 2 * b / L;
    k[1][1] = k[4][4] = c; k[1][4] = k[4][1] = -c; k[1][2] = k[2][1] = k[1][5] = k[5][1] = d;
    k[4][2] = k[2][4] = k[4][5] = k[5][4] = -d; k[2][2] = k[5][5] = e; k[2][5] = k[5][2] = f;
    return k;
  };
  const mul = (A, B) => A.map((r) => B[0].map((_, j) => r.reduce((t, v, q) => t + v * B[q][j], 0)));
  const flip = (A) => A[0].map((_, i) => A.map((r) => r[i]));
  const info = bars.map(([i, j, s]) => {
    const dx = nodes[j][0] - nodes[i][0], dz = nodes[j][1] - nodes[i][1], L = Math.hypot(dx, dz), c = dx / L, n2 = dz / L;
    const T = [[c, n2, 0, 0, 0, 0], [-n2, c, 0, 0, 0, 0], [0, 0, 1, 0, 0, 0], [0, 0, 0, c, n2, 0], [0, 0, 0, -n2, c, 0], [0, 0, 0, 0, 0, 1]];
    const k = local(L, s), kg = mul(flip(T), mul(k, T));
    const m = [3 * i, 3 * i + 1, 3 * i + 2, 3 * j, 3 * j + 1, 3 * j + 2];
    m.forEach((p, a) => m.forEach((q, b) => { K[p][q] += kg[a][b]; }));
    return { T, k, m, s };
  });
  for (const [i, fx, fz, mz] of loads) { F[3 * i] += fx; F[3 * i + 1] += fz; F[3 * i + 2] += mz; }
  const free = [...Array(n).keys()].filter((d) => !fixed.includes(d));
  const A = free.map((p) => [...free.map((q) => K[p][q]), F[p]]);
  for (let i = 0; i < A.length; i++) {
    let p = i;
    for (let r = i + 1; r < A.length; r++) if (Math.abs(A[r][i]) > Math.abs(A[p][i])) p = r;
    [A[i], A[p]] = [A[p], A[i]];
    for (let r = 0; r < A.length; r++) {
      if (r === i) continue;
      const f = A[r][i] / A[i][i];
      for (let c = i; c <= A.length; c++) A[r][c] -= f * A[i][c];
    }
  }
  const u = new Float64Array(n);
  free.forEach((d, i) => { u[d] = A[i][A.length] / A[i][i]; });
  const R = (d) => K[d].reduce((t, v, q) => t + v * u[q], 0) - F[d];
  const stress = info.map(({ T, k, m, s }) => {
    const ul = T.map((r) => r.reduce((t, v, a) => t + v * u[m[a]], 0));
    const f = k.map((r) => r.reduce((t, v, a) => t + v * ul[a], 0));
    return { axial: f[3], sig: Math.abs(f[3]) / s.A + Math.max(Math.abs(f[2]), Math.abs(f[5])) / s.S };
  });
  return { R, stress };
}

const NODES = [[9, bedTop], [9, zRail], [9, 56], [21, zRail], [21, 56], [railEnd, zRail], [40, bedTop], [17, zRail], [27, zRail]];
const BARS = [[0, 1, T22], [1, 2, T22], [2, 4, T15], [1, 7, T22], [7, 3, T22], [3, 8, T22], [8, 5, T22], [3, 4, T15], [4, 5, T15], [5, 6, T15]];
const BAR_NAMES = ['Aft post below the rail', 'Aft post', 'Top rail', 'Rail X 9-17', 'Rail X 17-21', 'Rail X 21-27', 'Rail forward of X 27', 'Forward post', 'Diagonal', 'Diagonal below the rail'];
const FWD_PIN = [18, 19];
const engineLb = 168 + 15, standLb = 45;
const mountAft = belt * (27 - 13) / (27 - 17) / 2;   // crank pull at the engine mounts, per side: up at X 17, down at X 27
const mountFwd = belt * (17 - 13) / (27 - 17) / 2;
const propArm = 56 - 52;                             // thrust line is 4 in below the top nodes
const powerCase = (sign) => [
  [4, thrust, -beltFwd + sign * torqueCouple / 2, thrust * propArm],
  [2, 0, -beltAft + sign * torqueCouple / 2, 0],
  [7, 0, mountAft - engineLb / 4 - sign * torqueCouple / 2, 0],
  [8, 0, -mountFwd - engineLb / 4 - sign * torqueCouple / 2, 0],
];
const CASES = [
  ['Full power, torque lifting this side', powerCase(1)],
  ['Full power, torque pressing this side', powerCase(-1)],
  ['3 g slam, engine idling', [[7, 0, -3 * (engineLb + standLb) / 4, 0], [8, 0, -3 * (engineLb + standLb) / 4, 0]]],
];

console.log('\nHull mounts, per side, frame with no bed between the feet:');
let maxUp = 0, maxFwdShear = 0, maxAftShear = 0;
for (const [foot, held] of [['aft foot rigid', [0, 1, 2]], ['aft foot pinned', [0, 1]]]) {
  for (const [name, loads] of CASES) {
    const { R, stress } = solveFrame(NODES, BARS, [...held, ...FWD_PIN], loads);
    const worst = stress.reduce((b, s, i) => (s.sig > b.sig ? { ...s, i } : b), { sig: 0 });
    console.log(`${foot}, ${name}: aft foot ${R(0).toFixed(0)} lbf X, ${R(1).toFixed(0)} lbf Z; forward foot ${R(18).toFixed(0)} lbf X, ${R(19).toFixed(0)} lbf Z; worst ${BAR_NAMES[worst.i]} ${worst.sig.toFixed(0)} psi, factor ${(LIMIT / worst.sig).toFixed(1)}`);
    maxUp = Math.max(maxUp, -R(1));
    maxFwdShear = Math.max(maxFwdShear, Math.abs(R(18)));
    maxAftShear = Math.max(maxAftShear, Math.abs(R(0)));
  }
}

// Aft shoe as a beam from the transom clip (X 2) to the bench clip (X 12.75), post at X 9.
const a0 = 9 - 2, b0 = 12.75 - 9, L0 = a0 + b0;
const clipTransom = maxUp * b0 / L0, clipBench = maxUp * a0 / L0;
console.log(`\nAft shoe: ${maxUp.toFixed(0)} lbf up at the post, ${clipTransom.toFixed(0)} lbf at the transom clip, ${clipBench.toFixed(0)} lbf at the bench clip`);
check('Aft shoe bending', maxUp * a0 * b0 / L0 / BED.S, LIMIT, 'psi');
const BOLT_SHEAR = 950;   // 1/4-20 316 single shear about 1,900 lbf ultimate, over 2
check('Forward shoe, six 1/4-20 in shear', maxFwdShear / 6, BOLT_SHEAR, 'lbf per bolt');
check('Aft shoe, six 1/4-20 in shear', maxAftShear / 6, BOLT_SHEAR, 'lbf per bolt');
check('Two 1/4-20 at the bench clip', clipBench / 2, BOLT_SHEAR, 'lbf per bolt');

// Weight of the fastening parts, lb. SHEET is 1/8 in 5052 per square inch.
const SHEET = 0.125 * 0.097, BOLT = 0.035, THROUGH = 0.07; // THROUGH: 3-1/2 in bolt through a shoe, with its crush sleeve
const fastening = [
  ['Aft shoe doublers, 1/8 x 5 x 10.5, 2', 2 * 5 * 10.5 * SHEET],
  ['Forward shoe doublers, 1/8 x 6 x 7.2 total, 2', 2 * 6 * 7.2 * SHEET],
  ['Transom and bench clips, 2 x 2 x 1/8 angle x 3 in, 4', 4 * 3 * (2 * 0.125 + 1.875 * 0.125) * 0.098],
  ['Cage foot pads and backing, bench, 4 x 5 x 1/8, 2 pairs', 4 * 4 * 5 * 0.125 * 0.098],
  ['Cage foot pads and backing, corner cap, 3 x 4 x 1/8, 2 pairs', 4 * 3 * 4 * 0.125 * 0.098],
  ['1/4-20 bolt sets: 24 through the shoes and bottom, 16 at the clips, 16 at the cage feet', 24 * THROUGH + 32 * BOLT],
  ['Sealant', 0.3],
];
let fastLb = 0;
for (const [name, w] of fastening) { fastLb += w; console.log(`${name}: ${w.toFixed(2)} lb`); }
console.log(`Hull fastening total: ${fastLb.toFixed(1)} lb`);
