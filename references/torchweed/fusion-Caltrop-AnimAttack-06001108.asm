004973f0: mov qword ptr [rsp + 0x18], rbx
004973f5: push rbp
004973f6: push rdi
004973f7: push r14
004973f9: sub rsp, 0x20
004973fd: cmp byte ptr [rip + 0x3101de8], 0
00497404: mov rdi, rcx
00497407: jne 0x180497428
00497409: lea rcx, [rip + 0x2f22f98]
00497410: call 0x1803633b0
00497415: lea rcx, [rip + 0x2f0c764]
0049741c: call 0x1803633b0
00497421: mov byte ptr [rip + 0x3101dc4], 1
00497428: mov rdx, qword ptr [rdi]
0049742b: xor ebx, ebx
0049742d: mov qword ptr [rsp + 0x40], rbx
00497432: mov rcx, rdi
00497435: mov qword ptr [rsp + 0x48], rsi
0049743a: mov rax, qword ptr [rdx + 0x5c8]
00497441: mov rdx, qword ptr [rdx + 0x5d0]
00497448: call rax
0049744a: mov rdx, qword ptr [rdi]
0049744d: mov rcx, rdi
00497450: xor r14b, r14b
00497453: mov rax, qword ptr [rdx + 0x5a8]
0049745a: mov rdx, qword ptr [rdx + 0x5b0]
00497461: call rax
00497463: mov rbp, rax
00497466: mov ecx, ebx
00497468: test rax, rax
0049746b: je 0x180497561
00497471: lea rsi, [rax + 0x20]
00497475: cmp ecx, dword ptr [rbp + 0x18]
00497478: jge 0x180497509
0049747e: cmp ebx, dword ptr [rbp + 0x18]
00497481: jae 0x180497567
00497487: mov rcx, qword ptr [rsi]
0049748a: test rcx, rcx
0049748d: je 0x180497561
00497493: mov r8, qword ptr [rip + 0x2f22f0e]
0049749a: lea rdx, [rsp + 0x40]
0049749f: call 0x180ac9930
004974a4: test al, al
004974a6: je 0x1804974fc
004974a8: mov rdx, qword ptr [rsp + 0x40]
004974ad: test rdx, rdx
004974b0: je 0x180497561
004974b6: mov eax, dword ptr [rdi + 0xf8]
004974bc: cmp dword ptr [rdx + 0xcc], eax
004974c2: jne 0x1804974fc
004974c4: mov r8, qword ptr [rdi]
004974c7: mov rcx, rdi
004974ca: mov rax, qword ptr [r8 + 0x428]
004974d1: mov r8, qword ptr [r8 + 0x430]
004974d8: call rax
004974da: test al, al
004974dc: je 0x1804974fc
004974de: mov r8, qword ptr [rdi]
004974e1: mov rcx, rdi
004974e4: mov rdx, qword ptr [rsp + 0x40]
004974e9: mov r14b, 1
004974ec: mov rax, qword ptr [r8 + 0x5e8]
004974f3: mov r8, qword ptr [r8 + 0x5f0]
004974fa: call rax
004974fc: inc ebx
004974fe: add rsi, 8
00497502: mov ecx, ebx
00497504: jmp 0x180497475
00497509: test r14b, r14b
0049750c: je 0x18049754e
0049750e: xor r8d, r8d
00497511: mov edx, 3
00497516: xor ecx, ecx
00497518: call 0x1826bcca0
0049751d: mov rcx, qword ptr [rip + 0x2f0c65c]
00497524: mov ebx, eax
00497526: cmp dword ptr [rcx + 0xe0], 0
0049752d: jne 0x180497534
0049752f: call 0x180318580
00497534: movss xmm2, dword ptr [rip + 0x24e460c]
0049753c: xor r9d, r9d
0049753f: movss xmm1, dword ptr [rip + 0x24e45f5]
00497547: mov ecx, ebx
00497549: call 0x18066a930
0049754e: mov rsi, qword ptr [rsp + 0x48]
00497553: mov rbx, qword ptr [rsp + 0x50]
00497558: add rsp, 0x20
0049755c: pop r14
0049755e: pop rdi
0049755f: pop rbp
00497560: ret
00497561: call 0x180363610
00497566: int3
00497567: call 0x180363600
0049756c: int3
0049756d: int3
0049756e: int3
0049756f: int3
