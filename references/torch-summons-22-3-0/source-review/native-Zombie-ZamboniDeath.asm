00706b00: mov qword ptr [rsp + 8], rbx
00706b05: push rdi
00706b06: sub rsp, 0x40
00706b0a: cmp byte ptr [rip + 0x47e4ce1], 0
00706b11: mov edi, edx
00706b13: mov rbx, rcx
00706b16: jne 0x706b5b
00706b18: lea rcx, [rip + 0x45244a1]
00706b1f: call 0x4feb70 ;
00706b24: lea rcx, [rip + 0x456e63d]
00706b2b: call 0x4feb70 ;
00706b30: lea rcx, [rip + 0x4593639]
00706b37: call 0x4feb70 ;
00706b3c: lea rcx, [rip + 0x4593555]
00706b43: call 0x4feb70 ;
00706b48: lea rcx, [rip + 0x4536591]
00706b4f: call 0x4feb70 ;
00706b54: mov byte ptr [rip + 0x47e4c97], 1
00706b5b: mov rcx, qword ptr [rip + 0x452445e]
00706b62: cmp dword ptr [rcx + 0xe4], 0
00706b69: jne 0x706b70
00706b6b: call 0x4feeb0 ;
00706b70: mov rax, qword ptr [rbx + 0x10]
00706b74: test dil, 0x20
00706b78: ja 0x706c0e
00706b7e: test rax, rax
00706b81: je 0x706dd4
00706b87: movss xmm2, dword ptr [rbx + 0x7c]
00706b8c: xor edi, edi
00706b8e: movss xmm1, dword ptr [rbx + 0x78]
00706b93: mov rcx, rax
00706b96: mov r9d, dword ptr [rbx + 0x38]
00706b9a: addss xmm2, dword ptr [rip + 0x352d0c6]
00706ba2: addss xmm1, dword ptr [rip + 0x352d222]
00706baa: inc r9d
00706bad: mov qword ptr [rsp + 0x28], rdi
00706bb2: mov dword ptr [rsp + 0x20], 0x3f
00706bba: call 0x7d9b50 ; Reloaded.TreeStateActivities.GameplayActivity.AddTodParticle
00706bbf: xor edx, edx
00706bc1: mov rcx, rbx
00706bc4: call 0x6ebf90 ; Reloaded.Gameplay.Zombie.DieNoLoot
00706bc9: xor edx, edx
00706bcb: mov rcx, rbx
00706bce: call 0x6f0500 ; Reloaded.Gameplay.Zombie.DropLoot
00706bd3: mov r8, qword ptr [rbx + 0x10]
00706bd7: test r8, r8
00706bda: je 0x706dd4
00706be0: mov r8, qword ptr [r8 + 0x58]
00706be4: test r8, r8
00706be7: je 0x706dd4
00706bed: mov rdx, qword ptr [rip + 0x456e574]
00706bf4: mov ecx, 6
00706bf9: mov r9d, 0x17
00706bff: mov rbx, qword ptr [rsp + 0x50]
00706c04: add rsp, 0x40
00706c08: pop rdi
00706c09: jmp 0x2040 ;
00706c0e: mov byte ptr [rbx + 0xd0], 1
00706c15: test rax, rax
00706c18: je 0x706dd4
00706c1e: mov r8, qword ptr [rax + 0x58]
00706c22: test r8, r8
00706c25: je 0x706dd4
00706c2b: mov rdx, qword ptr [rip + 0x456e536]
00706c32: mov ecx, 6
00706c37: mov r9d, 0x16
00706c3d: call 0x2040 ;
00706c42: mov rcx, qword ptr [rbx + 0x10]
00706c46: mov dword ptr [rbx + 0x74], 1
00706c4d: test rcx, rcx
00706c50: je 0x706dd4
00706c56: movss xmm2, dword ptr [rbx + 0x7c]
00706c5b: xor edi, edi
00706c5d: movss xmm1, dword ptr [rbx + 0x78]
00706c62: mov r9d, dword ptr [rbx + 0x38]
00706c66: addss xmm2, dword ptr [rip + 0x352d586]
00706c6e: addss xmm1, dword ptr [rip + 0x352d346]
00706c76: inc r9d
00706c79: mov qword ptr [rsp + 0x28], rdi
00706c7e: mov dword ptr [rsp + 0x20], 0x3e
00706c86: call 0x7d9b50 ; Reloaded.TreeStateActivities.GameplayActivity.AddTodParticle
00706c8b: test rax, rax
00706c8e: je 0x706dd4
00706c94: mov rcx, qword ptr [rax + 0xa0]
00706c9b: test rcx, rcx
00706c9e: je 0x706dd4
00706ca4: mov edx, dword ptr [rbx + 0x34]
00706ca7: xor r8d, r8d
00706caa: call 0x7ac710 ; Source.Controllers.EffectController.SetZombieLayerRow
00706caf: mov dword ptr [rbx + 0x88], edi
00706cb5: mov rcx, qword ptr [rip + 0x4524304]
00706cbc: cmp dword ptr [rcx + 0xe4], edi
00706cc2: jne 0x706cc9
00706cc4: call 0x4feeb0 ;
00706cc9: xor edx, edx
00706ccb: mov ecx, 4
00706cd0: call 0x6cbe80 ; Reloaded.Gameplay.Common.Rand
00706cd5: test eax, eax
00706cd7: jne 0x706d2a
00706cd9: movss xmm0, dword ptr [rip + 0x352d3bb]
00706ce1: comiss xmm0, dword ptr [rbx + 0x78]
00706ce5: jbe 0x706d2a
00706ce7: movss xmm0, dword ptr [rip + 0x352d0bd]
00706cef: mov r9d, 0xa
00706cf5: mov rdx, qword ptr [rip + 0x4593474]
00706cfc: mov r8d, 3
00706d02: mov qword ptr [rsp + 0x28], rdi
00706d07: mov rcx, rbx
00706d0a: movss dword ptr [rsp + 0x20], xmm0
00706d10: call 0x6f55d0 ; Reloaded.Gameplay.Zombie.PlayZombieReanim
00706d15: mov dword ptr [rbx + 0xbc], 0x118
00706d1f: mov rbx, qword ptr [rsp + 0x50]
00706d24: add rsp, 0x40
00706d28: pop rdi
00706d29: ret
00706d2a: mov rcx, qword ptr [rbx + 0x10]
00706d2e: test rcx, rcx
00706d31: je 0x706dd4
00706d37: mov qword ptr [rsp + 0x28], rdi
00706d3c: xor r9d, r9d
00706d3f: xorps xmm2, xmm2
00706d42: mov dword ptr [rsp + 0x20], 0x3b
00706d4a: xorps xmm1, xmm1
00706d4d: call 0x7d9b50 ; Reloaded.TreeStateActivities.GameplayActivity.AddTodParticle
00706d52: test rax, rax
00706d55: je 0x706d91
00706d57: mov rcx, qword ptr [rbx + 0x1d0]
00706d5e: test rcx, rcx
00706d61: je 0x706dd4
00706d63: movss xmm0, dword ptr [rip + 0x352d19d]
00706d6b: mov r8, rax
00706d6e: movss xmm3, dword ptr [rip + 0x352d17e]
00706d76: mov rdx, qword ptr [rip + 0x4536363]
00706d7d: mov qword ptr [rsp + 0x30], rdi
00706d82: mov dword ptr [rsp + 0x28], edi
00706d86: movss dword ptr [rsp + 0x20], xmm0
00706d8c: call 0x60f880 ; Reloaded.Characters.CharacterAnimationController.AttachParticleToTrack
00706d91: movss xmm0, dword ptr [rip + 0x352d01b]
00706d99: mov r9d, 0xa
00706d9f: mov dword ptr [rbx + 0xbc], 0x118
00706da9: mov r8d, 3
00706daf: mov rdx, qword ptr [rip + 0x45932e2]
00706db6: mov rcx, rbx
00706db9: mov qword ptr [rsp + 0x28], rdi
00706dbe: movss dword ptr [rsp + 0x20], xmm0
00706dc4: call 0x6f55d0 ; Reloaded.Gameplay.Zombie.PlayZombieReanim
00706dc9: mov rbx, qword ptr [rsp + 0x50]
00706dce: add rsp, 0x40
00706dd2: pop rdi
00706dd3: ret
00706dd4: call 0x4fee20 ;
00706dd9: int3
00706dda: int3
00706ddb: int3
00706ddc: int3
00706ddd: int3
00706dde: int3
00706ddf: int3
