0049d280: mov r8d, 4
0049d286: mov eax, r8d
0049d289: sub r8d, edx
0049d28c: sub eax, edx
0049d28e: lea eax, [rax + rax*4]
0049d291: add eax, eax
0049d293: movd xmm0, eax
0049d297: lea eax, [r8 + r8*4]
0049d29b: add eax, eax
0049d29d: cvtdq2ps xmm0, xmm0
0049d2a0: movd xmm1, eax
0049d2a4: cvtdq2ps xmm1, xmm1
0049d2a7: movss dword ptr [rcx + 0x248], xmm0
0049d2af: movss xmm0, dword ptr [rip + 0x24de8c5]
0049d2b7: comiss xmm0, xmm1
0049d2ba: jbe 0x18049d2c6
0049d2bc: mov dword ptr [rcx + 0x248], 0x40a00000
0049d2c6: ret
0049d2c7: int3
0049d2c8: int3
0049d2c9: int3
0049d2ca: int3
0049d2cb: int3
0049d2cc: int3
0049d2cd: int3
0049d2ce: int3
0049d2cf: int3
