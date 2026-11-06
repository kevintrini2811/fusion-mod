004eb450: mov qword ptr [rsp + 8], rbx
004eb455: mov qword ptr [rsp + 0x18], rsi
004eb45a: push rdi
004eb45b: sub rsp, 0x40
004eb45f: cmp byte ptr [rip + 0x30adfe8], 0
004eb466: mov rdi, rdx
004eb469: mov rbx, rcx
004eb46c: jne 0x1804eb481
004eb46e: lea rcx, [rip + 0x2ecc09b]
004eb475: call 0x1803633b0
004eb47a: mov byte ptr [rip + 0x30adfcd], 1
004eb481: xor esi, esi
004eb483: mov qword ptr [rsp + 0x58], rsi
004eb488: test rdi, rdi
004eb48b: je 0x1804eb57c
004eb491: mov r8, qword ptr [rip + 0x2ecc078]
004eb498: lea rdx, [rsp + 0x58]
004eb49d: mov rcx, rdi
004eb4a0: call 0x180ac9930
004eb4a5: test al, al
004eb4a7: je 0x1804eb56c
004eb4ad: mov rdx, qword ptr [rsp + 0x58]
004eb4b2: xor r8d, r8d
004eb4b5: mov rcx, rbx
004eb4b8: call 0x180515f50
004eb4bd: test al, al
004eb4bf: je 0x1804eb56c
004eb4c5: mov rdx, qword ptr [rsp + 0x58]
004eb4ca: test rdx, rdx
004eb4cd: je 0x1804eb57c
004eb4d3: mov eax, dword ptr [rdx + 0x54]
004eb4d6: test eax, eax
004eb4d8: jne 0x1804eb50e
004eb4da: mov rax, qword ptr [rbx + 0x30]
004eb4de: test rax, rax
004eb4e1: je 0x1804eb57c
004eb4e7: mov rcx, qword ptr [rax + 0x48]
004eb4eb: test rcx, rcx
004eb4ee: je 0x1804eb57c
004eb4f4: mov qword ptr [rsp + 0x30], rsi
004eb4f9: mov r9d, 0x14
004eb4ff: mov byte ptr [rsp + 0x28], 1
004eb504: mov dword ptr [rsp + 0x20], 0x19
004eb50c: jmp 0x1804eb536
004eb50e: cmp eax, 0xf
004eb511: jne 0x1804eb56c
004eb513: mov rax, qword ptr [rbx + 0x30]
004eb517: test rax, rax
004eb51a: je 0x1804eb57c
004eb51c: mov rcx, qword ptr [rax + 0x48]
004eb520: test rcx, rcx
004eb523: je 0x1804eb57c
004eb525: mov qword ptr [rsp + 0x30], rsi
004eb52a: xor r9d, r9d
004eb52d: mov byte ptr [rsp + 0x28], 1
004eb532: mov dword ptr [rsp + 0x20], esi
004eb536: mov r8, rbx
004eb539: call 0x180420810
004eb53e: mov eax, dword ptr [rbx + 0x248]
004eb544: inc eax
004eb546: mov dword ptr [rbx + 0x248], eax
004eb54c: cmp eax, 0x32
004eb54f: jle 0x1804eb56c
004eb551: mov r8, qword ptr [rbx]
004eb554: mov edx, 0x12c
004eb559: mov rcx, rbx
004eb55c: mov rax, qword ptr [r8 + 0x5b8]
004eb563: mov r8, qword ptr [r8 + 0x5c0]
004eb56a: call rax
004eb56c: mov rbx, qword ptr [rsp + 0x50]
004eb571: mov rsi, qword ptr [rsp + 0x60]
004eb576: add rsp, 0x40
004eb57a: pop rdi
004eb57b: ret
004eb57c: call 0x180363610
004eb581: int3
004eb582: int3
004eb583: int3
004eb584: int3
004eb585: int3
004eb586: int3
004eb587: int3
004eb588: int3
004eb589: int3
004eb58a: int3
004eb58b: int3
004eb58c: int3
004eb58d: int3
004eb58e: int3
004eb58f: int3
