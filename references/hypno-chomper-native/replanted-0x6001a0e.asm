006b3890: mov qword ptr [rsp + 0x18], rbx
006b3895: push rbp
006b3896: push rsi
006b3897: push rdi
006b3898: sub rsp, 0x80
006b389f: cmp byte ptr [rip + 0x4837d8f], 0
006b38a6: mov esi, r8d
006b38a9: movsxd rdi, r9d
006b38ac: mov ebp, edx
006b38ae: mov rbx, rcx
006b38b1: jne 0x1806b39ba
006b38b7: lea rcx, [rip + 0x4577702]
006b38be: call 0x1804feb70
006b38c3: lea rcx, [rip + 0x45c189e]
006b38ca: call 0x1804feb70
006b38cf: lea rcx, [rip + 0x4579ba2]
006b38d6: call 0x1804feb70
006b38db: lea rcx, [rip + 0x45f910e]
006b38e2: call 0x1804feb70
006b38e7: lea rcx, [rip + 0x45dccba]
006b38ee: call 0x1804feb70
006b38f3: lea rcx, [rip + 0x45d39f6]
006b38fa: call 0x1804feb70
006b38ff: lea rcx, [rip + 0x45e96d2]
006b3906: call 0x1804feb70
006b390b: lea rcx, [rip + 0x45d499e]
006b3912: call 0x1804feb70
006b3917: lea rcx, [rip + 0x45d4872]
006b391e: call 0x1804feb70
006b3923: lea rcx, [rip + 0x45f8e4e]
006b392a: call 0x1804feb70
006b392f: lea rcx, [rip + 0x45d3b6a]
006b3936: call 0x1804feb70
006b393b: lea rcx, [rip + 0x45bccce]
006b3942: call 0x1804feb70
006b3947: lea rcx, [rip + 0x45a5a02]
006b394e: call 0x1804feb70
006b3953: lea rcx, [rip + 0x45fab56]
006b395a: call 0x1804feb70
006b395f: lea rcx, [rip + 0x45e2642]
006b3966: call 0x1804feb70
006b396b: lea rcx, [rip + 0x45fb90e]
006b3972: call 0x1804feb70
006b3977: lea rcx, [rip + 0x45d389a]
006b397e: call 0x1804feb70
006b3983: lea rcx, [rip + 0x45b4e5e]
006b398a: call 0x1804feb70
006b398f: lea rcx, [rip + 0x45e4cda]
006b3996: call 0x1804feb70
006b399b: lea rcx, [rip + 0x45d3a26]
006b39a2: call 0x1804feb70
006b39a7: lea rcx, [rip + 0x45f41ca]
006b39ae: call 0x1804feb70
006b39b3: mov byte ptr [rip + 0x4837c7b], 1
006b39ba: cmp qword ptr [rbx + 0x18], 0
006b39bf: mov qword ptr [rsp + 0xa0], r14
006b39c7: mov qword ptr [rsp + 0xa8], r15
006b39cf: movaps xmmword ptr [rsp + 0x70], xmm6
006b39d4: movaps xmmword ptr [rsp + 0x60], xmm7
006b39d9: movaps xmmword ptr [rsp + 0x50], xmm8
006b39df: mov dword ptr [rbx + 0x74], ebp
006b39e2: mov dword ptr [rbx + 0x34], esi
006b39e5: je 0x1806b3a2a
006b39e7: mov rcx, qword ptr [rbx + 0x18]
006b39eb: xor r9d, r9d
006b39ee: mov r8d, esi
006b39f1: mov edx, ebp
006b39f3: call 0x18065f590
006b39f8: mov rcx, qword ptr [rbx + 0x18]
006b39fc: movd xmm0, eax
006b3a00: cvtdq2ps xmm0, xmm0
006b3a03: movss dword ptr [rbx + 0x20], xmm0
006b3a08: test rcx, rcx
006b3a0b: je 0x1806b4b6e
006b3a11: xor r9d, r9d
006b3a14: mov r8d, esi
006b3a17: mov edx, ebp
006b3a19: call 0x18065f630
006b3a1e: movd xmm0, eax
006b3a22: cvtdq2ps xmm0, xmm0
006b3a25: movss dword ptr [rbx + 0x24], xmm0
006b3a2a: xor r15d, r15d
006b3a2d: mov byte ptr [rbx + 0x134], 1
006b3a34: mov qword ptr [rbx + 0x78], r15
006b3a38: mov dword ptr [rbx + 0xdc], r15d
006b3a3f: mov rcx, qword ptr [rip + 0x457757a]
006b3a46: cmp dword ptr [rcx + 0xe4], r15d
006b3a4d: jne 0x1806b3a54
006b3a4f: call 0x1804feeb0
006b3a54: xor r8d, r8d
006b3a57: mov edx, 0x12
006b3a5c: mov ecx, 0xc
006b3a61: call 0x1806cbdf0
006b3a66: mov dword ptr [rbx + 0x80], eax
006b3a6c: xor ecx, ecx
006b3a6e: mov eax, dword ptr [rsp + 0xc0]
006b3a75: mov dword ptr [rbx + 0x12c], eax
006b3a7b: mov eax, dword ptr [rbx + 0x34]
006b3a7e: mov dword ptr [rbx + 0xd4], eax
006b3a84: mov qword ptr [rbx + 0x84], 5
006b3a8f: mov byte ptr [rbx + 0x135], r15b
006b3a96: mov byte ptr [rbx + 0x3c], r15b
006b3a9a: mov dword ptr [rbx + 0x70], edi
006b3a9d: mov dword ptr [rbx + 0x8c], 0x12c
006b3aa7: mov qword ptr [rbx + 0x98], 0xc8
006b3ab2: mov qword ptr [rbx + 0xcc], 0xffffffffffffffff
006b3abd: mov dword ptr [rbx + 0xa0], r15d
006b3ac4: call 0x18063cd70
006b3ac9: xor ecx, ecx
006b3acb: mov dword ptr [rbx + 0xd8], eax
006b3ad1: call 0x18063cd70
006b3ad6: xor ecx, ecx
006b3ad8: mov dword ptr [rbx + 0xe0], eax
006b3ade: call 0x18063cd70
006b3ae3: xor ecx, ecx
006b3ae5: mov dword ptr [rbx + 0xe4], eax
006b3aeb: call 0x18063cd70
006b3af0: xor ecx, ecx
006b3af2: mov dword ptr [rbx + 0xe8], eax
006b3af8: call 0x18063cd70
006b3afd: xor ecx, ecx
006b3aff: mov dword ptr [rbx + 0xec], eax
006b3b05: call 0x18063cd70
006b3b0a: xor ecx, ecx
006b3b0c: mov dword ptr [rbx + 0xf0], eax
006b3b12: call 0x18063cd70
006b3b17: xor ecx, ecx
006b3b19: mov dword ptr [rbx + 0xf4], eax
006b3b1f: call 0x18063cd70
006b3b24: xor edx, edx
006b3b26: mov dword ptr [rbx + 0xf8], eax
006b3b2c: mov ecx, edi
006b3b2e: mov qword ptr [rbx + 0xfc], r15
006b3b35: mov qword ptr [rbx + 0x104], r15
006b3b3c: mov dword ptr [rbx + 0x28], 0x50
006b3b43: mov dword ptr [rbx + 0x2c], 0x50
006b3b4a: mov qword ptr [rbx + 0x10c], r15
006b3b51: mov byte ptr [rbx + 0x136], r15b
006b3b58: mov qword ptr [rbx + 0x124], r15
006b3b5f: mov dword ptr [rbx + 0x130], 0xffffffff
006b3b69: call 0x18077df20
006b3b6e: mov rbp, rax
006b3b71: test rax, rax
006b3b74: je 0x1806b4b6e
006b3b7a: mov ecx, dword ptr [rax + 0x60]
006b3b7d: xor edx, edx
006b3b7f: mov dword ptr [rbx + 0xa8], ecx
006b3b85: mov ecx, dword ptr [rax + 0x5c]
006b3b88: mov dword ptr [rbx + 0x94], ecx
006b3b8e: mov rcx, rbx
006b3b91: call 0x1806ac2a0
006b3b96: mov rdx, qword ptr [rsp + 0xc8]
006b3b9e: lea rcx, [rbx + 0x140]
006b3ba5: mov qword ptr [rbx + 0x140], rdx
006b3bac: mov dword ptr [rbx + 0x38], eax
006b3baf: call 0x1804fdea0
006b3bb4: mov rcx, qword ptr [rip + 0x45798bd]
006b3bbb: mov rsi, qword ptr [rbx + 0x140]
006b3bc2: cmp dword ptr [rcx + 0xe4], r15d
006b3bc9: jne 0x1806b3bd0
006b3bcb: call 0x1804feeb0
006b3bd0: xor r8d, r8d
006b3bd3: xor edx, edx
006b3bd5: mov rcx, rsi
006b3bd8: call 0x183832770
006b3bdd: test al, al
006b3bdf: je 0x1806b3c33
006b3be1: mov rcx, qword ptr [rbx + 0x10]
006b3be5: test rcx, rcx
006b3be8: je 0x1806b4b6e
006b3bee: movzx eax, byte ptr [rsp + 0xd8]
006b3bf6: mov r9b, 1
006b3bf9: mov qword ptr [rsp + 0x38], r15
006b3bfe: mov r8, rbx
006b3c01: mov byte ptr [rsp + 0x30], r15b
006b3c06: mov edx, edi
006b3c08: mov byte ptr [rsp + 0x28], al
006b3c0c: movzx eax, byte ptr [rsp + 0xd0]
006b3c14: mov byte ptr [rsp + 0x20], al
006b3c18: call 0x1807dbab0
006b3c1d: lea rcx, [rbx + 0x140]
006b3c24: mov qword ptr [rbx + 0x140], rax
006b3c2b: mov rdx, rax
006b3c2e: call 0x1804fdea0
006b3c33: mov rax, qword ptr [rbx + 0x118]
006b3c3a: mov esi, r15d
006b3c3d: mov ecx, r15d
006b3c40: test rax, rax
006b3c43: je 0x1806b4b6e
006b3c49: mov r14d, 0x20
006b3c4f: nop
006b3c50: cmp ecx, dword ptr [rax + 0x18]
006b3c53: jge 0x1806b3c9c
006b3c55: mov rax, qword ptr [rbx + 0x118]
006b3c5c: test rax, rax
006b3c5f: je 0x1806b4b6e
006b3c65: cmp esi, dword ptr [rax + 0x18]
006b3c68: jae 0x1806b4b74
006b3c6e: mov rcx, qword ptr [r14 + rax]
006b3c72: test rcx, rcx
006b3c75: je 0x1806b4b6e
006b3c7b: xor edx, edx
006b3c7d: call 0x1806c52a0
006b3c82: mov rax, qword ptr [rbx + 0x118]
006b3c89: inc esi
006b3c8b: add r14, 8
006b3c8f: mov ecx, esi
006b3c91: test rax, rax
006b3c94: je 0x1806b4b6e
006b3c9a: jmp 0x1806b3c50
006b3c9c: cmp dword ptr [rbp + 0x40], -1
006b3ca0: movss xmm6, dword ptr [rip + 0x3580110]
006b3ca8: movss xmm7, dword ptr [rip + 0x35800fc]
006b3cb0: movss xmm8, dword ptr [rip + 0x35800fb]
006b3cb9: je 0x1806b3e5f
006b3cbf: mov rcx, qword ptr [rbx + 0x140]
006b3cc6: test rcx, rcx
006b3cc9: je 0x1806b4b6e
006b3ccf: mov rdx, qword ptr [rip + 0x45e22d2]
006b3cd6: xor r9d, r9d
006b3cd9: xor r8d, r8d
006b3cdc: call 0x1807c8940
006b3ce1: test al, al
006b3ce3: je 0x1806b3d38
006b3ce5: cmp dword ptr [rbx + 0x70], 0x33
006b3ce9: je 0x1806b3d38
006b3ceb: mov rcx, qword ptr [rip + 0x45772ce]
006b3cf2: mov rsi, qword ptr [rbx + 0x140]
006b3cf9: cmp dword ptr [rcx + 0xe4], r15d
006b3d00: jne 0x1806b3d07
006b3d02: call 0x1804feeb0
006b3d07: xor r8d, r8d
006b3d0a: movaps xmm1, xmm6
006b3d0d: movaps xmm0, xmm7
006b3d10: call 0x1806cbd40
006b3d15: test rsi, rsi
006b3d18: je 0x1806b4b6e
006b3d1e: mov rdx, qword ptr [rip + 0x45dc883]
006b3d25: movaps xmm3, xmm0
006b3d28: xor r8d, r8d
006b3d2b: mov qword ptr [rsp + 0x20], r15
006b3d30: mov rcx, rsi
006b3d33: call 0x1807c8210
006b3d38: mov rcx, qword ptr [rip + 0x4579739]
006b3d3f: mov rsi, qword ptr [rbx + 0x10]
006b3d43: cmp dword ptr [rcx + 0xe4], r15d
006b3d4a: jne 0x1806b3d51
006b3d4c: call 0x1804feeb0
006b3d51: xor r8d, r8d
006b3d54: xor edx, edx
006b3d56: mov rcx, rsi
006b3d59: call 0x183832900
006b3d5e: test al, al
006b3d60: je 0x1806b3e33
006b3d66: mov rcx, qword ptr [rbx + 0x10]
006b3d6a: test rcx, rcx
006b3d6d: je 0x1806b4b6e
006b3d73: xor edx, edx
006b3d75: call 0x1807dfa20
006b3d7a: test al, al
006b3d7c: je 0x1806b3e33
006b3d82: mov rcx, qword ptr [rbx + 0x140]
006b3d89: test rcx, rcx
006b3d8c: je 0x1806b4b6e
006b3d92: mov rdx, qword ptr [rip + 0x45d4517]
006b3d99: movaps xmm3, xmm6
006b3d9c: mov qword ptr [rsp + 0x28], r15
006b3da1: xor r8d, r8d
006b3da4: mov dword ptr [rsp + 0x20], r15d
006b3da9: call 0x1807c8270
006b3dae: cmp dword ptr [rbx + 0x70], 3
006b3db2: je 0x1806b3de9
006b3db4: cmp dword ptr [rbx + 0x70], 0x31
006b3db8: je 0x1806b3de9
006b3dba: cmp dword ptr [rbx + 0x70], 0x32
006b3dbe: jne 0x1806b3e33
006b3dc0: mov rcx, qword ptr [rip + 0x45771f9]
006b3dc7: mov rsi, qword ptr [rbx + 0x140]
006b3dce: cmp dword ptr [rcx + 0xe4], r15d
006b3dd5: jne 0x1806b3ddc
006b3dd7: call 0x1804feeb0
006b3ddc: movss xmm0, dword ptr [rip + 0x357ffbc]
006b3de4: movaps xmm1, xmm7
006b3de7: jmp 0x1806b3e11
006b3de9: mov rcx, qword ptr [rip + 0x45771d0]
006b3df0: mov rsi, qword ptr [rbx + 0x140]
006b3df7: cmp dword ptr [rcx + 0xe4], r15d
006b3dfe: jne 0x1806b3e05
006b3e00: call 0x1804feeb0
006b3e05: movss xmm1, dword ptr [rip + 0x35800d7]
006b3e0d: movaps xmm0, xmm8
006b3e11: xor r8d, r8d
006b3e14: call 0x1806cbd40
006b3e19: test rsi, rsi
006b3e1c: je 0x1806b4b6e
006b3e22: xor r9d, r9d
006b3e25: xor r8d, r8d
006b3e28: movaps xmm1, xmm0
006b3e2b: mov rcx, rsi
006b3e2e: call 0x1807c8740
006b3e33: mov rcx, qword ptr [rip + 0x4577186]
006b3e3a: cmp dword ptr [rcx + 0xe4], r15d
006b3e41: jne 0x1806b3e48
006b3e43: call 0x1804feeb0
006b3e48: xor edx, edx
006b3e4a: mov ecx, 0x190
006b3e4f: call 0x1806cbe80
006b3e54: add eax, 0x190
006b3e59: mov dword ptr [rbx + 0xfc], eax
006b3e5f: mov eax, dword ptr [rbx + 0x70]
006b3e62: cmp eax, 8
006b3e65: je 0x1806b3e7b
006b3e67: cmp eax, 0x1f
006b3e6a: ja 0x1806b3e76
006b3e6c: mov ecx, 0x8100f600
006b3e71: bt ecx, eax
006b3e74: jb 0x1806b3e7b
006b3e76: cmp eax, 0x2a
006b3e79: jne 0x1806b3e9d
006b3e7b: cmp qword ptr [rbx + 0x18], r15
006b3e7f: je 0x1806b3e9d
006b3e81: mov rcx, qword ptr [rbx + 0x18]
006b3e85: xor edx, edx
006b3e87: call 0x180671af0
006b3e8c: test al, al
006b3e8e: jne 0x1806b3e9d
006b3e90: xor r8d, r8d
006b3e93: mov dl, 1
006b3e95: mov rcx, rbx
006b3e98: call 0x1806b5700
006b3e9d: cmp dword ptr [rbx + 0xa8], r15d
006b3ea4: jg 0x1806b3eab
006b3ea6: mov eax, r15d
006b3ea9: jmp 0x1806b3f08
006b3eab: cmp dword ptr [rbx + 0x70], 1
006b3eaf: je 0x1806b3edc
006b3eb1: cmp dword ptr [rbx + 0x70], 0x29
006b3eb5: je 0x1806b3edc
006b3eb7: cmp dword ptr [rbx + 0x70], 9
006b3ebb: je 0x1806b3edc
006b3ebd: mov rcx, qword ptr [rip + 0x45770fc]
006b3ec4: mov esi, dword ptr [rbx + 0xa8]
006b3eca: cmp dword ptr [rcx + 0xe4], r15d
006b3ed1: jne 0x1806b3ed8
006b3ed3: call 0x1804feeb0
006b3ed8: xor ecx, ecx
006b3eda: jmp 0x1806b3efe
006b3edc: mov rcx, qword ptr [rip + 0x45770dd]
006b3ee3: mov esi, dword ptr [rbx + 0xa8]
006b3ee9: cmp dword ptr [rcx + 0xe4], r15d
006b3ef0: jne 0x1806b3ef7
006b3ef2: call 0x1804feeb0
006b3ef7: shr esi, 1
006b3ef9: mov ecx, 0x12c
006b3efe: xor r8d, r8d
006b3f01: mov edx, esi
006b3f03: call 0x1806cbdf0
006b3f08: mov dword ptr [rbx + 0xa4], eax
006b3f0e: cmp edi, 0x1b
006b3f11: je 0x1806b48fe
006b3f17: cmp edi, 1
006b3f1a: jne 0x1806b3faa
006b3f20: mov rcx, qword ptr [rip + 0x4579551]
006b3f27: mov rdi, qword ptr [rbx + 0x10]
006b3f2b: cmp dword ptr [rcx + 0xe4], r15d
006b3f32: jne 0x1806b3f39
006b3f34: call 0x1804feeb0
006b3f39: xor r8d, r8d
006b3f3c: xor edx, edx
006b3f3e: mov rcx, rdi
006b3f41: call 0x183832900
006b3f46: test al, al
006b3f48: je 0x1806b4a85
006b3f4e: mov rax, qword ptr [rbx + 0x10]
006b3f52: test rax, rax
006b3f55: je 0x1806b4b6e
006b3f5b: mov rcx, qword ptr [rax + 0xc8]
006b3f62: test rcx, rcx
006b3f65: je 0x1806b4a85
006b3f6b: cmp byte ptr [rcx + 0x81], r15b
006b3f72: je 0x1806b4a85
006b3f78: mov rcx, qword ptr [rbx + 0x140]
006b3f7f: test rcx, rcx
006b3f82: je 0x1806b4b6e
006b3f88: mov rdx, qword ptr [rip + 0x45e46e1]
006b3f8f: movaps xmm3, xmm8
006b3f93: mov qword ptr [rsp + 0x28], r15
006b3f98: xor r8d, r8d
006b3f9b: mov dword ptr [rsp + 0x20], r15d
006b3fa0: call 0x1807c8270
006b3fa5: jmp 0x1806b4a85
006b3faa: cmp edi, 0x34
006b3fad: ja 0x1806b3fc3
006b3faf: movabs rcx, 0x100000000000a1
006b3fb9: bt rcx, rdi
006b3fbd: jb 0x1806b488e
006b3fc3: cmp edi, 0x28
006b3fc6: je 0x1806b488e
006b3fcc: cmp edi, 0x1c
006b3fcf: je 0x1806b47e7
006b3fd5: cmp edi, 0x12
006b3fd8: je 0x1806b4725
006b3fde: cmp edi, 3
006b3fe1: je 0x1806b46ea
006b3fe7: cmp edi, 0x31
006b3fea: je 0x1806b4682
006b3ff0: cmp edi, 0x32
006b3ff3: je 0x1806b46ea
006b3ff9: cmp edi, 0x17
006b3ffc: je 0x1806b466f
006b4002: cmp edi, 0x24
006b4005: je 0x1806b4660
006b400b: cmp edi, 0x2d
006b400e: je 0x1806b4a85
006b4014: cmp edi, 2
006b4017: je 0x1806b45e4
006b401d: cmp edi, 0x30
006b4020: je 0x1806b4587
006b4026: cmp edi, 0x14
006b4029: je 0x1806b45e4
006b402f: cmp edi, 4
006b4032: je 0x1806b451a
006b4038: cmp edi, 0xb
006b403b: je 0x1806b449e
006b4041: cmp edi, 9
006b4044: je 0x1806b43df
006b404a: cmp edi, 8
006b404d: je 0x1806b436b
006b4053: cmp edi, 0x18
006b4056: je 0x1806b436b
006b405c: cmp edi, 0x1e
006b405f: je 0x1806b4355
006b4065: cmp edi, 6
006b4068: je 0x1806b4346
006b406e: cmp edi, 0x19
006b4071: je 0x1806b4297
006b4077: cmp edi, 0x16
006b407a: je 0x1806b4a85
006b4080: cmp edi, 0x26
006b4083: je 0x1806b4249
006b4089: cmp edi, 0x1a
006b408c: je 0x1806b423a
006b4092: cmp edi, 0x23
006b4095: je 0x1806b422b
006b409b: cmp edi, 0xd
006b409e: je 0x1806b4346
006b40a4: cmp edi, 0x2f
006b40a7: je 0x1806b41de
006b40ad: cmp edi, 0x22
006b40b0: je 0x1806b41b4
006b40b6: cmp edi, 0x1f
006b40b9: je 0x1806b4a85
006b40bf: cmp edi, 0x2e
006b40c2: je 0x1806b41a5
006b40c8: cmp edi, 0x33
006b40cb: je 0x1806b4a85
006b40d1: cmp edi, 0x21
006b40d4: je 0x1806b410a
006b40d6: cmp edi, 0x10
006b40d9: jne 0x1806b4a85
006b40df: xor edx, edx
006b40e1: mov rcx, rbx
006b40e4: call 0x1806b21f0
006b40e9: test al, al
006b40eb: je 0x1806b4a85
006b40f1: mov dword ptr [rbx + 0x88], 0x30
006b40fb: mov dword ptr [rbx + 0xa0], 0x64
006b4105: jmp 0x1806b4a85
006b410a: xor edx, edx
006b410c: mov rcx, rbx
006b410f: call 0x1806b21f0
006b4114: test al, al
006b4116: je 0x1806b412c
006b4118: mov dword ptr [rbx + 0x88], 0x2f
006b4122: mov dword ptr [rbx + 0xa0], 0x64
006b412c: mov rcx, qword ptr [rip + 0x4579345]
006b4133: mov rdi, qword ptr [rbx + 0x10]
006b4137: cmp dword ptr [rcx + 0xe4], r15d
006b413e: jne 0x1806b4145
006b4140: call 0x1804feeb0
006b4145: xor r8d, r8d
006b4148: xor edx, edx
006b414a: mov rcx, rdi
006b414d: call 0x183832900
006b4152: test al, al
006b4154: je 0x1806b4a85
006b415a: mov rcx, qword ptr [rbx + 0x10]
006b415e: test rcx, rcx
006b4161: je 0x1806b4b6e
006b4167: xor edx, edx
006b4169: call 0x1807e2df0
006b416e: test al, al
006b4170: je 0x1806b4a85
006b4176: mov rcx, qword ptr [rbx + 0x140]
006b417d: test rcx, rcx
006b4180: je 0x1806b4b6e
006b4186: mov r8, qword ptr [rcx]
006b4189: mov rdx, qword ptr [rip + 0x45a51c0]
006b4190: mov rax, qword ptr [r8 + 0x1f8]
006b4197: mov r8, qword ptr [r8 + 0x200]
006b419e: call rax
006b41a0: jmp 0x1806b4a85
006b41a5: mov dword ptr [rbx + 0x8c], 0x1c2
006b41af: jmp 0x1806b4a85
006b41b4: mov rcx, qword ptr [rbx + 0x140]
006b41bb: test rcx, rcx
006b41be: je 0x1806b4b6e
006b41c4: mov rdx, qword ptr [rip + 0x45b461d]
006b41cb: xor r9d, r9d
006b41ce: mov r8d, 0xffffffff
006b41d4: call 0x1807c8db0
006b41d9: jmp 0x1806b4a85
006b41de: xor edx, edx
006b41e0: mov rcx, rbx
006b41e3: call 0x1806b21f0
006b41e8: test al, al
006b41ea: je 0x1806b4a85
006b41f0: mov rcx, qword ptr [rbx + 0x140]
006b41f7: mov dword ptr [rbx + 0x88], 0x23
006b4201: mov dword ptr [rbx + 0xa0], 0x1f4
006b420b: test rcx, rcx
006b420e: je 0x1806b4b6e
006b4214: mov rdx, qword ptr [rip + 0x45f395d]
006b421b: xor r9d, r9d
006b421e: xor r8d, r8d
006b4221: call 0x1807c8230
006b4226: jmp 0x1806b4a85
006b422b: mov dword ptr [rbx + 0x9c], 0x64
006b4235: jmp 0x1806b4a85
006b423a: mov dword ptr [rbx + 0x88], 0x1e
006b4244: jmp 0x1806b4a85
006b4249: mov rcx, qword ptr [rip + 0x4576d70]
006b4250: mov rdi, qword ptr [rbx + 0x140]
006b4257: cmp dword ptr [rcx + 0xe4], r15d
006b425e: jne 0x1806b4265
006b4260: call 0x1804feeb0
006b4265: movss xmm1, dword ptr [rip + 0x357fb4f]
006b426d: xor r8d, r8d
006b4270: movaps xmm0, xmm6
006b4273: call 0x1806cbd40
006b4278: test rdi, rdi
006b427b: je 0x1806b4b6e
006b4281: xor r9d, r9d
006b4284: xor r8d, r8d
006b4287: movaps xmm1, xmm0
006b428a: mov rcx, rdi
006b428d: call 0x1807c8740
006b4292: jmp 0x1806b4a85
006b4297: mov dword ptr [rbx + 0xa0], 0x32
006b42a1: cmp qword ptr [rbx + 0x18], r15
006b42a5: je 0x1806b42c9
006b42a7: cmp byte ptr [rbx + 0x137], r15b
006b42ae: je 0x1806b42c9
006b42b0: mov rcx, qword ptr [rbx + 0x10]
006b42b4: test rcx, rcx
006b42b7: je 0x1806b4b6e
006b42bd: xor edx, edx
006b42bf: call 0x1807e3010
006b42c4: cmp eax, 0x2b
006b42c7: je 0x1806b42fe
006b42c9: movss xmm2, dword ptr [rbx + 0x24]
006b42ce: mov r9d, 0x7a121
006b42d4: movss xmm1, dword ptr [rbx + 0x20]
006b42d9: mov rcx, rbx
006b42dc: addss xmm2, dword ptr [rip + 0x357fae0]
006b42e4: addss xmm1, dword ptr [rip + 0x357fad8]
006b42ec: mov qword ptr [rsp + 0x28], r15
006b42f1: mov dword ptr [rsp + 0x20], 0x2e
006b42f9: call 0x1806ab220
006b42fe: xor edx, edx
006b4300: mov rcx, rbx
006b4303: call 0x1806b21f0
006b4308: test al, al
006b430a: je 0x1806b4a85
006b4310: mov rax, qword ptr [rbx + 0x10]
006b4314: test rax, rax
006b4317: je 0x1806b4b6e
006b431d: mov r8, qword ptr [rax + 0x58]
006b4321: test r8, r8
006b4324: je 0x1806b4b6e
006b432a: mov rdx, qword ptr [rip + 0x45c0e37]
006b4331: mov ecx, 0x19
006b4336: mov r9d, 0x6b
006b433c: call 0x180002040
006b4341: jmp 0x1806b4a85
006b4346: mov dword ptr [rbx + 0x88], 1
006b4350: jmp 0x1806b4a85
006b4355: mov dword ptr [rbx + 0x8c], 0xfa0
006b435f: mov dword ptr [rbx + 0x28], 0x78
006b4366: jmp 0x1806b4a85
006b436b: xor edx, edx
006b436d: mov rcx, rbx
006b4370: call 0x1806b21f0
006b4375: test al, al
006b4377: je 0x1806b4a85
006b437d: mov rcx, qword ptr [rip + 0x4576c3c]
006b4384: movss xmm6, dword ptr [rbx + 0x20]
006b4389: cmp dword ptr [rcx + 0xe4], r15d
006b4390: jne 0x1806b4397
006b4392: call 0x1804feeb0
006b4397: xor edx, edx
006b4399: mov ecx, 0xa
006b439e: call 0x1806cbe80
006b43a3: sub eax, 5
006b43a6: xor edx, edx
006b43a8: mov ecx, 6
006b43ad: movd xmm0, eax
006b43b1: cvtdq2ps xmm0, xmm0
006b43b4: addss xmm0, xmm6
006b43b8: movss xmm6, dword ptr [rbx + 0x24]
006b43bd: movss dword ptr [rbx + 0x20], xmm0
006b43c2: call 0x1806cbe80
006b43c7: sub eax, 3
006b43ca: movd xmm0, eax
006b43ce: cvtdq2ps xmm0, xmm0
006b43d1: addss xmm0, xmm6
006b43d5: movss dword ptr [rbx + 0x24], xmm0
006b43da: jmp 0x1806b4a85
006b43df: xor edx, edx
006b43e1: mov rcx, rbx
006b43e4: call 0x1806b21f0
006b43e9: test al, al
006b43eb: jne 0x1806b4428
006b43ed: mov rcx, qword ptr [rbx + 0x140]
006b43f4: test rcx, rcx
006b43f7: je 0x1806b4b6e
006b43fd: xor r9d, r9d
006b4400: xor r8d, r8d
006b4403: cmp byte ptr [rbx + 0x136], r15b
006b440a: jne 0x1806b441a
006b440c: mov rdx, qword ptr [rip + 0x45f8365]
006b4413: call 0x1807c8230
006b4418: jmp 0x1806b4485
006b441a: mov rdx, qword ptr [rip + 0x45f85cf]
006b4421: call 0x1807c8230
006b4426: jmp 0x1806b4485
006b4428: mov rcx, qword ptr [rip + 0x4576b91]
006b442f: movss xmm6, dword ptr [rbx + 0x20]
006b4434: cmp dword ptr [rcx + 0xe4], r15d
006b443b: jne 0x1806b4442
006b443d: call 0x1804feeb0
006b4442: xor edx, edx
006b4444: mov ecx, 0xa
006b4449: call 0x1806cbe80
006b444e: sub eax, 5
006b4451: xor edx, edx
006b4453: mov ecx, 0xa
006b4458: movd xmm0, eax
006b445c: cvtdq2ps xmm0, xmm0
006b445f: addss xmm0, xmm6
006b4463: movss xmm6, dword ptr [rbx + 0x24]
006b4468: movss dword ptr [rbx + 0x20], xmm0
006b446d: call 0x1806cbe80
006b4472: sub eax, 5
006b4475: movd xmm0, eax
006b4479: cvtdq2ps xmm0, xmm0
006b447c: addss xmm0, xmm6
006b4480: movss dword ptr [rbx + 0x24], xmm0
006b4485: mov dword ptr [rbx + 0x88], 0x17
006b448f: mov dword ptr [rbx + 0xa0], 0x2ee0
006b4499: jmp 0x1806b4a85
006b449e: xor edx, edx
006b44a0: mov rcx, rbx
006b44a3: call 0x1806b21f0
006b44a8: test al, al
006b44aa: je 0x1806b4a85
006b44b0: mov rcx, qword ptr [rbx + 0x140]
006b44b7: test rcx, rcx
006b44ba: je 0x1806b4b6e
006b44c0: mov rdx, qword ptr [rip + 0x45f9fe9]
006b44c7: mov r9d, 3
006b44cd: xor r8d, r8d
006b44d0: mov qword ptr [rsp + 0x20], r15
006b44d5: call 0x1807c81d0
006b44da: mov r8, qword ptr [rbx + 0x10]
006b44de: mov dword ptr [rbx + 0x88], 8
006b44e8: test r8, r8
006b44eb: je 0x1806b4b6e
006b44f1: mov r8, qword ptr [r8 + 0x58]
006b44f5: test r8, r8
006b44f8: je 0x1806b4b6e
006b44fe: mov rdx, qword ptr [rip + 0x45c0c63]
006b4505: mov ecx, 6
006b450a: mov r9d, 0x36
006b4510: call 0x180002040
006b4515: jmp 0x1806b4a85
006b451a: mov rcx, qword ptr [rbx + 0x140]
006b4521: test rcx, rcx
006b4524: je 0x1806b4b6e
006b452a: xor r9d, r9d
006b452d: xor r8d, r8d
006b4530: movaps xmm1, xmm8
006b4534: call 0x1807c8740
006b4539: xor edx, edx
006b453b: mov rcx, rbx
006b453e: call 0x1806b21f0
006b4543: test al, al
006b4545: jne 0x1806b4578
006b4547: mov rcx, qword ptr [rbx + 0x140]
006b454e: test rcx, rcx
006b4551: je 0x1806b4b6e
006b4557: mov rdx, qword ptr [rip + 0x45e8a7a]
006b455e: xor r9d, r9d
006b4561: xor r8d, r8d
006b4564: call 0x1807c8230
006b4569: mov dword ptr [rbx + 0x88], 0x10
006b4573: jmp 0x1806b4a85
006b4578: mov dword ptr [rbx + 0xa0], 0x5dc
006b4582: jmp 0x1806b4a85
006b4587: mov rcx, qword ptr [rip + 0x4576a32]
006b458e: mov rdi, qword ptr [rbx + 0x140]
006b4595: cmp dword ptr [rcx + 0xe4], r15d
006b459c: jne 0x1806b45a3
006b459e: call 0x1804feeb0
006b45a3: movss xmm1, dword ptr [rip + 0x357f6b5]
006b45ab: xor r8d, r8d
006b45ae: movss xmm0, dword ptr [rip + 0x357f80a]
006b45b6: call 0x1806cbd40
006b45bb: test rdi, rdi
006b45be: je 0x1806b4b6e
006b45c4: xor r9d, r9d
006b45c7: xor r8d, r8d
006b45ca: movaps xmm1, xmm0
006b45cd: mov rcx, rdi
006b45d0: call 0x1807c8740
006b45d5: mov dword ptr [rbx + 0xa0], 0xc8
006b45df: jmp 0x1806b4a85
006b45e4: xor edx, edx
006b45e6: mov rcx, rbx
006b45e9: call 0x1806b21f0
006b45ee: test al, al
006b45f0: je 0x1806b4a85
006b45f6: mov rcx, qword ptr [rbx + 0x140]
006b45fd: mov dword ptr [rbx + 0x9c], 0x64
006b4607: test rcx, rcx
006b460a: je 0x1806b4b6e
006b4610: mov rdx, qword ptr [rip + 0x45bbff9]
006b4617: mov r9d, 3
006b461d: xor r8d, r8d
006b4620: mov qword ptr [rsp + 0x20], r15
006b4625: call 0x1807c81d0
006b462a: mov r8, qword ptr [rbx + 0x10]
006b462e: test r8, r8
006b4631: je 0x1806b4b6e
006b4637: mov r8, qword ptr [r8 + 0x58]
006b463b: test r8, r8
006b463e: je 0x1806b4b6e
006b4644: mov rdx, qword ptr [rip + 0x45c0b1d]
006b464b: mov ecx, 6
006b4650: mov r9d, 0x39
006b4656: call 0x180002040
006b465b: jmp 0x1806b4a85
006b4660: mov dword ptr [rbx + 0x8c], 0x190
006b466a: jmp 0x1806b4a85
006b466f: mov dword ptr [rbx + 0x8c], 0x1f40
006b4679: mov dword ptr [rbx + 0x2c], 0x50
006b4680: jmp 0x1806b46f4
006b4682: mov dword ptr [rbx + 0x8c], 0xfa0
006b468c: mov rcx, qword ptr [rip + 0x457692d]
006b4693: cmp dword ptr [rcx + 0xe4], r15d
006b469a: jne 0x1806b46a1
006b469c: call 0x1804feeb0
006b46a1: xor edx, edx
006b46a3: mov ecx, 0x3e8
006b46a8: call 0x1806cbe80
006b46ad: mov rcx, qword ptr [rbx + 0x140]
006b46b4: add eax, 0x3e8
006b46b9: mov dword ptr [rbx + 0xfc], eax
006b46bf: test rcx, rcx
006b46c2: je 0x1806b4b6e
006b46c8: movdqa xmm0, xmmword ptr [rip + 0x357f8b0]
006b46d0: lea rdx, [rsp + 0x40]
006b46d5: xor r9d, r9d
006b46d8: movaps xmmword ptr [rsp + 0x40], xmm0
006b46dd: xor r8d, r8d
006b46e0: call 0x1807cab40
006b46e5: jmp 0x1806b4a85
006b46ea: mov dword ptr [rbx + 0x8c], 0xfa0
006b46f4: mov rcx, qword ptr [rip + 0x45768c5]
006b46fb: cmp dword ptr [rcx + 0xe4], r15d
006b4702: jne 0x1806b4709
006b4704: call 0x1804feeb0
006b4709: xor edx, edx
006b470b: mov ecx, 0x3e8
006b4710: call 0x1806cbe80
006b4715: add eax, 0x3e8
006b471a: mov dword ptr [rbx + 0xfc], eax
006b4720: jmp 0x1806b4a85
006b4725: mov rcx, qword ptr [rip + 0x4576894]
006b472c: cmp dword ptr [rcx + 0xe4], r15d
006b4733: jne 0x1806b473a
006b4735: call 0x1804feeb0
006b473a: movss xmm1, dword ptr [rip + 0x357f67a]
006b4742: xor r8d, r8d
006b4745: movaps xmm0, xmm6
006b4748: call 0x1806cbd40
006b474d: mov rcx, qword ptr [rbx + 0x140]
006b4754: movaps xmm6, xmm0
006b4757: test rcx, rcx
006b475a: je 0x1806b4b6e
006b4760: xor r9d, r9d
006b4763: xor r8d, r8d
006b4766: movaps xmm1, xmm0
006b4769: call 0x1807c8740
006b476e: mov rcx, qword ptr [rbx + 0x140]
006b4775: test rcx, rcx
006b4778: je 0x1806b4b6e
006b477e: mov rdx, qword ptr [rip + 0x45d2b6b]
006b4785: movaps xmm3, xmm6
006b4788: mov r8d, 2
006b478e: mov qword ptr [rsp + 0x20], r15
006b4793: call 0x1807c8210
006b4798: mov rcx, qword ptr [rbx + 0x140]
006b479f: test rcx, rcx
006b47a2: je 0x1806b4b6e
006b47a8: mov rdx, qword ptr [rip + 0x45d2c19]
006b47af: movaps xmm3, xmm6
006b47b2: mov r8d, 3
006b47b8: mov qword ptr [rsp + 0x20], r15
006b47bd: call 0x1807c8210
006b47c2: mov rcx, qword ptr [rbx + 0x140]
006b47c9: test rcx, rcx
006b47cc: je 0x1806b4b6e
006b47d2: mov rdx, qword ptr [rip + 0x45d2cc7]
006b47d9: movaps xmm3, xmm6
006b47dc: mov r8d, 4
006b47e2: jmp 0x1806b4a7b
006b47e7: mov rcx, qword ptr [rip + 0x45767d2]
006b47ee: cmp dword ptr [rcx + 0xe4], r15d
006b47f5: jne 0x1806b47fc
006b47f7: call 0x1804feeb0
006b47fc: movss xmm1, dword ptr [rip + 0x357f5b8]
006b4804: xor r8d, r8d
006b4807: movaps xmm0, xmm6
006b480a: call 0x1806cbd40
006b480f: mov rcx, qword ptr [rbx + 0x140]
006b4816: movaps xmm6, xmm0
006b4819: test rcx, rcx
006b481c: je 0x1806b4b6e
006b4822: xor r9d, r9d
006b4825: xor r8d, r8d
006b4828: movaps xmm1, xmm0
006b482b: call 0x1807c8740
006b4830: mov rcx, qword ptr [rbx + 0x140]
006b4837: test rcx, rcx
006b483a: je 0x1806b4b6e
006b4840: mov rdx, qword ptr [rip + 0x45d29d1]
006b4847: movaps xmm3, xmm6
006b484a: mov r8d, 2
006b4850: mov qword ptr [rsp + 0x20], r15
006b4855: call 0x1807c8210
006b485a: mov rcx, qword ptr [rbx + 0x140]
006b4861: test rcx, rcx
006b4864: je 0x1806b4b6e
006b486a: mov rdx, qword ptr [rip + 0x45d391f]
006b4871: movaps xmm3, xmm6
006b4874: mov qword ptr [rsp + 0x28], r15
006b4879: mov r8d, 3
006b487f: mov dword ptr [rsp + 0x20], r15d
006b4884: call 0x1807c8270
006b4889: jmp 0x1806b4a85
006b488e: mov rcx, qword ptr [rip + 0x4578be3]
006b4895: mov rdi, qword ptr [rbx + 0x140]
006b489c: cmp dword ptr [rcx + 0xe4], r15d
006b48a3: jne 0x1806b48aa
006b48a5: call 0x1804feeb0
006b48aa: xor r8d, r8d
006b48ad: xor edx, edx
006b48af: mov rcx, rdi
006b48b2: call 0x183832900
006b48b7: test al, al
006b48b9: je 0x1806b4a85
006b48bf: mov rcx, qword ptr [rbx + 0x140]
006b48c6: test rcx, rcx
006b48c9: je 0x1806b4b6e
006b48cf: xor r8d, r8d
006b48d2: xor edx, edx
006b48d4: call 0x1807c78a0
006b48d9: mov rcx, qword ptr [rbx + 0x140]
006b48e0: test rcx, rcx
006b48e3: je 0x1806b4b6e
006b48e9: mov rdx, qword ptr [rip + 0x45d2928]
006b48f0: movaps xmm3, xmm0
006b48f3: mov r8d, 2
006b48f9: jmp 0x1806b4a7b
006b48fe: mov dword ptr [rbx + 0x9c], 0x32
006b4908: cmp qword ptr [rbx + 0x18], r15
006b490c: je 0x1806b4a5e
006b4912: cmp byte ptr [rbx + 0x137], r15b
006b4919: je 0x1806b4a5e
006b491f: mov rcx, qword ptr [rbx + 0x10]
006b4923: test rcx, rcx
006b4926: je 0x1806b4b6e
006b492c: xor edx, edx
006b492e: call 0x1807e3010
006b4933: cmp eax, 0x2b
006b4936: je 0x1806b4a5e
006b493c: mov rcx, qword ptr [rbx + 0x10]
006b4940: test rcx, rcx
006b4943: je 0x1806b4b6e
006b4949: xor edx, edx
006b494b: call 0x1807e3010
006b4950: cmp eax, 0x32
006b4953: je 0x1806b4a5e
006b4959: mov rcx, qword ptr [rbx + 0x140]
006b4960: test rcx, rcx
006b4963: je 0x1806b4b6e
006b4969: movss xmm6, dword ptr [rip + 0x357f44b]
006b4971: xor r8d, r8d
006b4974: mov rdx, qword ptr [rip + 0x45fa905]
006b497b: movaps xmm3, xmm6
006b497e: mov qword ptr [rsp + 0x28], r15
006b4983: mov dword ptr [rsp + 0x20], 3
006b498b: call 0x1807c8270
006b4990: mov rdi, qword ptr [rbx + 0x10]
006b4994: test rdi, rdi
006b4997: je 0x1806b4b6e
006b499d: mov rdi, qword ptr [rdi + 0x208]
006b49a4: test rdi, rdi
006b49a7: je 0x1806b4b6e
006b49ad: mov rcx, qword ptr [rip + 0x4578ac4]
006b49b4: mov rdi, qword ptr [rdi + 0xb0]
006b49bb: cmp dword ptr [rcx + 0xe4], r15d
006b49c2: jne 0x1806b49c9
006b49c4: call 0x1804feeb0
006b49c9: xor r8d, r8d
006b49cc: xor edx, edx
006b49ce: mov rcx, rdi
006b49d1: call 0x183832900
006b49d6: test al, al
006b49d8: je 0x1806b4a85
006b49de: test rdi, rdi
006b49e1: je 0x1806b4b6e
006b49e7: movss xmm0, dword ptr [rip + 0x357f369]
006b49ef: mov edx, 0xffffffff
006b49f4: movss xmm3, dword ptr [rip + 0x357f244]
006b49fc: mov r8d, 2
006b4a02: mov qword ptr [rsp + 0x30], r15
006b4a07: mov rcx, rdi
006b4a0a: mov byte ptr [rsp + 0x28], r15b
006b4a0f: movss dword ptr [rsp + 0x20], xmm0
006b4a15: call 0x1807add10
006b4a1a: mov rcx, qword ptr [rbx + 0x10]
006b4a1e: test rcx, rcx
006b4a21: je 0x1806b4b6e
006b4a27: mov rcx, qword ptr [rcx + 0x100]
006b4a2e: test rcx, rcx
006b4a31: je 0x1806b4b6e
006b4a37: xor edx, edx
006b4a39: call 0x180664f70
006b4a3e: movaps xmm3, xmm6
006b4a41: mov qword ptr [rsp + 0x28], r15
006b4a46: mov r8d, 3
006b4a4c: movss dword ptr [rsp + 0x20], xmm7
006b4a52: mov edx, eax
006b4a54: mov rcx, rdi
006b4a57: call 0x1807ad960
006b4a5c: jmp 0x1806b4a85
006b4a5e: mov rcx, qword ptr [rbx + 0x140]
006b4a65: test rcx, rcx
006b4a68: je 0x1806b4b6e
006b4a6e: mov rdx, qword ptr [rip + 0x45dbb33]
006b4a75: movaps xmm3, xmm7
006b4a78: xor r8d, r8d
006b4a7b: mov qword ptr [rsp + 0x20], r15
006b4a80: call 0x1807c8210
006b4a85: mov rcx, qword ptr [rip + 0x45789ec]
006b4a8c: mov rdi, qword ptr [rbx + 0x10]
006b4a90: cmp dword ptr [rcx + 0xe4], r15d
006b4a97: jne 0x1806b4a9e
006b4a99: call 0x1804feeb0
006b4a9e: xor r8d, r8d
006b4aa1: xor edx, edx
006b4aa3: mov rcx, rdi
006b4aa6: call 0x183832900
006b4aab: test al, al
006b4aad: je 0x1806b4ae8
006b4aaf: mov rcx, qword ptr [rbx + 0x10]
006b4ab3: test rcx, rcx
006b4ab6: je 0x1806b4b6e
006b4abc: xor edx, edx
006b4abe: call 0x1807e3010
006b4ac3: cmp eax, 0x27
006b4ac6: jne 0x1806b4ae8
006b4ac8: cmp dword ptr [rbx + 0x70], 3
006b4acc: je 0x1806b4ada
006b4ace: cmp dword ptr [rbx + 0x70], 1
006b4ad2: je 0x1806b4ada
006b4ad4: cmp dword ptr [rbx + 0x70], 0x26
006b4ad8: jne 0x1806b4ae8
006b4ada: mov eax, dword ptr [rbx + 0x8c]
006b4ae0: add eax, eax
006b4ae2: mov dword ptr [rbx + 0x8c], eax
006b4ae8: cmp dword ptr [rbx + 0x70], 0x21
006b4aec: mov eax, dword ptr [rbx + 0x8c]
006b4af2: mov dword ptr [rbx + 0x90], eax
006b4af8: je 0x1806b4b3b
006b4afa: cmp qword ptr [rbx + 0x18], r15
006b4afe: je 0x1806b4b3b
006b4b00: cmp byte ptr [rbx + 0x137], r15b
006b4b07: je 0x1806b4b3b
006b4b09: mov r8d, dword ptr [rbx + 0x34]
006b4b0d: xor r9d, r9d
006b4b10: mov edx, dword ptr [rbx + 0x74]
006b4b13: mov rcx, qword ptr [rbx + 0x18]
006b4b17: call 0x18065de10
006b4b1c: test rax, rax
006b4b1f: je 0x1806b4b3b
006b4b21: mov rcx, qword ptr [rax + 0x140]
006b4b28: test rcx, rcx
006b4b2b: je 0x1806b4b6e
006b4b2d: xor r9d, r9d
006b4b30: xor r8d, r8d
006b4b33: xorps xmm1, xmm1
006b4b36: call 0x1807c8740
006b4b3b: movaps xmm8, xmmword ptr [rsp + 0x50]
006b4b41: movaps xmm7, xmmword ptr [rsp + 0x60]
006b4b46: movaps xmm6, xmmword ptr [rsp + 0x70]
006b4b4b: mov r15, qword ptr [rsp + 0xa8]
006b4b53: mov r14, qword ptr [rsp + 0xa0]
006b4b5b: mov rbx, qword ptr [rsp + 0xb0]
006b4b63: add rsp, 0x80
006b4b6a: pop rdi
006b4b6b: pop rsi
006b4b6c: pop rbp
006b4b6d: ret
006b4b6e: call 0x1804fee20
006b4b73: int3
006b4b74: call 0x1804fee10
006b4b79: int3
006b4b7a: int3
006b4b7b: int3
006b4b7c: int3
006b4b7d: int3
006b4b7e: int3
006b4b7f: int3
