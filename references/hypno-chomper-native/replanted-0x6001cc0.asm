0070a570: push rbx
0070a572: sub rsp, 0x20
0070a576: cmp byte ptr [rip + 0x47e121c], 0
0070a57d: mov rbx, rcx
0070a580: jne 0x18070a5a1
0070a582: lea rcx, [rip + 0x451f3a7]
0070a589: call 0x1804feb70
0070a58e: lea rcx, [rip + 0x457ec9b]
0070a595: call 0x1804feb70
0070a59a: mov byte ptr [rip + 0x47e11f8], 1
0070a5a1: mov rcx, qword ptr [rip + 0x457ec88]
0070a5a8: mov edx, 4
0070a5ad: call 0x1804fdf20
0070a5b2: lea rcx, [rbx + 0x150]
0070a5b9: mov qword ptr [rbx + 0x150], rax
0070a5c0: mov rdx, rax
0070a5c3: call 0x1804fdea0
0070a5c8: xor edx, edx
0070a5ca: mov rcx, rbx
0070a5cd: call 0x1806ce700
0070a5d2: mov rcx, qword ptr [rip + 0x451f357]
0070a5d9: call 0x181137af0
0070a5de: lea rcx, [rbx + 0x220]
0070a5e5: mov qword ptr [rbx + 0x220], rax
0070a5ec: mov rdx, rax
0070a5ef: add rsp, 0x20
0070a5f3: pop rbx
0070a5f4: jmp 0x1804fdea0
0070a5f9: int3
0070a5fa: int3
0070a5fb: int3
0070a5fc: int3
0070a5fd: int3
0070a5fe: int3
0070a5ff: int3
0070a600: sub rsp, 0x38
0070a604: cmp byte ptr [rip + 0x47e12c8], 0
0070a60b: movaps xmmword ptr [rsp + 0x20], xmm6
0070a610: movaps xmm6, xmm0
0070a613: jne 0x18070a628
0070a615: lea rcx, [rip + 0x45a084c]
0070a61c: call 0x1804feb70
0070a621: mov byte ptr [rip + 0x47e12ab], 1
0070a628: mov rcx, qword ptr [rip + 0x45a0839]
0070a62f: cmp dword ptr [rcx + 0xe4], 0
0070a636: jne 0x18070a63d
0070a638: call 0x1804feeb0
0070a63d: cvtss2sd xmm6, xmm6
0070a641: lea rdx, [rsp + 0x50]
0070a646: xorps xmm0, xmm0
0070a649: comisd xmm6, xmm0
0070a64d: movaps xmm0, xmm6
0070a650: jb 0x18070a6a6
0070a652: call 0x1805d9360
0070a657: movsd xmm1, qword ptr [rip + 0x35295f1]
0070a65f: ucomisd xmm0, xmm1
0070a663: jp 0x18070a68c
0070a665: jne 0x18070a68c
0070a667: movsd xmm0, qword ptr [rsp + 0x50]
0070a66d: cvttsd2si rax, xmm0
0070a672: test al, 1
0070a674: je 0x18070a6ec
0070a676: addsd xmm0, qword ptr [rip + 0x352979a]
0070a67e: cvttsd2si eax, xmm0
0070a682: movaps xmm6, xmmword ptr [rsp + 0x20]
0070a687: add rsp, 0x38
0070a68b: ret
0070a68c: addsd xmm6, xmm1
0070a690: movaps xmm0, xmm6
0070a693: call 0x1805d9710
0070a698: cvttsd2si eax, xmm0
0070a69c: movaps xmm6, xmmword ptr [rsp + 0x20]
0070a6a1: add rsp, 0x38
0070a6a5: ret
0070a6a6: call 0x1805d9360
0070a6ab: ucomisd xmm0, qword ptr [rip + 0x35295cd]
0070a6b3: jp 0x18070a6dc
0070a6b5: jne 0x18070a6dc
0070a6b7: movsd xmm0, qword ptr [rsp + 0x50]
0070a6bd: cvttsd2si rax, xmm0
0070a6c2: test al, 1
0070a6c4: je 0x18070a6ec
0070a6c6: subsd xmm0, qword ptr [rip + 0x352974a]
0070a6ce: cvttsd2si eax, xmm0
0070a6d2: movaps xmm6, xmmword ptr [rsp + 0x20]
0070a6d7: add rsp, 0x38
0070a6db: ret
0070a6dc: subsd xmm6, qword ptr [rip + 0x352956c]
0070a6e4: movaps xmm0, xmm6
0070a6e7: call 0x1805d9c80
0070a6ec: movaps xmm6, xmmword ptr [rsp + 0x20]
0070a6f1: cvttsd2si eax, xmm0
0070a6f5: add rsp, 0x38
0070a6f9: ret
0070a6fa: int3
0070a6fb: int3
0070a6fc: int3
0070a6fd: int3
0070a6fe: int3
0070a6ff: int3
