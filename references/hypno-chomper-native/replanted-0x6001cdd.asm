006f1bd0: mov qword ptr [rsp + 0x10], rbp
006f1bd5: mov qword ptr [rsp + 0x18], rsi
006f1bda: push rdi
006f1bdb: sub rsp, 0x50
006f1bdf: cmp byte ptr [rip + 0x47f9bc8], 0
006f1be6: mov ebp, edx
006f1be8: mov rdi, rcx
006f1beb: jne 0x1806f1c18
006f1bed: lea rcx, [rip + 0x45393cc]
006f1bf4: call 0x1804feb70
006f1bf9: lea rcx, [rip + 0x458d9f0]
006f1c00: call 0x1804feb70
006f1c05: lea rcx, [rip + 0x458dabc]
006f1c0c: call 0x1804feb70
006f1c11: mov byte ptr [rip + 0x47f9b96], 1
006f1c18: xorps xmm0, xmm0
006f1c1b: mov qword ptr [rsp + 0x60], rbx
006f1c20: xorps xmm1, xmm1
006f1c23: lea rcx, [rsp + 0x40]
006f1c28: xor r8d, r8d
006f1c2b: mov rdx, rdi
006f1c2e: movups xmmword ptr [rsp + 0x30], xmm0
006f1c33: movups xmmword ptr [rsp + 0x20], xmm1
006f1c38: call 0x1806f2c90
006f1c3d: xor esi, esi
006f1c3f: mov edx, esi
006f1c41: movups xmm0, xmmword ptr [rax]
006f1c44: mov rax, qword ptr [rdi + 0x18]
006f1c48: movups xmmword ptr [rsp + 0x30], xmm0
006f1c4d: test rax, rax
006f1c50: je 0x1806f1d39
006f1c56: nop word ptr [rax + rax]
006f1c60: mov rcx, qword ptr [rax + 0x78]
006f1c64: test rcx, rcx
006f1c67: je 0x1806f1d39
006f1c6d: cmp edx, dword ptr [rcx + 0x28]
006f1c70: jge 0x1806f1d35
006f1c76: mov rax, qword ptr [rdi + 0x18]
006f1c7a: test rax, rax
006f1c7d: je 0x1806f1d39
006f1c83: mov rcx, qword ptr [rax + 0x78]
006f1c87: test rcx, rcx
006f1c8a: je 0x1806f1d39
006f1c90: mov r8, qword ptr [rip + 0x458da31]
006f1c97: mov edx, esi
006f1c99: call 0x182449330
006f1c9e: mov rbx, rax
006f1ca1: test rax, rax
006f1ca4: je 0x1806f1d39
006f1caa: mov ecx, dword ptr [rax + 0x34]
006f1cad: cmp dword ptr [rdi + 0x34], ecx
006f1cb0: jne 0x1806f1d0b
006f1cb2: xor r8d, r8d
006f1cb5: lea rcx, [rsp + 0x40]
006f1cba: mov rdx, rax
006f1cbd: call 0x1806b1a00
006f1cc2: mov rcx, qword ptr [rip + 0x45392f7]
006f1cc9: movups xmm0, xmmword ptr [rax]
006f1ccc: movups xmmword ptr [rsp + 0x20], xmm0
006f1cd1: cmp dword ptr [rcx + 0xe4], 0
006f1cd8: jne 0x1806f1cdf
006f1cda: call 0x1804feeb0
006f1cdf: xor r8d, r8d
006f1ce2: lea rdx, [rsp + 0x20]
006f1ce7: lea rcx, [rsp + 0x30]
006f1cec: call 0x1806cbaa0
006f1cf1: cmp eax, 0x14
006f1cf4: jl 0x1806f1d0b
006f1cf6: xor r9d, r9d
006f1cf9: mov r8d, ebp
006f1cfc: mov rdx, rbx
006f1cff: mov rcx, rdi
006f1d02: call 0x1806e9710
006f1d07: test al, al
006f1d09: jne 0x1806f1d1d
006f1d0b: mov rax, qword ptr [rdi + 0x18]
006f1d0f: inc esi
006f1d11: mov edx, esi
006f1d13: test rax, rax
006f1d16: je 0x1806f1d39
006f1d18: jmp 0x1806f1c60
006f1d1d: mov rax, rbx
006f1d20: mov rbx, qword ptr [rsp + 0x60]
006f1d25: mov rbp, qword ptr [rsp + 0x68]
006f1d2a: mov rsi, qword ptr [rsp + 0x70]
006f1d2f: add rsp, 0x50
006f1d33: pop rdi
006f1d34: ret
006f1d35: xor eax, eax
006f1d37: jmp 0x1806f1d20
006f1d39: call 0x1804fee20
006f1d3e: int3
006f1d3f: int3
