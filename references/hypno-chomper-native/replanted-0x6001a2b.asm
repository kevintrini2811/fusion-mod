006b1660: mov qword ptr [rsp + 8], rbx
006b1665: mov qword ptr [rsp + 0x10], rsi
006b166a: push rdi
006b166b: sub rsp, 0x20
006b166f: xorps xmm0, xmm0
006b1672: mov rdi, rcx
006b1675: movups xmmword ptr [rcx], xmm0
006b1678: mov rcx, qword ptr [rdx + 0x10]
006b167c: mov esi, r8d
006b167f: mov rbx, rdx
006b1682: test rcx, rcx
006b1685: je 0x1806b19eb
006b168b: xor edx, edx
006b168d: call 0x1807dfa20
006b1692: test al, al
006b1694: jne 0x1806b19a3
006b169a: cmp esi, 1
006b169d: jne 0x1806b16e3
006b169f: cmp dword ptr [rbx + 0x70], 0x1c
006b16a3: jne 0x1806b16e3
006b16a5: movss xmm0, dword ptr [rbx + 0x20]
006b16aa: addss xmm0, dword ptr [rip + 0x358282e]
006b16b2: mov eax, dword ptr [rbx + 0x24]
006b16b5: mov dword ptr [rdi + 4], eax
006b16b8: mov rax, rdi
006b16bb: mov dword ptr [rdi], 0
006b16c1: movss dword ptr [rdi + 8], xmm0
006b16c6: movd xmm0, dword ptr [rbx + 0x2c]
006b16cb: cvtdq2ps xmm0, xmm0
006b16ce: movss dword ptr [rdi + 0xc], xmm0
006b16d3: mov rbx, qword ptr [rsp + 0x30]
006b16d8: mov rsi, qword ptr [rsp + 0x38]
006b16dd: add rsp, 0x20
006b16e1: pop rdi
006b16e2: ret
006b16e3: cmp dword ptr [rbx + 0x70], 0x34
006b16e7: je 0x1806b1971
006b16ed: cmp dword ptr [rbx + 0x70], 0x11
006b16f1: je 0x1806b1952
006b16f7: cmp dword ptr [rbx + 0x70], 6
006b16fb: je 0x1806b1914
006b1701: cmp dword ptr [rbx + 0x70], 0x15
006b1705: je 0x1806b18f2
006b170b: cmp dword ptr [rbx + 0x70], 0x2e
006b170f: je 0x1806b18f2
006b1715: cmp dword ptr [rbx + 0x70], 4
006b1719: je 0x1806b18dc
006b171f: cmp dword ptr [rbx + 0x70], 0x16
006b1723: je 0x1806b189e
006b1729: cmp dword ptr [rbx + 0x70], 8
006b172d: je 0x1806b1860
006b1733: cmp dword ptr [rbx + 0x70], 0x18
006b1737: je 0x1806b1860
006b173d: cmp dword ptr [rbx + 0x70], 0xa
006b1741: je 0x1806b1822
006b1747: cmp dword ptr [rbx + 0x70], 0x2a
006b174b: je 0x1806b17de
006b1751: cmp dword ptr [rbx + 0x70], 0x13
006b1755: je 0x1806b17c9
006b1757: cmp dword ptr [rbx + 0x70], 0x2b
006b175b: je 0x1806b179b
006b175d: movss xmm0, dword ptr [rbx + 0x20]
006b1762: addss xmm0, dword ptr [rip + 0x35824fe]
006b176a: mov eax, dword ptr [rbx + 0x24]
006b176d: mov dword ptr [rdi + 4], eax
006b1770: mov rax, rdi
006b1773: mov dword ptr [rdi + 8], 0x44480000
006b177a: movss dword ptr [rdi], xmm0
006b177e: movd xmm0, dword ptr [rbx + 0x2c]
006b1783: cvtdq2ps xmm0, xmm0
006b1786: movss dword ptr [rdi + 0xc], xmm0
006b178b: mov rbx, qword ptr [rsp + 0x30]
006b1790: mov rsi, qword ptr [rsp + 0x38]
006b1795: add rsp, 0x20
006b1799: pop rdi
006b179a: ret
006b179b: mov dword ptr [rdi], 0xc4480000
006b17a1: mov rax, rdi
006b17a4: mov dword ptr [rdi + 4], 0xc4160000
006b17ab: mov dword ptr [rdi + 8], 0x44c80000
006b17b2: mov dword ptr [rdi + 0xc], 0x44960000
006b17b9: mov rbx, qword ptr [rsp + 0x30]
006b17be: mov rsi, qword ptr [rsp + 0x38]
006b17c3: add rsp, 0x20
006b17c7: pop rdi
006b17c8: ret
006b17c9: mov eax, dword ptr [rbx + 0x20]
006b17cc: movd xmm0, dword ptr [rbx + 0x28]
006b17d1: mov dword ptr [rdi], eax
006b17d3: mov eax, dword ptr [rbx + 0x24]
006b17d6: mov dword ptr [rdi + 4], eax
006b17d9: jmp 0x1806b19c3
006b17de: movss xmm0, dword ptr [rbx + 0x20]
006b17e3: mov rax, rdi
006b17e6: subss xmm0, dword ptr [rip + 0x35825de]
006b17ee: mov dword ptr [rdi + 8], 0x43700000
006b17f5: mov dword ptr [rdi + 0xc], 0x43700000
006b17fc: movss dword ptr [rdi], xmm0
006b1800: movss xmm0, dword ptr [rbx + 0x24]
006b1805: subss xmm0, dword ptr [rip + 0x35825bf]
006b180d: movss dword ptr [rdi + 4], xmm0
006b1812: mov rbx, qword ptr [rsp + 0x30]
006b1817: mov rsi, qword ptr [rsp + 0x38]
006b181c: add rsp, 0x20
006b1820: pop rdi
006b1821: ret
006b1822: movss xmm0, dword ptr [rbx + 0x20]
006b1827: addss xmm0, dword ptr [rip + 0x3582439]
006b182f: mov eax, dword ptr [rbx + 0x24]
006b1832: mov dword ptr [rdi + 4], eax
006b1835: mov rax, rdi
006b1838: mov dword ptr [rdi + 8], 0x43aa0000
006b183f: movss dword ptr [rdi], xmm0
006b1843: movd xmm0, dword ptr [rbx + 0x2c]
006b1848: cvtdq2ps xmm0, xmm0
006b184b: movss dword ptr [rdi + 0xc], xmm0
006b1850: mov rbx, qword ptr [rsp + 0x30]
006b1855: mov rsi, qword ptr [rsp + 0x38]
006b185a: add rsp, 0x20
006b185e: pop rdi
006b185f: ret
006b1860: movss xmm0, dword ptr [rbx + 0x20]
006b1865: addss xmm0, dword ptr [rip + 0x35823fb]
006b186d: mov eax, dword ptr [rbx + 0x24]
006b1870: mov dword ptr [rdi + 4], eax
006b1873: mov rax, rdi
006b1876: mov dword ptr [rdi + 8], 0x43660000
006b187d: movss dword ptr [rdi], xmm0
006b1881: movd xmm0, dword ptr [rbx + 0x2c]
006b1886: cvtdq2ps xmm0, xmm0
006b1889: movss dword ptr [rdi + 0xc], xmm0
006b188e: mov rbx, qword ptr [rsp + 0x30]
006b1893: mov rsi, qword ptr [rsp + 0x38]
006b1898: add rsp, 0x20
006b189c: pop rdi
006b189d: ret
006b189e: movss xmm0, dword ptr [rbx + 0x20]
006b18a3: addss xmm0, dword ptr [rip + 0x358251d]
006b18ab: mov eax, dword ptr [rbx + 0x24]
006b18ae: mov dword ptr [rdi + 4], eax
006b18b1: mov rax, rdi
006b18b4: mov dword ptr [rdi + 8], 0x41f00000
006b18bb: movss dword ptr [rdi], xmm0
006b18bf: movd xmm0, dword ptr [rbx + 0x2c]
006b18c4: cvtdq2ps xmm0, xmm0
006b18c7: movss dword ptr [rdi + 0xc], xmm0
006b18cc: mov rbx, qword ptr [rsp + 0x30]
006b18d1: mov rsi, qword ptr [rsp + 0x38]
006b18d6: add rsp, 0x20
006b18da: pop rdi
006b18db: ret
006b18dc: mov eax, dword ptr [rbx + 0x20]
006b18df: mov dword ptr [rdi], eax
006b18e1: mov eax, dword ptr [rbx + 0x24]
006b18e4: mov dword ptr [rdi + 4], eax
006b18e7: mov eax, dword ptr [rbx + 0x28]
006b18ea: sub eax, 0x19
006b18ed: jmp 0x1806b19bf
006b18f2: mov eax, dword ptr [rbx + 0x24]
006b18f5: movss xmm0, dword ptr [rbx + 0x20]
006b18fa: addss xmm0, dword ptr [rip + 0x35824ba]
006b1902: mov dword ptr [rdi + 4], eax
006b1905: mov eax, dword ptr [rbx + 0x28]
006b1908: sub eax, 0x32
006b190b: movss dword ptr [rdi], xmm0
006b190f: jmp 0x1806b19bf
006b1914: movss xmm0, dword ptr [rbx + 0x20]
006b1919: addss xmm0, dword ptr [rip + 0x35824ab]
006b1921: mov eax, dword ptr [rbx + 0x24]
006b1924: mov dword ptr [rdi + 4], eax
006b1927: mov rax, rdi
006b192a: mov dword ptr [rdi + 8], 0x42200000
006b1931: movss dword ptr [rdi], xmm0
006b1935: movd xmm0, dword ptr [rbx + 0x2c]
006b193a: cvtdq2ps xmm0, xmm0
006b193d: movss dword ptr [rdi + 0xc], xmm0
006b1942: mov rbx, qword ptr [rsp + 0x30]
006b1947: mov rsi, qword ptr [rsp + 0x38]
006b194c: add rsp, 0x20
006b1950: pop rdi
006b1951: ret
006b1952: mov eax, dword ptr [rbx + 0x24]
006b1955: movss xmm0, dword ptr [rbx + 0x20]
006b195a: addss xmm0, dword ptr [rip + 0x358245a]
006b1962: mov dword ptr [rdi + 4], eax
006b1965: mov eax, dword ptr [rbx + 0x28]
006b1968: sub eax, 0x23
006b196b: movss dword ptr [rdi], xmm0
006b196f: jmp 0x1806b19bf
006b1971: mov eax, dword ptr [rbx + 0x24]
006b1974: movd xmm0, dword ptr [rbx + 0x2c]
006b1979: mov dword ptr [rdi + 4], eax
006b197c: mov eax, dword ptr [rbx + 0x20]
006b197f: cvtdq2ps xmm0, xmm0
006b1982: mov dword ptr [rdi + 8], eax
006b1985: mov rax, rdi
006b1988: mov dword ptr [rdi], 0
006b198e: movss dword ptr [rdi + 0xc], xmm0
006b1993: mov rbx, qword ptr [rsp + 0x30]
006b1998: mov rsi, qword ptr [rsp + 0x38]
006b199d: add rsp, 0x20
006b19a1: pop rdi
006b19a2: ret
006b19a3: mov eax, dword ptr [rbx + 0x24]
006b19a6: xorps xmm0, xmm0
006b19a9: movss xmm1, dword ptr [rbx + 0x20]
006b19ae: mov dword ptr [rdi + 4], eax
006b19b1: addss xmm1, xmm0
006b19b5: mov eax, dword ptr [rbx + 0x28]
006b19b8: sub eax, 0x14
006b19bb: movss dword ptr [rdi], xmm1
006b19bf: movd xmm0, eax
006b19c3: movd xmm1, dword ptr [rbx + 0x2c]
006b19c8: cvtdq2ps xmm1, xmm1
006b19cb: cvtdq2ps xmm0, xmm0
006b19ce: movss dword ptr [rdi + 0xc], xmm1
006b19d3: movss dword ptr [rdi + 8], xmm0
006b19d8: mov rbx, qword ptr [rsp + 0x30]
006b19dd: mov rax, rdi
006b19e0: mov rsi, qword ptr [rsp + 0x38]
006b19e5: add rsp, 0x20
006b19e9: pop rdi
006b19ea: ret
006b19eb: call 0x1804fee20
006b19f0: int3
006b19f1: int3
006b19f2: int3
006b19f3: int3
006b19f4: int3
006b19f5: int3
006b19f6: int3
006b19f7: int3
006b19f8: int3
006b19f9: int3
006b19fa: int3
006b19fb: int3
006b19fc: int3
006b19fd: int3
006b19fe: int3
006b19ff: int3
