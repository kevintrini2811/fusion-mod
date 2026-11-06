"""Recipe availability reports and static release checks. No game execution."""
import argparse
from collections import Counter
import difflib
import hashlib
import json
import math
import os
from pathlib import Path
import re
import stat
import sys
import tempfile
import xml.etree.ElementTree as ET

from source_index import PROJECT, sha256
from registrations import registration_errors
from scared_content import merged_recipe_source, CONTENT_PATH

RECIPE = re.compile(r"\[\((\w+\.\w+), (\w+\.\w+)\)\] = (\w+\.\w+),?")


# Source wrapper identities for instantaneous recipes whose final result is
# already native. These are recipe names, never additional plant registrations.
TRANSIENT_RECIPES = {
    ("SeedType.Puffshroom", "SeedType.Fumeshroom", "SeedType.Fumeshroom"):
        "Puff-shroom Spreading: Fume-shroom",
    ("SeedType.Puffshroom", "SeedType.Scaredyshroom", "SeedType.Scaredyshroom"):
        "Puff-shroom Spreading: Scaredy-shroom",
}


def recipes(text=None):
    if text is None:
        text = (PROJECT / "src/FusionRecipes.cs").read_text()
    rows = RECIPE.findall(text)
    if len(rows) != text.count("] ="):
        raise ValueError("Unrecognized recipe syntax; update checker")
    return rows


def availability(text=None):
    if text is None:
        text = (PROJECT / "tools/ingredient-availability.json").read_text()
    entries = json.loads(text)["ingredients"]
    for name, entry in entries.items():
        if not isinstance(entry["available"], bool) or not entry.get("evidence"):
            raise ValueError(f"Availability needs explicit boolean and evidence: {name}")
    return entries


def recipe_state(source, rows, entries):
    """Share the explicit availability and source-recipe rules with documentation."""
    errors = []

    def require(condition, message):
        if not condition:
            errors.append(message)

    by_identity = {}
    for name, entry in entries.items():
        if entry["available"]:
            identity = entry["identity"]
            require(identity not in by_identity, f"Duplicate available identity: {identity}")
            by_identity[identity] = name
    pairs = [tuple(sorted((a, b))) for a, b, _ in rows]
    require(len({(a, b) for a, b, _ in rows}) == len(rows), "Duplicate directed recipes")
    for pair, count in Counter(pairs).items():
        if count == 1:
            continue
        matching = [(a, b, result) for a, b, result in rows if tuple(sorted((a, b))) == pair]
        require(count == 2 and len({r for _, _, r in matching}) == 2, "Conflicting recipe directions")
        for a, b, result in matching:
            require(any(not row["both_orders"] and (row["plant1"], row["plant2"], row["fusion"])
                == (by_identity.get(a), by_identity.get(b), by_identity.get(result)) for row in source["recipes"]),
                f"Directional recipe lacks source evidence: {a}, {b}, {result}")
    for row in source["recipes"]:
        values = tuple(row[key] for key in ("plant1", "plant2", "fusion"))
        if any(not isinstance(value, str) or not value.strip() or value != value.strip()
               or any(char in value for char in "|\r\n") for value in values):
            raise ValueError("Source names must be nonempty single-line recipe columns")
    archived = {(tuple(sorted((r["plant1"], r["plant2"]))), r["fusion"]) for r in source["recipes"]}
    require(len(archived) == len(source["recipes"]), "Duplicate source recipes")
    require(source["recipe_count"] == len(source["recipes"]), "Archive recipe count mismatch")
    require(source["result_plant_count"] == len({r["fusion"] for r in source["recipes"]}),
            "Archive result count mismatch")
    implemented = set()
    for a, b, result in rows:
        require(all(x in by_identity for x in (a, b, result)), f"Unverified availability: {a}, {b}, {result}")
        if all(x in by_identity for x in (a, b, result)):
            row = (tuple(sorted((by_identity[a], by_identity[b]))),
                   TRANSIENT_RECIPES.get((a, b, result), by_identity[result]))
            require(row in archived, f"Recipe missing in source archive: {row}")
            implemented.add(row)
    return by_identity, implemented, archived - implemented, errors


def documentation_patch():
    """Return a reviewable patch for recipe lists and counts; never write files."""
    names = ("fusionen-quellen.json", "src/FusionRecipes.cs", "tools/ingredient-availability.json",
             "fusionen.txt", "README.md", CONTENT_PATH)
    originals = {name: (PROJECT / name).read_bytes() for name in names}
    source = merged_recipe_source(originals["fusionen-quellen.json"])
    _, implemented, remaining, errors = recipe_state(
        source, recipes(originals["src/FusionRecipes.cs"].decode("utf-8")),
        availability(originals["tools/ingredient-availability.json"].decode("utf-8")))
    if errors:
        raise ValueError("Cannot generate documentation: " + "; ".join(errors))
    archived = {}
    for row in source["recipes"]:
        values = tuple(row[key] for key in ("plant1", "plant2", "fusion"))
        archived[(tuple(sorted(values[:2])), values[2])] = values

    def recipe_line(line):
        values = tuple(value.strip() for value in line.split("|"))
        if len(values) != 3 or not all(values):
            raise ValueError(f"Malformed documentation recipe: {line.rstrip()}")
        key = (tuple(sorted(values[:2])), values[2])
        if key not in archived:
            raise ValueError(f"Documentation recipe absent from source archive: {line.rstrip()}")
        return key

    def section(text, title):
        headings = list(re.finditer(rf"(?m)^## {re.escape(title)}\r?\n", text))
        if len(headings) != 1:
            raise ValueError(f"Expected one README section: {title}")
        start = headings[0].end()
        following = re.search(r"(?m)^## ", text[start:])
        end = start + following.start() if following else len(text)
        return start, end

    pending = originals["fusionen.txt"].decode("utf-8")
    pending_lines = pending.splitlines()
    header = "Plant1 | Plant2 | Fusion name"
    if not pending_lines or pending_lines[0] not in (header, "Pflanze1 | Pflanze2 | Fusionname"):
        raise ValueError("Invalid pending header")
    for line in pending_lines[1:]:
        if line.strip():
            recipe_line(line)
    newline = "\r\n" if "\r\n" in pending else "\n"
    pending_rows = sorted((archived[key] for key in remaining),
                          key=lambda row: (tuple(value.casefold() for value in row), row))
    updated_pending = newline.join([header, *(" | ".join(row) for row in pending_rows)]) + newline

    readme = originals["README.md"].decode("utf-8")
    start, end = section(readme, "List")
    summary = readme[start:end]
    for pattern, replacement in (
            (r"\*\*\d+ open recipes\*\*", f"**{len(remaining)} open recipes**"),
            (r"\*\*\d+ result plants\*\*",
             f"**{len({result for _, result in remaining})} result plants**")):
        summary, count = re.subn(pattern, replacement, summary)
        if count != 1:
            raise ValueError("Expected one pending recipe/result count in README List")
    updated_readme = readme[:start] + summary + readme[end:]
    start, end = section(updated_readme, "Status")
    status = updated_readme[start:end]
    blocks = list(re.finditer(r"(?m)^```text\r?\n(.*?)^```\r?$", status, re.DOTALL))
    if len(blocks) != 1:
        raise ValueError("Expected one text recipe block in README Status")
    block = blocks[0]
    kept, seen = [], set()
    for line in block[1].splitlines(keepends=True):
        if not line.strip():
            kept.append(line)
            continue
        key = recipe_line(line)
        if key in seen:
            raise ValueError("Duplicate README active recipe")
        seen.add(key)
        if key in implemented:
            kept.append(line)
    newline = "\r\n" if "\r\n" in status else "\n"
    additions = sorted((archived[key] for key in implemented - seen),
                       key=lambda row: (tuple(value.casefold() for value in row), row))
    active = "".join(kept) + "".join(" | ".join(row) + newline for row in additions)
    status = status[:block.start(1)] + active + status[block.end(1):]
    updated_readme = updated_readme[:start] + status + updated_readme[end:]

    chunks = []
    for name, updated in (("fusionen.txt", updated_pending), ("README.md", updated_readme)):
        original = originals[name].decode("utf-8")
        for line in difflib.unified_diff(original.splitlines(keepends=True), updated.splitlines(keepends=True),
                                         fromfile=f"a/{name}", tofile=f"b/{name}"):
            chunks.append(line if line.endswith("\n") else line + "\n\\ No newline at end of file\n")
    # Emit nothing if an input changed while the complete patch was prepared.
    if any((PROJECT / name).read_bytes() != data for name, data in originals.items()):
        raise ValueError("Inputs changed during documentation generation; regenerate the patch")
    return "".join(chunks)


def documentation_notes(result):
    """Prepare factual recipe context and a manual capability review; never write files."""
    names = ("fusionen-quellen.json", "src/FusionRecipes.cs", "tools/ingredient-availability.json")
    originals = {name: (PROJECT / name).read_bytes() for name in names}
    source = json.loads(originals[names[0]])
    entries = availability(originals[names[2]].decode("utf-8"))
    _, implemented, _, errors = recipe_state(
        source, recipes(originals[names[1]].decode("utf-8")), entries)
    if errors:
        raise ValueError("Cannot generate notes: " + "; ".join(errors))
    selected = [row for row in source["recipes"] if row["fusion"].casefold() == result.casefold()]
    if not selected:
        raise ValueError(f"Unknown result: {result}")
    ids = [row["fusion_id"] for row in selected]
    if any(type(value) is not int or value < 0 for value in ids) or len(set(ids)) != 1 or \
            len({row["fusion"] for row in selected}) != 1:
        raise ValueError(f"Ambiguous source identity: {result}")
    name = selected[0]["fusion"]
    entry = entries.get(name, {})
    identity = entry.get("identity") if entry.get("available", False) else None
    lines = [f"# {name}: Capabilities and limitations", "",
             "**Draft: The technical sections have not been completed or approved yet.**", "",
             "Only archive and registration details are copied automatically. "
             "They do not confirm capabilities or gameplay behavior. "
             "Replace each unchecked item with an evidenced finding or an explicit limitation.", "",
             "## Identity and recipes", "",
             f"- Source ID from the recipe archive: `{ids[0]}`. Not a Replanted `SeedType`.",
             f"- Result identity from the availability list: `{identity}`." if identity else
             "- Result identity: not marked as available.",
             f"- Recorded availability evidence: {entry.get('evidence', 'No entry available.')}", ""]
    for row in selected:
        pair = tuple(sorted((row["plant1"], row["plant2"])))
        missing = [ingredient for ingredient in pair
                   if not entries.get(ingredient, {}).get("available", False)]
        if (pair, name) in implemented:
            status = "actively registered; behavior still requires technical review"
        elif missing:
            status = "blocked; unavailable ingredients: " + ", ".join(missing)
        else:
            status = "ingredients available; recipe not registered yet"
        lines.append(f"- `{row['plant1']} | {row['plant2']} | {name}`: {status}.")
    lines.extend(["", "## Original capabilities", "",
                  "- [ ] Connect the source ID, actual source type, and prefab script. "
                  "Record the DLL checksum, methods/RVAs, and relative evidence paths.",
                  "- [ ] Describe triggers, effects, damage/HP, intervals, duration, range, "
                  "and units. Separate base behavior, upgrades, and synergies.",
                  "- [ ] Document targeting rules for lanes, ground/air, hypnosis, death, armor, and terrain. "
                  "Trace damage to the code that actually writes health.", "",
                  "## Implementation in Replanted", "",
                  "- [ ] Name the responsible mod methods and the native callers actually used. "
                  "Map each original capability to its implementation or limitation.",
                  "- [ ] Document both card orders, both glove directions, "
                  "cost/cooldown, terrain, support plants, and behavior when creation fails.",
                  "- [ ] Name graphics models, animations, events, sizes/anchors, and "
                  "English and German almanac entries. Separate rendering from simulation.", "",
                  "## Differences and limitations", "",
                  "- [ ] Name each deliberate adaptation with its reason and evidence. "
                  "Explicitly list unported capabilities, click upgrades, synergies, blocked recipes, "
                  "and mode limitations.",
                  "- [ ] Mark unsupported assumptions as unresolved. "
                  "Missing evidence does not establish whether a capability is absent or supported.", "",
                  "## Persistence and lifecycle", "",
                  "- [ ] Document the native storage pair and all state fields, including "
                  "initialization, serialization, loading, reset, death, pool reuse, and board teardown.",
                  "- [ ] Describe transient state, behavior on pause/target loss, and known "
                  "losses on loading. Describe compatible saves and safe removal.", "",
                  "## Validation and handoff", "",
                  "- [ ] List builds and static checks actually performed, with their results "
                  "and evidence paths. This draft does not perform any of these checks.",
                  "- [ ] List existing regression tests and specific manual gameplay checks. "
                  "Document separately who actually performed each check.",
                  "Game launches and gameplay tests remain the user's responsibility. "
                  "Source analysis, builds, and previews do not confirm gameplay behavior.", "",
                  "## Draft input snapshot", "",
                  "SHA-256 hashes of the project files read; not proof of a current source export:", ""])
    lines.extend(f"- `{path}`: `{hashlib.sha256(data).hexdigest()}`."
                 for path, data in originals.items())
    if any((PROJECT / path).read_bytes() != data for path, data in originals.items()):
        raise ValueError("Inputs changed during notes generation; regenerate the draft")
    return "\n".join(lines) + "\n"


def report(result=None, strict=False):
    source = merged_recipe_source()["recipes"]
    entries = availability()
    selected = [r for r in source if result is None or r["fusion"].casefold() == result.casefold()]
    if not selected:
        raise ValueError(f"Unknown result: {result}")
    blocked = False
    for row in selected:
        missing = [name for name in (row["plant1"], row["plant2"])
                   if not entries.get(name, {}).get("available", False)]
        blocked |= bool(missing)
        print(f"{row['plant1']} | {row['plant2']} | {row['fusion']} | "
              + ("BLOCKED: " + ", ".join(missing) if missing else "AVAILABLE"))
    print(f"{len(selected)} source recipes; availability is explicit, never inferred from registration.")
    return not (strict and blocked)


def version():
    return ET.parse(PROJECT / "FusionMod.csproj").findtext("PropertyGroup/Version")


def renderer_models():
    """Read the literal model names from the existing renderer declarations."""
    text = "\n".join(p.read_text() for p in sorted((PROJECT / "src").glob("Imported*Visuals.cs")))
    names = re.findall(r'\bnew\s*\(\s*FusionPlant\.\w+\s*,\s*"(\w+)"\s*\)', text)
    if len(names) != len(re.findall(r'\bnew\s*\(\s*FusionPlant\.', text)):
        raise ValueError("Unrecognized renderer model declaration; use a literal resource name")
    return sorted(set(names))


def sync_resources(write=False):
    """Plan or append missing model resources without rewriting existing XML."""
    path = PROJECT / "FusionMod.csproj"
    if write and path.is_symlink():
        raise ValueError("Refusing to replace a symlinked project file")
    original = path.read_bytes()
    project = ET.fromstring(original)
    if project.tag != "Project":
        raise ValueError("Unsupported project XML namespace")
    parents = {child: parent for parent in project.iter() for child in parent}
    registered, patterns = {}, {}
    slug_pattern = r"[a-z][a-z0-9]*(?:-[a-z0-9]+)*"
    for entry in project.findall(".//EmbeddedResource"):
        group = parents[entry]
        if (group.tag != "ItemGroup" or parents.get(group) is not project
                or "Condition" in group.attrib
                or set(entry.attrib) != {"Include", "LogicalName"} or len(entry)):
            raise ValueError("Resource sync requires unconditional, literal EmbeddedResource entries")
        pattern, logical = entry.attrib["Include"], entry.attrib["LogicalName"]
        if logical in {"FusionMod.Assets.menu-icon.png", "FusionMod.Assets.hypno-zombie.png"} or (
                logical == "FusionMod.DebugCards.%(Filename)%(Extension)"
                and pattern == "assets/debug-cards/eyes-zombie.png;assets/debug-cards/zombie-packets.*"):
            continue
        match = re.fullmatch(r"FusionMod\.(\w+)\.%\(Filename\)%\(Extension\)", logical)
        if not match or not re.fullmatch(rf"assets/{slug_pattern}/\*", pattern):
            raise ValueError(f"Unsupported model resource: {pattern}")
        name = match[1]
        if name in registered or pattern in patterns:
            raise ValueError(f"Duplicate model resource name or directory: {name}")
        registered[name], patterns[pattern] = pattern, name
    names = renderer_models()
    if not names:
        raise ValueError("No renderer model declarations found")
    missing = []
    assets = PROJECT / "assets"
    candidates = {}
    for model in sorted(assets.glob("*/model.json")):
        slug = model.parent.name
        if model.is_file() and re.fullmatch(slug_pattern, slug):
            name = "".join(part[0].upper() + part[1:] for part in slug.split("-"))
            candidates.setdefault(name, []).append(model)
    for name in names:
        if name in registered:
            continue
        matches = candidates.get(name, [])
        if len(matches) != 1:
            raise ValueError(f"Expected one exported model for {name}; found {len(matches)}")
        model, = matches
        if not model.resolve().is_relative_to(assets.resolve()):
            raise ValueError(f"Model path escapes assets: {model}")
        pattern = f"assets/{model.parent.name}/*"
        if pattern in patterns:
            raise ValueError(f"Model directory already registered as {patterns[pattern]}: {pattern}")
        missing.append((pattern, f"FusionMod.{name}.%(Filename)%(Extension)"))
    for pattern, logical in missing:
        print(f'    <EmbeddedResource Include="{pattern}" LogicalName="{logical}" />')
    if not missing:
        print(f"Model resources: all {len(names)} renderer models are registered; no changes")
        return True
    if not write:
        print(f"Model resources: {len(missing)} missing; use resources --write to append")
        return False
    text = original.decode("utf-8")
    closing = re.search(r"(?m)^([ \t]*)</ItemGroup>[ \t]*\r?\n[ \t]*</Project>[ \t]*(?:\r?\n)?\Z", text)
    if (not closing or project[-1].tag != "ItemGroup" or "Condition" in project[-1].attrib):
        raise ValueError("Expected an unconditional final ItemGroup; project left unchanged")
    newline = "\r\n" if "\r\n" in text else "\n"
    indent = closing[1] + "  "
    additions = "".join(f'{indent}<EmbeddedResource Include="{pattern}" LogicalName="{logical}" />{newline}'
                        for pattern, logical in missing)
    updated = (text[:closing.start()] + additions + text[closing.start():]).encode("utf-8")
    mode = stat.S_IMODE(path.stat().st_mode)
    fd, temporary = tempfile.mkstemp(dir=path.parent, prefix=".model-resources-")
    try:
        with os.fdopen(fd, "wb") as stream:
            os.fchmod(stream.fileno(), mode)
            stream.write(updated)
            stream.flush()
            os.fsync(stream.fileno())
        if path.is_symlink() or path.read_bytes() != original:
            raise ValueError("Project changed during resource sync; refusing to overwrite")
        os.replace(temporary, path)
        directory = os.open(path.parent, os.O_DIRECTORY)
        try:
            os.fsync(directory)
        finally:
            os.close(directory)
    finally:
        if os.path.exists(temporary):
            os.unlink(temporary)
    print(f"Model resources: appended {len(missing)} registrations; run fusion_checks.py check")
    return True


def check():
    errors = registration_errors(PROJECT)
    def require(condition, message):
        if not condition:
            errors.append(message)
    source = merged_recipe_source()
    rows = recipes()
    entries = availability()
    by_identity, implemented, expected_pending, recipe_errors = recipe_state(source, rows, entries)
    errors.extend(recipe_errors)
    from almanac import check_catalog
    errors.extend(check_catalog(PROJECT, rows, by_identity, source))
    plant_text = (PROJECT / "src/FusionPlant.cs").read_text()
    markers = re.findall(r"FusionPlant (\w+) = new\(SeedType\.(\w+), SeedType\.(\w+)\)", plant_text)
    require(len(markers) == plant_text.count("= new(SeedType."), "Unrecognized persisted identity syntax")
    counts = Counter((native, marker) for _, native, marker in markers)
    for pair, count in counts.items():
        require(count == 1, f"Duplicate save identity: {pair}")
        require(pair[1] not in ("None", "Imitater"), f"Unsafe save marker: {pair}")
    for name, _, _ in markers:
        require(f"FusionPlant.{name}" in by_identity, f"Variant missing explicit availability: {name}")
    project = ET.parse(PROJECT / "FusionMod.csproj")
    resources = {}
    for resource in project.findall(".//EmbeddedResource"):
        pattern = resource.attrib["Include"]
        files = [path for part in pattern.split(";") for path in PROJECT.glob(part)]
        require(bool(files), f"Missing resource glob: {pattern}")
        logical = resource.attrib["LogicalName"]
        prefix = logical.split(".%(Filename)")[0]
        resources[prefix] = {p.name: p for p in files if p.is_file()}
        if logical in {"FusionMod.Assets.menu-icon.png", "FusionMod.Assets.hypno-zombie.png"} or (
                logical == "FusionMod.DebugCards.%(Filename)%(Extension)"
                and pattern == "assets/debug-cards/eyes-zombie.png;assets/debug-cards/zombie-packets.*"):
            from PIL import Image
            with Image.open(files[0]) as artwork:
                require(artwork.format == "PNG" and artwork.mode == "RGBA", "Invalid Fusion presentation artwork")
                artwork.verify()
            continue
        if prefix == "FusionMod.Almanac":
            require(pattern == "assets/almanac/*", "Unexpected almanac resource path")
            continue
        if prefix == "FusionMod.EyesArmor":
            from PIL import Image
            require(set(resources[prefix]) == {kind + stage + ".png" for kind in ("cone", "bucket") for stage in ("", "2", "3")} | {"eyes-cone.png", "eyes-bucket.png", "placement.json"},
                    "Incomplete Eyes Zombie armor stages")
            for path in resources[prefix].values():
                if path.name == "placement.json":
                    fits = json.loads(path.read_text())
                    require(set(fits) == {kind + stage for kind in ("cone", "bucket") for stage in ("", "2", "3")},
                            "Incomplete native armor placement geometry")
                    require(all(len(fit) == 3 and all(isinstance(value, (int, float)) and math.isfinite(value) for value in fit)
                                and fit[0] > 0 for fit in fits.values()), "Invalid native armor placement geometry")
                    continue
                with Image.open(path) as armor:
                    require(armor.mode == "RGBA", "Armor must preserve transparency")
                    armor.verify()
            continue
        if prefix == "FusionMod.ZombieAlmanac":
            from PIL import Image
            require(pattern == "assets/zombie-almanac/*", "Unexpected zombie almanac resource path")
            expected = {key + suffix + ".png" for key in ("eyes", "eyes-cone", "eyes-bucket", "hypno")
                        for suffix in ("", "-duck")}
            require(set(resources[prefix]) == expected, "Incomplete zombie almanac thumbnails")
            proof = json.loads((PROJECT.parent / "zombie-library/assets/almanac-sources.json").read_text())
            for name, path in resources[prefix].items():
                require(proof["outputs"].get(str(path.resolve())) == sha256(path), f"Zombie thumbnail changed: {name}")
                with Image.open(path) as thumbnail:
                    require(thumbnail.format == "PNG" and thumbnail.mode == "RGBA" and thumbnail.size == (262, 262),
                            f"Invalid native zombie thumbnail: {name}")
                    thumbnail.verify()
            continue
        if prefix == "FusionMod.PoolDuck":
            from PIL import Image
            require(pattern == "assets/pool-duck/*", "Unexpected pool duck resources")
            require(set(resources[prefix]) == {"duck.png"}, "Unexpected pool duck asset files")
            proof = json.loads((PROJECT / "references/pool-debug-18-native/duck-asset.json").read_text())
            require(sha256(PROJECT / "assets/pool-duck/duck.png") == proof["sha256"], "Native duck tube changed")
            with Image.open(PROJECT / "assets/pool-duck/duck.png") as duck:
                require(duck.format == "PNG" and duck.mode == "RGBA", "Invalid native duck tube")
                duck.verify()
            continue
        if prefix == "FusionMod.GoldenBean":
            from PIL import Image
            require(pattern == "assets/golden-bean/*", "Unexpected golden bean resource path")
            require(set(resources[prefix]) == {"icon.png"}, "Unexpected golden bean resources")
            proof = json.loads((PROJECT / "references/golden-bean-10-0-0/icon.json").read_text())
            require(sha256(PROJECT / "assets/golden-bean/icon.png") == proof["sha256"], "Golden bean icon changed")
            with Image.open(PROJECT / "assets/golden-bean/icon.png") as icon:
                require(icon.format == "PNG" and icon.mode == "RGBA", "Golden bean icon must be RGBA PNG")
                icon.verify()
            continue
        if prefix == "FusionMod.DebugCards":
            from PIL import Image
            # Debug Mod owns the chooser packet atlas after extraction. Fusion
            # retains only its eyes portrait and static zombie packet resources.
            require(pattern == "assets/debug-cards/eyes-zombie.png;assets/debug-cards/zombie-packets.*",
                    "Unexpected debug card resource path")
            require(set(resources[prefix]) == {"eyes-zombie.png", "zombie-packets.png", "zombie-packets.json"},
                    "Unexpected debug card resources")
            proof = json.loads((PROJECT / "references/debug-cards-rage-5-0-0/card-preview.json").read_text())
            for filename, digest in proof["files"].items():
                require(sha256(PROJECT / filename) == digest, f"Debug card preview changed: {filename}")
            with Image.open(PROJECT / "assets/debug-cards/eyes-zombie.png") as preview:
                require(preview.format == "PNG" and preview.mode == "RGBA", "Debug card preview must be RGBA PNG")
                preview.verify()
            zombies = json.loads((PROJECT / "assets/debug-cards/zombie-packets.json").read_text())
            zombie_proof = json.loads((PROJECT / "references/zombie-accuracy-13-0-9/static-zombie-assets.json").read_text())
            require(len(zombie_proof["native_pose_inputs"]) == 8, "Missing native zombie variant poses")
            for filename, digest in zombie_proof["native_pose_inputs"].items():
                require(sha256(PROJECT / filename) == digest, f"Native zombie pose changed: {filename}")
            for filename, digest in zombie_proof["outputs"].items():
                require(sha256(PROJECT / "assets/debug-cards" / filename) == digest, f"Static zombie atlas changed: {filename}")
            require(len(zombies) == 74 and {(entry["Type"], entry["Role"]) for entry in zombies}
                    == {(kind, role) for kind in range(37) for role in ("Portrait", "Packet")},
                    "Incomplete static zombie atlas")
            with Image.open(PROJECT / "assets/debug-cards/zombie-packets.png") as atlas:
                require(atlas.format == "PNG" and atlas.mode == "RGBA", "Invalid static zombie atlas")
                for entry in zombies:
                    x, y, width, height = (entry[key] for key in ("X", "Y", "Width", "Height"))
                    expected = (256, 320) if entry["Role"] == "Portrait" else (128, 176)
                    require((width, height) == expected and 0 <= x <= atlas.width-width and 0 <= y <= atlas.height-height,
                            "Invalid static zombie atlas rectangle")
                atlas.verify()
            with Image.open(PROJECT / "assets/debug-cards/packet-background.png") as packet:
                require(packet.format == "PNG" and packet.mode == "RGBA" and packet.size == (128, 176), "Invalid native packet background")
                packet.verify()
            continue
        require("model.json" in resources[prefix], f"Missing model: {pattern}")
        if "model.json" not in resources[prefix]:
            continue
        model = json.loads(resources[prefix]["model.json"].read_text())
        sprites = model["Sprites"]
        ground = model.get("GroundNode", -1)
        require(ground == -1 or 0 <= ground < len(model["Nodes"]), f"Invalid original footprint: {prefix}")
        if 0 <= ground < len(model["Nodes"]):
            require(model["Nodes"][ground]["Sprite"] >= 0,
                    f"Original footprint has no sprite: {prefix}")
        for sprite in sprites.values():
            require(sprite["File"] in resources[prefix], f"Missing sprite resource: {prefix}.{sprite['File']}")
        for node in model["Nodes"]:
            require(node["Sprite"] == -1 or str(node["Sprite"]) in sprites, f"Missing node sprite: {prefix}")
            require(node.get("MaskSprite", -1) == -1 or str(node["MaskSprite"]) in sprites,
                    f"Missing original mask sprite: {prefix}")
        for skin in model.get("Skins", []):
            require(0 <= skin["Node"] < len(model["Nodes"]), f"Invalid skin node: {prefix}")
            require(str(skin["Sprite"]) in sprites, f"Missing skin sprite: {prefix}")
            require(len(skin["Vertices"]) == len(skin["Weights"]) == len(skin["Indices"]),
                    f"Skin vertex/weight count differs: {prefix}")
            require(len(skin["Bones"]) == len(skin["BindPoses"])
                    and all(0 <= i < len(model["Nodes"]) for i in skin["Bones"]),
                    f"Invalid skin bone references: {prefix}")
            require(all(len(m) == 16 for m in skin["BindPoses"]), f"Invalid skin bind pose: {prefix}")
            require(all(abs(sum(w)-1) < .0001 for w in skin["Weights"]), f"Invalid skin weights: {prefix}")
            require(all(0 <= i < len(skin["Bones"]) for indices in skin["Indices"] for i in indices),
                    f"Invalid skin bone index: {prefix}")
            require(len(skin["Triangles"]) % 3 == 0
                    and all(0 <= i < len(skin["Vertices"]) for i in skin["Triangles"]),
                    f"Invalid skin triangles: {prefix}")
        for name, variants in (model.get("DamageSprites") or {}).items():
            require(any(n["Name"] == name and n["Sprite"] >= 0 for n in model["Nodes"]),
                    f"Missing damage sprite node: {prefix}.{name}")
            require(len(variants) == 2 and all(str(s) in sprites for s in variants),
                    f"Missing damage sprite variants: {prefix}.{name}")
        manifest_path = PROJECT / "references" / Path(pattern).parent.name / "visual-assets.json"
        require(manifest_path.exists(), f"Missing export manifest: {prefix}")
        if manifest_path.exists():
            expected = json.loads(manifest_path.read_text())["files"]
            actual = {name: sha256(path) for name, path in resources[prefix].items()}
            require(expected == actual, f"Export checksum mismatch: {prefix}")
    for name in renderer_models():
        require(f"FusionMod.{name}" in resources, f"Unembedded model: {name}")
    require("FusionMod.Almanac" in resources, "Unembedded fusion almanac")
    current = version()
    require(bool(re.fullmatch(r"\d+\.\d+\.\d+", current or "")), "Invalid project version")
    mod = (PROJECT / "src/FusionMod.cs").read_text()
    require(re.search(r'MelonInfo\([^\n]*"' + re.escape(current) + '"', mod) is not None, "MelonInfo version mismatch")
    readme = (PROJECT / "README.md").read_text()
    require(readme.splitlines()[0] == f"# Fusion Mod {current}", "README version mismatch")
    pending_lines = (PROJECT / "fusionen.txt").read_text().splitlines()
    require(pending_lines[0] in ("Plant1 | Plant2 | Fusion name", "Pflanze1 | Pflanze2 | Fusionname"), "Invalid pending header")
    pending = [tuple(x.strip() for x in line.split("|")) for line in pending_lines[1:] if line.strip()]
    require(all(len(row) == 3 for row in pending), "Malformed pending recipe")
    require(len(set(pending)) == len(pending), "Duplicate pending rows")
    remaining = {(tuple(sorted((r[0], r[1]))), r[2]) for r in pending if len(r) == 3}
    require(remaining == expected_pending, "Pending list differs from source minus implemented recipes")
    section = readme.split("## List\n", 1)[1].split("\n## ", 1)[0]
    require(f"**{len(pending)} open recipes**" in section, "README pending recipe count mismatch")
    require(f"**{len({r[2] for r in pending})} result plants**" in section, "README result count mismatch")
    active_section = readme.split("## Status\n", 1)[1].split("```text\n", 1)[1].split("```", 1)[0]
    active = {tuple(x.strip() for x in line.split("|")) for line in active_section.splitlines() if line.strip()}
    active_normalized = {(tuple(sorted((a, b))), c) for a, b, c in active}
    require(active_normalized == implemented, "README active recipes mismatch")
    for error in errors:
        print("ERROR:", error)
    print(f"Static check: {len(rows)} active recipes, {len(pending)} pending recipes, {len(errors)} errors")
    return not errors


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description=__doc__)
    commands = parser.add_subparsers(dest="command", required=True)
    commands.add_parser("check")
    commands.add_parser("docs", help="Print a read-only patch for pending recipes and README lists/counts")
    notes_parser = commands.add_parser("notes", help="Print a draft for capability and limitation documentation")
    notes_parser.add_argument("--result", required=True)
    resource_parser = commands.add_parser("resources", help="Plan missing model resource registrations")
    resource_parser.add_argument("--write", action="store_true", help="Append missing registrations atomically")
    recipe_parser = commands.add_parser("recipes")
    recipe_parser.add_argument("--result")
    recipe_parser.add_argument("--strict", action="store_true", help="Fail if any alternative is blocked")
    args = parser.parse_args()
    try:
        if args.command == "docs":
            sys.stdout.write(documentation_patch())
            ok = True
        elif args.command == "notes":
            sys.stdout.write(documentation_notes(args.result))
            ok = True
        elif args.command == "resources":
            ok = sync_resources(args.write)
        else:
            ok = check() if args.command == "check" else report(args.result, args.strict)
    except (ValueError, KeyError, IndexError, OSError, ET.ParseError) as error:
        parser.exit(1, f"ERROR: {error}\n")
    raise SystemExit(0 if ok else 1)
