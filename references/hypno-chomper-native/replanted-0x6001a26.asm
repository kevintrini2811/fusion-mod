006b6170: push rbx
006b6172: sub rsp, 0x80
006b6179: cmp byte ptr [rip + 0x48354c5], 0
006b6180: mov rbx, rcx
006b6183: jne 0x1806b61a4
006b6185: lea rcx, [rip + 0x45befdc]
006b618c: call 0x1804feb70
006b6191: lea rcx, [rip + 0x45772e0]
006b6198: call 0x1804feb70
006b619d: mov byte ptr [rip + 0x48354a1], 1
006b61a4: cmp qword ptr [rbx + 0x18], 0
006b61a9: je 0x1806b632c
006b61af: cmp byte ptr [rbx + 0x137], 0
006b61b6: je 0x1806b632c
006b61bc: mov qword ptr [rsp + 0x90], rbp
006b61c4: mov qword ptr [rsp + 0x98], rsi
006b61cc: mov qword ptr [rsp + 0xa0], rdi
006b61d4: movaps xmmword ptr [rsp + 0x70], xmm6
006b61d9: mov rcx, qword ptr [rbx + 0x10]
006b61dd: test rcx, rcx
006b61e0: je 0x1806b7aad
006b61e6: xor edx, edx
006b61e8: call 0x1807e3010
006b61ed: cmp eax, 0x2b
006b61f0: je 0x1806b630f
006b61f6: mov rcx, qword ptr [rbx + 0x10]
006b61fa: test rcx, rcx
006b61fd: je 0x1806b7aad
006b6203: xor edx, edx
006b6205: call 0x1807e3010
006b620a: cmp eax, 0x32
006b620d: je 0x1806b630f
006b6213: cmp dword ptr [rbx + 0x88], 2
006b621a: je 0x1806b6222
006b621c: cmp byte ptr [rbx + 0x3c], 0
006b6220: je 0x1806b622f
006b6222: sub dword ptr [rbx + 0x98], 1
006b6229: js 0x1806b7a9e
006b622f: cmp dword ptr [rbx + 0x124], 0
006b6236: jle 0x1806b6292
006b6238: mov eax, dword ptr [rbx + 0x124]
006b623e: dec eax
006b6240: mov dword ptr [rbx + 0x124], eax
006b6246: cmp eax, 0x3c
006b6249: jne 0x1806b627c
006b624b: mov rax, qword ptr [rbx + 0x10]
006b624f: test rax, rax
006b6252: je 0x1806b7aad
006b6258: mov r8, qword ptr [rax + 0x58]
006b625c: test r8, r8
006b625f: je 0x1806b7aad
006b6265: mov rdx, qword ptr [rip + 0x45beefc]
006b626c: mov ecx, 6
006b6271: mov r9d, 0x53
006b6277: call 0x180002040
006b627c: cmp dword ptr [rbx + 0x124], 0
006b6283: jne 0x1806b6292
006b6285: xor r8d, r8d
006b6288: xor edx, edx
006b628a: mov rcx, rbx
006b628d: call 0x1806b5700
006b6292: cmp byte ptr [rbx + 0x136], 0
006b6299: jne 0x1806b630f
006b629b: cmp byte ptr [rbx + 0x3c], 0
006b629f: jne 0x1806b630f
006b62a1: cmp dword ptr [rbx + 0x128], 0
006b62a8: jne 0x1806b630f
006b62aa: xor edx, edx
006b62ac: mov rcx, rbx
006b62af: call 0x1806bc710
006b62b4: cmp dword ptr [rbx + 0xa0], 0
006b62bb: jle 0x1806b62c3
006b62bd: dec dword ptr [rbx + 0xa0]
006b62c3: mov rcx, qword ptr [rip + 0x45771ae]
006b62ca: mov rdi, qword ptr [rbx + 0x10]
006b62ce: cmp dword ptr [rcx + 0xe4], 0
006b62d5: jne 0x1806b62dc
006b62d7: call 0x1804feeb0
006b62dc: xor r8d, r8d
006b62df: xor edx, edx
006b62e1: mov rcx, rdi
006b62e4: call 0x183832900
006b62e9: test al, al
006b62eb: je 0x1806b6335
006b62ed: mov rcx, qword ptr [rbx + 0x10]
006b62f1: test rcx, rcx
006b62f4: je 0x1806b7aad
006b62fa: xor edx, edx
006b62fc: call 0x1807dfa20
006b6301: test al, al
006b6303: je 0x1806b6335
006b6305: xor edx, edx
006b6307: mov rcx, rbx
006b630a: call 0x1806b7d50
006b630f: mov rdi, qword ptr [rsp + 0xa0]
006b6317: mov rsi, qword ptr [rsp + 0x98]
006b631f: mov rbp, qword ptr [rsp + 0x90]
006b6327: movaps xmm6, xmmword ptr [rsp + 0x70]
006b632c: add rsp, 0x80
006b6333: pop rbx
006b6334: ret
006b6335: cmp dword ptr [rbx + 0x70], 0x11
006b6339: je 0x1806b7a58
006b633f: cmp dword ptr [rbx + 0x70], 0xf
006b6343: je 0x1806b78fb
006b6349: cmp dword ptr [rbx + 0x70], 0xe
006b634d: je 0x1806b78c8
006b6353: cmp dword ptr [rbx + 0x70], 6
006b6357: je 0x1806b74f7
006b635d: cmp dword ptr [rbx + 0x70], 0x1b
006b6361: je 0x1806b7454
006b6367: cmp dword ptr [rbx + 0x70], 0x21
006b636b: je 0x1806b742d
006b6371: cmp dword ptr [rbx + 0x70], 0x10
006b6375: je 0x1806b7412
006b637b: cmp dword ptr [rbx + 0x70], 0x30
006b637f: je 0x1806b7250
006b6385: cmp dword ptr [rbx + 0x70], 0x23
006b6389: je 0x1806b7212
006b638f: cmp dword ptr [rbx + 0x70], 0x25
006b6393: je 0x1806b7179
006b6399: cmp dword ptr [rbx + 0x70], 0x2f
006b639d: je 0x1806b6ee6
006b63a3: cmp dword ptr [rbx + 0x70], 0x1a
006b63a7: je 0x1806b6ed7
006b63ad: cmp dword ptr [rbx + 0x70], 0x1f
006b63b1: je 0x1806b6ec8
006b63b7: cmp dword ptr [rbx + 0x70], 0x2d
006b63bb: je 0x1806b6eb9
006b63c1: cmp dword ptr [rbx + 0x70], 9
006b63c5: je 0x1806b6cd1
006b63cb: cmp dword ptr [rbx + 0x70], 1
006b63cf: je 0x1806b6eaa
006b63d5: cmp dword ptr [rbx + 0x70], 0x29
006b63d9: je 0x1806b6eaa
006b63df: cmp dword ptr [rbx + 0x70], 0x26
006b63e3: je 0x1806b6eaa
006b63e9: cmp dword ptr [rbx + 0x70], 0xb
006b63ed: je 0x1806b6a8a
006b63f3: cmp dword ptr [rbx + 0x70], 0x16
006b63f7: je 0x1806b6908
006b63fd: cmp dword ptr [rbx + 0x70], 4
006b6401: je 0x1806b653b
006b6407: cmp dword ptr [rbx + 0x70], 0x15
006b640b: je 0x1806b6441
006b640d: cmp dword ptr [rbx + 0x70], 0x2e
006b6411: je 0x1806b6441
006b6413: cmp dword ptr [rbx + 0x70], 0x13
006b6417: je 0x1806b6432
006b6419: cmp dword ptr [rbx + 0x70], 0xd
006b641d: jne 0x1806b7a62
006b6423: xor edx, edx
006b6425: mov rcx, rbx
006b6428: call 0x1806bbef0
006b642d: jmp 0x1806b7a62
006b6432: xor edx, edx
006b6434: mov rcx, rbx
006b6437: call 0x1806bdc80
006b643c: jmp 0x1806b7a62
006b6441: cmp byte ptr [rip + 0x483521c], 0
006b6448: jne 0x1806b645d
006b644a: lea rcx, [rip + 0x4574b6f]
006b6451: call 0x1804feb70
006b6456: mov byte ptr [rip + 0x4835207], 1
006b645d: cmp dword ptr [rbx + 0x88], 0x12
006b6464: je 0x1806b648f
006b6466: mov edx, dword ptr [rbx + 0x34]
006b6469: xor r9d, r9d
006b646c: xor r8d, r8d
006b646f: mov rcx, rbx
006b6472: call 0x1806b0040
006b6477: test rax, rax
006b647a: je 0x1806b7a62
006b6480: xor edx, edx
006b6482: mov rcx, rbx
006b6485: call 0x1806b5b90
006b648a: jmp 0x1806b7a62
006b648f: cmp dword ptr [rbx + 0xa0], 0
006b6496: je 0x1806b64d1
006b6498: cmp dword ptr [rbx + 0x70], 0x2e
006b649c: mov eax, dword ptr [rbx + 0xa0]
006b64a2: je 0x1806b64ab
006b64a4: cmp eax, 0x4b
006b64a7: je 0x1806b64b9
006b64a9: jmp 0x1806b64d9
006b64ab: cmp eax, 0x46
006b64ae: je 0x1806b64b9
006b64b0: cmp dword ptr [rbx + 0xa0], 0x20
006b64b7: jne 0x1806b64d9
006b64b9: xor r9d, r9d
006b64bc: mov edx, 0x14
006b64c1: mov r8d, 0x21
006b64c7: mov rcx, rbx
006b64ca: call 0x1806ad520
006b64cf: jmp 0x1806b64d9
006b64d1: xor esi, esi
006b64d3: mov dword ptr [rbx + 0x88], esi
006b64d9: mov rcx, qword ptr [rbx + 0x140]
006b64e0: test rcx, rcx
006b64e3: je 0x1806b7aad
006b64e9: xor r8d, r8d
006b64ec: xor edx, edx
006b64ee: call 0x1807c7a30
006b64f3: test eax, eax
006b64f5: jle 0x1806b7a62
006b64fb: mov rcx, qword ptr [rip + 0x4574abe]
006b6502: cmp dword ptr [rcx + 0xe4], 0
006b6509: jne 0x1806b6510
006b650b: call 0x1804feeb0
006b6510: movss xmm1, dword ptr [rip + 0x357d8a0]
006b6518: xor r8d, r8d
006b651b: movss xmm0, dword ptr [rip + 0x357d891]
006b6523: call 0x1806cbd40
006b6528: xor r8d, r8d
006b652b: movaps xmm1, xmm0
006b652e: mov rcx, rbx
006b6531: call 0x1806b4c50
006b6536: jmp 0x1806b7a62
006b653b: cmp byte ptr [rip + 0x483511a], 0
006b6542: jne 0x1806b6593
006b6544: lea rcx, [rip + 0x4574a75]
006b654b: call 0x1804feb70
006b6550: lea rcx, [rip + 0x45bec11]
006b6557: call 0x1804feb70
006b655c: lea rcx, [rip + 0x45e1975]
006b6563: call 0x1804feb70
006b6568: lea rcx, [rip + 0x45dea31]
006b656f: call 0x1804feb70
006b6574: lea rcx, [rip + 0x45e0a2d]
006b657b: call 0x1804feb70
006b6580: lea rcx, [rip + 0x45dc931]
006b6587: call 0x1804feb70
006b658c: mov byte ptr [rip + 0x48350c9], 1
006b6593: cmp dword ptr [rbx + 0x88], 0
006b659a: je 0x1806b683e
006b65a0: cmp dword ptr [rbx + 0x88], 0xf
006b65a7: je 0x1806b6656
006b65ad: cmp dword ptr [rbx + 0x88], 0x10
006b65b4: jne 0x1806b7a62
006b65ba: mov edx, dword ptr [rbx + 0x34]
006b65bd: xor r9d, r9d
006b65c0: xor r8d, r8d
006b65c3: mov rcx, rbx
006b65c6: call 0x1806b0040
006b65cb: test rax, rax
006b65ce: jne 0x1806b74e8
006b65d4: mov rcx, qword ptr [rbx + 0x10]
006b65d8: test rcx, rcx
006b65db: je 0x1806b7aad
006b65e1: mov edx, dword ptr [rbx + 0xf4]
006b65e7: xor r8d, r8d
006b65ea: call 0x1807e1430
006b65ef: mov rdi, rax
006b65f2: test rax, rax
006b65f5: je 0x1806b7a62
006b65fb: xor edx, edx
006b65fd: mov rcx, rbx
006b6600: call 0x1806ad0c0
006b6605: mov rcx, qword ptr [rip + 0x45749b4]
006b660c: mov ebp, eax
006b660e: cmp dword ptr [rcx + 0xe4], 0
006b6615: jne 0x1806b661c
006b6617: call 0x1804feeb0
006b661c: xor esi, esi
006b661e: mov r9d, 0xa
006b6624: mov qword ptr [rsp + 0x30], rsi
006b6629: mov r8d, ebp
006b662c: mov dword ptr [rsp + 0x28], 1
006b6634: mov edx, 0x32
006b6639: mov ecx, 0xc8
006b663e: mov dword ptr [rsp + 0x20], 3
006b6646: call 0x1806cc120
006b664b: mov dword ptr [rdi + 0x80], eax
006b6651: jmp 0x1806b7a62
006b6656: mov rcx, qword ptr [rbx + 0x140]
006b665d: test rcx, rcx
006b6660: je 0x1806b7aad
006b6666: xor r8d, r8d
006b6669: xor edx, edx
006b666b: call 0x1807c7a30
006b6670: test eax, eax
006b6672: jle 0x1806b7a62
006b6678: mov rcx, qword ptr [rip + 0x4574941]
006b667f: cmp dword ptr [rcx + 0xe4], 0
006b6686: jne 0x1806b668d
006b6688: call 0x1804feeb0
006b668d: movss xmm1, dword ptr [rip + 0x357d723]
006b6695: xor r8d, r8d
006b6698: movss xmm0, dword ptr [rip + 0x357d714]
006b66a0: call 0x1806cbd40
006b66a5: cmp byte ptr [rip + 0x4834fac], 0
006b66ac: movaps xmm6, xmm0
006b66af: mov rdi, qword ptr [rip + 0x45dc802]
006b66b6: jne 0x1806b66d7
006b66b8: lea rcx, [rip + 0x45dc3c1]
006b66bf: call 0x1804feb70
006b66c4: lea rcx, [rip + 0x45dab2d]
006b66cb: call 0x1804feb70
006b66d0: mov byte ptr [rip + 0x4834f81], 1
006b66d7: test rdi, rdi
006b66da: je 0x1806b7aad
006b66e0: mov r8, qword ptr [rip + 0x45dab11]
006b66e7: xor r9d, r9d
006b66ea: mov rdx, qword ptr [rip + 0x45dc38f]
006b66f1: mov rcx, rdi
006b66f4: call 0x182a24e50
006b66f9: mov rcx, qword ptr [rbx + 0x140]
006b6700: test rcx, rcx
006b6703: je 0x1806b7aad
006b6709: xor esi, esi
006b670b: movaps xmm3, xmm6
006b670e: mov qword ptr [rsp + 0x28], rsi
006b6713: xor r8d, r8d
006b6716: mov rdx, rax
006b6719: mov dword ptr [rsp + 0x20], esi
006b671d: call 0x1807c8270
006b6722: mov rcx, qword ptr [rbx + 0x10]
006b6726: test rcx, rcx
006b6729: je 0x1806b7aad
006b672f: mov edx, dword ptr [rbx + 0x70]
006b6732: xor r8d, r8d
006b6735: call 0x1807dd8a0
006b673a: test rax, rax
006b673d: je 0x1806b7aad
006b6743: mov rcx, qword ptr [rbx + 0x10]
006b6747: test rcx, rcx
006b674a: je 0x1806b7aad
006b6750: mov r9d, dword ptr [rbx + 0x38]
006b6754: xorps xmm2, xmm2
006b6757: mov eax, dword ptr [rax + 0x40]
006b675a: add r9d, 2
006b675e: mov qword ptr [rsp + 0x28], rsi
006b6763: xorps xmm1, xmm1
006b6766: mov dword ptr [rsp + 0x20], eax
006b676a: call 0x1807d96d0
006b676f: mov rdi, rax
006b6772: test rax, rax
006b6775: je 0x1806b7aad
006b677b: xor r8d, r8d
006b677e: xor edx, edx
006b6780: mov rcx, rax
006b6783: call 0x1806caf80
006b6788: subss xmm6, dword ptr [rip + 0x357d604]
006b6790: xor r8d, r8d
006b6793: mov rcx, rdi
006b6796: movaps xmm1, xmm6
006b6799: call 0x1806caee0
006b679e: mov rdx, qword ptr [rip + 0x45de7fb]
006b67a5: xor r8d, r8d
006b67a8: mov rcx, rdi
006b67ab: call 0x1806cac10
006b67b0: mov dword ptr [rdi + 0x80], 0xa
006b67ba: xor r8d, r8d
006b67bd: mov rdx, qword ptr [rip + 0x45de7dc]
006b67c4: mov rcx, rdi
006b67c7: call 0x18060f880
006b67cc: mov rdx, qword ptr [rip + 0x45de7cd]
006b67d3: xor r9d, r9d
006b67d6: xor r8d, r8d
006b67d9: mov rcx, rdi
006b67dc: call 0x18060f880
006b67e1: mov rcx, qword ptr [rbx + 0x10]
006b67e5: test rcx, rcx
006b67e8: je 0x1806b7aad
006b67ee: xor r8d, r8d
006b67f1: mov rdx, rdi
006b67f4: call 0x1807e13a0
006b67f9: mov rdx, qword ptr [rbx + 0x140]
006b6800: xor r9d, r9d
006b6803: mov dword ptr [rbx + 0xf4], eax
006b6809: mov rcx, rdi
006b680c: mov r8, qword ptr [rip + 0x45e0795]
006b6813: call 0x18060f880
006b6818: xor edx, edx
006b681a: mov dword ptr [rbx + 0x88], 0x10
006b6824: mov ecx, 0x190
006b6829: call 0x1806cbe80
006b682e: add eax, 0x190
006b6833: mov dword ptr [rbx + 0xfc], eax
006b6839: jmp 0x1806b7a62
006b683e: cmp dword ptr [rbx + 0xa0], 0
006b6845: jne 0x1806b7a62
006b684b: mov rcx, qword ptr [rbx + 0x10]
006b684f: test rcx, rcx
006b6852: je 0x1806b7aad
006b6858: movd xmm2, dword ptr [rbx + 0x2c]
006b685d: xor esi, esi
006b685f: movd xmm1, dword ptr [rbx + 0x28]
006b6864: mov r9d, dword ptr [rbx + 0x38]
006b6868: cvtdq2ps xmm2, xmm2
006b686b: mov qword ptr [rsp + 0x28], rsi
006b6870: mov dword ptr [rsp + 0x20], 0x31
006b6878: cvtdq2ps xmm1, xmm1
006b687b: mulss xmm2, dword ptr [rip + 0x357d3bd]
006b6883: mulss xmm1, dword ptr [rip + 0x357d3b5]
006b688b: addss xmm2, dword ptr [rbx + 0x24]
006b6890: addss xmm1, dword ptr [rbx + 0x20]
006b6895: call 0x1807d9b50
006b689a: movss xmm0, dword ptr [rip + 0x357d642]
006b68a2: mov r9d, 0x14
006b68a8: mov rdx, qword ptr [rip + 0x45e1629]
006b68af: mov r8d, 3
006b68b5: mov qword ptr [rsp + 0x28], rsi
006b68ba: mov rcx, rbx
006b68bd: movss dword ptr [rsp + 0x20], xmm0
006b68c3: call 0x1806b4b80
006b68c8: mov r8, qword ptr [rbx + 0x10]
006b68cc: mov dword ptr [rbx + 0x88], 0xf
006b68d6: test r8, r8
006b68d9: je 0x1806b7aad
006b68df: mov r8, qword ptr [r8 + 0x58]
006b68e3: test r8, r8
006b68e6: je 0x1806b7aad
006b68ec: mov rdx, qword ptr [rip + 0x45be875]
006b68f3: mov ecx, 6
006b68f8: mov r9d, 0x50
006b68fe: call 0x180002040
006b6903: jmp 0x1806b7a62
006b6908: cmp byte ptr [rip + 0x4834d43], 0
006b690f: jne 0x1806b693c
006b6911: lea rcx, [rip + 0x45746a8]
006b6918: call 0x1804feb70
006b691d: lea rcx, [rip + 0x45c92b4]
006b6924: call 0x1804feb70
006b6929: lea rcx, [rip + 0x45c9380]
006b6930: call 0x1804feb70
006b6935: mov byte ptr [rip + 0x4834d16], 1
006b693c: xorps xmm0, xmm0
006b693f: lea rcx, [rsp + 0x60]
006b6944: xorps xmm1, xmm1
006b6947: xor r9d, r9d
006b694a: xor r8d, r8d
006b694d: mov rdx, rbx
006b6950: movups xmmword ptr [rsp + 0x50], xmm0
006b6955: movups xmmword ptr [rsp + 0x40], xmm1
006b695a: call 0x1806b1660
006b695f: xor esi, esi
006b6961: mov edx, esi
006b6963: movups xmm0, xmmword ptr [rax]
006b6966: mov rax, qword ptr [rbx + 0x18]
006b696a: movups xmmword ptr [rsp + 0x50], xmm0
006b696f: test rax, rax
006b6972: je 0x1806b7ab3
006b6978: nop dword ptr [rax + rax]
006b6980: mov rcx, qword ptr [rax + 0x80]
006b6987: test rcx, rcx
006b698a: je 0x1806b7ab3
006b6990: cmp edx, dword ptr [rcx + 0x28]
006b6993: jge 0x1806b7a62
006b6999: mov rax, qword ptr [rbx + 0x18]
006b699d: test rax, rax
006b69a0: je 0x1806b7ab3
006b69a6: mov rcx, qword ptr [rax + 0x80]
006b69ad: test rcx, rcx
006b69b0: je 0x1806b7ab3
006b69b6: mov r8, qword ptr [rip + 0x45c92f3]
006b69bd: mov edx, esi
006b69bf: call 0x182449330
006b69c4: mov rdi, rax
006b69c7: test rax, rax
006b69ca: je 0x1806b7ab3
006b69d0: mov ecx, dword ptr [rbx + 0x34]
006b69d3: cmp dword ptr [rax + 0x34], ecx
006b69d6: jne 0x1806b6a74
006b69dc: cmp dword ptr [rax + 0xa8], 0
006b69e3: je 0x1806b69f7
006b69e5: cmp dword ptr [rax + 0xa8], 0xe
006b69ec: je 0x1806b69f7
006b69ee: cmp dword ptr [rax + 0xa8], 1
006b69f5: jne 0x1806b6a74
006b69f7: xor r8d, r8d
006b69fa: lea rcx, [rsp + 0x60]
006b69ff: mov rdx, rdi
006b6a02: call 0x1806c86f0
006b6a07: mov rcx, qword ptr [rip + 0x45745b2]
006b6a0e: movups xmm0, xmmword ptr [rax]
006b6a11: movups xmmword ptr [rsp + 0x40], xmm0
006b6a16: cmp dword ptr [rcx + 0xe4], 0
006b6a1d: jne 0x1806b6a24
006b6a1f: call 0x1804feeb0
006b6a24: xor r8d, r8d
006b6a27: lea rdx, [rsp + 0x40]
006b6a2c: lea rcx, [rsp + 0x50]
006b6a31: call 0x1806cbaa0
006b6a36: cmp eax, 0xa
006b6a39: jl 0x1806b6a74
006b6a3b: cmp dword ptr [rdi + 0xa8], 0
006b6a42: je 0x1806b6a66
006b6a44: cmp dword ptr [rdi + 0xa8], 0xe
006b6a4b: je 0x1806b6a66
006b6a4d: cmp dword ptr [rdi + 0xa8], 1
006b6a54: jne 0x1806b6a74
006b6a56: mov edx, dword ptr [rbx + 0x74]
006b6a59: xor r8d, r8d
006b6a5c: mov rcx, rdi
006b6a5f: call 0x1806c71e0
006b6a64: jmp 0x1806b6a74
006b6a66: mov edx, dword ptr [rbx + 0x74]
006b6a69: xor r8d, r8d
006b6a6c: mov rcx, rdi
006b6a6f: call 0x1806c6f60
006b6a74: mov rax, qword ptr [rbx + 0x18]
006b6a78: inc esi
006b6a7a: mov edx, esi
006b6a7c: test rax, rax
006b6a7f: je 0x1806b7ab3
006b6a85: jmp 0x1806b6980
006b6a8a: cmp byte ptr [rip + 0x4834bbd], 0
006b6a91: jne 0x1806b6aa6
006b6a93: lea rcx, [rip + 0x45df50e]
006b6a9a: call 0x1804feb70
006b6a9f: mov byte ptr [rip + 0x4834ba8], 1
006b6aa6: cmp dword ptr [rbx + 0x88], 8
006b6aad: je 0x1806b6bba
006b6ab3: cmp dword ptr [rbx + 0x88], 9
006b6aba: jne 0x1806b7a62
006b6ac0: cmp dword ptr [rbx + 0xa0], 0
006b6ac7: jne 0x1806b7a62
006b6acd: mov rcx, qword ptr [rbx + 0x10]
006b6ad1: test rcx, rcx
006b6ad4: je 0x1806b7aad
006b6ada: xor edx, edx
006b6adc: call 0x1807df9f0
006b6ae1: mov rcx, qword ptr [rbx + 0x18]
006b6ae5: test rcx, rcx
006b6ae8: je 0x1806b7aad
006b6aee: mov r8d, dword ptr [rbx + 0x34]
006b6af2: xor r9d, r9d
006b6af5: mov edx, dword ptr [rbx + 0x74]
006b6af8: test al, al
006b6afa: jne 0x1806b6b25
006b6afc: call 0x18065def0
006b6b01: test rax, rax
006b6b04: je 0x1806b6b39
006b6b06: xor edx, edx
006b6b08: mov rcx, rax
006b6b0b: call 0x1806c2390
006b6b10: mov rcx, qword ptr [rbx + 0x18]
006b6b14: test rcx, rcx
006b6b17: je 0x1806b7aad
006b6b1d: inc dword ptr [rcx + 0x2f4]
006b6b23: jmp 0x1806b6b39
006b6b25: call 0x18065f400
006b6b2a: test rax, rax
006b6b2d: je 0x1806b6b39
006b6b2f: xor edx, edx
006b6b31: mov rcx, rax
006b6b34: call 0x1806ebf90
006b6b39: mov rcx, qword ptr [rbx + 0x10]
006b6b3d: test rcx, rcx
006b6b40: je 0x1806b7aad
006b6b46: movss xmm2, dword ptr [rbx + 0x24]
006b6b4b: xor esi, esi
006b6b4d: movss xmm1, dword ptr [rbx + 0x20]
006b6b52: mov r9d, dword ptr [rbx + 0x38]
006b6b56: movss xmm6, dword ptr [rip + 0x357d266]
006b6b5e: add r9d, 4
006b6b62: addss xmm2, dword ptr [rip + 0x357d262]
006b6b6a: addss xmm1, xmm6
006b6b6e: mov qword ptr [rsp + 0x28], rsi
006b6b73: mov dword ptr [rsp + 0x20], 0x19
006b6b7b: call 0x1807d9b50
006b6b80: xor edx, edx
006b6b82: mov rcx, rbx
006b6b85: call 0x1806acd40
006b6b8a: mov rcx, qword ptr [rbx + 0x18]
006b6b8e: test rcx, rcx
006b6b91: je 0x1806b7aad
006b6b97: movss xmm1, dword ptr [rbx + 0x20]
006b6b9c: mov r9d, 0xc
006b6ba2: movss xmm2, dword ptr [rbx + 0x24]
006b6ba7: addss xmm1, xmm6
006b6bab: mov qword ptr [rsp + 0x20], rsi
006b6bb0: call 0x18065c570
006b6bb5: jmp 0x1806b7a62
006b6bba: mov rcx, qword ptr [rbx + 0x140]
006b6bc1: test rcx, rcx
006b6bc4: je 0x1806b7aad
006b6bca: xor r8d, r8d
006b6bcd: xor edx, edx
006b6bcf: call 0x1807c7a30
006b6bd4: test eax, eax
006b6bd6: jle 0x1806b7a62
006b6bdc: cmp byte ptr [rip + 0x4834a75], 0
006b6be3: mov rdi, qword ptr [rip + 0x45df3be]
006b6bea: jne 0x1806b6c0b
006b6bec: lea rcx, [rip + 0x45dbe8d]
006b6bf3: call 0x1804feb70
006b6bf8: lea rcx, [rip + 0x45da5f9]
006b6bff: call 0x1804feb70
006b6c04: mov byte ptr [rip + 0x4834a4d], 1
006b6c0b: test rdi, rdi
006b6c0e: je 0x1806b7aad
006b6c14: mov r8, qword ptr [rip + 0x45da5dd]
006b6c1b: xor r9d, r9d
006b6c1e: mov rdx, qword ptr [rip + 0x45dbe5b]
006b6c25: mov rcx, rdi
006b6c28: call 0x182a24e50
006b6c2d: mov rcx, qword ptr [rbx + 0x140]
006b6c34: mov rdi, rax
006b6c37: test rcx, rcx
006b6c3a: je 0x1806b7aad
006b6c40: movss xmm1, dword ptr [rip + 0x357d164]
006b6c48: xor r9d, r9d
006b6c4b: xor r8d, r8d
006b6c4e: call 0x18060f880
006b6c53: mov rcx, qword ptr [rbx + 0x140]
006b6c5a: test rcx, rcx
006b6c5d: je 0x1806b7aad
006b6c63: movss xmm3, dword ptr [rip + 0x357d149]
006b6c6b: xor esi, esi
006b6c6d: mov qword ptr [rsp + 0x28], rsi
006b6c72: xor r8d, r8d
006b6c75: mov rdx, rdi
006b6c78: mov dword ptr [rsp + 0x20], esi
006b6c7c: call 0x1807c8270
006b6c81: movss xmm2, dword ptr [rbx + 0x24]
006b6c86: mov rcx, rbx
006b6c89: movss xmm1, dword ptr [rbx + 0x20]
006b6c8e: mov r9d, dword ptr [rbx + 0x38]
006b6c92: addss xmm2, dword ptr [rip + 0x357d12a]
006b6c9a: addss xmm1, dword ptr [rip + 0x357d122]
006b6ca2: add r9d, 4
006b6ca6: mov qword ptr [rsp + 0x28], rsi
006b6cab: mov dword ptr [rbx + 0xa0], 0x190
006b6cb5: mov dword ptr [rbx + 0x88], 9
006b6cbf: mov dword ptr [rsp + 0x20], 0x18
006b6cc7: call 0x1806ab220
006b6ccc: jmp 0x1806b7a62
006b6cd1: cmp byte ptr [rip + 0x483498e], 0
006b6cd8: jne 0x1806b6d11
006b6cda: lea rcx, [rip + 0x45742df]
006b6ce1: call 0x1804feb70
006b6ce6: lea rcx, [rip + 0x45be47b]
006b6ced: call 0x1804feb70
006b6cf2: lea rcx, [rip + 0x45de457]
006b6cf9: call 0x1804feb70
006b6cfe: lea rcx, [rip + 0x45dc513]
006b6d05: call 0x1804feb70
006b6d0a: mov byte ptr [rip + 0x4834955], 1
006b6d11: cmp dword ptr [rbx + 0x88], 0x17
006b6d18: je 0x1806b6db9
006b6d1e: cmp dword ptr [rbx + 0x88], 0x18
006b6d25: je 0x1806b6d36
006b6d27: xor edx, edx
006b6d29: mov rcx, rbx
006b6d2c: call 0x1806bacd0
006b6d31: jmp 0x1806b7a62
006b6d36: mov rcx, qword ptr [rbx + 0x140]
006b6d3d: test rcx, rcx
006b6d40: je 0x1806b7aad
006b6d46: xor r8d, r8d
006b6d49: xor edx, edx
006b6d4b: call 0x1807c7a30
006b6d50: test eax, eax
006b6d52: jle 0x1806b7a62
006b6d58: mov rcx, qword ptr [rip + 0x4574261]
006b6d5f: cmp dword ptr [rcx + 0xe4], 0
006b6d66: jne 0x1806b6d6d
006b6d68: call 0x1804feeb0
006b6d6d: movss xmm1, dword ptr [rip + 0x357d043]
006b6d75: xor r8d, r8d
006b6d78: movss xmm0, dword ptr [rip + 0x357d034]
006b6d80: call 0x1806cbd40
006b6d85: mov rdx, qword ptr [rip + 0x45dc48c]
006b6d8c: xor esi, esi
006b6d8e: mov qword ptr [rsp + 0x28], rsi
006b6d93: mov r9d, 0xa
006b6d99: xor r8d, r8d
006b6d9c: movss dword ptr [rsp + 0x20], xmm0
006b6da2: mov rcx, rbx
006b6da5: call 0x1806b4b80
006b6daa: mov dword ptr [rbx + 0x88], 0x19
006b6db4: jmp 0x1806b7a62
006b6db9: cmp dword ptr [rbx + 0xa0], 0
006b6dc0: jne 0x1806b6eaa
006b6dc6: cmp byte ptr [rip + 0x483488b], 0
006b6dcd: mov rdi, qword ptr [rip + 0x45de37c]
006b6dd4: jne 0x1806b6df5
006b6dd6: lea rcx, [rip + 0x45dbca3]
006b6ddd: call 0x1804feb70
006b6de2: lea rcx, [rip + 0x45da40f]
006b6de9: call 0x1804feb70
006b6dee: mov byte ptr [rip + 0x4834863], 1
006b6df5: test rdi, rdi
006b6df8: je 0x1806b7aad
006b6dfe: mov r8, qword ptr [rip + 0x45da3f3]
006b6e05: xor r9d, r9d
006b6e08: mov rdx, qword ptr [rip + 0x45dbc71]
006b6e0f: mov rcx, rdi
006b6e12: call 0x182a24e50
006b6e17: mov rcx, qword ptr [rbx + 0x140]
006b6e1e: mov rdi, rax
006b6e21: test rcx, rcx
006b6e24: je 0x1806b7aad
006b6e2a: movss xmm1, dword ptr [rip + 0x357cf7a]
006b6e32: xor r9d, r9d
006b6e35: xor r8d, r8d
006b6e38: call 0x18060f880
006b6e3d: mov rcx, qword ptr [rbx + 0x140]
006b6e44: test rcx, rcx
006b6e47: je 0x1806b7aad
006b6e4d: movss xmm3, dword ptr [rip + 0x357cf5f]
006b6e55: xor esi, esi
006b6e57: mov qword ptr [rsp + 0x28], rsi
006b6e5c: xor r8d, r8d
006b6e5f: mov rdx, rdi
006b6e62: mov dword ptr [rsp + 0x20], 3
006b6e6a: call 0x1807c8270
006b6e6f: mov r8, qword ptr [rbx + 0x10]
006b6e73: mov dword ptr [rbx + 0x88], 0x18
006b6e7d: test r8, r8
006b6e80: je 0x1806b7aad
006b6e86: mov r8, qword ptr [r8 + 0x58]
006b6e8a: test r8, r8
006b6e8d: je 0x1806b7aad
006b6e93: mov rdx, qword ptr [rip + 0x45be2ce]
006b6e9a: mov ecx, 6
006b6e9f: mov r9d, 0x3f
006b6ea5: call 0x180002040
006b6eaa: xor edx, edx
006b6eac: mov rcx, rbx
006b6eaf: call 0x1806bacd0
006b6eb4: jmp 0x1806b7a62
006b6eb9: xor edx, edx
006b6ebb: mov rcx, rbx
006b6ebe: call 0x1806b9030
006b6ec3: jmp 0x1806b7a62
006b6ec8: xor edx, edx
006b6eca: mov rcx, rbx
006b6ecd: call 0x1806b9bc0
006b6ed2: jmp 0x1806b7a62
006b6ed7: xor edx, edx
006b6ed9: mov rcx, rbx
006b6edc: call 0x1806b84a0
006b6ee1: jmp 0x1806b7a62
006b6ee6: cmp byte ptr [rip + 0x4834781], 0
006b6eed: jne 0x1806b6f1a
006b6eef: lea rcx, [rip + 0x45740ca]
006b6ef6: call 0x1804feb70
006b6efb: lea rcx, [rip + 0x45be266]
006b6f02: call 0x1804feb70
006b6f07: lea rcx, [rip + 0x45dc8f2]
006b6f0e: call 0x1804feb70
006b6f13: mov byte ptr [rip + 0x4834754], 1
006b6f1a: cmp dword ptr [rbx + 0x88], 0x23
006b6f21: je 0x1806b70b4
006b6f27: cmp dword ptr [rbx + 0x88], 0x24
006b6f2e: je 0x1806b7018
006b6f34: cmp dword ptr [rbx + 0x88], 0x25
006b6f3b: je 0x1806b6fab
006b6f3d: cmp dword ptr [rbx + 0x88], 0x26
006b6f44: jne 0x1806b7a62
006b6f4a: mov rcx, qword ptr [rbx + 0x140]
006b6f51: test rcx, rcx
006b6f54: je 0x1806b7aad
006b6f5a: movss xmm1, dword ptr [rip + 0x357d032]
006b6f62: xor r9d, r9d
006b6f65: xor r8d, r8d
006b6f68: call 0x1807c8910
006b6f6d: test al, al
006b6f6f: je 0x1806b7a62
006b6f75: mov rax, qword ptr [rbx + 0x10]
006b6f79: test rax, rax
006b6f7c: je 0x1806b7aad
006b6f82: mov r8, qword ptr [rax + 0x58]
006b6f86: test r8, r8
006b6f89: je 0x1806b7aad
006b6f8f: mov rdx, qword ptr [rip + 0x45be1d2]
006b6f96: mov ecx, 6
006b6f9b: mov r9d, 0x4c
006b6fa1: call 0x180002040
006b6fa6: jmp 0x1806b7a62
006b6fab: mov rax, qword ptr [rbx + 0x18]
006b6faf: mov rdi, qword ptr [rbx + 0x140]
006b6fb6: test rax, rax
006b6fb9: je 0x1806b7aad
006b6fbf: mov rcx, qword ptr [rip + 0x4573ffa]
006b6fc6: mov esi, dword ptr [rax + 0x1ec]
006b6fcc: cmp dword ptr [rcx + 0xe4], 0
006b6fd3: jne 0x1806b6fda
006b6fd5: call 0x1804feeb0
006b6fda: xor r9d, r9d
006b6fdd: lea rcx, [rsp + 0x60]
006b6fe2: mov r8d, 0x4b
006b6fe8: mov edx, esi
006b6fea: call 0x1806cb9c0
006b6fef: test rdi, rdi
006b6ff2: je 0x1806b7aad
006b6ff8: movups xmm0, xmmword ptr [rax]
006b6ffb: xor r9d, r9d
006b6ffe: lea rdx, [rsp + 0x50]
006b7003: xor r8d, r8d
006b7006: mov rcx, rdi
006b7009: movaps xmmword ptr [rsp + 0x50], xmm0
006b700e: call 0x18060f880
006b7013: jmp 0x1806b7a62
006b7018: mov rcx, qword ptr [rbx + 0x140]
006b701f: test rcx, rcx
006b7022: je 0x1806b7aad
006b7028: movss xmm1, dword ptr [rip + 0x357cc10]
006b7030: xor r9d, r9d
006b7033: xor r8d, r8d
006b7036: call 0x1807c8910
006b703b: test al, al
006b703d: je 0x1806b7070
006b703f: mov rax, qword ptr [rbx + 0x10]
006b7043: test rax, rax
006b7046: je 0x1806b7aad
006b704c: mov r8, qword ptr [rax + 0x58]
006b7050: test r8, r8
006b7053: je 0x1806b7aad
006b7059: mov rdx, qword ptr [rip + 0x45be108]
006b7060: mov ecx, 6
006b7065: mov r9d, 0x40
006b706b: call 0x180002040
006b7070: mov rcx, qword ptr [rbx + 0x140]
006b7077: test rcx, rcx
006b707a: je 0x1806b7aad
006b7080: xor r8d, r8d
006b7083: xor edx, edx
006b7085: call 0x1807c7a30
006b708a: test eax, eax
006b708c: jle 0x1806b7a62
006b7092: movss xmm1, dword ptr [rip + 0x357cd1a]
006b709a: xor r8d, r8d
006b709d: mov rcx, rbx
006b70a0: mov dword ptr [rbx + 0x88], 0x25
006b70aa: call 0x1806b4c50
006b70af: jmp 0x1806b7a62
006b70b4: cmp dword ptr [rbx + 0xa0], 0
006b70bb: jne 0x1806b7a62
006b70c1: cmp byte ptr [rip + 0x4834590], 0
006b70c8: mov dword ptr [rbx + 0x88], 0x24
006b70d2: mov rdi, qword ptr [rip + 0x45dc727]
006b70d9: jne 0x1806b70fa
006b70db: lea rcx, [rip + 0x45db99e]
006b70e2: call 0x1804feb70
006b70e7: lea rcx, [rip + 0x45da10a]
006b70ee: call 0x1804feb70
006b70f3: mov byte ptr [rip + 0x483455e], 1
006b70fa: test rdi, rdi
006b70fd: je 0x1806b7aad
006b7103: mov r8, qword ptr [rip + 0x45da0ee]
006b710a: xor r9d, r9d
006b710d: mov rdx, qword ptr [rip + 0x45db96c]
006b7114: mov rcx, rdi
006b7117: call 0x182a24e50
006b711c: mov rcx, qword ptr [rbx + 0x140]
006b7123: mov rdi, rax
006b7126: test rcx, rcx
006b7129: je 0x1806b7aad
006b712f: movss xmm1, dword ptr [rip + 0x357cc85]
006b7137: xor r9d, r9d
006b713a: xor r8d, r8d
006b713d: call 0x18060f880
006b7142: mov rcx, qword ptr [rbx + 0x140]
006b7149: test rcx, rcx
006b714c: je 0x1806b7aad
006b7152: movss xmm3, dword ptr [rip + 0x357cc5a]
006b715a: xor esi, esi
006b715c: mov qword ptr [rsp + 0x28], rsi
006b7161: xor r8d, r8d
006b7164: mov rdx, rdi
006b7167: mov dword ptr [rsp + 0x20], 3
006b716f: call 0x1807c8270
006b7174: jmp 0x1806b7a62
006b7179: cmp dword ptr [rbx + 0x88], 0x28
006b7180: je 0x1806b71de
006b7182: cmp dword ptr [rbx + 0x88], 0x29
006b7189: jne 0x1806b7a62
006b718f: mov rcx, qword ptr [rbx + 0x140]
006b7196: test rcx, rcx
006b7199: je 0x1806b7aad
006b719f: xor r8d, r8d
006b71a2: xor edx, edx
006b71a4: call 0x1807c7a30
006b71a9: test eax, eax
006b71ab: jle 0x1806b7a62
006b71b1: movss xmm1, dword ptr [rip + 0x357cbf3]
006b71b9: xor r8d, r8d
006b71bc: mov rcx, rbx
006b71bf: call 0x1806b4c50
006b71c4: xor esi, esi
006b71c6: xor edx, edx
006b71c8: mov rcx, rbx
006b71cb: mov dword ptr [rbx + 0x88], esi
006b71d1: call 0x1806ac2a0
006b71d6: mov dword ptr [rbx + 0x38], eax
006b71d9: jmp 0x1806b7a62
006b71de: cmp dword ptr [rbx + 0xa0], 0
006b71e5: jne 0x1806b7a62
006b71eb: mov edx, dword ptr [rbx + 0x34]
006b71ee: xor r9d, r9d
006b71f1: inc edx
006b71f3: xor r8d, r8d
006b71f6: mov ecx, 0x4a768
006b71fb: call 0x180665070
006b7200: mov dword ptr [rbx + 0x38], eax
006b7203: mov dword ptr [rbx + 0x88], 0x29
006b720d: jmp 0x1806b7a62
006b7212: cmp dword ptr [rbx + 0x88], 2
006b7219: jne 0x1806b7a62
006b721f: mov rcx, qword ptr [rbx + 0x140]
006b7226: test rcx, rcx
006b7229: je 0x1806b7aad
006b722f: xor r8d, r8d
006b7232: xor edx, edx
006b7234: call 0x1807c7a30
006b7239: test eax, eax
006b723b: jle 0x1806b7a62
006b7241: xor edx, edx
006b7243: mov rcx, rbx
006b7246: call 0x1806acd40
006b724b: jmp 0x1806b7a62
006b7250: cmp byte ptr [rip + 0x483441e], 0
006b7257: jne 0x1806b726c
006b7259: lea rcx, [rip + 0x45dd758]
006b7260: call 0x1804feb70
006b7265: mov byte ptr [rip + 0x4834409], 1
006b726c: cmp dword ptr [rbx + 0x88], 0x2a
006b7273: jne 0x1806b7373
006b7279: mov rcx, qword ptr [rbx + 0x140]
006b7280: test rcx, rcx
006b7283: je 0x1806b7aad
006b7289: movss xmm1, dword ptr [rip + 0x357cc2b]
006b7291: xor r9d, r9d
006b7294: xor r8d, r8d
006b7297: call 0x1807c8910
006b729c: xor esi, esi
006b729e: test al, al
006b72a0: je 0x1806b72e1
006b72a2: mov rcx, qword ptr [rbx + 0x10]
006b72a6: test rcx, rcx
006b72a9: je 0x1806b7aad
006b72af: movss xmm2, dword ptr [rbx + 0x24]
006b72b4: mov r9d, 0x61a80
006b72ba: movss xmm1, dword ptr [rbx + 0x20]
006b72bf: addss xmm2, dword ptr [rip + 0x357cafd]
006b72c7: addss xmm1, dword ptr [rip + 0x357caf5]
006b72cf: mov qword ptr [rsp + 0x28], rsi
006b72d4: mov dword ptr [rsp + 0x20], 0x47
006b72dc: call 0x1807d9b50
006b72e1: mov rcx, qword ptr [rbx + 0x140]
006b72e8: test rcx, rcx
006b72eb: je 0x1806b7aad
006b72f1: xor r8d, r8d
006b72f4: xor edx, edx
006b72f6: call 0x1807c7a30
006b72fb: test eax, eax
006b72fd: jle 0x1806b7a62
006b7303: xor edx, edx
006b7305: mov rcx, rbx
006b7308: call 0x1806acd40
006b730d: mov rcx, qword ptr [rbx + 0x18]
006b7311: test rcx, rcx
006b7314: je 0x1806b7aad
006b731a: mov r9d, dword ptr [rbx + 0x12c]
006b7321: mov r8d, dword ptr [rbx + 0x34]
006b7325: mov edx, dword ptr [rbx + 0x74]
006b7328: mov qword ptr [rsp + 0x38], rsi
006b732d: mov byte ptr [rsp + 0x30], sil
006b7332: mov byte ptr [rsp + 0x28], sil
006b7337: mov dword ptr [rsp + 0x20], 0x30
006b733f: call 0x1806553e0
006b7344: mov rdi, rax
006b7347: test rax, rax
006b734a: je 0x1806b7aad
006b7350: mov rcx, qword ptr [rax + 0x140]
006b7357: test rcx, rcx
006b735a: je 0x1806b7aad
006b7360: xor edx, edx
006b7362: call 0x1807b6ef0
006b7367: mov byte ptr [rdi + 0x139], 1
006b736e: jmp 0x1806b7a62
006b7373: cmp dword ptr [rbx + 0xa0], 0
006b737a: jne 0x1806b7a62
006b7380: cmp byte ptr [rip + 0x48342d1], 0
006b7387: mov dword ptr [rbx + 0x88], 0x2a
006b7391: mov rdi, qword ptr [rip + 0x45dd620]
006b7398: jne 0x1806b73b9
006b739a: lea rcx, [rip + 0x45db6df]
006b73a1: call 0x1804feb70
006b73a6: lea rcx, [rip + 0x45d9e4b]
006b73ad: call 0x1804feb70
006b73b2: mov byte ptr [rip + 0x483429f], 1
006b73b9: test rdi, rdi
006b73bc: je 0x1806b7aad
006b73c2: mov r8, qword ptr [rip + 0x45d9e2f]
006b73c9: xor r9d, r9d
006b73cc: mov rdx, qword ptr [rip + 0x45db6ad]
006b73d3: mov rcx, rdi
006b73d6: call 0x182a24e50
006b73db: mov rcx, qword ptr [rbx + 0x140]
006b73e2: test rcx, rcx
006b73e5: je 0x1806b7aad
006b73eb: movss xmm3, dword ptr [rip + 0x357cbbd]
006b73f3: xor esi, esi
006b73f5: mov qword ptr [rsp + 0x28], rsi
006b73fa: xor r8d, r8d
006b73fd: mov rdx, rax
006b7400: mov dword ptr [rsp + 0x20], 3
006b7408: call 0x1807c8270
006b740d: jmp 0x1806b7a62
006b7412: cmp dword ptr [rbx + 0x88], 0x30
006b7419: jne 0x1806b7a62
006b741f: cmp dword ptr [rbx + 0xa0], 0
006b7426: je 0x1806b7447
006b7428: jmp 0x1806b7a62
006b742d: cmp dword ptr [rbx + 0x88], 0x2f
006b7434: jne 0x1806b7a62
006b743a: cmp dword ptr [rbx + 0xa0], 0
006b7441: jne 0x1806b7a62
006b7447: xor esi, esi
006b7449: mov dword ptr [rbx + 0x88], esi
006b744f: jmp 0x1806b7a62
006b7454: cmp byte ptr [rip + 0x483420e], 0
006b745b: jne 0x1806b7470
006b745d: lea rcx, [rip + 0x45734ac]
006b7464: call 0x1804feb70
006b7469: mov byte ptr [rip + 0x48341f9], 1
006b7470: mov rcx, qword ptr [rbx + 0x140]
006b7477: test rcx, rcx
006b747a: je 0x1806b7aad
006b7480: xor r8d, r8d
006b7483: xor edx, edx
006b7485: call 0x1807c7a30
006b748a: test eax, eax
006b748c: jle 0x1806b74ce
006b748e: mov rcx, qword ptr [rbx + 0x140]
006b7495: test rcx, rcx
006b7498: je 0x1806b7aad
006b749e: xor r8d, r8d
006b74a1: xor edx, edx
006b74a3: call 0x1807c76c0
006b74a8: test eax, eax
006b74aa: je 0x1806b74ce
006b74ac: mov rcx, qword ptr [rbx + 0x140]
006b74b3: test rcx, rcx
006b74b6: je 0x1806b7aad
006b74bc: mov rdx, qword ptr [rip + 0x457344d]
006b74c3: xor r9d, r9d
006b74c6: xor r8d, r8d
006b74c9: call 0x1807c8230
006b74ce: cmp dword ptr [rbx + 0x88], 2
006b74d5: je 0x1806b7a62
006b74db: cmp dword ptr [rbx + 0x9c], 0
006b74e2: jne 0x1806b7a62
006b74e8: xor edx, edx
006b74ea: mov rcx, rbx
006b74ed: call 0x1806ad780
006b74f2: jmp 0x1806b7a62
006b74f7: cmp byte ptr [rip + 0x4834157], 0
006b74fe: jne 0x1806b7537
006b7500: lea rcx, [rip + 0x45bdc61]
006b7507: call 0x1804feb70
006b750c: lea rcx, [rip + 0x45dbd95]
006b7513: call 0x1804feb70
006b7518: lea rcx, [rip + 0x4581c49]
006b751f: call 0x1804feb70
006b7524: lea rcx, [rip + 0x45f68a5]
006b752b: call 0x1804feb70
006b7530: mov byte ptr [rip + 0x483411e], 1
006b7537: cmp dword ptr [rbx + 0x88], 1
006b753e: je 0x1806b781c
006b7544: cmp dword ptr [rbx + 0x88], 0xa
006b754b: je 0x1806b76d5
006b7551: cmp dword ptr [rbx + 0x88], 0xb
006b7558: je 0x1806b7636
006b755e: cmp dword ptr [rbx + 0x88], 0xd
006b7565: je 0x1806b75c1
006b7567: cmp dword ptr [rbx + 0x88], 0xe
006b756e: je 0x1806b757d
006b7570: cmp dword ptr [rbx + 0x88], 0xc
006b7577: jne 0x1806b7a62
006b757d: mov rcx, qword ptr [rbx + 0x140]
006b7584: test rcx, rcx
006b7587: je 0x1806b7aad
006b758d: xor r8d, r8d
006b7590: xor edx, edx
006b7592: call 0x1807c7a30
006b7597: test eax, eax
006b7599: jle 0x1806b7a62
006b759f: movss xmm1, dword ptr [rip + 0x357c80d]
006b75a7: xor r8d, r8d
006b75aa: mov rcx, rbx
006b75ad: call 0x1806b4c50
006b75b2: mov dword ptr [rbx + 0x88], 1
006b75bc: jmp 0x1806b7a62
006b75c1: cmp dword ptr [rbx + 0xa0], 0
006b75c8: jne 0x1806b7a62
006b75ce: mov rcx, qword ptr [rbx + 0x140]
006b75d5: test rcx, rcx
006b75d8: je 0x1806b7aad
006b75de: movss xmm1, dword ptr [rip + 0x357c7d6]
006b75e6: xor r9d, r9d
006b75e9: xor r8d, r8d
006b75ec: call 0x18060f880
006b75f1: mov rcx, qword ptr [rbx + 0x140]
006b75f8: test rcx, rcx
006b75fb: je 0x1806b7aad
006b7601: movss xmm3, dword ptr [rip + 0x357c7ab]
006b7609: xor esi, esi
006b760b: mov rdx, qword ptr [rip + 0x45dbc96]
006b7612: xor r8d, r8d
006b7615: mov qword ptr [rsp + 0x28], rsi
006b761a: mov dword ptr [rsp + 0x20], 3
006b7622: call 0x1807c8270
006b7627: mov dword ptr [rbx + 0x88], 0xe
006b7631: jmp 0x1806b7a62
006b7636: mov rcx, qword ptr [rbx + 0x140]
006b763d: test rcx, rcx
006b7640: je 0x1806b7aad
006b7646: xor r8d, r8d
006b7649: xor edx, edx
006b764b: call 0x1807c7a30
006b7650: test eax, eax
006b7652: jle 0x1806b7a62
006b7658: mov rcx, qword ptr [rbx + 0x140]
006b765f: test rcx, rcx
006b7662: je 0x1806b7aad
006b7668: movss xmm3, dword ptr [rip + 0x357c748]
006b7670: xor esi, esi
006b7672: mov rdx, qword ptr [rip + 0x4581aef]
006b7679: xor r8d, r8d
006b767c: mov qword ptr [rsp + 0x20], rsi
006b7681: call 0x1807c8210
006b7686: mov rcx, qword ptr [rbx + 0x10]
006b768a: test rcx, rcx
006b768d: je 0x1806b7aad
006b7693: xor edx, edx
006b7695: call 0x1807df5d0
006b769a: test al, al
006b769c: je 0x1806b76bc
006b769e: mov rcx, qword ptr [rbx + 0x140]
006b76a5: test rcx, rcx
006b76a8: je 0x1806b7aad
006b76ae: xor r9d, r9d
006b76b1: xor r8d, r8d
006b76b4: xorps xmm1, xmm1
006b76b7: call 0x1807c8740
006b76bc: mov dword ptr [rbx + 0x88], 0xd
006b76c6: mov dword ptr [rbx + 0xa0], 0xfa0
006b76d0: jmp 0x1806b7a62
006b76d5: cmp dword ptr [rbx + 0xa0], 0
006b76dc: jne 0x1806b7a62
006b76e2: mov rax, qword ptr [rbx + 0x10]
006b76e6: test rax, rax
006b76e9: je 0x1806b7aad
006b76ef: mov r8, qword ptr [rax + 0x58]
006b76f3: test r8, r8
006b76f6: je 0x1806b7aad
006b76fc: mov rdx, qword ptr [rip + 0x45bda65]
006b7703: mov ecx, 6
006b7708: mov r9d, 0x3d
006b770e: call 0x180002040
006b7713: mov edx, dword ptr [rbx + 0x34]
006b7716: xor r9d, r9d
006b7719: xor r8d, r8d
006b771c: mov rcx, rbx
006b771f: call 0x1806b0040
006b7724: xor bpl, bpl
006b7727: mov rdi, rax
006b772a: test rax, rax
006b772d: je 0x1806b7749
006b772f: cmp dword ptr [rax + 0x70], 0x17
006b7733: je 0x1806b7741
006b7735: cmp dword ptr [rax + 0x70], 0x20
006b7739: je 0x1806b7741
006b773b: cmp dword ptr [rax + 0x70], 0x19
006b773f: jne 0x1806b7749
006b7741: mov bpl, 1
006b7744: xor sil, sil
006b7747: jmp 0x1806b7751
006b7749: xor sil, sil
006b774c: test rdi, rdi
006b774f: je 0x1806b7779
006b7751: xor edx, edx
006b7753: mov rcx, rdi
006b7756: call 0x1806f3670
006b775b: test al, al
006b775d: jne 0x1806b777c
006b775f: xor edx, edx
006b7761: mov rcx, rdi
006b7764: call 0x1806f35e0
006b7769: test al, al
006b776b: jne 0x1806b7779
006b776d: cmp dword ptr [rdi + 0x74], 0xc
006b7771: je 0x1806b7779
006b7773: cmp dword ptr [rdi + 0x74], 0xb
006b7777: jne 0x1806b777c
006b7779: mov sil, 1
006b777c: test bpl, bpl
006b777f: jne 0x1806b77c0
006b7781: test sil, sil
006b7784: jne 0x1806b780d
006b778a: test rdi, rdi
006b778d: je 0x1806b7aad
006b7793: xor edx, edx
006b7795: mov rcx, rdi
006b7798: call 0x1806ec320
006b779d: mov rax, qword ptr [rbx + 0x18]
006b77a1: test rax, rax
006b77a4: je 0x1806b7aad
006b77aa: mov byte ptr [rax + 0x239], sil
006b77b1: mov dword ptr [rbx + 0x88], 0xb
006b77bb: jmp 0x1806b7a62
006b77c0: mov rax, qword ptr [rbx + 0x10]
006b77c4: test rax, rax
006b77c7: je 0x1806b7aad
006b77cd: mov r8, qword ptr [rax + 0x58]
006b77d1: test r8, r8
006b77d4: je 0x1806b7aad
006b77da: mov rdx, qword ptr [rip + 0x45bd987]
006b77e1: mov ecx, 6
006b77e6: mov r9d, 1
006b77ec: call 0x180002040
006b77f1: test rdi, rdi
006b77f4: je 0x1806b7aad
006b77fa: xor r9d, r9d
006b77fd: xor r8d, r8d
006b7800: mov edx, 0x28
006b7805: mov rcx, rdi
006b7808: call 0x1806f8f60
006b780d: mov dword ptr [rbx + 0x88], 0xc
006b7817: jmp 0x1806b7a62
006b781c: mov edx, dword ptr [rbx + 0x34]
006b781f: xor r9d, r9d
006b7822: xor r8d, r8d
006b7825: mov rcx, rbx
006b7828: call 0x1806b0040
006b782d: test rax, rax
006b7830: je 0x1806b7a62
006b7836: mov rcx, qword ptr [rbx + 0x140]
006b783d: test rcx, rcx
006b7840: je 0x1806b7aad
006b7846: movss xmm1, dword ptr [rip + 0x357c56e]
006b784e: xor r9d, r9d
006b7851: xor r8d, r8d
006b7854: call 0x18060f880
006b7859: mov rcx, qword ptr [rbx + 0x140]
006b7860: test rcx, rcx
006b7863: je 0x1806b7aad
006b7869: xor r9d, r9d
006b786c: xor r8d, r8d
006b786f: mov edx, 3
006b7874: call 0x1807c8640
006b7879: mov rcx, qword ptr [rbx + 0x140]
006b7880: test rcx, rcx
006b7883: je 0x1806b7aad
006b7889: movss xmm3, dword ptr [rip + 0x357c65f]
006b7891: xor esi, esi
006b7893: mov rdx, qword ptr [rip + 0x45f6536]
006b789a: xor r8d, r8d
006b789d: mov qword ptr [rsp + 0x28], rsi
006b78a2: mov dword ptr [rsp + 0x20], 3
006b78aa: call 0x1807c8270
006b78af: mov dword ptr [rbx + 0x88], 0xa
006b78b9: mov dword ptr [rbx + 0xa0], 0x46
006b78c3: jmp 0x1806b7a62
006b78c8: cmp byte ptr [rbx + 0x136], 0
006b78cf: jne 0x1806b7a62
006b78d5: cmp dword ptr [rbx + 0x88], 2
006b78dc: je 0x1806b7a62
006b78e2: mov dword ptr [rbx + 0x88], 2
006b78ec: mov dword ptr [rbx + 0x9c], 0x64
006b78f6: jmp 0x1806b7a62
006b78fb: cmp byte ptr [rip + 0x4833d59], 0
006b7902: jne 0x1806b7947
006b7904: lea rcx, [rip + 0x45bd85d]
006b790b: call 0x1804feb70
006b7910: lea rcx, [rip + 0x45d6609]
006b7917: call 0x1804feb70
006b791c: lea rcx, [rip + 0x45b8ced]
006b7923: call 0x1804feb70
006b7928: lea rcx, [rip + 0x45d6441]
006b792f: call 0x1804feb70
006b7934: lea rcx, [rip + 0x45d650d]
006b793b: call 0x1804feb70
006b7940: mov byte ptr [rip + 0x4833d14], 1
006b7947: cmp byte ptr [rbx + 0x136], 0
006b794e: jne 0x1806b7a62
006b7954: cmp dword ptr [rbx + 0x88], 2
006b795b: je 0x1806b7a62
006b7961: mov rcx, qword ptr [rbx + 0x140]
006b7968: mov dword ptr [rbx + 0x88], 2
006b7972: mov dword ptr [rbx + 0x9c], 0x64
006b797c: test rcx, rcx
006b797f: je 0x1806b7aad
006b7985: movss xmm3, dword ptr [rip + 0x357c55f]
006b798d: xor esi, esi
006b798f: mov rdx, qword ptr [rip + 0x45b8c7a]
006b7996: xor r8d, r8d
006b7999: mov qword ptr [rsp + 0x28], rsi
006b799e: mov dword ptr [rsp + 0x20], 3
006b79a6: call 0x1807c8270
006b79ab: mov rcx, qword ptr [rbx + 0x140]
006b79b2: test rcx, rcx
006b79b5: je 0x1806b7aad
006b79bb: movss xmm2, dword ptr [rip + 0x357c289]
006b79c3: xor r9d, r9d
006b79c6: mov rdx, qword ptr [rip + 0x45d63a3]
006b79cd: mov qword ptr [rsp + 0x20], rsi
006b79d2: call 0x18060f880
006b79d7: mov rcx, qword ptr [rbx + 0x140]
006b79de: test rcx, rcx
006b79e1: je 0x1806b7aad
006b79e7: movss xmm6, dword ptr [rip + 0x357c3a5]
006b79ef: xor r9d, r9d
006b79f2: mov rdx, qword ptr [rip + 0x45d644f]
006b79f9: movaps xmm2, xmm6
006b79fc: mov qword ptr [rsp + 0x20], rsi
006b7a01: call 0x18060f880
006b7a06: mov rcx, qword ptr [rbx + 0x140]
006b7a0d: test rcx, rcx
006b7a10: je 0x1806b7aad
006b7a16: mov rdx, qword ptr [rip + 0x45d6503]
006b7a1d: xor r9d, r9d
006b7a20: movaps xmm2, xmm6
006b7a23: mov qword ptr [rsp + 0x20], rsi
006b7a28: call 0x18060f880
006b7a2d: mov r8, qword ptr [rbx + 0x10]
006b7a31: test r8, r8
006b7a34: je 0x1806b7aad
006b7a36: mov r8, qword ptr [r8 + 0x58]
006b7a3a: test r8, r8
006b7a3d: je 0x1806b7aad
006b7a3f: mov rdx, qword ptr [rip + 0x45bd722]
006b7a46: mov ecx, 6
006b7a4b: mov r9d, 0x39
006b7a51: call 0x180002040
006b7a56: jmp 0x1806b7a62
006b7a58: xor edx, edx
006b7a5a: mov rcx, rbx
006b7a5d: call 0x1806bd300
006b7a62: cmp dword ptr [rbx + 0x94], 1
006b7a69: jne 0x1806b7a75
006b7a6b: xor edx, edx
006b7a6d: mov rcx, rbx
006b7a70: call 0x1806bc420
006b7a75: cmp dword ptr [rbx + 0x9c], 0
006b7a7c: jle 0x1806b630f
006b7a82: sub dword ptr [rbx + 0x9c], 1
006b7a89: jne 0x1806b630f
006b7a8f: xor edx, edx
006b7a91: mov rcx, rbx
006b7a94: call 0x1806ad780
006b7a99: jmp 0x1806b630f
006b7a9e: xor edx, edx
006b7aa0: mov rcx, rbx
006b7aa3: call 0x1806acd40
006b7aa8: jmp 0x1806b630f
006b7aad: call 0x1804fee20
006b7ab2: int3
006b7ab3: call 0x1804fee20
006b7ab8: int3
006b7ab9: int3
006b7aba: int3
006b7abb: int3
006b7abc: int3
006b7abd: int3
006b7abe: int3
006b7abf: int3
