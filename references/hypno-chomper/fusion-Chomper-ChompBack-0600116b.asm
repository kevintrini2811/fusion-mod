0049c930: push rbx
0049c932: sub rsp, 0x20
0049c936: cmp byte ptr [rip + 0x30fc8e1], 0
0049c93d: mov rbx, rcx
0049c940: jne 0x18049c961
0049c942: lea rcx, [rip + 0x2f07237]
0049c949: call 0x1803633b0
0049c94e: lea rcx, [rip + 0x2f33f83]
0049c955: call 0x1803633b0
0049c95a: mov byte ptr [rip + 0x30fc8bd], 1
0049c961: mov rcx, qword ptr [rip + 0x2f07218]
0049c968: cmp dword ptr [rcx + 0xe0], 0
0049c96f: jne 0x18049c976
0049c971: call 0x180318580
0049c976: movss xmm2, dword ptr [rip + 0x24df1ca]
0049c97e: xor r9d, r9d
0049c981: movss xmm1, dword ptr [rip + 0x24df1b3]
0049c989: mov ecx, 0x31
0049c98e: call 0x18066a930
0049c993: lea rcx, [rbx + 0x90]
0049c99a: mov qword ptr [rbx + 0x90], 0
0049c9a5: xor edx, edx
0049c9a7: call 0x180362730
0049c9ac: mov rcx, qword ptr [rbx + 0x28]
0049c9b0: test rcx, rcx
0049c9b3: je 0x18049c9c9
0049c9b5: mov rdx, qword ptr [rip + 0x2f33f1c]
0049c9bc: xor r8d, r8d
0049c9bf: add rsp, 0x20
0049c9c3: pop rbx
0049c9c4: jmp 0x182678fd0
0049c9c9: call 0x180363610
0049c9ce: int3
0049c9cf: int3
