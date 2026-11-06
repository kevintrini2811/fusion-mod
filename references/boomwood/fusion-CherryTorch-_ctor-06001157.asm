0049b260: push rbx
0049b262: sub rsp, 0x20
0049b266: cmp byte ptr [rip + 0x30fdfa5], 0
0049b26d: mov rbx, rcx
0049b270: jne 0x18049b285
0049b272: lea rcx, [rip + 0x2f3767f]
0049b279: call 0x1803633b0
0049b27e: mov byte ptr [rip + 0x30fdf8d], 1
0049b285: mov rcx, qword ptr [rip + 0x2f3766c]
0049b28c: mov edx, 3
0049b291: call 0x1803627b0
0049b296: lea rcx, [rbx + 0x258]
0049b29d: mov qword ptr [rbx + 0x258], rax
0049b2a4: mov rdx, rax
0049b2a7: call 0x180362730
0049b2ac: xor edx, edx
0049b2ae: mov rcx, rbx
0049b2b1: add rsp, 0x20
0049b2b5: pop rbx
0049b2b6: jmp 0x180464260
0049b2bb: int3 
0049b2bc: int3 
0049b2bd: int3 
0049b2be: int3 
0049b2bf: int3 
