0065d080: push rbp
0065d082: push rsi
0065d083: push r12
0065d085: push r15
0065d087: lea rbp, [rsp - 0x27]
0065d08c: sub rsp, 0xc8
0065d093: cmp byte ptr [rip + 0x2f3ce31], 0
0065d09a: mov r12d, r9d
0065d09d: movsxd r15, edx
0065d0a0: mov rsi, rcx
0065d0a3: jne 0x18065d0f4
0065d0a5: lea rcx, [rip + 0x2d86304]
0065d0ac: call 0x1803633b0
0065d0b1: lea rcx, [rip + 0x2d86f58]
0065d0b8: call 0x1803633b0
0065d0bd: lea rcx, [rip + 0x2d3eb2c]
0065d0c4: call 0x1803633b0
0065d0c9: lea rcx, [rip + 0x2d6a390]
0065d0d0: call 0x1803633b0
0065d0d5: lea rcx, [rip + 0x2d1b374]
0065d0dc: call 0x1803633b0
0065d0e1: lea rcx, [rip + 0x2d5ff58]
0065d0e8: call 0x1803633b0
0065d0ed: mov byte ptr [rip + 0x2f3cdd7], 1
0065d0f4: xor eax, eax
0065d0f6: mov dword ptr [rsi + 0x204], 0x3e4ccccd
0065d100: mov qword ptr [rbp - 0x11], rax
0065d104: xorps xmm0, xmm0
0065d107: movups xmmword ptr [rbp - 0x21], xmm0
0065d10b: test r15d, r15d
0065d10e: je 0x18065d61f
0065d114: mov qword ptr [rsp + 0x100], r13
0065d11c: mov qword ptr [rsp + 0xc0], r14
0065d124: mov qword ptr [rsp + 0xf0], rbx
0065d12c: mov qword ptr [rsp + 0xf8], rdi
0065d134: xor r13d, r13d
0065d137: mov r14, r15
0065d13a: cmp byte ptr [rsi + 0x12d], al
0065d140: jne 0x18065d26a
0065d146: mov rcx, qword ptr [rip + 0x2d6a313]
0065d14d: cmp dword ptr [rcx + 0xe0], eax
0065d153: jne 0x18065d15a
0065d155: call 0x180318580
0065d15a: mov rcx, r15
0065d15d: xor edx, edx
0065d15f: call 0x180a10d80
0065d164: mov r8, qword ptr [rsi]
0065d167: mov edx, eax
0065d169: mov rcx, rsi
0065d16c: mov r9, qword ptr [r8 + 0x358]
0065d173: mov r8, qword ptr [r8 + 0x360]
0065d17a: call r9
0065d17d: test al, al
0065d17f: jne 0x18065d264
0065d185: xor edx, edx
0065d187: mov ecx, 0x1c
0065d18c: call 0x1807a2090
0065d191: test al, al
0065d193: je 0x18065d1a2
0065d195: cmp dword ptr [rsi + 0x1f0], 0x32
0065d19c: jle 0x18065d264
0065d1a2: xor edx, edx
0065d1a4: mov ecx, 0x1d
0065d1a9: call 0x1807a2090
0065d1ae: test al, al
0065d1b0: je 0x18065d26a
0065d1b6: cmp dword ptr [rsi + 0x1f0], 0x64
0065d1bd: jg 0x18065d26a
0065d1c3: cmp dword ptr [rsi + 0x1f4], r13d
0065d1ca: jne 0x18065d22a
0065d1cc: xor edx, edx
0065d1ce: mov rcx, rsi
0065d1d1: call 0x18214ffe0
0065d1d6: mov rcx, qword ptr [rip + 0x2d861d3]
0065d1dd: mov rbx, rax
0065d1e0: call 0x18030dc40
0065d1e5: mov r8, qword ptr [rip + 0x2d5fe54]
0065d1ec: xor r9d, r9d
0065d1ef: mov rdx, rsi
0065d1f2: mov rcx, rax
0065d1f5: mov rdi, rax
0065d1f8: call 0x1809bf5b0
0065d1fd: movss xmm0, dword ptr [rip + 0x231e927]
0065d205: xor r9d, r9d
0065d208: mov qword ptr [rsp + 0x38], r13
0065d20d: xor r8d, r8d
0065d210: mov dword ptr [rsp + 0x30], 1
0065d218: mov rdx, rbx
0065d21b: mov byte ptr [rsp + 0x28], r13b
0065d220: mov qword ptr [rsp + 0x20], rdi
0065d225: call 0x1807d9550
0065d22a: movsxd rbx, dword ptr [rsi + 0x1f4]
0065d231: cmp r14, rbx
0065d234: jle 0x18065d5ff
0065d23a: mov rcx, qword ptr [rip + 0x2d6a21f]
0065d241: cmp dword ptr [rcx + 0xe0], r13d
0065d248: jne 0x18065d24f
0065d24a: call 0x180318580
0065d24f: xor edx, edx
0065d251: mov rcx, r14
0065d254: call 0x180a10d80
0065d259: mov dword ptr [rsi + 0x1f4], eax
0065d25f: sub r14, rbx
0065d262: jmp 0x18065d26a
0065d264: mov r14d, 1
0065d26a: mov rax, qword ptr [rsi]
0065d26d: mov r8d, r12d
0065d270: mov ebx, dword ptr [rbp + 0x6f]
0065d273: mov rdx, r14
0065d276: movzx r9d, byte ptr [rbp + 0x77]
0065d27b: mov rcx, rsi
0065d27e: mov r10, qword ptr [rax + 0x378]
0065d285: mov rax, qword ptr [rax + 0x380]
0065d28c: mov qword ptr [rsp + 0x28], rax
0065d291: mov dword ptr [rsp + 0x20], ebx
0065d295: call r10
0065d298: mov rdi, rax
0065d29b: test rax, rax
0065d29e: je 0x18065d5ff
0065d2a4: xor edx, edx
0065d2a6: mov rcx, rsi
0065d2a9: call 0x1806596c0
0065d2ae: inc dword ptr [rsi + 0x1f0]
0065d2b4: mov dword ptr [rsi + 0x208], r13d
0065d2bb: mov rcx, qword ptr [rip + 0x2d6a19e]
0065d2c2: cmp dword ptr [rcx + 0xe0], r13d
0065d2c9: jne 0x18065d2d0
0065d2cb: call 0x180318580
0065d2d0: movaps xmmword ptr [rsp + 0xb0], xmm6
0065d2d8: xor edx, edx
0065d2da: mov rcx, rdi
0065d2dd: movaps xmmword ptr [rsp + 0xa0], xmm7
0065d2e5: call 0x180a10d80
0065d2ea: xor r9d, r9d
0065d2ed: mov r8d, eax
0065d2f0: mov edx, r12d
0065d2f3: mov rcx, rsi
0065d2f6: call 0x18064e280
0065d2fb: cmp ebx, -1
0065d2fe: je 0x18065d5c9
0065d304: cmp byte ptr [rsi + 0x12d], r13b
0065d30b: jne 0x18065d5c9
0065d311: xor edx, edx
0065d313: mov ecx, ebx
0065d315: call 0x1807f8ce0
0065d31a: test al, al
0065d31c: jne 0x18065d6b7
0065d322: mov rdx, rsi
0065d325: mov qword ptr [rbp - 0x19], r13
0065d329: lea rcx, [rbp - 0x21]
0065d32d: mov qword ptr [rbp - 0x11], rdi
0065d331: mov qword ptr [rbp - 0x21], rsi
0065d335: call 0x180362730
0065d33a: mov rcx, qword ptr [rip + 0x2d3e8af]
0065d341: movsd xmm7, qword ptr [rbp - 0x11]
0065d346: mov dword ptr [rbp - 0x19], ebx
0065d349: movups xmm6, xmmword ptr [rbp - 0x21]
0065d34d: cmp dword ptr [rcx + 0xe0], r13d
0065d354: jne 0x18065d35b
0065d356: call 0x180318580
0065d35b: xor edx, edx
0065d35d: movaps xmmword ptr [rbp - 0x41], xmm6
0065d361: lea rcx, [rbp - 0x41]
0065d365: movsd qword ptr [rbp - 0x31], xmm7
0065d36a: call 0x180681660
0065d36f: mov rax, qword ptr [rsi + 0x30]
0065d373: cmp r12d, 0xe
0065d377: je 0x18065d62d
0065d37d: test rax, rax
0065d380: je 0x18065d73f
0065d386: mov rcx, qword ptr [rax + 0x220]
0065d38d: xor r14d, r14d
0065d390: movss xmm1, dword ptr [rsi + 0x280]
0065d398: xorps xmm6, xmm6
0065d39b: test rcx, rcx
0065d39e: je 0x18065d73f
0065d3a4: movss xmm0, dword ptr [rsi + 0x27c]
0065d3ac: lea rax, [rbp - 0x41]
0065d3b0: unpcklps xmm0, xmm1
0065d3b3: mov r8, rdi
0065d3b6: movq r9, xmm0
0065d3bb: mov qword ptr [rsp + 0x28], r13
0065d3c0: mov edx, ebx
0065d3c2: movaps xmmword ptr [rbp - 0x41], xmm6
0065d3c6: mov dword ptr [rbp - 0x31], r14d
0065d3ca: mov qword ptr [rsp + 0x20], rax
0065d3cf: call 0x1809e4a60
0065d3d4: mov rax, qword ptr [rsi + 0x30]
0065d3d8: test rax, rax
0065d3db: je 0x18065d73f
0065d3e1: mov rax, qword ptr [rax + 0x220]
0065d3e8: test rax, rax
0065d3eb: je 0x18065d73f
0065d3f1: cmp ebx, dword ptr [rax + 0x18]
0065d3f4: jne 0x18065d4c9
0065d3fa: mov rax, qword ptr [rip + 0x2d86c0f]
0065d401: cmp dword ptr [rax + 0xe0], r13d
0065d408: jne 0x18065d419
0065d40a: mov rcx, rax
0065d40d: call 0x180318580
0065d412: mov rax, qword ptr [rip + 0x2d86bf7]
0065d419: mov rax, qword ptr [rax + 0xb8]
0065d420: mov rcx, qword ptr [rax]
0065d423: test rcx, rcx
0065d426: je 0x18065d73f
0065d42c: xor r8d, r8d
0065d42f: mov edx, 0x14
0065d434: call 0x180760e20
0065d439: test al, al
0065d43b: jne 0x18065d44d
0065d43d: xor edx, edx
0065d43f: mov ecx, 0x7d3
0065d444: call 0x1807a1fb0
0065d449: test al, al
0065d44b: je 0x18065d4c9
0065d44d: movd xmm0, r15d
0065d452: xor r9d, r9d
0065d455: cvtdq2ps xmm0, xmm0
0065d458: mov edx, 4
0065d45d: mov rcx, rsi
0065d460: mulss xmm0, dword ptr [rip + 0x231e6cc]
0065d468: cvttss2si edi, xmm0
0065d46c: mov r8d, edi
0065d46f: call 0x18064e280
0065d474: mov rax, qword ptr [rsi + 0x30]
0065d478: test rax, rax
0065d47b: je 0x18065d73f
0065d481: mov rcx, qword ptr [rax + 0x220]
0065d488: movss xmm1, dword ptr [rsi + 0x280]
0065d490: test rcx, rcx
0065d493: je 0x18065d73f
0065d499: movss xmm0, dword ptr [rsi + 0x27c]
0065d4a1: lea rax, [rbp - 0x41]
0065d4a5: unpcklps xmm0, xmm1
0065d4a8: mov edx, ebx
0065d4aa: movq r9, xmm0
0065d4af: movsxd r8, edi
0065d4b2: mov qword ptr [rsp + 0x28], r13
0065d4b7: movaps xmmword ptr [rbp - 0x41], xmm6
0065d4bb: mov dword ptr [rbp - 0x31], r14d
0065d4bf: mov qword ptr [rsp + 0x20], rax
0065d4c4: call 0x1809e4a60
0065d4c9: mov rax, qword ptr [rip + 0x2d86b40]
0065d4d0: cmp dword ptr [rax + 0xe0], r13d
0065d4d7: jne 0x18065d4e8
0065d4d9: mov rcx, rax
0065d4dc: call 0x180318580
0065d4e1: mov rax, qword ptr [rip + 0x2d86b28]
0065d4e8: mov rax, qword ptr [rax + 0xb8]
0065d4ef: mov rcx, qword ptr [rax]
0065d4f2: test rcx, rcx
0065d4f5: je 0x18065d73f
0065d4fb: xor r8d, r8d
0065d4fe: mov edx, 0x12
0065d503: call 0x180760e20
0065d508: test al, al
0065d50a: je 0x18065d5c9
0065d510: movsxd rcx, dword ptr [rsi + 0xb4]
0065d517: movsxd rax, dword ptr [rsi + 0xbc]
0065d51e: movsxd r8, dword ptr [rsi + 0xb8]
0065d525: add rax, rcx
0065d528: add rax, qword ptr [rsi + 0xa8]
0065d52f: movsxd rcx, dword ptr [rsi + 0xb0]
0065d536: cqo
0065d538: sub rax, rdx
0065d53b: add r8, rcx
0065d53e: add r8, qword ptr [rsi + 0xa0]
0065d545: sar rax, 1
0065d548: cmp r8, rax
0065d54b: jge 0x18065d5c9
0065d54d: movd xmm0, r15d
0065d552: xor r9d, r9d
0065d555: cvtdq2ps xmm0, xmm0
0065d558: mov edx, 4
0065d55d: mov rcx, rsi
0065d560: mulss xmm0, dword ptr [rip + 0x231e5c4]
0065d568: cvttss2si edi, xmm0
0065d56c: mov r8d, edi
0065d56f: call 0x18064e280
0065d574: mov rcx, qword ptr [rsi + 0x30]
0065d578: test rcx, rcx
0065d57b: je 0x18065d73f
0065d581: mov rcx, qword ptr [rcx + 0x220]
0065d588: movss xmm1, dword ptr [rsi + 0x280]
0065d590: test rcx, rcx
0065d593: je 0x18065d73f
0065d599: movss xmm0, dword ptr [rsi + 0x27c]
0065d5a1: lea rax, [rbp - 0x41]
0065d5a5: unpcklps xmm0, xmm1
0065d5a8: mov edx, ebx
0065d5aa: mov qword ptr [rsp + 0x28], r13
0065d5af: movq r9, xmm0
0065d5b4: movaps xmmword ptr [rbp - 0x41], xmm6
0065d5b8: mov dword ptr [rbp - 0x31], r14d
0065d5bc: movsxd r8, edi
0065d5bf: mov qword ptr [rsp + 0x20], rax
0065d5c4: call 0x1809e4a60
0065d5c9: cmp qword ptr [rsi + 0xa0], r13
0065d5d0: jge 0x18065d5d9
0065d5d2: mov qword ptr [rsi + 0xa0], r13
0065d5d9: mov rdx, qword ptr [rsi]
0065d5dc: mov rcx, rsi
0065d5df: mov rax, qword ptr [rdx + 0x608]
0065d5e6: mov rdx, qword ptr [rdx + 0x610]
0065d5ed: call rax
0065d5ef: movaps xmm6, xmmword ptr [rsp + 0xb0]
0065d5f7: movaps xmm7, xmmword ptr [rsp + 0xa0]
0065d5ff: mov rdi, qword ptr [rsp + 0xf8]
0065d607: mov rbx, qword ptr [rsp + 0xf0]
0065d60f: mov r13, qword ptr [rsp + 0x100]
0065d617: mov r14, qword ptr [rsp + 0xc0]
0065d61f: add rsp, 0xc8
0065d626: pop r15
0065d628: pop r12
0065d62a: pop rsi
0065d62b: pop rbp
0065d62c: ret
0065d62d: test rax, rax
0065d630: je 0x18065d745
0065d636: mov rdi, qword ptr [rax + 0x220]
0065d63d: movss xmm6, dword ptr [rsi + 0x27c]
0065d645: movss xmm7, dword ptr [rsi + 0x280]
0065d64d: movdqa xmm1, xmmword ptr [rip + 0x231e69b]
0065d655: lea rdx, [rbp - 0x61]
0065d659: mov r8, qword ptr [rip + 0x2d1adf0]
0065d660: lea rcx, [rbp - 0x41]
0065d664: xorps xmm0, xmm0
0065d667: movdqa xmmword ptr [rbp - 0x61], xmm1
0065d66c: xor eax, eax
0065d66e: movups xmmword ptr [rbp - 0x41], xmm0
0065d672: mov dword ptr [rbp - 0x31], eax
0065d675: call 0x1818203b0
0065d67a: test rdi, rdi
0065d67d: je 0x18065d745
0065d683: mov eax, dword ptr [rbp - 0x31]
0065d686: mov r8d, 0x1388
0065d68c: movups xmm0, xmmword ptr [rbp - 0x41]
0065d690: mov dword ptr [rbp - 0x51], eax
0065d693: mov edx, ebx
0065d695: lea rax, [rbp - 0x61]
0065d699: unpcklps xmm6, xmm7
0065d69c: mov qword ptr [rsp + 0x28], r13
0065d6a1: movq r9, xmm6
0065d6a6: movaps xmmword ptr [rbp - 0x61], xmm0
0065d6aa: mov rcx, rdi
0065d6ad: mov qword ptr [rsp + 0x20], rax
0065d6b2: jmp 0x18065d5c4
0065d6b7: mov rax, qword ptr [rsi + 0x30]
0065d6bb: test rax, rax
0065d6be: je 0x18065d739
0065d6c0: mov r14, qword ptr [rax + 0x220]
0065d6c7: lea rdx, [rbp - 0x61]
0065d6cb: movdqa xmm1, xmmword ptr [rip + 0x231eb5d]
0065d6d3: lea rcx, [rbp - 0x41]
0065d6d7: mov r8, qword ptr [rip + 0x2d1ad72]
0065d6de: xor eax, eax
0065d6e0: movss xmm6, dword ptr [rsi + 0x27c]
0065d6e8: xorps xmm0, xmm0
0065d6eb: movss xmm7, dword ptr [rsi + 0x280]
0065d6f3: mov dword ptr [rbp - 0x31], eax
0065d6f6: movups xmmword ptr [rbp - 0x41], xmm0
0065d6fa: movaps xmmword ptr [rbp - 0x61], xmm1
0065d6fe: call 0x1818203b0
0065d703: test r14, r14
0065d706: je 0x18065d739
0065d708: mov eax, dword ptr [rbp - 0x31]
0065d70b: mov r8, rdi
0065d70e: movups xmm0, xmmword ptr [rbp - 0x41]
0065d712: mov dword ptr [rbp - 0x51], eax
0065d715: mov edx, ebx
0065d717: lea rax, [rbp - 0x61]
0065d71b: unpcklps xmm6, xmm7
0065d71e: mov qword ptr [rsp + 0x28], r13
0065d723: movq r9, xmm6
0065d728: movaps xmmword ptr [rbp - 0x61], xmm0
0065d72c: mov rcx, r14
0065d72f: mov qword ptr [rsp + 0x20], rax
0065d734: jmp 0x18065d5c4
0065d739: call 0x180363610
0065d73e: int3
0065d73f: call 0x180363610
0065d744: int3
0065d745: call 0x180363610
0065d74a: int3
0065d74b: int3
0065d74c: int3
0065d74d: int3
0065d74e: int3
0065d74f: int3
