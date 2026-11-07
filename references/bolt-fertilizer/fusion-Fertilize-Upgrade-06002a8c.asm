00682810: mov rax, rsp
00682813: mov qword ptr [rax + 8], rcx
00682817: push rbx
00682818: push rsi
00682819: push rdi
0068281a: push r12
0068281c: push r13
0068281e: push r14
00682820: push r15
00682822: sub rsp, 0xe0
00682829: movaps xmmword ptr [rax - 0x48], xmm6
0068282d: movaps xmmword ptr [rax - 0x58], xmm7
00682831: mov r14, rcx
00682834: cmp byte ptr [rip + 0x2f177d4], 0
0068283b: jne 0x1806828a4
0068283d: lea rcx, [rip + 0x2cf72bc]
00682844: call 0x1803633b0
00682849: lea rcx, [rip + 0x2d056e8]
00682850: call 0x1803633b0
00682855: lea rcx, [rip + 0x2d5e934]
0068285c: call 0x1803633b0
00682861: lea rcx, [rip + 0x2d5e9d8]
00682868: call 0x1803633b0
0068286d: lea rcx, [rip + 0x2d5ea7c]
00682874: call 0x1803633b0
00682879: lea rcx, [rip + 0x2d102a8]
00682880: call 0x1803633b0
00682885: lea rcx, [rip + 0x2d12fac]
0068288c: call 0x1803633b0
00682891: lea rcx, [rip + 0x2d59300]
00682898: call 0x1803633b0
0068289d: mov byte ptr [rip + 0x2f1776b], 1
006828a4: xorps xmm0, xmm0
006828a7: xor eax, eax
006828a9: movups xmmword ptr [rsp + 0xa0], xmm0
006828b1: mov qword ptr [rsp + 0xb0], rax
006828b9: mov qword ptr [rsp + 0x138], rax
006828c1: mov rcx, qword ptr [rip + 0x2d592d0]
006828c8: cmp byte ptr [r14 + 0x60], al
006828cc: jne 0x1806829aa
006828d2: mov rbx, qword ptr [r14 + 0x68]
006828d6: cmp dword ptr [rcx + 0xe0], eax
006828dc: jne 0x1806828e3
006828de: call 0x180318580
006828e3: xor r8d, r8d
006828e6: xor edx, edx
006828e8: mov rcx, rbx
006828eb: call 0x1826b9be0
006828f0: test al, al
006828f2: jne 0x180682924
006828f4: mov rbx, qword ptr [r14 + 0x70]
006828f8: mov rcx, qword ptr [rip + 0x2d59299]
006828ff: cmp dword ptr [rcx + 0xe0], 0
00682906: jne 0x18068290d
00682908: call 0x180318580
0068290d: xor r8d, r8d
00682910: xor edx, edx
00682912: mov rcx, rbx
00682915: call 0x1826b9be0
0068291a: test al, al
0068291c: jne 0x1806829d0
00682922: jmp 0x18068295b
00682924: mov rcx, qword ptr [r14 + 0x68]
00682928: test rcx, rcx
0068292b: je 0x1806836ca
00682931: mov rdx, qword ptr [rcx]
00682934: mov rax, qword ptr [rdx + 0x178]
0068293b: mov rdx, qword ptr [rdx + 0x180]
00682942: call rax
00682944: mov rcx, qword ptr [r14 + 0x68]
00682948: test rcx, rcx
0068294b: je 0x1806836ca
00682951: xor r8d, r8d
00682954: xor edx, edx
00682956: call 0x18044dc80
0068295b: xor edx, edx
0068295d: mov rcx, r14
00682960: call 0x1826b16d0
00682965: mov rcx, qword ptr [rip + 0x2d5922c]
0068296c: mov rbx, rax
0068296f: cmp dword ptr [rcx + 0xe0], 0
00682976: jne 0x18068297d
00682978: call 0x180318580
0068297d: xor edx, edx
0068297f: mov rcx, rbx
00682982: call 0x1826b8410
00682987: movaps xmm6, xmmword ptr [rsp + 0xd0]
0068298f: movaps xmm7, xmmword ptr [rsp + 0xc0]
00682997: add rsp, 0xe0
0068299e: pop r15
006829a0: pop r14
006829a2: pop r13
006829a4: pop r12
006829a6: pop rdi
006829a7: pop rsi
006829a8: pop rbx
006829a9: ret
006829aa: mov rbx, qword ptr [r14 + 0x70]
006829ae: cmp dword ptr [rcx + 0xe0], eax
006829b4: jne 0x1806829bb
006829b6: call 0x180318580
006829bb: xor r8d, r8d
006829be: xor edx, edx
006829c0: mov rcx, rbx
006829c3: call 0x1826b9a40
006829c8: test al, al
006829ca: jne 0x1806835e2
006829d0: mov rax, qword ptr [r14 + 0x70]
006829d4: test rax, rax
006829d7: je 0x1806836ca
006829dd: mov r12d, dword ptr [rax + 0xf8]
006829e4: mov r15d, dword ptr [rax + 0xf4]
006829eb: xor r13b, r13b
006829ee: mov byte ptr [rsp + 0x130], r13b
006829f6: xor r8d, r8d
006829f9: mov edx, r12d
006829fc: mov ecx, r15d
006829ff: call 0x18079a530
00682a04: test rax, rax
00682a07: je 0x1806836ca
00682a0d: mov r8, qword ptr [rip + 0x2d10114]
00682a14: mov rdx, rax
00682a17: lea rcx, [rsp + 0x80]
00682a1f: call 0x18169b560
00682a24: movups xmm0, xmmword ptr [rsp + 0x80]
00682a2c: movups xmmword ptr [rsp + 0xa0], xmm0
00682a34: movsd xmm1, qword ptr [rsp + 0x90]
00682a3d: movsd qword ptr [rsp + 0xb0], xmm1
00682a46: xor esi, esi
00682a48: mov qword ptr [rsp + 0x80], rsi
00682a50: lea rbx, [rsp + 0xa0]
00682a58: mov qword ptr [rsp + 0x88], rbx
00682a60: xorps xmm7, xmm7
00682a63: movss xmm6, dword ptr [rip + 0x22f90d1]
00682a6b: nop dword ptr [rax + rax]
00682a70: mov rdx, qword ptr [rip + 0x2d5e7c9]
00682a77: lea rcx, [rsp + 0xa0]
00682a7f: call 0x18142fda0
00682a84: test al, al
00682a86: je 0x1806834ac
00682a8c: mov rdi, qword ptr [rsp + 0xb0]
00682a94: test rdi, rdi
00682a97: je 0x180683720
00682a9d: cmp dword ptr [rdi + 0xf8], r12d
00682aa4: jne 0x180682a70
00682aa6: cmp dword ptr [rdi + 0xf4], r15d
00682aad: jne 0x180682a70
00682aaf: cmp byte ptr [r14 + 0x60], 0
00682ab4: je 0x180682afe
00682ab6: xor r8d, r8d
00682ab9: mov dl, 1
00682abb: mov rcx, rdi
00682abe: call 0x18044c950
00682ac3: mov rsi, rax
00682ac6: mov rcx, qword ptr [rip + 0x2d590cb]
00682acd: cmp dword ptr [rcx + 0xe0], 0
00682ad4: jne 0x180682adb
00682ad6: call 0x180318580
00682adb: xor r8d, r8d
00682ade: xor edx, edx
00682ae0: mov rcx, rsi
00682ae3: call 0x1826b9be0
00682ae8: xor esi, esi
00682aea: test al, al
00682aec: je 0x180682afe
00682aee: mov r13b, 1
00682af1: mov byte ptr [rsp + 0x130], r13b
00682af9: jmp 0x180682a70
00682afe: xor edx, edx
00682b00: mov ecx, 0x103
00682b05: call 0x1807a0520
00682b0a: test al, al
00682b0c: jne 0x180682a70
00682b12: cmp byte ptr [r14 + 0x60], al
00682b16: jne 0x180682a70
00682b1c: mov rax, qword ptr [rdi]
00682b1f: movd xmm1, dword ptr [rdi + 0xe4]
00682b27: cvtdq2ps xmm1, xmm1
00682b2a: mov rdx, qword ptr [rax + 0x2c8]
00682b31: mov rax, qword ptr [rax + 0x2d0]
00682b38: mov qword ptr [rsp + 0x28], rax
00682b3d: mov byte ptr [rsp + 0x20], 0
00682b42: mov r9b, 1
00682b45: xor r8d, r8d
00682b48: mov rcx, rdi
00682b4b: call rdx
00682b4d: movss xmm0, dword ptr [rdi + 0x1d8]
00682b55: comiss xmm0, xmm7
00682b58: jbe 0x180682b72
00682b5a: mov rcx, rdi
00682b5d: comiss xmm0, xmm6
00682b60: jbe 0x180682b67
00682b62: movaps xmm1, xmm6
00682b65: jmp 0x180682b6a
00682b67: movaps xmm1, xmm0
00682b6a: xor r8d, r8d
00682b6d: call 0x180472e10
00682b72: mov rdx, qword ptr [rip + 0x2cf6f87]
00682b79: mov rax, qword ptr [rdx + 0xb8]
00682b80: mov rcx, qword ptr [rax]
00682b83: test rcx, rcx
00682b86: je 0x18068371b
00682b8c: cmp byte ptr [rcx + 0x23a], 0
00682b93: jne 0x180682a70
00682b99: cmp byte ptr [rcx + 0x25e], 0
00682ba0: jne 0x180682a70
00682ba6: mov eax, dword ptr [rdi + 0x194]
00682bac: cmp eax, 0xe4
00682bb1: jle 0x1806830bc
00682bb7: cmp eax, 0x49c
00682bbc: jle 0x180682e31
00682bc2: cmp eax, 0x4a9
00682bc7: jle 0x180682cf1
00682bcd: cmp eax, 0x4d3
00682bd2: jne 0x180682c6d
00682bd8: mov r8, qword ptr [rdi]
00682bdb: mov rax, qword ptr [r8 + 0x348]
00682be2: mov r8, qword ptr [r8 + 0x350]
00682be9: mov edx, 2
00682bee: mov rcx, rdi
00682bf1: call rax
00682bf3: mov rax, qword ptr [rip + 0x2d0533e]
00682bfa: cmp dword ptr [rax + 0xe0], 0
00682c01: jne 0x180682c12
00682c03: mov rcx, rax
00682c06: call 0x180318580
00682c0b: mov rax, qword ptr [rip + 0x2d05326]
00682c12: mov rax, qword ptr [rax + 0xb8]
00682c19: mov rcx, qword ptr [rax]
00682c1c: xor eax, eax
00682c1e: mov qword ptr [rsp + 0x138], rax
00682c26: test rcx, rcx
00682c29: je 0x1806836d0
00682c2f: mov qword ptr [rsp + 0x48], rsi
00682c34: mov qword ptr [rsp + 0x40], rsi
00682c39: mov byte ptr [rsp + 0x38], 1
00682c3e: mov byte ptr [rsp + 0x30], 1
00682c43: movsd xmm0, qword ptr [rsp + 0x138]
00682c4c: movsd qword ptr [rsp + 0x28], xmm0
00682c52: mov qword ptr [rsp + 0x20], rsi
00682c57: mov r9d, 0x431
00682c5d: mov r8d, r12d
00682c60: mov edx, r15d
00682c63: call 0x1809df0b0
00682c68: jmp 0x180682a70
00682c6d: cmp eax, 0x5a5
00682c72: jne 0x180682a70
00682c78: mov rax, qword ptr [rip + 0x2d052b9]
00682c7f: cmp dword ptr [rax + 0xe0], 0
00682c86: jne 0x180682c97
00682c88: mov rcx, rax
00682c8b: call 0x180318580
00682c90: mov rax, qword ptr [rip + 0x2d052a1]
00682c97: mov rax, qword ptr [rax + 0xb8]
00682c9e: mov rcx, qword ptr [rax]
00682ca1: xor eax, eax
00682ca3: mov qword ptr [rsp + 0x138], rax
00682cab: test rcx, rcx
00682cae: je 0x1806836d5
00682cb4: mov qword ptr [rsp + 0x48], rsi
00682cb9: mov qword ptr [rsp + 0x40], rsi
00682cbe: mov byte ptr [rsp + 0x38], 1
00682cc3: mov byte ptr [rsp + 0x30], al
00682cc7: movsd xmm0, qword ptr [rsp + 0x138]
00682cd0: movsd qword ptr [rsp + 0x28], xmm0
00682cd6: mov qword ptr [rsp + 0x20], rsi
00682cdb: mov r9d, 0x5cc
00682ce1: mov r8d, r12d
00682ce4: mov edx, r15d
00682ce7: call 0x1809df0b0
00682cec: jmp 0x180682a70
00682cf1: cmp eax, 0x4a5
00682cf6: jne 0x180682d91
00682cfc: mov r8, qword ptr [rdi]
00682cff: mov rax, qword ptr [r8 + 0x348]
00682d06: mov r8, qword ptr [r8 + 0x350]
00682d0d: mov edx, 2
00682d12: mov rcx, rdi
00682d15: call rax
00682d17: mov rax, qword ptr [rip + 0x2d0521a]
00682d1e: cmp dword ptr [rax + 0xe0], 0
00682d25: jne 0x180682d36
00682d27: mov rcx, rax
00682d2a: call 0x180318580
00682d2f: mov rax, qword ptr [rip + 0x2d05202]
00682d36: mov rax, qword ptr [rax + 0xb8]
00682d3d: mov rcx, qword ptr [rax]
00682d40: xor eax, eax
00682d42: mov qword ptr [rsp + 0x138], rax
00682d4a: test rcx, rcx
00682d4d: je 0x1806836da
00682d53: mov qword ptr [rsp + 0x48], rsi
00682d58: mov qword ptr [rsp + 0x40], rsi
00682d5d: mov byte ptr [rsp + 0x38], 1
00682d62: mov byte ptr [rsp + 0x30], 1
00682d67: movsd xmm0, qword ptr [rsp + 0x138]
00682d70: movsd qword ptr [rsp + 0x28], xmm0
00682d76: mov qword ptr [rsp + 0x20], rsi
00682d7b: mov r9d, 0x40f
00682d81: mov r8d, r12d
00682d84: mov edx, r15d
00682d87: call 0x1809df0b0
00682d8c: jmp 0x180682a70
00682d91: cmp eax, 0x4a9
00682d96: jne 0x180682a70
00682d9c: mov r8, qword ptr [rdi]
00682d9f: mov rax, qword ptr [r8 + 0x348]
00682da6: mov r8, qword ptr [r8 + 0x350]
00682dad: mov edx, 2
00682db2: mov rcx, rdi
00682db5: call rax
00682db7: mov rax, qword ptr [rip + 0x2d0517a]
00682dbe: cmp dword ptr [rax + 0xe0], 0
00682dc5: jne 0x180682dd6
00682dc7: mov rcx, rax
00682dca: call 0x180318580
00682dcf: mov rax, qword ptr [rip + 0x2d05162]
00682dd6: mov rax, qword ptr [rax + 0xb8]
00682ddd: mov rcx, qword ptr [rax]
00682de0: xor eax, eax
00682de2: mov qword ptr [rsp + 0x138], rax
00682dea: test rcx, rcx
00682ded: je 0x1806836df
00682df3: mov qword ptr [rsp + 0x48], rsi
00682df8: mov qword ptr [rsp + 0x40], rsi
00682dfd: mov byte ptr [rsp + 0x38], 1
00682e02: mov byte ptr [rsp + 0x30], 1
00682e07: movsd xmm0, qword ptr [rsp + 0x138]
00682e10: movsd qword ptr [rsp + 0x28], xmm0
00682e16: mov qword ptr [rsp + 0x20], rsi
00682e1b: mov r9d, 0x42f
00682e21: mov r8d, r12d
00682e24: mov edx, r15d
00682e27: call 0x1809df0b0
00682e2c: jmp 0x180682a70
00682e31: cmp eax, 0x422
00682e36: jle 0x180682f7c
00682e3c: cmp eax, 0x426
00682e41: jne 0x180682edc
00682e47: mov r8, qword ptr [rdi]
00682e4a: mov rax, qword ptr [r8 + 0x348]
00682e51: mov r8, qword ptr [r8 + 0x350]
00682e58: mov edx, 2
00682e5d: mov rcx, rdi
00682e60: call rax
00682e62: mov rax, qword ptr [rip + 0x2d050cf]
00682e69: cmp dword ptr [rax + 0xe0], 0
00682e70: jne 0x180682e81
00682e72: mov rcx, rax
00682e75: call 0x180318580
00682e7a: mov rax, qword ptr [rip + 0x2d050b7]
00682e81: mov rax, qword ptr [rax + 0xb8]
00682e88: mov rcx, qword ptr [rax]
00682e8b: xor eax, eax
00682e8d: mov qword ptr [rsp + 0x138], rax
00682e95: test rcx, rcx
00682e98: je 0x1806836e4
00682e9e: mov qword ptr [rsp + 0x48], rsi
00682ea3: mov qword ptr [rsp + 0x40], rsi
00682ea8: mov byte ptr [rsp + 0x38], 1
00682ead: mov byte ptr [rsp + 0x30], 1
00682eb2: movsd xmm0, qword ptr [rsp + 0x138]
00682ebb: movsd qword ptr [rsp + 0x28], xmm0
00682ec1: mov qword ptr [rsp + 0x20], rsi
00682ec6: mov r9d, 0x433
00682ecc: mov r8d, r12d
00682ecf: mov edx, r15d
00682ed2: call 0x1809df0b0
00682ed7: jmp 0x180682a70
00682edc: cmp eax, 0x49c
00682ee1: jne 0x180682a70
00682ee7: mov r8, qword ptr [rdi]
00682eea: mov rax, qword ptr [r8 + 0x348]
00682ef1: mov r8, qword ptr [r8 + 0x350]
00682ef8: mov edx, 2
00682efd: mov rcx, rdi
00682f00: call rax
00682f02: mov rax, qword ptr [rip + 0x2d0502f]
00682f09: cmp dword ptr [rax + 0xe0], 0
00682f10: jne 0x180682f21
00682f12: mov rcx, rax
00682f15: call 0x180318580
00682f1a: mov rax, qword ptr [rip + 0x2d05017]
00682f21: mov rax, qword ptr [rax + 0xb8]
00682f28: mov rcx, qword ptr [rax]
00682f2b: xor eax, eax
00682f2d: mov qword ptr [rsp + 0x138], rax
00682f35: test rcx, rcx
00682f38: je 0x1806836e9
00682f3e: mov qword ptr [rsp + 0x48], rsi
00682f43: mov qword ptr [rsp + 0x40], rsi
00682f48: mov byte ptr [rsp + 0x38], 1
00682f4d: mov byte ptr [rsp + 0x30], 1
00682f52: movsd xmm0, qword ptr [rsp + 0x138]
00682f5b: movsd qword ptr [rsp + 0x28], xmm0
00682f61: mov qword ptr [rsp + 0x20], rsi
00682f66: mov r9d, 0x432
00682f6c: mov r8d, r12d
00682f6f: mov edx, r15d
00682f72: call 0x1809df0b0
00682f77: jmp 0x180682a70
00682f7c: cmp eax, 0x40d
00682f81: jne 0x18068301c
00682f87: mov r8, qword ptr [rdi]
00682f8a: mov rax, qword ptr [r8 + 0x348]
00682f91: mov r8, qword ptr [r8 + 0x350]
00682f98: mov edx, 2
00682f9d: mov rcx, rdi
00682fa0: call rax
00682fa2: mov rax, qword ptr [rip + 0x2d04f8f]
00682fa9: cmp dword ptr [rax + 0xe0], 0
00682fb0: jne 0x180682fc1
00682fb2: mov rcx, rax
00682fb5: call 0x180318580
00682fba: mov rax, qword ptr [rip + 0x2d04f77]
00682fc1: mov rax, qword ptr [rax + 0xb8]
00682fc8: mov rcx, qword ptr [rax]
00682fcb: xor eax, eax
00682fcd: mov qword ptr [rsp + 0x138], rax
00682fd5: test rcx, rcx
00682fd8: je 0x1806836ee
00682fde: mov qword ptr [rsp + 0x48], rsi
00682fe3: mov qword ptr [rsp + 0x40], rsi
00682fe8: mov byte ptr [rsp + 0x38], 1
00682fed: mov byte ptr [rsp + 0x30], 1
00682ff2: movsd xmm0, qword ptr [rsp + 0x138]
00682ffb: movsd qword ptr [rsp + 0x28], xmm0
00683001: mov qword ptr [rsp + 0x20], rsi
00683006: mov r9d, 0x430
0068300c: mov r8d, r12d
0068300f: mov edx, r15d
00683012: call 0x1809df0b0
00683017: jmp 0x180682a70
0068301c: cmp eax, 0x422
00683021: jne 0x180682a70
00683027: mov r8, qword ptr [rdi]
0068302a: mov rax, qword ptr [r8 + 0x348]
00683031: mov r8, qword ptr [r8 + 0x350]
00683038: mov edx, 2
0068303d: mov rcx, rdi
00683040: call rax
00683042: mov rax, qword ptr [rip + 0x2d04eef]
00683049: cmp dword ptr [rax + 0xe0], 0
00683050: jne 0x180683061
00683052: mov rcx, rax
00683055: call 0x180318580
0068305a: mov rax, qword ptr [rip + 0x2d04ed7]
00683061: mov rax, qword ptr [rax + 0xb8]
00683068: mov rcx, qword ptr [rax]
0068306b: xor eax, eax
0068306d: mov qword ptr [rsp + 0x138], rax
00683075: test rcx, rcx
00683078: je 0x1806836f3
0068307e: mov qword ptr [rsp + 0x48], rsi
00683083: mov qword ptr [rsp + 0x40], rsi
00683088: mov byte ptr [rsp + 0x38], 1
0068308d: mov byte ptr [rsp + 0x30], 1
00683092: movsd xmm0, qword ptr [rsp + 0x138]
0068309b: movsd qword ptr [rsp + 0x28], xmm0
006830a1: mov qword ptr [rsp + 0x20], rsi
006830a6: mov r9d, 0xe
006830ac: mov r8d, r12d
006830af: mov edx, r15d
006830b2: call 0x1809df0b0
006830b7: jmp 0x180682a70
006830bc: cmp eax, 0xc
006830bf: jle 0x1806832ee
006830c5: cmp eax, 0x1c
006830c8: jle 0x1806831ce
006830ce: cmp eax, 0x24
006830d1: jne 0x18068314c
006830d3: mov rax, qword ptr [rip + 0x2d04e5e]
006830da: cmp dword ptr [rax + 0xe0], 0
006830e1: jne 0x1806830f2
006830e3: mov rcx, rax
006830e6: call 0x180318580
006830eb: mov rax, qword ptr [rip + 0x2d04e46]
006830f2: mov rax, qword ptr [rax + 0xb8]
006830f9: mov rcx, qword ptr [rax]
006830fc: xor eax, eax
006830fe: mov qword ptr [rsp + 0x138], rax
00683106: test rcx, rcx
00683109: je 0x1806836f8
0068310f: mov qword ptr [rsp + 0x48], rsi
00683114: mov qword ptr [rsp + 0x40], rsi
00683119: mov byte ptr [rsp + 0x38], 1
0068311e: mov byte ptr [rsp + 0x30], al
00683122: movsd xmm0, qword ptr [rsp + 0x138]
0068312b: movsd qword ptr [rsp + 0x28], xmm0
00683131: mov qword ptr [rsp + 0x20], rsi
00683136: mov r9d, 0x4df
0068313c: mov r8d, r12d
0068313f: mov edx, r15d
00683142: call 0x1809df0b0
00683147: jmp 0x180682a70
0068314c: cmp eax, 0xe4
00683151: jne 0x180682a70
00683157: mov dword ptr [rdi + 0x190], 0x19
00683161: mov r8, qword ptr [rdi]
00683164: mov rax, qword ptr [r8 + 0x348]
0068316b: mov r8, qword ptr [r8 + 0x350]
00683172: mov edx, 2
00683177: mov rcx, rdi
0068317a: call rax
0068317c: mov rdx, qword ptr [rdi + 0x98]
00683183: test rdx, rdx
00683186: je 0x1806836fd
0068318c: xor r8d, r8d
0068318f: lea rcx, [rsp + 0x70]
00683194: call 0x1826c8510
00683199: movsd xmm0, qword ptr [rax]
0068319d: mov eax, dword ptr [rax + 8]
006831a0: mov dword ptr [rsp + 0x68], eax
006831a4: movaps xmm1, xmm0
006831a7: shufps xmm1, xmm1, 0x55
006831ab: movsd qword ptr [rsp + 0x60], xmm0
006831b1: xor r9d, r9d
006831b4: xor r8d, r8d
006831b7: mov edx, 0x100
006831bc: unpcklps xmm0, xmm1
006831bf: movq rcx, xmm0
006831c4: call 0x1807a1300
006831c9: jmp 0x180682a70
006831ce: cmp eax, 0x11
006831d1: jne 0x18068326c
006831d7: mov r8, qword ptr [rdi]
006831da: mov rax, qword ptr [r8 + 0x348]
006831e1: mov r8, qword ptr [r8 + 0x350]
006831e8: mov edx, 2
006831ed: mov rcx, rdi
006831f0: call rax
006831f2: mov rax, qword ptr [rip + 0x2d04d3f]
006831f9: cmp dword ptr [rax + 0xe0], 0
00683200: jne 0x180683211
00683202: mov rcx, rax
00683205: call 0x180318580
0068320a: mov rax, qword ptr [rip + 0x2d04d27]
00683211: mov rax, qword ptr [rax + 0xb8]
00683218: mov rcx, qword ptr [rax]
0068321b: xor eax, eax
0068321d: mov qword ptr [rsp + 0x138], rax
00683225: test rcx, rcx
00683228: je 0x180683702
0068322e: mov qword ptr [rsp + 0x48], rsi
00683233: mov qword ptr [rsp + 0x40], rsi
00683238: mov byte ptr [rsp + 0x38], 1
0068323d: mov byte ptr [rsp + 0x30], 1
00683242: movsd xmm0, qword ptr [rsp + 0x138]
0068324b: movsd qword ptr [rsp + 0x28], xmm0
00683251: mov qword ptr [rsp + 0x20], rsi
00683256: mov r9d, 0x424
0068325c: mov r8d, r12d
0068325f: mov edx, r15d
00683262: call 0x1809df0b0
00683267: jmp 0x180682a70
0068326c: cmp eax, 0x1c
0068326f: jne 0x180682a70
00683275: mov rax, qword ptr [rip + 0x2d04cbc]
0068327c: cmp dword ptr [rax + 0xe0], 0
00683283: jne 0x180683294
00683285: mov rcx, rax
00683288: call 0x180318580
0068328d: mov rax, qword ptr [rip + 0x2d04ca4]
00683294: mov rax, qword ptr [rax + 0xb8]
0068329b: mov rcx, qword ptr [rax]
0068329e: xor eax, eax
006832a0: mov qword ptr [rsp + 0x138], rax
006832a8: test rcx, rcx
006832ab: je 0x180683707
006832b1: mov qword ptr [rsp + 0x48], rsi
006832b6: mov qword ptr [rsp + 0x40], rsi
006832bb: mov byte ptr [rsp + 0x38], 1
006832c0: mov byte ptr [rsp + 0x30], al
006832c4: movsd xmm0, qword ptr [rsp + 0x138]
006832cd: movsd qword ptr [rsp + 0x28], xmm0
006832d3: mov qword ptr [rsp + 0x20], rsi
006832d8: mov r9d, 0x460
006832de: mov r8d, r12d
006832e1: mov edx, r15d
006832e4: call 0x1809df0b0
006832e9: jmp 0x180682a70
006832ee: cmp eax, 3
006832f1: jne 0x18068338c
006832f7: mov r8, qword ptr [rdi]
006832fa: mov rax, qword ptr [r8 + 0x348]
00683301: mov r8, qword ptr [r8 + 0x350]
00683308: mov edx, 2
0068330d: mov rcx, rdi
00683310: call rax
00683312: mov rax, qword ptr [rip + 0x2d04c1f]
00683319: cmp dword ptr [rax + 0xe0], 0
00683320: jne 0x180683331
00683322: mov rcx, rax
00683325: call 0x180318580
0068332a: mov rax, qword ptr [rip + 0x2d04c07]
00683331: mov rax, qword ptr [rax + 0xb8]
00683338: mov rcx, qword ptr [rax]
0068333b: xor eax, eax
0068333d: mov qword ptr [rsp + 0x138], rax
00683345: test rcx, rcx
00683348: je 0x18068370c
0068334e: mov qword ptr [rsp + 0x48], rsi
00683353: mov qword ptr [rsp + 0x40], rsi
00683358: mov byte ptr [rsp + 0x38], 1
0068335d: mov byte ptr [rsp + 0x30], 1
00683362: movsd xmm0, qword ptr [rsp + 0x138]
0068336b: movsd qword ptr [rsp + 0x28], xmm0
00683371: mov qword ptr [rsp + 0x20], rsi
00683376: mov r9d, 0x403
0068337c: mov r8d, r12d
0068337f: mov edx, r15d
00683382: call 0x1809df0b0
00683387: jmp 0x180682a70
0068338c: cmp eax, 7
0068338f: jne 0x18068342a
00683395: mov r8, qword ptr [rdi]
00683398: mov rax, qword ptr [r8 + 0x348]
0068339f: mov r8, qword ptr [r8 + 0x350]
006833a6: mov edx, 2
006833ab: mov rcx, rdi
006833ae: call rax
006833b0: mov rax, qword ptr [rip + 0x2d04b81]
006833b7: cmp dword ptr [rax + 0xe0], 0
006833be: jne 0x1806833cf
006833c0: mov rcx, rax
006833c3: call 0x180318580
006833c8: mov rax, qword ptr [rip + 0x2d04b69]
006833cf: mov rax, qword ptr [rax + 0xb8]
006833d6: mov rcx, qword ptr [rax]
006833d9: xor eax, eax
006833db: mov qword ptr [rsp + 0x138], rax
006833e3: test rcx, rcx
006833e6: je 0x180683711
006833ec: mov qword ptr [rsp + 0x48], rsi
006833f1: mov qword ptr [rsp + 0x40], rsi
006833f6: mov byte ptr [rsp + 0x38], 1
006833fb: mov byte ptr [rsp + 0x30], 1
00683400: movsd xmm0, qword ptr [rsp + 0x138]
00683409: movsd qword ptr [rsp + 0x28], xmm0
0068340f: mov qword ptr [rsp + 0x20], rsi
00683414: mov r9d, 0x42e
0068341a: mov r8d, r12d
0068341d: mov edx, r15d
00683420: call 0x1809df0b0
00683425: jmp 0x180682a70
0068342a: cmp eax, 0xc
0068342d: jne 0x180682a70
00683433: mov rax, qword ptr [rip + 0x2d04afe]
0068343a: cmp dword ptr [rax + 0xe0], 0
00683441: jne 0x180683452
00683443: mov rcx, rax
00683446: call 0x180318580
0068344b: mov rax, qword ptr [rip + 0x2d04ae6]
00683452: mov rax, qword ptr [rax + 0xb8]
00683459: mov rcx, qword ptr [rax]
0068345c: xor eax, eax
0068345e: mov qword ptr [rsp + 0x138], rax
00683466: test rcx, rcx
00683469: je 0x180683716
0068346f: mov qword ptr [rsp + 0x48], rsi
00683474: mov qword ptr [rsp + 0x40], rsi
00683479: mov byte ptr [rsp + 0x38], 1
0068347e: mov byte ptr [rsp + 0x30], al
00683482: movsd xmm0, qword ptr [rsp + 0x138]
0068348b: movsd qword ptr [rsp + 0x28], xmm0
00683491: mov qword ptr [rsp + 0x20], rsi
00683496: mov r9d, 0x42b
0068349c: mov r8d, r12d
0068349f: mov edx, r15d
006834a2: call 0x1809df0b0
006834a7: jmp 0x180682a70
006834ac: mov rdx, qword ptr [rip + 0x2d5dcdd]
006834b3: mov rcx, rbx
006834b6: call 0x1803eab80
006834bb: jmp 0x1806834f3
006834bd: mov rdx, qword ptr [rip + 0x2d5dccc]
006834c4: mov rcx, qword ptr [rsp + 0x88]
006834cc: call 0x1803eab80
006834d1: mov rcx, qword ptr [rsp + 0x80]
006834d9: test rcx, rcx
006834dc: jne 0x180683726
006834e2: mov r14, qword ptr [rsp + 0x120]
006834ea: movzx r13d, byte ptr [rsp + 0x130]
006834f3: test r13b, r13b
006834f6: jne 0x18068295b
006834fc: cmp byte ptr [r14 + 0x60], r13b
00683500: je 0x18068295b
00683506: mov rdi, qword ptr [r14 + 0x58]
0068350a: xor edx, edx
0068350c: mov rcx, r14
0068350f: call 0x1826b1710
00683514: test rax, rax
00683517: je 0x1806836ca
0068351d: xor r8d, r8d
00683520: mov rdx, rax
00683523: lea rcx, [rsp + 0x70]
00683528: call 0x1826c8510
0068352d: movsd xmm7, qword ptr [rax]
00683531: mov esi, dword ptr [rax + 8]
00683534: cmp byte ptr [rip + 0x2f1590a], r13b
0068353b: jne 0x180683550
0068353d: lea rcx, [rip + 0x2cef68c]
00683544: call 0x1803633b0
00683549: mov byte ptr [rip + 0x2f158f5], 1
00683550: mov rax, qword ptr [rip + 0x2cef679]
00683557: mov rcx, qword ptr [rax + 0xb8]
0068355e: movups xmm6, xmmword ptr [rcx]
00683561: xor edx, edx
00683563: mov rcx, r14
00683566: call 0x1826b1710
0068356b: test rax, rax
0068356e: je 0x1806836ca
00683574: xor edx, edx
00683576: mov rcx, rax
00683579: call 0x1826c6c40
0068357e: mov rbx, rax
00683581: mov rcx, qword ptr [rip + 0x2d58610]
00683588: cmp dword ptr [rcx + 0xe0], 0
0068358f: jne 0x180683596
00683591: call 0x180318580
00683596: mov rdx, qword ptr [rip + 0x2d1229b]
0068359d: movdqa xmmword ptr [rsp + 0x80], xmm6
006835a6: movsd qword ptr [rsp + 0x60], xmm7
006835ac: mov dword ptr [rsp + 0x68], esi
006835b0: mov qword ptr [rsp + 0x20], rdx
006835b5: mov r9, rbx
006835b8: lea r8, [rsp + 0x80]
006835c0: lea rdx, [rsp + 0x60]
006835c5: mov rcx, rdi
006835c8: call 0x180b99cc0
006835cd: test rax, rax
006835d0: je 0x1806836ca
006835d6: mov ecx, dword ptr [r14 + 0x30]
006835da: mov dword ptr [rax + 0x30], ecx
006835dd: jmp 0x18068295b
006835e2: xor edx, edx
006835e4: mov rcx, r14
006835e7: call 0x1826b16d0
006835ec: mov rbx, rax
006835ef: mov rcx, qword ptr [rip + 0x2d585a2]
006835f6: cmp dword ptr [rcx + 0xe0], 0
006835fd: jne 0x180683604
006835ff: call 0x180318580
00683604: xor edx, edx
00683606: mov rcx, rbx
00683609: call 0x1826b8410
0068360e: mov rbx, qword ptr [r14 + 0x58]
00683612: xor edx, edx
00683614: mov rcx, r14
00683617: call 0x1826b1710
0068361c: test rax, rax
0068361f: je 0x1806836ca
00683625: xor r8d, r8d
00683628: mov rdx, rax
0068362b: lea rcx, [rsp + 0x70]
00683630: call 0x1826c8510
00683635: movsd xmm7, qword ptr [rax]
00683639: mov edi, dword ptr [rax + 8]
0068363c: cmp byte ptr [rip + 0x2f15802], 0
00683643: jne 0x180683658
00683645: lea rcx, [rip + 0x2cef584]
0068364c: call 0x1803633b0
00683651: mov byte ptr [rip + 0x2f157ed], 1
00683658: mov rax, qword ptr [rip + 0x2cef571]
0068365f: mov rcx, qword ptr [rax + 0xb8]
00683666: movups xmm6, xmmword ptr [rcx]
00683669: xor edx, edx
0068366b: mov rcx, r14
0068366e: call 0x1826b1710
00683673: test rax, rax
00683676: je 0x1806836ca
00683678: xor edx, edx
0068367a: mov rcx, rax
0068367d: call 0x1826c6c40
00683682: mov rdx, qword ptr [rip + 0x2d121af]
00683689: movdqa xmmword ptr [rsp + 0x80], xmm6
00683692: movsd qword ptr [rsp + 0x60], xmm7
00683698: mov dword ptr [rsp + 0x68], edi
0068369c: mov qword ptr [rsp + 0x20], rdx
006836a1: mov r9, rax
006836a4: lea r8, [rsp + 0x80]
006836ac: lea rdx, [rsp + 0x60]
006836b1: mov rcx, rbx
006836b4: call 0x180b99cc0
006836b9: test rax, rax
006836bc: je 0x1806836ca
006836be: mov ecx, dword ptr [r14 + 0x30]
006836c2: mov dword ptr [rax + 0x30], ecx
006836c5: jmp 0x180682987
006836ca: call 0x180363610
006836cf: nop
006836d0: call 0x180363610
006836d5: call 0x180363610
006836da: call 0x180363610
006836df: call 0x180363610
006836e4: call 0x180363610
006836e9: call 0x180363610
006836ee: call 0x180363610
006836f3: call 0x180363610
006836f8: call 0x180363610
006836fd: call 0x180363610
00683702: call 0x180363610
00683707: call 0x180363610
0068370c: call 0x180363610
00683711: call 0x180363610
00683716: call 0x180363610
0068371b: call 0x180363610
00683720: call 0x180363610
00683725: nop
00683726: call 0x1803636a0
0068372b: int3
0068372c: int3
0068372d: int3
0068372e: int3
0068372f: int3
