"""Export the actual 4.0.5.2 prefab, sprites and streamed cubic animation curves.

Requires UnityPy 1.25.4 (and Pillow); run with a Python environment containing it.
No Unity player, editor or game process is started. No unofficial project input.
"""
import hashlib
import fcntl
from io import BytesIO
import json
import math
import os
from pathlib import Path
import struct
import tempfile
from time import perf_counter
import zlib
import argparse

import UnityPy
from PIL import Image
from source_index import source_fingerprints, sha256

PROJECT = Path(__file__).resolve().parents[1]
SOURCE = PROJECT.parent / "pvzfusion/english-4.0.5.2/Game Files"
# Only prefab IDs are registered; their root transforms and controllers come from the source.
PLANTS = {
    "hot-potato": 44436,
    "boomwood": 66712,
    "fire-nut-projectile": 50822,
    "cherry-drone-projectile": 63898,
    "fire-cherry-projectile": 61267,
    "small-fire-projectile": 37232,
    "potato-fire-projectile": 42393,
    "hypno-fire-projectile": 45921,
    "nutwood": 40368,
    "torchthree": 54602,
    "sparkwood": 52570,
    "hypnowood": 66713,
    "tide-shroom": 64446,
    "blacklight-plantern": 50149,
    "scorchwood": 66709,
    "stenchwood": 66711,
    "stench-fire-projectile": 75189,
    "scorch-projectile": 75190,
    "roasted-squash": 68177,
    "roasted-wall-nut": 49517,
    "roasted-pumpkin": 61663,
    "roasted-potato": 44434,
    "umbrella-kale": 40541,
    "umbrella-rind": 40542,
    "lumos-umbrella": 40551,
    "ice-pot": 35123,
    "threetato-mine": 52857,
    "marigold-mine": 44433,
    "chompkin": 61654,
    "pumpkin-mine": 61667,
    "sunny-pumpkin": 61660,
    "glacier-shroom": 70225,
    "glacier-projectile": 33964,
    "belch-shroom": 70207,
    "hungry-shroom": 75224,
    "trigger-shroom": 70223,
    "spicicle": 67871,
    "cherry-pop": 42856,
    "ball-cactus": 64136,
    "cabbage-dropper": 33230,
    "butter-fly": 33228,
    "multipeater": 37769,
    "sea-pot": 35125,
    "blover-pot": 35111,
    "scaredy-pot": 35115,
    "triplantern": 50146,
    "aurora-plantern": 50150,
    "phoenix-threepeater": 61562,
    "phoenix-projectile": 42394,
    "solar-pult": 44714,
    "solar-cabbage-projectile": 41230,
    "speedy-projectile": 73655,
    "shivery-spore-projectile": 73650,
    "glowver": 42481,
    "frenzy-shroom": 75225,
    "frenzy-projectile": 73656,
    "perfume-shroom": 70220,
    "tangle-weed": 39569,
    "tangle-mine": 44435,
    "kelp-mine": 51362,
    "sea-mine": 45540,
    "potato-masher": 68546,
    "potato-mash": 68175,
    "golden-nut": 49504,
    "golden-sunflower": 63100,
    "strangle-kelp": 75432,
    "squashweed": 64167,
    "kelp-spreader": 65147,
    "squash-spreader": 63446,
    "mini-squash-projectile": 71993,
    "butterweed": 64166,
    "springweed": 64164,
    "splash": 68168,
    "sea-nut": 49509,
    "thornweed": 64160,
    "cactus-spike-projectile": 34021,
    "starthorn": 52556,
    "doom-cactus": 62067,
    "lumos-cactus": 62071,
    "cactus-star-projectile": 34017,
    "doom-cactus-projectile": 33976,
    "light-spike-projectile": 33995,
    "light-pierce-projectile": 33968,
    "beamshooter": 37190,
    "rebeamer": 37193,
    "split-beam": 37175,
    "gatling-beam": 37185,
    "ray-shroom": 70204,
    "blubber-shroom": 70216,
    "sunrise-shroom": 70201,
    "bush-shroom": 70202,
    "umbrella-clove": 40539,
    "umbrella-thorn": 40538,
    "damage-pult": 44727,
    "starburst-shroom": 70210,
    "doom-cabbage-projectile": 66428,
    "star-cap-projectile": 70926,
    "doom-pea": 37183,
    "gatling-doom": 38423,
    "doom-spreader": 63451,
    "death-star": 52562,
    "doom-pea-projectile": 77869,
    "doom-projectile": 75385,
    "big-doom-projectile": 75387,
    "doom-meteor-projectile": 47924,
    "kernel-morph": 46060,
    "rust-shroom": 70208,
    "metal-pult": 44730,
    "iron-star": 52554,
    "magnet-kernel-projectile": 52340,
    "magnet-butter-projectile": 70927,
    "iron-melon-projectile": 66419,
    "iron-star-projectile": 34013,
    "pumpkin-pod": 61642,
    "melon-shell": 61644,
    "cryo-pumpkin": 61633,
    "pyro-pumpkin": 61640,
    "doom-garlic": 43589,
    "doom-nut": 49501,
    "doomflower": 46021,
    "crater-tot": 40307,
    "doom-seed": 37375,
    "potato-seed-projectile": 60844,
    "saguaro-shroom": 70232,
    "ice-cactus": 62069,
    "magnet-cactus": 62068,
    "illumi-nut": 49494,
    "ice-cactus-projectile": 33999,
    "magnet-cactus-projectile": 33974,
    "dormant-shroom": 39395,
    "morph-shroom": 70206,
    "melon-morph": 44733,
    "starmorph": 52574,
    "metal-melon-projectile": 66401,
    "metal-star-projectile": 34009,
    "lettuce-pult": 44736,
    "butternut-pult": 44738,
    "gourd-pult": 44739,
    "spring-melon": 43528,
    "lettuce-projectile": 64734,
    "little-lettuce-projectile": 44724,
    "butternut-projectile": 64733,
    "butter-squash-projectile": 64736,
    "gourd-projectile": 64735,
    "windy-pumpkin": 61643,
    "spiky-pumpkin": 61669,
    "jack-o-plantern": 61658,
    "sea-shell": 61648,

    "cherry-pumpkin": 61655,
    "stinky-pumpkin": 61635,
    "scareless-pumpkin": 61670,
    "nut-shell": 61668,

    "starry-pumpkin": 61664,
    "thorny-pumpkin": 61652,
    "puff-kin": 56887,
    "fuming-pumpkin": 61637,
    "zombie-block-projectile": 50824,
    "zombie-block2-projectile": 50826,
    "zombie-block3-projectile": 50825,
    "beacon-shroom": 64447,
    "jelly-shroom": 59254,
    "swell-shroom": 59300,
    "hermit-shroom": 73922,
    "minnow-shroom": 51401,
    "urchin-shroom": 64445,
    "shark-shroom": 73974,
    "sea-star": 52568,
    "sea-spore-projectile": 73653,
    "sea-star-projectile": 33977,
    "sea-spike-projectile": 34021,

    "hypno-pepper": 50246,
    "hypno-melon": 44717,
    "flame-cattail": 55664,
    "frost-cattail": 55662,
    "hypno-melon-projectile": 66439,
    "fire-track-projectile": 33985,
    "ice-track-projectile": 33986,
    "starlic": 52569,
    "winter-starfruit": 52561,
    "spicy-starfruit": 52567,
    "starshooter": 37182,
    "garlic-star-projectile": 33993,
    "ice-star-projectile": 34014,
    "fire-star-projectile": 33970,
    "star-pea-projectile": 33978,
    "garbage-pult": 44731,
    "clove-pult": 46067,
    "garlic-pult": 44737,
    "summer-melon": 44735,
    "garlic-cabbage-projectile": 66409,
    "garlic-kernel-projectile": 75725,
    "garlic-butter-projectile": 43000,
    "garlic-melon-projectile": 66435,
    "fire-melon-projectile": 66438,

    "boost-shroom": 70200,
    "blowtorch-shroom": 70221,
    "wallop-shroom": 70227,
    "caltrop-shroom": 64161,
    "wallop-projectile": 56290,
    "cyclover": 42479,
    "disguster": 42485,
    "breeze-blover": 42475,
    "mind-blover": 42482,
    "polar-projectile": 59054,
    "tingle-kelp": 42858,
    "grapple-kelp": 42676,
    "tangle-nut": 53647,
    "spike-kelp": 50769,
    "kelp-projectile": 34004,
    "cabbage-nut": 49496,
    "melon-nut": 49493,
    "three-nuts": 44136,
    "squash-nut": 49512,
    "coldslaw": 44721,
    "kimchi-pult": 44720,
    "cobsicle": 46068,
    "salad-pult": 40254,
    "coldslaw-projectile": 66424,
    "kimchi-projectile": 66410,
    "ice-kernel-projectile": 75728,
    "ice-butter-projectile": 42998,
    "salad-projectile": 66427,
    "taco-pult": 44722,
    "cob-pult": 44719,
    "overripe-melon": 44725,
    "cabbage-tri-pult": 44740,
    "taco-projectile": 76525,
    "butter-taco-projectile": 74758,
    "cob-projectile": 66402,
    "butter-cob-projectile": 66415,
    "overripe-projectile": 66440,
    "tri-cabbage-projectile": 66426,
    "cabbage-projectile": 66433,
    "pea-pot": 35114,
    "tri-pot": 35130,
    "kernel-tri-pult": 46065,
    "melon-tri-pult": 44734,
    "kernel-projectile": 75727,
    "butter-projectile": 42996,
    "melon-projectile": 66434,
    "cherry-pot": 35109,
    "hot-pot": 35113,
    "hypno-pot": 35122,
    "doom-pot": 35127,
    "nut-pot": 35134,
    "garlic-pot": 35110,
    "spike-pot": 35124,
    "melon-pot": 35128,
    "cabbage-pot": 35117,
    "butter-pot": 35118,
    "umbrella-pot": 35121,
    "silver-pot": 35119,
    "silver-umbrella": 40548,
    "silver-kernel": 46064,
    "lumos-pot": 35133,
    "silver-kernel-projectile": 75731,
    "silver-butter-projectile": 43002,
    "silver-nut": 49489,
    "silver-garlic": 43592,
    "silver-ice-shroom": 55288,
    "silver-sunflower": 63101,
    "silver-doom-shroom": 39393,
    "silver-hypno-shroom": 59249,
    "silver-cabbage": 44723,
    "silver-melon": 44728,
    "silver-cabbage-projectile": 66429,
    "silver-melon-projectile": 66413,
    "cherry-spreader": 63447,
    "cherry-spreader-projectile": 33982,
    "cherry-spreader-bomb-projectile": 57035,
    "solar-pot": 35112,
    "sunset-shroom": 64443,
    "solar-flare": 50243,
    "solar-blover": 42487,
    "blover-sun-projectile": 63438,
    "buck-shroom": 57323,
    "small-iron-projectile": 34018,
    "sprout-pult": 33774,
    "baby-corn": 33773,
    "slice-pult": 33775,
    "small-cabbage-projectile": 48867,
    "small-kernel-projectile": 75723,
    "small-butter-projectile": 42997,
    "small-melon-projectile": 72616,
    "sun-shroom": 50766,
    "gnome-shroom": 56884,
    "umbrella-cap": 53646,
    "hail-shroom": 56882,
    "icicle-shroom": 42855,
    "twinkle-shroom": 42857,
    "nugget-shroom": 42859,
    "icicle-projectile": 73650,
    "nut-shroom": 47691,
    "bell-pepper": 42861,
    "boom-shroom": 56883,
    "penny-shroom": 50765,
    "pea-shroom": 57324,
    "repeat-shroom": 61621,
    "split-pea-shroom": 57322,
    "gatling-pea-shroom": 66581,
    "puff-pea-projectile": 34007,
    "football-helmet": 40918,
    "football-nut": 49272,
    "football-gatling": 38417,
    "helmet-projectile": 34022,
    "fallout-blover": 42486,
    "solarfruit": 52553,
    "hypno-garlic": 43588,
    "flytrapper": 53765,
    "hypno-pea": 37181,
    "hypno-repeater": 54444,
    "split-hypno": 37189,
    "gatling-hypno": 38424,
    "hypno-projectile": 63610,
    "sunny-shroom": 75230,
    "starry-shroom": 75229,
    "sturdy-shroom": 75226,
    "camo-shroom": 37300,
    "shivery-projectile": 62921,
    "speedy-shroom": 75222,
    "blasty-shroom": 75219,
    "shivery-shroom": 75227,
    "spuddy-shroom": 75220,
    "cherry-spore": 73654,
    "potato-spore": 73652,
    "black-cherry": 37633,
    "frozen-cherry": 66980,
    "demise-shroom": 39392,
    "sundae-shroom": 55289,
    "gloom-shroom": 33647,
    "frost-gloom-shroom": 33651,
    "flame-gloom-shroom": 33650,
    "gutsy-shroom": 48132,
    "gutsy-projectile": 73653,
    "stench-spreader": 63448,
    "snow-threepeater": 63450,
    "scorched-threepeater": 63449,
    "charred-threepeater": 63452,
    "icerock": 60659,
    "hearthrock": 60661,
    "foul-shroom": 70211,
    "starch-shroom": 70226,
    "doom-squash": 68176,
    "jawbreaker": 68169,
    "doom-chomper": 34840,
    "frostbiter": 34843,

    "frost-shroom": 70222,
    "flame-shroom": 70230,
    "soot-shroom": 70203,
    "fume-spreader": 70217,
    "starry-squash": 68167,
    "solar-squash": 68180,
    "spicy-kelp": 75431,
    "doom-kelp": 75433,
    "butter-nut": 49492,
    "umbrella-husk": 40547,
    "torchflower": 46022,
    "land-mine": 73148,
    "foul-nut": 49500,
    "star-nut": 49516,
    "pea-squash": 68174,
    "iron-squash": 68173,
    "plain-pea-projectile": 33962,
    "plain-star-projectile": 33973,
    "poison-pea": 37179,
    "poison-repeater": 54442,
    "split-poison": 37180,
    "gatling-poison": 38419,
    "poison-projectile": 34023,
    "thorn-nut": 72702,
    "ice-mine": 44437,
    "hypno-mine": 44432,
    "chip-mine": 44155,
    "blaze-pea": 37186,
    "blaze-repeater": 54445,
    "split-blaze": 37194,
    "gatling-blaze": 38415,
    "blaze-projectile": 34008,
    "solarweed": 64168,
    "iceweed": 64165,
    "scorchweed": 64162,
    "spike-nut": 67954,
    "flame-wall-nut": 49511,
    "frost-wall-nut": 77109,
    "flame-tall-nut": 34405,
    "frost-tall-nut": 49276,
    "cherry-squash": 68165,
    "spicy-squash": 68166,
    "icy-squash": 68179,
    "hypno-squash": 68164,
    "twin-marigold": 50942,
    "umbrella-shell": 40549,
    "chomp-mine": 56013,
    "solar-chomper": 32211,
    "solar-nut": 54016,
    "twin-solar-nut": 42902,
    "solar-nut-ball": 43517,
    "peatato-mine": 71626,
    "solar-mine": 33164,
    "mine-nut": 46252,
    "potato-projectile": 57125,
    "bucket": 40920,
    "buck-nut": 64886,
    "buckshooter": 37176,
    "iron-projectile": 33960,
    "pea-nut": 41474,
    "chomp-shooter": 65657,
    "chompzilla": 56150,
    "chomp-nut": 45123,
    "nut-projectile": 50823,
    "chomp-projectile": 33967,
    "cherry-chomper": 47417,
    "cherry-nut": 36039,
    "solar-bomb": 50022,
    "cluster-mine": 57083,
    "snow-repeater": 35603,
    "snow-split": 37188,
    "gatling-snow": 38426,
    "solar-shooter": 46095,
    "solar-projectile": 63439,
    "cherry-shooter": 37187,
    "cherrypeater": 37191,
    "split-cherry": 37192,
    "gatling-cherry": 63745,
    "cherry-projectile": 61266,
    "explode-o-shooter": 47421,
    "explode-projectile": 61265,
}


def vector(value, components):
    return [value[c] for c in components]


# Verified absent source paths: references/cactus-fusions-2-33-0/stale-bindings.json.
CACTUS_STALE_BINDINGS = {
    ('squashweed', 3964, 3695363022, 4, 1),
    ('squashweed', 3964, 1162580596, 4, 1),
    ('squashweed', 3964, 843883234, 4, 1),
    ('squashweed', 3964, 2888316737, 4, 1),
    ('squashweed', 3964, 3695363022, 4, 4),
    ('squashweed', 3964, 1162580596, 4, 4),
    ('squashweed', 3964, 843883234, 4, 4),
    ('squashweed', 3964, 2888316737, 4, 4),
    ('squashweed', 3963, 3695363022, 4, 1),
    ('squashweed', 3963, 1162580596, 4, 1),
    ('squashweed', 3963, 843883234, 4, 1),
    ('squashweed', 3963, 2888316737, 4, 1),
    ('squashweed', 3963, 1162580596, 4, 3),
    ('squashweed', 3963, 843883234, 4, 3),
    ('squashweed', 3963, 2888316737, 4, 3),
    ('squashweed', 3963, 3695363022, 4, 3),
    ('squashweed', 3963, 3695363022, 4, 4),
    ('squashweed', 3963, 1162580596, 4, 4),
    ('squashweed', 3963, 843883234, 4, 4),
    ('squashweed', 3963, 2888316737, 4, 4),

    ('starthorn', 4675, 1475706289, 4, 1),
    ('starthorn', 4675, 1475706289, 4, 4),
    ('starthorn', 4675, 1475706289, 4, 3),
    ('starthorn', 4675, 3122533480, 1, 2086281974),
    ('starthorn', 4675, 588727762, 1, 2086281974),
    ('starthorn', 4675, 1410364740, 1, 2086281974),
    ('starthorn', 4675, 3396657383, 1, 2086281974),
    ('starthorn', 4676, 1475706289, 4, 1),
    ('starthorn', 4676, 587537642, 4, 1),
    ('starthorn', 4676, 1475706289, 4, 4),
    ('starthorn', 4676, 587537642, 4, 4),
    ('doom-cactus', 3948, 2866508787, 1, 2086281974),
}

class SourceAssets:
    """Own one batch's read-only source trees, decoded channels and sprite PNGs."""

    def __init__(self, objects):
        self.objects = objects
        self._trees = {}
        self._prefabs = {}
        self._sprites = {}
        self._channels = {}

    def read(self, path_id):
        if path_id not in self._trees:
            self._trees[path_id] = self.objects[path_id].read_typetree()
        return self._trees[path_id]

    def prefab(self, path_id):
        """Resolve one prefab's root Transform and optional AnimatorController."""
        if path_id not in self._prefabs:
            def local_object(reference):
                target = reference["m_PathID"]
                if reference["m_FileID"] != 0 or type(target) is not int or target == 0:
                    raise ValueError(f"Prefab {path_id}: expected a nonnull local object reference")
                if target not in self.objects:
                    raise ValueError(f"Prefab {path_id}: missing referenced object {target}")
                return target

            if path_id not in self.objects or self.objects[path_id].type.name != "GameObject":
                raise ValueError(f"Prefab {path_id}: expected a GameObject")
            components = [local_object(ref["component"])
                          for ref in self.read(path_id)["m_Component"]]
            if len(components) != len(set(components)):
                raise ValueError(f"Prefab {path_id}: duplicate component references")
            transforms = [part for part in components if self.objects[part].type.name == "Transform"]
            animators = [part for part in components if self.objects[part].type.name == "Animator"]
            if len(transforms) != 1 or len(animators) > 1:
                raise ValueError(f"Prefab {path_id}: expected one root Transform and at most one Animator")
            transform, = transforms
            if local_object(self.read(transform)["m_GameObject"]) != path_id:
                raise ValueError(f"Prefab {path_id}: root Transform belongs to another GameObject")
            controller = None
            if animators:
                animator = self.read(animators[0])
                if local_object(animator["m_GameObject"]) != path_id:
                    raise ValueError(f"Prefab {path_id}: Animator belongs to another GameObject")
                controller = local_object(animator["m_Controller"])
                if self.objects[controller].type.name != "AnimatorController":
                    raise ValueError(f"Prefab {path_id}: expected an AnimatorController")
            self._prefabs[path_id] = (transform, controller)
        return self._prefabs[path_id]

    def channels(self, path_id):
        """Decode immutable source channels before any model-specific remapping."""
        if path_id not in self._channels:
            clip = self.read(path_id)
            data = clip["m_MuscleClip"]["m_Clip"]["data"]
            stream = data["m_StreamedClip"]
            # Player builds store curves in m_StreamedClip, even though
            # m_PositionCurves/m_FloatCurves are empty. Decode every channel.
            words = stream["data"]
            raw = struct.pack(f"<{len(words)}I", *words)
            pointer_curves = sum(b["isPPtrCurve"] for b in clip["m_ClipBindingConstant"]["genericBindings"])
            # Object-reference keys follow the numeric streamed channels but
            # are excluded from Unity's curveCount.
            channels = [[] for _ in range(stream["curveCount"] + pointer_curves)]
            offset = 0
            key_format = struct.Struct("<i4f")
            view = memoryview(raw)
            duration = clip["m_MuscleClip"]["m_StopTime"]
            while offset < len(raw):
                time, count = struct.unpack_from("<fi", raw, offset)
                offset += 8
                end = offset + count * key_format.size
                if count < 0 or end > len(raw):
                    raise ValueError(f"Clip {path_id}: invalid streamed frame size")
                for index, a, b, c, d in key_format.iter_unpack(view[offset:end]):
                    if not 0 <= index < len(channels):
                        raise ValueError(f"Clip {path_id}: streamed channel outside binding table")
                    if time <= duration:
                        coefficients = (a, b, c, d)
                        assert all(math.isfinite(c) for c in coefficients)
                        channels[index].append((max(-1, time), coefficients))
                offset = end
            assert data["m_DenseClip"]["m_CurveCount"] == 0, "Do not silently drop dense channels"
            for value in data["m_ConstantClip"]["data"]:
                channels.append([(0, (0, 0, 0, value))])
            self._channels[path_id] = tuple(tuple(channel) for channel in channels)
        return self._channels[path_id]

    def sprite(self, path_id):
        if path_id not in self._sprites:
            # UnityPy's object supplies both metadata and image. Reading an
            # additional dict would decode the same Sprite twice.
            data = self.objects[path_id].read()
            skipped = 0
            try:
                decoded = data.image
            except ZeroDivisionError:
                # Unity rasterizes zero-area triangles to no pixels. UnityPy's
                # integer projection can collapse a thin source triangle and
                # then attempt to invert its singular affine matrix. Preserve
                # every nondegenerate triangle and restore the library helper.
                from unittest.mock import patch
                from UnityPy.export import SpriteHelper
                copy_triangle = SpriteHelper.copy_triangle

                def rasterize(source, source_triangle, destination, triangle):
                    nonlocal skipped
                    (ax, ay), (bx, by), (cx, cy) = triangle
                    if (bx-ax)*(cy-ay) == (by-ay)*(cx-ax):
                        skipped += 1
                        return
                    return copy_triangle(source, source_triangle, destination, triangle)

                with patch.object(SpriteHelper, "copy_triangle", rasterize):
                    decoded = data.image
                if skipped == 0:
                    decoded.close()
                    raise
            with decoded as image, BytesIO() as stream:
                image.save(stream, format="PNG")
                png = stream.getvalue()
                # UnityPy exports a trimmed image. Preserve the original pivot
                # relative to that rectangle instead of recentering each part.
                offset = data.m_RD.textureRectOffset
                pivot = tuple(
                    (getattr(data.m_Pivot, axis) * getattr(data.m_Rect, dimension)
                     - getattr(offset, axis)) / size
                    for axis, dimension, size in zip("xy", ("width", "height"), image.size))
            filename = f"sprite-{path_id}.png"
            metadata = dict(File=filename, Pivot=pivot, PixelsPerUnit=data.m_PixelsToUnits)
            evidence = dict(path_id=path_id, name=data.m_Name, type="Sprite", file=filename,
                            sha256=hashlib.sha256(png).hexdigest())
            if skipped:
                evidence["zero_area_triangles_skipped"] = skipped
            self._sprites[path_id] = metadata, png, evidence
        return self._sprites[path_id]


def export(plant, assets, bundle_sha256, *, output_root=None):
    """Export one model using the batch's shared source assets."""
    PLANT = plant
    PREFAB = PLANTS[plant]
    TRANSFORM, CONTROLLER = assets.prefab(PREFAB)
    root = PROJECT if output_root is None else output_root
    OUTPUT = root / "assets" / PLANT
    REFERENCE = root / "references" / PLANT
    OUTPUT.mkdir(parents=True, exist_ok=True)
    REFERENCE.mkdir(parents=True, exist_ok=True)
    bundle = SOURCE / "PlantsVsZombiesRH_Data/data.unity3d"
    nodes, sprites, paths, evidence = [], {}, {}, []
    transforms_by_id, skin_sources, skins = {}, [], []
    objects, read = assets.objects, assets.read

    def sprite(path_id):
        if path_id in sprites:
            return
        metadata, png, proof = assets.sprite(path_id)
        (OUTPUT / metadata["File"]).write_bytes(png)
        sprites[path_id] = metadata.copy()
        evidence.append(proof.copy())

    def visit(transform_id, parent=-1, path=""):
        transform = read(transform_id)
        go = read(transform["m_GameObject"]["m_PathID"])
        if parent >= 0:
            path = f"{path}/{go['m_Name']}".lstrip("/")
        index = len(nodes)
        transforms_by_id[transform_id] = index
        paths[zlib.crc32(path.encode())] = index
        node = dict(Name=go["m_Name"], Path=path, Parent=parent, Active=go["m_IsActive"],
                    Position=vector(transform["m_LocalPosition"], "xyz"),
                    Rotation=vector(transform["m_LocalRotation"], "xyzw"),
                    Scale=vector(transform["m_LocalScale"], "xyz"), Sprite=-1, Order=0,
                    FlipX=False, FlipY=False, Color=[1, 1, 1, 1])
        nodes.append(node)
        for component in go["m_Component"]:
            component_id = component["component"]["m_PathID"]
            kind = objects[component_id].type.name
            if PLANT == "grapple-kelp" and kind == "MonoBehaviour":
                component = objects[component_id]
                script = component.read(check_read=False).m_Script.deref().read_typetree()
                if script["m_ClassName"] == "SpriteSkin":
                    skin_sources.append((index, component_id))
            if PLANT == "grapple-kelp" and kind == "SpriteMask":
                mask = read(component_id)
                sprite_id = mask["m_Sprite"]["m_PathID"]
                sprite(sprite_id)
                node.update(MaskSprite=sprite_id, MaskCutoff=mask["m_MaskAlphaCutoff"])
            if kind != "SpriteRenderer":
                continue
            renderer = read(component_id)
            sprite_id = renderer["m_Sprite"]["m_PathID"]
            if sprite_id:
                sprite(sprite_id)
                node.update(Sprite=sprite_id, Order=renderer["m_SortingOrder"],
                            FlipX=renderer["m_FlipX"], FlipY=renderer["m_FlipY"],
                            Color=vector(renderer["m_Color"], "rgba"), Enabled=renderer["m_Enabled"])
                if (PLANT in ("ice-star-projectile", "star-pea-projectile", "poison-projectile") and renderer["m_Materials"] == [{"m_FileID": 0, "m_PathID": 11}]) or (PLANT == "ice-cactus-projectile" and renderer["m_Materials"] == [{"m_FileID": 0, "m_PathID": 94}]):
                    # The original Gray material desaturates before renderer
                    # tint. Bake this fixed material adaptation into the local
                    # PNG, so native sprite materials and offline previews agree.
                    material_id = 94 if PLANT == "ice-cactus-projectile" else 11
                    material = read(material_id)
                    assert material["m_Name"] == ("LightGray" if material_id == 94 else "Gray")
                    brightness = dict(material["m_SavedProperties"]["m_Floats"]).get("_Brightness", 1)
                    assert brightness == (2 if material_id == 94 else 1)
                    filename = OUTPUT / sprites[sprite_id]["File"]
                    color = node["Color"]
                    with Image.open(filename) as image:
                        gray = image.convert("L")
                        channels = [gray.point(lambda value, factor=factor: min(255, round(value * brightness * factor))) for factor in color[:3]]
                        alpha = image.getchannel("A").point(lambda value: round(value * color[3]))
                        Image.merge("RGBA", (*channels, alpha)).save(filename)
                    proof = next(item for item in evidence if item.get("path_id") == sprite_id and item.get("type") == "Sprite")
                    proof.update(sha256=sha256(filename), gray_material_baked=True, source_renderer_color=color,
                                 source_material_path_id=material_id, source_brightness=brightness, adaptation="Luminance grayscale, source brightness and original renderer tint")
                    node["Color"] = [1, 1, 1, 1]
                if PLANT == "grapple-kelp":
                    node["MaskInteraction"] = renderer["m_MaskInteraction"]
        for child in transform["m_Children"]:
            visit(child["m_PathID"], index, path)

    visit(TRANSFORM)
    if skin_sources:
        from UnityPy.helpers.MeshHelper import MeshHandler
        for node_index, component_id in skin_sources:
            raw = objects[component_id].get_raw_data()
            name_length = struct.unpack_from("<I", raw, 28)[0]
            offset = (32 + name_length + 3) & ~3
            root_file, root_bone = struct.unpack_from("<iq", raw, offset)
            count = struct.unpack_from("<I", raw, offset + 12)[0]
            if root_file != 0 or root_bone not in transforms_by_id or not 0 < count < 256:
                raise ValueError(f"Unsupported SpriteSkin layout: {component_id}")
            bones = []
            for bone in range(count):
                file_id, transform_id = struct.unpack_from("<iq", raw, offset + 16 + 12*bone)
                if file_id != 0 or transform_id not in transforms_by_id:
                    raise ValueError(f"SpriteSkin bone outside original prefab: {component_id}")
                bones.append(transforms_by_id[transform_id])
            sprite_id = nodes[node_index]["Sprite"]
            source_sprite = objects[sprite_id].read()
            mesh = MeshHandler(source_sprite.m_RD, objects[sprite_id].version)
            mesh.process()
            bindposes = source_sprite.m_RD.m_Bindpose
            assert len(bindposes) == len(bones) == len(source_sprite.m_Bones)
            assert len(mesh.m_Vertices) == len(mesh.m_BoneWeights) == len(mesh.m_BoneIndices)
            assert all(abs(sum(w)-1) < .0001 for w in mesh.m_BoneWeights)
            assert all(0 <= b < count for row in mesh.m_BoneIndices for b in row)
            vertices = [list(v) for v in mesh.m_Vertices]
            skins.append(dict(Node=node_index, Sprite=sprite_id, Bones=bones, Vertices=vertices,
                              Weights=[list(v) for v in mesh.m_BoneWeights],
                              Indices=[list(v) for v in mesh.m_BoneIndices],
                              BindPoses=[[getattr(b, f"e{r}{c}") for r in range(4) for c in range(4)] for b in bindposes],
                              Triangles=[i for group in mesh.get_triangles() for triangle in group for i in triangle]))
            # Mesh export rasterizes in its original vertex bounding box.
            # Its pivot must retain original vertex coordinates for native UVs.
            with Image.open(OUTPUT / sprites[sprite_id]["File"]) as decoded:
                ppu = source_sprite.m_PixelsToUnits
                sprites[sprite_id]["Pivot"] = [-min(v[a] for v in vertices)*ppu / decoded.size[a] for a in range(2)]
            evidence.append(dict(path_id=component_id, type="SpriteSkin", bones=count,
                                 vertices=len(vertices), raw_sha256=hashlib.sha256(raw).hexdigest()))
    damage_sprites = None
    if PLANT == "chomp-nut":
        # Original ResourceManager paths plants/_mixer/nutchomper/cracked*.
        # NutChomper.Awake/ReplaceSprite load these independently of its prefab.
        damage_sprites = {"Wallnut_body": [15822, 14774], "head": [14884, 15528]}
        for ids in damage_sprites.values():
            for path_id in ids:
                sprite(path_id)
    controller = read(CONTROLLER) if CONTROLLER is not None else None
    layers = controller["m_Controller"]["m_LayerArray"] if controller else []
    projectile = PLANT.endswith("-projectile") or PLANT in ("cherry-spore", "potato-spore")
    passive = PLANT in ("hot-potato", "boomwood") or PLANT in ("roasted-squash", "roasted-wall-nut", "roasted-pumpkin", "roasted-potato", "umbrella-kale", "umbrella-rind", "lumos-umbrella", "ice-pot") or PLANT in ("threetato-mine", "marigold-mine") or PLANT in ("triplantern", "aurora-plantern", "glowver", "frenzy-shroom") or PLANT in ("tangle-mine", "kelp-mine", "sea-mine", "potato-masher", "potato-mash") or PLANT in ("golden-nut", "golden-sunflower") or PLANT in ("butterweed", "springweed", "splash", "sea-nut") or PLANT in ("starthorn", "doom-cactus", "lumos-cactus", "thornweed") or PLANT in ("ray-shroom", "blubber-shroom", "sunrise-shroom", "bush-shroom") or PLANT in ("kernel-morph", "rust-shroom", "metal-pult", "iron-star", "pyro-pumpkin", "doom-garlic", "doom-nut", "doomflower", "crater-tot", "doom-seed", "potato-seed-projectile") or PLANT in ("dormant-shroom", "morph-shroom", "starmorph", "melon-morph") or PLANT in ("spring-melon", "little-lettuce-projectile") or PLANT in ("windy-pumpkin", "spiky-pumpkin", "jack-o-plantern", "sea-shell") or PLANT in ("cherry-pumpkin", "stinky-pumpkin", "scareless-pumpkin", "nut-shell", "starry-pumpkin", "thorny-pumpkin", "puff-kin", "fuming-pumpkin", "zombie-block-projectile", "zombie-block2-projectile", "zombie-block3-projectile") or PLANT in ("beacon-shroom", "jelly-shroom", "swell-shroom", "hermit-shroom") or PLANT in ("minnow-shroom", "urchin-shroom", "shark-shroom", "sea-star") or PLANT in ("hypno-pepper", "hypno-melon", "flame-cattail", "frost-cattail") or PLANT in ("starlic", "winter-starfruit", "spicy-starfruit") or PLANT in ("glowver", "cyclover", "disguster", "breeze-blover", "mind-blover", "tingle-kelp", "grapple-kelp", "tangle-nut", "spike-kelp") or PLANT in ("solar-pot", "sunset-shroom", "solar-flare", "solar-blover") or PLANT in ("sun-shroom", "gnome-shroom", "umbrella-cap") or PLANT in ("hail-shroom", "icicle-shroom", "twinkle-shroom", "nugget-shroom") or PLANT in ("nut-shroom", "bell-pepper", "boom-shroom", "penny-shroom") or PLANT in ("pea-shroom", "repeat-shroom", "split-pea-shroom", "gatling-pea-shroom") or PLANT in ("football-helmet", "football-nut") or PLANT in ("fallout-blover", "solarfruit", "hypno-garlic", "flytrapper") or PLANT in ("sunny-shroom", "starry-shroom", "sturdy-shroom", "camo-shroom") or PLANT in ("speedy-shroom", "blasty-shroom", "shivery-shroom", "spuddy-shroom", "cherry-spore", "potato-spore") or PLANT in ("black-cherry", "frozen-cherry", "demise-shroom", "sundae-shroom") or PLANT in ("gloom-shroom", "frost-gloom-shroom", "flame-gloom-shroom") or PLANT == "charred-threepeater" or PLANT in ("icerock", "hearthrock", "foul-shroom", "starch-shroom") or PLANT in ("jawbreaker", "doom-chomper", "frostbiter") or PLANT in ("frost-shroom", "soot-shroom", "fume-spreader") or PLANT in ("spicy-kelp", "doom-kelp") or PLANT in ("butter-nut", "umbrella-husk", "torchflower", "land-mine") or PLANT in ("foul-nut", "star-nut") or PLANT in ("thorn-nut", "ice-mine", "hypno-mine", "chip-mine") or PLANT in ("solarweed", "iceweed", "scorchweed", "spike-nut") or PLANT in ("flame-wall-nut", "frost-wall-nut", "flame-tall-nut", "frost-tall-nut") or PLANT.endswith("-squash") or PLANT in ("twin-marigold", "umbrella-shell", "chomp-mine", "solar-chomper", "solar-nut", "twin-solar-nut", "solar-nut-ball", "peatato-mine", "solar-mine", "mine-nut", "bucket", "buck-nut", "cherry-nut", "solar-bomb", "cluster-mine", "cherry-chomper", "chompzilla", "chomp-nut")
    if PLANT in ("sun-shroom", "gnome-shroom", "umbrella-cap", "hail-shroom", "icicle-shroom", "twinkle-shroom", "nugget-shroom", "nut-shroom", "bell-pepper", "boom-shroom", "penny-shroom", "pea-shroom", "repeat-shroom", "split-pea-shroom", "gatling-pea-shroom", "sunny-shroom", "starry-shroom", "sturdy-shroom", "camo-shroom", "doom-squash", "jawbreaker", "doom-chomper", "frostbiter", "solarweed", "iceweed", "scorchweed", "spike-nut"):
        assert len(layers) == 1
        assert layers[0]["data"]["(int&)m_LayerBlendingMode"] == 0
        assert not layers[0]["data"]["m_SkeletonMask"]["data"]["m_Data"]
    if PLANT == "black-cherry":
        assert len(layers) == 2 and layers[1]["data"]["m_DefaultWeight"] == 1
    if PLANT in ("gloom-shroom", "frost-gloom-shroom", "flame-gloom-shroom", "gutsy-shroom"):
        assert len(layers) == (2 if PLANT in ("flame-gloom-shroom", "gutsy-shroom") else 1)
        assert layers[0]["data"]["(int&)m_LayerBlendingMode"] == 0
        assert all(not layer["data"]["m_SkeletonMask"]["data"]["m_Data"] for layer in layers)
    if PLANT in ("damage-pult", "lettuce-pult", "butternut-pult", "gourd-pult", "garbage-pult", "clove-pult", "garlic-pult", "summer-melon", "coldslaw", "kimchi-pult", "cobsicle", "salad-pult", "taco-pult", "cob-pult", "overripe-melon", "cabbage-tri-pult", "pea-pot", "tri-pot", "kernel-tri-pult", "cherry-pot", "hot-pot", "hypno-pot", "doom-pot", "nut-pot", "garlic-pot", "spike-pot", "melon-pot", "cabbage-pot", "butter-pot", "umbrella-pot", "silver-pot", "silver-umbrella", "silver-kernel", "lumos-pot", "silver-doom-shroom", "silver-hypno-shroom", "silver-cabbage", "silver-melon", "silver-nut", "silver-garlic", "silver-ice-shroom", "silver-sunflower", "buck-shroom", "sprout-pult", "baby-corn", "slice-pult"):
        passive = True
        assert len(layers) == (2 if PLANT in ("garbage-pult", "summer-melon", "salad-pult", "coldslaw", "kimchi-pult", "silver-cabbage", "hot-pot", "cabbage-tri-pult", "damage-pult") else 1)
        assert layers[0]["data"]["(int&)m_LayerBlendingMode"] == 0
        assert not layers[0]["data"]["m_SkeletonMask"]["data"]["m_Data"]
        if PLANT in ("garbage-pult", "summer-melon", "coldslaw", "kimchi-pult", "silver-cabbage", "hot-pot", "cabbage-tri-pult", "damage-pult"):
            assert layers[1]["data"]["(int&)m_LayerBlendingMode"] == 1
            assert layers[1]["data"]["m_DefaultWeight"] == 1
            assert layers[1]["data"]["m_Binding"] == zlib.crc32(b"fire")
    if PLANT == "salad-pult":
        assert layers[1]["data"]["(int&)m_LayerBlendingMode"] == 1
        assert layers[1]["data"]["m_DefaultWeight"] == 1
        assert layers[1]["data"]["m_Binding"] == zlib.crc32(b"shoot")
    if PLANT in ("glowver", "cyclover", "disguster", "breeze-blover", "mind-blover", "solar-pot", "sunset-shroom", "solar-flare", "solar-blover"):
        assert len(layers) == (2 if PLANT == "solar-flare" else 1)
        assert layers[0]["data"]["(int&)m_LayerBlendingMode"] == 0
        assert all(not layer["data"]["m_SkeletonMask"]["data"]["m_Data"] for layer in layers)
        if PLANT == "solar-flare":
            assert layers[1]["data"]["(int&)m_LayerBlendingMode"] == 1
            assert layers[1]["data"]["m_DefaultWeight"] == 1
            assert layers[1]["data"]["m_Binding"] == zlib.crc32(b"shake")
    if PLANT in ("melon-shell", "cryo-pumpkin", "cabbage-nut", "melon-nut", "squash-nut"):
        passive = True
        assert len(layers) == 1 and layers[0]["data"]["(int&)m_LayerBlendingMode"] == 0
    if PLANT in ("boost-shroom", "blowtorch-shroom", "wallop-shroom", "caltrop-shroom"):
        passive = True
        assert len(layers) == (2 if PLANT == "blowtorch-shroom" else 1)
        assert layers[0]["data"]["(int&)m_LayerBlendingMode"] == 0
        assert all(not layer["data"]["m_SkeletonMask"]["data"]["m_Data"] for layer in layers)
    if PLANT in ("saguaro-shroom", "ice-cactus", "magnet-cactus", "illumi-nut"):
        passive = True
        assert len(layers) == (2 if PLANT == "magnet-cactus" else 1)
        assert layers[0]["data"]["(int&)m_LayerBlendingMode"] == 0
        assert all(not layer["data"]["m_SkeletonMask"]["data"]["m_Data"] for layer in layers)
    if PLANT in ("umbrella-clove", "umbrella-thorn", "starburst-shroom"):
        passive = True
        assert len(layers) == 1
        assert layers[0]["data"]["(int&)m_LayerBlendingMode"] == 0
        assert not layers[0]["data"]["m_SkeletonMask"]["data"]["m_Data"]
    if PLANT in ("squashweed", "perfume-shroom", "tangle-weed", "blover-pot", "scaredy-pot", "sea-pot"):
        passive = True
        assert len(layers) == 1 and layers[0]["data"]["(int&)m_LayerBlendingMode"] == 0
    if PLANT in ("cherry-pop", "ball-cactus", "belch-shroom", "hungry-shroom", "trigger-shroom", "spicicle"):
        passive = True
        assert len(layers) == (2 if PLANT == "spicicle" else 1) and layers[0]["data"]["(int&)m_LayerBlendingMode"] == 0
    if PLANT in ("chompkin", "pumpkin-mine", "sunny-pumpkin", "glacier-shroom"):
        passive = True
        assert len(layers) == 1
    beam = PLANT in ("beamshooter", "rebeamer", "split-beam", "gatling-beam")
    if beam:
        passive = True
        assert len(layers) == 1 and layers[0]["data"]["(int&)m_LayerBlendingMode"] == 0
        assert not layers[0]["data"]["m_SkeletonMask"]["data"]["m_Data"]
    if PLANT in ("tide-shroom", "blacklight-plantern", "scorchwood", "stenchwood", "nutwood", "torchthree", "sparkwood", "hypnowood"):
        passive = True
        assert len(layers) == 1
    shoot_layer = layers[1]["data"] if PLANT in ("little-lettuce-projectile", "salad-pult", "black-cherry", "flame-gloom-shroom", "peatato-mine", "threetato-mine", "flame-wall-nut", "flame-tall-nut", "solar-squash", "spicy-kelp", "doom-kelp") else None if passive else ((layers[1]["data"] if not projectile else layers[0]["data"]) if controller else None)
    assert shoot_layer is None or shoot_layer["(int&)m_LayerBlendingMode"] == (0 if projectile and PLANT != "little-lettuce-projectile" else 1)
    assert passive or projectile or shoot_layer["m_DefaultWeight"] == 1
    if PLANT in ("flame-gloom-shroom", "flame-wall-nut", "flame-tall-nut", "solar-squash", "spicy-kelp", "doom-kelp"):
        assert len(layers) == 2 and shoot_layer["m_DefaultWeight"] == 1
        assert layers[0]["data"]["(int&)m_LayerBlendingMode"] == 0
    if PLANT in ("peatato-mine", "threetato-mine"):
        assert shoot_layer["m_Binding"] == zlib.crc32(b"shoot")
        assert shoot_layer["m_DefaultWeight"] == 1
    assert shoot_layer is None or not shoot_layer["m_SkeletonMask"]["data"]["m_Data"]
    if PLANT == "starch-shroom":
        assert len(layers) == 2
        flash_layer = layers[1]["data"]
        assert flash_layer["(int&)m_LayerBlendingMode"] == 1
        assert abs(flash_layer["m_DefaultWeight"] - .97) < .000001
        assert not flash_layer["m_SkeletonMask"]["data"]["m_Data"]
    if PLANT in ("cherry-spreader", "stench-spreader", "snow-threepeater", "scorched-threepeater", "doom-spreader", "phoenix-threepeater"):
        assert len(layers) == (5 if PLANT == "doom-spreader" else 4)
        for head in range(1, len(layers)):
            layer = layers[head]["data"]
            assert layer["(int&)m_LayerBlendingMode"] == 1 and layer["m_DefaultWeight"] == 1
            assert not layer["m_SkeletonMask"]["data"]["m_Data"]
    clips = {}
    ignored_bindings = []
    for reference in controller["m_AnimationClips"] if controller else []:
        path_id = reference["m_PathID"]
        clip = read(path_id)
        duration = clip["m_MuscleClip"]["m_StopTime"]
        # Each model owns mutable keys: object-reference bindings replace the
        # source mapping index with a sprite ID below.
        channels = [[dict(Time=time, Coefficients=list(coefficients))
                     for time, coefficients in channel] for channel in assets.channels(path_id)]
        bindings, cursor = [], 0
        for binding in clip["m_ClipBindingConstant"]["genericBindings"]:
            attribute = binding["attribute"]
            if binding["typeID"] == 4 and attribute in (1, 3, 4):
                width = 3
            elif binding["typeID"] == 212 and binding["isPPtrCurve"] == 1 and binding["customType"] == 23:
                attribute, width = 5, 1
                mapping = clip["m_ClipBindingConstant"]["pptrCurveMapping"]
                for key in channels[cursor]:
                    assert key["Coefficients"][:3] == [0, 0, 0]
                    reference = mapping[int(key["Coefficients"][3])]
                    assert reference["m_FileID"] == 0 and reference["m_PathID"]
                    sprite(reference["m_PathID"])
                    key["Coefficients"][3] = reference["m_PathID"]
            elif binding["typeID"] == 212 and attribute == zlib.crc32(b"m_Color.a"):
                if binding["path"] in paths:
                    attribute, width = 7, 1
                else:
                    # Only these verified stale tracks may be ignored.
                    assert (PLANT in ("blaze-pea", "poison-pea", "hypno-pea", "doom-pea") and path_id == 4338) or PLANT in ("triplantern", "aurora-plantern"), (PLANT, path_id, binding)
                    assert binding["path"] in (3325650563, 1870304024) or PLANT in ("triplantern", "aurora-plantern"), (PLANT, path_id, binding)
                    ignored_bindings.append(dict(clip=path_id, binding=binding, reason="Renderer absent from original prefab; Unity ignores this track"))
                    cursor += 1
                    continue
            elif binding["typeID"] == 212 and attribute == zlib.crc32(b"m_SortingOrder"):
                attribute, width = 12, 1
            elif binding["typeID"] == 212 and attribute == zlib.crc32(b"m_Enabled"):
                attribute, width = 6, 1
            elif (PLANT in ("boost-shroom", "blowtorch-shroom", "caltrop-shroom")
                  and binding["typeID"] == 198 and attribute == 1087013301):
                ignored_bindings.append(dict(clip=path_id, binding=binding,
                    reason="Fume emission is owned by FumeVariants using native FumeCloud"))
                cursor += 1
                continue
            elif (PLANT == "fume-spreader" and path_id == 4138
                  and binding["typeID"] == 198 and attribute == 1087013301
                  and binding["path"] in (545976955, 726995299, 2992390361)):
                # These three original particle emission curves are replaced
                # by native FumeCloud effects at the saved attack event.
                ignored_bindings.append(dict(clip=path_id, binding=binding,
                    reason="Original ParticleSystem emission replaced by native FumeCloud"))
                cursor += 1
                continue
            elif (PLANT == "cyclover" and path_id == 3850 and binding["typeID"] == 198
                  and attribute == 1087013301 and binding["path"] == 922205677):
                ignored_bindings.append(dict(clip=path_id, binding=binding,
                    reason="Wind emission is owned by BloverFusions using native FumeCloud"))
                cursor += 1
                continue
            else:
                assert binding["typeID"] == 1 and attribute == zlib.crc32(b"m_IsActive"), (PLANT, path_id, binding)
                attribute, width = 0, 1
            if binding["path"] not in paths:
                # These shipped clips have verified stale transform tracks.
                # Unity cannot apply it: this transform is absent in its prefab.
                assert (
                    PLANT in ("triplantern", "aurora-plantern")
                    or (PLANT == "cherry-drone-projectile" and path_id in (4441, 4442, 4443)
                        and binding["path"] in (2937602033, 3344912154, 2978337722)
                        and binding["typeID"] == 4 and attribute in (1, 3, 4))
                    or (PLANT == "cherry-drone-projectile" and binding["typeID"] == 1 and attribute == 0
                        and (path_id == 4441 or path_id == 4443 and binding["path"] in (2409646917, 380181247, 1638804073, 3344912154)))
                    or (PLANT == "sparkwood" and path_id in (4675, 4676))
                    or (PLANT == "stench-fire-projectile" and path_id == 3714 and binding["path"] == 3075048504 and binding["typeID"] == 212 and attribute == 5)
                    or (PLANT, path_id, binding["path"], binding["typeID"], binding["attribute"]) in CACTUS_STALE_BINDINGS
                    or (PLANT == "solar-pult" and path_id == 3901 and binding["path"] == 3829906448
                        and binding["typeID"] == 212 and binding["isPPtrCurve"] == 1 and attribute == 5)
                    or (PLANT == "phoenix-threepeater" and binding["typeID"] == 4
                        and ((path_id in (4725, 4729) and binding["path"] == 1697178676 and attribute in (1, 3, 4))
                             or (path_id in (4726, 4730) and binding["path"] in (1715825968, 3269839459) and attribute in (1, 3, 4))))
                    or (PLANT == "tangle-mine" and path_id == 4371
                        and binding["path"] == 487598247 and binding["typeID"] == 1 and attribute == 0)
                    or (PLANT in ("split-beam", "gatling-beam") and (path_id == 4329 or PLANT == "split-beam" and path_id == 4330)
                        and attribute in (1, 3, 4) and binding["path"] in
                        (3237490456, 3281950316, 4173889738, 3814635193, 2050198470, 142323157, 2599858270))
                    or (PLANT == "death-star" and ((path_id == 4675 and attribute == 0
                        and binding["path"] in (4071916437, 3122533480, 588727762, 1410364740, 3396657383))
                        or (path_id == 4676 and ((attribute in (0, 1, 3) and binding["path"] in (4071916437, 4133887216, 1861999629, 1869565258))
                            or (attribute == 4 and binding["path"] == 1861999629)))))
                    or (PLANT == "hypno-pepper" and attribute in (1,3,4) and ((path_id == 4243 and binding["path"] in (2403679267,3966615996,410818238)) or (path_id == 4242 and binding["path"] in (410818238,1039954639,2767528821,3966615996))))
                    or (PLANT == "hypno-melon" and path_id == 4295 and attribute == 0 and binding["path"] in (3847630310,2086632540))
                    or (PLANT in ("winter-starfruit", "starmorph", "iron-star") and path_id == 4675 and attribute == 0 and binding["path"] in (3122533480, 588727762, 1410364740, 3396657383))
                    or (PLANT in ("damage-pult", "garbage-pult") and path_id == 3901 and binding["path"] == 3829906448 and attribute == 5)
                    or (PLANT == "blowtorch-shroom" and path_id in (4158, 4159) and binding["path"] in (3653964320, 1086473114) and attribute in (0, 4))
                    or (PLANT == "spike-kelp" and path_id == 4695 and attribute == 0
                     and binding["path"] in (2567088312, 784642, 1997326740, 3915930679, 993902582,
                                             2721377868, 3576946394, 1264012153, 1011907567))
                    or (PLANT in ("solar-pot", "lumos-pot", "cabbage-pot", "butter-pot", "umbrella-pot") and path_id == 4355 and binding["path"] in (2586839070, 54090148) and attribute in (1, 3, 4))
                    or (PLANT == "silver-doom-shroom" and path_id in (4097, 4096)
                        and binding["path"] in (266101495, 2027639393) and attribute in (0, 1, 3, 4))
                    or (PLANT == "cabbage-tri-pult" and path_id == 3915
                        and binding["path"] in (2066509225, 2019510890, 3780516816, 137740005,
                            2521901894, 1239188067, 805949527, 2143261350)
                        and attribute in (1, 4))
                    or (PLANT in ("coldslaw", "silver-cabbage", "cabbage-tri-pult") and path_id == 3901
                        and binding["path"] == 3829906448 and attribute == 5)
                    or (PLANT == "solar-flare" and path_id in (4242, 4243, 4244) and binding["path"] == 410818238 and attribute in (1, 3, 4))
                    or (PLANT == "pea-shroom" and path_id == 4978
                        and binding["path"] in (2792144893, 1063644743) and attribute == 0)
                    or (PLANT in ("hypno-garlic", "silver-garlic") and path_id == 4162
                        and binding["path"] in (4211282168, 1644839234, 353202644) and attribute in (1, 3, 4))
                    or (PLANT == "hypno-pea" and path_id == 4339
                        and binding["path"] in (3575261756, 1276345222) and attribute == 0)
                    or (PLANT == "split-hypno" and path_id == 4336
                        and binding["path"] in (3575261756, 1276345222, 4201947147, 3184843483) and attribute == 0)
                    or (PLANT in ("gatling-hypno", "football-gatling") and path_id == 4187
                        and binding["path"] in (2975661367, 676703373) and attribute == 0)
                    or (PLANT == "jawbreaker" and path_id in (4648, 4649)
                        and binding["path"] == 2241019588 and attribute == 1)
                    or (PLANT == "doom-chomper" and path_id in (4026, 4027, 4028, 4029)
                        and binding["path"] in (209581121, 2451136994, 3843969396)
                        and attribute in (1, 3, 4))
                    or (PLANT in ("hypno-mine", "roasted-potato") and ((path_id in (4401, 4397)
                        and binding["path"] == 1943561251 and attribute == 1)
                        or (path_id == 4400 and binding["path"] in (487598247, 1943561251) and attribute == 0)))
                    or (PLANT in ("blaze-repeater", "poison-repeater", "hypno-repeater") and path_id == 4098
                     and binding["path"] in (2197108821, 1132193685, 728460060, 1537053841, 1814163765, 1852265793)
                     and attribute in (1, 4))
                    or (PLANT == "solar-nut-ball" and path_id == 4881
                     and binding["path"] in (3325891172, 1597260766) and attribute in (1, 3, 4))
                    or (PLANT == "solar-shooter" and path_id == 4980
                     and binding["path"] == 2349996250 and attribute in (1, 3))
                    or (PLANT == "explode-o-shooter" and path_id == 5022
                        and binding["path"] in (111691728, 4230504051, 3607432624)
                        and attribute == 1)
                    or (PLANT == "chompzilla" and path_id in (5030, 5031)
                        and binding["path"] in (1217738756, 3516688830, 3190828700, 658079526)
                        and attribute in (1, 3, 4))
                    or (PLANT == "chomp-nut" and path_id in (4956, 4957, 4958, 4959)
                        and binding["path"] in (2469664670, 362833200, 3751629467, 1493770293)
                        and attribute in (1, 3, 4))
                    or (PLANT == "cherry-chomper" and attribute in (1, 3, 4)
                        and ((path_id == 4898 and binding["path"] in (2648463464, 81070546, 1943157060, 3988172007))
                             or (path_id == 4899 and binding["path"] in (2451136994, 209581121))))
                ), (PLANT, path_id, binding)
                ignored_bindings.append(dict(clip=path_id, binding=binding, reason="Target absent from original prefab; Unity ignores this track"))
                cursor += width
                continue
            assert binding["isPPtrCurve"] == 0 or attribute == 5
            bindings.append(dict(Node=paths[binding["path"]], Attribute=attribute,
                                 Channels=channels[cursor:cursor + width]))
            cursor += width
        assert cursor == len(channels) and all(channels)
        clips[clip["m_Name"]] = dict(Duration=duration, Bindings=bindings)
        evidence.append(dict(path_id=path_id, name=clip["m_Name"], type="AnimationClip",
                             sample_rate=clip["m_SampleRate"], duration=duration,
                             channels=len(channels), events=clip["m_Events"]))
    if PLANT == "starch-shroom":
        assert all(binding["Attribute"] == 5 for binding in clips["flash"]["Bindings"])
    if controller is None:
        clips["idle"] = dict(Duration=1, Bindings=[])
    elif PLANT == "cherry-spreader-bomb-projectile":
        assert set(clips) == {"idle", "charge"}
    elif PLANT == "phoenix-projectile":
        assert set(clips) == {"FirePeaIdle", "stop"}, set(clips)
        clips["idle"] = clips.pop("FirePeaIdle")
        # stop is the source collision state; preserve it for evidence.
    elif PLANT == "cherry-drone-projectile":
        assert set(clips) == {"idle", "shoot", "blowloop"}, set(clips)
    elif PLANT == "fire-cherry-projectile":
        assert set(clips) == {"ireCherryIdle"}, set(clips)
        clips["idle"] = clips.pop("ireCherryIdle")
    elif PLANT in ("scorch-projectile", "small-fire-projectile", "potato-fire-projectile"):
        assert set(clips) == {"FirePeaIdle"}
        clips["idle"] = clips.pop("FirePeaIdle")
    elif PLANT in ("fire-track-projectile", "stench-fire-projectile", "hypno-fire-projectile"):
        assert set(clips) == {"idle"}
    elif projectile and PLANT != "little-lettuce-projectile":
        assert set(clips) == {"Solar"}
        clips["idle"] = clips.pop("Solar")
    expected_clips = {"idle", "shoot", "blowloop"} if PLANT == "cherry-drone-projectile" else {"idle", "shoot", "fire"} if PLANT == "solar-pult" else {"idle", "stop"} if PLANT == "phoenix-projectile" else {"idle", "charge"} if PLANT == "cherry-spreader-bomb-projectile" else {"idle"} if projectile else {"idle", "shoot"}
    if PLANT in ("glowver", "cyclover", "disguster", "breeze-blover", "mind-blover", "fallout-blover", "solarfruit", "hypno-garlic", "flytrapper"):
        assert len(layers) == 1 and layers[0]["data"]["(int&)m_LayerBlendingMode"] == 0
        assert not layers[0]["data"]["m_SkeletonMask"]["data"]["m_Data"]
        expected_clips = {"garbage-pult": {"idle", "shoot", "fire"}, "clove-pult": {"idle", "shoot", "shoot2"}, "garlic-pult": {"idle", "shoot"}, "summer-melon": {"idle", "shoot", "fire"}, "boost-shroom": {"idle", "shoot"}, "wallop-shroom": {"idle", "shoot"}, "blowtorch-shroom": {"idle", "shoot", "fire"}, "caltrop-shroom": {"idle", "attack"}, "cyclover": {"idle", "blow"}, **{n: {"idle", "blow", "blowloop"} for n in ("glowver", "disguster", "breeze-blover", "mind-blover")}, "fallout-blover": {"idle", "blow", "blowloop"}, "solarfruit": {"idle", "shoot"},
                          "hypno-garlic": {"idle"}, "flytrapper": {"idle", "bite", "chew", "swallow"}}[PLANT]
    if passive and not beam and not PLANT.endswith("-squash") and PLANT not in ("fallout-blover", "solarfruit", "hypno-garlic", "flytrapper"):
        expected_clips = {"hot-potato": {"idle", "fire"}, "boomwood": {"idle"}, "fire-cherry-projectile": {"idle"}, **{name: {"idle"} for name in ("nutwood", "torchthree", "hypnowood")}, "sparkwood": {"idle", "shoot"}, "tide-shroom": {"idle", "blow", "blowloop"}, "blacklight-plantern": {"idle"}, "scorchwood": {"idle"}, "stenchwood": {"idle"}, "stench-fire-projectile": {"idle"}, "roasted-wall-nut": {"idle", "nothing"}, "roasted-pumpkin": {"idle"}, "roasted-potato": {"idle", "rise", "armed", "mashed", "flash"}, "umbrella-kale": {"idle", "block", "block2"}, "umbrella-rind": {"idle", "block", "block2"}, "lumos-umbrella": {"idle", "block"}, "ice-pot": {"idle"}, "chompkin": {"idle", "chomp"}, "pumpkin-mine": {"idle"}, "sunny-pumpkin": {"idle"}, "glacier-shroom": {"idle", "shoot"}, "belch-shroom": {"idle", "shoot", "chomp"}, "hungry-shroom": {"idle", "shoot", "scared", "scaredidle", "scaredgrow"}, "trigger-shroom": {"idle", "shoot"}, "spicicle": {"idle", "explode", "shake"}, "cherry-pop": {"idle"}, "ball-cactus": {"idle", "shoot"}, "multipeater": {"idle", "idle2", "idle3", "shoot1", "shoot2", "shoot3", "fly"}, "sea-pot": {"idle"}, "blover-pot": {"idle"}, "scaredy-pot": {"idle"}, "triplantern": {"idle"}, "aurora-plantern": {"idle"}, "perfume-shroom": {"idle", "shoot"}, "tangle-weed": {"idle", "attack"}, "tangle-mine": {"idle","rise","armed","mashed","flash","grab"}, "kelp-mine": {"idle","grab"}, "sea-mine": {"idle"}, "potato-masher": {"idle","rise","idle2","flash"}, "potato-mash": {"idle","lookleft","lookright","jump","jumppop"}, "golden-nut": {"idle", "nothing"}, "golden-sunflower": {"idle"}, "squashweed": {"idle", "attack"}, "butterweed": {"idle", "attack"}, "springweed": {"idle", "attack"}, "splash": {"idle", "lookleft", "lookright", "jump", "jumppop"}, "sea-nut": {"idle", "round"}, "thornweed": {"idle", "attack"}, "starthorn": {"idle", "shoot"}, "doom-cactus": {"idle", "idle2", "rise", "lower", "shoot1", "shoot2", "superskill"}, "lumos-cactus": {"idle", "idle2", "rise", "lower", "shoot1", "shoot2"}, "ray-shroom": {"idle", "shoot"}, "blubber-shroom": {"idle", "shoot"}, "sunrise-shroom": {"idle"}, "bush-shroom": {"idle", "shoot", "block"}, "umbrella-clove": {"idle", "block"}, "umbrella-thorn": {"idle", "block"}, "damage-pult": {"idle", "shoot", "fire"}, "starburst-shroom": {"idle", "shoot"}, "doom-cabbage-projectile": {"idle"}, "star-cap-projectile": {"idle"}, "kernel-morph": {"idle", "shoot", "shoot2"}, "rust-shroom": {"idle", "shoot"}, "metal-pult": {"idle", "shoot"}, "iron-star": {"idle", "shoot"}, "melon-shell": {"idle"}, "cryo-pumpkin": {"idle"}, "pyro-pumpkin": {"idle", "fire"}, "doom-garlic": {"idle"}, "doom-nut": {"idle", "nothing"}, "doomflower": {"idle"}, "crater-tot": {"idle", "rise", "unrised", "flash"}, "doom-seed": {"idle"}, "potato-seed-projectile": {"idle"}, "illumi-nut": {"idle"}, "saguaro-shroom": {"idle", "shoot", "1to2", "2to1", "idle2", "shoot2"}, **{n: {"idle", "shoot1", "rise", "lower", "idle2", "shoot2"} for n in ("ice-cactus", "magnet-cactus")}, "dormant-shroom": {"idle"}, "morph-shroom": {"idle", "shoot"}, "melon-morph": {"idle", "shoot"}, "starmorph": {"idle", "shoot"}, "little-lettuce-projectile": {"idle", "shoot"}, "lettuce-pult": {"idle", "shoot"}, "butternut-pult": {"idle", "shoot", "shoot2"}, "gourd-pult": {"idle", "shoot"}, "spring-melon": {"idle", "idle2", "shoot", "wait"}, **{n: {"idle"} for n in ("windy-pumpkin", "spiky-pumpkin", "jack-o-plantern", "sea-shell")},**{n: {"idle"} for n in ("cherry-pumpkin", "stinky-pumpkin", "scareless-pumpkin", "nut-shell", "starry-pumpkin", "thorny-pumpkin", "puff-kin", "zombie-block-projectile", "zombie-block2-projectile", "zombie-block3-projectile")}, "fuming-pumpkin": {"idle", "shoot"}, "beacon-shroom": {"idle", "grow", "idle2"}, "jelly-shroom": {"idle", "eat"}, "swell-shroom": {"idle", "freezed"}, "hermit-shroom": {"idle", "shoot", "suohuiqu", "scaredidle", "jihuo", "sea"}, "minnow-shroom": {"idle","shoot"}, "urchin-shroom": {"idle","idle2","grow","shoot","shoot2","shoot_grown","shoot2_grown"}, "shark-shroom": {"idle","shoot","eat"}, "sea-star": {"idle","idle2","grow","shoot","shoot2"}, "starlic": {"idle", "shoot"}, "winter-starfruit": {"idle", "shoot"}, "spicy-starfruit": {"idle", "shake"}, "starshooter": {"idle", "shoot"}, "garbage-pult": {"idle", "shoot", "fire"}, "clove-pult": {"idle", "shoot", "shoot2"}, "garlic-pult": {"idle", "shoot"}, "summer-melon": {"idle", "shoot", "fire"}, "boost-shroom": {"idle", "shoot"}, "wallop-shroom": {"idle", "shoot"}, "blowtorch-shroom": {"idle", "shoot", "fire"}, "caltrop-shroom": {"idle", "attack"}, "cyclover": {"idle", "blow"}, **{n: {"idle", "blow", "blowloop"} for n in ("glowver", "disguster", "breeze-blover", "mind-blover")}, "tingle-kelp": {"idle", "explode", "water"}, "grapple-kelp": {"idle", "attack", "f(x)=sinx", "suohuiqu", "water"}, "tangle-nut": {"idle"}, "spike-kelp": {"idle", "attack", "grab", "water"}, "cabbage-nut": {"idle", "shoot"}, "melon-nut": {"idle", "shoot"}, "squash-nut": {"idle", "jump"}, "coldslaw": {"idle", "shoot", "fire"}, "kimchi-pult": {"idle", "shoot", "fire"}, "cobsicle": {"idle", "shoot", "shoot2"}, "salad-pult": {"idle", "shoot"}, "taco-pult": {"idle", "shoot", "shoot2"}, "cob-pult": {"idle", "shoot", "shoot2"}, "overripe-melon": {"idle", "shoot"}, "cabbage-tri-pult": {"idle", "shoot", "fire"}, "pea-pot": {"idle"}, "tri-pot": {"idle"}, "kernel-tri-pult": {"idle", "shoot", "shoot2"}, "melon-tri-pult": {"idle", "shoot"}, "silver-nut": {"idle", "nothing"}, **{name: {"idle"} for name in ("silver-garlic", "silver-ice-shroom", "silver-sunflower")}, "sun-shroom": {"idle", "grow", "idle2"}, "gnome-shroom": {"idle"}, "umbrella-cap": {"idle", "block"}, "hail-shroom": {"idle"}, **{name: {"idle", "shoot"} for name in ("icicle-shroom", "twinkle-shroom", "nugget-shroom")}, "nut-shroom": {"idle"}, "bell-pepper": {"idle", "explode"}, "boom-shroom": {"idle"}, "penny-shroom": {"idle", "grow", "idle2"}, **{name: {"idle", "shoot"} for name in ("pea-shroom", "repeat-shroom", "split-pea-shroom")}, "gatling-pea-shroom": {"idle_g", "shoot_g"}, "football-helmet": {"idle"}, "football-nut": {"idlefootball"}, "sunny-shroom": {"idle", "scared", "scaredidle", "scaredgrow"}, "sturdy-shroom": {"idle", "scared", "scaredidle", "scaredgrow"}, "camo-shroom": {"idle"}, "starry-shroom": {"idle", "shoot", "scared", "scaredidle", "scaredgrow"}, **{name: {"idle", "shoot", "scared", "scaredidle", "scaredgrow"} for name in ("speedy-shroom", "blasty-shroom", "shivery-shroom", "spuddy-shroom", "frenzy-shroom")}, "cherry-spore": {"idle"}, "potato-spore": {"idle"}, "black-cherry": {"idle", "shake"}, "frozen-cherry": {"idle"}, "demise-shroom": {"idle", "explode"}, "sundae-shroom": {"idle"}, "gloom-shroom": {"idle", "attack"}, "frost-gloom-shroom": {"idle", "attack_ice"}, "flame-gloom-shroom": {"idle", "idle_fire", "attack_fire"}, "charred-threepeater": {"idle"}, "icerock": {"idle", "attack"}, "hearthrock": {"idle", "attack"}, "foul-shroom": {"idle", "shoot"}, "starch-shroom": {"idle", "shoot", "flash"}, "jawbreaker": {"idle", "lookleft", "lookright", "jump", "jumppop"}, "doom-chomper": {"idle", "bite", "chew", "swallow"}, "frostbiter": {"idle", "bite", "chew", "swallow"}, "frost-shroom": {"idle", "shoot"}, "soot-shroom": {"idle", "shoot", "rest"}, "fume-spreader": {"idle", "shoot"}, "spicy-kelp": {"idle", "grab"}, "doom-kelp": {"idle", "grab"}, "butter-nut": {"idle", "nothing"}, "umbrella-husk": {"idle", "block"}, "torchflower": {"idle"}, "land-mine": {"idle"}, "foul-nut": {"idle", "nothing"}, "star-nut": {"idle", "nothing"}, "thorn-nut": {"idle", "rise", "idle2", "unrise"},
                          "ice-mine": {"idle", "Unearthed", "rise"},
                          "hypno-mine": {"idle", "rise", "armed", "mashed", "flash"},
                          "chip-mine": {"idle"}, "solarweed": {"idle", "attack"}, "iceweed": {"idle", "attack"},
                          "scorchweed": {"idle", "attack"}, "spike-nut": {"idle", "nothing"},
                          "flame-wall-nut": {"idle", "fire"}, "frost-wall-nut": {"idle"},
                          "flame-tall-nut": {"idle_fire", "fire"}, "frost-tall-nut": {"iceidle"}, "twin-marigold": {"idle"}, "umbrella-shell": {"idle", "block"}, "chomp-mine": {"idle", "bite", "chew", "swallow"},
                          "solar-chomper": {"idle", "bite", "chew", "swallow"},
                          "solar-nut": {"idle", "nothing"},
                          "twin-solar-nut": {"idle", "nothing"},
                          "solar-nut-ball": {"Round", "idle_big"},
                          "threetato-mine": {"idle", "idle2", "rise", "down", "shoot", "shooting"},
                          "marigold-mine": {"idle", "rise", "armed", "unrise", "flash"},
                          "peatato-mine": {"idle", "rise", "armed", "mashed", "shoot"},
                          "solar-mine": {"idle", "rise", "armed", "mashed", "flash"},
                          "mine-nut": {"armed", "flash"}, "bucket": {"idle"}, "buck-nut": {"idle", "nothing"}, "cherry-nut": {"idle", "nothing"},
                          "solar-bomb": {"Bomb", "idle", "shake"},
                          "cluster-mine": {"idle"},
                          "cherry-chomper": {"idle", "bite", "chew", "swallow"},
                          "chomp-nut": {"idle", "bite", "chew", "swallow"},
                          "chompzilla": {"idle", "bite"},
                          "buck-shroom": {"idle", "shoot"},
                          "sprout-pult": {"idle", "shoot"},
                          "baby-corn": {"idle", "shoot", "shoot2"},
                          "slice-pult": {"idle", "shoot"},
                          "solar-pot": {"idle"},
                          "sunset-shroom": {"idle", "idle1", "idle2", "grow1", "grow2"},
                          "hypno-pepper": {"idle", "explode", "shake"},
                          "hypno-melon": {"idle", "shoot"},
                          "flame-cattail": {"idle", "shoot", "fire"},
                          "frost-cattail": {"idle", "shoot"},
                          "solar-flare": {"idle", "explode", "shake"},
                          "solar-blover": {"idle", "blow", "blowloop"},
                          "silver-doom-shroom": {"idle", "explode"},
                          "silver-hypno-shroom": {"idle"},
                          "silver-cabbage": {"idle", "shoot", "fire"},
                          "silver-melon": {"idle", "shoot"},
                          "cherry-pot": {"idle"}, "hot-pot": {"idle", "fire"}, "hypno-pot": {"idle"}, "doom-pot": {"idle"},
                          "nut-pot": {"idle"}, "garlic-pot": {"idle"}, "melon-pot": {"idle"}, "spike-pot": {"idle", "attack"},
                          "cabbage-pot": {"idle"}, "butter-pot": {"idle"}, "umbrella-pot": {"idle"},
                          "silver-pot": {"idle"}, "lumos-pot": {"idle"},
                          "silver-umbrella": {"idle", "block"},
                          "silver-kernel": {"idle", "shoot", "shoot2"}}[PLANT]
    if PLANT in ("strangle-kelp", "kelp-spreader", "squashweed", "butterweed", "springweed"):
        expected_clips = {"idle", "attack"}
    if PLANT == "splash":
        expected_clips = {"idle", "lookleft", "lookright", "jump", "jumppop"}
    if PLANT == "sea-nut":
        expected_clips = {"idle", "round"}
    if PLANT == "three-nuts":
        expected_clips = {"idle", "idle2", "idle3", "fall1", "fall2", "shoot"}
    if PLANT == "squash-nut":
        expected_clips = {"idle", "jump"}
    if PLANT.endswith("-squash"):
        expected_clips = {"idle", "lookleft", "lookright", "jump", "jumppop"}
    if PLANT == "solar-squash":
        expected_clips.add("sun")
    if PLANT in ("cherry-spreader", "stench-spreader", "snow-threepeater", "scorched-threepeater", "doom-spreader", "phoenix-threepeater"):
        expected_clips = {"idle", "head1", "shoot1", "head2", "shoot2", "head3", "shoot3"}
    if PLANT == "doom-spreader":
        expected_clips.add("back")
    if PLANT == "death-star":
        expected_clips = {"idle", "shoot", "shake"}
    if PLANT == "charred-threepeater":
        expected_clips = {"idle"}
    if PLANT == "chomp-shooter":
        expected_clips.add("bite")
    if PLANT == "flame-shroom":
        expected_clips.add("fire")
    if PLANT == "gatling-cherry":
        # The original controller also contains the rapid-fire upgrade state.
        # Preserve it in the export; normal four-shot fire uses `shoot`.
        expected_clips.add("shooting")
    if PLANT == "gutsy-shroom":
        expected_clips.add("shoot1")
    if PLANT in ("boost-shroom", "wallop-shroom"):
        expected_clips = {"idle", "shoot"}
    if PLANT == "blowtorch-shroom":
        expected_clips = {"idle", "shoot", "fire"}
    if PLANT == "caltrop-shroom":
        expected_clips = {"idle", "attack"}
    expected_clips = {"hypno-pepper": {"idle", "explode", "shake"}, "hypno-melon": {"idle", "shoot"}, "flame-cattail": {"idle", "shoot", "fire"}, "frost-cattail": {"idle", "shoot"}}.get(PLANT, expected_clips)
    if beam:
        expected_clips = {"idle", "shoot", "shooting"}
    expected_clips = {"strangle-kelp": {"idle", "searchzombie", "grab"}, "kelp-spreader": {"idle", "grab", "shoot"}, "squash-spreader": {"idle", "head1", "head2", "head3", "shoot1", "shoot2", "shoot3"}, "squashweed": {"idle", "attack"}}.get(PLANT, expected_clips)
    expected_clips = {"perfume-shroom": {"idle", "shoot"}, "tangle-weed": {"idle", "attack"}, "tangle-mine": {"idle", "rise", "armed", "mashed", "flash", "grab"},
        "kelp-mine": {"idle", "grab"}, "sea-mine": {"idle"},
        "potato-masher": {"idle", "rise", "idle2", "flash"},
        "potato-mash": {"idle", "lookleft", "lookright", "jump", "jumppop"}}.get(PLANT, expected_clips)
    if PLANT == "triplantern":
        expected_clips = {"idle", "light"}
    if PLANT == "aurora-plantern":
        expected_clips = {"idle"}
    if PLANT == "multipeater":
        assert len(layers) == 3
        assert layers[0]["data"]["(int&)m_LayerBlendingMode"] == 0
        assert all(layer["data"]["(int&)m_LayerBlendingMode"] == 1
                   and layer["data"]["m_DefaultWeight"] == 1 for layer in layers[1:])
        expected_clips = {"idle", "idle2", "idle3", "shoot1", "shoot2", "shoot3", "fly"}
    assert set(clips) == expected_clips, list(clips)
    if PLANT == "gatling-pea-shroom":
        clips["idle"] = clips.pop("idle_g")
        clips["shoot"] = clips.pop("shoot_g")
    if PLANT == "flame-tall-nut":
        clips["idle"] = clips["idle_fire"]
    if PLANT == "football-nut":
        clips["idle"] = clips["idlefootball"]
    if PLANT == "frost-tall-nut":
        clips["idle"] = clips["iceidle"]
    if PLANT == "solar-nut-ball":
        clips["idle"] = clips["Round"]
    if PLANT == "mine-nut":
        clips["idle"] = clips["armed"]
    if PLANT == "ice-mine":
        # Source names the buried pose Unearthed, and the raised loop idle.
        clips["armed"] = clips["idle"]
        clips["idle"] = clips["Unearthed"]
    if PLANT == "ice-mine":
        # IceMine.OnFlash switches these serialized sprites for 200 ms.
        # The IL2CPP script has no managed typetree, so verify its exact tail.
        raw = objects[195419].get_raw_data()
        refs = [struct.unpack_from("<iq", raw, offset) for offset in (640, 652, 664)]
        assert len(raw) == 676 and refs == [(0, 139533), (0, 20092), (0, 11880)]
        assert read(read(139533)["m_GameObject"]["m_PathID"])["m_Name"] == "PotatoMine_head"
        for path_id in (20092, 11880):
            sprite(path_id)
        head = next(i for i, node in enumerate(nodes) if node["Name"] == "PotatoMine_head")
        clips["flash"] = dict(Duration=.2, Bindings=[dict(Node=head, Attribute=5,
            Channels=[[dict(Time=0, Coefficients=[0, 0, 0, 11880])]])])
        evidence.append(dict(path_id=195419, type="MonoBehaviour", source="serialized tail",
            flash_renderer=139533, original_sprite=20092, flash_sprite=11880,
            runtime_clip="flash", duration=.2))
    # Unity's additive Shoot layer uses the first clip pose as reference.
    # Position deltas and scale ratios must preserve the underlying idle pose.
    additive_clip = "idle_fire" if PLANT == "flame-gloom-shroom" else "fire" if PLANT in ("coldslaw", "kimchi-pult", "flame-wall-nut", "flame-tall-nut", "hot-pot", "cabbage-tri-pult") else "shake" if PLANT in ("black-cherry", "solar-bomb", "hypno-pepper", "death-star") else "sun" if PLANT == "solar-squash" else "grab" if PLANT in ("spicy-kelp", "doom-kelp") else "shoot"
    additive_clips = ["head1", "head2", "head3", "shoot1", "shoot2", "shoot3"] if PLANT in (
        "cherry-spreader", "stench-spreader", "snow-threepeater", "scorched-threepeater", "doom-spreader") else ["shoot1", "shoot2", "shoot3", "fly"] if PLANT == "multipeater" else ["shoot", "shoot1"] if PLANT == "gutsy-shroom" else ["fire"] if PLANT == "flame-cattail" else [additive_clip]
    if PLANT == "doom-spreader":
        additive_clips.append("back")
    for binding in (binding for name in additive_clips for binding in clips.get(name, {}).get("Bindings", [])):
        if binding["Attribute"] == 3:
            assert all(channel[0]["Coefficients"][3] != 0 for channel in binding["Channels"])
    ground = -1 if projectile or PLANT == "sparkwood" else next(i for i, node in enumerate(nodes) if node["Path"] == "Shadow")
    if PLANT in ("roasted-pumpkin", "chompkin", "pumpkin-mine", "sunny-pumpkin", "melon-shell", "cryo-pumpkin", "pyro-pumpkin", "pumpkin-pod", "windy-pumpkin", "jack-o-plantern", "sea-shell", "cherry-pumpkin", "stinky-pumpkin", "scareless-pumpkin", "nut-shell", "starry-pumpkin", "thorny-pumpkin", "fuming-pumpkin"):
        assert nodes[ground]["Sprite"] == -1
        ground = next(i for i, node in enumerate(nodes) if node["Name"] == "shadowimage")
    if PLANT in ("starthorn", "thornweed"):
        # These source prefabs have an empty Shadow; runtime fits all artwork.
        assert nodes[ground]["Sprite"] == -1
        ground = -1
    if PLANT == "spiky-pumpkin":
        # Original underground shell has no visible shadow.
        ground = -1
    if PLANT == "puff-kin":
        # Three independent original shadows; fit the complete mushroom ring.
        ground = -1
    if PLANT in ("sunset-shroom", "tide-shroom"):
        # This aquatic prefab has an empty Shadow. Its original surface foam
        # supplies the visible ground reference, as with the existing kelp.
        assert nodes[ground]["Sprite"] == -1
        ground = next(i for i, node in enumerate(nodes) if node["Path"] == "SeaShroom_body/SeaShroom_foam")
        assert nodes[ground]["Sprite"] >= 0
    if PLANT in ("solarfruit", "starlic", "winter-starfruit", "spicy-starfruit", "starmorph", "iron-star", "death-star"):
        # SunStarPrefab deliberately has an empty Shadow. The original body
        # supplies its visible footprint; do not invent a replacement shadow.
        assert nodes[ground]["Sprite"] == -1
        ground = next(i for i, node in enumerate(nodes) if node["Path"] == ("Starfruit_body/Starfruit_body_1" if PLANT == "starlic" else "Starfruit_body"))
        assert nodes[ground]["Sprite"] >= 0
    if PLANT in ("spicy-kelp", "doom-kelp", "tingle-kelp", "grapple-kelp", "tangle-nut", "spike-kelp", "kelp-mine"):
        # Aquatic kelp has an empty Shadow; its original surface ripple is the footprint.
        assert nodes[ground]["Sprite"] == -1
        footprint = {"tingle-kelp": "Tanglekelp_whitewater1", "grapple-kelp": "body1",
                     "tangle-nut": "Wallnut_body/Tanglekelp_whitewater1", "spike-kelp": "Tanglekelp_whitewater1"}.get(PLANT, "Tanglekelp_whitewater")
        ground = next(i for i, node in enumerate(nodes) if node["Path"] == footprint)
        assert nodes[ground]["Sprite"] >= 0
    if PLANT in ("solarweed", "iceweed", "scorchweed", "tangle-weed", "icerock", "hearthrock"):
        # These three original prefabs deliberately have an empty Shadow.
        # Their visible ground-cover body supplies the footprint instead.
        assert nodes[ground]["Sprite"] == -1
        footprint = "SpikeRock_body" if PLANT in ("icerock", "hearthrock") else "Caltrop_body/Caltrop_body2" if PLANT == "solarweed" else "Caltrop_body"
        ground = next(i for i, node in enumerate(nodes) if node["Path"] == footprint)
        assert nodes[ground]["Sprite"] >= 0
    if PLANT in ("beacon-shroom", "jelly-shroom", "swell-shroom", "hermit-shroom", "minnow-shroom", "urchin-shroom", "shark-shroom", "sea-star", "caltrop-shroom", "frost-cattail", "flame-cattail"):
        # The source Shadow and body root have no sprite. Fit its complete
        # original artwork; ImportedPeaVisuals uses the same bounds contract.
        assert nodes[ground]["Sprite"] == -1
        ground = -1
    if PLANT in ("strangle-kelp", "kelp-spreader", "squashweed", "butterweed", "springweed"):
        assert nodes[ground]["Sprite"] == -1
        ground = -1
    if PLANT == "splash":
        assert nodes[ground]["Sprite"] == -1
        ground = next(i for i, node in enumerate(nodes) if node["Path"] == "Squash_body/SeaShroom_foam")
    if PLANT == "sea-mine":
        assert nodes[ground]["Sprite"] == -1
        ground = next(i for i, node in enumerate(nodes) if node["Path"] == "Body/SeaShroom_foam")
        assert nodes[ground]["Sprite"] >= 0
    if PLANT == "sea-nut":
        assert nodes[ground]["Sprite"] == -1
        ground = next(i for i, node in enumerate(nodes) if node["Path"] == "SeaNut")
    artifact = dict(Nodes=nodes, Sprites={str(k): v for k, v in sprites.items()}, Clips=clips,
                    ShootAdditive=PLANT in ("little-lettuce-projectile", "salad-pult", "peatato-mine", "threetato-mine") or (not projectile and not passive), GroundNode=ground,
                    DamageSprites=damage_sprites,
                    IdleSpeed=controller["m_Controller"]["m_StateMachineArray"][0]["data"]
                    ["m_StateConstantArray"][0]["data"]["m_Speed"] if controller else 1)
    if skins:
        artifact["Skins"] = skins
    (OUTPUT / "model.json").write_text(json.dumps(artifact, separators=(",", ":"), allow_nan=False) + "\n")
    # This directory owns generated sprite files. Remove stale parts only
    # after a complete, validated export, so corrected prefabs cannot retain
    # unrelated sprites from an older selection in the embedded resource set.
    exported_files = {"model.json"} | {data["File"] for data in sprites.values()}
    for old in OUTPUT.glob("sprite-*.png"):
        if old.name not in exported_files:
            old.unlink()
    files = {f.name: sha256(f) for f in sorted(OUTPUT.iterdir())}
    for item in evidence:
        if item["type"] == "Sprite" and files[item["file"]] != item["sha256"]:
            raise ValueError(f"Exported sprite checksum mismatch: {item['file']}")
    manifest = dict(bundle=str(bundle.relative_to(PROJECT.parent)),
                    bundle_sha256=bundle_sha256,
                    prefab_path_id=PREFAB, animator_controller_path_id=CONTROLLER,
                    ignored_bindings=ignored_bindings, shoot_layer=shoot_layer,
                    animator_layers=layers,
                    additive_reference=f"{', '.join(additive_clips)} sample at 0 seconds" if shoot_layer or PLANT in ("black-cherry", "solar-bomb") else None,
                    static_prefab=CONTROLLER is None,
                    ground_node=None if ground < 0 else nodes[ground],
                    serialized_file="resources.assets", objects=evidence,
                    files=files)
    (REFERENCE / "visual-assets.json").write_text(json.dumps(manifest, indent=2, ensure_ascii=False) + "\n")
    action = "Prepared" if output_root is not None else "Exported"
    print(f"{action} {plant}: {len(nodes)} original nodes, {len(sprites)} sprites and {len(clips)} animation clips.")


def publish_exports(staging, plants):
    """Move verified files on the same filesystem; publish success markers last."""
    generated = {}
    for plant in plants:
        for kind in ("assets", "references"):
            target = PROJECT / kind / plant
            if ((PROJECT / kind).is_symlink() or target.is_symlink()
                    or not target.resolve().is_relative_to(PROJECT.resolve())):
                raise ValueError(f"Export destination must stay inside project: {target}")
        source = staging / "assets" / plant
        generated[plant] = {path.name for path in source.iterdir()}
        output = PROJECT / "assets" / plant
        if output.exists():
            unexpected = {path.name for path in output.iterdir()
                          if path.is_symlink() or not path.is_file()
                          or (path.name not in generated[plant] and not path.match("sprite-*.png"))}
            if unexpected:
                raise ValueError(f"Unexpected files in generated model {plant}: {sorted(unexpected)}")
    # All decoding and source verification have succeeded. A publication error
    # must not leave an older success marker on partially replaced assets.
    for plant in plants:
        (PROJECT / "references" / plant / "visual-assets.json").unlink(missing_ok=True)
    for plant in plants:
        output = PROJECT / "assets" / plant
        output.mkdir(parents=True, exist_ok=True)
        source = staging / "assets" / plant
        for name in sorted(generated[plant]):
            path = source / name
            destination = output / path.name
            if not destination.exists() or path.read_bytes() != destination.read_bytes():
                os.replace(path, destination)
        for old in output.glob("sprite-*.png"):
            if old.name not in generated[plant]:
                old.unlink()
    for plant in plants:
        reference = PROJECT / "references" / plant
        reference.mkdir(parents=True, exist_ok=True)
        os.replace(staging / "references" / plant / "visual-assets.json",
                   reference / "visual-assets.json")


def export_batch(plants):
    started = perf_counter()
    fingerprints = source_fingerprints()
    bundle = SOURCE / "PlantsVsZombiesRH_Data/data.unity3d"
    environment = UnityPy.load(str(bundle))
    # Export needs only the selected prefabs, not a complete name index of
    # every source GameObject, controller and AnimationClip.
    objects = {o.path_id: o for o in environment.objects if o.assets_file.name == "resources.assets"}
    assets = SourceAssets(objects)
    for plant in plants:
        assets.prefab(PLANTS[plant])
    loaded = perf_counter()
    # Stage on the project's filesystem: final publication renames files
    # instead of writing every PNG and model a second time.
    with tempfile.TemporaryDirectory(prefix=".export-", dir=PROJECT) as temporary:
        staging = Path(temporary)
        for plant in plants:
            export(plant, assets, fingerprints[str(bundle)], output_root=staging)
        if any(sha256(path) != digest for path, digest in fingerprints.items()):
            raise ValueError("Source changed during export; no assets published")
        prepared = perf_counter()
        publish_exports(staging, plants)
    print(f"Source load/validation: {loaded-started:.2f}s; decode/stage/verify: "
          f"{prepared-loaded:.2f}s; publish: {perf_counter()-prepared:.2f}s")


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--plant", choices=PLANTS, action="append", help="Repeat for a batch export")
    parser.add_argument("--all", action="store_true", help="Export all registered models")
    args = parser.parse_args(argv)
    if args.all and args.plant:
        parser.error("Choose --all or --plant")
    plants = list(dict.fromkeys(PLANTS if args.all else args.plant or ["snow-repeater"]))
    lock_path = PROJECT / ".cache/export.lock"
    lock_path.parent.mkdir(parents=True, exist_ok=True)
    with lock_path.open("a") as lock:
        fcntl.flock(lock, fcntl.LOCK_EX | fcntl.LOCK_NB)
        export_batch(plants)


if __name__ == "__main__":
    main()
