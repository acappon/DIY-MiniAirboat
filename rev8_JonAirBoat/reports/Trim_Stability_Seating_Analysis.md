# Trim, Stability and Seating — Design Analysis

**Date:** 2026-11-02. **Status:** first-pass analysis for design decisions. Every weight and many positions are estimates (Section 2), so treat the numbers as planning values. Calibrate them with the float test in Section 9 before final dimensions are set.
**Applies to:** the airboat with a Toyota 2GR-FE 3.5 L V6 on an 1860-class hull (branch rev8/1860-hull). **Current design basis: Section 13.**
**Model:** `CAD/analysis/trim_stability.js` (results in `CAD/analysis/results.json`). It covers hydrostatics of the Lowe 1448 reference hull, Savitsky (1964) planing equilibrium with the airboat's high thrust line, the Savitsky–Brown flap formula for the trim tab, and basic flotation.

Coordinates: X forward from the transom, Y + to port, Z up from the hull bottom (inches).

> **Note (2026-11-03):** the design is now one airboat with a Toyota 2GR V6 on an 1860 hull, and **Section 13 is the current design basis**. Sections 1–11 (`trim_stability.js`: 1448 hull, the earlier Suzuki 1.3 L engines as "Prototype A/B") and Section 12 (hull comparison with those engines) are kept as history: they record why the hull and engine changed. Their method still applies; their numbers do not. Redo Sections 2–9 for the 1860 and 2GR once the drivetrain is sized.

---

## 1. Summary

| Question | Answer |
|---|---|
| Solo driver at the dock | Sits **about 1.1° stern-down**, with 7.3 in of draft at the transom and **7.7 in of freeboard to the 15 in transom top**. That is the "slightly low at the stern" you asked for. |
| Solo driver at top speed | Runs at **about 2.7° bow-up trim**, with about 94 in of wetted length (the flat bottom ends at 101 in). Estimated top speed: about **27 mph (A)** and about **32 mph (B)**. Treat these as rough, ±25%, until thrust is measured in the static test. |
| Is solo trim "close to ideal"? | **Close, but a little flat.** The lowest-drag trim for a flat bottom is about 3.5–4.5°. **The airboat's thrust line, 53 in above the bottom and about 32 in above the CG, pushes the bow down at speed, and that effect is far larger than anything weight placement can do.** Moving the fuel tank 90 in aft and the seat 3 in aft only changes running trim by about 0.1°. So the specified layout is about as good as this hull and thrust line allow, and the trim at the dock is right. |
| Trim with one passenger | No adjustment needed. Running trim rises to about 3.6° (better for drag) and top speed drops about 10%. Sit the passenger centered or slightly to port (Section 6). |
| Trim with two passengers | **Prototype A is marginal.** Top speed is about 17 mph at 5.6° trim, and the wetted length runs onto the bow rise, so the boat is barely on plane. Set the driver seat fully forward and expect a slow hole shot. **Prototype B handles two passengers** at about 28 mph and 3.1°. Proposed rule: A carries driver + 1, and B carries driver + 2. |
| Trim tab (starboard) | Its job is **roll**: cancelling prop torque. It cannot raise the bow; a transom tab always pushes the bow down. The 8 x 5 in tab needs about 3° at 35 mph (A) but 9° at 25 mph (B). **Recommend 10 x 6 in, 0–10° on the turnbuckle** (Section 6). |
| Swamped flotation | The boat needs about **880 lb of buoyancy, or 14.5 ft³ of closed-cell foam**, centered near **X 39 in** so it floats level when swamped. A layout that provides about 12 ft³ plus the existing bench foam is in Section 7. |
| Middle-bench removal | **Yes, add a cross member.** Make the storage box frame a structural thwart bolted side to side (Section 8). The removed bench also took away its built-in foam, which Section 7 replaces. |
| **Capacity plate (new issue)** | The Lowe 1448 plate allows about **825 lb** of persons, motor and gear, and **25 hp maximum**. The airboat equipment alone is about **890 lb before any crew**. With a solo driver it is about 1,090 lb (132%), and with three people about 1,490 lb (180%). The hydrostatics still show good freeboard, but this is a real deviation that needs a decision (Section 10). |

---

## 2. Weight and balance (Prototype A, solo driver, full fuel)

| Group | Weight (lb) | Main items and where they sit |
|---|---|---|
| Hull (middle bench removed) | 248 | Lowe 1448 listed at about 260 lb, minus about 12 lb for the bench; CG X 62 |
| Hull armor | 90 | UHMW strips 55 lb (nearly neutral when swamped), chine angles, bow skin, fasteners, trim tab |
| Drivetrain mounts | 65 | Beds, doublers, plates (X 32), cage supports (X 8) |
| Drivetrain (A) | 420 | Engine 185 lb (X 38, Z 18), steel A-frame 100 lb, redrive 55 lb, radiator 25 lb at Z 66, prop 20 lb at Z 53 |
| Cage and rudders | 110 | Steel EMT cage 85 lb (X 5, Z 52), rudders and linkage |
| Controls | 45 | Stick and pushrods (port), table and dash 30 lb (X 108, starboard) |
| Electrical | 53 | Group 31 LiFePO4 battery about 28 lb under the driver seat (X 100, Z 6), power hub, lights. (A 70 lb AGM until 2026-11-02; the lighter battery adds about 0.2° of bow-up static trim and takes about 0.07 in off the transom freeboard. Running trim is almost unchanged.) |
| Seating | 80 | Driver bucket seat on tracks, storage box and padded lid, backrest/grab bar |
| Gear in storage box | 25 | PFDs, anchor, extinguisher, lines |
| Fuel, 6.5 gal full | 47 | Front floor, X 135 |
| Driver | 200 | Seated CG X 106, Z 24 |
| **Total** | **1,425** | **LCG X 57.8 in, VCG Z 20.6 in, TCG 0.16 in to starboard** |

Prototype B adds about 17 lb: the G13B head is about +10 lb and the 4-blade prop about +7 lb. Passengers are 200 lb each, seated on the box at X 66.

---

## 3. At the dock (static float, fresh water)

| Case | Weight (lb) | LCG (in) | Static trim | Draft at transom | Freeboard to transom top | GM (in) |
|---|---|---|---|---|---|---|
| A, solo, full fuel | 1,425 | 57.8 | 1.06° stern-down | 7.3 in | 7.7 in | 29 |
| A, solo, 1/4 fuel | 1,395 | 56.1 | 1.35° | 7.6 in | 7.4 in | 29 |
| A, driver + 1 | 1,625 | 58.8 | 1.11° | 8.1 in | 6.9 in | 25 |
| A, driver + 2 | 1,825 | 59.6 | 1.19° | 8.9 in | 6.1 in | 22 |
| B, solo | 1,442 | 57.4 | 1.16° | 7.5 in | 7.5 in | 28 |
| B, driver + 2 | 1,842 | 59.2 | 1.29° | 9.1 in | 5.9 in | 22 |

- The boat sits **1.1–1.3° stern-down at the dock in every case**, as wanted. Because the passengers sit close to the LCG, they hardly change the static trim; they mostly add draft.
- Transom freeboard stays **at least 5.9 in**, which meets proposed requirement R3.4 (at least 4 in). Cutting the throttle suddenly from speed is still the stern-swamping risk to test (R3.4). A 1448T hull with a 20 in transom would add 5 in of margin.
- **Roll stiffness is high**: GM is 22–29 in, typical of a wide, flat jonboat. A heeling moment of 1,000 in-lb (a 200 lb person 5 in off center) heels the boat about 1.4°.
- **Both crew moved to one gunwale** (R3.3): about 11,000 in-lb heels the boat roughly 15° by the small-angle estimate, leaving about 3–4 in of side freeboard. That is right at the proposed R3.3 limit, so measure it on the water.

## 4. Planing trim (Savitsky, with the high thrust line)

| Case | Top speed (est.) | Trim at top speed | Wetted length | Trim at 25 mph |
|---|---|---|---|---|
| A, solo | 27 mph | 2.7° | 94 in | 3.0° |
| A, driver + 1 | 24 mph | 3.6° | 97 in | 3.4° |
| A, driver + 2 | 17 mph | 5.7° | 108 in (on the bow rise) | 3.7° |
| B, solo | 32 mph | 2.1° | 94 in | 3.1° |
| B, driver + 1 | 30.5 mph | 2.5° | 94 in | 3.4° |
| B, driver + 2 | 28.5 mph | 3.1° | 95 in | 3.8° |

**What sets the running trim.** Thrust acts at the prop shaft, Z 53 in, about 32 in above the CG. At top speed, thrust of about 230 lb on a 2.7 ft lever is about **620 ft-lb pushing the bow down**. The hull balances this by moving its center of lift forward of the CG. That keeps trim low (2–3°) and the wetted length long (close to the whole 101 in flat bottom).

**LCG sensitivity** (solo, 35 mph): moving the LCG from 58 to 30 in only raises trim from 1.8° to 2.5°. Realistic weight moves within the boat change it much less:

| Lever (solo, A) | LCG | Trim at 30 mph | Resistance at 30 mph | Top speed |
|---|---|---|---|---|
| As specified (fuel at the front, seat mid-track) | 57.8 | 2.27° | 256 lb | 26.8 mph |
| Seat 3 in aft | 57.3 | 2.28° | 255 lb | 26.8 mph |
| Fuel tank beside the engine (X 45) instead of at the front | 54.8 | 2.35° | 250 lb | 27.0 mph |
| Both | 54.4 | 2.36° | 249 lb | 27.0 mph |

**Conclusion:** keep the fuel tank up front as you planned. Moving it aft would only cut drag about 2% and add about 0.25 mph, and it would put the fuel on the floor beside a hot engine. The **dock trim is what the weight layout controls**, and that is already where you want it. Things that would genuinely raise running trim, such as a lower prop axis or tilting the thrust line down a few degrees, are drivetrain changes. They are worth noting for Prototype B's mount design.

## 5. Trimming for passengers

| Load | What to do | Expected behavior |
|---|---|---|
| Solo | Driver seat mid-track. Trim tab set for roll (Section 6). | Dock 1.1° stern-down; top speed trim about 2.7°. |
| Driver + 1 | Passenger on the box, centered or about 6–9 in to port. No seat change needed. | Running trim rises to about 3.6°, closer to the lowest-drag trim. About 10% slower. |
| Driver + 2, Prototype B | Passengers side by side on the box. Seat mid-track. | Trim about 3.1° at about 28 mph. Fine. |
| Driver + 2, Prototype A | Driver seat **fully forward** and fuel full. Expect a long hole shot. Avoid in shallow water until tested. | Barely planing: about 17 mph at 5.6°. Seat forward only changes trim by about 0.1°, so the real fix is more thrust. **Proposed: A is limited to driver + 1.** |

The seat track mainly sets driver comfort and reach. Its trim effect is small, about 0.1° over its full travel, which is good news: drivers can set it for comfort without upsetting the boat.

## 6. Roll balance and the trim tab

**Prop torque.** The G13 turns counterclockwise as seen from its front, and it faces aft, so the prop turns clockwise looking aft. The reaction torque on the boat is counterclockwise looking aft, which **rolls the boat starboard side down**. At full power:

| | Power | Prop RPM (est.) | Reaction torque | Static-equivalent heel without correction |
|---|---|---|---|---|
| A | about 70 hp | 2,400 | about 153 ft-lb (1,840 in-lb) | about 2.6° to starboard |
| B | about 100 hp | 2,300 | about 228 ft-lb (2,740 in-lb) | about 3.9° to starboard |

**Starboard trim tab.** A tab on the starboard side lifts the starboard stern, which is the right side to fix this. Lift from the Savitsky–Brown flap formula, acting about 19 in off the centerline:

| Tab size | Lift per degree at 35 mph | Deflection to cancel the torque: A at 35 / 25 mph | B at 35 / 25 mph |
|---|---|---|---|
| 8 x 5 in (as sketched) | 32 lb/° | 3.0° / 5.8° | 4.4° / 8.7° |
| **10 x 6 in (recommended)** | 48 lb/° | 2.0° / 3.9° | 2.9° / 5.8° |

- Tab lift falls with speed squared, so one fixed turnbuckle setting is a compromise. Set it for normal cruise. **Start at about 4° (A) or 6° (B)** with the 10 x 6 tab, then fine-tune with the inclinometer on the water.
- The tab also pushes the bow down slightly: about −0.1° per 2° of tab. That is acceptable, but it is another reason not to oversize it.
- **Static list at the dock:** the starboard table and dash give a 0.16 in starboard TCG, about 0.3° of list. Offset the battery about 7–8 in to port inside the dry box to zero it (the lithium battery is lighter than the AGM, which only needed about 3 in). That also helps a little against torque under power.
- **One passenger:** sitting about 9 in to port cancels Prototype A's torque at full power. That puts about 2° of port list at rest, so treat it as an option, not a rule.

## 7. Swamped flotation (requirement: must not sink)

The analysis assumes the hull is completely swamped, the crew are in the water wearing PFDs, and all equipment is aboard. Each item keeps only its weight-in-water fraction: aluminum 0.63, steel 0.87, the engine about 0.80 and the lithium battery about 0.4 (the AGM was 0.57). UHMW and the fuel tank are slightly buoyant.

| | Prototype B (the heavier) |
|---|---|
| Weight in water to support | **732 lb**, center at **X 39 in** |
| With a 20% margin | **878 lb buoyancy** |
| 2 lb/ft³ closed-cell foam needed | **14.5 ft³** |

The foam must sit **centered around X 35–40** so the swamped boat floats level instead of stern-down. That is why your stern blocks beside the prop cage are the right idea.

| Proposed foam location | Size (approx.) | Volume | Center X |
|---|---|---|---|
| Removable blocks, each side of the cage and engine, strapped in (X 2–34) | 32 x 13 x 12 in each, notched around the cage-support foot and knee | 2 x 2.9 = 5.8 ft³ | 18 |
| Fixed blocks beside the engine, outboard of the beds (X 34–54) | 20 x 12 x 11 in each | 2 x 1.5 = 3.0 ft³ | 44 |
| Under a false floor in the storage box (replaces the middle bench's foam) | about 60 x 18 x 5 in | 3.1 ft³ | 64 |
| **Subtotal (new)** | | **11.9 ft³** | **36** |
| Existing foam in the rear and front benches | **measure on the boat** | about 3 ft³ or more needed | |

- **Use fuel-resistant closed-cell foam**, such as closed-cell polyethylene, or encapsulated dock-float billets, which are cheap, tough and DIY-friendly. Plain polystyrene dissolves in gasoline.
- Keep foam **at least 6 in from the exhaust** on the port side, or fit a heat shield.
- Strap the removable blocks to welded or bolted tie-downs so they cannot float out when swamped. A loose block gives no flotation.
- Verify with a **swamp test** in shallow water before open-water use (new requirement R5.7).

## 8. Middle bench removal: cross member

The middle bench (X about 78–86) was a thwart that tied the two hull sides together, and its foam was part of the factory flotation. Recommendation:

- **Make the storage box frame the new thwart.** Use a 1-1/2 x 1-1/2 x 1/8 in aluminum angle frame across the full inside width at X 55–73. Bolt the top rails to the gunwale and through the side ribs with #10 or 1/4-20 bolts and sealant, like the rest of the hull work. That puts the thwart only about 15 in aft of where the bench was.
- **Bolt the backrest/grab-bar uprights to this frame** at the box's aft corners (X about 54), with the angled stays running down beside the engine to the bed flanges. The thwart, bar and stays then form one stiff hoop that also serves as the grab rail.
- The dash table can tie into the starboard gunwale for local stiffness. No second thwart is needed forward: the span from the box to the front bench is similar to the original spacing between benches.

## 9. Seating and walk-through layout (for the CAD)

**Modeled in CAD 2026-11-02** (Onshape "Seating and interior mounts" and "Console" Part Studios); the CAD values replace the proposals below where they differ: storage box X 63-79 in (clear of the engine-bed ends at X 62), cushion top Z 18 in; grab bar uprights at X 58 in; driver hip point X 103, Z 17 in; dry box X 97-112 in; console table X 101-122 in at Z 25 in. `analysis/trim_stability.js` now uses the CAD weights and positions: static trim with one passenger 1.17° (was 1.29°), list about zero.

| Item | Proposed position | Notes |
|---|---|---|
| Storage box / passenger seat | X 55–73, full width, padded top at about Z 19 | Lid hinges at the back. Box back at X 57, so the lid opens 90° clear of the bar at X 54. |
| Backrest/grab bar | Uprights at X 54 at the box corners; crossbar about 15 in above the seat (Z about 34) | Stays angle down and aft beside the engine to the bed flanges. Pad the crossbar. |
| Driver bucket seat | Seat back about X 90, H-point about X 100, cushion about Z 16 on the battery dry box | Tracks with ±3 in travel. Keep the passengers' knee room (about 20 in) aft of the seat back. |
| Battery dry box | Under the seat, X 92–108, battery offset about 7–8 in to port | Group 31 LiFePO4 is about 13 x 6.8 x 9 in, the same footprint as the AGM; box about 12 in tall. |
| Control stick | Port side of the seat, base at about Y +11 | Leaves a **walkway of about 13–14 in clear on the port side**. The grip can angle inboard to widen it. |
| Table and dash | Starboard of the driver, X 100–118, table at armrest height (about Z 25) | Fills to the starboard side: no passage there. Fiddle edge; front grip edge about 6 in long, then the dash turns up. |
| Fuel tank, 6.5 gal | Front floor or front bench, about X 130–140 | Strap it down. Fill and vent outside the cockpit. |
| Inclinometer | Two-axis bubble on the dash top or table, in the driver's view | Use it to set the trim tab and to check the dock trim. |
| **Grab points along the walkway** | Backrest bar (X 54) → driver seat back top (X about 90) → **add a port gunwale grab rail, X about 85–135** | Without the gunwale rail there is nothing to hold forward of the driver seat on the port side. |

## 10. Issues and decisions needed

1. **Capacity plate exceeded.** The Lowe 1448 plate allows about 825 lb of persons, motor and gear, and 25 hp maximum. This build carries about 890 lb of equipment before any crew. Options:
   - **(a)** Accept it, and replace R1.2 with performance-based limits: freeboard (R3.4), flotation (R5.7) and a maximum crew number (R1.1).
   - **(b)** Use a bigger hull for Prototype B, such as a 1548 or 1648, or a 1448T with its 20 in transom.
   - **(c)** Cut weight. The steel A-frame (100 lb) and the steel EMT cage (85 lb) are the biggest candidates.

   Also check California registration rules for a modified hull.
2. **Prototype A with three aboard** is marginal (above). Proposed: A is rated for driver + 1, and B for driver + 2.
3. **Trim tab size:** 10 x 6 in recommended over 8 x 5 in, so Prototype B stays within about 6° at cruise.
4. **Existing bench foam:** measure it, because it decides how much new foam to add.
5. **Estimates to replace with measurements:**
   - Hull weight
   - Engine dressed weight
   - Steel A-frame and cage weights
   - Static thrust (static test 3.1)
   - Prop RPM and power

## 11. Verification (feeds the requirements)

- **Float test at the dock** (before the drivetrain goes in, then again after). Measure draft at the transom and at the bow-rise start, and read the inclinometer. Compare with Section 3; a difference of more than 1 in or 0.5° means the weight estimates need correcting.
- **Static test (3.1):** measured thrust replaces the 330 / 440 lbf estimates and resets the top-speed predictions.
- **On-water tests:**
  - running trim (inclinometer, photos) per load case
  - top speed (GPS)
  - list at power with and without the tab
  - both-crew-to-one-side freeboard
  - swamp test

## 12. Hull size comparison: 1448 vs 1652 vs 1860 (added 2026-11-02)

**Model:** `CAD/analysis/compare_hulls.js`, with results in `CAD/analysis/compare_results.json`. It uses the same method as above, applied to hulls scaled from dealer specs:

| Hull | Bottom / beam | Plate (persons + motor + gear) | Hull weight | Transom | Flat bottom (assumed) |
|---|---|---|---|---|---|
| Lowe 1448 | 48.8" / 68" | 825 lb | 248 lb | 15" | to 101" |
| Lowe L1652MT | 52" / 75" | 1,216 lb | 438 lb | 20" | to 121" |
| 1860 class | 60" / 84" | 1,550 lb | 640 lb | 20" | to 140" |

Equipment counted against the plate is everything except the bare hull, including the armor, which scales with bottom area:

| | 1448 | 1652 | 1860 |
|---|---|---|---|
| Baseline equipment (steel A-frame and cage, AGM battery) | ~977 lb | ~1,002 lb | ~1,040 lb |
| **Lightened** equipment (aluminum A-frame and cage, lithium Group 31, which saves about 120 lb) | ~857 lb | ~882 lb | ~920 lb |

**Design load: solo + 200 lb of gear, or driver + 1 passenger (they give nearly identical results).**

| Hull | Equipment | Plate use | Proto | Top speed (est.) | Trim at top speed | Thrust margin at the hump (12–16 mph) | Transom freeboard at the dock |
|---|---|---|---|---|---|---|---|
| 1448 | lightened | 152% | A | 25.5 mph | 3.0° | 65 lb | 7.8" |
| **1652** | baseline | 115% | A | 20.3 mph | 4.4° | **18 lb (marginal)** | 11.5" |
| **1652** | **lightened** | **105%** | **A** | **22.8 mph** | **3.5°** | **42 lb** | **12.3"** |
| **1652** | lightened | 107% | **B** | **29.3 mph** | 2.5° | 127 lb | 12.2" |
| 1860 | baseline | 93% | A | 16.5 mph | 4.7° | **2 lb (may not plane)** | 12.0" |
| 1860 | lightened | 85% | A | 19.3 mph | 3.8° | 23 lb (marginal) | 12.8" |
| 1860 | lightened | 86% | B | 26.5 mph | 2.6° | 109 lb | 12.7" |

**Driver + 2 passengers (the occasional slow trip):**

| Hull | Equipment | Plate use | A | B |
|---|---|---|---|---|
| 1652 | lightened | 122% | barely planes, ~17 mph | **planes, ~27 mph** |
| 1860 | lightened | 98% | does not plane | planes, ~24.5 mph |

**Solo, no gear:**

| Hull | Equipment | Plate use | A | B |
|---|---|---|---|---|
| 1652 | lightened | 89% | ~25 mph | ~30 mph |
| 1860 | lightened | 72% | ~22 mph | ~27 mph |

**Swamped flotation needed** (foam, 1.2 x in-water weight):

| Hull | Foam needed | Centered at |
|---|---|---|
| 1448 | ~12–15 ft³ | about X 40 |
| 1652 | ~15–17 ft³ | about X 46–49 |
| 1860 | ~18–20 ft³ | about X 55–58 |

**What this means:**
1. **1652 + lightened equipment is the best match for the stated use** (mostly solo with gear, sometimes one passenger):
   - Prototype A planes at the design load at about 23 mph, with modest thrust to spare.
   - Prototype B planes at about 29 mph, and even planes with three aboard.
   - The 20" transom gives about 12" of stern freeboard at the dock, compared with about 7–8" on the 1448.
2. **1652 is still about 5% over its plate at the design load, even lightened.** Closing the gap needs about 65 lb more cut, for example lighter armor coverage on the bigger bottom, a lighter seat or a smaller box. Otherwise it is a small, deliberate overrun.
3. **The 1860 only makes sense with the G13B.** Its extra ~200 lb of hull uses up Prototype A's thrust: A at the design load has almost no margin over the hump. With the G13B it is the only option that keeps three people within the plate.
4. **Running trim stays at 2–4.5° on every hull.** The thrust-line effect from Section 4 carries over, and the bigger hulls run a little higher trim because they are heavier.
5. **Weight cuts matter more than hull size for Prototype A.** On the 1652, lightening raises the hump margin from 18 to 42 lb, which is the difference between a doubtful and a dependable hole shot.

**Caveats:**
- The bow rise and flat-bottom length of the 1652 and 1860 are scaled assumptions.
- Savitsky's method is least accurate near the hump, so treat the hump margins as relative rankings, not absolute values.
- Thrust is still estimated, so the static test (3.1) will move all speeds.

## 13. Design basis: Toyota 2GR V6 on an 1860 hull (2026-11-03)

**Why the design changed.** On the 1652, the 70–100 hp engines left only about 40–80 lb for the driver's gear, so every pound of cargo changed performance enough for users to notice. The 1860 hull roughly doubles the payload under the plate, and an engine with much more power for little more weight makes cargo almost irrelevant.

**Engine study** (1860 hull, same 64 in prop; weights include the redrive and radiator and are rough, ±15%):

| Engine | hp | lb | lb/hp | Payload under the plate | Top speed, solo + 100 lb gear | Top speed, driver + 2 |
|---|---|---|---|---|---|---|
| Subaru EJ25 | 165 | 375 | 2.3 | 442 lb | 32.5 mph | 31 mph |
| Honda K24 | 200 | 400 | 2.0 | 417 lb | 35 mph | 33.5 mph |
| GM Ecotec 2.0 turbo (LNF) | 260 | 455 | 1.75 | 362 lb | 38 mph | 37 mph |
| **Toyota 2GR-FE 3.5 V6** | **270** | **480** | **1.8** | **337 lb** | **38.5 mph** | **37 mph** |
| GM LS 5.3 aluminum | 300 | 570 | 1.9 | 247 lb | 39 mph | 38 mph |
| Suzuki Hayabusa 1300 | 175 | 240 | 1.4 | 577 lb | 33 mph | 32 mph |
| Lycoming O-360 (aircraft) | 180 | 285 | 1.6 | 529 lb | 33.5 mph | 32 mph |

**Decision: the Toyota 2GR-FE.** It is very common in junkyards (Avalon, Camry V6, RAV4 V6, Highlander, Sienna, Venza, Lexus ES/RX), cheap (typically a few hundred dollars at a self-service yard, about $850–1,700 used online), naturally aspirated and reliable. The K24 is the simpler, milder alternative; the Hayabusa has the best ratio but is costly and rare.

**2GR on the 1860** (`CAD/analysis/performance_1860.js`): equipment about **1,213 lb**, plate about 1,550 lb, so about **337 lb of payload** (a 200 lb driver plus about 137 lb of gear). Static thrust about **850 lbf** (scaled from the earlier prop by power^(2/3)); prop torque about **620 ft-lb**.

| Load (full fuel) | Weight | Dock trim (R3.6: 0.5–2°) | Transom freeboard | Top speed | Running trim (R3.7: 2–4.5°) | Thrust margin at the hump |
|---|---|---|---|---|---|---|
| Solo | 2,053 lb | 2.1° | 11.4 in | 38.5 mph | 1.5° | 421 lbf |
| Solo + 100 lb gear | 2,153 lb | 2.2° | 11.0 in | 38.5 mph | 1.6° | 404 lbf |
| Driver + 1 | 2,253 lb | 2.3° | 10.6 in | 37.75 mph | 1.7° | 385 lbf |
| Driver + 2 | 2,453 lb | 2.4° | 9.9 in | 37 mph | 1.9° | 347 lbf |

**What this means:**
1. **Cargo barely matters now.** 100 lb of gear costs nothing measurable in top speed, and the boat planes with any load with a large margin.
2. **The boat sits a little stern-heavy at the dock** (about 2.1–2.4°, just over the target): the 2GR is heavy and sits aft. Putting the fuel tank forward (planned) and gear forward bring it back.
3. **Running trim is too flat** (about 1.5°): about 850 lbf on the high thrust line pushes the bow down. A lower prop axis or a few degrees of down-tilt is a design driver for the 2GR drivetrain.
4. **Prop torque is about four times the earlier engine's,** so the trim tab, redrive, prop, shaft, A-frame, mounts and cage all need resizing (R4.9). That work is on hold until the 1860 branch settles.

**Caveats:** the 1860 hull shape is scaled from dealer specs, the 2GR weights are estimates, and thrust is not measured. The prop is still the 64 in prop of the earlier design; a prop sized for 270 hp will change the thrust and speeds. Treat speeds as ±25% until the static test.

## Sources and assumptions
- Lowe L1448M specifications (max load about 825 lb, max 25 hp, hull about 260 lb, 15 in transom; the 1448T has a 20 in transom): [iboats](https://boats.iboats.com/research/2023-lowe-boats-l1448m-jon/2335347.html), [boattrader](https://resources.boattrader.com/boat/2025-lowe-1448-1448t-jon-4320060). **Check the capacity plate on the actual boat.**
- Planing method: Savitsky, D. (1964), "Hydrodynamic Design of Planing Hulls", Marine Technology. Flap lift and drag: Savitsky & Brown (1976).
- Hull shape: the Onshape reference hull. The bow rise and bottom taper forward of X 101 are approximated, because the mesh is incomplete there. Fresh water.
- Thrust falls linearly from the static value to zero at about 85 mph. Air drag CdA is about 12 ft² solo, plus 3 ft² per passenger.
- Section 13: 3–4 lbf of static thrust per hp for airboat props ([Whirlwind newsletter, 2020](https://whirlwindpropellers.com/airboats/wp-content/uploads/2020/02/WWPC-1st-Qtr-2020-Newsletter.pdf)); used 2GR-FE prices ([eBay](https://www.ebay.com/p/654693864), [Auto Power Source](https://autopowersource.com/collections/used-toyota-engines-for-sale?page=1)); 1860 hull specs and prices ([Tracker Grizzly 1860, Bass Pro](https://www.bassproboatingcenters.com/boats-for-sale/boatmodel/2026+tracker+grizzly+1860+utility-9961491.html), [DuraCraft 1860 listing](https://ca.boats.com/power-boats/1990-duracraft-1860-dc-10050333)); Hayabusa airboat use ([Southern Airboat forum](https://www.southernairboat.com/community/threads/suzuki-hayabusa-1340cc-197-hp.72545/)).
