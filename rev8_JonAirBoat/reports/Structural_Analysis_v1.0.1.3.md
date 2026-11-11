# Structural Analysis — v1.0.1.3

**Boat:** minimal 1448, Rotax 800R E-TEC, seat module (v1.0.1.3), Onshape version v1.0.1.3
**Model:** `CAD/analysis/structural_check.js`, with `stand_800r.js`, `bearing_plate.js` and `controls_800r.js`
**Status:** a hobby design calculation by hand and simple frame analysis. It has not been checked by a licensed professional, and nothing has been load tested. Use it at your own risk.

## Result

The primary structure still holds up: the stand, cage, drive line and hull girder are unchanged from v1.0.1.2 and keep their margins. The v1.0.1.3 changes add new load paths, and every one meets the 1.5 target:
- **Seat module:** replaces the middle bench and grab rail. The carbon struts are now the boat's thwart, and the seat base is a sandwich panel.
- **Oarlocks:** on 4 in risers.
- **Rudder blades:** honeycomb.

| New or changed part | Worst case | Factor | Note |
|---|---|---|---|
| Seat struts, 1 x 2 x 1/8 in carbon (the thwart) | Two people, 4 g hard landing | 2.1 | 6061 tube was 1.5. Sag 0.27 in at rest |
| Strut to clip, sleeved bolts in the carbon | Two people, 4 g | 10.9 | Every bolt through carbon has a crush sleeve |
| Strut clip to the hull side | Two people, 4 g | 3.5 | Bolt bearing in an assumed 0.080 in side |
| Seat base, 3/8 in fiberglass/foam sandwich | 200 lb on the front edge | 3.0 skins, 3.5 core | 3/8 in plywood was 2.5 |
| Seatback brackets | Driver leaning back, 150 lbf at 10 in | 1.5 | Bolts in the carbon 4.4 |
| Dry box on the struts, eight #10 bolts | 4 g | 79 | No aluminum rivets into carbon |
| Oarlock riser, 4 in | 100 lbf pull on the handle (about 150 lbf at the pin) | 31.6 | Bolts 5.0, rail bearing 4.3 |
| Rudder blades, honeycomb | Not separately checked | — | Same outline and shafts; the shaft check below governs |

**Removed:** the grab rail. Its worst result, 1.1 at the saddle, goes with it.

**Galvanic isolation** (carbon touching aluminum corrodes the aluminum) is a durability item, not a strength one:
- one ply of glass over each strut
- G10 shims at every aluminum contact
- crush sleeves in every bolt hole
- stainless bolts in Tef-Gel

It is in the Build Guide, step 2.17.

**Weight:** equipment is 488.1 lb and payload 336.9 lb, against the 335 lb design load (200 lb driver + 135 lb gear).

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
| Seat | Two 200 lb people at 4 g; 200 lb standing on the seat front edge; 150 lbf leaning on the seatback |
| Oars | 100 lbf pull on each handle |

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

### Seat module

| Part | Load case | Demand | Limit | Factor | Note |
|---|---|---|---|---|---|
| Strut 1 x 2 x 1/8 carbon, bending | Two people seated, at rest | 5,849 psi | 50,000 psi | 8.5 | Design allowable 50 ksi (80–100 ksi to break) |
| Strut, sag at mid span | Two people seated, at rest | 0.27 in | 0.5 in | 1.9 | Flexural modulus about 8 Msi |
| Strut, bending | Two people, 4 g hard landing | 23,394 psi | 50,000 psi | 2.1 | Simply supported; the end clips make it stiffer |
| Strut to end clip, two 1/4-20 in crush sleeves | Two people, 4 g | 200 lbf each | 2,188 lbf | 10.9 | Bearing in the two carbon walls, 35 ksi |
| End clip, two 1/4-20 through the hull side | Two people, 4 g | 200 lbf each | 950 lbf | 4.8 | |
| End clip, bolt bearing in the 0.080 hull side | Two people, 4 g | 200 lbf each | 690 lbf | 3.5 | Hull gauge assumed; fender washer inside |
| Seat base sandwich, skins | 200 lb on the front edge, 5 in past the strut | 10,025 psi | 30,000 psi | 3.0 | 0.020 in E-glass skins on a 5/16 in PVC core |
| Seat base sandwich, core shear | 200 lb on the front edge | 40 psi | 140 psi | 3.5 | The 2-1/2 in pad spreads a heel load |
| Seatback bracket 1/8 x 1-1/4 6061 | Driver leaning back, 150 lbf at 10 in | 23,040 psi | 35,000 psi | 1.5 | |
| Seatback bracket bolts in the carbon strut | Same | 500 lbf | 2,188 lbf | 4.4 | |
| Dry box on the struts, eight #10 bolts | 4 g hard landing | 5 lbf each | 413 lbf | 79 | Bearing in the 0.063 box flange |

### Oarlocks

A hard pull of 100 lbf on the handle puts 100 x (24 + 49) / 49 = 149 lbf on the pin, fore and aft, 6 in above the rail top.

| Part | Load case | Demand | Limit | Factor | Note |
|---|---|---|---|---|---|
| Riser 1-1/2 x 3 x 4 6061, pocketed to 1/8 in walls | Hard pull | 1,109 psi | 35,000 psi | 31.6 | |
| Two 5/16-18 through the rail and doubler, 2-1/4 in apart | Hard pull, tension in one bolt | 397 lbf | 2,000 lbf | 5.0 | 316 stainless |
| Rail bearing under the riser, 1/8 in doubler | Hard pull, prying on the rail lip | 397 lbf | 1,725 lbf | 4.3 | |

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

- **Seatback brackets:** 1.5, right on the target, for a driver leaning back hard. Use 3/16 in plate if the seatback is often used as a backrest by heavy people.
- **Rudder shaft and stand rail:** 1.3 and 1.4 on the 7 ksi running limit, and they pass. The rail's figure relies on engine mounts at X 17 and X 27, so recheck it when the real mount points are known.
- **Hull bottom panel:** 1.9 by assumption. A jon boat this light was never meant for 34 mph at this weight over rough water. Watch for oil-canning and cracked rivets or welds, and slow down in chop.

## Where weight could come off

None of these are applied. They're options if you need margin back. The rudder and controls allowances were already cut to 8.5 lb each in v1.0.1.3; the 5 lb brackets allowance remains.

| Option | Saves | Effect | Catch |
|---|---|---|---|
| Stand 1-1/2 in tube, 0.125 to 0.095 in wall | 4.0 lb | Stress +2%, stiffness -3% | Check the stock exists. Thinner wall reduces bolt bearing |
| Hollow prop shaft, 3/4 in bore | 2.0 lb | Stress +7%, factor stays about 4 | Gun drilling cost |
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
2. Check the rib attachment at X 37.75 and X 95.25, the hull sides where the seat struts clip on (X 76.5 and 85.5), and the rails where the oarlock risers bolt on (X 66).
3. Check the corner caps carry the aft cage legs.
4. Do the stand tap test, and the 300 lb rail proof pull.
