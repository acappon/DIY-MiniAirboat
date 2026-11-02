# DIY Jon-Air Airboat — Release Notes v1.0.0.2

**Release date:** 2026-11-01
**CAD source:** Onshape document "DIY Jon-Air boat", version **v1.0.0.2**
(https://cad.onshape.com/documents/7328492526b52c17df5ca2ec)
**Previous release:** v1.0.0.1 (2026-11-01)
**Design rule:** everything must be buildable with DIY backyard-mechanic skills.

This release adds the drivetrain installation: engine beds, mount plates and cage supports modeled on the hull, the rev7 drivetrain seated on them, and the cage base narrowed so it fits inside the hull. The Build Guide gains Section 2 (drivetrain installation, 15 tasks) and Section 3 (static test), and the reports folder now holds the static test plan, protocol and a blank performance report.

---

## Package contents

| Location | What it is |
|---|---|
| `Airboat Build Tracker v1.0.0.2.xlsx` | BOM (42 lines), Build Guide (20 tasks in 3 sections) and Read Me (pro labor rate, legend) |
| `CAD/Airboat_v1.0.0.2_Assembly.step` | Full assembly: hull and armor, drivetrain mounts, cage and rudders, drivetrain, crew mannequins (STEP, inches) |
| `CAD/Airboat_v1.0.0.2_Hull_and_armor.step` | Hull Part Studio: Lowe 1448 reference hull with all armor parts and fasteners |
| `CAD/Airboat_v1.0.0.2_Drivetrain_mounts.step` | New: engine beds, doublers, mount plates, cage supports, captive nuts |
| `CAD/Airboat_v1.0.0.2_Cage_and_rudders.step` | Cage and rudders Part Studio, with the narrowed cage base |
| `CAD/FeatureScript/` | Source for the custom Onshape features: Hull armor, Crew mannequins, Drivetrain mounts (new), Utility features (new: Translate all parts, Narrow cage base) |
| `renderedImages/studio/` | 6 viewpoints, each with and without driver/passenger mannequins (12 images) |
| `renderedImages/scenes/` | Concept scenes: underway, top speed, docked (no crew), docked (crew aboard) |
| `renderedImages/buildSteps/` | Per-task renders linked from the Build Guide (15 images) |
| `renderedImages/transparent/` | Raw transparent-background renders, for reuse in other layouts |
| `reports/` | These notes, the static test plan, the step-by-step static test protocol, and the static test performance report template |

---

## What changed since v1.0.0.1

### Drivetrain mounts (new Part Studio "Drivetrain mounts", Build Guide 2.2–2.7)
- **Engine beds:** two 2 x 6 x 1/8 in 6061 rectangular tubes on edge, 14.5 in each side of the centerline, X 2–62 in from the transom. Each has welded 1-1/2 in angle flanges along the bottom, notches over the ribs at 37.85 and 54.75 in, and 1/4-20 flange bolts every 6 in per side (staggered). They sit on 1/8 in 5052 bottom doublers between the ribs.
- **Mount plates:** six 1/4 in 6061 plates on the beds (A-frame aft feet, A-frame forward feet, brace feet), each with two welded 3/8-16 captive nuts. Mount surface at Z 6.375 in.
- **Drivetrain raised 0.7 in:** the rev7 A-frame feet were at Z 5.675 in, so the drivetrain Part Studio gets a "Raise onto engine-bed mount plates" feature (+0.7 in) to seat the module on the plates.
- **Cage supports, front:** one per side. A 2 x 2 x 1/4 in angle post follows the hull-side flare from a 1/4 in bottom foot near the chine up to a 1/4 in top plate at Z 16.9 in. A horizontal angle knee runs aft to a clip plate on the transom. The top plate has four captive nuts at X 10.75 / 13.25 in, 29.2 / 31.7 in off the centerline.
- **Cage supports, aft (new in this release):** a short vertical 2 x 2 x 1/4 in angle post stands on each knee (X 2–4 in, Z 10–18.05 in) under a 1/4 in aft top plate (X 0.55–5.55 in) with four captive nuts at X 1.8 / 4.3 in. The aft pads sit 1.4 in higher than the front pads. In v1.0.0.1 the rev7 aft cage feet had no support.

### Prop cage
- **Base narrowed 2.75 in per side** (feature "Narrow cage base" in the cage Part Studio). The rev7 foot pads overhung the hull side at gunwale height. The feet, isolators and foot bolts now move inboard 2.75 in. The legs, base rails and side diagonals are rebuilt as 1 in x 0.065 in tubes whose lower ends move the full 2.75 in, tapering to no change where the legs meet the drum hoops. The drum, hoops, screens and prop location do not move.
- **Mesh shown see-through:** the drum wrap and the front and aft screens now render at about 25% opacity, so the prop, engine and frame are visible through the cage. This changes appearance only.

### Build Guide
- **Section 2, "Install the drivetrain" (tasks 2.1–2.15):**
  1. Layout and templates.
  2. Shop fabrication of the beds, mount plates and cage supports.
  3. Bench cut-outs.
  4. Installing the beds and supports in the boat.
  5. Building the A-frame as a liftable module, then the trial lift and installation.
  6. Systems hook-up.
  7. Cage build around the prop location, then rudders and steering.
  8. Pre-test sign-off.
  9. Propeller, installed last.
- **Section 3:** task 3.1, static test of the installed drivetrain.
- **Tasks 2.4, 2.7 and 2.12** were updated for the narrowed cage base and the aft supports.
- **Renders:** each new task has one render.

### Reports (new)
- `Static_Test_Plan_Drivetrain.md`: purpose, setup, pass/fail and abort criteria, hazards and PPE.
- `Static_Test_Protocol_Drivetrain.md`: printable step-by-step checklist with data sheets.
- `Static_Test_Performance_Report.md`: **template only. The static test has not been run.**

### Build tracker totals at this release
- **BOM:** about **$10,317** (42 lines; includes the $4,000 example boat). The mount plate stock line grew from a 12 x 24 to a 24 x 24 in plate (about $130) for the aft cage plates.
- **Build Guide:** 20 tasks, **160 DIY hours**, 74.5 pro hours ≈ **$6,333** pro labor at $85/hr.

---

## Known issues and open items

1. **Prop clearance not re-checked after the 0.7 in raise.** The drivetrain now sits 0.7 in higher than in the rev7 CAD, but the cage drum did not move. Check prop tip clearance to the top of the cage (minimum 2 in) during task 2.12 and before the static test.
2. **Front cage post top:** the slanted hull-side post is cut square to its own axis, so one corner pokes up through the front top plate in the CAD. On the real part, cut the post top to match the plate (about 27°) before welding.
3. **Narrowed cage tubes are simplified:** they are modeled as straight 1 in x 0.065 in tubes between the new end points. The rev7 copes and end details at the joints are not modeled, so fit the real joints on the boat.
4. **Hull ↔ airboat placement** still uses a shared origin (X = 0 at the transom, Y = 0 on the centerline, Z = 0 at the hull bottom), and the Lowe hull is a measured reference mesh. Confirm all mount positions on the actual boat with the task 2.1 templates before cutting.
5. **No driver seat or steering stick is modeled yet.** The mannequins sit on the existing middle bench.
6. **Carried over from v1.0.0.1:**
   - The chine-angle side leg can stand off the hull near the bow.
   - The bow-face geometry is extrapolated.
   - Fastener lengths are nominal.
   - The UHMW expansion slots are not modeled.
7. **Renders:** the scenes are concept images. The boat is a CAD render composited onto a stylized background, and the waterline and spray are approximate. Mannequins are simplified shapes.
8. **Prices are estimates** as of 2026-11-01, before tax and shipping.

## Suggested next steps
- Check prop-to-cage clearance with the raised drivetrain, and adjust the raise or the cage drum height if needed.
- Model a raised driver seat and steering stick, then re-run the crew ergonomics check.
- Run the static test (Build Guide 3.1), then fill in the performance report.
