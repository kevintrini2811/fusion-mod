00496c70: push rbx
00496c72: sub rsp, 0x30
00496c76: movss xmm0, dword ptr [rcx + 0x1d0]
00496c7e: mov rbx, rcx
00496c81: movaps xmmword ptr [rsp + 0x20], xmm6
00496c86: xorps xmm6, xmm6
00496c89: comiss xmm0, xmm6
00496c8c: jbe 0x180496cc0
00496c8e: mov rdx, qword ptr [rcx]
00496c91: mov rax, qword ptr [rdx + 0x408]
00496c98: mov rdx, qword ptr [rdx + 0x410]
00496c9f: call rax
00496ca1: comiss xmm6, dword ptr [rbx + 0x1d0]
00496ca8: jb 0x180496cc0
00496caa: xor edx, edx
00496cac: mov rcx, rbx
00496caf: call 0x1804970c0
00496cb4: mov eax, dword ptr [rbx + 0x13c]
00496cba: mov dword ptr [rbx + 0x1d0], eax
00496cc0: movaps xmm6, xmmword ptr [rsp + 0x20]
00496cc5: add rsp, 0x30
00496cc9: pop rbx
00496cca: ret
00496ccb: int3
00496ccc: int3
00496ccd: int3
00496cce: int3
00496ccf: int3
