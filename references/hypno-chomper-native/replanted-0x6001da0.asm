006f5d80: mov qword ptr [rsp + 8], rbx
006f5d85: mov qword ptr [rsp + 0x10], rbp
006f5d8a: mov qword ptr [rsp + 0x18], rsi
006f5d8f: mov qword ptr [rsp + 0x20], rdi
006f5d94: push r14
006f5d96: sub rsp, 0x20
006f5d9a: xor edx, edx
006f5d9c: mov rbx, rcx
006f5d9f: call 0x1806ce570
006f5da4: xor ebp, ebp
006f5da6: xor eax, eax
006f5da8: mov qword ptr [rbx + 0x70], rbp
006f5dac: xorps xmm0, xmm0
006f5daf: mov qword ptr [rbx + 0x78], rbp
006f5db3: xorps xmm1, xmm1
006f5db6: mov qword ptr [rbx + 0x88], rbp
006f5dbd: mov edi, ebp
006f5dbf: mov qword ptr [rbx + 0x90], rbp
006f5dc6: mov ecx, ebp
006f5dc8: mov qword ptr [rbx + 0x98], rbp
006f5dcf: mov dword ptr [rbx + 0xa0], ebp
006f5dd5: mov word ptr [rbx + 0xa4], bp
006f5ddc: mov qword ptr [rbx + 0xa8], rbp
006f5de3: mov qword ptr [rbx + 0xb0], rbp
006f5dea: mov qword ptr [rbx + 0xb8], rbp
006f5df1: mov dword ptr [rbx + 0xc4], ebp
006f5df7: mov byte ptr [rbx + 0xc8], bpl
006f5dfe: mov dword ptr [rbx + 0xcc], ebp
006f5e04: mov byte ptr [rbx + 0xd0], bpl
006f5e0b: mov qword ptr [rbx + 0xd4], rbp
006f5e12: mov dword ptr [rbx + 0xdc], ebp
006f5e18: mov byte ptr [rbx + 0xe0], bpl
006f5e1f: movups xmmword ptr [rbx + 0xe4], xmm0
006f5e26: movups xmmword ptr [rbx + 0xf4], xmm1
006f5e2d: mov qword ptr [rbx + 0x104], rbp
006f5e34: mov qword ptr [rbx + 0x10c], rbp
006f5e3b: mov dword ptr [rbx + 0x114], ebp
006f5e41: mov qword ptr [rbx + 0x118], rbp
006f5e48: mov qword ptr [rbx + 0x120], rbp
006f5e4f: mov qword ptr [rbx + 0x128], rbp
006f5e56: mov qword ptr [rbx + 0x130], rbp
006f5e5d: mov qword ptr [rbx + 0x138], rbp
006f5e64: mov dword ptr [rbx + 0x140], ebp
006f5e6a: mov byte ptr [rbx + 0x144], bpl
006f5e71: mov dword ptr [rbx + 0x148], eax
006f5e77: mov rax, qword ptr [rbx + 0x150]
006f5e7e: test rax, rax
006f5e81: je 0x1806f5fb1
006f5e87: mov r14d, 0x20
006f5e8d: nop dword ptr [rax]
006f5e90: cmp ecx, dword ptr [rax + 0x18]
006f5e93: jge 0x1806f5ed3
006f5e95: mov rsi, qword ptr [rbx + 0x150]
006f5e9c: xor ecx, ecx
006f5e9e: call 0x18063cd70
006f5ea3: test rsi, rsi
006f5ea6: je 0x1806f5fb1
006f5eac: cmp edi, dword ptr [rsi + 0x18]
006f5eaf: jae 0x1806f5fb7
006f5eb5: mov dword ptr [r14 + rsi], eax
006f5eb9: inc edi
006f5ebb: mov rax, qword ptr [rbx + 0x150]
006f5ec2: add r14, 4
006f5ec6: mov ecx, edi
006f5ec8: test rax, rax
006f5ecb: je 0x1806f5fb1
006f5ed1: jmp 0x1806f5e90
006f5ed3: xor eax, eax
006f5ed5: mov byte ptr [rbx + 0x158], bpl
006f5edc: mov qword ptr [rbx + 0x15c], rbp
006f5ee3: lea rcx, [rbx + 0x198]
006f5eea: mov qword ptr [rbx + 0x164], rax
006f5ef1: xor edx, edx
006f5ef3: mov qword ptr [rbx + 0x16c], rax
006f5efa: mov qword ptr [rbx + 0x174], rbp
006f5f01: mov qword ptr [rbx + 0x17c], rax
006f5f08: mov qword ptr [rbx + 0x184], rbp
006f5f0f: mov qword ptr [rbx + 0x18c], rbp
006f5f16: mov qword ptr [rbx + 0x198], rbp
006f5f1d: call 0x1804fdea0
006f5f22: xor eax, eax
006f5f24: lea rcx, [rbx + 0x1c8]
006f5f2b: mov qword ptr [rbx + 0x1a8], rax
006f5f32: xor edx, edx
006f5f34: mov dword ptr [rbx + 0x1b0], ebp
006f5f3a: mov byte ptr [rbx + 0x1b4], al
006f5f40: mov dword ptr [rbx + 0x1bc], ebp
006f5f46: mov qword ptr [rbx + 0x1c8], rbp
006f5f4d: call 0x1804fdea0
006f5f52: lea rcx, [rbx + 0x1d0]
006f5f59: mov qword ptr [rbx + 0x1d0], rbp
006f5f60: xor edx, edx
006f5f62: call 0x1804fdea0
006f5f67: mov rsi, qword ptr [rsp + 0x40]
006f5f6c: xorps xmm0, xmm0
006f5f6f: mov rdi, qword ptr [rsp + 0x48]
006f5f74: xor eax, eax
006f5f76: mov word ptr [rbx + 0x1d8], bp
006f5f7d: mov qword ptr [rbx + 0x1dc], rbp
006f5f84: mov rbp, qword ptr [rsp + 0x38]
006f5f89: movups xmmword ptr [rbx + 0x1e4], xmm0
006f5f90: mov qword ptr [rbx + 0x1f4], rax
006f5f97: mov qword ptr [rbx + 0x1fc], rax
006f5f9e: movups xmmword ptr [rbx + 0x204], xmm0
006f5fa5: mov rbx, qword ptr [rsp + 0x30]
006f5faa: add rsp, 0x20
006f5fae: pop r14
006f5fb0: ret
006f5fb1: call 0x1804fee20
006f5fb6: int3
006f5fb7: call 0x1804fee10
006f5fbc: int3
006f5fbd: int3
006f5fbe: int3
006f5fbf: int3
