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
2. **Middle bench removed from the hull** (next). The battery, dry box case and wiring stay where they are for now.
3. **New electrical dry box, gunwale struts and side storage compartments** (to do).
4. **Foam seat and folding seatback** (to do).

## Known issues and open items
- The **passenger handhold** went with the grab rail; nothing replaces it yet.
- Removing the middle bench removes foam that the capsize flotation relies on (`reports/Flotation_Inverted_v1.0.1.2.md`); the plan is closed-cell foam under a raised floor in the side compartments.
- `reports/Structural_Analysis_v1.0.1.2.md` and `reports/Drivetrain_800R_ETEC.md` still describe the grab rail; they are the v1.0.1.2 record and will be superseded at release.

## Still to do before release
- Steps 2–4 above, then rerun weight, flotation and upright stability, update the electrical documents for the new dry box, and regenerate the Build Guide PDF.
- Update the CAD in Onshape, create version v1.0.1.3, export the STEP files and re-render.
