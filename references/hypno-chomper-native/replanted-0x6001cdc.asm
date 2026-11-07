006eacf0: mov qword ptr [rsp + 8], rbx
006eacf5: mov qword ptr [rsp + 0x10], rbp
006eacfa: mov qword ptr [rsp + 0x18], rsi
006eacff: push rdi
006ead00: sub rsp, 0x30
006ead04: cmp byte ptr [rip + 0x4800aa2], 0
006ead0b: mov rdi, rcx
006ead0e: jne 0x1806ead2f
006ead10: lea rcx, [rip + 0x4596739]
006ead17: call 0x1804feb70
006ead1c: lea rcx, [rip + 0x4596805]
006ead23: call 0x1804feb70
006ead28: mov byte ptr [rip + 0x4800a7e], 1
006ead2f: mov rax, qword ptr [rdi + 0x18]
006ead33: xor ebp, ebp
006ead35: mov esi, ebp
006ead37: mov ebx, ebp
006ead39: mov edx, ebp
006ead3b: test rax, rax
006ead3e: je 0x1806eae22
006ead44: nop dword ptr [rax]
006ead48: nop dword ptr [rax + rax]
006ead50: mov rcx, qword ptr [rax + 0x70]
006ead54: test rcx, rcx
006ead57: je 0x1806eae22
006ead5d: cmp edx, dword ptr [rcx + 0x28]
006ead60: jge 0x1806eae0b
006ead66: mov rax, qword ptr [rdi + 0x18]
006ead6a: test rax, rax
006ead6d: je 0x1806eae22
006ead73: mov rcx, qword ptr [rax + 0x70]
006ead77: test rcx, rcx
006ead7a: je 0x1806eae22
006ead80: mov r8, qword ptr [rip + 0x45967a1]
006ead87: mov edx, ebx
006ead89: call 0x182449330
006ead8e: test rax, rax
006ead91: je 0x1806eae22
006ead97: cmp byte ptr [rax + 0x144], bpl
006ead9e: jne 0x1806eadf9
006eada0: cmp dword ptr [rax + 0x74], 1
006eada4: je 0x1806eadf9
006eada6: cmp dword ptr [rax + 0x74], 2
006eadaa: je 0x1806eadf9
006eadac: cmp dword ptr [rax + 0x74], 3
006eadb0: je 0x1806eadf9
006eadb2: cmp dword ptr [rax + 0x70], 0x14
006eadb6: jne 0x1806eadf9
006eadb8: cmp dword ptr [rax + 0xd8], -1
006eadbf: je 0x1806eadf9
006eadc1: mov rcx, qword ptr [rdi + 0x18]
006eadc5: test rcx, rcx
006eadc8: je 0x1806eae22
006eadca: mov r8d, dword ptr [rax + 0x34]
006eadce: mov r9d, 2
006eadd4: mov edx, dword ptr [rax + 0xd8]
006eadda: mov qword ptr [rsp + 0x20], rbp
006eaddf: call 0x18065f2a0
006eade4: test rax, rax
006eade7: je 0x1806eadf9
006eade9: xor edx, edx
006eadeb: mov rcx, rax
006eadee: call 0x1806b3190
006eadf3: test al, al
006eadf5: je 0x1806eadf9
006eadf7: inc esi
006eadf9: mov rax, qword ptr [rdi + 0x18]
006eadfd: inc ebx
006eadff: mov edx, ebx
006eae01: test rax, rax
006eae04: je 0x1806eae22
006eae06: jmp 0x1806ead50
006eae0b: mov rbx, qword ptr [rsp + 0x40]
006eae10: mov eax, esi
006eae12: mov rsi, qword ptr [rsp + 0x50]
006eae17: mov rbp, qword ptr [rsp + 0x48]
006eae1c: add rsp, 0x30
006eae20: pop rdi
006eae21: ret
006eae22: call 0x1804fee20
006eae27: int3
006eae28: int3
006eae29: int3
006eae2a: int3
006eae2b: int3
006eae2c: int3
006eae2d: int3
006eae2e: int3
006eae2f: int3
