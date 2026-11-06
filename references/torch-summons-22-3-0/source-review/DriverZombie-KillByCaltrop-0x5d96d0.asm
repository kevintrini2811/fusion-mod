005d96d0: mov qword ptr [rsp + 0x10], rbx
005d96d5: mov qword ptr [rsp + 8], rcx
005d96da: push rsi
005d96db: push rdi
005d96dc: push r12
005d96de: push r14
005d96e0: push r15
005d96e2: sub rsp, 0x50
005d96e6: mov rsi, rcx
005d96e9: cmp byte ptr [rip + 0x2fc0432], 0
005d96f0: jne 0x1805d9799
005d96f6: lea rcx, [rip + 0x2dd632b]
005d96fd: call 0x1803633b0
005d9702: lea rcx, [rip + 0x2dd887f]
005d9709: call 0x1803633b0
005d970e: lea rcx, [rip + 0x2dd905b]
005d9715: call 0x1803633b0
005d971a: lea rcx, [rip + 0x2dd9df7]
005d9721: call 0x1803633b0
005d9726: lea rcx, [rip + 0x2dca453]
005d972d: call 0x1803633b0
005d9732: lea rcx, [rip + 0x2dd88b7]
005d9739: call 0x1803633b0
005d973e: lea rcx, [rip + 0x2dd97c3]
005d9745: call 0x1803633b0
005d974a: lea rcx, [rip + 0x2de656f]
005d9751: call 0x1803633b0
005d9756: lea rcx, [rip + 0x2dc00f3]
005d975d: call 0x1803633b0
005d9762: lea rcx, [rip + 0x2da010f]
005d9769: call 0x1803633b0
005d976e: lea rcx, [rip + 0x2deb74b]
005d9775: call 0x1803633b0
005d977a: lea rcx, [rip + 0x2d9efd7]
005d9781: call 0x1803633b0
005d9786: lea rcx, [rip + 0x2df1b53]
005d978d: call 0x1803633b0
005d9792: mov byte ptr [rip + 0x2fc0389], 1
005d9799: xor r14d, r14d
005d979c: mov qword ptr [rsp + 0x98], r14
005d97a4: mov qword ptr [rsp + 0x20], r14
005d97a9: mov rcx, qword ptr [rsi + 0x28]
005d97ad: test rcx, rcx
005d97b0: je 0x1805d9c96
005d97b6: xor r8d, r8d
005d97b9: mov rdx, qword ptr [rip + 0x2deb700]
005d97c0: call 0x182678fd0
005d97c5: mov rcx, qword ptr [rsi + 0x28]
005d97c9: test rcx, rcx
005d97cc: je 0x1805d9c96
005d97d2: xor r8d, r8d
005d97d5: mov rdx, qword ptr [rip + 0x2da009c]
005d97dc: call 0x182678fd0
005d97e1: xor edx, edx
005d97e3: mov rcx, rsi
005d97e6: call 0x1826b1710
005d97eb: test rax, rax
005d97ee: je 0x1805d9c96
005d97f4: xor r8d, r8d
005d97f7: xor edx, edx
005d97f9: mov rcx, rax
005d97fc: call 0x1826c6b90
005d9801: test rax, rax
005d9804: je 0x1805d9c96
005d980a: mov rdx, qword ptr [rip + 0x2dd9d07]
005d9811: mov rcx, rax
005d9814: call 0x180ac9290
005d9819: mov rbx, rax
005d981c: mov rcx, qword ptr [rip + 0x2dca35d]
005d9823: cmp dword ptr [rcx + 0xe0], r14d
005d982a: jne 0x1805d9838
005d982c: call 0x180318580
005d9831: mov rcx, qword ptr [rip + 0x2dca348]
005d9838: mov rcx, qword ptr [rcx + 0xb8]
005d983f: mov rdx, qword ptr [rcx + 0x30]
005d9843: test rdx, rdx
005d9846: je 0x1805d9c96
005d984c: cmp dword ptr [rdx + 0x18], 0x1e
005d9850: jbe 0x1805d9ccf
005d9856: test rbx, rbx
005d9859: je 0x1805d9c96
005d985f: xor r8d, r8d
005d9862: mov rdx, qword ptr [rdx + 0x110]
005d9869: mov rcx, rbx
005d986c: call 0x1826c2030
005d9871: xor edx, edx
005d9873: mov rcx, rsi
005d9876: call 0x1826b1710
005d987b: test rax, rax
005d987e: je 0x1805d9c96
005d9884: xor r8d, r8d
005d9887: mov edx, 1
005d988c: mov rcx, rax
005d988f: call 0x1826c6b90
005d9894: test rax, rax
005d9897: je 0x1805d9c96
005d989d: mov rdx, qword ptr [rip + 0x2dd9c74]
005d98a4: mov rcx, rax
005d98a7: call 0x180ac9290
005d98ac: mov rcx, qword ptr [rip + 0x2dca2cd]
005d98b3: mov rdx, qword ptr [rcx + 0xb8]
005d98ba: mov rcx, qword ptr [rdx + 0x30]
005d98be: test rcx, rcx
005d98c1: je 0x1805d9c96
005d98c7: cmp dword ptr [rcx + 0x18], 0x20
005d98cb: jbe 0x1805d9ccf
005d98d1: test rax, rax
005d98d4: je 0x1805d9c96
005d98da: xor r8d, r8d
005d98dd: mov rdx, qword ptr [rcx + 0x120]
005d98e4: mov rcx, rax
005d98e7: call 0x1826c2030
005d98ec: xor edx, edx
005d98ee: mov rcx, rsi
005d98f1: call 0x1826b1710
005d98f6: test rax, rax
005d98f9: je 0x1805d9c96
005d98ff: xor r8d, r8d
005d9902: mov edx, 1
005d9907: mov rcx, rax
005d990a: call 0x1826c6b90
005d990f: test rax, rax
005d9912: je 0x1805d9c96
005d9918: xor r8d, r8d
005d991b: xor edx, edx
005d991d: mov rcx, rax
005d9920: call 0x1826c6b90
005d9925: test rax, rax
005d9928: je 0x1805d9c96
005d992e: xor edx, edx
005d9930: mov rcx, rax
005d9933: call 0x1826b16d0
005d9938: test rax, rax
005d993b: je 0x1805d9c96
005d9941: xor r8d, r8d
005d9944: mov dl, 1
005d9946: mov rcx, rax
005d9949: call 0x1826b53f0
005d994e: xor edx, edx
005d9950: mov rcx, rsi
005d9953: call 0x1826b1710
005d9958: test rax, rax
005d995b: je 0x1805d9c96
005d9961: xor r8d, r8d
005d9964: mov edx, 1
005d9969: mov rcx, rax
005d996c: call 0x1826c6b90
005d9971: test rax, rax
005d9974: je 0x1805d9c96
005d997a: xor r8d, r8d
005d997d: xor edx, edx
005d997f: mov rcx, rax
005d9982: call 0x1826c6b90
005d9987: test rax, rax
005d998a: je 0x1805d9c96
005d9990: xor edx, edx
005d9992: mov rcx, rax
005d9995: call 0x1826b16d0
005d999a: mov rbx, rax
005d999d: test rax, rax
005d99a0: je 0x1805d9c96
005d99a6: xor r8d, r8d
005d99a9: mov dl, 1
005d99ab: mov rcx, rax
005d99ae: call 0x1826b53f0
005d99b3: xor edx, edx
005d99b5: mov rcx, rbx
005d99b8: call 0x1826b59a0
005d99bd: test rax, rax
005d99c0: je 0x1805d9c96
005d99c6: xor edx, edx
005d99c8: mov rcx, rax
005d99cb: call 0x1826c6bd0
005d99d0: mov qword ptr [rsp + 0x98], rax
005d99d8: mov qword ptr [rsp + 0x30], r14
005d99dd: lea r12, [rsp + 0x98]
005d99e5: mov qword ptr [rsp + 0x38], r12
005d99ea: lea r15, [rsp + 0x20]
005d99ef: mov qword ptr [rsp + 0x40], r15
005d99f4: mov r8, qword ptr [rsp + 0x98]
005d99fc: test r8, r8
005d99ff: je 0x1805d9cc3
005d9a05: xor ecx, ecx
005d9a07: mov rdx, qword ptr [rip + 0x2dd94fa]
005d9a0e: call 0x180002cc0
005d9a13: test al, al
005d9a15: je 0x1805d9b9e
005d9a1b: mov rbx, qword ptr [rsp + 0x98]
005d9a23: test rbx, rbx
005d9a26: je 0x1805d9cbe
005d9a2c: mov r9, qword ptr [rip + 0x2dd94d5]
005d9a33: mov r10, qword ptr [rbx]
005d9a36: movzx ecx, r14w
005d9a3a: movzx edx, word ptr [r10 + 0x12e]
005d9a42: cmp r14w, dx
005d9a46: jae 0x1805d9a64
005d9a48: mov r8, qword ptr [r10 + 0xb0]
005d9a4f: nop
005d9a50: movzx eax, cx
005d9a53: add rax, rax
005d9a56: cmp qword ptr [r8 + rax*8], r9
005d9a5a: je 0x1805d9a99
005d9a5c: inc cx
005d9a5f: cmp cx, dx
005d9a62: jb 0x1805d9a50
005d9a64: mov r8d, 1
005d9a6a: mov rdx, r9
005d9a6d: mov rcx, rbx
005d9a70: call 0x1802f3420
005d9a75: mov rdx, rax
005d9a78: mov rax, qword ptr [rdx]
005d9a7b: mov rdx, qword ptr [rdx + 8]
005d9a7f: mov rcx, rbx
005d9a82: call rax
005d9a84: mov rbx, rax
005d9a87: mov rdx, qword ptr [rip + 0x2dbfdc2]
005d9a8e: test rax, rax
005d9a91: je 0x1805d9c9c
005d9a97: jmp 0x1805d9ab9
005d9a99: movzx ecx, cx
005d9a9c: add rcx, rcx
005d9a9f: mov ecx, dword ptr [r8 + rcx*8 + 8]
005d9aa4: inc ecx
005d9aa6: movsxd rdx, ecx
005d9aa9: shl rdx, 4
005d9aad: add rdx, 0x138
005d9ab4: add rdx, r10
005d9ab7: jmp 0x1805d9a78
005d9ab9: mov r8, qword ptr [rax]
005d9abc: movzx eax, byte ptr [rdx + 0x130]
005d9ac3: cmp byte ptr [r8 + 0x130], al
005d9aca: jb 0x1805d9cb6
005d9ad0: movzx ecx, al
005d9ad3: mov rax, qword ptr [r8 + 0xc8]
005d9ada: cmp qword ptr [rax + rcx*8 - 8], rdx
005d9adf: jne 0x1805d9cb6
005d9ae5: mov rdx, qword ptr [rip + 0x2dd849c]
005d9aec: mov rcx, rbx
005d9aef: call 0x180ac9290
005d9af4: test rax, rax
005d9af7: je 0x1805d9cb1
005d9afd: mov rdx, qword ptr [rip + 0x2dd8c6c]
005d9b04: mov rcx, rax
005d9b07: call 0x180ac9290
005d9b0c: mov rdi, rax
005d9b0f: mov ecx, dword ptr [rsi + 0xcc]
005d9b15: mov dword ptr [rsp + 0x90], ecx
005d9b1c: lea rdx, [rsp + 0x90]
005d9b24: mov rcx, qword ptr [rip + 0x2de6195]
005d9b2b: call 0x180318da0
005d9b30: xor r8d, r8d
005d9b33: mov rdx, rax
005d9b36: mov rcx, qword ptr [rip + 0x2d9ec1b]
005d9b3d: call 0x181898f40
005d9b42: test rdi, rdi
005d9b45: je 0x1805d9cac
005d9b4b: xor r8d, r8d
005d9b4e: mov rdx, rax
005d9b51: mov rcx, rdi
005d9b54: call 0x182697aa0
005d9b59: mov rdx, qword ptr [rip + 0x2dd8428]
005d9b60: mov rcx, rbx
005d9b63: call 0x180ac9290
005d9b68: test rax, rax
005d9b6b: je 0x1805d9ca7
005d9b71: mov rdx, qword ptr [rip + 0x2dd8bf8]
005d9b78: mov rcx, rax
005d9b7b: call 0x180ac9290
005d9b80: test rax, rax
005d9b83: je 0x1805d9ca2
005d9b89: xor r8d, r8d
005d9b8c: mov edx, 0x8c
005d9b91: mov rcx, rax
005d9b94: call 0x182697af0
005d9b99: jmp 0x1805d99f4
005d9b9e: mov rdx, qword ptr [rip + 0x2dd844b]
005d9ba5: mov rcx, qword ptr [r12]
005d9ba9: call 0x180362750
005d9bae: mov qword ptr [r15], rax
005d9bb1: test rax, rax
005d9bb4: je 0x1805d9bc7
005d9bb6: xor ecx, ecx
005d9bb8: mov r8, rax
005d9bbb: mov rdx, qword ptr [rip + 0x2dd842e]
005d9bc2: call 0x180002cc0
005d9bc7: jmp 0x1805d9be9
005d9bc9: lea rcx, [rsp + 0x38]
005d9bce: call 0x180404f50
005d9bd3: mov rcx, qword ptr [rsp + 0x30]
005d9bd8: test rcx, rcx
005d9bdb: jne 0x1805d9cc9
005d9be1: mov rsi, qword ptr [rsp + 0x80]
005d9be9: mov rdx, qword ptr [rip + 0x2dd5e38]
005d9bf0: mov rcx, rsi
005d9bf3: call 0x180ac9290
005d9bf8: test rax, rax
005d9bfb: je 0x1805d9c96
005d9c01: xor r8d, r8d
005d9c04: xor edx, edx
005d9c06: mov rcx, rax
005d9c09: call 0x1826b0bd0
005d9c0e: mov rbx, qword ptr [rsi + 0x150]
005d9c15: cmp byte ptr [rip + 0x2fbf225], 0
005d9c1c: jne 0x1805d9c31
005d9c1e: lea rcx, [rip + 0x2dcc5ab]
005d9c25: call 0x1803633b0
005d9c2a: mov byte ptr [rip + 0x2fbf210], 1
005d9c31: mov rax, qword ptr [rip + 0x2dcc598]
005d9c38: mov rdx, qword ptr [rax + 0xb8]
005d9c3f: movss xmm1, dword ptr [rdx + 4]
005d9c44: test rbx, rbx
005d9c47: je 0x1805d9c96
005d9c49: xor r8d, r8d
005d9c4c: movss xmm0, dword ptr [rdx]
005d9c50: unpcklps xmm0, xmm1
005d9c53: movq rdx, xmm0
005d9c58: mov rcx, rbx
005d9c5b: call 0x18270ff00
005d9c60: mov byte ptr [rsi + 0x135], 1
005d9c67: xor r9d, r9d
005d9c6a: movss xmm2, dword ptr [rip + 0x23a1efe]
005d9c72: mov rdx, qword ptr [rip + 0x2df1667]
005d9c79: mov rcx, rsi
005d9c7c: call 0x1826b6ec0
005d9c81: mov rbx, qword ptr [rsp + 0x88]
005d9c89: add rsp, 0x50
005d9c8d: pop r15
005d9c8f: pop r14
005d9c91: pop r12
005d9c93: pop rdi
005d9c94: pop rsi
005d9c95: ret
005d9c96: call 0x180363610
005d9c9b: nop
005d9c9c: call 0x180363610
005d9ca1: nop
005d9ca2: call 0x180363610
005d9ca7: call 0x180363610
005d9cac: call 0x180363610
005d9cb1: call 0x180363610
005d9cb6: mov rcx, rbx
005d9cb9: call 0x180362770
005d9cbe: call 0x180363610
005d9cc3: call 0x180363610
005d9cc8: nop
005d9cc9: call 0x1803636a0
005d9cce: int3
005d9ccf: call 0x180363600
005d9cd4: int3
005d9cd5: int3
005d9cd6: int3
005d9cd7: int3
005d9cd8: int3
005d9cd9: int3
005d9cda: int3
005d9cdb: int3
005d9cdc: int3
005d9cdd: int3
005d9cde: int3
005d9cdf: int3
