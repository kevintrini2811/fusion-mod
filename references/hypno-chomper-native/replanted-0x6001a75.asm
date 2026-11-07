006b1c60: push rsi
006b1c62: sub rsp, 0x60
006b1c66: cmp byte ptr [rip + 0x4839a0d], 0
006b1c6d: mov rsi, rcx
006b1c70: movaps xmmword ptr [rsp + 0x50], xmm6
006b1c75: movaps xmmword ptr [rsp + 0x40], xmm7
006b1c7a: movaps xmmword ptr [rsp + 0x30], xmm8
006b1c80: movaps xmmword ptr [rsp + 0x20], xmm9
006b1c86: jne 0x1806b1c9b
006b1c88: lea rcx, [rip + 0x4579331]
006b1c8f: call 0x1804feb70
006b1c94: mov byte ptr [rip + 0x48399df], 1
006b1c9b: movss xmm6, dword ptr [rip + 0x3582115]
006b1ca3: movss xmm8, dword ptr [rip + 0x3582118]
006b1cac: movss xmm7, dword ptr [rip + 0x3582108]
006b1cb4: movss xmm9, dword ptr [rip + 0x358228b]
006b1cbd: mov qword ptr [rsp + 0x70], rbx
006b1cc2: mov qword ptr [rsp + 0x78], rdi
006b1cc7: nop word ptr [rax + rax]
006b1cd0: cmp dword ptr [rsi + 0x70], 0x2d
006b1cd4: mov r9, qword ptr [rsi + 0x118]
006b1cdb: je 0x1806b1cf7
006b1cdd: test r9, r9
006b1ce0: je 0x1806b1e3a
006b1ce6: cmp dword ptr [r9 + 0x18], 0
006b1ceb: jbe 0x1806b1e89
006b1cf1: mov rbx, qword ptr [r9 + 0x20]
006b1cf5: jmp 0x1806b1d69
006b1cf7: xor eax, eax
006b1cf9: lea rdx, [r9 + 0x20]
006b1cfd: xor r8d, r8d
006b1d00: test r9, r9
006b1d03: je 0x1806b1e3a
006b1d09: nop dword ptr [rax]
006b1d10: cmp eax, dword ptr [r9 + 0x18]
006b1d14: jae 0x1806b1e89
006b1d1a: mov rcx, qword ptr [rdx]
006b1d1d: test rcx, rcx
006b1d20: je 0x1806b1e3a
006b1d26: cmp dword ptr [rcx + 0x20], 0
006b1d2a: je 0x1806b1d61
006b1d2c: inc eax
006b1d2e: inc r8
006b1d31: add rdx, 8
006b1d35: cmp r8, 5
006b1d39: jl 0x1806b1d10
006b1d3b: mov rdi, qword ptr [rsp + 0x78]
006b1d40: mov rbx, qword ptr [rsp + 0x70]
006b1d45: movaps xmm6, xmmword ptr [rsp + 0x50]
006b1d4a: movaps xmm7, xmmword ptr [rsp + 0x40]
006b1d4f: movaps xmm8, xmmword ptr [rsp + 0x30]
006b1d55: movaps xmm9, xmmword ptr [rsp + 0x20]
006b1d5b: add rsp, 0x60
006b1d5f: pop rsi
006b1d60: ret
006b1d61: movsxd rcx, eax
006b1d64: mov rbx, qword ptr [r9 + rcx*8 + 0x20]
006b1d69: test rbx, rbx
006b1d6c: je 0x1806b1d3b
006b1d6e: xor edx, edx
006b1d70: mov rcx, rsi
006b1d73: call 0x1806af170
006b1d78: mov rdi, rax
006b1d7b: test rax, rax
006b1d7e: je 0x1806b1d3b
006b1d80: movss xmm0, dword ptr [rax + 0x70]
006b1d85: addss xmm0, xmm6
006b1d89: movss dword ptr [rbx + 0x10], xmm0
006b1d8e: movss xmm1, dword ptr [rax + 0x74]
006b1d93: addss xmm1, xmm6
006b1d97: movss dword ptr [rbx + 0x14], xmm1
006b1d9c: mov rcx, qword ptr [rip + 0x457921d]
006b1da3: cmp dword ptr [rcx + 0xe4], 0
006b1daa: jne 0x1806b1db1
006b1dac: call 0x1804feeb0
006b1db1: xor r8d, r8d
006b1db4: movaps xmm1, xmm8
006b1db8: movaps xmm0, xmm7
006b1dbb: call 0x1806cbd40
006b1dc0: movss dword ptr [rbx + 0x18], xmm0
006b1dc5: xor r8d, r8d
006b1dc8: movaps xmm0, xmm9
006b1dcc: xorps xmm1, xmm1
006b1dcf: call 0x1806cbd40
006b1dd4: addss xmm0, xmm7
006b1dd8: movss dword ptr [rbx + 0x1c], xmm0
006b1ddd: cmp dword ptr [rdi + 0xb0], 1
006b1de4: je 0x1806b1e24
006b1de6: cmp dword ptr [rdi + 0xb0], 2
006b1ded: je 0x1806b1e0e
006b1def: cmp dword ptr [rdi + 0xb0], 3
006b1df6: jne 0x1806b1e40
006b1df8: xor edx, edx
006b1dfa: mov dword ptr [rbx + 0x20], 0x14
006b1e01: mov rcx, rdi
006b1e04: call 0x1806972a0
006b1e09: jmp 0x1806b1cd0
006b1e0e: xor edx, edx
006b1e10: mov dword ptr [rbx + 0x20], 0x13
006b1e17: mov rcx, rdi
006b1e1a: call 0x1806972a0
006b1e1f: jmp 0x1806b1cd0
006b1e24: mov dword ptr [rbx + 0x20], 0x12
006b1e2b: xor edx, edx
006b1e2d: mov rcx, rdi
006b1e30: call 0x1806972a0
006b1e35: jmp 0x1806b1cd0
006b1e3a: call 0x1804fee20
006b1e3f: int3
006b1e40: lea rcx, [rip + 0x45a0731]
006b1e47: call 0x1804feb90
006b1e4c: mov rcx, rax
006b1e4f: call 0x1804fedd0
006b1e54: lea rcx, [rip + 0x45dce4d]
006b1e5b: mov rbx, rax
006b1e5e: call 0x1804feb90
006b1e63: mov rdx, rax
006b1e66: xor r8d, r8d
006b1e69: mov rcx, rbx
006b1e6c: call 0x182c223d0
006b1e71: lea rcx, [rip + 0x4591f48]
006b1e78: call 0x1804feb90
006b1e7d: mov rdx, rax
006b1e80: mov rcx, rbx
006b1e83: call 0x1804fede0
006b1e88: int3
006b1e89: call 0x1804fee10
006b1e8e: int3
006b1e8f: int3
