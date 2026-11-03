# Versioning and releases

`VERSION` holds one line: the **next** release (for example `1.0.0.3`). It works like a `version.h`: everything in progress carries that number, so the newest files are always easy to spot, and it is bumped right after a release is declared done.

## Files that carry the version
| File | In development | At release |
|---|---|---|
| `Airboat Build Tracker v<VERSION>.xlsx` | Read Me A1: "v<VERSION> (in development, not yet released)" | "release v<VERSION> (date)" |
| `reports/Release_Notes_v<VERSION>.md` | In-development banner, release date TBD | Banner removed, date filled in |
| `CAD/Airboat_v<VERSION>_*.step` | Not made yet | Exported from the Onshape version v<VERSION> |
| Onshape version `v<VERSION>` | Not made yet | Created in the document |
| Built boat (hardware) | — | Write the version it was built to on the build-plate label and in the build log, so a part or a boat can be traced back to its drawings |

`node tools/set_version.js` stamps the tracker and release notes from `VERSION` (run `npm install` in `tools/` once).

## Release checklist
1. Finish the "Still to do before release" list in the release notes.
2. In Onshape, create version `v<VERSION>`, export the STEP files as `CAD/Airboat_v<VERSION>_*.step`, and re-render the images.
3. Refresh the totals and package contents in the release notes.
4. `node tools/set_version.js --release YYYY-MM-DD`. This dates the docs and removes the older trackers and STEP files from the package (they stay in git history).
5. Commit, tag `v<VERSION>`, push.
6. **Right after the release is declared done:** bump `VERSION` (for example to `1.0.0.4`), run `node tools/set_version.js`, and commit. The new tracker and release notes are created for the next version.
