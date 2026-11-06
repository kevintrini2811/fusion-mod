"""Read signed enum defaults from the authoritative v31 metadata, without game execution."""
import json
from pathlib import Path
import struct
import sys
import hashlib

ROOT = Path(__file__).resolve().parents[2]
SOURCE = ROOT.parent / "pvzfusion/english-4.0.5.2/Game Files/PlantsVsZombiesRH_Data/il2cpp_data/Metadata/global-metadata.dat"
data = SOURCE.read_bytes()
assert struct.unpack_from("<II", data) == (0xFAB11BAF, 31)
strings = struct.unpack_from("<I", data, 24)[0]
fields = struct.unpack_from("<I", data, 96)[0]
types, type_bytes = struct.unpack_from("<II", data, 160)
defaults, default_bytes = struct.unpack_from("<II", data, 64)
values = struct.unpack_from("<I", data, 72)[0]

def text(index):
    start = strings + index
    return data[start:data.index(b"\0", start)].decode()

def signed_constant(index):
    pos = values + index
    first = data[pos]
    if first < 0x80:
        unsigned, count = first, 1
    elif first < 0xC0:
        unsigned, count = ((first & 0x3F) << 8) | data[pos + 1], 2
    elif first < 0xE0:
        unsigned, count = ((first & 0x1F) << 24) | (data[pos+1] << 16) | (data[pos+2] << 8) | data[pos+3], 4
    elif first == 0xF0:
        unsigned, count = struct.unpack_from("<I", data, pos+1)[0], 5
    elif first == 0xFE:
        unsigned, count = 0xFFFFFFFE, 1
    elif first == 0xFF:
        unsigned, count = 0xFFFFFFFF, 1
    else:
        raise ValueError("Invalid compressed metadata constant")
    return (unsigned >> 1) ^ -(unsigned & 1), data[pos:pos+count].hex()

name = sys.argv[1]
selected = None
for i in range(type_bytes // 88):
    offset = types + i*88
    n, ns = struct.unpack_from("<II", data, offset)
    if text(n) == name and not text(ns):
        if selected is not None:
            raise ValueError("Ambiguous enum type")
        selected = offset
if selected is None:
    raise ValueError("Enum type absent")
start = struct.unpack_from("<i", data, selected+32)[0]
count = struct.unpack_from("<H", data, selected+68)[0]
defaults = {struct.unpack_from("<i", data, defaults+i)[0]: struct.unpack_from("<iii", data, defaults+i)
            for i in range(0, default_bytes, 12)}
rows = []
for index in range(start, start+count):
    if index not in defaults:
        continue
    _, type_index, value_index = defaults[index]
    value, raw = signed_constant(value_index)
    rows.append(dict(name=text(struct.unpack_from("<I", data, fields+index*12)[0]), value=value,
                     field_index=index, type_index=type_index, data_index=value_index, compressed_bytes=raw))
print(json.dumps(dict(type=name, source=str(SOURCE), sha256=hashlib.sha256(data).hexdigest(), fields=rows), indent=2))
