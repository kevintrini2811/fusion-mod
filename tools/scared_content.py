"""Scared source additions; the authoritative Fusion recipe archive stays intact."""
import json
from pathlib import Path
from source_index import PROJECT, sha256

CONTENT_PATH = "references/scared-2-12-0/content.json"
SOURCE_ID = 901001  # Almanac key only. Never passed to native plant APIs.


def verify_source(source):
    expected = json.loads((PROJECT / "references/scared-2-12-0/source-sha256.json").read_text())
    actual = {str(source / name): sha256(source / name) for name in expected}
    if any(actual[str(source / name)] != digest for name, digest in expected.items()):
        raise ValueError("Unsupported Scared source; revalidate metadata, code and asset identities")
    return actual


def content(project=PROJECT):
    return json.loads((project / CONTENT_PATH).read_text())


def merged_recipe_source(raw=None, project=PROJECT):
    source = json.loads(raw if raw is not None else (project / "fusionen-quellen.json").read_text())
    source["recipes"].append(content(project)["recipe"])
    source["recipe_count"] = len(source["recipes"])
    source["result_plant_count"] = len({r["fusion"] for r in source["recipes"]})
    return source


def localized_source(english, german, project=PROJECT):
    source = content(project)
    english[SOURCE_ID] = source["english_source"]
    german["Entries"][str(SOURCE_ID)] = source["german"]
    german["Ingredients"]["Eyes Zombie Head"] = "Augen-Zombie-Kopf"


def portrait(project=PROJECT):
    path = project / "references/scared-2-12-0/portrait.png"
    return path.read_bytes(), dict(source_id=SOURCE_ID, source_enum="BloodSunFlower",
        serialized_file="resources.assets", sprite_path_id=6131,
        sprite_name="BloodSunFlower", source="PVZ-Scared", source_portrait=str(path.relative_to(project)))
