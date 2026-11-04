// Radiator sizing and belt width for the minimal 1448 airboat with a Rotax 800R E-TEC.
// First estimates for choosing parts, not measurements. The static test confirms them.
// Usage: node radiator_800r.js
'use strict';

// ---------------- Heat to the coolant ----------------
// No BRP figure was found. A two-stroke gives the coolant about 0.85 to 1 times its shaft power (assumed).
const KW_PER_HP = 0.7457;
const heat = hp => ({ lowKw: +(hp * KW_PER_HP * 0.85).toFixed(0), highKw: +(hp * KW_PER_HP).toFixed(0) });

// ---------------- Air speed at the radiator, from actuator-disc theory ----------------
const RHO = 1.2, R_PROP = 33 * 0.0254, DISC_X = 5;   // kg/m3, m, prop disc station in inches
const AREA = Math.PI * R_PROP ** 2;
const LBF = 4.448, MPH = 0.447;
const inducedStatic = t => Math.sqrt(t * LBF / (2 * RHO * AREA));
const inducedMoving = (t, v) => (-v + Math.sqrt(v * v + 4 * (t * LBF) / (2 * RHO * AREA))) / 2;
const upstream = x => { const d = (x - DISC_X) * 0.0254; return 1 - d / Math.sqrt(d * d + R_PROP * R_PROP); };

// ---------------- Radiator ----------------
const K_CORE = 3.5;                    // core loss in dynamic heads, assumed: core speed = air speed / sqrt(1 + K)
const TEMP_DIFF = 50;                  // coolant 85 C, air 35 C
const CP = 1005;
function rejected(wIn, hIn, airMs, effectiveness) {
  const area = wIn * hIn * 0.0254 ** 2;
  const vCore = airMs / Math.sqrt(1 + K_CORE);
  return +(RHO * area * vCore * CP * effectiveness * TEMP_DIFF / 1000).toFixed(0);
}

const STATIC_THRUST = 497, RUN_THRUST = 300, RUN_MPH = 35;
const cases = [];
for (const r of [
  { name: 'old: 16 x 20 at X 33', w: 16, h: 20, x: 33 },
  { name: '16 x 20 at X 16', w: 16, h: 20, x: 16 },
  { name: 'chosen: 18 x 22 at X 16', w: 18, h: 22, x: 16 },
  { name: '20 x 24 at X 16', w: 20, h: 24, x: 16 },
]) {
  const vStatic = inducedStatic(STATIC_THRUST) * upstream(r.x);
  const v = RUN_MPH * MPH;
  const vRun = v + inducedMoving(RUN_THRUST, v) * upstream(r.x);
  cases.push({ radiator: r.name, staticAirMs: +vStatic.toFixed(1), staticKw: rejected(r.w, r.h, vStatic, 0.6),
    runAirMs: +vRun.toFixed(1), runKw: rejected(r.w, r.h, vRun, 0.5) });
}

// Warm-up at full throttle tied to the dock with the chosen radiator.
const HEAT_MASS = 8 * 3.6 + 35 * 0.9;  // kJ/K: about 8 kg of coolant and 35 kg of engine metal, assumed
const chosen = cases[2];
const surplus = heat(120).highKw - chosen.staticKw;
const staticSeconds = Math.round((95 - 60) * HEAT_MASS / surplus);

// ---------------- Belt width: Gates Poly Chain GT Carbon drive design manual, 8mm pitch, 32 groove ----------------
// The tables stop at 5,500 rpm, so the ratings at 7,600 rpm extend them at the falling slope. Confirm in DesignFlex.
const LENGTH_FACTOR = 1.22;            // 8MGT-2000, 250 teeth
const widths = [
  { mm: 36, at4500: 116.3, at5000: 127.3, at5500: 138.0, addOn5500: 11.8 },
  { mm: 62, at4500: 200.4, at5000: 219.3, at5500: 237.7, addOn5500: 20.3 },
];
const SF_TARGET = 1.5, RPM = 7600;
const belts = widths.map(w => {
  const step = w.at5500 - w.at5000;
  const fade = w.at5000 - w.at4500 - step;
  let hp = w.at5500, s = step;
  for (let r = 5500; r < RPM; r += 500) { const f = Math.min(1, (RPM - r) / 500); s -= fade; hp += s * f; }
  const rated = (hp + w.addOn5500 * RPM / 5500) * LENGTH_FACTOR;
  return { widthMm: w.mm, ratedHp: Math.round(rated), sfAt120hp: +(rated / 120).toFixed(1), sfAt160hp: +(rated / 160).toFixed(1), meetsTarget: rated / 120 >= SF_TARGET };
});

const pdS = 3.208, pdL = 9.423, LENGTH = 78.74;
const b = LENGTH - Math.PI / 2 * (pdL + pdS);
const center = (b + Math.sqrt(b * b - 2 * (pdL - pdS) ** 2)) / 4;

console.log(JSON.stringify({ heatAt120hp: heat(120), heatAt60hp: heat(60), radiators: cases,
  chosenStaticSecondsFrom60To95C: staticSeconds, belts, centerDistanceIn: +center.toFixed(2),
  crankZForPropZ52: +(52 - center).toFixed(2) }, null, 1));
