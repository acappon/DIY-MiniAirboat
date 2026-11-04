# Engine Selection for the Minimal 1448 — Requirements and Budget

**Date:** 2026-11-03. **Branch:** `main`. **Status:** research done; the lead candidate is the Rotax 800R E-TEC snowmobile two-stroke (Section 4a), awaiting the user's decision. The GSX-R1000 choice (Minimal_1448_Study.md, Bike_Engine_Stand_and_Reduction.md) does not meet these requirements.
**Model:** `CAD/analysis/minimal_1448.js` (the "Engine budget" section at the end).

## 1. Requirements (user, 2026-11-03)

| # | Requirement |
|---|---|
| E1 | Normal use: **driver + 150 lb of gear**, within the capacity plate. |
| E2 | Also: **driver + one passenger** (200 lb each), within the capacity plate. Not both passenger and gear at once. |
| E3 | Gets on plane easily in both cases. |
| E4 | Top speed **at least 33 mph** in both cases. |
| E5 | Lightweight and inexpensive: the original intent of the 1448 build. |

**Capacity plate:** the Lowe L1448 lists **825 lb** maximum (persons, motor and gear), 4 persons / 500 lb, 25 hp maximum, about 260 lb hull ([iboats L1448](https://boats.iboats.com/2016-lowe-boats-l1448-jon/1669710.html)). Newer L1448M listings show 831 lb. Use the plate on the boat you buy. The model uses 248 lb for the hull; at 260 lb the trim and speed results change only slightly.

## 2. Why the GSX-R1000 fails

At about 487 lb of equipment it leaves 338 lb under the plate. **Driver + passenger is 62 lb over; driver + 150 lb of gear is 12 lb over.** The study's "driver + 1" claim did not subtract the passenger correctly. It is also the heaviest option (262 lb engine system and stand) and carries about 140 hp on a hull plated for 25 hp.

## 3. The budget

The outfit without engine and prop is about 250 lb (stand 45, cage 45, UHMW strips 45, fuel 45, seat, rudders, controls, battery, lights, safety gear). E1/E2 need 400 lb of payload, so:

**Engine system ≤ about 160 lb** (825 − 250 − 15 prop − 400), counting the engine, reduction, radiator, exhaust and any stand weight over 45 lb.

Worst case of E1 and E2, from the model (top mph / thrust margin at the 12–16 mph hump, lbf):

| Engine system | Payload | 70 hp | 80 hp | 90 hp | 100 hp | 110 hp | 120 hp |
|---|---|---|---|---|---|---|---|
| 125 lb | 435 lb | 29 / 138 | 30.25 / 164 | 31.5 / 190 | 32.5 / 215 | 33.5 / 239 | 34.5 / 263 |
| 150 lb | 410 lb | 29 / 134 | 30.5 / 160 | 31.5 / 186 | 32.75 / 211 | 33.75 / 235 | 34.75 / 259 |
| 175 lb | 385 lb (fails E1/E2) | 29 / 130 | 30.5 / 156 | 31.75 / 182 | 32.75 / 207 | 33.75 / 231 | 34.75 / 255 |

- **Top speed depends almost only on power.** 33 mph needs about **105–110 hp delivered at the prop**, continuously.
- **Planing is not the problem:** every case keeps more than 100 lbf of thrust to spare at the hump. Even the 65 hp Rotax 582 planes easily; it just tops out at 28–30 mph.
- **Uncertainty:** the thrust rule (440 lbf at 100 hp, scaled by power^2/3) is ±25%, so the 105–110 hp figure is a planning number. A static thrust test settles it.

**Target: about 110 hp at the prop from an engine system of 160 lb or less.** That is about 0.7 hp per lb, which only aircraft and snowmobile two-strokes reach; four-stroke bike and car engines are too heavy for their power once a reduction and radiator are added.

## 4. Candidates

| Engine | Power used | Engine system (est.) | Meets E1–E4? | Price | Notes |
|---|---|---|---|---|---|
| Rotax 582 + gearbox | 65 hp | 103 lb | **No:** 28–30 mph | ~$4,000–5,500 used | Planes easily, most payload; too slow. |
| GSX-R1000 + belt | 140 hp | 206 lb + 11 lb stand | **No:** 62 lb over the plate | ~$2,000–4,800 | Current BOM choice. |
| Hirth 3701 + gearbox | 100 hp | ~140 lb (127 lb with gearbox, starter and exhaust, plus radiator) ([Wikipedia](https://en.wikipedia.org/wiki/Hirth_3701)) | **Borderline:** ~32.75 mph, 420 lb payload | ~$10,000 new (old estimate) | Aircraft engine with its own gearbox; fails E5 on price. |
| 600-class snowmobile two-stroke + belt | ~95–100 hp | ~164–171 lb | **No:** 32.25–33.25 mph and 389–396 lb payload | not found | See Section 4a. |
| **Rotax 800R E-TEC + belt** | ~120 hp | ~168 lb | **Yes on speed** (~35 mph); 8 lb short on payload | **~$1,100–2,500 used** (asking) | **Lead candidate.** See Section 4a. |
| Rotax 850 E-TEC + belt | ~135 hp | ~182 lb | **No:** 378 lb payload | not checked | Heavier for power the boat doesn't need. |

## 4a. Snowmobile two-strokes, researched 2026-11-03

**Bare-engine weights** (reported by [PowersportsGuide](https://powersportsguide.com/snowmobile-weight/); no source is cited and it doesn't say what's included, so weigh the real engine): 600 HO E-TEC 75 lb, 600R E-TEC 82 lb, **800R E-TEC 79 lb**, 850 E-TEC 93 lb. BRP publishes only whole-sled dry weights ([Ski-Doo MXZ](https://ski-doo.brp.com/us/en/models/previous-models/2022/mxz.html)). Polaris weights weren't found; the Patriot 850 is reported only about 6.5 lb heavier than the Liberty 800 ([Supertrax](https://www.supertraxmag.com/features/hypersled-engines-polaris-patriot-850/)).

**Installed estimate** = bare engine + exhaust and silencer 20 + radiator and coolant 15 + belt, pulleys, prop shaft and bearings 39 (as in the bike design) + oil tank, ECU and harness 10 + mounts 5 = **bare + 89 lb**. Power at the prop is taken at about 75–80% of the rated power, for continuous load and belt losses.

| Engine | System lb | hp at prop | Payload | Solo + 150 lb: mph / hump lbf | Driver + 1: mph / hump lbf | Dock trim |
|---|---|---|---|---|---|---|
| 600 HO E-TEC | 164 | 95 | 396 | 32.5 / 202 | 32.25 / 196 | 1.2–1.4° |
| 600R E-TEC | 171 | 100 | 389 | 33.25 / 213 | 32.75 / 207 | 1.2–1.5° |
| **800R E-TEC** | **168** | **120** | **392** | **35 / 262** | **34.75 / 256** | **1.2–1.4°** |
| 850 E-TEC | 182 | 135 | 378 | 36.5 / 294 | 36 / 288 | 1.3–1.5° |

- **The 800R E-TEC meets E3 and E4 with about 2 mph to spare,** and dock trim is now inside the 0.5–2° target (the GSX-R1000 was 2.4°). It is **8 lb short on E1/E2**. Close the gap by trimming the outfit: UHMW strips over the wear zones only (45 → about 35 lb), or a 5 gal tank instead of 6 (−7.5 lb).
- **Price:** complete used 800R E-TEC engines list at about $1,100–2,500 (asking prices on [eBay](https://www.ebay.com/shop/skidoo-800-etec-engine?_nkw=skidoo+800+etec+engine)); a whole running 2013 MXZ TNT 800R listed at $2,900 ([Snowmobile Trader](https://www.snowmobiletrader.com/Used-Ski-Doo-Engines/snowmobiles-for-sale?make=Ski-Doo%7C326747&category=Engines&condition=U)). That is about the same as the GSX-R1000 budget ($2,000).
- **Buying a whole running sled may be the better deal:** it comes with the ECU, harness, DESS key, oil tank, exhaust, gauges and a known-running engine, and the rest of the sled can be resold.
- **E-TEC electronics:** the engine needs its own ECU, harness and the matching **DESS key** to run. Buy an engine with all three, or a whole sled. Older carbureted 800s are simpler to wire but weren't researched.
- **Starting:** choose one with electric start or BRP's SHOT start, or plan on a pull start.

**What changes from the bike design:** a single belt stage of about 3:1 (engine peak about 8,000 rpm to about 2,600 at the prop), against 1.57:1 behind the bike gearbox. Belt torque is lower (about 80 lb-ft at the engine against 180 lb-ft at the bike countershaft), so the belt is narrower, but the large pulley grows to about 15 in. The bike gearbox lock, the clutch lever and the clutch start switch are dropped.

## 5. Things that change with a snowmobile two-stroke

- **Reduction:** about 7,500–8,000 rpm engine to about 2,600 prop rpm, roughly 2.9–3.1:1. The 14MGT belt and straddle-mounted shaft design carries over; the bike-gearbox lock and clutch lever do not. A Rotax C or E gearbox is an alternative where the case allows it.
- **No clutch:** the prop turns whenever the engine runs (normal for airboats). The clutch lever (BOM line 47) goes.
- **Fuel:** two-strokes burn roughly 0.6–0.7 lb/hp/h, about 10–12 gal/h at full power. A 6 gal tank is under an hour of fast running; check E1's 150 lb of gear against a bigger tank.
- **Noise:** a tuned-pipe two-stroke is loud. Budget a good muffler; the California shoreline limit is out of reach for either engine type.
- **Engine life:** snowmobile two-strokes are rebuilt far more often than car engines; price the top-end kit.

## 6. Other levers if no engine fits

- **Lighter outfit:** the 45 lb of UHMW strips and the 45 lb cage are the biggest items. Saving 30–50 lb there widens the engine budget to about 190–210 lb.
- **Relax E4 to about 30 mph:** the Rotax 582 then meets everything else with 460 lb of payload.

## 7. Next steps

1. ~~Weights and prices for 600- and 800-class engines~~: done (Section 4a); the weights are still unverified.
2. **Decide:** the Rotax 800R E-TEC, plus an outfit trim of at least 8 lb.
3. Then redesign the reduction for about 3:1, check the prop rating at about 120 hp, and update the stand, BOM (engine, reduction, clutch line, fuel tank) and the model's engine entry.
4. Weigh the real engine with its exhaust when it's bought.
