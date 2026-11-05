"""Rasterize decoded native Spine triangles without Unity or game execution."""
import json
import math
from pathlib import Path

from PIL import Image, ImageDraw


def render(attachments, folder, scale=1):
    parts = [a for a in attachments if a['color'][3] > 0.001
             and 'locator' not in a['name'].lower() and a['name'] != 'anim_sprout']
    points = [(x, y) for a in parts for x, y in zip(a['vertices'][::2], a['vertices'][1::2])]
    left, bottom = min(p[0] for p in points), min(p[1] for p in points)
    right, top = max(p[0] for p in points), max(p[1] for p in points)
    canvas = Image.new('RGBA', (math.ceil((right-left)*scale)+4, math.ceil((top-bottom)*scale)+4))
    textures = {}
    for a in parts:
        if a['page'] not in textures:
            textures[a['page']] = Image.open(Path(folder)/a['page']).convert('RGBA')
        texture = textures[a['page']]
        vertices = [(2+(x-left)*scale, 2+(top-y)*scale) for x,y in zip(a['vertices'][::2],a['vertices'][1::2])]
        uv = [(u*texture.width, v*texture.height) for u,v in zip(a['uvs'][::2],a['uvs'][1::2])]
        layer = Image.new('RGBA', canvas.size)
        for i in range(0,len(a['triangles']),3):
            ids = a['triangles'][i:i+3]
            (ax,ay),(bx,by),(cx,cy)=[vertices[j] for j in ids]
            det=(bx-ax)*(cy-ay)-(cx-ax)*(by-ay)
            if abs(det)<1e-7:continue
            def solve(axis):
                av,bv,cv=[uv[j][axis] for j in ids]
                u=((bv-av)*(cy-ay)-(cv-av)*(by-ay))/det
                v=((bx-ax)*(cv-av)-(cx-ax)*(bv-av))/det
                return [u,v,av-u*ax-v*ay]
            affine=solve(0)+solve(1)
            x0=max(0,math.floor(min(vertices[j][0] for j in ids)));y0=max(0,math.floor(min(vertices[j][1] for j in ids)))
            x1=min(canvas.width,math.ceil(max(vertices[j][0] for j in ids)));y1=min(canvas.height,math.ceil(max(vertices[j][1] for j in ids)))
            if x1<=x0 or y1<=y0:continue
            affine[2]+=affine[0]*x0+affine[1]*y0;affine[5]+=affine[3]*x0+affine[4]*y0
            patch=texture.transform((x1-x0,y1-y0),Image.Transform.AFFINE,affine,Image.Resampling.BICUBIC)
            mask=Image.new('L',patch.size);ImageDraw.Draw(mask).polygon([(vertices[j][0]-x0,vertices[j][1]-y0) for j in ids],fill=255)
            # Replace within each triangle; shared edges must not double alpha.
            layer.paste(patch,(x0,y0),mask)
        if any(abs(c-1)>1e-6 for c in a['color']):
            layer=Image.merge('RGBA', [channel.point(lambda value, c=c: round(value*c))
                                    for channel,c in zip(layer.split(),a['color'])])
        canvas.alpha_composite(layer)
    return canvas


if __name__ == '__main__':
    import sys
    data=json.loads(Path(sys.argv[1]).read_text())
    render(data['attachments'],sys.argv[2]).save(sys.argv[3])
