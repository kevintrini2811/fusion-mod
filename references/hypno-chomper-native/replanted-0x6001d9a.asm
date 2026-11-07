006f5af0: push rbx
006f5af2: sub rsp, 0x20
006f5af6: cmp byte ptr [rip + 0x47f5d28], 0
006f5afd: mov rbx, rcx
006f5b00: jne 0x1806f5b15
006f5b02: lea rcx, [rip + 0x45a29b7]
006f5b09: call 0x1804feb70
006f5b0e: mov byte ptr [rip + 0x47f5d10], 1
006f5b15: cmp dword ptr [rbx + 0x70], 0x10
006f5b19: mov dword ptr [rbx + 0x108], 0
006f5b23: jne 0x1806f5b32
006f5b25: xor r8d, r8d
006f5b28: mov dl, 1
006f5b2a: mov rcx, rbx
006f5b2d: call 0x1806e6750
006f5b32: cmp dword ptr [rbx + 0x70], 0x1a
006f5b36: mov qword ptr [rsp + 0x30], rdi
006f5b3b: je 0x1806f5b5f
006f5b3d: cmp dword ptr [rbx + 0x70], 0x1b
006f5b41: je 0x1806f5b5f
006f5b43: cmp dword ptr [rbx + 0x70], 0x1f
006f5b47: je 0x1806f5b5f
006f5b49: cmp dword ptr [rbx + 0x70], 0x1c
006f5b4d: je 0x1806f5b5f
006f5b4f: cmp dword ptr [rbx + 0x70], 0x1d
006f5b53: je 0x1806f5b5f
006f5b55: cmp dword ptr [rbx + 0x70], 0x1e
006f5b59: jne 0x1806f5c0b
006f5b5f: mov rcx, qword ptr [rbx + 0x10]
006f5b63: test rcx, rcx
006f5b66: je 0x1806f5c5a
006f5b6c: mov edx, dword ptr [rbx + 0x1a8]
006f5b72: xor r8d, r8d
006f5b75: call 0x1807e1430
006f5b7a: mov rdi, rax
006f5b7d: test rax, rax
006f5b80: je 0x1806f5c0b
006f5b86: cmp dword ptr [rbx + 0x70], 0x1a
006f5b8a: jne 0x1806f5bbf
006f5b8c: mov rcx, qword ptr [rbx + 0x1d0]
006f5b93: test rcx, rcx
006f5b96: je 0x1806f5c5a
006f5b9c: mov rdx, qword ptr [rip + 0x45a291d]
006f5ba3: xor r9d, r9d
006f5ba6: mov r8d, 5
006f5bac: call 0x1807c7e10
006f5bb1: test al, al
006f5bb3: je 0x1806f5bbf
006f5bb5: movss xmm1, dword ptr [rip + 0x353e337]
006f5bbd: jmp 0x1806f5c00
006f5bbf: cmp dword ptr [rbx + 0x70], 0x1d
006f5bc3: jne 0x1806f5bf8
006f5bc5: mov rcx, qword ptr [rbx + 0x1d0]
006f5bcc: test rcx, rcx
006f5bcf: je 0x1806f5c5a
006f5bd5: mov rdx, qword ptr [rip + 0x45a28e4]
006f5bdc: xor r9d, r9d
006f5bdf: mov r8d, 5
006f5be5: call 0x1807c7e10
006f5bea: test al, al
006f5bec: je 0x1806f5bf8
006f5bee: movss xmm1, dword ptr [rip + 0x353e3de]
006f5bf6: jmp 0x1806f5c00
006f5bf8: movss xmm1, dword ptr [rip + 0x353e1b8]
006f5c00: xor r8d, r8d
006f5c03: mov rcx, rdi
006f5c06: call 0x1806caee0
006f5c0b: cmp qword ptr [rbx + 0x1a0], 0
006f5c13: je 0x1806f5c3c
006f5c15: mov rcx, qword ptr [rbx + 0x1a0]
006f5c1c: xor edx, edx
006f5c1e: call 0x1806ca910
006f5c23: lea rcx, [rbx + 0x1a0]
006f5c2a: mov qword ptr [rbx + 0x1a0], 0
006f5c35: xor edx, edx
006f5c37: call 0x1804fdea0
006f5c3c: xor edx, edx
006f5c3e: mov rcx, rbx
006f5c41: call 0x1806fbf10
006f5c46: xor edx, edx
006f5c48: mov rcx, rbx
006f5c4b: mov rdi, qword ptr [rsp + 0x30]
006f5c50: add rsp, 0x20
006f5c54: pop rbx
006f5c55: jmp 0x1806f7c70
006f5c5a: call 0x1804fee20
006f5c5f: int3
