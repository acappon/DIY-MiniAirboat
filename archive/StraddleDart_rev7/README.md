# Straddle Dart: rev 7 design package

**Straddle Dart** is an open-source DIY mini-airboat: a 12.5 ft riveted-aluminium airboat for one or two people. It's powered by a Suzuki G13B car engine (1.3 L twin-cam, about 100 hp) through a belt redrive, and designed to be built in a backyard shop.

## Why build one?

A new small airboat costs $32,000 to $60,000 or more. A used one is cheaper, but it comes with someone else's corrosion and an engine you may not be able to get parts for. This boat costs about $24,500 in materials and about 520 hours of skilled backyard work. A pro shop would charge about $41,000 for that labour, so every hour you put in is worth about $79. Build with friends and share the boat, and it costs four co-owners about $6,100 and 130 hours each.

You end up owning a boat you understand completely, with engineering checks you can read and car parts from any parts store. If you have more time than money, this is how you turn one into the other.

*New and used prices are rough figures from published pricing guides and listings (mostly undated, checked Oct 2026); get current quotes. Materials and labour come from `BOM_rev7.xlsx` and `Build_Guide_rev7.xlsx`.*

## About rev 7

Rev 7 takes the rev 6 design, converts it to native, editable Fusion features, and adds a new driver station, a full electrical system and an engineering review.

New here? Start with [GETTING_STARTED.md](GETTING_STARTED.md). It says what to read first and explains why the cockpit is built the way it is: low seat, grip, foot ladders, knee braces, and no seat belt.

## What's new in rev 7

**Hull and drivetrain**
- **Engine:** Suzuki G13B, the 1.3 L twin-cam 16-valve G13 (about 100 hp at 6,500 rpm), instead of the 70 hp single-cam G13. It's the same engine family and block size, through the same 2.5:1 belt; the prop is pitched finer to absorb the extra power. It restores the planing margin with two aboard.
- **Bow:** redesigned, with a conic roll-up into a 15° raked bow plate.
- **Exhaust:** raised above the aft foam.
- **Bottom:** 0.125 in plating with frames added at 60, 84 and 111.5 in, sized to ISO 12215-5 as a sheltered-water (category D) planing cruiser. See the operating limits in GETTING_STARTED.md.
- **Finish:** the hull is left bare 5052, primed only at laps and cut edges.
- **Chine guards:** brake-formed from 1/8 in strip.
- **Rivet pitch:** relaxed to 1 in above the waterline.
- **Forward A-frame crossbar:** deepened to 2 × 3 for fatigue.
- **Prop shaft:** specified as pre-hardened 4140.

**Driver station**
- **Console:** braced side frame with 3/16 in walls. It has a sheet-metal top and skins, and the dash folds up under a closed hood so nobody can get at the key wiring. The dash carries the tach, temperature and fuel gauges, key, kill-switch lanyard, NAV-OFF-ANCHOR switch, two light dimmers, USB and 12 V.
- **Grip:** a Hurst T-handle across the boat on a 1-1/8 in stainless stick, on a plate bolted through the console top and frame. The handle is a real Hurst shift knob on a pinned M16 stud with a nylon-insert lock nut, rated for a 450 lb pull; the stick, plate and frame below it are built for 600 lb.
- **Armrest:** hinged; it swings up to let the driver in.
- **Knee braces:** padded and curved to starboard, room for a large leg.
- **Fittings:** cup holder, swivel phone holder, life-jacket hook and trim inclinometers.
- **Fiddle rail:** a 1/2 in raised edge round the console top, like a galley table, so small things don't roll or slide off.
- **Foot ladders:** two of them, a landing for each foot like motorcycle pegs. The throttle pedal clamps to any rung of the right-hand ladder.

**Steering**
- **New linkage:** aft-pointing horns, a tie bar, and a drive link to a bellcrank on the aft hoop, then a straight 1 × .065 rod to the lever. It replaces a dog-legged pushrod that would have bent.
- **Lever:** moved 1.5 in outboard, clear of the left knee.

**Prop cage**
- **Tube:** DOM tube throughout: hoops 1 × .083 (for the G13B's stronger slipstream), longitudinals and aft bars 1 × .065.
- **Bracing:** a diagonal in each side bay.

**Lights, power and electrical**
- 30 in LED light bar and a remote-controlled searchlight.
- Nav and anchor lights, plus a removable flag staff with the international-orange flag Florida requires.
- Bench split into a sealed battery bay (Group 31 AGM, 12-circuit fuse hub, main switch) and a gasketed dry-storage / cooler.
- Full 12 V system with sealed marine connectors.

**Deck and safety**
- Cleats, bow knees and bow eye.
- Extinguisher and air horn.
- A trim-tab stiffener behind the tab on the transom.
- Swamped-boat warning labels.

## Where the design lives

The live design is in Fusion (Autodesk cloud). It's split into separate design files so each part of the boat can be edited quickly; they're brought together in one assembly.

| Fusion file | Location | What's in it |
|---|---|---|
| Airboat rev7 - Assembly | Default Project | Inserts the six files below at their positions, plus the joint between the steering rod and the lever |
| Airboat rev7 - Hull | Default Project / Airboat rev 7 split | Skin, stringers, frames, UHMW, rails, knees, chine guards, nose cap |
| Airboat rev7 - Drivetrain | same folder | G13B, belt redrive, A-frames and braces, engine mounts, prop, radiator, exhaust |
| Airboat rev7 - Flotation | same folder | Foam blocks, floor, sump, deck plates, bow foam |
| Airboat rev7 - Cage and rudders | same folder | Prop cage, screens, rudders, steering linkage |
| Airboat rev7 - Layout | same folder | Bench and battery bay, driver station and console, foot ladders and pedal, lights, searchlight, electrical harnesses, deck and safety gear, trim tab |
| Airboat rev7 - Render people | same folder | Three posed mannequins for renders (driver, passenger, someone reaching into the cooler) |

The original single design, **DIY Mini-Airboat** (version 21), stays in Default Project as an archive of the pre-split model. The file **Airboat cage and rudders (trial)** in the same place was a test copy and can be deleted.

To change something, open its own file, edit and save it. Then open the assembly and accept the updated references.

## What's in this folder

| File | What it is | Current? |
|---|---|---|
| `GETTING_STARTED.md` | Where to start, and the reasoning behind the cockpit layout | yes |
| `BOM_rev7.xlsx` | Bill of materials: hull, drivetrain and everything-else tabs with a rev 7 section and a summary; totals recalculate in Excel | yes |
| `Build_Guide_rev7.xlsx` | Step-by-step build guide with DIY hours, pro labour cost and a key check for every step. The hull section opens with an optional CNC-cutting shortcut | yes |
| `Team/Build_Ledger_rev7.xlsx` | First draft of the build-team ledger: pledges and completed work per step and boat, sign-offs, cash and material contributions, the resulting ownership split, and a usage log for co-owners. Each step is worth its pro labour cost from the build guide. Made by `tools/make_ledger_rev7.py` | draft, 31 Oct |
| `Engineering/Stability_and_Flotation.html` | Stability and flotation report for engineering review: weights from the CAD, intact hydrostatics and righting curves, the Coast Guard capacity method, swamped flotation | yes, 31 Oct (G13B, 0.125 bottom) |
| `Engineering/Structural_Checks.html` | Structural report for engineering review. It covers 50 checks on the drive tower, prop cage, driver side frame, engine mounts, prop shaft, steering, transom, trim tab and foot ladders, each with before-and-after tables for the fixes, plus the ISO 12215-5 bottom plating check | yes |
| `Engineering/calc/` | The Python scripts and data that reproduce both reports | yes |
| `Labels/Swamped_Warning_Label.svg` | Warning label: a swamped boat can be rolled by one person pulling on the side. Print 3 on UV-laminated marine vinyl | yes |
| `Electrical/Wiring_Schematic.html` | Wiring schematic: one-line power diagram, control and signal diagram (key and kill switch, relay coils, gauges and senders, light switches, bilge switch, every wire with its colour), fuse-hub table, wire list with gauges, colours and lengths, connectors. Open it in a browser | yes, 1 Nov |
| `Electrical/Wiring_One_Line_Diagram.svg` / `.pdf`, `Wiring_Control_and_Signals.svg` / `.pdf` | The two diagrams as standalone drawings: SVG to view on GitHub, PDF on one 11 × 17 landscape sheet for printing | yes, 1 Nov |
| `CAD/StraddleDart_rev7_*.f3d` | One Fusion archive per design file (Hull, Drivetrain, Flotation, Cage and rudders, Layout, Render people), each with its full history, parameters and joints. To rebuild the assembly offline, upload all six to one Fusion project, insert them into a new design, and recreate the steering rod's ball joint at the lever's rod end | yes, 31 Oct (current) |
| `CAD/StraddleDart_rev7_complete.step` | Whole boat, STEP (render mannequins left out) | as above |
| `CAD/hull.step`, `drivetrain.step`, `flotation.step`, `cage.step`, `layout.step` | Each subassembly, STEP | as above |
| `Renders/photo/`, `Renders/previews/` | Photo renders (Fusion local render, 1600 × 1000) and shaded previews of the 11 shots | yes, 31 Oct (current) |
| `Marketing/index.html` | Marketing kit: brochure, video storyboards, South Florida marketing plan, builder cost calculator, copy pack. Online at https://claude.ai/artifact/JRPSEGVPXhh7M1UbTcewxk | **no**: describes the 30 Oct design |
| `StraddleDart_rev7_package.zip` | Zip of this whole folder | yes, 31 Oct |

## Estimates

| | Figure | Source |
|---|---|---|
| Empty boat | about 1,320 lb | CAD weight model before the 0.125 bottom (1,247 lb), plus about 52 lb for the bottom and 19 lb for the G13B and heavier hoops; the BOM's own total is 1,301 lb |
| Materials | about $24,500, including 15% contingency | `BOM_rev7.xlsx`, Summary tab |
| Build time | about 520 DIY hours (32 weekends of 16 hours) | `Build_Guide_rev7.xlsx` |
| Time to plane (0-15 mph), full fuel | about 3.9 s solo, 4.7 s two aboard | `Engineering/calc/performance.py`, calibrated to the original design summary; static thrust about 430 lb is an estimate |
| Top speed | about 40-42 mph solo, 38-41 mph two aboard | same |
| Cruise at 30 mph | about 36-40 hp, 3.5-3.8 gph; range 52-57 mi calculated, plan on 40-50 mi | same |
| Persons capacity | 2 persons / 400 lb, 498 lb total by the Coast Guard method; label 2 persons / 400 lb, 490 lb total | Stability report |

## Useful parameters

Each design file keeps its own user parameters. Change them in that file (Modify, Change Parameters); the features follow. Those marked * are read when their build script runs, so changing them means re-running the script.

| File | Parameters | Control |
|---|---|---|
| Layout | `drv_armrest_h`, `drv_elbow_fwd`, `drv_reach` | Armrest height and where the grip lands for the seated driver (the console follows) |
| Layout | `drv_post_wall`, `drv_brace_wall` | Side-frame and knee-brace walls |
| Layout | `drv_brace_top_y`*, `drv_brace_sag`* | Knee-brace shape (the pads are built by the console script) |
| Layout | `con_*`* | Console, dash hood, cup holder, phone holder, life-jacket hook |
| Layout | `lay_ladder_y`, `lay_ladder_out_P`, `lay_ladder_out_S` | Positions of the foot-ladder rails |
| Layout | `thr_rung`*, `thr_y`* | Which right-ladder rung the throttle pedal clamps to, and its position across the boat (the throttle cable route is rebuilt with them) |
| Layout | `lay_handle_*` | Passenger grab handles |
| Cage and rudders | `cage_*` | Hoop size and position, tube sizes, rudder rails, side-bay diagonals |
| Cage and rudders | `steer_rod_od`, `steer_link_od`, `rudder_horn_*` | Steering rod, links and horns |
| Hull | `hull_*`, `hat_*`, `uhmw_*`, `nose_*`, `rail_*` | Skin, stringers, UHMW, nose cap and rub rails (bow geometry) |
| Drivetrain | `g13_*`, `exh_*` | Starter and bellhousing references; raised exhaust |
| Flotation | `flot_*` | Bow foam |

## Moving parts

Drag a part, or use Drive Joints:
- **Cage and rudders:** the rudder hinge joints move both rudders, the tie bar, the drive link, the bellcrank and the steering rod.
- **Layout:** the steering lever, throttle pedal, armrest, cooler lid and stays, searchlight yaw and pitch, and the searchlight remote's handle and grip.
- **Drivetrain:** prop and belt drive.

## Re-rendering

The 11 shots (`01 Front 3-4 starboard` through `11 Cooler open reaching in`) are saved as named views in **Airboat rev7 - Assembly**. For each one:
1. Show the mannequins from **Render people** as needed (driver for 06; driver and passenger for 07 and 08; reaching for 11).
2. For 10 and 11, open the cooler lid to 95° by driving the joint `cooler_lid_hinge` in steps.
3. Render from the Render workspace. Cloud rendering is fastest and keeps the load off your PC.

Note that the posed driver's left hand was shaped around the steering lever's old position, so it overlaps the lever by about 1.5 in now.
