# DIY Jon-Air Airboat — Release Notes v1.0.0.4

> **In development: not yet released.** Updated as the work proceeds. Branch **rev8/1860-hull**.

**Release date:** TBD
**CAD source:** Onshape document "DIY Jon-Air boat", version **v1.0.0.4** (to be created at release)

## What changed since the previous release

### One airboat: Toyota 2GR-FE V6 on an 1860-class hull
- **Why:** on the 1652, the 70–100 hp Suzuki 1.3 L engines left only about 40–80 lb for the driver's gear, so every pound of cargo was noticeable. An engine study (Trim_Stability_Seating_Analysis.md, Section 13) compared seven engines on an 18 ft hull.
- **Engine:** Toyota 2GR-FE 3.5 L V6, about 270 hp, about 370 lb dressed, very common in junkyards (Avalon, Camry V6, RAV4 V6, Highlander, Sienna, Venza, Lexus ES/RX). It replaces both the G13 (Prototype A) and the G13B (Prototype B); the A/B prototype plan, and the Prototype C (G13B turbo) idea, are dropped.
- **Hull:** an 1860-class jonboat (about 18 ft, 60 in bottom, 84 in beam, plate about 1,550 lb). The CAD hull is still the 1652 placeholder until it is refit.
- **Model estimate** (`CAD/analysis/performance_1860.js`, replaces `prototypes_1652.js`): about 38.5 mph solo and 37 mph with two passengers, a large thrust margin to get on plane, and about 337 lb of payload under the plate (driver plus about 137 lb of gear). ±25% until the static test.
- **Requirements:** scope, R1.1, R1.2, R1.8, R1.9, R2.1 (750 lbf), R2.3 (35 mph), R2.5, R2.6, R3.6, R3.7, R3.8, R4.8 (engine computer), R4.9 (drivetrain rated for 270 hp), R5.7 (flagged for redo), R7.2, R7.3, R7.4, R7.5, and new open questions on speed goals and the plate's horsepower rating.
- **Build tracker:** Read Me; BOM line 1 (used 1860 hull, about $5,500), line 30 (2GR engine), new lines 43 (standalone ECU and harness) and 44 (EFI fuel supply); the Prototype B lines are gone (later lines renumbered by one); redrive (31) and prop (32) are marked on hold. Build Guide 1.1 (finding a used 1860) and the Section 2 engine commentary are rewritten; step 2.20 notes the 2GR's electronic throttle; the Prototype B on-water test (4.3) is removed.

## Known issues and open items
- **Not yet updated for the 2GR (on hold until the 1860 branch settles):** redrive ratio, pulleys and belt, prop, cage size, engine mounts, A-frame and the drivetrain CAD. Build Guide Section 2 still describes the earlier 1.3 L drivetrain and 1652 dimensions.
- **Hull CAD and hull-dependent parts** (armor layout, interior rails, mounts) are still for the 1652 placeholder.
- **Running trim is too flat** (about 1.5°) with about 850 lbf on the high thrust line; the 2GR drivetrain needs a lower or down-tilted thrust line (R3.7).
- **Dock trim is slightly stern-heavy** (about 2.1°, R3.6).
- **BOM total is about $19,100**, above the R7.2 target of $11,000 (the redrive and prop prices are not yet updated).
- **The 2GR's electronic throttle** has to be matched to the pedal and cable controls (R4.8, Build Guide 2.20).
- `trim_stability.js` and Sections 1–12 of the trim report are history (1448/1652 hulls, earlier engines).

## Still to do before release
- Fuel tank: a fixed tank in front of the bow bench with a gunwale fill and overboard vent, sized for the 2GR's fuel burn. Modeling has started (`CAD/FeatureScript/Fuel_tank.fs`, currently a 6.6 gal portable tank aft of the bench, to be moved).
- Starboard trim tab, sized for the 2GR's prop torque (about 620 ft-lb).
