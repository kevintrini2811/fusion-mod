006f12f0: mov qword ptr [rsp + 0x18], rbx
006f12f5: push rsi
006f12f6: sub rsp, 0x40
006f12fa: cmp byte ptr [rip + 0x47fa4a7], 0
006f1301: mov esi, edx
006f1303: mov rbx, rcx
006f1306: jne 0x1806f131b
006f1308: lea rcx, [rip + 0x4539cb1]
006f130f: call 0x1804feb70
006f1314: mov byte ptr [rip + 0x47fa48d], 1
006f131b: mov rcx, qword ptr [rip + 0x4539c9e]
006f1322: cmp dword ptr [rcx + 0xe4], 0
006f1329: jne 0x1806f1330
006f132b: call 0x1804feeb0
006f1330: test sil, 0x20
006f1334: ja 0x1806f135e
006f1336: cmp byte ptr [rbx + 0x144], 0
006f133d: jne 0x1806f1351
006f133f: cmp dword ptr [rbx + 0x74], 1
006f1343: je 0x1806f1351
006f1345: cmp dword ptr [rbx + 0x74], 2
006f1349: je 0x1806f1351
006f134b: cmp dword ptr [rbx + 0x74], 3
006f134f: jne 0x1806f135e
006f1351: xor al, al
006f1353: mov rbx, qword ptr [rsp + 0x60]
006f1358: add rsp, 0x40
006f135c: pop rsi
006f135d: ret
006f135e: mov rcx, qword ptr [rip + 0x4539c5b]
006f1365: cmp dword ptr [rcx + 0xe4], 0
006f136c: jne 0x1806f1373
006f136e: call 0x1804feeb0
006f1373: movzx eax, byte ptr [rbx + 0x110]
006f137a: movaps xmmword ptr [rsp + 0x30], xmm6
006f137f: test esi, 0x80
006f1385: ja 0x1806f139d
006f1387: test al, al
006f1389: je 0x1806f13a1
006f138b: movaps xmm6, xmmword ptr [rsp + 0x30]
006f1390: xor al, al
006f1392: mov rbx, qword ptr [rsp + 0x60]
006f1397: add rsp, 0x40
006f139b: pop rsi
006f139c: ret
006f139d: test al, al
006f139f: je 0x1806f138b
006f13a1: cmp dword ptr [rbx + 0x70], 0x14
006f13a5: jne 0x1806f13b3
006f13a7: cmp dword ptr [rbx + 0x74], 6
006f13ab: je 0x1806f13b3
006f13ad: cmp dword ptr [rbx + 0x74], 7
006f13b1: jne 0x1806f138b
006f13b3: cmp dword ptr [rbx + 0xb8], 9
006f13ba: je 0x1806f138b
006f13bc: cmp dword ptr [rbx + 0x70], 0x19
006f13c0: jne 0x1806f142e
006f13c2: cmp dword ptr [rbx + 0x74], 0x56
006f13c6: movss xmm6, dword ptr [rip + 0x3542872]
006f13ce: jne 0x1806f13ef
006f13d0: mov rcx, qword ptr [rbx + 0x1d0]
006f13d7: test rcx, rcx
006f13da: je 0x1806f15f5
006f13e0: xor r8d, r8d
006f13e3: xor edx, edx
006f13e5: call 0x1807c78e0
006f13ea: comiss xmm6, xmm0
006f13ed: ja 0x1806f138b
006f13ef: cmp dword ptr [rbx + 0x74], 0x5a
006f13f3: jne 0x1806f1418
006f13f5: mov rcx, qword ptr [rbx + 0x1d0]
006f13fc: test rcx, rcx
006f13ff: je 0x1806f15f5
006f1405: xor r8d, r8d
006f1408: xor edx, edx
006f140a: call 0x1807c78e0
006f140f: comiss xmm0, xmm6
006f1412: ja 0x1806f138b
006f1418: cmp dword ptr [rbx + 0x74], 0x57
006f141c: je 0x1806f142e
006f141e: cmp dword ptr [rbx + 0x74], 0x58
006f1422: je 0x1806f142e
006f1424: cmp dword ptr [rbx + 0x74], 0x59
006f1428: jne 0x1806f138b
006f142e: cmp dword ptr [rbx + 0x70], 0xd
006f1432: jne 0x1806f1446
006f1434: xor edx, edx
006f1436: mov rcx, rbx
006f1439: call 0x1806f1f20
006f143e: test eax, eax
006f1440: jg 0x1806f138b
006f1446: cmp dword ptr [rbx + 0x74], 0xc
006f144a: je 0x1806f15c9
006f1450: cmp dword ptr [rbx + 0x74], 0x47
006f1454: je 0x1806f15c9
006f145a: cmp dword ptr [rbx + 0x74], 0x21
006f145e: je 0x1806f15c9
006f1464: cmp dword ptr [rbx + 0x74], 0x22
006f1468: je 0x1806f15c9
006f146e: cmp dword ptr [rbx + 0x74], 0x23
006f1472: je 0x1806f15c9
006f1478: cmp dword ptr [rbx + 0x74], 0x34
006f147c: je 0x1806f15c9
006f1482: cmp dword ptr [rbx + 0x74], 0x36
006f1486: je 0x1806f15c9
006f148c: cmp dword ptr [rbx + 0x74], 0x3a
006f1490: je 0x1806f15c9
006f1496: cmp dword ptr [rbx + 0x74], 0x4a
006f149a: je 0x1806f15c9
006f14a0: cmp dword ptr [rbx + 0x74], 0xe
006f14a4: je 0x1806f15c9
006f14aa: cmp dword ptr [rbx + 0x74], 0x13
006f14ae: je 0x1806f15c9
006f14b4: cmp dword ptr [rbx + 0x74], 0x32
006f14b8: je 0x1806f15c9
006f14be: cmp dword ptr [rbx + 0x70], 0xd
006f14c2: je 0x1806f14e4
006f14c4: xor r8d, r8d
006f14c7: lea rcx, [rsp + 0x20]
006f14cc: mov rdx, rbx
006f14cf: call 0x1806f2d70
006f14d4: movups xmm0, xmmword ptr [rax]
006f14d7: comiss xmm0, dword ptr [rip + 0x354290a]
006f14de: ja 0x1806f138b
006f14e4: cmp dword ptr [rbx + 0x70], 0xb
006f14e8: mov qword ptr [rsp + 0x58], rdi
006f14ed: jne 0x1806f1507
006f14ef: cmp byte ptr [rbx + 0x115], 0
006f14f6: je 0x1806f1507
006f14f8: xor edi, edi
006f14fa: cmp byte ptr [rbx + 0xa5], dil
006f1501: sete dil
006f1505: jmp 0x1806f1509
006f1507: xor edi, edi
006f1509: mov rcx, qword ptr [rip + 0x4539ab0]
006f1510: cmp dword ptr [rcx + 0xe4], 0
006f1517: jne 0x1806f151e
006f1519: call 0x1804feeb0
006f151e: mov qword ptr [rsp + 0x50], rbp
006f1523: test sil, 4
006f1527: jbe 0x1806f152e
006f1529: test dil, dil
006f152c: jne 0x1806f15a9
006f152e: mov rcx, qword ptr [rip + 0x4539a8b]
006f1535: mov ebp, dword ptr [rbx + 0x74]
006f1538: cmp dword ptr [rcx + 0xe4], 0
006f153f: jne 0x1806f1546
006f1541: call 0x1804feeb0
006f1546: test sil, 0x40
006f154a: jbe 0x1806f1551
006f154c: cmp ebp, 0x20
006f154f: je 0x1806f15a9
006f1551: mov rcx, qword ptr [rip + 0x4539a68]
006f1558: cmp dword ptr [rcx + 0xe4], 0
006f155f: jne 0x1806f1566
006f1561: call 0x1804feeb0
006f1566: test sil, 2
006f156a: jbe 0x1806f1578
006f156c: cmp dword ptr [rbx + 0x74], 0x49
006f1570: je 0x1806f15a9
006f1572: cmp dword ptr [rbx + 0x74], 0x4a
006f1576: je 0x1806f15a9
006f1578: mov rcx, qword ptr [rip + 0x4539a41]
006f157f: cmp dword ptr [rcx + 0xe4], 0
006f1586: jne 0x1806f158d
006f1588: call 0x1804feeb0
006f158d: test sil, 1
006f1591: jbe 0x1806f15c5
006f1593: cmp dword ptr [rbx + 0x74], 0x49
006f1597: je 0x1806f15c5
006f1599: cmp dword ptr [rbx + 0x74], 0x4a
006f159d: je 0x1806f15c5
006f159f: test dil, dil
006f15a2: jne 0x1806f15c5
006f15a4: cmp ebp, 0x20
006f15a7: je 0x1806f15c5
006f15a9: mov al, 1
006f15ab: mov rbp, qword ptr [rsp + 0x50]
006f15b0: mov rdi, qword ptr [rsp + 0x58]
006f15b5: movaps xmm6, xmmword ptr [rsp + 0x30]
006f15ba: mov rbx, qword ptr [rsp + 0x60]
006f15bf: add rsp, 0x40
006f15c3: pop rsi
006f15c4: ret
006f15c5: xor al, al
006f15c7: jmp 0x1806f15ab
006f15c9: mov rcx, qword ptr [rip + 0x45399f0]
006f15d0: cmp dword ptr [rcx + 0xe4], 0
006f15d7: jne 0x1806f15de
006f15d9: call 0x1804feeb0
006f15de: movaps xmm6, xmmword ptr [rsp + 0x30]
006f15e3: test sil, 0x10
006f15e7: mov rbx, qword ptr [rsp + 0x60]
006f15ec: seta al
006f15ef: add rsp, 0x40
006f15f3: pop rsi
006f15f4: ret
006f15f5: call 0x1804fee20
006f15fa: int3
006f15fb: int3
006f15fc: int3
006f15fd: int3
006f15fe: int3
006f15ff: int3
