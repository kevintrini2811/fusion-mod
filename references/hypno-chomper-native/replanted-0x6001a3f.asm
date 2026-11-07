006b8760: push rbx
006b8762: sub rsp, 0x30
006b8766: cmp byte ptr [rip + 0x4832ee8], 0
006b876d: mov rbx, rcx
006b8770: jne 0x1806b87a9
006b8772: lea rcx, [rip + 0x45bc9ef]
006b8779: call 0x1804feb70
006b877e: lea rcx, [rip + 0x45dab23]
006b8785: call 0x1804feb70
006b878a: lea rcx, [rip + 0x45809d7]
006b8791: call 0x1804feb70
006b8796: lea rcx, [rip + 0x45f5633]
006b879d: call 0x1804feb70
006b87a2: mov byte ptr [rip + 0x4832eac], 1
006b87a9: cmp dword ptr [rbx + 0x88], 1
006b87b0: mov qword ptr [rsp + 0x48], rsi
006b87b5: mov qword ptr [rsp + 0x50], rdi
006b87ba: je 0x1806b8ad1
006b87c0: cmp dword ptr [rbx + 0x88], 0xa
006b87c7: je 0x1806b896a
006b87cd: cmp dword ptr [rbx + 0x88], 0xb
006b87d4: je 0x1806b88be
006b87da: cmp dword ptr [rbx + 0x88], 0xd
006b87e1: je 0x1806b8840
006b87e3: cmp dword ptr [rbx + 0x88], 0xe
006b87ea: je 0x1806b87f5
006b87ec: cmp dword ptr [rbx + 0x88], 0xc
006b87f3: jne 0x1806b8830
006b87f5: mov rcx, qword ptr [rbx + 0x140]
006b87fc: test rcx, rcx
006b87ff: je 0x1806b8b82
006b8805: xor r8d, r8d
006b8808: xor edx, edx
006b880a: call 0x1807c7a30
006b880f: test eax, eax
006b8811: jle 0x1806b8830
006b8813: movss xmm1, dword ptr [rip + 0x357b599]
006b881b: xor r8d, r8d
006b881e: mov rcx, rbx
006b8821: call 0x1806b4c50
006b8826: mov dword ptr [rbx + 0x88], 1
006b8830: mov rdi, qword ptr [rsp + 0x50]
006b8835: mov rsi, qword ptr [rsp + 0x48]
006b883a: add rsp, 0x30
006b883e: pop rbx
006b883f: ret
006b8840: cmp dword ptr [rbx + 0xa0], 0
006b8847: jne 0x1806b8830
006b8849: mov rcx, qword ptr [rbx + 0x140]
006b8850: test rcx, rcx
006b8853: je 0x1806b8b82
006b8859: movss xmm1, dword ptr [rip + 0x357b55b]
006b8861: xor r9d, r9d
006b8864: xor r8d, r8d
006b8867: call 0x18060f880
006b886c: mov rcx, qword ptr [rbx + 0x140]
006b8873: test rcx, rcx
006b8876: je 0x1806b8b82
006b887c: movss xmm3, dword ptr [rip + 0x357b530]
006b8884: xor r8d, r8d
006b8887: mov rdx, qword ptr [rip + 0x45daa1a]
006b888e: mov qword ptr [rsp + 0x28], 0
006b8897: mov dword ptr [rsp + 0x20], 3
006b889f: call 0x1807c8270
006b88a4: mov dword ptr [rbx + 0x88], 0xe
006b88ae: mov rdi, qword ptr [rsp + 0x50]
006b88b3: mov rsi, qword ptr [rsp + 0x48]
006b88b8: add rsp, 0x30
006b88bc: pop rbx
006b88bd: ret
006b88be: mov rcx, qword ptr [rbx + 0x140]
006b88c5: test rcx, rcx
006b88c8: je 0x1806b8b82
006b88ce: xor r8d, r8d
006b88d1: xor edx, edx
006b88d3: call 0x1807c7a30
006b88d8: test eax, eax
006b88da: jle 0x1806b8830
006b88e0: mov rcx, qword ptr [rbx + 0x140]
006b88e7: test rcx, rcx
006b88ea: je 0x1806b8b82
006b88f0: movss xmm3, dword ptr [rip + 0x357b4c0]
006b88f8: xor r8d, r8d
006b88fb: mov rdx, qword ptr [rip + 0x4580866]
006b8902: mov qword ptr [rsp + 0x20], 0
006b890b: call 0x1807c8210
006b8910: mov rcx, qword ptr [rbx + 0x10]
006b8914: test rcx, rcx
006b8917: je 0x1806b8b82
006b891d: xor edx, edx
006b891f: call 0x1807df5d0
006b8924: test al, al
006b8926: je 0x1806b8946
006b8928: mov rcx, qword ptr [rbx + 0x140]
006b892f: test rcx, rcx
006b8932: je 0x1806b8b82
006b8938: xor r9d, r9d
006b893b: xor r8d, r8d
006b893e: xorps xmm1, xmm1
006b8941: call 0x1807c8740
006b8946: mov dword ptr [rbx + 0x88], 0xd
006b8950: mov dword ptr [rbx + 0xa0], 0xfa0
006b895a: mov rdi, qword ptr [rsp + 0x50]
006b895f: mov rsi, qword ptr [rsp + 0x48]
006b8964: add rsp, 0x30
006b8968: pop rbx
006b8969: ret
006b896a: cmp dword ptr [rbx + 0xa0], 0
006b8971: jne 0x1806b8830
006b8977: mov rax, qword ptr [rbx + 0x10]
006b897b: test rax, rax
006b897e: je 0x1806b8b82
006b8984: mov r8, qword ptr [rax + 0x58]
006b8988: test r8, r8
006b898b: je 0x1806b8b82
006b8991: mov rdx, qword ptr [rip + 0x45bc7d0]
006b8998: mov ecx, 6
006b899d: mov r9d, 0x3d
006b89a3: mov qword ptr [rsp + 0x40], rbp
006b89a8: call 0x180002040
006b89ad: mov edx, dword ptr [rbx + 0x34]
006b89b0: xor r9d, r9d
006b89b3: xor r8d, r8d
006b89b6: mov rcx, rbx
006b89b9: call 0x1806b0040
006b89be: xor bpl, bpl
006b89c1: mov rdi, rax
006b89c4: test rax, rax
006b89c7: je 0x1806b89e3
006b89c9: cmp dword ptr [rax + 0x70], 0x17
006b89cd: je 0x1806b89db
006b89cf: cmp dword ptr [rax + 0x70], 0x20
006b89d3: je 0x1806b89db
006b89d5: cmp dword ptr [rax + 0x70], 0x19
006b89d9: jne 0x1806b89e3
006b89db: mov bpl, 1
006b89de: xor sil, sil
006b89e1: jmp 0x1806b89eb
006b89e3: xor sil, sil
006b89e6: test rdi, rdi
006b89e9: je 0x1806b8a13
006b89eb: xor edx, edx
006b89ed: mov rcx, rdi
006b89f0: call 0x1806f3670
006b89f5: test al, al
006b89f7: jne 0x1806b8a16
006b89f9: xor edx, edx
006b89fb: mov rcx, rdi
006b89fe: call 0x1806f35e0
006b8a03: test al, al
006b8a05: jne 0x1806b8a13
006b8a07: cmp dword ptr [rdi + 0x74], 0xc
006b8a0b: je 0x1806b8a13
006b8a0d: cmp dword ptr [rdi + 0x74], 0xb
006b8a11: jne 0x1806b8a16
006b8a13: mov sil, 1
006b8a16: test bpl, bpl
006b8a19: mov rbp, qword ptr [rsp + 0x40]
006b8a1e: jne 0x1806b8a6a
006b8a20: test sil, sil
006b8a23: jne 0x1806b8ab7
006b8a29: test rdi, rdi
006b8a2c: je 0x1806b8b82
006b8a32: xor edx, edx
006b8a34: mov rcx, rdi
006b8a37: call 0x1806ec320
006b8a3c: mov rax, qword ptr [rbx + 0x18]
006b8a40: test rax, rax
006b8a43: je 0x1806b8b82
006b8a49: mov byte ptr [rax + 0x239], sil
006b8a50: mov dword ptr [rbx + 0x88], 0xb
006b8a5a: mov rdi, qword ptr [rsp + 0x50]
006b8a5f: mov rsi, qword ptr [rsp + 0x48]
006b8a64: add rsp, 0x30
006b8a68: pop rbx
006b8a69: ret
006b8a6a: mov rax, qword ptr [rbx + 0x10]
006b8a6e: test rax, rax
006b8a71: je 0x1806b8b82
006b8a77: mov r8, qword ptr [rax + 0x58]
006b8a7b: test r8, r8
006b8a7e: je 0x1806b8b82
006b8a84: mov rdx, qword ptr [rip + 0x45bc6dd]
006b8a8b: mov ecx, 6
006b8a90: mov r9d, 1
006b8a96: call 0x180002040
006b8a9b: test rdi, rdi
006b8a9e: je 0x1806b8b82
006b8aa4: xor r9d, r9d
006b8aa7: xor r8d, r8d
006b8aaa: mov edx, 0x28
006b8aaf: mov rcx, rdi
006b8ab2: call 0x1806f8f60
006b8ab7: mov dword ptr [rbx + 0x88], 0xc
006b8ac1: mov rdi, qword ptr [rsp + 0x50]
006b8ac6: mov rsi, qword ptr [rsp + 0x48]
006b8acb: add rsp, 0x30
006b8acf: pop rbx
006b8ad0: ret
006b8ad1: mov edx, dword ptr [rbx + 0x34]
006b8ad4: xor r9d, r9d
006b8ad7: xor r8d, r8d
006b8ada: mov rcx, rbx
006b8add: call 0x1806b0040
006b8ae2: test rax, rax
006b8ae5: je 0x1806b8830
006b8aeb: mov rcx, qword ptr [rbx + 0x140]
006b8af2: test rcx, rcx
006b8af5: je 0x1806b8b82
006b8afb: movss xmm1, dword ptr [rip + 0x357b2b9]
006b8b03: xor r9d, r9d
006b8b06: xor r8d, r8d
006b8b09: call 0x18060f880
006b8b0e: mov rcx, qword ptr [rbx + 0x140]
006b8b15: test rcx, rcx
006b8b18: je 0x1806b8b82
006b8b1a: xor r9d, r9d
006b8b1d: xor r8d, r8d
006b8b20: mov edx, 3
006b8b25: call 0x1807c8640
006b8b2a: mov rcx, qword ptr [rbx + 0x140]
006b8b31: test rcx, rcx
006b8b34: je 0x1806b8b82
006b8b36: movss xmm3, dword ptr [rip + 0x357b3b2]
006b8b3e: xor r8d, r8d
006b8b41: mov rdx, qword ptr [rip + 0x45f5288]
006b8b48: mov qword ptr [rsp + 0x28], 0
006b8b51: mov dword ptr [rsp + 0x20], 3
006b8b59: call 0x1807c8270
006b8b5e: mov rdi, qword ptr [rsp + 0x50]
006b8b63: mov rsi, qword ptr [rsp + 0x48]
006b8b68: mov dword ptr [rbx + 0x88], 0xa
006b8b72: mov dword ptr [rbx + 0xa0], 0x46
006b8b7c: add rsp, 0x30
006b8b80: pop rbx
006b8b81: ret
006b8b82: call 0x1804fee20
006b8b87: int3
006b8b88: int3
006b8b89: int3
006b8b8a: int3
006b8b8b: int3
006b8b8c: int3
006b8b8d: int3
006b8b8e: int3
006b8b8f: int3
