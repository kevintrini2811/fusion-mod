"""Compose original Fusion artwork with native lawn and native zombie body.

Uses the existing offline model/Spine exporters; never starts the game.
"""
import copy
import hashlib
import json
import math
from pathlib import Path

from PIL import Image, ImageOps
from preview_snow_repeater import Preview
from render_zombie_pose import render

ROOT = Path(__file__).resolve().parents[1]


def cutout(name, subtree=None):
    preview = Preview(name, "idle")
    # Reuse the exact preview camera and pose, changing only canvas alpha.
    draw = preview.draw
    def draw_cutout(pose, camera):
        if subtree is not None:
            nodes = preview.model["Nodes"]
            head = next(i for i, node in enumerate(nodes) if node["Name"] == subtree)
            for i, node in enumerate(nodes):
                ancestor = i
                while ancestor >= 0 and ancestor != head:
                    ancestor = nodes[ancestor]["Parent"]
                # Hypno Pea's nozzle is a sibling of PeaShooter_Head.
                if ancestor < 0 and node["Name"] != "Shoot":
                    pose.enabled[i] = False
            # Fit the isolated head, not the whole plant's ground camera. The
            # latter can clip its nozzle before the transparent crop is made.
            x0, y0, x1, y1 = preview.bounds(pose)
            scale = min(400 / (x1 - x0), 340 / (y1 - y0))
            camera = [scale, 0, 210 - scale * (x0 + x1) / 2,
                      0, -scale, 180 + scale * (y0 + y1) / 2]
        return draw(pose, camera, (0, 0, 0, 0))
    preview.draw = draw_cutout
    return preview.render(0)


def build():
    reference = ROOT / 'references/presentation-20-0-0'
    reference.mkdir(exist_ok=True)
    art = cutout('chompzilla')
    art = ImageOps.contain(art.crop(art.getbbox()), (440, 440), Image.Resampling.LANCZOS)
    lawn = ROOT.parent / 'plant-library/assets/lawn.png'
    icon = ImageOps.fit(Image.open(lawn).convert('RGBA'), (512, 512), method=Image.Resampling.LANCZOS)
    icon.putalpha(255)
    icon.alpha_composite(art, ((512 - art.width) // 2, 512 - art.height - 28))
    icon.save(ROOT / 'assets/menu-icon.png')

    source = ROOT / 'references/zombie-accuracy-13-0-9'
    body = json.loads((source / 'body.json').read_text())
    head = json.loads((source / 'head-26.json').read_text())
    transforms = json.loads((source / 'head-transforms.json').read_text())
    assets = json.loads((source / 'head-skeleton-assets.json').read_text())
    key = 'P_Reanim_ZombotanyPeaShooter'
    renderer = key + '_Render'
    local = next(c['data']['m_LocalPosition'] for c in transforms[renderer] if c['type'] == 'Transform')
    controller = next(c['data'] for c in transforms[key] if 'm_visualOffset' in c['data'])
    neck = next(b for b in body['bones'] if b['name'] == 'Zombie_neck')
    scale = assets[renderer]['scale']
    visual = controller['m_visualOffset']
    angle = math.radians(controller['m_attachedRotationOffset']) + math.atan2(neck['c'], neck['a'])
    cosine, sine = math.cos(angle), math.sin(angle)
    points = []
    for part in head['attachments']:
        if part['color'][3] <= .001 or 'locator' in part['name'].lower() or part['name'] == 'anim_sprout':
            continue
        for x, y in zip(part['vertices'][::2], part['vertices'][1::2]):
            x, y = -(x * scale + local['x']), y * scale + local['y']
            points.append((neck['x'] * .564 + x * cosine - y * sine + visual['x'],
                           neck['y'] * .564 + x * sine + y * cosine + visual['y']))
    x0, x1 = min(x for x, _ in points), max(x for x, _ in points)
    y0, y1 = min(y for _, y in points), max(y for _, y in points)
    art = cutout('hypno-pea', 'PeaShooter_Head')
    art = ImageOps.mirror(art.crop(art.getbbox()))
    cutout_path = reference / 'hypno-head.png'
    art.save(cutout_path)
    # Fit the full imported head to the native head envelope, keeping its aspect.
    ratio = min((x1 - x0) / art.width, (y1 - y0) / art.height)
    width, height = art.width * ratio, art.height * ratio
    center_x, center_y = (x0 + x1) / 2, (y0 + y1) / 2
    x0, x1 = center_x - width / 2, center_x + width / 2
    y0, y1 = center_y - height / 2, center_y + height / 2
    parts = copy.deepcopy(body['attachments'])
    for part in parts:
        part['vertices'] = [value * .564 for value in part['vertices']]
    parts.append(dict(name='fusion-hypno-pea-head', page=str(cutout_path.resolve()),
                      vertices=[x0, y0, x1, y0, x1, y1, x0, y1],
                      uvs=[0, 1, 1, 1, 1, 0, 0, 0], triangles=[0, 1, 2, 2, 3, 0], color=[1, 1, 1, 1]))
    output = ROOT / 'assets/hypno-zombie.png'
    render(parts, source / 'spine', 2).save(output)
    paths = [lawn, ROOT / 'assets/chompzilla/model.json', ROOT / 'assets/hypno-pea/model.json',
             source / 'body.json', source / 'head-26.json', Path(__file__)]
    proof = dict(game_started=False,
                 inputs={str(path): hashlib.sha256(path.read_bytes()).hexdigest() for path in paths},
                 outputs={str(path): hashlib.sha256(path.read_bytes()).hexdigest()
                          for path in [ROOT / 'assets/menu-icon.png', output]})
    (reference / 'sources.json').write_text(json.dumps(proof, indent=2) + '\n')


if __name__ == '__main__':
    build()
