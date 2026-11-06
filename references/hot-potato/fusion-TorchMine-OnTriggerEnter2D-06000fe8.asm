00489f90: mov qword ptr [rsp + 8], rbx
00489f95: mov qword ptr [rsp + 0x18], rsi
00489f9a: push rdi
00489f9b: sub rsp, 0x40
00489f9f: cmp byte ptr [rip + 0x310f1ab], 0
00489fa6: mov rdi, rdx
00489fa9: mov rbx, rcx
00489fac: jne 0x180489fcd
00489fae: lea rcx, [rip + 0x2f2d55b]
00489fb5: call 0x1803633b0
00489fba: lea rcx, [rip + 0x2f51bd7]
00489fc1: call 0x1803633b0
00489fc6: mov byte ptr [rip + 0x310f184], 1
00489fcd: xor esi, esi
00489fcf: mov qword ptr [rsp + 0x58], rsi
00489fd4: test rdi, rdi
00489fd7: je 0x18048a1b9
00489fdd: mov r8, qword ptr [rip + 0x2f2d52c]
00489fe4: lea rdx, [rsp + 0x58]
00489fe9: mov rcx, rdi
00489fec: call 0x180ac9930
00489ff1: test al, al
00489ff3: je 0x18048a0f1
00489ff9: mov rax, qword ptr [rsp + 0x58]
00489ffe: test rax, rax
0048a001: je 0x18048a1b9
0048a007: mov rcx, qword ptr [rip + 0x2f51b8a]
0048a00e: mov rdi, qword ptr [rax + 0x88]
0048a015: cmp dword ptr [rcx + 0xe0], esi
0048a01b: jne 0x18048a022
0048a01d: call 0x180318580
0048a022: xor r8d, r8d
0048a025: mov rdx, rbx
0048a028: mov rcx, rdi
0048a02b: call 0x1826b9a40
0048a030: test al, al
0048a032: jne 0x18048a0f1
0048a038: mov rdx, qword ptr [rsp + 0x58]
0048a03d: test rdx, rdx
0048a040: je 0x18048a1b9
0048a046: mov eax, dword ptr [rbx + 0x20]
0048a049: cmp dword ptr [rdx + 0xe0], eax
0048a04f: jne 0x18048a0f1
0048a055: cmp dword ptr [rdx + 0xc8], 5
0048a05c: je 0x18048a070
0048a05e: mov eax, dword ptr [rbx + 0xf8]
0048a064: cmp dword ptr [rdx + 0xc0], eax
0048a06a: jne 0x18048a0f1
0048a070: mov eax, dword ptr [rdx + 0x54]
0048a073: cmp eax, 2
0048a076: jne 0x18048a101
0048a07c: mov rax, qword ptr [rbx + 0x30]
0048a080: test rax, rax
0048a083: je 0x18048a1b9
0048a089: mov rcx, qword ptr [rax + 0x48]
0048a08d: test rcx, rcx
0048a090: je 0x18048a1b9
0048a096: mov r9d, dword ptr [rdx + 0xdc]
0048a09d: mov r8, rbx
0048a0a0: mov qword ptr [rsp + 0x30], rsi
0048a0a5: mov byte ptr [rsp + 0x28], 1
0048a0aa: mov dword ptr [rsp + 0x20], 0xe8
0048a0b2: call 0x180420810
0048a0b7: mov rcx, qword ptr [rsp + 0x58]
0048a0bc: test rcx, rcx
0048a0bf: je 0x18048a1b9
0048a0c5: test rax, rax
0048a0c8: je 0x18048a1b9
0048a0ce: mov ecx, dword ptr [rcx + 0xe8]
0048a0d4: mov dword ptr [rax + 0xe8], ecx
0048a0da: mov eax, dword ptr [rbx + 0x248]
0048a0e0: inc eax
0048a0e2: mov dword ptr [rbx + 0x248], eax
0048a0e8: cmp eax, 0x64
0048a0eb: jg 0x18048a199
0048a0f1: mov rbx, qword ptr [rsp + 0x50]
0048a0f6: mov rsi, qword ptr [rsp + 0x60]
0048a0fb: add rsp, 0x40
0048a0ff: pop rdi
0048a100: ret 
0048a101: cmp eax, 7
0048a104: jne 0x18048a14b
0048a106: mov rax, qword ptr [rbx + 0x30]
0048a10a: test rax, rax
0048a10d: je 0x18048a1b9
0048a113: mov rcx, qword ptr [rax + 0x48]
0048a117: test rcx, rcx
0048a11a: je 0x18048a1b9
0048a120: mov qword ptr [rsp + 0x30], rsi
0048a125: mov r9d, 0x14
0048a12b: mov byte ptr [rsp + 0x28], 1
0048a130: mov r8, rbx
0048a133: mov dword ptr [rsp + 0x20], 0x19
0048a13b: call 0x180420810
0048a140: mov eax, dword ptr [rbx + 0x248]
0048a146: add eax, 8
0048a149: jmp 0x18048a0e2
0048a14b: cmp eax, 0xa
0048a14e: jne 0x18048a0f1
0048a150: mov rax, qword ptr [rbx + 0x30]
0048a154: test rax, rax
0048a157: je 0x18048a1b9
0048a159: mov rcx, qword ptr [rax + 0x48]
0048a15d: test rcx, rcx
0048a160: je 0x18048a1b9
0048a162: mov qword ptr [rsp + 0x30], rsi
0048a167: mov r9d, 0x14
0048a16d: mov byte ptr [rsp + 0x28], 1
0048a172: mov r8, rbx
0048a175: mov dword ptr [rsp + 0x20], 0x32
0048a17d: call 0x180420810
0048a182: mov eax, dword ptr [rbx + 0x248]
0048a188: inc eax
0048a18a: mov dword ptr [rbx + 0x248], eax
0048a190: cmp eax, 0x64
0048a193: jle 0x18048a0f1
0048a199: xor edx, edx
0048a19b: mov dword ptr [rbx + 0x248], esi
0048a1a1: mov rcx, rbx
0048a1a4: call 0x18048a1c0
0048a1a9: mov rbx, qword ptr [rsp + 0x50]
0048a1ae: mov rsi, qword ptr [rsp + 0x60]
0048a1b3: add rsp, 0x40
0048a1b7: pop rdi
0048a1b8: ret 
0048a1b9: call 0x180363610
0048a1be: int3 
0048a1bf: int3 
