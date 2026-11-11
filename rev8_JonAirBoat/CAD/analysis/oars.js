// Oars for the minimal 1448 (v1.0.1.3): rowing facing aft on the folded seatback, and shipping the oars in their locks
// along inside each rail. Checks the clearances against the CAD in Minimal_cockpit.fs, Minimal_dash.fs and Minimal_layout.fs.
// Run: node oars.js
// Boat axes: X forward from the transom, Y to port, Z up from the hull bottom, inches.
'use strict';

const OAR = 84, INBOARD = 24, BLADE = 22, SHAFT_R = 0.8, BLADE_W = 6;
const LOCK = { x: 66, y: 34.5, z: 25.5 };      // pin and shaft center: rail top 19.5, 1/8 doubler, 4 in riser, socket, ring
const SEAT_TOP = 16.4;                          // folded seatback lying on the seat pad
const HIP = { x: 81, z: SEAT_TOP + 3.5 };       // rower facing aft, sitting about 4-1/2 in in from the seat's aft edge (X 76.5)
const KNEE = { x: 63, y: 4, r: 2.5 };           // knee tops about Z 24-26 (seat only 15 in above the floor), feet braced aft
const WATER_Z = 5.3;                            // driver + 150 lb, from stability_upright.js (draft about 5-1/2 in)

const out = [];
const check = (what, clear, need, note = '') => out.push({ what, clearIn: +clear.toFixed(1), needIn: need, ok: clear >= need ? 'yes' : 'NO', note });
const dist = (a, b) => Math.hypot(a[0] - b[0], a[1] - b[1], a[2] - b[2]);
// Shortest distance from point p to segment ab.
function segDist(p, a, b) {
  const ab = b.map((v, i) => v - a[i]), ap = p.map((v, i) => v - a[i]);
  const t = Math.max(0, Math.min(1, ap.reduce((s, v, i) => s + v * ab[i], 0) / ab.reduce((s, v) => s + v * v, 0)));
  return dist(p, a.map((v, i) => v + t * ab[i]));
}
function segSeg(a, b, c, d, n = 200) { let m = 1e9; for (let i = 0; i <= n; i++) { const t = i / n; m = Math.min(m, segDist(a.map((v, k) => v + t * (b[k] - v)), c, d)); } return m; }

// ---- Rowing ----
// Rule of thumb for a rowboat: pins 9-12 in aft of the seat's aft edge (toward the rower's feet), 7-9 in above the seat.
check('Pins aft of the seat edge (X 76.5)', 76.5 - LOCK.x, 9, 'rule of thumb 9-12 in');
check('Pin height above the folded seatback', LOCK.z - SEAT_TOP, 7, 'rule of thumb 7-9 in; the high seat puts the knees high, so the top of the range');
// Handle sweep, stroke from 45 deg aft of square (catch) to 35 deg forward (finish); handles at pin height +/- 2 in.
const handle = th => [LOCK.x - INBOARD * Math.sin(th * Math.PI / 180), LOCK.y - INBOARD * Math.cos(th * Math.PI / 180)];
// With 24 in inboard on a 69 in spread the handle ends never come closer than 10.5 in to the centerline, so the hands pass
// beside the knees (knees together, centers 4 in off the centerline, 2-1/2 in radius, X 58-68), not over them.
let kneeClear = 1e9;
for (let th = -35; th <= 45; th += 1) {
  const [hx, hy] = handle(th);
  if (hx > KNEE.x - 5 && hx < KNEE.x + 5) kneeClear = Math.min(kneeClear, (hy - 0.5) - (KNEE.y + KNEE.r));
}
check('Hands beside the knees through the stroke (side gap)', kneeClear, 2, 'hand 1/2 in inboard of the handle end');
const finish = handle(-35);
check('Handle ends apart at the finish (no overlap)', 2 * finish[1], 4, 'handle end ' + finish.map(v => v.toFixed(1)).join(', '));
// Leaning back at the finish: elbows past the torso, about X 90, |Y| 16, Z 26; head about X 97, Z 45.
check('Port elbow to the steering stick grip (X 95.25, Y 21, Z 27.5-31.5)', segDist([90, 16, 26], [95.25, 21, 27.5], [95.25, 21, 31.5]) - 0.9 - 1.5, 2, 'stick at neutral; tie it there while rowing');
check('Starboard elbow to the dash pod (X 101+, Y -5 to -21, Z 24-30)', 101 - 90 - 1.5, 2);
check('Head to the console (top Z 30), leaning back', 45 - 30, 4);
// Blade in the water: blade center about 49 in outboard of the pin.
const dip = Math.asin((LOCK.z - WATER_Z) / (OAR - INBOARD - BLADE / 2)) * 180 / Math.PI;
out.push({ what: 'Shaft angle down to the water at the blade center', clearIn: +dip.toFixed(0), needIn: '15-25 deg', ok: dip >= 15 && dip <= 25 ? 'yes' : 'NO', note: 'degrees, not inches' });
// Blades through the stroke stay outboard: at the finish the blade tip is at X 66 - 60 sin 35 = 31.6, Y 34.5 + 49 = 83.6,
// far outside the cage (radius 36 about the centerline, X 1-11) and the cage legs (Y 30.5).
const tip = th => [LOCK.x - (OAR - INBOARD) * Math.sin(-th * Math.PI / 180), LOCK.y + (OAR - INBOARD) * Math.cos(th * Math.PI / 180)];
check('Blade tip to the cage drum at the finish, outboard', tip(-35)[1] - 36, 6);

// ---- Shipped: swung blade forward in the ring, lying on a rest at X 118, Y 31.5 ----
const oy = x => LOCK.y - (x - LOCK.x) * 3 / 52;
const A = [42, oy(42), LOCK.z], B = [126, oy(126), LOCK.z];
for (const sd of [1, -1]) {
  const a = [A[0], sd * A[1], A[2]], b = [B[0], sd * B[1], B[2]];
  if (sd < 0) {
    // Console arm from (102.5, -21, 24) to the gunwale at (102.5, -33.4, 19.3), 1 in tube.
    check('Starboard oar to the console arm', segSeg(a, b, [102.5, -21, 24], [102.5, -33.4, 19.3]) - SHAFT_R - 0.5, 1, 'without the risers the oar would lie on the arm');
    check('Starboard oar to the fire extinguisher (Y -25, r 1.75, top Z 27.1)', Math.abs(oy(99.5)) - SHAFT_R - 26.75, 2);
    check('Starboard oar to the dash pod (Y -21 outboard edge)', Math.abs(oy(104)) - SHAFT_R - 21, 2);
    check('Starboard blade to the fuel line (125, -27.4, 16)', LOCK.z - 0.3 - 16.2, 2);
  } else {
    check('Port oar to the steering stick grip (Y 21, r 0.9)', oy(95.25) - SHAFT_R - 21.9, 2, 'the stick moves fore and aft only');
    check('Port oar to the driver\'s left elbow on the stick (about Y 25, Z 22)', Math.hypot(oy(90) - SHAFT_R - 25, LOCK.z - 22), 2);
  }
  check((sd > 0 ? 'Port' : 'Starboard') + ' blade to the sidelight (108, 31, 17)', LOCK.z - 0.3 - 17.5, 2);
  check((sd > 0 ? 'Port' : 'Starboard') + ' oar to the storage lid opened (Y 19.5-28.4)', oy(86.5) - SHAFT_R - 28.4, 1);
  check((sd > 0 ? 'Port' : 'Starboard') + ' handle end to the stand and cage legs (X 36 and aft)', 42 - 36, 3);
}
console.table(out);
