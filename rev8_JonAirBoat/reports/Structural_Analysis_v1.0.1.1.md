# Structural Analysis — v1.0.1.1

**Boat:** minimal 1448, Rotax 800R E-TEC, Onshape version v1.0.1.1-beta
**Model:** `CAD/analysis/structural_check.js`, with `stand_800r.js`, `bearing_plate.js`, `grab_rail.js` and `controls_800r.js`
**Status:** a hobby design calculation by hand and simple frame analysis. It has not been checked by a licensed professional, and nothing has been load tested. Use it at your own risk.

## Result

The primary structure holds up. The stand, cage, drive line and hull girder all have margin. The weight cuts didn't cause a failure, but looking at parts that had never been checked found **one real failure and two parts short of target**, all in the driver's controls. Those are fixed, and they're already in the CAD, BOM and Build Guide.

| Change | Was | Now | Factor to yield | Weight |
|---|---|---|---|---|
| **Steering stick** | 1-1/8 x 0.065 in, factor 0.6: would bend | 1-1/2 x 0.120 in | 1.9 | +0.7 lb |
| Stop on the stick | None: a hard pull reached 543 lbf in the rudder cable | Stop on the pivot cheeks | Cable no longer loaded | +0.1 lb |
| Throttle pedal | 1/8 in plate, factor 1.0 | 3/16 in plate | 2.3 | +0.1 lb |
| Console frame | 1 x 0.065 in, factor 1.2 | 1 x 0.095 in | 1.6 | +0.3 lb |
| Stand gusset joints | Three 5/16 in bolts each side | Four | 1.7 on the running limit | About nil |
| Cage mesh | Not specified | Front and rim only, none in the prop wake | Saves about 220 lbf of thrust | None |

The stick and pedal fit inside the weight allowance already carried, so only the console adds to the weight model. **Equipment is 471.9 lb, payload 353.1 lb, margin +3.1 lb.**

## How the checks were done

**Limits, 6061-T6:** 35 ksi yield, and 11 ksi yield near a weld. Loads that repeat every engine or prop turn are held to 7 ksi at bolt holes, for fatigue. The target is a factor of 1.5 to yield on limit loads, and 1.0 on the 7 ksi running limit.

**Load cases:**

| Case | Values |
|---|---|
| Full power | 497 lbf thrust, 244 lb-ft torque, 993 lbf belt pull |
| Hard landing | 4 g vertical at the boat's full weight |
| Trailer pothole | 3 g sideways |
| Crash stop | 4 g fore and aft on the cage |
| Driver | 167 lbf at the stick, 67 lbf sideways, 200 lbf on the pedal: typical light-aircraft pilot forces |
| Grab rail | 300 lbf, as you specified |

**Hull assumptions:** Lowe doesn't publish its gauges. I assumed 0.100 in bottom and 0.080 in sides in 5052, formed keels about 7 in apart, and 0.063 in bench panels. Measure these.

## Results

### Stand

| Part | Load case | Demand | Limit | Factor | Note |
|---|---|---|---|---|---|
| Engine rail and posts, worst member | Full power, running | 4910 psi | 7000 psi | 1.4 | Engine mount points assumed at X 17 and X 27 |
| Engine rail and posts, worst member | 4 g hard landing | 2340 psi | 35000 psi | 15.0 |  |
| Diagonal, 1-1/2 x 1-1/2 x 1/8, buckling | Full power | 918 lbf | 6823 lbf | 7.4 |  |
| Gusset joint, four 5/16 in bolts, bearing in 1/8 in wall | Full power | 12160 psi | 21000 psi | 1.7 | Four bolts each side, about 3,800 in-lb at the rail. Was three, factor 1.3 |
| Cross-straps, 1 x 1/8, side load in tension | 3 g sideways, trailer pothole | 5200 psi | 35000 psi | 6.7 | One strap of each X works, net of a 5/16 in hole |

### Drive line

| Part | Load case | Demand | Limit | Factor | Note |
|---|---|---|---|---|---|
| Prop shaft 1-1/2 in 4140, solid, combined stress | Full power, running | 13770 psi | 60000 psi | 4.4 | Goodman simplified: 30 ksi fatigue shear doubled |
| Forward bearing plate at the cutouts | Full power, running | 4200 psi | 7000 psi | 1.7 | Peak 14 ksi steady, 30% alternating |
| Forward bearing plate at the cutouts | Full power, peak to yield | 14000 psi | 35000 psi | 2.5 |  |
| Aft bearing plate, 3/8 in | Full power, peak to yield | 12400 psi | 35000 psi | 2.8 | bearing_plate.js |
| Prop speed against the maker limit | 7,900 engine rpm | 2687 rpm | 3200 rpm | 1.2 | Warp Drive HP manual |
| Power against the maker limit | Full power | 120 hp | 160 hp | 1.3 | Warp Drive HP manual |
| Forward bearing plate, gyroscopic load in a spin turn | 30 deg/s yaw at full rpm | 119 lbf | 580 lbf | 4.9 | Small against the 580 lbf belt share |

### Cage

| Part | Load case | Demand | Limit | Factor | Note |
|---|---|---|---|---|---|
| Hoop bending, 1-1/4 x 0.065, top load | 3 g sideways | 7010 psi | 35000 psi | 5.0 | Assumes the wall; BOM says thin-wall |
| Hoop bending, top load | 4 g hard landing | 9350 psi | 35000 psi | 3.7 |  |
| Hoop deflection against 2.6 in of tip clearance | 3 g sideways | 0.7 in | 2.6 in | 3.6 |  |
| Legs, bending, fore and aft | 4 g, fore and aft crash stop | 4220 psi | 35000 psi | 8.3 | Struts share the load |
| Struts 1 x 0.065, buckling | 3 g fore and aft | 140 lbf | 2500 lbf | 17.9 |  |
| Foot pad bolts, 4 x 1/4-20, shear | 4 g fore and aft | 15.3 lbf each | 950 lbf each | 62.3 |  |
| Mesh pull, aft face in the prop wake | Full power, if the aft face is meshed | 27.5 lbf per stringer | 950 lbf per stringer | 34.5 | Also costs about 220 lbf of thrust: mesh the front and rim only |
| Hoop, person leaning on top | 200 lb at the top, two hoops | 7920 psi | 35000 psi | 4.4 | Not a step: warn in the guide |

### Controls

| Part | Load case | Demand | Limit | Factor | Note |
|---|---|---|---|---|---|
| Rudder shaft 1 x 0.125 | Full power at the stall, running | 5190 psi | 7000 psi | 1.3 |  |
| Rudder shaft 1 x 0.125 | Hard stop, 2 times stall load | 10380 psi | 35000 psi | 3.4 |  |
| Steering stick 1-1/2 x 0.120 | 167 lbf at the grip | 18070 psi | 35000 psi | 1.9 | Was 1-1/8 x 0.065, factor 0.6 |
| Stick base, two 1/4-20 rivnuts per side | 67 lbf sideways | 218 lbf each | 500 lbf each | 2.3 | Rivnut pull-out in about 0.1 in rib |
| Rudder cable, with a stop on the stick | 167 lbf at the grip | 100 lbf | 500 lbf | 5.0 | Without the stop it would see 543 lbf: factor 0.9 |
| Throttle pedal 3/16 in plate | 200 lbf panic stomp | 15120 psi | 35000 psi | 2.3 | Was 1/8 in, factor 1.0 |
| Console arm 1 x 0.095 | 150 lb pull on the dash | 21460 psi | 35000 psi | 1.6 | Was 1 x 0.065, factor 1.2 |
| Console post, buckling | 150 lb on the dash | 150 lbf | 5701 lbf | 38.0 |  |

### Grab rail

| Part | Load case | Demand | Limit | Factor | Note |
|---|---|---|---|---|---|
| Stays, buckling | 300 lbf at the top | 339 lbf | 2373 lbf | 7.0 |  |
| Leg bend, one side | 300 lbf at the top | 2008 in-lb | 4343 in-lb | 2.2 |  |
| Leg at the saddle end | 300 lbf on one corner | 1200 in-lb | 1365 in-lb | 1.1 | To weld-zone yield; 2.5 to rupture |
| Cable 1/8 in 7x19 | 300 lbf sideways | 523 lbf | 1760 lbf | 3.4 |  |

### Hull

| Part | Load case | Demand | Limit | Factor | Note |
|---|---|---|---|---|---|
| Hull girder bending, sheer | Dock, full load | 630 psi | 23000 psi | 36.4 |  |
| Hull girder bending, sheer | 4 g hard landing | 2530 psi | 23000 psi | 9.1 |  |
| Bottom panel between formed keels, 7 in span | 5 psi slam at speed | 12250 psi | 23000 psi | 1.9 | Span and gauge assumed; UHMW sheet helps |
| Forward shoe on the rib, one side | Full power | 485 lbf | 2400 lbf | 4.9 | Rib crippling estimate, two walls 1.5 in tall |
| Forward shoe on the rib, one side | 4 g hard landing | 228 lbf | 2400 lbf | 10.5 |  |
| Bench clip, two 1/4-20 in panel | Full power | 128 lbf each | 950 lbf each | 7.4 |  |
| Bench clip into the panel, pull-through | Full power | 128 lbf each | 300 lbf each | 2.3 | Panel gauge unknown, 0.063 in assumed, with a backing plate |
| Shoe bolts, six 1/4-20, shear | Full power | 26 lbf each | 950 lbf each | 36.5 |  |
| Transom clip | Full power | 138 lbf | 900 lbf | 6.5 |  |
| Fuel tank straps, two | 4 g hard landing | 90 lbf each | 300 lbf each | 3.3 | Use rated cam straps |
| Battery strap in the dry box | 4 g hard landing | 18 lbf | 200 lbf | 11.1 |  |

The tightest results:

- **Rail at the saddle:** 1.1 to weld-zone yield, under the 1.5 target. It's the worst case, 300 lbf pulled on one corner. It has 2.5 to rupture, so a hard pull could leave it bent, not broken. I left it as is, because 1/8 in wall would add 1.2 lb for a rare load. Say if you want it stiffer.
- **Rudder shaft and stand rail:** 1.3 and 1.4 on the 7 ksi running limit, and they pass. The rail's figure relies on engine mounts at X 17 and X 27, so recheck it when the real mount points are known.
- **Hull bottom panel:** 1.9 by assumption. A jon boat this light was never meant for 34 mph at this weight over rough water. Watch for oil-canning and cracked rivets or welds, and slow down in chop.

## Where weight could come off

None of these are applied. They're options if you need margin back.

| Option | Saves | Effect | Catch |
|---|---|---|---|
| Stand 1-1/2 in tube, 0.125 to 0.095 in wall | 4.0 lb | Stress +2%, stiffness -3% | Check the stock exists. Thinner wall reduces bolt bearing |
| Hollow prop shaft, 3/4 in bore | 2.0 lb | Stress +7%, factor stays about 4 | Gun drilling cost |
| Rudder allowance | About 2.6 lb | None | Only once parts are weighed |
| Stick and pedal allowance | About 3 lb | None | Only once parts are weighed |
| Cage allowance | Perhaps 10 to 20 lb | None | Tube is about 15 lb of the 45 lb carried, so most depends on the mesh, which isn't chosen |

**Don't cut:** the 2 x 2 x 1/8 in stand tube. At 0.095 in wall it saves another 3.4 lb but loses 20% of the stiffness and moves the rail to about 6,170 psi, close to the limit.

The largest likely gain is the cage allowance. Weigh the real cage before cutting anything else.

## What this doesn't cover

- **Engine vibration:** a two-cylinder two-stroke with no damper. The stand's 18–24 Hz sway gives a prop resonance at 3,150–4,200 engine rpm, to pass through, not hold. Confirm with the tap test.
- **Fatigue of the joints:** the 7 ksi limit is a simple rule. Inspect the stand and gussets on a schedule.
- **Blade loss:** losing one blade makes about 8,600 lbf of out-of-balance force. No part of this design survives that, and the cage won't contain it. Inspect the blades before each run, keep bystanders out of the prop plane, and stop at once for any new vibration.
- **Bird or debris strike** on the prop, and collision with fixed objects.
- **Engine mounts and PTO:** needs the BRP service manual.
- **Forward bearing plate:** hand check and a coarse brick-element check. A proper FEA is still worthwhile before cutting.
- **Buckling of thin sheet:** bench and transom panels are unknown until inspected.

## Check on the real boat

1. Measure hull bottom, side and bench panel gauges.
2. Check the rib attachment at X 37.75, X 54.75, X 70.65 and X 95.25.
3. Check the corner caps carry the aft cage legs.
4. Do the stand tap test, and the 300 lb rail proof pull.
