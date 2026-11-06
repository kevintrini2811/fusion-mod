006f9160: mov qword ptr [rsp + 8], rbx
006f9165: mov qword ptr [rsp + 0x10], rsi
006f916a: push rdi
006f916b: sub rsp, 0x20
006f916f: cmp byte ptr [rip + 0x47f265b], 0
006f9176: mov esi, r8d
006f9179: mov ebx, edx
006f917b: mov rdi, rcx
006f917e: jne 0x6f9193
006f9180: lea rcx, [rip + 0x4531e39]
006f9187: call 0x4feb70 ;
006f918c: mov byte ptr [rip + 0x47f263e], 1
006f9193: mov rcx, qword ptr [rip + 0x4531e26]
006f919a: cmp dword ptr [rcx + 0xe4], 0
006f91a1: jne 0x6f91a8
006f91a3: call 0x4feeb0 ;
006f91a8: test sil, 8
006f91ac: ja 0x6f91b8
006f91ae: mov dword ptr [rdi + 0xa8], 0x19
006f91b8: mov eax, dword ptr [rdi + 0x13c]
006f91be: mov ecx, ebx
006f91c0: cmp eax, ebx
006f91c2: cmovl ecx, eax
006f91c5: sub eax, ecx
006f91c7: sub ebx, ecx
006f91c9: mov dword ptr [rdi + 0x13c], eax
006f91cf: test eax, eax
006f91d1: jne 0x6f91e0
006f91d3: xor r8d, r8d
006f91d6: mov edx, esi
006f91d8: mov rcx, rdi
006f91db: call 0x6f3aa0 ; Reloaded.Gameplay.Zombie.LandFlyer
006f91e0: mov eax, ebx
006f91e2: mov rbx, qword ptr [rsp + 0x30]
006f91e7: mov rsi, qword ptr [rsp + 0x38]
006f91ec: add rsp, 0x20
006f91f0: pop rdi
006f91f1: ret
006f91f2: int3
006f91f3: int3
006f91f4: int3
006f91f5: int3
006f91f6: int3
006f91f7: int3
006f91f8: int3
006f91f9: int3
006f91fa: int3
006f91fb: int3
006f91fc: int3
006f91fd: int3
006f91fe: int3
006f91ff: int3
