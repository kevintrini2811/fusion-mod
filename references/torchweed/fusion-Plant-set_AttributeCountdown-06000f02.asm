00472e10: movss xmm0, dword ptr [rcx + 0x1dc]
00472e18: xorps xmm2, xmm2
00472e1b: comiss xmm0, xmm2
00472e1e: jb 0x180472e3a
00472e20: comiss xmm2, dword ptr [rcx + 0x1d8]
00472e27: jb 0x180472e3a
00472e29: comiss xmm1, xmm2
00472e2c: jbe 0x180472e3a
00472e2e: movss xmm1, dword ptr [rip + 0x250910a]
00472e36: maxss xmm1, xmm0
00472e3a: xor r8d, r8d
00472e3d: jmp 0x18046e290
00472e42: int3
00472e43: int3
00472e44: int3
00472e45: int3
00472e46: int3
00472e47: int3
00472e48: int3
00472e49: int3
00472e4a: int3
00472e4b: int3
00472e4c: int3
00472e4d: int3
00472e4e: int3
00472e4f: int3
