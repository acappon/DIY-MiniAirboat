// Grab rail and sling backrest behind the middle bench: hand sizing for a 300 lbf load in any direction.
// Run: node grab_rail.js
// Geometry is from CAD/FeatureScript/Minimal_cockpit.fs: hoop 20 in wide overall, foot pins on the rib at X 70.65, a dog-leg bend
// just aft of the bench at X 74.6, Z 12.5, upper legs leaning back 12 degrees to the top at Z 32, a 2 in aft bow across the top,
// stays from 1-1/2 in below the 4 in corners to pins on the rib at X 54.75, and crossed cables in the plane of the stays.
// Welded 6061-T6: about 11 ksi yield and 24 ksi ultimate near a weld, 35 ksi yield away from it.
// The sling carries none of the grab load; its only effect on the structure is to pull the legs inward.

const LB_IN3 = 0.098, E = 10e6, FTY = 35000, FTY_HAZ = 11000, FTU_HAZ = 24000, LOAD = 300;
const R = Math.PI / 180;
const tube = (d, t) => {
  const di = d - 2 * t;
  const A = Math.PI / 4 * (d * d - di * di), I = Math.PI / 64 * (d ** 4 - di ** 4);
  return { A, I, S: I / (d / 2), lbIn: A * LB_IN3 };
};
const hoop = tube(1.5, 0.083), stay = tube(1, 0.065);
const foot = [70.65, 2.5], stayFoot = [54.75, 2.5], kink = [74.6, 12.5], topZ = 32, lean = 12 * R, bow = 2;
const d2 = [-Math.sin(lean), Math.cos(lean)];
const upperLen = (topZ - kink[1]) / Math.cos(lean);
const top = [kink[0] + d2[0] * upperLen, topZ];
const joint = [top[0] - d2[0] * 5.5, top[1] - d2[1] * 5.5];
const stayLen = Math.hypot(joint[0] - stayFoot[0], joint[1] - stayFoot[1]);
const sd = [(joint[0] - stayFoot[0]) / stayLen, (joint[1] - stayFoot[1]) / stayLen];
const span = 18.5; // leg centerlines

// Fore-aft load at the top: moments about the foot pins. The stays carry it in tension or compression.
const arm = Math.abs((joint[0] - foot[0]) * sd[1] - (joint[1] - foot[1]) * sd[0]);
const stayEach = LOAD * (topZ - foot[1]) / arm / 2;
const euler = Math.PI ** 2 * E * stay.I / (stayLen * stayLen);
console.log(`Top X ${top[0].toFixed(2)}, joint ${joint.map(v => v.toFixed(2))}; stay ${(Math.atan2(sd[1], sd[0]) / R).toFixed(1)} deg, ${stayLen.toFixed(1)} in long`);
console.log(`Fore-aft ${LOAD} lbf: each stay ${stayEach.toFixed(0)} lbf, vertical at each mount ${(stayEach * sd[1]).toFixed(0)} lbf, buckling factor ${(euler / stayEach).toFixed(1)}`);

// The lower leg is bent, so its load runs off the straight line between the foot pin and the stay joint. No weld here: mandrel bend only.
const fl = Math.hypot(joint[0] - foot[0], joint[1] - foot[1]);
const off = Math.abs((kink[0] - foot[0]) * (joint[1] - foot[1]) - (kink[1] - foot[1]) * (joint[0] - foot[0])) / fl;
const leg = Math.hypot(LOAD / 2 - stayEach * sd[0], stayEach * sd[1]);
const mKink = leg * off;
console.log(`Dog-leg bend: offset ${off.toFixed(2)} in, leg ${leg.toFixed(0)} lbf, ${mKink.toFixed(0)} in-lb, factor ${(hoop.S * FTY / mKink).toFixed(1)} to yield; ${(hoop.S * FTY / mKink / 2).toFixed(1)} with the whole load on one side; ${(hoop.S * FTY_HAZ / mKink / 2).toFixed(2)} if it were a welded miter`);

// Load at one top corner: the worst case for the leg above the stay joint. The saddle ends at the bend tangent, 4 in below the top.
const mCorner = LOAD * 4;
console.log(`Corner ${LOAD} lbf: ${mCorner} in-lb at the saddle end, factor ${(hoop.S * FTY_HAZ / mCorner).toFixed(2)} to weld-zone yield, ${(hoop.S * FTU_HAZ / mCorner).toFixed(2)} to rupture`);
// Load at the center of the top bar: bending as a straight span, plus twist in the legs from the bow.
const mBar = LOAD * 10.5 / 4;
console.log(`Top bar center: ${mBar.toFixed(0)} in-lb, factor ${(hoop.S * FTY / mBar).toFixed(1)} to yield; twist from the bow ${(LOAD * bow / 2).toFixed(0)} in-lb per leg`);

// Side load at the top, taken by one cable from just below the stay joint to just above the opposite stay foot.
const cableRun = stayLen - 3;
const cableLen = Math.hypot(span, cableRun);
const tension = LOAD * cableLen / span;
const uplift = LOAD * (topZ - stayFoot[1]) / span;
console.log(`Side ${LOAD} lbf: cable ${cableLen.toFixed(1)} in, ${tension.toFixed(0)} lbf, factor ${(1760 / tension).toFixed(1)} on 1/8 in 7x19 (1,760 lb break); uplift on one stay foot ${uplift.toFixed(0)} lbf`);
// Side shear at a stay foot: pin 0.65 in above the flange top, two 3/16 x 1-1/4 in cheeks tied by the pin. No welds in the clip.
const sCheeks = 2 * 1.25 * 0.1875 ** 2 / 6;
console.log(`Stay clip side load: factor ${(sCheeks * FTY / (LOAD * 0.65)).toFixed(1)} to yield`);

// Sling: 17 in between the legs, about 14.5 in tall. Inward pull on one leg, from a normal load P at sag s.
const slingH = upperLen - 1.2 - 4;
for (const [P, s] of [[100, 1.5], [300, 1.5]]) {
  const w = P / 17, H = w * 17 * 17 / (8 * s);
  console.log(`Sling ${P} lbf at ${s} in sag: ${H.toFixed(0)} lbf inward per leg, ${(H / slingH).toFixed(1)} lb per inch of fabric`);
}
const pIn = 450, L = 22; // 300 lbf sling load plus lacing tension, on the leg between the bench and the top
console.log(`Leg inward ${pIn} lbf over ${L} in: factor ${(hoop.S * FTY / (pIn * L / 8)).toFixed(1)}, deflection ${(5 * pIn * L ** 3 / (384 * E * hoop.I)).toFixed(3)} in`);

// Weight
const lowerLen = Math.hypot(kink[0] - foot[0], kink[1] - foot[1]) + 0.65;
const topArc = 23.6; // from the CAD path, corners and bow
const hoopIn = 2 * (lowerLen + upperLen - 4) + topArc;
const items = [
  ['Hoop, 1-1/2 x 0.083 6061', hoopIn * hoop.lbIn],
  ['Stays, 1 x 0.065 6061', 2 * (stayLen + 0.6) * stay.lbIn],
  ['Saddles at the stay joints, 3 in of 1/8 in', 2 * 3 * Math.PI * 1.625 * 0.125 * LB_IN3],
  ['Cables, four swaged ends, two turnbuckles, four eye tabs', 0.6],
  ['Foot clips, 2 x 2 x 1/8 x 1-1/2 in, 4', 4 * 1.5 * 0.484 * LB_IN3],
  ['Stay clips, 2 x 2 x 3/16 x 1-1/4 in, 4', 4 * 1.25 * 0.71 * LB_IN3],
  ['Pins, clips, lanyards, crush sleeves', 0.4],
  ['Rivnuts and bolts, 16 sets', 0.3],
  ['Sling mesh, hems, grommets, lacing', 0.5],
];
let sum = 0;
for (const [n, lb] of items) { sum += lb; console.log(`  ${n}: ${lb.toFixed(2)} lb`); }
console.log(`Total ${sum.toFixed(1)} lb, hoop ${hoopIn.toFixed(0)} in`);
