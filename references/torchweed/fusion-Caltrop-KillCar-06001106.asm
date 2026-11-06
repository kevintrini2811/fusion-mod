00497570: mov qword ptr [rsp + 0x18], rbx
00497575: mov qword ptr [rsp + 0x20], rbp
0049757a: push rdi
0049757b: sub rsp, 0x60
0049757f: cmp byte ptr [rip + 0x3101c65], 0
00497586: mov rbx, rcx
00497589: jne 0x1804975aa
0049758b: lea rcx, [rip + 0x2f22e16]
00497592: call 0x1803633b0
00497597: lea rcx, [rip + 0x2f0c5e2]
0049759e: call 0x1803633b0
004975a3: mov byte ptr [rip + 0x3101c41], 1
004975aa: mov rdx, qword ptr [rbx]
004975ad: xor edi, edi
004975af: mov qword ptr [rsp + 0x70], rdi
004975b4: mov rcx, rbx
004975b7: mov qword ptr [rsp + 0x78], rsi
004975bc: movaps xmmword ptr [rsp + 0x50], xmm6
004975c1: mov rax, qword ptr [rdx + 0x5a8]
004975c8: mov rdx, qword ptr [rdx + 0x5b0]
004975cf: movaps xmmword ptr [rsp + 0x40], xmm7
004975d4: call rax
004975d6: mov rbp, rax
004975d9: mov ecx, edi
004975db: test rax, rax
004975de: je 0x180497785
004975e4: movss xmm6, dword ptr [rip + 0x24e455c]
004975ec: lea rsi, [rax + 0x20]
004975f0: movss xmm7, dword ptr [rip + 0x24e4544]
004975f8: nop dword ptr [rax + rax]
00497600: cmp ecx, dword ptr [rbp + 0x18]
00497603: jge 0x18049775e
00497609: cmp edi, dword ptr [rbp + 0x18]
0049760c: jae 0x18049777f
00497612: mov rcx, qword ptr [rsi]
00497615: test rcx, rcx
00497618: je 0x180497785
0049761e: mov r8, qword ptr [rip + 0x2f22d83]
00497625: lea rdx, [rsp + 0x70]
0049762a: call 0x180ac9930
0049762f: test al, al
00497631: je 0x180497751
00497637: mov rcx, qword ptr [rsp + 0x70]
0049763c: test rcx, rcx
0049763f: je 0x180497785
00497645: mov eax, dword ptr [rbx + 0xf8]
0049764b: cmp dword ptr [rcx + 0xcc], eax
00497651: jne 0x180497751
00497657: mov ecx, dword ptr [rcx + 0x58]
0049765a: xor edx, edx
0049765c: call 0x1807f8390
00497661: test al, al
00497663: je 0x180497751
00497669: mov rcx, qword ptr [rsp + 0x70]
0049766e: test rcx, rcx
00497671: je 0x180497785
00497677: cmp dword ptr [rcx + 0x68], 1
0049767b: je 0x180497751
00497681: mov rdx, qword ptr [rcx]
00497684: mov rax, qword ptr [rdx + 0x618]
0049768b: mov rdx, qword ptr [rdx + 0x620]
00497692: call rax
00497694: mov rcx, qword ptr [rip + 0x2f0c4e5]
0049769b: cmp dword ptr [rcx + 0xe0], 0
004976a2: jne 0x1804976a9
004976a4: call 0x180318580
004976a9: xor r9d, r9d
004976ac: movaps xmm2, xmm6
004976af: movaps xmm1, xmm7
004976b2: mov ecx, 0x4d
004976b7: call 0x18066a930
004976bc: xor edx, edx
004976be: mov rcx, rbx
004976c1: call 0x180471ba0
004976c6: mov rcx, rbx
004976c9: cmp eax, 0x5b8
004976ce: je 0x1804976f2
004976d0: mov r8, qword ptr [rbx]
004976d3: xor edx, edx
004976d5: mov rax, qword ptr [r8 + 0x348]
004976dc: mov r8, qword ptr [r8 + 0x350]
004976e3: call rax
004976e5: inc edi
004976e7: add rsi, 8
004976eb: mov ecx, edi
004976ed: jmp 0x180497600
004976f2: xor edx, edx
004976f4: call 0x180471f30
004976f9: test rax, rax
004976fc: je 0x180497785
00497702: mov rcx, qword ptr [rax]
00497705: xor r9d, r9d
00497708: mov r8, qword ptr [rsp + 0x70]
0049770d: mov edx, 0x1f4
00497712: mov r10, qword ptr [rcx + 0x1f8]
00497719: mov rcx, qword ptr [rcx + 0x200]
00497720: mov qword ptr [rsp + 0x30], rcx
00497725: mov rcx, rax
00497728: mov byte ptr [rsp + 0x28], 0
0049772d: mov dword ptr [rsp + 0x20], 0xffffffff
00497735: call r10
00497738: xor edx, edx
0049773a: mov rcx, rbx
0049773d: call 0x180471f30
00497742: test rax, rax
00497745: je 0x180497785
00497747: xor edx, edx
00497749: mov rcx, rax
0049774c: call 0x180468c40
00497751: inc edi
00497753: add rsi, 8
00497757: mov ecx, edi
00497759: jmp 0x180497600
0049775e: movaps xmm7, xmmword ptr [rsp + 0x40]
00497763: lea r11, [rsp + 0x60]
00497768: mov rbx, qword ptr [r11 + 0x20]
0049776c: mov rbp, qword ptr [r11 + 0x28]
00497770: movaps xmm6, xmmword ptr [rsp + 0x50]
00497775: mov rsi, qword ptr [rsp + 0x78]
0049777a: mov rsp, r11
0049777d: pop rdi
0049777e: ret
0049777f: call 0x180363600
00497784: int3
00497785: call 0x180363610
0049778a: int3
0049778b: int3
0049778c: int3
0049778d: int3
0049778e: int3
0049778f: int3
