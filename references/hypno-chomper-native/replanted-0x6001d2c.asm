006e9710: mov qword ptr [rsp + 8], rbx
006e9715: mov qword ptr [rsp + 0x10], rbp
006e971a: mov qword ptr [rsp + 0x18], rsi
006e971f: push rdi
006e9720: sub rsp, 0x30
006e9724: mov rdi, rcx
006e9727: mov esi, r8d
006e972a: mov rcx, qword ptr [rcx + 0x10]
006e972e: mov rbx, rdx
006e9731: test rcx, rcx
006e9734: je 0x1806e998e
006e973a: xor edx, edx
006e973c: call 0x1807dfa20
006e9741: test al, al
006e9743: je 0x1806e974e
006e9745: cmp esi, 2
006e9748: jne 0x1806e9973
006e974e: test rbx, rbx
006e9751: je 0x1806e998e
006e9757: xor edx, edx
006e9759: mov rcx, rbx
006e975c: call 0x1806b3380
006e9761: test al, al
006e9763: jne 0x1806e9973
006e9769: cmp dword ptr [rbx + 0x70], 0x13
006e976d: je 0x1806e9973
006e9773: cmp byte ptr [rdi + 0x115], al
006e9779: jne 0x1806e979f
006e977b: mov rcx, qword ptr [rdi + 0x18]
006e977f: test rcx, rcx
006e9782: je 0x1806e998e
006e9788: mov r8d, dword ptr [rbx + 0x34]
006e978c: xor r9d, r9d
006e978f: mov edx, dword ptr [rbx + 0x74]
006e9792: call 0x180662110
006e9797: test al, al
006e9799: jne 0x1806e9973
006e979f: cmp dword ptr [rdi + 0x74], 0x20
006e97a3: je 0x1806e997a
006e97a9: xor edx, edx
006e97ab: mov rcx, rbx
006e97ae: call 0x1806b2430
006e97b3: test al, al
006e97b5: jne 0x1806e9914
006e97bb: cmp esi, 1
006e97be: jne 0x1806e9801
006e97c0: cmp dword ptr [rbx + 0x70], 2
006e97c4: je 0x1806e9973
006e97ca: cmp dword ptr [rbx + 0x70], 0x14
006e97ce: je 0x1806e9973
006e97d4: cmp dword ptr [rbx + 0x70], 0x1b
006e97d8: je 0x1806e9973
006e97de: cmp dword ptr [rbx + 0x70], 0x11
006e97e2: je 0x1806e9973
006e97e8: cmp dword ptr [rbx + 0x70], 0xf
006e97ec: je 0x1806e97f4
006e97ee: cmp dword ptr [rbx + 0x70], 0xe
006e97f2: jne 0x1806e9801
006e97f4: cmp byte ptr [rbx + 0x136], 0
006e97fb: jne 0x1806e986c
006e97fd: xor al, al
006e97ff: jmp 0x1806e986e
006e9801: cmp dword ptr [rdi + 0x74], 0x4c
006e9805: je 0x1806e980d
006e9807: cmp dword ptr [rdi + 0x74], 0x4d
006e980b: jne 0x1806e9883
006e980d: xor bpl, bpl
006e9810: cmp dword ptr [rbx + 0x70], 3
006e9814: je 0x1806e9822
006e9816: cmp dword ptr [rbx + 0x70], 0x17
006e981a: je 0x1806e9822
006e981c: cmp dword ptr [rbx + 0x70], 0x1e
006e9820: jne 0x1806e9825
006e9822: mov bpl, 1
006e9825: mov rcx, qword ptr [rdi + 0x18]
006e9829: test rcx, rcx
006e982c: je 0x1806e998e
006e9832: mov r8d, dword ptr [rbx + 0x34]
006e9836: xor r9d, r9d
006e9839: mov edx, dword ptr [rbx + 0x74]
006e983c: call 0x18065e170
006e9841: xor edx, edx
006e9843: movzx ecx, bpl
006e9847: test rax, rax
006e984a: cmove edx, ecx
006e984d: xor eax, eax
006e984f: test esi, esi
006e9851: movzx ecx, dl
006e9854: cmove eax, ecx
006e9857: test eax, eax
006e9859: jne 0x1806e9973
006e985f: cmp esi, 3
006e9862: jne 0x1806e9883
006e9864: test dl, dl
006e9866: je 0x1806e9973
006e986c: mov al, 1
006e986e: mov rbx, qword ptr [rsp + 0x40]
006e9873: mov rbp, qword ptr [rsp + 0x48]
006e9878: mov rsi, qword ptr [rsp + 0x50]
006e987d: add rsp, 0x30
006e9881: pop rdi
006e9882: ret
006e9883: test esi, esi
006e9885: jne 0x1806e98bb
006e9887: mov rcx, qword ptr [rdi + 0x18]
006e988b: test rcx, rcx
006e988e: je 0x1806e998e
006e9894: mov r8d, dword ptr [rbx + 0x34]
006e9898: xor r9d, r9d
006e989b: mov edx, dword ptr [rbx + 0x74]
006e989e: mov qword ptr [rsp + 0x20], 0
006e98a7: call 0x18065f2a0
006e98ac: cmp rax, rbx
006e98af: je 0x1806e986c
006e98b1: test rax, rax
006e98b4: je 0x1806e986c
006e98b6: xor r8d, r8d
006e98b9: jmp 0x1806e98fd
006e98bb: cmp esi, 2
006e98be: jne 0x1806e986c
006e98c0: mov rcx, qword ptr [rdi + 0x18]
006e98c4: test rcx, rcx
006e98c7: je 0x1806e998e
006e98cd: mov r8d, dword ptr [rbx + 0x34]
006e98d1: mov r9d, 6
006e98d7: mov edx, dword ptr [rbx + 0x74]
006e98da: mov qword ptr [rsp + 0x20], 0
006e98e3: call 0x18065f2a0
006e98e8: cmp rax, rbx
006e98eb: je 0x1806e986c
006e98f1: test rax, rax
006e98f4: je 0x1806e986c
006e98fa: mov r8d, esi
006e98fd: xor r9d, r9d
006e9900: mov rdx, rax
006e9903: mov rcx, rdi
006e9906: call 0x1806e9710
006e990b: test al, al
006e990d: jne 0x1806e9973
006e990f: jmp 0x1806e986c
006e9914: cmp dword ptr [rdi + 0x70], 0x17
006e9918: je 0x1806e986c
006e991e: cmp dword ptr [rdi + 0x70], 0x20
006e9922: je 0x1806e986c
006e9928: cmp dword ptr [rdi + 0x70], 0xc
006e992c: je 0x1806e986c
006e9932: mov rcx, qword ptr [rdi + 0x18]
006e9936: test rcx, rcx
006e9939: je 0x1806e998e
006e993b: mov r8d, dword ptr [rbx + 0x34]
006e993f: xor r9d, r9d
006e9942: mov edx, dword ptr [rbx + 0x74]
006e9945: call 0x180662110
006e994a: test al, al
006e994c: jne 0x1806e986c
006e9952: mov rcx, qword ptr [rdi + 0x18]
006e9956: test rcx, rcx
006e9959: je 0x1806e998e
006e995b: mov r8d, dword ptr [rbx + 0x34]
006e995f: xor r9d, r9d
006e9962: mov edx, dword ptr [rbx + 0x74]
006e9965: call 0x18065de10
006e996a: test rax, rax
006e996d: jne 0x1806e986c
006e9973: xor al, al
006e9975: jmp 0x1806e986e
006e997a: cmp dword ptr [rbx + 0x70], 4
006e997e: jne 0x1806e9973
006e9980: cmp dword ptr [rbx + 0x88], 0
006e9987: jne 0x1806e9973
006e9989: jmp 0x1806e986c
006e998e: call 0x1804fee20
006e9993: int3
006e9994: int3
006e9995: int3
006e9996: int3
006e9997: int3
006e9998: int3
006e9999: int3
006e999a: int3
006e999b: int3
006e999c: int3
006e999d: int3
006e999e: int3
006e999f: int3
