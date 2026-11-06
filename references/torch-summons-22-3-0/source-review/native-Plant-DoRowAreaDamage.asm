006ad520: mov qword ptr [rsp + 0x18], rbp
006ad525: mov qword ptr [rsp + 0x20], rsi
006ad52a: push r12
006ad52c: push r14
006ad52e: push r15
006ad530: sub rsp, 0x50
006ad534: cmp byte ptr [rip + 0x483e10c], 0
006ad53b: mov r14d, r8d
006ad53e: mov r15d, edx
006ad541: mov rsi, rcx
006ad544: jne 0x6ad57d
006ad546: lea rcx, [rip + 0x457da73]
006ad54d: call 0x4feb70 ;
006ad552: lea rcx, [rip + 0x45d3ef7]
006ad559: call 0x4feb70 ;
006ad55e: lea rcx, [rip + 0x45d3fc3]
006ad565: call 0x4feb70 ;
006ad56a: lea rcx, [rip + 0x45c7bf7]
006ad571: call 0x4feb70 ;
006ad576: mov byte ptr [rip + 0x483e0ca], 1
006ad57d: xorps xmm0, xmm0
006ad580: mov qword ptr [rsp + 0x70], rbx
006ad585: xorps xmm1, xmm1
006ad588: mov qword ptr [rsp + 0x78], rdi
006ad58d: xor r8d, r8d
006ad590: xor edx, edx
006ad592: mov rcx, rsi
006ad595: movups xmmword ptr [rsp + 0x30], xmm0
006ad59a: movups xmmword ptr [rsp + 0x20], xmm1
006ad59f: call 0x6b12f0 ; Reloaded.Gameplay.Plant.GetDamageRangeFlags
006ad5a4: xor r9d, r9d
006ad5a7: lea rcx, [rsp + 0x40]
006ad5ac: xor r8d, r8d
006ad5af: mov rdx, rsi
006ad5b2: mov r12d, eax
006ad5b5: call 0x6b1660 ; Reloaded.Gameplay.Plant.GetPlantAttackRect
006ad5ba: mov rcx, qword ptr [rsi + 0x18]
006ad5be: xor ebp, ebp
006ad5c0: xor edx, edx
006ad5c2: movups xmm0, xmmword ptr [rax]
006ad5c5: movups xmmword ptr [rsp + 0x30], xmm0
006ad5ca: test rcx, rcx
006ad5cd: je 0x6ad76f
006ad5d3: mov rax, qword ptr [rcx + 0x70]
006ad5d7: test rax, rax
006ad5da: je 0x6ad76f
006ad5e0: cmp edx, dword ptr [rax + 0x28]
006ad5e3: jge 0x6ad74e
006ad5e9: mov rax, qword ptr [rsi + 0x18]
006ad5ed: test rax, rax
006ad5f0: je 0x6ad76f
006ad5f6: mov rcx, qword ptr [rax + 0x70]
006ad5fa: test rcx, rcx
006ad5fd: je 0x6ad76f
006ad603: mov r8, qword ptr [rip + 0x45d3f1e]
006ad60a: mov edx, ebp
006ad60c: call 0x2449330 ;
006ad611: mov rdi, rax
006ad614: test rax, rax
006ad617: je 0x6ad76f
006ad61d: mov ecx, dword ptr [rax + 0x34]
006ad620: xor eax, eax
006ad622: sub ecx, dword ptr [rsi + 0x34]
006ad625: cmp dword ptr [rdi + 0x70], 0x19
006ad629: cmovne eax, ecx
006ad62c: cmp dword ptr [rsi + 0x70], 0x2a
006ad630: je 0x6ad63c
006ad632: test eax, eax
006ad634: jne 0x6ad73c
006ad63a: jmp 0x6ad647 ;
006ad63c: inc eax
006ad63e: cmp eax, 2
006ad641: ja 0x6ad73c
006ad647: movzx ebx, byte ptr [rdi + 0x116]
006ad64e: xor edx, edx
006ad650: mov rcx, rsi
006ad653: call 0x6b2290 ; Reloaded.Gameplay.Plant.IsOnHighGround
006ad658: cmp bl, al
006ad65a: jne 0x6ad73c
006ad660: xor r8d, r8d
006ad663: mov edx, r12d
006ad666: mov rcx, rdi
006ad669: call 0x6f12f0 ; Reloaded.Gameplay.Zombie.EffectedByDamage
006ad66e: test al, al
006ad670: je 0x6ad73c
006ad676: xor r8d, r8d
006ad679: lea rcx, [rsp + 0x40]
006ad67e: mov rdx, rdi
006ad681: call 0x6f2d70 ; Reloaded.Gameplay.Zombie.GetZombieRect
006ad686: mov rcx, qword ptr [rip + 0x457d933]
006ad68d: movups xmm0, xmmword ptr [rax]
006ad690: movups xmmword ptr [rsp + 0x20], xmm0
006ad695: cmp dword ptr [rcx + 0xe4], 0
006ad69c: jne 0x6ad6a3
006ad69e: call 0x4feeb0 ;
006ad6a3: xor r8d, r8d
006ad6a6: lea rdx, [rsp + 0x20]
006ad6ab: lea rcx, [rsp + 0x30]
006ad6b0: call 0x6cbaa0 ; Reloaded.Gameplay.Common.GetRectOverlap
006ad6b5: test eax, eax
006ad6b7: jle 0x6ad73c
006ad6bd: cmp dword ptr [rdi + 0x70], 0xc
006ad6c1: mov ebx, r15d
006ad6c4: je 0x6ad6cc
006ad6c6: cmp dword ptr [rdi + 0x70], 0x16
006ad6ca: jne 0x6ad703
006ad6cc: mov rcx, qword ptr [rip + 0x457d8ed]
006ad6d3: cmp dword ptr [rcx + 0xe4], 0
006ad6da: jne 0x6ad6e1
006ad6dc: call 0x4feeb0 ;
006ad6e1: test r14b, 0x20
006ad6e5: jbe 0x6ad703
006ad6e7: mov ebx, 0x708
006ad6ec: xor edx, edx
006ad6ee: mov rcx, rsi
006ad6f1: cmp dword ptr [rsi + 0x70], 0x2e
006ad6f5: je 0x6ad6fe
006ad6f7: call 0x6acd40 ; Reloaded.Gameplay.Plant.Die
006ad6fc: jmp 0x6ad703 ;
006ad6fe: call 0x6b5a90 ; Reloaded.Gameplay.Plant.SpikeRockTakeDamage
006ad703: xor r9d, r9d
006ad706: mov r8d, r14d
006ad709: mov edx, ebx
006ad70b: mov rcx, rdi
006ad70e: call 0x6f8f60 ; Reloaded.Gameplay.Zombie.TakeDamage
006ad713: mov r8, qword ptr [rsi + 0x10]
006ad717: test r8, r8
006ad71a: je 0x6ad76f
006ad71c: mov r8, qword ptr [r8 + 0x58]
006ad720: test r8, r8
006ad723: je 0x6ad76f
006ad725: mov rdx, qword ptr [rip + 0x45c7a3c]
006ad72c: mov ecx, 6
006ad731: mov r9d, 1
006ad737: call 0x2040 ;
006ad73c: mov rcx, qword ptr [rsi + 0x18]
006ad740: inc ebp
006ad742: mov edx, ebp
006ad744: test rcx, rcx
006ad747: je 0x6ad76f
006ad749: jmp 0x6ad5d3 ;
006ad74e: mov rdi, qword ptr [rsp + 0x78]
006ad753: lea r11, [rsp + 0x50]
006ad758: mov rbp, qword ptr [r11 + 0x30]
006ad75c: mov rsi, qword ptr [r11 + 0x38]
006ad760: mov rbx, qword ptr [rsp + 0x70]
006ad765: mov rsp, r11
006ad768: pop r15
006ad76a: pop r14
006ad76c: pop r12
006ad76e: ret
006ad76f: call 0x4fee20 ;
006ad774: int3
006ad775: int3
006ad776: int3
006ad777: int3
006ad778: int3
006ad779: int3
006ad77a: int3
006ad77b: int3
006ad77c: int3
006ad77d: int3
006ad77e: int3
006ad77f: int3
