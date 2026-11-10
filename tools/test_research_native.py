"""Synthetic native-analysis regressions. Run explicitly; never start the game."""
import argparse
import contextlib
import io
import json
from pathlib import Path
import tempfile
import types
import unittest
from unittest.mock import Mock, patch

import capstone

import research_native as native


BASE = 0x180000000


def decoder():
    result = capstone.Cs(capstone.CS_ARCH_X86, capstone.CS_MODE_64)
    result.detail = True
    return result


def method(owner, name, token, rva):
    return dict(type=owner, method=name, token=token, rva=rva)


class NativeFlowChecks(unittest.TestCase):
    def test_direct_call_tail_jump_and_conditional_exit_keep_instruction_addresses(self):
        # call +0x100; internal jne; external jmp; indirect call; indirect jmp.
        code = bytes.fromhex("e8fb000000 7500 e9f4000000 ffd0 ff20")
        decoded = list(decoder().disasm(code, BASE + 0x1000))
        direct, indirect = native.flow_edges(decoded, BASE, 0x1000, 0x1010)
        self.assertEqual([(e["instruction"], e["site"], e["target"]) for e in direct],
                         [("call", 0x1000, 0x1100), ("jmp", 0x1007, 0x1100)])
        self.assertEqual([e["instruction"] for e in indirect], ["call", "jmp"])
        code = bytes.fromhex("0f85fa000000")
        direct, _ = native.flow_edges(list(decoder().disasm(code, BASE + 0x1000)), BASE, 0x1000, 0x1010)
        self.assertEqual((direct[0]["instruction"], direct[0]["target"]), ("jne", 0x1100))

    def test_aliases_and_missing_bodies_survive_range_construction(self):
        records = [method("A", "First", "0x6000001", 0x1000),
                   method("B", "Alias", "0x6000002", 0x1000),
                   method("A", "Abstract", "0x6000003", None),
                   method("A", "Outside", "0x6000004", 0x9000)]
        pe = types.SimpleNamespace(OPTIONAL_HEADER=types.SimpleNamespace(ImageBase=BASE),
                                   sections=[types.SimpleNamespace(VirtualAddress=0x1000,
                                             Misc_VirtualSize=0x200, Characteristics=0x20000000)])
        ranges, missing = native.method_ranges(records, [BASE + 0x1000, BASE + 0x1100, BASE + 0x9000], pe)
        self.assertEqual(ranges, {0x1000: 0x1100})
        self.assertEqual([r["end"] for r in records[:2]], [0x1100, 0x1100])
        self.assertEqual([r["method"] for r in missing], ["Abstract", "Outside"])

    def test_fields_distinguish_reads_writes_addresses_and_wide_reset(self):
        field = dict(type="Plant", field="mStateCountdown", offset=0xA0, token="0x4000001")
        code = bytes.fromhex("8b83a0000000 8983a0000000 488d93a0000000 "
                             "488d939c000000 4889bb9c000000 ff83a0000000")
        hits = native.field_candidates(list(decoder().disasm(code, BASE + 0x1000)), BASE, [field])
        self.assertEqual([h["access"] for h in hits], ["read", "write", "address", "write", "read+write"])
        self.assertEqual(hits[3]["width"], 8)
        # LEA of a preceding field does not read the following field.
        self.assertNotIn("lea rdx, [rbx + 0x9c]", [h["instruction"] for h in hits])

    def test_selection_keeps_overloads_and_rejects_ambiguous_types(self):
        index = dict(types={"One.Plant": {}, "Two.Plant": {}}, methods=[
            method("One.Plant", "Fire", "0x6000001", 0x1000),
            method("One.Plant", "Fire", "0x6000002", 0x1010)])
        self.assertEqual(native.select_methods(["One.Plant.Fire"], index), {"0x6000001", "0x6000002"})
        self.assertEqual(native.select_methods(["0x06000001"], index), {"0x6000001"})
        with self.assertRaises(ValueError):
            native.select_methods(["Plant.Fire"], index)
        with self.assertRaises(ValueError):
            native.select_methods(["One.Plant.Missing"], index)

    def test_report_preserves_tail_caller_aliases_and_indirect_limits(self):
        records = [method("Plant", "Eat", "0x6000001", 0x1000),
                   method("Other", "Alias", "0x6000002", 0x1000),
                   method("Zombie", "Caller", "0x6000003", 0x1100)]
        edge = dict(site=0x1100, instruction="jmp", operand=hex(BASE + 0x1000), target=0x1000)
        index = dict(methods=records, types={"Plant": {"parent": None}}, missing=[], graph={
            "4096": dict(end=0x1002, direct=[], indirect=[dict(site=0x1000, instruction="call", operand="rax")],
                         decoded_end=0x1002),
            "4352": dict(end=0x1105, direct=[edge], indirect=[], decoded_end=0x1105)})
        pe = types.SimpleNamespace(OPTIONAL_HEADER=types.SimpleNamespace(ImageBase=BASE),
                                   get_data=lambda *_: bytes.fromhex("ffd0"))
        files, evidence, markdown = native.report(index, {"0x6000001"}, [], pe, decoder())
        self.assertIn("replanted-0x6000001.asm", files)
        self.assertEqual(evidence["methods"][0]["callers"][0]["caller_symbol"], "Zombie.Caller [0x6000003]")
        self.assertIn("Other.Alias", markdown)
        self.assertIn("target unresolved", markdown)
        self.assertIn("candidate offsets without proof of the object type", markdown)
        index["methods"] += [method("Alias", f"Shared{i}", hex(0x6000100 + i), 0x1000) for i in range(30)]
        _, evidence, markdown = native.report(index, {"0x6000001"}, [], pe, decoder())
        self.assertEqual(len(evidence["aliases"][0x1000]), 32)
        self.assertIn("29 more aliases", markdown)
        self.assertNotIn("Alias.Shared29", markdown)


class NativeArchiveChecks(unittest.TestCase):
    def setUp(self):
        temporary = tempfile.TemporaryDirectory()
        self.addCleanup(temporary.cleanup)
        self.project = Path(temporary.name)
        self.patch = patch.object(native, "PROJECT", self.project)
        self.patch.start()
        self.addCleanup(self.patch.stop)

    def test_cache_hit_and_changed_inputs(self):
        cached = dict(sha256={"input": "old"}, graph={}, missing=[],
                      dependencies=dict(capstone=capstone.__version__, pefile=native.pefile.__version__))
        native.atomic_json(self.project / ".cache/native-index.json", cached)
        with patch.object(native, "build_index") as build, contextlib.redirect_stdout(io.StringIO()):
            self.assertEqual(native.native_index(None, None, None, {"input": "old"}), cached)
            build.assert_not_called()
            build.return_value = dict(graph={}, missing=[])
            with patch.object(native, "fingerprints", return_value={"input": "new"}):
                updated = native.native_index(None, None, None, {"input": "new"})
            self.assertEqual(updated["sha256"], {"input": "new"})
            build.assert_called_once()

    def test_source_change_does_not_publish_cache(self):
        with patch.object(native, "build_index", return_value={}), \
                patch.object(native, "fingerprints", return_value={"input": "changed"}):
            with self.assertRaises(ValueError):
                native.native_index(None, None, None, {"input": "original"})
        self.assertFalse((self.project / ".cache/native-index.json").exists())

    def test_field_offsets_require_matching_metadata_token_and_name(self):
        folder = self.project / "Reloaded.Gameplay"
        folder.mkdir()
        path = folder / "Plant.cs"
        path.write_text('[Token(Token = "0x4000001")]\n[FieldOffset(Offset = "0xA0")]\npublic int countdown;\n')
        index = dict(types={"Reloaded.Gameplay.Plant": {"fields": {"0x4000001": "countdown"}}})
        fields, inputs = native.select_fields(["Plant.countdown"], index, self.project)
        self.assertEqual(fields[0]["offset"], 0xA0)
        self.assertEqual(inputs[str(path)], native.sha256(path))
        index["types"]["Reloaded.Gameplay.Plant"]["fields"]["0x4000001"] = "different"
        with self.assertRaises(ValueError):
            native.select_fields(["Plant.countdown"], index, self.project)

    def test_output_escape_and_empty_selection_are_rejected(self):
        for slug, methods in [("../escape", ["Plant.Fire"]), ("good", [])]:
            with self.subTest(slug=slug), self.assertRaises(ValueError):
                native.research(argparse.Namespace(slug=slug, method=methods, field=[]))
        (self.project / "references").mkdir()
        (self.project / "references/outside").symlink_to(self.project, target_is_directory=True)
        with self.assertRaises(ValueError):
            native.research(argparse.Namespace(slug="outside", method=["Plant.Fire"], field=[]))

    def test_same_slug_cannot_be_written_concurrently(self):
        args = argparse.Namespace(slug="locked", method=["Plant.Fire"], field=[])
        with patch.object(native, "archive", side_effect=lambda *_: native.research(args)):
            with self.assertRaises(BlockingIOError):
                native.research(args)

    def test_failed_export_removes_success_marker_and_preserves_port_notes(self):
        output = self.project / "references/example"
        output.mkdir(parents=True)
        marker = output / "native-inputs.json"
        marker.write_text("previous success")
        notes = output / "port-notes.md"
        notes.write_text("manual evidence")
        args = argparse.Namespace(game_dir=self.project, dummy_dir=self.project, refresh=False,
                                  method=["Plant.Fire"], field=[])
        pe = Mock()
        pe.__enter__ = Mock(return_value=pe)
        pe.__exit__ = Mock(return_value=False)
        pe.FILE_HEADER.Machine = 0x8664
        with patch.object(native, "fingerprints", return_value={}), \
                patch.object(native.pefile, "PE", return_value=pe), \
                patch.object(native, "native_index", return_value={}), \
                patch.object(native, "select_methods", return_value=set()), \
                patch.object(native, "select_fields", return_value=([], {})), \
                patch.object(native, "report", return_value=({"body.asm": "body"}, {}, "report")), \
                patch.object(native.os, "replace", side_effect=OSError("disk failure")):
            # Staging JSON also uses os.replace. Fail only the final replacements.
            def stage_json(path, data):
                Path(path).write_text(json.dumps(data))
            with patch.object(native, "atomic_json", side_effect=stage_json), self.assertRaises(OSError):
                native.archive(args, output)
        self.assertFalse(marker.exists())
        self.assertEqual(notes.read_text(), "manual evidence")


if __name__ == "__main__":
    unittest.main()
