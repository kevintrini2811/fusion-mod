006f46e0: mov dword ptr [rsp + 0x10], edx
006f46e4: push rbx
006f46e5: push rsi
006f46e6: push rdi
006f46e7: push r14
006f46e9: sub rsp, 0x68
006f46ed: cmp byte ptr [rip + 0x47f70b8], 0
006f46f4: mov r14d, edx
006f46f7: mov rsi, rcx
006f46fa: jne 0x1806f471b
006f46fc: lea rcx, [rip + 0x45368bd]
006f4703: call 0x1804feb70
006f4708: lea rcx, [rip + 0x4592e71]
006f470f: call 0x1804feb70
006f4714: mov byte ptr [rip + 0x47f7091], 1
006f471b: cmp byte ptr [rip + 0x47f708b], 0
006f4722: jne 0x1806f4743
006f4724: lea rcx, [rip + 0x458cd25]
006f472b: call 0x1804feb70
006f4730: lea rcx, [rip + 0x458cdf1]
006f4737: call 0x1804feb70
006f473c: mov byte ptr [rip + 0x47f706a], 1
006f4743: mov rax, qword ptr [rsi + 0x18]
006f4747: xor edi, edi
006f4749: mov qword ptr [rsp + 0xa0], rbp
006f4751: xor ebx, ebx
006f4753: mov qword ptr [rsp + 0x60], r12
006f4758: xor edx, edx
006f475a: mov qword ptr [rsp + 0x58], r13
006f475f: mov qword ptr [rsp + 0x50], r15
006f4764: mov dword ptr [rsp + 0x90], edi
006f476b: test rax, rax
006f476e: je 0x1806f4b42
006f4774: nop dword ptr [rax]
006f4778: nop dword ptr [rax + rax]
006f4780: mov rcx, qword ptr [rax + 0x70]
006f4784: test rcx, rcx
006f4787: je 0x1806f4b42
006f478d: mov eax, dword ptr [rcx + 0x28]
006f4790: mov rcx, qword ptr [rsi + 0x18]
006f4794: test rcx, rcx
006f4797: je 0x1806f4b42
006f479d: cmp edx, eax
006f479f: jge 0x1806f4850
006f47a5: mov rcx, qword ptr [rcx + 0x70]
006f47a9: test rcx, rcx
006f47ac: je 0x1806f4b42
006f47b2: mov r8, qword ptr [rip + 0x458cd6f]
006f47b9: mov edx, ebx
006f47bb: call 0x182449330
006f47c0: test rax, rax
006f47c3: je 0x1806f4b42
006f47c9: cmp byte ptr [rax + 0x144], 0
006f47d0: jne 0x1806f483a
006f47d2: cmp dword ptr [rax + 0x74], 1
006f47d6: je 0x1806f483a
006f47d8: cmp dword ptr [rax + 0x74], 2
006f47dc: je 0x1806f483a
006f47de: cmp dword ptr [rax + 0x74], 3
006f47e2: je 0x1806f483a
006f47e4: cmp dword ptr [rax + 0x70], 0x14
006f47e8: jne 0x1806f483a
006f47ea: cmp dword ptr [rax + 0xd8], -1
006f47f1: je 0x1806f483a
006f47f3: mov rcx, qword ptr [rsi + 0x18]
006f47f7: test rcx, rcx
006f47fa: je 0x1806f4b42
006f4800: mov r8d, dword ptr [rax + 0x34]
006f4804: mov r9d, 2
006f480a: mov edx, dword ptr [rax + 0xd8]
006f4810: mov qword ptr [rsp + 0x20], 0
006f4819: call 0x18065f2a0
006f481e: test rax, rax
006f4821: je 0x1806f483a
006f4823: xor edx, edx
006f4825: mov rcx, rax
006f4828: call 0x1806b3190
006f482d: test al, al
006f482f: je 0x1806f483a
006f4831: inc edi
006f4833: mov dword ptr [rsp + 0x90], edi
006f483a: mov rax, qword ptr [rsi + 0x18]
006f483e: inc ebx
006f4840: mov edx, ebx
006f4842: test rax, rax
006f4845: je 0x1806f4b42
006f484b: jmp 0x1806f4780
006f4850: xor edx, edx
006f4852: call 0x180658b70
006f4857: dec eax
006f4859: mov dword ptr [rsp + 0xa8], eax
006f4860: mov rcx, qword ptr [rip + 0x4592d19]
006f4867: mov edx, 0x36
006f486c: call 0x1804fdf20
006f4871: xor r8d, r8d
006f4874: xor ebp, ebp
006f4876: xor r15d, r15d
006f4879: mov qword ptr [rsp + 0x38], r8
006f487e: mov r12, rax
006f4881: xor eax, eax
006f4883: mov qword ptr [rsp + 0x30], rax
006f4888: cmp r14d, -1
006f488c: je 0x1806f4897
006f488e: cmp r14d, r15d
006f4891: jne 0x1806f4a18
006f4897: xor edi, edi
006f4899: lea rax, [rax + rax*2]
006f489d: xor ebx, ebx
006f489f: lea rax, [rax + 9]
006f48a3: mov qword ptr [rsp + 0x40], rbx
006f48a8: lea r14, [r12 + rax*4]
006f48ac: nop dword ptr [rax]
006f48b0: mov rcx, qword ptr [rsi + 0x18]
006f48b4: mov r13d, 1
006f48ba: test rcx, rcx
006f48bd: je 0x1806f4b42
006f48c3: xor r9d, r9d
006f48c6: mov r8d, edi
006f48c9: mov edx, r15d
006f48cc: call 0x18065def0
006f48d1: test rax, rax
006f48d4: jne 0x1806f49f1
006f48da: mov rdx, qword ptr [rsi + 0x18]
006f48de: test rdx, rdx
006f48e1: je 0x1806f4b42
006f48e7: mov rdx, qword ptr [rdx + 0x120]
006f48ee: test rdx, rdx
006f48f1: je 0x1806f4b42
006f48f7: mov rax, qword ptr [rdx + 0x10]
006f48fb: cmp r15d, dword ptr [rax]
006f48fe: jae 0x1806f4b48
006f4904: mov rcx, qword ptr [rax + 0x10]
006f4908: cmp edi, ecx
006f490a: jae 0x1806f4b48
006f4910: mov r8, qword ptr [rsp + 0x38]
006f4915: imul rcx, r8
006f4919: add rcx, rbx
006f491c: cmp dword ptr [rdx + rcx*4 + 0x20], 2
006f4921: je 0x1806f49ea
006f4927: mov rcx, qword ptr [rsi + 0x18]
006f492b: mov r9d, 2
006f4931: mov r8d, edi
006f4934: mov qword ptr [rsp + 0x20], 0
006f493d: mov edx, r15d
006f4940: call 0x18065f2a0
006f4945: mov rbx, rax
006f4948: test rax, rax
006f494b: je 0x1806f4981
006f494d: mov eax, dword ptr [rsp + 0xa8]
006f4954: cmp dword ptr [rsp + 0x90], eax
006f495b: jne 0x1806f496f
006f495d: xor edx, edx
006f495f: mov rcx, rbx
006f4962: call 0x1806b3190
006f4967: test al, al
006f4969: jne 0x1806f49f1
006f496f: cmp dword ptr [rbx + 0x70], 0xb
006f4973: je 0x1806f49f1
006f4975: cmp dword ptr [rbx + 0x70], 0x2f
006f4979: je 0x1806f49f1
006f497b: mov r13d, 0x2710
006f4981: mov rcx, qword ptr [rsi + 0x18]
006f4985: test rcx, rcx
006f4988: je 0x1806f4b42
006f498e: xor r9d, r9d
006f4991: mov r8d, edi
006f4994: mov edx, r15d
006f4997: call 0x180656160
006f499c: test al, al
006f499e: jne 0x1806f49f1
006f49a0: test r12, r12
006f49a3: je 0x1806f4b42
006f49a9: cmp ebp, dword ptr [r12 + 0x18]
006f49ae: jae 0x1806f4b48
006f49b4: mov dword ptr [r14 - 4], r15d
006f49b8: cmp ebp, dword ptr [r12 + 0x18]
006f49bd: jae 0x1806f4b48
006f49c3: mov dword ptr [r14], edi
006f49c6: cmp ebp, dword ptr [r12 + 0x18]
006f49cb: jae 0x1806f4b48
006f49d1: mov rax, qword ptr [rsp + 0x30]
006f49d6: inc ebp
006f49d8: inc rax
006f49db: mov dword ptr [r14 + 4], r13d
006f49df: mov qword ptr [rsp + 0x30], rax
006f49e4: add r14, 0xc
006f49e8: jmp 0x1806f49f6
006f49ea: mov rax, qword ptr [rsp + 0x30]
006f49ef: jmp 0x1806f49fb
006f49f1: mov rax, qword ptr [rsp + 0x30]
006f49f6: mov r8, qword ptr [rsp + 0x38]
006f49fb: inc qword ptr [rsp + 0x40]
006f4a00: inc edi
006f4a02: mov rbx, qword ptr [rsp + 0x40]
006f4a07: cmp edi, 6
006f4a0a: jl 0x1806f48b0
006f4a10: mov r14d, dword ptr [rsp + 0x98]
006f4a18: inc r8
006f4a1b: inc r15d
006f4a1e: mov qword ptr [rsp + 0x38], r8
006f4a23: cmp r15d, 9
006f4a27: jl 0x1806f4888
006f4a2d: test ebp, ebp
006f4a2f: je 0x1806f4b36
006f4a35: mov rcx, qword ptr [rip + 0x4536584]
006f4a3c: cmp dword ptr [rcx + 0xe4], 0
006f4a43: jne 0x1806f4a4a
006f4a45: call 0x1804feeb0
006f4a4a: xor r8d, r8d
006f4a4d: mov edx, ebp
006f4a4f: mov rcx, r12
006f4a52: call 0x1806ccf70
006f4a57: xor r9d, r9d
006f4a5a: mov ecx, 0x49f98
006f4a5f: mov r8d, 4
006f4a65: movsd xmm0, qword ptr [rax]
006f4a69: movsd qword ptr [rsp + 0x40], xmm0
006f4a6f: mov eax, dword ptr [rsp + 0x40]
006f4a73: mov rdx, qword ptr [rsp + 0x40]
006f4a78: shr rdx, 0x20
006f4a7c: mov dword ptr [rsi + 0x34], edx
006f4a7f: mov dword ptr [rsi + 0xd8], eax
006f4a85: call 0x180665070
006f4a8a: mov rcx, qword ptr [rsi + 0x18]
006f4a8e: mov dword ptr [rsi + 0x38], eax
006f4a91: mov dword ptr [rsi + 0x214], eax
006f4a97: test rcx, rcx
006f4a9a: je 0x1806f4b42
006f4aa0: mov r8d, dword ptr [rsi + 0x34]
006f4aa4: mov r9d, 2
006f4aaa: mov edx, dword ptr [rsi + 0xd8]
006f4ab0: mov qword ptr [rsp + 0x20], 0
006f4ab9: call 0x18065f2a0
006f4abe: test rax, rax
006f4ac1: je 0x1806f4acc
006f4ac3: mov eax, dword ptr [rax + 0x38]
006f4ac6: mov dword ptr [rsi + 0x214], eax
006f4acc: imul eax, dword ptr [rsi + 0x34], 0x2710
006f4ad3: mov rcx, qword ptr [rsi + 0x18]
006f4ad7: add eax, 0x30d43
006f4adc: mov dword ptr [rsi + 0x58], eax
006f4adf: test rcx, rcx
006f4ae2: je 0x1806f4b42
006f4ae4: mov r8d, dword ptr [rsi + 0x34]
006f4ae8: xor r9d, r9d
006f4aeb: mov edx, dword ptr [rsi + 0xd8]
006f4af1: call 0x18065f590
006f4af6: mov edx, dword ptr [rsi + 0x34]
006f4af9: xor r8d, r8d
006f4afc: mov rcx, rsi
006f4aff: movd xmm0, eax
006f4b03: cvtdq2ps xmm0, xmm0
006f4b06: movss dword ptr [rsi + 0x78], xmm0
006f4b0b: call 0x1806f29d0
006f4b10: movss dword ptr [rsi + 0x7c], xmm0
006f4b15: mov r15, qword ptr [rsp + 0x50]
006f4b1a: mov r13, qword ptr [rsp + 0x58]
006f4b1f: mov r12, qword ptr [rsp + 0x60]
006f4b24: mov rbp, qword ptr [rsp + 0xa0]
006f4b2c: add rsp, 0x68
006f4b30: pop r14
006f4b32: pop rdi
006f4b33: pop rsi
006f4b34: pop rbx
006f4b35: ret
006f4b36: xor edx, edx
006f4b38: mov rcx, rsi
006f4b3b: call 0x1806ebf90
006f4b40: jmp 0x1806f4b15
006f4b42: call 0x1804fee20
006f4b47: int3
006f4b48: call 0x1804fee10
006f4b4d: int3
006f4b4e: int3
006f4b4f: int3
