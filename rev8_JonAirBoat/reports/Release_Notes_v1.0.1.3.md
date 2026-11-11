# DIY Jon-Air Airboat — Release Notes v1.0.1.3

> **In development: not yet released.** Branch `main` (Lowe 1448 hull). Updated as the work proceeds.

**Release date:** TBD
**CAD source:** Onshape document "DIY Jon-Air boat", version **v1.0.1.3** (to be created at release)

## What changed since v1.0.1.2

### Seating redesign (in progress)
The middle bench and the grab rail give way to a purpose-built seat over three boxes: a small sealed electrical dry box on the centerline, tied to the gunwales by two aluminum struts along its fore and aft edges, with a dry storage compartment on each side. A high-density foam pad runs across all three (room for a passenger beside the driver on short trips), and a seatback hinged just above the foam folds flat and is lightly padded on its back for sitting facing aft.

1. **Grab rail and sling backrest removed** (done). Gone from the CAD (`Minimal_cockpit.fs`), the weight model and the structural check; `CAD/analysis/grab_rail.js` and the step image are deleted. Tracker: BOM line 79 deleted (later lines renumbered: trim tab is now line 79, flotation foam line 80, bubble level line 81, UHMW backing bars line 82), Build Guide step 2.16 deleted (flotation foam is now step 2.16). Its four rib mounts at X 54.75 and X 70.65 go with it, so nothing is fastened to those ribs any more.
   - Equipment about **465.1 lb** (was 471.9), payload about **359.9 lb** against the 350 lb needed, **+9.9 lb of margin** (was +3.1).
   - BOM total about **$15,280** (was $15,755).
2. **Middle bench removed** (done in the tracker; the CAD change goes in with the Onshape update). Build Guide step 2.5 is now "Remove the middle bench": measure the bench fastenings and the beam first, keep a temporary spreader across the gunwales until the new struts are in (the bench is the boat's main thwart), drill out the rivets, lift the bench out whole with its foam, and seal every empty hole with closed-end rivets in sealant (the bench ends sit near or below the waterline). Step 2.1 no longer relies on the middle bench foam. New BOM line 83: closed-end rivets and the spreader, about $25. The dry box, battery switch and harness sleeve (BOM lines 57, 73, 76; step 2.11) are left as they are until step 3. BOM total about **$15,305**.
   - In Onshape the bench is the composite part "Bench - Middle" in the "Lowe 1448 Boat (reference mesh)" Part Studio; it will be removed with a Delete part feature. Two thin bench-top lip strips are part of the imported "Hull" composite and stay in the reference.
3. **Seat module** (done in CAD source, analysis and tracker; Onshape update next). `CAD/FeatureScript/Minimal_cockpit.fs`, new Build Guide step **2.17** (done right after 2.5), BOM lines 84–87.
   - **Struts:** two 1 x 2 x 1/8 in 6061 tubes hull side to hull side at the old bench-top level (Z 9–11, X 76.5 and 85.5), bolted to the hull sides with angle clips: the new thwart and the seat support.
   - **Electrical dry box:** 0.063 in 5052, 8 x 10 x 7-1/4 in, hung between the struts, gasketed 0.080 in lid bolted into rivnuts. Battery at the forward end, fuse block on a bracket over it with the fuses facing the lid, relays K1–K5 at the aft end. Harness leaves through a gland in the aft wall (the bench sleeve is gone).
   - **Battery switch:** on a plate on the forward strut, port of the box, knob forward: usable without unbolting the seat.
   - **Dry storage each side:** 0.032 in walls under the struts, stopped 1-1/2 in above the floor for bilge drainage; closed-cell foam floor (flotation); 0.063 in lid outboard of the seat, hinged on the aft edge.
4. **Seat and seatback** (as 3).
   - **Seat:** 15 x 38 in across all three boxes (room for a passenger beside the driver), 3/8 in marine plywood base bolted to the struts over the dry box lid, 2-1/2 in closed-cell polyethylene pad (also flotation), marine vinyl. Seat top at about Z 14.1 (the bench was Z 12.5); driver's hip stays at about X 82. 6 mm plywood was too weak for someone standing on the front edge.
   - **Seatback:** 18 x 15 in, padded front and back, hinged just above the foam on two brackets bolted to the aft strut, 12° recline against a stop, folds flat forward. Facing aft on it is for engine-off use.
5. **Flotation restored.** The middle bench foam (about 4.8 ft³) was most of the capsized boat's forward flotation; without it the boat sank stern first. Now: seat pad, compartment floors and **two 6 in footwell side blocks** (X 98–128, Z 1.5–13; the starboard one has a gap for the console foot). Capsized: level (list −0.3°, pitch −0.4°), hull bottom about **3.0 in** clear (was 4.0), **169 lb** reserve (was 258). The fire extinguisher moves to the top of the starboard block (about X 99.5), and the throttle cable climbs over it.
6. **Totals.** Equipment **496.0 lb** (v1.0.1.2: 471.9), payload **329 lb**, **21 lb short** of the 350 lb for a 200 lb driver and 150 lb of gear: the driver can carry about 129 lb of gear. The dash load label now reads **MAXIMUM LOAD 370 LB** ... **LEAVES 325 LB** (was 395 and 350). Upright stability barely changes (GM 43.2 in, prop-torque heel 3.9°, dock trim 0.68°); top speed about 34.75 mph. New structural checks: struts factor 1.5 at 4 g with two people, end clips 3.5–4.8, seat base 2.5, seatback brackets 1.5. BOM total about **$15,745**.

## Known issues and open items
- The **passenger handhold** went with the grab rail; nothing replaces it yet.
- **Payload is 21 lb short** of the 350 lb design load (driver + 150 lb gear): about 129 lb of gear with a 200 lb driver.
- **Wiring:** the wire routes in `CAD/FeatureScript/Minimal_wiring.fs` (from `electrical/wire_list.py`) still end at the old dry box points; the new box keeps the battery, fuse block, relays and switch within about 1–4 in of them, but the routes, the wire list and the schematic need a pass for the new box exits and the switch plate.
- Capsized float height is about 3.0 in, down from 4.0 in; 8 in side blocks would restore 4 in but leave the footwell too narrow for a passenger's feet.
- `reports/Structural_Analysis_v1.0.1.2.md` and `reports/Drivetrain_800R_ETEC.md` still describe the grab rail; they are the v1.0.1.2 record and will be superseded at release.

## Still to do before release
- Update the electrical documents for the new dry box, rewrite the stability and flotation reports for v1.0.1.3, re-render the build step images, and regenerate the Build Guide PDF.
- Update the CAD in Onshape, create version v1.0.1.3, export the STEP files and re-render.
