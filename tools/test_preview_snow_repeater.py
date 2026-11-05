"""Synthetic artwork regression checks. Run explicitly; never start game code."""
import argparse
import contextlib
import io
import json
from pathlib import Path
import tempfile
import unittest
from unittest.mock import patch

from PIL import Image

import preview_snow_repeater as preview


class CalibrationTests(unittest.TestCase):
    def setUp(self):
        temporary = tempfile.TemporaryDirectory()
        self.addCleanup(temporary.cleanup)
        self.project = Path(temporary.name)
        assets = self.project/"assets/fixture"
        assets.mkdir(parents=True)
        Image.new("RGBA", (10, 4), "red").save(assets/"sprite.png")
        self.model = dict(
            GroundNode=1, ShootAdditive=False, IdleSpeed=1,
            Nodes=[self.node("root", -1, -1, [50, 30, 0], [2, 2, 1]),
                   self.node("Shadow", 0, 1, [3, -1, 0], [1, 1, 1])],
            Sprites={"1": dict(File="sprite.png", Pivot=[.25, .5], PixelsPerUnit=10)},
            Clips={name: dict(Duration=1, Bindings=[]) for name in ("idle", "shoot")})
        for name in ("ImportedPeaVisuals", "ImportedProjectileVisuals", "PuffArtworkSizing"):
            source = self.project/f"src/{name}.cs"
            source.parent.mkdir(exist_ok=True)
            source.write_text("// synthetic adapter source\n")
        (self.project/"tools").mkdir()
        (self.project/"tools/preview_snow_repeater.py").write_text("# synthetic preview source\n")
        (self.project/"tools/plant-registrations.json").write_text(json.dumps(dict(
            plants=[dict(name="Fixture", native="Puffshroom")])))

    @staticmethod
    def node(name, parent, sprite, position, scale):
        return dict(Name=name, Path=name, Parent=parent, Sprite=sprite, Position=position,
                    Scale=scale, Rotation=[0, 0, 0, 1], Active=True, Enabled=True,
                    Order=0, FlipX=False, FlipY=False, Color=[1, 1, 1, 1])

    def asset(self):
        (self.project/"assets/fixture/model.json").write_text(json.dumps(self.model))
        return preview.Preview("fixture", project=self.project)

    def assertBounds(self, actual, expected):
        for a, b in zip(actual, expected):
            self.assertAlmostEqual(a, b)

    def test_sorting_order_curve_resets_on_next_sample(self):
        self.model["Clips"]["shoot"]["Bindings"] = [dict(Node=1,Attribute=12,
            Channels=[[dict(Time=0,Coefficients=[0,0,0,-3])]])]
        asset = self.asset()
        self.assertEqual(asset.pose(0, .5).order[1], -3)
        self.assertEqual(asset.pose(0).order[1], 0)

    def test_ground_alignment_includes_hierarchy_pivot_and_scene_position(self):
        asset = self.asset()
        pose = asset.pose(0)
        self.assertBounds(asset.bounds(pose), [55.5, 27.6, 57.5, 28.4])
        alignment = preview.plant_alignment(asset, "ground", 80)
        _, metrics = preview.calibration_frame(asset, pose, alignment, "fixture")
        self.assertBounds(metrics["visible_bounds"], [-40, 0, 40, 32])
        self.assertEqual(alignment["scale"], 40)

    def test_flip_around_off_centre_pivot_matches_drawn_pixels(self):
        self.model["Nodes"][0]["Position"] = [0, 0, 0]
        self.model["Nodes"][0]["Scale"] = [1, 1, 1]
        self.model["Nodes"][1]["Position"] = [0, 0, 0]
        self.model["Nodes"][1]["FlipX"] = True
        self.model["Nodes"][1]["FlipY"] = True
        self.model["Sprites"]["1"]["Pivot"] = [.25, .25]
        asset = self.asset()
        pose = asset.pose(0)
        self.assertBounds(asset.bounds(pose), [-.75, -.3, .25, .1])
        frame = asset.draw(pose, [100, 0, 100, 0, -100, 100])
        self.assertEqual(frame.getpixel((30, 110))[:3], (255, 0, 0))
        self.assertEqual(frame.getpixel((150, 110))[:3], (40, 46, 55))

    def test_fixed_alignment_keeps_root_motion_visible(self):
        self.model["Clips"]["idle"]["Bindings"] = [dict(
            Node=0, Attribute=1, Channels=[
                [dict(Time=0, Coefficients=[0, 0, 10, 50])],
                [dict(Time=0, Coefficients=[0, 0, 0, 30])],
                [dict(Time=0, Coefficients=[0, 0, 0, 0])]])]
        asset = self.asset()
        alignment = preview.plant_alignment(asset, "ground", 80)
        _, metrics = preview.calibration_frame(asset, asset.pose(.5), alignment, "fixture")
        self.assertBounds(metrics["visible_bounds"], [160, 0, 240, 32])
        self.assertTrue(metrics["clipped"])
        self.assertEqual(asset.pose(.5, zero_root=True).transforms[0][2], 0)

    def test_artwork_reference_uses_active_disabled_parts_like_runtime(self):
        extra = self.node("body", 0, 1, [10, 0, 0], [1, 1, 1])
        extra["Enabled"] = False
        self.model["Nodes"].append(extra)
        asset = self.asset()
        ground = preview.plant_alignment(asset, "ground", 80)
        artwork = preview.plant_alignment(asset, "artwork", 50)
        self.assertGreater(artwork["reference_width"], ground["reference_width"])
        self.assertEqual(asset.bounds(asset.pose(0)), asset.bounds(asset.pose(0), [1]))

    def test_zero_scale_frame_does_not_crash_drawing(self):
        self.model["Nodes"][1]["Scale"] = [0, 0, 1]
        asset = self.asset()
        self.assertEqual(asset.draw(asset.pose(0), [1, 0, 0, 0, 1, 0]).size, (420, 360))
        with self.assertRaisesRegex(ValueError, "No usable"):
            preview.plant_alignment(asset, "ground", 80)

    def test_png_decoding_shared_between_measurement_and_frames(self):
        asset = self.asset()
        original = Image.open
        with patch.object(preview.Image, "open", wraps=original) as opened:
            asset.bounds(asset.pose(0))
            asset.draw(asset.pose(0), [1, 0, 100, 0, -1, 100])
            asset.draw(asset.pose(.5), [1, 0, 100, 0, -1, 100])
        self.assertEqual(opened.call_count, 1)

    def test_existing_source_preview_uses_shared_renderer(self):
        asset = self.asset()
        self.assertEqual(asset.render(0).size, (420, 360))
        self.assertEqual(asset.render(.5, .25).size, (420, 360))

    def test_sheet_and_report_contain_projectile_scale_and_provenance(self):
        asset = self.asset()
        args = argparse.Namespace(fit="ground", target_width=None, projectile="fixture",
                                  projectile_clip=None, projectile_reference_width=1.5,
                                  projectile_factor=.3, projectile_zero_root=True)
        output = self.project/"references/fixture"
        with contextlib.redirect_stdout(io.StringIO()):
            preview.calibration_preview(asset, output, args, self.project)
        report = json.loads((output/"calibration.json").read_text())
        self.assertEqual(len(report["plant"]["frames"]), 8)
        self.assertEqual(len(report["projectile"]["frames"]), 4)
        self.assertAlmostEqual(report["projectile"]["alignment"]["scale"], 16)
        self.assertAlmostEqual(report["projectile"]["alignment"]["render_scale"], 45.12)
        self.assertIn("assets/fixture/sprite.png", report["inputs_sha256"])
        self.assertFalse(report["game_started"])
        self.assertFalse(report["runtime_verified"])
        with Image.open(output/"calibration.png") as sheet:
            self.assertEqual(sheet.size, (1680, 1140))

    def test_cli_requires_calibration_widths_and_puff_artwork(self):
        self.asset()
        invalid = (["--plant", "fixture", "--calibrate"],
                   ["--plant", "fixture", "--calibrate", "--fit", "artwork"],
                   ["--plant", "fixture", "--projectile", "fixture"],
                   ["--plant", "fixture", "--calibrate", "--fit", "artwork",
                    "--target-width", "50", "--projectile", "fixture"])
        with patch.object(preview, "PROJECT", self.project), contextlib.redirect_stderr(io.StringIO()):
            for argv in invalid:
                with self.subTest(argv=argv), self.assertRaises(SystemExit):
                    preview.parse_args(argv)
            valid = preview.parse_args(["--plant", "fixture", "--calibrate", "--fit", "artwork",
                                        "--target-width", "50"])
            self.assertEqual(valid.target_width, 50)
        for text in ("nan", "inf", "0", "-1"):
            with self.subTest(text=text), self.assertRaises(argparse.ArgumentTypeError):
                preview.positive_number(text)


if __name__ == "__main__":
    unittest.main()
