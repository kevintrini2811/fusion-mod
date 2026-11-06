0064e280: push rbx
0064e282: push rdi
0064e283: push r14
0064e285: sub rsp, 0x30
0064e289: cmp byte ptr [rip + 0x2f4bc3c], 0
0064e290: mov r14d, edx
0064e293: movsxd rdi, r8d
0064e296: mov rbx, rcx
0064e299: jne 0x18064e2ae
0064e29b: lea rcx, [rip + 0x2d8d8f6]
0064e2a2: call 0x1803633b0
0064e2a7: mov byte ptr [rip + 0x2f4bc1e], 1
0064e2ae: mov rax, qword ptr [rbx + 0x30]
0064e2b2: test rax, rax
0064e2b5: je 0x18064e614
0064e2bb: mov rcx, qword ptr [rax + 0x58]
0064e2bf: test rcx, rcx
0064e2c2: je 0x18064e614
0064e2c8: mov qword ptr [rsp + 0x58], rbp
0064e2cd: lea eax, [r14 - 1]
0064e2d1: mov qword ptr [rsp + 0x60], rsi
0064e2d6: xor bpl, bpl
0064e2d9: movd xmm0, edi
0064e2dd: xor sil, sil
0064e2e0: cvtdq2ps xmm0, xmm0
0064e2e3: mov qword ptr [rsp + 0x68], r15
0064e2e8: xor r15b, r15b
0064e2eb: addss xmm0, dword ptr [rcx + 0x70]
0064e2f0: movss dword ptr [rcx + 0x70], xmm0
0064e2f5: cmp eax, 0xd
0064e2f8: ja 0x18064e463
0064e2fe: lea eax, [r14 - 1]
0064e302: movsxd rcx, eax
0064e305: lea rdx, [rip - 0x64e30c]
0064e30c: mov eax, dword ptr [rdx + rcx*4 + 0x64e61c]
0064e313: add rax, rdx
0064e316: jmp rax
0064e318: mov bpl, 1
0064e31b: jmp 0x18064e431
0064e320: mov sil, 1
0064e323: jmp 0x18064e431
0064e328: mov rsi, qword ptr [rbx + 0x78]
0064e32c: jmp 0x18064e470
0064e331: mov r15b, 1
0064e334: mov rsi, qword ptr [rbx + 0x78]
0064e338: mov rcx, qword ptr [rip + 0x2d8d859]
0064e33f: cmp dword ptr [rcx + 0xe0], 0
0064e346: jne 0x18064e34d
0064e348: call 0x180318580
0064e34d: xor r8d, r8d
0064e350: xor edx, edx
0064e352: mov rcx, rsi
0064e355: call 0x1826b9be0
0064e35a: test al, al
0064e35c: je 0x18064e376
0064e35e: mov r8, qword ptr [rbx]
0064e361: mov edx, edi
0064e363: mov rcx, rbx
0064e366: mov rax, qword ptr [r8 + 0x598]
0064e36d: mov r8, qword ptr [r8 + 0x5a0]
0064e374: call rax
0064e376: mov rcx, qword ptr [rip + 0x2d8d81b]
0064e37d: mov rsi, qword ptr [rbx + 0x70]
0064e381: cmp dword ptr [rcx + 0xe0], 0
0064e388: jne 0x18064e38f
0064e38a: call 0x180318580
0064e38f: xor r8d, r8d
0064e392: xor edx, edx
0064e394: mov rcx, rsi
0064e397: call 0x1826b9be0
0064e39c: test al, al
0064e39e: je 0x18064e3ba
0064e3a0: mov r8, qword ptr [rbx]
0064e3a3: mov edx, edi
0064e3a5: mov rcx, rbx
0064e3a8: mov rax, qword ptr [r8 + 0x578]
0064e3af: mov r8, qword ptr [r8 + 0x580]
0064e3b6: call rax
0064e3b8: mov edi, eax
0064e3ba: test edi, edi
0064e3bc: je 0x18064e3d6
0064e3be: mov r8, qword ptr [rbx]
0064e3c1: mov edx, edi
0064e3c3: mov rcx, rbx
0064e3c6: mov rax, qword ptr [r8 + 0x348]
0064e3cd: mov r8, qword ptr [r8 + 0x350]
0064e3d4: call rax
0064e3d6: test r15b, r15b
0064e3d9: je 0x18064e3ef
0064e3db: cmp qword ptr [rbx + 0xa0], 0
0064e3e3: jg 0x18064e3ef
0064e3e5: xor edx, edx
0064e3e7: mov rcx, rbx
0064e3ea: call 0x18065abf0
0064e3ef: mov rsi, qword ptr [rsp + 0x60]
0064e3f4: mov rbp, qword ptr [rsp + 0x58]
0064e3f9: mov r15, qword ptr [rsp + 0x68]
0064e3fe: add rsp, 0x30
0064e402: pop r14
0064e404: pop rdi
0064e405: pop rbx
0064e406: ret
0064e407: mov rcx, qword ptr [rip + 0x2d8d78a]
0064e40e: mov r14, qword ptr [rbx + 0x78]
0064e412: cmp dword ptr [rcx + 0xe0], 0
0064e419: jne 0x18064e420
0064e41b: call 0x180318580
0064e420: xor r8d, r8d
0064e423: xor edx, edx
0064e425: mov rcx, r14
0064e428: call 0x1826b9a40
0064e42d: test al, al
0064e42f: je 0x18064e463
0064e431: mov rax, qword ptr [rbx]
0064e434: xor r9d, r9d
0064e437: movss xmm1, dword ptr [rip + 0x232d751]
0064e43f: xor r8d, r8d
0064e442: mov rcx, rbx
0064e445: mov rdx, qword ptr [rax + 0x4e8]
0064e44c: mov rax, qword ptr [rax + 0x4f0]
0064e453: mov qword ptr [rsp + 0x20], rax
0064e458: call rdx
0064e45a: test sil, sil
0064e45d: jne 0x18064e522
0064e463: mov rsi, qword ptr [rbx + 0x78]
0064e467: test bpl, bpl
0064e46a: jne 0x18064e338
0064e470: mov rcx, qword ptr [rip + 0x2d8d721]
0064e477: cmp dword ptr [rcx + 0xe0], 0
0064e47e: jne 0x18064e485
0064e480: call 0x180318580
0064e485: xor r8d, r8d
0064e488: xor edx, edx
0064e48a: mov rcx, rsi
0064e48d: call 0x1826b9be0
0064e492: test al, al
0064e494: je 0x18064e4b0
0064e496: mov r8, qword ptr [rbx]
0064e499: mov edx, edi
0064e49b: mov rcx, rbx
0064e49e: mov rax, qword ptr [r8 + 0x598]
0064e4a5: mov r8, qword ptr [r8 + 0x5a0]
0064e4ac: call rax
0064e4ae: mov edi, eax
0064e4b0: mov rcx, qword ptr [rip + 0x2d8d6e1]
0064e4b7: mov rsi, qword ptr [rbx + 0x70]
0064e4bb: cmp dword ptr [rcx + 0xe0], 0
0064e4c2: jne 0x18064e4c9
0064e4c4: call 0x180318580
0064e4c9: xor r8d, r8d
0064e4cc: xor edx, edx
0064e4ce: mov rcx, rsi
0064e4d1: call 0x1826b9be0
0064e4d6: test al, al
0064e4d8: je 0x18064e3ba
0064e4de: test edi, edi
0064e4e0: je 0x18064e3d6
0064e4e6: jmp 0x18064e3a0
0064e4eb: movsxd rcx, dword ptr [rbx + 0xb0]
0064e4f2: add rcx, qword ptr [rbx + 0xa0]
0064e4f9: cmp rcx, rdi
0064e4fc: jge 0x18064e334
0064e502: mov r8, qword ptr [rbx]
0064e505: mov edx, 2
0064e50a: mov rcx, rbx
0064e50d: mov rax, qword ptr [r8 + 0x328]
0064e514: mov r8, qword ptr [r8 + 0x330]
0064e51b: call rax
0064e51d: jmp 0x18064e3ef
0064e522: mov rcx, qword ptr [rip + 0x2d8d66f]
0064e529: mov rsi, qword ptr [rbx + 0x70]
0064e52d: cmp dword ptr [rcx + 0xe0], 0
0064e534: jne 0x18064e53b
0064e536: call 0x180318580
0064e53b: xor r8d, r8d
0064e53e: xor edx, edx
0064e540: mov rcx, rsi
0064e543: call 0x1826b9be0
0064e548: test al, al
0064e54a: je 0x18064e566
0064e54c: mov r8, qword ptr [rbx]
0064e54f: mov edx, edi
0064e551: mov rcx, rbx
0064e554: mov rax, qword ptr [r8 + 0x578]
0064e55b: mov r8, qword ptr [r8 + 0x580]
0064e562: call rax
0064e564: mov edi, eax
0064e566: test edi, edi
0064e568: je 0x18064e3ef
0064e56e: jmp 0x18064e5f7
0064e573: mov rcx, qword ptr [rip + 0x2d8d61e]
0064e57a: mov rsi, qword ptr [rbx + 0x78]
0064e57e: cmp dword ptr [rcx + 0xe0], 0
0064e585: jne 0x18064e58c
0064e587: call 0x180318580
0064e58c: xor r8d, r8d
0064e58f: xor edx, edx
0064e591: mov rcx, rsi
0064e594: call 0x1826b9be0
0064e599: test al, al
0064e59b: je 0x18064e5b5
0064e59d: mov r8, qword ptr [rbx]
0064e5a0: mov edx, edi
0064e5a2: mov rcx, rbx
0064e5a5: mov rax, qword ptr [r8 + 0x598]
0064e5ac: mov r8, qword ptr [r8 + 0x5a0]
0064e5b3: call rax
0064e5b5: mov rcx, qword ptr [rip + 0x2d8d5dc]
0064e5bc: mov rsi, qword ptr [rbx + 0x70]
0064e5c0: cmp dword ptr [rcx + 0xe0], 0
0064e5c7: jne 0x18064e5ce
0064e5c9: call 0x180318580
0064e5ce: xor r8d, r8d
0064e5d1: xor edx, edx
0064e5d3: mov rcx, rsi
0064e5d6: call 0x1826b9be0
0064e5db: test al, al
0064e5dd: je 0x18064e5f7
0064e5df: mov r8, qword ptr [rbx]
0064e5e2: mov edx, edi
0064e5e4: mov rcx, rbx
0064e5e7: mov rax, qword ptr [r8 + 0x578]
0064e5ee: mov r8, qword ptr [r8 + 0x580]
0064e5f5: call rax
0064e5f7: mov r8, qword ptr [rbx]
0064e5fa: mov edx, edi
0064e5fc: mov rcx, rbx
0064e5ff: mov rax, qword ptr [r8 + 0x348]
0064e606: mov r8, qword ptr [r8 + 0x350]
0064e60d: call rax
0064e60f: jmp 0x18064e3ef
0064e614: call 0x180363610
0064e619: int3
0064e61a: nop
0064e61c: xor al, 0xe3
0064e61e: add byte ptr fs:[rdi], al
0064e621: in al, 0x64
0064e623: add byte ptr [rax], bl
0064e625: jrcxz 0x18064e68b
0064e627: add byte ptr [rdx], ah
0064e629: in eax, 0x64
0064e62b: add byte ptr [rax], ah
0064e62d: jrcxz 0x18064e693
0064e62f: add byte ptr [rbx - 0x1b], dh
0064e632: add byte ptr fs:[rax], ch
0064e635: jrcxz 0x18064e69b
0064e637: add byte ptr [rax], ch
0064e639: jrcxz 0x18064e69f
0064e63b: add byte ptr [rax], ch
0064e63d: jrcxz 0x18064e6a3
0064e63f: add byte ptr [rbx + riz*8], dh
0064e642: add bl, ch
0064e645: in al, 0x64
0064e647: add byte ptr [rcx], dh
0064e649: jrcxz 0x18064e6af
0064e64b: add byte ptr [rax], ch
0064e64d: jrcxz 0x18064e6b3
0064e64f: add byte ptr [rbx + riz*8], dh
0064e652: add ah, cl
0064e655: int3
0064e656: int3
0064e657: int3
0064e658: int3
0064e659: int3
0064e65a: int3
0064e65b: int3
0064e65c: int3
0064e65d: int3
0064e65e: int3
0064e65f: int3
