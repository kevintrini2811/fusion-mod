006e99a0: mov qword ptr [rsp + 8], rbx
006e99a5: push rdi
006e99a6: sub rsp, 0x30
006e99aa: cmp byte ptr [rip + 0x4801e42], 0
006e99b1: mov edi, edx
006e99b3: mov rbx, rcx
006e99b6: jne 0x6e99e3
006e99b8: lea rcx, [rip + 0x4541601]
006e99bf: call 0x4feb70 ;
006e99c4: lea rcx, [rip + 0x458b79d]
006e99cb: call 0x4feb70 ;
006e99d0: lea rcx, [rip + 0x45a9ac9]
006e99d7: call 0x4feb70 ;
006e99dc: mov byte ptr [rip + 0x4801e10], 1
006e99e3: mov rcx, qword ptr [rip + 0x45415d6]
006e99ea: cmp dword ptr [rcx + 0xe4], 0
006e99f1: jne 0x6e99f8
006e99f3: call 0x4feeb0 ;
006e99f8: mov rax, qword ptr [rbx + 0x10]
006e99fc: and edi, 0x20
006e99ff: test rax, rax
006e9a02: je 0x6e9b87
006e9a08: test edi, edi
006e9a0a: jne 0x6e9a97
006e9a10: movss xmm2, dword ptr [rbx + 0x7c]
006e9a15: xor edi, edi
006e9a17: movss xmm1, dword ptr [rbx + 0x78]
006e9a1c: mov rcx, rax
006e9a1f: mov r9d, dword ptr [rbx + 0x38]
006e9a23: addss xmm2, dword ptr [rip + 0x354a23d]
006e9a2b: addss xmm1, dword ptr [rip + 0x354a399]
006e9a33: inc r9d
006e9a36: mov qword ptr [rsp + 0x28], rdi
006e9a3b: mov dword ptr [rsp + 0x20], 0x41
006e9a43: call 0x7d9b50 ; Reloaded.TreeStateActivities.GameplayActivity.AddTodParticle
006e9a48: xor edx, edx
006e9a4a: mov rcx, rbx
006e9a4d: call 0x6ebf90 ; Reloaded.Gameplay.Zombie.DieNoLoot
006e9a52: xor edx, edx
006e9a54: mov rcx, rbx
006e9a57: call 0x6f0500 ; Reloaded.Gameplay.Zombie.DropLoot
006e9a5c: mov r8, qword ptr [rbx + 0x10]
006e9a60: test r8, r8
006e9a63: je 0x6e9b87
006e9a69: mov r8, qword ptr [r8 + 0x58]
006e9a6d: test r8, r8
006e9a70: je 0x6e9b87
006e9a76: mov rdx, qword ptr [rip + 0x458b6eb]
006e9a7d: mov ecx, 6
006e9a82: mov r9d, 0x17
006e9a88: mov rbx, qword ptr [rsp + 0x40]
006e9a8d: add rsp, 0x30
006e9a91: pop rdi
006e9a92: jmp 0x2040 ;
006e9a97: mov r8, qword ptr [rax + 0x58]
006e9a9b: test r8, r8
006e9a9e: je 0x6e9b87
006e9aa4: mov rdx, qword ptr [rip + 0x458b6bd]
006e9aab: mov ecx, 6
006e9ab0: mov r9d, 0x16
006e9ab6: call 0x2040 ;
006e9abb: mov rcx, qword ptr [rbx + 0x10]
006e9abf: mov dword ptr [rbx + 0x74], 1
006e9ac6: test rcx, rcx
006e9ac9: je 0x6e9b87
006e9acf: movss xmm2, dword ptr [rbx + 0x7c]
006e9ad4: xor edi, edi
006e9ad6: movss xmm1, dword ptr [rbx + 0x78]
006e9adb: mov r9d, dword ptr [rbx + 0x38]
006e9adf: addss xmm2, dword ptr [rip + 0x354a70d]
006e9ae7: addss xmm1, dword ptr [rip + 0x354a4cd]
006e9aef: inc r9d
006e9af2: mov qword ptr [rsp + 0x28], rdi
006e9af7: mov dword ptr [rsp + 0x20], 0x3e
006e9aff: call 0x7d9b50 ; Reloaded.TreeStateActivities.GameplayActivity.AddTodParticle
006e9b04: test rax, rax
006e9b07: je 0x6e9b87
006e9b09: mov rcx, qword ptr [rax + 0xa0]
006e9b10: test rcx, rcx
006e9b13: je 0x6e9b87
006e9b15: mov edx, dword ptr [rbx + 0x34]
006e9b18: xor r8d, r8d
006e9b1b: call 0x7ac710 ; Source.Controllers.EffectController.SetZombieLayerRow
006e9b20: mov edx, 0x2f
006e9b25: mov dword ptr [rbx + 0x88], edi
006e9b2b: mov r9d, 0x3b
006e9b31: mov qword ptr [rsp + 0x20], rdi
006e9b36: mov r8d, 0x4d
006e9b3c: mov rcx, rbx
006e9b3f: call 0x6e4c80 ; Reloaded.Gameplay.Zombie.AddAttachedParticle
006e9b44: movss xmm0, dword ptr [rip + 0x354a268]
006e9b4c: mov r9d, 0xa
006e9b52: mov dword ptr [rbx + 0xbc], 0x118
006e9b5c: mov r8d, 3
006e9b62: mov rdx, qword ptr [rip + 0x45a9937]
006e9b69: mov rcx, rbx
006e9b6c: mov qword ptr [rsp + 0x28], rdi
006e9b71: movss dword ptr [rsp + 0x20], xmm0
006e9b77: call 0x6f55d0 ; Reloaded.Gameplay.Zombie.PlayZombieReanim
006e9b7c: mov rbx, qword ptr [rsp + 0x40]
006e9b81: add rsp, 0x30
006e9b85: pop rdi
006e9b86: ret
006e9b87: call 0x4fee20 ;
006e9b8c: int3
006e9b8d: int3
006e9b8e: int3
006e9b8f: int3
