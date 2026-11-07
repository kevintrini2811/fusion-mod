006b0040: mov qword ptr [rsp + 0x20], rbx
006b0045: push rdi
006b0046: push r12
006b0048: push r13
006b004a: push r14
006b004c: push r15
006b004e: sub rsp, 0x100
006b0055: cmp byte ptr [rip + 0x483b5e0], 0
006b005c: mov ebx, r8d
006b005f: mov r13d, edx
006b0062: mov rdi, rcx
006b0065: jne 0x1806b0092
006b0067: lea rcx, [rip + 0x457af52]
006b006e: call 0x1804feb70
006b0073: lea rcx, [rip + 0x45d13d6]
006b007a: call 0x1804feb70
006b007f: lea rcx, [rip + 0x45d14a2]
006b0086: call 0x1804feb70
006b008b: mov byte ptr [rip + 0x483b5aa], 1
006b0092: mov qword ptr [rsp + 0x130], rbp
006b009a: xorps xmm0, xmm0
006b009d: mov qword ptr [rsp + 0x138], rsi
006b00a5: xorps xmm1, xmm1
006b00a8: movaps xmmword ptr [rsp + 0xf0], xmm6
006b00b0: xor r8d, r8d
006b00b3: movaps xmmword ptr [rsp + 0xe0], xmm7
006b00bb: mov edx, ebx
006b00bd: movaps xmmword ptr [rsp + 0xd0], xmm8
006b00c6: mov rcx, rdi
006b00c9: movaps xmmword ptr [rsp + 0xc0], xmm9
006b00d2: movaps xmmword ptr [rsp + 0xb0], xmm10
006b00db: movaps xmmword ptr [rsp + 0xa0], xmm11
006b00e4: movaps xmmword ptr [rsp + 0x90], xmm12
006b00ed: movaps xmmword ptr [rsp + 0x80], xmm13
006b00f6: movaps xmmword ptr [rsp + 0x70], xmm14
006b00fc: movaps xmmword ptr [rsp + 0x60], xmm15
006b0102: movups xmmword ptr [rsp + 0x30], xmm0
006b0107: movups xmmword ptr [rsp + 0x40], xmm1
006b010c: call 0x1806b12f0
006b0111: xor r9d, r9d
006b0114: mov dword ptr [rsp + 0x140], eax
006b011b: mov r8d, ebx
006b011e: lea rcx, [rsp + 0x50]
006b0123: mov rdx, rdi
006b0126: call 0x1806b1660
006b012b: xor r12d, r12d
006b012e: xor r15d, r15d
006b0131: xor r14d, r14d
006b0134: xor edx, edx
006b0136: movups xmm0, xmmword ptr [rax]
006b0139: mov rax, qword ptr [rdi + 0x18]
006b013d: movups xmmword ptr [rsp + 0x30], xmm0
006b0142: test rax, rax
006b0145: je 0x1806b0572
006b014b: movss xmm14, dword ptr [rip + 0x3583b4c]
006b0154: movss xmm13, dword ptr [rip + 0x3583c5f]
006b015d: movss xmm12, dword ptr [rip + 0x3583c5e]
006b0166: movss xmm15, dword ptr [rip + 0x3583ad1]
006b016f: nop
006b0170: mov rcx, qword ptr [rax + 0x70]
006b0174: test rcx, rcx
006b0177: je 0x1806b0572
006b017d: cmp edx, dword ptr [rcx + 0x28]
006b0180: jge 0x1806b04f4
006b0186: mov rax, qword ptr [rdi + 0x18]
006b018a: test rax, rax
006b018d: je 0x1806b0572
006b0193: mov rcx, qword ptr [rax + 0x70]
006b0197: test rcx, rcx
006b019a: je 0x1806b0572
006b01a0: mov r8, qword ptr [rip + 0x45d1381]
006b01a7: mov edx, r14d
006b01aa: call 0x182449330
006b01af: mov rbx, rax
006b01b2: test rax, rax
006b01b5: je 0x1806b0572
006b01bb: mov ecx, dword ptr [rax + 0x34]
006b01be: xor esi, esi
006b01c0: sub ecx, r13d
006b01c3: cmp dword ptr [rax + 0x70], 0x19
006b01c7: cmovne esi, ecx
006b01ca: cmp byte ptr [rax + 0x112], 0
006b01d1: je 0x1806b01e1
006b01d3: xor edx, edx
006b01d5: mov rcx, rax
006b01d8: call 0x1806f3830
006b01dd: test al, al
006b01df: je 0x1806b01ff
006b01e1: cmp dword ptr [rdi + 0x70], 4
006b01e5: je 0x1806b04dc
006b01eb: cmp dword ptr [rdi + 0x70], 6
006b01ef: je 0x1806b04dc
006b01f5: cmp dword ptr [rdi + 0x70], 0x13
006b01f9: je 0x1806b04dc
006b01ff: mov rcx, qword ptr [rdi + 0x10]
006b0203: xor bpl, bpl
006b0206: test rcx, rcx
006b0209: je 0x1806b0572
006b020f: xor edx, edx
006b0211: call 0x1807e3010
006b0216: cmp eax, 0x1a
006b0219: jne 0x1806b022f
006b021b: cmp dword ptr [rdi + 0x70], 0
006b021f: je 0x1806b022c
006b0221: cmp dword ptr [rdi + 0x70], eax
006b0224: je 0x1806b022c
006b0226: cmp dword ptr [rdi + 0x70], 7
006b022a: jne 0x1806b022f
006b022c: mov bpl, 1
006b022f: cmp dword ptr [rdi + 0x70], 0x2b
006b0233: je 0x1806b028a
006b0235: cmp dword ptr [rdi + 0x70], 0x2a
006b0239: je 0x1806b027e
006b023b: test bpl, bpl
006b023e: jne 0x1806b024a
006b0240: test esi, esi
006b0242: jne 0x1806b04dc
006b0248: jmp 0x1806b028a
006b024a: mov rax, qword ptr [rdi + 0x18]
006b024e: test rax, rax
006b0251: je 0x1806b0572
006b0257: mov rcx, qword ptr [rax + 0x110]
006b025e: test rcx, rcx
006b0261: je 0x1806b0572
006b0267: xor r9d, r9d
006b026a: mov r8, rbx
006b026d: mov rdx, rdi
006b0270: call 0x18067db70
006b0275: test al, al
006b0277: jne 0x1806b028a
006b0279: jmp 0x1806b04dc
006b027e: lea eax, [rsi + 1]
006b0281: cmp eax, 2
006b0284: ja 0x1806b04dc
006b028a: mov edx, dword ptr [rsp + 0x140]
006b0291: xor r8d, r8d
006b0294: mov rcx, rbx
006b0297: call 0x1806f12f0
006b029c: test al, al
006b029e: je 0x1806b04dc
006b02a4: xor esi, esi
006b02a6: cmp dword ptr [rdi + 0x70], 6
006b02aa: jne 0x1806b0314
006b02ac: cmp dword ptr [rbx + 0x70], 0x24
006b02b0: je 0x1806b04dc
006b02b6: cmp dword ptr [rbx + 0x74], 0x25
006b02ba: jne 0x1806b02de
006b02bc: movss xmm0, dword ptr [rsp + 0x30]
006b02c2: addss xmm0, xmm13
006b02c7: movss dword ptr [rsp + 0x30], xmm0
006b02cd: movss xmm1, dword ptr [rsp + 0x38]
006b02d3: subss xmm1, xmm13
006b02d8: movss dword ptr [rsp + 0x38], xmm1
006b02de: cmp dword ptr [rbx + 0x74], 0x14
006b02e2: je 0x1806b04dc
006b02e8: cmp dword ptr [rbx + 0x70], 0x14
006b02ec: jne 0x1806b02fd
006b02ee: mov eax, dword ptr [rdi + 0x74]
006b02f1: cmp dword ptr [rbx + 0xd8], eax
006b02f7: je 0x1806b04dc
006b02fd: cmp byte ptr [rbx + 0xa5], sil
006b0304: jne 0x1806b030f
006b0306: cmp dword ptr [rdi + 0x88], 0xa
006b030d: jne 0x1806b0314
006b030f: mov esi, 0x3c
006b0314: cmp dword ptr [rdi + 0x70], 4
006b0318: jne 0x1806b038d
006b031a: cmp dword ptr [rbx + 0x70], 0x12
006b031e: jne 0x1806b032d
006b0320: cmp byte ptr [rbx + 0x114], 0
006b0327: jne 0x1806b04dc
006b032d: cmp dword ptr [rbx + 0x74], 0xc
006b0331: je 0x1806b04dc
006b0337: cmp dword ptr [rbx + 0x74], 0xb
006b033b: je 0x1806b04dc
006b0341: cmp dword ptr [rbx + 0x70], 3
006b0345: jne 0x1806b0369
006b0347: movss xmm0, dword ptr [rsp + 0x30]
006b034d: addss xmm0, xmm12
006b0352: movss dword ptr [rsp + 0x30], xmm0
006b0358: movss xmm1, dword ptr [rsp + 0x38]
006b035e: subss xmm1, xmm12
006b0363: movss dword ptr [rsp + 0x38], xmm1
006b0369: cmp dword ptr [rbx + 0x70], 0x14
006b036d: jne 0x1806b037e
006b036f: mov eax, dword ptr [rdi + 0x74]
006b0372: cmp dword ptr [rbx + 0xd8], eax
006b0378: jne 0x1806b04dc
006b037e: cmp byte ptr [rbx + 0xa5], 0
006b0385: mov eax, 0x1e
006b038a: cmovne esi, eax
006b038d: cmp dword ptr [rdi + 0x70], 0x31
006b0391: jne 0x1806b039d
006b0393: cmp dword ptr [rbx + 0x74], 0xc
006b0397: je 0x1806b04dc
006b039d: cmp dword ptr [rdi + 0x70], 0x13
006b03a1: jne 0x1806b03b0
006b03a3: cmp byte ptr [rbx + 0x115], 0
006b03aa: je 0x1806b04dc
006b03b0: xor r8d, r8d
006b03b3: lea rcx, [rsp + 0x50]
006b03b8: mov rdx, rbx
006b03bb: call 0x1806f2d70
006b03c0: movups xmm6, xmmword ptr [rax]
006b03c3: movss dword ptr [rsp + 0x40], xmm6
006b03c9: movaps xmm9, xmm6
006b03cd: shufps xmm9, xmm6, 0x55
006b03d2: movaps xmm7, xmm6
006b03d5: shufps xmm7, xmm6, 0xaa
006b03d9: movaps xmm8, xmm6
006b03dd: shufps xmm8, xmm6, 0xff
006b03e2: movss dword ptr [rsp + 0x44], xmm9
006b03e9: movss dword ptr [rsp + 0x48], xmm7
006b03ef: movss dword ptr [rsp + 0x4c], xmm8
006b03f6: test bpl, bpl
006b03f9: jne 0x1806b0446
006b03fb: mov rcx, qword ptr [rip + 0x457abbe]
006b0402: cmp dword ptr [rcx + 0xe4], 0
006b0409: jne 0x1806b0410
006b040b: call 0x1804feeb0
006b0410: xor r8d, r8d
006b0413: lea rdx, [rsp + 0x40]
006b0418: lea rcx, [rsp + 0x30]
006b041d: call 0x1806cbaa0
006b0422: neg esi
006b0424: cmp eax, esi
006b0426: jl 0x1806b04dc
006b042c: movss xmm8, dword ptr [rsp + 0x4c]
006b0433: movss xmm7, dword ptr [rsp + 0x48]
006b0439: movss xmm9, dword ptr [rsp + 0x44]
006b0440: movss xmm6, dword ptr [rsp + 0x40]
006b0446: cmp dword ptr [rdi + 0x70], 0x2b
006b044a: movaps xmm0, xmm6
006b044d: xorps xmm0, xmm14
006b0451: cvttss2si esi, xmm0
006b0455: jne 0x1806b04cc
006b0457: movss xmm10, dword ptr [rdi + 0x20]
006b045d: movss xmm11, dword ptr [rdi + 0x24]
006b0463: mov rcx, qword ptr [rip + 0x457ab56]
006b046a: cmp dword ptr [rcx + 0xe4], 0
006b0471: jne 0x1806b0478
006b0473: call 0x1804feeb0
006b0478: mulss xmm8, xmm15
006b047d: addss xmm11, xmm12
006b0482: mov qword ptr [rsp + 0x20], 0
006b048b: mulss xmm7, xmm15
006b0490: addss xmm10, xmm12
006b0495: addss xmm8, xmm9
006b049a: movaps xmm1, xmm11
006b049e: addss xmm7, xmm6
006b04a2: movaps xmm0, xmm10
006b04a6: movaps xmm3, xmm8
006b04aa: movaps xmm2, xmm7
006b04ad: call 0x1806cb840
006b04b2: cvttss2si esi, xmm0
006b04b6: xor edx, edx
006b04b8: mov rcx, rbx
006b04bb: neg esi
006b04bd: call 0x1806f3650
006b04c2: test al, al
006b04c4: je 0x1806b04cc
006b04c6: add esi, 0x2710
006b04cc: test r15, r15
006b04cf: je 0x1806b04d6
006b04d1: cmp esi, r12d
006b04d4: jle 0x1806b04dc
006b04d6: mov r12d, esi
006b04d9: mov r15, rbx
006b04dc: mov rax, qword ptr [rdi + 0x18]
006b04e0: inc r14d
006b04e3: mov edx, r14d
006b04e6: test rax, rax
006b04e9: je 0x1806b0572
006b04ef: jmp 0x1806b0170
006b04f4: movaps xmm15, xmmword ptr [rsp + 0x60]
006b04fa: mov rax, r15
006b04fd: movaps xmm14, xmmword ptr [rsp + 0x70]
006b0503: movaps xmm13, xmmword ptr [rsp + 0x80]
006b050c: movaps xmm12, xmmword ptr [rsp + 0x90]
006b0515: movaps xmm11, xmmword ptr [rsp + 0xa0]
006b051e: movaps xmm10, xmmword ptr [rsp + 0xb0]
006b0527: movaps xmm9, xmmword ptr [rsp + 0xc0]
006b0530: movaps xmm8, xmmword ptr [rsp + 0xd0]
006b0539: movaps xmm7, xmmword ptr [rsp + 0xe0]
006b0541: movaps xmm6, xmmword ptr [rsp + 0xf0]
006b0549: mov rsi, qword ptr [rsp + 0x138]
006b0551: mov rbp, qword ptr [rsp + 0x130]
006b0559: mov rbx, qword ptr [rsp + 0x148]
006b0561: add rsp, 0x100
006b0568: pop r15
006b056a: pop r14
006b056c: pop r13
006b056e: pop r12
006b0570: pop rdi
006b0571: ret
006b0572: call 0x1804fee20
006b0577: int3
006b0578: int3
006b0579: int3
006b057a: int3
006b057b: int3
006b057c: int3
006b057d: int3
006b057e: int3
006b057f: int3
