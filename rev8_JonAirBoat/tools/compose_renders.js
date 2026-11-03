// Builds the release's renderedImages folder from raw Onshape renders (transparent PNGs).
// Usage: node compose_renders.js <rawDir> <outDir> <versionLabel>
const sharp = require('sharp');
const fs = require('fs');
const path = require('path');

const [RAW, OUT, VER] = process.argv.slice(2);
const W = 1920, H = 1080;

const views = {
  v1: 'front-starboard 3-4 view',
  v2: 'rear-port 3-4 view',
  v3: 'starboard profile',
  v4: 'top view',
  v5: 'low front-starboard view',
  v6: 'starboard 3-4 view',
};

function caption(text, w, h, dark) {
  const fill = dark ? '#ffffff' : '#1f2a36';
  return Buffer.from(`<svg width="${w}" height="${h}" xmlns="http://www.w3.org/2000/svg">
    <text x="28" y="${h - 26}" font-family="Arial, Helvetica, sans-serif" font-size="26" fill="${fill}" opacity="0.9">${text}</text></svg>`);
}

async function loadBoat(file) {
  const { data, info } = await sharp(file).trim({ threshold: 1 }).ensureAlpha().raw().toBuffer({ resolveWithObject: true });
  return { data, w: info.width, h: info.height };
}

function toPng(b) {
  return sharp(b.data, { raw: { width: b.w, height: b.h, channels: 4 } }).png().toBuffer();
}

// Fake the boat sitting in water: per column, tint the lowest `draft` px of the hull with water colour
// and draw a foam line at the waterline. Returns per-column waterline y (or -1).
function immerse(b, draft, water, foam) {
  const line = new Int32Array(b.w).fill(-1);
  const bots = new Int32Array(b.w).fill(-1);
  for (let x = 0; x < b.w; x++)
    for (let y = b.h - 1; y >= 0; y--) if (b.data[(y * b.w + x) * 4 + 3] > 128) { bots[x] = y; break; }
  // Only columns whose lowest pixel is near the hull bottom touch the water (not rudders or the cage).
  const lowest = Math.max(...bots);
  for (let x = 0; x < b.w; x++) {
    const bot = bots[x];
    if (bot < 0 || bot < lowest - 0.15 * b.h) continue;
    const top = Math.max(0, bot - draft);
    line[x] = top;
    for (let y = top; y <= bot; y++) {
      const i = (y * b.w + x) * 4;
      if (b.data[i + 3] < 20) continue;
      const k = Math.min(1, 0.55 + 0.45 * (y - top) / Math.max(1, draft));
      for (let c = 0; c < 3; c++) b.data[i + c] = Math.round(b.data[i + c] * (1 - k) + water[c] * k);
      b.data[i + 3] = 255;
    }
    if (foam) {
      for (let y = Math.max(0, top - 2); y <= Math.min(b.h - 1, top + 2); y++) {
        const i = (y * b.w + x) * 4;
        if (b.data[i + 3] < 20) continue;
        for (let c = 0; c < 3; c++) b.data[i + c] = Math.round(b.data[i + c] * 0.25 + 245 * 0.75);
      }
    }
  }
  return line;
}

async function scaled(b, width) {
  const png = await toPng(b);
  const out = await sharp(png).resize({ width }).raw().toBuffer({ resolveWithObject: true });
  return { data: out.data, w: out.info.width, h: out.info.height };
}

function skyWater(horizon, opts) {
  const { skyTop, skyBot, waterTop, waterBot, shore } = opts;
  let ripples = '';
  for (let i = 0; i < 70; i++) {
    const y = horizon + 8 + Math.pow(i / 70, 1.7) * (H - horizon);
    const x = (i * 397) % W;
    const len = 60 + (i * 53) % 260;
    ripples += `<rect x="${x}" y="${y.toFixed(1)}" width="${len}" height="${(1 + i / 30).toFixed(1)}" fill="#ffffff" opacity="${(0.08 + (i % 5) * 0.03).toFixed(2)}"/>`;
  }
  let trees = '';
  if (shore) {
    for (let x = -40; x < W + 40; x += 38) {
      const h = 30 + ((x * 7919) % 70);
      trees += `<ellipse cx="${x}" cy="${horizon - h / 2}" rx="34" ry="${h / 2 + 10}" fill="${shore}"/>`;
    }
    for (let x = 0; x < W; x += 9) {
      const h = 10 + ((x * 31) % 22);
      trees += `<rect x="${x}" y="${horizon - h}" width="2" height="${h}" fill="#5f6b3a" opacity="0.8"/>`;
    }
  }
  return `<defs>
      <linearGradient id="sky" x1="0" y1="0" x2="0" y2="1"><stop offset="0" stop-color="${skyTop}"/><stop offset="1" stop-color="${skyBot}"/></linearGradient>
      <linearGradient id="water" x1="0" y1="0" x2="0" y2="1"><stop offset="0" stop-color="${waterTop}"/><stop offset="1" stop-color="${waterBot}"/></linearGradient>
    </defs>
    <rect width="${W}" height="${horizon}" fill="url(#sky)"/>
    <ellipse cx="420" cy="150" rx="220" ry="40" fill="#ffffff" opacity="0.55"/>
    <ellipse cx="560" cy="130" rx="160" ry="34" fill="#ffffff" opacity="0.5"/>
    <ellipse cx="1480" cy="210" rx="260" ry="36" fill="#ffffff" opacity="0.45"/>
    ${trees}
    <rect y="${horizon}" width="${W}" height="${H - horizon}" fill="url(#water)"/>
    ${ripples}`;
}

async function scene(name, opts) {
  const b = await loadBoat(path.join(RAW, opts.raw));
  let boat = await scaled(b, opts.width);
  const draft = Math.round(opts.width * opts.draftFrac);
  const line = immerse(boat, draft, opts.waterRGB, opts.foam);
  let boatPng = await toPng(boat);
  let bw = boat.w, bh = boat.h;
  if (opts.tilt) {
    boatPng = await sharp(boatPng).rotate(opts.tilt, { background: { r: 0, g: 0, b: 0, alpha: 0 } }).png().toBuffer();
    const m = await sharp(boatPng).metadata();
    bw = m.width; bh = m.height;
  }
  // Place the boat so its average waterline sits at opts.waterY.
  let sum = 0, n = 0, minX = bw, maxX = 0;
  for (let x = 0; x < line.length; x++) if (line[x] >= 0) { sum += line[x]; n++; minX = Math.min(minX, x); maxX = Math.max(maxX, x); }
  const avgLine = sum / n + (bh - boat.h) / 2;
  const left = Math.round(opts.cx - bw / 2);
  const top = Math.round(opts.waterY - avgLine);
  const layers = [];
  const bg = `<svg width="${W}" height="${H}" xmlns="http://www.w3.org/2000/svg">${skyWater(opts.horizon, opts.palette)}${opts.behind ? opts.behind({ left, top, bw, bh, minX, maxX }) : ''}</svg>`;
  if (opts.reflection) {
    const refl = await sharp(boatPng).flip().blur(4).ensureAlpha(0.3).png().toBuffer();
    const faded = await sharp(refl).composite([{ input: Buffer.from(`<svg width="${bw}" height="${bh}"><rect width="${bw}" height="${bh}" fill="#000" opacity="0.72"/></svg>`), blend: 'dest-out' }]).png().toBuffer();
    layers.push({ input: faded, left, top: Math.round(opts.waterY + (opts.waterY - top) - bh + 6) });
  }
  layers.push({ input: boatPng, left, top });
  if (opts.front) layers.push({ input: Buffer.from(`<svg width="${W}" height="${H}" xmlns="http://www.w3.org/2000/svg">${opts.front({ left, top, bw, bh, minX, maxX })}</svg>`), left: 0, top: 0 });
  layers.push({ input: caption(`DIY Jon-Air airboat ${VER} - ${opts.title} (concept render, stylized background)`, W, H, true), left: 0, top: 0 });
  await sharp(Buffer.from(bg)).composite(layers).png().toFile(path.join(OUT, name));
  console.log('scene', name);
}

const spray = (cx, cy, rx, ry, op, blur) => `<ellipse cx="${cx}" cy="${cy}" rx="${rx}" ry="${ry}" fill="#ffffff" opacity="${op}" filter="url(#b${blur})"/>`;
const blurDefs = `<defs><filter id="b4"><feGaussianBlur stdDeviation="4"/></filter><filter id="b10"><feGaussianBlur stdDeviation="10"/></filter><filter id="b20"><feGaussianBlur stdDeviation="20"/></filter></defs>`;

(async () => {
  fs.mkdirSync(path.join(OUT, 'studio'), { recursive: true });
  fs.mkdirSync(path.join(OUT, 'scenes'), { recursive: true });

  // Studio shots on a soft backdrop, with and without crew.
  for (const [v, label] of Object.entries(views)) {
    for (const crew of ['nocrew', 'crew']) {
      const f = path.join(RAW, `${v}_${crew}.png`);
      const backdrop = Buffer.from(`<svg width="1600" height="1000" xmlns="http://www.w3.org/2000/svg"><defs><radialGradient id="g" cx="0.5" cy="0.45" r="0.75"><stop offset="0" stop-color="#ffffff"/><stop offset="1" stop-color="#d9e1ea"/></radialGradient></defs><rect width="1600" height="1000" fill="url(#g)"/></svg>`);
      const name = `${v}_${label.replace(/[^a-z0-9]+/gi, '-')}_${crew === 'crew' ? 'with-crew' : 'no-crew'}.png`;
      await sharp(backdrop).composite([
        { input: f, left: 0, top: 0 },
        { input: caption(`DIY Jon-Air airboat ${VER} - ${label}, ${crew === 'crew' ? 'driver and passenger mannequins' : 'no crew'}`, 1600, 1000, false), left: 0, top: 0 },
      ]).png().toFile(path.join(OUT, 'studio', name));
      console.log('studio', name);
    }
  }

  const swamp = { skyTop: '#4f8fd6', skyBot: '#cfe6f7', waterTop: '#5d7f86', waterBot: '#2f4b55', shore: '#3f5a2e' };

  await scene('scenes/underway_with-crew.png', {
    raw: 'v6_crew.png', width: 1250, draftFrac: 0.018, waterRGB: [70, 100, 108], foam: true,
    horizon: 470, waterY: 830, cx: 1000, palette: swamp, title: 'underway',
    behind: ({ left, top, bw, bh }) => `${blurDefs}
      <polygon points="${left + 40},830 ${left - 700},780 ${left - 760},900 ${left + 60},860" fill="#e8f2f4" opacity="0.35" filter="url(#b10)"/>
      ${spray(left + 20, 820, 120, 26, 0.6, 10)}${spray(left - 160, 815, 180, 30, 0.35, 20)}`,
    front: ({ left, bw }) => `${blurDefs}${spray(left + bw - 120, 836, 90, 14, 0.55, 4)}${spray(left + bw * 0.5, 846, bw * 0.45, 9, 0.3, 4)}`,
  });

  await scene('scenes/top-speed_with-crew.png', {
    raw: 'v5_crew.png', width: 1350, draftFrac: 0.008, waterRGB: [70, 100, 108], foam: true, tilt: -3,
    horizon: 430, waterY: 860, cx: 1020, palette: { ...swamp, skyTop: '#3d7fd0' }, title: 'top speed',
    behind: ({ left, top, bw }) => {
      let streaks = '';
      for (let i = 0; i < 26; i++) {
        const y = 560 + i * 19;
        streaks += `<rect x="${(i * 211) % 600}" y="${y}" width="${500 + (i * 97) % 700}" height="2" fill="#ffffff" opacity="0.22"/>`;
      }
      return `${blurDefs}${streaks}
        ${spray(left + 80, 800, 260, 90, 0.55, 20)}${spray(left - 120, 760, 300, 120, 0.4, 20)}${spray(left - 350, 720, 280, 110, 0.25, 20)}
        ${spray(left + 60, 840, 200, 30, 0.7, 10)}`;
    },
    front: ({ left, bw }) => `${blurDefs}${spray(left + bw * 0.62, 870, 160, 22, 0.75, 4)}${spray(left + bw * 0.45, 880, bw * 0.4, 12, 0.45, 4)}`,
  });

  const dock = ({ left, bw }) => {
    let planks = '';
    for (let x = 0; x < W; x += 46) planks += `<rect x="${x}" y="560" width="42" height="34" fill="#8a6a45"/><rect x="${x}" y="560" width="42" height="4" fill="#a98458"/>`;
    let piles = '';
    for (let x = 60; x < W; x += 300) piles += `<rect x="${x}" y="520" width="34" height="210" fill="#6b4f33"/><rect x="${x}" y="518" width="34" height="8" fill="#8f6c47"/>`;
    return `${planks}<rect y="594" width="${W}" height="14" fill="#5b432b"/>${piles}
      <path d="M ${left + bw * 0.78} 640 C ${left + bw * 0.8} 600, ${left + bw * 0.84} 590, ${left + bw * 0.86} 566" stroke="#d8c8a0" stroke-width="5" fill="none"/>`;
  };
  const calm = { skyTop: '#f2a65a', skyBot: '#fbe3c2', waterTop: '#7f8f8f', waterBot: '#3e5257', shore: '#4b5d36' };
  await scene('scenes/docked_no-crew.png', {
    raw: 'v3_nocrew.png', width: 1500, draftFrac: 0.016, waterRGB: [85, 104, 106], foam: false, reflection: true,
    horizon: 480, waterY: 800, cx: 960, palette: calm, title: 'docked', behind: dock,
  });
  await scene('scenes/docked_with-crew.png', {
    raw: 'v3_crew.png', width: 1500, draftFrac: 0.016, waterRGB: [85, 104, 106], foam: false, reflection: true,
    horizon: 480, waterY: 800, cx: 960, palette: calm, title: 'docked, crew aboard', behind: dock,
  });
})().catch(e => { console.error(e); process.exit(1); });
