006afb20: mov qword ptr [rsp + 8], rbx
006afb25: mov qword ptr [rsp + 0x10], rbp
006afb2a: mov qword ptr [rsp + 0x18], rsi
006afb2f: push rdi
006afb30: sub rsp, 0x30
006afb34: cmp byte ptr [rip + 0x483bb05], 0
006afb3b: mov edi, r8d
006afb3e: mov esi, edx
006afb40: mov rbx, rcx
006afb43: jne 0x1806afba0
006afb45: lea rcx, [rip + 0x457b474]
006afb4c: call 0x1804feb70
006afb51: lea rcx, [rip + 0x45b8d68]
006afb58: call 0x1804feb70
006afb5d: lea rcx, [rip + 0x45e895c]
006afb64: call 0x1804feb70
006afb69: lea rcx, [rip + 0x45ce040]
006afb70: call 0x1804feb70
006afb75: lea rcx, [rip + 0x45e8a1c]
006afb7c: call 0x1804feb70
006afb81: lea rcx, [rip + 0x45d86e0]
006afb88: call 0x1804feb70
006afb8d: lea rcx, [rip + 0x45b8c54]
006afb94: call 0x1804feb70
006afb99: mov byte ptr [rip + 0x483baa0], 1
006afba0: xor r9d, r9d
006afba3: mov r8d, edi
006afba6: mov edx, esi
006afba8: mov rcx, rbx
006afbab: call 0x1806b0040
006afbb0: mov rbp, rax
006afbb3: test rax, rax
006afbb6: je 0x1806b001a
006afbbc: mov rcx, qword ptr [rbx + 0x140]
006afbc3: test rcx, rcx
006afbc6: je 0x1806b002f
006afbcc: xor edx, edx
006afbce: call 0x1807c7650
006afbd3: cmp dword ptr [rbx + 0x70], 0x1c
006afbd7: jne 0x1806afc52
006afbd9: cmp edi, 1
006afbdc: jne 0x1806afc52
006afbde: mov rcx, qword ptr [rbx + 0x140]
006afbe5: test rcx, rcx
006afbe8: je 0x1806b002f
006afbee: movss xmm1, dword ptr [rip + 0x35841c6]
006afbf6: xor r9d, r9d
006afbf9: mov r8d, 3
006afbff: call 0x18060f880
006afc04: mov rcx, qword ptr [rbx + 0x140]
006afc0b: test rcx, rcx
006afc0e: je 0x1806b002f
006afc14: movss xmm3, dword ptr [rip + 0x35842d8]
006afc1c: mov r8d, 3
006afc22: mov rdx, qword ptr [rip + 0x45d863f]
006afc29: mov qword ptr [rsp + 0x28], 0
006afc32: mov dword ptr [rsp + 0x20], 3
006afc3a: call 0x1807c8270
006afc3f: movzx eax, dil
006afc43: mov dword ptr [rbx + 0xdc], 0x1a
006afc4d: jmp 0x1806b001a
006afc52: mov rcx, qword ptr [rbx + 0x140]
006afc59: test rcx, rcx
006afc5c: je 0x1806b002f
006afc62: mov rdx, qword ptr [rip + 0x45e8857]
006afc69: xor r9d, r9d
006afc6c: mov r8d, 2
006afc72: call 0x1807c8940
006afc77: test al, al
006afc79: je 0x1806afd9a
006afc7f: movsxd rcx, dword ptr [rbx + 0x70]
006afc83: test ecx, ecx
006afc85: je 0x1806afca5
006afc87: cmp ecx, 0x34
006afc8a: ja 0x1806afc9c
006afc8c: movabs rdx, 0x100000100400a0
006afc96: bt rdx, rcx
006afc9a: jb 0x1806afca5
006afc9c: cmp ecx, 0x28
006afc9f: jne 0x1806afd9a
006afca5: mov rcx, qword ptr [rbx + 0x140]
006afcac: test rcx, rcx
006afcaf: je 0x1806b002f
006afcb5: movss xmm1, dword ptr [rip + 0x35840ff]
006afcbd: xor r9d, r9d
006afcc0: mov r8d, 2
006afcc6: call 0x18060f880
006afccb: mov rcx, qword ptr [rbx + 0x140]
006afcd2: test rcx, rcx
006afcd5: je 0x1806b002f
006afcdb: movss xmm3, dword ptr [rip + 0x3584211]
006afce3: mov r8d, 2
006afce9: mov rdx, qword ptr [rip + 0x45cdec0]
006afcf0: mov qword ptr [rsp + 0x28], 0
006afcf9: mov dword ptr [rsp + 0x20], 3
006afd01: call 0x1807c8270
006afd06: cmp dword ptr [rbx + 0x70], 7
006afd0a: mov dword ptr [rbx + 0xdc], 0x23
006afd14: je 0x1806afd63
006afd16: cmp dword ptr [rbx + 0x70], 0x1c
006afd1a: je 0x1806afd63
006afd1c: cmp dword ptr [rbx + 0x70], 0x34
006afd20: je 0x1806afd63
006afd22: cmp dword ptr [rbx + 0x70], 0x28
006afd26: jne 0x1806b0018
006afd2c: mov rcx, qword ptr [rbx + 0x140]
006afd33: test rcx, rcx
006afd36: je 0x1806b002f
006afd3c: movss xmm1, dword ptr [rip + 0x3584290]
006afd44: xor r9d, r9d
006afd47: mov r8d, 2
006afd4d: call 0x1807c8740
006afd52: mov al, 1
006afd54: mov dword ptr [rbx + 0xdc], 0x64
006afd5e: jmp 0x1806b001a
006afd63: mov rcx, qword ptr [rbx + 0x140]
006afd6a: test rcx, rcx
006afd6d: je 0x1806b002f
006afd73: movss xmm1, dword ptr [rip + 0x35840ad]
006afd7b: xor r9d, r9d
006afd7e: mov r8d, 2
006afd84: call 0x1807c8740
006afd89: mov al, 1
006afd8b: mov dword ptr [rbx + 0xdc], 0x1a
006afd95: jmp 0x1806b001a
006afd9a: cmp dword ptr [rbx + 0x88], 0x20
006afda1: je 0x1806affdc
006afda7: cmp dword ptr [rbx + 0x70], 0x2a
006afdab: je 0x1806aff9c
006afdb1: cmp dword ptr [rbx + 0x70], 0x2b
006afdb5: je 0x1806aff5c
006afdbb: mov rcx, qword ptr [rbx + 0x140]
006afdc2: test rcx, rcx
006afdc5: je 0x1806b002f
006afdcb: mov rdx, qword ptr [rip + 0x45e86ee]
006afdd2: xor r9d, r9d
006afdd5: xor r8d, r8d
006afdd8: call 0x1807c8940
006afddd: mov rcx, rbx
006afde0: test al, al
006afde2: jne 0x1806afe02
006afde4: mov r9d, edi
006afde7: mov qword ptr [rsp + 0x20], 0
006afdf0: mov r8d, esi
006afdf3: mov rdx, rbp
006afdf6: call 0x1806b0580
006afdfb: mov al, 1
006afdfd: jmp 0x1806b001a
006afe02: movss xmm0, dword ptr [rip + 0x35840ea]
006afe0a: mov r9d, 0x14
006afe10: mov rdx, qword ptr [rip + 0x45e86a9]
006afe17: mov r8d, 3
006afe1d: mov qword ptr [rsp + 0x28], 0
006afe26: movss dword ptr [rsp + 0x20], xmm0
006afe2c: call 0x1806b4b80
006afe31: cmp dword ptr [rbx + 0x70], 0xa
006afe35: je 0x1806aff8e
006afe3b: cmp dword ptr [rbx + 0x70], 8
006afe3f: je 0x1806aff4b
006afe45: cmp dword ptr [rbx + 0x70], 0xd
006afe49: je 0x1806aff3a
006afe4f: cmp dword ptr [rbx + 0x70], 0x20
006afe53: je 0x1806aff29
006afe59: cmp dword ptr [rbx + 0x70], 0x27
006afe5d: je 0x1806aff18
006afe63: cmp dword ptr [rbx + 0x70], 0x2c
006afe67: je 0x1806aff18
006afe6d: cmp dword ptr [rbx + 0x70], 0x22
006afe71: je 0x1806afe91
006afe73: cmp dword ptr [rbx + 0x70], 0x1a
006afe77: mov eax, 0x23
006afe7c: mov ecx, 0x1d
006afe81: cmovne eax, ecx
006afe84: mov dword ptr [rbx + 0xdc], eax
006afe8a: mov al, 1
006afe8c: jmp 0x1806b001a
006afe91: mov rcx, qword ptr [rip + 0x457b128]
006afe98: cmp dword ptr [rcx + 0xe4], 0
006afe9f: jne 0x1806afea6
006afea1: call 0x1804feeb0
006afea6: xor edx, edx
006afea8: mov ecx, 4
006afead: call 0x1806cbe80
006afeb2: test eax, eax
006afeb4: jne 0x1806aff07
006afeb6: mov rcx, qword ptr [rbx + 0x140]
006afebd: test rcx, rcx
006afec0: je 0x1806b002f
006afec6: mov rdx, qword ptr [rip + 0x45b891b]
006afecd: xor r9d, r9d
006afed0: xor r8d, r8d
006afed3: call 0x1807c8db0
006afed8: mov rcx, qword ptr [rbx + 0x140]
006afedf: test rcx, rcx
006afee2: je 0x1806b002f
006afee8: mov rdx, qword ptr [rip + 0x45b89d1]
006afeef: xor r9d, r9d
006afef2: mov r8d, 0xffffffff
006afef8: call 0x1807c8db0
006afefd: mov dword ptr [rbx + 0x88], 0x27
006aff07: mov dword ptr [rbx + 0xdc], 0x1e
006aff11: mov al, 1
006aff13: jmp 0x1806b001a
006aff18: mov dword ptr [rbx + 0xdc], 0x24
006aff22: mov al, 1
006aff24: jmp 0x1806b001a
006aff29: mov dword ptr [rbx + 0xdc], 0x20
006aff33: mov al, 1
006aff35: jmp 0x1806b001a
006aff3a: mov dword ptr [rbx + 0xdc], 0x19
006aff44: mov al, 1
006aff46: jmp 0x1806b001a
006aff4b: mov dword ptr [rbx + 0xdc], 0x1d
006aff55: mov al, 1
006aff57: jmp 0x1806b001a
006aff5c: movss xmm0, dword ptr [rip + 0x3583cfc]
006aff64: mov r9d, 0xa
006aff6a: mov rdx, qword ptr [rip + 0x45e854f]
006aff71: mov r8d, 3
006aff77: mov qword ptr [rsp + 0x28], 0
006aff80: mov rcx, rbx
006aff83: movss dword ptr [rsp + 0x20], xmm0
006aff89: call 0x1806b4b80
006aff8e: mov dword ptr [rbx + 0xdc], 0x32
006aff98: mov al, 1
006aff9a: jmp 0x1806b001a
006aff9c: movss xmm0, dword ptr [rip + 0x3584000]
006affa4: mov r9d, 0x14
006affaa: mov rdx, qword ptr [rip + 0x45e850f]
006affb1: mov r8d, 3
006affb7: mov qword ptr [rsp + 0x28], 0
006affc0: mov rcx, rbx
006affc3: movss dword ptr [rsp + 0x20], xmm0
006affc9: call 0x1806b4b80
006affce: mov al, 1
006affd0: mov dword ptr [rbx + 0xdc], 0xc8
006affda: jmp 0x1806b001a
006affdc: movss xmm0, dword ptr [rip + 0x3583f10]
006affe4: mov r9d, 0x14
006affea: mov rdx, qword ptr [rip + 0x45e85a7]
006afff1: mov r8d, 3
006afff7: mov qword ptr [rsp + 0x28], 0
006b0000: mov rcx, rbx
006b0003: movss dword ptr [rsp + 0x20], xmm0
006b0009: call 0x1806b4b80
006b000e: mov dword ptr [rbx + 0xdc], 0x17
006b0018: mov al, 1
006b001a: mov rbx, qword ptr [rsp + 0x40]
006b001f: mov rbp, qword ptr [rsp + 0x48]
006b0024: mov rsi, qword ptr [rsp + 0x50]
006b0029: add rsp, 0x30
006b002d: pop rdi
006b002e: ret
006b002f: call 0x1804fee20
006b0034: int3
006b0035: int3
006b0036: int3
006b0037: int3
006b0038: int3
006b0039: int3
006b003a: int3
006b003b: int3
006b003c: int3
006b003d: int3
006b003e: int3
006b003f: int3
