006af3d0: push rdi
006af3d2: push r12
006af3d4: push r13
006af3d6: push r14
006af3d8: push r15
006af3da: sub rsp, 0x80
006af3e1: cmp byte ptr [rip + 0x483c260], 0
006af3e8: mov rdi, rcx
006af3eb: jne 0x1806af418
006af3ed: lea rcx, [rip + 0x457bbcc]
006af3f4: call 0x1804feb70
006af3f9: lea rcx, [rip + 0x45d2050]
006af400: call 0x1804feb70
006af405: lea rcx, [rip + 0x45d211c]
006af40c: call 0x1804feb70
006af411: mov byte ptr [rip + 0x483c230], 1
006af418: mov qword ptr [rsp + 0xb0], rbx
006af420: xorps xmm0, xmm0
006af423: mov qword ptr [rsp + 0xb8], rbp
006af42b: xor r8d, r8d
006af42e: mov qword ptr [rsp + 0xc0], rsi
006af436: xor edx, edx
006af438: movaps xmmword ptr [rsp + 0x70], xmm6
006af43d: mov rcx, rdi
006af440: movaps xmmword ptr [rsp + 0x60], xmm7
006af445: movaps xmmword ptr [rsp + 0x50], xmm8
006af44b: movups xmmword ptr [rsp + 0x20], xmm0
006af450: call 0x1806b12f0
006af455: xor r9d, r9d
006af458: lea rcx, [rsp + 0x40]
006af45d: xor r8d, r8d
006af460: mov rdx, rdi
006af463: mov r13d, eax
006af466: call 0x1806b1660
006af46b: xor r12d, r12d
006af46e: xor r15d, r15d
006af471: xor r14d, r14d
006af474: xor edx, edx
006af476: movups xmm2, xmmword ptr [rax]
006af479: mov rax, qword ptr [rdi + 0x18]
006af47d: movss dword ptr [rsp + 0x30], xmm2
006af483: movaps xmm0, xmm2
006af486: shufps xmm0, xmm2, 0x55
006af48a: movaps xmm1, xmm2
006af48d: shufps xmm1, xmm2, 0xaa
006af491: shufps xmm2, xmm2, 0xff
006af495: movss dword ptr [rsp + 0x3c], xmm2
006af49b: movss dword ptr [rsp + 0x34], xmm0
006af4a1: movss dword ptr [rsp + 0x38], xmm1
006af4a7: test rax, rax
006af4aa: je 0x1806af740
006af4b0: movss xmm7, dword ptr [rip + 0x3584904]
006af4b8: mov esi, 0x46
006af4bd: movss xmm8, dword ptr [rip + 0x35847a2]
006af4c6: nop word ptr [rax + rax]
006af4d0: mov rcx, qword ptr [rax + 0x70]
006af4d4: test rcx, rcx
006af4d7: je 0x1806af740
006af4dd: cmp edx, dword ptr [rcx + 0x28]
006af4e0: jge 0x1806af73b
006af4e6: mov rax, qword ptr [rdi + 0x18]
006af4ea: test rax, rax
006af4ed: je 0x1806af740
006af4f3: mov rcx, qword ptr [rax + 0x70]
006af4f7: test rcx, rcx
006af4fa: je 0x1806af740
006af500: mov r8, qword ptr [rip + 0x45d2021]
006af507: mov edx, r14d
006af50a: call 0x182449330
006af50f: mov rbx, rax
006af512: test rax, rax
006af515: je 0x1806af740
006af51b: mov eax, dword ptr [rax + 0x34]
006af51e: sub eax, dword ptr [rdi + 0x34]
006af521: cmp dword ptr [rbx + 0x70], 0x24
006af525: je 0x1806af6eb
006af52b: xor ecx, ecx
006af52d: cmp dword ptr [rbx + 0x70], 0x19
006af531: cmovne ecx, eax
006af534: test ecx, ecx
006af536: jne 0x1806af6eb
006af53c: cmp byte ptr [rbx + 0x112], cl
006af542: je 0x1806af6eb
006af548: xor edx, edx
006af54a: mov rcx, rbx
006af54d: call 0x1806f3830
006af552: test al, al
006af554: jne 0x1806af6eb
006af55a: xor r8d, r8d
006af55d: mov edx, r13d
006af560: mov rcx, rbx
006af563: call 0x1806f12f0
006af568: test al, al
006af56a: je 0x1806af6eb
006af570: xor r8d, r8d
006af573: lea rcx, [rsp + 0x40]
006af578: mov rdx, rbx
006af57b: call 0x1806f2d70
006af580: movups xmm2, xmmword ptr [rax]
006af583: movaps xmm0, xmm2
006af586: movss dword ptr [rsp + 0x20], xmm2
006af58c: shufps xmm0, xmm2, 0x55
006af590: movaps xmm1, xmm2
006af593: movss dword ptr [rsp + 0x24], xmm0
006af599: movaps xmm0, xmm2
006af59c: shufps xmm0, xmm2, 0xff
006af5a0: shufps xmm1, xmm2, 0xaa
006af5a4: movss dword ptr [rsp + 0x2c], xmm0
006af5aa: movss dword ptr [rsp + 0x28], xmm1
006af5b0: cmp dword ptr [rbx + 0x74], 0xb
006af5b4: jne 0x1806af5ce
006af5b6: movss xmm0, dword ptr [rdi + 0x20]
006af5bb: addss xmm0, xmm7
006af5bf: comiss xmm0, xmm2
006af5c2: ja 0x1806af612
006af5c4: cmp dword ptr [rbx + 0x74], 0xb
006af5c8: je 0x1806af6eb
006af5ce: cmp dword ptr [rbx + 0x74], 0xc
006af5d2: je 0x1806af6eb
006af5d8: cmp dword ptr [rbx + 0x74], 0x3a
006af5dc: je 0x1806af6eb
006af5e2: cmp dword ptr [rbx + 0x74], 0x34
006af5e6: je 0x1806af6eb
006af5ec: cmp dword ptr [rbx + 0x74], 0x35
006af5f0: je 0x1806af6eb
006af5f6: cmp dword ptr [rbx + 0x74], 0x36
006af5fa: je 0x1806af6eb
006af600: xor edx, edx
006af602: mov rcx, rbx
006af605: call 0x1806f35c0
006af60a: test al, al
006af60c: jne 0x1806af6eb
006af612: cmp byte ptr [rbx + 0xa5], 0
006af619: mov ebp, 0x6e
006af61e: mov rcx, qword ptr [rip + 0x457b99b]
006af625: cmove ebp, esi
006af628: cmp dword ptr [rcx + 0xe4], 0
006af62f: jne 0x1806af636
006af631: call 0x1804feeb0
006af636: xor r8d, r8d
006af639: lea rdx, [rsp + 0x20]
006af63e: lea rcx, [rsp + 0x30]
006af643: call 0x1806cbaa0
006af648: mov esi, eax
006af64a: neg esi
006af64c: cmp esi, ebp
006af64e: jg 0x1806af6e6
006af654: cmp dword ptr [rbx + 0x74], 0xd
006af658: movss xmm6, dword ptr [rsp + 0x30]
006af65e: je 0x1806af696
006af660: cmp dword ptr [rbx + 0x74], 0xb
006af664: je 0x1806af696
006af666: cmp dword ptr [rbx + 0x74], 0x37
006af66a: je 0x1806af696
006af66c: cmp dword ptr [rbx + 0x70], 0x18
006af670: je 0x1806af696
006af672: cmp dword ptr [rbx + 0x70], 7
006af676: je 0x1806af696
006af678: mov rcx, qword ptr [rdi + 0x10]
006af67c: test rcx, rcx
006af67f: je 0x1806af740
006af685: xor edx, edx
006af687: call 0x1807df7e0
006af68c: test al, al
006af68e: je 0x1806af69b
006af690: movss xmm6, dword ptr [rsp + 0x30]
006af696: subss xmm6, xmm8
006af69b: xor edx, edx
006af69d: mov rcx, rbx
006af6a0: call 0x1806f3a20
006af6a5: test al, al
006af6a7: jne 0x1806af6ba
006af6a9: movss xmm0, dword ptr [rsp + 0x28]
006af6af: addss xmm0, dword ptr [rsp + 0x20]
006af6b5: comiss xmm6, xmm0
006af6b8: ja 0x1806af6e6
006af6ba: mov rcx, qword ptr [rdi + 0x18]
006af6be: test rcx, rcx
006af6c1: je 0x1806af740
006af6c3: xor r8d, r8d
006af6c6: mov rdx, rbx
006af6c9: call 0x180676eb0
006af6ce: cmp eax, dword ptr [rdi + 0x120]
006af6d4: je 0x1806af6ff
006af6d6: test r15, r15
006af6d9: je 0x1806af6e0
006af6db: cmp esi, r12d
006af6de: jge 0x1806af6e6
006af6e0: mov r15, rbx
006af6e3: mov r12d, esi
006af6e6: mov esi, 0x46
006af6eb: mov rax, qword ptr [rdi + 0x18]
006af6ef: inc r14d
006af6f2: mov edx, r14d
006af6f5: test rax, rax
006af6f8: je 0x1806af740
006af6fa: jmp 0x1806af4d0
006af6ff: mov rax, rbx
006af702: movaps xmm8, xmmword ptr [rsp + 0x50]
006af708: movaps xmm7, xmmword ptr [rsp + 0x60]
006af70d: movaps xmm6, xmmword ptr [rsp + 0x70]
006af712: mov rsi, qword ptr [rsp + 0xc0]
006af71a: mov rbp, qword ptr [rsp + 0xb8]
006af722: mov rbx, qword ptr [rsp + 0xb0]
006af72a: add rsp, 0x80
006af731: pop r15
006af733: pop r14
006af735: pop r13
006af737: pop r12
006af739: pop rdi
006af73a: ret
006af73b: mov rax, r15
006af73e: jmp 0x1806af702
006af740: call 0x1804fee20
006af745: int3
006af746: int3
006af747: int3
006af748: int3
006af749: int3
006af74a: int3
006af74b: int3
006af74c: int3
006af74d: int3
006af74e: int3
006af74f: int3
