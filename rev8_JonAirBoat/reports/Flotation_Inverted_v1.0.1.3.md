# Capsized Flotation Analysis — v1.0.1.3

**Boat:** minimal 1448, Rotax 800R E-TEC, seat module (v1.0.1.3), Onshape version v1.0.1.3
**Condition:** upside down in fresh water, driver swimming, 6 gal tank full, 200 lb of gear strapped behind the driver. The gear load is heavier than the 135 lb design load, so it is conservative.
**Model:** `CAD/analysis/flotation_inverted.js` (`chosen()`)
**Status:** a hobby design calculation from estimated weights, with the rear bench foam as reported from inspection: full and sound. It has not been checked by a licensed professional or tested in the water. Use it at your own risk.

## What changed since v1.0.1.2

The middle bench is gone. Its foam (about 4.8 ft³) was most of the capsized boat's forward flotation: without it the boat sank stern first. The seat module and two footwell blocks replace it, all closed-cell and all inside the hull:

| Flotation | Size | Volume |
|---|---|---|
| Rear bench (factory foam, kept intact) | X 13.5–33, full width, Z 2.9–13.7 | 7.26 ft³ |
| Bow wedge | Under the front shelf, X 154–156 | 0.41 ft³ |
| Two aft corner blocks | X 3–13, Y 10.5 out to the hull side, Z 2–10 | 1.52 ft³ |
| Seat pad, 2-1/2 in closed-cell PE | 15 x 38 in, Z 11.6–14.1 | 0.82 ft³ |
| Seat compartment floors | Both compartments, Z 1.5–5 | 0.70 ft³ |
| **Footwell side blocks, 6 in thick** | X 98–128 against each hull side, Z 1.5–13; gap at X 106–110 starboard for the console foot | 2.14 ft³ |
| Folding seatback foam | 18 x 15 x 2-1/4 in, counted as an item with SG 0.14 | 0.35 ft³ |

The seatback was at first booked as nearly neutral (SG 0.5), which counted only about 3 lb of its lift. Counted as the closed-cell float it is, the capsized float height went from 3.0 in to 3.9 in.

## Result

The boat floats level upside down, like a raft:
- **Hull bottom out of the water:** about **3.9 in** (v1.0.1.2: 3.9 in).
- **Attitude:** list −0.3°, pitch −0.6°.
- **Reserve buoyancy:** about **190 lb**.

| Righting arm, in | 5° | 10° | 20° | 30° |
|---|---|---|---|---|
| Heel | 2.4 | 3.2 | 5.2 | 6.7 |

| Righting arm, in | 3° | 6° |
|---|---|---|
| Pitch | 5.9 | 9.0 |

All arms are positive, so the boat returns to level.

## Method

As in v1.0.1.2:
- **No credit** for air trapped under the hull or for the sealed dry box.
- Each item under water weighs its dry weight less the water it displaces. Foam is modeled in 1 in cubes.
- The float height, heel and pitch are found by balancing weight and buoyancy.

Since v1.0.1.2:
- **Oars:** operator gear, inside the 200 lb gear load.
- **Carbon struts and sandwich seat base:** booked at their composite densities.

## Alternatives checked

| Side block thickness | Foam weight | Hull bottom clear | Pitch | Reserve |
|---|---|---|---|---|
| **6 in (chosen)** | 4.3 lb | 3.9 in | level | 190 lb |
| 5 in | 3.6 lb | 3.0 in | level | 169 lb |
| 4 in | 2.9 lb | 2.9 in | 1° | 147 lb |
| none | 0 | 1.2 in | 13° bow down | 61 lb |

Thinner blocks save almost nothing: foam weighs 2 lb/ft³ but lifts 62 lb/ft³. The 6 in blocks stay (user decision, 2026-11-10). About 150 lb of reserve is the floor to keep.

## Build notes

- **Foam:** closed-cell, about 2 lb/ft³, fuel resistant. Use polyethylene, or XPS in a fuel-proof skin. That includes the seat pad and seatback.
- **Rear bench foam:** keep it intact, except the small access pocket; seal the cut faces.
- **Straps:** the footwell blocks are strapped to the hull side. The fire extinguisher stands on the starboard block.
- **Bilge:** compartment walls stop 1-1/2 in above the floor so water drains under. Keep the blocks clear of the pump and hose.
- **Trim:** the bow wedge is the fine adjustment. Trim it on the real boat.

## Limits of this analysis

- **Weights** are estimates until parts are weighed.
- **Rear bench foam** is taken as the full bench volume, from inspection. Recheck it now and then for water soaking.
- **Seatback attachment:** the seatback has to stay attached to count. It is bolted to brackets on the aft strut.
- **The capsize itself:** this is the settled condition only, not the capsize, a hot engine going under, or righting the boat.
- **Fuel:** a tank upside down can leak through its vent. Treat the water around a capsized boat as a fire risk.
- **Rules:** this isn't USCG flotation compliance.

## Next step

Roll the finished boat in shallow water, with a crane or helpers, and check the float attitude. Trim the wedge to suit.
