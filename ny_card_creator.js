// ny_card_creator.js
#!/usr/bin/env node
const fs = require('fs');
const { createCanvas, loadImage, registerFont } = require('canvas');
const { program } = require('commander');

const TEMPLATES = {
    1: { bg: '#FFFFFF', text: '#000000' },
    2: { bg: '#006496', text: '#FFD700' },
    3: { bg: '#C83232', text: '#FFFFFF' }
};

function hexToRgb(hex) {
    const result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);
    return result ? [parseInt(result[1], 16), parseInt(result[2], 16), parseInt(result[3], 16)] : [0,0,0];
}

async function createCard(name, message, template, bgColor, textColor, photoPath, bgImagePath, output) {
    const width = 800, height = 600;
    const canvas = createCanvas(width, height);
    const ctx = canvas.getContext('2d');

    let bg = bgColor || TEMPLATES[template].bg;
    let txt = textColor || TEMPLATES[template].text;

    if (bgImagePath && fs.existsSync(bgImagePath)) {
        const bgImg = await loadImage(bgImagePath);
        ctx.drawImage(bgImg, 0, 0, width, height);
    } else {
        ctx.fillStyle = bg;
        ctx.fillRect(0, 0, width, height);
    }

    ctx.textAlign = 'center';
    ctx.textBaseline = 'middle';
    ctx.fillStyle = txt;

    ctx.font = 'bold 60px "Liberation Sans"';
    ctx.fillText('❄️ Happy New Year! ❄️', width/2, 80);

    ctx.font = 'bold 70px "Liberation Sans"';
    ctx.fillText(name, width/2, 200);

    ctx.font = 'italic 30px "Liberation Sans"';
    const words = message.split(' ');
    let lines = [], line = '';
    for (const w of words) {
        if ((line + ' ' + w).length <= 30) {
            line = line ? line + ' ' + w : w;
        } else {
            lines.push(line);
            line = w;
        }
    }
    if (line) lines.push(line);
    let y = 380;
    for (const l of lines) {
        ctx.fillText(l, width/2, y);
        y += 40;
    }

    ctx.font = '40px "Liberation Sans"';
    ctx.fillText('⭐  🎄  🎆', width/2, 520);

    if (photoPath && fs.existsSync(photoPath)) {
        const photo = await loadImage(photoPath);
        const size = 150;
        ctx.drawImage(photo, width - size - 20, 20, size, size);
    }

    const buffer = canvas.toBuffer('image/png');
    fs.writeFileSync(output, buffer);
    console.log(`Card saved to ${output}`);
}

program
    .option('-n, --name <name>', 'Recipient name')
    .option('-m, --message <message>', 'Custom message')
    .option('-t, --template <template>', 'Template (1-3)', parseInt, 1)
    .option('--bg <color>', 'Background color')
    .option('--text <color>', 'Text color')
    .option('--photo <path>', 'Photo path')
    .option('--bg-image <path>', 'Background image path')
    .option('-o, --output <file>', 'Output PNG', 'ny_card.png')
    .option('--html <file>', 'Output HTML file')
    .parse(process.argv);

const opts = program.opts();
if (!opts.name || !opts.message) {
    console.error('Error: -n and -m are required');
    process.exit(1);
}
if (opts.template < 1 || opts.template > 3) {
    console.error('Template must be 1-3');
    process.exit(1);
}

createCard(opts.name, opts.message, opts.template, opts.bg, opts.text, opts.photo, opts.bgImage, opts.output)
    .then(() => {
        if (opts.html) {
            const bg = opts.bg || '#FFFFFF';
            const txt = opts.text || '#000000';
            const html = `<!DOCTYPE html>
<html><head><title>New Year Card</title>
<style>body{font-family:sans-serif;text-align:center;background:#1a1a2e;}
.card{background:${bg};color:${txt};border-radius:20px;padding:40px;max-width:600px;margin:50px auto;box-shadow:0 4px 8px rgba(0,0,0,0.3);}
h1{font-size:3em;}.name{font-size:2.5em;}.msg{font-size:1.5em;}</style>
</head><body><div class="card">
<h1>❄️ Happy New Year! ❄️</h1>
<div class="name">${opts.name}</div>
<div class="msg">${opts.message}</div>
<div style="font-size:3em;">⭐ 🎄 🎆</div>
</div></body></html>`;
            fs.writeFileSync(opts.html, html);
            console.log(`HTML card saved to ${opts.html}`);
        }
    })
    .catch(console.error);
