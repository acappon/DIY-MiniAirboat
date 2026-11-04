# Engine Selection for the Minimal 1448 — Requirements and Budget

**Date:** 2026-11-03. **Branch:** `main`. **Status:** research in progress; the engine decision is reopened. The GSX-R1000 choice (Minimal_1448_Study.md, Bike_Engine_Stand_and_Reduction.md) does not meet these requirements.
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
| **600-class snowmobile two-stroke** (Rotax 600 E-TEC, Polaris 600) + belt | ~100–110 hp (110–120 hp rated) | **~130–150 lb: unverified** | **Borderline to yes** | used engines likely ~$1,000–2,500: **unverified** | Built for long full-throttle runs. Airboat builders report 110 hp 600s on belt or Rotax C/E gearbox reductions ([Southern Airboat](https://www.southernairboat.com/community/threads/using-a-115hp-snowmobile-engine.46522/)). |
| **800-class snowmobile two-stroke** (Rotax 800/850 E-TEC, Polaris 800) + belt | ~120–130 hp (150–165 hp rated) | **~145–165 lb: unverified** | **Likely yes:** ~34–35 mph if under 160 lb | used, **unverified** | Most margin on speed; weight is the question. |

Bare-engine weights for snowmobile engines are not published the way aircraft engine weights are; BRP lists only whole-sled dry weights. **The first job is to weigh, or find a documented weight for, a bare 600 and 800 engine with its stock exhaust.**

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

1. Get documented weights and used prices for bare 600- and 800-class snowmobile two-strokes, with exhaust and an EFI or carburetor setup, from salvage listings or builder data.
2. Check E1–E4 in the model for each candidate with real weights.
3. Decide the engine, then update the stand, reduction and BOM (engine, reduction, clutch line, fuel tank size).
