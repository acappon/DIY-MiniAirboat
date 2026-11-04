# Stability Analysis — v1.0.1.0

**Boat:** minimal 1448, Rotax 800R E-TEC, Onshape version v1.0.1.0-structural
**Model:** `CAD/analysis/stability_upright.js`
**Status:** a hobby design calculation from estimated weights and an analytic hull shape. It has not been checked by a licensed professional, and nothing here has been tested on the water. Use it at your own risk.

## Result

The boat is very stiff at rest in all three load cases, and it passes every steady check. Nothing needed adding for stability. The one thing to change is how gear is carried: **tie it down.** Loose gear sliding to one side is the worst case studied. It lists the boat 4.4° on its own, and takes the worst steady heel from about 10° to about 14°, with about 5.7° left before water comes aboard.

| | 1. Driver only | 2. Driver + 150 lb tied down | 3a. Driver + 150 lb loose |
|---|---|---|---|
| Weight, lb | 916 | 1,066 | 1,066 |
| CG forward of the transom, in | 54.5 | 55.2 | 57.0 |
| CG above the bottom, in | 20.0 | 18.6 | 18.4 |
| Draft at the dock, in | 5.1 | 5.8 | 5.5 |
| Dock trim, stern down | 0.87° | 0.97° | 0.68° |
| Transom freeboard, in | 9.9 | 9.2 | 9.5 |
| GM, in | 44.0 | 39.7 | 40.3 |
| Water comes aboard at | 21° | 18.5° | 19.5° |
| Top speed, mph | 35 | 34.5 | 34.25 |
| Thrust margin at the hump, lbf | 280 | 259 | 260 |

Load case 2 is the heaviest design case, and it stays under the 825 lb plate: 468 lb of equipment plus 350 lb of driver and gear is 818 lb.

The boat has so much GM because a flat 48 in bottom resists heel strongly. CG height barely matters at this draft. Adding gear low even lowers the CG, so the boat gets a little stiffer but sits deeper.

## Loose gear

Case 3a starts with 150 lb in six bundles around the boat. Then it asks what happens if they slide.

| Loose gear | List | Trim | Transom freeboard | Water aboard at | Worst steady heel | Top speed |
|---|---|---|---|---|---|---|
| 3a. As scattered | 0° | 0.68° | 9.5 in | 19.5° | 9.6° | 34.25 mph |
| 3b. All slid to one side | **4.4°** | 0.68° | 9.5 in | 19.5° | **13.8°** | 34.25 mph |
| 3c. All slid forward | 0° | -0.31° | 10.7 in | 22.5° | 9.2° | 33.25 mph |
| 3d. All slid aft | 0° | 1.41° | 8.7 in | 17.5° | 9.7° | 35 mph |

- **To one side:** the most serious case, because gear tends to slide during a turn, which is when the boat is already leaning on it.
- **Forward:** the bow runs wetter, which cost 1 mph and takes the boat outside its normal planing shape. It's the one case where the model no longer shows a clean plane.
- **Aft:** the lowest flooding angle, 17.5°.

## Heel curve

Righting arm in inches, positive pushes the boat back upright. Port and starboard are within 0.2 in of each other.

| Heel | 5° | 10° | 15° | 20° | 25° | 30° |
|---|---|---|---|---|---|---|
| 1. Driver only | 3.9 | 7.6 | 9.8 | 10.6 | 10.8 | 10.6 |
| 2. Tied down | 3.5 | 7.0 | 9.5 | 10.6 | 11.1 | 11.2 |
| 3b. Slid to one side, leaning that way | 0.4 | 4.0 | 6.6 | 7.8 | 8.5 | 8.7 |

The arms keep growing past the angle where water comes aboard, so flooding sets the limit, not the curve. Each case has about 1,000 lb of hull behind a 10 in arm at that point.

## Heeling forces

| Heel, degrees | 1. Driver only | 2. Tied down | 3b. Slid to one side |
|---|---|---|---|
| Prop torque at full power, 244 lb-ft | 4.0 | 3.8 | 8.2 |
| 30 mph beam wind | 1.8 | 1.8 | 6.2 |
| Prop torque, wind and driver leaning 15 in, all one way | 10.2 | 9.7 | 13.8 |
| 200 lb person stepping on the gunwale | 9.0 | 8.75 | 12.75 |
| Freeboard left at that step | 4.8 in | 4.2 in | 2.5 in |

Prop torque is the biggest steady force. It's why the boat will sit about 4° over at full throttle at the dock, and you'll see that on the static test. Which way it leans depends on prop rotation.

## Pitch under thrust

Thrust at Z 52 acts high above the water and pushes the bow down, by about 2.1 to 2.3° from dock trim at full throttle, or 3.1° with loose gear forward. The bow keeps about 10 in of freeboard or more in every case, so it won't dig in at rest.

## Running

Hard turns are harder to analyze. The figures below are a simple comparison, not a prediction.

| | 1. Driver only | 2. Tied down | 3b. Slid to one side |
|---|---|---|---|
| Sideways force to trip over a chine, g | 1.22 | 1.31 | 1.18 |

Tripping needs the chine to catch something. A flat hull skids on open water long before that, at a few tenths of a g, so open-water turns should be safe. **The real risk is a chine catching a stump, a bank or a shallow edge while skidding sideways.** A sliding load reduces the margin at the worst moment, which is another reason to tie gear down.

## Operating limits to adopt

1. Tie all gear down, low and on the centerline, behind the driver.
2. Keep total load under the 825 lb plate. Equipment is 468 lb, so that leaves 357 lb for driver and gear.
3. Don't step on the gunwale with gear piled on that side. Board over the centerline.
4. Keep the transom clear at the dock: full throttle and following waves are worst at 8.7 to 9.9 in of freeboard.
5. Avoid fast skids near banks or shallow edges.

## Limits of this analysis

- **Weights and CGs** are estimates until parts are weighed.
- **Hull shape** is the analytic model. It's close to the mesh, but it isn't a measured hull.
- **Dry bilge:** water on the floor would sit on the low side and reduce stability. This wasn't modeled. Keep the bilge pump working.
- **Waves:** this is calm-water, static stability, without wave capsize or broaching.
- **Rules:** no formal stability standard was applied.

Capsized flotation is in `reports/Flotation_Inverted_v1.0.1.0.md`.
