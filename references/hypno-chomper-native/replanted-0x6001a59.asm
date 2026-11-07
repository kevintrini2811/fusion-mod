006bdc80: push rbx
006bdc82: sub rsp, 0x30
006bdc86: cmp byte ptr [rip + 0x482d9df], 0
006bdc8d: mov rbx, rcx
006bdc90: jne 0x1806bdcb1
006bdc92: lea rcx, [rip + 0x45b74cf]
006bdc99: call 0x1804feb70
006bdc9e: lea rcx, [rip + 0x45afc5b]
006bdca5: call 0x1804feb70
006bdcaa: mov byte ptr [rip + 0x482d9bb], 1
006bdcb1: cmp dword ptr [rbx + 0x88], 0x22
006bdcb8: mov qword ptr [rsp + 0x40], rsi
006bdcbd: mov qword ptr [rsp + 0x48], rdi
006bdcc2: jne 0x1806bde47
006bdcc8: cmp dword ptr [rbx + 0xa0], 0x32
006bdccf: jne 0x1806bdd0b
006bdcd1: mov rcx, qword ptr [rbx + 0x18]
006bdcd5: test rcx, rcx
006bdcd8: je 0x1806bdf58
006bdcde: mov edx, dword ptr [rbx + 0x120]
006bdce4: xor r8d, r8d
006bdce7: call 0x180677050
006bdcec: mov rdi, rax
006bdcef: test rax, rax
006bdcf2: je 0x1806bdd0b
006bdcf4: xor edx, edx
006bdcf6: mov rcx, rax
006bdcf9: call 0x1806ec590
006bdcfe: xor r8d, r8d
006bdd01: xor edx, edx
006bdd03: mov rcx, rdi
006bdd06: call 0x1806f5850
006bdd0b: cmp dword ptr [rbx + 0xa0], 0x14
006bdd12: jne 0x1806bddfa
006bdd18: mov edx, dword ptr [rbx + 0x34]
006bdd1b: xor r9d, r9d
006bdd1e: mov r8d, 6
006bdd24: mov ecx, 0x4af38
006bdd29: call 0x180665070
006bdd2e: mov rcx, qword ptr [rbx + 0x10]
006bdd32: mov edi, eax
006bdd34: test rcx, rcx
006bdd37: je 0x1806bdf58
006bdd3d: movss xmm2, dword ptr [rbx + 0x24]
006bdd42: xor esi, esi
006bdd44: movss xmm1, dword ptr [rbx + 0x20]
006bdd49: mov r9d, eax
006bdd4c: addss xmm2, dword ptr [rip + 0x3576184]
006bdd54: subss xmm1, dword ptr [rip + 0x3576190]
006bdd5c: mov qword ptr [rsp + 0x28], rsi
006bdd61: mov dword ptr [rsp + 0x20], 0x20
006bdd69: call 0x1807d96d0
006bdd6e: test rax, rax
006bdd71: je 0x1806bdf58
006bdd77: movss xmm1, dword ptr [rip + 0x3576011]
006bdd7f: xor r9d, r9d
006bdd82: movaps xmm2, xmm1
006bdd85: mov rcx, rax
006bdd88: call 0x1806ca790
006bdd8d: mov rcx, qword ptr [rbx + 0x10]
006bdd91: test rcx, rcx
006bdd94: je 0x1806bdf58
006bdd9a: movss xmm2, dword ptr [rbx + 0x24]
006bdd9f: mov r9d, edi
006bdda2: movss xmm1, dword ptr [rbx + 0x20]
006bdda7: addss xmm2, dword ptr [rip + 0x3576239]
006bddaf: addss xmm1, dword ptr [rip + 0x3576209]
006bddb7: mov qword ptr [rsp + 0x28], rsi
006bddbc: mov dword ptr [rsp + 0x20], 0x16
006bddc4: call 0x1807d9b50
006bddc9: mov r8, qword ptr [rbx + 0x10]
006bddcd: test r8, r8
006bddd0: je 0x1806bdf58
006bddd6: mov r8, qword ptr [r8 + 0x58]
006bddda: test r8, r8
006bdddd: je 0x1806bdf58
006bdde3: mov rdx, qword ptr [rip + 0x45b737e]
006bddea: mov ecx, 6
006bddef: mov r9d, 0x35
006bddf5: call 0x180002040
006bddfa: cmp dword ptr [rbx + 0xa0], 0
006bde01: jne 0x1806bde37
006bde03: xor edx, edx
006bde05: mov rcx, rbx
006bde08: call 0x1806acd40
006bde0d: mov rcx, qword ptr [rbx + 0x18]
006bde11: test rcx, rcx
006bde14: je 0x1806bdf58
006bde1a: mov edx, dword ptr [rbx + 0x120]
006bde20: xor r8d, r8d
006bde23: call 0x180677050
006bde28: test rax, rax
006bde2b: je 0x1806bde37
006bde2d: xor edx, edx
006bde2f: mov rcx, rax
006bde32: call 0x1806ec320
006bde37: mov rdi, qword ptr [rsp + 0x48]
006bde3c: mov rsi, qword ptr [rsp + 0x40]
006bde41: add rsp, 0x30
006bde45: pop rbx
006bde46: ret
006bde47: mov edx, dword ptr [rbx + 0x34]
006bde4a: xor r9d, r9d
006bde4d: xor r8d, r8d
006bde50: mov rcx, rbx
006bde53: call 0x1806b0040
006bde58: mov rdi, rax
006bde5b: test rax, rax
006bde5e: je 0x1806bde37
006bde60: mov rax, qword ptr [rbx + 0x10]
006bde64: test rax, rax
006bde67: je 0x1806bdf58
006bde6d: mov r8, qword ptr [rax + 0x58]
006bde71: test r8, r8
006bde74: je 0x1806bdf58
006bde7a: mov rdx, qword ptr [rip + 0x45b72e7]
006bde81: mov ecx, 6
006bde86: mov r9d, 0x44
006bde8c: call 0x180002040
006bde91: xor r8d, r8d
006bde94: mov dword ptr [rbx + 0x88], 0x22
006bde9e: xor edx, edx
006bdea0: mov dword ptr [rbx + 0xa0], 0x64
006bdeaa: mov rcx, rdi
006bdead: call 0x1806f5850
006bdeb2: mov rcx, qword ptr [rdi + 0x1d0]
006bdeb9: test rcx, rcx
006bdebc: je 0x1806bdf58
006bdec2: xor edx, edx
006bdec4: call 0x1807c8d30
006bdec9: mov rcx, qword ptr [rbx + 0x10]
006bdecd: test rcx, rcx
006bded0: je 0x1806bdf58
006bded6: mov r9d, dword ptr [rdi + 0x38]
006bdeda: xor esi, esi
006bdedc: mov qword ptr [rsp + 0x28], rsi
006bdee1: xorps xmm2, xmm2
006bdee4: xorps xmm1, xmm1
006bdee7: mov dword ptr [rsp + 0x20], 0x98
006bdeef: call 0x1807d96d0
006bdef4: mov rsi, rax
006bdef7: test rax, rax
006bdefa: je 0x1806bdf2e
006bdefc: mov ecx, dword ptr [rax + 0x6c]
006bdeff: xor edx, edx
006bdf01: call 0x1806be7d0
006bdf06: mov dword ptr [rbx + 0x13c], eax
006bdf0c: mov rcx, qword ptr [rsi + 0xd0]
006bdf13: test rcx, rcx
006bdf16: je 0x1806bdf58
006bdf18: mov r8, qword ptr [rip + 0x45af9e1]
006bdf1f: xor r9d, r9d
006bdf22: mov rdx, qword ptr [rdi + 0x1d0]
006bdf29: call 0x1807c7500
006bdf2e: mov rcx, qword ptr [rbx + 0x18]
006bdf32: test rcx, rcx
006bdf35: je 0x1806bdf58
006bdf37: xor r8d, r8d
006bdf3a: mov rdx, rdi
006bdf3d: call 0x180676eb0
006bdf42: mov rdi, qword ptr [rsp + 0x48]
006bdf47: mov rsi, qword ptr [rsp + 0x40]
006bdf4c: mov dword ptr [rbx + 0x120], eax
006bdf52: add rsp, 0x30
006bdf56: pop rbx
006bdf57: ret
006bdf58: call 0x1804fee20
006bdf5d: int3
006bdf5e: int3
006bdf5f: int3
