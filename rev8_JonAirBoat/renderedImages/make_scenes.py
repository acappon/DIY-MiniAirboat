"""Makes the concept scenes in renderedImages/scenes: a CAD render of the boat, composited onto a drawn background.
  python make_scenes.py <render with driver.png> <render without driver.png> <version>
Renders are transparent PNGs from Onshape, starboard view, stern left. Needs numpy and Pillow.
"""
import math
import os
import random
import sys

import numpy as np
from PIL import Image, ImageDraw, ImageFilter, ImageFont

W, H = 1920, 1080
HERE = os.path.dirname(os.path.abspath(__file__))
# Camera of the boat render: right and up vectors.
RIGHT, UP = (0.966, -0.26, 0), (0.07, 0.261, 0.963)
EXTENT = [(-6, 30, 40), (-6, -30, 40), (160, 0, 19), (158, -23, 19), (0, -24.4, 0), (0, 24.4, 0), (-4, 0, 132), (4, 36, 52), (4, -36, 52)]


def proj(p):
    return sum(a * b for a, b in zip(RIGHT, p)), sum(a * b for a, b in zip(UP, p))


def gradient(top, bottom, h):
    t = np.linspace(0, 1, h)[:, None, None]
    return (np.array(top) * (1 - t) + np.array(bottom) * t)[:, :].repeat(W, axis=1).astype('uint8')


def blob_layer(draw, n, y, ry, w, color, rng):
    for _ in range(n):
        x = rng.randrange(-100, W + 100)
        r = rng.randrange(w // 2, w)
        draw.ellipse((x - r, y - rng.randrange(ry // 2, ry), x + r, y + ry), fill=color)


def reeds(draw, x0, x1, ybase, hmin, hmax, n, colors, rng):
    for _ in range(n):
        x = rng.randrange(x0, x1)
        h = rng.randrange(hmin, hmax)
        lean = rng.randrange(-16, 17)
        draw.line((x, ybase + rng.randrange(0, 30), x + lean, ybase - h), fill=rng.choice(colors), width=rng.choice((2, 3, 4)))


def water(img, top, c0, c1, line, rng):
    a = np.array(img)
    a[top:] = gradient(c0, c1, H - top)
    img = Image.fromarray(a)
    d = ImageDraw.Draw(img)
    for _ in range(420):
        y = rng.randrange(top + 6, H)
        x = rng.randrange(-40, W)
        d.line((x, y, x + rng.randrange(30, 170) * (1 + (y - top) / 300), y), fill=line, width=1 + (y - top) // 300)
    return img


def day(rng, reed=True):
    img = Image.fromarray(gradient((48, 118, 200), (205, 226, 240), H))
    cl = Image.new('RGBA', (W, H), (0, 0, 0, 0))
    d = ImageDraw.Draw(cl)
    for _ in range(9):
        x, y = rng.randrange(0, W), rng.randrange(40, 340)
        for _ in range(7):
            r = rng.randrange(70, 210)
            d.ellipse((x - r * 2, y - r / 3, x + r * 2, y + r / 3), fill=(255, 255, 255, 170))
            x += rng.randrange(-90, 150)
    img.paste(cl.filter(ImageFilter.GaussianBlur(24)), (0, 0), cl.filter(ImageFilter.GaussianBlur(24)))
    d = ImageDraw.Draw(img)
    blob_layer(d, 34, 450, 48, 90, (120, 158, 124), rng)
    blob_layer(d, 40, 462, 34, 70, (66, 122, 80), rng)
    img = water(img, 468, (116, 168, 170), (24, 88, 112), (190, 222, 225), rng)
    if reed:
        reeds(ImageDraw.Draw(img), 0, W, 468, 12, 28, 900, [(120, 150, 60), (150, 170, 70)], rng)
    return img


def night(rng):
    a = np.zeros((H, W, 3), 'uint8')
    a[:500] = gradient((12, 16, 52), (214, 112, 74), 500)
    img = Image.fromarray(a)
    d = ImageDraw.Draw(img)
    for _ in range(420):
        x, y = rng.randrange(W), rng.randrange(330)
        d.point((x, y), fill=(230, 230, 245))
    halo = Image.new('RGBA', (W, H), (0, 0, 0, 0))
    ImageDraw.Draw(halo).ellipse((1520, 40, 1680, 200), fill=(235, 225, 200, 120))
    halo = halo.filter(ImageFilter.GaussianBlur(26))
    img.paste(halo, (0, 0), halo)
    d.ellipse((1566, 86, 1634, 154), fill=(245, 238, 215))
    blob_layer(d, 34, 480, 50, 74, (22, 34, 56), rng)
    for x, s in ((100, .7), (240, 1), (1120, 1), (1400, .8), (1740, 1.1)):
        d.polygon([(x - 8 * s, 500), (x, 500 - 280 * s), (x + 8 * s, 500)], fill=(14, 22, 42))
        for k in range(8):
            yy = 500 - (70 + 26 * k) * s
            side = 1 if k % 2 else -1
            cx = x + side * (40 + (8 - k) * 10) * s
            d.ellipse((cx - 62 * s, yy - 10 * s, cx + 62 * s, yy + 10 * s), fill=(14, 22, 42))
    img = water(img, 500, (112, 82, 112), (14, 26, 62), (150, 130, 165), rng)
    return img


def boat(path, scale, cx, wl_y):
    """Boat image placed with its waterline at wl_y, trimmed to the hull, with its lower part cut at the water."""
    im = Image.open(path).convert('RGBA')
    x0, y0, x1, y1 = im.getbbox()
    im = im.crop((x0, y0, x1, y1))
    pts = [proj(p) for p in EXTENT]
    umin, umax = min(p[0] for p in pts), max(p[0] for p in pts)
    vmin, vmax = min(p[1] for p in pts), max(p[1] for p in pts)
    w = int(im.width * scale)
    h = int(im.height * scale)
    im = im.resize((w, h), Image.LANCZOS)
    sx, sy = w / (umax - umin), h / (vmax - vmin)
    a = np.array(im)
    # Waterline along the near side, Z 5.5.
    u0, v0 = proj((0, -26, 5.5))
    u1, v1 = proj((150, -24, 5.5))
    xa, ya = (u0 - umin) * sx, (vmax - v0) * sy
    xb, yb = (u1 - umin) * sx, (vmax - v1) * sy
    yy, xx = np.mgrid[0:h, 0:w]
    line = ya + (xx - xa) * (yb - ya) / (xb - xa)
    a[..., 3][yy > line] = 0
    im = Image.fromarray(a)
    ox = int(cx - w / 2)
    oy = int(wl_y - (ya + yb) / 2)
    return im, ox, oy, (xa + ox, ya + oy, xb + ox, yb + oy)


def place(bg, im, ox, oy, wl, reflect=0.22):
    refl = im.transpose(Image.FLIP_TOP_BOTTOM).filter(ImageFilter.GaussianBlur(3))
    r = np.array(refl)
    r[..., 3] = (r[..., 3] * reflect).astype('uint8')
    bg.paste(Image.fromarray(r), (ox, int(wl[1] + wl[3]) // 2 - 4), Image.fromarray(r))
    bg.paste(im, (ox, oy), im)
    d = ImageDraw.Draw(bg)
    d.line((wl[0] - 10, wl[1] + 1, wl[2] + 30, wl[3] + 1), fill=(255, 255, 255), width=3)


def caption(img, text):
    bar = Image.new('RGBA', (W, 64), (0, 0, 0, 120))
    img.paste(bar, (0, H - 64), bar)
    d = ImageDraw.Draw(img)
    try:
        f = ImageFont.truetype('segoeui.ttf', 26)
    except OSError:
        f = ImageFont.load_default()
    d.text((28, H - 50), text, fill=(255, 255, 255), font=f)


def spray(img, wl):
    s = Image.new('RGBA', (W, H), (0, 0, 0, 0))
    d = ImageDraw.Draw(s)
    x, y = wl[0], wl[1]
    for i in range(16):
        r = 70 - i * 3
        d.ellipse((x - 40 - i * 34 - r * 2, y - 70 - r / 2, x - 40 - i * 34 + r * 2, y - 40 + r / 2), fill=(255, 255, 255, 210))
    s = s.filter(ImageFilter.GaussianBlur(22))
    img.paste(s, (0, 0), s)


def docks(img, wl_y):
    d = ImageDraw.Draw(img)
    y = wl_y - 34
    d.rectangle((0, y, W, y + 18), fill=(120, 92, 62))
    for x in range(0, W, 46):
        d.line((x, y, x, y + 18), fill=(84, 62, 42), width=2)
    for x in range(40, W, 300):
        d.rectangle((x, y - 40, x + 18, y + 62), fill=(92, 70, 48))


def snake(img):
    d = ImageDraw.Draw(img)
    d.rounded_rectangle((1290, 806, 1890, 860), 26, fill=(82, 58, 38))
    pts = [(1330, 782), (1380, 798), (1440, 806), (1500, 796), (1560, 780), (1620, 790), (1680, 812), (1750, 816), (1830, 800)]
    d.line(pts, fill=(60, 46, 26), width=30, joint='curve')
    d.line(pts, fill=(196, 160, 96), width=20, joint='curve')
    for i in range(len(pts) - 1):
        for t in (.25, .75):
            x = pts[i][0] + (pts[i + 1][0] - pts[i][0]) * t
            y = pts[i][1] + (pts[i + 1][1] - pts[i][1]) * t
            d.ellipse((x - 11, y - 6, x + 11, y + 6), fill=(70, 50, 28))
    d.ellipse((1298, 766, 1346, 796), fill=(170, 136, 82))
    d.line((1298, 780, 1278, 774), fill=(200, 40, 40), width=2)


def beam(img, x0, y0):
    b = Image.new('RGBA', (W, H), (0, 0, 0, 0))
    d = ImageDraw.Draw(b)
    d.polygon([(x0, y0), (1760, 660), (1560, 960), (x0, y0 + 10)], fill=(255, 248, 215, 110))
    d.ellipse((x0 - 42, y0 - 42, x0 + 42, y0 + 42), fill=(255, 250, 225, 170))
    b = b.filter(ImageFilter.GaussianBlur(9))
    img.paste(b, (0, 0), b)


def main():
    crew, none, ver = sys.argv[1:4]
    out = os.path.join(HERE, 'scenes')
    for tag, path in (('solo', crew), ('no-crew', none)):
        rng = random.Random(7)
        cap = 'with the driver' if tag == 'solo' else 'no crew'
        img = day(rng)
        im, ox, oy, wl = boat(path, 0.62, 960, 880)
        spray(img, wl)
        place(img, im, ox, oy, wl)
        caption(img, f'DIY Jon-Air airboat {ver} - underway, {cap} (concept render, stylized background)')
        img.convert('RGB').save(os.path.join(out, f'underway_{tag}.png'))

        rng = random.Random(11)
        img = day(rng, reed=False)
        docks(img, 800)
        im, ox, oy, wl = boat(path, 0.62, 960, 860)
        place(img, im, ox, oy, wl)
        caption(img, f'DIY Jon-Air airboat {ver} - docked, {cap} (concept render, stylized background)')
        img.convert('RGB').save(os.path.join(out, f'docked_{tag}.png'))

        rng = random.Random(23)
        img = night(rng)
        im, ox, oy, wl = boat(path, 0.56, 640, 900)
        dark = np.array(im).astype('float32')
        dark[..., :3] *= (0.62, 0.62, 0.72)
        im = Image.fromarray(dark.astype('uint8'))
        place(img, im, ox, oy, wl, reflect=0.14)
        snake(img)
        lx = ox + int((proj((6, 0, 92))[0] - min(proj(p)[0] for p in EXTENT)) * im.width / (max(proj(p)[0] for p in EXTENT) - min(proj(p)[0] for p in EXTENT)))
        ly = oy + int((max(proj(p)[1] for p in EXTENT) - proj((6, 0, 92))[1]) * im.height / (max(proj(p)[1] for p in EXTENT) - min(proj(p)[1] for p in EXTENT)))
        beam(img, lx, ly)
        d = ImageDraw.Draw(img)
        reeds(d, 0, 380, 1040, 90, 240, 170, [(24, 50, 36), (38, 74, 48)], rng)
        reeds(d, 1720, 1920, 1060, 100, 210, 110, [(24, 50, 36), (38, 74, 48)], rng)
        caption(img, f'DIY Jon-Air airboat {ver} - hunting for snakes, {cap} (concept render, stylized background)')
        img.convert('RGB').save(os.path.join(out, f'snake-hunt_{tag}.png'))


if __name__ == '__main__':
    main()
