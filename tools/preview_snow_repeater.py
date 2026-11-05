"""Preview exported original artwork; calibrate sizes and anchors without game code.

Requires Pillow. Calibration is an asset measurement, not a runtime test.
"""
import argparse
import bisect
from dataclasses import dataclass
import hashlib
import json
import math
from functools import cache
from pathlib import Path

from PIL import Image, ImageDraw

from source_index import atomic_json

PROJECT = Path(__file__).resolve().parents[1]
FRAME_WIDTH = 420
CAMERA_X = 210


@dataclass
class Pose:
    transforms: list
    active: list
    enabled: list
    sprite_ids: list
    alpha: list
    order: list | None = None


def value(channel, time):
    times, coefficients = channel
    index = max(0, bisect.bisect_right(times, time) - 1)
    a, b, c, d = coefficients[index]
    dt = max(0, time - times[index])
    return ((a * dt + b) * dt + c) * dt + d


def multiply(a, b):
    return [a[0]*b[0]+a[1]*b[3], a[0]*b[1]+a[1]*b[4], a[0]*b[2]+a[1]*b[5]+a[2],
            a[3]*b[0]+a[4]*b[3], a[3]*b[1]+a[4]*b[4], a[3]*b[2]+a[4]*b[5]+a[5]]


class Preview:
    def __init__(self, plant, clip=None, project=PROJECT):
        self.plant = plant
        self.assets = project / "assets" / plant
        self.model = json.loads((self.assets / "model.json").read_text())
        self.action = clip or ("shoot2" if "head1" in self.model["Clips"] else "shoot" if "shoot" in self.model["Clips"] else "Bomb" if "Bomb" in self.model["Clips"] else "idle")
        if self.action not in self.model["Clips"]:
            raise ValueError(f"Unknown clip: {self.action}")
        self.camera_scale = 100 if plant in ("thorn-nut", "demise-shroom") else 110 if plant.endswith("-tall-nut") or plant == "football-nut" else 65 if plant.endswith("-squash") or plant == "jawbreaker" else 35 if plant == "umbrella-husk" else 55 if plant == "umbrella-shell" else 120 if plant == "twin-solar-nut" else 145 if plant == "solar-chomper" else 100 if plant == "chomp-nut" else 145 if plant in ("flytrapper", "cherry-chomper", "chompzilla", "doom-chomper", "frostbiter") else 190
        nodes = self.model["Nodes"]
        self.draw_order = tuple(sorted(range(len(nodes)), key=lambda i: nodes[i]["Order"]))
        self.base_angles = tuple(math.atan2(2*(n["Rotation"][3]*n["Rotation"][2]),
                                          1-2*n["Rotation"][2]**2) for n in nodes)

    @cache
    def sprite_image(self, filename):
        """Decode once per invocation; copy the shared RGBA image before mutation."""
        with Image.open(self.assets / filename) as source:
            return source.convert("RGBA")


    @cache
    def clip_bindings(self, name):
        """Prepare immutable sampling data on first use of each clip."""
        additive = (name == "shoot" and self.model["ShootAdditive"]) or (self.plant == "flame-gloom-shroom" and name == "idle_fire") or name in ("shake", "fire", "sun", "grab", "head1", "head2", "head3", "shoot1", "shoot2", "shoot3")
        if self.plant in ("sea-star", "urchin-shroom", "taco-pult", "cob-pult") and name == "shoot2":
            additive = False
        prepared = []
        for binding in self.model["Clips"][name]["Bindings"]:
            channels = tuple(
                (tuple(key["Time"] for key in keys),
                 tuple(tuple(key["Coefficients"]) for key in keys))
                for keys in binding["Channels"])
            reference = tuple(value(channel, 0) for channel in channels) if additive else None
            prepared.append((binding["Node"], binding["Attribute"], channels, reference))
        return tuple(prepared)


    def pose(self, idle_time, shoot_time=None, runtime=None, zero_root=False):
        nodes = self.model["Nodes"]
        position = [n["Position"][:] for n in nodes]
        scale = [n["Scale"][:] for n in nodes]
        angle = list(self.base_angles)
        active = [n["Active"] for n in nodes]
        enabled = [n.get("Enabled", True) for n in nodes]
        sprite_ids = [n["Sprite"] for n in nodes]
        alpha = [n["Color"][3] for n in nodes]
        order = [n["Order"] for n in nodes]
        initial = "idle" if "idle" in self.model["Clips"] else "walk"
        samples = [(initial, idle_time % self.model["Clips"][initial]["Duration"]), (self.action, shoot_time)]
        if self.model.get("Affines") and shoot_time is not None:
            samples = [(self.action, shoot_time)]
        if self.plant in ("minnow-shroom", "urchin-shroom", "shark-shroom", "sea-star", "tingle-kelp", "grapple-kelp", "spike-kelp") and shoot_time is not None:
            samples = [(self.action, shoot_time)]
        if self.plant in ("taco-pult", "cob-pult", "overripe-melon", "cabbage-tri-pult") and shoot_time is not None:
            samples = [(self.action, shoot_time)]
        if (self.plant in ("solar-pot", "sunset-shroom", "solar-flare", "solar-blover") or self.plant.endswith("-squash") or self.plant in ("buck-shroom", "sprout-pult", "baby-corn", "slice-pult", "nut-shroom", "bell-pepper", "boom-shroom", "penny-shroom", "sunny-shroom", "starry-shroom", "sturdy-shroom", "camo-shroom", "speedy-shroom", "blasty-shroom", "shivery-shroom", "spuddy-shroom", "demise-shroom", "gloom-shroom", "frost-gloom-shroom", "flame-gloom-shroom", "umbrella-husk", "umbrella-shell", "solar-nut-ball", "chomp-mine", "solar-chomper", "cherry-chomper", "chomp-nut", "chompzilla") or (self.plant in ("peatato-mine", "solar-mine", "mine-nut") and self.action != "shoot")) and shoot_time is not None:
            samples = [(self.action, shoot_time)]
        if "head1" in self.model["Clips"]:
            samples = [("idle", idle_time % self.model["Clips"]["idle"]["Duration"])]
            for head in range(1, 4):
                name = f"{'shoot' if shoot_time is not None else 'head'}{head}"
                samples.append((name, shoot_time if shoot_time is not None else idle_time % self.model["Clips"][name]["Duration"]))
        if runtime is not None:
            elapsed, corrected = runtime
            rolling = elapsed >= 1
            time = max(0, elapsed-1) * 2 if corrected else elapsed
            clip = "Round" if rolling else "idle_big"
            samples = [(clip, time % self.model["Clips"][clip]["Duration"])]
        if self.plant in ("black-cherry", "solar-flare") and self.action != "shake":
            samples.append(("shake", idle_time % self.model["Clips"]["shake"]["Duration"]))
        if self.plant == "solar-bomb" and shoot_time is not None:
            samples.append(("shake", idle_time % self.model["Clips"]["shake"]["Duration"]))
        if self.plant in ("coldslaw", "kimchi-pult", "flame-wall-nut", "flame-tall-nut", "cabbage-tri-pult") and self.action != "fire":
            samples.append(("fire", idle_time % self.model["Clips"]["fire"]["Duration"]))
        if self.plant == "flame-gloom-shroom" and self.action != "idle_fire":
            samples.append(("idle_fire", idle_time % self.model["Clips"]["idle_fire"]["Duration"]))
        if self.plant == "solar-squash" and self.action != "sun":
            samples.append(("sun", idle_time % self.model["Clips"]["sun"]["Duration"]))
        affine_values = {a["Node"]: a["Values"][:] for a in self.model.get("Affines", [])}
        for name, time in samples:
            if time is None:
                continue
            for i, attribute, channels, reference in self.clip_bindings(name):
                values = [value(channel, time) for channel in channels]
                if attribute == 0:
                    active[i] = values[0] >= 0.5
                elif attribute == 1:
                    position[i] = [p + v - r for p, v, r in zip(position[i], values, reference)] if reference is not None else values
                elif attribute == 3:
                    scale[i] = [p * v / r for p, v, r in zip(scale[i], values, reference)] if reference is not None else values
                elif attribute == 4:
                    angle[i] = angle[i] + math.radians(values[2] - reference[2]) if reference is not None else math.radians(values[2])
                elif attribute == 5:
                    sprite_ids[i] = int(values[0])
                elif attribute == 6:
                    enabled[i] = values[0] >= .5
                elif attribute == 7:
                    alpha[i] = max(0, min(1, values[0]))
                elif attribute == 12:
                    order[i] = int(values[0])
                elif 8 <= attribute <= 11:
                    affine_values[i][attribute - 8] = values[0]
        for affine in self.model.get("Affines", []):
            ax, ay, sx, sy = affine_values[affine["Node"]]
            a, b, c, d = sx*math.cos(ay), sy*math.sin(ax), -sx*math.sin(ay), sy*math.cos(ax)
            first, second = math.atan2(c-b, a+d), math.atan2(c+b, a-d)
            r, s = math.hypot(a+d, c-b), math.hypot(a-d, c+b)
            angle[affine["Outer"]] = (first+second)/2
            scale[affine["Outer"]] = [(r+s)/2, (r-s)/2, 1]
            angle[affine["Inner"]] = (first-second)/2
        if self.plant == "spuddy-shroom":
            for i, node in enumerate(nodes):
                if node["Path"] == "ScaredyShroom_head/light1": active[i] = True
                elif node["Path"] == "ScaredyShroom_head/light2": active[i] = False
        if runtime is not None and runtime[1]:
            # Same division of ownership as ProjectileView: native motion moves
            # the projectile; source Round contributes only the body's pose.
            position[0] = [0, 0, 0]
        if zero_root:
            position[0] = [0, 0, 0]
        transforms = []
        for i, n in enumerate(nodes):
            c, s = math.cos(angle[i]), math.sin(angle[i])
            matrix = [c*scale[i][0], -s*scale[i][1], position[i][0],
                      s*scale[i][0], c*scale[i][1], position[i][1]]
            if n["Parent"] >= 0:
                matrix = multiply(transforms[n["Parent"]], matrix)
                active[i] = active[i] and active[n["Parent"]]
            transforms.append(matrix)
        return Pose(transforms, active, enabled, sprite_ids, alpha, order)


    def render(self, idle_time, shoot_time=None, runtime=None):
        pose = self.pose(idle_time, shoot_time, runtime)
        nodes = self.model["Nodes"]
        transforms = pose.transforms
        # Match the runtime ground anchor. Prefab root positions belong to the
        # source scene, so they must not move the plant outside the preview camera.
        ground = self.model["GroundNode"]
        if ground < 0:
            # Unanimated projectiles may have no shadow renderer. Anchor their
            # first visible sprite instead of indexing a non-rendering last node.
            ground = next((i for i, node in enumerate(nodes) if node["Name"] == "Shadow" and node["Sprite"] >= 0),
                          next(i for i, node in enumerate(nodes) if node["Sprite"] >= 0))
        footprint = self.model["GroundNode"] >= 0 or nodes[ground]["Name"] == "Shadow"
        node = nodes[ground]
        sprite = self.model["Sprites"][str(node["Sprite"])]
        width, height = self.sprite_image(sprite["File"]).size
        x = (0.5 - sprite["Pivot"][0]) * width / sprite["PixelsPerUnit"]
        y = ((0 if footprint else .5) - sprite["Pivot"][1]) * height / sprite["PixelsPerUnit"]
        a, b, c, d, e, f = transforms[ground]
        ground_x, ground_y = a*x + b*y + c, d*x + e*y + f
        camera = [self.camera_scale, 0, CAMERA_X - self.camera_scale*ground_x,
                  0, -self.camera_scale, (325 if footprint else 180) + self.camera_scale*ground_y]
        if runtime is not None:
            elapsed, corrected = runtime
            native_x = max(0, elapsed-1) * 125
            camera_scale = 80 / 1.517076
            # Fixed native controller origin. The old source-only preview moved
            # its camera with the shadow, hiding the runtime root-motion bug.
            camera = [camera_scale, 0, 80 + native_x,
                      0, -camera_scale, 220]
            if corrected:
                centre_x, centre_y = transforms[ground][2], transforms[ground][5]
                camera[2] -= camera_scale * centre_x
                camera[5] = 230 + camera_scale * centre_y
        return self.draw(pose, camera)


    def draw(self, pose, camera, background=(40, 46, 55, 255)):
        nodes = self.model["Nodes"]
        transforms, active, enabled = pose.transforms, pose.active, pose.enabled
        sprite_ids, alpha = pose.sprite_ids, pose.alpha
        canvas = Image.new("RGBA", (FRAME_WIDTH, 360), background)
        skins = {skin["Node"]: skin for skin in self.model.get("Skins", [])}
        for i in sorted(self.draw_order, key=lambda i: pose.order[i]) if pose.order is not None else self.draw_order:
            n = nodes[i]
            if not active[i] or not enabled[i] or n["Sprite"] < 0:
                continue
            sprite = self.model["Sprites"][str(sprite_ids[i])]
            image = self.sprite_image(sprite["File"])
            if alpha[i] != 1:
                image = image.copy()
                image.putalpha(image.getchannel("A").point(lambda x: round(x * alpha[i])))
            if n["FlipX"]: image = image.transpose(Image.Transpose.FLIP_LEFT_RIGHT)
            if n["FlipY"]: image = image.transpose(Image.Transpose.FLIP_TOP_BOTTOM)
            if i in skins:
                self.draw_skin(canvas, image, sprite, skins[i], transforms, camera)
                continue
            ppu = sprite["PixelsPerUnit"]
            # Image pixels (top-left) to the original sprite's pivot, then prefab
            # hierarchy, then a fixed camera used only for this offline preview.
            pivot_x, pivot_y = sprite["Pivot"]
            if n["FlipX"]: pivot_x = 1 - pivot_x
            if n["FlipY"]: pivot_y = 1 - pivot_y
            pixel = [1/ppu, 0, -pivot_x*image.width/ppu,
                     0, -1/ppu, (1-pivot_y)*image.height/ppu]
            matrix = multiply(camera, multiply(transforms[i], pixel))
            a, b, c, d, e, f = matrix
            det = a*e-b*d
            if not math.isfinite(det):
                raise ValueError("Non-finite sprite transform")
            if abs(det) < 1e-12:
                continue  # A zero-scale animation frame has no visible area.
            inverse = (e/det, -b/det, (b*f-e*c)/det, -d/det, a/det, (d*c-a*f)/det)
            canvas.alpha_composite(image.transform(canvas.size, Image.Transform.AFFINE, inverse,
                                                   Image.Resampling.BICUBIC))
        return canvas

    @staticmethod
    def draw_skin(canvas, image, sprite, skin, transforms, camera):
        from UnityPy.export.SpriteHelper import copy_triangle
        positions, texture = [], []
        width, height = image.size
        ppu = sprite["PixelsPerUnit"]
        px, py = sprite["Pivot"]
        for vertex, weights, indices in zip(skin["Vertices"], skin["Weights"], skin["Indices"]):
            x = y = 0
            for weight, index in zip(weights, indices):
                if weight == 0:
                    continue
                b = skin["BindPoses"][index]
                bind = [b[0], b[1], b[3], b[4], b[5], b[7]]
                m = multiply(camera, multiply(transforms[skin["Bones"][index]], bind))
                x += weight * (m[0]*vertex[0] + m[1]*vertex[1] + m[2])
                y += weight * (m[3]*vertex[0] + m[4]*vertex[1] + m[5])
            positions.append((x, y))
            texture.append((vertex[0]*ppu + px*width, (1-py)*height - vertex[1]*ppu))
        layer = Image.new("RGBA", canvas.size)
        for at in range(0, len(skin["Triangles"]), 3):
            triangle = skin["Triangles"][at:at+3]
            destination = tuple(positions[j] for j in triangle)
            (ax, ay), (bx, by), (cx, cy) = destination
            if abs((bx-ax)*(cy-ay) - (by-ay)*(cx-ax)) < 1e-8:
                continue
            left = max(0, math.floor(min(x for x, y in destination)))
            top = max(0, math.floor(min(y for x, y in destination)))
            right = min(canvas.width, math.ceil(max(x for x, y in destination)))
            bottom = min(canvas.height, math.ceil(max(y for x, y in destination)))
            if right <= left or bottom <= top:
                continue
            tile = Image.new("RGBA", (right-left, bottom-top))
            copy_triangle(image, tuple(texture[j] for j in triangle), tile,
                          tuple((x-left, y-top) for x, y in destination))
            layer.alpha_composite(tile, (left, top))
        canvas.alpha_composite(layer)

    def bounds(self, pose, indices=None, include_disabled=False):
        """Sprite.bounds corners in prefab space, including Unity pivot flips."""
        points = []
        for i in range(len(self.model["Nodes"])) if indices is None else indices:
            node = self.model["Nodes"][i]
            if node["Sprite"] < 0 or not pose.active[i]:
                continue
            if not include_disabled and (not pose.enabled[i] or pose.alpha[i] <= 0):
                continue
            sprite = self.model["Sprites"][str(pose.sprite_ids[i])]
            width, height = self.sprite_image(sprite["File"]).size
            px, py = sprite["Pivot"]
            ppu = sprite["PixelsPerUnit"]
            a, b, c, d, e, f = pose.transforms[i]
            for x in (-px * width / ppu, (1 - px) * width / ppu):
                for y in (-py * height / ppu, (1 - py) * height / ppu):
                    if node["FlipX"]: x_local = -x
                    else: x_local = x
                    if node["FlipY"]: y_local = -y
                    else: y_local = y
                    points.append((a*x_local + b*y_local + c, d*x_local + e*y_local + f))
        if not points:
            return None
        if not all(math.isfinite(v) for point in points for v in point):
            raise ValueError("Non-finite sprite bounds")
        return [min(p[0] for p in points), min(p[1] for p in points),
                max(p[0] for p in points), max(p[1] for p in points)]


def plant_alignment(preview, fit, target_width):
    """Match View.Create's initial bounds; never refit an animated frame."""
    ground = preview.model["GroundNode"]
    if fit == "ground" and ground < 0:
        raise ValueError("Ground fitting requires a GroundNode; use --fit artwork --target-width")
    bounds = preview.bounds(preview.pose(0), [ground] if fit == "ground" else None,
                            include_disabled=True)
    if bounds is None or bounds[2] <= bounds[0] or bounds[3] <= bounds[1]:
        raise ValueError("No usable initial reference bounds")
    scale = target_width / (bounds[2] - bounds[0])
    offset = [-.5 * (bounds[0] + bounds[2]) * scale, -bounds[1] * scale]
    return dict(fit=fit, target_width=target_width, reference_bounds=bounds,
                reference_width=bounds[2] - bounds[0], scale=scale, offset=offset)


def calibration_frame(preview, pose, alignment, label, projectile=False):
    """One logical board pixel = two image pixels; camera stays fixed."""
    scale, (ox, oy) = alignment["scale"], alignment["offset"]
    origin = [CAMERA_X, 180 if projectile else 300]
    camera = [2*scale, 0, origin[0] + 2*ox, 0, -2*scale, origin[1] - 2*oy]
    frame = preview.draw(pose, camera)
    draw = ImageDraw.Draw(frame)
    for x in range(10, FRAME_WIDTH, 40):
        draw.line((x, 40, x, 350), fill=(65, 71, 80))
    for y in range(origin[1] % 40, 360, 40):
        draw.line((10, y, FRAME_WIDTH-10, y), fill=(65, 71, 80))
    x, y = origin
    draw.rectangle((x-80, y-160, x+80, y), outline=(90, 180, 110), width=2)
    draw.line((x-8, y, x+8, y), fill="yellow", width=2)
    draw.line((x, y-8, x, y+8), fill="yellow", width=2)
    bounds = preview.bounds(pose)
    board_bounds = None if bounds is None else [bounds[0]*scale+ox, bounds[1]*scale+oy,
                                               bounds[2]*scale+ox, bounds[3]*scale+oy]
    clipped = False
    if board_bounds is not None:
        left, bottom, right, top = board_bounds
        box = [x+2*left, y-2*top, x+2*right, y-2*bottom]
        clipped = box[0] < 0 or box[1] < 0 or box[2] > FRAME_WIDTH or box[3] > 360
        draw.rectangle(box, outline=(230, 140, 80))
        label += f"\n{right-left:.2f} x {top-bottom:.2f} board px"
    if clipped:
        label += "\nCLIPPED: inspect JSON bounds"
    draw.rectangle((0, 0, FRAME_WIDTH, 48), fill=(40, 46, 55))
    draw.text((10, 4), label, fill="white")
    nodes = [{"path": node["Path"], "origin": [t[2]*scale+ox, t[5]*scale+oy]}
             for node, t in zip(preview.model["Nodes"], pose.transforms)]
    return frame, dict(visible_bounds=board_bounds, clipped=clipped, nodes=nodes)


def calibration_preview(preview, output, args, project=PROJECT):
    output.mkdir(parents=True, exist_ok=True)
    (output/"calibration.json").unlink(missing_ok=True)
    alignment = plant_alignment(preview, args.fit, args.target_width or 80)
    duration = preview.model["Clips"][preview.action]["Duration"]
    samples = [("idle", t, False) for t in (0, .5, 1, 1.5)]
    samples += [(preview.action, duration*t, True) for t in (0, .4, .533333, .99999)]
    projectile = Preview(args.projectile, args.projectile_clip, project) if args.projectile else None
    rows = 3 if projectile else 2
    sheet = Image.new("RGBA", (4*FRAME_WIDTH, rows*380), (40, 46, 55, 255))
    report = dict(schema=1, game_started=False, runtime_verified=False,
                  units="logical board pixels, Y up; 2.82 render units per board pixel",
                  grid_step=20, image_pixels_per_board_pixel=2,
                  plant=dict(model=preview.plant, alignment=alignment, frames=[]))
    for i, (clip, time, action) in enumerate(samples):
        pose = preview.pose(time, time if action else None)
        frame, metrics = calibration_frame(preview, pose, alignment, f"{preview.plant}: {clip} {time:.3f}s")
        sheet.alpha_composite(frame, ((i % 4)*FRAME_WIDTH, (i // 4)*380))
        report["plant"]["frames"].append(dict(clip=clip, time=time, **metrics))
    if projectile:
        shot_alignment = dict(reference_width=args.projectile_reference_width,
                              factor=args.projectile_factor,
                              scale=80*args.projectile_factor/args.projectile_reference_width,
                              offset=[0, 0], zero_root=args.projectile_zero_root)
        report["projectile"] = dict(model=projectile.plant, alignment=shot_alignment, frames=[])
        duration = projectile.model["Clips"][projectile.action]["Duration"]
        for i, fraction in enumerate((0, .25, .5, .99999)):
            time = duration*fraction
            pose = projectile.pose(time, time, zero_root=args.projectile_zero_root)
            frame, metrics = calibration_frame(projectile, pose, shot_alignment,
                                               f"{projectile.plant}: {projectile.action} {time:.3f}s", True)
            sheet.alpha_composite(frame, (i*FRAME_WIDTH, 760))
            report["projectile"]["frames"].append(dict(clip=projectile.action, time=time, **metrics))
    # A report identifies the assets and adapter source inspected, not a proof
    # that the selected settings are the correct runtime settings.
    paths = {project/"tools/preview_snow_repeater.py", project/"src/ImportedPeaVisuals.cs",
             project/"src/ImportedProjectileVisuals.cs", project/"src/PuffArtworkSizing.cs",
             project/"tools/plant-registrations.json"}
    for asset in (preview, projectile):
        if asset is not None:
            paths.add(asset.assets/"model.json")
            paths.update(asset.assets/sprite["File"] for sprite in asset.model["Sprites"].values())
    report["inputs_sha256"] = {str(p.relative_to(project)): hashlib.sha256(p.read_bytes()).hexdigest()
                              for p in sorted(paths)}
    for entry in (report["plant"], report.get("projectile")):
        if entry:
            entry["alignment"]["render_scale"] = entry["alignment"]["scale"] * 2.82
            entry["alignment"]["render_offset"] = [v*2.82 for v in entry["alignment"]["offset"]]
    # Publish the success report last; a failed generation has no fresh report.
    sheet.convert("RGB").save(output/"calibration.png")
    report["sheet_sha256"] = hashlib.sha256((output/"calibration.png").read_bytes()).hexdigest()
    atomic_json(output/"calibration.json", report)
    print(f"Calibration: {output / 'calibration.png'}; measurements: {output / 'calibration.json'}")


def positive_number(text):
    value = float(text)
    if not math.isfinite(value) or value <= 0:
        raise argparse.ArgumentTypeError("Expected a finite positive number")
    return value


def parse_args(argv=None):
    models = sorted(p.parent.name for p in (PROJECT/"assets").glob("*/model.json") if p.is_file())
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--plant", choices=models, default="snow-repeater")
    parser.add_argument("--clip", help="Preview a specific original animation clip")
    mode = parser.add_mutually_exclusive_group()
    mode.add_argument("--runtime-projectile", action="store_true", help="Solar Nut adapter comparison")
    mode.add_argument("--calibrate", action="store_true", help="Fixed-scale size/anchor sheet and JSON")
    parser.add_argument("--output-dir", type=Path, help="Default: references/<plant>")
    parser.add_argument("--fit", choices=("ground", "artwork"), default="ground")
    parser.add_argument("--target-width", type=positive_number, help="Plant width in board pixels; ground default: 80")
    parser.add_argument("--projectile", choices=models, help="Compare a projectile at the same board scale")
    parser.add_argument("--projectile-clip", help="Projectile clip; default uses the existing preview selection")
    parser.add_argument("--projectile-reference-width", type=positive_number,
                        help="Proven source width used by the runtime projectile adapter; never guessed")
    parser.add_argument("--projectile-factor", type=positive_number, default=1)
    parser.add_argument("--projectile-zero-root", action="store_true", help="Discard root position, as selected runtime adapters do")
    args = parser.parse_args(argv)
    if args.runtime_projectile and args.plant != "solar-nut-ball":
        parser.error("--runtime-projectile requires --plant solar-nut-ball")
    calibration_options = (args.fit != "ground" or args.target_width is not None or args.projectile
                           or args.projectile_clip or args.projectile_reference_width is not None
                           or args.projectile_factor != 1 or args.projectile_zero_root)
    if calibration_options and not args.calibrate:
        parser.error("Calibration options require --calibrate")
    if args.calibrate:
        plants = json.loads((PROJECT/"tools/plant-registrations.json").read_text())["plants"]
        name = "".join(part[0].upper()+part[1:] for part in args.plant.split("-"))
        puff = any(p["name"] == name and p["native"] == "Puffshroom" for p in plants)
        if puff and args.fit != "artwork":
            parser.error("Puff imports require --fit artwork and a measured --target-width")
        if args.fit == "artwork" and args.target_width is None:
            parser.error("--fit artwork requires a measured --target-width in board pixels")
    if args.projectile and args.projectile_reference_width is None:
        parser.error("--projectile requires --projectile-reference-width")
    if not args.projectile and (args.projectile_clip or args.projectile_reference_width is not None
                                or args.projectile_factor != 1 or args.projectile_zero_root):
        parser.error("Projectile options require --projectile")
    return args



def original_preview(preview, output, runtime_projectile=False, clip=None):
    if runtime_projectile:
        times = (.5, 1.01, 1.99, 2.01)
        sheet = Image.new("RGBA", (4 * FRAME_WIDTH, 760), (40, 46, 55, 255))
        for row, corrected in enumerate((False, True)):
            for col, elapsed in enumerate(times):
                frame = preview.render(0, runtime=(elapsed, corrected))
                ImageDraw.Draw(frame).text((12, 12),
                    f"{'0.1.43' if corrected else '0.1.42'} adapter: {elapsed:.2f}s", fill="white")
                sheet.alpha_composite(frame, (col * FRAME_WIDTH, row * 380))
        sheet.convert("RGB").save(output / "runtime-adapter-comparison.png")
        frames = [preview.render(0, runtime=(i/30, True)) for i in range(120)]
        frames[0].save(output / "runtime-adapter-preview.gif", save_all=True,
                       append_images=frames[1:], duration=33, loop=0, disposal=2)
        print("Offline adapter comparison exported; no game started.")
        return
    duration = preview.model["Clips"][preview.action]["Duration"]
    samples = [("idle", t) for t in [0, 0.5, 1, 1.5]] + [(preview.action, duration*t) for t in [0, .4, .533333, .99999]]
    sheet = Image.new("RGBA", (4 * FRAME_WIDTH, 760), (40, 46, 55, 255))
    for i, (name, time) in enumerate(samples):
        frame = preview.render(time, time if i >= 4 else None)
        ImageDraw.Draw(frame).text((12, 12), f"Fusion: {name} {time:.3f}s", fill="white")
        sheet.alpha_composite(frame, ((i % 4)*FRAME_WIDTH, (i//4)*380))
    suffix = f"-{clip}" if clip else ""
    sheet.convert("RGB").save(output / f"original-animation-preview{suffix}.png")
    frames = [preview.render(i/30) for i in range(60)]
    frames += [preview.render(i/30, min(i/30, duration-.00001)) for i in range(math.ceil(duration*30))]
    frames[0].save(output / f"original-animation-preview{suffix}.gif", save_all=True,
                   append_images=frames[1:], duration=33, loop=0, disposal=2)
    print("Original sprite/animation contact sheet and GIF exported; no game started.")


def main(argv=None):
    args = parse_args(argv)
    preview = Preview(args.plant, args.clip)
    output = args.output_dir or PROJECT/"references"/args.plant
    if args.calibrate:
        calibration_preview(preview, output, args)
    else:
        output.mkdir(parents=True, exist_ok=True)
        original_preview(preview, output, args.runtime_projectile, args.clip)


if __name__ == "__main__":
    main()

