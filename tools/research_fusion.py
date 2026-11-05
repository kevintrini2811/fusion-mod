"""Archive static Fusion 4.0.5.2 evidence, sharing source data across batch jobs.

Use the existing single-result options or --batch with a JSON list of jobs.
Requires pefile, capstone, and UnityPy for prefab research or an index rebuild.
Never executes game code. Metadata offsets are specific to this source build.
"""
import argparse
import bisect
from functools import cache
import hashlib
import json
from pathlib import Path
import struct

import capstone
import pefile
from source_index import atomic_json, sha256, source_index

PROJECT = Path(__file__).resolve().parents[1]
SOURCE = PROJECT.parent / "pvzfusion/english-4.0.5.2/Game Files"


def read_jobs(parser, args):
    if args.batch is not None:
        if any(value is not None for value in
               (args.slug, args.selected_types, args.source_id, args.prefab)):
            parser.error("--batch cannot be combined with single-result options")
        raw = json.loads(args.batch.read_text())
    else:
        raw = [dict(result=args.result, slug=args.slug, types=args.selected_types,
                    source_id=args.source_id, prefab=args.prefab)]
    if not isinstance(raw, list) or not raw:
        raise ValueError("Batch must be a nonempty JSON list of jobs")
    jobs, slugs = [], set()
    for number, job in enumerate(raw, 1):
        if not isinstance(job, dict) or set(job) - {"result", "slug", "types", "source_id", "prefab"}:
            raise ValueError(f"Job {number}: invalid job fields")
        result, slug, selected = (job.get(key) for key in ("result", "slug", "types"))
        if not isinstance(result, str) or not result.strip():
            raise ValueError(f"Job {number}: result must be a nonempty string")
        if not isinstance(slug, str) or not slug or any(
                c not in "abcdefghijklmnopqrstuvwxyz0123456789-" for c in slug):
            raise ValueError(f"Job {number}: slug must contain only lowercase letters, digits and hyphens")
        if slug in slugs:
            raise ValueError(f"Duplicate output slug: {slug}")
        if not isinstance(selected, list) or not selected or any(
                not isinstance(name, str) or not name.strip() for name in selected):
            raise ValueError(f"Job {number}: types must be a nonempty list of type names")
        for key in ("source_id", "prefab"):
            value = job.get(key)
            if value is not None and (type(value) is not int or value < (1 if key == "prefab" else 0)):
                raise ValueError(f"Job {number}: invalid {key}")
        slugs.add(slug)
        jobs.append(argparse.Namespace(result=result, slug=slug, selected_types=selected,
                                       source_id=job.get("source_id"), prefab=job.get("prefab")))
    return jobs


def research(jobs):
    archive_path = PROJECT / "fusionen-quellen.json"
    almanac_path = SOURCE / "Mods/PvZ_Fusion_Translator/Dumps/LawnStrings.json"
    archive_bytes, almanac_bytes = archive_path.read_bytes(), almanac_path.read_bytes()
    archive, almanac = json.loads(archive_bytes), json.loads(almanac_bytes)["plants"]

    @cache
    def load_environment():
        import UnityPy
        return UnityPy.load(str(SOURCE / "PlantsVsZombiesRH_Data/data.unity3d"))

    # A cache miss can use the same lazy environment as prefab research.
    # On a cache hit without prefabs, UnityPy is never loaded.
    index = source_index(environment=load_environment)
    fingerprints = {Path(path): digest for path, digest in index["sha256"].items()}
    fingerprints[archive_path] = hashlib.sha256(archive_bytes).hexdigest()
    fingerprints[almanac_path] = hashlib.sha256(almanac_bytes).hexdigest()
    if fingerprints[SOURCE / "GameAssembly.dll"] != archive["game_assembly_sha256"]:
        raise ValueError("Source build changed; revalidate metadata offsets and method table")
    metadata_path = SOURCE / "PlantsVsZombiesRH_Data/il2cpp_data/Metadata/global-metadata.dat"
    metadata = metadata_path.read_bytes()
    if struct.unpack_from("<II", metadata) != (0xFAB11BAF, 31):
        raise ValueError("Unsupported source metadata")
    strings = struct.unpack_from("<I", metadata, 24)[0]
    methods, methods_size = struct.unpack_from("<II", metadata, 48)
    types, types_size = struct.unpack_from("<II", metadata, 160)
    if methods_size % 36 or types_size % 88:
        raise ValueError("Invalid source method or type table")

    def string(position):
        start = strings + position
        return metadata[start:metadata.index(b"\0", start)].decode()

    indexed_types = index["classes"]
    by_runtime_type = {
        struct.unpack_from("<i", metadata, types + entry["index"] * 88 + 8)[0]: entry
        for entry in indexed_types
    }
    by_index = {entry["index"]: entry for entry in indexed_types}

    @cache
    def owned_name(type_index):
        entry = by_index[type_index]
        parent = struct.unpack_from("<i", metadata, types + type_index * 88 + 12)[0]
        if parent == -1:
            return entry["name"]
        return owned_name(by_runtime_type[parent]["index"]) + "/" + entry["name"]

    # Validate every recipe, output location and type before writing research files.
    prepared, output_paths = [], set()
    for job in jobs:
        output = PROJECT / "references" / job.slug
        if not output.resolve().is_relative_to((PROJECT / "references").resolve()):
            raise ValueError(f"Output escapes references: {job.slug}")
        if output.resolve() in output_paths:
            raise ValueError(f"Duplicate output directory: {job.slug}")
        output_paths.add(output.resolve())
        recipes = [row for row in archive["recipes"] if row["fusion"] == job.result]
        if not recipes and job.source_id is None:
            raise ValueError(f"{job.result}: result absent from recipe archive; base ingredients require --source-id")
        if recipes and job.source_id is not None:
            raise ValueError("--source-id is only valid for non-recipe base ingredients")
        if not recipes:
            ingredient_ids = {row[key + "_id"] for row in archive["recipes"]
                              for key in ("plant1", "plant2") if row[key] == job.result}
            if ingredient_ids != {job.source_id}:
                raise ValueError("Base ingredient name/ID must match the recipe archive")
        result_ids = {row["fusion_id"] for row in recipes} if recipes else {job.source_id}
        descriptions = [plant for plant in almanac if plant["seedType"] in result_ids]
        if not descriptions and len(result_ids) == 1:
            # Some source-build plants have no entry in the Chinese dump. Use
            # the checksum-pinned translation shipped with our source archive.
            translated_path = PROJECT / "references/fusion-almanac/LawnStringsTranslate.en.json"
            translated_bytes = translated_path.read_bytes()
            if hashlib.sha256(translated_bytes).hexdigest() != archive["translation_names_sha256"]:
                raise ValueError("English source differs from the pinned recipe archive translation")
            descriptions = [plant for plant in json.loads(translated_bytes)["plants"]
                            if plant["seedType"] in result_ids]
            fingerprints[translated_path] = hashlib.sha256(translated_bytes).hexdigest()
        if len(result_ids) != 1 or len(descriptions) != 1:
            raise ValueError(f"{job.result}: expected one source ID and almanac entry")
        found, selected_types = set(), []
        for entry in indexed_types:
            name, namespace = entry["name"], entry["namespace"]
            qualified = f"{namespace}::{name}"
            owned = f"{namespace}::{owned_name(entry['index'])}"
            selected = owned if owned in job.selected_types else qualified if qualified in job.selected_types else name
            if selected not in job.selected_types:
                continue
            if selected in found:
                raise ValueError(f"Ambiguous type name: {name}; qualify its namespace or owning type before export")
            found.add(selected)
            selected_types.append(entry)
        if found != set(job.selected_types):
            raise ValueError(f"{job.result}: missing types: {sorted(set(job.selected_types) - found)}")
        prepared.append((job, output, recipes, descriptions[0], selected_types))

    @cache
    def objects():
        return {obj.path_id: obj for obj in load_environment().objects
                if obj.assets_file.name == "resources.assets"}

    @cache
    def prefab_files(path_id):
        source_objects = objects()
        root_object = source_objects[path_id]
        if root_object.type.name != "GameObject":
            raise ValueError(f"Prefab {path_id} is not a GameObject")
        root = root_object.read_typetree()
        prefab = dict(game_object=path_id, name=root["m_Name"], components=[])
        files = {f"{path_id}-GameObject.json": root}
        for reference in root["m_Component"]:
            component_id = reference["component"]["m_PathID"]
            obj = source_objects[component_id]
            # Some shipped MonoBehaviours have incomplete custom type trees.
            data = obj.read_typetree(check_read=obj.type.name != "MonoBehaviour")
            files[f"{component_id}-{obj.type.name}.json"] = data
            component = dict(path_id=component_id, type=obj.type.name)
            if obj.type.name == "MonoBehaviour":
                script = obj.read(check_read=False).m_Script.deref().read_typetree()
                component.update(script=data["m_Script"], script_name=script["m_ClassName"])
            if obj.type.name == "Animator":
                controller_id = data["m_Controller"]["m_PathID"]
                controller = source_objects[controller_id].read_typetree()
                files[f"{controller_id}-AnimatorController.json"] = controller
                component["controller"] = controller_id
                component["clips"] = [ref["m_PathID"] for ref in controller["m_AnimationClips"]]
            prefab["components"].append(component)
        files["prefab.json"] = prefab
        return files

    # Missing prefab IDs fail before any previous success marker is removed.
    for job, *_ in prepared:
        if job.prefab is not None:
            prefab_files(job.prefab)

    with pefile.PE(str(SOURCE / "GameAssembly.dll"), fast_load=True) as pe:
        base = pe.OPTIONAL_HEADER.ImageBase
        decoder = capstone.Cs(capstone.CS_ARCH_X86, capstone.CS_MODE_64)
        module_pointer_offset = pe.__data__.find(struct.pack("<Q", base + 0x331D670))
        if module_pointer_offset < 8:
            raise ValueError("Source CodeGenModule pointer not found")
        method_count = struct.unpack_from("<Q", pe.__data__, module_pointer_offset - 8)[0]
        if not 10000 < method_count < 200000:
            raise ValueError("Invalid source method pointer count")
        pointers = struct.unpack(f"<{method_count}Q", pe.get_data(0x331D670, method_count * 8))
        starts = sorted(set(pointer - base for pointer in pointers if pointer))

        @cache
        def type_methods(type_index):
            entry = by_index[type_index]
            offset = types + type_index * 88
            start = struct.unpack_from("<i", metadata, offset + 36)[0]
            count = struct.unpack_from("<H", metadata, offset + 64)[0]
            result = []
            for method in range(start, start + count):
                offset = methods + method * 36
                name_index, declaring_type = struct.unpack_from("<Ii", metadata, offset)
                if declaring_type != type_index:
                    raise ValueError("Method declaring type mismatch")
                token = struct.unpack_from("<I", metadata, offset + 24)[0]
                method_name = string(name_index)
                pointer_index = (token & 0xFFFFFF) - 1
                if not 0 <= pointer_index < len(pointers):
                    raise ValueError("Method token outside source pointer table")
                pointer = pointers[pointer_index]
                if pointer <= base:
                    raise ValueError(f"Method has no native body: {entry['name']}.{method_name}")
                rva = pointer - base
                # Preserve full ranges, including tail jumps and alternate paths.
                next_index = bisect.bisect_right(starts, rva)
                if next_index == len(starts):
                    raise ValueError(f"Missing end of native method: {method_name}")
                end = starts[next_index]
                if not 0 < end - rva < 0x10000:
                    raise ValueError(f"Invalid native method range: {method_name}")
                text = "\n".join(f"{ins.address-base:08x}: {ins.mnemonic} {ins.op_str}"
                                 for ins in decoder.disasm(pe.get_data(rva, end-rva), pointer)) + "\n"
                filename = f"fusion-{entry['name']}-{method_name.replace('.', '_')}-{token:08x}.asm"
                evidence = dict(type=entry["name"], namespace=entry["namespace"], method=method_name,
                                token=hex(token), rva=hex(rva), end=hex(end))
                result.append((filename, text, evidence))
            return result

        # Decode all requested types before touching existing research files.
        for _, _, _, _, selected in prepared:
            for entry in selected:
                type_methods(entry["index"])
        for _, output, *_ in prepared:
            output.mkdir(parents=True, exist_ok=True)
            # A failed refresh must not leave an old success marker on new files.
            (output / "research-inputs.json").unlink(missing_ok=True)
        for job, output, recipes, description, selected in prepared:
            (output / "recipes.json").write_text(json.dumps(recipes, indent=2) + "\n")
            (output / "almanac.json").write_text(json.dumps(description, indent=2, ensure_ascii=False) + "\n")
            evidence = []
            for entry in selected:
                for filename, text, method in type_methods(entry["index"]):
                    (output / filename).write_text(text)
                    evidence.append(method)
            (output / "method-addresses.json").write_text(json.dumps(evidence, indent=2) + "\n")
            if job.prefab is not None:
                for filename, data in prefab_files(job.prefab).items():
                    (output / filename).write_text(json.dumps(data, indent=2) + "\n")
            print(f"{job.result}: archived {len(evidence)} methods.")

    # Hash once at the end of the entire batch, never once per result.
    # The proof uses the starting hashes of the data actually researched.
    if any(sha256(path) != digest for path, digest in fingerprints.items()):
        raise ValueError("Source changed during research; no new success markers published")
    inputs = [SOURCE / "GameAssembly.dll", metadata_path, almanac_path, archive_path]
    inputs += [path for path in fingerprints if path.name == "LawnStringsTranslate.en.json"]
    for job, output, *_ in prepared:
        job_inputs = inputs + ([SOURCE / "PlantsVsZombiesRH_Data/data.unity3d"]
                              if job.prefab is not None else [])
        atomic_json(output / "research-inputs.json",
                    dict(result=job.result, types=job.selected_types,
                         sha256={str(path.relative_to(PROJECT.parent)): fingerprints[path]
                                 for path in job_inputs}))
    print(f"Research complete: {len(jobs)} job(s); source checksums unchanged.")


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__)
    selection = parser.add_mutually_exclusive_group(required=True)
    selection.add_argument("--result", help="Exact English result in the recipe archive")
    selection.add_argument("--batch", type=Path, help="JSON list of result/slug/types/source_id/prefab jobs")
    parser.add_argument("--slug", help="Output directory below references")
    parser.add_argument("--type", action="append", dest="selected_types",
                        help="Type name, Namespace::Type, or ::Outer/Nested")
    parser.add_argument("--source-id", type=int, help="Almanac ID for a non-recipe base ingredient")
    parser.add_argument("--prefab", type=int, help="Root GameObject PathID in resources.assets")
    args = parser.parse_args(argv)
    try:
        research(read_jobs(parser, args))
    except (ValueError, KeyError, IndexError, OSError, struct.error, pefile.PEFormatError) as error:
        parser.exit(1, f"ERROR: {error}\n")


if __name__ == "__main__":
    main()
