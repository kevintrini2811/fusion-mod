00495f60: mov qword ptr [rsp + 8], rbx
00495f65: mov qword ptr [rsp + 0x10], rbp
00495f6a: mov qword ptr [rsp + 0x18], rsi
00495f6f: push rdi
00495f70: sub rsp, 0x30
00495f74: mov ebp, edx
00495f76: mov rdi, r9
00495f79: xor edx, edx
00495f7b: mov esi, r8d
00495f7e: mov rbx, rcx
00495f81: call 0x180471ba0
00495f86: mov rcx, rbx
00495f89: cmp eax, 0x5b8
00495f8e: je 0x180495fbb
00495f90: mov r9, rdi
00495f93: mov qword ptr [rsp + 0x20], 0
00495f9c: mov r8d, esi
00495f9f: mov edx, ebp
00495fa1: call 0x180467470
00495fa6: mov rbx, qword ptr [rsp + 0x40]
00495fab: mov rbp, qword ptr [rsp + 0x48]
00495fb0: mov rsi, qword ptr [rsp + 0x50]
00495fb5: add rsp, 0x30
00495fb9: pop rdi
00495fba: ret
00495fbb: xor edx, edx
00495fbd: call 0x180471f30
00495fc2: test rax, rax
00495fc5: je 0x180495fed
00495fc7: mov rcx, qword ptr [rax]
00495fca: mov r9, rdi
00495fcd: mov r8d, esi
00495fd0: mov edx, ebp
00495fd2: mov r10, qword ptr [rcx + 0x358]
00495fd9: mov rcx, qword ptr [rcx + 0x360]
00495fe0: mov qword ptr [rsp + 0x20], rcx
00495fe5: mov rcx, rax
00495fe8: call r10
00495feb: jmp 0x180495fa6
00495fed: call 0x180363610
00495ff2: int3
00495ff3: int3
00495ff4: int3
00495ff5: int3
00495ff6: int3
00495ff7: int3
00495ff8: int3
00495ff9: int3
00495ffa: int3
00495ffb: int3
00495ffc: int3
00495ffd: int3
00495ffe: int3
00495fff: int3
