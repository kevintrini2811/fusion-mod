0049d040: push rbx
0049d042: sub rsp, 0x20
0049d046: cmp byte ptr [rip + 0x30fc1cb], 0
0049d04d: mov rbx, rcx
0049d050: jne 0x18049d065
0049d052: lea rcx, [rip + 0x2f46fb7]
0049d059: call 0x1803633b0
0049d05e: mov byte ptr [rip + 0x30fc1b3], 1
0049d065: xor edx, edx
0049d067: mov rcx, rbx
0049d06a: call 0x18046e840
0049d06f: mov rax, qword ptr [rip + 0x2f46f9a]
0049d076: cmp dword ptr [rax + 0xe0], 0
0049d07d: jne 0x18049d08e
0049d07f: mov rcx, rax
0049d082: call 0x180318580
0049d087: mov rax, qword ptr [rip + 0x2f46f82]
0049d08e: mov rax, qword ptr [rax + 0xb8]
0049d095: mov rcx, qword ptr [rax]
0049d098: test rcx, rcx
0049d09b: je 0x18049d1a5
0049d0a1: xor r9d, r9d
0049d0a4: mov edx, 6
0049d0a9: mov r8d, 1
0049d0af: call 0x180760f60
0049d0b4: test al, al
0049d0b6: je 0x18049d0d0
0049d0b8: movss xmm0, dword ptr [rbx + 0x154]
0049d0c0: addss xmm0, dword ptr [rip + 0x24dea74]
0049d0c8: movss dword ptr [rbx + 0x154], xmm0
0049d0d0: mov rax, qword ptr [rip + 0x2f46f39]
0049d0d7: cmp dword ptr [rax + 0xe0], 0
0049d0de: jne 0x18049d0ef
0049d0e0: mov rcx, rax
0049d0e3: call 0x180318580
0049d0e8: mov rax, qword ptr [rip + 0x2f46f21]
0049d0ef: mov rax, qword ptr [rax + 0xb8]
0049d0f6: mov rcx, qword ptr [rax]
0049d0f9: test rcx, rcx
0049d0fc: je 0x18049d1a5
0049d102: xor r9d, r9d
0049d105: mov edx, 6
0049d10a: mov r8d, 2
0049d110: call 0x180760f60
0049d115: test al, al
0049d117: je 0x18049d150
0049d119: mov rdx, qword ptr [rbx]
0049d11c: mov rcx, rbx
0049d11f: mov rax, qword ptr [rdx + 0x5a8]
0049d126: mov rdx, qword ptr [rdx + 0x5b0]
0049d12d: call rax
0049d12f: mov r8, qword ptr [rbx]
0049d132: mov rcx, rbx
0049d135: addss xmm0, dword ptr [rip + 0x24dea33]
0049d13d: mov rax, qword ptr [r8 + 0x5b8]
0049d144: mov r8, qword ptr [r8 + 0x5c0]
0049d14b: movaps xmm1, xmm0
0049d14e: call rax
0049d150: mov rax, qword ptr [rbx + 0x30]
0049d154: test rax, rax
0049d157: je 0x18049d1a5
0049d159: cmp byte ptr [rax + 0x23b], 0
0049d160: je 0x18049d19f
0049d162: mov rdx, qword ptr [rbx]
0049d165: mov rcx, rbx
0049d168: mov rax, qword ptr [rdx + 0x5a8]
0049d16f: mov rdx, qword ptr [rdx + 0x5b0]
0049d176: call rax
0049d178: mov r8, qword ptr [rbx]
0049d17b: mov rcx, rbx
0049d17e: addss xmm0, dword ptr [rip + 0x24de9ea]
0049d186: mov rax, qword ptr [r8 + 0x5b8]
0049d18d: mov r8, qword ptr [r8 + 0x5c0]
0049d194: movaps xmm1, xmm0
0049d197: add rsp, 0x20
0049d19b: pop rbx
0049d19c: jmp rax
0049d19f: add rsp, 0x20
0049d1a3: pop rbx
0049d1a4: ret
0049d1a5: call 0x180363610
0049d1aa: int3
0049d1ab: int3
0049d1ac: int3
0049d1ad: int3
0049d1ae: int3
0049d1af: int3
