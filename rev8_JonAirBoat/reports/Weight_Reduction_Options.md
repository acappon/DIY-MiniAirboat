# Weight Reduction Options

**Date:** 2026-11-02. **Basis:** the equipment weight estimate for a 1652 hull, about **1,002 lb** (everything except the bare hull, crew and cargo), from `CAD/analysis/compare_hulls.js`.
**Goal:** keep the 400 lb design load (solo + gear, or driver + 1) within the 1652's ~1,216 lb capacity plate. That means equipment of **about 816 lb or less**, so roughly **185 lb** has to come out.
All weights are estimates; weigh the real parts as they are bought or built (requirement R7.4).

## Where the weight is (1652, baseline)

| Item | Weight (lb) | Share |
|---|---|---|
| Engine, dressed (G13) | 185 | 18% |
| Hull armor (UHMW, chine angles, bow skin, trim tab), scaled to the 1652 bottom | ~115 | 11% |
| A-frame, braces, peak plates (steel) | 100 | 10% |
| Prop cage (steel EMT, mesh) | 85 | 8% |
| Battery, Group 31 AGM | 70 | 7% |
| Mounts and cage supports | 65 | 6% |
| Redrive, belt, pulleys | 55 | 5% |
| Fuel, 6.5 gal + tank | 47 | 5% |
| Seating: driver seat, storage box, backrest bar | 80 | 8% |
| Table/dash, steering, rudders, radiator, exhaust, lights, wiring, safety gear, prop and shaft | ~200 | 20% |

## Options, ranked by pounds saved

| # | Change | Saves (lb) | Cost impact | DIY impact | Risks and notes |
|---|---|---|---|---|---|
| 1 | **A-frame in 6061-T6 aluminum** instead of steel: 2 x 2 x 1/4 in or 2-1/2 x 2-1/2 x 3/16 in tube legs (aluminum is a third as stiff, so the walls go up), 1/2 in aluminum peak plates | **40–50** | +$100–200 material | Aluminum welding needs TIG, so it becomes a shop task. **Or design it bolted**, with gussets and 3/8 in bolts, which is DIY. | Vibration fatigue at welds. Use generous gussets; inspect at every static and on-water test (R4.1). |
| 2 | **Lithium (LiFePO4) Group 31 battery** with cranking rating, about 25–30 lb | **40–45** | +$300–600 | None, a direct swap | It must be rated for engine starting, and the alternator charging must suit lithium; use a built-in BMS. Do not use it below about 32 °F unless it is rated for that. |
| 3 | **Aluminum prop cage**: 6061 or 6063 tube instead of steel EMT; keep the mesh | **30–40** | +$150–300 | The hoops must be rolled: 6061-T6 cracks when bent tight, so use 6063 or a local rolling shop. Then bolt or TIG the joints. | The cage is a safety part (R4.2, R5.4). Keep the tip clearance under load; check it in the static test. |
| 4 | **Lighter seating and furniture**: aluminum-frame storage box with a thin marine-ply or HDPE lid, a lightweight marine bucket seat (about 12 lb), an aluminum-tube backrest bar, and the table and dash in 0.063 in aluminum sheet | **30–40** | Neutral | DIY | Keep the storage box frame strong; it is the new thwart (R5.8). |
| 5 | **Armor only where it wears**: UHMW on the center strips and chine edges only, or 3/16 in instead of 1/4 in UHMW | **15–30** | Saves money | Less work | The bottom wears faster in the uncovered areas. UHMW is almost neutral in water, so this changes the capacity use, not flotation. |
| 6 | **Strip the engine**: tubular exhaust header instead of the cast manifold, smaller or lighter alternator, remove unused brackets and pumps, lighter flywheel if the redrive maker allows | **10–25** | Varies | DIY | Check what the redrive needs. Don't remove the flywheel mass the starter and redrive depend on without checking. |
| 7 | **Aluminum radiator** (all-aluminum or a small race radiator sized for about 70–100 hp) | **5–10** | +$100–200 | DIY | Cooling margin must pass R4.4 in the static and on-water tests. |
| 8 | **Carry less fuel** on short solo trips (half tank) | 20 (operational) | None | None | Keep a 1/3 reserve (R2.5). This doesn't count toward the plate math for full-tank trips. |
| 9 | **Lighter gear** | Up to you | — | — | The 200 lb gear allowance is the largest single item you control. |

## Packages

| Package | Includes | Saves | Equipment total | Design-load plate use (1652) |
|---|---|---|---|---|
| Baseline | — | 0 | ~1,002 lb | 115% |
| **Lightened** (used in the hull comparison) | 1 + 2 + 3 (A-frame, battery, cage) | ~120 | ~882 lb | 105% |
| **Recommended** | 1 + 2 + 3 + 4 + 5 + 6 | ~170–230 | ~775–830 lb | **97–102%** |
| Maximum | All of the above + 7 | ~180–245 | ~760–820 lb | 95–101% |

**Recommendation:** adopt the **Recommended** package for Prototype A, and put the bolted aluminum A-frame (option 1) first in line, since it keeps the build DIY. That brings the 1652 to about its plate at the design load. It also raises Prototype A's thrust margin when getting on plane, which the hull comparison shows is the limiting factor for the G13.

## Decisions (2026-11-02) and where they leave the weight

| Option | Decision | Saves |
|---|---|---|
| 1. Aluminum A-frame (bolted, DIY) | Open; recommended | 40–50 |
| 2. Battery | **Keep the capacity: sized above starting needs, so the 12 V outlet can be used and still leave enough to start.** A same-size lithium (LiFePO4) Group 31 is still an option: its usable capacity is about 80–90% versus about 50% for AGM, so it gives more reserve for the outlet at about 40 lb less. | 0 with AGM (40–45 with lithium) |
| 3. Prop cage | **Keep steel.** Protection against branches is the priority. | 0 |
| 4. Lighter interior | Open; recommended | 30–40 |
| 5. Armor | **3/16 in UHMW everywhere** instead of 1/4 in (full coverage kept) | ~18 |
| 6. Strip the engine | Open; recommended | 10–25 |
| 7. Radiator | **Keep the Honda Civic radiator.** It is already aluminum core with plastic tanks, so there is nothing to gain. | 0 |

| Result (1652, design load solo + 200 lb gear or driver + 1) | Saves | Equipment | Plate use |
|---|---|---|---|
| Options 1, 4, 5, 6, AGM battery | ~100–135 lb | ~870–905 lb | **~104–107%** |
| Same, with a lithium Group 31 | ~140–180 lb | ~825–865 lb | ~101–104% |
| Solo, no gear (either battery) | | | ~82–88% |

The design load ends up about 4–7% over the plate, about 50–90 lb. Ways to close it:
- **(a)** Accept the small overrun deliberately and stow the heaviest gear low and centered.
- **(b)** Set the planning gear allowance to about 120–150 lb instead of 200.
- **(c)** Use the lithium battery.

## Maximum payload (driver + gear) on a 1652, all cuts made (2026-11-02)

The wide storage box is kept. All the remaining cuts are taken (options 1, 4, 5 and 6) with the AGM battery, which gives equipment of about **870–905 lb**. Detail design always adds weight, so a growth allowance is carried on top:

| Growth allowance on equipment | Equipment | Max driver + gear within the 1,216 lb plate | With a 200 lb driver, gear up to |
|---|---|---|---|
| None (today's estimate) | 870–905 lb | 310–345 lb | 110–145 lb |
| **5% (recommended planning value)** | **915–950 lb** | **265–300 lb** | **65–100 lb** |
| 10% | 955–995 lb | 220–260 lb | 20–60 lb |

A lithium Group 31 battery would add about 40 lb to each of these limits.

**Planning value: about 280 lb of driver + gear** (a 200 lb driver with about 80 lb of gear), until real parts are weighed. Track the actual weight in a running weight budget as parts are bought or built (R7.4, R7.5).

## What it changes in the project
- **CAD:** A-frame and cage material, wall thicknesses and joints (drivetrain and cage Part Studios). Seat, box and dash parts in the new interior.
- **BOM:** the A-frame, cage, battery, seat and box lines, plus the armor coverage line.
- **Build Guide:** the A-frame build (2.8) becomes bolted aluminum, and the cage (2.12) gains a tube-rolling step.
- **Requirements:** R7.5 (weight budget) gets a weigh-in at each build stage.

## Running weight log (driver gear allowance)

Design payload is 280 lb of driver + gear on a 1652 (R1.1, R7.5). Every change below moves the driver's gear allowance pound for pound.

| Date | Change | Weight effect vs the plan | Driver gear allowance |
|---|---|---|---|
| 2026-11-02 | Starting point (200 lb driver) | — | ~80 lb |
| 2026-11-02 | Transom notch cut and capped with aluminum channel (1652 refit) | +1.5 lb | ~78 lb |
| 2026-11-02 | **Aluminum A-frame modeled in CAD.** The steel rev7 A-frame measured **~145 lb** in CAD, not the 100 lb estimated. The bolted aluminum version is 76.7 lb of 6061 (22 parts) plus ~4 lb of bolts, about **81 lb**. That is ~64 lb lighter than the steel, but ~26 lb heavier than the 55 lb this plan had assumed. | +26 lb | **~52 lb** |

Notes:
- The 100 lb steel estimate was low, so other drivetrain estimates may be off too. A full CAD weigh-in (materials assigned to every part) should replace the estimates before the next release.
- Ways to win back A-frame weight:
  - Pocket the 1/2 in peak plates with lightening holes (about 5–8 lb).
  - Use 3/8 in aluminum peak plates (about 6.5 lb). The plates take mostly in-plane loads, but check the bearing-flange bolt areas first.
