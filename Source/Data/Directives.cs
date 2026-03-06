using System;
using System.Collections.Generic;

namespace BetterAsmHighlighter.Data
{
    // https://learn.microsoft.com/en-us/cpp/assembler/masm/directives-reference
    internal static class Directives
    {
        public static readonly HashSet<string> All = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            // Procedure
            "proc", "endp", "proto", "invoke",

            // Simplified segment
            ".code", ".const", ".data", ".data?",
            ".fardata", ".fardata?",
            ".model", ".stack",
            ".startup", ".exit",

            // Segment
            "segment", "ends", "group",
            ".alpha", ".dosseg", ".seq",

            // Code labels
            "align", "even", "label", "org",

            // Data definition
            "db", "dw", "dd", "dq", "dt", "df",
            "byte", "sbyte", "word", "sword", "dword", "sdword",
            "fword", "qword", "tbyte",
            "real4", "real8", "real10",
            "mmword", "xmmword", "ymmword",
            "ptr", "dup",

            // Equates
            "equ", "textequ",

            // Structure and record
            "struct", "union", "record", "typedef",

            // Scope
            "extern", "extrn", "externdef",
            "public", "comm",

            // Program
            "end",

            // Conditional assembly
            "if", "ife", "if2",
            "ifb", "ifnb",
            "ifdef", "ifndef",
            "ifdif", "ifdifi",
            "ifidn", "ifidni",
            "else", "elseif", "elseif2",
            "endif",

            // Conditional control flow
            ".if", ".else", ".elseif",
            ".endif", ".endw",
            ".while", ".repeat",
            ".until", ".untilcxz",
            ".break", ".continue",

            // Conditional error
            ".err", ".err2",
            ".errb", ".errnb",
            ".errdef", ".errndef",
            ".errdif", ".errdifi",
            ".erridn", ".erridni",
            ".erre", ".errnz",

            // Macros
            "macro", "endm", "exitm",
            "local", "purge", "goto",
            "for", "forc",
            "repeat", "while",

            // Miscellaneous
            "alias", "assume", "comment", "echo",
            "include", "includelib",
            "option", "popcontext", "pushcontext",
            ".radix", ".safeseh", ".fpo",

            // Listing control
            ".cref", ".nocref",
            ".list", ".nolist",
            ".listall", ".listif", ".nolistif",
            ".listmacro", ".listmacroall", ".nolistmacro",
            ".tfcond",
            "page", "subtitle", "title",

            // Processor
            ".386", ".386p", ".387",
            ".486", ".486p",
            ".586", ".586p",
            ".686", ".686p",
            ".k3d", ".mmx", ".xmm",

            // x64
            ".allocstack", ".endprolog",
            ".pushframe", ".pushreg",
            ".savereg", ".savexmm128",
            ".setframe",

            // String
            "catstr", "instr", "sizestr", "substr",
        };
    }
}
