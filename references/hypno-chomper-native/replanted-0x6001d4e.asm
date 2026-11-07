006e9560: push rbx
006e9562: sub rsp, 0x20
006e9566: cmp dword ptr [rcx + 0x70], 0x22
006e956a: mov rbx, rcx
006e956d: je 0x1806e95fe
006e9573: cmp dword ptr [rcx + 0x70], 0x24
006e9577: je 0x1806e95fe
006e957d: cmp dword ptr [rcx + 0x70], 0xc
006e9581: je 0x1806e95fe
006e9583: xor edx, edx
006e9585: call 0x1806f1f20
006e958a: cmp eax, -1
006e958d: jne 0x1806e95fe
006e958f: cmp byte ptr [rbx + 0x144], 0
006e9596: jne 0x1806e95fe
006e9598: cmp dword ptr [rbx + 0x74], 1
006e959c: je 0x1806e95fe
006e959e: cmp dword ptr [rbx + 0x74], 2
006e95a2: je 0x1806e95fe
006e95a4: cmp dword ptr [rbx + 0x74], 3
006e95a8: je 0x1806e95fe
006e95aa: cmp dword ptr [rbx + 0x74], 0x20
006e95ae: je 0x1806e95fe
006e95b0: cmp dword ptr [rbx + 0x74], 0x21
006e95b4: je 0x1806e95fe
006e95b6: cmp dword ptr [rbx + 0x74], 0x22
006e95ba: je 0x1806e95fe
006e95bc: cmp dword ptr [rbx + 0x74], 0x23
006e95c0: je 0x1806e95fe
006e95c2: cmp dword ptr [rbx + 0x74], 0xe
006e95c6: je 0x1806e95fe
006e95c8: cmp dword ptr [rbx + 0x74], 0x32
006e95cc: je 0x1806e95fe
006e95ce: cmp byte ptr [rbx + 0x110], 0
006e95d5: jne 0x1806e95fe
006e95d7: cmp dword ptr [rbx + 0x70], 0x19
006e95db: jne 0x1806e95f6
006e95dd: cmp dword ptr [rbx + 0x74], 0x57
006e95e1: je 0x1806e95f6
006e95e3: cmp dword ptr [rbx + 0x74], 0x58
006e95e7: je 0x1806e95f6
006e95e9: cmp dword ptr [rbx + 0x74], 0x59
006e95ed: sete al
006e95f0: add rsp, 0x20
006e95f4: pop rbx
006e95f5: ret
006e95f6: mov al, 1
006e95f8: add rsp, 0x20
006e95fc: pop rbx
006e95fd: ret
006e95fe: xor al, al
006e9600: add rsp, 0x20
006e9604: pop rbx
006e9605: ret
006e9606: int3
006e9607: int3
006e9608: int3
006e9609: int3
006e960a: int3
006e960b: int3
006e960c: int3
006e960d: int3
006e960e: int3
006e960f: int3
