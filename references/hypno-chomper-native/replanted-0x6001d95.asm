006f3830: push rbx
006f3832: sub rsp, 0x20
006f3836: cmp byte ptr [rip + 0x47f7fe4], 0
006f383d: mov rbx, rcx
006f3840: jne 0x1806f3861
006f3842: lea rcx, [rip + 0x458bda7]
006f3849: call 0x1804feb70
006f384e: lea rcx, [rip + 0x458be73]
006f3855: call 0x1804feb70
006f385a: mov byte ptr [rip + 0x47f7fc0], 1
006f3861: cmp dword ptr [rbx + 0xb8], 3
006f3868: mov qword ptr [rsp + 0x30], rsi
006f386d: mov qword ptr [rsp + 0x38], rdi
006f3872: je 0x1806f390c
006f3878: mov rax, qword ptr [rbx + 0x18]
006f387c: xor edi, edi
006f387e: mov edx, edi
006f3880: test rax, rax
006f3883: je 0x1806f391e
006f3889: nop dword ptr [rax]
006f3890: mov rcx, qword ptr [rax + 0x78]
006f3894: test rcx, rcx
006f3897: je 0x1806f391e
006f389d: cmp edx, dword ptr [rcx + 0x28]
006f38a0: jge 0x1806f38fa
006f38a2: mov rax, qword ptr [rbx + 0x18]
006f38a6: test rax, rax
006f38a9: je 0x1806f391e
006f38ab: mov rcx, qword ptr [rax + 0x78]
006f38af: test rcx, rcx
006f38b2: je 0x1806f391e
006f38b4: mov r8, qword ptr [rip + 0x458be0d]
006f38bb: mov edx, edi
006f38bd: call 0x182449330
006f38c2: test rax, rax
006f38c5: je 0x1806f391e
006f38c7: cmp dword ptr [rax + 0x70], 0x13
006f38cb: jne 0x1806f38eb
006f38cd: mov rcx, qword ptr [rbx + 0x18]
006f38d1: mov esi, dword ptr [rax + 0x120]
006f38d7: test rcx, rcx
006f38da: je 0x1806f391e
006f38dc: xor r8d, r8d
006f38df: mov rdx, rbx
006f38e2: call 0x180676eb0
006f38e7: cmp esi, eax
006f38e9: je 0x1806f390c
006f38eb: mov rax, qword ptr [rbx + 0x18]
006f38ef: inc edi
006f38f1: mov edx, edi
006f38f3: test rax, rax
006f38f6: je 0x1806f391e
006f38f8: jmp 0x1806f3890
006f38fa: xor al, al
006f38fc: mov rdi, qword ptr [rsp + 0x38]
006f3901: mov rsi, qword ptr [rsp + 0x30]
006f3906: add rsp, 0x20
006f390a: pop rbx
006f390b: ret
006f390c: mov rdi, qword ptr [rsp + 0x38]
006f3911: mov al, 1
006f3913: mov rsi, qword ptr [rsp + 0x30]
006f3918: add rsp, 0x20
006f391c: pop rbx
006f391d: ret
006f391e: call 0x1804fee20
006f3923: int3
006f3924: int3
006f3925: int3
006f3926: int3
006f3927: int3
006f3928: int3
006f3929: int3
006f392a: int3
006f392b: int3
006f392c: int3
006f392d: int3
006f392e: int3
006f392f: int3
