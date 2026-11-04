"""Wire by wire check of electrical/Wire_List.csv against the ABYC E-11 limits and the wire color guide.
Run: python electrical_check.py        Prints a summary and every exception.

Limits used, from the July 2003 edition of E-11, the only full text read:
  - 16 AWG minimum for conductors, unless inside a sheath or under 1 A in an electronic circuit.
  - Table IV ampacity for 105 C insulation, outside engine spaces.
  - Voltage drop: 3% for panel feeds and navigation lights, 10% for general lighting and other loads.
Colors: E-11 Tables XIV and XV, and the Lighting and Supplies guide.
"""
import csv
import os
import re

HERE = os.path.dirname(os.path.abspath(__file__))
AMPACITY = {'18': 20, '16': 25, '14': 35, '12': 45, '10': 60, '8': 80, '6': 120, '4': 160, '2': 210}
CRITICAL = re.compile(r'stern|side light|all.round|dash|bus', re.I)
GUIDE = [
    (r'negative|return|ground', {'black'}, 'black or yellow: negative'),
    (r'start', {'yellow/red', 'red'}, 'yellow/red: starting circuit'),
    (r'nav|stern|side light|all.round|tach', {'dark gray'}, 'gray: navigation lights, tachometer'),
    (r'pump', {'brown'}, 'brown: pumps'),
    (r'temp', {'tan', 'black'}, 'tan: water temperature'),
    (r'gas|fuel gauge|sender', {'pink', 'black'}, 'pink: fuel gauge'),
    (r'ignition|key on|instrument|gauge', {'purple'}, 'purple: ignition, instrument feed'),
    (r'switch bus|accessory|usb', {'orange'}, 'orange: panel to accessory switches'),
    (r'battery|hub|main|LED|searchlight|outlet', {'red'}, 'red: positive power feed'),
]
RESERVED = {'green': 'bonding only', 'green/yellow': 'bonding only'}


def main():
    rows = list(csv.DictReader(open(os.path.join(HERE, 'Wire_List.csv'), encoding='utf-8')))
    colors = {}
    undersize, over, dropped, flagged, reserved = [], [], [], [], []
    for r in rows:
        c = r['Color'].strip()
        colors[c] = colors.get(c, 0) + 1
        awg, amps, drop = r['AWG'].strip(), float(r['Amps'] or 0), float(r['Drop %'] or 0)
        name = r['Function']
        if awg.isdigit() and int(awg) > 16:
            undersize.append((r['Wire'], awg, name))
        if awg in AMPACITY and amps > AMPACITY[awg] and float(r['Length ft'] or 0) > 0 and 'tarter' not in name and 'attery' not in name:
            over.append((r['Wire'], awg, amps))
        limit = 3 if CRITICAL.search(name) else 10
        if drop > limit:
            dropped.append((r['Wire'], drop, limit, name))
        if c in RESERVED:
            reserved.append((r['Wire'], c))
        matched = None
        for pat, ok, label in GUIDE:
            if re.search(pat, name, re.I):
                matched = (ok, label)
                break
        if matched and c not in matched[0]:
            flagged.append((r['Wire'], name, c, matched[1]))
    print('Wires:', len(rows))
    print('Colors:', sorted(colors.items(), key=lambda kv: -kv[1]))
    print('Below 16 AWG:', undersize)
    print('Over Table IV ampacity:', over)
    print('Over the drop limit:', dropped)
    print('Reserved colors used:', reserved)
    print('Largest drops:', sorted(((float(r['Drop %'] or 0), r['Wire'], r['Function']) for r in rows), reverse=True)[:5])
    print('Color differs from the keyword match, review by hand:')
    for f in flagged:
        print('  ', f)


if __name__ == '__main__':
    main()
