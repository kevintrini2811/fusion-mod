0049c9d0: mov qword ptr [rsp + 8], rbx
0049c9d5: push rdi
0049c9d6: sub rsp, 0x20
0049c9da: cmp byte ptr [rip + 0x30fc83c], 0
0049c9e1: mov rdi, rdx
0049c9e4: mov rbx, rcx
0049c9e7: jne 0x18049c9fc
0049c9e9: lea rcx, [rip + 0x2f07190]
0049c9f0: call 0x1803633b0
0049c9f5: mov byte ptr [rip + 0x30fc821], 1
0049c9fc: test rdi, rdi
0049c9ff: je 0x18049ca9a
0049ca05: mov r8, qword ptr [rdi]
0049ca08: mov edx, 2
0049ca0d: mov rcx, rdi
0049ca10: mov rax, qword ptr [r8 + 0x328]
0049ca17: mov r8, qword ptr [r8 + 0x330]
0049ca1e: call rax
0049ca20: movss xmm1, dword ptr [rbx + 0x248]
0049ca28: xor r8d, r8d
0049ca2b: mov rcx, rbx
0049ca2e: call 0x180472e10
0049ca33: lea rcx, [rbx + 0x90]
0049ca3a: mov byte ptr [rbx + 0x24c], 1
0049ca41: xor edx, edx
0049ca43: mov qword ptr [rbx + 0x90], 0
0049ca4e: call 0x180362730
0049ca53: mov rcx, qword ptr [rip + 0x2f07126]
0049ca5a: cmp dword ptr [rcx + 0xe0], 0
0049ca61: jne 0x18049ca68
0049ca63: call 0x180318580
0049ca68: movss xmm2, dword ptr [rip + 0x24df0d8]
0049ca70: xor r9d, r9d
0049ca73: movss xmm1, dword ptr [rip + 0x24df0c1]
0049ca7b: mov ecx, 0x31
0049ca80: call 0x18066a930
0049ca85: mov dword ptr [rbx + 0x190], 0x17
0049ca8f: mov rbx, qword ptr [rsp + 0x30]
0049ca94: add rsp, 0x20
0049ca98: pop rdi
0049ca99: ret
0049ca9a: call 0x180363610
0049ca9f: int3
