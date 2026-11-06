007cba00: mov qword ptr [rsp + 8], rbx
007cba05: push rdi
007cba06: sub rsp, 0x20
007cba0a: cmp byte ptr [rip + 0x4720470], 0
007cba11: mov rbx, rcx
007cba14: jne 0x1807cba29
007cba16: lea rcx, [rip + 0x4488083]
007cba1d: call 0x1804feb70
007cba22: mov byte ptr [rip + 0x4720458], 1
007cba29: mov ecx, dword ptr [rbx + 0x10]
007cba2c: xor edx, edx
007cba2e: mov rdi, qword ptr [rbx + 0x18]
007cba32: call 0x1807e9150
007cba37: test rdi, rdi
007cba3a: je 0x1807cba93
007cba3c: mov edx, dword ptr [rbx + 0x14]
007cba3f: xor r8d, r8d
007cba42: add edx, eax
007cba44: mov rcx, rdi
007cba47: call 0x1837ea7a0
007cba4c: mov rcx, qword ptr [rbx + 0x18]
007cba50: test rcx, rcx
007cba53: je 0x1807cba93
007cba55: mov rax, qword ptr [rip + 0x4488044]
007cba5c: xor r8d, r8d
007cba5f: cmp dword ptr [rbx + 0x10], 0xc3500
007cba66: mov rdx, qword ptr [rax + 0xb8]
007cba6d: jge 0x1807cba81
007cba6f: mov edx, dword ptr [rdx + 4]
007cba72: mov rbx, qword ptr [rsp + 0x30]
007cba77: add rsp, 0x20
007cba7b: pop rdi
007cba7c: jmp 0x1837ea6d0
007cba81: mov edx, dword ptr [rdx + 8]
007cba84: mov rbx, qword ptr [rsp + 0x30]
007cba89: add rsp, 0x20
007cba8d: pop rdi
007cba8e: jmp 0x1837ea6d0
007cba93: call 0x1804fee20
007cba98: int3 
007cba99: int3 
007cba9a: int3 
007cba9b: int3 
007cba9c: int3 
007cba9d: int3 
007cba9e: int3 
007cba9f: int3 
