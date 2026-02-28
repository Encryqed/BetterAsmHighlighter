using System;
using System.Collections.Generic;

namespace BetterAsmHighlighter.Data
{
    internal static class Registers
    {
        public static readonly HashSet<string> All = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            // 64-bit
            "rax", "rbx", "rcx", "rdx", "rsi", "rdi", "rbp", "rsp",
            "r8", "r9", "r10", "r11", "r12", "r13", "r14", "r15",

            // 32-bit
            "eax", "ebx", "ecx", "edx", "esi", "edi", "ebp", "esp",
            "r8d", "r9d", "r10d", "r11d", "r12d", "r13d", "r14d", "r15d",

            // 16-bit
            "ax", "bx", "cx", "dx", "si", "di", "bp", "sp",
            "r8w", "r9w", "r10w", "r11w", "r12w", "r13w", "r14w", "r15w",

            // 8-bit
            "al", "bl", "cl", "dl", "ah", "bh", "ch", "dh",
            "sil", "dil", "bpl", "spl",
            "r8b", "r9b", "r10b", "r11b", "r12b", "r13b", "r14b", "r15b",

            // Instruction pointer
            "rip", "eip",

            // Flags
            "rflags", "eflags",

            // Segment
            "cs", "ds", "es", "fs", "gs", "ss",

            // Control
            "cr0", "cr2", "cr3", "cr4",

            // Debug
            "dr0", "dr1", "dr2", "dr3", "dr6", "dr7",
        };
    }
}
