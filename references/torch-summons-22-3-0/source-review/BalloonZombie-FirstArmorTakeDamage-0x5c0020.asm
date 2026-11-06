005c0020: sub rsp, 0x28
005c0024: cmp edx, dword ptr [rcx + 0xb0]
005c002a: jl 0x1805c0046
005c002c: mov rdx, qword ptr [rcx]
005c002f: mov rax, qword ptr [rdx + 0x628]
005c0036: mov rdx, qword ptr [rdx + 0x630]
005c003d: call rax
005c003f: xor eax, eax
005c0041: add rsp, 0x28
005c0045: ret
005c0046: sub dword ptr [rcx + 0xb0], edx
005c004c: xor eax, eax
005c004e: add rsp, 0x28
005c0052: ret
005c0053: int3
005c0054: int3
005c0055: int3
005c0056: int3
005c0057: int3
005c0058: int3
005c0059: int3
005c005a: int3
005c005b: int3
005c005c: int3
005c005d: int3
005c005e: int3
005c005f: int3
