"""Export native armor attachments for imported Eyes Zombie heads, without Unity."""
import json
import subprocess
from pathlib import Path
from render_zombie_pose import render
from PIL import Image

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

    eyes = Image.open(ROOT/'assets/debug-cards/eyes-zombie.png').convert('RGBA')
    for kind in ('cone', 'bucket'):
        canvas = Image.new('RGBA', (eyes.width, eyes.height + 70))
        canvas.alpha_composite(eyes, (0, 70))
        helmet = Image.open(OUTPUT/(kind+'.png')).convert('RGBA')
        helmet.thumbnail((125, 110), Image.Resampling.LANCZOS)
        canvas.alpha_composite(helmet, (max(0, 60-helmet.width//2), 94-helmet.height))
        canvas.save(OUTPUT/('eyes-'+kind+'.png'))

if __name__ == '__main__':
    main()
