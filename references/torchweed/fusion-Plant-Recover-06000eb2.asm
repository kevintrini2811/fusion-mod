0046c9d0: mov rax, rsp
0046c9d3: mov byte ptr [rax + 0x20], r9b
0046c9d7: mov dword ptr [rax + 0x18], r8d
0046c9db: movss dword ptr [rax + 0x10], xmm1
0046c9e0: mov qword ptr [rax + 8], rcx
0046c9e4: push rbx
0046c9e5: push rsi
0046c9e6: push rdi
0046c9e7: push r12
0046c9e9: push r14
0046c9eb: push r15
0046c9ed: sub rsp, 0x108
0046c9f4: movaps xmmword ptr [rax - 0x48], xmm6
0046c9f8: movaps xmmword ptr [rax - 0x58], xmm7
0046c9fc: movaps xmmword ptr [rax - 0x68], xmm8
0046ca01: movaps xmmword ptr [rax - 0x78], xmm9
0046ca06: movaps xmmword ptr [rax - 0x88], xmm10
0046ca0e: movaps xmm6, xmm1
0046ca11: mov rdi, rcx
0046ca14: mov r12, rcx
0046ca17: mov qword ptr [rsp + 0x60], rcx
0046ca1c: cmp byte ptr [rip + 0x312c69a], 0
0046ca23: jne 0x18046cafc
0046ca29: lea rcx, [rip + 0x2f057e0]
0046ca30: call 0x1803633b0
0046ca35: lea rcx, [rip + 0x2f10e44]
0046ca3c: call 0x1803633b0
0046ca41: lea rcx, [rip + 0x2f74748]
0046ca48: call 0x1803633b0
0046ca4d: lea rcx, [rip + 0x2f747ec]
0046ca54: call 0x1803633b0
0046ca59: lea rcx, [rip + 0x2f74890]
0046ca60: call 0x1803633b0
0046ca65: lea rcx, [rip + 0x2f2f184]
0046ca6c: call 0x1803633b0
0046ca71: lea rcx, [rip + 0x2f1ae78]
0046ca78: call 0x1803633b0
0046ca7d: lea rcx, [rip + 0x2f3b634]
0046ca84: call 0x1803633b0
0046ca89: lea rcx, [rip + 0x2f3b908]
0046ca90: call 0x1803633b0
0046ca95: lea rcx, [rip + 0x2f3b6d4]
0046ca9c: call 0x1803633b0
0046caa1: lea rcx, [rip + 0x2f153a8]
0046caa8: call 0x1803633b0
0046caad: lea rcx, [rip + 0x2f26074]
0046cab4: call 0x1803633b0
0046cab9: lea rcx, [rip + 0x2f6f0d8]
0046cac0: call 0x1803633b0
0046cac5: lea rcx, [rip + 0x2f738fc]
0046cacc: call 0x1803633b0
0046cad1: lea rcx, [rip + 0x2f14f68]
0046cad8: call 0x1803633b0
0046cadd: lea rcx, [rip + 0x2f2ef04]
0046cae4: call 0x1803633b0
0046cae9: lea rcx, [rip + 0x2f14d50]
0046caf0: call 0x1803633b0
0046caf5: mov byte ptr [rip + 0x312c5c1], 1
0046cafc: xor r15d, r15d
0046caff: mov qword ptr [rsp + 0x70], r15
0046cb04: xorps xmm0, xmm0
0046cb07: xor eax, eax
0046cb09: movups xmmword ptr [rsp + 0x78], xmm0
0046cb0e: mov qword ptr [rsp + 0x88], rax
0046cb16: movups xmmword ptr [rsp + 0x90], xmm0
0046cb1e: xor edx, edx
0046cb20: mov rcx, rdi
0046cb23: call 0x180472ae0
0046cb28: test rax, rax
0046cb2b: je 0x18046d34c
0046cb31: cmp dword ptr [rax + 0x18], r15d
0046cb35: jbe 0x18046d352
0046cb3b: movss xmm0, dword ptr [rax + 0x20]
0046cb40: xorps xmm10, xmm10
0046cb44: comiss xmm0, xmm10
0046cb48: ja 0x18046d2ac
0046cb4e: comiss xmm10, xmm6
0046cb52: jae 0x18046d2ac
0046cb58: mov rax, qword ptr [rdi + 0x30]
0046cb5c: test rax, rax
0046cb5f: je 0x18046d34c
0046cb65: mov rcx, qword ptr [rax + 0x2b0]
0046cb6c: test rcx, rcx
0046cb6f: je 0x18046d34c
0046cb75: mov r8, qword ptr [rip + 0x2f3b81c]
0046cb7c: mov edx, 6
0046cb81: call 0x181600980
0046cb86: test al, al
0046cb88: jne 0x18046cb9a
0046cb8a: xor edx, edx
0046cb8c: mov ecx, 0x3f0
0046cb91: call 0x1807a2090
0046cb96: test al, al
0046cb98: je 0x18046cbab
0046cb9a: mulss xmm6, dword ptr [rip + 0x250ef9a]
0046cba2: movss dword ptr [rsp + 0x148], xmm6
0046cbab: mov rcx, qword ptr [rdi + 0x1a8]
0046cbb2: test rcx, rcx
0046cbb5: je 0x18046d34c
0046cbbb: mov r8, qword ptr [rip + 0x2f3b4f6]
0046cbc2: mov edx, 0x17
0046cbc7: call 0x181600980
0046cbcc: test al, al
0046cbce: je 0x18046ce33
0046cbd4: mov edx, dword ptr [rdi + 0xf8]
0046cbda: dec edx
0046cbdc: xor r8d, r8d
0046cbdf: mov ecx, dword ptr [rdi + 0xf4]
0046cbe5: call 0x18079a530
0046cbea: test rax, rax
0046cbed: je 0x18046d34c
0046cbf3: mov r8, qword ptr [rip + 0x2f25f2e]
0046cbfa: mov rdx, rax
0046cbfd: lea rcx, [rsp + 0x40]
0046cc02: call 0x18169b560
0046cc07: movups xmm0, xmmword ptr [rsp + 0x40]
0046cc0c: movups xmmword ptr [rsp + 0x78], xmm0
0046cc11: movsd xmm1, qword ptr [rsp + 0x50]
0046cc17: movsd qword ptr [rsp + 0x88], xmm1
0046cc20: mov qword ptr [rsp + 0x40], r15
0046cc25: lea rbx, [rsp + 0x78]
0046cc2a: mov qword ptr [rsp + 0x48], rbx
0046cc2f: movss xmm7, dword ptr [rip + 0x250eef5]
0046cc37: nop word ptr [rax + rax]
0046cc40: mov rdx, qword ptr [rip + 0x2f745f9]
0046cc47: lea rcx, [rsp + 0x78]
0046cc4c: call 0x18142fda0
0046cc51: test al, al
0046cc53: je 0x18046ccb6
0046cc55: mov rsi, qword ptr [rsp + 0x88]
0046cc5d: test rsi, rsi
0046cc60: je 0x18046d35e
0046cc66: mov rcx, qword ptr [rsi + 0x1a8]
0046cc6d: test rcx, rcx
0046cc70: je 0x18046d358
0046cc76: mov r8, qword ptr [rip + 0x2f3b4f3]
0046cc7d: mov edx, 0x17
0046cc82: call 0x18160fe90
0046cc87: mov rax, qword ptr [rsi]
0046cc8a: movaps xmm1, xmm6
0046cc8d: mulss xmm1, xmm7
0046cc91: mov rdx, qword ptr [rax + 0x2c8]
0046cc98: mov rax, qword ptr [rax + 0x2d0]
0046cc9f: mov qword ptr [rsp + 0x28], rax
0046cca4: mov byte ptr [rsp + 0x20], 0
0046cca9: mov r9b, 1
0046ccac: xor r8d, r8d
0046ccaf: mov rcx, rsi
0046ccb2: call rdx
0046ccb4: jmp 0x18046cc40
0046ccb6: mov rdx, qword ptr [rip + 0x2f744d3]
0046ccbd: mov rcx, rbx
0046ccc0: call 0x1803eab80
0046ccc5: jmp 0x18046cd0b
0046ccc7: mov rdx, qword ptr [rip + 0x2f744c2]
0046ccce: mov rcx, qword ptr [rsp + 0x48]
0046ccd3: call 0x1803eab80
0046ccd8: mov rcx, qword ptr [rsp + 0x40]
0046ccdd: test rcx, rcx
0046cce0: jne 0x18046d364
0046cce6: xor r15d, r15d
0046cce9: movss xmm6, dword ptr [rsp + 0x148]
0046ccf2: mov rdi, qword ptr [rsp + 0x140]
0046ccfa: xorps xmm10, xmm10
0046ccfe: movss xmm7, dword ptr [rip + 0x250ee26]
0046cd06: mov r12, qword ptr [rsp + 0x60]
0046cd0b: mov edx, dword ptr [rdi + 0xf8]
0046cd11: inc edx
0046cd13: xor r8d, r8d
0046cd16: mov ecx, dword ptr [rdi + 0xf4]
0046cd1c: call 0x18079a530
0046cd21: test rax, rax
0046cd24: je 0x18046d34c
0046cd2a: mov r8, qword ptr [rip + 0x2f25df7]
0046cd31: mov rdx, rax
0046cd34: lea rcx, [rsp + 0x40]
0046cd39: call 0x18169b560
0046cd3e: movups xmm0, xmmword ptr [rsp + 0x40]
0046cd43: movups xmmword ptr [rsp + 0x78], xmm0
0046cd48: movsd xmm1, qword ptr [rsp + 0x50]
0046cd4e: movsd qword ptr [rsp + 0x88], xmm1
0046cd57: mov qword ptr [rsp + 0x40], r15
0046cd5c: lea rbx, [rsp + 0x78]
0046cd61: mov qword ptr [rsp + 0x48], rbx
0046cd66: nop word ptr [rax + rax]
0046cd70: mov rdx, qword ptr [rip + 0x2f744c9]
0046cd77: lea rcx, [rsp + 0x78]
0046cd7c: call 0x18142fda0
0046cd81: test al, al
0046cd83: je 0x18046cde6
0046cd85: mov rsi, qword ptr [rsp + 0x88]
0046cd8d: test rsi, rsi
0046cd90: je 0x18046d36f
0046cd96: mov rcx, qword ptr [rsi + 0x1a8]
0046cd9d: test rcx, rcx
0046cda0: je 0x18046d36a
0046cda6: mov r8, qword ptr [rip + 0x2f3b3c3]
0046cdad: mov edx, 0x17
0046cdb2: call 0x18160fe90
0046cdb7: mov rax, qword ptr [rsi]
0046cdba: movaps xmm1, xmm6
0046cdbd: mulss xmm1, xmm7
0046cdc1: mov rdx, qword ptr [rax + 0x2c8]
0046cdc8: mov rax, qword ptr [rax + 0x2d0]
0046cdcf: mov qword ptr [rsp + 0x28], rax
0046cdd4: mov byte ptr [rsp + 0x20], 0
0046cdd9: mov r9b, 1
0046cddc: xor r8d, r8d
0046cddf: mov rcx, rsi
0046cde2: call rdx
0046cde4: jmp 0x18046cd70
0046cde6: mov rdx, qword ptr [rip + 0x2f743a3]
0046cded: mov rcx, rbx
0046cdf0: call 0x1803eab80
0046cdf5: jmp 0x18046ce33
0046cdf7: mov rdx, qword ptr [rip + 0x2f74392]
0046cdfe: mov rcx, qword ptr [rsp + 0x48]
0046ce03: call 0x1803eab80
0046ce08: mov rcx, qword ptr [rsp + 0x40]
0046ce0d: test rcx, rcx
0046ce10: jne 0x18046d375
0046ce16: xor r15d, r15d
0046ce19: movss xmm6, dword ptr [rsp + 0x148]
0046ce22: mov rdi, qword ptr [rsp + 0x140]
0046ce2a: xorps xmm10, xmm10
0046ce2e: mov r12, qword ptr [rsp + 0x60]
0046ce33: mov rax, qword ptr [rdi + 0x30]
0046ce37: test rax, rax
0046ce3a: je 0x18046d34c
0046ce40: cmp byte ptr [rax + 0x27a], 0
0046ce47: je 0x18046ce6d
0046ce49: xor ecx, ecx
0046ce4b: call 0x1805f7790
0046ce50: test rax, rax
0046ce53: je 0x18046d34c
0046ce59: cmp dword ptr [rax + 0x14], 2
0046ce5d: jne 0x18046ce6d
0046ce5f: movss xmm7, dword ptr [rip + 0x250eced]
0046ce67: mulss xmm6, xmm7
0046ce6b: jmp 0x18046ce75
0046ce6d: movss xmm7, dword ptr [rip + 0x250ecdf]
0046ce75: xor ecx, ecx
0046ce77: call 0x1807a2030
0046ce7c: movss xmm9, dword ptr [rip + 0x250ecbf]
0046ce85: test al, al
0046ce87: je 0x18046ce8e
0046ce89: mulss xmm6, xmm9
0046ce8e: mov rcx, qword ptr [rdi + 0x58]
0046ce92: test rcx, rcx
0046ce95: je 0x18046d34c
0046ce9b: mov r8, qword ptr [rip + 0x2f14fae]
0046cea2: mov edx, 5
0046cea7: call 0x1816ff910
0046ceac: test al, al
0046ceae: je 0x18046ceb4
0046ceb0: mulss xmm6, xmm7
0046ceb4: xor edx, edx
0046ceb6: mov ecx, 0x3fc
0046cebb: call 0x1807a1fb0
0046cec0: test al, al
0046cec2: je 0x18046cecc
0046cec4: mulss xmm6, dword ptr [rip + 0x250ec80]
0046cecc: cmp byte ptr [rsp + 0x160], 0
0046ced4: jne 0x18046cf0d
0046ced6: xor edx, edx
0046ced8: mov ecx, 0x3fb
0046cedd: call 0x1807a1fb0
0046cee2: test al, al
0046cee4: je 0x18046cf0d
0046cee6: mov qword ptr [rsp + 0x20], r15
0046ceeb: movss xmm7, dword ptr [rip + 0x250ec55]
0046cef3: movaps xmm3, xmm7
0046cef6: movss xmm2, dword ptr [rip + 0x250ec76]
0046cefe: mov edx, 0x68
0046cf03: mov rcx, rdi
0046cf06: call 0x1803f0a10
0046cf0b: jmp 0x18046cf15
0046cf0d: movss xmm7, dword ptr [rip + 0x250ec33]
0046cf15: mov r9, qword ptr [rip + 0x2f052f4]
0046cf1c: lea r8, [rsp + 0x70]
0046cf21: mov edx, 0x67
0046cf26: mov rcx, rdi
0046cf29: call 0x180ae1610
0046cf2e: test al, al
0046cf30: je 0x18046cfa7
0046cf32: mov rcx, qword ptr [rsp + 0x70]
0046cf37: test rcx, rcx
0046cf3a: je 0x18046d34c
0046cf40: mov rdx, qword ptr [rcx]
0046cf43: mov rax, qword ptr [rdx + 0x1d8]
0046cf4a: mov rdx, qword ptr [rdx + 0x1e0]
0046cf51: call rax
0046cf53: movaps xmm8, xmm0
0046cf57: comiss xmm6, xmm0
0046cf5a: ja 0x18046cf60
0046cf5c: movaps xmm8, xmm6
0046cf60: mov rbx, qword ptr [rsp + 0x70]
0046cf65: test rbx, rbx
0046cf68: je 0x18046d34c
0046cf6e: mov rdx, qword ptr [rbx]
0046cf71: mov rax, qword ptr [rdx + 0x1d8]
0046cf78: mov rdx, qword ptr [rdx + 0x1e0]
0046cf7f: mov rcx, rbx
0046cf82: call rax
0046cf84: mov r8, qword ptr [rbx]
0046cf87: subss xmm0, xmm8
0046cf8c: mov rax, qword ptr [r8 + 0x1e8]
0046cf93: mov r8, qword ptr [r8 + 0x1f0]
0046cf9a: movaps xmm1, xmm0
0046cf9d: mov rcx, rbx
0046cfa0: call rax
0046cfa2: subss xmm6, xmm8
0046cfa7: xor r8d, r8d
0046cfaa: mov edx, 0xdc
0046cfaf: mov rcx, qword ptr [rdi + 0x30]
0046cfb3: call 0x18079c030
0046cfb8: mov r14, rax
0046cfbb: mov rax, qword ptr [rip + 0x2f1487e]
0046cfc2: cmp dword ptr [rax + 0xe0], 0
0046cfc9: jne 0x18046cfda
0046cfcb: mov rcx, rax
0046cfce: call 0x180318580
0046cfd3: mov rax, qword ptr [rip + 0x2f14866]
0046cfda: mov rcx, qword ptr [rax + 0xb8]
0046cfe1: mov rsi, qword ptr [rcx + 8]
0046cfe5: test rsi, rsi
0046cfe8: jne 0x18046d05b
0046cfea: cmp dword ptr [rax + 0xe0], esi
0046cff0: jne 0x18046d001
0046cff2: mov rcx, rax
0046cff5: call 0x180318580
0046cffa: mov rax, qword ptr [rip + 0x2f1483f]
0046d001: mov rax, qword ptr [rax + 0xb8]
0046d008: mov rbx, qword ptr [rax]
0046d00b: mov rcx, qword ptr [rip + 0x2f1a8de]
0046d012: call 0x18030dc40
0046d017: mov rsi, rax
0046d01a: xor r9d, r9d
0046d01d: mov r8, qword ptr [rip + 0x2f2e9c4]
0046d024: mov rdx, rbx
0046d027: mov rcx, rax
0046d02a: call 0x1815d0450
0046d02f: mov rax, qword ptr [rip + 0x2f1480a]
0046d036: mov rcx, qword ptr [rax + 0xb8]
0046d03d: mov qword ptr [rcx + 8], rsi
0046d041: mov rax, qword ptr [rip + 0x2f147f8]
0046d048: mov rcx, qword ptr [rax + 0xb8]
0046d04f: add rcx, 8
0046d053: mov rdx, rsi
0046d056: call 0x180362730
0046d05b: mov r8, qword ptr [rip + 0x2f1081e]
0046d062: mov rdx, rsi
0046d065: mov rcx, r14
0046d068: call 0x180aee650
0046d06d: mov rbx, rax
0046d070: mov rcx, qword ptr [rip + 0x2f6eb21]
0046d077: cmp dword ptr [rcx + 0xe0], 0
0046d07e: jne 0x18046d085
0046d080: call 0x180318580
0046d085: xor r8d, r8d
0046d088: xor edx, edx
0046d08a: mov rcx, rbx
0046d08d: call 0x1826b9be0
0046d092: test al, al
0046d094: jne 0x18046d2da
0046d09a: cmp dword ptr [rsp + 0x150], 0xf
0046d0a2: jne 0x18046d0ae
0046d0a4: xor edx, edx
0046d0a6: mov rcx, rdi
0046d0a9: call 0x180469900
0046d0ae: cvttss2si eax, xmm6
0046d0b2: add eax, dword ptr [rdi + 0xe8]
0046d0b8: js 0x18046d136
0046d0ba: cvttss2si eax, xmm6
0046d0be: add dword ptr [rdi + 0xe8], eax
0046d0c4: mov qword ptr [rsp + 0x98], r15
0046d0cc: mov qword ptr [rsp + 0x90], rdi
0046d0d4: mov rdx, rdi
0046d0d7: lea rcx, [rsp + 0x90]
0046d0df: call 0x180362730
0046d0e4: cvttss2si eax, xmm6
0046d0e8: mov dword ptr [rsp + 0x98], eax
0046d0ef: movaps xmm0, xmmword ptr [rsp + 0x90]
0046d0f7: movdqa xmmword ptr [rsp + 0x40], xmm0
0046d0fd: lea rdx, [rsp + 0x40]
0046d102: mov rcx, qword ptr [rip + 0x2f14937]
0046d109: call 0x180318da0
0046d10e: mov rbx, rax
0046d111: mov rcx, qword ptr [rip + 0x2f2ead8]
0046d118: cmp dword ptr [rcx + 0xe0], 0
0046d11f: jne 0x18046d126
0046d121: call 0x180318580
0046d126: xor r8d, r8d
0046d129: mov rdx, rbx
0046d12c: mov ecx, 0xf
0046d131: call 0x180681790
0046d136: mov rdx, qword ptr [r12]
0046d13a: mov rax, qword ptr [rdx + 0x2e8]
0046d141: mov rdx, qword ptr [rdx + 0x2f0]
0046d148: mov rcx, rdi
0046d14b: call rax
0046d14d: cmp byte ptr [rsp + 0x158], 0
0046d155: je 0x18046d27e
0046d15b: mov rdx, qword ptr [rdi + 0x38]
0046d15f: test rdx, rdx
0046d162: je 0x18046d34c
0046d168: xor r8d, r8d
0046d16b: lea rcx, [rsp + 0x40]
0046d170: call 0x1826c8510
0046d175: movsd xmm0, qword ptr [rax]
0046d179: movsd qword ptr [rsp + 0x60], xmm0
0046d17f: mov eax, dword ptr [rax + 8]
0046d182: mov dword ptr [rsp + 0x68], eax
0046d186: mov rax, qword ptr [rip + 0x2f7323b]
0046d18d: cmp dword ptr [rax + 0xe0], 0
0046d194: jne 0x18046d1a5
0046d196: mov rcx, rax
0046d199: call 0x180318580
0046d19e: mov rax, qword ptr [rip + 0x2f73223]
0046d1a5: mov rax, qword ptr [rax + 0xb8]
0046d1ac: mov rcx, qword ptr [rax]
0046d1af: movss xmm1, dword ptr [rsp + 0x64]
0046d1b5: addss xmm1, xmm9
0046d1ba: test rcx, rcx
0046d1bd: je 0x18046d34c
0046d1c3: mov qword ptr [rsp + 0x30], r15
0046d1c8: movss dword ptr [rsp + 0x28], xmm10
0046d1cf: mov byte ptr [rsp + 0x20], 1
0046d1d4: mov r9d, dword ptr [rdi + 0xf8]
0046d1db: movss xmm0, dword ptr [rsp + 0x60]
0046d1e1: unpcklps xmm0, xmm1
0046d1e4: movq r8, xmm0
0046d1e9: mov edx, 0x10
0046d1ee: call 0x180465c40
0046d1f3: mov rbx, rax
0046d1f6: mov rcx, qword ptr [rip + 0x2f6e99b]
0046d1fd: cmp dword ptr [rcx + 0xe0], 0
0046d204: jne 0x18046d20b
0046d206: call 0x180318580
0046d20b: xor r8d, r8d
0046d20e: xor edx, edx
0046d210: mov rcx, rbx
0046d213: call 0x1826b9be0
0046d218: test al, al
0046d21a: je 0x18046d27e
0046d21c: test rbx, rbx
0046d21f: je 0x18046d34c
0046d225: xor edx, edx
0046d227: mov rcx, rbx
0046d22a: call 0x1826b1710
0046d22f: mov rbx, rax
0046d232: movss xmm0, dword ptr [rip + 0x250ed2a]
0046d23a: unpcklps xmm0, xmm10
0046d23e: movsd qword ptr [rsp + 0x60], xmm0
0046d244: movss dword ptr [rsp + 0x68], xmm10
0046d24b: xor r8d, r8d
0046d24e: lea rdx, [rsp + 0x60]
0046d253: lea rcx, [rsp + 0x40]
0046d258: call 0x1826a4b10
0046d25d: test rbx, rbx
0046d260: je 0x18046d34c
0046d266: movups xmm0, xmmword ptr [rax]
0046d269: movaps xmmword ptr [rsp + 0x40], xmm0
0046d26e: xor r8d, r8d
0046d271: lea rdx, [rsp + 0x40]
0046d276: mov rcx, rbx
0046d279: call 0x1826c8cc0
0046d27e: mov rdx, qword ptr [r12]
0046d282: mov rax, qword ptr [rdx + 0x2b8]
0046d289: mov rdx, qword ptr [rdx + 0x2c0]
0046d290: mov rcx, rdi
0046d293: call rax
0046d295: mov rdx, qword ptr [r12]
0046d299: mov rax, qword ptr [rdx + 0x2f8]
0046d2a0: mov rdx, qword ptr [rdx + 0x300]
0046d2a7: mov rcx, rdi
0046d2aa: call rax
0046d2ac: lea r11, [rsp + 0x108]
0046d2b4: movaps xmm6, xmmword ptr [r11 - 0x18]
0046d2b9: movaps xmm7, xmmword ptr [r11 - 0x28]
0046d2be: movaps xmm8, xmmword ptr [r11 - 0x38]
0046d2c3: movaps xmm9, xmmword ptr [r11 - 0x48]
0046d2c8: movaps xmm10, xmmword ptr [r11 - 0x58]
0046d2cd: mov rsp, r11
0046d2d0: pop r15
0046d2d2: pop r14
0046d2d4: pop r12
0046d2d6: pop rdi
0046d2d7: pop rsi
0046d2d8: pop rbx
0046d2d9: ret
0046d2da: mulss xmm6, dword ptr [rip + 0x250e842]
0046d2e2: comiss xmm7, xmm6
0046d2e5: ja 0x18046d2ea
0046d2e7: movaps xmm7, xmm6
0046d2ea: test rbx, rbx
0046d2ed: je 0x18046d34c
0046d2ef: xor edx, edx
0046d2f1: mov rcx, rbx
0046d2f4: call 0x180660220
0046d2f9: xorps xmm0, xmm0
0046d2fc: cvtsi2ss xmm0, rax
0046d301: mulss xmm0, dword ptr [rip + 0x250e7ff]
0046d309: comiss xmm0, xmm7
0046d30c: ja 0x18046d311
0046d30e: movaps xmm7, xmm0
0046d311: mov rax, qword ptr [rbx]
0046d314: cvttss2si edx, xmm7
0046d318: mov r10, qword ptr [rax + 0x1f8]
0046d31f: mov rax, qword ptr [rax + 0x200]
0046d326: mov qword ptr [rsp + 0x30], rax
0046d32b: mov byte ptr [rsp + 0x28], 0
0046d330: mov dword ptr [rsp + 0x20], 0xffffffff
0046d338: mov r9d, 4
0046d33e: mov r8, rdi
0046d341: mov rcx, rbx
0046d344: call r10
0046d347: jmp 0x18046d2ac
0046d34c: call 0x180363610
0046d351: int3
0046d352: call 0x180363600
0046d357: nop
0046d358: call 0x180363610
0046d35d: nop
0046d35e: call 0x180363610
0046d363: nop
0046d364: call 0x1803636a0
0046d369: nop
0046d36a: call 0x180363610
0046d36f: call 0x180363610
0046d374: nop
0046d375: call 0x1803636a0
0046d37a: int3
0046d37b: int3
0046d37c: int3
0046d37d: int3
0046d37e: int3
0046d37f: int3
