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
// A-frame. A light engine stand on two short beds, a smaller cage, thin UHMW only.
// [name, lb, X, Y, Z]; weights are estimates until parts are weighed.
const h = HULLS['1448'];
const OUTFIT = [
  // 41.5 lb stand on four separate shoes, plus 7.6 lb of hull fastening (stand_800r.js, 2026-11-03).
  ['Engine stand (bolted aluminum) on four shoes, with doublers, clips, cage foot pads and their bolts', 49.1, 18, 0, 22],
  ['Prop cage, thin-wall aluminum tube, with mesh guard', 45, 4, 0, 38],
  ['Twin rudders and linkage', 12, -6, 0, 36],
  // Cockpit, 2026-11-03 (seat module 2026-11-10): driver on the seat module where the middle bench was, stick to port on the rib at X 95, throttle pedal with heel rests, push-pull cables along the hull sides.
  ['Stick, rudder cable, throttle pedal and cable, heel rests', 10, 70, 8, 8],
  ['Small lithium battery', 4.5, 83.6, 0, 6.5],
  // Seat module, v1.0.1.3 (Minimal_cockpit.fs), replacing the middle bench and its dry box case. Estimates until weighed:
  // struts 2 x 58.8 in of 1 x 2 x 1/8 tube 7.9, clips 0.4, bolts 0.4; box 0.063 sheet 2.1, flange 0.6, 0.080 lid 0.7, brackets, gasket, rivnuts 0.5;
  // walls 0.032 sheet 2.5, lids 2 x 1.05; seat base 3/8 in ply 4.3, PE pad 1.8, vinyl 0.9, strips and bolts 0.2; seatback 3.0.
  ['Seat struts 1 x 2 x 1/8 6061, end clips and bolts (thwart)', 8.7, 81.5, 0, 10],
  ['Electrical dry box 0.063 5052 with lid, gasket, rivnuts, battery and fuse brackets', 3.9, 81.5, 0, 7.5],
  ['Storage compartment walls 0.032 5052 and two aft-hinged lids with hinges and latches', 4.6, 81.5, 0, 6.5],
  ['Seat: 3/8 in plywood base, 2-1/2 in closed-cell pad, vinyl, bolts', 7.2, 84, 0, 12.9],
  ['Folding seatback, padded both sides, hinge brackets and pivot', 3, 75, 0, 21],
  ['Seat module small parts: rivets, sealing strips, battery switch plate', 0.4, 81.5, 0, 9],
  // On the front deck step (top at Z 16.55).
  ['Fuel, 6 gal portable tank, full', 45, 143, 0, 21],
  // Sender 0.3, gauge 0.3, tie-down straps 0.4, 10 ft of extra fuel line 1.
  ['Fuel gauge sender and gauge, tank straps, longer fuel line', 2, 110, -10, 15],
  // Safety and regulatory gear, itemized 2026-11-03 (was one 15 lb line):
  ['Inflatable PFDs (2) and dock lines', 6, 85, 0, 8],
  ['Fire extinguisher, USCG B-I / UL 5-B, with bracket, within reach of the driver', 4, 99.5, -25, 20],
  ['Compact can air horn in a clip, at the console', 0.5, 105, -14, 25],
  ['Florida airboat flag (10 x 12 in orange) on a fiberglass whip at least 10 ft above the bottom, with mount', 2, -4, 0, 100],
  ['Navigation lights: two LED sidelights and an all-round white', 1.5, 60, 0, 50],
  // 45 -> 35 lb (2026-11-03): strips over the wear zones only, to keep 400 lb of payload with the 800R E-TEC.
  // 35 -> 32 lb (2026-11-03) from the CAD strips: 864 in^3 at 0.034 lb/in^3 is 29.4 lb, 56 #10 screw sets 1.4, sealant 0.4, 0.8 reserve.
  // 32 -> 35.6 lb (user, 2026-11-04): whole bottom to X 157, 1/8 in, in place of the 3/16 in strips. 924 in^3 at 0.034 lb/in^3 is 31.4 lb,
  // 80 #10 screw sets 2.0, sealant 0.5, two 44 in backing bars forward of the last rib 1.1, 0.6 reserve.
  ['UHMW 1/8 in over the whole bottom, and fasteners (no chine angles or bow skin)', 35.6, 77, 0, 1.2],
  ['Small trim tab and turnbuckle', 3, -2, -19, 1],
  // Added 2026-11-03 with the exhaust layout (Drivetrain_800R_ETEC.md, Section 6a): about 4-6 ft of tailpipe to an aft transom corner.
  // Itemized 2026-11-03 from the CAD layout: 41 in of 2 in x 0.049 steel tailpipe 3.6, three 0.040 in shields with standoffs 2.2, pipe guard 1.
  ['Tailpipe, heat shields and pipe guard', 7, 12, 14, 32],
  // Added 2026-11-03: two 1 x 0.065 in cage struts 1.1, two 1 x 1/8 in stand cross-straps 0.9, brackets 0.5.
  ['Cage struts and stand cross-straps', 2.5, 25, 0, 50],
  // Allowance added 2026-11-03 for items the concept study left out (estimates until weighed):
  // Gauges, lanyard kill switch, joystick, switches, key, USB port and 12 V outlet (Minimal_dash.fs); exhaust gauge and probe 0.3; round bubble level 0.15, shelf 0.1 and load label 0.05.
  ['Dash: gas, tach, temperature and exhaust gauges, bubble level, load label, switches, joystick, key, USB port, 12 V outlet, panel', 4.4, 103, -13, 28],
  ['Adjustable marine phone or tablet holder, without the device', 0.8, 100, 0, 28],
  // 1 in x 0.095 tube frame 1.5, rib foot 0.2, sheet pod 1.2.
  ['Console frame and pod, starboard, clear above two sets of knees', 2.9, 105, -18, 24],
  // Capsize flotation (flotation_inverted.js): 2 lb/ft^3 closed-cell foam, added to the foam in the rear bench; the seat pad counts too.
  ['Flotation foam, bow wedge', 0.8, 155, 0, 12],
  ['Flotation foam, two aft corner blocks', 3.1, 8, 0, 6],
  ['Flotation foam, seat compartment bottoms', 1.4, 81.5, 0, 3.3],
  ['Flotation foam, footwell side blocks, with straps', 4.1, 113, 0, 7.3],
  // electrical/wire_list.py: wire 8.3 (2 AWG cable 2.8, 16 AWG per the E-11 review), connectors and glands 0.8, relays 0.5, fuse block 0.6, main fuse, inline fuse and bus strips 0.3, battery switch 0.5, marine starter solenoid 0.6, floor conduit and bench sleeve 0.6.
  ['Harness: all wire, connectors, relays, fuse block, battery switch, solenoid, conduit', 12.2, 62, 0, 7],
  ['Belt guard', 3, 13, 0, 37],
  ['Radiator brackets and coolant overflow bottle', 3, 25, 0, 40],
  ['Brackets, rivnuts and bolts not counted in other items', 5, 40, 0, 10],
  ['Bilge pump and hose', 2, 20, 0, 1],
  // On top of the prop cage (user, 2026-11-03): 20 in single-row LED light bar (~5 lb, 100 W) and a motorized
  // remote searchlight (Golight Stryker LED class, ~6-7 lb, 3.7 A), with brackets and wiring.
  ['LED light bar, 20 in, on the cage top, with bracket', 6, 8, 0, 90],
  ['Motorized remote searchlight on the cage top, with mount', 8, 6, 0, 92],
];
// [key, name, hp used, lb (engine + reduction + radiator + exhaust + mounts), prop lb, static thrust lbf, Vp mph]
// Static thrust: power^(2/3) scaling from 440 lbf at 100 hp (64 in prop), the same rule as compare_hulls.js.
const T = hp => Math.round(440 * Math.pow(hp / 100, 2 / 3));
const ENGINES = [
  ['rotax', 'Rotax 582 + B/C gearbox, electric start (65 hp)', 65, 103, 12, T(65), 75],
  // gsxr: 206 lb engine system from archive/rev8_JonAirBoat/bike_reduction.js, plus 11 lb for its heavier stand and beds (56 lb against the 45 lb in OUTFIT).
  ['gsxr', 'GSX-R1000-class bike engine + its gearbox + belt reduction (150 hp used)', 150, 217, 20, T(150), 85],
  // etec800: 79 lb bare (unverified) + exhaust 20, radiator 16.5, belt/shaft/bearings 36.3 (petal bearing plates), oil tank/ECU/harness 10, mounts 5
  // (archive/Engine_Selection_1448.md, Section 4a); about 120 hp at the prop.
  ['etec800', 'Rotax 800R E-TEC snowmobile two-stroke + 3:1 belt reduction (120 hp used)', 120, 166.8, 15, T(120), 82],
];
const CREW = {
  solo: [['Driver', 200, 83.5, 0, 27]],
  soloGear60: [['Driver', 200, 83.5, 0, 27], ['Gear between the benches', 60, 60, 0, 10]],
  soloGear150: [['Driver', 200, 83.5, 0, 27], ['Gear between the benches', 150, 60, 0, 10]],
  plus1: [['Driver', 200, 83.5, 11, 27], ['Passenger on the seat', 200, 83.5, -11, 27]],
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
// Targets: driver + 150 lb gear within the 825 lb plate; planes easily; at least 33 mph.
// (The driver + passenger case was dropped 2026-11-03; plus1 is still reported above for reference.)
// Sweep the engine system weight (engine, reduction, radiator, exhaust, extra stand over the 45 lb in OUTFIT;
// prop 15 lb) and the power used at the prop. Each cell: worst-case top mph / worst hump margin (lbf).
// Top speed depends almost only on power; the plate sets the weight limit.
const CASES = {
  soloGear150: [['Driver', 200, 83.5, 0, 27], ['Gear amidships', 150, 60, 0, 10]],
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
console.log('Engine budget: worst-case top mph / hump margin lbf (payload must be at least 350 lb)');
console.table(budget);

module.exports = { h, OUTFIT, ENGINES, hullItem, mass, floatAt, top, humpMargin };
