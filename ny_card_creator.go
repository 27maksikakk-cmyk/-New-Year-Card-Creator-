// ny_card_creator.go
package main

import (
	"flag"
	"fmt"
	"image"
	"image/color"
	"image/draw"
	"image/png"
	"os"
	"strconv"
	"strings"

	"github.com/fogleman/gg"
)

var templates = map[int]struct{ bg, text color.RGBA }{
	1: {color.RGBA{255, 255, 255, 255}, color.RGBA{0, 0, 0, 255}},
	2: {color.RGBA{0, 100, 150, 255}, color.RGBA{255, 215, 0, 255}},
	3: {color.RGBA{200, 50, 50, 255}, color.RGBA{255, 255, 255, 255}},
}

func hexToColor(hex string) color.RGBA {
	if strings.HasPrefix(hex, "#") {
		hex = hex[1:]
	}
	if len(hex) == 6 {
		r, _ := strconv.ParseUint(hex[0:2], 16, 8)
		g, _ := strconv.ParseUint(hex[2:4], 16, 8)
		b, _ := strconv.ParseUint(hex[4:6], 16, 8)
		return color.RGBA{uint8(r), uint8(g), uint8(b), 255}
	}
	return color.RGBA{0, 0, 0, 255}
}

func createCard(name, message string, template int, bgHex, textHex, photoPath, bgImagePath, output string) error {
	dc := gg.NewContext(800, 600)

	// Background
	var bg color.RGBA
	if bgHex != "" {
		bg = hexToColor(bgHex)
	} else {
		bg = templates[template].bg
	}
	dc.SetColor(bg)
	dc.Clear()

	if bgImagePath != "" {
		img, err := gg.LoadImage(bgImagePath)
		if err == nil {
			dc.DrawImage(img, 0, 0)
		}
	}

	var textCol color.RGBA
	if textHex != "" {
		textCol = hexToColor(textHex)
	} else {
		textCol = templates[template].text
	}
	dc.SetColor(textCol)

	dc.LoadFontFace("/usr/share/fonts/truetype/liberation/LiberationSans-Bold.ttf", 60)
	dc.DrawStringAnchored("❄️ Happy New Year! ❄️", 400, 80, 0.5, 0.5)

	dc.LoadFontFace("/usr/share/fonts/truetype/liberation/LiberationSans-Bold.ttf", 70)
	dc.DrawStringAnchored(name, 400, 200, 0.5, 0.5)

	// Wrap message
	dc.LoadFontFace("/usr/share/fonts/truetype/liberation/LiberationSans-Italic.ttf", 30)
	words := strings.Fields(message)
	var lines []string
	line := ""
	for _, w := range words {
		if len(line)+len(w)+1 <= 30 {
			if line == "" {
				line = w
			} else {
				line += " " + w
			}
		} else {
			lines = append(lines, line)
			line = w
		}
	}
	if line != "" {
		lines = append(lines, line)
	}
	y := 380
	for _, l := range lines {
		dc.DrawStringAnchored(l, 400, float64(y), 0.5, 0.5)
		y += 40
	}

	dc.LoadFontFace("/usr/share/fonts/truetype/liberation/LiberationSans-Regular.ttf", 40)
	dc.DrawStringAnchored("⭐  🎄  🎆", 400, 520, 0.5, 0.5)

	if photoPath != "" {
		img, err := gg.LoadImage(photoPath)
		if err == nil {
			dc.DrawImage(img, 800-150-20, 20)
		}
	}

	return dc.SavePNG(output)
}

func main() {
	name := flag.String("n", "", "Recipient's name")
	message := flag.String("m", "", "Custom message")
	template := flag.Int("t", 1, "Template (1-3)")
	bgHex := flag.String("bg", "", "Background color (hex)")
	textHex := flag.String("text", "", "Text color (hex)")
	photo := flag.String("photo", "", "Photo path")
	bgImage := flag.String("bg-image", "", "Background image path")
	output := flag.String("o", "ny_card.png", "Output file")
	html := flag.String("html", "", "HTML output")
	flag.Parse()

	if *name == "" || *message == "" {
		fmt.Println("Error: -n and -m are required")
		os.Exit(1)
	}
	if *template < 1 || *template > 3 {
		fmt.Println("Template must be 1-3")
		os.Exit(1)
	}

	err := createCard(*name, *message, *template, *bgHex, *textHex, *photo, *bgImage, *output)
	if err != nil {
		fmt.Println("Error:", err)
		os.Exit(1)
	}
	fmt.Printf("Card saved to %s\n", *output)

	if *html != "" {
		bg := *bgHex
		if bg == "" {
			bg = "#FFFFFF"
		}
		txt := *textHex
		if txt == "" {
			txt = "#000000"
		}
		htmlContent := fmt.Sprintf(`<!DOCTYPE html>
<html><head><title>New Year Card</title>
<style>body{font-family:sans-serif;text-align:center;background:#1a1a2e;}
.card{background:%s;color:%s;border-radius:20px;padding:40px;max-width:600px;margin:50px auto;box-shadow:0 4px 8px rgba(0,0,0,0.3);}
h1{font-size:3em;}.name{font-size:2.5em;}.msg{font-size:1.5em;}</style>
</head><body><div class="card">
<h1>❄️ Happy New Year! ❄️</h1>
<div class="name">%s</div>
<div class="msg">%s</div>
<div style="font-size:3em;">⭐ 🎄 🎆</div>
</div></body></html>`, bg, txt, *name, *message)
		os.WriteFile(*html, []byte(htmlContent), 0644)
		fmt.Printf("HTML card saved to %s\n", *html)
	}
}
