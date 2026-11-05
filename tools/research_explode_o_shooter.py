"""Archive static behavior evidence from the installed Fusion 4.0.5.2 source.

Requires pefile and capstone. Never loads or executes either game's DLL.
Metadata table offsets and the CodeGenModule address are specific to this build.
"""
import json
import struct
import bisect
from pathlib import Path

import capstone
import pefile

PROJECT = Path(__file__).resolve().parents[1]
SOURCE = PROJECT.parent / "pvzfusion/english-4.0.5.2/Game Files"
OUTPUT = PROJECT / "references/explode-o-shooter"
metadata = (SOURCE / "PlantsVsZombiesRH_Data/il2cpp_data/Metadata/global-metadata.dat").read_bytes()
assert struct.unpack_from("<II", metadata) == (0xFAB11BAF, 31)
strings = struct.unpack_from("<I", metadata, 24)[0]
methods, methods_size = struct.unpack_from("<II", metadata, 48)
types, types_size = struct.unpack_from("<II", metadata, 160)
assert methods_size % 36 == 0 and types_size % 88 == 0


def string(index):
    start = strings + index
    return metadata[start:metadata.index(b"\0", start)].decode()


pe = pefile.PE(str(SOURCE / "GameAssembly.dll"))
base = pe.OPTIONAL_HEADER.ImageBase
decoder = capstone.Cs(capstone.CS_ARCH_X86, capstone.CS_MODE_64)
module_pointer_offset = pe.__data__.find(struct.pack("<Q", base + 0x331D670))
assert module_pointer_offset >= 8
method_count = struct.unpack_from("<Q", pe.__data__, module_pointer_offset - 8)[0]
assert 10000 < method_count < 200000
pointers = struct.unpack(f"<{method_count}Q", pe.get_data(0x331D670, method_count * 8))
starts = sorted(set(pointer - base for pointer in pointers if pointer))
OUTPUT.mkdir(parents=True, exist_ok=True)
evidence = []
for index in range(types_size // 88):
    offset = types + index * 88
    name = string(struct.unpack_from("<I", metadata, offset)[0])
    if name not in ("SuperCherryShooter", "Bullet_superCherry", "Shooter", "BombCherry"):
        continue
    start = struct.unpack_from("<i", metadata, offset + 36)[0]
    count = struct.unpack_from("<H", metadata, offset + 64)[0]
    for method in range(start, start + count):
        offset = methods + method * 36
        name_index, declaring_type = struct.unpack_from("<Ii", metadata, offset)
        assert declaring_type == index
        token = struct.unpack_from("<I", metadata, offset + 24)[0]
        method_name = string(name_index)
        pointer = struct.unpack("<Q", pe.get_data(0x331D670 + ((token & 0xFFFFFF) - 1) * 8, 8))[0]
        rva = pointer - base
        # Bound by the next actual method pointer. PE unwind entries may split
        # one method into multiple regions; first-ret extraction also truncates
        # alternate paths. This interval may include alignment or native helpers.
        end = starts[bisect.bisect_right(starts, rva)]
        assert 0 < end - rva < 0x10000
        text = "\n".join(f"{i.address-base:08x}: {i.mnemonic} {i.op_str}"
                         for i in decoder.disasm(pe.get_data(rva, end-rva), pointer)) + "\n"
        (OUTPUT / f"fusion-{name}-{method_name.replace('.', '_')}.asm").write_text(text)
        evidence.append(dict(type=name, method=method_name, token=hex(token), rva=hex(rva), end=hex(end)))
(OUTPUT / "method-addresses.json").write_text(json.dumps(evidence, indent=2) + "\n")
print(f"Archived {len(evidence)} methods from actual metadata and PE function ranges.")

# Installed Replanted module and wrapper tokens, audited for this game build.
# Decode only actual method intervals; no game code is loaded or executed.
GAME = Path("/mnt/FastGaming1/SteamLibrary/steamapps/common/PVZ Replanted")
native = pefile.PE(str(GAME / "GameAssembly.dll"))
native_base = native.OPTIONAL_HEADER.ImageBase
count, table = struct.unpack_from("<QQ", native.__data__, native.get_offset_from_rva(0x3dab750) + 8)
assert 1000 < count < 1000000
native_pointers = struct.unpack(f"<{count}Q", native.get_data(table - native_base, count * 8))
native_starts = sorted(set(p - native_base for p in native_pointers if p))
for name, token in (("mouse-plant", 100669173), ("seed-wasplanted", 100670396),
                    ("seed-update", 100670383), ("plant-initialize", 100669966),
                    ("radius", 100669282)):
    rva = native_pointers[(token & 0xFFFFFF) - 1] - native_base
    end = native_starts[bisect.bisect_right(native_starts, rva)]
    assert 0 < end - rva < 0x10000
    (OUTPUT / f"replanted-{name}.asm").write_text("\n".join(
        f"{i.address-native_base:08x}: {i.mnemonic} {i.op_str}"
        for i in decoder.disasm(native.get_data(rva, end-rva), native_base+rva)) + "\n")
print("Archived native planting, recharge, shooter initialization and explosion behavior.")
