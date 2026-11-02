# CAD Connection Review — 1652 branch

**Date:** 2026-11-02. **Scope:** every Part Studio in the Onshape "1652 hull" branch (drivetrain, cage and rudders, drivetrain mounts, hull) plus the joints between studios.
**Method:** CAD checks of every pair of parts:
- **Overlap:** solids that share volume. This is impossible to build.
- **Contact:** touching faces with nothing modeled that actually holds them.
- **Gap:** a part with nothing under it.

Joints between studios were checked against the hull inside surface and the rear bench. Fit-only items, such as bolts in clearance holes and belts on pulleys, are left out.

## 1. Findings that change the design

| # | Where | What the CAD shows | Fix (DIY, bolted) | Weight effect |
|---|---|---|---|---|
| 1 | **Rear bench vs drivetrain** | The factory rear bench (X 12–32) overlaps everything below:<br>• engine block (330 in³)<br>• flywheel and flex coupling<br>• redrive housings<br>• all four A-frame legs<br>• both rear engine mounts<br>• both exhaust hangers<br>• both cage support posts<br>The v1.0.0.2 / 1448 design had the same conflict. Build Guide 2.5 only covered leg holes. | **Remove the rear bench center** between the hull sides, as already done for the middle bench. Keep or replace its outboard ends as foam-filled side boxes (Y about 17–34). They double as the stern flotation blocks already planned (R5.7) and as side steps. The cage supports, cage and transom knees carry the stern side-to-side loads, so no thwart is needed there. | Hull gets lighter by the bench's aluminum (about 10–15 lb). The hull is not counted in the plate math, so the gear allowance doesn't change. The bench's factory foam must be replaced (already in the flotation plan). |
| 2 | **Engine mounts float over the beds** | The four rubber engine mounts (rev7) sit at Z 6.37, but the bed top is at 6.125. Their footprint (Y 7.6–16.3) overhangs the bed inboard by about 4.4 in. The forward pair also overlaps the brace-foot plate (X 44–45.25). | Add **four engine-mount plates**, 1/4 in 6061, about 6.5 × 9.5 in. Each bridges from the bed flange inboard to Y 7.5, with 1/2-13 rivnuts or captive nuts for the mount studs. Merge the forward pair with the brace-foot plates into one plate per side. Bolt them to the bed tops with 3/8 in rivnuts (see #4). | **+6.4 lb** (plates and bolts) |
| 3 | **Exhaust hangers float** | Both hanger brackets end at Z 6.37, outboard of the starboard bed (Y −13.8 to −23.3). There's nothing under them except the hull floor, 6 in below. | Stand each hanger on a 1 × 1 × 1/8 in aluminum angle post with a foot clip, bolted to the floor through a doubler pad (#10 hardware and sealant, like the hull armor). Keep at least 2 in from the fuel line. | **+1.0 lb** |
| 4 | **Captive nuts inside the engine beds** | The 3/8-16 captive nuts under the A-frame and brace mount plates sit inside the 2 × 6 in bed tube (0.04 in³ overlap with the bed top). A nut can't be welded there, because the tube is closed. | Drill through the plate and bed top, then set **3/8-16 steel rivnuts (plus-nuts)** in the bed top wall. Same for the new engine-mount plates. DIY with a rivnut tool. | about 0 |
| 5 | **Cage support post** | The 2 × 2 angle post runs 0.15 in³ through its top plate and 0.23 in³ through the knee. The post bottom also misses its bottom foot by 0.14 in, so the foot touches nothing. | Cut the post top to the plate angle (about 29°, already noted in v1.0.0.2). Butt the knee against the post with a 2 × 2 × 1/4 clip and two 5/16 bolts. Extend the foot 0.15 in outboard to the hull side so the post lands on it. Model all three. | **+0.5 lb** |

## 2. Joints that are drawn touching but have no hardware yet

Each needs the connection added to the CAD and the BOM.

| # | Joint | Recommended connection | Hardware weight |
|---|---|---|---|
| 6 | A-frame leg ↔ peak plate (×4) | Clevis: two 3/8-16 Gr 5 bolts through the slotted leg and the plate, with aluminum crush sleeves inside the tube | 0.8 lb |
| 7 | A-frame foot clips ↔ legs (×8) | Two 5/16 bolts per clip, through the leg (with crush sleeves) | 0.8 lb |
| 8 | A-frame crossbars ↔ legs (×4 ends) | 2 × 2 × 1/4 angle clip at each end, with the crossbar end cut at about 4° to match the leg lean (CAD shows 0.008 in³ overlap). Two 5/16 bolts each way. | 1.8 lb |
| 9 | A-frame braces (×2) | Top: the tube currently runs 0.09 in³ into its bracket, so shorten it and bolt through with one 3/8 bolt and a crush sleeve. Bottom: the tube stops about 1 in above its shoe, so extend it to the shoe and through-bolt. | 0.7 lb |
| 10 | Jack-screw block ↔ aft peak plate | Welded to steel in rev7. On aluminum: two 5/16 bolts into tapped holes in the block. | 0.1 lb |
| 11 | Radiator cradle rails ↔ peak plates (×4) | Welded to steel in rev7. On aluminum: a 1-1/2 × 1-1/2 × 1/8 clip and two 1/4 bolts at each end. | 1.2 lb |
| 12 | Tailpipe saddle ↔ transom | Rev7 had it sitting on the 1448's 15 in transom top. On the 1652 it sits against the inside of the notched transom: two 5/16 bolts through the transom, with sealant. | 0.1 lb |
| 13 | Cage support post ↔ hull side; knee clip ↔ transom; bottom foot ↔ floor | Through-bolt with #10 or 1/4-20 hardware and marine sealant, like the hull armor. | (in mounts BOM) |
| 14 | Engine beds and doublers ↔ hull floor | Already bolted in the design (#10 through the bed flanges every 6 in). OK. | — |

Items 6–13 come to about 5.5 lb. The aluminum A-frame estimate already carried about 4 lb of bolts, so the net is about **+1.5 lb**.

## 3. Prop cage and steering (rev7 parts)

| # | Finding | Comment |
|---|---|---|
| 15 | The rebuilt cage base tubes cross the hoops, pads and each other at their joints (0.003–0.07 in³). | These are welded tube nodes; the CAD just doesn't show the copes. On the real cage, cope and weld. |
| 16 | **The cage is steel EMT (galvanized).** | Welding galvanized EMT gives off zinc fumes (metal-fume fever). For a welded cage, use **plain black steel or DOM tube** of the same sizes; it is about the same weight and welds cleanly. Or grind the zinc back about 1 in at every joint and weld outdoors with a respirator. A bolted cage would need about 40 clamp fittings (about +8–10 lb) and would be less stiff, so it's not recommended for a safety-critical guard. |
| 17 | Steering pins, rod ends, bellcrank bolt and horn pins pass through solid parts (0.03–0.16 in³ each). | The holes simply aren't modeled. Drill to suit; no design change. |
| 18 | The drum mesh crosses the tubes slightly. | The mesh is zip-tied or wired over the frame. No change. |

## 4. Welding vs bolting

- **Aluminum (A-frame, mounts, plates):** welding is *not* lighter in practice. TIG welding 6061-T6 drops the metal next to the weld to about 60% of its strength. So a welded A-frame needs thicker members or gussets that cost about what the clips and bolts weigh (about 4–5 lb). It's also a shop job (TIG). Bolted stays DIY, can be inspected and repaired, and handles vibration better. **Recommendation: keep the aluminum structure bolted.** The engine beds stay shop-welded as already planned (Build Guide 2.2/2.3).
- **Steel cage:** welding *is* the lighter and stiffer choice (see #16). It's a MIG job that a confident DIY welder can do; otherwise it goes to a shop. **Recommendation: welded black-steel cage.**
- **Anything into the hull skin:** bolted with sealant (never welded), as already decided.

## 5. Weight summary (driver gear allowance)

| Change | Weight |
|---|---|
| Engine-mount plates (#2) | +6.4 lb |
| Exhaust hanger stands (#3) | +1.0 lb |
| Cage support post joints (#5) | +0.5 lb |
| Bolted joint hardware beyond the A-frame allowance (#6–#12) | +1.5 lb |
| **Total** | **+9.4 lb**, so the gear allowance goes from **~52 lb to ~43 lb** |
| Rear bench center removed (#1) | −10 to −15 lb of hull (better for performance; no effect on the plate math) |

## 6. Decisions needed
1. Remove the rear bench center and replace its ends with foam side boxes? (Recommended.)
2. Welded black-steel cage instead of galvanized EMT? (Recommended.)
3. OK to add the engine-mount plates, hanger stands and joint hardware to the CAD and BOM? This brings the gear allowance to about 43 lb.
