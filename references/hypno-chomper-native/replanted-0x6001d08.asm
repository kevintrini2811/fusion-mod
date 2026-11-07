006e9450: mov qword ptr [rsp + 8], rbx
006e9455: push rdi
006e9456: sub rsp, 0x30
006e945a: cmp byte ptr [rip + 0x4802363], 0
006e9461: mov rbx, rcx
006e9464: jne 0x1806e9485
006e9466: lea rcx, [rip + 0x4595fd3]
006e946d: call 0x1804feb70
006e9472: lea rcx, [rip + 0x45abbff]
006e9479: call 0x1804feb70
006e947e: mov byte ptr [rip + 0x480233f], 1
006e9485: movss xmm0, dword ptr [rip + 0x354aa63]
006e948d: xor edi, edi
006e948f: mov rdx, qword ptr [rip + 0x45abbe2]
006e9496: mov r9d, 0x14
006e949c: mov qword ptr [rsp + 0x28], rdi
006e94a1: mov r8d, 3
006e94a7: mov rcx, rbx
006e94aa: movss dword ptr [rsp + 0x20], xmm0
006e94b0: call 0x1806f55d0
006e94b5: mov rcx, qword ptr [rbx + 0x18]
006e94b9: test rcx, rcx
006e94bc: je 0x1806e9553
006e94c2: mov r8d, dword ptr [rbx + 0x34]
006e94c6: mov r9d, 2
006e94cc: mov edx, dword ptr [rbx + 0xd8]
006e94d2: mov qword ptr [rsp + 0x20], rdi
006e94d7: call 0x18065f2a0
006e94dc: mov rdi, rax
006e94df: test rax, rax
006e94e2: je 0x1806e9548
006e94e4: xor edx, edx
006e94e6: mov rcx, rax
006e94e9: call 0x1806b3380
006e94ee: test al, al
006e94f0: jne 0x1806e9548
006e94f2: cmp dword ptr [rdi + 0x70], 0x2f
006e94f6: je 0x1806e9548
006e94f8: mov rax, qword ptr [rbx + 0x18]
006e94fc: test rax, rax
006e94ff: je 0x1806e9553
006e9501: mov rcx, qword ptr [rax + 0x78]
006e9505: test rcx, rcx
006e9508: je 0x1806e9553
006e950a: mov r8, qword ptr [rip + 0x4595f2f]
006e9511: mov rdx, rdi
006e9514: call 0x1824484c0
006e9519: xor edx, edx
006e951b: mov ecx, eax
006e951d: call 0x1806be7d0
006e9522: mov dword ptr [rbx + 0x17c], eax
006e9528: xor r9d, r9d
006e952b: mov dword ptr [rdi + 0x128], 1
006e9535: xor r8d, r8d
006e9538: mov edx, dword ptr [rbx + 0x34]
006e953b: mov ecx, 0x4a768
006e9540: call 0x180665070
006e9545: mov dword ptr [rbx + 0x38], eax
006e9548: mov rbx, qword ptr [rsp + 0x40]
006e954d: add rsp, 0x30
006e9551: pop rdi
006e9552: ret
006e9553: call 0x1804fee20
006e9558: int3
006e9559: int3
006e955a: int3
006e955b: int3
006e955c: int3
006e955d: int3
006e955e: int3
006e955f: int3
