# Minimal 1448 Airboat — Concept Study

**Date:** 2026-11-03. **Branch:** `study/minimal-1448` (from `main`, the 1448 version v1.0.0.2). **Status:** concept study; every outfit weight is an estimate until parts are weighed.
**Decision (2026-11-03): build around a 1000 cc sport-bike engine** (for example a Suzuki GSX-R1000) with its gearbox and a belt reduction: cheaper than the Rotax 582 at US prices, about 38 mph with any load, and about 375 lb of payload under the plate.
**Model:** `CAD/analysis/minimal_1448.js` (1448 hull hydrostatics and Savitsky planing, the same method as the rev8 analyses).

## 1. Why this study

Every earlier variation (1448, 1652 and 1860 hulls; Suzuki G13 and G13B, a turbo G13B, a Toyota 2GR V6) kept the same outfit: a car engine with a belt redrive and radiator, a bolted A-frame, a full cage, full hull armor, and a cockpit with a storage-box thwart, console and battery dry box. On the 1652 that outfit weighed about 973 lb:

| Group | lb |
|---|---|
| Car engine, redrive, radiator, exhaust | 285 |
| Cockpit: storage box/thwart, grab bar, rails, console, dry box, battery, seat | 170 |
| A-frame, engine mounts, cage supports | 155 |
| Cage, rudders, prop, shaft | 145 |
| Hull armor (UHMW, chine angles, bow skin) | 110 |
| Controls, lights, trim tab, safety gear | 61 |
| Fuel | 47 |

So the hull was never the problem: the outfit was. A cheap, light, one-owner airboat needs a lighter outfit, not a different hull.

## 2. The minimal outfit (solo first, one passenger allowed)

- **Keep the 1448's factory benches.** They are structure (no new thwart needed), flotation, and the passenger seat.
- **Light engine and stand:** an engine with its own reduction (no car redrive, no A-frame), on a bolted aluminum stand on two short beds across the ribs.
- **Smaller cage** in thin-wall aluminum tube with a mesh guard; twin rudders.
- **Simple cockpit:** a raised driver seat on a frame, stick and rudder linkage, throttle cable and lever, a small lithium battery for electric start, kill switch, navigation lights. No console, dash, storage box or dry box.
- **Thin armor:** 3/16 in UHMW bottom strips only (no chine angles or bow skin).

| Outfit item | lb (estimate) |
|---|---|
| Engine stand and short beds | 45 |
| Prop cage with mesh | 45 |
| UHMW strips 3/16 in and fasteners | 45 |
| Fuel, 6 gal portable tank, full | 45 |
| Raised seat on a frame | 15 |
| Safety gear | 15 |
| Rudders and linkage | 12 |
| Stick, linkage, throttle | 10 |
| Small lithium battery and switches | 10 |
| Lights, small trim tab | 8 |
| **Outfit without engine and prop** | **250** |

## 3. Two light engines

| | Rotax 582 (two-stroke, ultralight aircraft) | 1000 cc sport-bike engine (for example a GSX-R1000) |
|---|---|---|
| Power used | 65 hp | about 150 hp (derated for continuous airboat load) |
| Weight with reduction, radiator, exhaust | **about 103 lb** with the B or C gearbox and electric start (about 90–95 lb without the starter) | about 180 lb: about 130 lb engine with its gearbox, plus a belt reduction, radiator and exhaust |
| Reduction | Its own gearbox (2.58–3:1), prop mounts on it | The bike gearbox (fixed gear) plus a belt reduction to get the prop down to about 2,300 rpm |
| Used price (US) | **about $4,000–5,500 with a gearbox** in US listings; remanufactured about $6,500; new about $6,500–8,200. UK listings are much lower (about £950–1,900), but shipping and import fees close much of the gap | complete running engines list from about $2,500 to $4,800; less from a crashed bike |
| Where to find | Ultralight aircraft sales and owner forums | Motorcycle salvage, crashed-bike auctions, eBay |
| Watch out for | Rebuild interval (TBO) **300 h or 5 years**; oil mixed with the fuel; loud; aircraft-part prices | About 10,000 rpm engine at sustained full load; no clutch (starts in a fixed gear); airboat use not proven at this size |

## 4. Results (1448 plate 825 lb, full fuel)

| | Rotax 582 | Bike engine |
|---|---|---|
| Equipment | **365 lb** | 450 lb |
| **Payload under the plate** | **460 lb** (a 200 lb driver + 260 lb, or driver + 1 + 60 lb) | **375 lb** (driver + 175 lb, or driver + 1) |
| Static thrust (estimate) | about 330 lbf | about 580 lbf |
| Top speed, solo / + 60 lb gear / + 150 lb gear | 29.75 / 28.5 / 27.5 mph | 38.75 / 37.5 / 36.5 mph |
| Top speed, driver + 1 | 28 mph | 37.25 mph |
| Thrust margin at the hump, solo / driver + 1 | 151 / 126 lbf | 348 / 321 lbf |
| Dock trim, solo (target 0.5–2°) | 1.4° | 2.1° |
| Transom freeboard at the dock | 9.7–10.7 in | 8.6–9.7 in |
| Running trim at top speed (target 2–4.5°) | 1.5–1.9° | 1.0–1.3° |

For comparison: the rev8 1652 design leaves about 80 lb of driver gear, and the 1860 with the Toyota 2GR about 137 lb.

**What this means:**
1. **Payload is three to five times what the full outfit allowed on any hull,** and cargo barely changes speed: 150 lb of gear costs the Rotax boat about 2 mph.
2. **The Rotax 582 is the best match for "small and light":** about 30 mph solo and 28 mph with a passenger, the most payload, and the simplest drivetrain (no redrive to build). The price is engine life, noise and cost: in the US a used 582 costs more than a crashed-bike engine.
3. **The bike engine is the performance option:** about 38 mph with any load, still about 375 lb of payload, but it needs a belt reduction and more cage and stand.
4. **Stow gear amidships, not in the bow.** 150 lb in the bow takes the dock trim to about level (Rotax). The light engine moves the balance forward compared with the car engine.
5. **Running trim is still flat** (1.0–1.9°): the prop axis is still high (about 53 in in the model). A shorter stand or a few degrees of down-tilt should be checked once a prop size is chosen.

## 5. Rough cost (starting from `main`'s v1.0.0.2 BOM, about $10,300)

| Change to the v1.0.0.2 BOM | Rotax 582 | Bike engine |
|---|---|---|
| Remove the G13 engine, redrive, 64 in prop, radiator, exhaust | −$3,720 | −$3,720 |
| Add the engine system and prop | +$5,000 (used 582 with gearbox about $4,500, 2-blade prop) | +$3,750 (engine, DIY belt reduction, radiator and exhaust, 3-blade prop) |
| Remove chine angles, bow skin and their hardware | −$350 | −$350 |
| Engine stand and short beds instead of the full engine beds | −$260 | −$260 |
| Add the items v1.0.0.2 did not list yet: cage, rudders, seat frame, lights | +$1,060 | +$1,060 |
| Smaller battery | −$50 | −$50 |
| **Estimated total** | **about $12,000** | **about $10,750** |

The hull is $4,000 of that (the v1.0.0.2 placeholder). A cheaper used 1448 brings the Rotax build to roughly $10,000–11,000 and the bike-engine build to roughly $9,000–10,000. **The v1.0.0.4 build tracker BOM for the bike engine comes to about $11,100:** it adds an inline EFI fuel pump (about $80) and keeps the v1.0.0.2 engine-bed lines until the engine stand is designed. These are planning numbers, not quotes; the v1.0.0.2 BOM also has no cage or rudder lines yet, so the "full outfit" designs cost more than their BOMs showed.

## 6. What you give up

- Real passenger seating and the storage box: a passenger sits on the factory middle bench.
- The dash and convenience features of the rev8 design (gauges, switch panel, outlets, inclinometer, console).
- Some armor protection (no chine angles or bow skin).
- Range: a small two-stroke burns fuel fast for its size, so 6 gal is a short outing; measure the burn rate early.

## 7. Open items

- **Capacity plate:** the 1448's maximum horsepower (25 hp) is an outboard rating; both engines exceed it. Check registration and insurance.
- **Weigh-in:** the outfit weights are estimates; weigh the real parts as they are chosen.
- **Prop:** pick a prop for the 582 (thin-chord blades suit it) or the bike engine, then redo the thrust, cage size and running trim.
- **Thrust test:** the static test protocol on `main` applies; the thrust figures above are rule-of-thumb estimates (±25%).
- **Decision made:** the bike engine (2026-11-03). Still open: which rev8 features, if any, come back; the specific engine (year and model) and its reduction ratio.

## Sources

- Rotax 582 weights by configuration and dry weight ([ultralightnews.ca](https://ultralightnews.ca/rotax582/rotax582specs.htm), [Wikipedia](https://en.wikipedia.org/wiki/Rotax_582)); new price ([Controller](https://www.controller.com/listings/for-sale/rotax/engines/30120), [Motive Aero](https://motive.aero/engine-details/rotax-582ul/317)); US used prices ([Craigslist Dallas, $4,000](https://www.craigslist.org/view/d/dallas-rotax/xj7TFw812d8Wp1NnRYywJV), [eBay 582 + 2.58 gearbox, $5,000](https://www.ebay.com/itm/196630422811), [eBay listings](https://www.ebay.com/shop/rotax-582-engine?_nkw=rotax+582+engine), [Karting Classifieds](https://kartingclassifieds.com/rotax-582-engine-2/)); UK used prices for comparison ([AFORS 582 engine, £950](https://afors.uk/engines+propellors/for-sale/standardView-64571), [AFORS 582 + E gearbox, £1,800](https://afors.uk/engines+propellors/for-sale/standardView-17896/Rotax-582-+-E-Gearbox---run-4hrs-since-rebuild)); TBO 300 h or 5 years and owner experience ([rotax-owner.com](https://www.rotax-owner.com/en/2-stroke-technical-questions/8579-582-reliability-statistics)); thin-chord props for a 582 airboat ([Southern Airboat forum](https://www.southernairboat.com/community/threads/help-needed.24618/)).
- GSX-R1000 engine weight about 130 lb ([suzukicycles.org](https://www.suzukicycles.org/GSX-R-series/GSX-R1000a.shtml)); used engine listings ([eBay](https://www.ebay.com/p/19002612287), [for-sale.co.uk](https://www.for-sale.co.uk/gsxr-1000-engine)); reduction ratios and full-load cautions for high-rpm engines ([Southern Airboat forum](https://www.southernairboat.com/community/threads/belt-or-dd.27539/)).
- 1448 hull data and the v1.0.0.2 BOM: `main` (Release_Notes_v1.0.0.2.md, the v1.0.0.2 build tracker).
