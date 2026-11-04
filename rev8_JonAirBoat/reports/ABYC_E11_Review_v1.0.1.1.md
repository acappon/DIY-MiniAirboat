# ABYC E-11 Review of the Electrical Plan — v1.0.1.1

**Reviewed:** `electrical/wire_list.py`, `electrical/Wire_List.csv`, `reports/Drivetrain_800R_ETEC.md` Section 9f, BOM lines 36 and 57–75, and the harness routes in CAD.
**Status:** desk review of the design. Nothing is built, so workmanship items can't be checked yet. Findings 1, 3, 4 and 5 are now applied to the plan. Finding 2 is applied except for the sled engine electrics, which stay a known deviation.

> **Edition caution:** checked against the **July 2003 edition of E-11**, the only full text publicly available. The current edition may differ in numbering and content, and this was not checked against it. Buy the current E-11 from ABYC, or have a marine electrician or surveyor confirm, before treating anything here as final. Lithium batteries are covered by a separate standard (E-13), which was not available and is not reviewed here.

## Summary

| Result | Count |
|---|---|
| Did not meet E-11 as first drawn | 4 (3 fixed, 1 fixed except the sled engine electrics) |
| Can't be confirmed yet, or depends on parts | 8 (see "Closed out for v1.0.1.1": 2 closed, the rest set by spec and waiting on the service manual or the build) |
| Meets E-11 | 9 |
| Not applicable | All AC sections: the boat has no AC system |

The overall architecture is sound. The fuse sizing, voltage drop, exhaust clearance and no-hull-return design all hold up. The main work is wire size, ignition protection and a few installation details.

## Did not meet E-11 as first drawn

| # | Clause (2003) | Finding | Proposed fix |
|---|---|---|---|
| 1 | 11.16.1.1.2 | **18 AWG is below the 16 AWG minimum.** 44 wires, about 315 ft. 18 AWG is allowed only inside a jacketed sheath with no more than 30 in outside it, inside panels, or for signals under 1 A in electronic circuits. These are individual wires in split loom, including power runs of up to 17 ft. | Make all general wiring 16 AWG. Keep W46 (type K extension) and W50 (supplied cable) as supplied, and W47–W49 to match the sled harness. **Done:** 39 wires are now 16 AWG. +1.1 lb, which took the margin from about +5.5 lb to about +4.4 lb (estimate). Searchlight drop improved from 5.9% to 3.7%. |
| 2 | 11.5.1.3 | **Ignition protection.** Components near a gasoline source must be ignition protected unless isolated. In an open space, isolation needs at least 2 ft of separation. The exception covers outboard engines, not a sled engine on a stand. The sled starter, solenoid, regulator and engine electrics sit on the same engine as the fuel connections and carry no marine rating. The bow light (X 157) is about 5 in from the tank (ends at X 152). | **Solenoid, done:** Cole Hersee 24059-08-BP. Its data sheet confirms UL listing and ratings, and the catalog family carries the ignition-protected mark, but check the mark on the part itself. **Bow light, done:** no bi-color light with a published J1171 or UL 1500 rating was found, so it is replaced by a pair of sidelights on the hull sides at X 108, about 3 ft from the tank and fuel fittings, in open air (11.5.1.3.3.2.3). **Sled engine electrics:** the starter, regulator and ECM probably can't comply. Record that as a known deviation for the surveyor or insurer. |
| 3 | 11.16.2.1.2, Table XIV | **Wire color conflicts.** W42 is green, which is reserved for DC grounding. W39 is yellow, which marks DC negative. | **Done:** see the color plan below. |
| 4 | 11.16.4.1.11 | **Too many conductors per stud on TB2:** 6 on the positive bus and 6 on the negative, and the limit is 4 per stud. TB1 has exactly 4. | **Done:** TB1 is a 6-screw bar and TB2 is a pair of 8-screw bars, with one ring terminal on each screw. |

## Can't be confirmed yet

| # | Clause (2003) | Item | What's needed |
|---|---|---|---|
| 5 | 11.7.1.2 | **Battery switch.** Required only above 800 CCA. BOM line 36 is a motorcycle-size lithium battery with no CCA chosen, and the plan has no switch. | **Done:** fitted anyway, so power can be cut quickly in a fire or for prop work. It is on the middle bench forward panel, in the positive feed, ahead of W01 and W03. The bilge pump feed (F7) moved to the battery side, which the clause allows. Still to check: the battery's CCA, and the switch's intermittent rating against the 800R starter. |
| 6 | 11.16.4.1.6, 11.16.4.2.2.1 | **Harness height.** The floor run sits at about Z 3.2, and W02 is a battery cable with no fuse, so it must stay above normal bilge water. The bilge pump pair runs at about Z 1.5–2 and must have watertight connections (the DT plug should do this). | **Done in the design:** the floor run is raised to Z 4 in conduit, with its underside about Z 3, about 1 in above an assumed 2 in bilge level. The bilge pump pair stays low by nature, with DT plugs. Still to check: the real pump's float height. |
| 7 | 11.16.4.1.8 | **Harness through the bench foam.** The plan runs it through foam, where it can't be inspected or replaced. | **Done:** a 2-1/4 in aluminum sleeve through the foam, with a gland at the box. |
| 8 | 11.16.4.1.9, .10 | **Loom and support.** Loom must be self-extinguishing (UL 94 V-2 or better), and conductors must be secured at least every 18 in. Battery cables get an exception within 36 in of the battery. Plastic straps can't be used where a failure over the engine or a moving shaft would be hazardous. | **Done:** BOM line 73 specifies V-2 conduit and loom, clips every 18 in, and lined metal clamps near the engine, belt and prop and along the cage. Check on the build. |
| 9 | 11.12.1.1 | **Charging lead.** The regulator's connection to the solenoid stud is not modeled, and its protection isn't defined. The charging output itself is unknown. | Service manual. Also check that this regulator suits a lithium battery, and what happens if the battery's protection circuit disconnects while the engine charges. |
| 10 | 11.12.1.3 | **Pump protection.** F7 and F8 must protect the bilge and fuel pumps against a locked rotor for seven hours. | Check with the chosen pumps. Fuel pump current is still an estimate. |
| 11 | 11.16.3.4 | **Spade terminals.** Friction terminals are allowed only up to 20 A, with limits on rating and voltage drop. W05 and W06 run at exactly 20 A. | Use ring terminals on the 12 V outlet. Choose rated terminals for the relays and switches. |
| 12 | 11.17.2.4 | **Gas sender.** If the sender is grounded through a metal tank, its return must run directly to the negative bus and be shared with nothing else. | Confirm the sender type. W44 and W45 are fine for an isolated two-wire sender. |

## Closed out for v1.0.1.1

| # | Decision | State |
|---|---|---|
| 1 | **Sled engine electrics:** the sled harness stays as built. It is a documented deviation, on an open boat, with ignition-protected parts on the boat side. | Accepted by design. Keep this review with the boat's papers. |
| 5 | **Battery and switch:** a 12 V lithium starting battery, at least 300 CCA and about 12 Ah, under the 800 CCA line. The switch stays fitted anyway: at least 300 A continuous and 600 A intermittent. | Spec set. Confirm against the starter draw. |
| 9 | **Charging:** regulator output lands on the solenoid stud, with a fuse sized to the output within 7 in of the stud. | Spec set. Fuse size and lithium suitability need the service manual. |
| 10 | **Pump protection:** F7 goes from 10 A to 5 A, or the pump maker's value if lower. F8 stays at 15 A until the fuel pump's current and fuse are known. | F7 closed. F8 needs the service manual. |
| 11 | **Spade terminals:** W05 and W06 now use ring terminals and a stud-type outlet. Relay and switch terminals are rated, sealed parts. | Closed. |
| 12 | **Gas sender:** an isolated two-wire sender in a plastic tank, so W44 and W45 stand. | Closed by BOM line 59. |
| 6 | **Pump float height:** the design assumes a 2 in bilge level. | Check on the boat. |

**Current editions, from public summaries only.** ABYC sells the full text, and neither was read in full.

- **E-11 (2025):** adds fault-current ratings for overcurrent devices. Where no battery rating exists, lithium is taken as 5,000 A per 100 Ah. For a 12 Ah battery that is about 600 A, within the ratings of the planned fuses. Check the MF1 data sheet.
- **E-13 (2025, lithium):** public sources give the size threshold as 500 Wh or 600 Wh. A 12 Ah battery is about 150 Wh, under either figure. It still needs a built-in BMS, and it follows the maker's instructions.
- **Bundling:** still unconfirmed for DC in the current edition. The cage run is lightly loaded, with 7.2 A on 12 AWG and 3.7 A on 16 AWG.

## Meets E-11

| Clause (2003) | Check |
|---|---|
| 11.12.1.2 | MF1 is on the switch output stud, F7 is within 7 in of the battery, and W57 carries the fusible link. The cranking cable is exempt, so that link goes beyond what's required. |
| 11.12.1.4, .5 | Every fuse is within 150% of the wire's ampacity (Table IV, 105 °C wire), and also passes at 16 AWG. |
| 11.16.1.2.7 | Navigation lights are under 1% (limit 3%). Dash feeds are 1.6–2.5% (limit 3%). Lighting is 4.0–5.9% (limit 10%). |
| 11.16.4.1.7 | Harness routes stay more than 23 in from the pipe, silencer and tailpipe (limit 9 in for dry exhaust). Wiring on the engine and the exhaust probe wire are excepted. |
| 11.16.1.2.2, .5 | BOM calls for marine stranded copper wire. Confirm UL 1426 markings when buying. |
| 11.16.2.1.1 | Each wire has a number and a schematic. Print the numbers on labels at both ends. |
| 11.16.4.1.1 | The fuse block and relays are in a waterproof box. |
| 11.16.4.2.2.3 | The positive cable has a route clear of the engine. Check this on the real boat. |
| 11.18 | No DC grounding system is installed, which the standard allows, and the hull carries no current. The engine block will tie the hull to battery negative through the stand, so keep that the only connection. Check this in the current edition. |

## Wire color plan

Source: E-11 Tables XIV and XV and the [Lighting and Supplies guide](https://blog.lightingandsupplies.com/understanding-abyc-color-codes-for-boat-wiring/). Table XV is one accepted code, not the only one, and other schemes are allowed with a wiring diagram. Only W42 and W39 are real conflicts. The rest follow the request to use the guide. **All of this plan is applied.**

**This changed one of the 2026-11-03 rules.** Every feed was red. The plan below moved feeds to their function colors. Battery cables, main feeds, LED bar, searchlight, 12 V outlet and all returns stay red and black. Black, not yellow, stays the negative color.

| Wire | Function | Was | Now | Why |
|---|---|---|---|---|
| W36 | Key START to solenoid | purple | **yellow/red** | Starting circuit |
| W35 | Key ON to K1 and engine | orange | **purple** | Ignition |
| W16, W27, W29, W31, W33 | Instrument feeds | red | **purple** | Instrument feed |
| W15, W22–W25 | Switch bus and USB feed | red | **orange** | Panel to accessory switches |
| W11, W13, W37, W52, W58 | Navigation lights | red, blue | **dark gray** | Navigation lights |
| W18 | Bilge pump feed | red | **brown** | Pumps |
| W20, W55 | Fuel pump feed | red | **brown** | Pumps |
| W40 | Pump signal to K5 coil | pink | **brown/white** | Pink means fuel gauge |
| W38 | LED switch to K3 coil | violet | **gray/blue** | Guide: flood lights |
| W39 | Searchlight switch to K4 coil | yellow | **gray/red** | Guide: spotlight. Yellow means negative |
| W41 | Tach signal | gray | **dark gray** | Tachometer |
| W42 | Temperature sender | green | **tan** | Water temperature. Green means grounding |
| W43 | Temperature sender return | brown | **black** | Return |
| W44 | Gas sender | tan | **pink** | Fuel gauge |
| W45 | Gas sender return | brown | **black** | Return |
| W46 | Exhaust probe | type K | no change | Thermocouple wire has its own colors |
| W47–W49 | DESS | white | no change | Match the sled harness |
| W50 | Searchlight remote | gray | no change | Supplied cable |

## Other things this review turned up

- **Build Guide:** the old step text says to keep 2 in between exhaust and wiring. E-11 calls for 9 in from dry exhaust unless there's a thermal barrier. Correct this in the rewrite.
- **Bundling:** the 2003 bundling tables are in the AC section. Later editions may derate bundled DC wires, which would matter for the cage run. Check this in the current edition.

## What was applied

- `electrical/wire_list.py` updated, with `electrical/Wire_List.csv`, `electrical/Schematic.html`, the schematic PDF and `CAD/FeatureScript/Minimal_wiring.fs` regenerated from it.
- Onshape Main workspace, "Minimal wiring (FeatureScript)", updated: all 56 wires rebuilt with the new names, sizes and colors.
- Battery switch and W57 added, W01 and W03 rerouted through it, F7 moved to the battery, and the battery switch, W57 and its marker added to Main assembly.
- TB1 and TB2 defined as screw-terminal bars.
- Cole Hersee 24059-08-BP starter solenoid specified.
- Bow light replaced by sidelights: W13 and W14 go to the port light, new W58 and W59 go to the starboard light, and the new parts are in Main assembly.
- Low oil light added (W60 to W62), using white/blue, the guide's oil indicator color.
- Floor run raised to Z 4 in conduit, with a sleeve through the bench foam; both are in Main assembly.
- Battery cables changed from 1/0 to 2 AWG for weight, and the fusible link to 6 AWG. Confirm the starter draw from the service manual.
- Harness 10.8 to 12.2 lb in `CAD/analysis/minimal_1448.js`. See the release notes for the current weight.
- BOM lines 46, 58, 65, 66, 67, 68, 71 and 73 updated, and new lines 76 (switch), 77 (solenoid) and 78 (low oil LED) added. All prices are estimates. See the release notes for the current BOM total.

## Sources

- [ABYC E-11, July 2003 edition](https://law.resource.org/pub/us/cfr/ibr/inc/ansi/abyc.e-11.2003.pdf) (public copy)
- [Understanding ABYC color codes for boat wiring](https://blog.lightingandsupplies.com/understanding-abyc-color-codes-for-boat-wiring/), Lighting and Supplies
- [Cole Hersee continuous duty solenoid data sheet](https://images.boatid.com/cole-hersee/items/pdf/24059-08-bp-data-sheet.pdf)
- [DC main overcurrent protection requirements](https://www.bluesea.com/resources/99), Blue Sea Systems
