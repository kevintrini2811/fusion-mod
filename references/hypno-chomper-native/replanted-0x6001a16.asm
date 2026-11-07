006b0580: push rbx
006b0582: push rbp
006b0583: push r14
006b0585: push r15
006b0587: sub rsp, 0xa8
006b058e: cmp byte ptr [rip + 0x483b0a6], 0
006b0595: mov r14d, r9d
006b0598: mov r15d, r8d
006b059b: mov rbp, rdx
006b059e: mov rbx, rcx
006b05a1: jne 0x1806b05c2
006b05a3: lea rcx, [rip + 0x45c4bbe]
006b05aa: call 0x1804feb70
006b05af: lea rcx, [rip + 0x45ccd9a]
006b05b6: call 0x1804feb70
006b05bb: mov byte ptr [rip + 0x483b079], 1
006b05c2: cmp dword ptr [rbx + 0x70], 0xa
006b05c6: mov qword ptr [rsp + 0xd8], rsi
006b05ce: mov qword ptr [rsp + 0xe0], rdi
006b05d6: mov qword ptr [rsp + 0xa0], r12
006b05de: movaps xmmword ptr [rsp + 0x90], xmm6
006b05e6: movaps xmmword ptr [rsp + 0x80], xmm7
006b05ee: movaps xmmword ptr [rsp + 0x70], xmm8
006b05f4: movaps xmmword ptr [rsp + 0x60], xmm9
006b05fa: movaps xmmword ptr [rsp + 0x50], xmm10
006b0600: je 0x1806b1022
006b0606: cmp dword ptr [rbx + 0x70], 0x2a
006b060a: je 0x1806b1007
006b0610: cmp dword ptr [rbx + 0x70], 0x1d
006b0614: je 0x1806b0ff8
006b061a: mov edx, dword ptr [rbx + 0x70]
006b061d: xor r12d, r12d
006b0620: cmp edx, 0x12
006b0623: jle 0x1806b06c8
006b0629: cmp edx, 0x20
006b062c: jle 0x1806b068e
006b062e: cmp edx, 0x22
006b0631: jne 0x1806b063d
006b0633: mov edi, 0xa
006b0638: jmp 0x1806b071a
006b063d: lea eax, [rdx - 0x27]
006b0640: cmp eax, 8
006b0643: ja 0x1806b067d
006b0645: lea eax, [rdx - 0x27]
006b0648: movsxd rcx, eax
006b064b: lea r8, [rip - 0x6b0652]
006b0652: mov eax, dword ptr [r8 + rcx*4 + 0x6b10f0]
006b065a: add rax, r8
006b065d: jmp rax
006b065f: mov edi, 3
006b0664: jmp 0x1806b071a
006b0669: mov edi, 5
006b066e: jmp 0x1806b071a
006b0673: mov edi, 0xb
006b0678: jmp 0x1806b071a
006b067d: cmp edx, 0x34
006b0680: jne 0x1806b106c
006b0686: mov edi, r12d
006b0689: jmp 0x1806b071a
006b068e: lea ecx, [rdx - 0x18]
006b0691: test ecx, ecx
006b0693: je 0x1806b070e
006b0695: sub ecx, 1
006b0698: je 0x1806b106c
006b069e: sub ecx, 1
006b06a1: je 0x1806b06c1
006b06a3: sub ecx, 1
006b06a6: je 0x1806b106c
006b06ac: cmp ecx, 1
006b06af: je 0x1806b0686
006b06b1: cmp edx, 0x20
006b06b4: jne 0x1806b106c
006b06ba: mov edi, 2
006b06bf: jmp 0x1806b071a
006b06c1: mov edi, 8
006b06c6: jmp 0x1806b071a
006b06c8: cmp edx, 8
006b06cb: jle 0x1806b06e7
006b06cd: cmp edx, 0xd
006b06d0: jne 0x1806b06d9
006b06d2: mov edi, 4
006b06d7: jmp 0x1806b071a
006b06d9: cmp edx, 0x12
006b06dc: jne 0x1806b106c
006b06e2: mov edi, r12d
006b06e5: jmp 0x1806b071a
006b06e7: test edx, edx
006b06e9: jne 0x1806b06f2
006b06eb: mov edi, 0xe
006b06f0: jmp 0x1806b071a
006b06f2: add edx, -5
006b06f5: je 0x1806b0715
006b06f7: sub edx, 1
006b06fa: je 0x1806b106c
006b0700: sub edx, 1
006b0703: je 0x1806b0686
006b0705: cmp edx, 1
006b0708: jne 0x1806b106c
006b070e: mov edi, 4
006b0713: jmp 0x1806b071a
006b0715: mov edi, 1
006b071a: cmp dword ptr [rbx + 0x70], 0x22
006b071e: jne 0x1806b07af
006b0724: cmp r14d, 1
006b0728: jne 0x1806b072f
006b072a: mov edi, 0xc
006b072f: mov rax, qword ptr [rbx + 0x10]
006b0733: test rax, rax
006b0736: je 0x1806b1066
006b073c: mov r8, qword ptr [rax + 0x58]
006b0740: test r8, r8
006b0743: je 0x1806b1066
006b0749: mov r9d, 3
006b074f: mov rdx, qword ptr [rip + 0x45c4a12]
006b0756: mov ecx, 6
006b075b: call 0x180002040
006b0760: cmp dword ptr [rbx + 0x70], 5
006b0764: je 0x1806b086e
006b076a: cmp dword ptr [rbx + 0x70], 0x2c
006b076e: je 0x1806b086e
006b0774: cmp dword ptr [rbx + 0x70], 8
006b0778: je 0x1806b078a
006b077a: cmp dword ptr [rbx + 0x70], 0xd
006b077e: je 0x1806b078a
006b0780: cmp dword ptr [rbx + 0x70], 0x18
006b0784: jne 0x1806b089f
006b078a: mov rax, qword ptr [rbx + 0x10]
006b078e: test rax, rax
006b0791: je 0x1806b1066
006b0797: mov r8, qword ptr [rax + 0x58]
006b079b: test r8, r8
006b079e: je 0x1806b1066
006b07a4: mov r9d, 0x1d
006b07aa: jmp 0x1806b088e
006b07af: cmp dword ptr [rbx + 0x70], r12d
006b07b3: jne 0x1806b072f
006b07b9: mov rsi, qword ptr [rbx + 0x158]
006b07c0: test rsi, rsi
006b07c3: je 0x1806b1066
006b07c9: mov r10, qword ptr [rsi]
006b07cc: movzx ecx, r12w
006b07d0: mov r9, qword ptr [rip + 0x45ccb79]
006b07d7: movzx edx, word ptr [r10 + 0x12e]
006b07df: cmp r12w, dx
006b07e3: jae 0x1806b0804
006b07e5: mov r8, qword ptr [r10 + 0xb0]
006b07ec: nop dword ptr [rax]
006b07f0: movzx eax, cx
006b07f3: add rax, rax
006b07f6: cmp qword ptr [r8 + rax*8], r9
006b07fa: je 0x1806b084f
006b07fc: inc cx
006b07ff: cmp cx, dx
006b0802: jb 0x1806b07f0
006b0804: mov r8d, 7
006b080a: mov rdx, r9
006b080d: mov rcx, rsi
006b0810: call 0x1804fbc00
006b0815: mov r8, qword ptr [rax]
006b0818: mov rcx, rsi
006b081b: mov rdx, qword ptr [rax + 8]
006b081f: call r8
006b0822: test al, al
006b0824: je 0x1806b072f
006b082a: mov rax, qword ptr [rbx + 0x10]
006b082e: test rax, rax
006b0831: je 0x1806b1066
006b0837: mov r8, qword ptr [rax + 0x58]
006b083b: test r8, r8
006b083e: je 0x1806b1066
006b0844: mov r9d, 0x6b
006b084a: jmp 0x1806b074f
006b084f: movzx ecx, cx
006b0852: add rcx, rcx
006b0855: mov eax, dword ptr [r8 + rcx*8 + 8]
006b085a: add eax, 7
006b085d: cdqe
006b085f: shl rax, 4
006b0863: add rax, 0x138
006b0869: add rax, r10
006b086c: jmp 0x1806b0815
006b086e: mov rax, qword ptr [rbx + 0x10]
006b0872: test rax, rax
006b0875: je 0x1806b1066
006b087b: mov r8, qword ptr [rax + 0x58]
006b087f: test r8, r8
006b0882: je 0x1806b1066
006b0888: mov r9d, 0x1b
006b088e: mov rdx, qword ptr [rip + 0x45c48d3]
006b0895: mov ecx, 6
006b089a: call 0x180002040
006b089f: movss xmm8, dword ptr [rip + 0x358370c]
006b08a8: xor sil, sil
006b08ab: cmp dword ptr [rbx + 0x70], 8
006b08af: movss xmm10, dword ptr [rip + 0x358350c]
006b08b8: je 0x1806b0b8b
006b08be: cmp dword ptr [rbx + 0x70], 0x18
006b08c2: je 0x1806b0b6f
006b08c8: cmp dword ptr [rbx + 0x70], 0x20
006b08cc: je 0x1806b0b53
006b08d2: cmp dword ptr [rbx + 0x70], 0x27
006b08d6: je 0x1806b0b37
006b08dc: cmp dword ptr [rbx + 0x70], 0x2c
006b08e0: je 0x1806b0b37
006b08e6: cmp dword ptr [rbx + 0x70], 0x2b
006b08ea: je 0x1806b0b1b
006b08f0: cmp dword ptr [rbx + 0x70], 0x22
006b08f4: jne 0x1806b093f
006b08f6: test r14d, r14d
006b08f9: jne 0x1806b091a
006b08fb: movss xmm6, dword ptr [rbx + 0x20]
006b0900: movss xmm7, dword ptr [rbx + 0x24]
006b0905: addss xmm6, dword ptr [rip + 0x358369b]
006b090d: subss xmm7, dword ptr [rip + 0x35836bb]
006b0915: jmp 0x1806b0b9f
006b091a: cmp r14d, 1
006b091e: jne 0x1806b095d
006b0920: movss xmm6, dword ptr [rbx + 0x20]
006b0925: movss xmm7, dword ptr [rbx + 0x24]
006b092a: addss xmm6, dword ptr [rip + 0x3583482]
006b0932: subss xmm7, dword ptr [rip + 0x35836a6]
006b093a: jmp 0x1806b0b9f
006b093f: cmp dword ptr [rbx + 0x70], r12d
006b0943: je 0x1806b0ad5
006b0949: cmp dword ptr [rbx + 0x70], 5
006b094d: je 0x1806b0ad5
006b0953: cmp dword ptr [rbx + 0x70], 7
006b0957: je 0x1806b0ad5
006b095d: cmp dword ptr [rbx + 0x70], 0x34
006b0961: je 0x1806b0aae
006b0967: cmp dword ptr [rbx + 0x70], 0x28
006b096b: je 0x1806b0a84
006b0971: cmp dword ptr [rbx + 0x70], 0x1c
006b0975: je 0x1806b0a24
006b097b: cmp dword ptr [rbx + 0x70], 0x12
006b097f: movss xmm6, dword ptr [rbx + 0x20]
006b0984: movss xmm7, dword ptr [rbx + 0x24]
006b0989: je 0x1806b0a0f
006b098f: cmp dword ptr [rbx + 0x70], 0xd
006b0993: je 0x1806b09fa
006b0995: cmp dword ptr [rbx + 0x70], 0x1a
006b0999: je 0x1806b09cb
006b099b: cmp dword ptr [rbx + 0x70], 0x2f
006b099f: je 0x1806b09b6
006b09a1: addss xmm6, dword ptr [rip + 0x3583403]
006b09a9: addss xmm7, dword ptr [rip + 0x3583523]
006b09b1: jmp 0x1806b0b9f
006b09b6: subss xmm6, dword ptr [rip + 0x358361a]
006b09be: subss xmm7, dword ptr [rip + 0x3583632]
006b09c6: jmp 0x1806b0b9f
006b09cb: test r14d, r14d
006b09ce: je 0x1806b09e5
006b09d0: addss xmm6, dword ptr [rip + 0x3583528]
006b09d8: addss xmm7, dword ptr [rip + 0x358350c]
006b09e0: jmp 0x1806b0b9f
006b09e5: addss xmm6, dword ptr [rip + 0x3583607]
006b09ed: subss xmm7, dword ptr [rip + 0x35833d3]
006b09f5: jmp 0x1806b0b9f
006b09fa: addss xmm6, dword ptr [rip + 0x35835ba]
006b0a02: addss xmm7, dword ptr [rip + 0x35834de]
006b0a0a: jmp 0x1806b0b9f
006b0a0f: addss xmm7, dword ptr [rip + 0x3583395]
006b0a17: addss xmm6, dword ptr [rip + 0x3583409]
006b0a1f: jmp 0x1806b0b9f
006b0a24: xor edx, edx
006b0a26: mov rcx, rbx
006b0a29: call 0x1806b15d0
006b0a2e: movss xmm0, dword ptr [rbx + 0x20]
006b0a33: mov qword ptr [rsp + 0xd0], rax
006b0a3b: movss xmm7, dword ptr [rsp + 0xd4]
006b0a44: addss xmm7, dword ptr [rbx + 0x24]
006b0a49: subss xmm7, dword ptr [rip + 0x3583577]
006b0a51: movss xmm6, dword ptr [rsp + 0xd0]
006b0a5a: addss xmm6, xmm0
006b0a5e: cmp r14d, 1
006b0a62: je 0x1806b0a74
006b0a64: addss xmm6, dword ptr [rip + 0x3583484]
006b0a6c: mov sil, 1
006b0a6f: jmp 0x1806b0b9f
006b0a74: subss xmm6, dword ptr [rip + 0x358356c]
006b0a7c: mov sil, 1
006b0a7f: jmp 0x1806b0b9f
006b0a84: xor edx, edx
006b0a86: mov rcx, rbx
006b0a89: call 0x1806b15d0
006b0a8e: mov qword ptr [rsp + 0xd0], rax
006b0a96: movss xmm6, dword ptr [rsp + 0xd0]
006b0a9f: addss xmm6, dword ptr [rbx + 0x20]
006b0aa4: addss xmm6, dword ptr [rip + 0x3583520]
006b0aac: jmp 0x1806b0afd
006b0aae: xor edx, edx
006b0ab0: mov rcx, rbx
006b0ab3: call 0x1806b15d0
006b0ab8: movss xmm6, dword ptr [rbx + 0x20]
006b0abd: mov qword ptr [rsp + 0xd0], rax
006b0ac5: subss xmm6, dword ptr [rsp + 0xd0]
006b0ace: addss xmm6, xmm8
006b0ad3: jmp 0x1806b0afd
006b0ad5: xor edx, edx
006b0ad7: mov rcx, rbx
006b0ada: call 0x1806b15d0
006b0adf: mov qword ptr [rsp + 0xd0], rax
006b0ae7: movss xmm6, dword ptr [rsp + 0xd0]
006b0af0: addss xmm6, dword ptr [rbx + 0x20]
006b0af5: addss xmm6, dword ptr [rip + 0x35833f3]
006b0afd: movss xmm7, dword ptr [rsp + 0xd4]
006b0b06: addss xmm7, dword ptr [rbx + 0x24]
006b0b0b: subss xmm7, dword ptr [rip + 0x35834b5]
006b0b13: mov sil, 1
006b0b16: jmp 0x1806b0b9f
006b0b1b: movss xmm6, dword ptr [rbx + 0x20]
006b0b20: movss xmm7, dword ptr [rbx + 0x24]
006b0b25: addss xmm6, dword ptr [rip + 0x358328f]
006b0b2d: subss xmm7, dword ptr [rip + 0x3583267]
006b0b35: jmp 0x1806b0b9f
006b0b37: movss xmm6, dword ptr [rbx + 0x20]
006b0b3c: movss xmm7, dword ptr [rbx + 0x24]
006b0b41: addss xmm6, dword ptr [rip + 0x3583277]
006b0b49: subss xmm7, dword ptr [rip + 0x358348b]
006b0b51: jmp 0x1806b0b9f
006b0b53: movss xmm6, dword ptr [rbx + 0x20]
006b0b58: movss xmm7, dword ptr [rbx + 0x24]
006b0b5d: addss xmm6, dword ptr [rip + 0x358336f]
006b0b65: subss xmm7, dword ptr [rip + 0x3583247]
006b0b6d: jmp 0x1806b0b9f
006b0b6f: movss xmm6, dword ptr [rbx + 0x20]
006b0b74: movss xmm7, dword ptr [rbx + 0x24]
006b0b79: addss xmm6, dword ptr [rip + 0x35832a7]
006b0b81: addss xmm7, dword ptr [rip + 0x358345b]
006b0b89: jmp 0x1806b0b9f
006b0b8b: movss xmm6, dword ptr [rbx + 0x20]
006b0b90: movss xmm7, dword ptr [rbx + 0x24]
006b0b95: addss xmm6, xmm10
006b0b9a: addss xmm7, xmm10
006b0b9f: mov rcx, qword ptr [rbx + 0x18]
006b0ba3: test rcx, rcx
006b0ba6: je 0x1806b1066
006b0bac: mov r8d, dword ptr [rbx + 0x34]
006b0bb0: xor r9d, r9d
006b0bb3: mov edx, dword ptr [rbx + 0x74]
006b0bb6: call 0x18065de10
006b0bbb: test rax, rax
006b0bbe: je 0x1806b0bcd
006b0bc0: test sil, sil
006b0bc3: jne 0x1806b0bcd
006b0bc5: addss xmm7, dword ptr [rip + 0x358335b]
006b0bcd: cmp dword ptr [rbx + 0x70], 5
006b0bd1: je 0x1806b0c5e
006b0bd7: cmp dword ptr [rbx + 0x70], 8
006b0bdb: je 0x1806b0c21
006b0bdd: cmp dword ptr [rbx + 0x70], 0xd
006b0be1: jne 0x1806b0cac
006b0be7: mov edx, dword ptr [rbx + 0x34]
006b0bea: xor r9d, r9d
006b0bed: mov r8d, 1
006b0bf3: mov ecx, 0x4ab50
006b0bf8: call 0x180665070
006b0bfd: mov rcx, qword ptr [rbx + 0x10]
006b0c01: test rcx, rcx
006b0c04: je 0x1806b1066
006b0c0a: movaps xmm1, xmm6
006b0c0d: mov qword ptr [rsp + 0x28], r12
006b0c12: addss xmm1, xmm8
006b0c17: mov dword ptr [rsp + 0x20], 0x33
006b0c1f: jmp 0x1806b0c99
006b0c21: mov edx, dword ptr [rbx + 0x34]
006b0c24: xor r9d, r9d
006b0c27: mov r8d, 1
006b0c2d: mov ecx, 0x4ab50
006b0c32: call 0x180665070
006b0c37: mov rcx, qword ptr [rbx + 0x10]
006b0c3b: test rcx, rcx
006b0c3e: je 0x1806b1066
006b0c44: movaps xmm1, xmm6
006b0c47: mov qword ptr [rsp + 0x28], r12
006b0c4c: addss xmm1, dword ptr [rip + 0x3583290]
006b0c54: mov dword ptr [rsp + 0x20], 0x33
006b0c5c: jmp 0x1806b0c99
006b0c5e: mov edx, dword ptr [rbx + 0x34]
006b0c61: xor r9d, r9d
006b0c64: mov r8d, 1
006b0c6a: mov ecx, 0x4ab50
006b0c6f: call 0x180665070
006b0c74: mov rcx, qword ptr [rbx + 0x10]
006b0c78: test rcx, rcx
006b0c7b: je 0x1806b1066
006b0c81: movaps xmm1, xmm6
006b0c84: mov qword ptr [rsp + 0x28], r12
006b0c89: addss xmm1, dword ptr [rip + 0x3583113]
006b0c91: mov dword ptr [rsp + 0x20], 0x2c
006b0c99: movaps xmm2, xmm7
006b0c9c: mov r9d, eax
006b0c9f: addss xmm2, dword ptr [rip + 0x3583235]
006b0ca7: call 0x1807d9b50
006b0cac: mov rcx, qword ptr [rbx + 0x18]
006b0cb0: test rcx, rcx
006b0cb3: je 0x1806b1066
006b0cb9: mov r9d, dword ptr [rbx + 0x38]
006b0cbd: movaps xmm2, xmm7
006b0cc0: mov qword ptr [rsp + 0x30], r12
006b0cc5: dec r9d
006b0cc8: mov dword ptr [rsp + 0x28], edi
006b0ccc: movaps xmm1, xmm6
006b0ccf: mov dword ptr [rsp + 0x20], r15d
006b0cd4: call 0x180655650
006b0cd9: mov rdi, rax
006b0cdc: test rax, rax
006b0cdf: je 0x1806b1066
006b0ce5: xor r8d, r8d
006b0ce8: mov edx, r14d
006b0ceb: mov rcx, rbx
006b0cee: call 0x1806b12f0
006b0cf3: mov dword ptr [rdi + 0xc0], eax
006b0cf9: cmp dword ptr [rbx + 0x70], 0x20
006b0cfd: je 0x1806b0ed9
006b0d03: cmp dword ptr [rbx + 0x70], 0x22
006b0d07: je 0x1806b0ed9
006b0d0d: cmp dword ptr [rbx + 0x70], 0x27
006b0d11: je 0x1806b0ed9
006b0d17: cmp dword ptr [rbx + 0x70], 0x2c
006b0d1b: je 0x1806b0ed9
006b0d21: cmp dword ptr [rbx + 0x70], 0x12
006b0d25: je 0x1806b0e33
006b0d2b: cmp dword ptr [rbx + 0x70], 8
006b0d2f: je 0x1806b0e27
006b0d35: cmp dword ptr [rbx + 0x70], 0x18
006b0d39: je 0x1806b0e27
006b0d3f: cmp dword ptr [rbx + 0x70], 0x1c
006b0d43: jne 0x1806b0d5e
006b0d45: cmp r14d, 1
006b0d49: jne 0x1806b0e6b
006b0d4f: mov dword ptr [rdi + 0xa4], 6
006b0d59: jmp 0x1806b0e6b
006b0d5e: cmp dword ptr [rbx + 0x70], 0x34
006b0d62: je 0x1806b0d4f
006b0d64: cmp dword ptr [rbx + 0x70], 0x2b
006b0d68: je 0x1806b0df3
006b0d6e: cmp dword ptr [rbx + 0x70], 0x2f
006b0d72: jne 0x1806b0e6b
006b0d78: xor r8d, r8d
006b0d7b: xor edx, edx
006b0d7d: mov rcx, rbx
006b0d80: call 0x1806b12f0
006b0d85: mov dword ptr [rdi + 0xc0], eax
006b0d8b: mov dword ptr [rdi + 0xa4], 1
006b0d95: mov qword ptr [rdi + 0x88], 0x3a83126f
006b0da0: mov dword ptr [rdi + 0x94], r12d
006b0da7: mov dword ptr [rdi + 0x90], 0xc1000000
006b0db1: mov eax, dword ptr [rbx + 0xcc]
006b0db7: sub eax, 0x28
006b0dba: movd xmm0, eax
006b0dbe: cvtdq2ps xmm0, xmm0
006b0dc1: movss dword ptr [rdi + 0xcc], xmm0
006b0dc9: mov rcx, qword ptr [rbx + 0x18]
006b0dcd: test rcx, rcx
006b0dd0: je 0x1806b1066
006b0dd6: mov r8d, dword ptr [rbx + 0xd0]
006b0ddd: xor r9d, r9d
006b0de0: mov edx, dword ptr [rbx + 0xcc]
006b0de6: call 0x18066be90
006b0deb: mov dword ptr [rdi + 0xd0], eax
006b0df1: jmp 0x1806b0e6b
006b0df3: mov dword ptr [rdi + 0xa4], 9
006b0dfd: mov dword ptr [rdi + 0x88], 0x40000000
006b0e07: mov rcx, qword ptr [rbx + 0x18]
006b0e0b: test rcx, rcx
006b0e0e: je 0x1806b1066
006b0e14: xor r8d, r8d
006b0e17: mov rdx, rbp
006b0e1a: call 0x180676eb0
006b0e1f: mov dword ptr [rdi + 0xd4], eax
006b0e25: jmp 0x1806b0e6b
006b0e27: mov dword ptr [rdi + 0xa4], 5
006b0e31: jmp 0x1806b0e6b
006b0e33: cmp r15d, dword ptr [rbx + 0x34]
006b0e37: jl 0x1806b0eb3
006b0e3d: jle 0x1806b0e6b
006b0e3f: mov dword ptr [rdi + 0xa4], 2
006b0e49: mov dword ptr [rdi + 0x8c], 0x40400000
006b0e53: movss xmm0, dword ptr [rdi + 0x98]
006b0e5b: subss xmm0, dword ptr [rip + 0x3582f69]
006b0e63: movss dword ptr [rdi + 0x98], xmm0
006b0e6b: movaps xmm10, xmmword ptr [rsp + 0x50]
006b0e71: movaps xmm9, xmmword ptr [rsp + 0x60]
006b0e77: movaps xmm8, xmmword ptr [rsp + 0x70]
006b0e7d: movaps xmm7, xmmword ptr [rsp + 0x80]
006b0e85: movaps xmm6, xmmword ptr [rsp + 0x90]
006b0e8d: mov r12, qword ptr [rsp + 0xa0]
006b0e95: mov rdi, qword ptr [rsp + 0xe0]
006b0e9d: mov rsi, qword ptr [rsp + 0xd8]
006b0ea5: add rsp, 0xa8
006b0eac: pop r15
006b0eae: pop r14
006b0eb0: pop rbp
006b0eb1: pop rbx
006b0eb2: ret
006b0eb3: mov dword ptr [rdi + 0xa4], 2
006b0ebd: mov dword ptr [rdi + 0x8c], 0xc0400000
006b0ec7: movss xmm0, dword ptr [rdi + 0x98]
006b0ecf: addss xmm0, dword ptr [rip + 0x3582ef5]
006b0ed7: jmp 0x1806b0e63
006b0ed9: test rbp, rbp
006b0edc: jne 0x1806b0ef5
006b0ede: movss xmm8, dword ptr [rip + 0x3583119]
006b0ee7: xorps xmm9, xmm9
006b0eeb: subss xmm8, xmm6
006b0ef0: jmp 0x1806b0fa1
006b0ef5: xor r8d, r8d
006b0ef8: lea rcx, [rsp + 0x40]
006b0efd: mov rdx, rbp
006b0f00: call 0x1806f2d70
006b0f05: movss xmm1, dword ptr [rip + 0x3582ebb]
006b0f0d: xor r8d, r8d
006b0f10: mov rcx, rbp
006b0f13: movups xmm9, xmmword ptr [rax]
006b0f17: call 0x18070a110
006b0f1c: movaps xmm8, xmm0
006b0f20: subss xmm8, xmm6
006b0f25: subss xmm8, dword ptr [rip + 0x3582d32]
006b0f2e: cmp dword ptr [rbp + 0x74], 0x35
006b0f32: movss xmm0, dword ptr [rip + 0x3582f6e]
006b0f3a: shufps xmm9, xmm9, 0x55
006b0f3f: subss xmm9, xmm7
006b0f44: jne 0x1806b0f4b
006b0f46: addss xmm8, xmm0
006b0f4b: cmp dword ptr [rbp + 0x70], 0x12
006b0f4f: jne 0x1806b0f5f
006b0f51: cmp byte ptr [rbp + 0x114], r12b
006b0f58: je 0x1806b0f5f
006b0f5a: addss xmm8, xmm0
006b0f5f: cmp dword ptr [rbp + 0x74], 0x3d
006b0f63: jne 0x1806b0f6e
006b0f65: addss xmm8, dword ptr [rip + 0x3582fee]
006b0f6e: cmp dword ptr [rbp + 0x70], 0x19
006b0f72: jne 0x1806b0fa1
006b0f74: mov rcx, qword ptr [rbx + 0x18]
006b0f78: test rcx, rcx
006b0f7b: je 0x1806b1066
006b0f81: mov r8d, dword ptr [rbx + 0x34]
006b0f85: xor r9d, r9d
006b0f88: mov edx, 8
006b0f8d: call 0x18065f630
006b0f92: movd xmm0, eax
006b0f96: cvtdq2ps xmm0, xmm0
006b0f99: subss xmm0, xmm7
006b0f9d: movaps xmm9, xmm0
006b0fa1: comiss xmm10, xmm8
006b0fa5: jbe 0x1806b0fab
006b0fa7: movaps xmm8, xmm10
006b0fab: divss xmm9, dword ptr [rip + 0x3582e7c]
006b0fb4: mov dword ptr [rdi + 0xa4], 1
006b0fbe: mov dword ptr [rdi + 0x8c], r12d
006b0fc5: mov dword ptr [rdi + 0x94], 0x3deb851f
006b0fcf: divss xmm8, dword ptr [rip + 0x3582e58]
006b0fd8: subss xmm9, dword ptr [rip + 0x3582ef7]
006b0fe1: movss dword ptr [rdi + 0x88], xmm8
006b0fea: movss dword ptr [rdi + 0x90], xmm9
006b0ff3: jmp 0x1806b0e6b
006b0ff8: xor edx, edx
006b0ffa: mov rcx, rbx
006b0ffd: call 0x1806b5e50
006b1002: jmp 0x1806b0e6b
006b1007: xor r9d, r9d
006b100a: mov edx, 0x14
006b100f: mov r8d, 2
006b1015: mov rcx, rbx
006b1018: call 0x1806ad520
006b101d: jmp 0x1806b0e6b
006b1022: xor r9d, r9d
006b1025: mov edx, 0x14
006b102a: mov r8d, 2
006b1030: mov rcx, rbx
006b1033: call 0x1806ad520
006b1038: mov r8, qword ptr [rbx + 0x10]
006b103c: test r8, r8
006b103f: je 0x1806b1066
006b1041: mov r8, qword ptr [r8 + 0x58]
006b1045: test r8, r8
006b1048: je 0x1806b1066
006b104a: mov rdx, qword ptr [rip + 0x45c4117]
006b1051: mov ecx, 6
006b1056: mov r9d, 0x1e
006b105c: call 0x180002040
006b1061: jmp 0x1806b0e6b
006b1066: call 0x1804fee20
006b106b: int3
006b106c: mov eax, dword ptr [rbx + 0x70]
006b106f: lea rcx, [rip + 0x45b3ec2]
006b1076: mov dword ptr [rsp + 0xd0], eax
006b107d: call 0x1804feb90
006b1082: mov rcx, rax
006b1085: lea rdx, [rsp + 0xd0]
006b108d: call 0x1804fde80
006b1092: lea rcx, [rip + 0x45de047]
006b1099: mov rbx, rax
006b109c: call 0x1804feb90
006b10a1: mov rcx, rax
006b10a4: xor r8d, r8d
006b10a7: mov rdx, rbx
006b10aa: call 0x182a20540
006b10af: lea rcx, [rip + 0x45a14c2]
006b10b6: mov rbx, rax
006b10b9: call 0x1804feb90
006b10be: mov rcx, rax
006b10c1: call 0x1804fedd0
006b10c6: xor r8d, r8d
006b10c9: mov rdx, rbx
006b10cc: mov rcx, rax
006b10cf: mov rdi, rax
006b10d2: call 0x182c223d0
006b10d7: lea rcx, [rip + 0x4592c12]
006b10de: call 0x1804feb90
006b10e3: mov rdx, rax
006b10e6: mov rcx, rdi
006b10e9: call 0x1804fede0
006b10ee: int3
006b10ef: nop
006b10f0: pop rdi
