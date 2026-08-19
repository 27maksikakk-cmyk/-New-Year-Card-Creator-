🎄 New Year Card Creator — Multi‑Language Greeting Card Generator
8 languages, one magical holiday card maker – create personalized New Year's cards with custom messages, festive themes, and photos – right from your terminal.

✨ Features
🎨 Multiple holiday templates – classic, modern, and festive designs

✍️ Custom text – name, year, and personalized message

🖼️ Add photos – overlay a portrait or logo onto the card

🌈 Color themes – pick background and text colors (hex or named)

❄️ Seasonal decorations – snowflakes, stars, and holiday emojis

📁 Output to PNG – high‑quality 800×600 card

🌐 HTML export – generate a web‑ready card (optional)

🧰 Supported Languages & Files
Language	File	Dependencies
Python	ny_card_creator.py	Pillow, requests (optional)
Go	ny_card_creator.go	github.com/fogleman/gg
JavaScript (Node)	ny_card_creator.js	canvas, sharp
Ruby	ny_card_creator.rb	rmagick (or chunky_png)
PHP	ny_card_creator.php	GD extension
Java	NYCardCreator.java	Java AWT, javax.imageio
C#	NYCardCreator.cs	SixLabors.ImageSharp
C++	ny_card_creator.cpp	CImg (header‑only)
🚀 Common Usage
All implementations follow the same CLI pattern:

bash
# Generate a simple card
<command> -n "Alice" -m "Happy New Year!" -o card.png

# Choose a template (1=classic, 2=modern, 3=festive)
<command> -n "Bob" -m "Best wishes!" -t 2 -o modern_card.png

# Customize colors
<command> -n "Charlie" -m "Cheers!" --bg "#FFD700" --text "#8B0000"

# Add a photo
<command> -n "Diana" -m "Happy 2026!" --photo portrait.jpg

# Use a background image
<command> -n "Eve" -m "New Year, New You" --bg-image snow.jpg

# Export to HTML
<command> -n "Frank" -m "Happy New Year!" --html card.html
Arguments:

-n, --name – recipient's name (required)

-m, --message – custom message (required)

-t, --template – template number (1‑3, default: 1)

--bg – background color (hex or name, default: #FFFFFF)

--text – text color (hex or name, default: #000000)

--photo – path to a photo to embed (optional)

--bg-image – path to a background image (optional)

-o, --output – output PNG file (default: ny_card.png)

--html – output HTML file (optional)

📸 Example Output (Text Description)
text
╔═══════════════════════════════════════════════════════════════╗
║                                                               ║
║                 ❄️  Happy New Year!  ❄️                      ║
║                                                               ║
║                          Alice                                ║
║                                                               ║
║               "Wishing you joy and peace"                    ║
║                                                               ║
║                    ⭐  🎄  🎆                               ║
║                                                               ║
╚═══════════════════════════════════════════════════════════════╝
(Actual output is a beautifully rendered 800×600 PNG.)

📁 Repository Structure
text
.
├── README.md
├── python/
│   └── ny_card_creator.py
├── go/
│   └── ny_card_creator.go
├── javascript/
│   └── ny_card_creator.js
├── ruby/
│   └── ny_card_creator.rb
├── php/
│   └── ny_card_creator.php
├── java/
│   └── NYCardCreator.java
├── csharp/
│   └── NYCardCreator.cs
└── cpp/
    └── ny_card_creator.cpp
