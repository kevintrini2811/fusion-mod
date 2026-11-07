006e9240: push rbx
006e9242: sub rsp, 0x40
006e9246: cmp byte ptr [rip + 0x48025da], 0
006e924d: mov rbx, rcx
006e9250: jne 0x1806e9289
006e9252: lea rcx, [rip + 0x45962bf]
006e9259: call 0x1804feb70
006e925e: lea rcx, [rip + 0x458bf03]
006e9265: call 0x1804feb70
006e926a: lea rcx, [rip + 0x4544207]
006e9271: call 0x1804feb70
006e9276: lea rcx, [rip + 0x45aeaab]
006e927d: call 0x1804feb70
006e9282: mov byte ptr [rip + 0x480259e], 1
006e9289: movss xmm0, dword ptr [rip + 0x354af27]
006e9291: xor r9d, r9d
006e9294: mov rdx, qword ptr [rip + 0x45aea8d]
006e929b: mov r8d, 3
006e92a1: mov qword ptr [rsp + 0x50], rsi
006e92a6: mov rcx, rbx
006e92a9: mov qword ptr [rsp + 0x28], 0
006e92b2: movss dword ptr [rsp + 0x20], xmm0
006e92b8: mov qword ptr [rsp + 0x58], rdi
006e92bd: call 0x1806f55d0
006e92c2: mov rax, qword ptr [rbx + 0x18]
006e92c6: test rax, rax
006e92c9: je 0x1806e9440
006e92cf: mov ecx, dword ptr [rbx + 0x17c]
006e92d5: xor edx, edx
006e92d7: mov rdi, qword ptr [rax + 0x78]
006e92db: call 0x1806be7d0
006e92e0: test rdi, rdi
006e92e3: je 0x1806e9440
006e92e9: mov r8, qword ptr [rip + 0x4596228]
006e92f0: mov edx, eax
006e92f2: mov rcx, rdi
006e92f5: call 0x182448850
006e92fa: mov rdi, rax
006e92fd: test rax, rax
006e9300: je 0x1806e9430
006e9306: mov dword ptr [rax + 0x128], 2
006e9310: mov rax, qword ptr [rbx + 0x10]
006e9314: test rax, rax
006e9317: je 0x1806e9440
006e931d: mov r8, qword ptr [rax + 0x58]
006e9321: test r8, r8
006e9324: je 0x1806e9440
006e932a: mov rdx, qword ptr [rip + 0x458be37]
006e9331: mov ecx, 6
006e9336: mov r9d, 0x44
006e933c: call 0x180002040
006e9341: mov rcx, qword ptr [rip + 0x4544130]
006e9348: mov rsi, qword ptr [rdi + 0x140]
006e934f: cmp dword ptr [rcx + 0xe4], 0
006e9356: jne 0x1806e935d
006e9358: call 0x1804feeb0
006e935d: xor r8d, r8d
006e9360: xor edx, edx
006e9362: mov rcx, rsi
006e9365: call 0x183832900
006e936a: test al, al
006e936c: je 0x1806e938d
006e936e: test rsi, rsi
006e9371: je 0x1806e9440
006e9377: movss xmm1, dword ptr [rip + 0x354ac11]
006e937f: xor r9d, r9d
006e9382: xor r8d, r8d
006e9385: mov rcx, rsi
006e9388: call 0x1807c8740
006e938d: cmp dword ptr [rdi + 0x70], 0x2b
006e9391: jne 0x1806e93fc
006e9393: mov rcx, qword ptr [rbx + 0x18]
006e9397: test rcx, rcx
006e939a: je 0x1806e9440
006e93a0: mov r8d, dword ptr [rbx + 0x34]
006e93a4: mov r9d, 8
006e93aa: mov edx, dword ptr [rbx + 0xd8]
006e93b0: mov qword ptr [rsp + 0x20], 0
006e93b9: call 0x18065f2a0
006e93be: test rax, rax
006e93c1: je 0x1806e93fc
006e93c3: mov rcx, qword ptr [rbx + 0x18]
006e93c7: test rcx, rcx
006e93ca: je 0x1806e9440
006e93cc: mov r8d, dword ptr [rbx + 0x34]
006e93d0: mov r9d, 0x10
006e93d6: mov edx, dword ptr [rbx + 0xd8]
006e93dc: mov qword ptr [rsp + 0x38], 0
006e93e5: mov byte ptr [rsp + 0x30], 0
006e93ea: mov byte ptr [rsp + 0x28], 0
006e93ef: mov dword ptr [rsp + 0x20], 0xffffffff
006e93f7: call 0x180668e90
006e93fc: mov rcx, qword ptr [rbx + 0x10]
006e9400: test rcx, rcx
006e9403: je 0x1806e9440
006e9405: xor edx, edx
006e9407: call 0x1807df5d0
006e940c: test al, al
006e940e: je 0x1806e9430
006e9410: mov rax, qword ptr [rbx + 0x18]
006e9414: test rax, rax
006e9417: je 0x1806e9440
006e9419: mov rcx, qword ptr [rax + 0x110]
006e9420: test rcx, rcx
006e9423: je 0x1806e9440
006e9425: xor r8d, r8d
006e9428: mov rdx, rdi
006e942b: call 0x180683a60
006e9430: mov rdi, qword ptr [rsp + 0x58]
006e9435: mov rsi, qword ptr [rsp + 0x50]
006e943a: add rsp, 0x40
006e943e: pop rbx
006e943f: ret
006e9440: call 0x1804fee20
006e9445: int3
006e9446: int3
006e9447: int3
006e9448: int3
006e9449: int3
006e944a: int3
006e944b: int3
006e944c: int3
006e944d: int3
006e944e: int3
006e944f: int3
