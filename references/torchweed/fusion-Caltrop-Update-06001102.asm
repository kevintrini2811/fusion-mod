004979d0: push rbx
004979d2: sub rsp, 0x30
004979d6: xor edx, edx
004979d8: movaps xmmword ptr [rsp + 0x20], xmm6
004979dd: mov rbx, rcx
004979e0: call 0x180470150
004979e5: movss xmm0, dword ptr [rbx + 0x1d0]
004979ed: xorps xmm6, xmm6
004979f0: comiss xmm0, xmm6
004979f3: jbe 0x180497a56
004979f5: mov rdx, qword ptr [rbx]
004979f8: mov rcx, rbx
004979fb: mov rax, qword ptr [rdx + 0x408]
00497a02: mov rdx, qword ptr [rdx + 0x410]
00497a09: call rax
00497a0b: comiss xmm6, dword ptr [rbx + 0x1d0]
00497a12: jb 0x180497a56
00497a14: mov rdx, qword ptr [rbx]
00497a17: mov rcx, rbx
00497a1a: mov rax, qword ptr [rdx + 0x5b8]
00497a21: mov rdx, qword ptr [rdx + 0x5c0]
00497a28: call rax
00497a2a: movss xmm1, dword ptr [rip + 0x24e40f2]
00497a32: xor r8d, r8d
00497a35: movss xmm0, dword ptr [rip + 0x24e4447]
00497a3d: movss xmm6, dword ptr [rbx + 0x13c]
00497a45: call 0x1826bcce0
00497a4a: addss xmm0, xmm6
00497a4e: movss dword ptr [rbx + 0x1d0], xmm0
00497a56: movaps xmm6, xmmword ptr [rsp + 0x20]
00497a5b: add rsp, 0x30
00497a5f: pop rbx
00497a60: ret
00497a61: int3
00497a62: int3
00497a63: int3
00497a64: int3
00497a65: int3
00497a66: int3
00497a67: int3
00497a68: int3
00497a69: int3
00497a6a: int3
00497a6b: int3
00497a6c: int3
00497a6d: int3
00497a6e: int3
00497a6f: int3
