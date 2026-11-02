// Compares the 1448 (reference), 1652 and 1860 jonboat hulls for the DIY Jon-Air airboat:
// capacity-plate use, static float, planing (Savitsky with the high thrust line) and flotation.
// Same frame and conventions as trim_stability.js. Hull shapes for 1652/1860 are scaled from
// dealer specs (length, bottom width, beam); bow rise and flat-bottom length are assumptions.
// Usage: node compare_hulls.js > compare_results.json
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

// Equipment (everything except the bare hull, crew and cargo). Armor scales with bottom area.
function equipment(h, proto, light) {
  const area = (h.bottom / 48.8) * (h.flatEnd / 101);
  const B = proto === 'B';
  const fuelX = h.flatEnd + 34;               // front floor, same relative spot as the 1448 layout
  return [
    ['Hull armor (UHMW, chine angles, bow skin, tab)', 90 * area, h.flatEnd * 0.62, 0, 1, 'armor', 0.35],
    ['Mounts and cage supports', 65, 27, 0, 6, 'equip', 0.63],
    ['Engine', B ? 195 : 185, 38, 0.5, 18, 'equip', 0.8],
    ['Redrive, belt, pulleys', 55, 18, 0, 34, 'equip', 0.87],
    [light ? 'A-frame, aluminum' : 'A-frame, steel', light ? 55 : 100, 18, 0, 35, 'equip', light ? 0.63 : 0.87],
    ['Prop shaft, hub, bearings', 15, 12, 0, 53, 'equip', 0.87],
    ['Prop', B ? 27 : 20, 5, 0, 53, 'equip', 0.4],
    ['Radiator, coolant', 25, 17.5, 0, 66, 'equip', 0.3],
    ['Exhaust', 20, 26, 9, 22, 'equip', 0.87],
    [light ? 'Cage, aluminum' : 'Cage, steel EMT', light ? 50 : 85, 5, 0, 52, 'equip', light ? 0.63 : 0.87],
    ['Rudders and linkage', 25, -7, 0, 45, 'equip', 0.63],
    ['Steering stick, pushrods', 15, 75, 12, 14, 'equip', 0.63],
    [light ? 'Battery, Group 31 lithium' : 'Battery, Group 31 AGM', light ? 30 : 70, 100, 3, 6, 'equip', light ? 0.4 : 0.57],
    ['Dry box, power hub, wiring', 15, 100, 0, 8, 'equip', 0.3],
    ['Lights', 10, 30, 0, 80, 'equip', 0.5],
    ['Driver seat, tracks', 25, 100, 0, 18, 'equip', 0.2],
    ['Storage box, lid', 35, 64, 0, 10, 'equip', 0.1],
    ['Backrest / grab bar', 20, 50, 0, 26, 'equip', 0.63],
    ['Table, dash', 30, 108, -20, 25, 'equip', 0.5],
    ['Safety gear', 25, 64, 0, 8, 'equip', 0],
    ['Fuel 6.5 gal + tank', 47, fuelX, 0, 6, 'equip', -0.2],
  ];
}
const LOADS = {
  solo: [['Driver', 200, 106, 0, 24, 'crew', 0]],
  soloGear: [['Driver', 200, 106, 0, 24, 'crew', 0], ['Gear on the storage box', 200, 64, 0, 14, 'cargo', 0.3]],
  plus1: [['Driver', 200, 106, 0, 24, 'crew', 0], ['Passenger', 200, 66, 0, 26, 'crew', 0]],
  plus2: [['Driver', 200, 106, 0, 24, 'crew', 0], ['Passenger', 200, 66, 12, 26, 'crew', 0], ['Passenger', 200, 66, -12, 26, 'crew', 0]],
};

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

const out = [];
for (const [hk, h] of Object.entries(HULLS)) {
  for (const light of [false, true]) {
    for (const proto of ['A', 'B']) {
      const eq = equipment(h, proto, light);
      const eqLb = eq.reduce((s, r) => s + r[1], 0);
      // Swamped flotation (no crew; cargo assumed lost or floating)
      const hullItem = ['Hull', h.hullLb, h.hullX, 0, 7, 'hull', 0.63];
      let sub = 0, smx = 0;
      for (const r of [hullItem, ...eq]) { sub += r[1] * r[6]; smx += r[1] * r[6] * r[2]; }
      for (const [lk, load] of Object.entries(LOADS)) {
        const items = [hullItem, ...eq, ...load];
        const mp = mass(items);
        const fl = floatAt(h, mp.W, mp.LCG);
        const GM = fl.hy.VCB + fl.hy.IT / fl.hy.V - mp.VCG;
        const persons = load.filter(r => r[5] === 'crew').length;
        const CdA = 12 + 3 * (persons - 1) + (lk === 'soloGear' ? 1 : 0);
        const t = top(h, mp, PROTO[proto], CdA);
        out.push({ hull: hk, light, proto, load: lk, W: mp.W, LCG: mp.LCG, VCG: mp.VCG,
          plateUse: (eqLb + load.reduce((s, r) => s + r[1], 0)) / h.plate, eqLb,
          staticTrim: fl.trim, transomFb: h.transom - fl.Tst, GM,
          top: t.mph, topTrim: t.s.tau, topWet: t.s.wet, flatEnd: h.flatEnd, planing: t.planing,
          hump: humpMargin(h, mp, PROTO[proto], CdA),
          foamFt3: sub * 1.2 / 60.4, foamX: smx / sub });
      }
    }
  }
}
console.log(JSON.stringify(out, null, 1));
