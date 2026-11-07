006b4e30: mov qword ptr [rsp + 8], rbx
006b4e35: push rdi
006b4e36: sub rsp, 0x20
006b4e3a: cmp byte ptr [rip + 0x4836830], 0
006b4e41: mov rbx, rcx
006b4e44: jne 0x1806b4e59
006b4e46: lea rcx, [rip + 0x457862b]
006b4e4d: call 0x1804feb70
006b4e52: mov byte ptr [rip + 0x4836818], 1
006b4e59: mov rcx, qword ptr [rip + 0x4578618]
006b4e60: mov rdi, qword ptr [rbx + 0x10]
006b4e64: cmp dword ptr [rcx + 0xe4], 0
006b4e6b: jne 0x1806b4e72
006b4e6d: call 0x1804feeb0
006b4e72: xor r8d, r8d
006b4e75: xor edx, edx
006b4e77: mov rcx, rdi
006b4e7a: call 0x183832770
006b4e7f: test al, al
006b4e81: jne 0x1806b4f30
006b4e87: mov rcx, qword ptr [rbx + 0x10]
006b4e8b: test rcx, rcx
006b4e8e: je 0x1806b4f3b
006b4e94: mov edx, dword ptr [rbx + 0xd8]
006b4e9a: xor r8d, r8d
006b4e9d: call 0x18060f880
006b4ea2: mov rcx, qword ptr [rbx + 0x10]
006b4ea6: test rcx, rcx
006b4ea9: je 0x1806b4f3b
006b4eaf: mov edx, dword ptr [rbx + 0xe0]
006b4eb5: xor r8d, r8d
006b4eb8: call 0x1807e14a0
006b4ebd: mov rcx, qword ptr [rbx + 0x10]
006b4ec1: test rcx, rcx
006b4ec4: je 0x1806b4f3b
006b4ec6: mov edx, dword ptr [rbx + 0xe4]
006b4ecc: xor r8d, r8d
006b4ecf: call 0x1807e14a0
006b4ed4: mov rcx, qword ptr [rbx + 0x10]
006b4ed8: test rcx, rcx
006b4edb: je 0x1806b4f3b
006b4edd: mov edx, dword ptr [rbx + 0xe8]
006b4ee3: xor r8d, r8d
006b4ee6: call 0x1807e14a0
006b4eeb: mov rcx, qword ptr [rbx + 0x10]
006b4eef: test rcx, rcx
006b4ef2: je 0x1806b4f3b
006b4ef4: mov edx, dword ptr [rbx + 0xec]
006b4efa: xor r8d, r8d
006b4efd: call 0x1807e14a0
006b4f02: mov rcx, qword ptr [rbx + 0x10]
006b4f06: test rcx, rcx
006b4f09: je 0x1806b4f3b
006b4f0b: mov edx, dword ptr [rbx + 0xf0]
006b4f11: xor r8d, r8d
006b4f14: call 0x1807e14a0
006b4f19: mov rcx, qword ptr [rbx + 0x10]
006b4f1d: test rcx, rcx
006b4f20: je 0x1806b4f3b
006b4f22: mov edx, dword ptr [rbx + 0xf8]
006b4f28: xor r8d, r8d
006b4f2b: call 0x1807e14a0
006b4f30: mov rbx, qword ptr [rsp + 0x30]
006b4f35: add rsp, 0x20
006b4f39: pop rdi
006b4f3a: ret
006b4f3b: call 0x1804fee20
006b4f40: int3
006b4f41: int3
006b4f42: int3
006b4f43: int3
006b4f44: int3
006b4f45: int3
006b4f46: int3
006b4f47: int3
006b4f48: int3
006b4f49: int3
006b4f4a: int3
006b4f4b: int3
006b4f4c: int3
006b4f4d: int3
006b4f4e: int3
006b4f4f: int3
