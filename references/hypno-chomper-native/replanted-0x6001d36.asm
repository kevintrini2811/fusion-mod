006f1d40: push rdi
006f1d42: sub rsp, 0x50
006f1d46: cmp byte ptr [rip + 0x47f9a96], 0
006f1d4d: mov rdi, rcx
006f1d50: jne 0x1806f1d7d
006f1d52: lea rcx, [rip + 0x4539267]
006f1d59: call 0x1804feb70
006f1d5e: lea rcx, [rip + 0x458f6eb]
006f1d65: call 0x1804feb70
006f1d6a: lea rcx, [rip + 0x458f7b7]
006f1d71: call 0x1804feb70
006f1d76: mov byte ptr [rip + 0x47f9a66], 1
006f1d7d: cmp dword ptr [rdi + 0x74], 0x20
006f1d81: xorps xmm0, xmm0
006f1d84: xorps xmm1, xmm1
006f1d87: mov qword ptr [rsp + 0x60], rbx
006f1d8c: movups xmmword ptr [rsp + 0x30], xmm0
006f1d91: mov qword ptr [rsp + 0x68], rsi
006f1d96: movups xmmword ptr [rsp + 0x20], xmm1
006f1d9b: je 0x1806f1f07
006f1da1: xor r8d, r8d
006f1da4: lea rcx, [rsp + 0x40]
006f1da9: mov rdx, rdi
006f1dac: call 0x1806f2c90
006f1db1: xor esi, esi
006f1db3: mov edx, esi
006f1db5: movups xmm0, xmmword ptr [rax]
006f1db8: mov rax, qword ptr [rdi + 0x18]
006f1dbc: movups xmmword ptr [rsp + 0x30], xmm0
006f1dc1: test rax, rax
006f1dc4: je 0x1806f1f19
006f1dca: nop word ptr [rax + rax]
006f1dd0: mov rcx, qword ptr [rax + 0x70]
006f1dd4: test rcx, rcx
006f1dd7: je 0x1806f1f19
006f1ddd: cmp edx, dword ptr [rcx + 0x28]
006f1de0: jge 0x1806f1f07
006f1de6: mov rax, qword ptr [rdi + 0x18]
006f1dea: test rax, rax
006f1ded: je 0x1806f1f19
006f1df3: mov rcx, qword ptr [rax + 0x70]
006f1df7: test rcx, rcx
006f1dfa: je 0x1806f1f19
006f1e00: mov r8, qword ptr [rip + 0x458f721]
006f1e07: mov edx, esi
006f1e09: call 0x182449330
006f1e0e: mov rbx, rax
006f1e11: test rax, rax
006f1e14: je 0x1806f1f19
006f1e1a: movzx eax, byte ptr [rax + 0x110]
006f1e21: cmp byte ptr [rdi + 0x110], al
006f1e27: je 0x1806f1ee2
006f1e2d: cmp dword ptr [rbx + 0x74], 0x49
006f1e31: je 0x1806f1ee2
006f1e37: cmp dword ptr [rbx + 0x74], 0x4a
006f1e3b: je 0x1806f1ee2
006f1e41: cmp dword ptr [rbx + 0x74], 0x20
006f1e45: je 0x1806f1ee2
006f1e4b: cmp dword ptr [rbx + 0x74], 4
006f1e4f: je 0x1806f1ee2
006f1e55: cmp dword ptr [rbx + 0x74], 5
006f1e59: je 0x1806f1ee2
006f1e5f: cmp dword ptr [rbx + 0x74], 8
006f1e63: je 0x1806f1ee2
006f1e65: cmp dword ptr [rbx + 0xb8], 9
006f1e6c: je 0x1806f1ee2
006f1e6e: cmp byte ptr [rbx + 0x144], 0
006f1e75: jne 0x1806f1ee2
006f1e77: cmp dword ptr [rbx + 0x74], 1
006f1e7b: je 0x1806f1ee2
006f1e7d: cmp dword ptr [rbx + 0x74], 2
006f1e81: je 0x1806f1ee2
006f1e83: cmp dword ptr [rbx + 0x74], 3
006f1e87: je 0x1806f1ee2
006f1e89: mov eax, dword ptr [rdi + 0x34]
006f1e8c: cmp dword ptr [rbx + 0x34], eax
006f1e8f: jne 0x1806f1ee2
006f1e91: xor r8d, r8d
006f1e94: lea rcx, [rsp + 0x40]
006f1e99: mov rdx, rbx
006f1e9c: call 0x1806f2d70
006f1ea1: mov rcx, qword ptr [rip + 0x4539118]
006f1ea8: movups xmm0, xmmword ptr [rax]
006f1eab: movups xmmword ptr [rsp + 0x20], xmm0
006f1eb0: cmp dword ptr [rcx + 0xe4], 0
006f1eb7: jne 0x1806f1ebe
006f1eb9: call 0x1804feeb0
006f1ebe: xor r8d, r8d
006f1ec1: lea rdx, [rsp + 0x20]
006f1ec6: lea rcx, [rsp + 0x30]
006f1ecb: call 0x1806cbaa0
006f1ed0: cmp eax, 0x14
006f1ed3: jge 0x1806f1ef4
006f1ed5: test eax, eax
006f1ed7: js 0x1806f1ee2
006f1ed9: cmp byte ptr [rbx + 0xa5], 0
006f1ee0: jne 0x1806f1ef4
006f1ee2: mov rax, qword ptr [rdi + 0x18]
006f1ee6: inc esi
006f1ee8: mov edx, esi
006f1eea: test rax, rax
006f1eed: je 0x1806f1f19
006f1eef: jmp 0x1806f1dd0
006f1ef4: mov rax, rbx
006f1ef7: mov rsi, qword ptr [rsp + 0x68]
006f1efc: mov rbx, qword ptr [rsp + 0x60]
006f1f01: add rsp, 0x50
006f1f05: pop rdi
006f1f06: ret
006f1f07: mov rsi, qword ptr [rsp + 0x68]
006f1f0c: xor eax, eax
006f1f0e: mov rbx, qword ptr [rsp + 0x60]
006f1f13: add rsp, 0x50
006f1f17: pop rdi
006f1f18: ret
006f1f19: call 0x1804fee20
006f1f1e: int3
006f1f1f: int3
