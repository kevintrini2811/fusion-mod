006bacd0: push rbx
006bacd2: sub rsp, 0x40
006bacd6: cmp byte ptr [rip + 0x4830961], 0
006bacdd: mov rbx, rcx
006bace0: jne 0x1806bad01
006bace2: lea rcx, [rip + 0x45702d7]
006bace9: call 0x1804feb70
006bacee: lea rcx, [rip + 0x45ba473]
006bacf5: call 0x1804feb70
006bacfa: mov byte ptr [rip + 0x483093d], 1
006bad01: cmp qword ptr [rbx + 0x18], 0
006bad06: je 0x1806bb0c3
006bad0c: cmp byte ptr [rbx + 0x137], 0
006bad13: je 0x1806bb0c3
006bad19: mov qword ptr [rsp + 0x58], rsi
006bad1e: mov qword ptr [rsp + 0x60], rdi
006bad23: mov qword ptr [rsp + 0x68], r14
006bad28: mov rcx, qword ptr [rbx + 0x10]
006bad2c: test rcx, rcx
006bad2f: je 0x1806bb0e7
006bad35: xor edx, edx
006bad37: call 0x1807e3010
006bad3c: cmp eax, 0x2b
006bad3f: je 0x1806bb0b4
006bad45: mov rcx, qword ptr [rbx + 0x10]
006bad49: test rcx, rcx
006bad4c: je 0x1806bb0e7
006bad52: xor edx, edx
006bad54: call 0x1807e3010
006bad59: cmp eax, 0x32
006bad5c: je 0x1806bb0b4
006bad62: mov rcx, qword ptr [rbx + 0x10]
006bad66: test rcx, rcx
006bad69: je 0x1806bb0e7
006bad6f: xor edx, edx
006bad71: call 0x1807df5d0
006bad76: test al, al
006bad78: jne 0x1806bb0b4
006bad7e: mov rcx, qword ptr [rbx + 0x10]
006bad82: test rcx, rcx
006bad85: je 0x1806bb0e7
006bad8b: xor edx, edx
006bad8d: call 0x1807e3010
006bad92: cmp eax, 0x47
006bad95: je 0x1806bb0b4
006bad9b: mov rcx, qword ptr [rbx + 0x10]
006bad9f: test rcx, rcx
006bada2: je 0x1806bb0e7
006bada8: xor edx, edx
006badaa: call 0x1807e3010
006badaf: cmp eax, 0x48
006badb2: je 0x1806bb0b4
006badb8: mov rcx, qword ptr [rbx + 0x18]
006badbc: test rcx, rcx
006badbf: je 0x1806bb0e7
006badc5: xor edx, edx
006badc7: call 0x18065f8a0
006badcc: test al, al
006badce: jne 0x1806bb0b4
006badd4: cmp dword ptr [rbx + 0x70], 0x26
006badd8: jne 0x1806bae20
006badda: mov rdx, qword ptr [rbx + 0x18]
006badde: test rdx, rdx
006bade1: je 0x1806bb0e7
006bade7: mov ecx, dword ptr [rdx + 0x1e8]
006baded: cmp dword ptr [rdx + 0x200], ecx
006badf3: jne 0x1806bae20
006badf5: cmp dword ptr [rbx + 0x88], 0x2e
006badfc: jne 0x1806bae0c
006badfe: cmp dword ptr [rbx + 0xa0], 0
006bae05: jne 0x1806bae20
006bae07: jmp 0x1806bb0b4
006bae0c: mov dword ptr [rbx + 0x88], 0x2e
006bae16: mov dword ptr [rbx + 0xa0], 0x1770
006bae20: mov rcx, qword ptr [rbx + 0x10]
006bae24: test rcx, rcx
006bae27: je 0x1806bb0e7
006bae2d: xor edx, edx
006bae2f: call 0x1807e3010
006bae34: cmp eax, 0x1f
006bae37: jne 0x1806bae60
006bae39: mov rax, qword ptr [rbx + 0x18]
006bae3d: test rax, rax
006bae40: je 0x1806bb0e7
006bae46: mov rcx, qword ptr [rax + 0x110]
006bae4d: test rcx, rcx
006bae50: je 0x1806bb0e7
006bae56: cmp dword ptr [rcx + 0x54], 0xa
006bae5a: jne 0x1806bb0b4
006bae60: mov esi, dword ptr [rbx + 0xa4]
006bae66: mov rax, qword ptr [rbx + 0x10]
006bae6a: dec esi
006bae6c: mov dword ptr [rbx + 0xa4], esi
006bae72: test rax, rax
006bae75: je 0x1806bb0e7
006bae7b: mov rcx, qword ptr [rax + 0xc8]
006bae82: xor r14d, r14d
006bae85: test rcx, rcx
006bae88: je 0x1806bae93
006bae8a: movzx edi, byte ptr [rcx + 0x82]
006bae91: jmp 0x1806bae96
006bae93: mov edi, r14d
006bae96: cmp esi, 0x64
006bae99: jg 0x1806baef2
006bae9b: test edi, edi
006bae9d: je 0x1806baea5
006bae9f: cmp dword ptr [rbx + 0x70], 0x26
006baea3: jne 0x1806baef2
006baea5: mov rcx, qword ptr [rip + 0x4570114]
006baeac: cmp dword ptr [rcx + 0xe4], r14d
006baeb3: jne 0x1806baeba
006baeb5: call 0x1804feeb0
006baeba: mov qword ptr [rsp + 0x30], r14
006baebf: xor r9d, r9d
006baec2: mov dword ptr [rsp + 0x28], 1
006baeca: mov r8d, esi
006baecd: xor edx, edx
006baecf: mov dword ptr [rsp + 0x20], 0x64
006baed7: mov ecx, 0x64
006baedc: call 0x1806cc120
006baee1: mov ecx, dword ptr [rbx + 0x104]
006baee7: cmp ecx, eax
006baee9: cmovg eax, ecx
006baeec: mov dword ptr [rbx + 0x104], eax
006baef2: cmp dword ptr [rbx + 0xa4], r14d
006baef9: jg 0x1806bb0b4
006baeff: mov rcx, qword ptr [rip + 0x45700ba]
006baf06: mov esi, dword ptr [rbx + 0xa8]
006baf0c: mov qword ptr [rsp + 0x50], rbp
006baf11: cmp dword ptr [rcx + 0xe4], r14d
006baf18: jne 0x1806baf1f
006baf1a: call 0x1804feeb0
006baf1f: lea ecx, [rsi - 0x96]
006baf25: xor r8d, r8d
006baf28: mov edx, esi
006baf2a: call 0x1806cbdf0
006baf2f: mov r8, qword ptr [rbx + 0x10]
006baf33: mov rbp, qword ptr [rsp + 0x50]
006baf38: mov dword ptr [rbx + 0xa4], eax
006baf3e: test r8, r8
006baf41: je 0x1806bb0e7
006baf47: mov r8, qword ptr [r8 + 0x58]
006baf4b: test r8, r8
006baf4e: je 0x1806bb0e7
006baf54: mov rdx, qword ptr [rip + 0x45ba20d]
006baf5b: mov ecx, 6
006baf60: mov r9d, 4
006baf66: call 0x180002040
006baf6b: test edi, edi
006baf6d: jne 0x1806bb00b
006baf73: cmp dword ptr [rbx + 0x70], 9
006baf77: je 0x1806bafcd
006baf79: cmp dword ptr [rbx + 0x70], 1
006baf7d: je 0x1806bafb8
006baf7f: cmp dword ptr [rbx + 0x70], 0x29
006baf83: jne 0x1806bb00b
006baf89: mov rcx, qword ptr [rbx + 0x18]
006baf8d: test rcx, rcx
006baf90: je 0x1806bb0e7
006baf96: movss xmm2, dword ptr [rbx + 0x24]
006baf9b: mov r9d, 4
006bafa1: movss xmm1, dword ptr [rbx + 0x20]
006bafa6: mov qword ptr [rsp + 0x28], r14
006bafab: mov dword ptr [rsp + 0x20], 2
006bafb3: call 0x180654ea0
006bafb8: mov rcx, qword ptr [rbx + 0x18]
006bafbc: test rcx, rcx
006bafbf: je 0x1806bb0e7
006bafc5: mov r9d, 4
006bafcb: jmp 0x1806bafef
006bafcd: mov rcx, qword ptr [rbx + 0x18]
006bafd1: test rcx, rcx
006bafd4: je 0x1806bb0e7
006bafda: cmp dword ptr [rbx + 0x88], 0x17
006bafe1: mov r9d, 4
006bafe7: jne 0x1806bafef
006bafe9: mov r9d, 5
006bafef: movss xmm2, dword ptr [rbx + 0x24]
006baff4: movss xmm1, dword ptr [rbx + 0x20]
006baff9: mov qword ptr [rsp + 0x28], r14
006baffe: mov dword ptr [rsp + 0x20], 2
006bb006: call 0x180654ea0
006bb00b: cmp dword ptr [rbx + 0x70], 0x26
006bb00f: jne 0x1806bb068
006bb011: mov rcx, qword ptr [rip + 0x456ffa8]
006bb018: cmp dword ptr [rcx + 0xe4], r14d
006bb01f: jne 0x1806bb026
006bb021: call 0x1804feeb0
006bb026: xor edx, edx
006bb028: mov ecx, 0x64
006bb02d: call 0x1806cbe80
006bb032: mov rcx, qword ptr [rbx + 0x18]
006bb036: test rcx, rcx
006bb039: je 0x1806bb0e7
006bb03f: movss xmm2, dword ptr [rbx + 0x24]
006bb044: mov r9d, r14d
006bb047: movss xmm1, dword ptr [rbx + 0x20]
006bb04c: cmp eax, 0xa
006bb04f: mov qword ptr [rsp + 0x28], r14
006bb054: setl r9b
006bb058: mov dword ptr [rsp + 0x20], 3
006bb060: inc r9d
006bb063: call 0x180654ea0
006bb068: mov rcx, qword ptr [rbx + 0x10]
006bb06c: test rcx, rcx
006bb06f: je 0x1806bb0e7
006bb071: xor edx, edx
006bb073: call 0x1807e3010
006bb078: cmp eax, 0x27
006bb07b: jne 0x1806bb0b4
006bb07d: cmp dword ptr [rbx + 0x70], 1
006bb081: je 0x1806bb0c9
006bb083: cmp dword ptr [rbx + 0x70], 0x26
006bb087: jne 0x1806bb0b4
006bb089: mov rcx, qword ptr [rbx + 0x18]
006bb08d: test rcx, rcx
006bb090: je 0x1806bb0e7
006bb092: mov qword ptr [rsp + 0x28], r14
006bb097: mov r9d, 1
006bb09d: mov dword ptr [rsp + 0x20], 3
006bb0a5: movss xmm2, dword ptr [rbx + 0x24]
006bb0aa: movss xmm1, dword ptr [rbx + 0x20]
006bb0af: call 0x180654ea0
006bb0b4: mov rdi, qword ptr [rsp + 0x60]
006bb0b9: mov rsi, qword ptr [rsp + 0x58]
006bb0be: mov r14, qword ptr [rsp + 0x68]
006bb0c3: add rsp, 0x40
006bb0c7: pop rbx
006bb0c8: ret
006bb0c9: mov rcx, qword ptr [rbx + 0x18]
006bb0cd: test rcx, rcx
006bb0d0: je 0x1806bb0e7
006bb0d2: mov qword ptr [rsp + 0x28], r14
006bb0d7: mov r9d, 4
006bb0dd: mov dword ptr [rsp + 0x20], 2
006bb0e5: jmp 0x1806bb0a5
006bb0e7: call 0x1804fee20
006bb0ec: int3
006bb0ed: int3
006bb0ee: int3
006bb0ef: int3
