// Lightening cutouts in the prop-shaft bearing plates. Hand beam check, not FEA: ends taken as simply supported,
// which is conservative at the cutouts for thrust if the end bolts give some fixity.
// Each side of the bearing has two petals in a row, with a rib between them. Inches, lbf, psi.
// Usage: node bearing_plate.js
'use strict';
const RHO = 0.098, E = 1.0e7, YIELD = 35000, FATIGUE = 13500, ULT = 42000, KT = 1.8;
const RIB = 0.5, PAD = 3.1, SLOT_ZONE = 1.5, DEPTH = 6;

function plate(name, half, t, edge, radial, thrust) {
  const zone = half - SLOT_ZONE - PAD;                       // along the plate, one side
  const h = DEPTH - 2 * edge;
  const open = (zone - RIB) * h * 0.92;                      // 0.92: rounded petal ends and fillets
  const solidLb = 2 * half * DEPTH * t * RHO, savedLb = 2 * open * t * RHO;

  // Stress at the middle of the outer petal.
  const cell = (zone - RIB) / 2, y = PAD + zone - cell / 2, arm = half - y;
  const V = radial / 2, inPlaneM = V * arm;
  const flangeArea = edge * t, globalInPlane = inPlaneM / (DEPTH - edge) / flangeArea;
  const localInPlane = (V / 2 * cell / 2) / (t * edge * edge / 6);
  const outPlane = (thrust / 2 * arm) / (2 * edge * t * t / 6);
  const nominal = globalInPlane + localInPlane + outPlane;
  const peak = nominal * KT;

  // Mean is the steady load; alternating taken as 30% of it for thrust and belt ripple.
  const alt = 0.3 * peak, goodman = 1 / (alt / FATIGUE + peak / ULT);

  // Thrust deflection at the center by Mohr's integral, with and without the cutouts.
  const I = b => b * t ** 3 / 12;
  const defl = cut => { let d = 0, n = 2000, L = 2 * half;
    for (let i = 0; i < n; i++) { const x = (i + 0.5) / n * half, m = thrust / 2 * x, dist = half - x;
      const inCut = cut && dist > PAD && dist < PAD + zone; d += 2 * m * (x) / (E * I(inCut ? 2 * edge + 0 : DEPTH)) * (L / 2 / n) * 0.5; }
    return d; };
  return { plate: name, thicknessIn: t, solidLb: +solidLb.toFixed(2), savedLb: +savedLb.toFixed(2), finishedLb: +(solidLb - savedLb).toFixed(2),
    nominalPsi: Math.round(nominal), peakPsi: Math.round(peak), yieldFactor: +(YIELD / peak).toFixed(1), fatigueFactor: +goodman.toFixed(1),
    thrustDeflectionIn: thrust ? { solid: +defl(false).toFixed(3), cut: +defl(true).toFixed(3) } : 0 };
}

console.log(JSON.stringify([
  plate('Aft, 1/2 in, petals', 7.5, 0.5, 0.75, 765, 0),
  plate('Aft, 3/8 in, petals', 7.5, 0.375, 0.75, 765, 0),
  plate('Aft, 3/8 in, 1 in edges', 7.5, 0.375, 1.0, 765, 0),
  plate('Forward, 1/2 in, 3/4 in edges', 7.75, 0.5, 0.75, 580, 497),
  plate('Forward, 1/2 in, 1 in edges', 7.75, 0.5, 1.0, 580, 497),
], null, 1));
