using System;
using System.Collections.Generic;

namespace BetterAsmHighlighter.Data
{
    internal static class Instructions
    {
        public static readonly HashSet<string> All = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            // Data movement
            "mov", "movzx", "movsx", "movsxd", "lea", "xchg",
            "push", "pop",

            // Arithmetic
            "add", "sub", "mul", "imul", "div", "idiv",
            "inc", "dec", "neg", "adc", "sbb",

            // Bitwise
            "and", "or", "xor", "not",
            "shl", "shr", "rol", "ror",

            // Compare / test
            "cmp", "test",

            // Jumps
            "jmp", "je", "jne", "jz", "jnz",
            "jl", "jle", "jg", "jge",

            // Control flow
            "call", "ret", "retn", "enter", "leave",

            // Flags
            "pushfq", "popfq",

            // Misc
            "nop", "int", "int3", "syscall",
            "cpuid", "rdtsc",
        };
    }
}
