# DIY Jon-Air Airboat — Release Notes v1.0.0.3 (DRAFT)

> **Draft: not yet released.** This file and `Airboat Build Tracker v1.0.0.3.xlsx` are updated as the work proceeds on branch `rev8/1652-hull`. At release time the CAD exports and renders are made, the Onshape version is created, the totals below are refreshed, and this banner is removed.

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
| `Airboat Build Tracker v1.0.0.3.xlsx` | BOM (61 lines), Build Guide (23 tasks in 4 sections) and Read Me (pro labor rate, legend) |
| `CAD/Airboat_v1.0.0.3_*.step` | *To be exported at release:* full assembly plus the hull, drivetrain mounts, drivetrain and cage Part Studios |
| `CAD/FeatureScript/` | Feature source: Hull armor, Crew mannequins, Drivetrain mounts (parametric for the hull), Utility features, **Jon hull (new, parametric 1652 placeholder)**, **A-frame aluminum (new)** |
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

### Interior layout (analysis; not yet modeled in CAD)
- Tandem centerline seating: a driver bucket seat on tracks over the battery dry box; the passenger(s) on a full-width padded storage box in front of the engine, with a backrest/grab bar. Fuel 6.5 gal up front, dash/table to the driver's starboard, stick on the port side with a walkway. A starboard turnbuckle trim tab. The hull must float when swamped.

### Build Guide
- **1.1** shops for a 1652 and ends by measuring the hull for the CAD. **1.2** carries a re-layout warning (see known issues).
- **2.1, 2.3–2.8, 2.10–2.12** rewritten for the 1652, rivnuts, the aluminum A-frame, the welded cage and the lithium battery.
- **Section 4 (new): on-water testing of both prototypes.** 4.1 requirements and test plan, 4.2 Prototype A (G13 + 3-blade), 4.3 Prototype B (G13B + 4-blade).

### Reports (new)
- `Requirements.md`, `Trim_Stability_Seating_Analysis.md`, `Weight_Reduction_Options.md` (including the running weight log), `Connection_Review.md`.

### Weight and payload
- Design payload: **280 lb of driver + gear** on the 1652 plate. With a 200 lb driver the **gear allowance is about 85 lb** as of this draft (see the running weight log).

### Build tracker totals (draft, 2026-11-02)
- **BOM:** about **$13,520** (61 lines; includes the $4,000 placeholder boat and about $850 of welding and rivnut tools).
- **Build Guide:** 23 tasks, **206 DIY hours**, 91.5 pro hours ≈ **$7,778** pro labor at $85/hr.

---

## Known issues and open items

1. **The hull is a parametric placeholder.** Measure the real 1652 (Build Guide 1.1 step 9) and update the CAD before cutting anything.
2. **Hull armor not re-laid out for the 1652.** The UHMW strip layout, rib stations and cut list in Build Guide 1.2 are still from the 1448 model.
3. **Interior not modeled:** seat, storage box, backrest, dash, stick, trim tab and fuel tank exist only in the analysis.
4. **Weights are mostly estimates.** Only the A-frame and the connection parts have CAD weights. A full CAD weigh-in should replace the estimates before release.
5. **Build-step 1.1 render** is still the old 1448 image.
6. **Carried over from v1.0.0.2:** front cage post top cut angle; simplified cage tubes; nominal fastener lengths; UHMW expansion slots not modeled.
7. **Prices are estimates** as of 2026-11-02, before tax and shipping.

## Still to do before release
- Create the Onshape version v1.0.0.3 and export the STEP files.
- Re-render the studio views, scenes and new build steps on the 1652 hull.
- Remove the v1.0.0.2 tracker and STEP files from the package (as was done for v1.0.0.1).
- Refresh the totals and contents above, and remove the draft banner.
