0049afc0: push rbx
0049afc2: push rdi
0049afc3: push r15
0049afc5: sub rsp, 0x70
0049afc9: cmp byte ptr [rip + 0x30fe241], 0
0049afd0: mov r15d, edx
0049afd3: mov rdi, rcx
0049afd6: jne 0x18049b003
0049afd8: lea rcx, [rip + 0x2f174f1]
0049afdf: call 0x1803633b0
0049afe4: lea rcx, [rip + 0x2efa9bd]
0049afeb: call 0x1803633b0
0049aff0: lea rcx, [rip + 0x2f40ba1]
0049aff7: call 0x1803633b0
0049affc: mov byte ptr [rip + 0x30fe20e], 1
0049b003: mov rax, qword ptr [rdi + 0x258]
0049b00a: xor ebx, ebx
0049b00c: mov qword ptr [rsp + 0x90], rbp
0049b014: xor ecx, ecx
0049b016: mov qword ptr [rsp + 0x98], rsi
0049b01e: mov qword ptr [rsp + 0xa0], r14
0049b026: movaps xmmword ptr [rsp + 0x60], xmm6
0049b02b: movaps xmmword ptr [rsp + 0x50], xmm7
0049b030: test rax, rax
0049b033: je 0x18049b239
0049b039: mov esi, 0x20
0049b03e: nop 
0049b040: cmp ecx, dword ptr [rax + 0x18]
0049b043: jge 0x18049b235
0049b049: mov rax, qword ptr [rdi + 0x258]
0049b050: test rax, rax
0049b053: je 0x18049b239
0049b059: cmp ebx, dword ptr [rax + 0x18]
0049b05c: jae 0x18049b24f
0049b062: mov rcx, qword ptr [rip + 0x2f40b2f]
0049b069: mov rbp, qword ptr [rsi + rax]
0049b06d: cmp dword ptr [rcx + 0xe0], 0
0049b074: jne 0x18049b07b
0049b076: call 0x180318580
0049b07b: xor r8d, r8d
0049b07e: xor edx, edx
0049b080: mov rcx, rbp
0049b083: call 0x1826b9a40
0049b088: test al, al
0049b08a: jne 0x18049b0a6
0049b08c: mov rax, qword ptr [rdi + 0x258]
0049b093: inc ebx
0049b095: add rsi, 8
0049b099: mov ecx, ebx
0049b09b: test rax, rax
0049b09e: je 0x18049b239
0049b0a4: jmp 0x18049b040
0049b0a6: mov rdx, qword ptr [rdi + 0x98]
0049b0ad: mov rsi, qword ptr [rdi + 0x250]
0049b0b4: test rdx, rdx
0049b0b7: je 0x18049b239
0049b0bd: xor r8d, r8d
0049b0c0: lea rcx, [rsp + 0x30]
0049b0c5: call 0x1826c8510
0049b0ca: cmp byte ptr [rip + 0x30fdd74], 0
0049b0d1: movsd xmm7, qword ptr [rax]
0049b0d5: mov r14d, dword ptr [rax + 8]
0049b0d9: jne 0x18049b0ee
0049b0db: lea rcx, [rip + 0x2ed7aee]
0049b0e2: call 0x1803633b0
0049b0e7: mov byte ptr [rip + 0x30fdd57], 1
0049b0ee: mov rax, qword ptr [rip + 0x2ed7adb]
0049b0f5: mov rcx, qword ptr [rax + 0xb8]
0049b0fc: movups xmm6, xmmword ptr [rcx]
0049b0ff: mov rcx, qword ptr [rdi + 0x30]
0049b103: test rcx, rcx
0049b106: je 0x18049b239
0049b10c: xor edx, edx
0049b10e: call 0x1826b1710
0049b113: mov rcx, qword ptr [rip + 0x2f40a7e]
0049b11a: mov rbp, rax
0049b11d: cmp dword ptr [rcx + 0xe0], 0
0049b124: jne 0x18049b12b
0049b126: call 0x180318580
0049b12b: mov rdx, qword ptr [rip + 0x2efa876]
0049b132: lea r8, [rsp + 0x40]
0049b137: mov qword ptr [rsp + 0x20], rdx
0049b13c: mov r9, rbp
0049b13f: lea rdx, [rsp + 0x30]
0049b144: movdqa xmmword ptr [rsp + 0x40], xmm6
0049b14a: mov rcx, rsi
0049b14d: movsd qword ptr [rsp + 0x30], xmm7
0049b153: mov dword ptr [rsp + 0x38], r14d
0049b158: call 0x180b99cc0
0049b15d: test rax, rax
0049b160: je 0x18049b239
0049b166: mov rdx, qword ptr [rip + 0x2f17363]
0049b16d: mov rcx, rax
0049b170: call 0x180b321e0
0049b175: mov r14, qword ptr [rdi + 0x258]
0049b17c: mov rsi, rax
0049b17f: test r14, r14
0049b182: je 0x18049b239
0049b188: test rax, rax
0049b18b: je 0x18049b1a5
0049b18d: mov rdx, qword ptr [r14]
0049b190: mov rcx, rax
0049b193: mov rdx, qword ptr [rdx + 0x40]
0049b197: call 0x180362750
0049b19c: test rax, rax
0049b19f: je 0x18049b23f
0049b1a5: cmp ebx, dword ptr [r14 + 0x18]
0049b1a9: jae 0x18049b24f
0049b1af: movsxd rax, ebx
0049b1b2: mov rdx, rsi
0049b1b5: mov qword ptr [r14 + rax*8 + 0x20], rsi
0049b1ba: movsxd rax, ebx
0049b1bd: add rax, 4
0049b1c1: lea rcx, [r14 + rax*8]
0049b1c5: call 0x180362730
0049b1ca: test rsi, rsi
0049b1cd: je 0x18049b239
0049b1cf: lea rcx, [rsi + 0x68]
0049b1d3: mov qword ptr [rsi + 0x68], rdi
0049b1d7: mov rdx, rdi
0049b1da: call 0x180362730
0049b1df: movd xmm0, ebx
0049b1e3: mov al, 1
0049b1e5: cvtdq2ps xmm0, xmm0
0049b1e8: mov byte ptr [rsi + 0x7c], 1
0049b1ec: mov dword ptr [rsi + 0x80], r15d
0049b1f3: mulss xmm0, dword ptr [rip + 0x24e0931]
0049b1fb: movss dword ptr [rsi + 0x70], xmm0
0049b200: mov dword ptr [rdi + 0x248], 0
0049b20a: movaps xmm7, xmmword ptr [rsp + 0x50]
0049b20f: movaps xmm6, xmmword ptr [rsp + 0x60]
0049b214: mov r14, qword ptr [rsp + 0xa0]
0049b21c: mov rsi, qword ptr [rsp + 0x98]
0049b224: mov rbp, qword ptr [rsp + 0x90]
0049b22c: add rsp, 0x70
0049b230: pop r15
0049b232: pop rdi
0049b233: pop rbx
0049b234: ret 
0049b235: xor al, al
0049b237: jmp 0x18049b20a
0049b239: call 0x180363610
0049b23e: int3 
0049b23f: call 0x180362fe0
0049b244: mov rcx, rax
0049b247: xor edx, edx
0049b249: call 0x1803635d0
0049b24e: int3 
0049b24f: call 0x180363600
0049b254: int3 
0049b255: int3 
0049b256: int3 
0049b257: int3 
0049b258: int3 
0049b259: int3 
0049b25a: int3 
0049b25b: int3 
0049b25c: int3 
0049b25d: int3 
0049b25e: int3 
0049b25f: int3 
