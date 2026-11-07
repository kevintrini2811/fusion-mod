004b14f0: mov qword ptr [rsp + 8], rbx
004b14f5: push rdi
004b14f6: sub rsp, 0x40
004b14fa: cmp byte ptr [rip + 0x30e7dba], 0
004b1501: mov rbx, rcx
004b1504: jne 0x1804b1519
004b1506: lea rcx, [rip + 0x2f2a68b]
004b150d: call 0x1803633b0
004b1512: mov byte ptr [rip + 0x30e7da2], 1
004b1519: mov rcx, qword ptr [rip + 0x2f2a678]
004b1520: mov rdi, qword ptr [rbx + 0x90]
004b1527: cmp dword ptr [rcx + 0xe0], 0
004b152e: jne 0x1804b1535
004b1530: call 0x180318580
004b1535: xor r8d, r8d
004b1538: xor edx, edx
004b153a: mov rcx, rdi
004b153d: call 0x1826b9be0
004b1542: test al, al
004b1544: je 0x1804b15ea
004b154a: mov rcx, qword ptr [rbx + 0x90]
004b1551: test rcx, rcx
004b1554: je 0x1804b160b
004b155a: mov ecx, dword ptr [rcx + 0x58]
004b155d: xor edx, edx
004b155f: call 0x1807f8490
004b1564: mov rcx, qword ptr [rbx + 0x90]
004b156b: test al, al
004b156d: jne 0x1804b15af
004b156f: test rcx, rcx
004b1572: je 0x1804b160b
004b1578: mov eax, dword ptr [rcx + 0x68]
004b157b: dec eax
004b157d: mov rdx, qword ptr [rbx]
004b1580: cmp eax, 2
004b1583: jbe 0x1804b15ed
004b1585: cmp byte ptr [rcx + 0x135], 0
004b158c: jne 0x1804b15ed
004b158e: mov rax, qword ptr [rdx + 0x608]
004b1595: mov r8, qword ptr [rdx + 0x610]
004b159c: mov rdx, rcx
004b159f: mov rcx, rbx
004b15a2: mov rbx, qword ptr [rsp + 0x50]
004b15a7: add rsp, 0x40
004b15ab: pop rdi
004b15ac: jmp rax
004b15af: test rcx, rcx
004b15b2: je 0x1804b160b
004b15b4: mov rax, qword ptr [rcx]
004b15b7: mov r9d, 4
004b15bd: mov r8, rbx
004b15c0: mov edx, 0x28
004b15c5: mov r10, qword ptr [rax + 0x1f8]
004b15cc: mov rax, qword ptr [rax + 0x200]
004b15d3: mov qword ptr [rsp + 0x30], rax
004b15d8: mov eax, dword ptr [rbx + 0x194]
004b15de: mov byte ptr [rsp + 0x28], 0
004b15e3: mov dword ptr [rsp + 0x20], eax
004b15e7: call r10
004b15ea: mov rdx, qword ptr [rbx]
004b15ed: mov rax, qword ptr [rdx + 0x618]
004b15f4: mov rcx, rbx
004b15f7: mov rdx, qword ptr [rdx + 0x620]
004b15fe: mov rbx, qword ptr [rsp + 0x50]
004b1603: add rsp, 0x40
004b1607: pop rdi
004b1608: jmp rax
004b160b: call 0x180363610
004b1610: int3
004b1611: int3
004b1612: int3
004b1613: int3
004b1614: int3
004b1615: int3
004b1616: int3
004b1617: int3
004b1618: int3
004b1619: int3
004b161a: int3
004b161b: int3
004b161c: int3
004b161d: int3
004b161e: int3
004b161f: int3
