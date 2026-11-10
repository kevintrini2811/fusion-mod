"""Batch research regressions with synthetic sources; never load game code."""
import argparse
import contextlib
import io
import json
from pathlib import Path
import struct
import tempfile
import types
import unittest
from unittest.mock import MagicMock, Mock, patch

import research_fusion
from source_index import sha256


def options(**overrides):
    values = dict(batch=None, result="A", slug="first", selected_types=["::Plant"],
                  source_id=None, prefab=7)
    values.update(overrides)
    return argparse.Namespace(**values)


class BatchInputChecks(unittest.TestCase):
    def setUp(self):
        self.temporary = tempfile.TemporaryDirectory()
        self.addCleanup(self.temporary.cleanup)
        self.batch = Path(self.temporary.name) / "batch.json"
        self.parser = argparse.ArgumentParser()

    def read(self, jobs):
        self.batch.write_text(json.dumps(jobs))
        return research_fusion.read_jobs(
            self.parser, options(batch=self.batch, result=None, slug=None,
                                 selected_types=None, prefab=None))

    def test_single_result_options_remain_supported(self):
        job, = research_fusion.read_jobs(self.parser, options())
        self.assertEqual((job.result, job.slug, job.selected_types, job.prefab),
                         ("A", "first", ["::Plant"], 7))

    def test_batch_keeps_each_result_and_its_types_separate(self):
        jobs = self.read([dict(result="A", slug="first", types=["::Plant"], prefab=7),
                          dict(result="B", slug="second", types=["::Other"], source_id=0)])
        self.assertEqual([job.result for job in jobs], ["A", "B"])
        self.assertEqual(jobs[1].selected_types, ["::Other"])
        self.assertEqual(jobs[1].source_id, 0)
        self.assertIsNone(jobs[1].prefab)

    def test_duplicate_outputs_and_invalid_jobs_are_rejected(self):
        valid = dict(result="A", slug="first", types=["::Plant"])
        invalid_batches = [[], {}, [valid, valid], [dict(valid, slug="../outside")],
                           [dict(valid, types="::Plant")], [dict(valid, types=[])],
                           [dict(valid, type=["::Plant"])], [dict(valid, prefab=True)],
                           [dict(valid, prefab=0)], [dict(valid, source_id=-1)]]
        for jobs in invalid_batches:
            with self.subTest(jobs=jobs), self.assertRaises(ValueError):
                self.read(jobs)

    def test_batch_cannot_silently_ignore_single_result_options(self):
        with contextlib.redirect_stderr(io.StringIO()), self.assertRaises(SystemExit):
            research_fusion.read_jobs(self.parser, options(batch=self.batch))


class BatchResearchChecks(unittest.TestCase):
    def setUp(self):
        self.temporary = tempfile.TemporaryDirectory()
        self.addCleanup(self.temporary.cleanup)
        root = Path(self.temporary.name)
        self.project, self.source = root / "fusion-mod", root / "source"
        self.project.mkdir()
        self.source.mkdir()
        dll = self.source / "GameAssembly.dll"
        dll.write_bytes(b"synthetic source DLL")
        bundle = self.source / "PlantsVsZombiesRH_Data/data.unity3d"
        bundle.parent.mkdir(parents=True)
        bundle.write_bytes(b"synthetic bundle")
        metadata_path = self.source / "PlantsVsZombiesRH_Data/il2cpp_data/Metadata/global-metadata.dat"
        metadata_path.parent.mkdir(parents=True)
        metadata = bytearray(512)
        struct.pack_into("<II", metadata, 0, 0xFAB11BAF, 31)
        struct.pack_into("<I", metadata, 24, 256)
        struct.pack_into("<II", metadata, 48, 320, 36)
        struct.pack_into("<II", metadata, 160, 384, 88)
        names = b"\0Plant\0Shoot\0"
        metadata[256:256 + len(names)] = names
        struct.pack_into("<Ii", metadata, 320, 7, 0)
        struct.pack_into("<I", metadata, 320 + 24, 0x06000001)
        struct.pack_into("<IIii", metadata, 384, 1, 0, 0, -1)
        struct.pack_into("<i", metadata, 384 + 36, 0)
        struct.pack_into("<H", metadata, 384 + 64, 1)
        metadata_path.write_bytes(metadata)
        archive = dict(game_assembly_sha256=sha256(dll), recipes=[
            dict(plant1="Pea", plant1_id=0, plant2="Nut", plant2_id=1,
                 fusion=name, fusion_id=number) for number, name in ((100, "A"), (101, "B"))])
        (self.project / "fusionen-quellen.json").write_text(json.dumps(archive))
        almanac_path = self.source / "Mods/PvZ_Fusion_Translator/Dumps/LawnStrings.json"
        almanac_path.parent.mkdir(parents=True)
        almanac_path.write_text(json.dumps(dict(plants=[dict(seedType=100), dict(seedType=101)])))
        self.index = dict(sha256={str(path): sha256(path) for path in (dll, bundle, metadata_path)},
                          classes=[dict(index=0, name="Plant", namespace="")])
        prefab = types.SimpleNamespace(
            path_id=7, type=types.SimpleNamespace(name="GameObject"),
            assets_file=types.SimpleNamespace(name="resources.assets"),
            read_typetree=lambda: dict(m_Name="Plant", m_Component=[]))
        self.load = Mock(return_value=types.SimpleNamespace(objects=[prefab]))
        base = 0x180000000
        pointers = struct.pack("<10001Q", base + 0x1000, base + 0x1100, *([0] * 9999))
        pe = types.SimpleNamespace(
            OPTIONAL_HEADER=types.SimpleNamespace(ImageBase=base),
            __data__=struct.pack("<QQ", 10001, base + 0x331D670),
            get_data=lambda rva, size: pointers if rva == 0x331D670 else b"\xc3")
        self.pe_context = MagicMock()
        self.pe_context.__enter__.return_value = pe
        self.instructions = [types.SimpleNamespace(address=base + 0x1000, mnemonic="ret", op_str="")]
        self.decoder = Mock()
        self.decoder.disasm.return_value = self.instructions
        self.jobs = [options(), options(result="B", slug="second")]

    def run_research(self):
        def source_index(environment):
            environment()  # Simulate a cold index using the shared lazy loader.
            return self.index

        with patch.multiple(research_fusion, PROJECT=self.project, SOURCE=self.source), \
             patch.object(research_fusion, "source_index", side_effect=source_index), \
             patch.object(research_fusion.pefile, "PE", return_value=self.pe_context) as open_pe, \
             patch.object(research_fusion.capstone, "Cs", return_value=self.decoder), \
             patch.dict("sys.modules", UnityPy=types.SimpleNamespace(load=self.load)), \
             contextlib.redirect_stdout(io.StringIO()):
            research_fusion.research(self.jobs)
        return open_pe

    def test_batch_shares_heavy_inputs_and_preserves_separate_evidence(self):
        open_pe = self.run_research()
        self.load.assert_called_once()
        open_pe.assert_called_once()
        self.decoder.disasm.assert_called_once()
        artifacts = []
        for job in self.jobs:
            output = self.project / "references" / job.slug
            proof = json.loads((output / "research-inputs.json").read_text())
            self.assertEqual(proof["result"], job.result)
            self.assertEqual(proof["types"], job.selected_types)
            self.assertEqual(json.loads((output / "recipes.json").read_text())[0]["fusion"], job.result)
            self.assertEqual(json.loads((output / "prefab.json").read_text())["game_object"], 7)
            self.assertEqual(len(json.loads((output / "method-addresses.json").read_text())), 1)
            for relative, digest in proof["sha256"].items():
                self.assertEqual(sha256(self.project.parent / relative), digest)
            artifacts.append((output / "fusion-Plant-Shoot-06000001.asm").read_text())
        self.assertEqual(artifacts, ["00001000: ret \n"] * 2)

    def test_missing_chinese_entry_uses_only_checksum_pinned_english(self):
        almanac = self.source / "Mods/PvZ_Fusion_Translator/Dumps/LawnStrings.json"
        almanac.write_text(json.dumps(dict(plants=[dict(seedType=100)])))
        translation = self.project / "references/fusion-almanac/LawnStringsTranslate.en.json"
        translation.parent.mkdir(parents=True)
        translation.write_text(json.dumps(dict(plants=[dict(seedType=101, name="B")])) )
        archive_path = self.project / "fusionen-quellen.json"
        archive = json.loads(archive_path.read_text())
        archive["translation_names_sha256"] = sha256(translation)
        archive_path.write_text(json.dumps(archive))
        self.run_research()
        output = self.project / "references/second"
        self.assertEqual(json.loads((output / "almanac.json").read_text())["name"], "B")
        proof = json.loads((output / "research-inputs.json").read_text())
        self.assertIn(str(translation.relative_to(self.project.parent)), proof["sha256"])
        translation.write_text("changed translation")
        with self.assertRaisesRegex(ValueError, "English source differs"):
            self.run_research()

    def test_source_changes_remove_success_markers_and_preserve_notes(self):
        for job in self.jobs:
            output = self.project / "references" / job.slug
            output.mkdir(parents=True)
            (output / "research-inputs.json").write_text("old proof")
            (output / "port-notes.md").write_text("keep these notes")

        def mutate_source(*_):
            (self.source / "GameAssembly.dll").write_bytes(b"changed during research")
            return self.instructions

        self.decoder.disasm.side_effect = mutate_source
        with self.assertRaisesRegex(ValueError, "Source changed during research"):
            self.run_research()
        for job in self.jobs:
            output = self.project / "references" / job.slug
            self.assertFalse((output / "research-inputs.json").exists())
            self.assertEqual((output / "port-notes.md").read_text(), "keep these notes")

    def test_bad_second_job_does_not_write_first_job(self):
        self.jobs[1].selected_types = ["::Missing"]
        with self.assertRaisesRegex(ValueError, "missing types"):
            self.run_research()
        self.assertFalse((self.project / "references").exists())

    def test_symlink_cannot_redirect_output_outside_references(self):
        references = self.project / "references"
        references.mkdir()
        outside = self.project.parent / "outside"
        outside.mkdir()
        (references / "first").symlink_to(outside, target_is_directory=True)
        with self.assertRaisesRegex(ValueError, "Output escapes references"):
            self.run_research()
        self.assertEqual(list(outside.iterdir()), [])

    def test_two_slugs_cannot_resolve_to_the_same_output(self):
        references = self.project / "references"
        first = references / "first"
        first.mkdir(parents=True)
        (references / "second").symlink_to(first, target_is_directory=True)
        with self.assertRaisesRegex(ValueError, "Duplicate output directory"):
            self.run_research()
        self.assertEqual(list(first.iterdir()), [])


if __name__ == "__main__":
    unittest.main()
