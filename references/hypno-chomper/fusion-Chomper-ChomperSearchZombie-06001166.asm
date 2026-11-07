0049caa0: mov rax, rsp
0049caa3: mov qword ptr [rax + 0x10], rbx
0049caa7: mov qword ptr [rax + 0x18], rsi
0049caab: mov qword ptr [rax + 8], rcx
0049caaf: push rdi
0049cab0: push r14
0049cab2: push r15
0049cab4: sub rsp, 0xf0
0049cabb: movaps xmmword ptr [rax - 0x28], xmm6
0049cabf: movaps xmmword ptr [rax - 0x38], xmm7
0049cac3: movaps xmmword ptr [rax - 0x48], xmm8
0049cac8: movaps xmmword ptr [rax - 0x58], xmm9
0049cacd: movaps xmmword ptr [rax - 0x68], xmm10
0049cad2: movaps xmmword ptr [rax - 0x78], xmm11
0049cad7: mov rsi, rcx
0049cada: mov r15, rcx
0049cadd: mov qword ptr [rsp + 0x40], rcx
0049cae2: cmp byte ptr [rip + 0x30fc731], 0
0049cae9: jne 0x18049cb5e
0049caeb: lea rcx, [rip + 0x2f1d8b6]
0049caf2: call 0x1803633b0
0049caf7: lea rcx, [rip + 0x2edd7a2]
0049cafe: call 0x1803633b0
0049cb03: lea rcx, [rip + 0x2edd83e]
0049cb0a: call 0x1803633b0
0049cb0f: lea rcx, [rip + 0x2edd8e2]
0049cb16: call 0x1803633b0
0049cb1b: lea rcx, [rip + 0x2f2a93e]
0049cb22: call 0x1803633b0
0049cb27: lea rcx, [rip + 0x2f24d9a]
0049cb2e: call 0x1803633b0
0049cb33: lea rcx, [rip + 0x2ee18e6]
0049cb3a: call 0x1803633b0
0049cb3f: lea rcx, [rip + 0x2f3f052]
0049cb46: call 0x1803633b0
0049cb4b: lea rcx, [rip + 0x2f35bc6]
0049cb52: call 0x1803633b0
0049cb57: mov byte ptr [rip + 0x30fc6bc], 1
0049cb5e: xorps xmm0, xmm0
0049cb61: xor eax, eax
0049cb63: movups xmmword ptr [rsp + 0x70], xmm0
0049cb68: mov qword ptr [rsp + 0x80], rax
0049cb70: xor ebx, ebx
0049cb72: mov qword ptr [rsp + 0x30], rbx
0049cb77: mov qword ptr [rsp + 0x128], rbx
0049cb7f: mov rax, qword ptr [rsi + 0x30]
0049cb83: test rax, rax
0049cb86: je 0x18049cf6a
0049cb8c: mov rdx, qword ptr [rax + 0x70]
0049cb90: test rdx, rdx
0049cb93: je 0x18049cf6a
0049cb99: mov r8, qword ptr [rip + 0x2f24d28]
0049cba0: lea rcx, [rsp + 0x58]
0049cba5: call 0x18169b560
0049cbaa: movups xmm0, xmmword ptr [rsp + 0x58]
0049cbaf: movups xmmword ptr [rsp + 0x70], xmm0
0049cbb4: movsd xmm1, qword ptr [rsp + 0x68]
0049cbba: movsd qword ptr [rsp + 0x80], xmm1
0049cbc3: mov qword ptr [rsp + 0x58], rbx
0049cbc8: lea rdi, [rsp + 0x70]
0049cbcd: mov qword ptr [rsp + 0x60], rdi
0049cbd2: mov rdx, qword ptr [rip + 0x2edd76f]
0049cbd9: lea rcx, [rsp + 0x70]
0049cbde: call 0x18142fda0
0049cbe3: test al, al
0049cbe5: je 0x18049cc98
0049cbeb: mov r14, qword ptr [rsp + 0x80]
0049cbf3: mov rcx, qword ptr [rip + 0x2f3ef9e]
0049cbfa: cmp dword ptr [rcx + 0xe0], 0
0049cc01: jne 0x18049cc08
0049cc03: call 0x180318580
0049cc08: xor r8d, r8d
0049cc0b: xor edx, edx
0049cc0d: mov rcx, r14
0049cc10: call 0x1826b9be0
0049cc15: test al, al
0049cc17: je 0x18049cbd2
0049cc19: test r14, r14
0049cc1c: je 0x18049cf76
0049cc22: cmp qword ptr [r14 + 0x148], rsi
0049cc29: jne 0x18049cbd2
0049cc2b: mov r8, qword ptr [rsi]
0049cc2e: mov rax, qword ptr [r8 + 0x5e8]
0049cc35: mov r8, qword ptr [r8 + 0x5f0]
0049cc3c: mov rdx, r14
0049cc3f: mov rcx, rsi
0049cc42: call rax
0049cc44: test al, al
0049cc46: je 0x18049cbd2
0049cc48: mov qword ptr [rsi + 0x90], r14
0049cc4f: lea rcx, [rsi + 0x90]
0049cc56: mov rdx, r14
0049cc59: call 0x180362730
0049cc5e: mov rcx, qword ptr [rsi + 0x28]
0049cc62: test rcx, rcx
0049cc65: je 0x18049cf70
0049cc6b: xor r8d, r8d
0049cc6e: mov rdx, qword ptr [rip + 0x2f35aa3]
0049cc75: call 0x182678fd0
0049cc7a: mov dword ptr [rsi + 0x190], 0x16
0049cc84: mov rdx, qword ptr [rip + 0x2edd615]
0049cc8b: mov rcx, rdi
0049cc8e: call 0x1803eab80
0049cc93: jmp 0x18049cf33
0049cc98: mov rdx, qword ptr [rip + 0x2edd601]
0049cc9f: mov rcx, rdi
0049cca2: call 0x1803eab80
0049cca7: jmp 0x18049ccd7
0049cca9: mov rdx, qword ptr [rip + 0x2edd5f0]
0049ccb0: mov rcx, qword ptr [rsp + 0x60]
0049ccb5: call 0x1803eab80
0049ccba: mov rcx, qword ptr [rsp + 0x58]
0049ccbf: test rcx, rcx
0049ccc2: jne 0x18049cf7c
0049ccc8: xor ebx, ebx
0049ccca: mov rsi, qword ptr [rsp + 0x110]
0049ccd2: mov r15, qword ptr [rsp + 0x40]
0049ccd7: mov rdx, qword ptr [rsi + 0x38]
0049ccdb: test rdx, rdx
0049ccde: je 0x18049cf6a
0049cce4: xor r8d, r8d
0049cce7: lea rcx, [rsp + 0x40]
0049ccec: call 0x1826c8510
0049ccf1: movsd xmm6, qword ptr [rax]
0049ccf5: mov edi, dword ptr [rax + 8]
0049ccf8: mov r8, qword ptr [rip + 0x2ee1721]
0049ccff: movss xmm1, dword ptr [rip + 0x24dee41]
0049cd07: lea rcx, [rsp + 0x30]
0049cd0c: call 0x181820660
0049cd11: movsd qword ptr [rsp + 0x40], xmm6
0049cd17: mov dword ptr [rsp + 0x48], edi
0049cd1b: mov qword ptr [rsp + 0x28], rbx
0049cd20: mov qword ptr [rsp + 0x20], rbx
0049cd25: mov r9, qword ptr [rsp + 0x30]
0049cd2a: mov r8, rbx
0049cd2d: lea rdx, [rsp + 0x40]
0049cd32: lea rcx, [rsp + 0x58]
0049cd37: call 0x180a12ef0
0049cd3c: movsd xmm11, qword ptr [rax]
0049cd41: movaps xmm10, xmm11
0049cd45: shufps xmm10, xmm10, 0x55
0049cd4a: cmp byte ptr [rip + 0x30fc2b9], 0
0049cd51: jne 0x18049cd66
0049cd53: lea rcx, [rip + 0x2f09476]
0049cd5a: call 0x1803633b0
0049cd5f: mov byte ptr [rip + 0x30fc2a4], 1
0049cd66: mov rax, qword ptr [rip + 0x2f09463]
0049cd6d: mov rcx, qword ptr [rax + 0xb8]
0049cd74: movss xmm7, dword ptr [rcx + 0x28]
0049cd79: movss xmm8, dword ptr [rcx + 0x2c]
0049cd7f: mov rdx, qword ptr [r15]
0049cd82: mov rax, qword ptr [rdx + 0x5a8]
0049cd89: mov rdx, qword ptr [rdx + 0x5b0]
0049cd90: mov rcx, rsi
0049cd93: call rax
0049cd95: movaps xmm9, xmm0
0049cd99: xor edx, edx
0049cd9b: mov ecx, dword ptr [rsi + 0x4c]
0049cd9e: call 0x1826b66c0
0049cda3: mov edi, eax
0049cda5: mov rcx, qword ptr [rip + 0x2f2a6b4]
0049cdac: cmp dword ptr [rcx + 0xe0], 0
0049cdb3: jne 0x18049cdba
0049cdb5: call 0x180318580
0049cdba: movss xmm6, dword ptr [rip + 0x24deefe]
0049cdc2: mov qword ptr [rsp + 0x28], rbx
0049cdc7: mov dword ptr [rsp + 0x20], edi
0049cdcb: movss xmm3, dword ptr [rip + 0x24ded9d]
0049cdd3: movaps xmm2, xmm9
0049cdd7: movaps xmm0, xmm7
0049cdda: unpcklps xmm0, xmm8
0049cdde: movq rdx, xmm0
0049cde3: movaps xmm1, xmm11
0049cde7: unpcklps xmm1, xmm10
0049cdeb: movq rcx, xmm1
0049cdf0: call 0x180a0f4c0
0049cdf5: mov r14, rax
0049cdf8: mov ecx, ebx
0049cdfa: test rax, rax
0049cdfd: je 0x18049cf6a
0049ce03: lea rdi, [rax + 0x20]
0049ce07: nop word ptr [rax + rax]
0049ce10: cmp ecx, dword ptr [r14 + 0x18]
0049ce14: jge 0x18049cee4
0049ce1a: cmp ebx, dword ptr [r14 + 0x18]
0049ce1e: jae 0x18049cf82
0049ce24: mov rcx, qword ptr [rdi]
0049ce27: test rcx, rcx
0049ce2a: je 0x18049cf6a
0049ce30: mov r8, qword ptr [rip + 0x2f1d571]
0049ce37: lea rdx, [rsp + 0x128]
0049ce3f: call 0x180ac9930
0049ce44: test al, al
0049ce46: je 0x18049ced7
0049ce4c: mov rdx, qword ptr [rsp + 0x128]
0049ce54: test rdx, rdx
0049ce57: je 0x18049cf6a
0049ce5d: mov eax, dword ptr [rsi + 0xf8]
0049ce63: cmp dword ptr [rdx + 0xcc], eax
0049ce69: jne 0x18049ced7
0049ce6b: mov r8, qword ptr [r15]
0049ce6e: mov rax, qword ptr [r8 + 0x5e8]
0049ce75: mov r8, qword ptr [r8 + 0x5f0]
0049ce7c: mov rcx, rsi
0049ce7f: call rax
0049ce81: test al, al
0049ce83: je 0x18049ced7
0049ce85: mov rax, qword ptr [rsp + 0x128]
0049ce8d: test rax, rax
0049ce90: je 0x18049cf6a
0049ce96: mov rdx, qword ptr [rax + 0x38]
0049ce9a: test rdx, rdx
0049ce9d: je 0x18049cf6a
0049cea3: xor r8d, r8d
0049cea6: lea rcx, [rsp + 0x58]
0049ceab: call 0x1826c8510
0049ceb0: movss xmm0, dword ptr [rax]
0049ceb4: comiss xmm6, xmm0
0049ceb7: jbe 0x18049ced7
0049ceb9: movaps xmm6, xmm0
0049cebc: mov rdx, qword ptr [rsp + 0x128]
0049cec4: mov qword ptr [rsi + 0x90], rdx
0049cecb: lea rcx, [rsi + 0x90]
0049ced2: call 0x180362730
0049ced7: inc ebx
0049ced9: add rdi, 8
0049cedd: mov ecx, ebx
0049cedf: jmp 0x18049ce10
0049cee4: mov rbx, qword ptr [rsi + 0x90]
0049ceeb: mov rcx, qword ptr [rip + 0x2f3eca6]
0049cef2: cmp dword ptr [rcx + 0xe0], 0
0049cef9: jne 0x18049cf00
0049cefb: call 0x180318580
0049cf00: xor r8d, r8d
0049cf03: xor edx, edx
0049cf05: mov rcx, rbx
0049cf08: call 0x1826b9be0
0049cf0d: test al, al
0049cf0f: je 0x18049cf33
0049cf11: mov rcx, qword ptr [rsi + 0x28]
0049cf15: test rcx, rcx
0049cf18: je 0x18049cf6a
0049cf1a: xor r8d, r8d
0049cf1d: mov rdx, qword ptr [rip + 0x2f357f4]
0049cf24: call 0x182678fd0
0049cf29: mov dword ptr [rsi + 0x190], 0x16
0049cf33: lea r11, [rsp + 0xf0]
0049cf3b: mov rbx, qword ptr [r11 + 0x28]
0049cf3f: mov rsi, qword ptr [r11 + 0x30]
0049cf43: movaps xmm6, xmmword ptr [r11 - 0x10]
0049cf48: movaps xmm7, xmmword ptr [r11 - 0x20]
0049cf4d: movaps xmm8, xmmword ptr [r11 - 0x30]
0049cf52: movaps xmm9, xmmword ptr [r11 - 0x40]
0049cf57: movaps xmm10, xmmword ptr [r11 - 0x50]
0049cf5c: movaps xmm11, xmmword ptr [r11 - 0x60]
0049cf61: mov rsp, r11
0049cf64: pop r15
0049cf66: pop r14
0049cf68: pop rdi
0049cf69: ret
0049cf6a: call 0x180363610
0049cf6f: nop
0049cf70: call 0x180363610
0049cf75: nop
0049cf76: call 0x180363610
0049cf7b: nop
0049cf7c: call 0x1803636a0
0049cf81: int3
0049cf82: call 0x180363600
0049cf87: int3
0049cf88: int3
0049cf89: int3
0049cf8a: int3
0049cf8b: int3
0049cf8c: int3
0049cf8d: int3
0049cf8e: int3
0049cf8f: int3
