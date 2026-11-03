// Writes a release copy of the tracker: stamps the version on the Read Me sheet and repoints
// Build Guide render links from renders/ to renderedImages/buildSteps/ (the package layout).
// Usage: node release_tracker.js <src.xlsx> <dst.xlsx> <version> <date>
const ExcelJS = require('exceljs');
const [SRC, DST, VER, DATE] = process.argv.slice(2);

(async () => {
  const wb = new ExcelJS.Workbook();
  await wb.xlsx.readFile(SRC);
  const rm = wb.getWorksheet('Read Me');
  rm.getCell('A1').value = `Airboat Build Tracker - release ${VER} (${DATE})`;
  const bg = wb.getWorksheet('Build Guide');
  let n = 0;
  bg.eachRow((row) => {
    row.eachCell((c) => {
      const v = c.value;
      if (v && typeof v === 'object' && v.hyperlink && v.hyperlink.startsWith('renders/')) {
        c.value = { text: v.text.split('renders/').join('renderedImages/buildSteps/'), hyperlink: v.hyperlink.replace('renders/', 'renderedImages/buildSteps/') };
        n++;
      }
    });
  });
  wb.calcProperties.fullCalcOnLoad = true;
  await wb.xlsx.writeFile(DST);
  console.log('relinked', n);
})();
