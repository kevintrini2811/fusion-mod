0070d8e0: mov qword ptr [rsp + 8], rbx
0070d8e5: mov qword ptr [rsp + 0x10], rsi
0070d8ea: push rdi
0070d8eb: sub rsp, 0x40
0070d8ef: cmp byte ptr [rip + 0x2e8ca3a], 0
0070d8f6: mov rbx, rdx
0070d8f9: mov rdi, rcx
0070d8fc: jne 0x18070d911
0070d8fe: lea rcx, [rip + 0x2c9627b]
0070d905: call 0x1803633b0
0070d90a: mov byte ptr [rip + 0x2e8ca1f], 1
0070d911: xor r8d, r8d
0070d914: mov edx, 0x64
0070d919: mov ecx, 0x62
0070d91e: call 0x1826bcca0
0070d923: mov rcx, qword ptr [rip + 0x2c96256]
0070d92a: mov esi, eax
0070d92c: cmp dword ptr [rcx + 0xe0], 0
0070d933: jne 0x18070d93a
0070d935: call 0x180318580
0070d93a: movss xmm2, dword ptr [rip + 0x226e206]
0070d942: xor r9d, r9d
0070d945: movss xmm1, dword ptr [rip + 0x226e1ef]
0070d94d: mov ecx, esi
0070d94f: call 0x18066a930
0070d954: test rbx, rbx
0070d957: je 0x18070d9ac
0070d959: mov rax, qword ptr [rbx]
0070d95c: mov r9d, 4
0070d962: mov edx, dword ptr [rdi + 0xdc]
0070d968: mov r8, rdi
0070d96b: mov rcx, rbx
0070d96e: mov r10, qword ptr [rax + 0x1f8]
0070d975: mov rax, qword ptr [rax + 0x200]
0070d97c: mov qword ptr [rsp + 0x30], rax
0070d981: mov eax, dword ptr [rdi + 0xa0]
0070d987: mov byte ptr [rsp + 0x28], 0
0070d98c: mov dword ptr [rsp + 0x20], eax
0070d990: call r10
0070d993: xor edx, edx
0070d995: mov rcx, rdi
0070d998: mov rbx, qword ptr [rsp + 0x50]
0070d99d: mov rsi, qword ptr [rsp + 0x58]
0070d9a2: add rsp, 0x40
0070d9a6: pop rdi
0070d9a7: jmp 0x1805ef340
0070d9ac: call 0x180363610
0070d9b1: int3
0070d9b2: int3
0070d9b3: int3
0070d9b4: int3
0070d9b5: int3
0070d9b6: int3
0070d9b7: int3
0070d9b8: int3
0070d9b9: int3
0070d9ba: int3
0070d9bb: int3
0070d9bc: int3
0070d9bd: int3
0070d9be: int3
0070d9bf: int3
