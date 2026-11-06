0049ada0: mov qword ptr [rsp + 8], rbx
0049ada5: push rdi
0049ada6: sub rsp, 0x60
0049adaa: mov rax, qword ptr [rcx + 0x30]
0049adae: mov rbx, rcx
0049adb1: test rax, rax
0049adb4: je 0x18049ae35
0049adb6: mov rdx, qword ptr [rcx + 0x38]
0049adba: mov rdi, qword ptr [rax + 0x48]
0049adbe: test rdx, rdx
0049adc1: je 0x18049ae35
0049adc3: xor r8d, r8d
0049adc6: lea rcx, [rsp + 0x50]
0049adcb: call 0x1826c8510
0049add0: movsd xmm3, qword ptr [rax]
0049add4: xorps xmm0, xmm0
0049add7: movss xmm2, dword ptr [rax + 4]
0049addc: addss xmm3, xmm0
0049ade0: addss xmm2, dword ptr [rip + 0x24e0d54]
0049ade8: test rdi, rdi
0049adeb: je 0x18049ae35
0049aded: mov r8d, dword ptr [rbx + 0xf8]
0049adf4: xor eax, eax
0049adf6: mov qword ptr [rsp + 0x40], rax
0049adfb: xor r9d, r9d
0049adfe: mov byte ptr [rsp + 0x38], 1
0049ae03: mov rcx, rdi
0049ae06: mov qword ptr [rsp + 0x30], rax
0049ae0b: mov eax, dword ptr [rbx + 0x194]
0049ae11: unpcklps xmm3, xmm2
0049ae14: mov dword ptr [rsp + 0x28], eax
0049ae18: movq rdx, xmm3
0049ae1d: mov dword ptr [rsp + 0x20], 0x708
0049ae25: call 0x18041e910
0049ae2a: mov rbx, qword ptr [rsp + 0x70]
0049ae2f: add rsp, 0x60
0049ae33: pop rdi
0049ae34: ret 
0049ae35: call 0x180363610
0049ae3a: int3 
0049ae3b: int3 
0049ae3c: int3 
0049ae3d: int3 
0049ae3e: int3 
0049ae3f: int3 
