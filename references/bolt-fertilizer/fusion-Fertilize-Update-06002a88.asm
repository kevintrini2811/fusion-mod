00682490: push rdi
00682492: sub rsp, 0x30
00682496: cmp byte ptr [rip + 0x2f17b6f], 0
0068249d: mov rdi, rcx
006824a0: jne 0x18068253d
006824a6: lea rcx, [rip + 0x2cf7653]
006824ad: call 0x1803633b0
006824b2: lea rcx, [rip + 0x2d08f6f]
006824b9: call 0x1803633b0
006824be: lea rcx, [rip + 0x2d0c5cb]
006824c5: call 0x1803633b0
006824ca: lea rcx, [rip + 0x2d00d97]
006824d1: call 0x1803633b0
006824d6: lea rcx, [rip + 0x2d6075b]
006824dd: call 0x1803633b0
006824e2: lea rcx, [rip + 0x2d47f97]
006824e9: call 0x1803633b0
006824ee: lea rcx, [rip + 0x2d3f743]
006824f5: call 0x1803633b0
006824fa: lea rcx, [rip + 0x2d10997]
00682501: call 0x1803633b0
00682506: lea rcx, [rip + 0x2d53233]
0068250d: call 0x1803633b0
00682512: lea rcx, [rip + 0x2d5967f]
00682519: call 0x1803633b0
0068251e: lea rcx, [rip + 0x2d4fe73]
00682525: call 0x1803633b0
0068252a: lea rcx, [rip + 0x2d58c57]
00682531: call 0x1803633b0
00682536: mov byte ptr [rip + 0x2f17acf], 1
0068253d: mov qword ptr [rsp + 0x40], rbx
00682542: xor edx, edx
00682544: mov rcx, rdi
00682547: mov qword ptr [rsp + 0x50], rsi
0068254c: call 0x1809d4a40
00682551: mov rax, qword ptr [rip + 0x2d531e8]
00682558: mov rcx, qword ptr [rax + 0xb8]
0068255f: mov rax, qword ptr [rcx]
00682562: test rax, rax
00682565: je 0x1806827fe
0068256b: mov rbx, qword ptr [rax + 0x50]
0068256f: xor edx, edx
00682571: mov rcx, rdi
00682574: call 0x1826b16d0
00682579: mov rcx, qword ptr [rip + 0x2d59618]
00682580: mov rsi, rax
00682583: cmp dword ptr [rcx + 0xe0], 0
0068258a: jne 0x180682591
0068258c: call 0x180318580
00682591: xor r8d, r8d
00682594: mov rdx, rsi
00682597: mov rcx, rbx
0068259a: call 0x1826b9be0
0068259f: test al, al
006825a1: je 0x18068279e
006825a7: cmp byte ptr [rdi + 0x80], 0
006825ae: jne 0x18068279e
006825b4: movaps xmmword ptr [rsp + 0x20], xmm6
006825b9: xor ecx, ecx
006825bb: movss xmm6, dword ptr [rdi + 0x30]
006825c0: call 0x1826c5210
006825c5: addss xmm0, xmm6
006825c9: movaps xmm6, xmmword ptr [rsp + 0x20]
006825ce: comiss xmm0, dword ptr [rip + 0x22f95d3]
006825d5: movss dword ptr [rdi + 0x30], xmm0
006825da: jbe 0x180682608
006825dc: xor edx, edx
006825de: mov rcx, rdi
006825e1: call 0x1826b16d0
006825e6: mov rcx, qword ptr [rip + 0x2d595ab]
006825ed: mov rbx, rax
006825f0: cmp dword ptr [rcx + 0xe0], 0
006825f7: jne 0x1806825fe
006825f9: call 0x180318580
006825fe: xor edx, edx
00682600: mov rcx, rbx
00682603: call 0x1826b8410
00682608: mov rax, qword ptr [rip + 0x2cf74f1]
0068260f: xor r9d, r9d
00682612: xor r8d, r8d
00682615: mov ecx, 0xe4
0068261a: mov rdx, qword ptr [rax + 0xb8]
00682621: mov rdx, qword ptr [rdx]
00682624: call 0x18079ddd0
00682629: test rax, rax
0068262c: je 0x1806827fe
00682632: cmp dword ptr [rax + 0x18], 0
00682636: mov edx, 0x103
0068263b: mov rax, qword ptr [rip + 0x2cf74be]
00682642: setg bl
00682645: xor r8d, r8d
00682648: mov rcx, qword ptr [rax + 0xb8]
0068264f: mov rcx, qword ptr [rcx]
00682652: call 0x18079c030
00682657: test rax, rax
0068265a: je 0x1806827fe
00682660: cmp byte ptr [rdi + 0x60], 0
00682664: je 0x18068279e
0068266a: movss xmm0, dword ptr [rdi + 0x30]
0068266f: comiss xmm0, dword ptr [rip + 0x22f9506]
00682676: jbe 0x18068279e
0068267c: cmp dword ptr [rax + 0x18], 0
00682680: setg al
00682683: or al, bl
00682685: je 0x18068279e
0068268b: xor ecx, ecx
0068268d: mov qword ptr [rsp + 0x48], rbp
00682692: call 0x18079b2c0
00682697: mov rbp, rax
0068269a: mov rax, qword ptr [rip + 0x2d58ae7]
006826a1: cmp dword ptr [rax + 0xe0], 0
006826a8: jne 0x1806826b9
006826aa: mov rcx, rax
006826ad: call 0x180318580
006826b2: mov rax, qword ptr [rip + 0x2d58acf]
006826b9: mov rcx, qword ptr [rax + 0xb8]
006826c0: mov rsi, qword ptr [rcx + 8]
006826c4: test rsi, rsi
006826c7: jne 0x18068273a
006826c9: cmp dword ptr [rax + 0xe0], esi
006826cf: jne 0x1806826e0
006826d1: mov rcx, rax
006826d4: call 0x180318580
006826d9: mov rax, qword ptr [rip + 0x2d58aa8]
006826e0: mov rax, qword ptr [rax + 0xb8]
006826e7: mov rcx, qword ptr [rip + 0x2d00b7a]
006826ee: mov rbx, qword ptr [rax]
006826f1: call 0x18030dc40
006826f6: mov r8, qword ptr [rip + 0x2d4fc9b]
006826fd: xor r9d, r9d
00682700: mov rdx, rbx
00682703: mov rcx, rax
00682706: mov rsi, rax
00682709: call 0x1815d0450
0068270e: mov rax, qword ptr [rip + 0x2d58a73]
00682715: mov rdx, rsi
00682718: mov rcx, qword ptr [rax + 0xb8]
0068271f: mov qword ptr [rcx + 8], rsi
00682723: mov rax, qword ptr [rip + 0x2d58a5e]
0068272a: mov rcx, qword ptr [rax + 0xb8]
00682731: add rcx, 8
00682735: call 0x180362730
0068273a: mov r8, qword ptr [rip + 0x2d0c34f]
00682741: mov rdx, rsi
00682744: mov rcx, rbp
00682747: call 0x180afbcc0
0068274c: mov rdx, qword ptr [rip + 0x2d08cd5]
00682753: mov rcx, rax
00682756: call 0x180afaf90
0068275b: mov rbp, qword ptr [rsp + 0x48]
00682760: mov rbx, rax
00682763: test rax, rax
00682766: je 0x1806827fe
0068276c: cmp dword ptr [rax + 0x18], 0
00682770: jg 0x1806827ae
00682772: xor edx, edx
00682774: mov rcx, rdi
00682777: call 0x1826b16d0
0068277c: mov rcx, qword ptr [rip + 0x2d59415]
00682783: mov rbx, rax
00682786: cmp dword ptr [rcx + 0xe0], 0
0068278d: jne 0x180682794
0068278f: call 0x180318580
00682794: xor edx, edx
00682796: mov rcx, rbx
00682799: call 0x1826b8410
0068279e: mov rsi, qword ptr [rsp + 0x50]
006827a3: mov rbx, qword ptr [rsp + 0x40]
006827a8: add rsp, 0x30
006827ac: pop rdi
006827ad: ret
006827ae: mov rcx, qword ptr [rip + 0x2d47ccb]
006827b5: cmp dword ptr [rcx + 0xe0], 0
006827bc: jne 0x1806827c3
006827be: call 0x180318580
006827c3: mov rdx, qword ptr [rip + 0x2d6046e]
006827ca: mov rcx, rbx
006827cd: call 0x180b7d920
006827d2: test rax, rax
006827d5: je 0x1806827fe
006827d7: mov r8d, dword ptr [rax + 0xf8]
006827de: xor r9d, r9d
006827e1: mov edx, dword ptr [rax + 0xf4]
006827e7: mov rcx, rdi
006827ea: mov rsi, qword ptr [rsp + 0x50]
006827ef: mov rbx, qword ptr [rsp + 0x40]
006827f4: add rsp, 0x30
006827f8: pop rdi
006827f9: jmp 0x180683730
006827fe: call 0x180363610
00682803: int3
00682804: int3
00682805: int3
00682806: int3
00682807: int3
00682808: int3
00682809: int3
0068280a: int3
0068280b: int3
0068280c: int3
0068280d: int3
0068280e: int3
0068280f: int3
