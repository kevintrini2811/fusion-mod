"""Synthetic exporter regression checks; never load a game or source bundle."""
import contextlib
import io
import json
from pathlib import Path
import struct
import tempfile
import types
import unittest
from unittest.mock import MagicMock, Mock, patch
import zlib

from PIL import Image

import export_snow_repeater as exporter


def source_object(path_id, kind, tree):
    return types.SimpleNamespace(path_id=path_id,
        assets_file=types.SimpleNamespace(name="resources.assets"),
        type=types.SimpleNamespace(name=kind), read_typetree=Mock(return_value=tree))


def reference(path_id, file_id=0):
    return dict(m_FileID=file_id, m_PathID=path_id)


def animation_clip(name, frames=(), curve_count=0, bindings=(), constants=()):
    raw = bytearray()
    for time, keys in frames:
        raw.extend(struct.pack("<fi", time, len(keys)))
        for index, coefficients in keys:
            raw.extend(struct.pack("<i4f", index, *coefficients))
    words = list(struct.unpack(f"<{len(raw) // 4}I", raw))
    return dict(m_Name=name, m_SampleRate=60, m_Events=[],
                m_MuscleClip=dict(m_StopTime=1, m_Clip=dict(data=dict(
                    m_StreamedClip=dict(data=words, curveCount=curve_count),
                    m_DenseClip=dict(m_CurveCount=0),
                    m_ConstantClip=dict(data=list(constants))))),
                m_ClipBindingConstant=dict(genericBindings=list(bindings), pptrCurveMapping=[]))


class PrefabResolutionChecks(unittest.TestCase):
    def setUp(self):
        self.objects = {
            1: source_object(1, "GameObject", dict(m_Component=[
                dict(component=reference(3)), dict(component=reference(2)),
                dict(component=reference(5))])),
            2: source_object(2, "Transform", dict(m_GameObject=reference(1))),
            3: source_object(3, "Animator", dict(
                m_GameObject=reference(1), m_Controller=reference(4))),
            4: source_object(4, "AnimatorController", {}),
            5: source_object(5, "MonoBehaviour", {}),
        }
        self.assets = exporter.SourceAssets(self.objects)

    def test_component_order_does_not_choose_the_transform(self):
        self.assertEqual(self.assets.prefab(1), (2, 4))
        self.assertIs(self.assets.prefab(1), self.assets.prefab(1))
        for path_id in (1, 2, 3):
            self.objects[path_id].read_typetree.assert_called_once()
        for path_id in (4, 5):
            self.objects[path_id].read_typetree.assert_not_called()
        with self.assertRaises(TypeError):
            self.assets.prefab(1)[0] = 99

    def test_prefab_without_animator_is_static(self):
        self.objects[1].read_typetree.return_value["m_Component"].pop(0)
        self.assertEqual(self.assets.prefab(1), (2, None))

    def test_missing_or_wrong_root_object_fails(self):
        for path_id in (99, 2):
            with self.subTest(path_id=path_id), self.assertRaisesRegex(ValueError, "GameObject"):
                self.assets.prefab(path_id)

    def test_missing_duplicate_or_multiple_root_transforms_fail(self):
        for components in ([3], [2, 2, 3], [2, 3, 6]):
            with self.subTest(components=components):
                self.objects[6] = source_object(6, "Transform", dict(m_GameObject=reference(1)))
                self.objects[1].read_typetree.return_value["m_Component"] = [
                    dict(component=reference(part)) for part in components]
                with self.assertRaises(ValueError):
                    exporter.SourceAssets(self.objects).prefab(1)

    def test_multiple_animators_fail_instead_of_choosing_one(self):
        self.objects[6] = source_object(6, "Animator", dict(
            m_GameObject=reference(1), m_Controller=reference(4)))
        self.objects[1].read_typetree.return_value["m_Component"].append(dict(component=reference(6)))
        with self.assertRaisesRegex(ValueError, "at most one Animator"):
            self.assets.prefab(1)

    def test_transform_and_animator_must_belong_to_root(self):
        self.objects[6] = source_object(6, "GameObject", {})
        for path_id in (2, 3):
            tree = self.objects[path_id].read_typetree.return_value
            with self.subTest(path_id=path_id), patch.dict(tree, m_GameObject=reference(6)), \
                 self.assertRaisesRegex(ValueError, "another GameObject"):
                exporter.SourceAssets(self.objects).prefab(1)

    def test_external_null_and_missing_references_fail(self):
        pointers = [self.objects[1].read_typetree.return_value["m_Component"][0]["component"],
                    self.objects[2].read_typetree.return_value["m_GameObject"],
                    self.objects[3].read_typetree.return_value["m_GameObject"],
                    self.objects[3].read_typetree.return_value["m_Controller"]]
        for pointer in pointers:
            for changes in (dict(m_FileID=1), dict(m_PathID=0), dict(m_PathID=999)):
                with self.subTest(pointer=pointer, changes=changes), patch.dict(pointer, changes), \
                     self.assertRaises(ValueError):
                    exporter.SourceAssets(self.objects).prefab(1)

    def test_animator_controller_type_is_required(self):
        self.objects[4].type.name = "AnimatorOverrideController"
        with self.assertRaisesRegex(ValueError, "expected an AnimatorController"):
            self.assets.prefab(1)

    def test_new_context_resolves_same_prefab_id_again(self):
        self.assertEqual(self.assets.prefab(1), (2, 4))
        self.objects[6] = source_object(6, "AnimatorController", {})
        self.objects[3].read_typetree.return_value["m_Controller"] = reference(6)
        self.assertEqual(exporter.SourceAssets(self.objects).prefab(1), (2, 6))
        self.assertEqual(self.assets.prefab(1), (2, 4))


class SourceChannelChecks(unittest.TestCase):
    def setUp(self):
        self.clip = animation_clip("idle", curve_count=1, constants=(.5,), frames=[
            (float("-inf"), [(0, (1, 2, 3, 4))]),
            (.25, [(0, (4, 3, 2, 1))]),
            (.25, [(0, (5, 6, 7, 8))]),
            (1, [(0, (8, 7, 6, 5))]),
            (2, [(0, (float("nan"), 0, 0, 0))])])
        self.reader = Mock(return_value=self.clip)
        self.assets = exporter.SourceAssets({7: types.SimpleNamespace(read_typetree=self.reader)})

    def test_shared_decode_preserves_boundaries_order_and_constants(self):
        channels = self.assets.channels(7)
        self.assertIs(self.assets.channels(7), channels)
        self.reader.assert_called_once()
        self.assertEqual(channels, (
            ((-1, (1, 2, 3, 4)), (.25, (4, 3, 2, 1)),
             (.25, (5, 6, 7, 8)), (1, (8, 7, 6, 5))),
            ((0, (0, 0, 0, .5)),)))
        with self.assertRaises(TypeError):
            channels[0][0][1][3] = 99

    def test_dense_channels_are_still_rejected(self):
        self.clip["m_MuscleClip"]["m_Clip"]["data"]["m_DenseClip"]["m_CurveCount"] = 1
        with self.assertRaisesRegex(AssertionError, "Do not silently drop dense channels"):
            self.assets.channels(7)

    def test_nonfinite_coefficients_within_clip_are_still_rejected(self):
        self.reader.return_value = animation_clip("idle", curve_count=1,
            frames=[(1, [(0, (0, 0, 0, float("nan")))])])
        with self.assertRaises(AssertionError):
            self.assets.channels(7)

    def test_new_source_context_does_not_reuse_decoded_channels(self):
        first = self.assets.channels(7)
        self.reader.return_value = animation_clip("idle", constants=(9,))
        other = exporter.SourceAssets({7: types.SimpleNamespace(read_typetree=self.reader)})
        self.assertEqual(other.channels(7), (((0, (0, 0, 0, 9)),),))
        self.assertEqual(self.reader.call_count, 2)
        self.assertIs(self.assets.channels(7), first)

    def test_invalid_frame_sizes_and_channel_indices_are_rejected(self):
        cases = [struct.pack("<fi", 0, -1), struct.pack("<fi", 0, 1),
                 struct.pack("<fi", 0, 1) + struct.pack("<i4f", -1, 0, 0, 0, 1),
                 struct.pack("<fi", 0, 1) + struct.pack("<i4f", 1, 0, 0, 0, 1)]
        for raw in cases:
            with self.subTest(raw=raw):
                self.reader.return_value = animation_clip("idle", curve_count=1)
                self.reader.return_value["m_MuscleClip"]["m_Clip"]["data"]["m_StreamedClip"]["data"] = list(
                    struct.unpack(f"<{len(raw)//4}I", raw))
                with self.assertRaisesRegex(ValueError, "invalid streamed frame size|outside binding table"):
                    exporter.SourceAssets({7: types.SimpleNamespace(read_typetree=self.reader)}).channels(7)


class BlackCherryExportChecks(unittest.TestCase):
    def setUp(self):
        self.temporary = tempfile.TemporaryDirectory()
        self.addCleanup(self.temporary.cleanup)
        root = Path(self.temporary.name)
        self.project = root / "fusion-mod"
        self.source = root / "source"
        prefab = exporter.PLANTS["black-cherry"]
        transform, controller = 119536, 6234

        def layer(mode):
            return {"data": {"(int&)m_LayerBlendingMode": mode,
                             "m_DefaultWeight": 1,
                             "m_SkeletonMask": {"data": {"m_Data": []}}}}

        self.layers = [layer(0), layer(1)]

        def transform_data(game_object, children):
            return dict(m_GameObject=reference(game_object),
                        m_Children=[dict(m_PathID=child) for child in children],
                        m_LocalPosition=dict(x=0, y=0, z=0),
                        m_LocalRotation=dict(x=0, y=0, z=0, w=1),
                        m_LocalScale=dict(x=1, y=1, z=1))

        trees = {
            prefab: dict(m_Name="BlackCherry", m_IsActive=True, m_Component=[
                dict(component=reference(transform)), dict(component=reference(5))]),
            5: dict(m_GameObject=reference(prefab), m_Controller=reference(controller)),
            transform: transform_data(prefab, [1]),
            1: transform_data(2, []),
            2: dict(m_Name="Shadow", m_IsActive=True, m_Component=[]),
            controller: dict(m_Controller=dict(
                m_LayerArray=self.layers,
                m_StateMachineArray=[dict(data=dict(
                    m_StateConstantArray=[dict(data=dict(m_Speed=1))]))]),
                m_AnimationClips=[dict(m_PathID=3), dict(m_PathID=4)]),
            3: animation_clip("idle"),
            4: animation_clip("shake"),
        }
        kinds = {prefab: "GameObject", transform: "Transform", 1: "Transform", 2: "GameObject",
                 controller: "AnimatorController", 3: "AnimationClip", 4: "AnimationClip", 5: "Animator"}
        self.objects = {path_id: source_object(path_id, kinds[path_id], tree)
                        for path_id, tree in trees.items()}
        self.assets = exporter.SourceAssets(self.objects)

    def export(self):
        with patch.multiple(exporter, PROJECT=self.project, SOURCE=self.source), \
             patch.object(exporter.UnityPy, "load", side_effect=AssertionError("Bundle loading forbidden")), \
             contextlib.redirect_stdout(io.StringIO()):
            exporter.export("black-cherry", self.assets, "0" * 64)

    def test_two_layer_controller_exports_model_and_evidence(self):
        self.export()
        model_path = self.project / "assets/black-cherry/model.json"
        model = json.loads(model_path.read_text())
        proof = json.loads((self.project / "references/black-cherry/visual-assets.json").read_text())
        self.assertEqual(set(model["Clips"]), {"idle", "shake"})
        self.assertEqual(model["Nodes"][model["GroundNode"]]["Path"], "Shadow")
        self.assertEqual(proof["shoot_layer"], self.layers[1]["data"])
        self.assertEqual(proof["additive_reference"], "shake sample at 0 seconds")
        self.assertEqual(proof["files"]["model.json"], exporter.sha256(model_path))

    def test_invalid_animation_layers_still_fail_before_publishing_model(self):
        cases = [{"m_DefaultWeight": 0.5},
                 {"(int&)m_LayerBlendingMode": 0},
                 {"m_SkeletonMask": {"data": {"m_Data": [1]}}}]
        for changes in cases:
            with self.subTest(changes=changes), \
                 patch.dict(self.layers[1]["data"], changes), self.assertRaises(AssertionError):
                self.export()
        self.assert_no_published_model()

    def test_missing_second_layer_fails_validation(self):
        self.layers.pop()
        with self.assertRaises(AssertionError):
            self.export()
        self.assert_no_published_model()

    def test_repeated_export_reuses_source_trees_and_preserves_output(self):
        self.export()
        paths = (self.project / "assets/black-cherry/model.json",
                 self.project / "references/black-cherry/visual-assets.json")
        before = [path.read_bytes() for path in paths]
        self.export()
        self.assertEqual([path.read_bytes() for path in paths], before)
        for obj in self.objects.values():
            obj.read_typetree.assert_called_once()

    def assert_no_published_model(self):
        self.assertFalse((self.project / "assets/black-cherry/model.json").exists())
        self.assertFalse((self.project / "references/black-cherry/visual-assets.json").exists())


class ExportBatchChecks(unittest.TestCase):
    def setUp(self):
        self.temporary = tempfile.TemporaryDirectory()
        self.addCleanup(self.temporary.cleanup)
        self.project = Path(self.temporary.name)
        self.names = ("snow-repeater", "black-cherry", "frozen-cherry")
        self.plants = {name: exporter.PLANTS[name] for name in self.names}
        self.bundle = exporter.SOURCE / "PlantsVsZombiesRH_Data/data.unity3d"
        self.source_objects = {}
        for prefab in self.plants.values():
            transform, animator, controller = (prefab + offset for offset in (1000000, 2000000, 3000000))
            self.source_objects[prefab] = source_object(prefab, "GameObject", dict(m_Component=[
                dict(component=reference(transform)), dict(component=reference(animator))]))
            self.source_objects[transform] = source_object(transform, "Transform", dict(m_GameObject=reference(prefab)))
            self.source_objects[animator] = source_object(animator, "Animator", dict(
                m_GameObject=reference(prefab), m_Controller=reference(controller)))
            self.source_objects[controller] = source_object(controller, "AnimatorController", {})
        self.resource = types.SimpleNamespace(
            path_id=7, assets_file=types.SimpleNamespace(name="resources.assets"))
        other_file = types.SimpleNamespace(
            path_id=7, assets_file=types.SimpleNamespace(name="other.assets"))
        self.objects = MagicMock()
        self.source_objects[7] = self.resource
        self.objects.__iter__.side_effect = lambda: iter([*self.source_objects.values(), other_file])
        self.environment = types.SimpleNamespace(objects=self.objects)
        self.load = Mock(return_value=self.environment)
        self.fingerprints = Mock(return_value={str(self.bundle): "0" * 64})
        self.write = Mock()
        self.publish = Mock()

    def run_batch(self, argv):
        with patch.multiple(exporter, PLANTS=self.plants, PROJECT=self.project,
                            source_fingerprints=self.fingerprints, export=self.write,
                            publish_exports=self.publish), \
             patch.object(exporter, "sha256", return_value="0" * 64), \
             patch.object(exporter.UnityPy, "load", self.load), \
             patch("source_index.source_index", side_effect=AssertionError("Full index forbidden")), \
             contextlib.redirect_stdout(io.StringIO()):
            exporter.main(argv)

    def test_batch_shares_object_map_and_keeps_serialized_files_separate(self):
        self.run_batch(["--plant", "black-cherry", "--plant", "frozen-cherry",
                        "--plant", "black-cherry"])
        self.load.assert_called_once_with(str(self.bundle))
        self.fingerprints.assert_called_once_with()
        self.objects.__iter__.assert_called_once()
        calls = self.write.call_args_list
        self.assertEqual([call.args[0] for call in calls], ["black-cherry", "frozen-cherry"])
        self.assertIs(calls[0].args[1], calls[1].args[1])
        for call in calls:
            self.assertIsInstance(call.args[1], exporter.SourceAssets)
            self.assertEqual(call.args[1].objects, self.source_objects)
            self.assertEqual(call.args[2], "0" * 64)

    def test_all_uses_registered_models_and_one_object_scan(self):
        self.run_batch(["--all"])
        self.assertEqual([call.args[0] for call in self.write.call_args_list], list(self.names))
        self.objects.__iter__.assert_called_once()

    def test_default_remains_snow_repeater(self):
        self.run_batch([])
        self.assertEqual(self.write.call_count, 1)
        self.assertEqual(self.write.call_args.args[0], "snow-repeater")

    def test_invalid_later_prefab_does_not_export_earlier_model(self):
        del self.source_objects[self.plants["frozen-cherry"]]
        with self.assertRaisesRegex(ValueError, "expected a GameObject"):
            self.run_batch(["--plant", "black-cherry", "--plant", "frozen-cherry"])
        self.write.assert_not_called()
        self.publish.assert_not_called()

    def test_invalid_later_controller_does_not_export_earlier_model(self):
        controller = self.plants["frozen-cherry"] + 3000000
        self.source_objects[controller].type.name = "AnimatorOverrideController"
        with self.assertRaisesRegex(ValueError, "expected an AnimatorController"):
            self.run_batch(["--plant", "black-cherry", "--plant", "frozen-cherry"])
        self.write.assert_not_called()

    def test_unsupported_build_is_rejected_before_loading_unity(self):
        self.fingerprints.side_effect = ValueError("Source build changed")
        with self.assertRaisesRegex(ValueError, "Source build changed"):
            self.run_batch([])
        self.load.assert_not_called()
        self.write.assert_not_called()
        self.publish.assert_not_called()

    def test_later_decode_failure_does_not_publish_any_model(self):
        self.write.side_effect = [None, ValueError("Invalid animation")]
        with self.assertRaisesRegex(ValueError, "Invalid animation"):
            self.run_batch(["--all"])
        self.publish.assert_not_called()
        self.assertFalse((self.project / "assets").exists())
        self.assertFalse(list(self.project.glob(".export-*")))

    def test_changed_source_does_not_publish_any_model(self):
        self.fingerprints.return_value[str(self.bundle)] = "1" * 64
        with self.assertRaisesRegex(ValueError, "Source changed during export"):
            self.run_batch([])
        self.publish.assert_not_called()
        self.assertFalse((self.project / "assets").exists())

    def test_invalid_later_root_does_not_export_earlier_model(self):
        prefab = self.plants["frozen-cherry"]
        self.source_objects[prefab].read_typetree.return_value["m_Component"].pop(0)
        with self.assertRaisesRegex(ValueError, "one root Transform"):
            self.run_batch(["--plant", "black-cherry", "--plant", "frozen-cherry"])
        self.write.assert_not_called()

    def test_concurrent_export_is_rejected_before_loading_source(self):
        lock_path = self.project / ".cache/export.lock"
        lock_path.parent.mkdir()
        with lock_path.open("a") as lock:
            exporter.fcntl.flock(lock, exporter.fcntl.LOCK_EX | exporter.fcntl.LOCK_NB)
            with self.assertRaises(BlockingIOError):
                self.run_batch([])
        self.load.assert_not_called()
        self.publish.assert_not_called()


class ExportPublicationChecks(unittest.TestCase):
    def setUp(self):
        self.temporary = tempfile.TemporaryDirectory()
        self.addCleanup(self.temporary.cleanup)
        self.project = Path(self.temporary.name)
        self.staging = self.project / ".staging"
        self.output = self.project / "assets/sample"
        self.reference = self.project / "references/sample/visual-assets.json"
        self.staged_output = self.staging / "assets/sample"
        self.staged_reference = self.staging / "references/sample/visual-assets.json"
        for path in (self.output, self.reference.parent, self.staged_output, self.staged_reference.parent):
            path.mkdir(parents=True)
        (self.output / "model.json").write_bytes(b"old model")
        (self.output / "sprite-10.png").write_bytes(b"unchanged PNG")
        (self.output / "sprite-99.png").write_bytes(b"stale PNG")
        (self.staged_output / "model.json").write_bytes(b"new model")
        (self.staged_output / "sprite-10.png").write_bytes(b"unchanged PNG")
        self.reference.write_bytes(b"old proof")
        self.proof = json.dumps(dict(files={path.name: exporter.sha256(path)
                                           for path in self.staged_output.iterdir()})).encode()
        self.staged_reference.write_bytes(self.proof)

    def publish(self):
        with patch.object(exporter, "PROJECT", self.project):
            exporter.publish_exports(self.staging, ["sample"])

    def test_publish_preserves_unchanged_files_and_verifies_new_content(self):
        inode = (self.output / "sprite-10.png").stat().st_ino
        self.publish()
        self.assertEqual((self.output / "sprite-10.png").stat().st_ino, inode)
        self.assertFalse((self.output / "sprite-99.png").exists())
        self.assertEqual((self.output / "model.json").read_bytes(), b"new model")
        self.assertEqual(self.reference.read_bytes(), self.proof)
        self.assertEqual({path.name: exporter.sha256(path) for path in self.output.iterdir()},
                         json.loads(self.proof)["files"])

    def test_unexpected_files_leave_old_export_and_proof_untouched(self):
        (self.output / "manual.txt").write_text("keep")
        with self.assertRaisesRegex(ValueError, "Unexpected files"):
            self.publish()
        self.assertEqual(self.reference.read_bytes(), b"old proof")
        self.assertEqual((self.output / "model.json").read_bytes(), b"old model")
        self.assertEqual((self.output / "manual.txt").read_text(), "keep")

    def test_publication_failure_removes_old_success_marker(self):
        with patch.object(exporter.os, "replace", side_effect=OSError("replace failed")), \
             self.assertRaisesRegex(OSError, "replace failed"):
            self.publish()
        self.assertFalse(self.reference.exists())

    def test_symlinked_destination_cannot_replace_outside_files(self):
        outside = self.project / "outside"
        self.output.rename(outside)
        self.output.symlink_to(outside, target_is_directory=True)
        with self.assertRaisesRegex(ValueError, "Export destination"):
            self.publish()
        self.assertEqual((outside / "model.json").read_bytes(), b"old model")
        self.assertEqual(self.reference.read_bytes(), b"old proof")


class SharedSpriteExportChecks(unittest.TestCase):
    def setUp(self):
        self.temporary = tempfile.TemporaryDirectory()
        self.addCleanup(self.temporary.cleanup)
        root = Path(self.temporary.name)
        self.project, self.source = root / "fusion-mod", root / "source"
        self.names = ("plain-pea-projectile", "plain-star-projectile")
        self.color = (10, 20, 30, 128)
        self.expected_png = root / "original.png"
        with Image.new("RGBA", (2, 4), self.color) as image:
            image.save(self.expected_png)

        def obj(path_id, tree, kind):
            return source_object(path_id, kind, tree)

        self.objects = {10: obj(10, dict(
            m_Name="shared sprite", m_Pivot=dict(x=.25, y=.75),
            m_Rect=dict(width=8, height=8), m_PixelsToUnits=100,
            m_RD=dict(textureRectOffset=dict(x=1, y=2))), "Sprite")}
        self.objects[10].read = Mock(side_effect=lambda: types.SimpleNamespace(
            image=Image.new("RGBA", (2, 4), self.color), m_Name="shared sprite",
            m_Pivot=types.SimpleNamespace(x=.25, y=.75),
            m_Rect=types.SimpleNamespace(width=8, height=8), m_PixelsToUnits=100,
            m_RD=types.SimpleNamespace(textureRectOffset=types.SimpleNamespace(x=1, y=2))))
        for renderer_id, name in enumerate(self.names, start=11):
            prefab = exporter.PLANTS[name]
            transform = prefab + 1000000
            self.objects[prefab] = obj(prefab, dict(m_Name=name, m_IsActive=True,
                m_Component=[dict(component=reference(transform)),
                             dict(component=reference(renderer_id))]), "GameObject")
            self.objects[transform] = obj(transform, dict(
                m_GameObject=reference(prefab), m_Children=[],
                m_LocalPosition=dict(x=0, y=0, z=0),
                m_LocalRotation=dict(x=0, y=0, z=0, w=1),
                m_LocalScale=dict(x=1, y=1, z=1)), "Transform")
            self.objects[renderer_id] = obj(renderer_id, dict(
                m_Sprite=dict(m_PathID=10), m_SortingOrder=0, m_FlipX=False, m_FlipY=False,
                m_Color=dict(r=1, g=1, b=1, a=1), m_Enabled=True), "SpriteRenderer")
        self.assets = exporter.SourceAssets(self.objects)

    def export(self, name):
        with patch.multiple(exporter, PROJECT=self.project, SOURCE=self.source), \
             patch.object(exporter.UnityPy, "load", side_effect=AssertionError("Bundle loading forbidden")), \
             contextlib.redirect_stdout(io.StringIO()):
            exporter.export(name, self.assets, "0" * 64)

    def add_shared_clip(self, clip):
        self.objects[20] = source_object(20, "AnimatorController", dict(
            m_Controller=dict(m_LayerArray=[dict(data={
                "(int&)m_LayerBlendingMode": 0, "m_DefaultWeight": 1,
                "m_SkeletonMask": dict(data=dict(m_Data=[]))})],
                m_StateMachineArray=[dict(data=dict(
                    m_StateConstantArray=[dict(data=dict(m_Speed=1))]))]),
            m_AnimationClips=[dict(m_PathID=21)]))
        self.objects[21] = source_object(21, "AnimationClip", clip)
        for name in self.names:
            prefab = exporter.PLANTS[name]
            animator = prefab + 2000000
            self.objects[animator] = source_object(animator, "Animator", dict(
                m_GameObject=reference(prefab), m_Controller=reference(20)))
            self.objects[prefab].read_typetree.return_value["m_Component"].append(
                dict(component=reference(animator)))

    def test_sorting_order_exports_separate_from_affine_channels(self):
        clip = animation_clip("Solar", constants=(-3,), bindings=[dict(
            typeID=212, attribute=zlib.crc32(b"m_SortingOrder"),
            isPPtrCurve=0, customType=26, path=0)])
        self.add_shared_clip(clip)
        self.export(self.names[0])
        model = json.loads((self.project / "assets" / self.names[0] / "model.json").read_text())
        binding, = model["Clips"]["idle"]["Bindings"]
        self.assertEqual(binding["Attribute"], 12)
        self.assertEqual(binding["Channels"][0][0]["Coefficients"][3], -3)

    def test_shared_object_curve_remaps_source_index_for_each_model(self):
        clip = animation_clip("Solar", frames=[(0, [(0, (0, 0, 0, 0))])], bindings=[
            dict(typeID=212, attribute=0, isPPtrCurve=1, customType=23, path=0)])
        clip["m_ClipBindingConstant"]["pptrCurveMapping"] = [dict(m_FileID=0, m_PathID=10)]
        self.add_shared_clip(clip)
        for name in self.names:
            self.export(name)
            model = json.loads((self.project / "assets" / name / "model.json").read_text())
            binding, = model["Clips"]["idle"]["Bindings"]
            self.assertEqual(binding["Node"], 0)
            self.assertEqual(binding["Attribute"], 5)
            self.assertEqual(binding["Channels"][0][0]["Coefficients"], [0, 0, 0, 10])
        self.assertEqual(self.assets.channels(21), (((0, (0, 0, 0, 0)),),))
        self.objects[21].read_typetree.assert_called_once()

    def test_shared_clip_binding_is_validated_for_each_model(self):
        clip = animation_clip("Solar", constants=(1,), bindings=[dict(
            typeID=1, attribute=zlib.crc32(b"m_IsActive"), isPPtrCurve=0,
            path=zlib.crc32(b"Shared"))])
        transform = exporter.PLANTS[self.names[0]] + 1000000
        self.objects[transform].read_typetree.return_value["m_Children"] = [dict(m_PathID=30)]
        self.objects[30] = types.SimpleNamespace(read_typetree=Mock(return_value=dict(
            m_GameObject=dict(m_PathID=31), m_Children=[],
            m_LocalPosition=dict(x=0, y=0, z=0),
            m_LocalRotation=dict(x=0, y=0, z=0, w=1),
            m_LocalScale=dict(x=1, y=1, z=1))))
        self.objects[31] = types.SimpleNamespace(read_typetree=Mock(return_value=dict(
            m_Name="Shared", m_IsActive=True, m_Component=[])))
        self.add_shared_clip(clip)
        self.export(self.names[0])
        with self.assertRaises(AssertionError):
            self.export(self.names[1])
        self.assertFalse((self.project / "references" / self.names[1] / "visual-assets.json").exists())

    def test_shared_sprite_is_extracted_once_with_separate_verified_outputs(self):
        with patch.object(exporter, "sha256", wraps=exporter.sha256) as hash_file:
            for name in self.names:
                self.export(name)
        self.objects[10].read_typetree.assert_not_called()
        self.objects[10].read.assert_called_once()
        expected_files = set()
        for name in self.names:
            output = self.project / "assets" / name
            sprite, model = output / "sprite-10.png", output / "model.json"
            expected_files.update((sprite, model))
            self.assertEqual(sprite.read_bytes(), self.expected_png.read_bytes())
            self.assertEqual(json.loads(model.read_text())["Sprites"]["10"],
                             dict(File="sprite-10.png", Pivot=[.5, 1], PixelsPerUnit=100))
            proof = json.loads((self.project / "references" / name / "visual-assets.json").read_text())
            sprite_proof, = [item for item in proof["objects"] if item["type"] == "Sprite"]
            self.assertEqual(sprite_proof["sha256"], exporter.sha256(self.expected_png))
            for filename, digest in proof["files"].items():
                self.assertEqual(exporter.sha256(output / filename), digest)
        self.assertEqual(len(hash_file.call_args_list), len(expected_files))
        self.assertEqual({call.args[0] for call in hash_file.call_args_list}, expected_files)

    def test_new_batch_with_same_path_id_uses_its_own_source(self):
        self.export(self.names[0])
        self.color = (200, 30, 40, 255)
        self.assets = exporter.SourceAssets(self.objects)
        self.export(self.names[1])
        sprites = [self.project / "assets" / name / "sprite-10.png" for name in self.names]
        self.assertNotEqual(sprites[0].read_bytes(), sprites[1].read_bytes())
        self.assertEqual(self.objects[10].read.call_count, 2)
        self.objects[10].read_typetree.assert_not_called()

    def test_corrupted_written_sprite_prevents_new_evidence(self):
        write_bytes = Path.write_bytes

        def corrupt_sprite(path, data):
            return write_bytes(path, b"corrupted PNG" if path.name == "sprite-10.png" else data)

        with patch.object(Path, "write_bytes", corrupt_sprite), \
             self.assertRaisesRegex(ValueError, "Exported sprite checksum mismatch"):
            self.export(self.names[0])
        self.assertFalse((self.project / "references" / self.names[0] / "visual-assets.json").exists())


if __name__ == "__main__":
    unittest.main()
