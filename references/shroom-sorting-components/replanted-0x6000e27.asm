007e9150: lea eax, [rcx - 0x186a0]
007e9156: movd xmm0, eax
007e915a: cvtdq2ps xmm0, xmm0
007e915d: mulss xmm0, dword ptr [rip + 0x344b41f]
007e9165: divss xmm0, dword ptr [rip + 0x344b41b]
007e916d: cvttss2si eax, xmm0
007e9171: sub eax, 0x7ffb
007e9176: ret 
007e9177: int3 
007e9178: int3 
007e9179: int3 
007e917a: int3 
007e917b: int3 
007e917c: int3 
007e917d: int3 
007e917e: int3 
007e917f: int3 
