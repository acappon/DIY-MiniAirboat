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
- **Exhaust:** keep the stock tuned pipe and silencer; the pipe's length sets the power, so don't shorten it. Route the pipe in CAD early: it's the bulkiest part of the engine. Keep it away from the fuel line, the belt and the driver.
- **Oil:** E-TEC engines inject oil from the sled's oil tank with an electric oil pump. Keep the oil tank, mount it upright, and run the engine on straight gasoline.
- **Fuel:** use the sled's in-tank fuel pump module (or one of the same pressure) in the boat's tank. Estimated burn about **10.8 gal/h at full power** and **5.4 gal/h at a 60 hp cruise**: a 6 gal tank lasts about 33 minutes flat out, or about 1.1 hours cruising. Measure it on the static test.
- **Electrical:** the ECU, harness and DESS key from the sled; the lanyard kill switch wired into the ECU's stop circuit; the small lithium battery for starting and the lights.
- **Throttle:** the sled's throttle cable to a hand lever or foot pedal.
- **Noise:** US snowmobiles are built to meet about 78 dB(A) at 50 ft at full throttle with the stock silencer (to confirm for the model year). With the prop (about 81 dBA at 2,590 rpm), expect about 82–84 dBA at 50 ft, about the same as the bike engine. California's 75 dB(A) shoreline limit is still out of reach.
- **Engine life:** budget a top-end rebuild (pistons, rings) at the interval in the service manual; a top-end kit is about $1,300.

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

**Outfit trim (decided 2026-11-03):** UHMW strips over the wear zones only (the keel and chine areas from the bow rise aft), about 45 → 35 lb. Equipment drops to about 423 lb and payload to about **402 lb**, which meets both load cases. If the stand comes out over 45 lb, or the engine weighs more than 79 lb, take the difference out of the outfit too.

Speeds, from the model: about **35 mph** with driver + 150 lb of gear stowed amidships (34 mph with it in the bow) and **34.75 mph** with driver + passenger. Both get on plane easily, with more than 250 lbf of thrust to spare at the hump. Dock trim is 0.5–2°.

## 9. Decisions and checks before the CAD

1. **PTO taper and side load:** from the service manual. Decide whether the small pulley needs an outboard bearing.
2. **Belt width** from Gates DesignFlex.
3. **Prop maximum rpm** from Warp Drive.
4. **Radiator** size and position in the prop wash.
5. **Tuned pipe routing** on the stand.
6. **Weigh the engine** with its pipe when it's bought, and update `minimal_1448.js`.
