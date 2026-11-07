0049cf90: push rbx
0049cf92: sub rsp, 0x20
0049cf96: cmp byte ptr [rip + 0x30fc27e], 0
0049cf9d: mov rbx, rcx
0049cfa0: jne 0x18049cfb5
0049cfa2: lea rcx, [rip + 0x2f06bd7]
0049cfa9: call 0x1803633b0
0049cfae: mov byte ptr [rip + 0x30fc266], 1
0049cfb5: xor edx, edx
0049cfb7: mov rcx, rbx
0049cfba: call 0x180468b30
0049cfbf: mov rax, qword ptr [rip + 0x2f06bba]
0049cfc6: cmp dword ptr [rax + 0xe0], 0
0049cfcd: jne 0x18049cfde
0049cfcf: mov rcx, rax
0049cfd2: call 0x180318580
0049cfd7: mov rax, qword ptr [rip + 0x2f06ba2]
0049cfde: mov rax, qword ptr [rax + 0xb8]
0049cfe5: cmp dword ptr [rax + 0xa8], 0
0049cfec: jne 0x18049d02f
0049cfee: mov rcx, qword ptr [rbx + 0x28]
0049cff2: test rcx, rcx
0049cff5: je 0x18049d035
0049cff7: xor edx, edx
0049cff9: call 0x182679210
0049cffe: xorps xmm1, xmm1
0049d001: ucomiss xmm0, xmm1
0049d004: jp 0x18049d008
0049d006: je 0x18049d02f
0049d008: cmp dword ptr [rbx + 0x190], 0
0049d00f: jne 0x18049d02f
0049d011: mov r8, qword ptr [rbx]
0049d014: xor edx, edx
0049d016: mov rcx, rbx
0049d019: mov rax, qword ptr [r8 + 0x5d8]
0049d020: mov r8, qword ptr [r8 + 0x5e0]
0049d027: add rsp, 0x20
0049d02b: pop rbx
0049d02c: jmp rax
0049d02f: add rsp, 0x20
0049d033: pop rbx
0049d034: ret
0049d035: call 0x180363610
0049d03a: int3
0049d03b: int3
0049d03c: int3
0049d03d: int3
0049d03e: int3
0049d03f: int3
