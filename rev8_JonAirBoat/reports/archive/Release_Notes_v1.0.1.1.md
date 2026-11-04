# DIY Jon-Air Airboat — Release Notes v1.0.1.1

> **Second beta.** Branch `main` (Lowe 1448 hull). The design is complete enough to review and start building from, but nothing here has been built, weighed or run. Weights, thrust, prices and hand calculations are estimates, and the items under "Open at beta" need checking before you cut metal.

**Release date:** 2026-11-04
**CAD source:** Onshape document "DIY Jon-Air boat", version **v1.0.1.1-beta**

## Package contents
- **Tracker:** `Airboat Build Tracker v1.0.1.1.xlsx`, with 83 BOM lines, BOM total about **$15,755**.
- **Build Guide:** `Airboat Build Guide v1.0.1.1.pdf`, made from the tracker's Build Guide tab by `reports/make_build_guide.py`. About 201 DIY hours and 96 pro hours.
- **CAD:** `CAD/Airboat_v1.0.1.1_*.step` (Assembly, Stand_engine_cage, Cockpit, Controls, Dash, Wiring), FeatureScript masters in `CAD/FeatureScript/`, and the calcs in `CAD/analysis/`.
- **Electrical:** `electrical/Minimal_1448_Electrical_Schematic.pdf`, `electrical/Wire_List.csv` and the generator `electrical/wire_list.py`.
- **Reports:** structural, stability, flotation and electrical compliance reports, the ABYC E-11 review, the drivetrain report, the static test plan, protocol and report, and the questions for makers, in `reports/`. Previous release notes and the two early design studies are in `reports/archive/`.
- **Renders:** `renderedImages/buildSteps/`, `renderedImages/studio/` (views v1 to v6 with and without the driver, and v8, the dash), the `Minimal_*_v1.0.1.1.png` part views, and `renderedImages/scenes/`. The scenes are concept images: a CAD render on a drawn background, made by `renderedImages/make_scenes.py`.
- **Status at release:** equipment about **471.9 lb**, payload about **353.1 lb**, about **+3.1 lb of margin** with driver + 150 lb of gear; about 34.5 mph; about 497 lbf of static thrust (estimates).

## What changed since v1.0.1.0
Same design and updated build documents, plus the two additions listed first.

- **Bubble level and load label:** a 2-3/4 in round bubble level on a shelf on top of the dash pod, so the driver can see list and trim from the seat and move gear to level the boat. Beside it, a label: maximum load 395 lb for people, gear and fuel. It also gives the tank weight, 45 lb full and about 8 lb empty, a 37 lb difference. The empty figure is implied by 6 gal of gas at about 6.1 lb/gal: weigh the real tank. Adds 0.3 lb and about $45.
- **UHMW:** 1/8 in over the whole bottom, transom to bow, in place of the three 3/16 in strips. Adds 3.6 lb. It uses 100° flat-head screws and adds rows at about X 3, X 140 and X 155, where there is no rib, the last two on backing bars. Step 1.2, step 2.6 and the stability and flotation numbers are updated
- **Build steps assembly (Onshape):** brought up to date with the rail, flotation foam, oil tank, side lights, battery switch, low oil light and harness conduit. The passenger is removed.
- **Step images:** redone or new for steps 1.2, 2.1, 2.3, 2.5, 2.11 and 2.13. Step 2.17 uses the flotation layout drawing.
- **Build Guide text:** step 1.2 rewritten for the whole-bottom UHMW sheet. Step 2.0 now gives the Warp Drive limits and the 7,400 to 7,500 rpm pitch target. Step 2.1 adds the bench foam check. Belt tension is in one place in step 2.8. The sign-off in step 2.14 now comes after steps 2.16 and 2.17.
- **Images:** the scenes, dash view and part views redone from the current CAD. All passenger images removed.
- **Steps 1.3 and 1.4:** cleared from the tracker, because the chine angles and bow skin are not part of this build. Their BOM lines stay at quantity 0.
- **Wiring CAD:** the 12 V outlet connector marker now shows a ring terminal, matching the wire list.
- **Archive:** superseded trackers and old STEP files deleted. Previous release notes and the two early design studies are in `reports/archive/`.

## Still open: can't be closed from a desk
**On the real boat**
- **UHMW sheet:** check that you can reach under the front shelf for the bow rows, test the countersink on an offcut, and watch the thin sheet for bubbling between ribs on a hot day. It has less wear life than 3/16 in.
- **Grab rail:** check the ribs at X 54.75 and X 70.65 (up to about 478 lbf of uplift), try the 12° lean seated with the stick at full travel, then proof-pull at 300 lb.
- What is inside the middle bench, front deck strength under the fuel tank, rib heights at the stick and console, and the pump float height against the conduit.
- Weigh the engine, and do the stand tap test.

**BRP service manual:** not found online. PTO taper and side load, starter draw, charging output and regulator, oil sensor pinout, oil line and pump wiring, engine-side wiring, fuel pump current, and coolant and exhaust temperature limits. Questions are in `reports/Supplier_Questions_v1.0.1.1.md`.

**Replies from makers**
- **Warp Drive:** approve the 800R and belt combination.
- **Gates:** confirm the 239 hp rating at 7,600 rpm, and advise on torsional vibration. DesignFlex Pro could not be reached.

**Paid standards:** the full text of E-11 (2025) and E-13 (2025) was not read.

## Other notes
- The sling is a flat sheet in CAD. The engine, pipe and radiator are placeholder envelopes until measured.
- The engine is far above the 1448 plate's 25 hp outboard rating (registration and insurance).
- Budget a two-stroke top-end rebuild at the service manual's interval.
