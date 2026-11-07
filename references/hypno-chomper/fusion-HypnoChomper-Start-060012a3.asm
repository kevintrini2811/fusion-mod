004b1760: mov qword ptr [rsp + 8], rbx
004b1765: push rdi
004b1766: sub rsp, 0x20
004b176a: cmp byte ptr [rip + 0x30e7b49], 0
004b1771: mov rdi, rcx
004b1774: jne 0x1804b17a1
004b1776: lea rcx, [rip + 0x2f2a803]
004b177d: call 0x1803633b0
004b1782: lea rcx, [rip + 0x2f29307]
004b1789: call 0x1803633b0
004b178e: lea rcx, [rip + 0x2ee2e83]
004b1795: call 0x1803633b0
004b179a: mov byte ptr [rip + 0x30e7b19], 1
004b17a1: xor edx, edx
004b17a3: mov rcx, rdi
004b17a6: call 0x18049d040
004b17ab: mov rcx, qword ptr [rip + 0x2f2a7ce]
004b17b2: mov edx, 2
004b17b7: call 0x1803627b0
004b17bc: mov rbx, rax
004b17bf: test rax, rax
004b17c2: je 0x1804b1819
004b17c4: cmp dword ptr [rax + 0x18], 0
004b17c8: mov rdx, qword ptr [rip + 0x2ee2e49]
004b17cf: jbe 0x1804b181f
004b17d1: lea rcx, [rax + 0x20]
004b17d5: mov qword ptr [rax + 0x20], rdx
004b17d9: call 0x180362730
004b17de: cmp dword ptr [rbx + 0x18], 1
004b17e2: mov rdx, qword ptr [rip + 0x2f292a7]
004b17e9: jbe 0x1804b181f
004b17eb: lea rcx, [rbx + 0x28]
004b17ef: mov qword ptr [rbx + 0x28], rdx
004b17f3: call 0x180362730
004b17f8: xor edx, edx
004b17fa: mov rcx, rbx
004b17fd: call 0x1826b6590
004b1802: xor edx, edx
004b1804: mov ecx, eax
004b1806: call 0x1826b66c0
004b180b: mov rbx, qword ptr [rsp + 0x30]
004b1810: mov dword ptr [rdi + 0x4c], eax
004b1813: add rsp, 0x20
004b1817: pop rdi
004b1818: ret
004b1819: call 0x180363610
004b181e: int3
004b181f: call 0x180363600
004b1824: int3
004b1825: int3
004b1826: int3
004b1827: int3
004b1828: int3
004b1829: int3
004b182a: int3
004b182b: int3
004b182c: int3
004b182d: int3
004b182e: int3
004b182f: int3
