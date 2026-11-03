# DIY Jon-Air Airboat — Release Notes v1.0.0.4

> **In development: not yet released.** Updated as the work proceeds.

**Release date:** TBD
**CAD source:** Onshape document "DIY Jon-Air boat", version **v1.0.0.4** (to be created at release)

## What changed since the previous release

### Prototype C added (G13B with a low-boost turbo)
- A third prototype for comparison: the G13B with a turbo at about 7–8 psi, estimated at about 140 hp and 550 lbf of static thrust. No airboat-specific G13B turbo kit was found; Swift GTi car kits and parts exist.
- New analysis `CAD/analysis/prototypes_1652.js` compares A, B and C on the 1652 hull (Trim_Stability_Seating_Analysis.md, Section 13). Estimated top speed solo: about 24.5 mph (A), 30 mph (B), 34 mph (C). C planes easily with any load, but runs below the 2° running-trim target, has about twice A's prop torque, and leaves about 39 lb of driver gear allowance.
- Requirements updated: scope, R2.1 (C thrust at least 20% over B), R2.3 (C at least 30 mph solo), R3.7, R3.8, new R4.8 (boost limit, standalone ECU, temperatures) and R4.9 (drivetrain rated for C's power), R7.3, R7.4, and a new open question on C's speed goal.

## Known issues and open items
- `trim_stability.js` and Sections 1–11 of the trim report still model the 1448 hull. The 1652 numbers are in Sections 12 and 13.
- Prototype C power, thrust and weight are estimates. The redrive, prop, A-frame and mounts have not been checked for 140 hp (R4.9).

## Still to do before release
- Fuel tank: a fixed tank in front of the bow bench with a gunwale fill and overboard vent (size to be chosen; 12 gal filled to need is recommended). Modeling has started (`CAD/FeatureScript/Fuel_tank.fs`, currently a 6.6 gal portable tank aft of the bench, to be moved).
- Starboard trim tab (sized for the largest prototype that will be built).
