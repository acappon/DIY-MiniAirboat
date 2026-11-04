# Static Test Plan: Installed Drivetrain

**Project:** DIY Jon-Air airboat
**Applies to:** Build Guide task 3.1 (after tasks 2.1–2.17)
**Companion documents:** `Static_Test_Protocol_Drivetrain.md` (step-by-step procedure and data sheets), `Static_Test_Performance_Report.md` (results)
**Build:** minimal 1448, Rotax 800R E-TEC two-stroke, 8MGT belt, 2.94:1, 66 in prop (v1.0.1.0). Oil is injected from a separate tank, so there is no oil pressure to read.
**Status:** Draft for the first static test. Values marked *planning estimate* are model estimates, not measurements. Replace them with service manual and maker data where available.

---

## 1. Purpose

Prove that the drivetrain, its mounts and the prop cage are safe and work as intended **before the boat goes on the water**. A static test means the boat is restrained on its trailer on land while the engine and prop run.

## 2. Objectives

| # | Objective | How it is measured |
|---|---|---|
| O1 | Stand, shoes, clips and bearing plates carry full thrust with no movement or damage | Torque stripes, reference marks at the shoes, visual crack inspection before and after, including every petal edge |
| O2 | Engine starts, idles and runs to full throttle reliably | Start attempts, idle RPM, throttle response |
| O3 | Cooling holds temperature under load | Coolant temperature gauge and IR thermometer, 10-minute run at 75% |
| O4 | Fuel, injection oil, coolant and exhaust systems have no leaks | Visual inspection at each phase |
| O5 | Prop and belt run smoothly with safe cage clearance | Vibration check, tap test, belt flutter watch, tip-clearance marks, no contact |
| O6 | Measure static thrust vs. prop RPM, and set the blade pitch | Crane scale in the restraint line, optical tachometer; 7,400–7,500 engine RPM at full throttle |
| O7 | Rudders have authority in the prop blast and no flutter | Thrust-line side force or visible yaw tendency, rudder buzz |
| O8 | Kill switch and throttle return work every time | Functional checks at idle and mid throttle |
| O9 | Record noise at crew and bystander positions, and decide on the second muffler | Sound-level meter |

## 3. Success criteria (pass / fail)

The test **passes** only if every item below is met.

- No cracks, deformation or fastener movement at the stand, shoes, clips, gussets, bearing plates, cage pads or struts. No cracks at any petal edge. Torque stripes are unbroken after the test.
- No leaks of fuel, injection oil or coolant. No exhaust leaks near the crew or fuel system.
- Coolant temperature stabilizes below **210 °F** (or the service manual limit) during the 10-minute run at 75% throttle.
- Exhaust temperature stays under the service manual limit at every step.
- The low oil light works, and the oil tank level falls during the run, which shows the pump is delivering.
- Prop tip clearance to the cage stays at least **2 in** all round after the test, with no witness marks on the cage or mesh.
- No contact between moving parts and wiring, hoses or structure.
- The kill switch stops the engine within 2 seconds, every time it is tried.
- The throttle returns to idle by itself from any position.
- Engine RPM never exceeds **7,900**, and prop RPM never exceeds **3,200** (Warp Drive HP manual).
- No belt flutter that touches the guard, and the belt tracks centered between the flanges.
- Measured static thrust is recorded at each RPM step. There is no pass/fail number for thrust in this first test; it becomes the baseline.

*Planning estimates:* about **480 lbf** static thrust at about 7,450 engine RPM and 2,530 prop RPM, about 113 hp. (The 497 lbf figure is at 120 hp and 7,600 RPM, which the engine reaches once the boat is moving.) If the measured value is far below this, investigate before the water test (prop pitch, engine tune, blocked inflow).

*Cooling estimate:* tied to the dock, the radiator sheds about 61 kW against 76–89 kW of heat at 120 hp, so **full throttle at the dock is limited to about 1 minute at a time**, and coolant should climb from 60 to 95 °C in about 75 seconds. The 10-minute run is at 75%, where heat and cooling are about even, so it may run warm: that is what the run is there to find out. Moving air fixes this on the water.

## 4. Abort criteria (stop immediately: kill switch)

Stop the engine at once if any of these happen:
- Anyone or anything enters the prop danger zone (see section 8).
- Any unusual noise (knock, scrape, squeal), new vibration, or something coming loose.
- Smoke, flame, fuel smell or visible leak.
- The boat or trailer moves, the restraint line goes slack and snaps tight, or any tie-down slips.
- Coolant temperature passes 220 °F (or the service manual limit), or exhaust temperature passes the service manual limit.
- The low oil light comes on.
- Engine RPM passes 7,900, or prop RPM goes over the prop limit.
- The belt slaps the guard, or heavy vibration appears at a steady throttle.
- Loss of communication between the test lead and the operator.

## 5. Test article and configuration

- Boat with all Section 1 and Section 2 tasks complete and the pre-test inspection (task 2.14) signed.
- Boat on its trailer. The trailer is chocked on both sides of the wheels, tongue on the hitch of a parked vehicle (parking brake on, in Park/in gear, engine off, keys removed) or on a heavy stand.
- **Restraint:** the boat bow eye goes to a fixed anchor *ahead* of the boat (prop thrust pushes the boat forward) through the crane scale. Use a rated strap or chain with a working load of at least **2,500 lb** (about 5x the planning thrust). Add a **second, slightly slack backup restraint** that does not go through the scale. The boat must also be strapped down to the trailer as for highway towing.
- Fuel: about 2 gal (enough for the test, limits fire load).
- Fire extinguisher at the operator and a second one held by the fire watch.

## 6. Instrumentation

| Instrument | Purpose | Location |
|---|---|---|
| Digital crane scale, 1,000 lb, with peak hold | Static thrust | In the main restraint line |
| Optical (laser) tachometer + reflective tape on one blade, or ignition-pickup tach | Prop RPM and engine RPM | Tape on a blade root; reader from beside the cage, outside the danger zone |
| Coolant temperature gauge (dash) + IR thermometer | Cooling | Dash; IR on radiator inlet/outlet and both prop-shaft bearing housings |
| Exhaust temperature gauge (dash) | Exhaust temperature, and any restriction after the pipe | Dash |
| Low oil light and a mark on the oil tank | Oil supply | Dash; tank level marked before the run |
| Sound-level meter (BOM line 40) | Noise, and the second muffler decision | Operator seat, 25 ft to the side, 50 ft |
| Soft mallet and a phone vibration app | Stand tap test | On the stand |
| Phone camera on a tripod (slow-motion if available) | Record cage, mounts, belt flutter | Beside the boat, outside the danger zone |
| Torque wrench, paint pen, feeler gauges, flashlight, mirror | Inspection | Tool tray |
| Stopwatch, data sheets, clipboard | Recording | Data recorder |

## 7. Crew roles (minimum 4 people)

| Role | Duties |
|---|---|
| **Test lead** | Runs the protocol, calls each step, decides go/no-go, holds the radio or uses hand signals. Never the operator. |
| **Operator** | On the middle bench with the kill-switch lanyard attached. Starts and stops the engine and sets the throttle only when the test lead calls for it. |
| **Data recorder** | Reads the crane scale and tach, writes data, keeps time. Stands at the side, ahead of the prop plane. |
| **Safety officer / fire watch** | Watches the danger zone, bystanders, leaks and smoke; holds the second extinguisher; can call STOP at any time. |

Everyone can call **"STOP"**. When anyone does, the operator pulls the kill switch, no questions asked.

## 8. Safety

### 8.1 Personal protective equipment (PPE): required for everyone inside 100 ft

| PPE | Why |
|---|---|
| **Double hearing protection:** foam ear plugs **and** ear muffs (NRR 25+ muffs) | Airboats commonly exceed 100–120 dBA near the prop; permanent hearing loss can happen in minutes |
| **Safety glasses** (operator and anyone near the cage: **full face shield** over glasses) | Debris thrown by the prop, belt failure, fuel spray |
| **Closed-toe shoes** with good grip | Wet, oily surfaces; dropped tools |
| **Close-fitting clothing.** No scarves, hoodie strings, loose jackets, hats or jewelry. Long hair tied back and tucked. | Anything loose can be pulled into the prop or belt |
| **Mechanic gloves** for inspection between runs | Hot exhaust, radiator and bearing housings |
| Operator: **life-vest-style chest protection is optional**, but the operator must wear the **kill-switch lanyard** | The lanyard stops the engine if the operator leaves the seat |

### 8.2 The prop danger zone

- **Never stand in the plane of the propeller disc**, on either side, at any distance you can avoid. If a blade or the hub fails, pieces leave in that plane at very high speed.
- **Nobody behind the boat within 100 ft** (prop blast picks up dirt, gravel and loose items). **Nobody in front within 25 ft** except behind the anchor.
- Mark the zones on the ground with cones or tape before starting.
- Only the operator is in the boat while the engine runs. Never reach into or touch the cage while the prop turns. Wait until it has fully stopped.

### 8.3 Foreign object damage (FOD) and suction

- The prop pulls air (and anything light) from in front of and around the cage. Before every run, sweep the boat and the area within 30 ft: rags, gloves, plastic bags, loose tools, hats, leaves.
- Check that the cage mesh is tight and every cage fastener is safety-wired or locked.
- Nothing is placed on the engine, the benches or the cage during a run.
- The radiator sits just ahead of the prop in the inflow, so check it for loose fins and debris too.

### 8.4 Other hazards to watch

| Hazard | Control |
|---|---|
| Boat breaks loose | Primary restraint through the scale plus a backup restraint; trailer chocked; tow vehicle parked with the keys out; stop if anything shifts |
| Fire (fuel near hot exhaust) | Leak check before every start; 2 extinguishers; minimal fuel; no smoking; nobody refuels a hot engine |
| Carbon monoxide | Outdoors only, never in a garage or shed, even with the door open |
| Burns | Tuned pipe, silencer, tailpipe, radiator and engine get hot; gloves; wait to cool; IR-check before touching |
| Belt or pulley pinch | There is no clutch: the prop and belt turn whenever the engine turns. Lock out (key out, lanyard off, battery switch off) before working on them |
| Battery and electrical | The automatic bilge pump stays live with the switch off. Disconnect the negative first and reconnect it last for work in the dry box; no metal jewelry; cover terminals |
| Coolant under pressure | Do not open the radiator cap hot |
| Hearing damage to neighbors and pets | Test in a suitable location and at a reasonable time; check local noise rules |
| Fatigue and distraction | Short test day; stop when people are tired; nobody under the influence |

### 8.5 Lockout before any hands-on work

Before touching the drivetrain, cage or prop: **ignition key out, lanyard removed, battery switch off, prop fully stopped.** The person working on it keeps the key in their pocket.

## 9. Test phases (summary; detailed steps in the protocol)

| Phase | What | Approx. time |
|---|---|---|
| A | Pre-run inspection and setup (task 2.14 checklist, tap test, restraint, zones, instruments, briefing) | 75 min |
| B | Engine run **with the prop removed**: start, idle, leaks, gauges, kill switch. Light throttle only: the engine has almost no load | 20 min |
| C | Prop installed (task 2.15): first start at idle, 2 minutes, then shut down and inspect | 20 min |
| D | Stepped throttle runs: 25% / 50% / 75% / 100% — thrust, RPM, temperatures; shut down and inspect after each step; set the blade pitch | 60 min |
| E | Rudder authority and kill-switch checks at mid throttle | 15 min |
| F | 10-minute endurance run at 75% | 20 min |
| G | Post-run inspection: torque check, cracks, clearances, leaks; prop bolt re-torque | 45 min |
| H | Data review and performance report | 30 min |

## 10. Records

- Completed protocol data sheets (paper or photos of them).
- Photos and video of the setup, gauges at each step, and the post-run inspection.
- The completed `Static_Test_Performance_Report.md`, signed by the test lead and safety officer.

## 11. After the test

- **Pass:** schedule the first on-water test with a separate water-test plan (calm, shallow, open water; chase person; PFDs).
- **Fail or abort:** record the cause, fix it, and repeat the affected phases (and always Phase G).
