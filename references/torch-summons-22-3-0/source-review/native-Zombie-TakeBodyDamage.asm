006f8340: mov qword ptr [rsp + 8], rbx
006f8345: mov qword ptr [rsp + 0x10], rbp
006f834a: mov qword ptr [rsp + 0x18], rsi
006f834f: mov qword ptr [rsp + 0x20], rdi
006f8354: push r12
006f8356: push r14
006f8358: push r15
006f835a: sub rsp, 0x30
006f835e: cmp byte ptr [rip + 0x47f346f], 0
006f8365: mov esi, r8d
006f8368: mov edi, edx
006f836a: mov rbx, rcx
006f836d: jne 0x6f8572
006f8373: lea rcx, [rip + 0x4532c46]
006f837a: call 0x4feb70 ;
006f837f: lea rcx, [rip + 0x457cde2]
006f8386: call 0x4feb70 ;
006f838b: lea rcx, [rip + 0x45449d6]
006f8392: call 0x4feb70 ;
006f8397: lea rcx, [rip + 0x458356a]
006f839e: call 0x4feb70 ;
006f83a3: lea rcx, [rip + 0x4567026]
006f83aa: call 0x4feb70 ;
006f83af: lea rcx, [rip + 0x45838b2]
006f83b6: call 0x4feb70 ;
006f83bb: lea rcx, [rip + 0x453a8d6]
006f83c2: call 0x4feb70 ;
006f83c7: lea rcx, [rip + 0x456a392]
006f83ce: call 0x4feb70 ;
006f83d3: lea rcx, [rip + 0x45669f6]
006f83da: call 0x4feb70 ;
006f83df: lea rcx, [rip + 0x456a102]
006f83e6: call 0x4feb70 ;
006f83eb: lea rcx, [rip + 0x45835ee]
006f83f2: call 0x4feb70 ;
006f83f7: lea rcx, [rip + 0x45836ba]
006f83fe: call 0x4feb70 ;
006f8403: lea rcx, [rip + 0x456a286]
006f840a: call 0x4feb70 ;
006f840f: lea rcx, [rip + 0x453a462]
006f8416: call 0x4feb70 ;
006f841b: lea rcx, [rip + 0x453a526]
006f8422: call 0x4feb70 ;
006f8427: lea rcx, [rip + 0x456863a]
006f842e: call 0x4feb70 ;
006f8433: lea rcx, [rip + 0x453a6b6]
006f843a: call 0x4feb70 ;
006f843f: lea rcx, [rip + 0x4566eb2]
006f8446: call 0x4feb70 ;
006f844b: lea rcx, [rip + 0x4539ffe]
006f8452: call 0x4feb70 ;
006f8457: lea rcx, [rip + 0x4541ba2]
006f845e: call 0x4feb70 ;
006f8463: lea rcx, [rip + 0x4566dae]
006f846a: call 0x4feb70 ;
006f846f: lea rcx, [rip + 0x4568022]
006f8476: call 0x4feb70 ;
006f847b: lea rcx, [rip + 0x4544d2e]
006f8482: call 0x4feb70 ;
006f8487: lea rcx, [rip + 0x456835a]
006f848e: call 0x4feb70 ;
006f8493: lea rcx, [rip + 0x453a586]
006f849a: call 0x4feb70 ;
006f849f: lea rcx, [rip + 0x4542972]
006f84a6: call 0x4feb70 ;
006f84ab: lea rcx, [rip + 0x45669f6]
006f84b2: call 0x4feb70 ;
006f84b7: lea rcx, [rip + 0x45680b2]
006f84be: call 0x4feb70 ;
006f84c3: lea rcx, [rip + 0x4566c76]
006f84ca: call 0x4feb70 ;
006f84cf: lea rcx, [rip + 0x4566ab2]
006f84d6: call 0x4feb70 ;
006f84db: lea rcx, [rip + 0x4566fce]
006f84e2: call 0x4feb70 ;
006f84e7: lea rcx, [rip + 0x45675ba]
006f84ee: call 0x4feb70 ;
006f84f3: lea rcx, [rip + 0x459cfb6]
006f84fa: call 0x4feb70 ;
006f84ff: lea rcx, [rip + 0x4566b5a]
006f8506: call 0x4feb70 ;
006f850b: lea rcx, [rip + 0x456a0a6]
006f8512: call 0x4feb70 ;
006f8517: lea rcx, [rip + 0x45683a2]
006f851e: call 0x4feb70 ;
006f8523: lea rcx, [rip + 0x45681ee]
006f852a: call 0x4feb70 ;
006f852f: lea rcx, [rip + 0x4542a8a]
006f8536: call 0x4feb70 ;
006f853b: lea rcx, [rip + 0x45429a6]
006f8542: call 0x4feb70 ;
006f8547: lea rcx, [rip + 0x45667a2]
006f854e: call 0x4feb70 ;
006f8553: lea rcx, [rip + 0x4583636]
006f855a: call 0x4feb70 ;
006f855f: lea rcx, [rip + 0x4544b7a]
006f8566: call 0x4feb70 ;
006f856b: mov byte ptr [rip + 0x47f3262], 1
006f8572: mov rcx, qword ptr [rip + 0x4532a47]
006f8579: cmp dword ptr [rcx + 0xe4], 0
006f8580: jne 0x6f8587
006f8582: call 0x4feeb0 ;
006f8587: test sil, 8
006f858b: ja 0x6f8597
006f858d: mov dword ptr [rbx + 0xa8], 0x19
006f8597: mov rcx, qword ptr [rip + 0x4532a22]
006f859e: cmp dword ptr [rcx + 0xe4], 0
006f85a5: jne 0x6f85ac
006f85a7: call 0x4feeb0 ;
006f85ac: test sil, 4
006f85b0: jbe 0x6f85bf
006f85b2: xor r8d, r8d
006f85b5: xor edx, edx
006f85b7: mov rcx, rbx
006f85ba: call 0x6e6430 ; Reloaded.Gameplay.Zombie.ApplyChill
006f85bf: mov r14d, dword ptr [rbx + 0x120]
006f85c6: xor edx, edx
006f85c8: mov rcx, rbx
006f85cb: call 0x6f2020 ; Reloaded.Gameplay.Zombie.GetBodyDamageIndex
006f85d0: mov ebp, r14d
006f85d3: xor edx, edx
006f85d5: sub ebp, edi
006f85d7: mov rcx, rbx
006f85da: mov dword ptr [rbx + 0x120], ebp
006f85e0: mov r15d, eax
006f85e3: call 0x6f2020 ; Reloaded.Gameplay.Zombie.GetBodyDamageIndex
006f85e8: xor r12d, r12d
006f85eb: mov edi, eax
006f85ed: cmp dword ptr [rbx + 0x70], 0xc
006f85f1: je 0x6f8d68
006f85f7: cmp dword ptr [rbx + 0x70], 0x16
006f85fb: je 0x6f8ca7
006f8601: cmp dword ptr [rbx + 0x70], 0x17
006f8605: je 0x6f8b87
006f860b: cmp dword ptr [rbx + 0x70], 0x20
006f860f: je 0x6f8b87
006f8615: cmp dword ptr [rbx + 0x70], 0x19
006f8619: je 0x6f88c8
006f861f: cmp dword ptr [rbx + 0x70], 0x24
006f8623: je 0x6f872e
006f8629: cmp dword ptr [rbx + 0x70], 0x14
006f862d: je 0x6f8f09
006f8633: cmp dword ptr [rbx + 0xb8], 0xa
006f863a: je 0x6f8f09
006f8640: cmp dword ptr [rbx + 0x74], 0x49
006f8644: je 0x6f8f09
006f864a: cmp dword ptr [rbx + 0x74], 0x4a
006f864e: je 0x6f8f09
006f8654: xor edx, edx
006f8656: mov rcx, rbx
006f8659: call 0x6f1f20 ; Reloaded.Gameplay.Zombie.GetBobsledPosition
006f865e: cmp eax, -1
006f8661: jne 0x6f8f09
006f8667: cmp byte ptr [rbx + 0x113], r12b
006f866e: je 0x6f86a4
006f8670: mov ecx, dword ptr [rbx + 0x124]
006f8676: mov eax, 0x55555556
006f867b: add ecx, ecx
006f867d: imul ecx
006f867f: mov eax, edx
006f8681: shr eax, 0x1f
006f8684: add edx, eax
006f8686: cmp dword ptr [rbx + 0x120], edx
006f868c: jge 0x6f86a4
006f868e: cmp dword ptr [rbx + 0x120], r12d
006f8695: jle 0x6f86a4
006f8697: xor r8d, r8d
006f869a: mov edx, esi
006f869c: mov rcx, rbx
006f869f: call 0x6ee6a0 ; Reloaded.Gameplay.Zombie.DropArm
006f86a4: cmp byte ptr [rbx + 0x112], r12b
006f86ab: je 0x6f8f09
006f86b1: mov eax, 0x55555556
006f86b6: imul dword ptr [rbx + 0x124]
006f86bc: mov eax, edx
006f86be: shr eax, 0x1f
006f86c1: add edx, eax
006f86c3: cmp dword ptr [rbx + 0x120], edx
006f86c9: jge 0x6f8f09
006f86cf: xor r8d, r8d
006f86d2: mov edx, esi
006f86d4: mov rcx, rbx
006f86d7: call 0x6ef6a0 ; Reloaded.Gameplay.Zombie.DropHead
006f86dc: xor edx, edx
006f86de: mov rcx, rbx
006f86e1: call 0x6f0500 ; Reloaded.Gameplay.Zombie.DropLoot
006f86e6: xor edx, edx
006f86e8: mov rcx, rbx
006f86eb: call 0x6f7dd0 ; Reloaded.Gameplay.Zombie.StopZombieSound
006f86f0: mov rcx, qword ptr [rbx + 0x18]
006f86f4: test rcx, rcx
006f86f7: je 0x6f8f4f
006f86fd: xor edx, edx
006f86ff: call 0x65f8a0 ; Reloaded.Gameplay.Board.HasLevelAwardDropped
006f8704: test al, al
006f8706: je 0x6f8715
006f8708: xor r8d, r8d
006f870b: mov edx, esi
006f870d: mov rcx, rbx
006f8710: call 0x6f4e90 ; Reloaded.Gameplay.Zombie.PlayDeathAnim
006f8715: cmp dword ptr [rbx + 0x74], 0x3b
006f8719: jne 0x6f8f09
006f871f: xor edx, edx
006f8721: mov rcx, rbx
006f8724: call 0x6ebf90 ; Reloaded.Gameplay.Zombie.DieNoLoot
006f8729: jmp 0x6f8f09 ;
006f872e: mov rax, qword ptr [rbx + 0x1d0]
006f8735: test rax, rax
006f8738: je 0x6f8f4f
006f873e: mov rcx, qword ptr [rax + 0x158]
006f8745: test rcx, rcx
006f8748: je 0x6f8f4f
006f874e: movss xmm3, dword ptr [rcx + 0x40]
006f8753: cmp r14d, 0x50
006f8757: jle 0x6f878b
006f8759: cmp ebp, 0x50
006f875c: jg 0x6f878b
006f875e: mov rcx, rax
006f8761: test rax, rax
006f8764: je 0x6f8f4f
006f876a: mov rdx, qword ptr [rip + 0x45834f7]
006f8771: xor r8d, r8d
006f8774: mov qword ptr [rsp + 0x28], r12
006f8779: mov dword ptr [rsp + 0x20], 3
006f8781: call 0x7c8270 ; Source.Controllers.ReloadedCharacterController.PlayAnimationOnTrack
006f8786: jmp 0x6f888a ;
006f878b: cmp r14d, 0xa0
006f8792: jle 0x6f87cd
006f8794: cmp ebp, 0xa0
006f879a: jg 0x6f87cd
006f879c: mov rcx, qword ptr [rbx + 0x1d0]
006f87a3: test rcx, rcx
006f87a6: je 0x6f8f4f
006f87ac: mov rdx, qword ptr [rip + 0x45833dd]
006f87b3: xor r8d, r8d
006f87b6: mov qword ptr [rsp + 0x28], r12
006f87bb: mov dword ptr [rsp + 0x20], 3
006f87c3: call 0x7c8270 ; Source.Controllers.ReloadedCharacterController.PlayAnimationOnTrack
006f87c8: jmp 0x6f888a ;
006f87cd: cmp r14d, 0xc8
006f87d4: jle 0x6f880c
006f87d6: cmp ebp, 0xc8
006f87dc: jg 0x6f880c
006f87de: mov rcx, qword ptr [rbx + 0x1d0]
006f87e5: test rcx, rcx
006f87e8: je 0x6f8f4f
006f87ee: mov rdx, qword ptr [rip + 0x45832c3]
006f87f5: xor r8d, r8d
006f87f8: mov qword ptr [rsp + 0x28], r12
006f87fd: mov dword ptr [rsp + 0x20], 3
006f8805: call 0x7c8270 ; Source.Controllers.ReloadedCharacterController.PlayAnimationOnTrack
006f880a: jmp 0x6f888a ;
006f880c: cmp r14d, 0xf0
006f8813: jle 0x6f884b
006f8815: cmp ebp, 0xf0
006f881b: jg 0x6f884b
006f881d: mov rcx, qword ptr [rbx + 0x1d0]
006f8824: test rcx, rcx
006f8827: je 0x6f8f4f
006f882d: mov rdx, qword ptr [rip + 0x45831ac]
006f8834: xor r8d, r8d
006f8837: mov qword ptr [rsp + 0x28], r12
006f883c: mov dword ptr [rsp + 0x20], 3
006f8844: call 0x7c8270 ; Source.Controllers.ReloadedCharacterController.PlayAnimationOnTrack
006f8849: jmp 0x6f888a ;
006f884b: cmp r14d, 0x12c
006f8852: jle 0x6f8881
006f8854: cmp ebp, 0x12c
006f885a: jg 0x6f888a
006f885c: mov rdx, qword ptr [rip + 0x45830a5]
006f8863: xor r8d, r8d
006f8866: mov rcx, qword ptr [rbx + 0x1d0]
006f886d: mov qword ptr [rsp + 0x28], r12
006f8872: mov dword ptr [rsp + 0x20], 3
006f887a: call 0x7c8270 ; Source.Controllers.ReloadedCharacterController.PlayAnimationOnTrack
006f887f: jmp 0x6f888a ;
006f8881: test r14d, r14d
006f8884: jle 0x6f8f09
006f888a: cmp dword ptr [rbx + 0x120], r12d
006f8891: jg 0x6f8f30
006f8897: mov rax, qword ptr [rbx + 0x18]
006f889b: test rax, rax
006f889e: je 0x6f8f4f
006f88a4: mov rcx, qword ptr [rax + 0xb0]
006f88ab: test rcx, rcx
006f88ae: je 0x6f8f4f
006f88b4: mov r8, qword ptr [rip + 0x45444ad]
006f88bb: mov rdx, rbx
006f88be: call 0x18dc5a0 ;
006f88c3: jmp 0x6f8f09 ;
006f88c8: mov rcx, qword ptr [rip + 0x45326f1]
006f88cf: cmp dword ptr [rcx + 0xe4], r12d
006f88d6: jne 0x6f88dd
006f88d8: call 0x4feeb0 ;
006f88dd: test sil, 8
006f88e1: ja 0x6f8914
006f88e3: mov rax, qword ptr [rbx + 0x10]
006f88e7: test rax, rax
006f88ea: je 0x6f8f4f
006f88f0: mov r8, qword ptr [rax + 0x58]
006f88f4: test r8, r8
006f88f7: je 0x6f8f4f
006f88fd: mov rdx, qword ptr [rip + 0x457c864]
006f8904: mov ecx, 6
006f8909: mov r9d, 0x2e
006f890f: call 0x2040 ;
006f8914: cmp r15d, edi
006f8917: je 0x6f8abd
006f891d: cmp edi, 1
006f8920: je 0x6f89ff
006f8926: cmp edi, 2
006f8929: jne 0x6f8abd
006f892f: mov rcx, qword ptr [rbx + 0x1d0]
006f8936: test rcx, rcx
006f8939: je 0x6f8f4f
006f893f: mov r8, qword ptr [rip + 0x4566642]
006f8946: xor r9d, r9d
006f8949: mov rdx, qword ptr [rip + 0x4539b00]
006f8950: call 0x7cae90 ; Source.Controllers.ReloadedController.SetImageOverride
006f8955: mov rcx, qword ptr [rbx + 0x1d0]
006f895c: test rcx, rcx
006f895f: je 0x6f8f4f
006f8965: mov r8, qword ptr [rip + 0x45667d4]
006f896c: xor r9d, r9d
006f896f: mov rdx, qword ptr [rip + 0x4539fd2]
006f8976: call 0x7cae90 ; Source.Controllers.ReloadedController.SetImageOverride
006f897b: mov rcx, qword ptr [rbx + 0x1d0]
006f8982: test rcx, rcx
006f8985: je 0x6f8f4f
006f898b: mov r8, qword ptr [rip + 0x4566966]
006f8992: xor r9d, r9d
006f8995: mov rdx, qword ptr [rip + 0x453a084]
006f899c: call 0x7cae90 ; Source.Controllers.ReloadedController.SetImageOverride
006f89a1: mov rcx, qword ptr [rbx + 0x1d0]
006f89a8: test rcx, rcx
006f89ab: je 0x6f8f4f
006f89b1: mov r8, qword ptr [rip + 0x4566af8]
006f89b8: xor r9d, r9d
006f89bb: mov rdx, qword ptr [rip + 0x453a12e]
006f89c2: call 0x7cae90 ; Source.Controllers.ReloadedController.SetImageOverride
006f89c7: mov rcx, qword ptr [rbx + 0x1d0]
006f89ce: test rcx, rcx
006f89d1: je 0x6f8f4f
006f89d7: mov r8, qword ptr [rip + 0x45663f2]
006f89de: xor r9d, r9d
006f89e1: mov rdx, qword ptr [rip + 0x453a2b0]
006f89e8: call 0x7cae90 ; Source.Controllers.ReloadedController.SetImageOverride
006f89ed: xor r8d, r8d
006f89f0: mov dl, 1
006f89f2: mov rcx, rbx
006f89f5: call 0x6e5570 ; Reloaded.Gameplay.Zombie.ApplyBossSmokeParticles
006f89fa: jmp 0x6f8abd ;
006f89ff: mov rcx, qword ptr [rbx + 0x1d0]
006f8a06: test rcx, rcx
006f8a09: je 0x6f8f4f
006f8a0f: mov r8, qword ptr [rip + 0x4566492]
006f8a16: xor r9d, r9d
006f8a19: mov rdx, qword ptr [rip + 0x4539a30]
006f8a20: call 0x7cae90 ; Source.Controllers.ReloadedController.SetImageOverride
006f8a25: mov rcx, qword ptr [rbx + 0x1d0]
006f8a2c: test rcx, rcx
006f8a2f: je 0x6f8f4f
006f8a35: mov r8, qword ptr [rip + 0x4566624]
006f8a3c: xor r9d, r9d
006f8a3f: mov rdx, qword ptr [rip + 0x4539f02]
006f8a46: call 0x7cae90 ; Source.Controllers.ReloadedController.SetImageOverride
006f8a4b: mov rcx, qword ptr [rbx + 0x1d0]
006f8a52: test rcx, rcx
006f8a55: je 0x6f8f4f
006f8a5b: mov r8, qword ptr [rip + 0x45667b6]
006f8a62: xor r9d, r9d
006f8a65: mov rdx, qword ptr [rip + 0x4539fb4]
006f8a6c: call 0x7cae90 ; Source.Controllers.ReloadedController.SetImageOverride
006f8a71: mov rcx, qword ptr [rbx + 0x1d0]
006f8a78: test rcx, rcx
006f8a7b: je 0x6f8f4f
006f8a81: mov r8, qword ptr [rip + 0x4566948]
006f8a88: xor r9d, r9d
006f8a8b: mov rdx, qword ptr [rip + 0x453a05e]
006f8a92: call 0x7cae90 ; Source.Controllers.ReloadedController.SetImageOverride
006f8a97: mov rcx, qword ptr [rbx + 0x1d0]
006f8a9e: test rcx, rcx
006f8aa1: je 0x6f8f4f
006f8aa7: mov r8, qword ptr [rip + 0x4566242]
006f8aae: xor r9d, r9d
006f8ab1: mov rdx, qword ptr [rip + 0x4539dc0]
006f8ab8: call 0x7cae90 ; Source.Controllers.ReloadedController.SetImageOverride
006f8abd: mov eax, 0x66666667
006f8ac2: imul dword ptr [rbx + 0x124]
006f8ac8: sar edx, 2
006f8acb: mov ecx, edx
006f8acd: shr ecx, 0x1f
006f8ad0: add edx, ecx
006f8ad2: cmp r14d, edx
006f8ad5: jl 0x6f8b6b
006f8adb: mov eax, 0x66666667
006f8ae0: imul dword ptr [rbx + 0x124]
006f8ae6: sar edx, 2
006f8ae9: mov eax, edx
006f8aeb: shr eax, 0x1f
006f8aee: add edx, eax
006f8af0: cmp dword ptr [rbx + 0x120], edx
006f8af6: jge 0x6f8b6b
006f8af8: mov rcx, qword ptr [rbx + 0x10]
006f8afc: test rcx, rcx
006f8aff: je 0x6f8f4f
006f8b05: movss xmm2, dword ptr [rip + 0x353b6ff]
006f8b0d: mov r9d, 0x61a80
006f8b13: movss xmm1, dword ptr [rip + 0x353b709]
006f8b1b: mov qword ptr [rsp + 0x28], r12
006f8b20: mov dword ptr [rsp + 0x20], 0x50
006f8b28: call 0x7d9b50 ; Reloaded.TreeStateActivities.GameplayActivity.AddTodParticle
006f8b2d: mov r8, qword ptr [rbx + 0x10]
006f8b31: test r8, r8
006f8b34: je 0x6f8f4f
006f8b3a: mov r8, qword ptr [r8 + 0x58]
006f8b3e: test r8, r8
006f8b41: je 0x6f8f4f
006f8b47: mov rdx, qword ptr [rip + 0x457c61a]
006f8b4e: mov ecx, 6
006f8b53: mov r9d, 0x2d
006f8b59: call 0x2040 ;
006f8b5e: xor r8d, r8d
006f8b61: mov dl, 1
006f8b63: mov rcx, rbx
006f8b66: call 0x6e5570 ; Reloaded.Gameplay.Zombie.ApplyBossSmokeParticles
006f8b6b: cmp dword ptr [rbx + 0x120], r12d
006f8b72: jg 0x6f8f30
006f8b78: mov dword ptr [rbx + 0x120], 1
006f8b82: jmp 0x6f8f09 ;
006f8b87: cmp r15d, edi
006f8b8a: je 0x6f8f09
006f8b90: cmp edi, 1
006f8b93: je 0x6f8c56
006f8b99: cmp edi, 2
006f8b9c: jne 0x6f8f09
006f8ba2: mov rcx, qword ptr [rbx + 0x1d0]
006f8ba9: test rcx, rcx
006f8bac: je 0x6f8f4f
006f8bb2: mov r8, qword ptr [rip + 0x45679b7]
006f8bb9: xor r9d, r9d
006f8bbc: mov rdx, qword ptr [rip + 0x4542255]
006f8bc3: call 0x7cae90 ; Source.Controllers.ReloadedController.SetImageOverride
006f8bc8: mov rcx, qword ptr [rbx + 0x1d0]
006f8bcf: test rcx, rcx
006f8bd2: je 0x6f8f4f
006f8bd8: mov r8, qword ptr [rip + 0x4567b39]
006f8bdf: xor r9d, r9d
006f8be2: mov rdx, qword ptr [rip + 0x45423d7]
006f8be9: call 0x7cae90 ; Source.Controllers.ReloadedController.SetImageOverride
006f8bee: mov rcx, qword ptr [rbx + 0x1d0]
006f8bf5: test rcx, rcx
006f8bf8: je 0x6f8f4f
006f8bfe: mov r8, qword ptr [rip + 0x4567e63]
006f8c05: xor r9d, r9d
006f8c08: mov rdx, qword ptr [rip + 0x45422d9]
006f8c0f: call 0x7cae90 ; Source.Controllers.ReloadedController.SetImageOverride
006f8c14: mov rcx, qword ptr [rbx + 0x1d0]
006f8c1b: test rcx, rcx
006f8c1e: je 0x6f8f4f
006f8c24: mov rdx, qword ptr [rip + 0x459c885]
006f8c2b: xor r9d, r9d
006f8c2e: cmp dword ptr [rbx + 0x70], 0x20
006f8c32: je 0x6f8c45
006f8c34: mov r8, qword ptr [rip + 0x4567bad]
006f8c3b: call 0x7cae90 ; Source.Controllers.ReloadedController.SetImageOverride
006f8c40: jmp 0x6f8f09 ;
006f8c45: mov r8, qword ptr [rip + 0x4567c74]
006f8c4c: call 0x7cae90 ; Source.Controllers.ReloadedController.SetImageOverride
006f8c51: jmp 0x6f8f09 ;
006f8c56: mov rcx, qword ptr [rbx + 0x1d0]
006f8c5d: test rcx, rcx
006f8c60: je 0x6f8f4f
006f8c66: mov r8, qword ptr [rip + 0x456782b]
006f8c6d: xor r9d, r9d
006f8c70: mov rdx, qword ptr [rip + 0x45421a1]
006f8c77: call 0x7cae90 ; Source.Controllers.ReloadedController.SetImageOverride
006f8c7c: mov rcx, qword ptr [rbx + 0x1d0]
006f8c83: test rcx, rcx
006f8c86: je 0x6f8f4f
006f8c8c: mov r8, qword ptr [rip + 0x4567dd5]
006f8c93: xor r9d, r9d
006f8c96: mov rdx, qword ptr [rip + 0x454224b]
006f8c9d: call 0x7cae90 ; Source.Controllers.ReloadedController.SetImageOverride
006f8ca2: jmp 0x6f8f09 ;
006f8ca7: mov rcx, qword ptr [rip + 0x4532312]
006f8cae: cmp dword ptr [rcx + 0xe4], r12d
006f8cb5: jne 0x6f8cbc
006f8cb7: call 0x4feeb0 ;
006f8cbc: test sil, 0x20
006f8cc0: ja 0x6f8d30
006f8cc2: cmp dword ptr [rbx + 0x120], r12d
006f8cc9: jle 0x6f8d30
006f8ccb: cmp r15d, edi
006f8cce: je 0x6f8f09
006f8cd4: cmp edi, 1
006f8cd7: je 0x6f8d05
006f8cd9: cmp edi, 2
006f8cdc: jne 0x6f8f09
006f8ce2: mov edx, 0x2f
006f8ce7: mov qword ptr [rsp + 0x20], r12
006f8cec: mov r9d, 0x3b
006f8cf2: mov r8d, 0x4d
006f8cf8: mov rcx, rbx
006f8cfb: call 0x6e4c80 ; Reloaded.Gameplay.Zombie.AddAttachedParticle
006f8d00: jmp 0x6f8f09 ;
006f8d05: mov rcx, qword ptr [rbx + 0x1d0]
006f8d0c: test rcx, rcx
006f8d0f: je 0x6f8f4f
006f8d15: mov r8, qword ptr [rip + 0x4566d8c]
006f8d1c: xor r9d, r9d
006f8d1f: mov rdx, qword ptr [rip + 0x45412da]
006f8d26: call 0x7cae90 ; Source.Controllers.ReloadedController.SetImageOverride
006f8d2b: jmp 0x6f8f09 ;
006f8d30: mov rcx, qword ptr [rbx + 0x1d0]
006f8d37: test rcx, rcx
006f8d3a: je 0x6f8f4f
006f8d40: mov r8, qword ptr [rip + 0x4566d61]
006f8d47: xor r9d, r9d
006f8d4a: mov rdx, qword ptr [rip + 0x45412af]
006f8d51: call 0x7cae90 ; Source.Controllers.ReloadedController.SetImageOverride
006f8d56: xor r8d, r8d
006f8d59: mov edx, esi
006f8d5b: mov rcx, rbx
006f8d5e: call 0x6e99a0 ; Reloaded.Gameplay.Zombie.CatapultDeath
006f8d63: jmp 0x6f8f09 ;
006f8d68: mov rcx, qword ptr [rip + 0x4532251]
006f8d6f: cmp dword ptr [rcx + 0xe4], r12d
006f8d76: jne 0x6f8d7d
006f8d78: call 0x4feeb0 ;
006f8d7d: test sil, 8
006f8d81: ja 0x6f8db4
006f8d83: mov rax, qword ptr [rbx + 0x10]
006f8d87: test rax, rax
006f8d8a: je 0x6f8f4f
006f8d90: mov r8, qword ptr [rax + 0x58]
006f8d94: test r8, r8
006f8d97: je 0x6f8f4f
006f8d9d: mov rdx, qword ptr [rip + 0x457c3c4]
006f8da4: mov ecx, 6
006f8da9: mov r9d, 0x2e
006f8daf: call 0x2040 ;
006f8db4: mov rcx, qword ptr [rip + 0x4532205]
006f8dbb: cmp dword ptr [rcx + 0xe4], r12d
006f8dc2: jne 0x6f8dc9
006f8dc4: call 0x4feeb0 ;
006f8dc9: test sil, 0x20
006f8dcd: ja 0x6f8eb4
006f8dd3: cmp dword ptr [rbx + 0x120], r12d
006f8dda: jle 0x6f8efc
006f8de0: cmp r15d, edi
006f8de3: je 0x6f8f09
006f8de9: cmp edi, 1
006f8dec: je 0x6f8e66
006f8dee: cmp edi, 2
006f8df1: jne 0x6f8f09
006f8df7: mov rcx, qword ptr [rbx + 0x1d0]
006f8dfe: test rcx, rcx
006f8e01: je 0x6f8f4f
006f8e07: mov r8, qword ptr [rip + 0x45697aa]
006f8e0e: xor r9d, r9d
006f8e11: mov rdx, qword ptr [rip + 0x45442c8]
006f8e18: call 0x7cae90 ; Source.Controllers.ReloadedController.SetImageOverride
006f8e1d: mov rcx, qword ptr [rbx + 0x1d0]
006f8e24: test rcx, rcx
006f8e27: je 0x6f8f4f
006f8e2d: mov r8, qword ptr [rip + 0x456992c]
006f8e34: xor r9d, r9d
006f8e37: mov rdx, qword ptr [rip + 0x4544372]
006f8e3e: call 0x7cae90 ; Source.Controllers.ReloadedController.SetImageOverride
006f8e43: mov edx, 0x1b
006f8e48: mov qword ptr [rsp + 0x20], r12
006f8e4d: mov r9d, 0x3b
006f8e53: mov r8d, 0x48
006f8e59: mov rcx, rbx
006f8e5c: call 0x6e4c80 ; Reloaded.Gameplay.Zombie.AddAttachedParticle
006f8e61: jmp 0x6f8f09 ;
006f8e66: mov rcx, qword ptr [rbx + 0x1d0]
006f8e6d: test rcx, rcx
006f8e70: je 0x6f8f4f
006f8e76: mov r8, qword ptr [rip + 0x456966b]
006f8e7d: xor r9d, r9d
006f8e80: mov rdx, qword ptr [rip + 0x4544259]
006f8e87: call 0x7cae90 ; Source.Controllers.ReloadedController.SetImageOverride
006f8e8c: mov rcx, qword ptr [rbx + 0x1d0]
006f8e93: test rcx, rcx
006f8e96: je 0x6f8f4f
006f8e9c: mov r8, qword ptr [rip + 0x45697ed]
006f8ea3: xor r9d, r9d
006f8ea6: mov rdx, qword ptr [rip + 0x4544303]
006f8ead: call 0x7cae90 ; Source.Controllers.ReloadedController.SetImageOverride
006f8eb2: jmp 0x6f8f09 ;
006f8eb4: mov rcx, qword ptr [rbx + 0x1d0]
006f8ebb: test rcx, rcx
006f8ebe: je 0x6f8f4f
006f8ec4: mov r8, qword ptr [rip + 0x45696ed]
006f8ecb: xor r9d, r9d
006f8ece: mov rdx, qword ptr [rip + 0x454420b]
006f8ed5: call 0x7cae90 ; Source.Controllers.ReloadedController.SetImageOverride
006f8eda: mov rcx, qword ptr [rbx + 0x1d0]
006f8ee1: test rcx, rcx
006f8ee4: je 0x6f8f4f
006f8ee6: mov r8, qword ptr [rip + 0x4569873]
006f8eed: xor r9d, r9d
006f8ef0: mov rdx, qword ptr [rip + 0x45442b9]
006f8ef7: call 0x7cae90 ; Source.Controllers.ReloadedController.SetImageOverride
006f8efc: xor r8d, r8d
006f8eff: mov edx, esi
006f8f01: mov rcx, rbx
006f8f04: call 0x706b00 ; Reloaded.Gameplay.Zombie.ZamboniDeath
006f8f09: cmp dword ptr [rbx + 0x120], r12d
006f8f10: jg 0x6f8f30
006f8f12: xor r8d, r8d
006f8f15: mov dword ptr [rbx + 0x120], r12d
006f8f1c: mov edx, esi
006f8f1e: mov rcx, rbx
006f8f21: call 0x6f4e90 ; Reloaded.Gameplay.Zombie.PlayDeathAnim
006f8f26: xor edx, edx
006f8f28: mov rcx, rbx
006f8f2b: call 0x6f0500 ; Reloaded.Gameplay.Zombie.DropLoot
006f8f30: mov rbx, qword ptr [rsp + 0x50]
006f8f35: mov rbp, qword ptr [rsp + 0x58]
006f8f3a: mov rsi, qword ptr [rsp + 0x60]
006f8f3f: mov rdi, qword ptr [rsp + 0x68]
006f8f44: add rsp, 0x30
006f8f48: pop r15
006f8f4a: pop r14
006f8f4c: pop r12
006f8f4e: ret
006f8f4f: call 0x4fee20 ;
006f8f54: int3
006f8f55: int3
006f8f56: int3
006f8f57: int3
006f8f58: int3
006f8f59: int3
006f8f5a: int3
006f8f5b: int3
006f8f5c: int3
006f8f5d: int3
006f8f5e: int3
006f8f5f: int3
