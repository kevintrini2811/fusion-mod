00497380: push rbx
00497382: sub rsp, 0x30
00497386: xor edx, edx
00497388: movaps xmmword ptr [rsp + 0x20], xmm6
0049738d: mov rbx, rcx
00497390: call 0x180470150
00497395: movss xmm0, dword ptr [rbx + 0x1d0]
0049739d: xorps xmm6, xmm6
004973a0: comiss xmm0, xmm6
004973a3: jbe 0x1804973da
004973a5: mov rdx, qword ptr [rbx]
004973a8: mov rcx, rbx
004973ab: mov rax, qword ptr [rdx + 0x408]
004973b2: mov rdx, qword ptr [rdx + 0x410]
004973b9: call rax
004973bb: comiss xmm6, dword ptr [rbx + 0x1d0]
004973c2: jb 0x1804973da
004973c4: xor edx, edx
004973c6: mov rcx, rbx
004973c9: call 0x1804970c0
004973ce: mov eax, dword ptr [rbx + 0x13c]
004973d4: mov dword ptr [rbx + 0x1d0], eax
004973da: movaps xmm6, xmmword ptr [rsp + 0x20]
004973df: add rsp, 0x30
004973e3: pop rbx
004973e4: ret
004973e5: int3
004973e6: int3
004973e7: int3
004973e8: int3
004973e9: int3
004973ea: int3
004973eb: int3
004973ec: int3
004973ed: int3
004973ee: int3
004973ef: int3
