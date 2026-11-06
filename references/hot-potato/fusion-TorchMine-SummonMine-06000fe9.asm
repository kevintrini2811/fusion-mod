0048a1c0: mov qword ptr [rsp + 0x18], rbx
0048a1c5: push rbp
0048a1c6: push rdi
0048a1c7: push r14
0048a1c9: sub rsp, 0x80
0048a1d0: cmp byte ptr [rip + 0x310ef7b], 0
0048a1d7: mov rbp, rcx
0048a1da: movaps xmmword ptr [rsp + 0x70], xmm6
0048a1df: jne 0x18048a20c
0048a1e1: lea rcx, [rip + 0x2efdd50]
0048a1e8: call 0x1803633b0
0048a1ed: lea rcx, [rip + 0x2f519a4]
0048a1f4: call 0x1803633b0
0048a1f9: lea rcx, [rip + 0x2f561c8]
0048a200: call 0x1803633b0
0048a205: mov byte ptr [rip + 0x310ef46], 1
0048a20c: mov edi, dword ptr [rbp + 0xf4]
0048a212: xorps xmm6, xmm6
0048a215: inc edi
0048a217: mov qword ptr [rsp + 0xa8], rsi
0048a21f: xor r14d, r14d
0048a222: mov ebx, r14d
0048a225: nop word ptr [rax + rax]
0048a230: mov rcx, qword ptr [rip + 0x2f51961]
0048a237: cmp dword ptr [rcx + 0xe0], r14d
0048a23e: jne 0x18048a245
0048a240: call 0x180318580
0048a245: xor r8d, r8d
0048a248: xor edx, edx
0048a24a: mov rcx, rbx
0048a24d: call 0x1826b9a40
0048a252: test al, al
0048a254: je 0x18048a3a1
0048a25a: mov rax, qword ptr [rip + 0x2efdcd7]
0048a261: cmp dword ptr [rax + 0xe0], r14d
0048a268: jne 0x18048a279
0048a26a: mov rcx, rax
0048a26d: call 0x180318580
0048a272: mov rax, qword ptr [rip + 0x2efdcbf]
0048a279: mov rax, qword ptr [rax + 0xb8]
0048a280: mov rcx, qword ptr [rax]
0048a283: xor eax, eax
0048a285: mov qword ptr [rsp + 0xa0], rax
0048a28d: test rcx, rcx
0048a290: je 0x18048a3c2
0048a296: movsd xmm0, qword ptr [rsp + 0xa0]
0048a29f: mov r9d, 0x58e
0048a2a5: mov r8d, dword ptr [rbp + 0xf8]
0048a2ac: mov edx, edi
0048a2ae: mov qword ptr [rsp + 0x48], r14
0048a2b3: mov qword ptr [rsp + 0x40], r14
0048a2b8: mov byte ptr [rsp + 0x38], 1
0048a2bd: mov byte ptr [rsp + 0x30], al
0048a2c1: movsd qword ptr [rsp + 0x28], xmm0
0048a2c7: mov qword ptr [rsp + 0x20], r14
0048a2cc: call 0x1809df0b0
0048a2d1: mov rcx, qword ptr [rip + 0x2f518c0]
0048a2d8: mov rbx, rax
0048a2db: cmp dword ptr [rcx + 0xe0], r14d
0048a2e2: jne 0x18048a2e9
0048a2e4: call 0x180318580
0048a2e9: xor r8d, r8d
0048a2ec: xor edx, edx
0048a2ee: mov rcx, rbx
0048a2f1: call 0x1826b9be0
0048a2f6: test al, al
0048a2f8: je 0x18048a386
0048a2fe: mov rax, qword ptr [rip + 0x2f560c3]
0048a305: cmp dword ptr [rax + 0xe0], r14d
0048a30c: jne 0x18048a31d
0048a30e: mov rcx, rax
0048a311: call 0x180318580
0048a316: mov rax, qword ptr [rip + 0x2f560ab]
0048a31d: mov rax, qword ptr [rax + 0xb8]
0048a324: mov rsi, qword ptr [rax]
0048a327: test rbx, rbx
0048a32a: je 0x18048a3c2
0048a330: mov rdx, qword ptr [rbx + 0x38]
0048a334: test rdx, rdx
0048a337: je 0x18048a3c2
0048a33d: xor r8d, r8d
0048a340: lea rcx, [rsp + 0x60]
0048a345: call 0x1826c8510
0048a34a: movsd xmm0, qword ptr [rax]
0048a34e: movaps xmm1, xmm0
0048a351: shufps xmm1, xmm1, 0x55
0048a355: test rsi, rsi
0048a358: je 0x18048a3c2
0048a35a: mov r9d, dword ptr [rbx + 0xf8]
0048a361: mov edx, 0xb
0048a366: unpcklps xmm0, xmm1
0048a369: mov rcx, rsi
0048a36c: mov qword ptr [rsp + 0x30], r14
0048a371: movq r8, xmm0
0048a376: movss dword ptr [rsp + 0x28], xmm6
0048a37c: mov byte ptr [rsp + 0x20], 1
0048a381: call 0x180465c40
0048a386: mov rax, qword ptr [rbp + 0x30]
0048a38a: inc edi
0048a38c: test rax, rax
0048a38f: je 0x18048a3c2
0048a391: mov eax, dword ptr [rax + 0x154]
0048a397: dec eax
0048a399: cmp edi, eax
0048a39b: jle 0x18048a230
0048a3a1: mov rsi, qword ptr [rsp + 0xa8]
0048a3a9: mov rbx, qword ptr [rsp + 0xb0]
0048a3b1: movaps xmm6, xmmword ptr [rsp + 0x70]
0048a3b6: add rsp, 0x80
0048a3bd: pop r14
0048a3bf: pop rdi
0048a3c0: pop rbp
0048a3c1: ret 
0048a3c2: call 0x180363610
0048a3c7: int3 
0048a3c8: int3 
0048a3c9: int3 
0048a3ca: int3 
0048a3cb: int3 
0048a3cc: int3 
0048a3cd: int3 
0048a3ce: int3 
0048a3cf: int3 
