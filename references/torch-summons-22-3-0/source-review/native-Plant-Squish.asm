006b5cd0: push rbx
006b5cd2: sub rsp, 0x20
006b5cd6: cmp byte ptr [rip + 0x4835969], 0
006b5cdd: mov rbx, rcx
006b5ce0: jne 0x6b5cf5
006b5ce2: lea rcx, [rip + 0x45bf47f]
006b5ce9: call 0x4feb70 ;
006b5cee: mov byte ptr [rip + 0x4835951], 1
006b5cf5: cmp dword ptr [rbx + 0x70], 0x11
006b5cf9: jne 0x6b5d16
006b5cfb: cmp dword ptr [rbx + 0x88], 5
006b5d02: je 0x6b5d6d
006b5d04: cmp dword ptr [rbx + 0x88], 6
006b5d0b: je 0x6b5d6d
006b5d0d: cmp dword ptr [rbx + 0x88], 7
006b5d14: je 0x6b5d6d
006b5d16: cmp byte ptr [rbx + 0x3c], 0
006b5d1a: jne 0x6b5d6d
006b5d1c: cmp dword ptr [rbx + 0x128], 2
006b5d23: je 0x6b5d6d
006b5d25: cmp byte ptr [rbx + 0x135], 0
006b5d2c: jne 0x6b5d6d
006b5d2e: cmp byte ptr [rbx + 0x136], 0
006b5d35: jne 0x6b5d73
006b5d37: cmp dword ptr [rbx + 0x70], 2
006b5d3b: je 0x6b5d5e
006b5d3d: cmp dword ptr [rbx + 0x70], 0x14
006b5d41: je 0x6b5d5e
006b5d43: cmp dword ptr [rbx + 0x70], 0xf
006b5d47: je 0x6b5d5e
006b5d49: cmp dword ptr [rbx + 0x70], 0xe
006b5d4d: je 0x6b5d5e
006b5d4f: cmp dword ptr [rbx + 0x70], 4
006b5d53: jne 0x6b5d73
006b5d55: cmp dword ptr [rbx + 0x88], 0
006b5d5c: je 0x6b5d73
006b5d5e: xor edx, edx
006b5d60: mov rcx, rbx
006b5d63: add rsp, 0x20
006b5d67: pop rbx
006b5d68: jmp 0x6ad780 ; Reloaded.Gameplay.Plant.DoSpecial
006b5d6d: add rsp, 0x20
006b5d71: pop rbx
006b5d72: ret
006b5d73: cmp dword ptr [rbx + 0x70], 0x11
006b5d77: jne 0x6b5d82
006b5d79: cmp dword ptr [rbx + 0x88], 0
006b5d80: jne 0x6b5d6d
006b5d82: mov edx, dword ptr [rbx + 0x34]
006b5d85: xor r9d, r9d
006b5d88: mov r8d, 8
006b5d8e: mov ecx, 0x497c8
006b5d93: call 0x665070 ; Reloaded.Gameplay.Board.MakeRenderOrder
006b5d98: mov r8, qword ptr [rbx + 0x10]
006b5d9c: mov dword ptr [rbx + 0x38], eax
006b5d9f: mov byte ptr [rbx + 0x3c], 1
006b5da3: mov dword ptr [rbx + 0x98], 0x1f4
006b5dad: test r8, r8
006b5db0: je 0x6b5e48
006b5db6: mov r8, qword ptr [r8 + 0x58]
006b5dba: test r8, r8
006b5dbd: je 0x6b5e48
006b5dc3: mov rdx, qword ptr [rip + 0x45bf39e]
006b5dca: mov ecx, 6
006b5dcf: mov r9d, 0x15
006b5dd5: call 0x2040 ;
006b5dda: xor edx, edx
006b5ddc: mov rcx, rbx
006b5ddf: call 0x6b4e30 ; Reloaded.Gameplay.Plant.RemoveEffects
006b5de4: mov rcx, qword ptr [rbx + 0x18]
006b5de8: test rcx, rcx
006b5deb: je 0x6b5e48
006b5ded: mov r8d, dword ptr [rbx + 0x34]
006b5df1: xor r9d, r9d
006b5df4: mov edx, dword ptr [rbx + 0x74]
006b5df7: call 0x65e170 ; Reloaded.Gameplay.Board.GetLadderAt
006b5dfc: test rax, rax
006b5dff: je 0x6b5e0b
006b5e01: xor edx, edx
006b5e03: mov rcx, rax
006b5e06: call 0x6c2390 ; Reloaded.Gameplay.GridItem.GridItemDie
006b5e0b: mov rcx, qword ptr [rbx + 0x10]
006b5e0f: test rcx, rcx
006b5e12: je 0x6b5e48
006b5e14: xor edx, edx
006b5e16: call 0x7df5d0 ; Reloaded.TreeStateActivities.GameplayActivity.IsIZombieLevel
006b5e1b: test al, al
006b5e1d: je 0x6b5d6d
006b5e23: mov rax, qword ptr [rbx + 0x18]
006b5e27: test rax, rax
006b5e2a: je 0x6b5e48
006b5e2c: mov rcx, qword ptr [rax + 0x110]
006b5e33: test rcx, rcx
006b5e36: je 0x6b5e48
006b5e38: xor r8d, r8d
006b5e3b: mov rdx, rbx
006b5e3e: add rsp, 0x20
006b5e42: pop rbx
006b5e43: jmp 0x683a60 ; Reloaded.Gameplay.Challenge.IZombiePlantDropRemainingSun
006b5e48: call 0x4fee20 ;
006b5e4d: int3
006b5e4e: int3
006b5e4f: int3
