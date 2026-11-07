00683df0: push rbx
00683df2: sub rsp, 0x20
00683df6: cmp byte ptr [rip + 0x2f16213], 0
00683dfd: mov rbx, rcx
00683e00: jne 0x180683e15
00683e02: lea rcx, [rip + 0x2cf833f]
00683e09: call 0x1803633b0
00683e0e: mov byte ptr [rip + 0x2f161fb], 1
00683e15: mov rcx, qword ptr [rip + 0x2cf832c]
00683e1c: cmp dword ptr [rcx + 0xe0], 0
00683e23: jne 0x180683e2a
00683e25: call 0x180318580
00683e2a: xor edx, edx
00683e2c: mov rcx, rbx
00683e2f: add rsp, 0x20
00683e33: pop rbx
00683e34: jmp 0x1809d5df0
00683e39: int3
00683e3a: int3
00683e3b: int3
00683e3c: int3
00683e3d: int3
00683e3e: int3
00683e3f: int3
