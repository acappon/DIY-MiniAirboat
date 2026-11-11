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
2. **Middle bench removed** (done in the tracker and in Onshape). Build Guide step 2.5 is now "Remove the middle bench": measure the bench fastenings and the beam first, keep a temporary spreader across the gunwales until the new struts are in (the bench is the boat's main thwart), drill out the rivets, lift the bench out whole with its foam, and seal every empty hole with closed-end rivets in sealant (the bench ends sit near or below the waterline). Step 2.1 no longer relies on the middle bench foam. New BOM line 83: closed-end rivets and the spreader, about $25. The dry box, battery switch and harness sleeve (BOM lines 57, 73, 76; step 2.11) are left as they are until step 3. BOM total about **$15,305**.
   - In Onshape the bench is the composite part "Bench - Middle" in the "Lowe 1448 Boat (reference mesh)" Part Studio; it will be removed with a Delete part feature. Two thin bench-top lip strips are part of the imported "Hull" composite and stay in the reference.
3. **Seat module** (done in CAD source, analysis, tracker and Onshape Main workspace). `CAD/FeatureScript/Minimal_cockpit.fs`, new Build Guide step **2.17** (done right after 2.5), BOM lines 84–87.
   - **Struts:** two 1 x 2 x 1/8 in 6061 tubes hull side to hull side at the old bench-top level (Z 9–11, X 76.5 and 85.5), bolted to the hull sides with angle clips: the new thwart and the seat support.
   - **Electrical dry box:** 0.063 in 5052, 8 x 10 x 7-1/4 in, hung between the struts, gasketed 0.080 in lid bolted into rivnuts. Battery at the forward end, fuse block on a bracket over it with the fuses facing the lid, relays K1–K5 at the aft end. Harness leaves through a gland in the aft wall (the bench sleeve is gone).
   - **Battery switch:** on a plate on the forward strut, port of the box, knob forward: usable without unbolting the seat.
   - **Dry storage each side:** 0.032 in walls under the struts, stopped 1-1/2 in above the floor for bilge drainage; closed-cell foam floor (flotation); 0.063 in lid outboard of the seat, hinged on the aft edge.
4. **Seat and seatback** (as 3).
   - **Seat:** 15 x 38 in across all three boxes (room for a passenger beside the driver), 3/8 in marine plywood base bolted to the struts over the dry box lid, 2-1/2 in closed-cell polyethylene pad (also flotation), marine vinyl. Seat top at about Z 14.1 (the bench was Z 12.5); the driver mannequin's hip point is now X 83.5, Z 14 (was X 82, Z 12.5), so the back meets the seatback, and the stick and pedal are 1-1/2 in closer. 6 mm plywood was too weak for someone standing on the front edge.
   - **Seatback:** 18 x 15 in, padded front and back, hinged just above the foam on two brackets bolted to the aft strut, 12° recline against a stop, folds flat forward. Facing aft on it is for engine-off use.
5. **Flotation restored.** The middle bench foam (about 4.8 ft³) was most of the capsized boat's forward flotation; without it the boat sank stern first. Now: seat pad, compartment floors and **two 6 in footwell side blocks** (X 98–128, Z 1.5–13; the starboard one has a gap for the console foot). Capsized: level (list −0.3°, pitch −0.4°), hull bottom about **3.0 in** clear (was 4.0), **169 lb** reserve (was 258). The fire extinguisher moves to the top of the starboard block (about X 99.5), and the throttle cable climbs over it.
6. **Totals.** Equipment **496.0 lb** (v1.0.1.2: 471.9), payload **329 lb**, **21 lb short** of the 350 lb for a 200 lb driver and 150 lb of gear: the driver can carry about 129 lb of gear. The dash load label now reads **MAXIMUM LOAD 370 LB** ... **LEAVES 325 LB** (was 395 and 350). Upright stability barely changes (GM 43.3 in, prop-torque heel 3.9°, dock trim 0.63°); top speed about 34.75 mph. New structural checks: struts factor 1.5 at 4 g with two people, end clips 3.5–4.8, seat base 2.5, seatback brackets 1.5. BOM total about **$15,745**.

7. **Oars and oarlocks** (new Build Guide step **2.18**, BOM lines 88–89, `CAD/analysis/oars.js`). For rowing facing aft on the folded seatback with the engine off, and carrying the oars in their locks.
   - **Oarlocks:** closed-ring oarlocks in top-mount sockets on **4 in 6061 risers** bolted through each rail at **X 66**, 10-1/2 in aft of the seat's aft edge. The pins sit about 9 in above the folded seatback. 7 ft oars with 24 in inboard leave the handle ends 30 in apart at the finish, and the hands pass about 3-1/2 in beside the knees. The shaft dips about 24° to the water, and the blades stay far outboard of the cage.
   - **Rowing clearances:** at the finish, the rower's port elbow clears the stick grip by about 5 in with the stick at neutral (tie it there while rowing), and the starboard elbow clears the dash by about 9-1/2 in.
   - **Shipping: yes, it works.** Each oar swings blade forward in its ring and lies just inside the rail on a padded rest with a rubber strap at X 118, from the handle at X 42 to the blade tip at X 126. Without the risers, the starboard oar would lie on the console arm where it meets the gunwale; with them it passes **about 4 in above** it. Clearances to the other parts:
     - extinguisher: 5 in
     - dash: 10 in
     - the stick: 10 in
     - the driver's elbow on the stick: about 8 in
     - opened storage lids: 4 in
     - sidelights: 7 in
     - fuel line: 8 in
     - stand: 6 in
   - **Structural:** a 100 lbf pull on the handle puts about 150 lbf on the pin, giving these factors:
     - riser: 31
     - bolts: 5.0
     - rail bearing with the doubler: 4.3
   - **Weight:** the oarlock hardware (rings, sockets, risers, doublers, rests) is boat equipment, **4.0 lb**. The oars (about 7.2 lb the pair) are **operator gear**: bring them or leave them ashore, and when aboard they come out of the people-and-gear load like any other gear. (First booked as equipment; corrected.)
   - **Cost and hours:** BOM total about **$15,950**. Build Guide about 212 DIY h and 101 pro h.

8. **Weight pass: carbon seat struts and a fiberglass/foam seat base** (BOM lines 84 and 86, step 2.17).
   - **Struts:** now 1 x 2 x 1/8 in roll-wrapped carbon tube, the same section, so about the same stiffness (sag 0.27 in with two people, was about 0.2). They go from 8.7 lb to **5.6 lb**. Factor at 4 g is **2.1**, up from 1.5.
   - **Galvanic isolation:** carbon touching aluminum corrodes the aluminum. Each strut gets one ply of glass. Every fastener through the carbon gets a 6061 crush sleeve, a G10 shim at the aluminum, and stainless bolts in Tef-Gel. That covers the end clips, dry box, compartment walls, switch plate, seatback brackets and seat base. The dry box and compartment walls are now bolted to the struts, not riveted.
   - **Seat base:** now a 3/8 in sandwich (5/16 in PVC foam core, 0.020 in E-glass skins, potted bolt inserts), down from 4.3 lb of plywood to **2.3 lb**. With 200 lb on the front edge: skins factor 3.0, core shear 3.5 (the plywood was 2.5).
   - **Weight:** **5.1 lb saved.**
   - **Cost and hours:** about **+$240** (BOM total about **$16,190**) and +3 DIY h (step 2.17 now 19 h).
9. **Honeycomb rudder blades; rudder and controls allowances cut** (BOM line 44, step 2.13, `CAD/analysis/controls_800r.js`).
   - **Rudder blades:** aluminum honeycomb, 0.020 in 5052 skins over 1/2 in hex core, with closed edges and potted hard points at the shaft and horn bolts. About 2.8 lb the pair, down from 4.4 lb for 0.040 skins over ribs. About +$80 (BOM total about **$16,270**).
   - **Allowances:** both cut to **8.5 lb** (5 lb off in all), keeping a margin over the itemized parts.
     - Rudders and linkage: 12 → 8.5 lb, against about 7.8 lb itemized.
     - Stick, pedal and cables: 10 → 8.5 lb, against 6.1 lb itemized.
   - **Weighing:** step 2.13 now weighs both sets before fitting. If either set comes in over its allowance, the load label comes down.
10. **Totals now** (oars as gear, carbon struts, sandwich seat base, honeycomb rudders, allowances cut): equipment **489.9 lb**, payload **335 lb**, **15 lb short** of 350: about **135 lb of gear** with a 200 lb driver, oars included if they come along.
    - Dash load label now **MAXIMUM LOAD 380 LB** ... **LEAVES 335 LB**.
    - Capsized flotation **3.9 in** clear, **190 lb** reserve, now counting the folding seatback's closed-cell foam as the float it is (0.35 ft³; it had been booked as nearly neutral). GM 43.6 in, dock trim 0.6°.

## Known issues and open items
- The **passenger handhold** went with the grab rail; nothing replaces it yet.
- **Payload is 15 lb short** of the 350 lb design load (driver + 150 lb gear): about 135 lb of gear with a 200 lb driver, including the oars (about 7.2 lb) when they come along. The rudder and controls allowances now keep only about 3 lb of margin over the itemized parts; weigh them in step 2.13.
- Never row, or unstrap the oars, with the engine running; the rower faces the prop cage.
- **Wiring:** the wire routes in `CAD/FeatureScript/Minimal_wiring.fs` (from `electrical/wire_list.py`) still end at the old dry box points; the new box keeps the battery, fuse block, relays and switch within about 1–4 in of them, but the routes, the wire list and the schematic need a pass for the new box exits and the switch plate.
- Capsized float height is about 3.9 in (v1.0.1.2: 4.0 in), with the seatback foam counted. It depends on the seatback being closed-cell foam and staying attached.
- `reports/Structural_Analysis_v1.0.1.2.md` and `reports/Drivetrain_800R_ETEC.md` still describe the grab rail; they are the v1.0.1.2 record and will be superseded at release.

## Onshape (Main workspace)
- "Minimal cockpit (FeatureScript)" uploaded; the "Minimal cockpit" Part Studio regenerates with 87 parts.
- "Lowe 1448 Boat (reference mesh)": new feature "Remove middle bench (v1.0.1.3 seat module)" deletes the "Bench - Middle" composite. (The Main assembly uses only the gunwale rail and handles from this studio.)
- "Crew mannequins": hip point X 83.5, Z 14.
- "Main assembly": the whole cockpit Part Studio re-inserted and the 71 old cockpit instances deleted (18 of them were left dangling by the removed grab rail, bench sleeve and dry box case). No mates were affected.
- "Minimal cockpit (FeatureScript)" uploaded again with the oarlocks, rests and shipped oars; the Part Studio regenerates with 123 parts. Preview: `renderedImages/wip_oars_shipped.png`.
- **Not yet:** the carbon strut and sandwich seat base part names and colors, and the honeycomb rudder envelope name in "Minimal 800R layout" (repo masters only; geometry unchanged), and the dash load label ("Minimal dash (FeatureScript)", now 380 / 335 lb) is changed in the repo master only; upload it before the v1.0.1.3 version. The "Render stages" and "Build steps" assemblies still need checking for dangling cockpit instances.

## Still to do before release
- Update the electrical documents for the new dry box, rewrite the stability and flotation reports for v1.0.1.3, re-render the build step images, and regenerate the Build Guide PDF.
- Update the CAD in Onshape, create version v1.0.1.3, export the STEP files and re-render.
- New renders (user, 2026-11-10):
  - **Night snake hunting:** operator rowing facing aft along a bank, searchlight aimed at the bank. Gear aboard (cooler, duffel bag, life jackets) so the oars read as carried gear, plus a snake tong / noose pole.
  - **25 mph:** one render with the oars and gear aboard, one without.
  - **Top speed:** solo rider only, no gear or oars.
