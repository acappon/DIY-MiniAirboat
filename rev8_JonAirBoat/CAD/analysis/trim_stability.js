// Trim, stability and flotation analysis for the DIY Jon-Air airboat (Lowe 1448 hull).
// Frame: X forward from the transom, Y + to port, Z up from the hull bottom. Inches and pounds
// unless noted. Every weight and position is an estimate; see the weight table in the report.
// Usage: node trim_stability.js > results.json
'use strict';

// ---------------- Hull geometry (from the Onshape hull reference) ----------------
const FLAT_END = 101;        // flat bottom ends here; bow rise to the bow corner
const BOW_X = 158, BOW_Z = 9.3;
const FLARE = 0.515;         // side flare: half-width grows 0.515 in per inch of height
const GUNWALE_Z = 19;
const TRANSOM_TOP = 15;      // Lowe L1448M transom height (lowest point the stern can ship water)
const HW0 = [[0, 24.03], [30, 24.2], [60, 24.38], [78, 24.47], [90, 24.44], [100, 24.49]];
const interp = (t, x) => { if (x <= t[0][0]) return t[0][1]; for (let i = 1; i < t.length; i++) if (x <= t[i][0]) return t[i-1][1] + (x - t[i-1][0]) / (t[i][0] - t[i-1][0]) * (t[i][1] - t[i-1][1]); return t[t.length-1][1]; };
// Assumed bow rise and bottom taper forward of the flat (the hull mesh is incomplete there).
const bottomZ = x => x <= FLAT_END ? 0 : BOW_Z * Math.pow((x - FLAT_END) / (BOW_X - FLAT_END), 1.5);
const halfW = x => x <= FLAT_END ? interp(HW0, x) : 24.49 - 6 * Math.pow((x - FLAT_END) / (BOW_X - FLAT_END), 2);

const RHO_FW = 62.4 / 1728;  // lb/in^3 fresh water

// Immersed volume, LCB, VCB, waterplane inertia for a waterline h(x) = T0 + x*tan(trim)
// (trim > 0 = bow down, i.e. waterline rises toward the bow ... we use stern-down positive below)
function hydro(Tst, trimDeg) {
  // Tst = draft at transom (in); trimDeg > 0 = stern down (bow up)
  const t = Math.tan(trimDeg * Math.PI / 180);
  let V = 0, Mx = 0, Mz = 0, IT = 0, IL = 0, Awp = 0, MxWp = 0;
  const dx = 0.25;
  for (let x = 0; x <= BOW_X; x += dx) {
    const h = Tst - x * t;            // water height above baseline at x
    const zb = bottomZ(x);
    const d = h - zb;
    if (d <= 0) continue;
    const hw = halfW(x);
    const A = 2 * hw * d + FLARE * d * d;
    const zc = zb + (hw * d * d + (2 / 3) * FLARE * d * d * d) / A; // centroid of the flared section above zb
    V += A * dx; Mx += A * x * dx; Mz += A * zc * dx;
    const hwl = hw + FLARE * d;       // half-beam at the waterline
    IT += (2 / 3) * hwl ** 3 * dx;
    Awp += 2 * hwl * dx; MxWp += 2 * hwl * x * dx;
  }
  const LCF = MxWp / Awp;
  for (let x = 0; x <= BOW_X; x += dx) {
    const h = Tst - x * t, d = h - bottomZ(x); if (d <= 0) continue;
    IL += 2 * (halfW(x) + FLARE * d) * (x - LCF) ** 2 * dx;
  }
  return { V, LCB: Mx / V, VCB: Mz / V, IT, IL, Awp, LCF };
}

// Solve draft at transom and trim so that buoyancy = W and LCB = LCG.
function floatAt(W, LCG) {
  const Vreq = W / RHO_FW;
  let trim = 0, Tst = 4;
  for (let it = 0; it < 400; it++) {
    // draft for this trim
    let lo = 0, hi = 20;
    for (let k = 0; k < 50; k++) { const m = (lo + hi) / 2; hydro(m, trim).V < Vreq ? lo = m : hi = m; }
    Tst = (lo + hi) / 2;
    const h = hydro(Tst, trim);
    const err = h.LCB - LCG;          // LCB forward of LCG -> need more stern-down
    if (Math.abs(err) < 0.01) break;
    trim += err * 0.05;
  }
  const h = hydro(Tst, trim);
  const tt = Math.tan(trim * Math.PI / 180);
  return { Tst, trim, h, draftAt: x => Tst - x * tt - bottomZ(x) };
}

// ---------------- Weights ----------------
// [name, weight lb, X, Y, Z, group, submerged factor (fraction of weight that remains when swamped)]
const AL = 1 - 62.4 / 169, ST = 1 - 62.4 / 490;
function baseItems(proto) {
  const B = proto === 'B';
  return [
    ['Hull, Lowe 1448 (middle bench removed)', 248, 62, 0, 7, 'Hull', AL],
    ['UHMW bottom strips (1/4 in, 8 pcs)', 55, 62, 0, -0.1, 'Hull armor', 1 - 62.4 / 58.7],
    ['Chine angles, bow skin, fasteners', 31, 80, 0, 4, 'Hull armor', AL],
    ['Engine beds, doublers, mount plates, nuts', 50, 32, 0, 4, 'Mounts', AL],
    ['Cage supports (posts, knees, plates)', 15, 8, 0, 12, 'Mounts', AL],
    [B ? 'Engine G13B DOHC, dressed' : 'Engine G13 SOHC, dressed', B ? 195 : 185, 38, 0.5, 18, 'Drivetrain', 0.80],
    ['Redrive, belt, pulleys, adapter', 55, 18, 0, 34, 'Drivetrain', ST],
    ['A-frame, braces, peak plates (steel)', 100, 18, 0, 35, 'Drivetrain', ST],
    ['Prop shaft, hub, bearings', 15, 12, 0, 53, 'Drivetrain', ST],
    [B ? 'Prop, 64 in 4-blade' : 'Prop, 64 in 3-blade', B ? 27 : 20, 5, 0, 53, 'Drivetrain', 0.4],
    ['Radiator, coolant, hoses', 25, 17.5, 0, 66, 'Drivetrain', 0.3],
    ['Exhaust', 20, 26, 9, 22, 'Drivetrain', ST],
    ['Prop cage (steel EMT, mesh)', 85, 5, 0, 52, 'Cage', ST],
    ['Rudders, horns, rails, bellcrank', 25, -7, 0, 45, 'Cage', AL],
    ['Steering stick and pushrods', 15, 75, 12, 14, 'Controls', AL],
    ['Battery, Group 31 LiFePO4 dual-purpose (~27 lb) + alternator protector', 28, 104.5, 6.85, 5.6, 'Electrical', 0.4],
    ['Dry box, power hub, fuses, wiring (CAD 2026-11-02)', 19.1, 104.5, 1, 7, 'Electrical', 0.3],
    ['Interior mount rails 1x2x1/8 6061 (CAD)', 11.9, 88, -4, 0.5, 'Seating', AL],
    ['Lights: LED bar, running lights, searchlight', 10, 30, 0, 80, 'Electrical', 0.5],
    ['Driver bucket seat and tracks (CAD)', 15, 104, 0, 18, 'Seating', 0.2],
    ['Storage box / thwart, foam, lid, cushion (CAD)', 45.7, 71, 0, 9, 'Seating', 0.1],
    ['Backrest / grab bar, stays, pad (CAD)', 7.5, 55, 0, 18, 'Seating', AL],
    ['Console: table, dash, instruments, frame (CAD)', 22.9, 112, -22, 22, 'Controls', 0.5],
    ['Trim tab (starboard) and turnbuckle', 4, -2, -19, 1, 'Hull armor', AL],
    ['Safety gear in storage box (PFDs, anchor, extinguisher, lines)', 25, 64, 0, 8, 'Gear', 0.0],
  ];
}
const FUEL_TANK = ['Fuel tank 6.5 gal + fuel', 0, 135, 0, 6, 'Fuel', -0.2]; // weight set per case
const DRIVER = (dx = 0) => ['Driver (200 lb)', 200, 104 + dx, 0, 26, 'Crew', 0];
const PASS = (y) => ['Passenger (200 lb) on box', 200, 69, y, 27, 'Crew', 0];

function massProps(items) {
  let W = 0, mx = 0, my = 0, mz = 0;
  for (const [, w, x, y, z] of items) { W += w; mx += w * x; my += w * y; mz += w * z; }
  return { W, LCG: mx / W, TCG: my / W, VCG: mz / W };
}

// ---------------- Planing (Savitsky 1964, flat bottom, with off-CG thrust and a flap) ----------------
const G = 32.174, RHO = 1.94, NU = 1.22e-5;
const B_FT = 48.8 / 12;
const THRUST_Z = 53.2;       // prop shaft height (in)
const AIR_Z = 40;            // centre of air drag (cage, crew) height (in)
function savitsky({ W, LCG, VCG }, Vmph, opts = {}) {
  const V = Vmph * 1.46667, b = B_FT, q = 0.5 * RHO * V * V;
  const Cv = V / Math.sqrt(G * b);
  const lcg = LCG / 12, vcg = VCG / 12;
  const CdA = opts.CdA ?? 12;
  const Dair = 0.5 * 0.00238 * V * V * CdA;
  const tab = opts.tabDeg || 0;
  const Lf = 0.046 * tab * (5 / 12) * (8 / 12) * q;   // Savitsky & Brown flap lift (lb), 8 x 5 in tab
  const xTab = -3 / 12;                                // tab lift acts ~3 in aft of the transom
  function state(tau) {
    const tr = tau * Math.PI / 180;
    const CL = (W - Lf) / (q * b * b);
    let lo = 0.02, hi = 30;
    for (let k = 0; k < 80; k++) { const m = (lo + hi) / 2; (Math.pow(tau, 1.1) * (0.012 * Math.sqrt(m) + 0.0055 * Math.pow(m, 2.5) / (Cv * Cv)) < CL) ? lo = m : hi = m; }
    const lam = (lo + hi) / 2;
    const lp = (0.75 - 1 / (5.21 * Cv * Cv / (lam * lam) + 2.39)) * lam * b;
    const Vm = V * Math.sqrt(Math.max(0.01, 1 - 0.012 * Math.sqrt(lam) * Math.pow(tau, 1.1) / (lam * Math.cos(tr))));
    const Rn = Vm * lam * b / NU;
    const Cf = 0.075 / Math.pow(Math.log10(Rn) - 2, 2) + 0.0004;
    const Df = q * (Vm / V) ** 2 * lam * b * b * Cf;
    const N = (W - Lf + Df * Math.sin(tr)) / Math.cos(tr);
    const Dtab = 0.0052 * Lf * (tau + tab);
    const R = N * Math.sin(tr) + Df * Math.cos(tr) + Dair + Dtab;
    const h = (THRUST_Z / 12) - vcg, ha = (AIR_Z / 12) - vcg;
    // moments about CG, bow-down positive
    const M = N * vcg * Math.sin(tr) - (lp - lcg) * N * Math.cos(tr)
            + Df * vcg * Math.cos(tr) + (lp - lcg) * Df * Math.sin(tr)
            + R * h - Dair * ha + Lf * (lcg - xTab);
    return { tau, lam, wetLen: lam * b * 12, lp: lp * 12, R, Df, Dair, Lf, Dtab, M, Cv };
  }
  let lo = 0.3, hi = 20;
  for (let k = 0; k < 80; k++) { const m = (lo + hi) / 2; state(m).M < 0 ? lo = m : hi = m; }
  return state((lo + hi) / 2);
}

const PROTOS = {
  A: { name: 'Prototype A (G13 + 3-blade)', hp: 70, T0: 330, Vp: 85, rpm: 2400 },
  B: { name: 'Prototype B (G13B + 4-blade)', hp: 100, T0: 440, Vp: 85, rpm: 2300 },
};
const thrustAt = (p, mph) => Math.max(0, p.T0 * (1 - mph / p.Vp));

function topSpeed(mp, p, opts) {
  let best = null;
  for (let v = 12; v <= 70; v += 0.25) {
    const s = savitsky(mp, v, opts);
    if (thrustAt(p, v) < s.R) return { mph: v, s };
    best = { mph: v, s };
  }
  return best;
}

// ---------------- Cases ----------------
const fuelLb = f => 7 + 6.5 * 6.1 * f;   // tank + gasoline at fraction f
function buildCase(proto, { pass = [], fuel = 1, seat = 0, fuelX = 135 } = {}) {
  const items = baseItems(proto).slice();
  items.push([FUEL_TANK[0], fuelLb(fuel), fuelX, FUEL_TANK[3], FUEL_TANK[4], FUEL_TANK[5], FUEL_TANK[6]]);
  items.push(DRIVER(seat));
  for (const y of pass) items.push(PASS(y));
  return items;
}

const out = { cases: [], sweep: [], tab: [], roll: [], flotation: {} };
const CASES = [
  ['A1', 'A', 'Driver solo, full fuel', {}],
  ['A1e', 'A', 'Driver solo, 1/4 fuel', { fuel: 0.25 }],
  ['A2', 'A', 'Driver + 1 passenger (box centre)', { pass: [0] }],
  ['A3', 'A', 'Driver + 2 passengers (box, side by side)', { pass: [12, -12] }],
  ['B1', 'B', 'Driver solo, full fuel', {}],
  ['B2', 'B', 'Driver + 1 passenger', { pass: [0] }],
  ['B3', 'B', 'Driver + 2 passengers', { pass: [12, -12] }],
];
for (const [id, proto, label, o] of CASES) {
  const items = buildCase(proto, o);
  const mp = massProps(items);
  const fl = floatAt(mp.W, mp.LCG);
  const GM = fl.h.VCB + fl.h.IT / fl.h.V - mp.VCG;
  const GML = fl.h.VCB + fl.h.IL / fl.h.V - mp.VCG;
  const nPass = (o.pass || []).length;
  const opts = { CdA: 12 + 3 * nPass };
  const p = PROTOS[proto];
  const ts = topSpeed(mp, p, opts);
  const cruise = savitsky(mp, 25, opts);
  out.cases.push({ id, proto, label, ...mp,
    staticTrim: fl.trim, draftTransom: fl.Tst, draftFlatEnd: fl.draftAt(FLAT_END),
    transomFreeboard: TRANSOM_TOP - fl.Tst, GM, heelPerInLb: 180 / Math.PI / (mp.W * GM),
    GML, mctPerDeg: mp.W * GML * Math.PI / 180,
    topMph: ts.mph, topTrim: ts.s.tau, topWet: ts.s.wetLen, topR: ts.s.R,
    cruiseTrim: cruise.tau, cruiseWet: cruise.wetLen, cruiseR: cruise.R });
}

// LCG sweep for the solo case: running trim and resistance at 35 mph and at 25 mph
{
  const mp = massProps(buildCase('A', {}));
  for (let lcg = 30; lcg <= 80; lcg += 2.5) {
    const s35 = savitsky({ ...mp, LCG: lcg }, 35), s25 = savitsky({ ...mp, LCG: lcg }, 25);
    out.sweep.push({ lcg, trim35: s35.tau, R35: s35.R, wet35: s35.wetLen, trim25: s25.tau, R25: s25.R, wet25: s25.wetLen });
  }
}

// Trim tab effect for each case at 35 mph (A) / top speed region
for (const c of ['A1', 'A2', 'A3']) {
  const o = CASES.find(k => k[0] === c)[3];
  const mp = massProps(buildCase('A', o));
  const CdA = 12 + 3 * (o.pass || []).length;
  for (const tab of [0, 2, 4, 6, 8, 10]) {
    const s = savitsky(mp, 30, { CdA, tabDeg: tab });
    out.tab.push({ case: c, tab, trim: s.tau, R: s.R, Lf: s.Lf, rollInLb: s.Lf * 19 });
  }
}

// Layout variants for the solo and passenger cases: trim levers available to the crew
out.variants = [];
for (const [id, proto, label, o] of [
  ['V0', 'A', 'Solo, as specified (fuel front, seat mid)', {}],
  ['V1', 'A', 'Solo, seat 3 in aft', { seat: -3 }],
  ['V2', 'A', 'Solo, fuel tank on floor beside engine (X 45)', { fuelX: 45 }],
  ['V3', 'A', 'Solo, fuel beside engine + seat 3 in aft', { fuelX: 45, seat: -3 }],
  ['V4', 'A', '1 passenger, seat 3 in fwd', { pass: [0], seat: 3 }],
  ['V5', 'A', '2 passengers, seat 3 in fwd', { pass: [12, -12], seat: 3 }],
  ['V6', 'B', 'B solo, fuel beside engine + seat 3 in aft', { fuelX: 45, seat: -3 }],
]) {
  const mp = massProps(buildCase(proto, o));
  const CdA = 12 + 3 * (o.pass || []).length;
  const s30 = savitsky(mp, 30, { CdA });
  const ts = topSpeed(mp, PROTOS[proto], { CdA });
  const fl = floatAt(mp.W, mp.LCG);
  out.variants.push({ id, label, LCG: mp.LCG, staticTrim: fl.trim, transomFreeboard: TRANSOM_TOP - fl.Tst, trim30: s30.tau, R30: s30.R, wet30: s30.wetLen, top: ts.mph, topTrim: ts.s.tau });
}

// Roll balance: static TCG moment, prop torque at full power, tab roll moment
for (const pk of ['A', 'B']) {
  const p = PROTOS[pk];
  const Q = p.hp * 5252 / p.rpm * 12;   // in-lb; prop CW seen looking aft -> reaction rolls starboard side down
  out.roll.push({ proto: pk, torqueInLb: Q, torqueFtLb: Q / 12,
    tabDegToCancelAt35: Q / (0.046 * (5 / 12) * (8 / 12) * 0.5 * RHO * (35 * 1.46667) ** 2 * 19) });
}

// Swamped flotation: weight that must be supported when the hull is full of water (crew float in PFDs)
{
  const items = buildCase('B', { fuel: 1 });    // B is the heavier prototype
  let sub = 0, mx = 0;
  const rows = [];
  for (const [n, w, x, , , g, f] of items) {
    if (g === 'Crew') continue;
    const s = w * f; sub += s; mx += s * x; rows.push([n, w, f, s]);
  }
  const need = sub * 1.2;                       // 20% margin
  const foamPerFt3 = 62.4 - 2;                  // closed-cell foam, 2 lb/ft^3
  out.flotation = { submergedLb: sub, submergedLCG: mx / sub, requiredBuoyancyLb: need, foamFt3: need / foamPerFt3, rows };
}
console.log(JSON.stringify(out, null, 1));
