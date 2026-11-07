006ce700: push rbx
006ce702: sub rsp, 0x20
006ce706: xor edx, edx
006ce708: mov dword ptr [rcx + 0x50], 0x3f800000
006ce70f: mov rbx, rcx
006ce712: mov dword ptr [rcx + 0x54], 0x3f800000
006ce719: call 0x18060f880
006ce71e: xor eax, eax
006ce720: mov byte ptr [rbx + 0x30], 1
006ce724: mov qword ptr [rbx + 0x20], rax
006ce728: mov qword ptr [rbx + 0x28], rax
006ce72c: mov dword ptr [rbx + 0x34], 0xffffffff
006ce733: mov dword ptr [rbx + 0x38], 0x61a80
006ce73a: add rsp, 0x20
006ce73e: pop rbx
006ce73f: ret
