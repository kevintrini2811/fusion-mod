006fb490: push rbx
006fb492: sub rsp, 0x50
006fb496: cmp byte ptr [rip + 0x2e9ee0c], 0
006fb49d: mov rbx, rcx
006fb4a0: jne 0x1806fb4c1
006fb4a2: lea rcx, [rip + 0x2c8c9e7]
006fb4a9: call 0x1803633b0
006fb4ae: lea rcx, [rip + 0x2ca86cb]
006fb4b5: call 0x1803633b0
006fb4ba: mov byte ptr [rip + 0x2e9ede8], 1
006fb4c1: xor edx, edx
006fb4c3: mov rcx, rbx
006fb4c6: call 0x1826b1710
006fb4cb: test rax, rax
006fb4ce: je 0x1806fb58d
006fb4d4: mov qword ptr [rsp + 0x60], rsi
006fb4d9: lea rcx, [rsp + 0x30]
006fb4de: mov qword ptr [rsp + 0x68], rdi
006fb4e3: xor r8d, r8d
006fb4e6: mov rdx, rax
006fb4e9: movaps xmmword ptr [rsp + 0x40], xmm6
006fb4ee: call 0x1826c8510
006fb4f3: mov rcx, qword ptr [rip + 0x2c8c996]
006fb4fa: mov esi, dword ptr [rbx + 0xc0]
006fb500: movsd xmm6, qword ptr [rax]
006fb504: cmp dword ptr [rcx + 0xe0], 0
006fb50b: mov edi, dword ptr [rax + 8]
006fb50e: jne 0x1806fb515
006fb510: call 0x180318580
006fb515: mov r9b, 1
006fb518: movsd qword ptr [rsp + 0x30], xmm6
006fb51e: mov r8d, esi
006fb521: mov dword ptr [rsp + 0x38], edi
006fb525: lea rdx, [rsp + 0x30]
006fb52a: mov qword ptr [rsp + 0x20], 0
006fb533: mov ecx, 0x12
006fb538: call 0x1809d82c0
006fb53d: mov rcx, qword ptr [rip + 0x2ca863c]
006fb544: movaps xmm6, xmmword ptr [rsp + 0x40]
006fb549: mov rdi, qword ptr [rsp + 0x68]
006fb54e: mov rsi, qword ptr [rsp + 0x60]
006fb553: cmp dword ptr [rcx + 0xe0], 0
006fb55a: jne 0x1806fb561
006fb55c: call 0x180318580
006fb561: movss xmm2, dword ptr [rip + 0x22805df]
006fb569: xor r9d, r9d
006fb56c: movss xmm1, dword ptr [rip + 0x22805c8]
006fb574: mov ecx, 0x64
006fb579: call 0x18066a930
006fb57e: xor edx, edx
006fb580: mov rcx, rbx
006fb583: add rsp, 0x50
006fb587: pop rbx
006fb588: jmp 0x1805ef340
006fb58d: call 0x180363610
006fb592: int3
006fb593: int3
006fb594: int3
006fb595: int3
006fb596: int3
006fb597: int3
006fb598: int3
006fb599: int3
006fb59a: int3
006fb59b: int3
006fb59c: int3
006fb59d: int3
006fb59e: int3
006fb59f: int3
