# DIY Jon-Air Airboat — Release Notes v1.0.1.0

> **First beta.** Branch `main` (Lowe 1448 hull). The design is complete enough to review and start building from, but nothing here has been built, weighed or run. Weights, thrust, prices and hand calculations are estimates, and the items under "Open at beta" need checking before you cut metal.

**Release date:** 2026-11-04
**CAD source:** Onshape document "DIY Jon-Air boat", version **v1.0.1.0-structural**

## Package contents
- **Tracker:** `Airboat Build Tracker v1.0.1.0.xlsx`, with 81 BOM lines, BOM total about **$15,690**.
- **Build Guide:** `Airboat Build Guide v1.0.1.0.pdf`, made from the tracker's Build Guide tab by `reports/make_build_guide.py`. About 199 DIY hours and 95 pro hours.
- **CAD:** `CAD/Airboat_v1.0.1.0_*.step` (Assembly, Stand_engine_cage, Cockpit, Controls, Dash, Wiring), FeatureScript masters in `CAD/FeatureScript/`, and the calcs in `CAD/analysis/`.
- **Electrical:** `electrical/Minimal_1448_Electrical_Schematic.pdf`, `electrical/Wire_List.csv` and the generator `electrical/wire_list.py`.
- **Reports:** structural, stability, flotation and electrical compliance reports, the ABYC E-11 review, the drivetrain report, the static test plan, protocol and report, and the questions for makers, in `reports/`, in `reports/`. Earlier reports, older trackers and older STEP files are in `reports/archive/`.
- **Renders:** `renderedImages/buildSteps/` and `renderedImages/studio/`. Studio views v1 to v6 are new for this release, with and without the driver.
- **Status at release:** equipment about **468.0 lb**, payload about **357.0 lb**, about **+7.0 lb of margin** with driver + 150 lb of gear; about 34.5 mph; about 497 lbf of static thrust (estimates).

## What changed since v1.0.0.4
- `VERSION` set to 1.0.1.0, the first beta release; new tracker copied from the v1.0.0.4 release.
- **Starboard hull clip fixed (`CAD/FeatureScript/Minimal_cockpit.fs`, Onshape Main workspace):** the streaks noted in v1.0.0.4 were the throttle cable and fuel line, not a wire. Both ran straight at about Y -31 while the hull tapers in from about X 75, so they came through the skin at about X 95-115. Both now follow the taper, about 0.65 in clear of the hull at the closest point (throttle cable near X 110). No part count or weight change. `electrical/wire_list.py` needed no change.
- **ABYC E-11 wire size and colors (`reports/ABYC_E11_Review_v1.0.1.0.md`):** general wiring goes from 18 to 16 AWG, and wire colors now follow E-11 Tables XIV and XV. Schematic, wire list and the Onshape wiring are regenerated from `electrical/wire_list.py`. BOM lines 67 and 68 updated.
- **Battery switch and dash bars:** a battery switch on the middle bench forward panel now feeds the starter (W01) and hub (W03), with a new cable W57 from the battery. MF1 moves to the switch output stud, and the automatic bilge pump (F7) is fused at the battery, ahead of the switch. TB1 and TB2 are screw-terminal bars with one wire per screw. BOM lines 58, 65, 66, 71 and 73 updated, new line 76.
- **Starter solenoid:** Cole Hersee 24059-08-BP replaces the sled part for E-11 ignition protection (BOM line 77). Harness 13.9 lb: equipment about **472.6 lb**, payload about **352.4 lb**, about **+2.4 lb of margin**.
- **Sidelights replace the bow light:** red and green lights on the hull sides at X 108, about 3 ft from the tank and fuel fittings, so they meet E-11 isolation (BOM line 46). New wires W58 and W59 serve the starboard light.
- **Oil tank and low oil light:** the sled oil tank is placed on the starboard side frame of the stand (envelope until measured). The stock oil level sensor drives a red LED on the dash between the temperature and exhaust gauges (W60 to W62, BOM line 78). BOM line 78 is the LED.
- **Harness routing:** the floor run rises to Z 4 in 2 in self-draining conduit on rib standoffs, and passes through the bench foam in an aluminum sleeve (BOM line 73). Keep gear off the conduit.
- **Belt:** Poly Chain GT Carbon 8MGT-2000-36, 36 mm wide, service factor about 2.0 at 120 hp (extended past the Gates tables). The engine sits 0.72 in higher for 29.28 in centers.
- **Radiator:** 18 x 22 x 2 in, moved to X 15–17 just ahead of the prop. Margin after this was about +0.1 lb, which led to the weight pass below.
- **Weight pass:** navigation lights re-estimated (-3.5 lb), LED bar 30 to 20 in (-4), battery cables 1/0 to 2 AWG with a 6 AWG link (-2.5), inflatable PFDs (-2), can air horn (-1.5), fiberglass flag whip (-1), lighter phone holder (-0.7). F2 is now 10 A.
- **Petal bearing plates:** CNC or waterjet cut, 3/8 in aft and 1/2 in forward, about 2.7 lb lighter (BOM line 31, about $160). Equipment about **457.0 lb**, payload about **368.0 lb**, about **+18 lb of margin**; BOM total about **$15,040**.
- **Build Guide rewritten:** steps 2.1–2.15 now describe the current build: shoes, bolted stand, petal plates, 800R module, all systems, cage, stick and rudders. `step2.11_systems_connected.png` is linked. New `Airboat Build Guide v1.0.1.0.pdf`. 188 DIY hours and 89.5 pro hours (estimates).
- **Static test documents updated for the 800R:** the plan, protocol and report now cover the shoes, stand, petal plates, belt and injected oil. Oil pressure checks are replaced by exhaust temperature, the low oil light and the tank level. New items: stand tap test, pitch setting, belt flutter, the second muffler decision, and a 1 minute limit at full throttle. Restraint raised to 2,500 lb, and lockout now uses the battery switch.
- **Removable grab rail and sling backrest:** a 1-1/2 in hoop, 20 in wide, behind the driver. The legs bend forward to just behind the bench, then lean back 12° to Z 32, with a 2 in aft bow in the top and laced mesh sling across the front. Stays and crossed cables behind it, on four pins with lanyards (BOM line 79, about $475; Build Guide 2.16). It mounts on the ribs at X 54.75 and X 70.65, with no welds to the hull and no holes through the bottom. Sized for 300 lbf (`CAD/analysis/grab_rail.js`). Adds 6.8 lb: equipment about **463.8 lb**, payload about **361.2 lb**, about **+11.2 lb of margin**; BOM total about **$15,515**.

## Closed out before release
- **Prop:** Warp Drive's HP manual gives 3,200 prop rpm, 160 hp and 62–72 in, so the prop runs at about 81% of its rpm limit. Static target is now 7,400–7,500 rpm, per the maker, with torques and check intervals in Build Guide 2.15.
- **Belt:** tension is now 457–499 lb, or 33–36 lb for 0.45 in of deflection (`CAD/analysis/belt_tension.js`), in Build Guide 2.8 and the protocol.
- **E-11:** sled engine deviation accepted; battery, switch, sender and bilge fuse specified; F7 down to 5 A; W05 and W06 on ring terminals. See "Closed out for v1.0.1.0" in the review.
- **E-13 and the 2025 E-11:** reviewed from public summaries only. The 150 Wh battery is under the E-13 size threshold.
- **Florida:** s. 327.391 requires a muffler but gives no decibel figure. The 90 dB(A) figure is removed from the guide.
- **Structural review (`reports/Structural_Analysis_v1.0.1.0.md`):** every load path checked. The primary structure has margin. Three driver-load parts were short and are fixed: steering stick now 1-1/2 x 0.120 in (was factor 0.6), throttle pedal 3/16 in, console frame 1 x 0.095 in. Added a stop on the stick, a fourth bolt at each gusset side, and cage mesh on the front and rim only. Weight options are listed but not applied.
- **Stability review (`reports/Stability_Analysis_v1.0.1.0.md`):** about 40 in of GM in all load cases, and water comes aboard at 17.5–22.5°. No changes needed. Tie gear down: loose gear sliding to one side lists the boat 4.4°.
- **Electrical compliance (`reports/Electrical_Compliance_v1.0.1.0.md`):** all 62 wires checked against E-11 and the color guide, with no exceptions outside the listed ones.
- **Capsized flotation:** both benches were inspected, full and sound. 1.94 ft³ of new foam (3.9 lb, BOM line 81, Build Guide 2.17) makes the boat float level upside down, with the hull bottom about 3.9 in clear of the water and 235 lb of reserve (`reports/Flotation_Inverted_v1.0.1.0.md`). Roll the finished boat in shallow water to confirm.
- **Second muffler:** not fitted. The stock silencer meets the rule, and it would now need an offsetting weight cut. Revisit only if the static test sound readings call for it.
- **rev8 features:** none come back.
- **Forward bearing plate:** a brick-element check at three mesh sizes (`CAD/analysis/bearing_plate_fea.py`) gives a peak of about 13.5 ksi at the cutouts with simple end supports and 8 ksi with fixed ends, and 0.06 in of deflection under thrust. The peak still rises with mesh refinement, because the mesh stair-steps the rounded corners, so it is not a converged figure. It agrees with the 14 ksi hand calculation, which stays the design number: yield factor about 2.5, fatigue factor about 1.6.
- **Questions for makers:** ready to send in `reports/Supplier_Questions_v1.0.1.0.md`.

## Still open: can't be closed from a desk
**On the real boat**
- **Grab rail:** check the ribs at X 54.75 and X 70.65 (up to about 478 lbf of uplift), try the 12° lean seated with the stick at full travel, then proof-pull at 300 lb.
- What is inside the middle bench, front deck strength under the fuel tank, rib heights at the stick and console, and the pump float height against the conduit.
- Weigh the engine, and do the stand tap test.

**BRP service manual:** not found online. PTO taper and side load, starter draw, charging output and regulator, oil sensor pinout, oil line and pump wiring, engine-side wiring, fuel pump current, and coolant and exhaust temperature limits. Questions are in `reports/Supplier_Questions_v1.0.1.0.md`.

**Replies from makers**
- **Warp Drive:** approve the 800R and belt combination.
- **Gates:** confirm the 239 hp rating at 7,600 rpm, and advise on torsional vibration. DesignFlex Pro could not be reached.

**Paid standards:** the full text of E-11 (2025) and E-13 (2025) was not read.

## Deferred
- `renderedImages/scenes/`, `v5_low-front-starboard-view_with-passenger.png`, `v7` and `v8` show earlier layouts. The passenger case is dropped.
- The Build steps assembly is not updated. `Render stages` and `Main assembly` are current.
- The outlet connector marker in Onshape version v1.0.1.0-structural still shows a spade terminal. `Minimal_wiring.fs` is correct.

## Other notes
- The sling is a flat sheet in CAD. The engine, pipe and radiator are placeholder envelopes until measured.
- The engine is far above the 1448 plate's 25 hp outboard rating (registration and insurance).
- Budget a two-stroke top-end rebuild at the service manual's interval.
