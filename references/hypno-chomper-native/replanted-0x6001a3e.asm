006af750: push rdi
006af752: sub rsp, 0xf0
006af759: cmp byte ptr [rip + 0x483bef4], 0
006af760: mov rdi, rcx
006af763: jne 0x1806af790
006af765: lea rcx, [rip + 0x457b854]
006af76c: call 0x1804feb70
006af771: lea rcx, [rip + 0x45d1cd8]
006af778: call 0x1804feb70
006af77d: lea rcx, [rip + 0x45d1da4]
006af784: call 0x1804feb70
006af789: mov byte ptr [rip + 0x483bec4], 1
006af790: cmp dword ptr [rdi + 0x100], 0
006af797: mov qword ptr [rsp + 0x100], rbx
006af79f: mov qword ptr [rsp + 0x108], rbp
006af7a7: mov qword ptr [rsp + 0x110], rsi
006af7af: mov qword ptr [rsp + 0x118], r14
006af7b7: movaps xmmword ptr [rsp + 0xe0], xmm6
006af7bf: movaps xmmword ptr [rsp + 0xd0], xmm7
006af7c7: movaps xmmword ptr [rsp + 0xc0], xmm8
006af7d0: movaps xmmword ptr [rsp + 0xb0], xmm9
006af7d9: movaps xmmword ptr [rsp + 0xa0], xmm10
006af7e2: movaps xmmword ptr [rsp + 0x90], xmm11
006af7eb: movaps xmmword ptr [rsp + 0x80], xmm12
006af7f4: movaps xmmword ptr [rsp + 0x70], xmm13
006af7fa: movaps xmmword ptr [rsp + 0x60], xmm14
006af800: movaps xmmword ptr [rsp + 0x50], xmm15
006af806: jg 0x1806afa9d
006af80c: xor r8d, r8d
006af80f: xor edx, edx
006af811: mov rcx, rdi
006af814: call 0x1806b12f0
006af819: movss xmm10, dword ptr [rip + 0x35845a2]
006af822: xor r14d, r14d
006af825: mov rcx, qword ptr [rdi + 0x18]
006af829: mov ebp, eax
006af82b: movss xmm9, dword ptr [rdi + 0x20]
006af831: mov esi, r14d
006af834: movss xmm12, dword ptr [rdi + 0x24]
006af83a: addss xmm9, xmm10
006af83f: addss xmm12, xmm10
006af844: mov edx, r14d
006af847: test rcx, rcx
006af84a: je 0x1806afb17
006af850: movss xmm11, dword ptr [rip + 0x35843e7]
006af859: movss xmm13, dword ptr [rip + 0x358455e]
006af862: movss xmm14, dword ptr [rip + 0x35846ed]
006af86b: movss xmm15, dword ptr [rip + 0x3584548]
006af874: nop dword ptr [rax]
006af878: nop dword ptr [rax + rax]
006af880: mov rax, qword ptr [rcx + 0x70]
006af884: test rax, rax
006af887: je 0x1806afb17
006af88d: cmp edx, dword ptr [rax + 0x28]
006af890: jge 0x1806afa99
006af896: mov rax, qword ptr [rdi + 0x18]
006af89a: test rax, rax
006af89d: je 0x1806afb17
006af8a3: mov rcx, qword ptr [rax + 0x70]
006af8a7: test rcx, rcx
006af8aa: je 0x1806afb17
006af8b0: mov r8, qword ptr [rip + 0x45d1c71]
006af8b7: mov edx, esi
006af8b9: call 0x182449330
006af8be: mov rbx, rax
006af8c1: test rax, rax
006af8c4: je 0x1806afb17
006af8ca: xor r8d, r8d
006af8cd: lea rcx, [rsp + 0x40]
006af8d2: mov rdx, rax
006af8d5: call 0x1806f2d70
006af8da: xor r8d, r8d
006af8dd: mov edx, ebp
006af8df: mov rcx, rbx
006af8e2: movups xmm6, xmmword ptr [rax]
006af8e5: movups xmmword ptr [rsp + 0x30], xmm6
006af8ea: call 0x1806f12f0
006af8ef: test al, al
006af8f1: je 0x1806afa83
006af8f7: cmp dword ptr [rbx + 0x70], 0x19
006af8fb: jne 0x1806af907
006af8fd: cmp dword ptr [rdi + 0x74], 5
006af901: jge 0x1806afa9d
006af907: mov eax, dword ptr [rdi + 0x34]
006af90a: cmp dword ptr [rbx + 0x34], eax
006af90d: je 0x1806afa72
006af913: cmp dword ptr [rbx + 0x70], 0x11
006af917: movss xmm6, dword ptr [rsp + 0x38]
006af91d: jne 0x1806af927
006af91f: addss xmm6, dword ptr [rip + 0x3584485]
006af927: mov rcx, qword ptr [rip + 0x457b692]
006af92e: cmp dword ptr [rcx + 0xe4], r14d
006af935: jne 0x1806af93c
006af937: call 0x1804feeb0
006af93c: movss xmm7, dword ptr [rsp + 0x3c]
006af942: movaps xmm2, xmm6
006af945: movss xmm8, dword ptr [rsp + 0x34]
006af94c: movaps xmm3, xmm7
006af94f: mulss xmm3, xmm11
006af954: movaps xmm1, xmm12
006af958: mov qword ptr [rsp + 0x20], r14
006af95d: mulss xmm2, xmm11
006af962: movaps xmm0, xmm9
006af966: addss xmm3, xmm8
006af96b: addss xmm2, dword ptr [rsp + 0x30]
006af971: call 0x1806cb840
006af976: divss xmm0, dword ptr [rip + 0x3584622]
006af97e: xor r8d, r8d
006af981: mov rcx, rbx
006af984: movaps xmm1, xmm0
006af987: call 0x18070a110
006af98c: movaps xmm1, xmm0
006af98f: movaps xmm0, xmm6
006af992: mulss xmm0, xmm11
006af997: subss xmm1, xmm0
006af99b: cvttss2si eax, xmm1
006af99f: movd xmm0, eax
006af9a3: cvtdq2ps xmm0, xmm0
006af9a6: addss xmm0, xmm6
006af9aa: comiss xmm0, xmm9
006af9ae: jbe 0x1806af9c1
006af9b0: movd xmm0, eax
006af9b4: cvtdq2ps xmm0, xmm0
006af9b7: comiss xmm9, xmm0
006af9bb: ja 0x1806afa9d
006af9c1: mulss xmm6, xmm11
006af9c6: xor r8d, r8d
006af9c9: mulss xmm7, xmm11
006af9ce: movd xmm2, eax
006af9d2: cvtdq2ps xmm2, xmm2
006af9d5: addss xmm7, xmm8
006af9da: addss xmm6, xmm2
006af9de: subss xmm7, xmm12
006af9e3: subss xmm6, xmm9
006af9e8: movaps xmm0, xmm7
006af9eb: movaps xmm1, xmm6
006af9ee: call 0x1806ab010
006af9f3: mov rcx, qword ptr [rip + 0x457b5c6]
006af9fa: movaps xmm6, xmm0
006af9fd: cmp dword ptr [rcx + 0xe4], r14d
006afa04: jne 0x1806afa0b
006afa06: call 0x1804feeb0
006afa0b: xor edx, edx
006afa0d: movaps xmm0, xmm6
006afa10: call 0x1806cbd30
006afa15: mov edx, dword ptr [rdi + 0x34]
006afa18: mov ecx, dword ptr [rbx + 0x34]
006afa1b: sub ecx, edx
006afa1d: sub edx, dword ptr [rbx + 0x34]
006afa20: cmp edx, ecx
006afa22: cmovle edx, ecx
006afa25: cmp edx, 2
006afa28: jl 0x1806afa50
006afa2a: comiss xmm0, xmm13
006afa2e: jbe 0x1806afa3d
006afa30: movss xmm1, dword ptr [rip + 0x35844bc]
006afa38: comiss xmm1, xmm0
006afa3b: ja 0x1806afa9d
006afa3d: movss xmm1, dword ptr [rip + 0x35845d3]
006afa45: comiss xmm1, xmm0
006afa48: jbe 0x1806afa83
006afa4a: comiss xmm0, xmm14
006afa4e: jmp 0x1806afa81
006afa50: comiss xmm0, xmm15
006afa54: jbe 0x1806afa5c
006afa56: comiss xmm10, xmm0
006afa5a: ja 0x1806afa9d
006afa5c: movss xmm1, dword ptr [rip + 0x35844ec]
006afa64: comiss xmm1, xmm0
006afa67: jbe 0x1806afa83
006afa69: comiss xmm0, dword ptr [rip + 0x35845ac]
006afa70: jmp 0x1806afa81
006afa72: movaps xmm0, xmm6
006afa75: shufps xmm0, xmm6, 0xaa
006afa79: addss xmm0, xmm6
006afa7d: comiss xmm9, xmm0
006afa81: ja 0x1806afa9d
006afa83: mov rcx, qword ptr [rdi + 0x18]
006afa87: inc esi
006afa89: mov edx, esi
006afa8b: test rcx, rcx
006afa8e: je 0x1806afb17
006afa94: jmp 0x1806af880
006afa99: xor al, al
006afa9b: jmp 0x1806afa9f
006afa9d: mov al, 1
006afa9f: movaps xmm15, xmmword ptr [rsp + 0x50]
006afaa5: movaps xmm14, xmmword ptr [rsp + 0x60]
006afaab: movaps xmm13, xmmword ptr [rsp + 0x70]
006afab1: movaps xmm12, xmmword ptr [rsp + 0x80]
006afaba: movaps xmm11, xmmword ptr [rsp + 0x90]
006afac3: movaps xmm10, xmmword ptr [rsp + 0xa0]
006afacc: movaps xmm9, xmmword ptr [rsp + 0xb0]
006afad5: movaps xmm8, xmmword ptr [rsp + 0xc0]
006afade: movaps xmm7, xmmword ptr [rsp + 0xd0]
006afae6: movaps xmm6, xmmword ptr [rsp + 0xe0]
006afaee: mov r14, qword ptr [rsp + 0x118]
006afaf6: mov rsi, qword ptr [rsp + 0x110]
006afafe: mov rbp, qword ptr [rsp + 0x108]
006afb06: mov rbx, qword ptr [rsp + 0x100]
006afb0e: add rsp, 0xf0
006afb15: pop rdi
006afb16: ret
006afb17: call 0x1804fee20
006afb1c: int3
006afb1d: int3
006afb1e: int3
006afb1f: int3
