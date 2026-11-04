# DIY Jon-Air Airboat — Release Notes v1.0.1.2

> **Third beta.** Branch `main` (Lowe 1448 hull). The design is complete enough to review and start building from, but nothing here has been built, weighed or run. Weights, thrust, prices and hand calculations are estimates, and the items under "Open at beta" need checking before you cut metal.

**Release date:** 2026-11-04
**CAD source:** Onshape document "DIY Jon-Air boat", version **v1.0.1.2**

## Package contents
- **Tracker:** `Airboat Build Tracker v1.0.1.2.xlsx`, with 83 BOM lines, BOM total about **$15,755**.
- **Build Guide:** `Airboat Build Guide v1.0.1.2.pdf`, made from the tracker's Build Guide tab by `reports/make_build_guide.py`. About 201 DIY hours and 96 pro hours.
- **CAD:** `CAD/Airboat_v1.0.1.2_*.step` (Assembly, Stand_engine_cage, Cockpit, Controls, Dash, Wiring), FeatureScript masters in `CAD/FeatureScript/`, and the calcs in `CAD/analysis/`.
- **Electrical:** `electrical/Minimal_1448_Electrical_Schematic.pdf`, `electrical/Wire_List.csv` and the generator `electrical/wire_list.py`.
- **Reports:** structural, stability, flotation and electrical compliance reports, the ABYC E-11 review, the drivetrain report, the static test plan, protocol and report, and the questions for makers, in `reports/`. Previous release notes and the two early design studies are in `reports/archive/`.
- **Renders:** `renderedImages/buildSteps/`, `renderedImages/studio/` (views v1 to v6 with and without the driver, and v8, the dash), the `Minimal_*_v1.0.1.2.png` part views, and `renderedImages/scenes/`. The scenes are concept images: a CAD render on a drawn background, made by `renderedImages/make_scenes.py`.
- **Status at release:** equipment about **471.9 lb**, payload about **353.1 lb**, about **+3.1 lb of margin** with driver + 150 lb of gear; about 34.5 mph; about 497 lbf of static thrust (estimates).

## What changed since v1.0.1.2
Same design, with the tracker reorganized for sharing the work and the cost.

- **Build Guide tab:** each step now has a material part and a labor part. Material: estimated cost, and who pledged or paid. Labor: DIY hours, pro hours, pro cost, and who pledged or did the work. A BOM line that lists two steps counts in the first one, so the step costs add up to the BOM total.
- **BOM tab:** a new Pledged / paid by column next to the cost of each line.
- **Build Guide PDF:** each step opens with a one-row table of material cost, DIY hours, pro hours, pro cost if hired and total if hired. Each section has a totals row, and a summary table at the front adds them up.

## Still open: can't be closed from a desk
**On the real boat**
- **UHMW sheet:** check that you can reach under the front shelf for the bow rows, test the countersink on an offcut, and watch the thin sheet for bubbling between ribs on a hot day. It has less wear life than 3/16 in.
- **Grab rail:** check the ribs at X 54.75 and X 70.65 (up to about 478 lbf of uplift), try the 12° lean seated with the stick at full travel, then proof-pull at 300 lb.
- What is inside the middle bench, front deck strength under the fuel tank, rib heights at the stick and console, and the pump float height against the conduit.
- Weigh the engine, and do the stand tap test.

**BRP service manual:** not found online. PTO taper and side load, starter draw, charging output and regulator, oil sensor pinout, oil line and pump wiring, engine-side wiring, fuel pump current, and coolant and exhaust temperature limits. Questions are in `reports/Supplier_Questions_v1.0.1.2.md`.

**Replies from makers**
- **Warp Drive:** approve the 800R and belt combination.
- **Gates:** confirm the 239 hp rating at 7,600 rpm, and advise on torsional vibration. DesignFlex Pro could not be reached.

**Paid standards:** the full text of E-11 (2025) and E-13 (2025) was not read.

## Other notes
- The sling is a flat sheet in CAD. The engine, pipe and radiator are placeholder envelopes until measured.
- The engine is far above the 1448 plate's 25 hp outboard rating (registration and insurance).
- Budget a two-stroke top-end rebuild at the service manual's interval.
