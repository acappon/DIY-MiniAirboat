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

**Locking the gear:** select 4th, remove the shift linkage and fit a detent stop or lock plate on the shift drum (or shaft) so it cannot jump out of gear. There is no clutch in use: start in gear with the clutch lever tied in (or the clutch cable to a lever on the stand), or start in neutral and select 4th at idle with the prop stopped. **Decide which before the CAD.**

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

- **Bearings:** two bearings about 10 in apart. The **forward bearing carries the thrust**: a pair of tapered rollers (a trailer-hub style housing) or an angular-contact flange unit. Do not rely on set-screw collars for thrust: use a shaft shoulder and a locknut.
- **Prop end:** a 6-bolt airboat prop hub keyed and pressed (or tapered) on the aft end.

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
| Belt, pulleys, prop shaft, bearings, tensioner | 32 |
| Radiator and hoses | 12 |
| Exhaust | 10 |
| Engine stand (6061 tube and plates) | 41 |
| Short beds and doublers | 15 |
| **Total** | **255** |

That is about 30 lb more than the concept study assumed (180 lb engine system plus a 45 lb stand). With it, the minimal 1448 has about **480 lb of equipment and about 345 lb of payload** under the 825 lb plate (was 375 lb). Solo dock trim rises to about 2.35° (target 0.5–2°); carry gear forward of the driver or lighten the stand. Speeds are unchanged: about 39 mph solo, 37.5 mph with a passenger.

## 8. Decisions and checks before the CAD

1. **Start-up method** with no clutch in use (Section 3).
2. **Primary reduction** and the countershaft spline: confirm on the engine (service manual).
3. **Belt width** from Gates DesignFlex; the pulley width follows.
4. **Thrust bearing type:** tapered-roller housing or angular-contact flange unit.
5. **Prop:** pick the actual 66 in 3-blade and its maximum rpm (it must exceed about 2,600 with margin).
6. **Cooling:** radiator position in the prop wash and the bike's thermostat and fan.
