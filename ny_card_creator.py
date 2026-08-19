# ny_card_creator.py
import sys, os, argparse, json, textwrap
from PIL import Image, ImageDraw, ImageFont, ImageFilter

TEMPLATES = {
    1: {"name": "Classic", "bg": (255,255,255), "text": (0,0,0), "font_size": 60},
    2: {"name": "Modern", "bg": (0,100,150), "text": (255,215,0), "font_size": 70},
    3: {"name": "Festive", "bg": (200,50,50), "text": (255,255,255), "font_size": 65}
}

def create_card(name, message, template, bg_color, text_color, photo_path, bg_image_path, output):
    # Use template defaults if colors not overridden
    if bg_color is None:
        bg_color = TEMPLATES[template]["bg"]
    else:
        bg_color = tuple(int(bg_color.lstrip('#')[i:i+2], 16) for i in (0,2,4))
    if text_color is None:
        text_color = TEMPLATES[template]["text"]
    else:
        text_color = tuple(int(text_color.lstrip('#')[i:i+2], 16) for i in (0,2,4))

    width, height = 800, 600
    if bg_image_path and os.path.exists(bg_image_path):
        bg_img = Image.open(bg_image_path).resize((width, height))
        img = bg_img.convert("RGB")
    else:
        img = Image.new("RGB", (width, height), bg_color)

    draw = ImageDraw.Draw(img)
    try:
        font_large = ImageFont.truetype("/usr/share/fonts/truetype/liberation/LiberationSans-Bold.ttf", 60)
        font_medium = ImageFont.truetype("/usr/share/fonts/truetype/liberation/LiberationSans-Regular.ttf", 40)
        font_small = ImageFont.truetype("/usr/share/fonts/truetype/liberation/LiberationSans-Italic.ttf", 30)
    except:
        font_large = ImageFont.load_default()
        font_medium = ImageFont.load_default()
        font_small = ImageFont.load_default()

    # Title
    title = "❄️ Happy New Year! ❄️"
    draw.text((width//2, 50), title, font=font_large, fill=text_color, anchor="mt")

    # Name
    draw.text((width//2, 200), name, font=font_large, fill=text_color, anchor="mt")

    # Message with wrapping
    wrapped = textwrap.fill(message, width=20)
    draw.text((width//2, 380), wrapped, font=font_small, fill=text_color, anchor="mt")

    # Decorations
    draw.text((width//2, 500), "⭐  🎄  🎆", font=font_medium, fill=text_color, anchor="mt")

    # Photo
    if photo_path and os.path.exists(photo_path):
        try:
            photo = Image.open(photo_path)
            photo.thumbnail((150, 150))
            img.paste(photo, (width - photo.width - 20, 20))
        except Exception as e:
            print(f"Warning: could not embed photo: {e}", file=sys.stderr)

    img.save(output)
    print(f"Card saved to {output}")

def main():
    parser = argparse.ArgumentParser(description="New Year Card Creator")
    parser.add_argument('-n', '--name', required=True, help="Recipient's name")
    parser.add_argument('-m', '--message', required=True, help="Custom message")
    parser.add_argument('-t', '--template', type=int, choices=[1,2,3], default=1, help="Template (1-3)")
    parser.add_argument('--bg', help="Background color (hex, e.g. #FF0000)")
    parser.add_argument('--text', help="Text color (hex)")
    parser.add_argument('--photo', help="Path to photo to embed")
    parser.add_argument('--bg-image', help="Path to background image")
    parser.add_argument('-o', '--output', default="ny_card.png", help="Output PNG file")
    parser.add_argument('--html', help="Export to HTML file (optional)")
    args = parser.parse_args()

    create_card(args.name, args.message, args.template, args.bg, args.text, args.photo, args.bg_image, args.output)

    if args.html:
        bg = args.bg or "#FFFFFF"
        txt = args.text or "#000000"
        html_content = f"""<!DOCTYPE html>
<html><head><title>New Year Card</title>
<style>body{{font-family:sans-serif;text-align:center;background:#1a1a2e;}}
.card{{background:{bg};color:{txt};border-radius:20px;padding:40px;max-width:600px;margin:50px auto;box-shadow:0 4px 8px rgba(0,0,0,0.3);}}
h1{{font-size:3em;}} .name{{font-size:2.5em;}} .msg{{font-size:1.5em;}}</style>
</head><body><div class="card">
<h1>❄️ Happy New Year! ❄️</h1>
<div class="name">{args.name}</div>
<div class="msg">{args.message}</div>
<div style="font-size:3em;">⭐ 🎄 🎆</div>
</div></body></html>"""
        with open(args.html, 'w') as f:
            f.write(html_content)
        print(f"HTML card saved to {args.html}")

if __name__ == "__main__":
    main()
