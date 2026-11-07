00548d10: push rbx
00548d12: sub rsp, 0x30
00548d16: cmp byte ptr [rip + 0x3050a10], 0
00548d1d: mov rbx, rcx
00548d20: jne 0x180548d41
00548d22: lea rcx, [rip + 0x2e7c9d7]
00548d29: call 0x1803633b0
00548d2e: lea rcx, [rip + 0x2e7c86b]
00548d35: call 0x1803633b0
00548d3a: mov byte ptr [rip + 0x30509ec], 1
00548d41: mov rdx, qword ptr [rbx]
00548d44: mov rcx, rbx
00548d47: mov rax, qword ptr [rdx + 0x408]
00548d4e: mov rdx, qword ptr [rdx + 0x410]
00548d55: call rax
00548d57: xorps xmm0, xmm0
00548d5a: comiss xmm0, dword ptr [rbx + 0x1d0]
00548d61: jbe 0x180548de8
00548d67: movss xmm1, dword ptr [rip + 0x2432f09]
00548d6f: xor r8d, r8d
00548d72: movss xmm0, dword ptr [rip + 0x24330c6]
00548d7a: movaps xmmword ptr [rsp + 0x20], xmm6
00548d7f: movss xmm6, dword ptr [rbx + 0x13c]
00548d87: call 0x1826bcce0
00548d8c: mov rdx, qword ptr [rbx]
00548d8f: mov rcx, rbx
00548d92: mulss xmm0, xmm6
00548d96: movss dword ptr [rbx + 0x1d0], xmm0
00548d9e: mov rax, qword ptr [rdx + 0x218]
00548da5: mov rdx, qword ptr [rdx + 0x220]
00548dac: call rax
00548dae: movaps xmm6, xmmword ptr [rsp + 0x20]
00548db3: test al, al
00548db5: je 0x180548de8
00548db7: mov edx, dword ptr [rbx + 0x128]
00548dbd: xor r8d, r8d
00548dc0: xor ecx, ecx
00548dc2: call 0x1826bcca0
00548dc7: mov rcx, qword ptr [rbx + 0x28]
00548dcb: test rcx, rcx
00548dce: je 0x180548dff
00548dd0: xor r8d, r8d
00548dd3: test eax, eax
00548dd5: je 0x180548dee
00548dd7: mov rdx, qword ptr [rip + 0x2e7c7c2]
00548dde: add rsp, 0x30
00548de2: pop rbx
00548de3: jmp 0x182678fd0
00548de8: add rsp, 0x30
00548dec: pop rbx
00548ded: ret
00548dee: mov rdx, qword ptr [rip + 0x2e7c90b]
00548df5: add rsp, 0x30
00548df9: pop rbx
00548dfa: jmp 0x182678fd0
00548dff: call 0x180363610
00548e04: int3
00548e05: int3
00548e06: int3
00548e07: int3
00548e08: int3
00548e09: int3
00548e0a: int3
00548e0b: int3
00548e0c: int3
00548e0d: int3
00548e0e: int3
00548e0f: int3
