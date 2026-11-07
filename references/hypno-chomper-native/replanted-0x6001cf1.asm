006f2c90: mov qword ptr [rsp + 8], rbx
006f2c95: push rdi
006f2c96: sub rsp, 0x60
006f2c9a: xorps xmm0, xmm0
006f2c9d: mov rdi, rdx
006f2ca0: movups xmmword ptr [rcx], xmm0
006f2ca3: mov rbx, rcx
006f2ca6: movups xmm1, xmmword ptr [rdx + 0xf4]
006f2cad: movups xmmword ptr [rcx], xmm1
006f2cb0: cmp dword ptr [rdx + 0x74], 0xc
006f2cb4: je 0x1806f2cbc
006f2cb6: cmp dword ptr [rdx + 0x74], 0x36
006f2cba: jne 0x1806f2cd7
006f2cbc: mov dword ptr [rcx], 0xc2200000
006f2cc2: mov dword ptr [rcx + 4], 0
006f2cc9: mov dword ptr [rcx + 8], 0x42c80000
006f2cd0: mov dword ptr [rcx + 0xc], 0x42e60000
006f2cd7: xor edx, edx
006f2cd9: mov rcx, rdi
006f2cdc: call 0x1806f3a20
006f2ce1: test al, al
006f2ce3: je 0x1806f2cfa
006f2ce5: movd xmm0, dword ptr [rdi + 0x28]
006f2cea: cvtdq2ps xmm0, xmm0
006f2ced: subss xmm0, dword ptr [rbx]
006f2cf1: subss xmm0, dword ptr [rbx + 8]
006f2cf6: movss dword ptr [rbx], xmm0
006f2cfa: xor r8d, r8d
006f2cfd: lea rcx, [rsp + 0x40]
006f2d02: mov rdx, rdi
006f2d05: call 0x1806f2170
006f2d0a: mov ecx, dword ptr [rax + 0x18]
006f2d0d: cvttss2si eax, dword ptr [rax + 0xc]
006f2d12: movd xmm2, eax
006f2d16: cvtdq2ps xmm2, xmm2
006f2d19: addss xmm2, dword ptr [rdi + 0x24]
006f2d1e: addss xmm2, dword ptr [rbx + 4]
006f2d23: movss xmm0, dword ptr [rdi + 0x20]
006f2d28: addss xmm0, dword ptr [rbx]
006f2d2c: movd xmm1, ecx
006f2d30: comiss xmm1, dword ptr [rip + 0x3541175]
006f2d37: movss dword ptr [rbx], xmm0
006f2d3b: movss dword ptr [rbx + 4], xmm2
006f2d40: jbe 0x1806f2d5b
006f2d42: cvttss2si eax, xmm1
006f2d46: movss xmm1, dword ptr [rbx + 0xc]
006f2d4b: movd xmm0, eax
006f2d4f: cvtdq2ps xmm0, xmm0
006f2d52: subss xmm1, xmm0
006f2d56: movss dword ptr [rbx + 0xc], xmm1
006f2d5b: mov rax, rbx
006f2d5e: mov rbx, qword ptr [rsp + 0x70]
006f2d63: add rsp, 0x60
006f2d67: pop rdi
006f2d68: ret
006f2d69: int3
006f2d6a: int3
006f2d6b: int3
006f2d6c: int3
006f2d6d: int3
006f2d6e: int3
006f2d6f: int3
