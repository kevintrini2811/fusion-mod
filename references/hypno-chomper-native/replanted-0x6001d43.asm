0070a110: push rbx
0070a112: sub rsp, 0x50
0070a116: cmp dword ptr [rcx + 0x104], 0
0070a11d: mov rbx, rcx
0070a120: movaps xmmword ptr [rsp + 0x40], xmm6
0070a125: movss xmm6, dword ptr [rcx + 0x88]
0070a12d: movaps xmmword ptr [rsp + 0x30], xmm7
0070a132: movaps xmm7, xmm1
0070a135: jle 0x18070a13f
0070a137: mulss xmm6, dword ptr [rip + 0x3529afd]
0070a13f: xor edx, edx
0070a141: call 0x1806f3a20
0070a146: test al, al
0070a148: je 0x18070a151
0070a14a: xorps xmm6, xmmword ptr [rip + 0x3529b4f]
0070a151: xor edx, edx
0070a153: mov rcx, rbx
0070a156: call 0x180709ec0
0070a15b: test al, al
0070a15d: je 0x18070a162
0070a15f: xorps xmm6, xmm6
0070a162: xor r8d, r8d
0070a165: lea rcx, [rsp + 0x20]
0070a16a: mov rdx, rbx
0070a16d: call 0x1806f2d70
0070a172: movups xmm1, xmmword ptr [rax]
0070a175: mulss xmm6, xmm7
0070a179: movaps xmm0, xmm1
0070a17c: movaps xmm7, xmmword ptr [rsp + 0x30]
0070a181: shufps xmm0, xmm1, 0xaa
0070a185: mulss xmm0, dword ptr [rip + 0x3529ab3]
0070a18d: addss xmm0, xmm1
0070a191: subss xmm0, xmm6
0070a195: movaps xmm6, xmmword ptr [rsp + 0x40]
0070a19a: add rsp, 0x50
0070a19e: pop rbx
0070a19f: ret
