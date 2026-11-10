"""Export native armor attachments for imported Eyes Zombie heads, without Unity."""
import json
import subprocess
from pathlib import Path
from render_zombie_pose import render
from PIL import Image
from preview_snow_repeater import Preview

ROOT = Path(__file__).resolve().parents[1]
SOURCE = ROOT / 'references/zombie-accuracy-13-0-9/spine'
OUTPUT = ROOT / 'assets/eyes-armor'

def main():
    OUTPUT.mkdir(exist_ok=True)
    fits = {}
    for kind in ('cone', 'bucket'):
        for stage in ('', '2', '3'):
            name = kind + stage
            path = OUTPUT / (name + '.json')
            subprocess.run(['node', str(ROOT/'tools/decode_zombie_pose.mjs'), str(SOURCE), 'Zombie',
                            str(path), 'default,Head,'+name, 'walk'], check=True, capture_output=True)
            data = json.loads(path.read_text())
            parts = [p for p in data['attachments'] if p['slot'] == 'anim_'+kind]
            if len(parts) != 1:
                raise ValueError('Missing native armor attachment: '+name)
            image = render(parts, SOURCE, 2)
            image.save(OUTPUT/(name+'.png'))
            head = next(p for p in data['attachments'] if p['slot'] == 'anim_head1')
            hx, hy = head['vertices'][::2], head['vertices'][1::2]
            ax, ay = parts[0]['vertices'][::2], parts[0]['vertices'][1::2]
            # Preserve the native helmet-to-head fit, including raster padding.
            head_width, head_height = max(hx)-min(hx), max(hy)-min(hy)
            left, bottom = min(ax)-1, max(ay)+1-image.height/2
            fits[name] = [image.width/2/head_width,
                          (left+image.width/4-(min(hx)+max(hx))/2)/head_width,
                          (bottom-min(hy))/head_height]
            path.unlink()
    (OUTPUT/'placement.json').write_text(json.dumps(fits, indent=2)+'\n')

    for kind in ('cone', 'bucket'):
        portrait, _ = armored_portrait(kind, fits[kind])
        portrait.save(OUTPUT/('eyes-'+kind+'.png'))


def armored_portrait(kind, fit):
    """Use the runtime EyesView.UpdateArmor socket, fit and source walk pose."""
    preview = Preview('eyes-zombie', 'walk')
    model = preview.model
    head = next(i for i, node in enumerate(model['Nodes']) if node['Name'] == 'anim_head1-sprite')
    head_sprite = model['Sprites'][str(model['Nodes'][head]['Sprite'])]
    head_image = preview.sprite_image(head_sprite['File'])
    head_width, head_height = [size / head_sprite['PixelsPerUnit'] for size in head_image.size]
    px, py = head_sprite['Pivot']
    helmet_file = OUTPUT/(kind+'.png')
    with Image.open(helmet_file) as helmet:
        helmet_width = helmet.width / 100
    scale = head_width * fit[0] / helmet_width
    position = [(0.5-px)*head_width + head_width*fit[1],
                -py*head_height + head_height*fit[2], 0]
    index = len(model['Nodes'])
    model['Nodes'].append(dict(Name='EyesArmor', Path='EyesArmor', Parent=head,
        Active=True, Enabled=True, Position=position, Rotation=[0, 0, 0, 1],
        Scale=[scale, scale, scale], Sprite=100000, Order=max(n['Order'] for n in model['Nodes'])+10,
        FlipX=False, FlipY=False, Color=[1, 1, 1, 1]))
    model['Sprites']['100000'] = dict(File=str(helmet_file), Pivot=[0.5, 0], PixelsPerUnit=100)
    preview.base_angles += (0,)
    preview.draw_order += (index,)
    pose = preview.pose(0)
    # Fit the complete pose to a transparent canvas; do not guess a helmet
    # position in portrait pixels or clip the imported head's transformed socket.
    bounds = preview.bounds(pose)
    x0, y0, x1, y1 = bounds
    camera_scale = min(400/(x1-x0), 340/(y1-y0))
    camera = [camera_scale, 0, 210-camera_scale*(x0+x1)/2,
              0, -camera_scale, 180+camera_scale*(y0+y1)/2]
    image = preview.draw(pose, camera, (0, 0, 0, 0))
    return image.crop(image.getbbox()), dict(parent=head, position=position, scale=scale,
                                           transform=pose.transforms[index], head=pose.transforms[head])

if __name__ == '__main__':
    main()
