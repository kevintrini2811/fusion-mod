004b1620: sub rsp, 0x28
004b1624: test rdx, rdx
004b1627: je 0x1804b1679
004b1629: cmp byte ptr [rdx + 0x135], 0
004b1630: jne 0x1804b1661
004b1632: mov edx, dword ptr [rdx + 0x68]
004b1635: cmp edx, 7
004b1638: jle 0x1804b1668
004b163a: lea eax, [rdx - 0xa]
004b163d: cmp eax, 0xe
004b1640: ja 0x1804b165c
004b1642: lea eax, [rdx - 0xa]
004b1645: movsxd rcx, eax
004b1648: lea r8, [rip - 0x4b164f]
004b164f: mov eax, dword ptr [r8 + rcx*4 + 0x4b1680]
004b1657: add rax, r8
004b165a: jmp rax
004b165c: cmp edx, 0x2c
004b165f: jne 0x1804b1672
004b1661: xor al, al
004b1663: add rsp, 0x28
004b1667: ret
004b1668: cmp edx, 1
004b166b: je 0x1804b1661
004b166d: cmp edx, 7
004b1670: je 0x1804b1661
004b1672: mov al, 1
004b1674: add rsp, 0x28
004b1678: ret
004b1679: call 0x180363610
004b167e: int3
004b167f: nop
