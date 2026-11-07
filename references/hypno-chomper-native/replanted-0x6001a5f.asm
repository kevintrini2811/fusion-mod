006ac470: mov qword ptr [rsp + 8], rbx
006ac475: mov qword ptr [rsp + 0x10], rbp
006ac47a: mov qword ptr [rsp + 0x18], rsi
006ac47f: push rdi
006ac480: sub rsp, 0x40
006ac484: cmp byte ptr [rip + 0x483f1e4], 0
006ac48b: mov esi, r8d
006ac48e: mov ebp, edx
006ac490: mov rbx, rcx
006ac493: jne 0x1806ac4a8
006ac495: lea rcx, [rip + 0x45ec024]
006ac49c: call 0x1804feb70
006ac4a1: mov byte ptr [rip + 0x483f1c7], 1
006ac4a8: cmp byte ptr [rip + 0x483f1a9], 0
006ac4af: mov dword ptr [rbx + 0x88], 0x26
006ac4b9: mov dword ptr [rbx + 0xdc], 0xf0
006ac4c3: mov rdi, qword ptr [rip + 0x45ebff6]
006ac4ca: jne 0x1806ac4eb
006ac4cc: lea rcx, [rip + 0x45e65ad]
006ac4d3: call 0x1804feb70
006ac4d8: lea rcx, [rip + 0x45e4d19]
006ac4df: call 0x1804feb70
006ac4e4: mov byte ptr [rip + 0x483f16d], 1
006ac4eb: test rdi, rdi
006ac4ee: je 0x1806ac5b1
006ac4f4: mov r8, qword ptr [rip + 0x45e4cfd]
006ac4fb: xor r9d, r9d
006ac4fe: mov rdx, qword ptr [rip + 0x45e657b]
006ac505: mov rcx, rdi
006ac508: call 0x182a24e50
006ac50d: mov rcx, qword ptr [rbx + 0x140]
006ac514: mov rdi, rax
006ac517: test rcx, rcx
006ac51a: je 0x1806ac5b1
006ac520: movss xmm1, dword ptr [rip + 0x3587894]
006ac528: xor r9d, r9d
006ac52b: xor r8d, r8d
006ac52e: call 0x18060f880
006ac533: mov rcx, qword ptr [rbx + 0x140]
006ac53a: test rcx, rcx
006ac53d: je 0x1806ac5b1
006ac53f: movss xmm3, dword ptr [rip + 0x358786d]
006ac547: xor r8d, r8d
006ac54a: mov qword ptr [rsp + 0x28], 0
006ac553: mov rdx, rdi
006ac556: mov dword ptr [rsp + 0x20], 3
006ac55e: call 0x1807c8270
006ac563: mov rcx, qword ptr [rbx + 0x140]
006ac56a: lea eax, [rbp - 0x2f]
006ac56d: mov dword ptr [rbx + 0xcc], eax
006ac573: mov dword ptr [rbx + 0xd0], esi
006ac579: movdqa xmm0, xmmword ptr [rip + 0x358770f]
006ac581: test rcx, rcx
006ac584: je 0x1806ac5b1
006ac586: xor r9d, r9d
006ac589: movdqa xmmword ptr [rsp + 0x30], xmm0
006ac58f: xor r8d, r8d
006ac592: lea rdx, [rsp + 0x30]
006ac597: call 0x18060f880
006ac59c: mov rbx, qword ptr [rsp + 0x50]
006ac5a1: mov rbp, qword ptr [rsp + 0x58]
006ac5a6: mov rsi, qword ptr [rsp + 0x60]
006ac5ab: add rsp, 0x40
006ac5af: pop rdi
006ac5b0: ret
006ac5b1: call 0x1804fee20
006ac5b6: int3
006ac5b7: int3
006ac5b8: int3
006ac5b9: int3
006ac5ba: int3
006ac5bb: int3
006ac5bc: int3
006ac5bd: int3
006ac5be: int3
006ac5bf: int3
