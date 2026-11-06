006f3600: cmp byte ptr [rcx + 0x144], 0
006f3607: jne 0x6f361d
006f3609: cmp dword ptr [rcx + 0x74], 1
006f360d: je 0x6f361d
006f360f: cmp dword ptr [rcx + 0x74], 2
006f3613: je 0x6f361d
006f3615: cmp dword ptr [rcx + 0x74], 3
006f3619: sete al
006f361c: ret
006f361d: mov al, 1
006f361f: ret
