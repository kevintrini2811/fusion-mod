0049d1b0: push rbx
0049d1b2: sub rsp, 0x20
0049d1b6: cmp byte ptr [rip + 0x30fc05c], 0
0049d1bd: mov rbx, rcx
0049d1c0: jne 0x18049d1e1
0049d1c2: lea rcx, [rip + 0x2f32147]
0049d1c9: call 0x1803633b0
0049d1ce: lea rcx, [rip + 0x2f3cb83]
0049d1d5: call 0x1803633b0
0049d1da: mov byte ptr [rip + 0x30fc038], 1
0049d1e1: mov rcx, qword ptr [rbx + 0x28]
0049d1e5: test rcx, rcx
0049d1e8: je 0x18049d219
0049d1ea: mov rdx, qword ptr [rip + 0x2f3211f]
0049d1f1: xor r8d, r8d
0049d1f4: call 0x182678fd0
0049d1f9: mov rcx, qword ptr [rbx + 0x28]
0049d1fd: test rcx, rcx
0049d200: je 0x18049d219
0049d202: mov rdx, qword ptr [rip + 0x2f3cb4f]
0049d209: xor r9d, r9d
0049d20c: xor r8d, r8d
0049d20f: add rsp, 0x20
0049d213: pop rbx
0049d214: jmp 0x182678eb0
0049d219: call 0x180363610
0049d21e: int3
0049d21f: int3
