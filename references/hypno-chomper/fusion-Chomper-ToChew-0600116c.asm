0049d220: push rbx
0049d222: sub rsp, 0x20
0049d226: cmp byte ptr [rip + 0x30fbff2], 0
0049d22d: mov rbx, rcx
0049d230: jne 0x18049d245
0049d232: lea rcx, [rip + 0x2f3cb1f]
0049d239: call 0x1803633b0
0049d23e: mov byte ptr [rip + 0x30fbfda], 1
0049d245: cmp byte ptr [rbx + 0x24c], 0
0049d24c: je 0x18049d266
0049d24e: mov rcx, qword ptr [rbx + 0x28]
0049d252: test rcx, rcx
0049d255: je 0x18049d273
0049d257: mov rdx, qword ptr [rip + 0x2f3cafa]
0049d25e: xor r8d, r8d
0049d261: call 0x182678cc0
0049d266: mov byte ptr [rbx + 0x24c], 0
0049d26d: add rsp, 0x20
0049d271: pop rbx
0049d272: ret
0049d273: call 0x180363610
0049d278: int3
0049d279: int3
0049d27a: int3
0049d27b: int3
0049d27c: int3
0049d27d: int3
0049d27e: int3
0049d27f: int3
