006f3aa0: mov qword ptr [rsp + 8], rbx
006f3aa5: push rdi
006f3aa6: sub rsp, 0x30
006f3aaa: cmp byte ptr [rip + 0x47f7d0e], 0
006f3ab1: mov edi, edx
006f3ab3: mov rbx, rcx
006f3ab6: jne 0x6f3ae3
006f3ab8: lea rcx, [rip + 0x4537501]
006f3abf: call 0x4feb70 ;
006f3ac4: lea rcx, [rip + 0x458169d]
006f3acb: call 0x4feb70 ;
006f3ad0: lea rcx, [rip + 0x45a3fc9]
006f3ad7: call 0x4feb70 ;
006f3adc: mov byte ptr [rip + 0x47f7cdc], 1
006f3ae3: mov rcx, qword ptr [rip + 0x45374d6]
006f3aea: cmp dword ptr [rcx + 0xe4], 0
006f3af1: jne 0x6f3af8
006f3af3: call 0x4feeb0 ;
006f3af8: test dil, 0x10
006f3afc: ja 0x6f3b6e
006f3afe: cmp dword ptr [rbx + 0x74], 0x49
006f3b02: jne 0x6f3b6e
006f3b04: mov rax, qword ptr [rbx + 0x10]
006f3b08: test rax, rax
006f3b0b: je 0x6f3bc6
006f3b11: mov r8, qword ptr [rax + 0x58]
006f3b15: test r8, r8
006f3b18: je 0x6f3bc6
006f3b1e: mov rdx, qword ptr [rip + 0x4581643]
006f3b25: mov ecx, 0x19
006f3b2a: mov r9d, 3
006f3b30: call 0x2040 ;
006f3b35: movss xmm0, dword ptr [rip + 0x35403b3]
006f3b3d: mov r9d, 0x14
006f3b43: mov dword ptr [rbx + 0x74], 0x4a
006f3b4a: mov r8d, 3
006f3b50: mov rdx, qword ptr [rip + 0x45a3f49]
006f3b57: mov rcx, rbx
006f3b5a: mov qword ptr [rsp + 0x28], 0
006f3b63: movss dword ptr [rsp + 0x20], xmm0
006f3b69: call 0x6f55d0 ; Reloaded.Gameplay.Zombie.PlayZombieReanim
006f3b6e: mov rcx, qword ptr [rbx + 0x18]
006f3b72: test rcx, rcx
006f3b75: je 0x6f3bc6
006f3b77: mov rcx, qword ptr [rcx + 0x158]
006f3b7e: test rcx, rcx
006f3b81: je 0x6f3bc6
006f3b83: movsxd rax, dword ptr [rbx + 0x34]
006f3b87: cmp eax, dword ptr [rcx + 0x18]
006f3b8a: jae 0x6f3bcc
006f3b8c: cmp dword ptr [rcx + rax*4 + 0x20], 2
006f3b91: je 0x6f3ba8
006f3b93: mov dword ptr [rbx + 0xb8], 7
006f3b9d: mov rbx, qword ptr [rsp + 0x40]
006f3ba2: add rsp, 0x30
006f3ba6: pop rdi
006f3ba7: ret
006f3ba8: xor edx, edx
006f3baa: mov rcx, rbx
006f3bad: call 0x6ebf90 ; Reloaded.Gameplay.Zombie.DieNoLoot
006f3bb2: xor edx, edx
006f3bb4: mov rcx, rbx
006f3bb7: mov rbx, qword ptr [rsp + 0x40]
006f3bbc: add rsp, 0x30
006f3bc0: pop rdi
006f3bc1: jmp 0x6f0500 ; Reloaded.Gameplay.Zombie.DropLoot
006f3bc6: call 0x4fee20 ;
006f3bcb: int3
006f3bcc: call 0x4fee10 ;
006f3bd1: int3
006f3bd2: int3
006f3bd3: int3
006f3bd4: int3
006f3bd5: int3
006f3bd6: int3
006f3bd7: int3
006f3bd8: int3
006f3bd9: int3
006f3bda: int3
006f3bdb: int3
006f3bdc: int3
006f3bdd: int3
006f3bde: int3
006f3bdf: int3
