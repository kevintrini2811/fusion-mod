006be2d0: mov qword ptr [rsp + 0x20], rbp
006be2d5: push rsi
006be2d6: sub rsp, 0x20
006be2da: cmp byte ptr [rip + 0x482d353], 0
006be2e1: mov rbp, rcx
006be2e4: jne 0x1806be31d
006be2e6: lea rcx, [rip + 0x456b643]
006be2ed: call 0x1804feb70
006be2f2: lea rcx, [rip + 0x45c22a7]
006be2f9: call 0x1804feb70
006be2fe: lea rcx, [rip + 0x45c21c3]
006be305: call 0x1804feb70
006be30a: lea rcx, [rip + 0x45eaa2f]
006be311: call 0x1804feb70
006be316: mov byte ptr [rip + 0x482d317], 1
006be31d: mov rcx, qword ptr [rip + 0x45c21a4]
006be324: mov edx, 5
006be329: mov qword ptr [rsp + 0x30], rbx
006be32e: mov qword ptr [rsp + 0x38], rdi
006be333: mov qword ptr [rsp + 0x40], r14
006be338: call 0x1804fdf20
006be33d: lea rcx, [rbp + 0x118]
006be344: mov qword ptr [rbp + 0x118], rax
006be34b: mov rdx, rax
006be34e: call 0x1804fdea0
006be353: mov rcx, qword ptr [rip + 0x45c2246]
006be35a: mov edx, 5
006be35f: call 0x1804fdf20
006be364: lea rcx, [rbp + 0x150]
006be36b: mov qword ptr [rbp + 0x150], rax
006be372: mov rdx, rax
006be375: call 0x1804fdea0
006be37a: xor edx, edx
006be37c: mov rcx, rbp
006be37f: call 0x1806ce700
006be384: mov rcx, qword ptr [rip + 0x456b5a5]
006be38b: call 0x181137af0
006be390: lea rcx, [rbp + 0x158]
006be397: mov qword ptr [rbp + 0x158], rax
006be39e: mov rdx, rax
006be3a1: call 0x1804fdea0
006be3a6: mov rax, qword ptr [rbp + 0x118]
006be3ad: xor esi, esi
006be3af: mov ecx, esi
006be3b1: test rax, rax
006be3b4: je 0x1806be44f
006be3ba: mov r14d, 0x20
006be3c0: cmp ecx, dword ptr [rax + 0x18]
006be3c3: jge 0x1806be435
006be3c5: mov rcx, qword ptr [rip + 0x45ea974]
006be3cc: mov rdi, qword ptr [rbp + 0x118]
006be3d3: call 0x1804fedd0
006be3d8: xor edx, edx
006be3da: mov rcx, rax
006be3dd: mov rbx, rax
006be3e0: call 0x18060f020
006be3e5: test rdi, rdi
006be3e8: je 0x1806be44f
006be3ea: test rbx, rbx
006be3ed: je 0x1806be403
006be3ef: mov rdx, qword ptr [rdi]
006be3f2: mov rcx, rbx
006be3f5: mov rdx, qword ptr [rdx + 0x40]
006be3f9: call 0x1804fdec0
006be3fe: test rax, rax
006be401: je 0x1806be455
006be403: cmp esi, dword ptr [rdi + 0x18]
006be406: jae 0x1806be465
006be408: movsxd rax, esi
006be40b: mov rdx, rbx
006be40e: add rax, 4
006be412: mov qword ptr [r14 + rdi], rbx
006be416: lea rcx, [rdi + rax*8]
006be41a: call 0x1804fdea0
006be41f: mov rax, qword ptr [rbp + 0x118]
006be426: inc esi
006be428: add r14, 8
006be42c: mov ecx, esi
006be42e: test rax, rax
006be431: je 0x1806be44f
006be433: jmp 0x1806be3c0
006be435: mov r14, qword ptr [rsp + 0x40]
006be43a: mov rdi, qword ptr [rsp + 0x38]
006be43f: mov rbx, qword ptr [rsp + 0x30]
006be444: mov rbp, qword ptr [rsp + 0x48]
006be449: add rsp, 0x20
006be44d: pop rsi
006be44e: ret
006be44f: call 0x1804fee20
006be454: int3
006be455: call 0x1804fe790
006be45a: mov rcx, rax
006be45d: xor edx, edx
006be45f: call 0x1804fede0
006be464: int3
006be465: call 0x1804fee10
006be46a: int3
006be46b: int3
006be46c: int3
006be46d: int3
006be46e: int3
006be46f: int3
006be470: mov qword ptr [rsp + 8], rbx
006be475: mov qword ptr [rsp + 0x10], rbp
006be47a: mov qword ptr [rsp + 0x18], rsi
006be47f: push rdi
006be480: push r14
006be482: push r15
006be484: sub rsp, 0x30
006be488: cmp byte ptr [rip + 0x482d24e], 0
006be48f: mov r15, rdx
006be492: mov rbx, rcx
006be495: jne 0x1806be4b6
006be497: lea rcx, [rip + 0x45d4bf2]
006be49e: call 0x1804feb70
006be4a3: lea rcx, [rip + 0x458010e]
006be4aa: call 0x1804feb70
006be4af: mov byte ptr [rip + 0x482d227], 1
006be4b6: movsxd rsi, dword ptr [rbx + 0x10]
006be4ba: mov r14, qword ptr [rip + 0x45800f7]
006be4c1: cmp esi, dword ptr [rbx + 8]
006be4c4: jbe 0x1806be4cd
006be4c6: xor ecx, ecx
006be4c8: call 0x182c05bf0
006be4cd: mov rcx, qword ptr [r14 + 0x20]
006be4d1: mov rbp, rsi
006be4d4: add rbp, qword ptr [rbx]
006be4d7: mov edi, dword ptr [rbx + 8]
006be4da: mov qword ptr [rsp + 0x28], 0
006be4e3: test byte ptr [rcx + 0x135], 1
006be4ea: jne 0x1806be4f1
006be4ec: call 0x1804fc300
006be4f1: sub edi, esi
006be4f3: mov qword ptr [rsp + 0x20], rbp
006be4f8: cmp byte ptr [rip + 0x482d1e0], 0
006be4ff: mov dword ptr [rsp + 0x28], edi
006be503: jne 0x1806be524
006be505: lea rcx, [rip + 0x45d474c]
006be50c: call 0x1804feb70
006be511: lea rcx, [rip + 0x45804c8]
006be518: call 0x1804feb70
006be51d: mov byte ptr [rip + 0x482d1bb], 1
006be524: cmp edi, 0x10
006be527: jge 0x1806be535
006be529: xor edx, edx
006be52b: mov ecx, 0x1c
006be530: call 0x182c05bc0
006be535: movaps xmm0, xmmword ptr [rsp + 0x20]
006be53a: lea rcx, [rsp + 0x20]
006be53f: mov rdx, qword ptr [rip + 0x45d4712]
006be546: movdqa xmmword ptr [rsp + 0x20], xmm0
006be54c: call 0x180721da0
006be551: mov rbp, qword ptr [rsp + 0x58]
006be556: mov rsi, qword ptr [rsp + 0x60]
006be55b: movups xmm0, xmmword ptr [rax]
006be55e: movups xmmword ptr [r15], xmm0
006be562: add dword ptr [rbx + 0x10], 0x10
006be566: mov rbx, qword ptr [rsp + 0x50]
006be56b: add rsp, 0x30
006be56f: pop r15
006be571: pop r14
006be573: pop rdi
006be574: ret
006be575: int3
006be576: int3
006be577: int3
006be578: int3
006be579: int3
006be57a: int3
006be57b: int3
006be57c: int3
006be57d: int3
006be57e: int3
006be57f: int3
006be580: mov qword ptr [rsp + 8], rbx
006be585: mov qword ptr [rsp + 0x10], rbp
006be58a: mov qword ptr [rsp + 0x18], rsi
006be58f: push rdi
006be590: push r14
006be592: push r15
006be594: sub rsp, 0x30
006be598: cmp byte ptr [rip + 0x482d13d], 0
006be59f: mov r15, rdx
006be5a2: mov rbx, rcx
006be5a5: jne 0x1806be5c6
006be5a7: lea rcx, [rip + 0x45d5bc2]
006be5ae: call 0x1804feb70
006be5b3: lea rcx, [rip + 0x4599446]
006be5ba: call 0x1804feb70
006be5bf: mov byte ptr [rip + 0x482d116], 1
006be5c6: movsxd rsi, dword ptr [rbx + 0x10]
006be5ca: mov r14, qword ptr [rip + 0x459942f]
006be5d1: cmp esi, dword ptr [rbx + 8]
006be5d4: jbe 0x1806be5dd
006be5d6: xor ecx, ecx
006be5d8: call 0x182c05bf0
006be5dd: mov rcx, qword ptr [r14 + 0x20]
006be5e1: mov rbp, rsi
006be5e4: add rbp, qword ptr [rbx]
006be5e7: mov edi, dword ptr [rbx + 8]
006be5ea: mov qword ptr [rsp + 0x28], 0
006be5f3: test byte ptr [rcx + 0x135], 1
006be5fa: jne 0x1806be601
006be5fc: call 0x1804fc300
006be601: sub edi, esi
006be603: mov qword ptr [rsp + 0x20], rbp
006be608: cmp byte ptr [rip + 0x482d0cf], 0
006be60f: mov dword ptr [rsp + 0x28], edi
006be613: jne 0x1806be634
006be615: lea rcx, [rip + 0x45d4714]
006be61c: call 0x1804feb70
006be621: lea rcx, [rip + 0x4599748]
006be628: call 0x1804feb70
006be62d: mov byte ptr [rip + 0x482d0aa], 1
006be634: cmp edi, 0x10
006be637: jae 0x1806be645
006be639: xor edx, edx
006be63b: mov ecx, 0x1c
006be640: call 0x182c05bc0
006be645: movaps xmm0, xmmword ptr [rsp + 0x20]
006be64a: lea rcx, [rsp + 0x20]
006be64f: mov rdx, qword ptr [rip + 0x45d46da]
006be656: movdqa xmmword ptr [rsp + 0x20], xmm0
006be65c: call 0x180721da0
006be661: movups xmm0, xmmword ptr [r15]
006be665: mov rbp, qword ptr [rsp + 0x58]
006be66a: mov rsi, qword ptr [rsp + 0x60]
006be66f: movups xmmword ptr [rax], xmm0
006be672: add dword ptr [rbx + 0x10], 0x10
006be676: mov rbx, qword ptr [rsp + 0x50]
006be67b: add rsp, 0x30
006be67f: pop r15
006be681: pop r14
006be683: pop rdi
006be684: ret
006be685: int3
006be686: int3
006be687: int3
006be688: int3
006be689: int3
006be68a: int3
006be68b: int3
006be68c: int3
006be68d: int3
006be68e: int3
006be68f: int3
006be690: sub rsp, 0x38
006be694: cmp byte ptr [rip + 0x482cf96], 0
006be69b: movaps xmmword ptr [rsp + 0x20], xmm6
006be6a0: mov qword ptr [rsp + 0x50], rcx
006be6a5: movss xmm6, dword ptr [rsp + 0x54]
006be6ab: movss xmm0, dword ptr [rsp + 0x50]
006be6b1: mulss xmm6, xmm6
006be6b5: mulss xmm0, xmm0
006be6b9: addss xmm6, xmm0
006be6bd: jne 0x1806be6d2
006be6bf: lea rcx, [rip + 0x45ec7a2]
006be6c6: call 0x1804feb70
006be6cb: mov byte ptr [rip + 0x482cf5f], 1
006be6d2: mov rcx, qword ptr [rip + 0x45ec78f]
006be6d9: cmp dword ptr [rcx + 0xe4], 0
006be6e0: jne 0x1806be6e7
006be6e2: call 0x1804feeb0
006be6e7: cvtps2pd xmm1, xmm6
006be6ea: xorps xmm0, xmm0
006be6ed: ucomisd xmm0, xmm1
006be6f1: ja 0x1806be6f9
006be6f3: sqrtpd xmm0, xmm1
006be6f7: jmp 0x1806be701
006be6f9: movaps xmm0, xmm1
006be6fc: call 0x1805d9bc0
006be701: movss xmm2, dword ptr [rip + 0x3575543]
006be709: movaps xmm6, xmmword ptr [rsp + 0x20]
006be70e: cvtsd2ss xmm0, xmm0
006be712: divss xmm2, xmm0
006be716: movaps xmm1, xmm2
006be719: mulss xmm2, dword ptr [rsp + 0x54]
006be71f: mulss xmm1, dword ptr [rsp + 0x50]
006be725: unpcklps xmm1, xmm2
006be728: movq rax, xmm1
006be72d: add rsp, 0x38
006be731: ret
006be732: int3
006be733: int3
006be734: int3
006be735: int3
006be736: int3
006be737: int3
006be738: int3
006be739: int3
006be73a: int3
006be73b: int3
006be73c: int3
006be73d: int3
006be73e: int3
006be73f: int3
006be740: sub rsp, 0x38
006be744: cmp byte ptr [rip + 0x482cee6], 0
006be74b: movaps xmmword ptr [rsp + 0x20], xmm6
006be750: movaps xmm6, xmm0
006be753: jne 0x1806be768
006be755: lea rcx, [rip + 0x45ec70c]
006be75c: call 0x1804feb70
006be761: mov byte ptr [rip + 0x482cec9], 1
006be768: mov rcx, qword ptr [rip + 0x45ec6f9]
006be76f: cmp dword ptr [rcx + 0xe4], 0
006be776: jne 0x1806be77d
006be778: call 0x1804feeb0
006be77d: xorps xmm1, xmm1
006be780: xorps xmm0, xmm0
006be783: cvtss2sd xmm1, xmm6
006be787: ucomisd xmm0, xmm1
006be78b: ja 0x1806be793
006be78d: sqrtpd xmm0, xmm1
006be791: jmp 0x1806be79b
006be793: movaps xmm0, xmm1
006be796: call 0x1805d9bc0
006be79b: movaps xmm6, xmmword ptr [rsp + 0x20]
006be7a0: xorps xmm1, xmm1
006be7a3: cvtsd2ss xmm1, xmm0
006be7a7: movss xmm0, dword ptr [rip + 0x357549d]
006be7af: divss xmm0, xmm1
006be7b3: add rsp, 0x38
006be7b7: ret
006be7b8: int3
006be7b9: int3
006be7ba: int3
006be7bb: int3
006be7bc: int3
006be7bd: int3
006be7be: int3
006be7bf: int3
