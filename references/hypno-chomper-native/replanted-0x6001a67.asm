006af170: mov qword ptr [rsp + 0x20], rsi
006af175: push rdi
006af176: push r12
006af178: push r15
006af17a: sub rsp, 0xa0
006af181: cmp byte ptr [rip + 0x483c4eb], 0
006af188: mov rdi, rcx
006af18b: movaps xmmword ptr [rsp + 0x70], xmm8
006af191: jne 0x1806af1be
006af193: lea rcx, [rip + 0x457be26]
006af19a: call 0x1804feb70
006af19f: lea rcx, [rip + 0x45cf00a]
006af1a6: call 0x1804feb70
006af1ab: lea rcx, [rip + 0x45cf0d6]
006af1b2: call 0x1804feb70
006af1b7: mov byte ptr [rip + 0x483c4b5], 1
006af1be: mov rax, qword ptr [rdi + 0x18]
006af1c2: xor r12d, r12d
006af1c5: mov qword ptr [rsp + 0xc0], rbx
006af1cd: mov r15d, r12d
006af1d0: mov qword ptr [rsp + 0xc8], rbp
006af1d8: xorps xmm8, xmm8
006af1dc: mov qword ptr [rsp + 0xd0], r14
006af1e4: mov esi, r12d
006af1e7: movaps xmmword ptr [rsp + 0x90], xmm6
006af1ef: mov edx, r12d
006af1f2: movaps xmmword ptr [rsp + 0x80], xmm7
006af1fa: movaps xmmword ptr [rsp + 0x60], xmm9
006af200: movaps xmmword ptr [rsp + 0x50], xmm10
006af206: movaps xmmword ptr [rsp + 0x40], xmm11
006af20c: movaps xmmword ptr [rsp + 0x30], xmm12
006af212: test rax, rax
006af215: je 0x1806af3c4
006af21b: movss xmm11, dword ptr [rip + 0x3584a1c]
006af224: movss xmm12, dword ptr [rip + 0x3584b97]
006af22d: nop dword ptr [rax]
006af230: mov rcx, qword ptr [rax + 0x88]
006af237: test rcx, rcx
006af23a: je 0x1806af3c4
006af240: cmp edx, dword ptr [rcx + 0x28]
006af243: jge 0x1806af366
006af249: mov rax, qword ptr [rdi + 0x18]
006af24d: test rax, rax
006af250: je 0x1806af3c4
006af256: mov rcx, qword ptr [rax + 0x88]
006af25d: test rcx, rcx
006af260: je 0x1806af3c4
006af266: mov r8, qword ptr [rip + 0x45cf01b]
006af26d: mov edx, esi
006af26f: call 0x182449330
006af274: mov rbx, rax
006af277: test rax, rax
006af27a: je 0x1806af3c4
006af280: cmp byte ptr [rax + 0x8c], r12b
006af287: jne 0x1806af354
006af28d: cmp dword ptr [rax + 0xb0], 1
006af294: je 0x1806af2ac
006af296: cmp dword ptr [rax + 0xb0], 2
006af29d: je 0x1806af2ac
006af29f: cmp dword ptr [rax + 0xb0], 3
006af2a6: jne 0x1806af354
006af2ac: cmp dword ptr [rax + 0xb4], 5
006af2b3: je 0x1806af354
006af2b9: cmp byte ptr [rax + 0xa8], r12b
006af2c0: jne 0x1806af354
006af2c6: cmp dword ptr [rax + 0xa4], 0x32
006af2cd: jl 0x1806af354
006af2d3: mov rcx, qword ptr [rip + 0x457bce6]
006af2da: movss xmm6, dword ptr [rdi + 0x20]
006af2df: movss xmm7, dword ptr [rdi + 0x24]
006af2e4: movss xmm9, dword ptr [rax + 0x70]
006af2ea: mov ebp, dword ptr [rax + 0x28]
006af2ed: movss xmm10, dword ptr [rax + 0x74]
006af2f3: mov r14d, dword ptr [rax + 0x2c]
006af2f7: cmp dword ptr [rcx + 0xe4], r12d
006af2fe: jne 0x1806af305
006af300: call 0x1804feeb0
006af305: xorps xmm3, xmm3
006af308: mov qword ptr [rsp + 0x20], r12
006af30d: xorps xmm2, xmm2
006af310: addss xmm7, xmm12
006af315: cvtsi2ss xmm3, r14d
006af31a: cvtsi2ss xmm2, ebp
006af31e: mulss xmm3, xmm11
006af323: addss xmm6, xmm12
006af328: mulss xmm2, xmm11
006af32d: movaps xmm1, xmm7
006af330: addss xmm3, xmm10
006af335: movaps xmm0, xmm6
006af338: addss xmm2, xmm9
006af33d: call 0x1806cb840
006af342: test r15, r15
006af345: je 0x1806af34d
006af347: comiss xmm8, xmm0
006af34b: jbe 0x1806af354
006af34d: mov r15, rbx
006af350: movaps xmm8, xmm0
006af354: mov rax, qword ptr [rdi + 0x18]
006af358: inc esi
006af35a: mov edx, esi
006af35c: test rax, rax
006af35f: je 0x1806af3c4
006af361: jmp 0x1806af230
006af366: movaps xmm12, xmmword ptr [rsp + 0x30]
006af36c: mov rax, r15
006af36f: movaps xmm11, xmmword ptr [rsp + 0x40]
006af375: movaps xmm10, xmmword ptr [rsp + 0x50]
006af37b: movaps xmm9, xmmword ptr [rsp + 0x60]
006af381: movaps xmm7, xmmword ptr [rsp + 0x80]
006af389: movaps xmm6, xmmword ptr [rsp + 0x90]
006af391: mov r14, qword ptr [rsp + 0xd0]
006af399: mov rbp, qword ptr [rsp + 0xc8]
006af3a1: mov rbx, qword ptr [rsp + 0xc0]
006af3a9: mov rsi, qword ptr [rsp + 0xd8]
006af3b1: movaps xmm8, xmmword ptr [rsp + 0x70]
006af3b7: add rsp, 0xa0
006af3be: pop r15
006af3c0: pop r12
006af3c2: pop rdi
006af3c3: ret
006af3c4: call 0x1804fee20
006af3c9: int3
006af3ca: int3
006af3cb: int3
006af3cc: int3
006af3cd: int3
006af3ce: int3
006af3cf: int3
