// Installation tension for the 8MGT-2000-36 Poly Chain GT Carbon belt, from the Gates drive design manual
// (Formulas 14 to 16, Table 11; manual pages 103-104). Run: node belt_tension.js
const hp = 120, rpm = 7600, pd = 3.208, PD = 9.423, C = 29.28, L = 78.74;
const M = 0.97, Y = 194, minTst = 84;      // Table 11, 8 mm pitch, 36 mm wide
const S = pd * rpm / 3820;
const Tst = Math.max(20 * hp / S + M * S * S, minTst);
const t = Math.sqrt(C * C - ((PD - pd) / 2) ** 2);
const fMin = (1.1 * Tst + t / L * Y) / 16, fMax = (1.2 * Tst + t / L * Y) / 16;
console.log(`Base static tension ${Tst.toFixed(0)} lb; new belt ${(1.1 * Tst).toFixed(0)} to ${(1.2 * Tst).toFixed(0)} lb per span`);
console.log(`Used belt ${(0.8 * Tst).toFixed(0)} to ${(0.9 * Tst).toFixed(0)} lb`);
console.log(`Span ${t.toFixed(2)} in: deflect ${(t / 64).toFixed(2)} in with ${fMin.toFixed(1)} to ${fMax.toFixed(1)} lb at mid-span`);
console.log(`Static side load on the shafts, about ${(2 * 1.2 * Tst).toFixed(0)} lb`);
