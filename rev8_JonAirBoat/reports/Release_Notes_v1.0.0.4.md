# DIY Jon-Air Airboat — Release Notes v1.0.0.4

> **In development: not yet released.** Branch `study/minimal-1448`, from `main` (v1.0.0.2, Lowe 1448 hull). Updated as the work proceeds.

**Release date:** TBD
**CAD source:** Onshape document "DIY Jon-Air boat", version **v1.0.0.4** (to be created at release)

## What changed since v1.0.0.2
- `VERSION` file added (1.0.0.4): the next release from this line of work carries that number. (Versions 1.0.0.3 and the 1652/1860 work live on the `rev8/1652-hull` and `rev8/1860-hull` branches.)
- **Decision: the minimal build uses a 1000 cc sport-bike engine** (for example a GSX-R1000 K5–K8) with its gearbox and a belt reduction.
- **Build tracker v1.0.0.4** (from v1.0.0.2): BOM line 30 bike engine ($2,000), 31 belt reduction ($600), 32 3-blade prop, 33 radiator, 34 exhaust, 35 fuel system with an inline EFI pump, 36 small lithium battery; lines 9–16 (chine angles, bow skin, their hardware, deck ports) set to qty 0; new lines 43–46 (cage, rudders and linkage, raised seat, navigation lights). BOM total about $11,100. Build Guide 1.3 and 1.4 marked not in the minimal build; the Section 2 header explains that steps 2.1–2.15 still describe the G13 module.
- **Engine stand and belt reduction, first design** (`reports/Bike_Engine_Stand_and_Reduction.md`, calc `CAD/analysis/bike_reduction.js`): gearbox locked in 4th, 14MGT belt 30T/47T, 1-1/2 in 4140 prop shaft, 66 in 3-blade prop at about 2,600 rpm (tip Mach 0.67), prop axis Z 52, bolted 6061 stand. Engine system and stand about 255 lb, so the minimal build now has about **345 lb** of payload (was 375 lb). **Clutch:** the bike clutch stays, worked by a lever on the steering stick with a parking lock and the clutch start switch (new BOM line 47, about $90; BOM total about $11,200).
- **Minimal 1448 concept study** (`reports/Minimal_1448_Study.md`, model `CAD/analysis/minimal_1448.js`). Keep the 1448's factory benches and replace the car-engine outfit with a light engine on a stand, a smaller cage, a simple cockpit and thin armor. Estimates: 365–450 lb of equipment and **375–460 lb of payload** under the 825 lb plate, against about 80 lb of driver gear on the rev8 1652 design.
  - **Rotax 582 with gearbox:** about 30 mph solo, 28 mph with a passenger, about $12,000 total at US used-engine prices (about $10,000–11,000 with a cheaper hull).
  - **1000 cc sport-bike engine with a belt reduction:** about 38 mph with any load, about $10,750 (about $9,000–10,000 with a cheaper hull).

## Known issues and open items
- Outfit weights, thrust and prices are estimates; nothing is modeled in CAD yet.
- Both engines exceed the 1448 plate's 25 hp outboard rating (registration and insurance).
- Running trim is flat (1.0–1.9°) with the prop axis where the model puts it; check a lower or tilted axis once a prop is chosen.
- The Rotax 582 has a 300 h / 5 year rebuild interval.

## Still to do before release
- Choose the specific bike engine and which rev8 features, if any, come back.
- Settle the open items in the stand and reduction note (primary ratio, belt width, thrust bearing, prop), design the cage and radiator mounting, then rewrite Build Guide steps 2.1–2.15 and model it in CAD.
