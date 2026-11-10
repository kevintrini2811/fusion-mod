"""Offline regression checks for registration generation. Never start the game."""
import copy
import json
from pathlib import Path
import tempfile
import unittest
from unittest.mock import patch

import registrations


class RegistrationChecks(unittest.TestCase):
    def setUp(self):
        self.temporary = tempfile.TemporaryDirectory()
        self.addCleanup(self.temporary.cleanup)
        self.project = Path(self.temporary.name)
        (self.project / "tools").mkdir()
        (self.project / "src").mkdir()
        self.data = {
            "plants": [
                {"name": "Alpha", "native": "Peashooter", "marker": "Cherrybomb"},
                {"name": "Beta", "native": "Repeater", "marker": "Cherrybomb"},
                {"name": "GloomShroom", "native": "Gloomshroom", "marker": "None"},
            ],
            "reserved": [{"name": "StarryPumpkin", "native": "Pumpkinshell", "marker": "Starfruit"}],
            "families": {"IsShooter": ["Alpha", "Beta"], "IsMixed": ["IsShooter", "GloomShroom"]},
        }
        self.manifest = self.project / registrations.MANIFEST
        self.manifest.write_text(json.dumps(self.data))
        self.plant = self.project / registrations.PLANT
        self.plant.write_text("previous plant source\n")
        self.renderer = self.project / registrations.RENDERER
        self.renderer.write_text("handwritten prefix\n" + registrations.START + "\nold models\n"
                                 + registrations.END + "\nhandwritten suffix\n")

    def publish(self):
        for name, content in registrations.generated_sources(self.project).items():
            (self.project / name).write_bytes(content)

    def test_duplicate_identity_is_rejected_across_active_and_reserved(self):
        for section in ("plants", "reserved"):
            with self.subTest(section=section):
                data = copy.deepcopy(self.data)
                data[section].append({"name": "Other", "native": "Peashooter", "marker": "Cherrybomb"})
                with self.assertRaisesRegex(ValueError, "Duplicate save identity"):
                    registrations.validate(data)

    def test_unsafe_markers_and_native_types_are_rejected(self):
        for field, value in (("native", "None"), ("native", "Imitater"), ("marker", "Imitater")):
            with self.subTest(field=field, value=value):
                data = copy.deepcopy(self.data)
                data["plants"][0][field] = value
                with self.assertRaisesRegex(ValueError, "Unsafe save identity"):
                    registrations.validate(data)

    def test_duplicate_names_and_misspelled_fields_are_rejected(self):
        data = copy.deepcopy(self.data)
        data["reserved"][0]["name"] = "Alpha"
        with self.assertRaisesRegex(ValueError, "Duplicate or invalid plant name"):
            registrations.validate(data)
        data = copy.deepcopy(self.data)
        data["plants"][0]["family"] = "IsShooter"
        with self.assertRaisesRegex(ValueError, "Expected name, native and marker"):
            registrations.validate(data)

    def test_unknown_reserved_and_cyclic_family_members_are_rejected(self):
        for member in ("MissingPlant", "StarryPumpkin", "IsMixed"):
            with self.subTest(member=member):
                data = copy.deepcopy(self.data)
                data["families"]["IsShooter"].append(member)
                with self.assertRaisesRegex(ValueError, "family member|Cyclic family"):
                    registrations.validate(data)

    def test_duplicate_json_keys_cannot_hide_a_family(self):
        self.manifest.write_text('{"plants":[],"plants":[]}')
        with self.assertRaisesRegex(ValueError, "Duplicate JSON key"):
            registrations.generated_sources(self.project)

    def test_reserved_identity_does_not_gain_recognition_or_renderer(self):
        sources = registrations.generated_sources(self.project)
        plant = sources[registrations.PLANT].decode()
        renderer = sources[registrations.RENDERER].decode()
        self.assertIn("FusionPlant StarryPumpkin => new(SeedType.Pumpkinshell, SeedType.Starfruit)", plant)
        self.assertNotIn("=> StarryPumpkin,", plant)
        self.assertNotIn("plant == StarryPumpkin", plant)
        self.assertNotIn("StarryPumpkin", renderer)
        self.assertIn("FusionPlant GloomShroom => SeedType.Gloomshroom", plant)
        self.assertIn('new(FusionPlant.GloomShroom, "GloomShroom")', renderer)
        self.assertIn("_ => PlantLibrary.Plants.Find(plant) is { } registered", plant)
        self.assertIn(": new(plant.mSeedType)", plant)

    def test_patch_is_read_only_and_clean_after_generation(self):
        originals = {path: path.read_bytes() for path in (self.plant, self.renderer, self.manifest)}
        generated_patch = registrations.registration_patch(self.project)
        self.assertIn("--- a/src/FusionPlant.cs", generated_patch)
        self.assertIn("--- a/src/ImportedPeaVisuals.cs", generated_patch)
        self.assertEqual({path: path.read_bytes() for path in originals}, originals)
        self.publish()
        self.assertEqual(registrations.registration_patch(self.project), "")
        self.assertEqual(registrations.registration_errors(self.project), [])

    def test_generation_preserves_handwritten_renderer_and_line_endings(self):
        self.renderer.write_bytes(self.renderer.read_bytes().replace(b"\n", b"\r\n"))
        renderer = registrations.generated_sources(self.project)[registrations.RENDERER]
        self.assertTrue(renderer.startswith(b"handwritten prefix\r\n"))
        self.assertTrue(renderer.endswith(b"handwritten suffix\r\n"))
        self.assertNotIn(b"\n", renderer.replace(b"\r\n", b""))

    def test_missing_or_duplicate_renderer_boundary_rejects_entire_patch(self):
        for content in ("no boundaries\n", registrations.START + registrations.START + registrations.END):
            with self.subTest(content=content):
                self.renderer.write_text(content)
                before = self.plant.read_bytes()
                with self.assertRaisesRegex(ValueError, "exactly one"):
                    registrations.registration_patch(self.project)
                self.assertEqual(self.plant.read_bytes(), before)

    def test_stale_sources_and_manifest_are_reported(self):
        self.publish()
        self.plant.write_text(self.plant.read_text().replace("plant == Alpha", "plant == Beta", 1))
        self.assertIn(registrations.PLANT, registrations.registration_errors(self.project)[0])
        self.publish()
        self.renderer.write_text(self.renderer.read_text().replace('new(FusionPlant.Beta, "Beta"),', ""))
        self.assertIn(registrations.RENDERER, registrations.registration_errors(self.project)[0])
        self.publish()
        self.data["families"]["IsShooter"] = ["Alpha"]
        self.manifest.write_text(json.dumps(self.data))
        self.assertIn(registrations.PLANT, registrations.registration_errors(self.project)[0])

    def test_concurrent_input_change_prevents_patch_publication(self):
        generate = registrations.generated_sources

        def change_manifest(project):
            result = generate(project)
            self.manifest.write_text(self.manifest.read_text() + "\n")
            return result

        with patch.object(registrations, "generated_sources", side_effect=change_manifest):
            with self.assertRaisesRegex(ValueError, "Inputs changed"):
                registrations.registration_patch(self.project)


if __name__ == "__main__":
    unittest.main()
