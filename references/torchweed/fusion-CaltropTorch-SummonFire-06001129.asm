004970c0: push rbx
004970c2: push r15
004970c4: sub rsp, 0x88
004970cb: cmp byte ptr [rip + 0x310212a], 0
004970d2: mov rbx, rcx
004970d5: jne 0x180497102
004970d7: lea rcx, [rip + 0x2f232ca]
004970de: call 0x1803633b0
004970e3: lea rcx, [rip + 0x2f0ca96]
004970ea: call 0x1803633b0
004970ef: lea rcx, [rip + 0x2f4a402]
004970f6: call 0x1803633b0
004970fb: mov byte ptr [rip + 0x31020fa], 1
00497102: mov rdx, qword ptr [rbx + 0x38]
00497106: xor r15d, r15d
00497109: mov qword ptr [rsp + 0xa8], rbp
00497111: mov qword ptr [rsp + 0xb0], rsi
00497119: mov qword ptr [rsp + 0xb8], rdi
00497121: mov qword ptr [rsp + 0x80], r14
00497129: movaps xmmword ptr [rsp + 0x70], xmm6
0049712e: movaps xmmword ptr [rsp + 0x50], xmm8
00497134: mov qword ptr [rsp + 0xa0], r15
0049713c: test rdx, rdx
0049713f: je 0x180497370
00497145: xor r8d, r8d
00497148: movaps xmmword ptr [rsp + 0x60], xmm7
0049714d: lea rcx, [rsp + 0x40]
00497152: call 0x1826c8510
00497157: movsd xmm7, qword ptr [rax]
0049715b: movaps xmm6, xmm7
0049715e: shufps xmm6, xmm6, 0x55
00497162: mov ecx, dword ptr [rbx + 0x4c]
00497165: xor edx, edx
00497167: movss xmm8, dword ptr [rip + 0x24e49d8]
00497170: call 0x1826b66c0
00497175: mov rcx, qword ptr [rip + 0x2f4a37c]
0049717c: mov edi, eax
0049717e: cmp dword ptr [rcx + 0xe0], r15d
00497185: jne 0x18049718c
00497187: call 0x180318580
0049718c: movaps xmm0, xmm8
00497190: mov qword ptr [rsp + 0x20], r15
00497195: movaps xmm1, xmm7
00497198: unpcklps xmm0, xmm8
0049719c: unpcklps xmm1, xmm6
0049719f: movq rdx, xmm0
004971a4: movq rcx, xmm1
004971a9: mov r9d, edi
004971ac: xorps xmm2, xmm2
004971af: call 0x18270ce10
004971b4: movaps xmm7, xmmword ptr [rsp + 0x60]
004971b9: mov rbp, rax
004971bc: mov edi, r15d
004971bf: mov ecx, r15d
004971c2: test rax, rax
004971c5: je 0x180497370
004971cb: movss xmm6, dword ptr [rip + 0x24e4969]
004971d3: lea r14, [rax + 0x20]
004971d7: cmp ecx, dword ptr [rbp + 0x18]
004971da: jge 0x180497334
004971e0: cmp edi, dword ptr [rbp + 0x18]
004971e3: jae 0x18049736a
004971e9: mov rcx, qword ptr [r14]
004971ec: test rcx, rcx
004971ef: je 0x180497370
004971f5: mov r8, qword ptr [rip + 0x2f231ac]
004971fc: lea rdx, [rsp + 0xa0]
00497204: call 0x180ac9930
00497209: test al, al
0049720b: je 0x180497327
00497211: mov rcx, qword ptr [rsp + 0xa0]
00497219: test rcx, rcx
0049721c: je 0x180497370
00497222: mov eax, dword ptr [rbx + 0xf8]
00497228: cmp dword ptr [rcx + 0xcc], eax
0049722e: jne 0x180497327
00497234: cmp byte ptr [rcx + 0x12d], r15b
0049723b: jne 0x180497327
00497241: xor r8d, r8d
00497244: mov edx, 0x3d
00497249: mov ecx, 0x3b
0049724e: call 0x1826bcca0
00497253: mov rcx, qword ptr [rip + 0x2f0c926]
0049725a: mov esi, eax
0049725c: cmp dword ptr [rcx + 0xe0], r15d
00497263: jne 0x18049726a
00497265: call 0x180318580
0049726a: xor r9d, r9d
0049726d: movaps xmm2, xmm8
00497271: movaps xmm1, xmm6
00497274: mov ecx, esi
00497276: call 0x18066a930
0049727b: mov rcx, qword ptr [rsp + 0xa0]
00497283: test rcx, rcx
00497286: je 0x180497370
0049728c: mov ecx, dword ptr [rcx + 0x58]
0049728f: xor edx, edx
00497291: call 0x1807f8390
00497296: mov r10, qword ptr [rsp + 0xa0]
0049729e: test al, al
004972a0: je 0x1804972dc
004972a2: test r10, r10
004972a5: je 0x180497370
004972ab: cmp dword ptr [r10 + 0x68], 1
004972b0: je 0x1804972dc
004972b2: mov rdx, qword ptr [r10]
004972b5: mov rcx, r10
004972b8: mov rax, qword ptr [rdx + 0x618]
004972bf: mov rdx, qword ptr [rdx + 0x620]
004972c6: call rax
004972c8: inc edi
004972ca: mov dword ptr [rbx + 0xe8], r15d
004972d1: add r14, 8
004972d5: mov ecx, edi
004972d7: jmp 0x1804971d7
004972dc: test r10, r10
004972df: je 0x180497370
004972e5: mov eax, dword ptr [rbx + 0x194]
004972eb: mov ecx, 0xc
004972f0: mov r8d, dword ptr [rbx + 0xf0]
004972f7: mov r9, rbx
004972fa: mov byte ptr [rsp + 0x30], r15b
004972ff: mov rdx, r10
00497302: mov dword ptr [rsp + 0x28], eax
00497306: mov dword ptr [rsp + 0x20], 1
0049730e: call 0x180002de0
00497313: mov rcx, qword ptr [rsp + 0xa0]
0049731b: test rcx, rcx
0049731e: je 0x180497370
00497320: xor edx, edx
00497322: call 0x18065f210
00497327: inc edi
00497329: add r14, 8
0049732d: mov ecx, edi
0049732f: jmp 0x1804971d7
00497334: movaps xmm8, xmmword ptr [rsp + 0x50]
0049733a: movaps xmm6, xmmword ptr [rsp + 0x70]
0049733f: mov r14, qword ptr [rsp + 0x80]
00497347: mov rdi, qword ptr [rsp + 0xb8]
0049734f: mov rsi, qword ptr [rsp + 0xb0]
00497357: mov rbp, qword ptr [rsp + 0xa8]
0049735f: add rsp, 0x88
00497366: pop r15
00497368: pop rbx
00497369: ret
0049736a: call 0x180363600
0049736f: int3
00497370: call 0x180363610
00497375: int3
00497376: int3
00497377: int3
00497378: int3
00497379: int3
0049737a: int3
0049737b: int3
0049737c: int3
0049737d: int3
0049737e: int3
0049737f: int3
