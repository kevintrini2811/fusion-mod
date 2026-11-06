00497790: sub rsp, 0x48
00497794: mov r11, rdx
00497797: test rdx, rdx
0049779a: je 0x1804977db
0049779c: mov rax, qword ptr [rdx]
0049779f: mov r8, rcx
004977a2: mov edx, dword ptr [rcx + 0xf0]
004977a8: mov r9d, 4
004977ae: mov r10, qword ptr [rax + 0x1f8]
004977b5: mov rax, qword ptr [rax + 0x200]
004977bc: mov qword ptr [rsp + 0x30], rax
004977c1: mov eax, dword ptr [rcx + 0x194]
004977c7: mov rcx, r11
004977ca: mov byte ptr [rsp + 0x28], 0
004977cf: mov dword ptr [rsp + 0x20], eax
004977d3: call r10
004977d6: add rsp, 0x48
004977da: ret
004977db: call 0x180363610
004977e0: int3
004977e1: int3
004977e2: int3
004977e3: int3
004977e4: int3
004977e5: int3
004977e6: int3
004977e7: int3
004977e8: int3
004977e9: int3
004977ea: int3
004977eb: int3
004977ec: int3
004977ed: int3
004977ee: int3
004977ef: int3
