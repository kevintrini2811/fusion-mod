004977f0: mov qword ptr [rsp + 8], rbx
004977f5: push rdi
004977f6: sub rsp, 0x20
004977fa: cmp byte ptr [rip + 0x31019e9], 0
00497801: mov rdi, rdx
00497804: mov rbx, rcx
00497807: jne 0x180497828
00497809: lea rcx, [rip + 0x2f22b98]
00497810: call 0x1803633b0
00497815: lea rcx, [rip + 0x2f37b6c]
0049781c: call 0x1803633b0
00497821: mov byte ptr [rip + 0x31019c2], 1
00497828: mov qword ptr [rsp + 0x38], 0
00497831: test rdi, rdi
00497834: je 0x1804978a6
00497836: mov r8, qword ptr [rip + 0x2f22b6b]
0049783d: lea rdx, [rsp + 0x38]
00497842: mov rcx, rdi
00497845: call 0x180ac9930
0049784a: test al, al
0049784c: je 0x18049789b
0049784e: mov rcx, qword ptr [rsp + 0x38]
00497853: test rcx, rcx
00497856: je 0x1804978a6
00497858: cmp dword ptr [rcx + 0x68], 1
0049785c: je 0x18049789b
0049785e: mov eax, dword ptr [rbx + 0xf8]
00497864: cmp dword ptr [rcx + 0xcc], eax
0049786a: jne 0x18049789b
0049786c: cmp byte ptr [rcx + 0x12d], 0
00497873: jne 0x18049789b
00497875: mov ecx, dword ptr [rcx + 0x58]
00497878: xor edx, edx
0049787a: call 0x1807f8390
0049787f: test al, al
00497881: je 0x18049789b
00497883: mov rcx, qword ptr [rbx + 0x28]
00497887: test rcx, rcx
0049788a: je 0x1804978a6
0049788c: mov rdx, qword ptr [rip + 0x2f37af5]
00497893: xor r8d, r8d
00497896: call 0x182678fd0
0049789b: mov rbx, qword ptr [rsp + 0x30]
004978a0: add rsp, 0x20
004978a4: pop rdi
004978a5: ret
004978a6: call 0x180363610
004978ab: int3
004978ac: int3
004978ad: int3
004978ae: int3
004978af: int3
