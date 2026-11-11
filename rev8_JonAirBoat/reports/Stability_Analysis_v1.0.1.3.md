# Stability Analysis — v1.0.1.3

**Boat:** minimal 1448, Rotax 800R E-TEC, seat module (v1.0.1.3), Onshape version v1.0.1.3
**Model:** `CAD/analysis/stability_upright.js`
**Status:** a hobby design calculation from estimated weights and an analytic hull shape. It has not been checked by a licensed professional, and nothing here has been tested on the water. Use it at your own risk.

**What changed since v1.0.1.2:**
- **Seating:** the middle bench and grab rail are replaced by the seat module (carbon struts, electrical dry box, storage compartments, sandwich seat, folding seatback). The driver's hip point moves to X 83.5, Z 14.
- **Oarlocks:** 4 in risers on the rails.
- **Rudders:** honeycomb rudder blades.
- **Design load:** now a 200 lb driver and **135 lb** of gear (was 150 lb).

## Result

The boat is very stiff at rest in every load case and passes every steady check, as in v1.0.1.2. The changes since then move the numbers by tenths. The advice is the same: **tie gear down.** Loose gear sliding to one side lists the boat 3.9° and takes the worst steady heel to about 13°, with about 6.7° left before water comes aboard.

| | 1. Driver only | 2. Driver + 135 lb tied down | 3a. Driver + 135 lb loose |
|---|---|---|---|
| Weight, lb | 938 | 1,073 | 1,073 |
| CG forward of the transom, in | 56.5 | 56.9 | 58.5 |
| CG above the bottom, in | 19.9 | 18.7 | 18.5 |
| Draft at the dock, in | 4.9 | 5.6 | 5.3 |
| Dock trim, stern down | 0.60° | 0.70° | 0.45° |
| Transom freeboard, in | 10.1 | 9.5 | 9.7 |
| GM, in | 43.6 | 39.8 | 40.3 |
| Water comes aboard at | 21.5° | 19.5° | 20° |
| Top speed, mph | 34.75 | 34.25 | 34 |
| Thrust margin at the hump, lbf | 278 | 259 | 260 |

**Against the plate:** load case 2 is the design case and sits on the 825 lb plate. Equipment is 488.1 lb (fuel included; the oars and dock lines count as gear), plus 335 lb of driver and gear, is 823.1 lb. The weight model gives 336.9 lb of payload, 1.9 lb over the design load. Weigh the boat at the end of the build.

**Dock trim:** about 0.2° less stern-down than v1.0.1.2. The seat module and driver sit a little further forward than the old bench.

## Loose gear

Case 3a starts with 135 lb in six bundles around the boat, then checks what happens if they slide.

| Loose gear | List | Trim | Transom freeboard | Water aboard at | Worst steady heel | Top speed |
|---|---|---|---|---|---|---|
| 3a. As scattered | 0° | 0.45° | 9.7 in | 20° | 9.5° | 34 mph |
| 3b. All slid to one side | **3.9°** | 0.45° | 9.7 in | 20° | **13.3°** | 34 mph |
| 3c. All slid forward | 0° | -0.42° | 10.8 in | 23° | 9.2° | 33 mph |
| 3d. All slid aft | 0° | 1.08° | 9.0 in | 18° | 9.6° | 34.75 mph |

- **To one side:** still the most serious case, because gear slides during a turn, when the boat is already leaning on it.
- **Forward:** the bow runs wetter, which costs about 1 mph, and the model no longer shows a clean plane.
- **Aft:** the lowest flooding angle, 18°.

## Heel curve

Righting arm in inches. A positive arm pushes the boat back upright. Port and starboard are within 0.3 in of each other except in 3b.

| Heel | 5° | 10° | 15° | 20° | 25° | 30° |
|---|---|---|---|---|---|---|
| 1. Driver only | 3.9 | 7.6 | 9.8 | 10.6 | 10.8 | 10.7 |
| 2. Tied down | 3.6 | 7.0 | 9.5 | 10.7 | 11.1 | 11.2 |
| 3b. Slid to one side, leaning that way | 0.8 | 4.3 | 6.9 | 8.1 | 8.7 | 9.0 |

The arms keep growing past the angle where water comes aboard, so flooding sets the limit, not the curve.

## Heeling forces

| Heel, degrees | 1. Driver only | 2. Tied down | 3b. Slid to one side |
|---|---|---|---|
| Prop torque at full power, 244 lb-ft | 3.9 | 3.8 | 7.7 |
| 30 mph beam wind | 1.8 | 1.7 | 5.7 |
| Prop torque, wind and driver leaning 15 in, all one way | 10.0 | 9.6 | 13.3 |
| 200 lb person stepping on the gunwale | 9.0 | 8.75 | 12.25 |
| Freeboard left at that step | 5.0 in | 4.5 in | 2.9 in |

Prop torque is the biggest steady force. The boat will sit about 4° over at full throttle at the dock.

## Pitch under thrust

Thrust at Z 52 pushes the bow down by 2.0 to 2.5° from dock trim at full throttle, or 3.1° with loose gear forward. The bow keeps 10 in of freeboard or more in every case.

## Running

| | 1. Driver only | 2. Tied down | 3b. Slid to one side |
|---|---|---|---|
| Sideways force to trip over a chine, g | 1.22 | 1.30 | 1.19 |

A flat hull skids on open water long before this, at a few tenths of a g. **The real risk is a chine catching a stump, a bank or a shallow edge while skidding sideways.**

## Rowing and oars

**Rowing:** with the engine off and the operator rowing facing aft on the folded seatback, the boat is at about case 1 trim with the rower's weight about 3 in further aft. The stability numbers above cover it.

**Shipped oars:** the oars are operator gear. They lie along the rails at Z 25.5, which raises the CG by under 0.1 in, too little to show in the table.

## Operating limits to adopt

1. Tie all gear down, low and on the centerline, behind the driver.
2. **Keep total load under the 825 lb plate.**
   - Equipment is 488.1 lb including a full 45 lb fuel tank.
   - The dash label reads **MAXIMUM LOAD 380 LB** for people, gear and fuel, and **335 lb** for people and gear with a full tank.
   - The oars and dock lines, when carried, count in that load.
   - Two 200 lb adults with a full tank are over the plate. That is for short trips with little else aboard.
3. Trim at rest with the round bubble level on the dash: move gear until the bubble is centered, then tie it down.
4. Don't step on the gunwale with gear piled on that side. Board over the centerline.
5. Keep the transom clear at the dock: full throttle and following waves are worst at 9 to 10 in of freeboard.
6. Avoid fast skids near banks or shallow edges.

## Limits of this analysis

- **Weights and CGs** are estimates until parts are weighed. The design load is met by 1.9 lb.
- **Hull shape** is the analytic model, close to the mesh but not a measured hull.
- **Bilge:** water on the floor would sit on the low side and reduce stability. Keep the bilge pump working.
- **Waves:** this is calm-water, static stability only.
- **Standards:** no formal stability standard was applied.

Capsized flotation is in `reports/Flotation_Inverted_v1.0.1.3.md`, structure in `reports/Structural_Analysis_v1.0.1.3.md`.
