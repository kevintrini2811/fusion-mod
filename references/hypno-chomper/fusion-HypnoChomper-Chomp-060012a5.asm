004b16c0: mov qword ptr [rsp + 8], rbx
004b16c5: push rdi
004b16c6: sub rsp, 0x30
004b16ca: xor r8d, r8d
004b16cd: mov rbx, rdx
004b16d0: mov rdi, rcx
004b16d3: call 0x18049c9d0
004b16d8: test rbx, rbx
004b16db: je 0x1804b1752
004b16dd: cmp byte ptr [rbx + 0x12d], 0
004b16e4: je 0x1804b1747
004b16e6: mov rdx, qword ptr [rdi + 0x38]
004b16ea: test rdx, rdx
004b16ed: je 0x1804b1752
004b16ef: xor r8d, r8d
004b16f2: lea rcx, [rsp + 0x20]
004b16f7: call 0x1826c8510
004b16fc: movsd xmm3, qword ptr [rax]
004b1700: xorps xmm0, xmm0
004b1703: movss xmm2, dword ptr [rax + 4]
004b1708: addss xmm3, xmm0
004b170c: addss xmm2, dword ptr [rip + 0x24ca428]
004b1714: unpcklps xmm3, xmm2
004b1717: xor r9d, r9d
004b171a: movq rcx, xmm3
004b171f: xor r8d, r8d
004b1722: mov edx, 8
004b1727: call 0x1807a1300
004b172c: movss xmm1, dword ptr [rdi + 0x1d8]
004b1734: xor r8d, r8d
004b1737: mulss xmm1, dword ptr [rip + 0x24ca3f1]
004b173f: mov rcx, rdi
004b1742: call 0x180472e10
004b1747: mov rbx, qword ptr [rsp + 0x40]
004b174c: add rsp, 0x30
004b1750: pop rdi
004b1751: ret
004b1752: call 0x180363610
004b1757: int3
004b1758: int3
004b1759: int3
004b175a: int3
004b175b: int3
004b175c: int3
004b175d: int3
004b175e: int3
004b175f: int3
