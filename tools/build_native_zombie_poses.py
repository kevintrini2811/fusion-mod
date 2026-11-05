"""Build variant portraits from native Spine setup poses, without Unity."""
import copy
import json
import math
import subprocess
from pathlib import Path

from render_zombie_pose import render

ROOT = Path(__file__).resolve().parents[1]
REFERENCE = ROOT / 'references/zombie-accuracy-13-0-9'
SOURCE = REFERENCE / 'spine'


def decode(name, output, skins, animation='-', atlas=None):
    path = REFERENCE / (output + '.json')
    args = ['node', str(ROOT/'tools/decode_zombie_pose.mjs'), str(SOURCE), name,
            str(path), skins, animation]
    if atlas:
        args.append(atlas)
    subprocess.run(args, check=True)
    return json.loads(path.read_text())


def build():
    output = REFERENCE / 'portraits'
    output.mkdir(exist_ok=True)
    transforms = json.loads((REFERENCE/'head-transforms.json').read_text())
    assets = json.loads((REFERENCE/'head-skeleton-assets.json').read_text())
    body = decode('Zombie', 'body', 'default,BackArm,FrontArm', 'walk')
    neck = next(b for b in body['bones'] if b['name'] == 'Zombie_neck')
    recipe = {
        26: ('PeaShooter', 'default,Peashooter', 'PeaShooter_Zombotany'),
        27: ('Wallnut', 'default,Wallnut', 'Wallnut'),
        28: ('Jalapeno', 'default', 'Jalapeno'),
        29: ('GatlingPea', 'default', 'GatlingPea_Zombotany'),
        30: ('Squash', 'default', 'Squash'),
        31: ('Tallnut', 'default,Tallnut', 'Tallnut'),
    }
    recipes = {}
    for kind, (name, skins, atlas) in recipe.items():
        key = 'P_Reanim_Zombotany'+name
        if (SOURCE/(key+'.skel')).exists():
            skeleton = key
        else:
            skeleton = name
        head = decode(skeleton, 'head-'+str(kind), skins, '-', atlas)
        render_name = key+'_Render'
        renderer = transforms.get(render_name, [])
        local = next((c['data']['m_LocalPosition'] for c in renderer if c['type']=='Transform'),
                     {'x': 0, 'y': -106.36})
        controller = next(c['data'] for c in transforms[key] if 'm_visualOffset' in c['data'])
        scale = assets.get(render_name, {}).get('scale', .564)
        visual = controller['m_visualOffset']
        angle = math.radians(controller['m_attachedRotationOffset'])+math.atan2(neck['c'], neck['a'])
        cosine, sine = math.cos(angle), math.sin(angle)
        parts = copy.deepcopy(body['attachments'])
        for part in parts:
            part['vertices'] = [value*.564 for value in part['vertices']]
        for part in head['attachments']:
            part = copy.deepcopy(part)
            result = []
            for x, y in zip(part['vertices'][::2], part['vertices'][1::2]):
                # The plant head faces the approaching plants, opposite to a
                # planted Peashooter. Use the actual native prefab offsets.
                x, y = -(x*scale+local['x']), y*scale+local['y']
                result += [neck['x']*.564+x*cosine-y*sine+visual['x'],
                           neck['y']*.564+x*sine+y*cosine+visual['y']]
            part['vertices'] = result
            parts.append(part)
        render(parts, SOURCE, 2).save(output/(str(kind)+'.png'))
        recipes[kind] = dict(skeleton=skeleton, skins=skins, atlas=atlas,
                             scale=scale, render_position=local, visual_offset=visual,
                             attachment='Zombie_neck', rotation=controller['m_attachedRotationOffset'])
    garg = decode('Zombie_gargantuar', 'giga',
        'default,Zombie_Gargantuar_Body1,Zombie_Gargantuar_Head_Redeye,Zombie_Gargantuar_Imp,'
        'Zombie_Gargantuar_Outerarm_Uninjured,Zombie_Gargantuar_Outerfoot_Uninjured,Zombie_Gargantuar_Pole', 'idle')
    render(garg['attachments'], SOURCE).save(output/'32.png')
    target = decode('Zombie_Target', 'target',
        'default,Zombie_Target_01,Zombie_Target_Head,Zombie_Target_Innerarm,Zombie_Target_Innerhand,'
        'Zombie_Target_Outerarm_Lower,Zombie_Target_Outerarm_Upper,Zombie_Target_Outerhand', 'idle')
    render(target['attachments'], SOURCE).save(output/'34.png')
    (REFERENCE/'pose-recipes.json').write_text(json.dumps(recipes, indent=2)+'\n')


if __name__ == '__main__':
    build()
