"""Makes the standalone Build Guide PDF from the Build Guide tab of the tracker, with its step images.
  python make_build_guide.py   reads the newest "Airboat Build Tracker v*.xlsx" and writes "Airboat Build Guide v*.pdf"
The tracker stays the master: change the steps there and run this again. Needs Microsoft Edge to print the PDF.
"""
import glob
import html
import os
import re
import subprocess
import tempfile
import zipfile

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
EDGE = r'C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe'


def rows_of(xml):
    rows = []
    for m in re.finditer(r'<row r="(\d+)"[^>]*>(.*?)</row>', xml, re.S):
        cells = {}
        for c in re.finditer(r'<c r="([A-Z]+)\d+"[^>]*?(?:/>|>(.*?)</c>)', m.group(2), re.S):
            v = c.group(2) or ''
            t = re.search(r'<t[^>]*>(.*?)</t>', v, re.S)
            n = re.search(r'<v>(.+?)</v>', v)
            cells[c.group(1)] = html.unescape(t.group(1)) if t else (n.group(1) if n else '')
        rows.append(cells)
    return rows


def body(text):
    out = []
    for block in text.split('\n'):
        block = block.strip()
        if not block:
            continue
        cls = ' class="n"' if re.match(r'(\d+\.|-) ', block) else ''
        if re.fullmatch(r'[A-Z][\w ,()]*:', block):
            cls = ' class="h"'
        out.append('<p%s>%s</p>' % (cls, html.escape(block)))
    return ''.join(out)


def sheet(rows):
    """A small spreadsheet: a header row, then rows of label, material cost, DIY hours, pro hours and pro cost."""
    out = ['<table class="sheet"><tr><th class="l">&nbsp;</th><th>Materials</th><th>DIY hours</th><th>Pro hours</th><th>Pro cost if hired</th><th>Total if hired</th></tr>']
    for label, mat, g, h, c, total in rows:
        out.append('<tr%s><td class="l">%s</td><td>$%s</td><td>%g</td><td>%g</td><td>$%s</td><td>$%s</td></tr>' % (
            ' class="tot"' if total else '', html.escape(label), format(round(mat), ','), g, h, format(round(c), ','), format(round(mat + c), ',')))
    return ''.join(out) + '</table>'


def main():
    def key(p):
        return [int(n) for n in re.search(r'v([\d.]+)\.xlsx', p).group(1).split('.')]
    tracker = max(glob.glob(os.path.join(ROOT, 'Airboat Build Tracker v*.xlsx')), key=key)
    version = re.search(r'v([\d.]+)\.xlsx', tracker).group(1)
    with zipfile.ZipFile(tracker) as z:
        rows = rows_of(z.read('xl/worksheets/sheet3.xml').decode('utf-8'))
        bom = rows_of(z.read('xl/worksheets/sheet2.xml').decode('utf-8'))
        rate = float(next(c['B'] for c in rows_of(z.read('xl/worksheets/sheet1.xml').decode('utf-8')) if c.get('A', '').startswith('Pro labor rate')))
    # A BOM line that lists two steps counts in the first one, as on the Build Guide tab.
    material = {}
    for c in bom[1:]:
        if c.get('A'):
            step = c.get('E', '').split(',')[0].strip()
            material[step] = material.get(step, 0) + float(c.get('F') or 0) * float(c.get('H') or 0)

    parts, sections = [], []
    for r in rows[1:]:
        a = r.get('A', '')
        if a.startswith('SECTION'):
            sections.append([a.title().replace('Section', 'Section'), 0.0, 0.0, 0.0, 0.0])
            parts.append('<h2>%s</h2>@@SECTION%d@@' % (html.escape(sections[-1][0]), len(sections) - 1))
            if r.get('E'):
                parts.append('<div class="note">%s</div>' % body(r['E']))
            continue
        if not re.fullmatch(r'\d+\.\d+', a) or 'NOT IN THE MINIMAL BUILD' in r.get('B', ''):
            continue
        g, h = float(r.get('I') or 0), float(r.get('J') or 0)
        mat = material.get(a, 0)
        sec = sections[-1]
        sec[1], sec[2], sec[3], sec[4] = sec[1] + mat, sec[2] + g, sec[3] + h, sec[4] + h * rate
        img = r.get('N', '')
        pic = ''
        if img and os.path.exists(os.path.join(ROOT, img)):
            pic = '<img src="%s">' % ('file:///' + os.path.join(ROOT, img).replace('\\', '/'))
        meta = 'Depends on: %s &nbsp;|&nbsp; Alongside: %s' % (html.escape(r.get('C', '-')), html.escape(r.get('D', '-')))
        skills = '<p class="skills">Skills: %s</p>' % html.escape(r['F']) if r.get('F') else ''
        parts.append('<section><h3>%s &nbsp; %s</h3>%s<p class="meta">%s</p>%s%s%s</section>' % (a, html.escape(r.get('B', '')), sheet([('Step ' + a, mat, g, h, h * rate, False)]), meta, body(r.get('E', '')), pic, skills))

    mat_all, diy, pro, cost = [sum(x[i] for x in sections) for i in (1, 2, 3, 4)]
    parts = [re.sub(r'@@SECTION(\d+)@@', lambda m: sheet([('Section total',) + tuple(sections[int(m.group(1))][1:]) + (True,)]), q) for q in parts]
    summary = sheet([(x[0],) + tuple(x[1:]) + (False,) for x in sections] + [('Total', mat_all, diy, pro, cost, True)])

    doc = f'''<!doctype html><html><head><meta charset="utf-8"><title>Airboat Build Guide v{version}</title><style>
@page {{ size: letter; margin: 0.65in; }}
body {{ font: 10.5pt/1.4 Arial, Helvetica, sans-serif; color: #111; }}
h1 {{ font-size: 22pt; margin: 0 0 4pt; }} h2 {{ font-size: 15pt; border-bottom: 2px solid #222; padding-bottom: 3pt; margin: 18pt 0 8pt; break-after: avoid; }}
h3 {{ font-size: 12pt; margin: 14pt 0 2pt; break-after: avoid; }} p {{ margin: 3pt 0; }} p.n {{ padding-left: 16pt; text-indent: -16pt; }} p.h {{ font-weight: bold; margin-top: 7pt; break-after: avoid; }}
p.meta, p.skills {{ color: #555; font-size: 9pt; }} .note {{ background: #f2f4f6; padding: 6pt 9pt; }}
img {{ display: block; max-width: 100%; max-height: 3.6in; margin: 7pt auto; break-inside: avoid; }}
table.sheet {{ width: 100%; border-collapse: collapse; table-layout: fixed; margin: 4pt 0 6pt; font-size: 9.5pt; page-break-inside: avoid; }}
table.sheet th, table.sheet td {{ border: 0.75pt solid #888; padding: 2pt 6pt; text-align: right; font-variant-numeric: tabular-nums; }}
table.sheet th {{ background: #e6e9ee; font-weight: bold; }}
table.sheet .l {{ width: 30%; text-align: left; }}
table.sheet tr.tot td {{ background: #f1f3f6; font-weight: bold; }}
</style></head><body><h1>DIY Jon-Air Airboat: Build Guide</h1><p>Version {version}, minimal 1448 build, Rotax 800R E-TEC. All costs and hours are estimates. Each step and section shows what it costs if you do it yourself (materials and DIY hours) or hire it out (pro cost, at ${rate:g}/h, on top of materials). Done entirely DIY, the build is about ${round(mat_all):,} and {diy:g} hours. With every step hired out, it is about ${round(mat_all + cost):,}. Steps with 0 pro hours, such as buying the boat and choosing the prop, are yours to do either way. Generated from the Build Guide tab of the tracker; BOM line numbers refer to that tracker.</p>{summary}{''.join(parts)}</body></html>'''

    pdf = os.path.join(ROOT, f'Airboat Build Guide v{version}.pdf')
    with tempfile.TemporaryDirectory() as tmp:
        page = os.path.join(tmp, 'guide.html')
        with open(page, 'w', encoding='utf-8') as f:
            f.write(doc)
        subprocess.run([EDGE, '--headless=new', '--disable-gpu', '--no-pdf-header-footer', '--allow-file-access-from-files',
                        '--user-data-dir=' + os.path.join(tmp, 'edge'), '--print-to-pdf=' + pdf, 'file:///' + page.replace('\\', '/')],
                       capture_output=True, timeout=120)
    print(os.path.basename(pdf), os.path.getsize(pdf), 'bytes', diy, 'DIY h', pro, 'pro h', round(mat_all, 2), 'materials')


if __name__ == '__main__':
    main()
