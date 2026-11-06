005bff10: push rbx
005bff12: sub rsp, 0x20
005bff16: cmp byte ptr [rip + 0x2fd9b91], 0
005bff1d: mov rbx, rcx
005bff20: jne 0x1805bff41
005bff22: lea rcx, [rip + 0x2de3c57]
005bff29: call 0x1803633b0
005bff2e: lea rcx, [rip + 0x2dbe5b3]
005bff35: call 0x1803633b0
005bff3a: mov byte ptr [rip + 0x2fd9b6d], 1
005bff41: cmp byte ptr [rbx + 0x298], 0
005bff48: jne 0x1805c0012
005bff4e: xor r8d, r8d
005bff51: mov byte ptr [rbx + 0x298], 1
005bff58: xor edx, edx
005bff5a: mov rcx, rbx
005bff5d: call 0x180650200
005bff62: mov rcx, qword ptr [rip + 0x2de3c17]
005bff69: cmp dword ptr [rcx + 0xe0], 0
005bff70: jne 0x1805bff77
005bff72: call 0x180318580
005bff77: movss xmm2, dword ptr [rip + 0x23bbbc9]
005bff7f: xor r9d, r9d
005bff82: movss xmm1, dword ptr [rip + 0x23bbbb2]
005bff8a: mov ecx, 0x4d
005bff8f: call 0x18066a930
005bff94: xor edx, edx
005bff96: mov rcx, rbx
005bff99: call 0x18065fe80
005bff9e: cmp eax, 1
005bffa1: je 0x1805bfff1
005bffa3: xor eax, eax
005bffa5: lea rcx, [rbx + 0x70]
005bffa9: xor edx, edx
005bffab: mov dword ptr [rbx + 0xb0], eax
005bffb1: mov dword ptr [rbx + 0x5c], eax
005bffb4: mov qword ptr [rbx + 0x70], rax
005bffb8: call 0x180362730
005bffbd: mov rcx, qword ptr [rbx + 0x28]
005bffc1: test rcx, rcx
005bffc4: je 0x1805c0018
005bffc6: mov rdx, qword ptr [rip + 0x2dbe51b]
005bffcd: xor r8d, r8d
005bffd0: call 0x182678fd0
005bffd5: mov rdx, qword ptr [rbx]
005bffd8: mov rcx, rbx
005bffdb: mov rax, qword ptr [rdx + 0x608]
005bffe2: mov rdx, qword ptr [rdx + 0x610]
005bffe9: add rsp, 0x20
005bffed: pop rbx
005bffee: jmp rax
005bfff1: mov r8, qword ptr [rbx]
005bfff4: mov edx, 2
005bfff9: mov rcx, rbx
005bfffc: mov rax, qword ptr [r8 + 0x328]
005c0003: mov r8, qword ptr [r8 + 0x330]
005c000a: add rsp, 0x20
005c000e: pop rbx
005c000f: jmp rax
005c0012: add rsp, 0x20
005c0016: pop rbx
005c0017: ret
005c0018: call 0x180363610
005c001d: int3
005c001e: int3
005c001f: int3
