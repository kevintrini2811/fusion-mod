00705fe0: mov qword ptr [rsp + 0x10], rbx
00705fe5: push rdi
00705fe6: sub rsp, 0x20
00705fea: cmp byte ptr [rip + 0x2e942fb], 0
00705ff1: mov rbx, rdx
00705ff4: mov rdi, rcx
00705ff7: jne 0x18070600c
00705ff9: lea rcx, [rip + 0x2cd5b98]
00706000: call 0x1803633b0
00706005: mov byte ptr [rip + 0x2e942e0], 1
0070600c: xor r9d, r9d
0070600f: mov r8b, 1
00706012: mov rdx, rbx
00706015: mov rcx, rdi
00706018: call 0x1805efbc0
0070601d: test rbx, rbx
00706020: je 0x18070607e
00706022: mov rcx, qword ptr [rip + 0x2cd5b6f]
00706029: mov qword ptr [rsp + 0x30], rsi
0070602e: mov rsi, qword ptr [rbx + 0x188]
00706035: cmp dword ptr [rcx + 0xe0], 0
0070603c: jne 0x180706043
0070603e: call 0x180318580
00706043: xor r8d, r8d
00706046: xor edx, edx
00706048: mov rcx, rsi
0070604b: call 0x1826b9be0
00706050: mov rsi, qword ptr [rsp + 0x30]
00706055: test al, al
00706057: je 0x180706073
00706059: mov rcx, qword ptr [rbx + 0x188]
00706060: test rcx, rcx
00706063: je 0x18070607e
00706065: mov edx, dword ptr [rdi + 0xdc]
0070606b: xor r8d, r8d
0070606e: call 0x18045e2e0
00706073: mov rbx, qword ptr [rsp + 0x38]
00706078: add rsp, 0x20
0070607c: pop rdi
0070607d: ret 
0070607e: call 0x180363610
00706083: int3 
00706084: int3 
00706085: int3 
00706086: int3 
00706087: int3 
00706088: int3 
00706089: int3 
0070608a: int3 
0070608b: int3 
0070608c: int3 
0070608d: int3 
0070608e: int3 
0070608f: int3 
