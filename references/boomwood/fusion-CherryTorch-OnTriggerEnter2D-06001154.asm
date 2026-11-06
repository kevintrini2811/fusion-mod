0049ae40: mov qword ptr [rsp + 8], rbx
0049ae45: mov qword ptr [rsp + 0x18], rsi
0049ae4a: push rdi
0049ae4b: sub rsp, 0x40
0049ae4f: cmp byte ptr [rip + 0x30fe3ba], 0
0049ae56: mov rdi, rdx
0049ae59: mov rbx, rcx
0049ae5c: jne 0x18049ae71
0049ae5e: lea rcx, [rip + 0x2f1c6ab]
0049ae65: call 0x1803633b0
0049ae6a: mov byte ptr [rip + 0x30fe39f], 1
0049ae71: xor esi, esi
0049ae73: mov qword ptr [rsp + 0x58], rsi
0049ae78: test rdi, rdi
0049ae7b: je 0x18049afb1
0049ae81: mov r8, qword ptr [rip + 0x2f1c688]
0049ae88: lea rdx, [rsp + 0x58]
0049ae8d: mov rcx, rdi
0049ae90: call 0x180ac9930
0049ae95: test al, al
0049ae97: je 0x18049af2e
0049ae9d: mov rdx, qword ptr [rsp + 0x58]
0049aea2: xor r8d, r8d
0049aea5: mov rcx, rbx
0049aea8: call 0x180515f50
0049aead: test al, al
0049aeaf: je 0x18049af2e
0049aeb1: mov rdx, qword ptr [rsp + 0x58]
0049aeb6: test rdx, rdx
0049aeb9: je 0x18049afb1
0049aebf: mov eax, dword ptr [rdx + 0x54]
0049aec2: test eax, eax
0049aec4: jne 0x18049af3e
0049aec6: mov rax, qword ptr [rbx + 0x30]
0049aeca: test rax, rax
0049aecd: je 0x18049afb1
0049aed3: mov rcx, qword ptr [rax + 0x48]
0049aed7: test rcx, rcx
0049aeda: je 0x18049afb1
0049aee0: mov qword ptr [rsp + 0x30], rsi
0049aee5: mov r9d, 0x14
0049aeeb: mov byte ptr [rsp + 0x28], 1
0049aef0: mov dword ptr [rsp + 0x20], 0x19
0049aef8: mov r8, rbx
0049aefb: call 0x180420810
0049af00: mov eax, dword ptr [rbx + 0x248]
0049af06: inc eax
0049af08: mov dword ptr [rbx + 0x248], eax
0049af0e: cmp eax, 0x1e
0049af11: jle 0x18049af2e
0049af13: mov r8, qword ptr [rbx]
0049af16: mov edx, 0x12c
0049af1b: mov rcx, rbx
0049af1e: mov rax, qword ptr [r8 + 0x5b8]
0049af25: mov r8, qword ptr [r8 + 0x5c0]
0049af2c: call rax
0049af2e: mov rbx, qword ptr [rsp + 0x50]
0049af33: mov rsi, qword ptr [rsp + 0x60]
0049af38: add rsp, 0x40
0049af3c: pop rdi
0049af3d: ret 
0049af3e: cmp eax, 3
0049af41: jne 0x18049af84
0049af43: mov r8, qword ptr [rbx]
0049af46: mov rcx, rbx
0049af49: mov edx, dword ptr [rdx + 0xdc]
0049af4f: mov rax, qword ptr [r8 + 0x5b8]
0049af56: mov r8, qword ptr [r8 + 0x5c0]
0049af5d: call rax
0049af5f: test al, al
0049af61: je 0x18049af2e
0049af63: mov rcx, qword ptr [rsp + 0x58]
0049af68: test rcx, rcx
0049af6b: je 0x18049afb1
0049af6d: xor edx, edx
0049af6f: call 0x1805ef340
0049af74: mov rbx, qword ptr [rsp + 0x50]
0049af79: mov rsi, qword ptr [rsp + 0x60]
0049af7e: add rsp, 0x40
0049af82: pop rdi
0049af83: ret 
0049af84: cmp eax, 0xf
0049af87: jne 0x18049af2e
0049af89: mov rax, qword ptr [rbx + 0x30]
0049af8d: test rax, rax
0049af90: je 0x18049afb1
0049af92: mov rcx, qword ptr [rax + 0x48]
0049af96: test rcx, rcx
0049af99: je 0x18049afb1
0049af9b: mov qword ptr [rsp + 0x30], rsi
0049afa0: xor r9d, r9d
0049afa3: mov byte ptr [rsp + 0x28], 1
0049afa8: mov dword ptr [rsp + 0x20], esi
0049afac: jmp 0x18049aef8
0049afb1: call 0x180363610
0049afb6: int3 
0049afb7: int3 
0049afb8: int3 
0049afb9: int3 
0049afba: int3 
0049afbb: int3 
0049afbc: int3 
0049afbd: int3 
0049afbe: int3 
0049afbf: int3 
