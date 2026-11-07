006acd40: mov qword ptr [rsp + 0x10], rbx
006acd45: push rdi
006acd46: sub rsp, 0x30
006acd4a: cmp byte ptr [rip + 0x483e8ec], 0
006acd51: mov rdi, rcx
006acd54: jne 0x1806acd8d
006acd56: lea rcx, [rip + 0x457e263]
006acd5d: call 0x1804feb70
006acd62: lea rcx, [rip + 0x45d26d7]
006acd69: call 0x1804feb70
006acd6e: lea rcx, [rip + 0x45c65ab]
006acd75: call 0x1804feb70
006acd7a: lea rcx, [rip + 0x45806f7]
006acd81: call 0x1804feb70
006acd86: mov byte ptr [rip + 0x483e8b0], 1
006acd8d: cmp qword ptr [rdi + 0x18], 0
006acd92: je 0x1806acdc4
006acd94: cmp byte ptr [rdi + 0x137], 0
006acd9b: je 0x1806acdc4
006acd9d: cmp dword ptr [rdi + 0x70], 0x13
006acda1: jne 0x1806acdc4
006acda3: mov edx, dword ptr [rdi + 0x120]
006acda9: xor r8d, r8d
006acdac: mov rcx, qword ptr [rdi + 0x18]
006acdb0: call 0x180677050
006acdb5: test rax, rax
006acdb8: je 0x1806acdc4
006acdba: xor edx, edx
006acdbc: mov rcx, rax
006acdbf: call 0x1806ec320
006acdc4: mov ebx, dword ptr [rdi + 0x13c]
006acdca: xor ecx, ecx
006acdcc: mov qword ptr [rsp + 0x40], rsi
006acdd1: call 0x18063cd70
006acdd6: cmp ebx, eax
006acdd8: je 0x1806ace2c
006acdda: mov rcx, qword ptr [rip + 0x4580697]
006acde1: mov rbx, qword ptr [rdi + 0x10]
006acde5: cmp dword ptr [rcx + 0xe4], 0
006acdec: jne 0x1806acdf3
006acdee: call 0x1804feeb0
006acdf3: xor r8d, r8d
006acdf6: xor edx, edx
006acdf8: mov rcx, rbx
006acdfb: call 0x183832900
006ace00: test al, al
006ace02: je 0x1806ace1f
006ace04: mov rcx, qword ptr [rdi + 0x10]
006ace08: test rcx, rcx
006ace0b: je 0x1806ad0ad
006ace11: mov edx, dword ptr [rdi + 0x13c]
006ace17: xor r8d, r8d
006ace1a: call 0x1807e14a0
006ace1f: xor ecx, ecx
006ace21: call 0x18063cd70
006ace26: mov dword ptr [rdi + 0x13c], eax
006ace2c: cmp dword ptr [rdi + 0x70], 0x2f
006ace30: jne 0x1806aceda
006ace36: cmp qword ptr [rdi + 0x18], 0
006ace3b: je 0x1806aceda
006ace41: xor esi, esi
006ace43: mov rax, qword ptr [rdi + 0x18]
006ace47: test rax, rax
006ace4a: je 0x1806ad0ad
006ace50: mov rcx, qword ptr [rax + 0xb8]
006ace57: test rcx, rcx
006ace5a: je 0x1806ad0ad
006ace60: mov r8, qword ptr [rip + 0x45c64b9]
006ace67: mov edx, esi
006ace69: call 0x1809c0bc0
006ace6e: test rax, rax
006ace71: je 0x1806acecf
006ace73: cmp dword ptr [rax + 0x7c], 8
006ace77: jne 0x1806acecf
006ace79: mov ebx, dword ptr [rax + 0x8c]
006ace7f: mov rax, qword ptr [rdi + 0x18]
006ace83: test rax, rax
006ace86: je 0x1806ad0ad
006ace8c: mov rcx, qword ptr [rax + 0x78]
006ace90: test rcx, rcx
006ace93: je 0x1806ad0ad
006ace99: mov r8, qword ptr [rip + 0x45d25a0]
006acea0: mov rdx, rdi
006acea3: call 0x1824484c0
006acea8: xor edx, edx
006aceaa: mov ecx, eax
006aceac: call 0x1806be7d0
006aceb1: cmp ebx, eax
006aceb3: jne 0x1806acecf
006aceb5: mov rcx, qword ptr [rdi + 0x18]
006aceb9: test rcx, rcx
006acebc: je 0x1806ad0ad
006acec2: xor r9d, r9d
006acec5: mov r8d, esi
006acec8: mov dl, 1
006aceca: call 0x180657e60
006acecf: inc esi
006aced1: cmp esi, 2
006aced4: jl 0x1806ace43
006aceda: xor edx, edx
006acedc: mov byte ptr [rdi + 0x135], 1
006acee3: mov rcx, rdi
006acee6: call 0x1806b4e30
006aceeb: cmp dword ptr [rdi + 0x70], 0x23
006aceef: je 0x1806acf27
006acef1: cmp qword ptr [rdi + 0x18], 0
006acef6: je 0x1806ad006
006acefc: cmp byte ptr [rdi + 0x137], 0
006acf03: je 0x1806acf27
006acf05: mov r8d, dword ptr [rdi + 0x34]
006acf09: xor r9d, r9d
006acf0c: mov edx, dword ptr [rdi + 0x74]
006acf0f: mov rcx, qword ptr [rdi + 0x18]
006acf13: call 0x18065e170
006acf18: test rax, rax
006acf1b: je 0x1806acf27
006acf1d: xor edx, edx
006acf1f: mov rcx, rax
006acf22: call 0x1806c2390
006acf27: cmp qword ptr [rdi + 0x18], 0
006acf2c: je 0x1806ad006
006acf32: cmp byte ptr [rdi + 0x137], 0
006acf39: je 0x1806ad006
006acf3f: mov r8d, dword ptr [rdi + 0x34]
006acf43: mov r9d, 2
006acf49: mov edx, dword ptr [rdi + 0x74]
006acf4c: mov rcx, qword ptr [rdi + 0x18]
006acf50: mov qword ptr [rsp + 0x20], 0
006acf59: call 0x18065f2a0
006acf5e: mov rcx, qword ptr [rdi + 0x18]
006acf62: mov rbx, rax
006acf65: test rcx, rcx
006acf68: je 0x1806ad0ad
006acf6e: mov r8d, dword ptr [rdi + 0x34]
006acf72: xor r9d, r9d
006acf75: mov edx, dword ptr [rdi + 0x74]
006acf78: call 0x18065de10
006acf7d: test rax, rax
006acf80: je 0x1806ad006
006acf86: cmp rbx, rax
006acf89: jne 0x1806ad006
006acf8b: mov rcx, qword ptr [rip + 0x45804e6]
006acf92: mov rbx, qword ptr [rdi + 0x140]
006acf99: cmp dword ptr [rcx + 0xe4], 0
006acfa0: jne 0x1806acfa7
006acfa2: call 0x1804feeb0
006acfa7: xor r8d, r8d
006acfaa: xor edx, edx
006acfac: mov rcx, rbx
006acfaf: call 0x183832900
006acfb4: test al, al
006acfb6: je 0x1806ad006
006acfb8: mov rcx, qword ptr [rip + 0x457e001]
006acfbf: mov rbx, qword ptr [rdi + 0x140]
006acfc6: cmp dword ptr [rcx + 0xe4], 0
006acfcd: jne 0x1806acfd4
006acfcf: call 0x1804feeb0
006acfd4: movss xmm1, dword ptr [rip + 0x3586ddc]
006acfdc: xor r8d, r8d
006acfdf: movss xmm0, dword ptr [rip + 0x3586dc5]
006acfe7: call 0x1806cbd40
006acfec: test rbx, rbx
006acfef: je 0x1806ad0ad
006acff5: xor r9d, r9d
006acff8: xor r8d, r8d
006acffb: movaps xmm1, xmm0
006acffe: mov rcx, rbx
006ad001: call 0x1807c8740
006ad006: mov rcx, qword ptr [rip + 0x458046b]
006ad00d: mov rbx, qword ptr [rdi + 0x10]
006ad011: cmp dword ptr [rcx + 0xe4], 0
006ad018: jne 0x1806ad01f
006ad01a: call 0x1804feeb0
006ad01f: xor r8d, r8d
006ad022: xor edx, edx
006ad024: mov rcx, rbx
006ad027: call 0x183832900
006ad02c: test al, al
006ad02e: je 0x1806ad056
006ad030: mov rcx, qword ptr [rdi + 0x10]
006ad034: test rcx, rcx
006ad037: je 0x1806ad0ad
006ad039: mov edx, dword ptr [rdi + 0xd8]
006ad03f: xor r8d, r8d
006ad042: call 0x1807e1000
006ad047: test rax, rax
006ad04a: je 0x1806ad056
006ad04c: xor edx, edx
006ad04e: mov rcx, rax
006ad051: call 0x1806d7cd0
006ad056: mov rcx, qword ptr [rip + 0x458041b]
006ad05d: mov rbx, qword ptr [rdi + 0x140]
006ad064: cmp dword ptr [rcx + 0xe4], 0
006ad06b: jne 0x1806ad072
006ad06d: call 0x1804feeb0
006ad072: xor r8d, r8d
006ad075: xor edx, edx
006ad077: mov rcx, rbx
006ad07a: call 0x183832900
006ad07f: test al, al
006ad081: je 0x1806ad096
006ad083: mov rcx, qword ptr [rdi + 0x140]
006ad08a: test rcx, rcx
006ad08d: je 0x1806ad0ad
006ad08f: xor edx, edx
006ad091: call 0x1807c9970
006ad096: mov rsi, qword ptr [rsp + 0x40]
006ad09b: mov rbx, qword ptr [rsp + 0x48]
006ad0a0: mov dword ptr [rdi + 0x70], 0xffffffff
006ad0a7: add rsp, 0x30
006ad0ab: pop rdi
006ad0ac: ret
006ad0ad: call 0x1804fee20
006ad0b2: int3
006ad0b3: int3
006ad0b4: int3
006ad0b5: int3
006ad0b6: int3
006ad0b7: int3
006ad0b8: int3
006ad0b9: int3
006ad0ba: int3
006ad0bb: int3
006ad0bc: int3
006ad0bd: int3
006ad0be: int3
006ad0bf: int3
