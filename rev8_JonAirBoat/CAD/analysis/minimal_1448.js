// Minimal solo-first airboat on the Lowe 1448 (study/minimal-1448): equipment weight, payload under the
// capacity plate, dock trim, top speed and the thrust margin at the hump, for two light engines.
// Hull hydrostatics and Savitsky planing are copied from compare_hulls.js on the rev8 branches (1448 entry).
// Usage: node minimal_1448.js
'use strict';

const HULLS = {
  '1448': { name: 'Lowe 1448 (reference)', bottom: 48.8, beam: 68.4, side: 19, flatEnd: 101, bowX: 158, bowZ: 9.3,
            hullLb: 248, hullX: 62, transom: 15, plate: 825 },
  '1652': { name: 'Lowe L1652MT', bottom: 52, beam: 75, side: 21, flatEnd: 121, bowX: 182, bowZ: 10.5,
            hullLb: 438, hullX: 72, transom: 20, plate: 1216 },
  '1860': { name: '1860 (Lowe Roughneck / Tracker Grizzly class)', bottom: 60, beam: 84, side: 22, flatEnd: 140, bowX: 208, bowZ: 11.5,
            hullLb: 640, hullX: 83, transom: 20, plate: 1550 },
};
for (const h of Object.values(HULLS)) h.flare = (h.beam - h.bottom) / 2 / h.side;

const RHO_IN = 62.4 / 1728;
const bottomZ = (h, x) => x <= h.flatEnd ? 0 : h.bowZ * Math.pow((x - h.flatEnd) / (h.bowX - h.flatEnd), 1.5);
const halfW = (h, x) => x <= h.flatEnd ? h.bottom / 2 : h.bottom / 2 - 6 * Math.pow((x - h.flatEnd) / (h.bowX - h.flatEnd), 2);

function hydro(h, Tst, trimDeg) {
  const t = Math.tan(trimDeg * Math.PI / 180), dx = 0.5;
  let V = 0, Mx = 0, Mz = 0, IT = 0;
  for (let x = 0; x <= h.bowX; x += dx) {
    const zb = bottomZ(h, x), d = Tst - x * t - zb; if (d <= 0) continue;
    const hw = halfW(h, x), A = 2 * hw * d + h.flare * d * d;
    V += A * dx; Mx += A * x * dx; Mz += (zb * A + hw * d * d + (2 / 3) * h.flare * d ** 3) * dx;
    IT += (2 / 3) * (hw + h.flare * d) ** 3 * dx;
  }
  return { V, LCB: Mx / V, VCB: Mz / V, IT };
}
function floatAt(h, W, LCG) {
  const Vreq = W / RHO_IN; let trim = 0, Tst = 4;
  for (let it = 0; it < 400; it++) {
    let lo = 0, hi = 25;
    for (let k = 0; k < 45; k++) { const m = (lo + hi) / 2; hydro(h, m, trim).V < Vreq ? lo = m : hi = m; }
    Tst = (lo + hi) / 2;
    const err = hydro(h, Tst, trim).LCB - LCG; if (Math.abs(err) < 0.02) break; trim += err * 0.05;
  }
  return { Tst, trim, hy: hydro(h, Tst, trim) };
}

function mass(items) {
  let W = 0, mx = 0, my = 0, mz = 0;
  for (const [, w, x, y, z] of items) { W += w; mx += w * x; my += w * y; mz += w * z; }
  return { W, LCG: mx / W, TCG: my / W, VCG: mz / W };
}

// Savitsky planing with off-CG thrust (see trim_stability.js for the derivation of the moment terms).
const G = 32.174, RHO = 1.94, NU = 1.22e-5, THRUST_Z = 53.2, AIR_Z = 40;
function savitsky(h, { W, LCG, VCG }, mph, CdA) {
  const V = mph * 1.46667, b = h.bottom / 12, q = 0.5 * RHO * V * V, Cv = V / Math.sqrt(G * b);
  const lcg = LCG / 12, vcg = VCG / 12, Dair = 0.5 * 0.00238 * V * V * CdA;
  function st(tau) {
    const tr = tau * Math.PI / 180, CL = W / (q * b * b);
    let lo = 0.02, hi = 40;
    for (let k = 0; k < 80; k++) { const m = (lo + hi) / 2; (Math.pow(tau, 1.1) * (0.012 * Math.sqrt(m) + 0.0055 * Math.pow(m, 2.5) / (Cv * Cv)) < CL) ? lo = m : hi = m; }
    const lam = (lo + hi) / 2;
    const lp = (0.75 - 1 / (5.21 * Cv * Cv / (lam * lam) + 2.39)) * lam * b;
    const Vm = V * Math.sqrt(Math.max(0.01, 1 - 0.012 * Math.sqrt(lam) * Math.pow(tau, 1.1) / (lam * Math.cos(tr))));
    const Cf = 0.075 / Math.pow(Math.log10(Vm * lam * b / NU) - 2, 2) + 0.0004;
    const Df = 0.5 * RHO * Vm * Vm * lam * b * b * Cf;
    const N = (W + Df * Math.sin(tr)) / Math.cos(tr);
    const R = N * Math.sin(tr) + Df * Math.cos(tr) + Dair;
    const M = N * vcg * Math.sin(tr) - (lp - lcg) * N * Math.cos(tr) + Df * vcg * Math.cos(tr) + (lp - lcg) * Df * Math.sin(tr)
            + R * (THRUST_Z / 12 - vcg) - Dair * (AIR_Z / 12 - vcg);
    return { tau, wet: lam * b * 12, R, M };
  }
  let lo = 0.3, hi = 20;
  for (let k = 0; k < 80; k++) { const m = (lo + hi) / 2; st(m).M < 0 ? lo = m : hi = m; }
  return st((lo + hi) / 2);
}
const PROTO = { A: { T0: 330, Vp: 85 }, B: { T0: 440, Vp: 85 } };
function top(h, mp, p, CdA) {
  let last = null;
  for (let v = 10; v <= 70; v += 0.25) {
    const s = savitsky(h, mp, v, CdA);
    if (p.T0 * (1 - v / p.Vp) < s.R) return { mph: v, s, planing: s.wet <= h.flatEnd && v >= 15 };
    last = { mph: v, s, planing: true };
  }
  return last;
}
// Hump check: is there thrust to spare at 12-16 mph (the hard part of getting on plane)?
function humpMargin(h, mp, p, CdA) {
  let minMargin = 1e9;
  for (let v = 12; v <= 16; v += 1) { const s = savitsky(h, mp, v, CdA); minMargin = Math.min(minMargin, p.T0 * (1 - v / p.Vp) - s.R); }
  return minMargin;
}


// ---------------- Minimal solo-first airboat on the 1448 ----------------
// The factory benches stay (structure, flotation, the passenger seat); no storage box, console, dry box or
// A-frame. A light engine stand on two short beds, a smaller cage, thin UHMW strips only.
// [name, lb, X, Y, Z]; weights are estimates until parts are weighed.
const h = HULLS['1448'];
const OUTFIT = [
  ['Engine stand (bolted aluminum) and two short beds across the ribs', 45, 18, 0, 22],
  ['Prop cage, thin-wall aluminum tube, with mesh guard', 45, 4, 0, 38],
  ['Twin rudders and linkage', 12, -6, 0, 36],
  ['Stick, rudder linkage, throttle cable and lever', 10, 62, 9, 10],
  ['Small lithium battery (electric start), cables, kill switch', 10, 40, 0, 6],
  ['Fuel, 6 gal portable tank, full', 45, 100, 0, 6],
  ['Raised driver seat on an aluminum frame', 15, 58, 0, 20],
  ['Safety gear: PFD, extinguisher, horn, lines', 15, 85, 0, 8],
  ['Navigation lights', 5, 30, 0, 50],
  // 45 -> 35 lb (2026-11-03): strips over the wear zones only, to keep 400 lb of payload with the 800R E-TEC.
  ['UHMW bottom strips 3/16 in over the wear zones, and fasteners (no chine angles or bow skin)', 35, 62, 0, -0.1],
  ['Small trim tab and turnbuckle', 3, -2, -19, 1],
  // Added 2026-11-03 with the exhaust layout (Drivetrain_800R_ETEC.md, Section 6a): about 4-6 ft of tailpipe to an aft transom corner.
  ['Tailpipe, heat shields and pipe guard', 5, 8, -20, 25],
  // Allowance added 2026-11-03 for items the concept study left out (estimates until weighed):
  ['Minimal dash: tach, coolant and exhaust temperature gauges, key/DESS post, start button, panel', 3, 60, 0, 30],
  ['Wiring, fuse block, switches', 3, 40, 0, 10],
  ['Belt guard', 3, 13, 0, 37],
  ['Radiator brackets and coolant overflow bottle', 3, 25, 0, 40],
  ['Brackets, rivnuts and bolts not counted in other items', 5, 40, 0, 10],
  ['Bilge pump and hose', 2, 20, 0, 1],
  ['Passenger cushion and grab handle', 4, 82, 0, 24],
  // On top of the prop cage (user, 2026-11-03): 30 in single-row LED light bar (~8.6 lb, 150 W) and a motorized
  // remote searchlight (Golight Stryker LED class, ~6-7 lb, 3.7 A), with brackets and wiring.
  ['LED light bar on the cage top, with bracket', 10, 8, 0, 90],
  ['Motorized remote searchlight on the cage top, with mount', 8, 6, 0, 92],
  ['Light bar and searchlight wiring and switches', 2, 30, 0, 40],
];
// [key, name, hp used, lb (engine + reduction + radiator + exhaust + mounts), prop lb, static thrust lbf, Vp mph]
// Static thrust: power^(2/3) scaling from 440 lbf at 100 hp (64 in prop), the same rule as compare_hulls.js.
const T = hp => Math.round(440 * Math.pow(hp / 100, 2 / 3));
const ENGINES = [
  ['rotax', 'Rotax 582 + B/C gearbox, electric start (65 hp)', 65, 103, 12, T(65), 75],
  // gsxr: 206 lb engine system from archive/rev8_JonAirBoat/bike_reduction.js, plus 11 lb for its heavier stand and beds (56 lb against the 45 lb in OUTFIT).
  ['gsxr', 'GSX-R1000-class bike engine + its gearbox + belt reduction (150 hp used)', 150, 217, 20, T(150), 85],
  // etec800: 79 lb bare (unverified) + exhaust 20, radiator 15, belt/shaft/bearings 39, oil tank/ECU/harness 10, mounts 5
  // (Engine_Selection_1448.md, Section 4a); about 120 hp at the prop.
  ['etec800', 'Rotax 800R E-TEC snowmobile two-stroke + 3:1 belt reduction (120 hp used)', 120, 168, 15, T(120), 82],
];
const CREW = {
  solo: [['Driver', 200, 58, 0, 30]],
  soloGear60: [['Driver', 200, 58, 0, 30], ['Gear (bow)', 60, 120, 0, 10]],
  soloGear150: [['Driver', 200, 58, 0, 30], ['Gear (bow)', 150, 120, 0, 10]],
  plus1: [['Driver', 200, 58, 0, 30], ['Passenger on the middle bench', 200, 82, 0, 22]],
};
const hullItem = ['Hull, Lowe 1448 with its benches', h.hullLb, h.hullX, 0, 7];
const out = [];
for (const [ek, en, hp, lb, propLb, T0, Vp] of ENGINES) {
  const eq = [...OUTFIT, ['Engine system: ' + en, lb, 20, 0, 22], ['Prop', propLb, 4, 0, 42]];
  const eqLb = eq.reduce((s, r) => s + r[1], 0);
  for (const [ck, crew] of Object.entries(CREW)) {
    const items = [hullItem, ...eq, ...crew];
    const mp = mass(items);
    const fl = floatAt(h, mp.W, mp.LCG);
    const CdA = 9 + 3 * (crew.length - 1);
    const t = top(h, mp, { T0, Vp }, CdA);
    const crewLb = crew.reduce((s, r) => s + r[1], 0);
    out.push({ engine: ek, load: ck, equipmentLb: eqLb, payloadUnderPlate: h.plate - eqLb, overPlateLb: Math.max(0, eqLb + crewLb - h.plate), staticThrust: T0,
      weightLb: Math.round(mp.W), dockTrim: +fl.trim.toFixed(2), transomFreeboard: +(h.transom - fl.Tst).toFixed(1),
      topMph: t.mph, topTrim: +t.s.tau.toFixed(2), humpMarginLbf: Math.round(humpMargin(h, mp, { T0, Vp }, CdA)) });
  }
}
console.table(out);

// ---------------- Engine budget (2026-11-03 targets) ----------------
// Targets: solo + 150 lb gear, or driver + passenger, within the 825 lb plate; planes easily; at least 33 mph.
// Sweep the engine system weight (engine, reduction, radiator, exhaust, extra stand over the 45 lb in OUTFIT;
// prop 15 lb) and the power used at the prop. Each cell: worst-case top mph / worst hump margin (lbf).
// Top speed depends almost only on power; the plate sets the weight limit.
const CASES = {
  soloGear150: [['Driver', 200, 58, 0, 30], ['Gear amidships', 150, 80, 0, 10]],
  plus1: CREW.plus1,
};
const outfitLb = OUTFIT.reduce((s, r) => s + r[1], 0);
const budget = [];
for (const sysLb of [100, 125, 150, 175, 200]) {
  const row = { systemLb: sysLb, payloadUnderPlate: h.plate - outfitLb - sysLb - 15 };
  for (const hp of [60, 70, 80, 90, 100, 110, 120]) {
    let mph = 1e9, hump = 1e9;
    for (const crew of Object.values(CASES)) {
      const mp = mass([hullItem, ...OUTFIT, ['Engine system', sysLb, 20, 0, 22], ['Prop', 15, 4, 0, 42], ...crew]);
      const p = { T0: T(hp), Vp: 70 + hp / 10 };
      const CdA = 9 + 3 * (crew.length - 1);
      mph = Math.min(mph, top(h, mp, p, CdA).mph);
      hump = Math.min(hump, Math.round(humpMargin(h, mp, p, CdA)));
    }
    row[hp + ' hp'] = mph + ' / ' + hump;
  }
  budget.push(row);
}
console.log('Engine budget: worst-case top mph / hump margin lbf (payload must be at least 400 lb)');
console.table(budget);
