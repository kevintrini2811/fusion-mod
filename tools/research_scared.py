"""Static Scared 1.0.1 research. Never loads or executes native game code.

Decode the shipped metadata cipher and evaluate only the generated arithmetic
cipher used for constants. Inputs must match the archived source fingerprints.
"""
import argparse
import bisect
from collections import defaultdict
import json
from pathlib import Path
import re
import struct
import capstone
import pefile
import UnityPy
from source_index import PROJECT, atomic_json, sha256
from scared_content import verify_source

parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument("--source", type=Path, default=Path("/mnt/NFS/Multimedia/Downloads"))
args = parser.parse_args()
source = args.source
output = PROJECT / "references/scared-2-12-0"
expected = verify_source(source)
raw = (source / "PVZ-Scared_Data/il2cpp_data/Metadata/global-metadata.dat").read_bytes()
p = pefile.PE(str(source / "GameAssembly.dll"), fast_load=True)
base = p.OPTIONAL_HEADER.ImageBase
d = capstone.Cs(capstone.CS_ARCH_X86, capstone.CS_MODE_64)
d.detail = True
box = p.get_data(0x25895a0, 256)

def decrypt_metadata(data, state):
    result = bytearray(data)
    for i in range(len(result)):
        state ^= (state << 13) & 0xffffffffffffffff
        state ^= state >> 7
        state ^= (state << 17) & 0xffffffffffffffff
        value = state
        for _ in range(3):
            value = (value >> 8) ^ state
        result[i] ^= box[value & 255]
    return bytes(result)

header = decrypt_metadata(raw[:0x208], 0x2a2c0ee099d43b6f)
def h(offset):
    return struct.unpack_from("<I", header, offset)[0]

strings = decrypt_metadata(raw[h(0xfc)-0x9f0:h(0xfc)-0x9f0+h(0x1d4)], 0x36da2449fd5c3f2c)
methods = decrypt_metadata(raw[h(0xd8)+0xcf0:h(0xd8)+0xcf0+h(0x168)], 0xf044852879de6812)
fields = decrypt_metadata(raw[h(0x1c0)-0x6a0:h(0x1c0)-0x6a0+h(0x1c)], 0x41cb4755911725af)
type_offset = h(0x94)-0x17b0
types = raw[type_offset:type_offset+h(0x68)]
assert len(types) % 88 == len(methods) % 36 == len(fields) % 12 == 0

def st(index):
    return strings[index:strings.index(b"\0", index)].decode()

module = struct.unpack("<3Q", p.get_data(0x20c2a90, 24))
assert p.get_data(module[0]-base, 20).split(b"\0")[0] == b"Assembly-CSharp.dll"
assert module[1] == 6317
pointers = struct.unpack("<6317Q", p.get_data(module[2]-base, 6317*8))
ends = sorted({value-base for value in pointers if value > base})
records = []
for index in range(1717, 2411):
    at = index*88
    name = st(struct.unpack_from("<I", types, at)[0])
    first = struct.unpack_from("<i", types, at+36)[0]
    count = struct.unpack_from("<H", types, at+64)[0]
    for method in range(first, first+count):
        name_index, owner = struct.unpack_from("<Ii", methods, method*36)
        token = struct.unpack_from("<I", methods, method*36+24)[0]
        assert owner == index and token >> 24 == 6
        pointer = pointers[(token & 0xffffff)-1]
        records.append(dict(type=name, method=st(name_index), token=hex(token), rva=pointer-base if pointer else 0))
by_address = defaultdict(list)
for record in records:
    by_address[record["rva"]].append(record["type"]+"."+record["method"])
selected = {"EyesZombie", "BloodSunFlower", "SunFlower", "Zombie", "Producer", "$DC", "ObfuzBootstrap", "$nC", "$NC", "$oC"}
for record in records:
    if record["type"] not in selected or record["rva"] <= 0:
        continue
    start = record["rva"]
    end = ends[bisect.bisect_right(ends, start)]
    assert 0 < end-start < 0x20000
    lines = []
    for ins in d.disasm(p.get_data(start, end-start), base+start):
        annotation = ""
        if ins.mnemonic in ("call", "jmp") and ins.operands[0].type == capstone.x86.X86_OP_IMM:
            names = by_address.get(ins.operands[0].imm-base, [])
            annotation = " ; " + " | ".join(names[:3]) if names else ""
        lines.append(f"{ins.address-base:08x}: {ins.mnemonic} {ins.op_str}{annotation}")
    (output / (record["type"]+"-"+record["token"]+".asm")).write_text("\n".join(lines)+"\n")
atomic_json(output / "method-addresses.json", [r for r in records if r["type"] in selected])
rt = {struct.unpack_from("<i", types, i*88+8)[0]: i for i in range(len(types)//88)}

def field_blob(usage_rva, size):
    encoded = struct.unpack("<Q", p.get_data(usage_rva, 8))[0]
    index = (encoded & 0x1fffffff) >> 1
    runtime_type, field = struct.unpack_from("<II", raw, h(0xb8)-0xdf8+index*8)
    field += struct.unpack_from("<i", types, rt[runtime_type]*88+32)[0]
    defaults = raw[h(0xf4)+0x16f8:h(0xf4)+0x16f8+h(0x114)]
    matches = [entry for entry in struct.iter_unpack("<III", defaults) if entry[0] == field]
    assert len(matches) == 1
    at = h(0x140)-0x1a14+matches[0][2]
    return raw[at:at+size]

environment = UnityPy.load(str(source / "PVZ-Scared_Data/resources.assets"))
key_object = next(o for o in environment.objects if o.assets_file.name == "resources.assets" and o.path_id == 3598)
key = key_object.read().m_Script.encode("utf-8", "surrogateescape")
assert len(key) == 1024

from capstone import x86_const as x
memory={0x1010:0x2000,0x2018:len(key)//4};memory.update({0x2020+i:struct.unpack_from("<I",key,i)[0] for i in range(0,len(key),4)})
def vm(value,opcode,salt):
 regs={"rcx":0x1000,"rdx":value,"r8":opcode,"r9":salt,"rsp":0x100000}
 aliases={"eax":"rax","ebx":"rbx","ecx":"rcx","edx":"rdx","r8d":"r8","r9d":"r9","r10d":"r10","r11d":"r11"}
 def rr(reg):return regs.get(aliases.get(reg,reg),0)&(0xffffffff if reg in aliases else 0xffffffffffffffff)
 def addr(m):return rr(d.reg_name(m.base))+rr(d.reg_name(m.index))*m.scale+m.disp
 def read(op):
  if op.type==x.X86_OP_IMM:return op.imm
  if op.type==x.X86_OP_REG:return rr(d.reg_name(op.reg))
  if op.type==x.X86_OP_MEM:return memory[addr(op.mem)]
  raise ValueError(op)
 def put(op,v):
  assert op.type==x.X86_OP_REG
  r=d.reg_name(op.reg);regs[aliases.get(r,r)]=v&((1<<(op.size*8))-1)
 start=struct.unpack("<I",p.get_data(0x5aa0b4+opcode*4,4))[0]
 for ins in d.disasm(p.get_data(start,300),base+start):
  op=ins.operands;mn=ins.mnemonic
  if mn=="ret":return rr("eax")
  if mn in ("test","cmp","je","jne","jbe","ja","nop"):continue
  if op and op[0].type==x.X86_OP_REG and d.reg_name(op[0].reg)=="rsp":continue
  if mn=="mov":put(op[0],read(op[1]))
  elif mn=="lea":put(op[0],addr(op[1].mem))
  elif mn=="imul":put(op[0],read(op[1])*read(op[2]))
  elif mn in ("add","sub","xor","or","and","shl","shr"):
   a,b=read(op[0]),read(op[1]);v={"add":lambda:a+b,"sub":lambda:a-b,"xor":lambda:a^b,"or":lambda:a|b,"and":lambda:a&b,"shl":lambda:a<<b,"shr":lambda:a>>b}[mn]();put(op[0],v)
  else:raise ValueError((hex(ins.address-base),mn,ins.op_str))
 raise ValueError("No return")
def dec(v,ops,salt):
 while ops:v=vm(v,ops&255,salt);ops>>=8
 return v

def block(b,ops,salt):
 out=bytearray();last=0
 for (v,) in struct.iter_unpack("<I",b):out.extend(struct.pack("<I",dec(v^last,ops,salt)));last=v
 return bytes(out)
b=block(field_blob(0x27eb9f8,2048),0x50cd8d8d,0xa43820a3);
constant_report = {}
blood=b;eyes=block(field_blob(0x27ec418,2048),0x3dd9cd45,0x593d4766)
import re
for file in output.glob("*.asm"):
 if not file.name.startswith(("EyesZombie","BloodSunFlower")):continue
 b=eyes if file.name.startswith("Eyes") else blood; text=file.read_text();lines=text.splitlines();constant_report[file.name] = []
 for j,line in enumerate(lines):
  if "call 0x180a28400" not in line and "call 0x180a28740" not in line:continue
  ctx="\n".join(lines[max(0,j-18):j]); vals={}
  for reg,num in re.findall(r"mov (edx|r8d|r9d), (0x[0-9a-f]+|\d+)",ctx):vals[reg]=int(num,0)
  off=vals["edx"];v=struct.unpack_from("<I",b,off)[0];ops=vals["r8d"];salt=vals["r9d"]
  result=struct.unpack("<f",struct.pack("<I",v^(dec(0xabcd,ops,salt)&0x7fffff)))[0] if "a28400" in line else dec(v,ops,salt)
  constant_report[file.name].append(dict(offset=hex(off), ops=hex(ops), salt=hex(salt), value=result))


atomic_json(output / "decoded-constants.json", constant_report)
for object_id, name in [(19762, "BloodSunFlower-PlantData"), (19798, "EyesZombie-ZombieData")]:
    obj = next(o for o in environment.objects if o.assets_file.name == "resources.assets" and o.path_id == object_id)
    (output / (name+".bin")).write_bytes(obj.get_raw_data())
atomic_json(output / "research-inputs.json", dict(sha256=expected, metadata_loader_rva="0x31cbd0",
    metadata_header_bytes=520, source_type_range=[1717, 2411], module_rva="0x20c2a90",
    secret_key_asset=3598, tool_sha256=sha256(Path(__file__)), game_started=False, native_code_executed=False))
print(f"Archived source methods, decoded constants and original data under {output}")
