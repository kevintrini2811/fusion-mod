006ebf90: mov qword ptr [rsp + 0x18], rbx
006ebf95: push rdi
006ebf96: sub rsp, 0x20
006ebf9a: cmp byte ptr [rip + 0x47ff800], 0
006ebfa1: mov rdi, rcx
006ebfa4: jne 0x1806ebfc5
006ebfa6: lea rcx, [rip + 0x4550dbb]
006ebfad: call 0x1804feb70
006ebfb2: lea rcx, [rip + 0x45414bf]
006ebfb9: call 0x1804feb70
006ebfbe: mov byte ptr [rip + 0x47ff7dc], 1
006ebfc5: xor edx, edx
006ebfc7: mov rcx, rdi
006ebfca: call 0x1806f7dd0
006ebfcf: mov ecx, dword ptr [rdi + 0x164]
006ebfd5: xor edx, edx
006ebfd7: call 0x18060f880
006ebfdc: mov rcx, qword ptr [rip + 0x4541495]
006ebfe3: mov rbx, qword ptr [rdi + 0x10]
006ebfe7: cmp dword ptr [rcx + 0xe4], 0
006ebfee: jne 0x1806ebff5
006ebff0: call 0x1804feeb0
006ebff5: mov qword ptr [rsp + 0x30], rbp
006ebffa: xor r8d, r8d
006ebffd: xor edx, edx
006ebfff: mov qword ptr [rsp + 0x38], rsi
006ec004: mov rcx, rbx
006ec007: call 0x183832900
006ec00c: test al, al
006ec00e: je 0x1806ec02b
006ec010: mov rcx, qword ptr [rdi + 0x10]
006ec014: test rcx, rcx
006ec017: je 0x1806ec30d
006ec01d: mov edx, dword ptr [rdi + 0x1a8]
006ec023: xor r8d, r8d
006ec026: call 0x1807e14a0
006ec02b: xor edx, edx
006ec02d: mov byte ptr [rdi + 0x144], 1
006ec034: mov rcx, rdi
006ec037: call 0x1806f9de0
006ec03c: cmp dword ptr [rdi + 0x70], 0xd
006ec040: jne 0x1806ec129
006ec046: xor edx, edx
006ec048: mov rcx, rdi
006ec04b: call 0x1806f1f20
006ec050: cmp eax, -1
006ec053: je 0x1806ec129
006ec059: cmp dword ptr [rdi + 0xc4], -2
006ec060: je 0x1806ec129
006ec066: cmp dword ptr [rdi + 0xc4], -3
006ec06d: je 0x1806ec129
006ec073: mov ebx, dword ptr [rdi + 0x148]
006ec079: xor ecx, ecx
006ec07b: call 0x18063cd70
006ec080: cmp ebx, eax
006ec082: je 0x1806ec0a4
006ec084: mov rcx, qword ptr [rdi + 0x18]
006ec088: test rcx, rcx
006ec08b: je 0x1806ec30d
006ec091: mov edx, dword ptr [rdi + 0x148]
006ec097: xor r8d, r8d
006ec09a: call 0x180676f10
006ec09f: mov rbp, rax
006ec0a2: jmp 0x1806ec0a7
006ec0a4: mov rbp, rdi
006ec0a7: test rbp, rbp
006ec0aa: je 0x1806ec30d
006ec0b0: cmp byte ptr [rbp + 0x144], 0
006ec0b7: jne 0x1806ec0c3
006ec0b9: xor edx, edx
006ec0bb: mov rcx, rbp
006ec0be: call 0x1806ebf90
006ec0c3: xor esi, esi
006ec0c5: mov ebx, 0x20
006ec0ca: nop word ptr [rax + rax]
006ec0d0: mov rdx, qword ptr [rbp + 0x150]
006ec0d7: test rdx, rdx
006ec0da: je 0x1806ec30d
006ec0e0: cmp esi, dword ptr [rdx + 0x18]
006ec0e3: jae 0x1806ec313
006ec0e9: mov rcx, qword ptr [rdi + 0x18]
006ec0ed: test rcx, rcx
006ec0f0: je 0x1806ec30d
006ec0f6: mov edx, dword ptr [rbx + rdx]
006ec0f9: xor r8d, r8d
006ec0fc: call 0x180676f10
006ec101: test rax, rax
006ec104: je 0x1806ec30d
006ec10a: cmp byte ptr [rax + 0x144], 0
006ec111: jne 0x1806ec11d
006ec113: xor edx, edx
006ec115: mov rcx, rax
006ec118: call 0x1806ebf90
006ec11d: inc esi
006ec11f: add rbx, 4
006ec123: cmp rbx, 0x2c
006ec127: jl 0x1806ec0d0
006ec129: cmp dword ptr [rdi + 0x70], 0x14
006ec12d: jne 0x1806ec1e8
006ec133: cmp byte ptr [rip + 0x47ff6b7], 0
006ec13a: jne 0x1806ec14f
006ec13c: lea rcx, [rip + 0x45933d5]
006ec143: call 0x1804feb70
006ec148: mov byte ptr [rip + 0x47ff6a2], 1
006ec14f: xor edx, edx
006ec151: mov rcx, rdi
006ec154: call 0x1806e8d10
006ec159: cmp qword ptr [rdi + 0x18], 0
006ec15e: je 0x1806ec1e8
006ec164: mov rax, qword ptr [rdi + 0x18]
006ec168: xor edx, edx
006ec16a: mov ecx, dword ptr [rdi + 0x17c]
006ec170: mov rbx, qword ptr [rax + 0x78]
006ec174: call 0x1806be7d0
006ec179: test rbx, rbx
006ec17c: je 0x1806ec30d
006ec182: mov r8, qword ptr [rip + 0x459338f]
006ec189: mov edx, eax
006ec18b: mov rcx, rbx
006ec18e: call 0x182448850
006ec193: test rax, rax
006ec196: je 0x1806ec1b5
006ec198: mov rdx, qword ptr [rdi + 0x18]
006ec19c: test rdx, rdx
006ec19f: je 0x1806ec30d
006ec1a5: inc dword ptr [rdx + 0x2f8]
006ec1ab: mov rcx, rax
006ec1ae: xor edx, edx
006ec1b0: call 0x1806acd40
006ec1b5: mov rcx, qword ptr [rdi + 0x18]
006ec1b9: test rcx, rcx
006ec1bc: je 0x1806ec30d
006ec1c2: mov edx, dword ptr [rdi + 0x148]
006ec1c8: xor r8d, r8d
006ec1cb: call 0x180677050
006ec1d0: test rax, rax
006ec1d3: je 0x1806ec1e8
006ec1d5: cmp byte ptr [rax + 0x144], 0
006ec1dc: jne 0x1806ec1e8
006ec1de: xor edx, edx
006ec1e0: mov rcx, rax
006ec1e3: call 0x1806ebf90
006ec1e8: cmp dword ptr [rdi + 0x70], 0x19
006ec1ec: jne 0x1806ec1f8
006ec1ee: xor edx, edx
006ec1f0: mov rcx, rdi
006ec1f3: call 0x1806e73c0
006ec1f8: cmp dword ptr [rdi + 0x70], 0x24
006ec1fc: jne 0x1806ec22a
006ec1fe: mov rax, qword ptr [rdi + 0x18]
006ec202: test rax, rax
006ec205: je 0x1806ec30d
006ec20b: mov rcx, qword ptr [rax + 0xb0]
006ec212: test rcx, rcx
006ec215: je 0x1806ec30d
006ec21b: mov r8, qword ptr [rip + 0x4550b46]
006ec222: mov rdx, rdi
006ec225: call 0x1818dc5a0
006ec22a: mov rcx, qword ptr [rip + 0x4541247]
006ec231: mov rbx, qword ptr [rdi + 0x1d0]
006ec238: cmp dword ptr [rcx + 0xe4], 0
006ec23f: jne 0x1806ec246
006ec241: call 0x1804feeb0
006ec246: xor r8d, r8d
006ec249: xor edx, edx
006ec24b: mov rcx, rbx
006ec24e: call 0x183832900
006ec253: test al, al
006ec255: je 0x1806ec287
006ec257: mov rcx, qword ptr [rdi + 0x1d0]
006ec25e: test rcx, rcx
006ec261: je 0x1806ec30d
006ec267: xor edx, edx
006ec269: call 0x1807c9970
006ec26e: lea rcx, [rdi + 0x1d0]
006ec275: mov qword ptr [rdi + 0x1d0], 0
006ec280: xor edx, edx
006ec282: call 0x1804fdea0
006ec287: cmp qword ptr [rdi + 0x1a0], 0
006ec28f: je 0x1806ec2b8
006ec291: mov rcx, qword ptr [rdi + 0x1a0]
006ec298: xor edx, edx
006ec29a: call 0x1806ca910
006ec29f: lea rcx, [rdi + 0x1a0]
006ec2a6: mov qword ptr [rdi + 0x1a0], 0
006ec2b1: xor edx, edx
006ec2b3: call 0x1804fdea0
006ec2b8: mov rcx, qword ptr [rip + 0x45411b9]
006ec2bf: mov rbx, qword ptr [rdi + 0x1c8]
006ec2c6: cmp dword ptr [rcx + 0xe4], 0
006ec2cd: jne 0x1806ec2d4
006ec2cf: call 0x1804feeb0
006ec2d4: xor r8d, r8d
006ec2d7: xor edx, edx
006ec2d9: mov rcx, rbx
006ec2dc: call 0x183832900
006ec2e1: test al, al
006ec2e3: je 0x1806ec2f8
006ec2e5: mov rcx, qword ptr [rdi + 0x1c8]
006ec2ec: test rcx, rcx
006ec2ef: je 0x1806ec30d
006ec2f1: xor edx, edx
006ec2f3: call 0x1807b3d70
006ec2f8: mov rsi, qword ptr [rsp + 0x38]
006ec2fd: mov rbp, qword ptr [rsp + 0x30]
006ec302: mov rbx, qword ptr [rsp + 0x40]
006ec307: add rsp, 0x20
006ec30b: pop rdi
006ec30c: ret
006ec30d: call 0x1804fee20
006ec312: int3
006ec313: call 0x1804fee10
006ec318: int3
006ec319: int3
006ec31a: int3
006ec31b: int3
006ec31c: int3
006ec31d: int3
006ec31e: int3
006ec31f: int3
