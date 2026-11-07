0049c870: push rbx
0049c872: sub rsp, 0x20
0049c876: mov rbx, rdx
0049c879: test rdx, rdx
0049c87c: je 0x18049c8ed
0049c87e: cmp byte ptr [rdx + 0x12d], 0
0049c885: jne 0x18049c8d3
0049c887: cmp byte ptr [rdx + 0x135], 0
0049c88e: jne 0x18049c8d3
0049c890: xor r8d, r8d
0049c893: mov edx, 0xd
0049c898: mov rcx, rbx
0049c89b: call 0x1809e65d0
0049c8a0: test al, al
0049c8a2: jne 0x18049c8d3
0049c8a4: mov edx, dword ptr [rbx + 0x68]
0049c8a7: cmp edx, 7
0049c8aa: jle 0x18049c8db
0049c8ac: lea eax, [rdx - 0xa]
0049c8af: cmp eax, 0xe
0049c8b2: ja 0x18049c8ce
0049c8b4: lea eax, [rdx - 0xa]
0049c8b7: movsxd rcx, eax
0049c8ba: lea r8, [rip - 0x49c8c1]
0049c8c1: mov eax, dword ptr [r8 + rcx*4 + 0x49c8f4]
0049c8c9: add rax, r8
0049c8cc: jmp rax
0049c8ce: cmp edx, 0x2c
0049c8d1: jne 0x18049c8e5
0049c8d3: xor al, al
0049c8d5: add rsp, 0x20
0049c8d9: pop rbx
0049c8da: ret
0049c8db: cmp edx, 1
0049c8de: je 0x18049c8d3
0049c8e0: cmp edx, 7
0049c8e3: je 0x18049c8d3
0049c8e5: mov al, 1
0049c8e7: add rsp, 0x20
0049c8eb: pop rbx
0049c8ec: ret
0049c8ed: call 0x180363610
0049c8f2: int3
0049c8f3: nop
0049c8f4: ror eax, cl
0049c8f6: add r13b, spl
0049c8f9: enter 0x49, -0x2d
0049c8fd: enter 0x49, -0x1b
0049c901: enter 0x49, -0x1b
0049c905: enter 0x49, -0x1b
0049c909: enter 0x49, -0x2d
0049c90d: enter 0x49, -0x1b
0049c911: enter 0x49, -0x1b
0049c915: enter 0x49, -0x1b
0049c919: enter 0x49, -0x2d
0049c91d: enter 0x49, -0x2d
0049c921: enter 0x49, -0x2d
0049c925: enter 0x49, -0x2d
0049c929: enter 0x49, -0x2d
