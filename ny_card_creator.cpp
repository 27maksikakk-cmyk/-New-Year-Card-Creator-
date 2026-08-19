// ny_card_creator.cpp
#include <iostream>
#include <fstream>
#include <string>
#include <vector>
#include <map>
#include <cstring>
#include <cmath>
#include <CImg.h>
using namespace cimg_library;

std::map<int, std::pair<unsigned char[3], unsigned char[3]>> templates = {
    {1, { {255,255,255}, {0,0,0} }},
    {2, { {0,100,150}, {255,215,0} }},
    {3, { {200,50,50}, {255,255,255} }}
};

void hexToRGB(const std::string& hex, unsigned char& r, unsigned char& g, unsigned char& b) {
    std::string h = hex;
    if (h[0] == '#') h = h.substr(1);
    r = std::stoi(h.substr(0,2), nullptr, 16);
    g = std::stoi(h.substr(2,2), nullptr, 16);
    b = std::stoi(h.substr(4,2), nullptr, 16);
}

void createCard(const std::string& name, const std::string& message, int templateId,
                const std::string& bgHex, const std::string& textHex,
                const std::string& photoPath, const std::string& bgImagePath,
                const std::string& output) {
    int width = 800, height = 600;
    CImg<unsigned char> img(width, height, 1, 3);

    unsigned char bgR=255,bgG=255,bgB=255;
    if (!bgHex.empty()) hexToRGB(bgHex, bgR, bgG, bgB);
    else {
        auto t = templates[templateId];
        bgR = t.first[0]; bgG = t.first[1]; bgB = t.first[2];
    }
    if (!bgImagePath.empty()) {
        CImg<unsigned char> bgImg(bgImagePath.c_str());
        bgImg.resize(width, height);
        img = bgImg;
    } else {
        img.fill(bgR, bgG, bgB);
    }

    unsigned char tR=0,tG=0,tB=0;
    if (!textHex.empty()) hexToRGB(textHex, tR, tG, tB);
    else {
        auto t = templates[templateId];
        tR = t.second[0]; tG = t.second[1]; tB = t.second[2];
    }

    const unsigned char black[] = { tR, tG, tB };
    img.draw_text(200, 50, "❄️ Happy New Year! ❄️", black, nullptr, 1, 60);
    img.draw_text(200, 150, name.c_str(), black, nullptr, 1, 70);

    std::vector<std::string> lines;
    std::string line;
    std::istringstream words(message);
    std::string word;
    while (words >> word) {
        if (line.length() + word.length() + 1 <= 30) {
            if (!line.empty()) line += " ";
            line += word;
        } else {
            lines.push_back(line);
            line = word;
        }
    }
    if (!line.empty()) lines.push_back(line);
    int y = 330;
    for (const auto& l : lines) {
        img.draw_text(200, y, l.c_str(), black, nullptr, 1, 30);
        y += 40;
    }
    img.draw_text(200, 470, "⭐  🎄  🎆", black, nullptr, 1, 40);

    if (!photoPath.empty()) {
        CImg<unsigned char> photo(photoPath.c_str());
        photo.resize(150, 150);
        img.draw_image(width - photo.width() - 20, 20, photo);
    }

    img.save_png(output.c_str());
    std::cout << "Card saved to " << output << std::endl;
}

int main(int argc, char* argv[]) {
    std::string name, message, output = "ny_card.png";
    int templateId = 1;
    std::string bgHex, textHex, photoPath, bgImagePath, htmlFile;

    for (int i = 1; i < argc; i++) {
        std::string arg = argv[i];
        if (arg == "-n" && i+1 < argc) name = argv[++i];
        else if (arg == "-m" && i+1 < argc) message = argv[++i];
        else if (arg == "-t" && i+1 < argc) templateId = std::stoi(argv[++i]);
        else if (arg == "--bg" && i+1 < argc) bgHex = argv[++i];
        else if (arg == "--text" && i+1 < argc) textHex = argv[++i];
        else if (arg == "--photo" && i+1 < argc) photoPath = argv[++i];
        else if (arg == "--bg-image" && i+1 < argc) bgImagePath = argv[++i];
        else if (arg == "-o" && i+1 < argc) output = argv[++i];
        else if (arg == "--html" && i+1 < argc) htmlFile = argv[++i];
    }
    if (name.empty() || message.empty()) {
        std::cerr << "Error: -n and -m are required" << std::endl;
        return 1;
    }
    if (templateId < 1 || templateId > 3) {
        std::cerr << "Template must be 1-3" << std::endl;
        return 1;
    }

    createCard(name, message, templateId, bgHex, textHex, photoPath, bgImagePath, output);

    if (!htmlFile.empty()) {
        std::string bg = bgHex.empty() ? "#FFFFFF" : bgHex;
        std::string txt = textHex.empty() ? "#000000" : textHex;
        std::ofstream f(htmlFile);
        f << "<!DOCTYPE html><html><head><title>New Year Card</title>"
          << "<style>body{font-family:sans-serif;text-align:center;background:#1a1a2e;}"
          << ".card{background:" << bg << ";color:" << txt << ";border-radius:20px;padding:40px;max-width:600px;margin:50px auto;box-shadow:0 4px 8px rgba(0,0,0,0.3);}"
          << "h1{font-size:3em;}.name{font-size:2.5em;}.msg{font-size:1.5em;}</style>"
          << "</head><body><div class=\"card\">"
          << "<h1>❄️ Happy New Year! ❄️</h1>"
          << "<div class=\"name\">" << name << "</div>"
          << "<div class=\"msg\">" << message << "</div>"
          << "<div style=\"font-size:3em;\">⭐ 🎄 🎆</div>"
          << "</div></body></html>";
        f.close();
        std::cout << "HTML card saved to " << htmlFile << std::endl;
    }
    return 0;
}
