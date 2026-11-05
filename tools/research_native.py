"""Archive Replanted callers, target-rule paths and field/lifecycle evidence.

Reads files only; never loads game code. Supports the locally verified x64 build.
The graph covers Assembly-CSharp method-pointer ranges, not every native function.
"""
import argparse
import bisect
from collections import defaultdict
import fcntl
import hashlib
import json
import os
from pathlib import Path
import re
import struct
import tempfile

import capstone
from capstone import x86_const as x86
import pefile

from source_index import PROJECT, atomic_json, sha256

GAME = Path("/mnt/FastGaming1/SteamLibrary/steamapps/common/PVZ Replanted")
DUMMY = PROJECT.parent / "glove-mod/research/dummy"
SUPPORTED_DLL = "e7333eaf87996fa44b4a3526986272319b7d01f7acdbcff52451652f61ceb696"
MODULE_RVA = 0x3DAB750
LIFECYCLE = re.compile(r"^(?:\.ctor|Reset|Serialize|Deserialize|PostDeserialize|"
                       r"Initialize|\w+Initialize|Die\w*|Remove\w*)$")
TARGET_RULE = re.compile(r"Target|AttackRect|Collision|IsFlying|DeadOrDying|NotOnGround")


def fingerprints(game):
    paths = [game / "GameAssembly.dll",
             game / "Replanted_Data/il2cpp_data/Metadata/global-metadata.dat",
             Path(__file__), PROJECT / "tools/source_index.py", PROJECT / "tools/requirements.txt"]
    result = {str(path): sha256(path) for path in paths}
    if result[str(paths[0])] != SUPPORTED_DLL:
        raise ValueError("Unsupported Replanted DLL; revalidate module RVA before analysis")
    return result


def metadata_symbols(data, pointers, base):
    if struct.unpack_from("<II", data) != (0xFAB11BAF, 31):
        raise ValueError("Unsupported Replanted metadata")
    strings = struct.unpack_from("<I", data, 24)[0]
    methods, methods_size = struct.unpack_from("<II", data, 48)
    fields, fields_size = struct.unpack_from("<II", data, 96)
    types, types_size = struct.unpack_from("<II", data, 160)
    images, images_size = struct.unpack_from("<II", data, 168)
    if methods_size % 36 or fields_size % 12 or types_size % 88 or images_size % 40:
        raise ValueError("Invalid Replanted metadata tables")

    def string(index):
        start = strings + index
        return data[start:data.index(b"\0", start)].decode()

    image = [struct.unpack_from("<II", data, at + 8)
             for at in range(images, images + images_size, 40)
             if string(struct.unpack_from("<I", data, at)[0]) == "Assembly-CSharp.dll"]
    if len(image) != 1:
        raise ValueError("Expected one Assembly-CSharp metadata image")
    first, count = image[0]
    if first + count > types_size // 88:
        raise ValueError("Type range outside metadata")
    runtime_types = {struct.unpack_from("<i", data, types + i * 88 + 8)[0]: i
                     for i in range(types_size // 88)}
    names = {}

    def type_name(index):
        if index not in names:
            at = types + index * 88
            name, namespace = (string(n) for n in struct.unpack_from("<II", data, at))
            parent = struct.unpack_from("<i", data, at + 12)[0]
            names[index] = (type_name(runtime_types[parent]) + "/" + name if parent != -1
                            else (namespace + "." if namespace else "") + name)
        return names[index]

    records, owners = [], {}
    for index in range(first, first + count):
        at = types + index * 88
        owner = type_name(index)
        field_start, method_start = struct.unpack_from("<ii", data, at + 32)
        method_count, _, field_count = struct.unpack_from("<HHH", data, at + 64)
        field_names = {}
        for field in range(field_start, field_start + field_count):
            if not 0 <= field < fields_size // 12:
                raise ValueError("Field outside metadata")
            name, _, token = struct.unpack_from("<III", data, fields + field * 12)
            field_names[hex(token)] = string(name)
        parent = struct.unpack_from("<i", data, at + 16)[0]
        owners[owner] = dict(fields=field_names,
                             parent=type_name(runtime_types[parent]) if parent in runtime_types else None)
        for method in range(method_start, method_start + method_count):
            if not 0 <= method < methods_size // 36:
                raise ValueError("Method outside metadata")
            offset = methods + method * 36
            name, declaring = struct.unpack_from("<Ii", data, offset)
            token = struct.unpack_from("<I", data, offset + 24)[0]
            pointer_index = (token & 0xFFFFFF) - 1
            if declaring != index or token >> 24 != 6 or not 0 <= pointer_index < len(pointers):
                raise ValueError("Metadata does not match native method table")
            pointer = pointers[pointer_index]
            records.append(dict(type=owner, method=string(name), token=hex(token),
                                rva=pointer - base if pointer > base else None))
    return records, owners


def method_ranges(records, pointers, pe):
    base = pe.OPTIONAL_HEADER.ImageBase
    starts = sorted({p - base for p in pointers if p > base})
    executable = [(s.VirtualAddress, s.VirtualAddress + s.Misc_VirtualSize)
                  for s in pe.sections if s.Characteristics & 0x20000000]
    ranges, missing = {}, []
    for record in records:
        rva = record["rva"]
        if rva is None:
            missing.append(dict(record, reason="No native method pointer (abstract/generic/external)"))
            continue
        section_end = next((end for start, end in executable if start <= rva < end), None)
        following = bisect.bisect_right(starts, rva)
        end = min(starts[following], section_end) if section_end and following < len(starts) else section_end
        if end is None or not 0 < end - rva < 0x10000:
            missing.append(dict(record, reason="No bounded executable method range"))
            continue
        record["end"] = end
        ranges[rva] = end
    return ranges, missing


def instructions(pe, decoder, rva, end):
    base = pe.OPTIONAL_HEADER.ImageBase
    code = pe.get_data(rva, end - rva)
    return list(decoder.disasm(code, base + rva))


def flow_edges(decoded, base, start, end):
    direct, indirect = [], []
    for ins in decoded:
        if ins.mnemonic not in ("call", "jmp") and not ins.group(capstone.CS_GRP_JUMP):
            continue
        if not ins.operands:
            continue
        operand = ins.operands[0]
        edge = dict(site=ins.address - base, instruction=ins.mnemonic, operand=ins.op_str)
        if operand.type == x86.X86_OP_IMM:
            target = operand.imm - base
            # Internal branches do not form inter-method call paths.
            if ins.mnemonic == "call" or not start <= target < end:
                direct.append(dict(edge, target=target))
        else:
            indirect.append(edge)
    return direct, indirect


def build_index(game, pe, decoder):
    base = pe.OPTIONAL_HEADER.ImageBase
    count, table = struct.unpack("<QQ", pe.get_data(MODULE_RVA + 8, 16))
    if not 1000 < count < 200000:
        raise ValueError("Invalid Replanted method pointer count")
    pointers = struct.unpack(f"<{count}Q", pe.get_data(table - base, count * 8))
    records, owners = metadata_symbols(
        (game / "Replanted_Data/il2cpp_data/Metadata/global-metadata.dat").read_bytes(), pointers, base)
    ranges, missing = method_ranges(records, pointers, pe)
    graph = {}
    for rva, end in sorted(ranges.items()):
        decoded = instructions(pe, decoder, rva, end)
        direct, indirect = flow_edges(decoded, base, rva, end)
        decoded_end = decoded[-1].address - base + decoded[-1].size if decoded else rva
        graph[str(rva)] = dict(end=end, direct=direct, indirect=indirect, decoded_end=decoded_end)
    return dict(methods=records, types=owners, graph=graph, missing=missing)


def native_index(game, pe, decoder, initial, refresh=False):
    path = PROJECT / ".cache/native-index.json"
    versions = dict(capstone=capstone.__version__, pefile=pefile.__version__)
    if not refresh and path.exists():
        try:
            saved = json.loads(path.read_text())
            if saved["sha256"] == initial and saved.get("dependencies") == versions:
                print("Native index: checksum-validated cache hit")
                return saved
        except (ValueError, KeyError):
            pass
    result = build_index(game, pe, decoder)
    if fingerprints(game) != initial:
        raise ValueError("Native inputs changed during indexing")
    result["sha256"] = initial
    result["dependencies"] = versions
    atomic_json(path, result)
    print(f"Native index rebuilt: {len(result['graph'])} ranges, {len(result['missing'])} methods without ranges")
    return result


def resolve_type(name, types):
    matches = [owner for owner in types if owner == name or owner.rsplit(".", 1)[-1] == name]
    if len(matches) != 1:
        raise ValueError(f"Unknown or ambiguous type: {name}; use its full namespace")
    return matches[0]


def select_methods(selectors, index):
    selected = set()
    for selector in selectors:
        if re.fullmatch(r"0x[0-9a-fA-F]+", selector):
            matches = [m for m in index["methods"] if int(m["token"], 16) == int(selector, 16)]
        else:
            owner, separator, method = selector.rpartition(".")
            if not separator:
                raise ValueError(f"Expected Type.Method or method token: {selector}")
            owner = resolve_type(owner, index["types"])
            matches = [m for m in index["methods"] if m["type"] == owner and m["method"] == method]
        if not matches:
            raise ValueError(f"Unknown method: {selector}")
        selected.update(m["token"] for m in matches)
    return selected


def select_fields(selectors, index, dummy):
    selected, inputs = [], {}
    for selector in selectors:
        name, separator, field = selector.rpartition(".")
        if not separator:
            raise ValueError(f"Expected Type.Field: {selector}")
        owner = resolve_type(name, index["types"])
        # The decompiled file supplies offsets only. Native metadata owns names/tokens.
        if "/" in owner:
            raise ValueError("Field offsets for nested types require a separately verified declaration")
        namespace, _, short = owner.rpartition(".")
        path = dummy / namespace / f"{short}.cs"
        content = path.read_bytes()
        inputs[str(path)] = hashlib.sha256(content).hexdigest()
        declarations = re.findall(
            r'\[Token\(Token = "(0x[0-9A-Fa-f]+)"\)\]\s*'
            r'\[FieldOffset\(Offset = "(0x[0-9A-Fa-f]+)"\)\]\s*'
            r'(?:public|private|protected|internal)\s+(?!static\b)[\w<>.,\[\]?]+\s+(\w+)\s*;',
            content.decode())
        matches = [(token, offset) for token, offset, member in declarations
                   if member == field and index["types"][owner]["fields"].get(hex(int(token, 16))) == field]
        if len(matches) != 1:
            raise ValueError(f"Missing or unverified instance field: {selector}")
        token, offset = matches[0]
        selected.append(dict(type=owner, field=field, token=hex(int(token, 16)), offset=int(offset, 16)))
    return selected, inputs


def field_candidates(decoded, base, fields):
    candidates = []
    for ins in decoded:
        for operand in ins.operands:
            if operand.type != x86.X86_OP_MEM:
                continue
            memory = operand.mem
            if memory.base in (0, x86.X86_REG_RIP, x86.X86_REG_RSP) or memory.index:
                continue
            for field in fields:
                matches = (memory.disp == field["offset"] if ins.mnemonic == "lea" else
                           memory.disp <= field["offset"] < memory.disp + operand.size)
                if matches:
                    access = ("address" if ins.mnemonic == "lea" else
                              ("read+write" if operand.access == (capstone.CS_AC_READ | capstone.CS_AC_WRITE) else
                               "write" if operand.access & capstone.CS_AC_WRITE else
                               "read" if operand.access & capstone.CS_AC_READ else "unknown"))
                    candidates.append(dict(field=field, site=ins.address - base, access=access,
                                           instruction=f"{ins.mnemonic} {ins.op_str}",
                                           base=ins.reg_name(memory.base), width=operand.size))
    return candidates


def report(index, selected, fields, pe, decoder):
    methods = index["methods"]
    owners = {m["type"] for m in methods if m["token"] in selected} | {f["type"] for f in fields}
    for owner in list(owners):
        parent = index["types"][owner]["parent"]
        while parent in index["types"] and parent not in owners:
            owners.add(parent)
            parent = index["types"][parent]["parent"]
    reasons = {token: ["requested"] for token in selected}
    for m in methods:
        if m["type"] in owners and (LIFECYCLE.search(m["method"]) or TARGET_RULE.search(m["method"])):
            reasons.setdefault(m["token"], []).append("lifecycle/target candidate")
    base = pe.OPTIONAL_HEADER.ImageBase
    decoded, candidates = {}, []
    for m in methods:
        if m["rva"] is None or str(m["rva"]) not in index["graph"]:
            continue
        owner_fields = [f for f in fields if f["type"] == m["type"]]
        if m["token"] not in reasons and not owner_fields:
            continue
        rva = m["rva"]
        if rva not in decoded:
            decoded[rva] = instructions(pe, decoder, rva, index["graph"][str(rva)]["end"])
        hits = field_candidates(decoded[rva], base, owner_fields)
        if hits:
            reasons.setdefault(m["token"], []).append("field-offset candidate")
            candidates.extend(dict(hit, method=m) for hit in hits)
    aliases = defaultdict(list)
    for m in methods:
        if m["rva"] is not None:
            aliases[m["rva"]].append(f"{m['type']}.{m['method']} [{m['token']}]")
    starts = sorted(int(rva) for rva in index["graph"])

    def alias_label(rva):
        labels = aliases[rva]
        suffix = f" / {len(labels) - 3} weitere Aliase (JSON aliases[{rva}])" if len(labels) > 3 else ""
        return " / ".join(labels[:3]) + suffix

    def symbol(rva):
        if rva in aliases:
            return alias_label(rva)
        position = bisect.bisect_right(starts, rva) - 1
        if position >= 0:
            start = starts[position]
            if rva < index["graph"][str(start)]["end"]:
                return f"range candidate: {alias_label(start)} + {hex(rva - start)}"
        return "unresolved native address"

    chosen = [dict(m, reasons=reasons[m["token"]]) for m in methods if m["token"] in reasons]
    chosen.sort(key=lambda m: (m["token"] not in selected, int(m["token"], 16)))
    chosen_rvas = {m["rva"] for m in chosen if m["rva"] is not None}
    incoming = defaultdict(list)
    for source, node in index["graph"].items():
        for edge in node["direct"]:
            position = bisect.bisect_right(starts, edge["target"]) - 1
            target_start = starts[position] if position >= 0 else None
            if (edge["target"] in chosen_rvas or target_start in chosen_rvas and
                    edge["target"] < index["graph"][str(target_start)]["end"]):
                incoming[target_start].append(dict(edge, caller=int(source), caller_symbol=symbol(int(source)),
                                                   target_symbol=symbol(edge["target"])))
    files, evidence = {}, []
    lines = ["# Native Verhaltensanalyse: Replanted", "",
             "Dieser Bericht sammelt statische Belege. Er bestätigt kein Spielverhalten.", "",
             "## Grenzen", "",
             "- Aufrufer: direkte Aufrufe und externe Sprünge aus Assembly-CSharp-Methodenbereichen, einschließlich `jmp`.",
             "- Bereiche enden am nächsten Methodenzeiger oder Sektionsende. Innere Adressen sind nur Zuordnungskandidaten.",
             "- Bereiche können zusätzliche native Hilfsfunktionen enthalten; auch der angegebene Aufrufername ist dann nur ein Kandidat.",
             "- Indirekte Aufrufe, native Funktionen außerhalb dieser Bereiche und eingebettete Kopien bleiben fachlich zu verfolgen.",
             "- Aliasnamen teilen dieselbe Adresse. Automatische Ziel- und Lebenszyklusauswahl beruht auf Methodennamen.",
             "- Die Anzeige begrenzt Aliasnamen auf drei und Listen pro Methode auf 20 Einträge. Das JSON enthält die vollständigen Listen.",
             "- Feldbelege sind Offsetkandidaten ohne Nachweis des Objekttyps. `lea` belegt nur eine Adresse.",
             "- Feldsuche umfasst Methoden des deklarierten Feldbesitzers. Fremde Schreiber und Registeralias müssen manuell geprüft werden.",
             "- Fehlende Treffer beweisen weder unbenutzte Helfer noch freie Felder oder fehlende Speicherung.", "",
             "## Abdeckung", "",
             f"{len(index['graph'])} eindeutige Bereiche; {len(index['missing'])} Methoden ohne analysierbaren Bereich.",
             f"{sum(node['decoded_end'] < node['end'] for node in index['graph'].values())} Bereiche mit vorzeitigem Decoderende.", ""]
    lines += ["## Speicherung und Feldkandidaten", ""]
    for field in fields:
        lines += [f"### {field['type']}.{field['field']} bei {hex(field['offset'])}", ""]
        hits = [c for c in candidates if c["field"] == field]
        lines += [f"- {c['method']['method']} [{c['method']['token']}] `{hex(c['site'])}` {c['access']}: `{c['instruction']}`."
                  for c in hits] or ["- Keine Offsetkandidaten im Suchbereich gefunden."]
        lines += [""]

    def display(items, empty):
        shown = items[:20] or [empty]
        if len(items) > 20:
            shown += [f"- Weitere {len(items) - 20} Einträge vollständig in native-analysis.json."]
        return shown

    for m in chosen:
        rva = m["rva"]
        lines += [f"## {m['type']}.{m['method']} [{m['token']}]", "", ", ".join(m["reasons"]), ""]
        if rva not in decoded:
            lines += ["Kein analysierbarer Methodenbereich; kein negativer Verhaltensnachweis.", ""]
            continue
        node = index["graph"][str(rva)]
        filename = f"replanted-{m['token']}.asm"
        files[filename] = "\n".join(f"{ins.address-base:08x}: {ins.mnemonic} {ins.op_str}"
                                     for ins in decoded[rva]) + "\n"
        record = dict(m, assembly=filename, callers=incoming[rva], **node)
        evidence.append(record)
        lines += [f"RVA `{hex(rva)}`; Bereichsende `{hex(node['end'])}`; Beleg [{filename}]({filename}).", "",
                  "Aufrufer:", ""]
        lines += display([f"- `{hex(e['site'])}` `{e['instruction']}`: {e['caller_symbol']} nach `{hex(e['target'])}` ({e['target_symbol']})."
                          for e in incoming[rva]], "- Keine direkten Aufrufer im analysierten Bereich gefunden.")
        lines += ["", "Ziele:", ""]
        lines += display([f"- `{hex(e['site'])}` `{e['instruction']}` nach `{hex(e['target'])}`: {symbol(e['target'])}."
                          for e in node["direct"]], "- Keine direkten externen Ziele gefunden.")
        lines += ["", "Indirekte Aufrufe und Sprünge:", ""]
        lines += display([f"- `{hex(e['site'])}`: `{e['instruction']} {e['operand']}`; Ziel offen."
                          for e in node["indirect"]], "- Keine im dekodierten Bereich gefunden.")
        if node["decoded_end"] < node["end"]:
            lines += [f"", f"Decoder endet vorzeitig bei `{hex(node['decoded_end'])}`. Rest manuell untersuchen."]
        lines += [""]
    lines += ["## Fachliche Übergabe", "",
              "1. Verfolge die relevanten Aufrufer und Ziele bis zum tatsächlichen Zustandsschreiber. Wähle weitere Methoden mit `--method`.",
              "2. Prüfe Zielregeln: Reihen, Entfernung, Flugzustand, Freund/Feind, Tod und Terrain an den konkreten Zweigen.",
              "3. Belege je Feld Objekttyp, Initialisierung, Schreiber, Serialize/Deserialize, Reset und Pool-Wiederverwendung.",
              "4. Prüfe Boardwechsel, DataID, Pause, Abbruch, Tod und Aufräumen. Dokumentiere nicht gespeicherte Zustände.",
              "5. Ergänze fachliche Ergebnisse in einer separaten port-notes.md; dieser Bericht wird beim erneuten Aufruf ersetzt.", ""]
    return files, dict(methods=evidence, fields=fields, field_candidates=candidates, aliases=dict(aliases),
                       missing=index["missing"]), "\n".join(lines)


def research(args):
    references = (PROJECT / "references").resolve()
    output = PROJECT / "references" / args.slug
    if not re.fullmatch(r"[a-z0-9][a-z0-9-]*", args.slug) or not output.resolve().is_relative_to(references):
        raise ValueError("Output slug must stay below references and use lowercase letters, digits and hyphens")
    if not args.method and not args.field:
        raise ValueError("Select at least one --method or --field")
    output.mkdir(parents=True, exist_ok=True)
    with (output / ".native-research.lock").open("a") as lock:
        fcntl.flock(lock, fcntl.LOCK_EX | fcntl.LOCK_NB)
        archive(args, output)


def archive(args, output):
    initial = fingerprints(args.game_dir)
    decoder = capstone.Cs(capstone.CS_ARCH_X86, capstone.CS_MODE_64)
    decoder.detail = True
    with pefile.PE(str(args.game_dir / "GameAssembly.dll"), fast_load=True) as pe:
        if pe.FILE_HEADER.Machine != 0x8664:
            raise ValueError("Expected x64 Replanted DLL")
        index = native_index(args.game_dir, pe, decoder, initial, args.refresh)
        selected = select_methods(args.method, index)
        fields, field_inputs = select_fields(args.field, index, args.dummy_dir)
        files, evidence, markdown = report(index, selected, fields, pe, decoder)
    initial.update(field_inputs)
    if any(sha256(Path(path)) != digest for path, digest in initial.items()):
        raise ValueError("Native inputs changed during research; existing evidence retained")
    marker = output / "native-inputs.json"
    # Prepare everything before invalidating previous evidence. Replacements do not
    # follow an old evidence-file symlink. The per-slug lock prevents mixed exports.
    with tempfile.TemporaryDirectory(dir=output, prefix=".native-") as staging:
        staged = Path(staging)
        for name, content in dict(files, **{"native-report.md": markdown}).items():
            (staged / name).write_text(content)
        atomic_json(staged / "native-analysis.json", evidence)
        marker.unlink(missing_ok=True)
        for path in sorted(staged.iterdir()):
            os.replace(path, output / path.name)
    if any(sha256(Path(path)) != digest for path, digest in initial.items()):
        raise ValueError("Native inputs changed during export; no success marker published")
    atomic_json(marker, dict(sha256=initial, methods=args.method, fields=args.field,
                            dependencies=index["dependencies"],
                            outputs={p.name: sha256(p) for p in
                                     [output / name for name in files] +
                                     [output / "native-report.md", output / "native-analysis.json"]},
                            game_started=False, runtime_tests=False))
    print(f"Archived {len(evidence['methods'])} methods, {len(evidence['field_candidates'])} field candidates: {output}")


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--slug", required=True, help="Output directory below references")
    parser.add_argument("--method", action="append", default=[], help="Type.Method (all overloads) or exact 0x06 token")
    parser.add_argument("--field", action="append", default=[], help="Declared Type.Field; candidates, not type proofs")
    parser.add_argument("--game-dir", type=Path, default=GAME)
    parser.add_argument("--dummy-dir", type=Path, default=DUMMY)
    parser.add_argument("--refresh", action="store_true", help="Rebuild the checksum-validated call index")
    args = parser.parse_args(argv)
    try:
        research(args)
    except (ValueError, KeyError, IndexError, OSError, struct.error, pefile.PEFormatError) as error:
        parser.exit(1, f"ERROR: {error}\n")


if __name__ == "__main__":
    main()
