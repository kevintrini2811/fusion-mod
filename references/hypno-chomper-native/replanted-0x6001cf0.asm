006f2d70: mov qword ptr [rsp + 8], rbx
006f2d75: push rdi
006f2d76: sub rsp, 0x60
006f2d7a: mov rdi, rdx
006f2d7d: xorps xmm0, xmm0
006f2d80: movups xmmword ptr [rcx], xmm0
006f2d83: mov rbx, rcx
006f2d86: movups xmm1, xmmword ptr [rdx + 0xe4]
006f2d8d: xor edx, edx
006f2d8f: movups xmmword ptr [rcx], xmm1
006f2d92: mov rcx, rdi
006f2d95: call 0x1806f3a20
006f2d9a: test al, al
006f2d9c: je 0x1806f2db3
006f2d9e: movd xmm0, dword ptr [rdi + 0x28]
006f2da3: cvtdq2ps xmm0, xmm0
006f2da6: subss xmm0, dword ptr [rbx]
006f2daa: subss xmm0, dword ptr [rbx + 8]
006f2daf: movss dword ptr [rbx], xmm0
006f2db3: xor r8d, r8d
006f2db6: lea rcx, [rsp + 0x40]
006f2dbb: mov rdx, rdi
006f2dbe: call 0x1806f2170
006f2dc3: mov edx, dword ptr [rax + 0x18]
006f2dc6: cvttss2si ecx, dword ptr [rdi + 0x24]
006f2dcb: cvttss2si eax, dword ptr [rax + 0xc]
006f2dd0: add ecx, eax
006f2dd2: movd xmm0, ecx
006f2dd6: cvtdq2ps xmm0, xmm0
006f2dd9: addss xmm0, dword ptr [rbx + 4]
006f2dde: movss dword ptr [rbx + 4], xmm0
006f2de3: movss xmm1, dword ptr [rdi + 0x20]
006f2de8: addss xmm1, dword ptr [rbx]
006f2dec: movd xmm0, edx
006f2df0: comiss xmm0, dword ptr [rip + 0x35410b5]
006f2df7: movss dword ptr [rbx], xmm1
006f2dfb: jbe 0x1806f2e16
006f2dfd: movss xmm1, dword ptr [rbx + 0xc]
006f2e02: cvttss2si eax, xmm0
006f2e06: movd xmm0, eax
006f2e0a: cvtdq2ps xmm0, xmm0
006f2e0d: subss xmm1, xmm0
006f2e11: movss dword ptr [rbx + 0xc], xmm1
006f2e16: mov rax, rbx
006f2e19: mov rbx, qword ptr [rsp + 0x70]
006f2e1e: add rsp, 0x60
006f2e22: pop rdi
006f2e23: ret
006f2e24: int3
006f2e25: int3
006f2e26: int3
006f2e27: int3
006f2e28: int3
006f2e29: int3
006f2e2a: int3
006f2e2b: int3
006f2e2c: int3
006f2e2d: int3
006f2e2e: int3
006f2e2f: int3
