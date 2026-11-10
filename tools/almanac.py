"""Export source almanac entries and portraits for the implemented FusionMod results.

Read-only validation is also used by the existing release checker. No game or
test program is started. Source snapshots are pinned under references/.
"""
import argparse
import hashlib
import json
from pathlib import Path
import re

from source_index import PROJECT, SOURCE, BUNDLE, sha256
from scared_content import merged_recipe_source, localized_source, content as scared_source, portrait, SOURCE_ID, CONTENT_PATH

REFERENCE = PROJECT / "references/fusion-almanac"
OUTPUT = PROJECT / "assets/almanac"
UPGRADES = "references/golden-bean-10-0-0/upgrades.json"


def plain(text):
    return re.sub(r"<[^>]*>", "", text.replace("\\n", "\n")).strip()


TEXT_FIELDS = ("Name", "Info", "Flavor", "Notes")
CJK = re.compile(r"[\u3400-\u9fff\uf900-\ufaff]")


def english_text(name, source, adjustment):
    info = adjustment.get("info", plain(source["info"]))
    for before, after in adjustment.get("replace", {}).items():
        if before not in info:
            raise ValueError(f"Almanac adaptation no longer matches source: {name}: {before}")
        info = info.replace(before, after)
    return dict(Name=name, Info=info, Flavor=adjustment.get("flavor", plain(source["introduce"])), Notes=adjustment.get("notes", ""))


def text_hash(text):
    content = json.dumps({key:text[key] for key in TEXT_FIELDS}, ensure_ascii=False,
                         sort_keys=True, separators=(",", ":")).encode("utf-8")
    return hashlib.sha256(content).hexdigest()


def localized_texts(source_id, name, source, adjustment, german):
    english = english_text(name, source, adjustment)
    translated = german[str(source_id)]
    if translated["EnglishSha256"] != text_hash(english):
        raise ValueError(f"German translation needs review against changed English text: {name}")
    return english, {key:translated[key] for key in TEXT_FIELDS}


def translation_names(rows, names, results, ids, german):
    required = {ingredient for _, pairs in results.values() for pair in pairs for ingredient in pair} - set(results)
    if set(german["Ingredients"]) != required:
        raise ValueError("German ingredient translations differ from active recipes")
    if set(german["Entries"]) != {str(value) for value in ids.values()}:
        raise ValueError("German translations must cover every implemented result exactly once")
    return german["Ingredients"] | {name:german["Entries"][str(ids[name])]["Name"] for name in results}


def catalog_inputs(rows, names, source):
    results = {}
    for a, b, result in rows:
        # Spreading is an instantaneous recipe ending in existing native plants.
        # Keep their native Almanac entries, portraits and behavior.
        from fusion_checks import TRANSIENT_RECIPES
        if (a, b, result) in TRANSIENT_RECIPES:
            continue
        name = names[result]
        pair = tuple(sorted((names[a], names[b])))
        results.setdefault(name, (result, set()))[1].add(pair)
    source_ids = {}
    for recipe in source["recipes"]:
        source_ids.setdefault(recipe["fusion"], set()).add(recipe["fusion_id"])
    ids = {}
    for name in results:
        if len(source_ids.get(name, ())) != 1:
            raise ValueError(f"Ambiguous source almanac identity: {name}")
        ids[name] = next(iter(source_ids[name]))
    # Coin-funded upgrades use a tool instead of seed/glove recipes. Keep them
    # in the same almanac without adding fictitious plant-pair recipes.
    for upgrade in json.loads((PROJECT / UPGRADES).read_text()):
        name = upgrade["name"]
        if names.get(upgrade["identity"]) != name:
            raise ValueError(f"Unavailable golden upgrade: {name}")
        results[name] = (upgrade["identity"], {("Golden Bean", upgrade["ingredient"])})
        ids[name] = upgrade["source_id"]
    # Ice Pot requires the original non-fusible Roasted summons.
    for summon in json.loads((PROJECT / "references/ice-pot/roasted-plants.json").read_text()):
        results[summon["name"]] = (summon["identity"], set())
        ids[summon["name"]] = summon["source_id"]
    return results, ids


def check_catalog(project, rows, names, source):
    errors = []
    output = project / "assets/almanac"
    reference = project / "references/fusion-almanac"
    try:
        expected, ids = catalog_inputs(rows, names, source)
        entries = json.loads((output / "catalog.json").read_text())
        english_source = {p["seedType"]:p for p in json.loads((reference / "LawnStringsTranslate.en.json").read_text())["plants"]}
        adaptations = json.loads((reference / "adaptations.json").read_text())
        german = json.loads((reference / "translations.de.json").read_text())
        localized_source(english_source, german, project)
        adaptations[str(SOURCE_ID)] = scared_source(project)["adaptation"]
        localized_names = translation_names(rows, names, expected, ids, german)
        if len(entries) != len(expected) or {e["CanonicalName"] for e in entries} != set(expected):
            errors.append("Almanac must contain every implemented result exactly once")
        seen = set()
        for entry in entries:
            name = entry["CanonicalName"]
            if name not in expected:
                continue
            identity, pairs = expected[name]
            if entry["Identity"] != identity or entry["SourceId"] != ids[name]:
                errors.append(f"Almanac identity differs from implemented result: {name}")
            if entry["SourceId"] in seen:
                errors.append(f"Duplicate almanac source ID: {name}")
            seen.add(entry["SourceId"])
            if entry["Recipes"] != [" + ".join(pair) for pair in sorted(pairs)]:
                errors.append(f"Almanac recipes differ from runtime: {name}")
            english_texts, german_texts = localized_texts(ids[name], name, english_source[ids[name]],
                adaptations.get(str(ids[name]), {}), german["Entries"])
            for language, expected_text in (("English", english_texts), ("German", german_texts)):
                text = entry[language]
                if {key:text[key] for key in TEXT_FIELDS} != expected_text:
                    errors.append(f"Almanac {language} text differs from reviewed translation: {name}")
                for key in TEXT_FIELDS:
                    value = text[key]
                    if (not isinstance(value, str) or key in ("Name", "Info") and not value.strip()
                            or "<" in value or ">" in value or CJK.search(value)):
                        errors.append(f"Missing or unnormalized almanac {language} {key}: {name}")
                expected_recipes = [" + ".join(pair if language == "English" else
                    [localized_names[ingredient] for ingredient in pair]) for pair in sorted(pairs)]
                if text["Recipes"] != expected_recipes:
                    errors.append(f"Almanac {language} recipes differ from runtime: {name}")
            if bool(english_texts["Notes"]) != bool(german_texts["Notes"]):
                errors.append(f"Almanac translation is missing Replanted notes: {name}")
            image = entry["Portrait"]
            if image != f'{entry["SourceId"]}.png' or not (output/image).is_file():
                errors.append(f"Missing or invalid almanac portrait: {name}")
        manifest = json.loads((reference / "export.json").read_text())
        actual = {p.name: sha256(p) for p in output.iterdir() if p.is_file()}
        if manifest["files"] != actual:
            errors.append("Almanac export checksum mismatch")
        for file, expected_hash in manifest["inputs"].items():
            if sha256(project / file) != expected_hash:
                errors.append(f"Almanac source snapshot changed; export again: {file}")
        if {p["source_id"] for p in manifest["portraits"]} != seen:
            errors.append("Almanac portrait provenance is incomplete")
    except (OSError, ValueError, KeyError, TypeError) as error:
        errors.append(f"Invalid fusion almanac: {error}")
    return errors


def export():
    # Import the source-bundle reader only for exporting, never for release checks.
    import UnityPy
    from fusion_checks import recipes, availability

    rows = recipes()
    names = {entry["identity"]: name for name, entry in availability().items() if entry["available"]}
    source = merged_recipe_source()
    results, ids = catalog_inputs(rows, names, source)
    en_path = REFERENCE / "LawnStringsTranslate.en.json"
    de_path = REFERENCE / "LawnStringsTranslate.de.json"
    if sha256(en_path) != source["translation_names_sha256"]:
        raise ValueError("English source differs from the pinned recipe archive translation")
    english = {p["seedType"]: p for p in json.loads(en_path.read_text())["plants"]}
    german = {p["seedType"]: p for p in json.loads(de_path.read_text())["plants"]}
    translated_german = json.loads((REFERENCE / "translations.de.json").read_text())
    localized_source(english, translated_german)
    localized_names = translation_names(rows, names, results, ids, translated_german)
    original_path = SOURCE / "Mods/PvZ_Fusion_Translator/Dumps/LawnStrings.json"
    original = {p["seedType"]: p for p in json.loads(original_path.read_text())["plants"]}
    enum = {int(number): name for name, number in re.findall(
        r"(\w+) = (-?\d+)", (REFERENCE / "PlantType.cs").read_text())}
    adaptations = json.loads((REFERENCE / "adaptations.json").read_text())
    adaptations[str(SOURCE_ID)] = scared_source()["adaptation"]
    enum[SOURCE_ID] = "BloodSunFlower"
    original[SOURCE_ID] = dict(description=scared_source()["original_english"])
    # The pinned build contains CactusPuff, but its Chinese dump omits it.
    # Preserve the pinned English entry, including its intentionally empty flavor.
    enum[1488] = "CactusPuff"
    original.setdefault(1488, english[1488])
    # These aliases come from the original preview SpriteRenderers and their
    # exported source portraits; they do not alias any runtime plant identity.
    aliases = json.loads((REFERENCE / "portrait-aliases.json").read_text())
    env = UnityPy.load(str(BUNDLE))
    objects = {obj.path_id: obj for obj in env.objects if obj.assets_file.name == "resources.assets"}
    sprites = {}
    for obj in objects.values():
        if obj.type.name == "Sprite":
            sprite = obj.read()
            sprites.setdefault(sprite.m_Name, []).append((obj, sprite))
    entries, proof, originals, image_bytes = [], [], [], {}
    for name, (identity, pairs) in sorted(results.items(), key=lambda item: item[0].casefold()):
        source_id = ids[name]
        text = english[source_id]
        localized = german.get(source_id, text)
        adjustment = adaptations.get(str(source_id), {})
        if source_id == SOURCE_ID:
            image = f"{source_id}.png"
            image_bytes[image], portrait_proof = portrait()
        else:
            alias = aliases.get(enum[source_id], {})
            portrait_name = alias.get("name", enum[source_id])
            matches = sprites.get(portrait_name, [])
            if not matches:
                matches = [match for key, values in sprites.items() if key.casefold() == portrait_name.casefold()
                           for match in values]
            if "sprite" in alias:
                matches = [match for match in matches if match[0].path_id == alias["sprite"]]
            if len(matches) != 1:
                raise ValueError(f"Expected one original portrait for {name} ({portrait_name}), got {len(matches)}")
            obj, sprite = matches[0]
            from io import BytesIO
            png = BytesIO()
            sprite.image.save(png, format="PNG")
            image = f"{source_id}.png"
            image_bytes[image] = png.getvalue()
            portrait_proof = dict(source_id=source_id, source_enum=enum[source_id], sprite_name=sprite.m_Name,
                serialized_file=obj.assets_file.name, sprite_path_id=obj.path_id, texture_path_id=sprite.m_RD.texture.path_id)
        en_text, de_text = localized_texts(source_id, name, text, adjustment, translated_german["Entries"])
        for translation in (en_text, de_text):
            if any(CJK.search(value) for value in translation.values()):
                raise ValueError(f"Untranslated Chinese in almanac entry: {name}")
        en_text["Recipes"] = [" + ".join(pair) for pair in sorted(pairs)]
        de_text["Recipes"] = [" + ".join(localized_names[ingredient] for ingredient in pair) for pair in sorted(pairs)]
        entries.append(dict(SourceId=source_id, Identity=identity, CanonicalName=name,
                            English=en_text, German=de_text, Recipes=en_text["Recipes"], Portrait=image))
        proof.append(portrait_proof)
        originals.append(dict(source_id=source_id, original=original[source_id],
                              english=text, german=localized))
    # Finish resolving every source before writing the export.
    OUTPUT.mkdir(exist_ok=True)
    for name, content in image_bytes.items():
        (OUTPUT/name).write_bytes(content)
    (OUTPUT / "catalog.json").write_text(json.dumps(entries, ensure_ascii=False, indent=2) + "\n")
    (REFERENCE / "original-entries.json").write_text(json.dumps(originals, ensure_ascii=False, indent=2) + "\n")
    input_paths = [PROJECT/UPGRADES, en_path, de_path, REFERENCE/"translations.de.json", REFERENCE/"PlantType.cs", REFERENCE/"adaptations.json",
                   REFERENCE/"portrait-aliases.json", Path(__file__), PROJECT/CONTENT_PATH,
                   PROJECT/"references/scared-2-12-0/portrait.png", PROJECT/"tools/scared_content.py"]
    manifest = dict(bundle=str(BUNDLE), bundle_sha256=sha256(BUNDLE),
                    original_almanac=str(original_path), original_sha256=sha256(original_path),
                    source_assembly_sha256=sha256(SOURCE/"MelonLoader/Il2CppAssemblies/Assembly-CSharp.dll"),
                    translation_url=source["translation_names_url"],
                    german_translation_url=source["translation_names_url"].replace("/English/", "/German/"),
                    inputs={str(p.relative_to(PROJECT)):sha256(p) for p in input_paths},
                    portraits=proof, files={p.name:sha256(p) for p in OUTPUT.iterdir() if p.is_file()})
    (REFERENCE / "export.json").write_text(json.dumps(manifest, ensure_ascii=False, indent=2) + "\n")
    print(f"Exported {len(entries)} almanac entries and original portraits; game not started")


if __name__ == "__main__":
    argparse.ArgumentParser(description=__doc__).parse_args()
    export()
