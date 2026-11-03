# Bike Engine Stand and Belt Reduction — Design Note

**Date:** 2026-11-03. **Branch:** `study/minimal-1448`. **Status:** first design, for review before CAD. Engine figures must be confirmed on the engine you buy (service manual, a measured engine).
**Calculations:** `CAD/analysis/bike_reduction.js`. **Concept:** `reports/Minimal_1448_Study.md`.
**Frame:** X forward from the transom, Y + to port, Z up from the hull bottom (inches), as in the rest of the project.

## 1. Drive train in one line

GSX-R1000 K5–K8 engine → its own gearbox locked in **4th gear** → **14MGT synchronous belt, 30T to 47T** → **1-1/2 in prop shaft** on two bearings → **66 in 3-blade prop at about 2,600 rpm**.

## 2. Engine data used

| Item | Value | Source / status |
|---|---|---|
| Gearbox ratios | 2.562, 2.052, 1.714, **1.500 (4th)**, 1.360, 1.269 | K5 spec ([motorcyclespecs](https://www.motorcyclespecs.co.za/model/suzu/suzuki_gsxr1000%20mat%20black%2005.htm)) |
| Primary reduction (crank to gearbox) | about 1.55 | **Not confirmed**: check the service manual |
| Rear-wheel power and torque | about 162 hp at 11,500 rpm; about 80 lb-ft at 8,500, nearly flat from 8,000 to 11,000 rpm | 2005 dyno ([Motostatz](https://motostatz.com/?p=311)) |
| Operating point | **9,500 rpm, about 140 hp at the gearbox output** | Chosen: below peak power, on the flat of the torque curve, for continuous load |
| Front sprocket | 17T, #530 chain | ([JT Sprockets](https://www.jtsprockets.com/catalogue/model/s2575)) |

## 3. Choosing the gear and the belt ratio

The prop wants about **2,600 rpm**: a 66 in prop then has a tip speed of about Mach 0.67, well under the Mach 0.85 limit (R2.7 on the rev8 branches), and is in the usual airboat range. The overall ratio is 9,500 / 2,600 ≈ 3.65, split between the bike's primary and gearbox and the belt.

| Gear locked | Countershaft rpm at 9,500 | Countershaft torque | Belt ratio needed |
|---|---|---|---|
| 3rd | 3,569 | 206 lb-ft | 1.37 |
| **4th** | **4,078** | **180 lb-ft** | **1.57** |
| 5th | 4,498 | 163 lb-ft | 1.73 |
| 6th | 4,820 | 153 lb-ft | 1.85 |

**4th gear** is the middle choice. A lower gear means more torque through the belt (a wider belt); a higher gear means a faster small pulley (more belt speed). All are within what the gearbox already carries: in 1st gear the bike sees about 320 lb-ft at the countershaft.

**Locking the gear:** select 4th, remove the shift linkage and fit a detent stop or lock plate on the shift drum (or shaft) so it cannot jump out of gear. The gearbox is never shifted with the engine running.

**Clutch (decided 2026-11-03): the bike clutch stays, worked by a hand lever on the steering stick.**
- **Lever:** a motorcycle or ATV clutch lever with a parking lock (a latch that holds it pulled in) and a clutch switch, clamped to the stick grip so the driver works it with the steering hand. Use a lever ratio that matches the bike lever, so the pull stays light.
- **Cable:** a longer custom clutch cable (about 5–6 ft) from the stick to the clutch arm on the engine. Leave a slack loop at the stick pivot so full stick travel does not pull the clutch, and route it clear of the belt and exhaust. (K5–K8 clutches are cable-operated; check the engine you buy.)
- **Starting:** pull and lock the lever, start on the button (the bike starter then spins only the engine, as designed), let it warm up with the prop stopped, then release the lever slowly at idle to bring the prop up to speed.
- **Interlocks:** keep the clutch switch in the start circuit (no start unless the lever is pulled); jumper the side-stand switch; keep the tip-over sensor, mounted level, so the engine stops if the boat capsizes. The lanyard kill switch stays the main stop.
- **At the dock:** lock the lever in to idle with the prop stopped; never leave the boat with the engine running.

## 4. Belt

| | **14MGT 30T / 47T (chosen)** | 8MGT 45T / 71T (alternative) |
|---|---|---|
| Pitch diameters | 5.26 / 8.25 in | 4.51 / 7.12 in |
| Prop rpm at 9,500 engine rpm | 2,603 | 2,585 |
| Belt speed | 5,620 ft/min (28.5 m/s) | 4,820 ft/min (24.5 m/s) |
| Effective tension at 140 hp | 822 lbf | 959 lbf |
| Shaft load (about 1.6 × effective tension) | about 1,315 lbf | about 1,535 lbf |

- **Width is not yet sized.** I could not get Gates' rating tables; size it in Gates DesignFlex (inputs: 140 hp, 4,078 rpm, 30T/47T 14MGT, center distance about 28 in, service factor for an airboat at least 1.5). Expect a Poly Chain GT Carbon 14MGT in the 37–68 mm range; confirm before buying.
- **Countershaft load is fine:** the belt pull (about 1,315 lbf) is well under the bike's own worst chain pull on that shaft (about 2,250 lbf: peak torque in 1st on the 17T sprocket), as long as the pulley sits where the sprocket sat.
- **Small pulley on the countershaft:** a 30T 14MGT pulley bored and splined to the countershaft, or bolted to a steel adapter cut from a 17T #530 sprocket (the sprocket's spline and retaining nut stay). Keep it on the same line as the sprocket.
- **Tension:** slotted prop-shaft bearing plates with jack bolts, so the whole upper shaft moves to tension the belt (no idler).

## 5. Prop shaft and bearings

Torque at the prop: about 282 lb-ft. Thrust: about 580 lbf (estimate). Belt pull: about 1,315 lbf, with the pulley 2 in outboard of its bearing.

| Shaft (4140 pre-hardened) | von Mises stress | Factor against about 30 ksi fatigue |
|---|---|---|
| 1-1/4 in | 20,900 psi | 1.4 |
| 1-3/8 in | 15,700 psi | 1.9 |
| **1-1/2 in (chosen)** | **12,100 psi** | **2.5** |

- **Layout (straddle mount):** aft bearing at X 8, belt pulley at X 13 between the bearings, forward bearing at X 20; the prop hub overhangs about 3 in aft of the aft bearing. With the pulley between the bearings, the shaft bending is lower than the overhung case in the table above, so 1-1/2 in has extra margin.

**Thrust bearing (decided 2026-11-03):**

| | **Forward: thrust bearing** | **Aft: radial only** |
|---|---|---|
| Unit | 4-bolt **tapered-roller** flange unit, 1-1/2 in bore (Dodge F4B-E-108R class) | 4-bolt **ball** flange unit, UCF208-24 |
| Rating (dynamic, radial) | about 7,070 lbf ([listing](https://mro-pt.com/bearings/mounted-bearings/mounted-tapered-roller-bearings/075478-dodge-f4b-e-108r-dry/)) | about 6,542 lbf ([Timken](https://www.mrosupply.com/bearings/5100834_ucf208-24_timken/)) |
| Weight | about 9.5 lb | about 4.2 lb |
| Radial load | about 770 lbf (belt share and prop gyroscopic load) | about 1,016 lbf (belt share, gyroscopic load, prop weight) |
| Thrust | all of it: about 580 lbf static | none |
| L10 life | about 2,500 h at full static thrust held continuously; about 9,600 h at cruise thrust (about 320 lbf); about 880 h at 1.5 x static thrust | about 1,700 h at full belt load |

- **Why tapered rollers:** they are made for combined radial and thrust load and carry thrust in both directions (the Type E unit has two cone rows). A plain ball insert unit held by set screws should not carry the prop thrust.
- **Holding the shaft for thrust:** prop thrust pushes the shaft **forward**. A heavy-duty **two-piece steel clamp collar** sits hard against the aft face of the forward bearing's inner ring and passes the thrust into it; a second clamp collar on the forward side takes any reverse load (windmilling, a bump). The unit's own set screws then only locate it. Choose collars rated at least about 1,750 lbf axial (twice the design thrust). No shaft machining is needed.
- **Gyroscopic load:** a hard turn at about 30°/s with the prop at 2,600 rpm puts a moment of about 2,700 in-lb on the shaft (about 220 lbf on each bearing); it is included in the loads above.
- **Bearing plates:** the forward plate takes the thrust out of plane, so make it **1/2 in 6061** (about 8,700 psi at 1.5 x thrust; 3/8 in would be about 15,500 psi, too high for fatigue), about 6 in deep, bolted between the stand side frames. Both plates have vertical slots and jack bolts so the whole shaft moves up and down to tension the belt.
- **The load ratings above come from distributor listings** and the thrust method is the generic tapered-roller formula (P = 0.4 Fr + 1.5 Fa). Confirm with the Dodge (or other maker's) catalog method for the exact unit before buying.
- **Prop end:** a steel prop flange keyed to the shaft (see Section 5a for the bolt pattern).

## 5a. Prop (chosen 2026-11-03)

**Whirlwind CarbonMax 68, 3-blade, 66 in**, ground-adjustable composite airboat prop (carbon spar, nickel leading-edge shield).

| Item | Value | Source |
|---|---|---|
| Diameter range of the model | 66–68 in; **choose 66 in** | Whirlwind installation guide |
| Maximum rpm | **3,150**; normal range 2,500–3,150 | Whirlwind installation guide |
| Pitch | adjustable 7.5–17.5° in 2.5° steps; start at 15° | Whirlwind installation guide |
| Hub mounting | 6 x 1/2 in grade 8 fine-thread bolts on a **4.750 in circle**, **2.260 in pilot** (SAE #2 / LXL flange); 45 ft-lb | Whirlwind installation guide |
| Rated application | 160–220 hp airboats (the 68 series) | ([Whirlwind](https://whirlwindpropellers.com/airboats/hp/160-220/)) |

**Why this prop:**
- **Our operating point fits it:** about 2,600 prop rpm at 9,500 engine rpm is inside its 2,500–3,150 rpm range, with 550 rpm of margin to the maximum. Tip speed is Mach 0.67 at 2,600 and Mach 0.81 at the 3,150 maximum, both under 0.85.
- **Adjustable pitch** sets the engine rpm without changing the belt: at a static full-throttle run, raise the pitch until the engine reaches its target. Whirlwind suggests a static prop rpm 100–200 below maximum for the most thrust; this design deliberately runs lower (about 2,600) for engine life, fuel and noise, which Whirlwind also allows.
- **66 in, not 68 in:** 66 in keeps about 4 in between the lower tip and the 15 in transom top at the Z 52 axis (68 in would leave about 3 in) and a smaller cage. 68 in would add only about 4% thrust.
- **3-blade:** absorbs about 140 hp in a 66 in disc at a moderate pitch; a 2-blade would need much more pitch.

**Fitting it:**
- **Prop flange:** a steel flange machined to the 6-bolt 4.750 in circle with a 2.260 in pilot, bored 1-1/2 in and keyed to the prop shaft, held by a shaft locknut and washer (a machine-shop job). Airboat suppliers also sell SAE #2 prop flanges that can be bored to suit.
- **Rotation:** with the sprocket side of the engine facing aft, the prop turns the way the countershaft drove the rear wheel. Turn the engine by hand in 4th to confirm the direction, then order the hub for that rotation (the hub is marked left and right).
- **Clearance:** at least 2 in from the blades to every part of the stand, engine and cage (Whirlwind's minimum).
- **First runs:** start at 15° pitch, run to 2,000 prop rpm, check bolt torque and that no blade moves in the hub; recheck torque after 5 h, then every 25 h (Whirlwind's procedure).

**Price:** not published online; call Whirlwind or a dealer. Planning estimate about $1,800 for the 3-blade, plus about $250 for the flange. **Budget alternative:** a Warp Drive 66 in 3-blade with its HP hub, listed at about $805–995 ([Warp Drive](https://warpdriveprops.com)); its maximum rpm is not published, so get it from Warp Drive (1-251-609-0969) before choosing it.

## 6. Layout on the 1448

- **Engine turned 90°:** crank along X (fore and aft), cylinders up, the sprocket side facing **aft**. The countershaft is then parallel to the prop shaft and the belt runs straight up behind the engine.
- **Engine:** sump bottom at about Z 14, countershaft at about Z 24, engine between about X 14 and X 31.
- **Belt:** from the countershaft (about Z 24) up to the prop shaft pulley (Z 52), center distance about 28 in, at about X 13.
- **Prop shaft:** Z 52 (the same axis height as the rev7 cage, so the 66 in disc clears the 15 in transom top by about 4 in), bearings at about X 12 and X 22, prop disc at about X 5.
- **Stand:** bolted 6061, 2 x 2 x 1/8 in square tube with 1/4 in gusset plates (the project's no-welding rule for aluminum):
  - two side frames, each a rectangle from the beds up to a bearing plate at Z 52, with a diagonal from the top forward to the bed (about 36 in fore-and-aft base);
  - cross tubes at the engine mounts (the engine hangs from its bike frame mount bosses on two plates), at the top (bearing plate), and at the feet;
  - two short beds (2 x 6 x 1/8 rectangular tube, about 40 in) across the ribs, each foot plate bolted down into rivnuts.
- **Loads on the stand:** 580 lbf thrust at Z 52 is a 30,000 in-lb overturning moment: about **420 lbf up at the aft feet and down at the forward feet** per side, plus the 282 lb-ft prop torque (reacted side to side). The cage bolts to the stand top and the transom.

## 7. Weight (estimates)

| Item | lb |
|---|---|
| Engine with gearbox, oil and coolant | 145 |
| Belt, pulleys, prop shaft, bearings and collars, tensioner | 39 |
| Radiator and hoses | 12 |
| Exhaust | 10 |
| Engine stand (6061 tube and plates) | 41 |
| Short beds and doublers | 15 |
| **Total** | **262** |

That is about 37 lb more than the concept study assumed (180 lb engine system plus a 45 lb stand). With it, the minimal 1448 has about **487 lb of equipment and about 338 lb of payload** under the 825 lb plate (was 375 lb). Solo dock trim rises to about 2.4° (target 0.5–2°); carry gear forward of the driver or lighten the stand. Speeds are unchanged: about 39 mph solo, 37.5 mph with a passenger.

## 8. Decisions and checks before the CAD

1. ~~Start-up method~~: decided, the bike clutch with a lever on the steering stick (Section 3).
2. **Primary reduction** and the countershaft spline: confirm on the engine (service manual).
3. **Belt width** from Gates DesignFlex; the pulley width follows.
4. ~~Thrust bearing type~~: decided, a tapered-roller flange unit forward and a ball flange unit aft (Section 5).
5. ~~Prop~~: decided, Whirlwind CarbonMax 68, 3-blade, 66 in, 3,150 rpm maximum (Section 5a). Still to do: confirm rotation, get a price, order the flange.
6. **Cooling:** radiator position in the prop wash and the bike's thermostat and fan.
