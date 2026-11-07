006b3380: cmp dword ptr [rcx + 0x70], 0x11
006b3384: jne 0x1806b33a1
006b3386: cmp dword ptr [rcx + 0x88], 5
006b338d: je 0x1806b33bc
006b338f: cmp dword ptr [rcx + 0x88], 6
006b3396: je 0x1806b33bc
006b3398: cmp dword ptr [rcx + 0x88], 7
006b339f: je 0x1806b33bc
006b33a1: cmp byte ptr [rcx + 0x3c], 0
006b33a5: jne 0x1806b33bc
006b33a7: cmp dword ptr [rcx + 0x128], 2
006b33ae: je 0x1806b33bc
006b33b0: cmp byte ptr [rcx + 0x135], 0
006b33b7: jne 0x1806b33bc
006b33b9: xor al, al
006b33bb: ret
006b33bc: mov al, 1
006b33be: ret
006b33bf: int3
