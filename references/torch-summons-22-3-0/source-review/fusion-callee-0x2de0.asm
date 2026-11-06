00002de0: mov qword ptr [rsp + 8], rbx
00002de5: mov qword ptr [rsp + 0x10], rsi
00002dea: push rdi
00002deb: sub rsp, 0x40
00002def: mov rax, qword ptr [rdx]
00002df2: mov rbx, r9
00002df5: mov r9d, dword ptr [rsp + 0x70]
00002dfa: mov edi, r8d
00002dfd: movzx r10d, cx
00002e01: mov rsi, rdx
00002e04: add r10, r10
00002e07: mov r8, rbx
00002e0a: mov edx, edi
00002e0c: mov rcx, rsi
00002e0f: mov r11, qword ptr [rax + r10*8 + 0x138]
00002e17: mov rax, qword ptr [rax + r10*8 + 0x140]
00002e1f: mov qword ptr [rsp + 0x30], rax
00002e24: movzx eax, byte ptr [rsp + 0x80]
00002e2c: mov byte ptr [rsp + 0x28], al
00002e30: mov eax, dword ptr [rsp + 0x78]
00002e34: mov dword ptr [rsp + 0x20], eax
00002e38: call r11
00002e3b: mov rbx, qword ptr [rsp + 0x50]
00002e40: mov rsi, qword ptr [rsp + 0x58]
00002e45: add rsp, 0x40
00002e49: pop rdi
00002e4a: ret
