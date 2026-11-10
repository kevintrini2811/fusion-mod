"""Export native static zombie art; never instantiate game actors or run Unity."""
import hashlib
import json
from pathlib import Path

import UnityPy
from PIL import Image

ROOT = Path(__file__).resolve().parents[1]
GAME = Path('/mnt/FastGaming1/SteamLibrary/steamapps/common/PVZ Replanted')
BUNDLES = GAME / 'Replanted_Data/StreamingAssets/aa/StandaloneWindows64'


def build():
    paths = sorted(set(BUNDLES.glob('uiatlas*.bundle'))
                   | set(BUNDLES.glob('defaultlocalgroup_assets_assets/art/characters/previews*.bundle'))
                   | set(BUNDLES.glob('defaultlocalgroup_assets_ui/sprites*.bundle')))
    definitions = next(BUNDLES.glob('defaultlocalgroup_assets_zombiedefinitiondata_*.bundle'))
    paths.append(definitions)
    environment = UnityPy.load(*(str(path) for path in paths))
    sprites, types = {}, {}
    for obj in environment.objects:
        if obj.type.name == 'Sprite':
            data = obj.read()
            sprites[data.m_Name] = obj
        elif obj.type.name == 'MonoBehaviour':
            data = obj.read_typetree()
            if 'm_zombieType' in data and 'm_previewSprite' in data:
                types[data['m_zombieType']] = data
    source = {}
    def image(name):
        obj = sprites[name]
        source[name] = obj.path_id
        return obj.read().image.convert('RGBA')

    portraits = {}
    for kind, definition in sorted(types.items()):
        name = definition['m_previewSprite']['m_SubObjectName']
        if name:
            portraits[kind] = image(name)
    portraits[25] = image('SPR_Almanac_Zombies_DrZomboss')
    # Variants missing a faithful static preview are rendered offline from
    # native skeletons, skins, attachment sockets and prefab transforms.
    pose_source = ROOT / 'references/zombie-accuracy-13-0-9/portraits'
    pose_inputs = {}
    for kind in (26, 27, 28, 29, 30, 31, 32, 34):
        path = pose_source / (str(kind) + '.png')
        portraits[kind] = Image.open(path).convert('RGBA')
        pose_inputs[str(path.relative_to(ROOT))] = hashlib.sha256(path.read_bytes()).hexdigest()
    portraits[33] = portraits[0].copy()
    if set(portraits) != set(range(37)):
        raise ValueError('Missing native zombie types: ' + str(set(range(37)) - set(portraits)))

    output = ROOT / 'assets/debug-cards'
    packet = Image.open(output / 'packet-background.png').convert('RGBA')
    # Raw images serve lawn placement; packet images serve chooser and bank.
    width, height, columns = 256, 352, 16
    atlas = Image.new('RGBA', (columns * width, 5 * height))
    entries = []
    for kind, original in sorted(portraits.items()):
        raw = Image.new('RGBA', (256, 320))
        art = original.copy()
        art.thumbnail((240, 308), Image.Resampling.LANCZOS)
        raw.alpha_composite(art, ((256-art.width)//2, 320-art.height))
        card = packet.copy()
        art = original.copy()
        art.thumbnail((104, 123), Image.Resampling.LANCZOS)
        card.alpha_composite(art, ((128-art.width)//2, 28+(123-art.height)//2))
        for offset, (role, artwork) in enumerate((('Portrait', raw), ('Packet', card))):
            slot = kind*2+offset
            x, y = slot % columns * width, slot // columns * height
            atlas.alpha_composite(artwork, (x, y))
            entries.append(dict(Type=kind, Role=role, X=x, Y=atlas.height-y-artwork.height,
                                Width=artwork.width, Height=artwork.height))
    atlas.save(output / 'zombie-packets.png')
    (output / 'zombie-packets.json').write_text(json.dumps(entries, indent=2)+'\n')
    proof = dict(bundles={str(p): hashlib.sha256(p.read_bytes()).hexdigest() for p in paths},
                 sprites=source, native_pose_inputs=pose_inputs,
                 shared_native_images={'33': 'Normal; profile-dependent Zombatar is not rendered'},
                 outputs={name: hashlib.sha256((output / name).read_bytes()).hexdigest()
                          for name in ('zombie-packets.png', 'zombie-packets.json')},
                 game_started=False)
    (ROOT / 'references/zombie-accuracy-13-0-9/static-zombie-assets.json').write_text(json.dumps(proof, indent=2)+'\n')
    import shutil
    debug_output = ROOT.parent / 'debug-mod/assets/debug-cards'
    debug_output.mkdir(parents=True, exist_ok=True)
    for name in ('zombie-packets.png', 'zombie-packets.json'):
        shutil.copy2(output / name, debug_output / name)
    print(f'{len(portraits)} zombie types, {len(entries)} cached sprites; {atlas.width}x{atlas.height}')


if __name__ == '__main__':
    build()
