004eb590: push rbx
004eb592: push rsi
004eb593: push rdi
004eb594: sub rsp, 0x80
004eb59b: cmp byte ptr [rip + 0x30adead], 0
004eb5a2: mov rsi, rcx
004eb5a5: jne 0x1804eb5de
004eb5a7: lea rcx, [rip + 0x2e9c98a]
004eb5ae: call 0x1803633b0
004eb5b3: lea rcx, [rip + 0x2e92e66]
004eb5ba: call 0x1803633b0
004eb5bf: lea rcx, [rip + 0x2ef05d2]
004eb5c6: call 0x1803633b0
004eb5cb: lea rcx, [rip + 0x2ef4df6]
004eb5d2: call 0x1803633b0
004eb5d7: mov byte ptr [rip + 0x30ade71], 1
004eb5de: xor ebx, ebx
004eb5e0: mov qword ptr [rsp + 0xa8], rbp
004eb5e8: mov qword ptr [rsp + 0xa0], rbx
004eb5f0: mov edi, 1
004eb5f5: mov qword ptr [rsp + 0xb0], r14
004eb5fd: nop dword ptr [rax]
004eb600: mov rax, qword ptr [rip + 0x2e9c931]
004eb607: cmp dword ptr [rax + 0xe0], ebx
004eb60d: jne 0x1804eb61e
004eb60f: mov rcx, rax
004eb612: call 0x180318580
004eb617: mov rax, qword ptr [rip + 0x2e9c91a]
004eb61e: mov rax, qword ptr [rax + 0xb8]
004eb625: mov rcx, qword ptr [rax]
004eb628: xor eax, eax
004eb62a: mov qword ptr [rsp + 0xb8], rax
004eb632: test rcx, rcx
004eb635: je 0x1804eb800
004eb63b: mov edx, dword ptr [rsi + 0xf4]
004eb641: mov r9d, 0x421
004eb647: movsd xmm0, qword ptr [rsp + 0xb8]
004eb650: add edx, edi
004eb652: mov r8d, dword ptr [rsi + 0xf8]
004eb659: mov qword ptr [rsp + 0x48], rbx
004eb65e: mov qword ptr [rsp + 0x40], rbx
004eb663: mov byte ptr [rsp + 0x38], 1
004eb668: mov byte ptr [rsp + 0x30], al
004eb66c: movsd qword ptr [rsp + 0x28], xmm0
004eb672: mov qword ptr [rsp + 0x20], rbx
004eb677: call 0x1809df0b0
004eb67c: mov rbp, rax
004eb67f: mov rax, qword ptr [rsi + 0x30]
004eb683: test rax, rax
004eb686: je 0x1804eb800
004eb68c: mov edx, dword ptr [rsi + 0xf4]
004eb692: mov ecx, dword ptr [rax + 0x154]
004eb698: add edx, edi
004eb69a: dec ecx
004eb69c: cmp edx, ecx
004eb69e: jg 0x1804eb6cb
004eb6a0: mov rcx, qword ptr [rip + 0x2ef04f1]
004eb6a7: inc edi
004eb6a9: cmp dword ptr [rcx + 0xe0], ebx
004eb6af: jne 0x1804eb6b6
004eb6b1: call 0x180318580
004eb6b6: xor r8d, r8d
004eb6b9: xor edx, edx
004eb6bb: mov rcx, rbp
004eb6be: call 0x1826b9a40
004eb6c3: test al, al
004eb6c5: jne 0x1804eb600
004eb6cb: mov rcx, qword ptr [rip + 0x2ef04c6]
004eb6d2: cmp dword ptr [rcx + 0xe0], ebx
004eb6d8: jne 0x1804eb6df
004eb6da: call 0x180318580
004eb6df: xor r8d, r8d
004eb6e2: xor edx, edx
004eb6e4: mov rcx, rbp
004eb6e7: call 0x1826b9be0
004eb6ec: test al, al
004eb6ee: jne 0x1804eb70b
004eb6f0: mov r14, qword ptr [rsp + 0xb0]
004eb6f8: mov rbp, qword ptr [rsp + 0xa8]
004eb700: add rsp, 0x80
004eb707: pop rdi
004eb708: pop rsi
004eb709: pop rbx
004eb70a: ret
004eb70b: mov rax, qword ptr [rip + 0x2ef4cb6]
004eb712: cmp dword ptr [rax + 0xe0], ebx
004eb718: jne 0x1804eb729
004eb71a: mov rcx, rax
004eb71d: call 0x180318580
004eb722: mov rax, qword ptr [rip + 0x2ef4c9f]
004eb729: mov rax, qword ptr [rax + 0xb8]
004eb730: mov r14, qword ptr [rax]
004eb733: test rbp, rbp
004eb736: je 0x1804eb800
004eb73c: mov rdx, qword ptr [rbp + 0x38]
004eb740: test rdx, rdx
004eb743: je 0x1804eb800
004eb749: xor r8d, r8d
004eb74c: movaps xmmword ptr [rsp + 0x70], xmm6
004eb751: lea rcx, [rsp + 0x50]
004eb756: call 0x1826c8510
004eb75b: mov r8, qword ptr [rip + 0x2e92cbe]
004eb762: lea rcx, [rsp + 0xa0]
004eb76a: movss xmm1, dword ptr [rip + 0x24903ca]
004eb772: movsd xmm6, qword ptr [rax]
004eb776: mov edi, dword ptr [rax + 8]
004eb779: call 0x181820660
004eb77e: mov r9, qword ptr [rsp + 0xa0]
004eb786: lea rdx, [rsp + 0x50]
004eb78b: mov r8, rbx
004eb78e: mov qword ptr [rsp + 0x28], rbx
004eb793: lea rcx, [rsp + 0x60]
004eb798: movsd qword ptr [rsp + 0x50], xmm6
004eb79e: mov dword ptr [rsp + 0x58], edi
004eb7a2: mov qword ptr [rsp + 0x20], rbx
004eb7a7: call 0x180a12ef0
004eb7ac: movaps xmm6, xmmword ptr [rsp + 0x70]
004eb7b1: movsd xmm2, qword ptr [rax]
004eb7b5: movaps xmm1, xmm2
004eb7b8: shufps xmm1, xmm1, 0x55
004eb7bc: test r14, r14
004eb7bf: je 0x1804eb800
004eb7c1: mov r9d, dword ptr [rbp + 0xf8]
004eb7c8: xorps xmm0, xmm0
004eb7cb: mov qword ptr [rsp + 0x30], rbx
004eb7d0: mov edx, 0xb
004eb7d5: movss dword ptr [rsp + 0x28], xmm0
004eb7db: mov rcx, r14
004eb7de: movaps xmm0, xmm2
004eb7e1: mov byte ptr [rsp + 0x20], 1
004eb7e6: unpcklps xmm0, xmm1
004eb7e9: movq r8, xmm0
004eb7ee: call 0x180465c40
004eb7f3: mov al, 1
004eb7f5: mov dword ptr [rsi + 0x248], ebx
004eb7fb: jmp 0x1804eb6f0
004eb800: call 0x180363610
004eb805: int3
004eb806: int3
004eb807: int3
004eb808: int3
004eb809: int3
004eb80a: int3
004eb80b: int3
004eb80c: int3
004eb80d: int3
004eb80e: int3
004eb80f: int3
