006b7ac0: push rdi
006b7ac2: sub rsp, 0x30
006b7ac6: cmp byte ptr [rip + 0x4833b8a], 0
006b7acd: mov rdi, rcx
006b7ad0: jne 0x1806b7af1
006b7ad2: lea rcx, [rip + 0x457599f]
006b7ad9: call 0x1804feb70
006b7ade: lea rcx, [rip + 0x45e0b8b]
006b7ae5: call 0x1804feb70
006b7aea: mov byte ptr [rip + 0x4833b66], 1
006b7af1: mov rcx, qword ptr [rdi + 0x140]
006b7af8: mov qword ptr [rsp + 0x40], rbx
006b7afd: test rcx, rcx
006b7b00: je 0x1806b7c8d
006b7b06: xor r8d, r8d
006b7b09: mov edx, 1
006b7b0e: call 0x1807c7a30
006b7b13: test eax, eax
006b7b15: jle 0x1806b7b2e
006b7b17: mov rcx, qword ptr [rdi + 0x140]
006b7b1e: test rcx, rcx
006b7b21: je 0x1806b7c8d
006b7b27: xor edx, edx
006b7b29: call 0x1807c7650
006b7b2e: cmp byte ptr [rdi + 0x136], 0
006b7b35: jne 0x1806b7c34
006b7b3b: cmp dword ptr [rdi + 0x70], 1
006b7b3f: jne 0x1806b7c67
006b7b45: mov rcx, qword ptr [rip + 0x457592c]
006b7b4c: mov rbx, qword ptr [rdi + 0x10]
006b7b50: cmp dword ptr [rcx + 0xe4], 0
006b7b57: jne 0x1806b7b5e
006b7b59: call 0x1804feeb0
006b7b5e: xor r8d, r8d
006b7b61: xor edx, edx
006b7b63: mov rcx, rbx
006b7b66: call 0x183832900
006b7b6b: test al, al
006b7b6d: je 0x1806b7c3f
006b7b73: mov rax, qword ptr [rdi + 0x10]
006b7b77: test rax, rax
006b7b7a: je 0x1806b7c8d
006b7b80: mov rcx, qword ptr [rax + 0xc8]
006b7b87: test rcx, rcx
006b7b8a: je 0x1806b7c3f
006b7b90: cmp byte ptr [rcx + 0x82], 0
006b7b97: je 0x1806b7c3f
006b7b9d: mov ebx, dword ptr [rdi + 0xf8]
006b7ba3: xor ecx, ecx
006b7ba5: call 0x18063cd70
006b7baa: cmp ebx, eax
006b7bac: jne 0x1806b7c34
006b7bb2: mov rcx, qword ptr [rdi + 0x140]
006b7bb9: test rcx, rcx
006b7bbc: je 0x1806b7c8d
006b7bc2: mov rdx, qword ptr [rip + 0x45e0aa7]
006b7bc9: xor r8d, r8d
006b7bcc: call 0x1807c7e60
006b7bd1: test al, al
006b7bd3: je 0x1806b7c34
006b7bd5: mov rcx, qword ptr [rdi + 0x10]
006b7bd9: test rcx, rcx
006b7bdc: je 0x1806b7c8d
006b7be2: movss xmm2, dword ptr [rdi + 0x24]
006b7be7: movss xmm1, dword ptr [rdi + 0x20]
006b7bec: mov r9d, dword ptr [rdi + 0x38]
006b7bf0: addss xmm2, dword ptr [rip + 0x357c3c0]
006b7bf8: addss xmm1, dword ptr [rip + 0x357c2f8]
006b7c00: add r9d, 2
006b7c04: mov qword ptr [rsp + 0x28], 0
006b7c0d: mov dword ptr [rsp + 0x20], 0x44
006b7c15: call 0x1807d96d0
006b7c1a: mov rcx, qword ptr [rdi + 0x10]
006b7c1e: test rcx, rcx
006b7c21: je 0x1806b7c8d
006b7c23: xor r8d, r8d
006b7c26: mov rdx, rax
006b7c29: call 0x1807e13a0
006b7c2e: mov dword ptr [rdi + 0xf8], eax
006b7c34: mov rbx, qword ptr [rsp + 0x40]
006b7c39: add rsp, 0x30
006b7c3d: pop rdi
006b7c3e: ret
006b7c3f: mov ebx, dword ptr [rdi + 0xf8]
006b7c45: xor ecx, ecx
006b7c47: call 0x18063cd70
006b7c4c: cmp ebx, eax
006b7c4e: je 0x1806b7c67
006b7c50: xor edx, edx
006b7c52: mov rcx, rdi
006b7c55: call 0x1806b4e30
006b7c5a: xor ecx, ecx
006b7c5c: call 0x18063cd70
006b7c61: mov dword ptr [rdi + 0xf8], eax
006b7c67: cmp dword ptr [rdi + 0xfc], 0
006b7c6e: jle 0x1806b7c34
006b7c70: sub dword ptr [rdi + 0xfc], 1
006b7c77: jne 0x1806b7c34
006b7c79: xor edx, edx
006b7c7b: mov rcx, rdi
006b7c7e: mov rbx, qword ptr [rsp + 0x40]
006b7c83: add rsp, 0x30
006b7c87: pop rdi
006b7c88: jmp 0x1806ad260
006b7c8d: call 0x1804fee20
006b7c92: int3
006b7c93: int3
006b7c94: int3
006b7c95: int3
006b7c96: int3
006b7c97: int3
006b7c98: int3
006b7c99: int3
006b7c9a: int3
006b7c9b: int3
006b7c9c: int3
006b7c9d: int3
006b7c9e: int3
006b7c9f: int3
