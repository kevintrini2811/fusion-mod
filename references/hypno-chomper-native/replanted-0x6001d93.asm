006f3930: mov qword ptr [rsp + 0x10], rbx
006f3935: push rdi
006f3936: sub rsp, 0x20
006f393a: cmp byte ptr [rip + 0x47f7edf], 0
006f3941: mov rdi, rcx
006f3944: jne 0x1806f3965
006f3946: lea rcx, [rip + 0x458bca3]
006f394d: call 0x1804feb70
006f3952: lea rcx, [rip + 0x458bd6f]
006f3959: call 0x1804feb70
006f395e: mov byte ptr [rip + 0x47f7ebb], 1
006f3965: mov rax, qword ptr [rdi + 0x18]
006f3969: xor ebx, ebx
006f396b: mov qword ptr [rsp + 0x30], rsi
006f3970: mov edx, ebx
006f3972: test rax, rax
006f3975: je 0x1806f3a11
006f397b: nop dword ptr [rax + rax]
006f3980: mov rcx, qword ptr [rax + 0x78]
006f3984: test rcx, rcx
006f3987: je 0x1806f3a11
006f398d: cmp edx, dword ptr [rcx + 0x28]
006f3990: jge 0x1806f39ff
006f3992: mov rax, qword ptr [rdi + 0x18]
006f3996: test rax, rax
006f3999: je 0x1806f3a11
006f399b: mov rcx, qword ptr [rax + 0x78]
006f399f: test rcx, rcx
006f39a2: je 0x1806f3a11
006f39a4: mov r8, qword ptr [rip + 0x458bd1d]
006f39ab: mov edx, ebx
006f39ad: call 0x182449330
006f39b2: test rax, rax
006f39b5: je 0x1806f3a11
006f39b7: cmp dword ptr [rax + 0x88], 0x22
006f39be: jne 0x1806f39de
006f39c0: mov rcx, qword ptr [rdi + 0x18]
006f39c4: mov esi, dword ptr [rax + 0x120]
006f39ca: test rcx, rcx
006f39cd: je 0x1806f3a11
006f39cf: xor r8d, r8d
006f39d2: mov rdx, rdi
006f39d5: call 0x180676eb0
006f39da: cmp esi, eax
006f39dc: je 0x1806f39ed
006f39de: mov rax, qword ptr [rdi + 0x18]
006f39e2: inc ebx
006f39e4: mov edx, ebx
006f39e6: test rax, rax
006f39e9: je 0x1806f3a11
006f39eb: jmp 0x1806f3980
006f39ed: mov al, 1
006f39ef: mov rsi, qword ptr [rsp + 0x30]
006f39f4: mov rbx, qword ptr [rsp + 0x38]
006f39f9: add rsp, 0x20
006f39fd: pop rdi
006f39fe: ret
006f39ff: mov rsi, qword ptr [rsp + 0x30]
006f3a04: xor al, al
006f3a06: mov rbx, qword ptr [rsp + 0x38]
006f3a0b: add rsp, 0x20
006f3a0f: pop rdi
006f3a10: ret
006f3a11: call 0x1804fee20
006f3a16: int3
006f3a17: int3
006f3a18: int3
006f3a19: int3
006f3a1a: int3
006f3a1b: int3
006f3a1c: int3
006f3a1d: int3
006f3a1e: int3
006f3a1f: int3
