# DIY Mini-Airboat: rev 7 design package

The rev 6 design converted to native, editable Fusion features, plus the rev 7 changes:
- redesigned bow
- raised exhaust
- driver station on a braced side frame, with:
  - armrest and Hurst-style grip
  - tilted dash with instruments, 12 V and USB outlets, phone holder
  - light switches and trim inclinometers
- LED light bar and remote searchlight
- rung-mounted throttle pedal and cable
- bench split into a sealed battery bay (Group 31 AGM, distribution hub) and a padded, gasketed dry-storage / cooler
- cleats, bow knees and bow eye
- extinguisher and air horn
- full 12 V electrical system: sealed battery bay with fuses, relays and dimmers, main switch on the bench front, engine, dash, lights and nav-light harnesses with sealed marine connectors, fuel gauge and sender, bilge float switch
- joints for every moving part

## Contents

| File | What it is |
|---|---|
| `CAD/Airboat_rev7.f3d` | Full Fusion design: parameters, joints, motion links, appearances, named views, render mannequins (hidden) |
| `CAD/Airboat_rev7_complete.step` | Whole boat, STEP |
| `CAD/hull.step`, `drivetrain.step`, `flotation.step`, `cage.step`, `layout.step` | Each subassembly, STEP (layout includes the hidden render mannequins) |
| `BOM_rev7.xlsx` | Bill of materials: the rev 6 workbook with a rev 7 section; totals recalculate in Excel |
| `Build_Guide_rev7.xlsx` | Step-by-step build guide with the rev 7 steps (47-58) and updated steps 6, 22, 31, 32, 41, 49, 50 |
| `Electrical/Wiring_Schematic.html` | Wiring schematic: one-line diagram, hub fuse table, wire list with gauges, colours and lengths, connectors |
| `Renders/photo/` | Photo renders of all 11 named views (Fusion local render, 1600 x 1000) |
| `Renders/previews/` | Shaded viewport images of the same 11 shots |

Estimates: materials about $22,400 including contingency; empty boat about 1,159 lb; about 510 DIY hours.

## Re-rendering

The images in `Renders/photo` are the current design. To render again (for example at higher quality or after a change), each shot is saved as a named view in the Fusion design (Browser, Named Views, 01-11). For each one:
1. Show only the figures listed below; the mannequins are under `layout_r6 / people_for_renders`.
2. Set the cooler lid (Drive Joints on `cooler_lid_hinge`).
3. Render from the Render workspace. Cloud rendering is fastest and keeps the load off your PC.

| Named view | Figures shown | Cooler lid |
|---|---|---|
| 01 Front 3-4 starboard | none | shut |
| 02 Front 3-4 port | none | shut |
| 03 Aft 3-4 port | none | shut |
| 04 Starboard profile | none | shut |
| 05 Top | none | shut |
| 06 Front 3-4 with driver | mannequin_driver | shut |
| 07 Front 3-4 driver and passenger | mannequin_driver, mannequin_passenger | shut |
| 08 Profile driver and passenger | mannequin_driver, mannequin_passenger | shut |
| 09 Driver eye view grip and dash | none | shut |
| 10 Cooler open | none | 95 deg |
| 11 Cooler open reaching in | mannequin_reaching | 95 deg |

## Useful parameters

| Parameter | Controls |
|---|---|
| `drv_armrest_h` | armrest height above the cushion (the console follows) |
| `drv_elbow_fwd`, `drv_reach` | where the grip lands for the seated driver |
| `drv_leg_clear` | leg space beside the bench |
| `thr_rung` | which ladder rung the throttle pedal clamps to |
| `drv_phone_rot` | phone holder: 0 portrait, 90 landscape |
| `lay_handle_*` | passenger grab handles |

Use the joints (drag, or Drive Joints) to pose the moving parts:
- steering
- prop and belt drive
- searchlight and its remote control
- cooler lid and stays
- throttle pedal
