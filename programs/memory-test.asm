#include "./rules.asm"

program:
    load r1, 52
    store r1, &0x05
    load r1, 4
    load r0, &r1, 1
