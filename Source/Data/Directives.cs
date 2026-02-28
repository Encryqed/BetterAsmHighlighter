using System;
using System.Collections.Generic;

namespace BetterAsmHighlighter.Data
{
    internal static class Directives
    {
        public static readonly HashSet<string> All = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            // Procedure
            "proc", "endp",

            // Segments
            ".code", ".data",

            // Program
            "end",

            // Data types
            "db", "dw", "dd", "dq",
            "byte", "word", "dword", "qword",

            // Misc
            "extern", "public", "equ", "dup", "org",
        };
    }
}
