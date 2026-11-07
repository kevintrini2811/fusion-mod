0070d860: mov qword ptr [rsp + 8], rbx
0070d865: push rdi
0070d866: sub rsp, 0x20
0070d86a: cmp byte ptr [rip + 0x2e8cac0], 0
0070d871: mov rbx, rcx
0070d874: jne 0x18070d889
0070d876: lea rcx, [rip + 0x2c96303]
0070d87d: call 0x1803633b0
0070d882: mov byte ptr [rip + 0x2e8caa8], 1
0070d889: xor r8d, r8d
0070d88c: mov edx, 0x64
0070d891: mov ecx, 0x62
0070d896: call 0x1826bcca0
0070d89b: mov rcx, qword ptr [rip + 0x2c962de]
0070d8a2: mov edi, eax
0070d8a4: cmp dword ptr [rcx + 0xe0], 0
0070d8ab: jne 0x18070d8b2
0070d8ad: call 0x180318580
0070d8b2: movss xmm2, dword ptr [rip + 0x226e28e]
0070d8ba: xor r9d, r9d
0070d8bd: movss xmm1, dword ptr [rip + 0x226e277]
0070d8c5: mov ecx, edi
0070d8c7: call 0x18066a930
0070d8cc: xor edx, edx
0070d8ce: mov rcx, rbx
0070d8d1: mov rbx, qword ptr [rsp + 0x30]
0070d8d6: add rsp, 0x20
0070d8da: pop rdi
0070d8db: jmp 0x1805ef340
