"""Checksum-validated index of the authoritative source, never executes game code."""
import argparse
import hashlib
import json
import os
from pathlib import Path
import struct
import tempfile

PROJECT = Path(__file__).resolve().parents[1]
SOURCE = PROJECT.parent / "pvzfusion/english-4.0.5.2/Game Files"
BUNDLE = SOURCE / "PlantsVsZombiesRH_Data/data.unity3d"
METADATA = SOURCE / "PlantsVsZombiesRH_Data/il2cpp_data/Metadata/global-metadata.dat"


def sha256(path):
    with Path(path).open("rb") as stream:
        return hashlib.file_digest(stream, "sha256").hexdigest()


def atomic_json(path, data):
    path = Path(path)
    path.parent.mkdir(parents=True, exist_ok=True)
    fd, temporary = tempfile.mkstemp(dir=path.parent, prefix=".index-")
    try:
        with os.fdopen(fd, "w") as stream:
            json.dump(data, stream, ensure_ascii=False, separators=(",", ":"))
            stream.write("\n")
            stream.flush()
            os.fsync(stream.fileno())
        os.replace(temporary, path)
    finally:
        if os.path.exists(temporary):
            os.unlink(temporary)


def source_fingerprints():
    """Validate the supported source build without loading Unity or its index."""
    paths = [BUNDLE, METADATA, SOURCE / "GameAssembly.dll", Path(__file__)]
    fingerprints = {str(p): sha256(p) for p in paths}
    archive_path = PROJECT / "fusionen-quellen.json"
    archive_bytes = archive_path.read_bytes()
    fingerprints[str(archive_path)] = hashlib.sha256(archive_bytes).hexdigest()
    archive = json.loads(archive_bytes)
    if fingerprints[str(SOURCE / "GameAssembly.dll")] != archive["game_assembly_sha256"]:
        raise ValueError("Source build changed; revalidate metadata offsets and method table")
    return fingerprints


def source_index(environment=None, refresh=False):
    """Use an environment or lazy factory; cache hits never invoke the factory."""
    fingerprints = source_fingerprints()
    cache = PROJECT / ".cache/source-index.json"
    if not refresh and cache.exists():
        try:
            saved = json.loads(cache.read_text())
            if saved["sha256"] == fingerprints:
                print("Source index: checksum-validated cache hit")
                return saved
        except (ValueError, KeyError):
            pass
    if environment is None:
        import UnityPy
        environment = UnityPy.load(str(BUNDLE))
    elif callable(environment):
        environment = environment()
    data = METADATA.read_bytes()
    if struct.unpack_from("<II", data) != (0xFAB11BAF, 31):
        raise ValueError("Unsupported source metadata")
    strings = struct.unpack_from("<I", data, 24)[0]
    types, size = struct.unpack_from("<II", data, 160)
    if size % 88:
        raise ValueError("Invalid type table")

    def string(index):
        start = strings + index
        return data[start:data.index(b"\0", start)].decode()

    classes = []
    for index in range(size // 88):
        offset = types + index * 88
        name, namespace = struct.unpack_from("<II", data, offset)
        classes.append(dict(index=index, name=string(name), namespace=string(namespace)))
    prefabs, animators, controllers, clips, scripts = {}, {}, {}, {}, {}
    for obj in environment.objects:
        kind = obj.type.name
        if kind not in ("GameObject", "Animator", "AnimatorController", "AnimationClip", "MonoScript"):
            continue
        if obj.assets_file.name != "resources.assets" and kind != "MonoScript":
            continue
        tree = obj.read_typetree()
        key = str(obj.path_id)
        if kind == "GameObject":
            prefabs[key] = dict(name=tree["m_Name"], components=tree["m_Component"])
        elif kind == "Animator":
            animators[key] = dict(game_object=tree["m_GameObject"], controller=tree["m_Controller"])
        elif kind == "AnimatorController":
            controllers[key] = dict(name=tree["m_Name"], clips=tree["m_AnimationClips"])
        elif kind == "AnimationClip":
            clips[key] = dict(name=tree["m_Name"], duration=tree["m_MuscleClip"]["m_StopTime"])
        else:
            scripts[f"{obj.assets_file.name}:{key}"] = dict(name=tree["m_ClassName"], namespace=tree["m_Namespace"])
    result = dict(sha256=fingerprints, classes=classes, prefabs=prefabs, animators=animators,
                  controllers=controllers, clips=clips, scripts=scripts)
    # Never publish an index if any source changed while it was being read.
    if fingerprints != {path: sha256(path) for path in fingerprints}:
        raise ValueError("Source changed during indexing")
    atomic_json(cache, result)
    print(f"Source index rebuilt: {len(classes)} classes, {len(prefabs)} GameObjects, {len(clips)} clips")
    return result


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--refresh", action="store_true")
    parser.add_argument("--query", default="", help="Case-insensitive name filter")
    args = parser.parse_args()
    index = source_index(refresh=args.refresh)
    for group in ("classes", "prefabs", "animators", "controllers", "clips", "scripts"):
        entries = index[group]
        if isinstance(entries, list):
            entries = {str(item["index"]): item for item in entries}
        matches = {key: value for key, value in entries.items() if args.query.casefold() in json.dumps(value).casefold()}
        print(group, json.dumps(matches, ensure_ascii=False) if args.query else f"{len(entries)} entries")
