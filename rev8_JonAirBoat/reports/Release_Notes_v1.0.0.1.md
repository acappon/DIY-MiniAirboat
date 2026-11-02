# DIY Jon-Air Airboat — Release Notes v1.0.0.1

**Release date:** 2026-11-01
**CAD source:** Onshape document "DIY Jon-Air boat", version **v1.0.0.1**
(https://cad.onshape.com/documents/7328492526b52c17df5ca2ec)
**Design rule:** everything must be buildable with DIY backyard-mechanic skills.

This is the first packaged release. It captures the hull armor design (build steps 1–4) and the first combined layout of the hull with the rev7 prop cage, rudders and drivetrain.

---

## Package contents

| Location | What it is |
|---|---|
| `Airboat Build Tracker v1.0.0.1.xlsx` | BOM (16 lines) and Build Guide (4 steps), plus a Read Me tab with the pro labor rate and legend |
| `CAD/Airboat_v1.0.0.1_Assembly.step` | Full assembly: hull, hull armor, cage and rudders, drivetrain, crew mannequins (STEP AP214, inches) |
| `CAD/Airboat_v1.0.0.1_Hull_and_armor.step` | Hull Part Studio only: Lowe 1448 reference hull plus all armor parts and fasteners |
| `CAD/FeatureScript/` | Source for the two custom Onshape features (Hull armor, Crew mannequins) |
| `renderedImages/studio/` | 6 viewpoints of the current model, each with and without driver/passenger mannequins (12 images) |
| `renderedImages/scenes/` | Concept scenes: underway, top speed, docked (no crew), docked (crew aboard) |
| `renderedImages/buildSteps/` | Per-step renders linked from the Build Guide |
| `renderedImages/transparent/` | The raw transparent-background renders, for reuse in other layouts |
| `reports/` | These release notes |

---

## What's in this release

### Hull (Build step 1)
- Reference hull: Lowe 1448 aluminum jonboat (imported mesh). Bottom 48.8 in wide, flat to 101 in from the transom, then a three-panel bow rise to the bow corner at 158 in / 9.3 in high. Gunwale at 19 in.
- Example used-boat listing and price in the BOM ($4,000, 2006 Tracker 1448 with trailer and motor, Craigslist Ventura).

### UHMW bottom (Build step 2)
- 1/4 in black UHMW, 8 pieces in 4 fore-aft strips with 1/4 in gaps, from the transom to 135 in (about 2/3 of the way up the bow rise). Cut from two 4x8 sheets.
- Seams staggered on ribs (center strips at 54.75 in, outer strips at 95.25 in).
- 56 × #10-24 × 1 in 82° flat-head stainless screws through the rib flanges, with 1 in fender washers and nylock nuts. Instructions call for slotted holes at all but one rib per piece to allow for UHMW thermal expansion, and marine sealant at every hole.

### Chine angles (Build step 3)
- 1/8 in 5052 aluminum, brake-formed to a 117° angle to match the hull side flare, transom to 131.75 in on each side.
- 44 × #10-24 × 5/8 in truss-head screws through the side leg into the hull side (every 6 in), with flat washers and nylocks. The bottom leg covers the UHMW edge without fastening through it.

### Bow skin (Build step 4)
- 0.063 in 5052-H32 aluminum, wrapping from inside the bow lip, over the gunwale rail, down the bow face and aft along the bow rise, lapping the UHMW leading edge by 3 in.
- 42 × #10-24 × 5/8 in truss-head screws around the bow face and across the rise forward of the UHMW.
- Two 6 in screw-out inspection ports in the front deck for reaching the nuts.

### Airboat layout
- Rev7 cage/rudders and drivetrain (imported STEP) placed with the hull in Assembly 1.
- Crew mannequins (driver and passenger, about 5'9", life vests, ear protection) seated on the middle bench, for renders and ergonomics checks.

### Build tracker totals at this release
- **BOM:** about **$5,067** (16 lines; includes the $4,000 example boat).
- **Build Guide:** 4 steps, **48 DIY hours**, 18.5 pro hours ≈ **$1,573** pro labor at $85/hr.

---

## Known issues and open items

1. **Hull ↔ airboat placement is assumed.** The rev7 cage and drivetrain files were modeled before this hull reference existed. They were placed using a shared origin (X = 0 at the transom, Y = 0 on the centerline, Z = 0 at the hull bottom). As a result:
   - The cage foot pads sit at Z = 16.9 in, about 2.6 in below the Lowe's gunwale rail top, and overlap the hull sides.
   - The A-frame and brace feet sit at Z = 5.7 in, with no floor, stringers or mounting deck modeled under them.
   - The A-frame legs pass through the rear bench, which will need to be removed or reworked.
2. **No driver seat or steering stick is modeled.** The mannequins sit on the existing middle bench, and the rudder pushrod ends forward of them without a control.
3. **Chine angles:** the formed profile is constant along the length. Near the bow, where the hull sides twist toward vertical, the side leg can stand off the hull by up to about 1/4 in.
4. **Bow face geometry** below the gunwale was extrapolated from the measured top portion because the hull mesh is incomplete there. The bow-skin width is approximate (±1/2 in).
5. **Fastener lengths are nominal.** The hull mesh has no thickness and rib flange thicknesses are estimated, so check screw lengths on the real boat before ordering.
6. **UHMW expansion slots** are described in the instructions but not modeled; all holes in the CAD are round.
7. **Renders:** the scenes are concept images. The boat is a CAD render composited onto a generated, stylized background. The waterline, trim and spray are approximate and are not a prediction of how the boat will float or plane. Mannequins are simplified shapes.
8. **Prices are estimates** as of 2026-11-01, before tax and shipping. The boat price is one example listing, and full 4x8 plastic sheets can carry $100+ freight each.

## Suggested next steps
- Decide where the cage and engine frame mount on this hull (gunwale-mounted cage pads, a floor/stringer deck for the A-frame) and update the airboat parts to suit.
- Add a raised driver seat and steering stick, then re-run the crew ergonomics check.
- Add build steps for the airboat structure and drivetrain installation.
