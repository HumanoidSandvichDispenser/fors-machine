#include "rules.asm"

program:
	push 2
	push_rpc
	jmp &.proc_fibonacci
	load r0, rax
	jmp &.end

	.proc_fibonacci:
		; fibonacci(n) = fibonacci(n-1) + fibonacci(n-2)
		; prologue
		push rbp
		load rbp, rsp

		load r0, &rbp, -2

		; n == 1 or n == 0
		load r3, 1
		load r4, 0
		cmp r1, r0, r3
		cmp r2, r0, r4
		or r1, r1, r2
		jmp_if r1, &.n_eq_1_or_0
		jmp &.else
	.n_eq_1_or_0:
		load rax, 1
		jmp &.proc_fibonacci_return
	.else:
		; call fibonacci(n-1)
		load r0, &rbp, -2
		incr r0, r0, -1
		push r0
		push_rpc
		jmp &.proc_fibonacci
		pop
		push rax

		; call fibonacci(n-2)
		load r0, &rbp, -2
		incr r0, r0, -2
		push r0
		push_rpc
		jmp &.proc_fibonacci
		pop
		push rax

		pop r3
		pop r4
		load rax, 0
		add rax, r3, r4
		jmp &.proc_fibonacci_return
	.proc_fibonacci_return:
		; epilogue
		leave
		pop rbp
		pop r0
		jmp r0

	.extern_proc_add:
		; prologue
		push rbp
		load rbp, rsp
		incr rbp, rbp, -1

		; body
		load r0, &rbp, -2
		load r1, &rbp, -3
		add rax, r0, r1

		; epilogue
		leave
		pop rbp
		pop rsp
		jmp r0

	.end:
		jmp &.end
