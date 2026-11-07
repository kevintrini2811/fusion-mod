006f5c60: push rbx
006f5c62: sub rsp, 0x20
006f5c66: cmp dword ptr [rcx + 0x10c], 0
006f5c6d: mov rbx, rcx
006f5c70: jle 0x1806f5cb6
006f5c72: mov dword ptr [rcx + 0x10c], 0
006f5c7c: mov rcx, qword ptr [rcx + 0x1c8]
006f5c83: test rcx, rcx
006f5c86: je 0x1806f5c8f
006f5c88: xor edx, edx
006f5c8a: call 0x1807b3d70
006f5c8f: cmp dword ptr [rbx + 0x70], 0x10
006f5c93: jne 0x1806f5ca2
006f5c95: xor r8d, r8d
006f5c98: mov dl, 1
006f5c9a: mov rcx, rbx
006f5c9d: call 0x1806e6750
006f5ca2: xor edx, edx
006f5ca4: mov rcx, rbx
006f5ca7: call 0x1806fbf10
006f5cac: xor edx, edx
006f5cae: mov rcx, rbx
006f5cb1: call 0x1806f7c70
006f5cb6: cmp dword ptr [rbx + 0x104], 0
006f5cbd: jle 0x1806f5cd8
006f5cbf: xor edx, edx
006f5cc1: mov dword ptr [rbx + 0x104], 0
006f5ccb: mov rcx, rbx
006f5cce: add rsp, 0x20
006f5cd2: pop rbx
006f5cd3: jmp 0x1806fbf10
006f5cd8: add rsp, 0x20
006f5cdc: pop rbx
006f5cdd: ret
006f5cde: int3
006f5cdf: int3
