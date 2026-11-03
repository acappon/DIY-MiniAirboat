# Letter to the next session: DIY Jon-Air airboat after v1.0.0.3

Written 2026-11-03 at the release of v1.0.0.3, by the Claude session that did the 1652-hull work, for whoever picks the project up next. Read this first, then `RELEASING.md` and the v1.0.0.3 release notes.

## Where everything is

- **Onshape document** "DIY Jon-Air boat" `7328492526b52c17df5ca2ec`. Work happens in branch workspace **"1652 hull"** `61b41121ca546ea0c61ea502`. Version **v1.0.0.3** = `c3dbde8a38d481144f69b895`.
- **Element ids (1652 workspace):** Assembly 1 `b27cd2d2c2d5ca5a35fa962d`; Hull 1652 `a032a2c9e1952c6991743bdb`; Drivetrain mounts `63da2a9e6161eaa30c4b3d4b`; Drivetrain and A-frame `00482d275e642cd558c6fb9b`; Cage `8fe0e5d06e1c3538b720647f`; Seating `4d8642a20e8757a837812ea2`; Console `bc12e9996ca542f63a7cf717`; Controls `3b490121cdebd22efe85ab3c`; Crew mannequins `1453bc15c6f04a206608549c`; old 1448 hull and armor `c42c16559a466793fdab6d3c` (suppressed in the assembly).
- **Feature Studios:** Controls `4056e6fb87dd3fef7f975999`; Motion sets `739de44b1c998370a3c55b0b`; Seating `a2b926cbc3b1e141aa671b02`; Console `6e0074139dc39d916408f99e`; Jon hull `55e2a58fd2b0ba0dbe7d2cea`; Mounts `5f162b21d862cfd00545bb4d`; A-frame `9d52273c0c699ea0fa858fbe`; Crew `8d7b77ef8a8adbe03e5c3562`.
- **Repo:** `C:\Users\acappon1\Documents\DIY-MiniAirboat` (GitHub acappon/DIY-MiniAirboat), branch `rev8/1652-hull`, project folder `rev8_JonAirBoat`. FeatureScript sources live in `CAD/FeatureScript` and are the master copies: edit there, then upload.

## How the user likes to work

- **DIY rule:** everything must be buildable by a backyard mechanic. Aluminum is bolted (rivnuts), never welded, except shop-welded bed flanges. The user MIG-welds steel (the prop cage, the stick).
- **Keep the paperwork current:** the build tracker xlsx (BOM, Build Guide, Read Me) and the release notes for the version in `VERSION`. Back the tracker up before editing it, and never touch the pledge or completion columns. To save usage, batch these updates once per finished feature rather than every turn.
- **Weight:** whenever a change adds weight, tell the user how much gear the driver can still bring. About **81 lb** as of v1.0.0.3 (280 lb payload budget).
- **Onshape API budget:** a hard limit of **2,500 calls per year**. About **904** were used by the end of 2026-11-03. Batch calls, check with one eval per studio, and ask before call-heavy jobs.
- **Claude usage:** the user noticed this design burning usage fast. Prefer one session per design piece, keep turns lean, and don't re-read big files that are already in context.
- **Commit and push are authorized** on `rev8/1652-hull`. Releases follow `RELEASING.md`. Bump `VERSION` to the next number right after a release.
- **Wiring waits:** no electrical design until the user says the seating and console are finished.

## Design state at v1.0.0.3

- **Hull:** parametric 1652-class placeholder (the real hull is not bought yet). Inside floor at Z 0; X runs forward from the transom; Y is positive to port.
- **Equipment:**
  - Drivetrain: Suzuki G13, belt drive, 3-blade prop. Prototype B (G13B, 4-blade) comes later.
  - Bolted aluminum A-frame; welded black-steel prop cage with twin rudders.
  - Group 31 LiFePO4 battery.
- **Seating and console:** tandem layout. The driver's bucket seat sits on tracks over the battery dry box. Passengers sit on a padded storage box with a backrest and grab bar. The console is to starboard of the driver.
- **Controls:**
  - Port-side steering stick driving a 43C push-pull cable to the bellcrank.
  - Heel-hinged throttle pedal driving a 33C cable. The pedal was redesigned this release: 30 deg up at idle, 25 deg stroke, three cable holes giving 1.3 to 1.7 in of travel. The user picked this over a T-H Marine Hot Foot (about $250-410 and 8 lb, against $70-90 and 1.5 lb).
- **Motion:** every moving part is mated in Assembly 1, with travel limits. Details are in the next section.

## Open items, roughly in priority order

1. **Electrical design** (schematic, wire list, every wire in CAD with color, AWG and connectors). It waits on the user saying seating and console are done.
2. **Measure the G13 throttle travel** to choose the pedal arm hole. The engine-end throttle position in the CAD is a placeholder.
3. **Confirm the 43C cable's rated load** against the roughly 160 lb hard-rudder link force.
4. **Not yet modeled:** fuel tank (6.5 gal, forward) and the starboard turnbuckle trim tab.
5. **Hull:** choose the real hull and replace the placeholder dimensions with measurements.
6. **Re-render** build steps 2.1 to 2.15 on the 1652 hull.
7. **Retired pushrod:** the rev7 steering pushrod still exists in the cage studio's default configuration. Delete it there.
8. **Weights:** replace the estimated weights with a full CAD weigh-in.

## Hard-won technical lessons

- **Motion sets** (`Motion_sets.fs`): each moving studio has an integer configuration input `set`.
  - Set 0 is everything, 1 is the fixed parts, and 2 and up are one moving group each.
  - Parts are picked by **bounding-box centre**, because part names can't be read during regeneration. If you move or resize a moving part, update its centre in `Motion_sets.fs`.
  - On failure, the feature renames a part "MOTION SETS ERROR ...". The eval endpoint ignores configuration, but `GET /parts?configuration=set=N` honours it.
- **Never export a studio with `set=1`.** That is the fixed parts only; it silently drops the rudders and other moving parts. Export with the default configuration.
- **Mates with limits refuse `set_mate_values`.** Test motion first, then add limits, or drag the mates in the Onshape UI.
- **Relation sign conventions:**
  - Gear relation: by default the second mate turns opposite; `reverse` makes it turn the same way.
  - Prop: 0.4 with `reverse` on. Idler: 0.783 with `reverse` off.
  - Rack-and-pinion: distance = 2 x pi x arm radius. The pedal is 21.991 in per revolution; both pedal and bellcrank have `reverse` on.
- **FeatureScript gotchas:**
  - Ids must not contain spaces or dots.
  - Don't reuse loop variable names at the same level.
  - Enum parameters can carry an older Feature Studio microversion's type, so compare them with `toString`.
- **Feature Studio uploads replace the whole file and echo it back,** which is expensive in tokens. Keep feature files small; consider splitting Controls into stick, pedal and cables.
- **Whole-studio inserts:** new parts appear automatically. After a regen that changes partIds, delete the dangling instances in one `/modify` call.
- **Toggling the crew:** use one `/modify` call with `suppressInstances` or `unsuppressInstances`. The 42 crew instance ids come from the assembly GET (elementId = crew studio).
- **Renders:** studio views are 1600x1000 shaded views of Assembly 1. The view matrices are in `tools/render_views.json`. `tools/compose_renders.js` (needs `sharp`) builds the studio and scene images from the transparent PNGs. Write renders to a short temp path first, because Windows long paths break sharp.
- **Large GET responses** are saved to a file by the Onshape tool. Parse them with node; don't read them into the conversation.
