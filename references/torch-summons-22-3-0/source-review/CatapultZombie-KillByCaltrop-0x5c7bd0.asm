005c7bd0: mov qword ptr [rsp + 0x10], rbx
005c7bd5: mov qword ptr [rsp + 8], rcx
005c7bda: push rsi
005c7bdb: push rdi
005c7bdc: push r12
005c7bde: push r14
005c7be0: push r15
005c7be2: sub rsp, 0x50
005c7be6: mov rsi, rcx
005c7be9: cmp byte ptr [rip + 0x2fd1efe], 0
005c7bf0: jne 0x1805c7c81
005c7bf6: lea rcx, [rip + 0x2de7e2b]
005c7bfd: call 0x1803633b0
005c7c02: lea rcx, [rip + 0x2dea37f]
005c7c09: call 0x1803633b0
005c7c0e: lea rcx, [rip + 0x2deab5b]
005c7c15: call 0x1803633b0
005c7c1a: lea rcx, [rip + 0x2dea3cf]
005c7c21: call 0x1803633b0
005c7c26: lea rcx, [rip + 0x2deb2db]
005c7c2d: call 0x1803633b0
005c7c32: lea rcx, [rip + 0x2df8087]
005c7c39: call 0x1803633b0
005c7c3e: lea rcx, [rip + 0x2dd1c0b]
005c7c45: call 0x1803633b0
005c7c4a: lea rcx, [rip + 0x2db1c27]
005c7c51: call 0x1803633b0
005c7c56: lea rcx, [rip + 0x2dfd263]
005c7c5d: call 0x1803633b0
005c7c62: lea rcx, [rip + 0x2e035c7]
005c7c69: call 0x1803633b0
005c7c6e: lea rcx, [rip + 0x2db0ae3]
005c7c75: call 0x1803633b0
005c7c7a: mov byte ptr [rip + 0x2fd1e6d], 1
005c7c81: xor r14d, r14d
005c7c84: mov qword ptr [rsp + 0x98], r14
005c7c8c: mov qword ptr [rsp + 0x20], r14
005c7c91: mov rcx, qword ptr [rsi + 0x28]
005c7c95: test rcx, rcx
005c7c98: je 0x1805c7fc0
005c7c9e: xor r8d, r8d
005c7ca1: mov rdx, qword ptr [rip + 0x2dfd218]
005c7ca8: call 0x182678fd0
005c7cad: mov rcx, qword ptr [rsi + 0x28]
005c7cb1: test rcx, rcx
005c7cb4: je 0x1805c7fc0
005c7cba: xor r8d, r8d
005c7cbd: mov rdx, qword ptr [rip + 0x2db1bb4]
005c7cc4: call 0x182678fd0
005c7cc9: xor edx, edx
005c7ccb: mov rcx, rsi
005c7cce: call 0x1826b1710
005c7cd3: test rax, rax
005c7cd6: je 0x1805c7fc0
005c7cdc: xor r8d, r8d
005c7cdf: mov edx, 1
005c7ce4: mov rcx, rax
005c7ce7: call 0x1826c6b90
005c7cec: test rax, rax
005c7cef: je 0x1805c7fc0
005c7cf5: xor r8d, r8d
005c7cf8: xor edx, edx
005c7cfa: mov rcx, rax
005c7cfd: call 0x1826c6b90
005c7d02: test rax, rax
005c7d05: je 0x1805c7fc0
005c7d0b: xor edx, edx
005c7d0d: mov rcx, rax
005c7d10: call 0x1826b16d0
005c7d15: mov rbx, rax
005c7d18: test rax, rax
005c7d1b: je 0x1805c7fc0
005c7d21: xor r8d, r8d
005c7d24: mov dl, 1
005c7d26: mov rcx, rax
005c7d29: call 0x1826b53f0
005c7d2e: xor edx, edx
005c7d30: mov rcx, rbx
005c7d33: call 0x1826b59a0
005c7d38: test rax, rax
005c7d3b: je 0x1805c7fc0
005c7d41: xor edx, edx
005c7d43: mov rcx, rax
005c7d46: call 0x1826c6bd0
005c7d4b: mov qword ptr [rsp + 0x98], rax
005c7d53: mov qword ptr [rsp + 0x30], r14
005c7d58: lea r12, [rsp + 0x98]
005c7d60: mov qword ptr [rsp + 0x38], r12
005c7d65: lea r15, [rsp + 0x20]
005c7d6a: mov qword ptr [rsp + 0x40], r15
005c7d6f: nop
005c7d70: mov r8, qword ptr [rsp + 0x98]
005c7d78: test r8, r8
005c7d7b: je 0x1805c7fed
005c7d81: xor ecx, ecx
005c7d83: mov rdx, qword ptr [rip + 0x2deb17e]
005c7d8a: call 0x180002cc0
005c7d8f: test al, al
005c7d91: je 0x1805c7f1e
005c7d97: mov rbx, qword ptr [rsp + 0x98]
005c7d9f: test rbx, rbx
005c7da2: je 0x1805c7fe8
005c7da8: mov r9, qword ptr [rip + 0x2deb159]
005c7daf: mov r10, qword ptr [rbx]
005c7db2: movzx ecx, r14w
005c7db6: movzx edx, word ptr [r10 + 0x12e]
005c7dbe: cmp r14w, dx
005c7dc2: jae 0x1805c7de4
005c7dc4: mov r8, qword ptr [r10 + 0xb0]
005c7dcb: nop dword ptr [rax + rax]
005c7dd0: movzx eax, cx
005c7dd3: add rax, rax
005c7dd6: cmp qword ptr [r8 + rax*8], r9
005c7dda: je 0x1805c7e19
005c7ddc: inc cx
005c7ddf: cmp cx, dx
005c7de2: jb 0x1805c7dd0
005c7de4: mov r8d, 1
005c7dea: mov rdx, r9
005c7ded: mov rcx, rbx
005c7df0: call 0x1802f3420
005c7df5: mov rdx, rax
005c7df8: mov rax, qword ptr [rdx]
005c7dfb: mov rdx, qword ptr [rdx + 8]
005c7dff: mov rcx, rbx
005c7e02: call rax
005c7e04: mov rbx, rax
005c7e07: mov rdx, qword ptr [rip + 0x2dd1a42]
005c7e0e: test rax, rax
005c7e11: je 0x1805c7fc6
005c7e17: jmp 0x1805c7e39
005c7e19: movzx ecx, cx
005c7e1c: add rcx, rcx
005c7e1f: mov ecx, dword ptr [r8 + rcx*8 + 8]
005c7e24: inc ecx
005c7e26: movsxd rdx, ecx
005c7e29: shl rdx, 4
005c7e2d: add rdx, 0x138
005c7e34: add rdx, r10
005c7e37: jmp 0x1805c7df8
005c7e39: mov r8, qword ptr [rax]
005c7e3c: movzx eax, byte ptr [rdx + 0x130]
005c7e43: cmp byte ptr [r8 + 0x130], al
005c7e4a: jb 0x1805c7fe0
005c7e50: movzx ecx, al
005c7e53: mov rax, qword ptr [r8 + 0xc8]
005c7e5a: cmp qword ptr [rax + rcx*8 - 8], rdx
005c7e5f: jne 0x1805c7fe0
005c7e65: mov rdx, qword ptr [rip + 0x2dea11c]
005c7e6c: mov rcx, rbx
005c7e6f: call 0x180ac9290
005c7e74: test rax, rax
005c7e77: je 0x1805c7fdb
005c7e7d: mov rdx, qword ptr [rip + 0x2dea8ec]
005c7e84: mov rcx, rax
005c7e87: call 0x180ac9290
005c7e8c: mov rdi, rax
005c7e8f: mov ecx, dword ptr [rsi + 0xcc]
005c7e95: mov dword ptr [rsp + 0x90], ecx
005c7e9c: lea rdx, [rsp + 0x90]
005c7ea4: mov rcx, qword ptr [rip + 0x2df7e15]
005c7eab: call 0x180318da0
005c7eb0: xor r8d, r8d
005c7eb3: mov rdx, rax
005c7eb6: mov rcx, qword ptr [rip + 0x2db089b]
005c7ebd: call 0x181898f40
005c7ec2: test rdi, rdi
005c7ec5: je 0x1805c7fd6
005c7ecb: xor r8d, r8d
005c7ece: mov rdx, rax
005c7ed1: mov rcx, rdi
005c7ed4: call 0x182697aa0
005c7ed9: mov rdx, qword ptr [rip + 0x2dea0a8]
005c7ee0: mov rcx, rbx
005c7ee3: call 0x180ac9290
005c7ee8: test rax, rax
005c7eeb: je 0x1805c7fd1
005c7ef1: mov rdx, qword ptr [rip + 0x2dea878]
005c7ef8: mov rcx, rax
005c7efb: call 0x180ac9290
005c7f00: test rax, rax
005c7f03: je 0x1805c7fcc
005c7f09: xor r8d, r8d
005c7f0c: mov edx, 0x8c
005c7f11: mov rcx, rax
005c7f14: call 0x182697af0
005c7f19: jmp 0x1805c7d70
005c7f1e: mov rdx, qword ptr [rip + 0x2dea0cb]
005c7f25: mov rcx, qword ptr [r12]
005c7f29: call 0x180362750
005c7f2e: mov qword ptr [r15], rax
005c7f31: test rax, rax
005c7f34: je 0x1805c7f47
005c7f36: xor ecx, ecx
005c7f38: mov r8, rax
005c7f3b: mov rdx, qword ptr [rip + 0x2dea0ae]
005c7f42: call 0x180002cc0
005c7f47: jmp 0x1805c7f69
005c7f49: lea rcx, [rsp + 0x38]
005c7f4e: call 0x180404f50
005c7f53: mov rcx, qword ptr [rsp + 0x30]
005c7f58: test rcx, rcx
005c7f5b: jne 0x1805c7ff3
005c7f61: mov rsi, qword ptr [rsp + 0x80]
005c7f69: mov rdx, qword ptr [rip + 0x2de7ab8]
005c7f70: mov rcx, rsi
005c7f73: call 0x180ac9290
005c7f78: test rax, rax
005c7f7b: je 0x1805c7fc0
005c7f7d: xor r8d, r8d
005c7f80: xor edx, edx
005c7f82: mov rcx, rax
005c7f85: call 0x1826b0bd0
005c7f8a: mov byte ptr [rsi + 0x135], 1
005c7f91: xor r9d, r9d
005c7f94: movss xmm2, dword ptr [rip + 0x23b3bd4]
005c7f9c: mov rdx, qword ptr [rip + 0x2e0328d]
005c7fa3: mov rcx, rsi
005c7fa6: call 0x1826b6ec0
005c7fab: mov rbx, qword ptr [rsp + 0x88]
005c7fb3: add rsp, 0x50
005c7fb7: pop r15
005c7fb9: pop r14
005c7fbb: pop r12
005c7fbd: pop rdi
005c7fbe: pop rsi
005c7fbf: ret
005c7fc0: call 0x180363610
005c7fc5: nop
005c7fc6: call 0x180363610
005c7fcb: nop
005c7fcc: call 0x180363610
005c7fd1: call 0x180363610
005c7fd6: call 0x180363610
005c7fdb: call 0x180363610
005c7fe0: mov rcx, rbx
005c7fe3: call 0x180362770
005c7fe8: call 0x180363610
005c7fed: call 0x180363610
005c7ff2: nop
005c7ff3: call 0x1803636a0
005c7ff8: int3
005c7ff9: int3
005c7ffa: int3
005c7ffb: int3
005c7ffc: int3
005c7ffd: int3
005c7ffe: int3
005c7fff: int3
