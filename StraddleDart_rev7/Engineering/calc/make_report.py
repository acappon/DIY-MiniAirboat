"""Builds ../Stability_and_Flotation.html from results.json, capacity.json and the weight model."""
import json, os, sys, html
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
import mass

HERE = os.path.dirname(os.path.abspath(__file__))
R = json.load(open(os.path.join(HERE, 'results.json')))
C = json.load(open(os.path.join(HERE, 'capacity.json')))
IN = 25.4


def f1(v):
    return '%.1f' % v


def chart(series, xmax, ymax, flood=None, ylabel='Righting arm GZ (in)'):
    """Inline SVG line chart; series = [(label, css class, [(deg, in)])]."""
    W, H, L, B, T, Rr = 640, 310, 56, 40, 30, 16
    sx = lambda d: L + d / xmax * (W - L - Rr)
    sy = lambda g: H - B - g / ymax * (H - B - T)
    out = ['<svg viewBox="0 0 %d %d" role="img" aria-label="%s">' % (W, H, html.escape(ylabel))]
    for g in range(0, int(ymax) + 1, max(1, int(ymax / 4))):
        out.append('<line class="grid" x1="%d" x2="%d" y1="%.1f" y2="%.1f"/><text class="tick" x="%d" y="%.1f">%d</text>'
                   % (L, W - Rr, sy(g), sy(g), L - 8, sy(g) + 4, g))
    for d in range(0, int(xmax) + 1, 5):
        out.append('<text class="tick mid" x="%.1f" y="%d">%d°</text>' % (sx(d), H - B + 18, d))
    if flood:
        out.append('<line class="flood" x1="%.1f" x2="%.1f" y1="%d" y2="%d"/><text class="ttl" x="%.1f" y="%d">water over the rail</text>' % (sx(flood), sx(flood), T, H - B, sx(flood) - 6, T + 12))
    for i, (lab, cls, pts) in enumerate(series):
        p = ' '.join('%.1f,%.1f' % (sx(d), sy(g)) for d, g in pts)
        out.append('<polyline class="ln %s" points="%s"/>' % (cls, p))
    out.append('<text class="ttl" x="%d" y="%d">%s</text></svg>' % (L - 30, T - 12, html.escape(ylabel)))
    out.append('<p class="legend">' + ''.join('<span class="%s">%s</span>' % (cls, html.escape(lab)) for lab, cls, _ in series) + '</p>')
    return ''.join(out)


def main():
    cases = [c for c in R['cases'] if 'GZ' not in c]
    gzc = [c for c in R['cases'] if 'GZ' in c]
    sw = R['swamped']
    solo = [c for c in cases if c['case'] == 'Solo, full fuel'][0]
    two = [c for c in cases if c['case'] == 'Solo + passenger, full fuel'][0]

    # weights by group
    items = mass.lightship()
    groups = {}
    for i in items:
        g = 'Rivets, sealant, paint, fasteners (not in CAD)' if i['rule'] == 'allowance' else mass.group_of(i)
        groups.setdefault(g, []).append(i)
    wrows = ''
    for g, its in groups.items():
        w, c = mass.cg(its)
        wrows += '<tr><td>%s</td><td>%.0f</td><td>%.1f</td><td>%.1f</td></tr>' % (g, w, c[0] / IN, c[2] / IN)
    W0, G0 = mass.cg(items)
    wrows += '<tr class="tot"><td>Empty boat</td><td>%.0f</td><td>%.1f</td><td>%.1f</td></tr>' % (W0, G0[0] / IN, G0[2] / IN)

    hrows = ''
    for c in cases:
        hrows += ('<tr><td>%s</td><td>%.0f</td><td>%.1f</td><td>%s</td><td>%+.1f°</td><td>%s</td><td>%.0f</td><td>%.1f°</td></tr>'
                  % (c['case'], c['W'], c['LCG'] / IN, f1(c['d_transom'] / IN), c['trim'], f1(c['fb'] / IN),
                     c['GM'] / IN, c['flood']))

    srows = ''
    for s in sw:
        srows += ('<tr><td>%s</td><td>%.0f</td><td>%+.0f</td><td>%+.1f°</td><td>%s</td><td>%s</td><td>%.0f</td><td>%.1f</td></tr>'
                  % (s['label'].replace('Swamped, ', ''), s['W'], s['reserve'], s['trim'], f1(s['fb_min'] / IN),
                     f1(s['fb_transom'] / IN), s['GM'] / IN, max(g for _, g in s['GZ']) / IN))

    flood = two['flood']
    intact_chart = chart([(c['case'].replace(' (GZ)', ''), 'a' if i == 0 else 'b',
                           [(d, g / IN) for d, g, _ in c['GZ'] if d <= 16])
                          for i, c in enumerate(gzc)], 16, 16, flood)
    sw_chart = chart([(s['label'].split(',')[1].strip() if i else 'bench sealed (design case)', 'abcd'[i],
                       [(0, 0)] + [(d, g / IN) for d, g in s['GZ']]) for i, s in enumerate(sw[:3])], 30, 3)

    d = sw[0]
    page = TEMPLATE
    for k, v in dict(
        W0='%.0f' % W0, LCG0=f1(G0[0] / IN), VCG0=f1(G0[2] / IN),
        WSOLO='%.0f' % solo['W'], WTWO='%.0f' % two['W'],
        T_SOLO=f1(solo['d_transom'] / IN), T_TWO=f1(two['d_transom'] / IN),
        FB_TWO=f1(two['fb'] / IN), FB_SOLO=f1(solo['fb'] / IN), TRIM_SOLO='%.1f' % solo['trim'],
        GM_TWO='%.0f' % (two['GM'] / IN), FLOOD='%.1f' % flood, FLOOD_SOLO='%.1f' % solo['flood'],
        HEEL_P='%.1f' % gzc[1]['heel_person_at_rail'], HEEL_W='%.1f' % gzc[1]['heel_wind30'],
        GZMAX_TWO=f1(max(g for dd, g, _ in gzc[1]['GZ'] if dd <= flood) / IN),
        S_RES='%.0f' % d['reserve'], S_FB=f1(d['fb_min'] / IN), S_FBX='%.0f' % (d['fb_x'] / IN), S_TRIM='%.1f' % d['trim'],
        S_GM='%.0f' % (d['GM'] / IN), S_LIST='%.1f' % d['list'], S_GZ=f1(max(g for _, g in d['GZ']) / IN),
        S_RM='%.0f' % (d['W'] * max(g for _, g in d['GZ']) / IN / 12),
        S2_GM='%.0f' % (sw[2]['GM'] / IN), S2_RES='%.0f' % sw[2]['reserve'], S2_FB=f1(sw[2]['fb_min'] / IN),
        G_FB=f1(sw[3]['fb_min'] / IN),
        SUBF_SENTENCE=(('It still floats with %.0f lb in reserve and the lowest point %.1f in above the water. <strong>Pass.</strong> '
                        'Subpart F requires the boat to stay afloat, not upright. Its swamped GM in this condition is %.1f in.')
                       % (C['subpartF_reserve_lb'], C['subpartF_fb_min_in'], C['subpartF_GM_in'])
                       if C['subpartF_fb_min_in'] > 0 and C['subpartF_GM_in'] > 0 else
                       ('It still floats, with %.0f lb in reserve, so it passes on the rule&rsquo;s wording (stay afloat). But it floats awash: '
                        'the lowest point of the sheer is %.1f in under water and the swamped GM is %.1f in, so in this condition it can turn over. '
                        '<strong>Marginal pass.</strong> It passed comfortably before the 0.125 in bottom, the G13B and the cage changes added about 75 lb.')
                       % (C['subpartF_reserve_lb'], -C['subpartF_fb_min_in'], C['subpartF_GM_in'])),
        HROWS=hrows, SROWS=srows, WROWS=wrows, INTACT_CHART=intact_chart, SW_CHART=sw_chart,
        **{k.upper(): ('%.0f' % v if isinstance(v, (int, float)) and abs(v) >= 20 else str(v)) for k, v in C.items()}
    ).items():
        page = page.replace('{{%s}}' % k, str(v))
    assert '{{' not in page, page[page.index('{{'):page.index('{{') + 40]
    open(os.path.join(HERE, '..', 'Stability_and_Flotation.html'), 'w', encoding='utf-8').write(page)
    print('written')


TEMPLATE = open(os.path.join(HERE, 'report_template.html'), encoding='utf-8').read()

if __name__ == '__main__':
    main()
