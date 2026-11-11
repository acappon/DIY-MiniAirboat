// Whole-boat structural check of the minimal 1448: every primary load path against one set of load cases and one set of limits.
// Run: node structural_check.js
// Hand and frame calculations, static. Sizes and positions are from the FeatureScript masters. Bolted joints are assumed for the stand.
//
// Limits, 6061-T6: yield 35 ksi, 11 ksi near a weld. Running loads, which repeat every engine or prop turn, are held to 7 ksi at holes,
// the same limit as stand_800r.js. Target: factor 1.5 to yield on limit loads, 1.0 on the 7 ksi running limit.
'use strict';
const E = 10e6, FY = 35000, FHAZ = 11000, RUN = 7000, F5052 = 23000, TARGET = 1.5;
const rows = [];
function row(group, item, loadCase, demand, cap, unit, note = '') { rows.push({ group, item, loadCase, demand, cap, unit, f: cap / demand, note }); }

const round = (d, t) => { const di = d - 2 * t, A = Math.PI / 4 * (d * d - di * di), I = Math.PI / 64 * (d ** 4 - di ** 4); return { A, I, S: I / (d / 2), r: Math.sqrt(I / A) }; };
const box = (b, h, t) => { const A = b * h - (b - 2 * t) * (h - 2 * t), I = (b * h ** 3 - (b - 2 * t) * (h - 2 * t) ** 3) / 12; return { A, I, S: I / (h / 2), r: Math.sqrt(I / A) }; };
const euler = (s, L, k = 1) => Math.PI ** 2 * E * s.I / (k * L) ** 2;

// Load cases
const THRUST = 497, TORQUE = 244 * 12, BELT = 993, W_ENGINE = 183, W_STAND = 45, W_CAGE = 45, W_LIGHTS = 14, W_FLAG = 2, G_SLAM = 4, G_TRAIL = 3;

// ---- Stand frame: 2D frame per side from stand_800r.js ----
const log = console.log; console.log = () => {}; const st = require('./stand_800r.js'); console.log = log;
function frame(T22, T15) {
  const { solveFrame, NODES, FWD_PIN } = st;
  const BARS = [[0, 1, T22], [1, 2, T22], [2, 4, T15], [1, 7, T22], [7, 3, T22], [3, 8, T22], [8, 5, T22], [3, 4, T15], [4, 5, T15], [5, 6, T15]];
  const thrust = THRUST / 2, beltFwd = BELT * 5 / 12 / 2, beltAft = BELT * 7 / 12 / 2, couple = TORQUE / 17;
  const mountAft = BELT * 14 / 10 / 2, mountFwd = BELT * 4 / 10 / 2;
  const P = sgn => [[4, thrust, -beltFwd + sgn * couple / 2, thrust * 4], [2, 0, -beltAft + sgn * couple / 2, 0], [7, 0, mountAft - W_ENGINE / 4 - sgn * couple / 2, 0], [8, 0, -mountFwd - W_ENGINE / 4 - sgn * couple / 2, 0]];
  const slam = [[7, 0, -G_SLAM * (W_ENGINE + W_STAND) / 4, 0], [8, 0, -G_SLAM * (W_ENGINE + W_STAND) / 4, 0]];
  const worst = (loads) => { let w = { sig: 0 }, up = 0, down = 0; for (const held of [[0, 1, 2], [0, 1]]) { const { R, stress } = solveFrame(NODES, BARS, [...held, ...FWD_PIN], loads); for (const s of stress) if (s.sig > w.sig) w = s; up = Math.max(up, -R(1)); down = Math.max(down, R(19)); } return { sig: w.sig, up, down }; };
  return { run: Math.max(worst(P(1)).sig, worst(P(-1)).sig), slam: worst(slam).sig, up: Math.max(worst(P(1)).up, worst(P(-1)).up), down: worst(P(1)).down };
}
const T22 = box(2, 2, 0.125), T15 = box(1.5, 1.5, 0.125);
const fr = frame(T22, T15);
row('Stand', 'Engine rail and posts, worst member', 'Full power, running', fr.run, RUN, 'psi', 'Engine mount points assumed at X 17 and X 27');
row('Stand', 'Engine rail and posts, worst member', `${G_SLAM} g hard landing`, fr.slam, FY, 'psi');
const diag = Math.hypot(19, 52.9), diagLoad = THRUST / 2 / (19 / diag) + TORQUE / 17 / (52.9 / diag);
row('Stand', 'Diagonal, 1-1/2 x 1-1/2 x 1/8, buckling', 'Full power', diagLoad, euler(T15, diag), 'lbf');
row('Stand', 'Gusset joint, four 5/16 in bolts, bearing in 1/8 in wall', 'Full power', 950 / (2 * 0.3125 * 0.125), RUN * 3, 'psi', 'Four bolts each side, about 3,800 in-lb at the rail. Was three, factor 1.3');
const lat = G_TRAIL * (W_ENGINE + W_STAND + W_CAGE / 2);
row('Stand', 'Cross-straps, 1 x 1/8, side load in tension', `${G_TRAIL} g sideways, trailer pothole`, lat / 2 / (Math.cos(0.6) * (1 * 0.125 - 0.3 * 0.125)), FY, 'psi', 'One strap of each X works, net of a 5/16 in hole');

// ---- Prop shaft and bearing plates ----
const shaftD = 1.5, shaftM = 700 * 5, shaftT = TORQUE;
const tauS = Math.hypot(shaftM, shaftT) * 16 / (Math.PI * shaftD ** 3);
row('Drive', 'Prop shaft 1-1/2 in 4140, solid, combined stress', 'Full power, running', tauS * 2, 60000, 'psi', 'Goodman simplified: 30 ksi fatigue shear doubled');
row('Drive', 'Forward bearing plate at the cutouts', 'Full power, running', 14000 * 0.3, RUN, 'psi', 'Peak 14 ksi steady, 30% alternating');
row('Drive', 'Forward bearing plate at the cutouts', 'Full power, peak to yield', 14000, FY, 'psi');
row('Drive', 'Aft bearing plate, 3/8 in', 'Full power, peak to yield', 12400, FY, 'psi', 'bearing_plate.js');
const w = 2687 * 2 * Math.PI / 60;
row('Drive', 'Prop speed against the maker limit', '7,900 engine rpm', 2687, 3200, 'rpm', 'Warp Drive HP manual');
row('Drive', 'Power against the maker limit', 'Full power', 120, 160, 'hp', 'Warp Drive HP manual');
const gyro = 11.2 / 386 * 33 * 33 / 3 * 271 * 0.5;
row('Drive', 'Forward bearing plate, gyroscopic load in a spin turn', '30 deg/s yaw at full rpm', gyro / 12, 580, 'lbf', 'Small against the 580 lbf belt share');

// ---- Cage ----
const hoop = round(1.25, 0.065), leg = box(1.25, 1.25, 0.065), strut = round(1, 0.065);
const R = 36;
const top = W_LIGHTS + W_FLAG + 0.3 * W_CAGE;
const ringM = 0.3 * G_TRAIL * top * R / 2;
row('Cage', 'Hoop bending, 1-1/4 x 0.065, top load', `${G_TRAIL} g sideways`, ringM / hoop.S, FY, 'psi', 'Assumes the wall; BOM says thin-wall');
const ringV = 0.3 * G_SLAM * top * R / 2;
row('Cage', 'Hoop bending, top load', `${G_SLAM} g hard landing`, ringV / hoop.S, FY, 'psi');
const dHoop = 0.15 * G_TRAIL * top * R ** 3 / (2 * E * hoop.I);
row('Cage', 'Hoop deflection against 2.6 in of tip clearance', `${G_TRAIL} g sideways`, dHoop, 2.6, 'in');
const legM = G_SLAM * (W_CAGE + W_LIGHTS + W_FLAG) * 0.4 * 20 / 4;
row('Cage', 'Legs, bending, fore and aft', `${G_SLAM} g, fore and aft crash stop`, legM / leg.S, FY, 'psi', 'Struts share the load');
row('Cage', 'Struts 1 x 0.065, buckling', '3 g fore and aft', 140, 2500, 'lbf');
row('Cage', 'Foot pad bolts, 4 x 1/4-20, shear', `${G_SLAM} g fore and aft`, G_SLAM * 61 / 16, 950, 'lbf each');
row('Cage', 'Mesh pull, aft face in the prop wake', 'Full power, if the aft face is meshed', 220 / 8, 950, 'lbf per stringer', 'Also costs about 220 lbf of thrust: mesh the front and rim only');
const walk = 200 * 36 / 4 * 0.3;
row('Cage', 'Hoop, person leaning on top', '200 lb at the top, two hoops', walk * 2 / 2 / hoop.S, FY, 'psi', 'Not a step: warn in the guide');

// ---- Rudders and linkage ----
const shaft = round(1, 0.125);
row('Controls', 'Rudder shaft 1 x 0.125', 'Full power at the stall, running', 5190, RUN, 'psi');
row('Controls', 'Rudder shaft 1 x 0.125', 'Hard stop, 2 times stall load', 5190 * 2, FY, 'psi');

// ---- Driver loads: typical light aircraft pilot forces ----
const GRIP = 26, CABLE = 8, F_FA = 167, F_LAT = 67, F_PED = 200;
function stick(d, t) { const s = round(d, t), M = F_FA * (GRIP - CABLE); return { sig: M / s.S, lb: s.A * 24 * 0.098 }; }
const S1 = stick(1.5, 0.12);
row('Controls', 'Steering stick 1-1/2 x 0.120', `${F_FA} lbf at the grip`, S1.sig, FY, 'psi', 'Was 1-1/8 x 0.065, factor 0.6');
row('Controls', 'Stick base, two 1/4-20 rivnuts per side', `${F_LAT} lbf sideways`, F_LAT * GRIP / 4 / 2, 500, 'lbf each', 'Rivnut pull-out in about 0.1 in rib');
row('Controls', 'Rudder cable, with a stop on the stick', `${F_FA} lbf at the grip`, 100, 500, 'lbf', 'Without the stop it would see 543 lbf: factor 0.9');
const PED1 = F_PED * 6.2 / 4 / (3.5 * 0.1875 ** 2 / 6);
row('Controls', 'Throttle pedal 3/16 in plate', `${F_PED} lbf panic stomp`, PED1, FY, 'psi', 'Was 1/8 in, factor 1.0');
const arm1 = round(1, 0.095);
row('Controls', 'Console arm 1 x 0.095', '150 lb pull on the dash', 150 * 8 / arm1.S, FY, 'psi', 'Was 1 x 0.065, factor 1.2');
row('Controls', 'Console post, buckling', '150 lb on the dash', 150, euler(arm1, 22), 'lbf');

// ---- Seat module (v1.0.1.3, Minimal_cockpit.fs): replaces the middle bench as the thwart and the seat ----
// Struts 1 x 2 x 1/8, 58.8 in between the hull sides, taken as simply supported (the bolted end clips add margin).
// Two people side by side at Y +/-10, shared by the two struts.
const STRUT = box(1, 2, 0.125), STRUT_SPAN = 58.8, PEOPLE = 400;
const strutM = g => g * PEOPLE / 2 / 2 * (STRUT_SPAN / 2 - 10);
row('Seat', 'Strut 1 x 2 x 1/8, bending', 'Two people seated, at rest', strutM(1) / STRUT.S, FY, 'psi');
row('Seat', 'Strut 1 x 2 x 1/8, bending', `Two people seated, ${G_SLAM} g hard landing`, strutM(G_SLAM) / STRUT.S, FY, 'psi', 'Simply supported; the end clips make it stiffer');
row('Seat', 'Strut end clip, two 1/4-20 through the hull side', `Two people, ${G_SLAM} g`, G_SLAM * PEOPLE / 4 / 2, 950, 'lbf each');
row('Seat', 'Strut end clip, bolt bearing in the 0.080 hull side', `Two people, ${G_SLAM} g`, G_SLAM * PEOPLE / 4 / 2, 0.25 * 0.08 * 1.5 * F5052, 'lbf each', 'Hull side gauge assumed; fender washer inside');
const ply = t => 15 * t * t / 6;   // seat base section modulus, 15 in wide
row('Seat', 'Seat base 3/8 marine plywood, 5 in past the forward strut', '200 lb standing on the front edge', 200 * 5 / ply(0.375), 7000, 'psi', 'Fir marine plywood, about 7,000 psi to break; 6 mm would be 7,200 psi');
row('Seat', 'Seatback bracket 1/8 x 1-1/4 6061, one side', 'Driver leaning back, 150 lbf at 10 in', 150 * 10 / 2 / (0.125 * 1.25 ** 2 / 6), FY, 'psi');
// ---- Oarlocks (v1.0.1.3): 7 ft oars, 24 in inboard, blade center about 49 in outboard of the pin; pin 6 in above the rail top ----
// A hard pull of 100 lbf on the handle puts 100 x (24 + 49) / 49 = 149 lbf on the pin, fore and aft.
const OAR_PIN = 100 * (24 + 49) / 49, OAR_ARM = 6, OAR_BOLTS = 2.25;
row('Oarlock', 'Riser 1-1/2 x 3 6061, bending at the base (pocketed to 1/8 in walls)', 'Hard pull, 100 lbf at the handle', OAR_PIN * OAR_ARM / ((1.5 * 27 - 1.25 * 2.75 ** 3) / 18), FY, 'psi');
row('Oarlock', 'Riser, two 5/16-18 through the rail and doubler, 2-1/4 in apart', 'Hard pull, tension in one bolt', OAR_PIN * OAR_ARM / OAR_BOLTS, 2000, 'lbf', '316 stainless 5/16-18 about 2,000 lbf working');
row('Oarlock', 'Rail bearing under the riser, 1/8 in doubler 1-1/2 x 4', 'Hard pull, prying on the rail lip', OAR_PIN * OAR_ARM / OAR_BOLTS, 0.3125 * 0.08 * 1.5 * F5052 * 2, 'lbf', 'Two bolt holes in the 0.080 rail, doubler under it');
row('Seat', 'Dry box hanging from the struts, rivets', `${G_SLAM} g hard landing`, G_SLAM * (4.5 + 3.9 + 2) / 8, 150, 'lbf each', 'Eight 3/16 aluminum rivets; battery, box and fuse block');

// ---- Hull, assumed bottom 0.100 and sides 0.080 in 5052; confirm by measuring ----
const sideI = 2 * 0.08 * 19 ** 3 / 12, Ab = 48 * 0.1, As = 2 * 19 * 0.08, Ag = 0.6;
const na = (As * 9.5 + Ag * 19) / (Ab + As + Ag);
const I = sideI + As * (9.5 - na) ** 2 + Ab * na * na + Ag * (19 - na) ** 2, Shull = I / (19 - na);
const Mhull = 1066 * 130 / 8;
row('Hull', 'Hull girder bending, sheer', 'Dock, full load', Mhull / Shull, F5052, 'psi');
row('Hull', 'Hull girder bending, sheer', `${G_SLAM} g hard landing`, G_SLAM * Mhull / Shull, F5052, 'psi');
const pSlam = 5, span = 7;
row('Hull', 'Bottom panel between formed keels, 7 in span', '5 psi slam at speed', pSlam * span * span / 12 / (0.1 * 0.1 / 6), F5052, 'psi', 'Span and gauge assumed; UHMW sheet helps');
row('Hull', 'Forward shoe on the rib, one side', 'Full power', fr.down, 2400, 'lbf', 'Rib crippling estimate, two walls 1.5 in tall');
row('Hull', 'Forward shoe on the rib, one side', `${G_SLAM} g hard landing`, G_SLAM * (W_ENGINE + W_STAND) / 4, 2400, 'lbf');
row('Hull', 'Bench clip, two 1/4-20 in panel', 'Full power', fr.up * 0.65 / 2, 950, 'lbf each');
row('Hull', 'Bench clip into the panel, pull-through', 'Full power', fr.up * 0.65 / 2, 300, 'lbf each', 'Panel gauge unknown, 0.063 in assumed, with a backing plate');
row('Hull', 'Shoe bolts, six 1/4-20, shear', 'Full power', 26, 950, 'lbf each');
row('Hull', 'Transom clip', 'Full power', fr.up * 0.35, 900, 'lbf');
row('Hull', 'Fuel tank straps, two', `${G_SLAM} g hard landing`, G_SLAM * 45 / 2, 300, 'lbf each', 'Use rated cam straps');
row('Hull', 'Battery strap in the dry box', `${G_SLAM} g hard landing`, G_SLAM * 4.5, 200, 'lbf');

// ---- Blade loss: not a design case ----
const lost = (11.2 / 3) * 0.7 / 386 * 16 * w * w;

module.exports = { rows, frame, box, lost };
if (require.main === module) {
  for (const r of rows) console.log([r.group, r.item, r.loadCase, Math.round(r.demand * 100) / 100, r.unit, Math.round(r.cap), r.f.toFixed(1), r.note].join(' | '));
  console.log('Blade loss, out of balance force lbf:', Math.round(lost));
  console.log('\nStand variants: running psi, slam psi, frame weight change');
  const lbIn = s => s.A * 0.098;
  const base = 2 * 53.9 * lbIn(T22) + 2 * 26.8 * lbIn(T22) + 2 * (41.9 + 56.2 + 12) * lbIn(T15) + 3 * 15 * lbIn(T15);
  for (const [n, a, b] of [['As built 2x2x1/8, 1.5x1.5x1/8', box(2, 2, .125), box(1.5, 1.5, .125)], ['2x2x1/8, 1.5x1.5x0.095', box(2, 2, .125), box(1.5, 1.5, .095)], ['2x2x0.095, 1.5x1.5x0.095', box(2, 2, .095), box(1.5, 1.5, .095)], ['Stiffer rail 2x2x3/16, 1.5x1.5x1/8', box(2, 2, .1875), box(1.5, 1.5, .125)]]) {
    const f = frame(a, b), lb = 2 * 53.9 * lbIn(a) + 2 * 26.8 * lbIn(a) + 2 * (41.9 + 56.2 + 12) * lbIn(b) + 3 * 15 * lbIn(b);
    const kBare = 12 * E * a.I / 42 ** 3 + 12 * E * b.I / diag ** 3;
    console.log(n, Math.round(f.run), Math.round(f.slam), (lb - base).toFixed(1) + ' lb', 'side stiffness', Math.round(kBare));
  }
  const solid = Math.PI / 4 * 1.5 ** 2 * 16 * 0.283, hollow = Math.PI / 4 * (1.5 ** 2 - 0.75 ** 2) * 16 * 0.283;
  console.log('Shaft solid', solid.toFixed(1), 'hollow 3/4 bore', hollow.toFixed(1), 'lb; stress up by', (1 / (1 - 0.5 ** 4)).toFixed(3));
  const tube = 2 * Math.PI * 36 * 2 + 8 * 10 + 4 * 26;
  console.log('Cage tube length in', Math.round(tube), 'lb at 1-1/4 x 0.065', (tube * hoop.A * 0.098).toFixed(1));
}
