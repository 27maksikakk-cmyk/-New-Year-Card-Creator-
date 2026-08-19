# ny_card_creator.php
<?php
$templates = [
    1 => ['bg' => '#FFFFFF', 'text' => '#000000'],
    2 => ['bg' => '#006496', 'text' => '#FFD700'],
    3 => ['bg' => '#C83232', 'text' => '#FFFFFF']
];

function hexToRgb($hex) {
    $hex = str_replace('#', '', $hex);
    if (strlen($hex) == 6) {
        return [hexdec($hex[0].$hex[1]), hexdec($hex[2].$hex[3]), hexdec($hex[4].$hex[5])];
    }
    return [0,0,0];
}

function createCard($name, $message, $template, $bgHex, $textHex, $photoPath, $bgImagePath, $output) {
    $width = 800; $height = 600;
    $img = imagecreatetruecolor($width, $height);

    if ($bgImagePath && file_exists($bgImagePath)) {
        $bgImg = imagecreatefromstring(file_get_contents($bgImagePath));
        if ($bgImg) {
            imagecopyresampled($img, $bgImg, 0,0,0,0, $width, $height, imagesx($bgImg), imagesy($bgImg));
            imagedestroy($bgImg);
        }
    } else {
        $bg = $bgHex ?: $templates[$template]['bg'];
        list($r,$g,$b) = hexToRgb($bg);
        $bgColor = imagecolorallocate($img, $r, $g, $b);
        imagefilledrectangle($img, 0, 0, $width, $height, $bgColor);
    }

    $text = $textHex ?: $templates[$template]['text'];
    list($r,$g,$b) = hexToRgb($text);
    $textColor = imagecolorallocate($img, $r, $g, $b);

    $font = '/usr/share/fonts/truetype/liberation/LiberationSans-Bold.ttf';
    $fontReg = '/usr/share/fonts/truetype/liberation/LiberationSans-Regular.ttf';
    $fontItalic = '/usr/share/fonts/truetype/liberation/LiberationSans-Italic.ttf';

    if (file_exists($font)) {
        imagettftext($img, 60, 0, 200, 80, $textColor, $font, '❄️ Happy New Year! ❄️');
        imagettftext($img, 70, 0, 200, 200, $textColor, $font, $name);
        $wrapped = wordwrap($message, 30, "\n");
        imagettftext($img, 30, 0, 200, 380, $textColor, $fontItalic, $wrapped);
        imagettftext($img, 40, 0, 200, 520, $textColor, $fontReg, '⭐  🎄  🎆');
    } else {
        imagestring($img, 5, 200, 50, 'Happy New Year!', $textColor);
        imagestring($img, 5, 200, 100, $name, $textColor);
        imagestring($img, 3, 200, 200, $message, $textColor);
    }

    if ($photoPath && file_exists($photoPath)) {
        $photo = imagecreatefromstring(file_get_contents($photoPath));
        if ($photo) {
            $size = 150;
            imagecopyresampled($img, $photo, $width - $size - 20, 20, 0,0, $size, $size, imagesx($photo), imagesy($photo));
            imagedestroy($photo);
        }
    }

    imagepng($img, $output);
    imagedestroy($img);
    echo "Card saved to $output\n";
}

$opts = getopt("n:m:t:o:", ["name:", "message:", "template:", "bg:", "text:", "photo:", "bg-image:", "output:", "html:"]);
$name = $opts['n'] ?? $opts['name'] ?? null;
$message = $opts['m'] ?? $opts['message'] ?? null;
$template = isset($opts['t']) ? (int)$opts['t'] : (isset($opts['template']) ? (int)$opts['template'] : 1);
$bg = $opts['bg'] ?? null;
$text = $opts['text'] ?? null;
$photo = $opts['photo'] ?? null;
$bgImage = $opts['bg-image'] ?? null;
$output = $opts['o'] ?? $opts['output'] ?? 'ny_card.png';
$html = $opts['html'] ?? null;

if (!$name || !$message) {
    fwrite(STDERR, "Error: -n and -m are required\n");
    exit(1);
}
if ($template < 1 || $template > 3) {
    fwrite(STDERR, "Template must be 1-3\n");
    exit(1);
}

createCard($name, $message, $template, $bg, $text, $photo, $bgImage, $output);

if ($html) {
    $bgColor = $bg ?: '#FFFFFF';
    $txtColor = $text ?: '#000000';
    $htmlContent = <<<HTML
<!DOCTYPE html>
<html><head><title>New Year Card</title>
<style>body{font-family:sans-serif;text-align:center;background:#1a1a2e;}
.card{background:{$bgColor};color:{$txtColor};border-radius:20px;padding:40px;max-width:600px;margin:50px auto;box-shadow:0 4px 8px rgba(0,0,0,0.3);}
h1{font-size:3em;}.name{font-size:2.5em;}.msg{font-size:1.5em;}</style>
</head><body><div class="card">
<h1>❄️ Happy New Year! ❄️</h1>
<div class="name">{$name}</div>
<div class="msg">{$message}</div>
<div style="font-size:3em;">⭐ 🎄 🎆</div>
</div></body></html>
HTML;
    file_put_contents($html, $htmlContent);
    echo "HTML card saved to $html\n";
}
?>
