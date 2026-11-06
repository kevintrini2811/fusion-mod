004978b0: mov qword ptr [rsp + 0x18], rbx
004978b5: mov qword ptr [rsp + 0x20], rbp
004978ba: push rdi
004978bb: sub rsp, 0x20
004978bf: cmp byte ptr [rip + 0x3101923], 0
004978c6: mov rdi, rcx
004978c9: jne 0x1804978ea
004978cb: lea rcx, [rip + 0x2f22ad6]
004978d2: call 0x1803633b0
004978d7: lea rcx, [rip + 0x2f37aaa]
004978de: call 0x1803633b0
004978e3: mov byte ptr [rip + 0x31018ff], 1
004978ea: mov rdx, qword ptr [rdi]
004978ed: xor ebx, ebx
004978ef: mov qword ptr [rsp + 0x30], rbx
004978f4: mov rcx, rdi
004978f7: mov qword ptr [rsp + 0x38], rsi
004978fc: mov rax, qword ptr [rdx + 0x5a8]
00497903: mov rdx, qword ptr [rdx + 0x5b0]
0049790a: call rax
0049790c: mov rbp, rax
0049790f: mov ecx, ebx
00497911: test rax, rax
00497914: je 0x1804979ca
0049791a: lea rsi, [rax + 0x20]
0049791e: nop
00497920: cmp ecx, dword ptr [rbp + 0x18]
00497923: jge 0x1804979af
00497929: cmp ebx, dword ptr [rbp + 0x18]
0049792c: jae 0x1804979c4
00497932: mov rcx, qword ptr [rsi]
00497935: test rcx, rcx
00497938: je 0x1804979ca
0049793e: mov r8, qword ptr [rip + 0x2f22a63]
00497945: lea rdx, [rsp + 0x30]
0049794a: call 0x180ac9930
0049794f: test al, al
00497951: je 0x1804979a2
00497953: mov r8, qword ptr [rdi]
00497956: mov rcx, rdi
00497959: mov rdx, qword ptr [rsp + 0x30]
0049795e: mov rax, qword ptr [r8 + 0x428]
00497965: mov r8, qword ptr [r8 + 0x430]
0049796c: call rax
0049796e: test al, al
00497970: je 0x1804979a2
00497972: mov rcx, qword ptr [rsp + 0x30]
00497977: test rcx, rcx
0049797a: je 0x1804979ca
0049797c: mov eax, dword ptr [rdi + 0xf8]
00497982: cmp dword ptr [rcx + 0xcc], eax
00497988: jne 0x1804979a2
0049798a: mov rcx, qword ptr [rdi + 0x28]
0049798e: test rcx, rcx
00497991: je 0x1804979ca
00497993: mov rdx, qword ptr [rip + 0x2f379ee]
0049799a: xor r8d, r8d
0049799d: call 0x182678fd0
004979a2: inc ebx
004979a4: add rsi, 8
004979a8: mov ecx, ebx
004979aa: jmp 0x180497920
004979af: mov rsi, qword ptr [rsp + 0x38]
004979b4: mov rbx, qword ptr [rsp + 0x40]
004979b9: mov rbp, qword ptr [rsp + 0x48]
004979be: add rsp, 0x20
004979c2: pop rdi
004979c3: ret
004979c4: call 0x180363600
004979c9: int3
004979ca: call 0x180363610
004979cf: int3
