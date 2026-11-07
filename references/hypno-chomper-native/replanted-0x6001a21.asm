006b1aa0: mov qword ptr [rsp + 8], rbx
006b1aa5: mov qword ptr [rsp + 0x10], rsi
006b1aaa: push rdi
006b1aab: sub rsp, 0x20
006b1aaf: mov edi, edx
006b1ab1: mov rbx, rcx
006b1ab4: mov ecx, edi
006b1ab6: xor edx, edx
006b1ab8: mov esi, r8d
006b1abb: call 0x1806859a0
006b1ac0: test al, al
006b1ac2: je 0x1806b1aed
006b1ac4: test rbx, rbx
006b1ac7: je 0x1806b1b9d
006b1acd: xor edx, edx
006b1acf: mov rcx, rbx
006b1ad2: call 0x1807df9f0
006b1ad7: test al, al
006b1ad9: jne 0x1806b1aed
006b1adb: xor eax, eax
006b1add: mov rbx, qword ptr [rsp + 0x30]
006b1ae2: mov rsi, qword ptr [rsp + 0x38]
006b1ae7: add rsp, 0x20
006b1aeb: pop rdi
006b1aec: ret
006b1aed: cmp edi, 0x30
006b1af0: jne 0x1806b1b91
006b1af6: cmp esi, -1
006b1af9: je 0x1806b1b91
006b1aff: test rbx, rbx
006b1b02: je 0x1806b1b9d
006b1b08: mov edx, esi
006b1b0a: xor r8d, r8d
006b1b0d: mov rcx, rbx
006b1b10: call 0x1807dd8a0
006b1b15: xor edx, edx
006b1b17: mov rcx, rbx
006b1b1a: mov rdi, rax
006b1b1d: call 0x1807df9f0
006b1b22: test al, al
006b1b24: jne 0x1806b1b4f
006b1b26: test rdi, rdi
006b1b29: je 0x1806b1b9d
006b1b2b: movd xmm0, dword ptr [rdi + 0x50]
006b1b30: cvtdq2ps xmm0, xmm0
006b1b33: mulss xmm0, dword ptr [rbx + 0xdc]
006b1b3b: cvttss2si eax, xmm0
006b1b3f: mov rbx, qword ptr [rsp + 0x30]
006b1b44: mov rsi, qword ptr [rsp + 0x38]
006b1b49: add rsp, 0x20
006b1b4d: pop rdi
006b1b4e: ret
006b1b4f: mov rax, qword ptr [rbx + 0xd0]
006b1b56: test rax, rax
006b1b59: je 0x1806b1b79
006b1b5b: cmp dword ptr [rax + 0x44], 4
006b1b5f: jne 0x1806b1b79
006b1b61: test rdi, rdi
006b1b64: je 0x1806b1b9d
006b1b66: mov eax, dword ptr [rdi + 0x58]
006b1b69: mov rbx, qword ptr [rsp + 0x30]
006b1b6e: mov rsi, qword ptr [rsp + 0x38]
006b1b73: add rsp, 0x20
006b1b77: pop rdi
006b1b78: ret
006b1b79: test rdi, rdi
006b1b7c: je 0x1806b1b9d
006b1b7e: mov eax, dword ptr [rdi + 0x54]
006b1b81: mov rbx, qword ptr [rsp + 0x30]
006b1b86: mov rsi, qword ptr [rsp + 0x38]
006b1b8b: add rsp, 0x20
006b1b8f: pop rdi
006b1b90: ret
006b1b91: test rbx, rbx
006b1b94: je 0x1806b1b9d
006b1b96: mov edx, edi
006b1b98: jmp 0x1806b1b0a
006b1b9d: call 0x1804fee20
006b1ba2: int3
006b1ba3: int3
006b1ba4: int3
006b1ba5: int3
006b1ba6: int3
006b1ba7: int3
006b1ba8: int3
006b1ba9: int3
006b1baa: int3
006b1bab: int3
006b1bac: int3
006b1bad: int3
006b1bae: int3
006b1baf: int3
