006f8f60: mov qword ptr [rsp + 0x10], rbx
006f8f65: mov qword ptr [rsp + 0x18], rbp
006f8f6a: push rsi
006f8f6b: sub rsp, 0x20
006f8f6f: cmp byte ptr [rip + 0x47f282e], 0
006f8f76: mov esi, r8d
006f8f79: mov ebp, edx
006f8f7b: mov rbx, rcx
006f8f7e: jne 0x6f8f93
006f8f80: lea rcx, [rip + 0x4532039]
006f8f87: call 0x4feb70 ;
006f8f8c: mov byte ptr [rip + 0x47f2811], 1
006f8f93: mov rcx, qword ptr [rbx + 0x10]
006f8f97: test rcx, rcx
006f8f9a: je 0x6f914f
006f8fa0: xor edx, edx
006f8fa2: call 0x7df9f0 ; Reloaded.TreeStateActivities.GameplayActivity.IsVersusMode
006f8fa7: test al, al
006f8fa9: je 0x6f8fd3
006f8fab: mov rax, qword ptr [rbx + 0x10]
006f8faf: test rax, rax
006f8fb2: je 0x6f914f
006f8fb8: mov rax, qword ptr [rax + 0xd0]
006f8fbf: test rax, rax
006f8fc2: je 0x6f8fd3
006f8fc4: mov eax, dword ptr [rax + 0x44]
006f8fc7: sub eax, 5
006f8fca: cmp eax, 1
006f8fcd: jbe 0x6f9120
006f8fd3: cmp dword ptr [rbx + 0x74], 0x10
006f8fd7: je 0x6f9120
006f8fdd: cmp byte ptr [rbx + 0x144], 0
006f8fe4: jne 0x6f9120
006f8fea: cmp dword ptr [rbx + 0x74], 1
006f8fee: je 0x6f9120
006f8ff4: cmp dword ptr [rbx + 0x74], 2
006f8ff8: je 0x6f9120
006f8ffe: cmp dword ptr [rbx + 0x74], 3
006f9002: je 0x6f9120
006f9008: cmp dword ptr [rbx + 0x70], 0x22
006f900c: je 0x6f9130
006f9012: mov qword ptr [rsp + 0x30], rdi
006f9017: cmp dword ptr [rbx + 0x74], 0x49
006f901b: mov edi, ebp
006f901d: je 0x6f9025
006f901f: cmp dword ptr [rbx + 0x74], 0x4a
006f9023: jne 0x6f908e
006f9025: cmp byte ptr [rip + 0x47f27a5], 0
006f902c: jne 0x6f9041
006f902e: lea rcx, [rip + 0x4531f8b]
006f9035: call 0x4feb70 ;
006f903a: mov byte ptr [rip + 0x47f2790], 1
006f9041: mov rcx, qword ptr [rip + 0x4531f78]
006f9048: cmp dword ptr [rcx + 0xe4], 0
006f904f: jne 0x6f9056
006f9051: call 0x4feeb0 ;
006f9056: test sil, 8
006f905a: ja 0x6f9066
006f905c: mov dword ptr [rbx + 0xa8], 0x19
006f9066: mov eax, dword ptr [rbx + 0x13c]
006f906c: mov ecx, ebp
006f906e: cmp eax, ebp
006f9070: cmovl ecx, eax
006f9073: sub eax, ecx
006f9075: sub edi, ecx
006f9077: mov dword ptr [rbx + 0x13c], eax
006f907d: test eax, eax
006f907f: jne 0x6f908e
006f9081: xor r8d, r8d
006f9084: mov edx, esi
006f9086: mov rcx, rbx
006f9089: call 0x6f3aa0 ; Reloaded.Gameplay.Zombie.LandFlyer
006f908e: test edi, edi
006f9090: jle 0x6f911b
006f9096: cmp dword ptr [rbx + 0x130], 0
006f909d: je 0x6f90ec
006f909f: mov rcx, qword ptr [rip + 0x4531f1a]
006f90a6: cmp dword ptr [rcx + 0xe4], 0
006f90ad: jne 0x6f90b4
006f90af: call 0x4feeb0 ;
006f90b4: test sil, 1
006f90b8: ja 0x6f90ec
006f90ba: xor r9d, r9d
006f90bd: mov r8d, esi
006f90c0: mov edx, ebp
006f90c2: mov rcx, rbx
006f90c5: call 0x6f97f0 ; Reloaded.Gameplay.Zombie.TakeShieldDamage
006f90ca: mov rcx, qword ptr [rip + 0x4531eef]
006f90d1: mov edi, eax
006f90d3: cmp dword ptr [rcx + 0xe4], 0
006f90da: jne 0x6f90e1
006f90dc: call 0x4feeb0 ;
006f90e1: test sil, 2
006f90e5: cmova edi, ebp
006f90e8: test edi, edi
006f90ea: jle 0x6f911b
006f90ec: cmp dword ptr [rbx + 0x11c], 0
006f90f3: je 0x6f910b
006f90f5: xor r9d, r9d
006f90f8: mov r8d, esi
006f90fb: mov edx, ebp
006f90fd: mov rcx, rbx
006f9100: call 0x6f9200 ; Reloaded.Gameplay.Zombie.TakeHelmDamage
006f9105: mov edi, eax
006f9107: test eax, eax
006f9109: jle 0x6f911b
006f910b: xor r9d, r9d
006f910e: mov r8d, esi
006f9111: mov edx, edi
006f9113: mov rcx, rbx
006f9116: call 0x6f8340 ; Reloaded.Gameplay.Zombie.TakeBodyDamage
006f911b: mov rdi, qword ptr [rsp + 0x30]
006f9120: mov rbx, qword ptr [rsp + 0x38]
006f9125: mov rbp, qword ptr [rsp + 0x40]
006f912a: add rsp, 0x20
006f912e: pop rsi
006f912f: ret
006f9130: xor r9d, r9d
006f9133: mov r8d, esi
006f9136: mov edx, ebp
006f9138: mov rcx, rbx
006f913b: mov rbx, qword ptr [rsp + 0x38]
006f9140: mov rbp, qword ptr [rsp + 0x40]
006f9145: add rsp, 0x20
006f9149: pop rsi
006f914a: jmp 0x6f97f0 ; Reloaded.Gameplay.Zombie.TakeShieldDamage
006f914f: call 0x4fee20 ;
006f9154: int3
006f9155: int3
006f9156: int3
006f9157: int3
006f9158: int3
006f9159: int3
006f915a: int3
006f915b: int3
006f915c: int3
006f915d: int3
006f915e: int3
006f915f: int3
