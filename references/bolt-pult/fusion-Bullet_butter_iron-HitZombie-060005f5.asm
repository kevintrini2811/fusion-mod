006fb5a0: mov qword ptr [rsp + 0x10], rbx
006fb5a5: push rdi
006fb5a6: sub rsp, 0x60
006fb5aa: cmp byte ptr [rip + 0x2e9ecf7], 0
006fb5b1: mov rbx, rdx
006fb5b4: mov rdi, rcx
006fb5b7: jne 0x1806fb5d8
006fb5b9: lea rcx, [rip + 0x2c8c8d0]
006fb5c0: call 0x1803633b0
006fb5c5: lea rcx, [rip + 0x2ca85b4]
006fb5cc: call 0x1803633b0
006fb5d1: mov byte ptr [rip + 0x2e9ecd0], 1
006fb5d8: test rbx, rbx
006fb5db: je 0x1806fb71a
006fb5e1: mov rax, qword ptr [rbx]
006fb5e4: cmp dword ptr [rbx + 0xb0], 0
006fb5eb: mov r9d, 4
006fb5f1: mov r10, qword ptr [rax + 0x1f8]
006fb5f8: mov r8, rdi
006fb5fb: mov rax, qword ptr [rax + 0x200]
006fb602: mov rcx, rbx
006fb605: mov qword ptr [rsp + 0x30], rax
006fb60a: mov eax, dword ptr [rdi + 0xa0]
006fb610: mov byte ptr [rsp + 0x28], 0
006fb615: mov dword ptr [rsp + 0x20], eax
006fb619: jne 0x1806fb623
006fb61b: mov edx, dword ptr [rdi + 0xdc]
006fb621: jmp 0x1806fb62f
006fb623: mov edx, dword ptr [rbx + 0xb0]
006fb629: add edx, dword ptr [rdi + 0xdc]
006fb62f: call r10
006fb632: mov r9, qword ptr [rbx]
006fb635: mov r8b, 1
006fb638: movss xmm1, dword ptr [rip + 0x2280538]
006fb640: mov rcx, rbx
006fb643: mov rax, qword ptr [r9 + 0x508]
006fb64a: mov r9, qword ptr [r9 + 0x510]
006fb651: call rax
006fb653: xor edx, edx
006fb655: mov rcx, rdi
006fb658: call 0x1826b1710
006fb65d: test rax, rax
006fb660: je 0x1806fb71a
006fb666: mov qword ptr [rsp + 0x70], rsi
006fb66b: lea rcx, [rsp + 0x40]
006fb670: xor r8d, r8d
006fb673: movaps xmmword ptr [rsp + 0x50], xmm6
006fb678: mov rdx, rax
006fb67b: call 0x1826c8510
006fb680: mov rcx, qword ptr [rip + 0x2c8c809]
006fb687: mov esi, dword ptr [rdi + 0xc0]
006fb68d: movsd xmm6, qword ptr [rax]
006fb691: cmp dword ptr [rcx + 0xe0], 0
006fb698: mov ebx, dword ptr [rax + 8]
006fb69b: jne 0x1806fb6a2
006fb69d: call 0x180318580
006fb6a2: mov r9b, 1
006fb6a5: movsd qword ptr [rsp + 0x40], xmm6
006fb6ab: mov r8d, esi
006fb6ae: mov dword ptr [rsp + 0x48], ebx
006fb6b2: lea rdx, [rsp + 0x40]
006fb6b7: mov qword ptr [rsp + 0x20], 0
006fb6c0: mov ecx, 0x12
006fb6c5: call 0x1809d82c0
006fb6ca: mov rcx, qword ptr [rip + 0x2ca84af]
006fb6d1: movaps xmm6, xmmword ptr [rsp + 0x50]
006fb6d6: mov rsi, qword ptr [rsp + 0x70]
006fb6db: cmp dword ptr [rcx + 0xe0], 0
006fb6e2: jne 0x1806fb6e9
006fb6e4: call 0x180318580
006fb6e9: movss xmm2, dword ptr [rip + 0x2280457]
006fb6f1: xor r9d, r9d
006fb6f4: movss xmm1, dword ptr [rip + 0x2280440]
006fb6fc: mov ecx, 0x64
006fb701: call 0x18066a930
006fb706: xor edx, edx
006fb708: mov rcx, rdi
006fb70b: mov rbx, qword ptr [rsp + 0x78]
006fb710: add rsp, 0x60
006fb714: pop rdi
006fb715: jmp 0x1805ef340
006fb71a: call 0x180363610
006fb71f: int3
