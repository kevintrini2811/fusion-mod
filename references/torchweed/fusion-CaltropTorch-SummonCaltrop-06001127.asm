00496eb0: mov qword ptr [rsp + 0x18], rbx
00496eb5: push rbp
00496eb6: push rdi
00496eb7: push r14
00496eb9: sub rsp, 0x80
00496ec0: cmp byte ptr [rip + 0x3102334], 0
00496ec7: mov rbp, rcx
00496eca: movaps xmmword ptr [rsp + 0x70], xmm6
00496ecf: jne 0x180496efc
00496ed1: lea rcx, [rip + 0x2ef1060]
00496ed8: call 0x1803633b0
00496edd: lea rcx, [rip + 0x2f44cb4]
00496ee4: call 0x1803633b0
00496ee9: lea rcx, [rip + 0x2f494d8]
00496ef0: call 0x1803633b0
00496ef5: mov byte ptr [rip + 0x31022ff], 1
00496efc: mov edi, dword ptr [rbp + 0xf4]
00496f02: xorps xmm6, xmm6
00496f05: inc edi
00496f07: mov qword ptr [rsp + 0xa8], rsi
00496f0f: xor r14d, r14d
00496f12: mov ebx, r14d
00496f15: nop word ptr [rax + rax]
00496f20: mov rcx, qword ptr [rip + 0x2f44c71]
00496f27: cmp dword ptr [rcx + 0xe0], r14d
00496f2e: jne 0x180496f35
00496f30: call 0x180318580
00496f35: xor r8d, r8d
00496f38: xor edx, edx
00496f3a: mov rcx, rbx
00496f3d: call 0x1826b9a40
00496f42: test al, al
00496f44: je 0x180497091
00496f4a: mov rax, qword ptr [rip + 0x2ef0fe7]
00496f51: cmp dword ptr [rax + 0xe0], r14d
00496f58: jne 0x180496f69
00496f5a: mov rcx, rax
00496f5d: call 0x180318580
00496f62: mov rax, qword ptr [rip + 0x2ef0fcf]
00496f69: mov rax, qword ptr [rax + 0xb8]
00496f70: mov rcx, qword ptr [rax]
00496f73: xor eax, eax
00496f75: mov qword ptr [rsp + 0xa0], rax
00496f7d: test rcx, rcx
00496f80: je 0x1804970b2
00496f86: movsd xmm0, qword ptr [rsp + 0xa0]
00496f8f: mov r9d, 0x483
00496f95: mov r8d, dword ptr [rbp + 0xf8]
00496f9c: mov edx, edi
00496f9e: mov qword ptr [rsp + 0x48], r14
00496fa3: mov qword ptr [rsp + 0x40], r14
00496fa8: mov byte ptr [rsp + 0x38], 1
00496fad: mov byte ptr [rsp + 0x30], al
00496fb1: movsd qword ptr [rsp + 0x28], xmm0
00496fb7: mov qword ptr [rsp + 0x20], r14
00496fbc: call 0x1809df0b0
00496fc1: mov rcx, qword ptr [rip + 0x2f44bd0]
00496fc8: mov rbx, rax
00496fcb: cmp dword ptr [rcx + 0xe0], r14d
00496fd2: jne 0x180496fd9
00496fd4: call 0x180318580
00496fd9: xor r8d, r8d
00496fdc: xor edx, edx
00496fde: mov rcx, rbx
00496fe1: call 0x1826b9be0
00496fe6: test al, al
00496fe8: je 0x180497076
00496fee: mov rax, qword ptr [rip + 0x2f493d3]
00496ff5: cmp dword ptr [rax + 0xe0], r14d
00496ffc: jne 0x18049700d
00496ffe: mov rcx, rax
00497001: call 0x180318580
00497006: mov rax, qword ptr [rip + 0x2f493bb]
0049700d: mov rax, qword ptr [rax + 0xb8]
00497014: mov rsi, qword ptr [rax]
00497017: test rbx, rbx
0049701a: je 0x1804970b2
00497020: mov rdx, qword ptr [rbx + 0x38]
00497024: test rdx, rdx
00497027: je 0x1804970b2
0049702d: xor r8d, r8d
00497030: lea rcx, [rsp + 0x60]
00497035: call 0x1826c8510
0049703a: movsd xmm0, qword ptr [rax]
0049703e: movaps xmm1, xmm0
00497041: shufps xmm1, xmm1, 0x55
00497045: test rsi, rsi
00497048: je 0x1804970b2
0049704a: mov r9d, dword ptr [rbx + 0xf8]
00497051: mov edx, 0xb
00497056: unpcklps xmm0, xmm1
00497059: mov rcx, rsi
0049705c: mov qword ptr [rsp + 0x30], r14
00497061: movq r8, xmm0
00497066: movss dword ptr [rsp + 0x28], xmm6
0049706c: mov byte ptr [rsp + 0x20], 1
00497071: call 0x180465c40
00497076: mov rax, qword ptr [rbp + 0x30]
0049707a: inc edi
0049707c: test rax, rax
0049707f: je 0x1804970b2
00497081: mov eax, dword ptr [rax + 0x154]
00497087: dec eax
00497089: cmp edi, eax
0049708b: jle 0x180496f20
00497091: mov rsi, qword ptr [rsp + 0xa8]
00497099: mov rbx, qword ptr [rsp + 0xb0]
004970a1: movaps xmm6, xmmword ptr [rsp + 0x70]
004970a6: add rsp, 0x80
004970ad: pop r14
004970af: pop rdi
004970b0: pop rbp
004970b1: ret
004970b2: call 0x180363610
004970b7: int3
004970b8: int3
004970b9: int3
004970ba: int3
004970bb: int3
004970bc: int3
004970bd: int3
004970be: int3
004970bf: int3
