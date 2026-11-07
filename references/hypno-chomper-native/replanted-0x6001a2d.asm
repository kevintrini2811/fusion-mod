006bd300: push rbx
006bd302: sub rsp, 0x60
006bd306: cmp byte ptr [rip + 0x482e33c], 0
006bd30d: mov rbx, rcx
006bd310: jne 0x1806bd361
006bd312: lea rcx, [rip + 0x456dca7]
006bd319: call 0x1804feb70
006bd31e: lea rcx, [rip + 0x45b7e43]
006bd325: call 0x1804feb70
006bd32a: lea rcx, [rip + 0x45d9d4f]
006bd331: call 0x1804feb70
006bd336: lea rcx, [rip + 0x45d9403]
006bd33d: call 0x1804feb70
006bd342: lea rcx, [rip + 0x45d9e0f]
006bd349: call 0x1804feb70
006bd34e: lea rcx, [rip + 0x45d959b]
006bd355: call 0x1804feb70
006bd35a: mov byte ptr [rip + 0x482e2e8], 1
006bd361: cmp dword ptr [rbx + 0x88], 0
006bd368: mov qword ptr [rsp + 0x70], rbp
006bd36d: mov qword ptr [rsp + 0x78], rsi
006bd372: mov qword ptr [rsp + 0x80], rdi
006bd37a: mov qword ptr [rsp + 0x88], r12
006bd382: mov qword ptr [rsp + 0x58], r14
006bd387: mov qword ptr [rsp + 0x50], r15
006bd38c: movaps xmmword ptr [rsp + 0x40], xmm6
006bd391: je 0x1806bd921
006bd397: cmp dword ptr [rbx + 0x88], 3
006bd39e: je 0x1806bd852
006bd3a4: cmp dword ptr [rbx + 0x88], 4
006bd3ab: je 0x1806bd7e0
006bd3b1: mov rcx, qword ptr [rbx + 0x18]
006bd3b5: test rcx, rcx
006bd3b8: je 0x1806bda72
006bd3be: movd xmm1, dword ptr [rbx + 0xcc]
006bd3c6: xor r9d, r9d
006bd3c9: movss xmm2, dword ptr [rbx + 0x24]
006bd3ce: cvtdq2ps xmm1, xmm1
006bd3d1: call 0x18066bd30
006bd3d6: mov rcx, qword ptr [rbx + 0x18]
006bd3da: mov ebp, eax
006bd3dc: mov r12d, dword ptr [rbx + 0xcc]
006bd3e3: test rcx, rcx
006bd3e6: je 0x1806bda72
006bd3ec: mov r8d, dword ptr [rbx + 0x34]
006bd3f0: xor r9d, r9d
006bd3f3: mov edx, eax
006bd3f5: call 0x18065f630
006bd3fa: cmp dword ptr [rbx + 0x88], 5
006bd401: lea esi, [rax + 8]
006bd404: je 0x1806bd66d
006bd40a: cmp dword ptr [rbx + 0x88], 6
006bd411: je 0x1806bd45e
006bd413: cmp dword ptr [rbx + 0x88], 7
006bd41a: jne 0x1806bd42f
006bd41c: cmp dword ptr [rbx + 0xa0], 0
006bd423: jne 0x1806bd42f
006bd425: xor edx, edx
006bd427: mov rcx, rbx
006bd42a: call 0x1806acd40
006bd42f: movaps xmm6, xmmword ptr [rsp + 0x40]
006bd434: mov r15, qword ptr [rsp + 0x50]
006bd439: mov r14, qword ptr [rsp + 0x58]
006bd43e: mov r12, qword ptr [rsp + 0x88]
006bd446: mov rdi, qword ptr [rsp + 0x80]
006bd44e: mov rsi, qword ptr [rsp + 0x78]
006bd453: mov rbp, qword ptr [rsp + 0x70]
006bd458: add rsp, 0x60
006bd45c: pop rbx
006bd45d: ret
006bd45e: mov rcx, qword ptr [rip + 0x456db5b]
006bd465: mov r14d, dword ptr [rbx + 0xa0]
006bd46c: cmp dword ptr [rcx + 0xe4], 0
006bd473: jne 0x1806bd47a
006bd475: call 0x1804feeb0
006bd47a: xor edi, edi
006bd47c: lea r9d, [rsi - 0x78]
006bd480: mov qword ptr [rsp + 0x30], rdi
006bd485: mov r8d, r14d
006bd488: mov dword ptr [rsp + 0x28], 1
006bd490: xor edx, edx
006bd492: mov ecx, 0xa
006bd497: mov dword ptr [rsp + 0x20], esi
006bd49b: call 0x1806cc120
006bd4a0: cmp dword ptr [rbx + 0xa0], 5
006bd4a7: movd xmm0, eax
006bd4ab: cvtdq2ps xmm0, xmm0
006bd4ae: movss dword ptr [rbx + 0x24], xmm0
006bd4b3: jne 0x1806bd4bf
006bd4b5: xor edx, edx
006bd4b7: mov rcx, rbx
006bd4ba: call 0x1806ae200
006bd4bf: cmp dword ptr [rbx + 0xa0], edi
006bd4c5: jne 0x1806bd42f
006bd4cb: mov rcx, qword ptr [rbx + 0x18]
006bd4cf: test rcx, rcx
006bd4d2: je 0x1806bda72
006bd4d8: mov r8d, dword ptr [rbx + 0x34]
006bd4dc: xor r9d, r9d
006bd4df: mov edx, ebp
006bd4e1: call 0x180662110
006bd4e6: test al, al
006bd4e8: jne 0x1806bd5c6
006bd4ee: mov rcx, qword ptr [rbx + 0x18]
006bd4f2: mov dword ptr [rbx + 0x88], 7
006bd4fc: mov dword ptr [rbx + 0xa0], 0x64
006bd506: test rcx, rcx
006bd509: je 0x1806bda72
006bd50f: movss xmm3, dword ptr [rip + 0x3576735]
006bd517: mov edx, 1
006bd51c: mov r8d, 4
006bd522: mov qword ptr [rsp + 0x20], rdi
006bd527: call 0x1806709c0
006bd52c: mov r8, qword ptr [rbx + 0x10]
006bd530: test r8, r8
006bd533: je 0x1806bda72
006bd539: mov r8, qword ptr [r8 + 0x58]
006bd53d: test r8, r8
006bd540: je 0x1806bda72
006bd546: mov rdx, qword ptr [rip + 0x45b7c1b]
006bd54d: mov ecx, 6
006bd552: mov r9d, 0x28
006bd558: call 0x180002040
006bd55d: mov rcx, qword ptr [rbx + 0x18]
006bd561: movss xmm6, dword ptr [rip + 0x3576863]
006bd569: test rcx, rcx
006bd56c: je 0x1806bda72
006bd572: xor edx, edx
006bd574: call 0x1806719c0
006bd579: test al, al
006bd57b: je 0x1806bd585
006bd57d: movss xmm6, dword ptr [rip + 0x3576a67]
006bd585: mov rcx, qword ptr [rbx + 0x10]
006bd589: test rcx, rcx
006bd58c: je 0x1806bda72
006bd592: addss xmm6, dword ptr [rbx + 0x24]
006bd597: movss xmm1, dword ptr [rbx + 0x20]
006bd59c: mov r9d, dword ptr [rbx + 0x38]
006bd5a0: addss xmm1, dword ptr [rip + 0x357681c]
006bd5a8: add r9d, 4
006bd5ac: mov qword ptr [rsp + 0x28], rdi
006bd5b1: movaps xmm2, xmm6
006bd5b4: mov dword ptr [rsp + 0x20], 0x61
006bd5bc: call 0x1807d9b50
006bd5c1: jmp 0x1806bd42f
006bd5c6: mov rcx, qword ptr [rbx + 0x10]
006bd5ca: test rcx, rcx
006bd5cd: je 0x1806bda72
006bd5d3: movss xmm2, dword ptr [rbx + 0x24]
006bd5d8: movss xmm1, dword ptr [rbx + 0x20]
006bd5dd: mov r9d, dword ptr [rbx + 0x38]
006bd5e1: addss xmm2, dword ptr [rip + 0x35767e3]
006bd5e9: addss xmm1, dword ptr [rip + 0x35767d3]
006bd5f1: inc r9d
006bd5f4: mov qword ptr [rsp + 0x28], rdi
006bd5f9: mov dword ptr [rsp + 0x20], 0x1a
006bd601: call 0x1807d9b50
006bd606: mov r8, qword ptr [rbx + 0x10]
006bd60a: test r8, r8
006bd60d: je 0x1806bda72
006bd613: mov r8, qword ptr [r8 + 0x58]
006bd617: test r8, r8
006bd61a: je 0x1806bda72
006bd620: mov rdx, qword ptr [rip + 0x45b7b41]
006bd627: mov ecx, 6
006bd62c: mov r9d, 1
006bd632: call 0x180002040
006bd637: mov r8, qword ptr [rbx + 0x10]
006bd63b: test r8, r8
006bd63e: je 0x1806bda72
006bd644: mov r8, qword ptr [r8 + 0x58]
006bd648: test r8, r8
006bd64b: je 0x1806bda72
006bd651: mov rdx, qword ptr [rip + 0x45b7b10]
006bd658: mov ecx, 0x19
006bd65d: mov r9d, 0xa1
006bd663: call 0x180002040
006bd668: jmp 0x1806bd425
006bd66d: mov rcx, qword ptr [rbx + 0x18]
006bd671: test rcx, rcx
006bd674: je 0x1806bda72
006bd67a: mov r8d, dword ptr [rbx + 0xd4]
006bd681: xor r9d, r9d
006bd684: mov edx, dword ptr [rbx + 0x74]
006bd687: call 0x18065f590
006bd68c: mov rcx, qword ptr [rbx + 0x18]
006bd690: mov r15d, eax
006bd693: test rcx, rcx
006bd696: je 0x1806bda72
006bd69c: mov r8d, dword ptr [rbx + 0xd4]
006bd6a3: xor r9d, r9d
006bd6a6: mov edx, dword ptr [rbx + 0x74]
006bd6a9: call 0x18065f630
006bd6ae: mov rcx, qword ptr [rip + 0x456d90b]
006bd6b5: mov ebp, eax
006bd6b7: mov r14d, dword ptr [rbx + 0xa0]
006bd6be: cmp dword ptr [rcx + 0xe4], 0
006bd6c5: jne 0x1806bd6cc
006bd6c7: call 0x1804feeb0
006bd6cc: xor edi, edi
006bd6ce: mov r9d, r15d
006bd6d1: mov qword ptr [rsp + 0x30], rdi
006bd6d6: mov r8d, r14d
006bd6d9: mov dword ptr [rsp + 0x28], 4
006bd6e1: mov edx, 0x14
006bd6e6: mov ecx, 0x32
006bd6eb: mov dword ptr [rsp + 0x20], r12d
006bd6f0: call 0x1806cc120
006bd6f5: mov r8d, dword ptr [rbx + 0xa0]
006bd6fc: mov r9d, ebp
006bd6ff: mov qword ptr [rsp + 0x30], rdi
006bd704: mov edx, 0x14
006bd709: mov dword ptr [rsp + 0x28], 4
006bd711: mov ecx, 0x32
006bd716: movd xmm0, eax
006bd71a: lea eax, [rsi - 0x78]
006bd71d: cvtdq2ps xmm0, xmm0
006bd720: mov dword ptr [rsp + 0x20], eax
006bd724: movss dword ptr [rbx + 0x20], xmm0
006bd729: call 0x1806cc120
006bd72e: movd xmm0, eax
006bd732: cvtdq2ps xmm0, xmm0
006bd735: movss dword ptr [rbx + 0x24], xmm0
006bd73a: cmp dword ptr [rbx + 0xa0], edi
006bd740: jne 0x1806bd42f
006bd746: cmp byte ptr [rip + 0x482df0b], dil
006bd74d: mov rsi, qword ptr [rip + 0x45d8fec]
006bd754: jne 0x1806bd775
006bd756: lea rcx, [rip + 0x45d5323]
006bd75d: call 0x1804feb70
006bd762: lea rcx, [rip + 0x45d3a8f]
006bd769: call 0x1804feb70
006bd76e: mov byte ptr [rip + 0x482dee3], 1
006bd775: test rsi, rsi
006bd778: je 0x1806bda72
006bd77e: mov r8, qword ptr [rip + 0x45d3a73]
006bd785: xor r9d, r9d
006bd788: mov rdx, qword ptr [rip + 0x45d52f1]
006bd78f: mov rcx, rsi
006bd792: call 0x182a24e50
006bd797: mov rcx, qword ptr [rbx + 0x140]
006bd79e: test rcx, rcx
006bd7a1: je 0x1806bda72
006bd7a7: movss xmm3, dword ptr [rip + 0x35764b9]
006bd7af: xor r8d, r8d
006bd7b2: mov qword ptr [rsp + 0x28], rdi
006bd7b7: mov rdx, rax
006bd7ba: mov dword ptr [rsp + 0x20], 3
006bd7c2: call 0x1807c8270
006bd7c7: mov dword ptr [rbx + 0x88], 6
006bd7d1: mov dword ptr [rbx + 0xa0], 0xa
006bd7db: jmp 0x1806bd42f
006bd7e0: cmp dword ptr [rbx + 0xa0], 0
006bd7e7: jne 0x1806bd42f
006bd7ed: xor edx, edx
006bd7ef: mov rcx, rbx
006bd7f2: call 0x1806af3d0
006bd7f7: test rax, rax
006bd7fa: je 0x1806bd823
006bd7fc: movss xmm1, dword ptr [rip + 0x357645c]
006bd804: xor r8d, r8d
006bd807: mov rcx, rax
006bd80a: call 0x18070a110
006bd80f: mov eax, dword ptr [rbx + 0x28]
006bd812: cdq
006bd813: sub eax, edx
006bd815: cvttss2si ecx, xmm0
006bd819: sar eax, 1
006bd81b: sub ecx, eax
006bd81d: mov dword ptr [rbx + 0xcc], ecx
006bd823: mov edx, dword ptr [rbx + 0x34]
006bd826: xor r9d, r9d
006bd829: xor r8d, r8d
006bd82c: mov dword ptr [rbx + 0x88], 5
006bd836: mov ecx, 0x4a768
006bd83b: mov dword ptr [rbx + 0xa0], 0x32
006bd845: call 0x180665070
006bd84a: mov dword ptr [rbx + 0x38], eax
006bd84d: jmp 0x1806bd42f
006bd852: cmp dword ptr [rbx + 0xa0], 0
006bd859: jne 0x1806bd42f
006bd85f: cmp byte ptr [rip + 0x482ddf2], 0
006bd866: mov rdi, qword ptr [rip + 0x45d9083]
006bd86d: jne 0x1806bd88e
006bd86f: lea rcx, [rip + 0x45d520a]
006bd876: call 0x1804feb70
006bd87b: lea rcx, [rip + 0x45d3976]
006bd882: call 0x1804feb70
006bd887: mov byte ptr [rip + 0x482ddca], 1
006bd88e: test rdi, rdi
006bd891: je 0x1806bda72
006bd897: mov r8, qword ptr [rip + 0x45d395a]
006bd89e: xor r9d, r9d
006bd8a1: mov rdx, qword ptr [rip + 0x45d51d8]
006bd8a8: mov rcx, rdi
006bd8ab: call 0x182a24e50
006bd8b0: mov rcx, qword ptr [rbx + 0x140]
006bd8b7: mov rsi, rax
006bd8ba: test rcx, rcx
006bd8bd: je 0x1806bda72
006bd8c3: movss xmm1, dword ptr [rip + 0x35764f1]
006bd8cb: xor r9d, r9d
006bd8ce: xor r8d, r8d
006bd8d1: call 0x18060f880
006bd8d6: mov rcx, qword ptr [rbx + 0x140]
006bd8dd: test rcx, rcx
006bd8e0: je 0x1806bda72
006bd8e6: movss xmm3, dword ptr [rip + 0x3576602]
006bd8ee: xor edi, edi
006bd8f0: mov qword ptr [rsp + 0x28], rdi
006bd8f5: xor r8d, r8d
006bd8f8: mov rdx, rsi
006bd8fb: mov dword ptr [rsp + 0x20], 3
006bd903: call 0x1807c8270
006bd908: mov dword ptr [rbx + 0x88], 4
006bd912: mov dword ptr [rbx + 0xa0], 0x2d
006bd91c: jmp 0x1806bd42f
006bd921: xor edx, edx
006bd923: mov rcx, rbx
006bd926: call 0x1806af3d0
006bd92b: mov rdi, rax
006bd92e: test rax, rax
006bd931: je 0x1806bd42f
006bd937: mov rcx, qword ptr [rbx + 0x18]
006bd93b: test rcx, rcx
006bd93e: je 0x1806bda72
006bd944: xor r8d, r8d
006bd947: mov rdx, rax
006bd94a: call 0x180676eb0
006bd94f: xor r8d, r8d
006bd952: mov dword ptr [rbx + 0x120], eax
006bd958: xorps xmm1, xmm1
006bd95b: mov rcx, rdi
006bd95e: call 0x18070a110
006bd963: mov eax, dword ptr [rbx + 0x28]
006bd966: cvttss2si ecx, xmm0
006bd96a: cdq
006bd96b: mov dword ptr [rbx + 0x88], 3
006bd975: sub eax, edx
006bd977: mov dword ptr [rbx + 0xa0], 0x50
006bd981: sar eax, 1
006bd983: movss xmm0, dword ptr [rbx + 0x20]
006bd988: sub ecx, eax
006bd98a: mov dword ptr [rbx + 0xcc], ecx
006bd990: mov rdi, qword ptr [rip + 0x45d97c1]
006bd997: movd xmm1, ecx
006bd99b: cvtdq2ps xmm1, xmm1
006bd99e: comiss xmm0, xmm1
006bd9a1: jbe 0x1806bd9aa
006bd9a3: mov rdi, qword ptr [rip + 0x45d96d6]
006bd9aa: cmp byte ptr [rip + 0x482dca7], 0
006bd9b1: jne 0x1806bd9d2
006bd9b3: lea rcx, [rip + 0x45d50c6]
006bd9ba: call 0x1804feb70
006bd9bf: lea rcx, [rip + 0x45d3832]
006bd9c6: call 0x1804feb70
006bd9cb: mov byte ptr [rip + 0x482dc86], 1
006bd9d2: test rdi, rdi
006bd9d5: je 0x1806bda72
006bd9db: mov r8, qword ptr [rip + 0x45d3816]
006bd9e2: xor r9d, r9d
006bd9e5: mov rdx, qword ptr [rip + 0x45d5094]
006bd9ec: mov rcx, rdi
006bd9ef: call 0x182a24e50
006bd9f4: mov rcx, qword ptr [rbx + 0x140]
006bd9fb: mov rsi, rax
006bd9fe: test rcx, rcx
006bda01: je 0x1806bda72
006bda03: movss xmm1, dword ptr [rip + 0x35763a1]
006bda0b: xor r9d, r9d
006bda0e: xor r8d, r8d
006bda11: call 0x18060f880
006bda16: mov rcx, qword ptr [rbx + 0x140]
006bda1d: test rcx, rcx
006bda20: je 0x1806bda72
006bda22: movss xmm3, dword ptr [rip + 0x35764c6]
006bda2a: xor edi, edi
006bda2c: mov qword ptr [rsp + 0x28], rdi
006bda31: xor r8d, r8d
006bda34: mov rdx, rsi
006bda37: mov dword ptr [rsp + 0x20], 3
006bda3f: call 0x1807c8270
006bda44: mov r8, qword ptr [rbx + 0x10]
006bda48: test r8, r8
006bda4b: je 0x1806bda72
006bda4d: mov r8, qword ptr [r8 + 0x58]
006bda51: test r8, r8
006bda54: je 0x1806bda72
006bda56: mov rdx, qword ptr [rip + 0x45b770b]
006bda5d: mov ecx, 6
006bda62: mov r9d, 0x29
006bda68: call 0x180002040
006bda6d: jmp 0x1806bd42f
006bda72: call 0x1804fee20
006bda77: int3
006bda78: int3
006bda79: int3
006bda7a: int3
006bda7b: int3
006bda7c: int3
006bda7d: int3
006bda7e: int3
006bda7f: int3
