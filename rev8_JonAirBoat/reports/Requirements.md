# DIY Jon-Air Airboat — Requirements (DRAFT for review)

**Status:** DRAFT, 2026-11-01; updated 2026-11-03 for one airboat with a Toyota 2GR-FE V6 on an 1860-class hull (branch rev8/1860-hull). Every value marked **(proposed)** is a starting point for the team to accept, change or delete. Values without that mark come from decisions already made, the CAD, or the static test plan.
**Applies to:** the airboat with a Toyota 2GR-FE 3.5 L V6 (about 270 hp) on an 1860-class hull. The prop, redrive, cage and engine mounts are not yet sized for the 2GR (on hold until the 1860 branch settles).
**Used by:** the static test (Build Guide 3.1) and the on-water test plan and protocol (Build Guide 4.1–4.3). Every requirement must trace to at least one test, inspection or analysis with a pass/fail criterion.

**Verification methods:** **T** = test (measured), **I** = inspection (look, measure a part, check a document), **A** = analysis (calculation or CAD), **D** = demonstration (do it once, pass/fail).
**Where verified:** **S** = static test (3.1), **W** = on-water test (4.2 / 4.3), **B** = during the build (Sections 1–2), **R** = design review.

---

## 1. Mission and crew

| ID | Requirement | Rationale / source | Method | Where |
|---|---|---|---|---|
| R1.1 | **Usage profile:** mostly solo with gear, sometimes driver + 1 passenger, rarely driver + 2 (short, slow trips). Adults about 5'9" and 200 lb each. **Design payload: 280 lb of driver + gear (a 200 lb driver with about 80 lb of gear).** On the 1860 with the 2GR the estimate leaves about 337 lb under the plate, so the driver can carry about 137 lb of gear; the 57 lb difference is the weight-growth allowance (R7.5). Driver + 1 (about 400 lb) is about 63 lb (4%) over the plate; the boat must still plane at that load. **Must plane** solo at the design payload and with driver + 1. With driver + 2, the boat may run slower but must meet the freeboard, stability and flotation requirements (R3.3, R3.4, R5.7). | Usage decision 2026-11-02; payload updated 2026-11-03 for the 1860 and 2GR (`CAD/analysis/performance_1860.js`). | D | W |
| R1.2 | Total load including crew, fuel and gear is within the hull's capacity plate (max persons and max weight). With the 1860 and the 2GR the equipment is about 1,213 lb against a plate of about 1,550 lb (check the plate on the hull you buy). | Legal and safety limit of the hull. The plate's maximum horsepower is an outboard rating (about 60 hp on a Tracker Grizzly 1860) and the airboat exceeds it: see the open questions. | I | B, W |
| R1.5 | Tandem centerline seating. The driver has a reclining bucket seat on tracks with about ±3 in of travel. The passenger(s) sit on a padded, full-width storage box in front of the engine, with a padded backrest/grab bar wide enough for two. | Seating decision 2026-11-02. | I | B |
| R1.6 | A crew member can move from the storage box past the driver seat to the bow along the port side, through a clear walkway of at least **12 in (proposed)**, with a handhold within reach at every point: backrest bar, driver seat back, and a port gunwale grab rail. | Safety in wakes and swells (decision 2026-11-02). | D | B, W |
| R1.7 | Dash and controls as specified 2026-11-02, all marine-grade and weatherproof: tach, fuel and temperature gauges; lanyard kill switch; key switch; light switches; searchlight switch and joystick; 12 V/20 A outlet; USB-A charger; phone/tablet holder; two-axis inclinometer visible to the seated driver. | Controls decision 2026-11-02. | I | B |
| R1.8 | Hull sized so that the design loads (solo and driver + 1) are within the hull's capacity plate. Driver + 2 is allowed only for short trips in calm, sheltered water. **Hull decision: an 1860-class hull (about 18 ft, 60 in bottom, about 1,550 lb plate)**, chosen 2026-11-03 together with the 2GR; the 1448 and 1652 were too sensitive to cargo weight. | Capacity analysis 2026-11-02; hull and engine study 2026-11-03 (Trim_Stability_Seating_Analysis.md, Section 13). | I, A | R |
| R1.9 | Gear stowage with tie-down points at three or more stations along the boat (proposed: bow/front deck about X 130, storage box about X 64, and both sides aft beside the engine above the foam blocks), so the solo driver can shift their gear (about 80 lb design value, up to about 137 lb, see R1.1) fore/aft and side to side to trim the boat, using the inclinometer. The walkway (R1.6) and the foam (R5.7) must stay clear and secured. | Trim-by-stowage decision 2026-11-02. Estimate (1448 analysis, to be redone for the 1860): moving 200 lb from the box to the bow changes static trim about 1.8°. With the 2GR the prop torque is far larger (R3.8), so stowage alone cannot balance it. | D | W |
| R1.3 | Driver can reach the throttle, steering stick and kill switch while seated, with a clear forward view over the bow. | Ergonomics; no seat or stick is modeled yet (release notes, known issue). | D | W |
| R1.4 | Two people can launch and retrieve the boat at a public ramp with a standard trailer. | Operations. | D | W |

## 2. Performance

| ID | Requirement | Rationale / source | Method | Where |
|---|---|---|---|---|
| R2.1 | Static thrust at full throttle: at least **750 lbf (proposed)**. | Model estimate about 850 lbf for about 270 hp (scaled by power^(2/3) on the current 64 in prop, `performance_1860.js`). Set the final value once the prop and redrive are sized. | T | S |
| R2.2 | Gets on plane with the driver + 1 load within **10 s (proposed)** from idle, in calm water. | Usability; an underpowered setup shows up here first. | T | W |
| R2.3 | Top speed: solo at least **35 mph (proposed)**; record driver + 1 and driver + 2. | 1860 model estimate: about 38.5 mph solo, 37.75 mph with driver + 1 and 37 mph with driver + 2, ±25% until thrust is measured. | T (GPS) | W |
| R2.4 | Runs on plane in **6 in (proposed)** of water without the hull or rudders touching bottom. | The reason to build an airboat: shallow water. | D | W |
| R2.5 | Fuel endurance of at least **1.5 h (proposed)** at cruise, keeping a 1/3 reserve. Measure fuel burn in gal/h at cruise and at full throttle. | The tank is being redesigned (a fixed tank in front of the bow bench). The 2GR burns much more fuel than a small engine, so size the tank from the measured burn rate. | T | W |
| R2.6 | Prop RPM never goes above the prop maker's maximum, at any throttle setting. | Prop safety; the redrive ratio is set for the 2GR (on hold). | T | S, W |
| R2.7 | Prop tip speed at maximum RPM is no more than **Mach 0.85**. | Noise and efficiency (static test report). | A, T | R, S |

## 3. Handling and stability

| ID | Requirement | Rationale / source | Method | Where |
|---|---|---|---|---|
| R3.1 | Full-left to full-right rudder turns the boat at cruise without the hull skidding out of control. Turning circle at least as tight as **4 boat lengths (proposed)** at cruise. | Steering authority. | T | W |
| R3.2 | Coasting stop from cruise (throttle to idle, then kill) within **100 ft (proposed)**. Record the distance at cruise and at top speed. | An airboat has no reverse or brakes; the crew needs to know the stopping distance. | T | W |
| R3.3 | With both crew on one side, at rest, the low-side freeboard is at least **3 in (proposed)**. | Static stability with a tall, heavy drivetrain. | T | W |
| R3.4 | At rest with the R1.1 load, transom freeboard is at least **4 in (proposed)**, and no water comes over the stern when the engine is cut quickly from cruise. | Airboats squat at the stern; stern swamping is a known risk. | T | W |
| R3.5 | No porpoising or chine-walking that the driver cannot correct with the throttle, at any speed up to top speed. | Safe planing behavior. | D | W |
| R3.6 | With an empty boat and a solo driver at the dock, the boat sits slightly stern-down: **0.5–2° (proposed)**. 1860 model estimate: about 2.1° with full fuel, slightly over, because the 2GR is heavy and aft. Shift gear forward (R1.9) or put the fuel tank forward (planned). | Trim decision 2026-11-02. | T (inclinometer) | B, W |
| R3.7 | With a solo driver at top speed, running trim is **2–4.5° (proposed)**. 1860 model estimate: about 1.5° (below the target): about 850 lbf on the high thrust line pushes the bow down hard. A lower prop axis or a down-tilted thrust line is a design driver for the 2GR drivetrain (on hold with the prop and redrive). | Lowest drag for a flat bottom is about 3.5–4.5°; the target is to be as close as the layout allows. | T | W |
| R3.8 | A turnbuckle-adjustable trim tab on the starboard side of the transom cancels prop-torque roll at cruise, so the boat runs within **1° (proposed)** of level across the beam. Analysis recommends 10 x 6 in, adjustable 0–10°. | The prop turns clockwise looking aft, so its torque rolls the boat starboard side down (about 620 ft-lb for the 2GR at full power: about 270 hp at about 2,300 prop rpm). The 10 x 6 in tab was sized for a 70–100 hp engine; resize it with the drivetrain. | T (inclinometer) | W |

## 4. Structure and drivetrain

| ID | Requirement | Rationale / source | Method | Where |
|---|---|---|---|---|
| R4.1 | No cracks, deformation or fastener movement at the beds, mount plates, cage supports, welds or bench holes after the static test and after each on-water session. | Static test pass/fail criterion 1. | I | S, W |
| R4.2 | Prop tip clearance to the cage and screens is at least **2 in** at every point, measured cold and after running. | Static test criterion 5. CAD check (2026-11-01): 4.5 in at the top, 5.9 in at the bottom. | A, I | R, S, W |
| R4.3 | No contact between moving parts and the structure, wiring, hoses or the shaft guard. | Static test criterion 6. The CAD check found the prop shaft touching the fixed shaft guard sleeve after the 0.7 in drivetrain raise; this must be fixed before the build. | A, I | R, S |
| R4.4 | Coolant stays below **210 °F** for 10 min at 75% throttle (static), and through a **20 min (proposed)** full-load run on the water. | Static test criterion 3; cooling airflow differs when moving. | T | S, W |
| R4.5 | Oil pressure stays within the engine maker's range at idle and at full throttle. | Static test criterion 4. | T | S, W |
| R4.6 | The bottom does not leak: the bilge is dry after **30 min (proposed)** afloat at rest and after each run. | Hull armor uses through-bolts in the bottom. | T | B, W |
| R4.7 | The drivetrain module can be lifted out and back in with an engine hoist without cutting anything. | Build Guide 2.8–2.10 design intent (liftable module). | D | B |
| R4.8 | The 2GR runs on an engine computer (standalone, or the stock ECU with its key and immobilizer) with a rev limiter, an overspeed fuel cut and a safe default (idle or shutdown) if a throttle sensor fails. | A junkyard car engine needs its electronics replaced or tamed; most 2GR donors have an electronic (drive-by-wire) throttle, which the pedal and cable (Build Guide 2.20) must work with. | I, T | S |
| R4.9 | The redrive belt and pulleys, prop, prop shaft and bearings, A-frame, engine mounts and cage are rated for about **270 hp and about 620 ft-lb of prop torque**, with the maker's margin. | The existing parts were sized for a 70–100 hp engine. Resizing is on hold until the 1860 branch settles. | A, I | R |

## 5. Safety

| ID | Requirement | Rationale / source | Method | Where |
|---|---|---|---|---|
| R5.1 | Lanyard kill switch stops the engine within **2 s** at idle, at 50% and at full throttle. | Static test criterion 7. | T | S, W |
| R5.2 | Throttle returns to idle by itself when released. | Static test criterion 8. | D | S, W |
| R5.3 | No fuel, oil or coolant leaks; no exhaust leaks near the crew or the fuel system. | Static test criterion 2. | I | S, W |
| R5.4 | The prop guard keeps hands, clothing and loose objects out of the prop disc from every crew position. | Personal safety. | I | R, B |
| R5.5 | Carries the legally required safety gear: life jackets for all aboard, a fire extinguisher, a sound signal and navigation lights if used after dark. | California and USCG equipment rules. | I | B, W |
| R5.6 | Hearing protection is worn by everyone aboard while the engine runs. Operator-seat noise is measured and recorded. | Static test plan PPE. | I, T | S, W |
| R5.7 | When completely swamped with all equipment aboard, the boat floats roughly level and does not sink. Flotation is at least **1.2 x** the in-water weight of the boat and equipment: about 880 lb, or 14.5 ft³ of fuel-resistant closed-cell foam, centered near X 35–40 in. Removable foam blocks are strapped down. (Estimate for the 1652 with the earlier engine; redo for the 1860 and the 2GR.) | Decision 2026-11-02; replaces the foam lost with the middle bench. | A, T (swamp test) | R, B |
| R5.8 | The middle-bench thwart is replaced by a structural cross member: the storage box frame, bolted side to side and tied to the backrest/grab bar. | Hull stiffness after removing the middle bench. | I | B |

## 6. Legal and environmental

| ID | Requirement | Rationale / source | Method | Where |
|---|---|---|---|---|
| R6.1 | Registered with the California DMV, with numbers displayed, before any on-water test on public water. | California registration law; the boat is a modified, motorized vessel. | I | B |
| R6.2 | Meets the California noise limits for motorized recreational vessels: **88 dB(A)** stationary per SAE J2005 (engines made on or after 1993), and **75 dB(A)** shoreline per SAE J1970 when operated. Confirm the current text before testing. | CA Harbors & Navigation Code § 654.05. Airboat prop noise can exceed these, so measure early. | T | S, W |
| R6.3 | Airboat operation is allowed on the chosen test water. | Many lakes and local agencies ban or restrict airboats. | I | R |

## 7. Buildability and cost

| ID | Requirement | Rationale / source | Method | Where |
|---|---|---|---|---|
| R7.1 | Every task can be done with DIY backyard-mechanic skills and common tools; any task that needs a pro, such as welding, is marked in the Build Guide with its pro hours. | Project design rule. | I | R |
| R7.2 | BOM total no more than **$11,000 (proposed)**, including the boat. | **The current estimate is about $19,100** with the 1860 and the 2GR (the redrive and prop prices are not yet updated), so this target needs a decision. | A | R |
| R7.3 | One airboat: one hull and one drivetrain (Toyota 2GR). The redrive pulleys and belt, prop and cage are sized for the 2GR. | Single-engine decision 2026-11-03; replaces the earlier two- and three-prototype plan. | A, I | R, B |
| R7.5 | Airboat equipment weight (everything except the bare hull, crew and cargo, with full fuel) is no more than the hull's plate rating minus the 280 lb design payload: **about 1,270 lb or less on an 1860 (about 1,550 lb plate)**. The current estimate is about 1,213 lb, which leaves about 57 lb of growth allowance. **Every added part reduces the driver's gear allowance pound for pound; report it when design changes add weight.** | Keeps the design payload (R1.1) within the capacity plate. | A, T (scale) | R, B |
| R7.4 | Total weight ready to run with fuel (no crew) is recorded at each build stage and compared with the estimate (about 1,213 lb of equipment plus a hull of about 640 lb). | Equipment estimate from `performance_1860.js`; the engine, redrive and radiator weights are estimates until the parts are weighed. | T (scale) | B |

---

## Open questions for the team
1. Accept, change or delete each **(proposed)** value.
2. Should a target cruise speed be added (for example, a comfortable cruise at 60% throttle)?
3. Is 35–40 mph with passengers a firm goal? It decides the prop and redrive sizing, which is on hold.
4. Any requirements for night use, cold-weather use or carrying cargo, such as hunting or fishing gear?
5. Which test water will be used, and does it allow airboats (R6.3)? This decides the noise and shallow-water test setup.
6. The capacity plate's maximum horsepower is an outboard rating (about 60 hp on an 1860), far below the 2GR. How does this affect registration (R6.1) and insurance?

**Next:** once these values are agreed, write `On_Water_Test_Plan.md` and `On_Water_Test_Protocol.md` (Build Guide 4.1). Each test will list the requirement IDs it verifies, and the plan will end with a traceability table so that no requirement is left untested.
