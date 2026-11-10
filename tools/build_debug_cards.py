"""Build the debug packet atlas from existing native packet and Fusion assets.
Reads images only; never starts the game. Original source assets stay intact.
"""
import hashlib
import json
from pathlib import Path
import xml.etree.ElementTree as ET
from PIL import Image, ImageDraw, ImageOps

ROOT = Path(__file__).resolve().parents[1]
OUT = ROOT.parent / "debug-mod/assets/debug-cards"
REFERENCES = ROOT / "references/chooser-fix-13-0-5"


def build():
    resources = {}
    for item in ET.parse(ROOT / "FusionMod.csproj").iter("EmbeddedResource"):
        pattern, logical = item.attrib.get("Include"), item.attrib.get("LogicalName")
        if not pattern or not logical:
            continue
        for path in ROOT.glob(pattern):
            if path.is_file():
                name = logical.replace("%(Filename)", path.stem).replace("%(Extension)", path.suffix)
                resources[name] = path
    catalog = json.loads((ROOT / "assets/almanac/catalog.json").read_text())
    portraits = ["FusionMod.Almanac." + entry["Portrait"] for entry in catalog]
    portraits += ["FusionMod.Bucket.sprite-15928.png", "FusionMod.FootballHelmet.sprite-15508.png", "FusionMod.EyesZombieHead.sprite-4582.png"]
    portraits = sorted(set(portraits))
    # Preserve the real packet border/header. The native empty store packet
    # supplies the unoccupied paper, rather than leaving an old plant behind.
    native = Image.open(REFERENCES / "native-peashooter.png").convert("RGBA")
    paper = Image.open(REFERENCES / "sprite-8921823210777159781.png").convert("RGBA")
    paper = paper.crop((12, 33, 167, 184)).resize((366, 474), Image.Resampling.LANCZOS)
    gray = ImageOps.grayscale(paper)
    tinted = ImageOps.colorize(gray, "#7a9c55", "#d9f0b5").convert("RGBA")
    # Restore the unoccupied frame area too: the original foreground overlaps
    # the inner border, and must not leave fragments beside imported portraits.
    draw = ImageDraw.Draw(native)
    draw.rectangle((23, 66, 428, 565), fill=native.getpixel((20, 80)))
    draw.rectangle((33, 72, 409, 559), fill="#426d25")
    native.paste(tinted, (39, 78))
    packet = native.resize((128, 176), Image.Resampling.LANCZOS)
    OUT.mkdir(parents=True, exist_ok=True)
    packet.save(OUT / "packet-background.png")
    width, height, columns = 128, 176, 16
    rows = (len(portraits) + columns - 1) // columns
    atlas = Image.new("RGBA", (columns * width, rows * height))
    entries, hashes = [], {}
    for i, resource in enumerate(portraits):
        path = resources[resource]
        portrait = Image.open(path).convert("RGBA")
        portrait.thumbnail((104, 123), Image.Resampling.LANCZOS)
        card = packet.copy()
        card.alpha_composite(portrait, ((width - portrait.width) // 2, 28 + (123 - portrait.height) // 2))
        x, y = i % columns * width, i // columns * height
        atlas.alpha_composite(card, (x, y))
        entries.append(dict(Resource=resource, X=x, Y=atlas.height-y-height, Width=width, Height=height))
        hashes[resource] = hashlib.sha256(path.read_bytes()).hexdigest()
    atlas.save(OUT / "fusion-packets.png")
    (OUT / "fusion-packets.json").write_text(json.dumps(entries, indent=2)+"\n")
    (REFERENCES / "atlas-inputs.json").write_text(json.dumps(hashes, indent=2)+"\n")
    print(f"{len(entries)} cards; atlas {atlas.width}x{atlas.height}; {atlas.width*atlas.height*4} decoded bytes")


if __name__ == "__main__":
    build()
