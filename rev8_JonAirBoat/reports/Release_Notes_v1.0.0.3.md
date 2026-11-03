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
| `Airboat Build Tracker v1.0.0.3.xlsx` | BOM (81 lines), Build Guide (27 tasks in 4 sections) and Read Me (pro labor rate, legend) |
| `CAD/Airboat_v1.0.0.3_*.step` | *To be exported at release:* full assembly plus the hull, drivetrain mounts, drivetrain, cage, seating and console Part Studios |
| `CAD/FeatureScript/` | Feature source: Hull armor, Crew mannequins, Drivetrain mounts (parametric for the hull), Utility features, **Jon hull (new, parametric 1652 placeholder)**, **A-frame aluminum (new)**, **Seating (new: seating and interior mounts)**, **Console (new: driver console)** |
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
- Still analysis only: fuel tank (6.5 gal up front), steering stick, trim tab.

### Build Guide
- **1.1** shops for a 1652 and ends by measuring the hull for the CAD. **1.2** carries a re-layout warning (see known issues).
- **2.1, 2.3–2.8, 2.10–2.12** rewritten for the 1652, rivnuts, the aluminum A-frame, the welded cage and the lithium battery.
- **2.16–2.19 (new):** interior mount rails, storage box/thwart with backrest and grab bar, battery dry box and driver seat, driver console.
- **Section 4 (new): on-water testing of both prototypes.** 4.1 requirements and test plan, 4.2 Prototype A (G13 + 3-blade), 4.3 Prototype B (G13B + 4-blade).

### Reports (new)
- `Requirements.md`, `Trim_Stability_Seating_Analysis.md`, `Weight_Reduction_Options.md` (including the running weight log), `Connection_Review.md`.

### Weight and payload
- Design payload: **280 lb of driver + gear** on the 1652 plate. With a 200 lb driver the **gear allowance is about 88 lb** as of this draft (see the running weight log).

### Build tracker totals (draft, 2026-11-02)
- **BOM:** about **$15,490** (81 lines; includes the $4,000 placeholder boat and about $850 of welding and rivnut tools).
- **Build Guide:** 27 tasks, **248 DIY hours**, 110.5 pro hours ≈ **$9,393** pro labor at $85/hr.

---

## Known issues and open items

1. **The hull is a parametric placeholder.** Measure the real 1652 (Build Guide 1.1 step 9) and update the CAD before cutting anything.
2. **Hull armor not re-laid out for the 1652.** The UHMW strip layout, rib stations and cut list in Build Guide 1.2 are still from the 1448 model.
3. **Not yet modeled:** steering stick, trim tab and fuel tank (analysis only). The rev7 steering pushrod still runs from the rudders to about X 96 in on the port side and passes about 1-2 in above the outboard edge of the passenger cushion; re-route it when the stick is designed.
4. **Front (bow) bench kept** as factory structure and flotation; the fuel tank goes just aft of it. The mannequins' feet hang about 1 in above the floor (high seats); a footrest can be added if needed.
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
