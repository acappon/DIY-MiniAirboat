# Electrical Compliance Check — v1.0.1.3

**Standard:** ABYC E-11, July 2003 edition, the only full text publicly available
**Color scheme:** E-11 Tables XIV and XV, and the [Lighting and Supplies guide](https://blog.lightingandsupplies.com/understanding-abyc-color-codes-for-boat-wiring/)
**Plan checked:** `electrical/Wire_List.csv`, 62 wires, and the schematic, after the v1.0.1.3 wiring pass for the seat-module dry box. Rerun with `electrical/electrical_check.py`.
**Status:** a hobby design check on paper. It has not been reviewed by a licensed professional or an ABYC-certified technician, and no wiring has been built. Use it at your own risk.

## What changed since v1.0.1.2

The wiring is the same 62 wires with the same sizes, fuses, colors and connectors. What changed is where it lives, now that the middle bench and its waterproof case are gone.

**Electrical dry box:** now part of the seat module.
- **Box:** 0.063 in 5052 aluminum, X 77.5–85.5, Y ±5, Z 3.75–11, under the seat.
- **Inside:** the battery at the forward end; the fuse block on a bracket over it, with the fuses facing the gasketed lid; relays K1–K5 at the aft end.

**Battery switch:** on a plate on the forward seat strut, port of the box, knob forward, reachable without unbolting the seat. Its cables enter the box through the forward gland.

**Glands:**
- **Aft wall:** the harness runs aft into the 2 in floor conduit, as before. The bench sleeve is gone.
- **Forward wall:** new, at Y 3.8, Z 8. The forward runs (dash, side lights, fuel pump and sender) leave under the forward strut.

**Forward routes:**
- They stay at Z 10 or lower under the seat base (Z 11.25), and climb to the hull side only outboard of the seat edge (|Y| 19).
- The first routes passed through the seat base; they no longer do.
- No route crosses the struts, battery, fuse block, relays or switch body. This is checked by script.

**Wire weight:** 8.7 lb (was 8.3), because the forward runs now go round the seat module.

**Voltage drop:** largest is 3.7% (searchlight pair, limit 10%); navigation lights and dash feeds stay under 3%.


On paper the plan **meets every E-11 item that can be checked now**, and the **color scheme follows your guide** throughout. It doesn't amount to a claim of full compliance, for three reasons:

- The current 2025 editions of E-11 and E-13 are paid documents, so I read only public summaries of them.
- The sled engine electrics are a documented deviation.
- Four items wait on the BRP service manual or the finished boat.

| Area | Result |
|---|---|
| Wires checked | 62 |
| Over the Table IV ampacity | 0 |
| Over the voltage drop limit | 0 |
| Below 16 AWG without an exception | 0 (5 wires use an exception) |
| Reserved colors misused | 0 |
| Color differs from the guide's function | 0 (6 wires have no guide color) |

## E-11 clause check

| Clause (2003) | Requirement | Plan | Status |
|---|---|---|---|
| 11.16.1.1.2 | 16 AWG minimum | 16 AWG general wiring; 12 AWG and larger for heavy loads | Meets |
| 11.5.1.3 | Ignition protection near gasoline | Marked parts, or 2 ft of separation. Solenoid Cole Hersee 24059-08-BP | Meets on the boat side. Check the marks when buying |
| 11.5.1.3 | The same, on the engine | Sled engine electrics aren't marine rated | **Accepted deviation**, on an open boat |
| 11.16.2.1.2 | Wire colors | See below | Meets |
| 11.16.4.1.11 | No more than 4 conductors on a stud | One ring on each screw of TB1 and TB2 | Meets |
| 11.7.1.2 | Battery switch above 800 CCA | About 300 CCA, so not required. Fitted anyway | Meets. Confirm against the starter draw |
| 11.12.1.1 | Charging lead protected | Fuse within 7 in of the solenoid stud | **Open:** size needs the service manual |
| 11.12.1.2 | Fuses close to the source | MF1 on the switch stud, F7 within 7 in of the battery. The switch is now outside the box on the forward strut, about 6 in of 2 AWG from the battery post | Meets |
| 11.12.1.3 | Pump protected at locked rotor, 7 hours | F7 down to 5 A | Bilge meets. **F8 open:** needs the fuel pump current |
| 11.12.1.4, .5 | Fuse within 150% of ampacity | Every fuse checked | Meets |
| 11.16.1.2.7 | Voltage drop | Dash feeds 1.0–1.6%, navigation lights under 1% (limit 3%); lighting 2.8–3.7% (limit 10%) | Meets |
| 11.16.3.4 | Friction terminals up to 20 A | W05 and W06 on ring terminals | Meets |
| 11.16.4.1.1 | Protection from water | Fuse block and relays in the gasketed seat-module dry box, cable glands in its walls | Meets |
| 11.16.4.1.6 | Above normal bilge water | Floor run at Z 4 in conduit. Bilge pump pair on sealed DT plugs | Meets. **Check the pump float height** |
| 11.16.4.1.7 | 9 in from dry exhaust | Harness over 23 in away | Meets |
| 11.16.4.1.8 | Not buried in foam | No bench foam on the route any more. The forward runs pass over the footwell foam blocks, not through them | Meets |
| 11.16.4.1.9, .10 | Loom and support | UL 94 V-2 loom, clips every 18 in, lined metal clamps near the engine | Meets. Check on the build |
| 11.16.4.2.2.3 | Positive cable routed safely | Route clear of the engine | Meets. Check on the boat |
| 11.17.2.4 | Sender return | Isolated two-wire sender in a plastic tank | Meets |
| 11.18 | Grounding | No DC grounding system. Hull carries no current | Meets |
| AC sections | Not applicable | No AC system | N/A |

## Wires under 16 AWG

| Wire | Function | Exception |
|---|---|---|
| W46 | Exhaust probe | Thermocouple wire is its own type |
| W47–W49 | DESS | Under 1 A, electronic circuit, sled harness |
| W50 | Searchlight remote | Supplied, jacketed cable |

## Color check

| Guide color | Guide use | Wires | Status |
|---|---|---|---|
| Red | Positive main power | W01, W03, W05, W07, W09, W51, W53, W54, W57 | Follows |
| Black | Negative return | 20 returns | Follows. Black is used throughout, not yellow |
| Yellow with red | Starting switch to solenoid | W36 | Follows |
| Purple | Ignition and instrument feeds | W16, W27, W29, W31, W33, W35, W62 | Follows |
| Orange | Panel to accessory switches | W15, W22–W25 | Follows |
| Gray | Navigation lights, tachometer | W11, W13, W37, W41, W52, W58 | Follows, in dark gray |
| Brown | Pumps | W18, W20, W55 | Follows |
| Pink | Fuel gauge sender | W44 | Follows |
| Tan | Water temperature sender | W42 | Follows |
| Green | Bonding only | None used | Follows |
| Light blue | Oil pressure | None: the two-stroke has no oil pressure gauge | N/A |
| Dark blue | Cabin and instrument lights | None used | N/A |

**Wires with no color in the guide:**

| Wire | Function | Color | Why |
|---|---|---|---|
| W38 | LED bar switch to K3 | Gray/blue | Relay coil signal, from E-11's flood light marking |
| W39 | Searchlight switch to K4 | Gray/red | Relay coil signal |
| W40 | Engine pump signal to K5 | Brown/white | Relay coil signal, in the pump color family |
| W60 | Low oil signal | White/blue | No standard color. Label it, because W48 uses the same color |
| W46 | Exhaust probe | Type K | Thermocouple wire has its own colors |
| W47–W49 | DESS | White, white/blue, white/green | Matches the sled harness |

E-11 allows other schemes if they're documented, and these are all in the wire list and on the schematic. Red wires run beyond the first fuse for the LED bar, searchlight and outlet: those are power feeds, which the guide allows.

## Current editions, public summaries only

- **E-11 (2025):** adds fault-current ratings for overcurrent devices. Lithium is taken as 5,000 A per 100 Ah without a battery rating, which is about 600 A for a 12 Ah battery. Check that MF1's rating exceeds this.
- **E-13 (2025, lithium):** public sources give the threshold as 500 Wh or 600 Wh. The 12 Ah battery is about 150 Wh, so it falls under either figure. Use a battery with a built-in BMS, and follow the maker's instructions.
- **Bundling:** not confirmed for DC. The cage run is lightly loaded.

## Open items

1. Charging output, regulator and its fuse, and whether the regulator suits a lithium battery. Needs the service manual.
2. Fuel pump current and fuse F8. Needs the service manual.
3. Battery CCA and switch rating against the starter draw. Needs the service manual.
4. Pump float height against the conduit. Needs the boat.
5. W57 (battery to switch) shows in Onshape as separate segments, because its short bends inside the box fail the CAD union. Its length and weight are correct.
6. Buy the 2025 E-11 and E-13 if you want this checked against the current text.

Full findings are in `reports/ABYC_E11_Review_v1.0.1.3.md`; nothing in that review changes with the v1.0.1.3 relocation. The schematic PDF (`electrical/Minimal_1448_Electrical_Schematic.pdf`) is regenerated from `Schematic.html` at release.
