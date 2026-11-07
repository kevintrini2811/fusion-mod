006f1a70: mov qword ptr [rsp + 0x10], rbp
006f1a75: mov qword ptr [rsp + 0x18], rsi
006f1a7a: mov qword ptr [rsp + 0x20], rdi
006f1a7f: push r14
006f1a81: sub rsp, 0x40
006f1a85: cmp byte ptr [rip + 0x47f9d53], 0
006f1a8c: mov rdi, rcx
006f1a8f: jne 0x1806f1ab0
006f1a91: lea rcx, [rip + 0x458db58]
006f1a98: call 0x1804feb70
006f1a9d: lea rcx, [rip + 0x458dc24]
006f1aa4: call 0x1804feb70
006f1aa9: mov byte ptr [rip + 0x47f9d2f], 1
006f1ab0: mov rax, qword ptr [rdi + 0x18]
006f1ab4: xor r14d, r14d
006f1ab7: mov qword ptr [rsp + 0x50], rbx
006f1abc: mov ebp, r14d
006f1abf: movaps xmmword ptr [rsp + 0x30], xmm6
006f1ac4: mov esi, r14d
006f1ac7: mov edx, r14d
006f1aca: test rax, rax
006f1acd: je 0x1806f1bc2
006f1ad3: movss xmm6, dword ptr [rip + 0x35422f5]
006f1adb: nop dword ptr [rax + rax]
006f1ae0: mov rcx, qword ptr [rax + 0x78]
006f1ae4: test rcx, rcx
006f1ae7: je 0x1806f1bc2
006f1aed: cmp edx, dword ptr [rcx + 0x28]
006f1af0: jge 0x1806f1b9f
006f1af6: mov rax, qword ptr [rdi + 0x18]
006f1afa: test rax, rax
006f1afd: je 0x1806f1bc2
006f1b03: mov rcx, qword ptr [rax + 0x78]
006f1b07: test rcx, rcx
006f1b0a: je 0x1806f1bc2
006f1b10: mov r8, qword ptr [rip + 0x458dbb1]
006f1b17: mov edx, esi
006f1b19: call 0x182449330
006f1b1e: mov rbx, rax
006f1b21: test rax, rax
006f1b24: je 0x1806f1bc2
006f1b2a: mov eax, dword ptr [rax + 0x34]
006f1b2d: cmp dword ptr [rdi + 0x34], eax
006f1b30: jne 0x1806f1b8d
006f1b32: movss xmm0, dword ptr [rbx + 0x20]
006f1b37: addss xmm0, xmm6
006f1b3b: comiss xmm0, dword ptr [rdi + 0x20]
006f1b3f: ja 0x1806f1b8d
006f1b41: xor edx, edx
006f1b43: mov rcx, rbx
006f1b46: call 0x1806b3380
006f1b4b: test al, al
006f1b4d: jne 0x1806f1b8d
006f1b4f: xor edx, edx
006f1b51: mov rcx, rbx
006f1b54: call 0x1806b2430
006f1b59: test al, al
006f1b5b: jne 0x1806f1b8d
006f1b5d: test rbp, rbp
006f1b60: je 0x1806f1b6a
006f1b62: mov eax, dword ptr [rbp + 0x74]
006f1b65: cmp dword ptr [rbx + 0x74], eax
006f1b68: jge 0x1806f1b8d
006f1b6a: mov rcx, qword ptr [rdi + 0x18]
006f1b6e: test rcx, rcx
006f1b71: je 0x1806f1bc2
006f1b73: mov r8d, dword ptr [rbx + 0x34]
006f1b77: mov r9d, 3
006f1b7d: mov edx, dword ptr [rbx + 0x74]
006f1b80: mov qword ptr [rsp + 0x20], r14
006f1b85: call 0x18065f2a0
006f1b8a: mov rbp, rax
006f1b8d: mov rax, qword ptr [rdi + 0x18]
006f1b91: inc esi
006f1b93: mov edx, esi
006f1b95: test rax, rax
006f1b98: je 0x1806f1bc2
006f1b9a: jmp 0x1806f1ae0
006f1b9f: movaps xmm6, xmmword ptr [rsp + 0x30]
006f1ba4: mov rax, rbp
006f1ba7: mov rbp, qword ptr [rsp + 0x58]
006f1bac: mov rbx, qword ptr [rsp + 0x50]
006f1bb1: mov rsi, qword ptr [rsp + 0x60]
006f1bb6: mov rdi, qword ptr [rsp + 0x68]
006f1bbb: add rsp, 0x40
006f1bbf: pop r14
006f1bc1: ret
006f1bc2: call 0x1804fee20
006f1bc7: int3
006f1bc8: int3
006f1bc9: int3
006f1bca: int3
006f1bcb: int3
006f1bcc: int3
006f1bcd: int3
006f1bce: int3
006f1bcf: int3
