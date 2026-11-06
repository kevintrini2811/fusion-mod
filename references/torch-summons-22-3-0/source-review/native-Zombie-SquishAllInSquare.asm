006f7550: mov qword ptr [rsp + 0x10], rbp
006f7555: mov qword ptr [rsp + 0x18], rsi
006f755a: push rdi
006f755b: push r14
006f755d: push r15
006f755f: sub rsp, 0x20
006f7563: cmp byte ptr [rip + 0x47f4281], 0
006f756a: mov ebp, r9d
006f756d: mov r14d, r8d
006f7570: mov r15d, edx
006f7573: mov rsi, rcx
006f7576: jne 0x6f7597
006f7578: lea rcx, [rip + 0x4588071]
006f757f: call 0x4feb70 ;
006f7584: lea rcx, [rip + 0x458813d]
006f758b: call 0x4feb70 ;
006f7590: mov byte ptr [rip + 0x47f4254], 1
006f7597: mov rax, qword ptr [rsi + 0x18]
006f759b: xor edi, edi
006f759d: mov qword ptr [rsp + 0x40], rbx
006f75a2: mov edx, edi
006f75a4: test rax, rax
006f75a7: je 0x6f7657
006f75ad: nop dword ptr [rax]
006f75b0: mov rcx, qword ptr [rax + 0x78]
006f75b4: test rcx, rcx
006f75b7: je 0x6f7657
006f75bd: cmp edx, dword ptr [rcx + 0x28]
006f75c0: jge 0x6f763e
006f75c2: mov rax, qword ptr [rsi + 0x18]
006f75c6: test rax, rax
006f75c9: je 0x6f7657
006f75cf: mov rcx, qword ptr [rax + 0x78]
006f75d3: test rcx, rcx
006f75d6: je 0x6f7657
006f75d8: mov r8, qword ptr [rip + 0x45880e9]
006f75df: mov edx, edi
006f75e1: call 0x2449330 ;
006f75e6: mov rbx, rax
006f75e9: test rax, rax
006f75ec: je 0x6f7657
006f75ee: cmp r14d, dword ptr [rax + 0x34]
006f75f2: jne 0x6f762c
006f75f4: cmp r15d, dword ptr [rax + 0x74]
006f75f8: jne 0x6f762c
006f75fa: cmp ebp, 1
006f75fd: jne 0x6f760d
006f75ff: xor edx, edx
006f7601: mov rcx, rax
006f7604: call 0x6b2430 ; Reloaded.Gameplay.Plant.IsSpiky
006f7609: test al, al
006f760b: jne 0x6f762c
006f760d: cmp dword ptr [rbx + 0x70], 0x2e
006f7611: je 0x6f762c
006f7613: mov rcx, qword ptr [rsi + 0x18]
006f7617: test rcx, rcx
006f761a: je 0x6f7657
006f761c: inc dword ptr [rcx + 0x2f8]
006f7622: xor edx, edx
006f7624: mov rcx, rbx
006f7627: call 0x6b5cd0 ; Reloaded.Gameplay.Plant.Squish
006f762c: mov rax, qword ptr [rsi + 0x18]
006f7630: inc edi
006f7632: mov edx, edi
006f7634: test rax, rax
006f7637: je 0x6f7657
006f7639: jmp 0x6f75b0 ;
006f763e: mov rbx, qword ptr [rsp + 0x40]
006f7643: mov rbp, qword ptr [rsp + 0x48]
006f7648: mov rsi, qword ptr [rsp + 0x50]
006f764d: add rsp, 0x20
006f7651: pop r15
006f7653: pop r14
006f7655: pop rdi
006f7656: ret
006f7657: call 0x4fee20 ;
006f765c: int3
006f765d: int3
006f765e: int3
006f765f: int3
