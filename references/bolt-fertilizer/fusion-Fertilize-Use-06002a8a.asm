00683730: mov rax, rsp
00683733: mov dword ptr [rax + 0x18], r8d
00683737: mov dword ptr [rax + 0x10], edx
0068373a: mov qword ptr [rax + 8], rcx
0068373e: push rbx
0068373f: push rsi
00683740: push rdi
00683741: push r12
00683743: push r13
00683745: push r14
00683747: push r15
00683749: sub rsp, 0x120
00683750: movaps xmmword ptr [rax - 0x48], xmm6
00683754: movaps xmmword ptr [rax - 0x58], xmm7
00683758: movaps xmmword ptr [rax - 0x68], xmm8
0068375d: movaps xmmword ptr [rax - 0x78], xmm9
00683762: movaps xmmword ptr [rax - 0x88], xmm10
0068376a: mov r12d, r8d
0068376d: mov r13d, edx
00683770: mov r14, rcx
00683773: cmp byte ptr [rip + 0x2f16894], 0
0068377a: jne 0x18068383b
00683780: lea rcx, [rip + 0x2cf6379]
00683787: call 0x1803633b0
0068378c: lea rcx, [rip + 0x2d2c90d]
00683793: call 0x1803633b0
00683798: lea rcx, [rip + 0x2d52aa9]
0068379f: call 0x1803633b0
006837a4: lea rcx, [rip + 0x2d5d9e5]
006837ab: call 0x1803633b0
006837b0: lea rcx, [rip + 0x2d5da89]
006837b7: call 0x1803633b0
006837bc: lea rcx, [rip + 0x2d52b35]
006837c3: call 0x1803633b0
006837c8: lea rcx, [rip + 0x2d5db21]
006837cf: call 0x1803633b0
006837d4: lea rcx, [rip + 0x2d52bcd]
006837db: call 0x1803633b0
006837e0: lea rcx, [rip + 0x2d1eca1]
006837e7: call 0x1803633b0
006837ec: lea rcx, [rip + 0x2d2038d]
006837f3: call 0x1803633b0
006837f8: lea rcx, [rip + 0x2d0f329]
006837ff: call 0x1803633b0
00683804: lea rcx, [rip + 0x2cf3f45]
0068380b: call 0x1803633b0
00683810: lea rcx, [rip + 0x2cfac09]
00683817: call 0x1803633b0
0068381c: lea rcx, [rip + 0x2d58375]
00683823: call 0x1803633b0
00683828: lea rcx, [rip + 0x2d5e539]
0068382f: call 0x1803633b0
00683834: mov byte ptr [rip + 0x2f167d3], 1
0068383b: xorps xmm0, xmm0
0068383e: xor eax, eax
00683840: movups xmmword ptr [rsp + 0x70], xmm0
00683845: mov qword ptr [rsp + 0x80], rax
0068384d: xorps xmm1, xmm1
00683850: movups xmmword ptr [rsp + 0x88], xmm1
00683858: mov qword ptr [rsp + 0x98], rax
00683860: cmp byte ptr [r14 + 0x60], al
00683864: jne 0x180683ae1
0068386a: mov rax, qword ptr [rip + 0x2cf628f]
00683871: mov rcx, qword ptr [rax + 0xb8]
00683878: mov rax, qword ptr [rcx]
0068387b: test rax, rax
0068387e: je 0x180683d81
00683884: mov rdx, qword ptr [rax + 0x90]
0068388b: test rdx, rdx
0068388e: je 0x180683d81
00683894: mov r8, qword ptr [rip + 0x2cf3eb5]
0068389b: lea rcx, [rsp + 0x38]
006838a0: call 0x18169b560
006838a5: movups xmm0, xmmword ptr [rsp + 0x38]
006838aa: movups xmmword ptr [rsp + 0x70], xmm0
006838af: movsd xmm1, qword ptr [rsp + 0x48]
006838b5: movsd qword ptr [rsp + 0x80], xmm1
006838be: xor ebx, ebx
006838c0: mov qword ptr [rsp + 0x38], rbx
006838c5: lea rdi, [rsp + 0x70]
006838ca: mov qword ptr [rsp + 0x40], rdi
006838cf: movss xmm10, dword ptr [rip + 0x22f827c]
006838d8: movss xmm9, dword ptr [rip + 0x22f824b]
006838e1: movss xmm7, dword ptr [rip + 0x22f825f]
006838e9: movss xmm8, dword ptr [rip + 0x22f824a]
006838f2: mov rdx, qword ptr [rip + 0x2d529ff]
006838f9: lea rcx, [rsp + 0x70]
006838fe: call 0x18142fda0
00683903: test al, al
00683905: je 0x180683a99
0068390b: mov rsi, qword ptr [rsp + 0x80]
00683913: mov rcx, qword ptr [rip + 0x2d5827e]
0068391a: cmp dword ptr [rcx + 0xe0], 0
00683921: jne 0x180683928
00683923: call 0x180318580
00683928: xor r8d, r8d
0068392b: xor edx, edx
0068392d: mov rcx, rsi
00683930: call 0x1826b9be0
00683935: test al, al
00683937: je 0x1806838f2
00683939: test rsi, rsi
0068393c: je 0x180683d97
00683942: cmp dword ptr [rsi + 0x28], r13d
00683946: jne 0x1806838f2
00683948: cmp dword ptr [rsi + 0x24], r12d
0068394c: jne 0x1806838f2
0068394e: mov r8, qword ptr [rsi]
00683951: mov rdx, qword ptr [rip + 0x2d1eb30]
00683958: movzx eax, byte ptr [rdx + 0x130]
0068395f: cmp byte ptr [r8 + 0x130], al
00683966: jb 0x1806838f2
00683968: movzx ecx, al
0068396b: mov rax, qword ptr [r8 + 0xc8]
00683972: cmp qword ptr [rax + rcx*8 - 8], rdx
00683977: jne 0x1806838f2
0068397d: cmp byte ptr [rsi + 0x58], 0
00683981: je 0x1806838f2
00683987: mov qword ptr [r14 + 0x68], rsi
0068398b: lea rcx, [r14 + 0x68]
0068398f: mov rdx, rsi
00683992: call 0x180362730
00683997: xor edx, edx
00683999: mov rcx, r14
0068399c: call 0x1826b1710
006839a1: mov r15, rax
006839a4: xor edx, edx
006839a6: mov rcx, rsi
006839a9: call 0x1826b1710
006839ae: test rax, rax
006839b1: je 0x180683d92
006839b7: xor r8d, r8d
006839ba: mov rdx, rax
006839bd: lea rcx, [rsp + 0xa0]
006839c5: call 0x1826c8510
006839ca: movsd xmm6, qword ptr [rax]
006839ce: mov esi, dword ptr [rax + 8]
006839d1: mov qword ptr [rsp + 0x30], rbx
006839d6: mov r8, qword ptr [rip + 0x2cfaa43]
006839dd: movaps xmm1, xmm10
006839e1: lea rcx, [rsp + 0x30]
006839e6: call 0x181820660
006839eb: movsd qword ptr [rsp + 0x50], xmm6
006839f1: mov dword ptr [rsp + 0x58], esi
006839f5: mov qword ptr [rsp + 0x28], rbx
006839fa: mov qword ptr [rsp + 0x20], rbx
006839ff: mov r9, qword ptr [rsp + 0x30]
00683a04: mov r8, rbx
00683a07: lea rdx, [rsp + 0x50]
00683a0c: lea rcx, [rsp + 0xb0]
00683a14: call 0x180a12ef0
00683a19: test r15, r15
00683a1c: je 0x180683d8d
00683a22: movsd xmm0, qword ptr [rax]
00683a26: movsd qword ptr [rsp + 0x60], xmm0
00683a2c: mov eax, dword ptr [rax + 8]
00683a2f: mov dword ptr [rsp + 0x68], eax
00683a33: xor r8d, r8d
00683a36: lea rdx, [rsp + 0x60]
00683a3b: mov rcx, r15
00683a3e: call 0x1826c8c20
00683a43: mov rcx, qword ptr [r14 + 0x78]
00683a47: test rcx, rcx
00683a4a: je 0x180683d87
00683a50: xor r9d, r9d
00683a53: movaps xmm2, xmm9
00683a57: mov rdx, qword ptr [rip + 0x2d5e30a]
00683a5e: call 0x1826785c0
00683a63: mov byte ptr [r14 + 0x80], 1
00683a6b: mov rcx, qword ptr [rip + 0x2d2010e]
00683a72: cmp dword ptr [rcx + 0xe0], 0
00683a79: jne 0x180683a80
00683a7b: call 0x180318580
00683a80: xor r9d, r9d
00683a83: movaps xmm2, xmm7
00683a86: movaps xmm1, xmm8
00683a8a: mov ecx, 0x41
00683a8f: call 0x18066a930
00683a94: jmp 0x1806838f2
00683a99: mov rdx, qword ptr [rip + 0x2d527a8]
00683aa0: mov rcx, rdi
00683aa3: call 0x1803eab80
00683aa8: jmp 0x180683b06
00683aaa: mov rdx, qword ptr [rip + 0x2d52797]
00683ab1: mov rcx, qword ptr [rsp + 0x40]
00683ab6: call 0x1803eab80
00683abb: mov rcx, qword ptr [rsp + 0x38]
00683ac0: test rcx, rcx
00683ac3: jne 0x180683d9d
00683ac9: mov r12d, dword ptr [rsp + 0x170]
00683ad1: mov r13d, dword ptr [rsp + 0x168]
00683ad9: mov r14, qword ptr [rsp + 0x160]
00683ae1: movss xmm8, dword ptr [rip + 0x22f8052]
00683aea: movss xmm7, dword ptr [rip + 0x22f8056]
00683af2: movss xmm9, dword ptr [rip + 0x22f8031]
00683afb: movss xmm10, dword ptr [rip + 0x22f8050]
00683b04: xor ebx, ebx
00683b06: mov rax, qword ptr [rip + 0x2cf5ff3]
00683b0d: mov rcx, qword ptr [rax + 0xb8]
00683b14: mov rax, qword ptr [rcx]
00683b17: test rax, rax
00683b1a: je 0x180683d81
00683b20: mov rax, qword ptr [rax + 0x40]
00683b24: test rax, rax
00683b27: je 0x180683d81
00683b2d: mov rdx, qword ptr [rax + 0x10]
00683b31: test rdx, rdx
00683b34: je 0x180683d81
00683b3a: mov r8, qword ptr [rip + 0x2d0efe7]
00683b41: lea rcx, [rsp + 0x38]
00683b46: call 0x18169b560
00683b4b: movups xmm0, xmmword ptr [rsp + 0x38]
00683b50: movups xmmword ptr [rsp + 0x88], xmm0
00683b58: movsd xmm1, qword ptr [rsp + 0x48]
00683b5e: movsd qword ptr [rsp + 0x98], xmm1
00683b67: mov qword ptr [rsp + 0x38], rbx
00683b6c: lea rdi, [rsp + 0x88]
00683b74: mov qword ptr [rsp + 0x40], rdi
00683b79: nop dword ptr [rax]
00683b80: mov rdx, qword ptr [rip + 0x2d5d6b9]
00683b87: lea rcx, [rsp + 0x88]
00683b8f: call 0x18142fda0
00683b94: test al, al
00683b96: je 0x180683cf8
00683b9c: mov rsi, qword ptr [rsp + 0x98]
00683ba4: mov rcx, qword ptr [rip + 0x2d57fed]
00683bab: cmp dword ptr [rcx + 0xe0], 0
00683bb2: jne 0x180683bb9
00683bb4: call 0x180318580
00683bb9: xor r8d, r8d
00683bbc: xor edx, edx
00683bbe: mov rcx, rsi
00683bc1: call 0x1826b9be0
00683bc6: test al, al
00683bc8: je 0x180683b80
00683bca: test rsi, rsi
00683bcd: je 0x180683db2
00683bd3: cmp dword ptr [rsi + 0xf4], r13d
00683bda: jne 0x180683b80
00683bdc: cmp dword ptr [rsi + 0xf8], r12d
00683be3: jne 0x180683b80
00683be5: mov qword ptr [r14 + 0x70], rsi
00683be9: lea rcx, [r14 + 0x70]
00683bed: mov rdx, rsi
00683bf0: call 0x180362730
00683bf5: xor edx, edx
00683bf7: mov rcx, r14
00683bfa: call 0x1826b1710
00683bff: mov r15, rax
00683c02: mov rdx, qword ptr [rsi + 0x38]
00683c06: test rdx, rdx
00683c09: je 0x180683dad
00683c0f: xor r8d, r8d
00683c12: lea rcx, [rsp + 0xb0]
00683c1a: call 0x1826c8510
00683c1f: movsd xmm6, qword ptr [rax]
00683c23: mov esi, dword ptr [rax + 8]
00683c26: mov qword ptr [rsp + 0x30], rbx
00683c2b: mov r8, qword ptr [rip + 0x2cfa7ee]
00683c32: movaps xmm1, xmm10
00683c36: lea rcx, [rsp + 0x30]
00683c3b: call 0x181820660
00683c40: movsd qword ptr [rsp + 0x60], xmm6
00683c46: mov dword ptr [rsp + 0x68], esi
00683c4a: mov qword ptr [rsp + 0x28], rbx
00683c4f: mov qword ptr [rsp + 0x20], rbx
00683c54: mov r9, qword ptr [rsp + 0x30]
00683c59: mov r8, rbx
00683c5c: lea rdx, [rsp + 0x60]
00683c61: lea rcx, [rsp + 0xa0]
00683c69: call 0x180a12ef0
00683c6e: test r15, r15
00683c71: je 0x180683da8
00683c77: movsd xmm0, qword ptr [rax]
00683c7b: movsd qword ptr [rsp + 0x50], xmm0
00683c81: mov eax, dword ptr [rax + 8]
00683c84: mov dword ptr [rsp + 0x58], eax
00683c88: xor r8d, r8d
00683c8b: lea rdx, [rsp + 0x50]
00683c90: mov rcx, r15
00683c93: call 0x1826c8c20
00683c98: cmp byte ptr [r14 + 0x80], 0
00683ca0: jne 0x180683cc2
00683ca2: mov rcx, qword ptr [r14 + 0x78]
00683ca6: test rcx, rcx
00683ca9: je 0x180683da3
00683caf: xor r9d, r9d
00683cb2: movaps xmm2, xmm9
00683cb6: mov rdx, qword ptr [rip + 0x2d5e0ab]
00683cbd: call 0x1826785c0
00683cc2: mov byte ptr [r14 + 0x80], 1
00683cca: mov rcx, qword ptr [rip + 0x2d1feaf]
00683cd1: cmp dword ptr [rcx + 0xe0], 0
00683cd8: jne 0x180683cdf
00683cda: call 0x180318580
00683cdf: xor r9d, r9d
00683ce2: movaps xmm2, xmm7
00683ce5: movaps xmm1, xmm8
00683ce9: mov ecx, 0x41
00683cee: call 0x18066a930
00683cf3: jmp 0x180683b80
00683cf8: mov rdx, qword ptr [rip + 0x2d5d491]
00683cff: mov rcx, rdi
00683d02: call 0x1803eab80
00683d07: jmp 0x180683d30
00683d09: mov rdx, qword ptr [rip + 0x2d5d480]
00683d10: mov rcx, qword ptr [rsp + 0x40]
00683d15: call 0x1803eab80
00683d1a: mov rcx, qword ptr [rsp + 0x38]
00683d1f: test rcx, rcx
00683d22: jne 0x180683db8
00683d28: mov r14, qword ptr [rsp + 0x160]
00683d30: mov rdx, qword ptr [rip + 0x2d2c369]
00683d37: mov rcx, r14
00683d3a: call 0x180ac9290
00683d3f: test rax, rax
00683d42: je 0x180683d81
00683d44: xor r8d, r8d
00683d47: mov dl, 1
00683d49: mov rcx, rax
00683d4c: call 0x1826b0bd0
00683d51: lea r11, [rsp + 0x120]
00683d59: movaps xmm6, xmmword ptr [r11 - 0x10]
00683d5e: movaps xmm7, xmmword ptr [r11 - 0x20]
00683d63: movaps xmm8, xmmword ptr [r11 - 0x30]
00683d68: movaps xmm9, xmmword ptr [r11 - 0x40]
00683d6d: movaps xmm10, xmmword ptr [r11 - 0x50]
00683d72: mov rsp, r11
00683d75: pop r15
00683d77: pop r14
00683d79: pop r13
00683d7b: pop r12
00683d7d: pop rdi
00683d7e: pop rsi
00683d7f: pop rbx
00683d80: ret
00683d81: call 0x180363610
00683d86: nop
00683d87: call 0x180363610
00683d8c: nop
00683d8d: call 0x180363610
00683d92: call 0x180363610
00683d97: call 0x180363610
00683d9c: nop
00683d9d: call 0x1803636a0
00683da2: nop
00683da3: call 0x180363610
00683da8: call 0x180363610
00683dad: call 0x180363610
00683db2: call 0x180363610
00683db7: nop
00683db8: call 0x1803636a0
00683dbd: int3
00683dbe: int3
00683dbf: int3
