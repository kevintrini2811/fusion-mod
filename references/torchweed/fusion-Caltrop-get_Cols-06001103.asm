00497a70: mov qword ptr [rsp + 0x18], rbx
00497a75: push rsi
00497a76: sub rsp, 0x90
00497a7d: cmp byte ptr [rip + 0x3101764], 0
00497a84: mov rsi, rcx
00497a87: jne 0x180497aa8
00497a89: lea rcx, [rip + 0x2ee6990]
00497a90: call 0x1803633b0
00497a95: lea rcx, [rip + 0x2f49a5c]
00497a9c: call 0x1803633b0
00497aa1: mov byte ptr [rip + 0x3101740], 1
00497aa8: mov rdx, qword ptr [rsi + 0x38]
00497aac: xor ebx, ebx
00497aae: mov qword ptr [rsp + 0xa0], rbx
00497ab6: test rdx, rdx
00497ab9: je 0x180497bd1
00497abf: mov qword ptr [rsp + 0xa8], rdi
00497ac7: lea rcx, [rsp + 0x30]
00497acc: movaps xmmword ptr [rsp + 0x80], xmm6
00497ad4: xor r8d, r8d
00497ad7: movaps xmmword ptr [rsp + 0x70], xmm7
00497adc: movaps xmmword ptr [rsp + 0x60], xmm8
00497ae2: movaps xmmword ptr [rsp + 0x50], xmm9
00497ae8: call 0x1826c8510
00497aed: mov r8, qword ptr [rip + 0x2ee692c]
00497af4: lea rcx, [rsp + 0xa0]
00497afc: movss xmm1, dword ptr [rip + 0x24e4038]
00497b04: movsd xmm6, qword ptr [rax]
00497b08: mov edi, dword ptr [rax + 8]
00497b0b: call 0x181820660
00497b10: mov r9, qword ptr [rsp + 0xa0]
00497b18: lea rdx, [rsp + 0x30]
00497b1d: mov r8d, ebx
00497b20: mov qword ptr [rsp + 0x28], rbx
00497b25: lea rcx, [rsp + 0x40]
00497b2a: movsd qword ptr [rsp + 0x30], xmm6
00497b30: mov dword ptr [rsp + 0x38], edi
00497b34: mov qword ptr [rsp + 0x20], rbx
00497b39: call 0x180a12ef0
00497b3e: movsd xmm7, qword ptr [rax]
00497b42: movaps xmm6, xmm7
00497b45: shufps xmm6, xmm6, 0x55
00497b49: mov ecx, dword ptr [rsi + 0x4c]
00497b4c: xor edx, edx
00497b4e: movss xmm8, dword ptr [rsi + 0x248]
00497b57: movss xmm9, dword ptr [rip + 0x24e4010]
00497b60: call 0x1826b66c0
00497b65: mov rcx, qword ptr [rip + 0x2f4998c]
00497b6c: mov edi, eax
00497b6e: cmp dword ptr [rcx + 0xe0], ebx
00497b74: jne 0x180497b7b
00497b76: call 0x180318580
00497b7b: movaps xmm1, xmm7
00497b7e: unpcklps xmm8, xmm9
00497b82: unpcklps xmm1, xmm6
00497b85: mov r9d, edi
00497b88: movq rcx, xmm1
00497b8d: mov qword ptr [rsp + 0x20], rbx
00497b92: xorps xmm2, xmm2
00497b95: movq rdx, xmm8
00497b9a: call 0x18270ce10
00497b9f: movaps xmm9, xmmword ptr [rsp + 0x50]
00497ba5: movaps xmm8, xmmword ptr [rsp + 0x60]
00497bab: movaps xmm7, xmmword ptr [rsp + 0x70]
00497bb0: movaps xmm6, xmmword ptr [rsp + 0x80]
00497bb8: mov rdi, qword ptr [rsp + 0xa8]
00497bc0: mov rbx, qword ptr [rsp + 0xb0]
00497bc8: add rsp, 0x90
00497bcf: pop rsi
00497bd0: ret
00497bd1: call 0x180363610
00497bd6: int3
00497bd7: int3
00497bd8: int3
00497bd9: int3
00497bda: int3
00497bdb: int3
00497bdc: int3
00497bdd: int3
00497bde: int3
00497bdf: int3
