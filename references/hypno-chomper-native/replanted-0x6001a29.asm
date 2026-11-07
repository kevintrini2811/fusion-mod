006b12f0: cmp dword ptr [rcx + 0x70], 0x1a
006b12f4: je 0x1806b13a2
006b12fa: cmp dword ptr [rcx + 0x70], 2
006b12fe: je 0x1806b139c
006b1304: cmp dword ptr [rcx + 0x70], 0x14
006b1308: je 0x1806b139c
006b130e: cmp dword ptr [rcx + 0x70], 0x2f
006b1312: je 0x1806b139c
006b1318: cmp dword ptr [rcx + 0x70], 0xf
006b131c: je 0x1806b139c
006b131e: cmp dword ptr [rcx + 0x70], 0x27
006b1322: je 0x1806b1396
006b1324: cmp dword ptr [rcx + 0x70], 0x20
006b1328: je 0x1806b1396
006b132a: cmp dword ptr [rcx + 0x70], 0x22
006b132e: je 0x1806b1396
006b1330: cmp dword ptr [rcx + 0x70], 0x2c
006b1334: je 0x1806b1396
006b1336: cmp dword ptr [rcx + 0x70], 4
006b133a: je 0x1806b1390
006b133c: cmp dword ptr [rcx + 0x70], 0x11
006b1340: je 0x1806b1396
006b1342: cmp dword ptr [rcx + 0x70], 8
006b1346: je 0x1806b138a
006b1348: cmp dword ptr [rcx + 0x70], 0x18
006b134c: je 0x1806b138a
006b134e: cmp dword ptr [rcx + 0x70], 0xa
006b1352: je 0x1806b138a
006b1354: cmp dword ptr [rcx + 0x70], 0x2a
006b1358: je 0x1806b138a
006b135a: cmp dword ptr [rcx + 0x70], 6
006b135e: je 0x1806b138a
006b1360: cmp dword ptr [rcx + 0x70], 0x2b
006b1364: je 0x1806b1384
006b1366: cmp dword ptr [rcx + 0x70], 0x13
006b136a: je 0x1806b137e
006b136c: cmp dword ptr [rcx + 0x70], 0x32
006b1370: mov eax, 0x11
006b1375: mov edx, 1
006b137a: cmovne eax, edx
006b137d: ret
006b137e: mov eax, 5
006b1383: ret
006b1384: mov eax, 0xb
006b1389: ret
006b138a: mov eax, 9
006b138f: ret
006b1390: mov eax, 0x4d
006b1395: ret
006b1396: mov eax, 0xd
006b139b: ret
006b139c: mov eax, 0x7f
006b13a1: ret
006b13a2: xor eax, eax
006b13a4: cmp edx, 1
006b13a7: setne al
006b13aa: inc eax
006b13ac: ret
006b13ad: int3
006b13ae: int3
006b13af: int3
