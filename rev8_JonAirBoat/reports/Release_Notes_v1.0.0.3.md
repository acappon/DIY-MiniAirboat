# DIY Jon-Air Airboat — Release Notes v1.0.0.3 (in development)

> **In development: not yet released.** This file and `Airboat Build Tracker v1.0.0.3.xlsx` carry the version in `VERSION` and are updated as the work proceeds on branch `rev8/1652-hull`. At release time the CAD exports and renders are made, the Onshape version is created, the totals below are refreshed, and `node tools/set_version.js --release <date>` removes this banner (see `RELEASING.md`).

**Release date:** TBD
**CAD source:** Onshape document "DIY Jon-Air boat", branch workspace "1652 hull", version **v1.0.0.3** (to be created at release)
(https://cad.onshape.com/documents/7328492526b52c17df5ca2ec)
**Previous release:** v1.0.0.2 (2026-11-01)
**Design rule:** everything must be buildable with DIY backyard-mechanic skills.

This release moves the design from the Lowe 1448 reference hull to a **1652-class hull** (about 16 ft x 52 in bottom, 20 in transom, ~1,216 lb capacity plate). It also brings in a bolted aluminum A-frame, the connection-review fixes (rivnuts, engine-mount plates, a welded black-steel cage), a lithium battery, and the project's requirements, trim/stability and weight analyses.

---

## Package contents

| Location | What it is |
|---|---|
| `Airboat Build Tracker v1.0.0.3.xlsx` | BOM (85 lines), Build Guide (28 tasks in 4 sections) and Read Me (pro labor rate, legend) |
| `CAD/Airboat_v1.0.0.3_*.step` | *To be exported at release:* full assembly plus the hull, drivetrain mounts, drivetrain, cage, seating, console and controls Part Studios |
| `CAD/FeatureScript/` | Feature source: Hull armor, Crew mannequins, Drivetrain mounts (parametric for the hull), Utility features, **Jon hull (new, parametric 1652 placeholder)**, **A-frame aluminum (new)**, **Seating (new: seating and interior mounts)**, **Console (new: driver console)**, **Controls (new: stick, throttle pedal, push-pull cables)** |
| `CAD/analysis/` | **New:** `trim_stability.js` (static trim, list, running trim, swamped flotation) and `compare_hulls.js` (1448 / 1652 / 1860 comparison). Run with node. |
| `renderedImages/` | *To be re-rendered at release on the 1652 hull* (studio views, scenes, build-step renders) |
| `reports/` | These notes, Requirements, Trim/Stability/Seating analysis, Weight Reduction Options (with the running weight log), Connection Review, and the static test plan, protocol and report template |

---

## What changed since v1.0.0.2

### Hull: 1652 class (Build Guide 1.1, 1.2, 2.1)
- **Parametric jonboat hull** (`Jon_hull.fs`, Lowe 1652 placeholder dimensions) replaces the 1448 reference mesh in the "1652 hull" branch. The inside floor is at Z 0 (outer bottom at −0.1 in).
- **Transom notch:** 47 in wide, cut down to 15.75 in, for cage and exhaust clearance, capped with aluminum channel.
- **Benches:** the middle bench comes out. The **rear bench center is removed** and the ends stay as foam-filled side boxes (port edge 17.5 in, starboard 23.75 in off the centerline). No bench holes are needed for the A-frame or cage any more.
- **Why the 1652:** the 1448's capacity plate is exceeded by the equipment alone (see the trim report and the hull comparison).

### Drivetrain mounts
- The mounts feature now takes the hull half-width, side flare, cage foot offset and transom position as parameters, and is refit to the 1652.
- **3/8-16 stainless hex rivnuts** in the engine-bed tops replace the welded captive nuts (captive nuts cannot be fitted inside a closed tube).
- **Four engine-mount plates** (1/4 in 6061) with 1/2-13 nylocks on the rubber-mount studs.
- Cage posts: knee clips, foot to the hull side, hole-saw reliefs for the outboard top-plate nuts; top-plate edges pulled inside the hull flare; knee brace ends at a clip inside the transom.
- Exhaust hangers bolt to the A-frame mount plates (no separate stands).

### A-frame: bolted aluminum (Build Guide 2.8)
- New "Aluminum A-frame (bolted)" feature: 1/2 in 6061 peak plates (same outline and holes), 2.5 x 2.5 x 3/16 legs, 2 x 2 x 3/16 crossbars and braces, angle-clip feet, crossbar shelf clips, brace shoes and top brackets.
- **CAD weight about 81 lb with hardware**, against about 145 lb for the rev7 steel A-frame (the plan had assumed 100 lb steel and 55 lb aluminum).

### Prop cage (Build Guide 2.12)
- Cage narrowing reduced from 2.75 to 0.75 in for the wider hull; the cage is raised 0.7 in so the drum and shaft guard are concentric with the raised prop shaft again (closes v1.0.0.2 known issue 1).
- **Welded black-steel (DOM/ERW) tube** at the rev7 sizes replaces galvanized EMT (no zinc fumes when welding). It is built as a module off the boat, then fitted. The MIG kit is in the BOM.

### Electrical
- **Group 31 LiFePO4 lithium dual-purpose battery** (about 1,000 CCA or more, built-in BMS) replaces the 70 lb AGM: about 42 lb lighter, with more usable capacity. It is charged by the alternator through an alternator protector, and offset about 7–8 in to port in the dry box under the driver seat.

### Seating and console (new Part Studios "Seating and interior mounts" and "Console", Build Guide 2.16-2.19)
- **Middle bench removed in the CAD; tandem centerline seating.** The crew mannequins now sit one behind the other: the passenger on the storage box (hip point X 68, Z 18 in) and the driver on the bucket seat (hip point X 103, Z 17 in).
- **Interior mount rails (the hull mounts):** six 1 x 2 x 1/8 in 6061 rails bolted to the ribs. Every interior part bolts to them with 5/16-18 rivnuts, so nothing goes through the bottom skin.
- **Passenger storage box = new thwart:** a 1-1/2 in angle frame at X 63-79 in, full width, whose corner posts follow the hull sides and bolt through them at the gunwale (replacing the bench's structural tie). Riveted 0.050 in panels, flotation foam under a false floor, hinged plywood lid with a vinyl cushion.
- **Backrest / grab bar:** inverted U of 1 in aluminum tube at X 58 in, crossbar at Z 34 in, stayed to the rails, with a back pad.
- **Driver:** battery dry box (X 97-112 in, offset to port) with the LiFePO4 battery and power hub, slide track and marine bucket seat.
- **Console (its own component in the assembly):** HDPE table at armrest height (Z 25 in) to the driver's starboard, a dash leaning 20 degrees back with tachometer/hour meter, temperature, fuel and volt gauges, oil-pressure and charge lights, key switch, lanyard kill switch, 6-gang switch panel and 12 V/USB outlet; a bolted 1 in angle frame down to two console mount rails. The instruments are named bodies, ready for the wiring design.
- **Checks:** no part of either studio overlaps another or hits the hull walls, the side boxes, the front bench or the engine beds; the drivetrain stays aft of X 53.5 in. CAD weight about 122 lb against 125 lb planned. Static trim and list improve slightly (list now about zero).
- Still analysis only: fuel tank (6.5 gal up front), trim tab.

### Controls: stick and foot throttle on Teleflex push-pull cables (new Part Studio "Controls (stick, throttle, cables)", Build Guide 2.20)
- **Steering stick** on the port side of the driver seat: 1-1/8 in DOM steel stick with a T-grip, pivoting fore-aft on a clevis base on its own mount rail. A lug 4.4 in above the pivot drives a **Teleflex/SeaStar 43C push-pull cable** (3 in travel, about 9 ft) that runs aft through grommets in the storage box panels and along the old rod line, outside the drum screen, to **bellcrank pin B** on the cage. The rev7 1 in steel pushrod and its rod end are removed from the assembly.
- **Foot-pedal throttle** on a new **footrest platform** for both feet (top at 4.5 in; the driver's feet no longer hang). The pedal is hinged at the heel; a down-arm outside the platform drives a **Teleflex/SeaStar 33C cable** (3 in travel, about 7 ft) along the starboard side of the dry box, through the storage box, to the throttle body.
- **Checks:** no overlaps in the studio; no hits on the hull, seating, console or engine beds; the cables pass the box panels through their grommets; no cable or end fitting touches the cage or drivetrain parts. Tightest bends 8.6 in (43C allows 5 in) and 12.8 in (33C allows 8 in).
- Weight about 22 lb against 15 lb planned (the footrest is new), so the gear allowance drops to about 81 lb. Static trim improves slightly.

### Moving parts mated in Assembly 1 (new feature "Motion sets", `CAD/FeatureScript/Motion_sets.fs`)
- **How it works:** the drivetrain, cage, controls and seating Part Studios each get a `set` configuration input and a last "Motion sets" feature. Set 0 is the studio as designed, set 1 is its fixed parts, and sets 2 and up are one rigid moving group each, with mate connectors on the joint axes. Assembly 1 inserts set 1 fixed and each moving set free, then mates them. The rev7 steering pushrod is left out of every set, so it is gone from the assembly (it stays in the studio's default view).
- **Drivetrain:** prop + prop flange + prop shaft + 75T pulley turn as one group on the prop-shaft axis; flywheel + flex coupling + jackshaft + 30T pulley turn on the crank axis. A gear relation makes the prop turn 0.4 x the crank (30T:75T), the same direction. The back-side idler turns 0.78 x the crank, opposite. The belt is a closed loop whose shape doesn't change as it runs, so it stays fixed.
- **Steering:** stick revolute on its pivot bolt, coupled 1.1:1 to the bellcrank (4.4 in lug / 4 in pin B arm, the 43C cable). The bellcrank drives the drive link, port rudder, tie bar and starboard rudder as a real four-bar and parallelogram linkage (revolute plus cylindrical mates), so the rudder angle is solved, not assumed. Both cable core rods slide in their hubs, coupled to the stick and bellcrank.
- **Throttle:** pedal revolute on its shaft; the pedal-end core rod slides 2.85 in per radian of pedal; the engine-end core rod moves out by the same amount.
- **Seating:** driver bucket seat slider on its tracks; storage box lid + cushion revolute on the aft-rail piano hinge.
- **Checked by driving the mates:** crank 90 deg → prop 36 deg, idler −70 deg. Stick 20 deg forward → bellcrank −22 deg, both rudders −26.5 deg (trailing edges to port: stick forward = turn left), steering core moves 1.54 in at both ends (matches the 3 in cable travel for ±20 deg). Pedal 10 deg → throttle core 0.50 in. Lid opens upward; seat slides fore-aft. All mates are left at zero (design position).
- **Mate limits (stops):** stick ±20 deg, both rudders ±30 deg (the stick stop reaches about 26.5 deg first), throttle pedal 0 to 12 deg (pedal flat on the footrest), storage lid 0 to 100 deg open, seat ±3 in (6 in track). With limits on, Onshape's API can no longer drive these mates; drag them in the Onshape assembly instead.
- **Rudder clearance checked:** both rudders and the tie bar swung to +30 and −30 deg hit none of the 58 cage parts they could reach.
- **Not mated (adjustments, not motion):** engine tilt on the slotted pivot bolts and jack screw (belt tension), seat-track lock positions, cable clamps. Crew mannequins stay fixed.

### Build Guide
- **1.1** shops for a 1652 and ends by measuring the hull for the CAD. **1.2** carries a re-layout warning (see known issues).
- **2.1, 2.3–2.8, 2.10–2.12** rewritten for the 1652, rivnuts, the aluminum A-frame, the welded cage and the lithium battery.
- **2.16–2.19 (new):** interior mount rails, storage box/thwart with backrest and grab bar, battery dry box and driver seat, driver console.
- **2.20 (new):** steering stick, throttle pedal and footrest, both push-pull cables; 2.11 and 2.13 now point to it.
- **Section 4 (new): on-water testing of both prototypes.** 4.1 requirements and test plan, 4.2 Prototype A (G13 + 3-blade), 4.3 Prototype B (G13B + 4-blade).

### Reports (new)
- `Requirements.md`, `Trim_Stability_Seating_Analysis.md`, `Weight_Reduction_Options.md` (including the running weight log), `Connection_Review.md`.

### Weight and payload
- Design payload: **280 lb of driver + gear** on the 1652 plate. With a 200 lb driver the **gear allowance is about 81 lb** as of this draft (see the running weight log).

### Build tracker totals (draft, 2026-11-02)
- **BOM:** about **$15,875** (85 lines; includes the $4,000 placeholder boat and about $850 of welding and rivnut tools).
- **Build Guide:** 28 tasks, **260 DIY hours**, 115.5 pro hours ≈ **$9,818** pro labor at $85/hr.

---

## Known issues and open items

1. **The hull is a parametric placeholder.** Measure the real 1652 (Build Guide 1.1 step 9) and update the CAD before cutting anything.
2. **Hull armor not re-laid out for the 1652.** The UHMW strip layout, rib stations and cut list in Build Guide 1.2 are still from the 1448 model.
3. **Not yet modeled:** trim tab and fuel tank (analysis only).
   - **Steering cable load:** the rev7 structural check puts about 160 lb on the steering link at hard rudder. Confirm the 43C cable's rated working load with SeaStar before the first run; if it is short, use the 64C series, two cables, or a solid rod.
   - **Throttle body position** on the G13 is a placeholder in the CAD; set the engine-end bracket on the real engine.
   - The steering cable's aft clamp tab is welded to a short arm off the aft cage hoop at fit-up (the arm is not modeled).
   - The rev7 pushrod is left out of the assembly (Motion sets); it still exists in the cage Part Studio's default view, so the per-studio cage STEP should be exported with `set=1` plus the moving sets, or the pushrod deleted there.
   - **Throttle pedal stroke is short.** The pedal stops at 12 deg (the toe reaches the footrest top), and with the 2.85 in down-arm that moves the throttle core only about 0.6 in. Check the G13 throttle lever's full-open travel on the real engine. If it needs more, lengthen the down-arm (it can drop to about 4.5 in before reaching the floor, about 0.95 in of travel) or use a smaller lever radius at the throttle body.
   - **Motion sets pick parts by their bounding-box centres.** If a moving part (or a fixed part a connector sits on) is resized or moved, update its centre in `Motion_sets.fs`, or the set will come out empty (the first part of that set is then renamed "MOTION SETS ERROR ...").
4. **Front (bow) bench kept** as factory structure and flotation; the fuel tank goes just aft of it. The driver's feet now rest on the footrest; the passenger's feet hang about 1 in above the floor (high box seat).
5. **Weights are partly estimates.** The A-frame, connection parts, seating and console have CAD weights; the rest are estimates. A full CAD weigh-in should replace the estimates before release.
6. **Build-step 1.1 render** is still the old 1448 image.
7. **Carried over from v1.0.0.2:** front cage post top cut angle; simplified cage tubes; nominal fastener lengths; UHMW expansion slots not modeled.
8. **Prices are estimates** as of 2026-11-02, before tax and shipping.

## Still to do before release
- Create the Onshape version v1.0.0.3 and export the STEP files.
- Re-render the studio views, scenes and new build steps on the 1652 hull (including 2.16-2.19).
- Electrical design: schematic and wire list, and every wire in CAD with color, AWG and end connectors (user request 2026-11-02).
- Remove the v1.0.0.2 tracker and STEP files from the package (as was done for v1.0.0.1).
- Refresh the totals and contents above, and remove the draft banner.
