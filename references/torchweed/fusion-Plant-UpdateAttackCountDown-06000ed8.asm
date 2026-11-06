0046fad0: push rbx
0046fad2: sub rsp, 0x40
0046fad6: movaps xmmword ptr [rsp + 0x30], xmm6
0046fadb: mov rbx, rcx
0046fade: movss xmm6, dword ptr [rcx + 0x150]
0046fae6: movaps xmmword ptr [rsp + 0x20], xmm7
0046faeb: movss xmm7, dword ptr [rcx + 0x1d0]
0046faf3: xor ecx, ecx
0046faf5: call 0x1826c5210
0046fafa: addss xmm6, dword ptr [rip + 0x250c046]
0046fb02: mulss xmm0, xmm6
0046fb06: movaps xmm6, xmmword ptr [rsp + 0x30]
0046fb0b: subss xmm7, xmm0
0046fb0f: movss dword ptr [rbx + 0x1d0], xmm7
0046fb17: movaps xmm7, xmmword ptr [rsp + 0x20]
0046fb1c: add rsp, 0x40
0046fb20: pop rbx
0046fb21: ret
0046fb22: int3
0046fb23: int3
0046fb24: int3
0046fb25: int3
0046fb26: int3
0046fb27: int3
0046fb28: int3
0046fb29: int3
0046fb2a: int3
0046fb2b: int3
0046fb2c: int3
0046fb2d: int3
0046fb2e: int3
0046fb2f: int3
