00489de0: push rbx
00489de2: sub rsp, 0x60
00489de6: cmp byte ptr [rip + 0x310f366], 0
00489ded: mov rbx, rcx
00489df0: jne 0x180489e1d
00489df2: lea rcx, [rip + 0x2f19d87]
00489df9: call 0x1803633b0
00489dfe: lea rcx, [rip + 0x2f565c3]
00489e05: call 0x1803633b0
00489e0a: lea rcx, [rip + 0x2ef6b27]
00489e11: call 0x1803633b0
00489e16: mov byte ptr [rip + 0x310f336], 1
00489e1d: mov rdx, qword ptr [rbx + 0x38]
00489e21: mov qword ptr [rsp + 0x70], rdi
00489e26: test rdx, rdx
00489e29: je 0x180489f7c
00489e2f: xor r8d, r8d
00489e32: lea rcx, [rsp + 0x50]
00489e37: call 0x1826c8510
00489e3c: mov r9d, dword ptr [rbx + 0xf8]
00489e43: lea rcx, [rsp + 0x40]
00489e48: mov r8d, dword ptr [rbx + 0x4c]
00489e4c: movss xmm1, dword ptr [rip + 0x24f1cf4]
00489e54: movsd xmm0, qword ptr [rax]
00489e58: mov eax, dword ptr [rax + 8]
00489e5b: mov dword ptr [rsp + 0x48], eax
00489e5f: mov eax, dword ptr [rbx + 0x194]
00489e65: mov qword ptr [rsp + 0x38], 0
00489e6e: mov qword ptr [rsp + 0x30], 0
00489e77: mov dword ptr [rsp + 0x28], eax
00489e7b: mov eax, dword ptr [rbx + 0xf0]
00489e81: mov dword ptr [rsp + 0x20], eax
00489e85: movsd qword ptr [rsp + 0x40], xmm0
00489e8b: call 0x180796680
00489e90: mov rax, qword ptr [rip + 0x2f56531]
00489e97: cmp dword ptr [rax + 0xe0], 0
00489e9e: jne 0x180489eaf
00489ea0: mov rcx, rax
00489ea3: call 0x180318580
00489ea8: mov rax, qword ptr [rip + 0x2f56519]
00489eaf: mov rax, qword ptr [rax + 0xb8]
00489eb6: mov rdx, qword ptr [rbx + 0x38]
00489eba: mov rdi, qword ptr [rax]
00489ebd: test rdx, rdx
00489ec0: je 0x180489f7c
00489ec6: xor r8d, r8d
00489ec9: lea rcx, [rsp + 0x50]
00489ece: call 0x1826c8510
00489ed3: movsd xmm2, qword ptr [rax]
00489ed7: movaps xmm1, xmm2
00489eda: shufps xmm1, xmm1, 0x55
00489ede: test rdi, rdi
00489ee1: je 0x180489f7c
00489ee7: xorps xmm0, xmm0
00489eea: mov qword ptr [rsp + 0x30], 0
00489ef3: movss dword ptr [rsp + 0x28], xmm0
00489ef9: mov r9d, 0xb
00489eff: movaps xmm0, xmm2
00489f02: mov byte ptr [rsp + 0x20], 1
00489f07: unpcklps xmm0, xmm1
00489f0a: mov edx, 8
00489f0f: movq r8, xmm0
00489f14: mov rcx, rdi
00489f17: call 0x180465c40
00489f1c: mov rcx, qword ptr [rip + 0x2f19c5d]
00489f23: cmp dword ptr [rcx + 0xe0], 0
00489f2a: jne 0x180489f31
00489f2c: call 0x180318580
00489f31: movss xmm2, dword ptr [rip + 0x24f1c0f]
00489f39: xor r9d, r9d
00489f3c: movss xmm1, dword ptr [rip + 0x24f1bf8]
00489f44: mov ecx, 0x2f
00489f49: call 0x18066a930
00489f4e: mov rcx, qword ptr [rip + 0x2ef69e3]
00489f55: cmp dword ptr [rcx + 0xe0], 0
00489f5c: jne 0x180489f63
00489f5e: call 0x180318580
00489f63: movss xmm0, dword ptr [rip + 0x24f1e15]
00489f6b: xor edx, edx
00489f6d: mov rdi, qword ptr [rsp + 0x70]
00489f72: add rsp, 0x60
00489f76: pop rbx
00489f77: jmp 0x1805b0070
00489f7c: call 0x180363610
00489f81: int3 
00489f82: int3 
00489f83: int3 
00489f84: int3 
00489f85: int3 
00489f86: int3 
00489f87: int3 
00489f88: int3 
00489f89: int3 
00489f8a: int3 
00489f8b: int3 
00489f8c: int3 
00489f8d: int3 
00489f8e: int3 
00489f8f: int3 
