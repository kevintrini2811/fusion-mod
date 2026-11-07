006aede0: mov qword ptr [rsp + 0x18], rsi
006aede5: push rdi
006aede6: sub rsp, 0x70
006aedea: cmp byte ptr [rip + 0x483c847], 0
006aedf1: mov rdi, rcx
006aedf4: movaps xmmword ptr [rsp + 0x50], xmm7
006aedf9: jne 0x1806aee0e
006aedfb: lea rcx, [rip + 0x45d12e6]
006aee02: call 0x1804feb70
006aee07: mov byte ptr [rip + 0x483c82a], 1
006aee0e: mov eax, dword ptr [rdi + 0x34]
006aee11: xor esi, esi
006aee13: mov r9d, dword ptr [rdi + 0x70]
006aee17: mov r8, rdi
006aee1a: mov rdx, qword ptr [rdi + 0x18]
006aee1e: mov rcx, qword ptr [rdi + 0x10]
006aee22: mov qword ptr [rsp + 0x30], rsi
006aee27: mov dword ptr [rsp + 0x28], eax
006aee2b: mov eax, dword ptr [rdi + 0x74]
006aee2e: mov dword ptr [rsp + 0x20], eax
006aee32: call 0x1806b33c0
006aee37: cmp dword ptr [rdi + 0x70], 0x23
006aee3b: movaps xmm7, xmm0
006aee3e: jne 0x1806aee4a
006aee40: cmp byte ptr [rdi + 0x3c], sil
006aee44: jne 0x1806af119
006aee4a: cmp byte ptr [rdi + 0x3c], sil
006aee4e: jne 0x1806af119
006aee54: movaps xmmword ptr [rsp + 0x60], xmm6
006aee59: mov qword ptr [rsp + 0x88], rbx
006aee61: xorps xmm6, xmm6
006aee64: cmp qword ptr [rdi + 0x18], rsi
006aee68: je 0x1806aef8f
006aee6e: cmp byte ptr [rdi + 0x137], sil
006aee75: je 0x1806aef8f
006aee7b: mov r8d, dword ptr [rdi + 0x34]
006aee7f: xor r9d, r9d
006aee82: mov edx, dword ptr [rdi + 0x74]
006aee85: mov rcx, qword ptr [rdi + 0x18]
006aee89: call 0x18065eba0
006aee8e: test rax, rax
006aee91: je 0x1806aeeb7
006aee93: mov rcx, qword ptr [rdi + 0x18]
006aee97: test rcx, rcx
006aee9a: je 0x1806af132
006aeea0: mov r8d, dword ptr [rdi + 0x34]
006aeea4: mov r9d, 6
006aeeaa: mov edx, dword ptr [rdi + 0x74]
006aeead: mov qword ptr [rsp + 0x20], rsi
006aeeb2: call 0x18065f2a0
006aeeb7: cmp dword ptr [rdi + 0x70], 0x13
006aeebb: jne 0x1806aef8f
006aeec1: mov ebx, dword ptr [rdi + 0x13c]
006aeec7: xor ecx, ecx
006aeec9: call 0x18063cd70
006aeece: cmp ebx, eax
006aeed0: je 0x1806aef8f
006aeed6: mov rax, qword ptr [rdi + 0x18]
006aeeda: test rax, rax
006aeedd: je 0x1806af132
006aeee3: mov ecx, dword ptr [rdi + 0x13c]
006aeee9: xor edx, edx
006aeeeb: mov rbx, qword ptr [rax + 0xa0]
006aeef2: call 0x1806be7d0
006aeef7: test rbx, rbx
006aeefa: je 0x1806af132
006aef00: mov r8, qword ptr [rip + 0x45d11e1]
006aef07: mov edx, eax
006aef09: mov rcx, rbx
006aef0c: call 0x182448850
006aef11: test rax, rax
006aef14: je 0x1806aef8f
006aef16: movss xmm2, dword ptr [rdi + 0x20]
006aef1b: movaps xmm3, xmm7
006aef1e: addss xmm3, dword ptr [rdi + 0x24]
006aef23: mov rcx, qword ptr [rax + 0xd0]
006aef2a: addss xmm2, xmm6
006aef2e: subss xmm3, dword ptr [rip + 0x3584d2a]
006aef36: subss xmm2, dword ptr [rip + 0x35850be]
006aef3e: test rcx, rcx
006aef41: je 0x1806af132
006aef47: mov r8, qword ptr [rcx]
006aef4a: lea rdx, [rsp + 0x40]
006aef4f: mov dword ptr [rsp + 0x48], 0x44020000
006aef57: mov dword ptr [rsp + 0x4c], 0x42c80000
006aef5f: movaps xmm0, xmmword ptr [rsp + 0x40]
006aef64: mov rax, qword ptr [r8 + 0x268]
006aef6b: movss xmm0, xmm2
006aef6f: mov r8, qword ptr [r8 + 0x270]
006aef76: shufps xmm0, xmm0, 0xe1
006aef7a: movss xmm0, xmm3
006aef7e: shufps xmm0, xmm0, 0xe1
006aef82: movaps xmmword ptr [rsp + 0x40], xmm0
006aef87: movdqa xmmword ptr [rsp + 0x40], xmm0
006aef8d: call rax
006aef8f: xor r9d, r9d
006aef92: movaps xmm2, xmm7
006aef95: xorps xmm1, xmm1
006aef98: mov rcx, rdi
006aef9b: call 0x1806ae840
006aefa0: cmp dword ptr [rdi + 0x70], 0x23
006aefa4: jne 0x1806aeff8
006aefa6: cmp qword ptr [rdi + 0x18], rsi
006aefaa: je 0x1806aefc1
006aefac: cmp byte ptr [rdi + 0x137], sil
006aefb3: je 0x1806aefc1
006aefb5: cmp qword ptr [rdi + 0x18], rsi
006aefb9: je 0x1806af132
006aefbf: jmp 0x1806aefc8
006aefc1: xor ecx, ecx
006aefc3: call 0x183840220
006aefc8: cmp byte ptr [rip + 0x483c663], sil
006aefcf: jne 0x1806aefe4
006aefd1: lea rcx, [rip + 0x45fbe90]
006aefd8: call 0x1804feb70
006aefdd: mov byte ptr [rip + 0x483c64e], 1
006aefe4: mov rcx, qword ptr [rip + 0x45fbe7d]
006aefeb: cmp dword ptr [rcx + 0xe4], esi
006aeff1: jne 0x1806aeff8
006aeff3: call 0x1804feeb0
006aeff8: cmp qword ptr [rdi + 0x18], rsi
006aeffc: je 0x1806af07b
006aeffe: cmp byte ptr [rdi + 0x137], sil
006af005: je 0x1806af07b
006af007: mov rcx, qword ptr [rdi + 0x10]
006af00b: test rcx, rcx
006af00e: je 0x1806af132
006af014: xor edx, edx
006af016: call 0x1807e3010
006af01b: cmp eax, 0x2b
006af01e: je 0x1806af07b
006af020: mov rcx, qword ptr [rdi + 0x10]
006af024: test rcx, rcx
006af027: je 0x1806af132
006af02d: xor edx, edx
006af02f: call 0x1807e3010
006af034: cmp eax, 0x32
006af037: je 0x1806af07b
006af039: mov rcx, qword ptr [rdi + 0x10]
006af03d: test rcx, rcx
006af040: je 0x1806af132
006af046: xor edx, edx
006af048: call 0x1807df5d0
006af04d: test al, al
006af04f: je 0x1806af07b
006af051: mov rax, qword ptr [rdi + 0x18]
006af055: test rax, rax
006af058: je 0x1806af132
006af05e: mov rcx, qword ptr [rax + 0x110]
006af065: test rcx, rcx
006af068: je 0x1806af132
006af06e: xor r8d, r8d
006af071: mov rdx, rdi
006af074: call 0x18067fd40
006af079: jmp 0x1806af082
006af07b: xor ecx, ecx
006af07d: call 0x18063cd70
006af082: cmp dword ptr [rdi + 0x70], 0x1f
006af086: jne 0x1806af10c
006af08c: movss xmm6, dword ptr [rip + 0x3584d28]
006af094: mov ebx, 0x20
006af099: nop dword ptr [rax]
006af0a0: mov rax, qword ptr [rdi + 0x118]
006af0a7: test rax, rax
006af0aa: je 0x1806af132
006af0b0: cmp esi, dword ptr [rax + 0x18]
006af0b3: jae 0x1806af12c
006af0b5: mov rcx, qword ptr [rax + rbx]
006af0b9: test rcx, rcx
006af0bc: je 0x1806af132
006af0be: cmp dword ptr [rcx + 0x20], 0
006af0c2: je 0x1806af0f6
006af0c4: movss xmm0, dword ptr [rcx + 0x18]
006af0c9: xor edx, edx
006af0cb: movss xmm1, dword ptr [rcx + 0x1c]
006af0d0: addss xmm0, dword ptr [rdi + 0x20]
006af0d5: addss xmm1, dword ptr [rdi + 0x24]
006af0da: subss xmm0, dword ptr [rcx + 0x10]
006af0df: subss xmm1, dword ptr [rcx + 0x14]
006af0e4: unpcklps xmm0, xmm1
006af0e7: movq rcx, xmm0
006af0ec: call 0x1806ab080
006af0f1: comiss xmm0, xmm6
006af0f4: ja 0x1806af10c
006af0f6: inc esi
006af0f8: add rbx, 8
006af0fc: cmp rbx, 0x48
006af100: jl 0x1806af0a0
006af102: xor edx, edx
006af104: mov rcx, rdi
006af107: call 0x1806ae500
006af10c: mov rbx, qword ptr [rsp + 0x88]
006af114: movaps xmm6, xmmword ptr [rsp + 0x60]
006af119: mov rsi, qword ptr [rsp + 0x90]
006af121: movaps xmm7, xmmword ptr [rsp + 0x50]
006af126: add rsp, 0x70
006af12a: pop rdi
006af12b: ret
006af12c: call 0x1804fee10
006af131: int3
006af132: call 0x1804fee20
006af137: int3
006af138: int3
006af139: int3
006af13a: int3
006af13b: int3
006af13c: int3
006af13d: int3
006af13e: int3
006af13f: int3
