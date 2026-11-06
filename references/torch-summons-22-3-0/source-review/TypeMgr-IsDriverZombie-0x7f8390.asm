007f8390: push rbx
007f8392: sub rsp, 0x20
007f8396: cmp byte ptr [rip + 0x2da251e], 0
007f839d: mov ebx, ecx
007f839f: jne 0x1807f83b4
007f83a1: lea rcx, [rip + 0x2bb8a98]
007f83a8: call 0x1803633b0
007f83ad: mov byte ptr [rip + 0x2da2507], 1
007f83b4: xor edx, edx
007f83b6: mov ecx, 0xd
007f83bb: call 0x180671000
007f83c0: test rax, rax
007f83c3: je 0x1807f83db
007f83c5: mov r8, qword ptr [rip + 0x2bb8a74]
007f83cc: mov edx, ebx
007f83ce: mov rcx, rax
007f83d1: add rsp, 0x20
007f83d5: pop rbx
007f83d6: jmp 0x181600980
007f83db: call 0x180363610
007f83e0: int3
007f83e1: int3
007f83e2: int3
007f83e3: int3
007f83e4: int3
007f83e5: int3
007f83e6: int3
007f83e7: int3
007f83e8: int3
007f83e9: int3
007f83ea: int3
007f83eb: int3
007f83ec: int3
007f83ed: int3
007f83ee: int3
007f83ef: int3
