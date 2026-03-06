using System;
using System.Collections.Generic;

namespace BetterAsmHighlighter.Data
{
    // https://www.felixcloutier.com/x86/
    internal static class Instructions
    {
        public static readonly HashSet<string> All = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            // Data movement
            "mov", "movzx", "movsx", "movsxd", "lea", "xchg",
            "push", "pop", "pusha", "pushad", "popa", "popad",
            "movbe", "movnti",

            // Arithmetic
            "add", "sub", "mul", "imul", "div", "idiv",
            "inc", "dec", "neg", "adc", "sbb",
            "adcx", "adox", "mulx",

            // Sign extension
            "cbw", "cwde", "cdqe",
            "cwd", "cdq", "cqo",

            // Bitwise
            "and", "or", "xor", "not",
            "shl", "shr", "sal", "sar",
            "rol", "ror", "rcl", "rcr",
            "shld", "shrd",
            "rorx", "sarx", "shlx", "shrx",
            "andn",

            // Bit manipulation
            "bt", "btc", "btr", "bts",
            "bsf", "bsr", "bswap",
            "popcnt", "lzcnt", "tzcnt",
            "blsi", "blsmsk", "blsr",
            "bextr", "bzhi",
            "pdep", "pext",

            // Compare / test
            "cmp", "test",

            // CMOVcc
            "cmova", "cmovae", "cmovb", "cmovbe", "cmovc", "cmove",
            "cmovg", "cmovge", "cmovl", "cmovle",
            "cmovna", "cmovnae", "cmovnb", "cmovnbe", "cmovnc", "cmovne",
            "cmovng", "cmovnge", "cmovnl", "cmovnle",
            "cmovno", "cmovnp", "cmovns", "cmovnz",
            "cmovo", "cmovp", "cmovpe", "cmovpo", "cmovs", "cmovz",

            // SETcc
            "seta", "setae", "setb", "setbe", "setc", "sete",
            "setg", "setge", "setl", "setle",
            "setna", "setnae", "setnb", "setnbe", "setnc", "setne",
            "setng", "setnge", "setnl", "setnle",
            "setno", "setnp", "setns", "setnz",
            "seto", "setp", "setpe", "setpo", "sets", "setz",

            // Jumps
            "jmp",
            "ja", "jae", "jb", "jbe", "jc", "je", "jg", "jge",
            "jl", "jle", "jna", "jnae", "jnb", "jnbe", "jnc", "jne",
            "jng", "jnge", "jnl", "jnle",
            "jno", "jnp", "jns", "jnz",
            "jo", "jp", "jpe", "jpo", "js", "jz",
            "jcxz", "jecxz", "jrcxz",

            // Control flow
            "call", "ret", "retn",
            "enter", "leave",
            "loop", "loope", "loopne", "loopz", "loopnz",
            "int", "int1", "int3", "into",
            "iret", "iretd", "iretq",
            "syscall", "sysenter", "sysexit", "sysret",
            "bound",

            // String operations
            "movs", "movsb", "movsw", "movsd", "movsq",
            "cmps", "cmpsb", "cmpsw", "cmpsd", "cmpsq",
            "scas", "scasb", "scasd", "scasw",
            "stos", "stosb", "stosd", "stosq", "stosw",
            "lods", "lodsb", "lodsd", "lodsq", "lodsw",
            "rep", "repe", "repne", "repz", "repnz",

            // Atomic / synchronization
            "lock",
            "cmpxchg", "cmpxchg8b", "cmpxchg16b",
            "xadd",

            // Flag manipulation
            "clc", "stc", "cmc",
            "cld", "std",
            "cli", "sti",
            "lahf", "sahf",
            "pushf", "pushfd", "pushfq",
            "popf", "popfd", "popfq",

            // Misc
            "nop", "pause", "hlt", "ud2",
            "cpuid", "rdtsc", "rdtscp", "rdpid",
            "xlat", "xlatb",
            "lfence", "sfence", "mfence",
            "clflush", "clflushopt", "clwb",
            "prefetchw", "prefetcht0", "prefetcht1", "prefetcht2", "prefetchnta",
            "rdrand", "rdseed",
            "crc32",
            "endbr32", "endbr64",
            "serialize",
            "xgetbv", "xsetbv",
            "xsave", "xsavec", "xsaveopt", "xrstor",
            "swapgs",
            "wbinvd", "invlpg",

            // LEA-related segment loads
            "lds", "les", "lfs", "lgs", "lss",

            // x87 FPU
            "fld", "fst", "fstp", "fild", "fist", "fistp", "fisttp",
            "fadd", "faddp", "fiadd",
            "fsub", "fsubp", "fisub", "fsubr", "fsubrp", "fisubr",
            "fmul", "fmulp", "fimul",
            "fdiv", "fdivp", "fidiv", "fdivr", "fdivrp", "fidivr",
            "fchs", "fabs", "fsqrt", "frndint",
            "fsin", "fcos", "fsincos", "fptan", "fpatan",
            "f2xm1", "fyl2x", "fyl2xp1", "fscale", "fxtract",
            "fprem", "fprem1",
            "fcom", "fcomp", "fcompp", "fcomi", "fcomip", "fucomi", "fucomip",
            "fucom", "fucomp", "fucompp", "ficom", "ficomp",
            "ftst", "fxam",
            "fxch",
            "fld1", "fldz", "fldpi", "fldl2e", "fldl2t", "fldlg2", "fldln2",
            "fldcw", "fstcw", "fnstcw", "fldenv", "fstenv", "fnstenv",
            "fstsw", "fnstsw",
            "finit", "fninit", "fclex", "fnclex",
            "fsave", "fnsave", "frstor",
            "fincstp", "fdecstp", "ffree", "fnop", "fwait", "wait",
            "fbld", "fbstp",
            "fxsave", "fxrstor",

            // MMX
            "movd", "movq",
            "paddb", "paddw", "paddd", "paddq",
            "paddsb", "paddsw", "paddusb", "paddusw",
            "psubb", "psubw", "psubd", "psubq",
            "psubsb", "psubsw", "psubusb", "psubusw",
            "pmullw", "pmulhw", "pmulhuw", "pmuludq",
            "pmaddwd",
            "pcmpeqb", "pcmpeqw", "pcmpeqd",
            "pcmpgtb", "pcmpgtw", "pcmpgtd",
            "pand", "pandn", "por", "pxor",
            "psllw", "pslld", "psllq",
            "psrlw", "psrld", "psrlq",
            "psraw", "psrad",
            "packsswb", "packssdw", "packuswb",
            "punpckhbw", "punpckhwd", "punpckhdq",
            "punpcklbw", "punpcklwd", "punpckldq",
            "emms",

            // SSE
            "movaps", "movups", "movss",
            "movlps", "movhps", "movlhps", "movhlps",
            "movmskps",
            "addps", "addss", "subps", "subss",
            "mulps", "mulss", "divps", "divss",
            "sqrtps", "sqrtss", "rsqrtps", "rsqrtss", "rcpps", "rcpss",
            "maxps", "maxss", "minps", "minss",
            "cmpps", "cmpss", "comiss", "ucomiss",
            "andps", "andnps", "orps", "xorps",
            "shufps", "unpckhps", "unpcklps",
            "cvtsi2ss", "cvtss2si", "cvttss2si",
            "cvtpi2ps", "cvtps2pi", "cvttps2pi",
            "ldmxcsr", "stmxcsr",
            "movntps", "movntq",
            "maskmovq",

            // SSE2
            "movapd", "movupd", "movsd",
            "movlpd", "movhpd",
            "movmskpd",
            "addpd", "addsd", "subpd", "subsd",
            "mulpd", "mulsd", "divpd", "divsd",
            "sqrtpd", "sqrtsd",
            "maxpd", "maxsd", "minpd", "minsd",
            "cmppd", "cmpsd", "comisd", "ucomisd",
            "andpd", "andnpd", "orpd", "xorpd",
            "shufpd", "unpckhpd", "unpcklpd",
            "cvtsi2sd", "cvtsd2si", "cvttsd2si",
            "cvtsd2ss", "cvtss2sd",
            "cvtdq2ps", "cvtps2dq", "cvttps2dq",
            "cvtdq2pd", "cvtpd2dq", "cvttpd2dq",
            "cvtpd2ps", "cvtps2pd",
            "movdqa", "movdqu",
            "movntpd", "movntdq",
            "maskmovdqu",
            "pshufhw", "pshuflw", "pshufd",
            "pslldq", "psrldq",
            "punpckhqdq", "punpcklqdq",
            "pcmpeqq", "pcmpgtq",
            "pmovmskb",
            "pinsrw", "pextrw",
            "psadbw", "pavgb", "pavgw",
            "pmaxub", "pmaxsw", "pminub", "pminsw",
            "movdq2q", "movq2dq",
            "lddqu",
            "clflush",

            // SSE3 / SSSE3
            "addsubpd", "addsubps",
            "haddpd", "haddps", "hsubpd", "hsubps",
            "movddup", "movshdup", "movsldup",
            "monitor", "mwait",
            "pabsb", "pabsw", "pabsd",
            "palignr",
            "pshufb",
            "phaddw", "phaddd", "phaddsw",
            "phsubw", "phsubd", "phsubsw",
            "pmaddubsw", "pmulhrsw",
            "psignb", "psignw", "psignd",

            // SSE4.1 / SSE4.2
            "blendpd", "blendps", "blendvpd", "blendvps",
            "pblendw", "pblendvb",
            "dpps", "dppd",
            "extractps", "insertps",
            "pinsrb", "pinsrd", "pinsrq",
            "pextrb", "pextrd", "pextrq",
            "pmaxsb", "pmaxsd", "pmaxuw", "pmaxud",
            "pminsb", "pminsd", "pminuw", "pminud",
            "packusdw", "pmulld", "pmuldq",
            "pmovsxbw", "pmovsxbd", "pmovsxbq",
            "pmovsxwd", "pmovsxwq", "pmovsxdq",
            "pmovzxbw", "pmovzxbd", "pmovzxbq",
            "pmovzxwd", "pmovzxwq", "pmovzxdq",
            "roundps", "roundpd", "roundss", "roundsd",
            "mpsadbw", "phminposuw",
            "ptest",
            "pcmpestri", "pcmpestrm", "pcmpistri", "pcmpistrm",
            "crc32", "popcnt",

            // AES-NI
            "aesenc", "aesenclast",
            "aesdec", "aesdeclast",
            "aesimc", "aeskeygenassist",
            "pclmulqdq",

            // SHA
            "sha1rnds4", "sha1nexte", "sha1msg1", "sha1msg2",
            "sha256rnds2", "sha256msg1", "sha256msg2",
        };
    }
}
