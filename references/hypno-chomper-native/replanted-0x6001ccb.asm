006ebef0: mov qword ptr [rsp + 8], rbx
006ebef5: push rdi
006ebef6: sub rsp, 0x20
006ebefa: cmp byte ptr [rip + 0x47ff8a1], 0
006ebf01: mov rbx, rcx
006ebf04: jne 0x1806ebf19
006ebf06: lea rcx, [rip + 0x454156b]
006ebf0d: call 0x1804feb70
006ebf12: mov byte ptr [rip + 0x47ff889], 1
006ebf19: mov rdi, qword ptr [rbx + 0x1d0]
006ebf20: mov byte ptr [rbx + 0x144], 1
006ebf27: mov rcx, qword ptr [rip + 0x454154a]
006ebf2e: cmp dword ptr [rcx + 0xe4], 0
006ebf35: jne 0x1806ebf3c
006ebf37: call 0x1804feeb0
006ebf3c: xor r8d, r8d
006ebf3f: xor edx, edx
006ebf41: mov rcx, rdi
006ebf44: call 0x183832900
006ebf49: test al, al
006ebf4b: je 0x1806ebf79
006ebf4d: mov rcx, qword ptr [rbx + 0x1d0]
006ebf54: test rcx, rcx
006ebf57: je 0x1806ebf84
006ebf59: xor edx, edx
006ebf5b: call 0x1807c9970
006ebf60: lea rcx, [rbx + 0x1d0]
006ebf67: mov qword ptr [rbx + 0x1d0], 0
006ebf72: xor edx, edx
006ebf74: call 0x1804fdea0
006ebf79: mov rbx, qword ptr [rsp + 0x30]
006ebf7e: add rsp, 0x20
006ebf82: pop rdi
006ebf83: ret
006ebf84: call 0x1804fee20
006ebf89: int3
006ebf8a: int3
006ebf8b: int3
006ebf8c: int3
006ebf8d: int3
006ebf8e: int3
006ebf8f: int3
