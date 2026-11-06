00496cd0: mov qword ptr [rsp + 8], rbx
00496cd5: mov qword ptr [rsp + 0x18], rsi
00496cda: push rdi
00496cdb: sub rsp, 0x40
00496cdf: cmp byte ptr [rip + 0x3102514], 0
00496ce6: mov rdi, rdx
00496ce9: mov rbx, rcx
00496cec: jne 0x180496d0d
00496cee: lea rcx, [rip + 0x2f2081b]
00496cf5: call 0x1803633b0
00496cfa: lea rcx, [rip + 0x2f44e97]
00496d01: call 0x1803633b0
00496d06: mov byte ptr [rip + 0x31024ed], 1
00496d0d: xor esi, esi
00496d0f: mov qword ptr [rsp + 0x58], rsi
00496d14: test rdi, rdi
00496d17: je 0x180496ea3
00496d1d: mov r8, qword ptr [rip + 0x2f207ec]
00496d24: lea rdx, [rsp + 0x58]
00496d29: mov rcx, rdi
00496d2c: call 0x180ac9930
00496d31: test al, al
00496d33: je 0x180496e29
00496d39: mov rax, qword ptr [rsp + 0x58]
00496d3e: test rax, rax
00496d41: je 0x180496ea3
00496d47: mov rcx, qword ptr [rip + 0x2f44e4a]
00496d4e: mov rdi, qword ptr [rax + 0x88]
00496d55: cmp dword ptr [rcx + 0xe0], esi
00496d5b: jne 0x180496d62
00496d5d: call 0x180318580
00496d62: xor r8d, r8d
00496d65: mov rdx, rbx
00496d68: mov rcx, rdi
00496d6b: call 0x1826b9a40
00496d70: test al, al
00496d72: jne 0x180496e29
00496d78: mov rdx, qword ptr [rsp + 0x58]
00496d7d: test rdx, rdx
00496d80: je 0x180496ea3
00496d86: mov eax, dword ptr [rbx + 0x20]
00496d89: cmp dword ptr [rdx + 0xe0], eax
00496d8f: jne 0x180496e29
00496d95: cmp dword ptr [rdx + 0xc8], 5
00496d9c: je 0x180496dac
00496d9e: mov eax, dword ptr [rbx + 0xf8]
00496da4: cmp dword ptr [rdx + 0xc0], eax
00496daa: jne 0x180496e29
00496dac: mov eax, dword ptr [rdx + 0x54]
00496daf: cmp eax, 2
00496db2: jne 0x180496e39
00496db8: mov rax, qword ptr [rbx + 0x30]
00496dbc: test rax, rax
00496dbf: je 0x180496ea3
00496dc5: mov rcx, qword ptr [rax + 0x48]
00496dc9: test rcx, rcx
00496dcc: je 0x180496ea3
00496dd2: mov r9d, dword ptr [rdx + 0xdc]
00496dd9: mov r8, rbx
00496ddc: mov qword ptr [rsp + 0x30], rsi
00496de1: mov byte ptr [rsp + 0x28], 1
00496de6: mov dword ptr [rsp + 0x20], 0xe8
00496dee: call 0x180420810
00496df3: mov rcx, qword ptr [rsp + 0x58]
00496df8: test rcx, rcx
00496dfb: je 0x180496ea3
00496e01: test rax, rax
00496e04: je 0x180496ea3
00496e0a: mov ecx, dword ptr [rcx + 0xe8]
00496e10: mov dword ptr [rax + 0xe8], ecx
00496e16: mov eax, dword ptr [rbx + 0x248]
00496e1c: inc eax
00496e1e: mov dword ptr [rbx + 0x248], eax
00496e24: cmp eax, 0x64
00496e27: jg 0x180496e83
00496e29: mov rbx, qword ptr [rsp + 0x50]
00496e2e: mov rsi, qword ptr [rsp + 0x60]
00496e33: add rsp, 0x40
00496e37: pop rdi
00496e38: ret
00496e39: cmp eax, 0xa
00496e3c: jne 0x180496e29
00496e3e: mov rax, qword ptr [rbx + 0x30]
00496e42: test rax, rax
00496e45: je 0x180496ea3
00496e47: mov rcx, qword ptr [rax + 0x48]
00496e4b: test rcx, rcx
00496e4e: je 0x180496ea3
00496e50: mov qword ptr [rsp + 0x30], rsi
00496e55: mov r9d, 0x14
00496e5b: mov byte ptr [rsp + 0x28], 1
00496e60: mov r8, rbx
00496e63: mov dword ptr [rsp + 0x20], 0x32
00496e6b: call 0x180420810
00496e70: mov eax, dword ptr [rbx + 0x248]
00496e76: inc eax
00496e78: mov dword ptr [rbx + 0x248], eax
00496e7e: cmp eax, 0x64
00496e81: jle 0x180496e29
00496e83: xor edx, edx
00496e85: mov dword ptr [rbx + 0x248], esi
00496e8b: mov rcx, rbx
00496e8e: call 0x180496eb0
00496e93: mov rbx, qword ptr [rsp + 0x50]
00496e98: mov rsi, qword ptr [rsp + 0x60]
00496e9d: add rsp, 0x40
00496ea1: pop rdi
00496ea2: ret
00496ea3: call 0x180363610
00496ea8: int3
00496ea9: int3
00496eaa: int3
00496eab: int3
00496eac: int3
00496ead: int3
00496eae: int3
00496eaf: int3
