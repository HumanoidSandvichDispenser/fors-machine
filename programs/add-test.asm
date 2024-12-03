#include "./rules.asm"

program:
	; add 1 and 2, so push 2 and 1

	push 2
	push 1
	push_rpc
	jmp &.extern_proc_add

	load r0, rax

	jmp &.end

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
		load r0, &rsp
		jmp r0

	.end:
		jmp &.end
