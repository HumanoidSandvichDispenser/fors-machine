#include "rules.asm"

program:
    .fibonacci:
        load r1, 1
        load r2, 1
        add r0, r1, r2
        load r2, r1
        load r1, r0
        jmp &.fibonacci
