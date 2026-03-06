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

            // x87 FPU
            "st", "st(0)", "st(1)", "st(2)", "st(3)", "st(4)", "st(5)", "st(6)", "st(7)",

            // MMX
            "mm0", "mm1", "mm2", "mm3", "mm4", "mm5", "mm6", "mm7",

            // SSE
            "xmm0", "xmm1", "xmm2", "xmm3", "xmm4", "xmm5", "xmm6", "xmm7",
            "xmm8", "xmm9", "xmm10", "xmm11", "xmm12", "xmm13", "xmm14", "xmm15",

            // AVX
            "ymm0", "ymm1", "ymm2", "ymm3", "ymm4", "ymm5", "ymm6", "ymm7",
            "ymm8", "ymm9", "ymm10", "ymm11", "ymm12", "ymm13", "ymm14", "ymm15",

            // AVX-512
            "zmm0", "zmm1", "zmm2", "zmm3", "zmm4", "zmm5", "zmm6", "zmm7",
            "zmm8", "zmm9", "zmm10", "zmm11", "zmm12", "zmm13", "zmm14", "zmm15",
            "zmm16", "zmm17", "zmm18", "zmm19", "zmm20", "zmm21", "zmm22", "zmm23",
            "zmm24", "zmm25", "zmm26", "zmm27", "zmm28", "zmm29", "zmm30", "zmm31",

            // AVX-512 opmask
            "k0", "k1", "k2", "k3", "k4", "k5", "k6", "k7",

            // MXCSR
            "mxcsr",
        };
    }
}
