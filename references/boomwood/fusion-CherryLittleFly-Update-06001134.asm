004988e0: push rbp
004988e2: push rbx
004988e3: push r14
004988e5: lea rbp, [rsp - 0x30]
004988ea: sub rsp, 0x130
004988f1: cmp byte ptr [rip + 0x3100908], 0
004988f8: mov rbx, rcx
004988fb: jne 0x180498934
004988fd: lea rcx, [rip + 0x2ee11fc]
00498904: call 0x1803633b0
00498909: lea rcx, [rip + 0x2f1f278]
00498910: call 0x1803633b0
00498915: lea rcx, [rip + 0x2f0b264]
0049891c: call 0x1803633b0
00498921: lea rcx, [rip + 0x2f43270]
00498928: call 0x1803633b0
0049892d: mov byte ptr [rip + 0x31008cc], 1
00498934: mov rax, qword ptr [rip + 0x2f0b245]
0049893b: xor r14d, r14d
0049893e: mov qword ptr [rbp + 0x60], r14
00498942: cmp dword ptr [rax + 0xe0], r14d
00498949: jne 0x18049895a
0049894b: mov rcx, rax
0049894e: call 0x180318580
00498953: mov rax, qword ptr [rip + 0x2f0b226]
0049895a: mov rax, qword ptr [rax + 0xb8]
00498961: cmp dword ptr [rax + 0xa8], r14d
00498968: jne 0x1804990b0
0049896e: mov rcx, qword ptr [rip + 0x2f43223]
00498975: mov qword ptr [rsp + 0x158], rdi
0049897d: mov rdi, qword ptr [rbx + 0x68]
00498981: cmp dword ptr [rcx + 0xe0], r14d
00498988: jne 0x18049898f
0049898a: call 0x180318580
0049898f: xor r8d, r8d
00498992: xor edx, edx
00498994: mov rcx, rdi
00498997: call 0x1826b9a40
0049899c: test al, al
0049899e: jne 0x1804990bc
004989a4: mov rcx, qword ptr [rip + 0x2f431ed]
004989ab: mov rdi, qword ptr [rbx + 0x20]
004989af: cmp dword ptr [rcx + 0xe0], r14d
004989b6: jne 0x1804989bd
004989b8: call 0x180318580
004989bd: mov qword ptr [rsp + 0x150], rsi
004989c5: xor r8d, r8d
004989c8: movaps xmmword ptr [rsp + 0x120], xmm6
004989d0: xor edx, edx
004989d2: movaps xmmword ptr [rsp + 0x110], xmm7
004989da: mov rcx, rdi
004989dd: movaps xmmword ptr [rsp + 0x100], xmm8
004989e6: movaps xmmword ptr [rsp + 0xf0], xmm9
004989ef: movaps xmmword ptr [rsp + 0xd0], xmm11
004989f8: movaps xmmword ptr [rsp + 0xa0], xmm14
00498a01: call 0x1826b9a40
00498a06: test al, al
00498a08: jne 0x180498b44
00498a0e: mov rcx, qword ptr [rbx + 0x20]
00498a12: test rcx, rcx
00498a15: je 0x1804990d4
00498a1b: mov r8, qword ptr [rip + 0x2f1f166]
00498a22: lea rdx, [rbp + 0x60]
00498a26: call 0x180ac9930
00498a2b: test al, al
00498a2d: je 0x180498b30
00498a33: mov rdx, qword ptr [rbx + 0x20]
00498a37: xor r8d, r8d
00498a3a: mov rcx, rbx
00498a3d: call 0x18052fe70
00498a42: test al, al
00498a44: je 0x180498b30
00498a4a: mov rdx, qword ptr [rbp + 0x60]
00498a4e: test rdx, rdx
00498a51: je 0x1804990d4
00498a57: xor r8d, r8d
00498a5a: lea rcx, [rsp + 0x70]
00498a5f: call 0x18270b980
00498a64: movsd xmm0, qword ptr [rax + 0x10]
00498a69: movups xmm1, xmmword ptr [rax]
00498a6c: movsd qword ptr [rbp - 0x80], xmm0
00498a71: movsd qword ptr [rsp + 0x50], xmm1
00498a77: lea rdx, [rsp + 0x50]
00498a7c: psrldq xmm1, 8
00498a81: xor r8d, r8d
00498a84: mov rcx, rbx
00498a87: movd dword ptr [rsp + 0x58], xmm1
00498a8d: call 0x180452e80
00498a92: mov rdi, qword ptr [rbx + 0x50]
00498a96: xor edx, edx
00498a98: mov rcx, rbx
00498a9b: call 0x1826b1710
00498aa0: test rax, rax
00498aa3: je 0x1804990d4
00498aa9: xor r8d, r8d
00498aac: lea rcx, [rsp + 0x50]
00498ab1: mov rdx, rax
00498ab4: call 0x1826c8510
00498ab9: xor edx, edx
00498abb: mov rcx, rbx
00498abe: movss xmm7, dword ptr [rax]
00498ac2: call 0x1826b1710
00498ac7: test rax, rax
00498aca: je 0x1804990d4
00498ad0: xor r8d, r8d
00498ad3: lea rcx, [rsp + 0x50]
00498ad8: mov rdx, rax
00498adb: call 0x1826c8510
00498ae0: cmp byte ptr [rip + 0x310035e], r14b
00498ae7: xorps xmm8, xmm8
00498aeb: movss xmm6, dword ptr [rax + 4]
00498af0: subss xmm6, dword ptr [rip + 0x24e305c]
00498af8: jne 0x180498b0d
00498afa: lea rcx, [rip + 0x2eda0cf]
00498b01: call 0x1803633b0
00498b06: mov byte ptr [rip + 0x3100338], 1
00498b0d: mov rax, qword ptr [rip + 0x2eda0bc]
00498b14: mov rcx, qword ptr [rax + 0xb8]
00498b1b: test rdi, rdi
00498b1e: je 0x1804990d4
00498b24: movss dword ptr [rsp + 0x58], xmm8
00498b2b: jmp 0x180498f0d
00498b30: lea rcx, [rbx + 0x20]
00498b34: mov qword ptr [rbx + 0x20], r14
00498b38: xor edx, edx
00498b3a: call 0x180362730
00498b3f: jmp 0x180498f33
00498b44: mov rdx, qword ptr [rbx]
00498b47: mov rcx, rbx
00498b4a: mov rax, qword ptr [rdx + 0x198]
00498b51: mov rdx, qword ptr [rdx + 0x1a0]
00498b58: call rax
00498b5a: lea rcx, [rbx + 0x20]
00498b5e: mov qword ptr [rbx + 0x20], rax
00498b62: mov rdx, rax
00498b65: call 0x180362730
00498b6a: xor edx, edx
00498b6c: mov byte ptr [rbx + 0x38], r14b
00498b70: mov rcx, rbx
00498b73: call 0x1826b1710
00498b78: test rax, rax
00498b7b: je 0x1804990d4
00498b81: xor r8d, r8d
00498b84: lea rcx, [rsp + 0x50]
00498b89: mov rdx, rax
00498b8c: call 0x1826c8510
00498b91: movss xmm1, dword ptr [rbx + 0x44]
00498b96: mulss xmm1, dword ptr [rip + 0x24e2fde]
00498b9e: movss xmm0, dword ptr [rax]
00498ba2: subss xmm1, dword ptr [rip + 0x24e30ee]
00498baa: comiss xmm0, xmm1
00498bad: jbe 0x180498f33
00498bb3: xor edx, edx
00498bb5: mov rcx, rbx
00498bb8: call 0x1826b1710
00498bbd: xor edx, edx
00498bbf: mov rcx, rbx
00498bc2: mov rsi, rax
00498bc5: call 0x1826b1710
00498bca: test rax, rax
00498bcd: je 0x1804990d4
00498bd3: xor r8d, r8d
00498bd6: lea rcx, [rsp + 0x60]
00498bdb: mov rdx, rax
00498bde: call 0x1826c8510
00498be3: movsd xmm0, qword ptr [rax]
00498be7: mov edi, dword ptr [rax + 8]
00498bea: mov rax, qword ptr [rbx + 0x68]
00498bee: movsd qword ptr [rsp + 0x50], xmm0
00498bf4: test rax, rax
00498bf7: je 0x1804990d4
00498bfd: mov rdx, qword ptr [rax + 0x38]
00498c01: test rdx, rdx
00498c04: je 0x1804990d4
00498c0a: movaps xmmword ptr [rsp + 0xe0], xmm10
00498c13: lea rcx, [rsp + 0x60]
00498c18: movaps xmmword ptr [rsp + 0xc0], xmm12
00498c21: xor r8d, r8d
00498c24: movaps xmmword ptr [rsp + 0xb0], xmm13
00498c2d: movaps xmmword ptr [rsp + 0x90], xmm15
00498c36: call 0x1826c8510
00498c3b: movss xmm7, dword ptr [rbx + 0x70]
00498c40: xorps xmm14, xmm14
00498c44: subss xmm7, dword ptr [rip + 0x24e2ef0]
00498c4c: movsd xmm0, qword ptr [rax]
00498c50: movss xmm8, dword ptr [rax + 8]
00498c56: movaps xmm11, xmm0
00498c5a: shufps xmm11, xmm11, 0x55
00498c5f: addss xmm8, xmm14
00498c64: addss xmm11, dword ptr [rip + 0x24e3203]
00498c6d: addss xmm7, xmm0
00498c71: movsd qword ptr [rsp + 0x70], xmm0
00498c77: movss xmm6, dword ptr [rbx + 0x3c]
00498c7c: xor ecx, ecx
00498c7e: call 0x1826c5210
00498c83: cmp byte ptr [rip + 0x310037f], r14b
00498c8a: movaps xmm10, xmm0
00498c8e: mulss xmm10, xmm6
00498c93: jne 0x180498ca8
00498c95: lea rcx, [rip + 0x2f362d4]
00498c9c: call 0x1803633b0
00498ca1: mov byte ptr [rip + 0x3100361], 1
00498ca8: movss xmm15, dword ptr [rsp + 0x50]
00498caf: movaps xmm12, xmm7
00498cb3: movaps xmm6, xmm11
00498cb7: movd xmm0, edi
00498cbb: subss xmm6, dword ptr [rsp + 0x54]
00498cc1: subss xmm12, xmm15
00498cc6: movaps xmm13, xmm8
00498cca: subss xmm13, xmm0
00498ccf: movaps xmm9, xmm6
00498cd3: mulss xmm9, xmm6
00498cd8: movaps xmm0, xmm12
00498cdc: mulss xmm0, xmm12
00498ce1: movaps xmm1, xmm13
00498ce5: mulss xmm1, xmm13
00498cea: addss xmm9, xmm0
00498cef: addss xmm9, xmm1
00498cf4: ucomiss xmm9, xmm14
00498cf8: jp 0x180498d00
00498cfa: je 0x180498d91
00498d00: comiss xmm10, xmm14
00498d04: jb 0x180498d1d
00498d06: mov eax, r14d
00498d09: movaps xmm0, xmm10
00498d0d: mulss xmm0, xmm10
00498d12: comiss xmm0, xmm9
00498d16: setae al
00498d19: test eax, eax
00498d1b: jne 0x180498d91
00498d1d: mov rcx, qword ptr [rip + 0x2f3624c]
00498d24: cmp dword ptr [rcx + 0xe0], r14d
00498d2b: jne 0x180498d32
00498d2d: call 0x180318580
00498d32: xorps xmm1, xmm1
00498d35: xorps xmm0, xmm0
00498d38: cvtss2sd xmm1, xmm9
00498d3d: ucomisd xmm0, xmm1
00498d41: ja 0x180498d49
00498d43: sqrtpd xmm0, xmm1
00498d47: jmp 0x180498d51
00498d49: movaps xmm0, xmm1
00498d4c: call 0x1803be1b8
00498d51: cvtsd2ss xmm0, xmm0
00498d55: movaps xmm7, xmm12
00498d59: movaps xmm8, xmm13
00498d5d: divss xmm6, xmm0
00498d61: divss xmm7, xmm0
00498d65: divss xmm8, xmm0
00498d6a: mulss xmm6, xmm10
00498d6f: mulss xmm7, xmm10
00498d74: addss xmm6, dword ptr [rsp + 0x54]
00498d7a: mulss xmm8, xmm10
00498d7f: movd xmm0, edi
00498d83: addss xmm7, xmm15
00498d88: movaps xmm11, xmm6
00498d8c: addss xmm8, xmm0
00498d91: xor edx, edx
00498d93: mov rcx, rbx
00498d96: call 0x1826b1710
00498d9b: movaps xmm15, xmmword ptr [rsp + 0x90]
00498da4: movaps xmm13, xmmword ptr [rsp + 0xb0]
00498dad: movaps xmm12, xmmword ptr [rsp + 0xc0]
00498db6: movaps xmm10, xmmword ptr [rsp + 0xe0]
00498dbf: test rax, rax
00498dc2: je 0x1804990d4
00498dc8: xor r8d, r8d
00498dcb: lea rcx, [rsp + 0x70]
00498dd0: mov rdx, rax
00498dd3: call 0x1826c8680
00498dd8: cmp byte ptr [rip + 0x3100066], r14b
00498ddf: movups xmm9, xmmword ptr [rax]
00498de3: jne 0x180498df8
00498de5: lea rcx, [rip + 0x2ed9de4]
00498dec: call 0x1803633b0
00498df1: mov byte ptr [rip + 0x310004d], 1
00498df8: mov rax, qword ptr [rip + 0x2ed9dd1]
00498dff: movss xmm6, dword ptr [rbx + 0x40]
00498e04: movdqa xmmword ptr [rsp + 0x60], xmm9
00498e0b: mov rcx, qword ptr [rax + 0xb8]
00498e12: movups xmm0, xmmword ptr [rcx]
00498e15: xor ecx, ecx
00498e17: movaps xmmword ptr [rsp + 0x70], xmm0
00498e1c: call 0x1826c5210
00498e21: mulss xmm0, xmm6
00498e25: lea r8, [rsp + 0x70]
00498e2a: lea rdx, [rsp + 0x60]
00498e2f: mov qword ptr [rsp + 0x20], r14
00498e34: lea rcx, [rsp + 0x50]
00498e39: movaps xmm3, xmm0
00498e3c: call 0x1826a4f30
00498e41: test rsi, rsi
00498e44: je 0x1804990d4
00498e4a: movups xmm0, xmmword ptr [rax]
00498e4d: lea r8, [rsp + 0x70]
00498e52: xor r9d, r9d
00498e55: unpcklps xmm7, xmm11
00498e59: lea rdx, [rsp + 0x50]
00498e5e: mov rcx, rsi
00498e61: movaps xmmword ptr [rsp + 0x70], xmm0
00498e66: movss dword ptr [rsp + 0x58], xmm8
00498e6d: movsd qword ptr [rsp + 0x50], xmm7
00498e73: call 0x1826c7810
00498e78: mov rdi, qword ptr [rbx + 0x50]
00498e7c: xor edx, edx
00498e7e: mov rcx, rbx
00498e81: call 0x1826b1710
00498e86: test rax, rax
00498e89: je 0x1804990d4
00498e8f: xor r8d, r8d
00498e92: lea rcx, [rsp + 0x70]
00498e97: mov rdx, rax
00498e9a: call 0x1826c8510
00498e9f: xor edx, edx
00498ea1: mov rcx, rbx
00498ea4: movss xmm7, dword ptr [rax]
00498ea8: call 0x1826b1710
00498ead: test rax, rax
00498eb0: je 0x1804990d4
00498eb6: xor r8d, r8d
00498eb9: lea rcx, [rsp + 0x70]
00498ebe: mov rdx, rax
00498ec1: call 0x1826c8510
00498ec6: cmp byte ptr [rip + 0x30fff78], r14b
00498ecd: movss xmm6, dword ptr [rax + 4]
00498ed2: subss xmm6, dword ptr [rip + 0x24e2c7a]
00498eda: jne 0x180498eef
00498edc: lea rcx, [rip + 0x2ed9ced]
00498ee3: call 0x1803633b0
00498ee8: mov byte ptr [rip + 0x30fff56], 1
00498eef: mov rax, qword ptr [rip + 0x2ed9cda]
00498ef6: mov rcx, qword ptr [rax + 0xb8]
00498efd: test rdi, rdi
00498f00: je 0x1804990d4
00498f06: movss dword ptr [rsp + 0x58], xmm14
00498f0d: movups xmm0, xmmword ptr [rcx]
00498f10: lea r8, [rsp + 0x70]
00498f15: xor r9d, r9d
00498f18: unpcklps xmm7, xmm6
00498f1b: lea rdx, [rsp + 0x50]
00498f20: mov rcx, rdi
00498f23: movaps xmmword ptr [rsp + 0x70], xmm0
00498f28: movsd qword ptr [rsp + 0x50], xmm7
00498f2e: call 0x1826c7810
00498f33: cmp byte ptr [rbx + 0x38], r14b
00498f37: je 0x18049906c
00498f3d: mov rdx, qword ptr [rbx]
00498f40: mov rcx, rbx
00498f43: mov rax, qword ptr [rdx + 0x1b8]
00498f4a: mov rdx, qword ptr [rdx + 0x1c0]
00498f51: call rax
00498f53: mov rax, qword ptr [rip + 0x2ee0ba6]
00498f5a: mov rcx, qword ptr [rax + 0xb8]
00498f61: mov rax, qword ptr [rcx]
00498f64: cmp byte ptr [rbx + 0x7c], r14b
00498f68: je 0x180498fe3
00498f6a: test rax, rax
00498f6d: je 0x1804990d4
00498f73: mov rdi, qword ptr [rax + 0x48]
00498f77: xor edx, edx
00498f79: mov rcx, rbx
00498f7c: call 0x1826b1710
00498f81: test rax, rax
00498f84: je 0x1804990d4
00498f8a: xor r8d, r8d
00498f8d: lea rcx, [rsp + 0x70]
00498f92: mov rdx, rax
00498f95: call 0x1826c8510
00498f9a: movsd xmm0, qword ptr [rax]
00498f9e: movaps xmm1, xmm0
00498fa1: shufps xmm1, xmm1, 0x55
00498fa5: mov r8, qword ptr [rbx + 0x20]
00498fa9: test r8, r8
00498fac: je 0x1804990d4
00498fb2: test rdi, rdi
00498fb5: je 0x1804990d4
00498fbb: mov eax, dword ptr [rbx + 0x74]
00498fbe: mov r9d, 2
00498fc4: mov qword ptr [rsp + 0x40], r14
00498fc9: mov byte ptr [rsp + 0x38], 1
00498fce: mov qword ptr [rsp + 0x30], r14
00498fd3: mov dword ptr [rsp + 0x28], eax
00498fd7: mov eax, dword ptr [rbx + 0x80]
00498fdd: mov dword ptr [rsp + 0x20], eax
00498fe1: jmp 0x180499055
00498fe3: test rax, rax
00498fe6: je 0x1804990d4
00498fec: mov rdi, qword ptr [rax + 0x48]
00498ff0: xor edx, edx
00498ff2: mov rcx, rbx
00498ff5: call 0x1826b1710
00498ffa: test rax, rax
00498ffd: je 0x1804990d4
00499003: xor r8d, r8d
00499006: lea rcx, [rsp + 0x70]
0049900b: mov rdx, rax
0049900e: call 0x1826c8510
00499013: movsd xmm0, qword ptr [rax]
00499017: movaps xmm1, xmm0
0049901a: shufps xmm1, xmm1, 0x55
0049901e: mov r8, qword ptr [rbx + 0x20]
00499022: test r8, r8
00499025: je 0x1804990d4
0049902b: test rdi, rdi
0049902e: je 0x1804990d4
00499034: mov eax, dword ptr [rbx + 0x74]
00499037: xor r9d, r9d
0049903a: mov qword ptr [rsp + 0x40], r14
0049903f: mov byte ptr [rsp + 0x38], 1
00499044: mov qword ptr [rsp + 0x30], r14
00499049: mov dword ptr [rsp + 0x28], eax
0049904d: mov dword ptr [rsp + 0x20], 0x708
00499055: mov r8d, dword ptr [r8 + 0xcc]
0049905c: mov rcx, rdi
0049905f: unpcklps xmm0, xmm1
00499062: movq rdx, xmm0
00499067: call 0x18041e910
0049906c: movaps xmm11, xmmword ptr [rsp + 0xd0]
00499075: movaps xmm9, xmmword ptr [rsp + 0xf0]
0049907e: movaps xmm8, xmmword ptr [rsp + 0x100]
00499087: movaps xmm7, xmmword ptr [rsp + 0x110]
0049908f: movaps xmm6, xmmword ptr [rsp + 0x120]
00499097: mov rsi, qword ptr [rsp + 0x150]
0049909f: movaps xmm14, xmmword ptr [rsp + 0xa0]
004990a8: mov rdi, qword ptr [rsp + 0x158]
004990b0: add rsp, 0x130
004990b7: pop r14
004990b9: pop rbx
004990ba: pop rbp
004990bb: ret 
004990bc: mov rdx, qword ptr [rbx]
004990bf: mov rcx, rbx
004990c2: mov rax, qword ptr [rdx + 0x1b8]
004990c9: mov rdx, qword ptr [rdx + 0x1c0]
004990d0: call rax
004990d2: jmp 0x1804990a8
004990d4: call 0x180363610
004990d9: int3 
004990da: int3 
004990db: int3 
004990dc: int3 
004990dd: int3 
004990de: int3 
004990df: int3 
