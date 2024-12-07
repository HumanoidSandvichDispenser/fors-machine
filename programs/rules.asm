;; https://github.com/hlorenzi/customasm

#bankdef program
{
	#addr 0x0000
    #size 0x8000
	#bits 32
	#outp 0
}

#subruledef register
{
	0 => 0x00
	1 => 0x01
	2 => 0x02
	3 => 0x03
	4 => 0x04
	5 => 0x05
	6 => 0x06
	7 => 0x07
	ax => 0x0c
	sp => 0x0d
	bp => 0x0e
	pc => 0x0f
}

#ruledef
{
	nop => 0x00000000
	halt => 0xFFFFFFFF

	load r{register: register}, {value: i16} => 0x01 @ register @ value
	load r{p: register}, r{q: register} => 0x02 @ p @ q @ 0x00
	load r{p: register}, &{addr: u16} => 0x03 @ p @ addr
	load r{p: register}, &r{q: register} => 0x04 @ p @ q @ 0x00
	load r{p: register}, &r{q: register}, {offset: s8} => 0x04 @ p @ q @ offset

	push {value: i16} => {
		asm {
			incr rsp, rsp, 1
			load r0, { value }
			store r0, &rsp
		}
	}
	push r{register: register} => {
		asm {
			incr rsp, rsp, 1
			store r{ register }, &rsp
		}
	}
	push_rpc => {
		asm {
			load r0, rpc
			incr r0, r0, 5
			push r0
		}
	}
	pop r{register: register} => {
		asm {
			load r{register}, &rsp
			pop
		}
	}
	pop => {
		asm { incr rsp, rsp, -1 }
	}
	leave => asm { load rsp, rbp }

	store {value: i8}, &{addr: u16} => 0x10 @ value @ addr
	store r{register: register}, &{addr: u16} => 0x11 @ register @ addr
	store r{register: register}, &r{q: register} => 0x12 @ register @ q @ 0x00
	store r{register: register}, &r{q: register}, {offset: s8} => 0x12 @ register @ q @ offset

	and r{p: register}, r{q: register}, r{r: register} => 0x20 @ p @ q @ r
	or r{p: register}, r{q: register}, r{r: register} => 0x21 @ p @ q @ r
	xor r{p: register}, r{q: register}, r{r: register} => 0x22 @ p @ q @ r
	neg r{p: register}, r{q: register} => 0x23 @ p @ q @ 0x00
	add r{p: register}, r{q: register}, r{r: register} => 0x24 @ p @ q @ r
	sub r{p: register}, r{q: register}, r{r: register} => 0x25 @ p @ q @ r
	mul r{p: register}, r{q: register}, r{r: register} => 0x26 @ p @ q @ r
	div r{p: register}, r{q: register}, r{r: register} => 0x27 @ p @ q @ r
	mod r{p: register}, r{q: register}, r{r: register} => 0x28 @ p @ q @ r
	lshift r{p: register}, r{q: register}, r{r: register} => 0x29 @ p @ q @ r
	rshift r{p: register}, r{q: register}, r{r: register} => 0x2a @ p @ q @ r
	lshift r{p: register}, r{q: register}, {value: i8} => 0x2b @ p @ q @ value
	rshift r{p: register}, r{q: register}, {value: i8} => 0x2c @ p @ q @ value
	incr r{p: register}, r{q: register}, {value: i8} => 0x2d @ p @ q @ value

	cmp r{p: register}, r{q: register}, r{r: register} => 0x40 @ p @ q @ r
	scmp r{p: register}, r{q: register}, r{r: register} => 0x41 @ p @ q @ r

	jmp &{address: u16} => 0x50 @ 0x00 @ address
	jmp_if r{p: register}, &{address: u16} => 0x51 @ p @ address
	jmp r{q: register} => 0x52 @ 0x00 @ q @ 0x00
	jmp_if r{p: register}, r{q: register} => 0x53 @ p @ q @ 0x00
	jmp_by {offset: i16} => 0x54 @ 0x00 @ offset
	jmp_by_if r{p: register}, {offset: i16} => 0x55 @ p @ offset
	jmp_by r{q: register} => 0x56 @ 0x00 @ q @ 0x00
	jmp_by_if r{p: register}, r{q: register} => 0x57 @ p @ q @ 0x00
}
