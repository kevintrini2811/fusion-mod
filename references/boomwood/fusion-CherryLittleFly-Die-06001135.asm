004987a0: mov qword ptr [rsp + 0x10], rbx
004987a5: push rdi
004987a6: sub rsp, 0x20
004987aa: cmp byte ptr [rip + 0x3100a50], 0
004987b1: mov rbx, rcx
004987b4: jne 0x1804987e1
004987b6: lea rcx, [rip + 0x2ee1343]
004987bd: call 0x1803633b0
004987c2: lea rcx, [rip + 0x2f21287]
004987c9: call 0x1803633b0
004987ce: lea rcx, [rip + 0x2f433c3]
004987d5: call 0x1803633b0
004987da: mov byte ptr [rip + 0x3100a20], 1
004987e1: mov rcx, qword ptr [rip + 0x2f433b0]
004987e8: mov rdi, qword ptr [rbx + 0x68]
004987ec: mov qword ptr [rsp + 0x30], 0
004987f5: cmp dword ptr [rcx + 0xe0], 0
004987fc: jne 0x180498803
004987fe: call 0x180318580
00498803: xor r8d, r8d
00498806: xor edx, edx
00498808: mov rcx, rdi
0049880b: call 0x1826b9be0
00498810: test al, al
00498812: je 0x180498882
00498814: mov rcx, qword ptr [rbx + 0x68]
00498818: test rcx, rcx
0049881b: je 0x1804988d5
00498821: mov r8, qword ptr [rip + 0x2f21228]
00498828: lea rdx, [rsp + 0x30]
0049882d: call 0x180ac9930
00498832: test al, al
00498834: je 0x180498882
00498836: mov rax, qword ptr [rip + 0x2ee12c3]
0049883d: mov rcx, qword ptr [rax + 0xb8]
00498844: mov rax, qword ptr [rcx]
00498847: test rax, rax
0049884a: je 0x1804988d5
00498850: cmp byte ptr [rax + 0x23a], 0
00498857: mov rcx, qword ptr [rsp + 0x30]
0049885c: jne 0x18049886d
0049885e: test rcx, rcx
00498861: je 0x1804988d5
00498863: movss xmm1, dword ptr [rip + 0x24e333d]
0049886b: jmp 0x18049887a
0049886d: test rcx, rcx
00498870: je 0x1804988d5
00498872: movss xmm1, dword ptr [rip + 0x24e32fa]
0049887a: xor r8d, r8d
0049887d: call 0x1804f33b0
00498882: mov rax, qword ptr [rip + 0x2ee1277]
00498889: mov rcx, qword ptr [rax + 0xb8]
00498890: mov rdx, qword ptr [rcx]
00498893: test rdx, rdx
00498896: je 0x1804988d5
00498898: dec dword ptr [rdx + 0x148]
0049889e: mov rcx, rbx
004988a1: xor edx, edx
004988a3: call 0x1826b16d0
004988a8: mov rcx, qword ptr [rip + 0x2f432e9]
004988af: mov rbx, rax
004988b2: cmp dword ptr [rcx + 0xe0], 0
004988b9: jne 0x1804988c0
004988bb: call 0x180318580
004988c0: xor edx, edx
004988c2: mov rcx, rbx
004988c5: call 0x1826b8410
004988ca: mov rbx, qword ptr [rsp + 0x38]
004988cf: add rsp, 0x20
004988d3: pop rdi
004988d4: ret 
004988d5: call 0x180363610
004988da: int3 
004988db: int3 
004988dc: int3 
004988dd: int3 
004988de: int3 
004988df: int3 
