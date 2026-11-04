# Rotax 800R E-TEC Drivetrain — Design Note

**Date:** 2026-11-03. **Branch:** `main`. **Status:** first design, for review before CAD. It replaces the GSX-R1000 design (archived: `archive/rev8_JonAirBoat/Bike_Engine_Stand_and_Reduction.md`). Engine figures must be confirmed on the engine you buy (BRP service manual, a weighed engine).
**Why this engine:** `reports/Engine_Selection_1448.md`. **Calculations:** `CAD/analysis/etec_reduction.js`, `CAD/analysis/minimal_1448.js`.
**Frame:** X forward from the transom, Y + to port, Z up from the hull bottom (inches).

## 1. Drive train in one line

Rotax 800R E-TEC two-stroke twin → **8MGT synchronous belt, 32T on the crank PTO to 94T** (2.94:1) → **1-1/2 in prop shaft** on two bearings → **Warp Drive 66 in 3-blade prop at about 2,590 rpm**. No gearbox, no clutch: the prop turns whenever the engine runs, as on most airboats.

## 2. Engine data used

| Item | Value | Source / status |
|---|---|---|
| Displacement, bore x stroke | 799.5 cc, 82 x 75.7 mm | BRP spec sheet ([2018 Renegade Enduro](https://ski-doo.brp.com/content/dam/ski-doo/United-States/MY2018/Documents/Spec-sheets/2018_Renegade%20Enduro.pdf)) |
| Maximum engine speed | 7,900 rpm | BRP spec sheet |
| Rated power | not published by BRP | magazine figures are about 150–160 hp: unverified |
| **Operating point** | **7,600 rpm, 120 hp at the prop shaft** | Chosen: below the maximum speed, about 75–80% of the rated power, for continuous load and belt losses |
| Bare weight | 79 lb | [PowersportsGuide](https://powersportsguide.com/snowmobile-weight/), no source given: **weigh it** |
| Crank torque at the operating point | about 83 lb-ft | 120 hp x 5,252 / 7,600 |

**Buying it:** get the engine with its **ECU, harness, DESS key**, oil pump, oil tank, tuned pipe and silencer, or buy a whole running sled (about $2,900 for a 2013 MXZ TNT 800R) and resell the rest. Complete used engines list at about $1,100–2,500. Prefer one with electric start or BRP's SHOT start.

## 3. Belt

| | **8MGT 32T / 94T (chosen)** | 8MGT 36T / 106T | 14MGT 28T / 82T |
|---|---|---|---|
| Pitch diameters | 3.21 / 9.42 in | 3.61 / 10.63 in | 4.91 / 14.39 in |
| Prop rpm at 7,600 | 2,587 | 2,581 | 2,595 |
| Belt speed | **6,383 ft/min (32.4 m/s)** | 7,181 ft/min (36.5 m/s) | 9,774 ft/min (49.7 m/s) |
| Effective tension at 120 hp | 620 lbf | 551 lbf | 405 lbf |
| Shaft load (about 1.6 × tension) | about 993 lbf | about 882 lbf | about 648 lbf |

- **Why 8MGT 32T:** it keeps the belt at or under about 6,500 ft/min, the usual limit for pulleys that aren't dynamically balanced. The bigger pulleys lower the belt pull but need balanced pulleys; 14MGT is far too fast at crank speed.
- **Width is not yet sized.** Size it in Gates DesignFlex: 120 hp, 7,600 rpm, 8MGT 32T/94T, center distance about 28–30 in, service factor at least 1.5. Expect a Poly Chain GT Carbon 8MGT in the 36–62 mm range.
- **Small pulley on the crank PTO:** the sled's drive clutch sits on a tapered PTO end with a center bolt. Use a pulley hub machined to the same taper (or a clutch-taper adapter) and the stock bolt. **Confirm the taper from the service manual.**
- **Side load on the crank: open item.** The belt puts about 1,000 lbf of side load on the PTO end, about 2,500 in-lb at the PTO main bearing with the pulley 2.5 in out. The sled's drive clutch also loads this bearing, but differently. If the service manual or BRP can't confirm it, add an **outboard support bearing** for the small pulley on the engine mounts (about +5 lb, about $60).
- **Tension:** slotted prop-shaft bearing plates with jack bolts, as before (no idler).
- **Quieter 4-blade option** (Build Guide 2.0): a 104T large pulley (10.43 in) gives about 2,340 rpm at the same engine speed.

## 4. Prop shaft and bearings

Carried over from the bike design. All the loads are lower now:

| | Bike design | **800R E-TEC** |
|---|---|---|
| Prop torque | 282 lb-ft | **244 lb-ft** |
| Static thrust (estimate) | 580 lbf | **497 lbf** |
| Belt pull on the shaft | 1,315 lbf | **993 lbf** |
| 1-1/2 in 4140 shaft, factor against 30 ksi fatigue | 2.5 | **3.0** (1-3/8 in would be 2.3) |
| Forward tapered-roller bearing L10 at full static thrust | about 2,500 h | **about 4,700 h** |
| Aft ball bearing L10 at full belt load | about 1,700 h | **about 4,000 h** |

- **Layout (straddle mount):** aft bearing at X 8, belt pulley at X 13, forward bearing at X 20; the prop hub overhangs about 3 in aft of the aft bearing.
- **Forward (thrust) bearing:** a 4-bolt tapered-roller flange unit, 1-1/2 in bore (Dodge F4B-E-108R class). Two heavy-duty two-piece steel clamp collars, one each side of its inner ring, carry the thrust (forward) and any reverse load; the set screws only locate it. Collars rated at least 1,750 lbf axial.
- **Aft bearing:** UCF208-24 ball flange unit, radial only.
- **Bearing plates:** 1/2 in 6061, about 6 in deep, bolted between the stand side frames, with vertical slots and jack bolts for belt tension.
- Ratings are from distributor listings and the generic tapered-roller formula (P = 0.4 Fr + 1.5 Fa); confirm with the maker's catalog before buying.

## 5. Prop (unchanged: Warp Drive 3-blade with HP blades, 66 in)

- **Model and price:** ground-adjustable carbon blades in a 2-piece aluminum hub, cut to 66 in, on an SAE #2 face plate; **$1,275** without face plate and bolts, about **$1,625** with the face plate (about $100) and a keyed steel flange (about $250) ([Warp Drive](https://warpdriveprops.com/warpdrivepropsairboats/)). The cheaper standard blades are for engines under 120 hp; this engine sits at that line, so stay with the HP blades.
- **Before ordering:** get the maximum rpm at 66 in from Warp Drive (not published; it must be well above 2,600), confirm the SAE #2 bolt circle and pilot, and tell them the rotation.
- **Rotation:** the prop turns the same way as the crank (one belt stage). Turn the engine by hand to confirm the direction.
- **Fallback:** Whirlwind CarbonMax 68, 3-blade, 66 in, published maximum 3,150 rpm, 6 x 1/2 in bolts on a 4.750 in circle.
- **Fitting:** at least 2 in from the blades to every part of the stand, engine and cage. Track the blades, torque the bolts to the maker's values, run up gradually and recheck the torque after the first few hours. Set the pitch on a tied-down run so the engine reaches about 7,600 rpm at full throttle, never over 7,900.

## 6. Engine systems

- **Cooling:** a sled cools through heat exchangers in the tunnel, so the boat needs a **radiator in the prop wash**, with the engine's thermostat and the coolant hoses from the sled. About 15 lb with coolant. Size it for about 120 hp continuous; mount it where the prop wash reaches it at idle too.
- **Exhaust and intake:** see Section 6a. Keep the sled's **airbox (intake silencer)**: on an open engine, two-stroke intake roar is about as loud as the exhaust.
- **Oil:** E-TEC engines inject oil from the sled's oil tank with an electric oil pump. Keep the oil tank, mount it upright, and run the engine on straight gasoline.
- **Fuel:** use the sled's in-tank fuel pump module (or one of the same pressure) in the boat's tank. Estimated burn about **10.8 gal/h at full power** and **5.4 gal/h at a 60 hp cruise**: a 6 gal tank lasts about 33 minutes flat out, or about 1.1 hours cruising. Measure it on the static test.
- **Electrical:** the ECU, harness and DESS key from the sled; the lanyard kill switch wired into the ECU's stop circuit; the small lithium battery for starting and the lights.
- **Throttle:** the sled's throttle cable to a hand lever or foot pedal.
- **Noise:** US snowmobiles are built to meet about 78 dB(A) at 50 ft at full throttle with the stock silencer and airbox (to confirm for the model year), and that figure includes the track. With the prop (about 81 dBA at 2,590 rpm) the loudest source, expect about 82–84 dBA at 50 ft, about the same as the bike engine. California's 75 dB(A) shoreline limit is still out of reach. The 4-blade prop (Build Guide 2.0) lowers the total more than a bigger muffler does.
- **Engine life:** budget a top-end rebuild (pistons, rings) at the interval in the service manual; a top-end kit is about $1,300.

## 6a. Exhaust layout (decided 2026-11-03)

**Parts, in order:** the engine's exhaust manifold → **stock tuned pipe (expansion chamber)** → **stock silencer** → optional second muffler (space reserved, fitted only if needed) → **tailpipe** over an aft corner of the transom.

**The tuned pipe stays stock.** BRP tuned its length and cone shapes for the 800R's port timing, and a lot of the engine's power depends on them. Don't cut, reshape or zigzag it; a custom pipe would be expensive and would likely lose power. With the crank running fore and aft, the exhaust ports face one side of the boat, so the pipe starts low on that side and loops about as it does in the sled. **Choose the engine's roll angle on the stand so the stock pipe fits.** That's a CAD job with the real pipe measured: expect roughly 3–4 ft of centerline and a belly about 5 in across.

**Muffling:**
- **Start with the stock silencer.** It is already a large, low-restriction part, and the prop (about 81 dBA at 50 ft) is louder than the exhaust: cutting the exhaust from about 76 to about 70 dBA lowers the total only from about 82 to about 81.5 dBA (estimates).
- **Reserve space for a second muffler** above the engine (about 6 in diameter x 18 in, to size), but fit it only if the static test's sound readings call for it.
- **If you add one:** a large straight-through, packed muffler with flow at least equal to the stock silencer. A tuned two-stroke is sensitive to restriction after the pipe: too much costs power and raises the exhaust temperature, which can burn a piston. Read the spark plugs after the first runs.
- **Weight:** a second muffler and its mounts are about 8–12 lb. The payload margin is only 2 lb (Section 8), so take the same weight out of the outfit before fitting it.

**Where it goes (air flows from the bow aft into the prop):**
- **Pipe and silencer above and beside the engine,** inside the area the engine and the bearing stand already block, near the prop axis: about X 14–30, Z 35–55. The center of the disc around the hub makes little thrust, so parts there cost almost nothing.
- **Not across the front of the cage.** Anything wide ahead of the outer half of the disc blocks thrust and makes a beat each time a blade passes (more noise).
- **Not upstream of the radiator.** The radiator gets clean air, alongside or forward of the exhaust parts, never behind them.
- **Belt and bearings:** keep the pipe at least 4–6 in from the belt (X about 13, Z 22–52) and the prop-shaft bearings, with a sheet-aluminum heat shield between. Use heat shields, not wrap, on the pipe itself.
- **Mounting:** the engine is on rubber mounts and the stand is not. Keep the sled's springs and ball joint at the pipe connections, and use a flexible joint between anything mounted on the engine and anything mounted on the stand.

**Tailpipe:**
- **Over an aft corner of the transom, not the center.** At the center the prop tips pass about Z 19 above a 15 in transom top: only about 4 in, so a pipe there can't keep the 2 in blade clearance and sits right at the tips. At an aft corner (about Y ±28 in, Z 15) the tips are about 13 in away.
- **Route:** from the silencer down the side of the stand, along the inside of the hull side, and out over the corner. Point the outlet aft and slightly outboard, **never into the prop**: hot gas is bad for the carbon blades.
- **Water:** transom freeboard at the dock is only about 9 in. A following wake or a swamping could push water up the tailpipe and into the cylinders and lock the engine. Keep the outlet at the transom top or higher; consider a flapper or a short upturn.

**Burns:** the pipe sits about 30–40 in behind the driver's seat, where people climb in and out. Put a guard or a perforated shield wherever a hand, knee or passenger could touch it.

## 7. Layout on the 1448

- **Engine turned 90°:** crank along X (fore and aft), cylinders up, PTO facing **aft**. The belt then runs straight up from the crank to the prop shaft at about X 13.
- **Engine:** crank at about Z 22 (center distance about 30 in to the prop shaft at Z 52), engine between about X 14 and X 30. The 800R is shorter and about 66 lb lighter than the GSX-R1000.
- **Prop shaft:** Z 52, bearings at about X 8 and X 20, prop disc at about X 5.
- **Stand:** bolted 6061, 2 x 2 x 1/8 in square tube with 1/4 in gusset plates, on two short beds across the ribs, as in the bike design. **Target 45 lb with the beds** (the bike stand came to 56 lb for a 145 lb engine). The engine hangs on its own sled mounting bosses.
- **Loads:** 497 lbf of thrust at Z 52 is about 25,800 in-lb of overturning moment, about **360 lbf** up at the aft feet and down at the forward feet per side, plus 244 lb-ft of prop torque.

## 8. Weight and payload

| Item | lb |
|---|---|
| Engine, bare (unverified) | 79 |
| Tuned pipe and silencer | 20 |
| Radiator, hoses and coolant | 15 |
| Belt, pulleys, prop shaft, bearings, collars, plates | 39 |
| Oil tank and oil, ECU, harness | 10 |
| Engine mounts | 5 |
| **Engine system** | **168** |
| Stand and beds (in the outfit) | 45 (target) |

With the 250 lb outfit and the 15 lb prop, equipment is 433 lb and payload is **392 lb** under the 825 lb plate: **8 lb short** of the 400 lb that driver + passenger or driver + 150 lb of gear needs.

**Outfit trim (decided 2026-11-03):** UHMW strips over the wear zones only (the keel and chine areas from the bow rise aft), about 45 → 35 lb. Equipment drops to about 423 lb and payload to about **402 lb**, which meets both load cases.

**Exhaust layout (Section 6a) adds about 5 lb** (tailpipe to the transom corner, heat shields, pipe guard): equipment about 428 lb, payload about **397 lb**. Driver + 150 lb of gear still fits; driver + passenger is about 3 lb over.

**Allowances and cage-top lights (added 2026-11-03, about 43 lb):**

| Item | lb (est.) |
|---|---|
| Minimal dash: tach, coolant and exhaust temperature gauges, key/DESS post, start button, panel | 3 |
| Wiring, fuse block, switches | 3 |
| Belt guard | 3 |
| Radiator brackets, coolant overflow bottle | 3 |
| Brackets, rivnuts and bolts not counted elsewhere | 5 |
| Bilge pump and hose | 2 |
| Passenger cushion and grab handle | 4 |
| LED light bar, 30 in single row, on the cage top, with bracket | 10 |
| Motorized remote searchlight (Golight Stryker LED class), on the cage top, with mount | 8 |
| Light wiring and switches | 2 |
| **Total** | **43** |

The anchor, line and paddle are counted as part of the 150 lb of gear.

**Safety and regulatory gear (itemized 2026-11-03; was one 15 lb line):** PFDs (2) and dock lines 8 lb; USCG B-I / UL 5-B fire extinguisher with bracket 4 lb, mounted near the fuel tank but within the driver's reach, not over the tank; air horn with bracket 2 lb, at the driver; **Florida airboat flag** (10 x 12 in international orange, at least 10 ft above the lowest part of the boat, Fla. Stat. 327.391; check the current FWC rule) on a whip with a fold-down mount 3 lb. Mount the flag on the **aft side of the prop cage**, so the prop blast blows it away from the blades; never forward of the prop. Net change: +2 lb.

**Result:** equipment about **473 lb**, payload about **352 lb** under the 825 lb plate.

| Load case | Needs | Margin | Top speed |
|---|---|---|---|
| Driver + 150 lb of gear | 350 lb | **+2 lb** | 34.25 mph (gear in the bow) |
| Driver + passenger (not a requirement since 2026-11-03) | 400 lb | −48 lb (over) | 35 mph |

Solo dock trim is about 2.35°, just over the 0.5–2° target; stow gear forward of the driver. **Design case (2026-11-03): driver + 150 lb of gear only;** the passenger requirement was dropped. The margin is only about 2 lb, so any weight added as the details are designed needs an offsetting cut.

**Electrical load:** the light bar (about 10.3 A), searchlight (about 3.7 A) and navigation lights come to about 15 A, about 200 W at 13.8 V. Check the 800R's spare charging output (service manual) before fitting, and size the battery for running the lights with the engine off. If the stand comes out over 45 lb, or the engine weighs more than 79 lb, take the difference out of the outfit too.

Speeds, from the model before the allowances below: about **35 mph** with driver + 150 lb of gear stowed amidships (34 mph with it in the bow) and **34.75 mph** with driver + passenger. Both get on plane easily, with more than 250 lbf of thrust to spare at the hump. Dock trim is 0.5–2°.

## 9. Decisions and checks before the CAD

1. **PTO taper and side load:** from the service manual. Decide whether the small pulley needs an outboard bearing.
2. **Belt width** from Gates DesignFlex.
3. **Prop maximum rpm** from Warp Drive.
4. **Radiator** size and position in the prop wash.
5. **Exhaust (Section 6a):** measure the stock pipe and silencer, set the engine's roll angle so they fit, reserve the second-muffler space, and route the tailpipe to an aft transom corner.
6. **Sound readings on the static test** (at 50 ft and at the driver's seat) to decide on the second muffler.
7. **Weigh the engine** with its pipe when it's bought, and update `minimal_1448.js`.

## 9a. First CAD layout (2026-11-03)

Onshape workspace Main (merged from `Minimal 1448`, 2026-11-03), part studio "Minimal 800R layout"; master `CAD/FeatureScript/Minimal_layout.fs`. Engine, pipe, silencer and radiator are placeholder envelopes, the engine roll angle is 0, and the exhaust is assumed on the port side. The blade sweep (X 4–6) and hub (3.5 in radius) are assumed.

**Choices the layout adds (provisional):**
- **Stand (trimmed to the 45 lb target, `CAD/analysis/stand_800r.js`; **beds replaced by four shoes and the rail raised to Z 14.9 in Section 9b**):** side frames at Y ±8.5 on 2 x 3 x 1/8 in beds, X 7–42. Aft posts and engine rails (Z 14) are 2 x 2 x 1/8 in. Forward posts on the rails, diagonals from the top of the forward posts to the beds at X 40, top rails and three ties are 1-1/2 x 1-1/2 x 1/8 in. The fourth tie, across the forward end of the beds, was removed: the hull bottom ties them there.
- **Heat shields:** three 0.040 in aluminum sheets on standoffs. One runs along the outside of the port side frame (X 11–31, Z 35–58), between the exhaust and the stand, belt and bearings. Two more guard the forward post and the diagonal where the pipe passes through the side frame. The pipe moved to X 25.5 there, and the silencer moved outboard to Y 15.5.
- **Stand cross-straps:** two 1 x 1/8 in flat straps in an X on the forward face of the diagonals, from Z 37 down to Z 8, tension only. The X stops below the cylinders, because a full-height X runs through the engine envelope.
- **Cage:** two 1-1/4 in hoops of 36 in radius at X 1 and X 11, four legs at Y ±30.5 (feet in Section 9b), eight stringers. Two 1 x 0.065 in round struts run from the forward hoop (45° up each side) to the top of the stand at X 21, so the cage bolts to both the hull and the stand. Rigid struts are used instead of guy wires: a failed wire ahead of the disc can be drawn into the prop. Fit rubber isolators at the strut ends if vibration calls for them.
- **Radiator:** 16 x 20 x 2 in on the starboard side at X 32–34, Z 44–64, forward of the exhaust and off the hub center, where the wash is weak.
- **Tailpipe:** it leaves the silencer outboard at X 15, runs outside the hoops and inside the cage legs, and exits at about Y 28, Z 16.5.
- **Flag:** whip to Z 132, flag at Z 120–130.

**Clearances measured:**

| Between | Inches | Target |
|---|---|---|
| Blade sweep to stand and bearing plates | 2.0 | at least 2 |
| Blade sweep to aft bearing corners | 1.0 | at least 2: **depends on the real hub**; these corners sit at the hub radius |
| Blade sweep to the cage | 2.6 | at least 2 |
| Tuned pipe and silencer to the bearing plates | 4.0 / 4.1 | 4–6 |
| Tuned pipe and silencer to the belt | over 4.5 | 4–6 |
| Tuned pipe to stand | 1.1 | shielded |
| Silencer to stand | 2.7 | shielded |
| Engine envelope to stand | 0.75 | check with the real engine |
| Tailpipe to cage leg | 0.9 | tight |
| Tailpipe to hoop | 1.9 | none set |
| Tailpipe to transom | 0.7 | none set |
| Aft hoop to transom | 0.5 | tight |
| Forward hoop to stand | 0.4 | tight |

**Weight:** the first stand came to about 55 lb. Trimmed, it is **45.0 lb** with an estimated 4 lb of gussets. The exhaust items now total about 7 lb (41 in of steel tailpipe 3.6 lb, shields and standoffs 2.2 lb, guard 1 lb), up from the 5 lb allowance. The cage struts and stand straps add about 2.5 lb with brackets. The UHMW allowance drops from 35 to **32 lb**, using the modeled strips: 864 in³ at 0.034 lb/in³ is 29.4 lb, plus 1.4 lb for the 56 screw sets, 0.4 lb of sealant and 0.8 lb of reserve. The passenger cushion and grab handle in Section 8 are removed (−4 lb): a passenger sits on the bare factory bench with nothing to hold, so the boat is treated as single-seat. Equipment is **470.5 lb** and payload **354.5 lb**: driver + 150 lb of gear has about **+4.5 lb of margin**. Solo dock trim is about 2.4°, over the 0.5–2° target: stow gear forward. The cage is still held to its 45 lb allowance.

**Hull fastening:** counted and designed, see Section 9b. Equipment is now **474.6 lb** and payload **350.4 lb**: about **+0.4 lb of margin**.

**Stand load check (`CAD/analysis/stand_800r.js`, static, full power):**

| Member | Load | Limit | Factor |
|---|---|---|---|
| Engine rail bending | 3,360 psi | 7,000 psi | 2.1 |
| Diagonal buckling | 918 lbf | 6,830 lbf | 7.4 |
| Forward post buckling | 379 lbf | 11,660 lbf | 30.8 |
| Aft post buckling | 462 lbf | 18,760 lbf | 40.6 |

This is a first hand check. It assumes the engine mounts sit at X 17 and X 27, and it does not cover two-stroke vibration, the joints, the gussets or the bolts.

**Side-to-side vibration:**

| Stage | Stiffness | Frequency | Prop matches at |
|---|---|---|---|
| Stand alone, cage on the hull | 1,040–2,080 lb/in | 13–18 Hz | 2,300–3,250 engine rpm |
| With straps, cage strutted on | 3,050–5,340 lb/in | **18–24 Hz** | **3,150–4,200 engine rpm** |

The ranges run from half-fixed to rigid joints. Pass through that rpm band rather than holding in it, and measure the real frequency with a tap test. The cage's own side sway was about 6 Hz before the struts; the struts should raise it toward the stand's frequency, but the hoop between the strut points was not calculated. Strut load at 3 g fore and aft is about 140 lbf, against a buckling limit of about 2,500 lbf.

## 9b. Hull mounts (2026-11-03)

Checking the layout against the reference mesh showed three clashes the first layout missed, because its reference hull had no bench or ribs: the engine rails hit the rear bench top (Z 13.7) by 0.7 in, the beds ran through the bench panels (which reach down to Z 2.4), and the beds hit the rib at X 37.75, whose top is at Z 1.52. The design below fixes all three without cutting the bench, so its flotation foam stays intact. Render: `renderedImages/Minimal_800R_hull_mounts_v1.0.0.4.png`.

**Stand:**
- **No continuous beds.** The rail closes the side frame above the bench, so each side stands on two separate shoes of the same 2 x 3 x 1/8 in tube.
- **Rail height:** moved from Z 14 to Z 14.9, which leaves 0.2 in over the bench top and 0.25 in under the forward hoop.
- **Aft shoes (X 2–12.75):** a short beam between the transom and the rear bench's aft panel, held by a 3 in wide 2 x 2 x 1/8 in clip at each end, so uplift at the aft post goes into those panels and not into the bottom skin. Six 1/4-20 bolts through the shoe and bottom add shear and sealing, with a 5 x 10.5 x 1/8 in 5052 doubler.
- **Forward shoes (X 34–44):** notched over the rib, with 0.08 in over the rib top, so a hard landing bears on the rib. Six 1/4-20 bolts through 1/8 in 5052 doublers, 6 in wide, cut around the rib.
- **Bottom bolts:** at Y ±8.5 they pass between the centerline and chine UHMW strips. Use truss heads (under the 3/16 in strip thickness) with sealant, and a crush sleeve inside the tube.

**Cage feet:**
- **Aft legs:** stand on the transom corner caps at about Z 17.9.
- **Forward legs:** slant forward from the hoop at X 11 to the bench top at X 14.5, next to the hull side, which widens the leg base from 10 in to 13.5 in.
- **Pads:** 1/8 in pads with matching backing plates and four 1/4-20 bolts each. The backing plates under the bench top need a small access pocket through the aft panel into the foam, refilled and sealed afterward.

**Load check (`CAD/analysis/stand_800r.js`, 2D frame per side, worst of rigid and pinned aft feet):**

| Item | Load | Limit | Factor |
|---|---|---|---|
| Rail X 9–17, full power | 4,910 psi | 7,000 psi | **1.4** |
| Aft shoe bending | 980 psi | 7,000 psi | 7.1 |
| Bench clip, two 1/4-20 | 128 lbf each | 950 lbf | 7.4 |
| Shoe bolts in shear | 25 lbf each | 950 lbf | 37 |

The aft foot lifts about 394 lbf per side at full power: about 137 lbf into the transom clip and 256 lbf into the bench clip. The forward foot presses down about 485 lbf. The rail's 1.4 is lower than the 2.1 in Section 9a because it now carries axial load as well as bending, and it assumes engine mounts at X 17 and X 27. Recheck it when the real mount points are known. This check does not cover vibration or the strength of the hull sheet itself.

**Clearances measured:**

| Between | Inches | Note |
|---|---|---|
| Rail to bench top | 0.2 | tight; fit a rubber strip if they touch under load |
| Rail to forward hoop | 0.25 | tight |
| Rail to engine envelope | 0.5 | check with the real engine |
| Forward shoe to rib top | 0.08 | meant to bear when loaded |
| Tailpipe to corner cap pad | 0.85 | shield if the pad runs hot |
| Tailpipe to aft leg | 1.0 | was 0.9 |
| Tailpipe to forward leg | 1.35 | |

**Weight:** the stand and shoes come to **41.5 lb**, down from 45. Fastening adds **7.6 lb** (doublers 2.3, clips 0.6, cage pads and backing 1.6, 56 bolt sets 2.8, sealant 0.3), so the model carries 49.1 lb in place of 45. Top speed and trim barely change: solo dock trim is about 2.44°, 0.86° with driver + 150 lb of gear. The clip and pad weights assume no extra parts appear in the fit-up.

**Check on the real hull before cutting metal:**
1. Bench panels: their gauge, how they are fastened to the hull, and whether they really stop at Z 2.4.
2. Whether the corner caps are aluminum and strong enough to carry a leg. This comes from the mesh only.
3. What is inside the rear bench, and whether the access pocket is practical.
4. Rib height at Y ±8.5 and the hull bottom gauge.
5. Transom condition: the clips bolt low on it, at about Z 4.5.

## 9c. Cockpit (2026-11-03)

Onshape: part studio "Minimal cockpit" (master `CAD/FeatureScript/Minimal_cockpit.fs`) and the tab **Main assembly**, which holds the hull mesh, the 800R layout, the cockpit and the driver. Render: `renderedImages/Minimal_cockpit_v1.0.0.4.png`. Sizes are placeholders and the cable and line routes are provisional.

- **Driver:** on the factory middle bench, centered, at X 82. The raised seat (15 lb) is gone. The passenger figure is removed.
- **Dry box:** a waterproof case about 10.6 x 9.7 x 6.9 in, set into the middle bench through its forward panel with the lid facing forward, X 82.6–89.5, Z 2–11.7. It holds the lithium battery and the power distribution fuse block. A sealed case keeps about the same flotation as the foam it replaces.
- **Steering stick:** to port at Y 21, on the rib at X 95.25, with its grip at Z 31.5. A push-pull cable runs aft along the port side above the benches.
- **Throttle pedal:** at Y -2.5, hinged at the heel on the floor, with the rib at X 108 as its full-throttle stop. A push-pull cable runs along the floor to the starboard side, then aft above the benches.
- **Heel rests:** two 1 in half-rounds, 6 in long, at X 100.9 and 102.7. They are held by the UHMW strip screws, so they add no new holes through the bottom.
- **Console:** to starboard, on a 1 x 0.065 in tube post from the rib at X 108 and an arm out to the gunwale. The pod runs Y -4.75 to -21.25 with its underside at Z 24, and holds the tach, coolant, exhaust and fuel gauges and the key post.
- **Two side by side (hand check):** with hip points at Y ±11, knees top out at about Z 19, leaving 5 in under the console. The passenger's shin clears the console post by about 3 in, and the driver's thigh clears the stick by about 2 in. The driver's right foot stretches about 9 in to the pedal. The pedal sits only about 0.5 in from where the passenger's left foot would rest, so that foot must stay clear of it.
- **Fuel tank:** a 6 gal tank on the front deck step, X 134–152, strapped down, with the sled pump module, a top-mount gauge sender and a fuel line along the starboard side above the benches.
- **Fire extinguisher:** it can no longer be both near the tank and within reach, so it moved to the starboard side next to the driver's right knee, at X 99.

**Weight:** the seat comes out (−15 lb) and the cockpit adds 8 lb: dry box and battery cable 3.4, console frame and pod 2.6, sender, gauge, straps and fuel line 2. Equipment is **467.6 lb** and payload **357.4 lb**: about **+7.4 lb of margin** with driver + 150 lb of gear. Weights are estimates.

**Trim and speed with driver + 150 lb of gear:** moving the driver and the tank forward fixes the solo dock trim, now about **0.94°**, inside the 0.5–2° target. It also means gear must not go in the bow:

| Gear stowed at | Dock trim | Top speed |
|---|---|---|
| X 120 (bow) | −0.3° (bow down) | 33.25 mph |
| X 100 | 0.12° | 33.75 mph |
| **X 60 (between the benches, design case)** | **1.03°** | **34.5 mph** |
| X 40 | 1.53° | 35 mph |

**Check on the real hull:**
1. What is inside the middle bench, and whether the forward panel can take the opening.
2. Front deck strength under 45 lb of fuel, and how to fasten the straps.
3. Rib height where the stick base and console foot land.

**Not covered:** there is no seat back, so hard acceleration pushes the driver aft against nothing but their own legs. The stick and pedal loads, the cable sizes and the electrical design are still open. A long battery cable run needs checking for voltage drop at start.

## 9d. Steering and throttle controls (2026-11-03)

Onshape: part studio "Minimal controls" (master `CAD/FeatureScript/Minimal_controls.fs`), in the tab Main assembly. Check: `CAD/analysis/controls_800r.js`. Render: `renderedImages/Minimal_controls_v1.0.0.4.png`. This replaces the cable routes in Section 9c.

**Steering:**
- **Direction:** stick forward turns left, stick back turns right. This is the usual stick-steering habit reported for other stick-steered boats; I could not find an airboat source that confirms it. Swapping the cable to the other side of the rudder shaft reverses it.
- **Stick:** the cable attaches 8 in above the pivot. At 25° of rudder, the stick swings ±13° and the grip moves about ±5.9 in.
- **Cable:** about 14 ft, along the port side, then across the top of the rear bench to Y 18, under the hoops, to a clamp on the aft hoop.
- **Rudders:** hinged at 25% chord on 1 x 0.125 in shafts, with a lower bracket on the aft hoop and an upper 1 x 0.065 in arm out to the hoop.
- **Linkage:** the cable pulls a 4.3 in outboard arm on the port shaft. A forward horn on each shaft carries a tie bar at X 0, so both rudders move together. Rudder stops are still to be set.

**Throttle:**
- **Pedal:** a crank on the hinge rod stands up between the driver's feet. Pressing the pedal swings it forward and pulls the cable about 1.16 in for 20° of pedal travel.
- **Cable:** about 16 ft. It stops on a post on the rib at X 108, passes ahead of the toes, and runs along the starboard side to a bracket at the engine, so it stays out of the passenger's footwell.
- **Return:** the pedal has its own return spring, in addition to the spring at the throttle body. Confirm the real throttle travel from the engine.

**Load check at full power (42 lbf on each rudder at the stall):**

| Item | Result |
|---|---|
| Prop wake at the rudders | about 90 mph (133 ft/s) |
| Hinge moment, both rudders | 100 in-lb |
| Cable load | 23 lbf |
| Force at the grip, including 5 lbf of friction | about 12 lbf |
| Shaft bending, 1 x 0.125 in | 5,190 psi, factor 1.3 on 7,000 psi |

A 3/4 x 0.065 in shaft failed this check, at a factor of 0.4. The friction figure is a round estimate.

**Clearances (hand figures):** tie bar to aft hoop about 0.13 in at center, opening as the rudders swing; rudder cable under the aft hoop about 0.5 in. Check the tie bar at full rudder on the real parts, because it is tight.

**Weight:** no change to the model: itemized, the rudders and linkage come to 9.4 lb against the 12 lb allowance, and the stick, pedal and cables to 6.1 lb against 10 lb. The margin stays at about **+7.4 lb**. I kept the allowances until real parts are weighed; if the items hold, about 6 lb more could be freed.
