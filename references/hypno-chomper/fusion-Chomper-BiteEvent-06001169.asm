0049c740: mov qword ptr [rsp + 8], rbx
0049c745: push rdi
0049c746: sub rsp, 0x40
0049c74a: cmp byte ptr [rip + 0x30fcacb], 0
0049c751: mov rbx, rcx
0049c754: jne 0x18049c769
0049c756: lea rcx, [rip + 0x2f3f43b]
0049c75d: call 0x1803633b0
0049c762: mov byte ptr [rip + 0x30fcab3], 1
0049c769: mov rcx, qword ptr [rip + 0x2f3f428]
0049c770: mov rdi, qword ptr [rbx + 0x90]
0049c777: cmp dword ptr [rcx + 0xe0], 0
0049c77e: jne 0x18049c785
0049c780: call 0x180318580
0049c785: xor r8d, r8d
0049c788: xor edx, edx
0049c78a: mov rcx, rdi
0049c78d: call 0x1826b9be0
0049c792: test al, al
0049c794: je 0x18049c843
0049c79a: mov rcx, qword ptr [rbx + 0x90]
0049c7a1: test rcx, rcx
0049c7a4: je 0x18049c864
0049c7aa: mov ecx, dword ptr [rcx + 0x58]
0049c7ad: xor edx, edx
0049c7af: call 0x1807f8490
0049c7b4: mov rcx, qword ptr [rbx + 0x90]
0049c7bb: test al, al
0049c7bd: jne 0x18049c808
0049c7bf: test rcx, rcx
0049c7c2: je 0x18049c864
0049c7c8: mov eax, dword ptr [rcx + 0x68]
0049c7cb: dec eax
0049c7cd: cmp eax, 2
0049c7d0: jbe 0x18049c843
0049c7d2: cmp byte ptr [rcx + 0x12d], 0
0049c7d9: jne 0x18049c843
0049c7db: cmp byte ptr [rcx + 0x135], 0
0049c7e2: jne 0x18049c843
0049c7e4: mov r8, qword ptr [rbx]
0049c7e7: mov rdx, rcx
0049c7ea: mov rcx, rbx
0049c7ed: mov rax, qword ptr [r8 + 0x608]
0049c7f4: mov r8, qword ptr [r8 + 0x610]
0049c7fb: mov rbx, qword ptr [rsp + 0x50]
0049c800: add rsp, 0x40
0049c804: pop rdi
0049c805: jmp rax
0049c808: test rcx, rcx
0049c80b: je 0x18049c864
0049c80d: mov rax, qword ptr [rcx]
0049c810: mov r9d, 4
0049c816: mov r8, rbx
0049c819: mov edx, 0x28
0049c81e: mov r10, qword ptr [rax + 0x1f8]
0049c825: mov rax, qword ptr [rax + 0x200]
0049c82c: mov qword ptr [rsp + 0x30], rax
0049c831: mov eax, dword ptr [rbx + 0x194]
0049c837: mov byte ptr [rsp + 0x28], 0
0049c83c: mov dword ptr [rsp + 0x20], eax
0049c840: call r10
0049c843: mov rdx, qword ptr [rbx]
0049c846: mov rcx, rbx
0049c849: mov rax, qword ptr [rdx + 0x618]
0049c850: mov rdx, qword ptr [rdx + 0x620]
0049c857: mov rbx, qword ptr [rsp + 0x50]
0049c85c: add rsp, 0x40
0049c860: pop rdi
0049c861: jmp rax
0049c864: call 0x180363610
0049c869: int3
0049c86a: int3
0049c86b: int3
0049c86c: int3
0049c86d: int3
0049c86e: int3
0049c86f: int3
