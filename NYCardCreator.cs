// NYCardCreator.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.Fonts;

class NYCardCreator
{
    static Dictionary<int, (Color bg, Color text)> Templates = new()
    {
        {1, (Color.White, Color.Black)},
        {2, (Color.FromRgb(0,100,150), Color.FromRgb(255,215,0))},
        {3, (Color.FromRgb(200,50,50), Color.White)}
    };

    static Color HexToColor(string hex)
    {
        hex = hex.TrimStart('#');
        return Color.FromRgb(
            Convert.ToByte(hex.Substring(0,2), 16),
            Convert.ToByte(hex.Substring(2,2), 16),
            Convert.ToByte(hex.Substring(4,2), 16)
        );
    }

    static void CreateCard(string name, string message, int template,
                           string bgHex, string textHex, string photoPath,
                           string bgImagePath, string output)
    {
        int width = 800, height = 600;
        using var image = new Image<Rgba32>(width, height);

        Color bgColor = bgHex != null ? HexToColor(bgHex) : Templates[template].bg;
        if (bgImagePath != null && File.Exists(bgImagePath))
        {
            using var bgImg = Image.Load(bgImagePath);
            bgImg.Mutate(x => x.Resize(width, height));
            image.Mutate(ctx => ctx.DrawImage(bgImg, new Point(0,0), 1f));
        }
        else
        {
            image.Mutate(ctx => ctx.Fill(bgColor));
        }

        Color textColor = textHex != null ? HexToColor(textHex) : Templates[template].text;

        var fontCollection = new FontCollection();
        FontFamily family;
        try {
            family = SystemFonts.Families.First(f => f.Name.Contains("Liberation") || f.Name.Contains("Arial"));
        } catch {
            family = SystemFonts.Families.First();
        }
        var fontBold = family.CreateFont(60, FontStyle.Bold);
        var fontItalic = family.CreateFont(30, FontStyle.Italic);
        var fontRegular = family.CreateFont(40, FontStyle.Regular);

        var options = new TextOptions(fontBold) { Origin = new PointF(width/2, 80), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Top };

        image.Mutate(ctx => {
            ctx.DrawText(options, "❄️ Happy New Year! ❄️", textColor);

            options.Font = fontBold;
            options.Origin = new PointF(width/2, 200);
            ctx.DrawText(options, name, textColor);

            var words = message.Split(' ');
            var lines = new List<string>();
            var line = "";
            foreach (var w in words) {
                if ((line + " " + w).Length <= 30) {
                    line = string.IsNullOrEmpty(line) ? w : line + " " + w;
                } else {
                    lines.Add(line);
                    line = w;
                }
            }
            if (!string.IsNullOrEmpty(line)) lines.Add(line);
            options.Font = fontItalic;
            int y = 380;
            foreach (var l in lines) {
                options.Origin = new PointF(width/2, y);
                ctx.DrawText(options, l, textColor);
                y += 40;
            }

            options.Font = fontRegular;
            options.Origin = new PointF(width/2, 520);
            ctx.DrawText(options, "⭐  🎄  🎆", textColor);

            if (photoPath != null && File.Exists(photoPath)) {
                using var photo = Image.Load(photoPath);
                int size = 150;
                photo.Mutate(x => x.Resize(size, size));
                ctx.DrawImage(photo, new Point(width - size - 20, 20), 1f);
            }
        });

        image.SaveAsPng(output);
        Console.WriteLine($"Card saved to {output}");
    }

    static void Main(string[] args)
    {
        var parsed = ParseArgs(args);
        if (!parsed.ContainsKey("n") || !parsed.ContainsKey("m")) {
            Console.Error.WriteLine("Error: -n and -m are required");
            return;
        }
        string name = parsed["n"];
        string message = parsed["m"];
        int template = parsed.ContainsKey("t") ? int.Parse(parsed["t"]) : 1;
        if (template < 1 || template > 3) {
            Console.Error.WriteLine("Template must be 1-3");
            return;
        }
        string bg = parsed.GetValueOrDefault("bg");
        string text = parsed.GetValueOrDefault("text");
        string photo = parsed.GetValueOrDefault("photo");
        string bgImage = parsed.GetValueOrDefault("bg-image");
        string output = parsed.GetValueOrDefault("o", "ny_card.png");

        CreateCard(name, message, template, bg, text, photo, bgImage, output);

        if (parsed.ContainsKey("html")) {
            string htmlFile = parsed["html"];
            string bgColor = bg ?? "#FFFFFF";
            string textColor = text ?? "#000000";
            var html = $@"<!DOCTYPE html>
<html><head><title>New Year Card</title>
<style>body{{font-family:sans-serif;text-align:center;background:#1a1a2e;}}
.card{{background:{bgColor};color:{textColor};border-radius:20px;padding:40px;max-width:600px;margin:50px auto;box-shadow:0 4px 8px rgba(0,0,0,0.3);}}
h1{{font-size:3em;}}.name{{font-size:2.5em;}}.msg{{font-size:1.5em;}}</style>
</head><body><div class=""card"">
<h1>❄️ Happy New Year! ❄️</h1>
<div class=""name"">{name}</div>
<div class=""msg"">{message}</div>
<div style=""font-size:3em;"">⭐ 🎄 🎆</div>
</div></body></html>";
            File.WriteAllText(htmlFile, html);
            Console.WriteLine($"HTML card saved to {htmlFile}");
        }
    }

    static Dictionary<string, string> ParseArgs(string[] args)
    {
        var dict = new Dictionary<string, string>();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i].StartsWith("-"))
            {
                string key = args[i].TrimStart('-');
                if (i + 1 < args.Length && !args[i+1].StartsWith("-"))
                    dict[key] = args[++i];
                else
                    dict[key] = "";
            }
        }
        return dict;
    }
}
