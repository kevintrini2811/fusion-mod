"""Regress portrait helmet sockets against the same source fit as EyesView."""
import json
import unittest
from PIL import ImageChops
from export_eyes_armor import OUTPUT, armored_portrait
from preview_snow_repeater import Preview, multiply


class EyesArmorTests(unittest.TestCase):
    def test_portraits_match_runtime_socket(self):
        fits = json.loads((OUTPUT / 'placement.json').read_text())
        preview = Preview('eyes-zombie', 'walk')
        head = next(i for i, n in enumerate(preview.model['Nodes']) if n['Name'] == 'anim_head1-sprite')
        sprite = preview.model['Sprites'][str(preview.model['Nodes'][head]['Sprite'])]
        image = preview.sprite_image(sprite['File'])
        width, height = [n / sprite['PixelsPerUnit'] for n in image.size]
        for kind in ('cone', 'bucket'):
            portrait, proof = armored_portrait(kind, fits[kind])
            self.assertEqual(proof['parent'], head)
            self.assertEqual(proof['head'], preview.pose(0).transforms[head])
            expected_x = (0.5 - sprite['Pivot'][0]) * width + width * fits[kind][1]
            expected_y = -sprite['Pivot'][1] * height + height * fits[kind][2]
            self.assertAlmostEqual(proof['position'][0], expected_x)
            self.assertAlmostEqual(proof['position'][1], expected_y)
            expected = multiply(proof['head'], [proof['scale'], 0, expected_x, 0, proof['scale'], expected_y])
            for actual, target in zip(proof['transform'], expected):
                self.assertAlmostEqual(actual, target)
            from PIL import Image
            with Image.open(OUTPUT / ('eyes-' + kind + '.png')) as stored:
                self.assertEqual(stored.size, portrait.size)
                self.assertIsNone(ImageChops.difference(stored, portrait).getbbox())
            # Moving the shared fit moves the actual helmet socket, ruling out
            # the former fixed portrait-pixel placement.
            moved = list(fits[kind]); moved[1] += 0.25
            _, moved_proof = armored_portrait(kind, moved)
            self.assertAlmostEqual(moved_proof['position'][0] - proof['position'][0], width * 0.25)


if __name__ == '__main__':
    unittest.main()
