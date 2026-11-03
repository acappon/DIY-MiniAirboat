// Stamps the release docs from the one-line VERSION file (like a version.h).
// VERSION always holds the NEXT release. Bump it right after a release is declared done,
// then run this script so every doc in progress shows the new version.
//
//   node tools/set_version.js                    in development: stamp the docs for VERSION
//   node tools/set_version.js --release 2026-11-20   declare VERSION released on that date
//
// In development it:
//   - creates "Airboat Build Tracker v<VERSION>.xlsx" from the newest older tracker if missing
//     (the older one is a release and stays untouched), and sets its Read Me A1 to
//     "... v<VERSION> (in development, not yet released)"
//   - creates reports/Release_Notes_v<VERSION>.md from a stub if missing
// With --release it:
//   - sets Read Me A1 to "... release v<VERSION> (<date>)", puts the date in the release
//     notes and removes their in-development banner
//   - git-removes the older-version trackers and STEP exports from the package (they remain in
//     git history); older release notes stay in reports/
// Needs exceljs: run "npm install" in tools/ once.
const fs = require('fs');
const path = require('path');
const { execFileSync } = require('child_process');
const ExcelJS = require('exceljs');

const ROOT = path.resolve(__dirname, '..');
const V = fs.readFileSync(path.join(ROOT, 'VERSION'), 'utf8').trim();
if (!/^\d+\.\d+\.\d+\.\d+$/.test(V)) throw new Error('VERSION must look like 1.0.0.3, got "' + V + '"');
const ri = process.argv.indexOf('--release');
const RELEASE = ri > 0 ? process.argv[ri + 1] : null;
if (ri > 0 && !/^\d{4}-\d{2}-\d{2}$/.test(RELEASE || '')) throw new Error('--release needs a date, YYYY-MM-DD');

const cmp = (a, b) => { const x = a.split('.').map(Number), y = b.split('.').map(Number); for (let i = 0; i < 4; i++) if (x[i] !== y[i]) return x[i] - y[i]; return 0; };
const verOf = (f, re) => { const m = f.match(re); return m ? m[1] : null; };
const TRACKER_RE = /^Airboat Build Tracker v(\d+\.\d+\.\d+\.\d+)\.xlsx$/;
const STEP_RE = /^Airboat_v(\d+\.\d+\.\d+\.\d+)_.*\.step$/;
const BANNER = '> **In development: not yet released.**';
const done = [];

(async () => {
  // Tracker
  const trackers = fs.readdirSync(ROOT).filter(f => TRACKER_RE.test(f)).sort((a, b) => cmp(verOf(a, TRACKER_RE), verOf(b, TRACKER_RE)));
  const tFile = path.join(ROOT, 'Airboat Build Tracker v' + V + '.xlsx');
  if (!fs.existsSync(tFile)) {
    const older = trackers.filter(f => cmp(verOf(f, TRACKER_RE), V) < 0).pop();
    if (!older) throw new Error('no older tracker to start v' + V + ' from');
    fs.copyFileSync(path.join(ROOT, older), tFile);
    done.push('created ' + path.basename(tFile) + ' from ' + older);
  }
  const wb = new ExcelJS.Workbook();
  await wb.xlsx.readFile(tFile);
  wb.getWorksheet('Read Me').getCell('A1').value = RELEASE
    ? 'Airboat Build Tracker - release v' + V + ' (' + RELEASE + ')'
    : 'Airboat Build Tracker - v' + V + ' (in development, not yet released)';
  wb.calcProperties.fullCalcOnLoad = true;
  await wb.xlsx.writeFile(tFile);
  done.push('stamped ' + path.basename(tFile) + (RELEASE ? ' as released ' + RELEASE : ' as in development'));

  // Release notes
  const nFile = path.join(ROOT, 'reports', 'Release_Notes_v' + V + '.md');
  if (!fs.existsSync(nFile)) {
    fs.writeFileSync(nFile, '# DIY Jon-Air Airboat — Release Notes v' + V + '\n\n' + BANNER + ' Updated as the work proceeds.\n\n' +
      '**Release date:** TBD\n**CAD source:** Onshape document "DIY Jon-Air boat", version **v' + V + '** (to be created at release)\n\n' +
      '## What changed since the previous release\n\n## Known issues and open items\n\n## Still to do before release\n');
    done.push('created ' + path.relative(ROOT, nFile));
  }
  if (RELEASE) {
    let s = fs.readFileSync(nFile, 'utf8');
    s = s.split('\n').filter(l => !l.startsWith('> **In development') && !l.startsWith('> **Draft')).join('\n')
      .replace(/^\*\*Release date:\*\* .*$/m, '**Release date:** ' + RELEASE)
      .replace(/ \((?:DRAFT|in development)\)$/m, '')
      .replace(/ \(to be (?:created|exported) at release\)/g, '')
      .replace(/\n{3,}/g, '\n\n');
    fs.writeFileSync(nFile, s);
    done.push('release notes dated ' + RELEASE + ', banner removed');

    // Remove older-version trackers and STEP exports from the package (they stay in git history).
    const old = trackers.filter(f => cmp(verOf(f, TRACKER_RE), V) < 0)
      .concat(fs.readdirSync(path.join(ROOT, 'CAD')).filter(f => STEP_RE.test(f) && cmp(verOf(f, STEP_RE), V) < 0).map(f => path.join('CAD', f)));
    for (const f of old) execFileSync('git', ['rm', '-q', '--', f], { cwd: ROOT });
    if (old.length) done.push('git rm ' + old.join(', '));
  }
  console.log('v' + V + ':\n  ' + done.join('\n  '));
})().catch(e => { console.error(e.message); process.exit(1); });
