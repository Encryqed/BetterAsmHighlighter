using System;
using System.Collections.Generic;
using System.Windows.Media;
using BetterAsmHighlighter.Editor;

namespace BetterAsmHighlighter.Data
{
    internal struct ThemeColor
    {
        public Color Foreground;
        public bool bIsBold;

        public ThemeColor(byte R, byte G, byte B, bool bIsBold = false)
        {
            Foreground = Color.FromRgb(R, G, B);
            this.bIsBold = bIsBold;
        }
    }

    internal static class Themes
    {
        public const string DARK = "Dark";
        public const string MONOKAI = "Monokai";
        public const string SOLARIZED_DARK = "Solarized Dark";

        public static readonly string[] Names = { DARK, MONOKAI, SOLARIZED_DARK };

        // All classification type keys in stable order for serialization
        public static readonly string[] AllTypes =
        {
            ClassificationTypes.COMMENT,
            ClassificationTypes.INSTRUCTION,
            ClassificationTypes.REGISTER,
            ClassificationTypes.DIRECTIVE,
            ClassificationTypes.NUMBER,
            ClassificationTypes.STRING,
            ClassificationTypes.LABEL,
            ClassificationTypes.STRUCTURE,
            ClassificationTypes.MEMBER,
            ClassificationTypes.FUNCTION,
            ClassificationTypes.GLOBAL,
            ClassificationTypes.OPERATOR,
        };

        // Format: asm.comment=6A9955:0;asm.instruction=569CD6:1;...
        public static string Serialize(Dictionary<string, ThemeColor> Colors)
        {
            List<string> Parts = new List<string>();

            foreach (string Key in AllTypes)
            {
                if (!Colors.TryGetValue(Key, out ThemeColor TC))
                    continue;

                string Hex = TC.Foreground.R.ToString("X2")
                           + TC.Foreground.G.ToString("X2")
                           + TC.Foreground.B.ToString("X2");
                string Bold = TC.bIsBold ? "1" : "0";
                Parts.Add(Key + "=" + Hex + ":" + Bold);
            }

            return string.Join(";", Parts);
        }

        public static Dictionary<string, ThemeColor> Deserialize(string Data)
        {
            Dictionary<string, ThemeColor> Colors = new Dictionary<string, ThemeColor>();

            if (string.IsNullOrEmpty(Data))
                return Colors;

            string[] Entries = Data.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string Entry in Entries)
            {
                // asm.comment=6A9955:0
                int EqIndex = Entry.IndexOf('=');
                if (EqIndex < 0)
                    continue;

                string Key = Entry.Substring(0, EqIndex);
                string Value = Entry.Substring(EqIndex + 1);

                int ColonIndex = Value.IndexOf(':');
                if (ColonIndex < 0)
                    continue;

                string Hex = Value.Substring(0, ColonIndex);
                string BoldStr = Value.Substring(ColonIndex + 1);

                if (Hex.Length != 6)
                    continue;

                byte R = Convert.ToByte(Hex.Substring(0, 2), 16);
                byte G = Convert.ToByte(Hex.Substring(2, 2), 16);
                byte B = Convert.ToByte(Hex.Substring(4, 2), 16);
                bool bIsBold = BoldStr == "1";

                Colors[Key] = new ThemeColor(R, G, B, bIsBold);
            }

            return Colors;
        }

        public static readonly Dictionary<string, Dictionary<string, ThemeColor>> Palettes =
            new Dictionary<string, Dictionary<string, ThemeColor>>
            {
                {
                    DARK, new Dictionary<string, ThemeColor>
                    {
                        { ClassificationTypes.COMMENT,     new ThemeColor(0x6A, 0x99, 0x55) },
                        { ClassificationTypes.INSTRUCTION, new ThemeColor(0x56, 0x9C, 0xD6, true) },
                        { ClassificationTypes.REGISTER,    new ThemeColor(0x4E, 0xC9, 0xB0) },
                        { ClassificationTypes.DIRECTIVE,   new ThemeColor(0xC5, 0x86, 0xC0) },
                        { ClassificationTypes.NUMBER,      new ThemeColor(0xB5, 0xCE, 0xA8) },
                        { ClassificationTypes.STRING,      new ThemeColor(0xCE, 0x91, 0x78) },
                        { ClassificationTypes.LABEL,       new ThemeColor(0xD7, 0xBA, 0x7D) },
                        { ClassificationTypes.STRUCTURE,   new ThemeColor(0xD1, 0x9A, 0x66) },
                        { ClassificationTypes.MEMBER,      new ThemeColor(0xC8, 0xC8, 0xC8) },
                        { ClassificationTypes.FUNCTION,    new ThemeColor(0xDC, 0xDC, 0xAA, true) },
                        { ClassificationTypes.GLOBAL,      new ThemeColor(0x9C, 0xDC, 0xFE) },
                        { ClassificationTypes.OPERATOR,    new ThemeColor(0xD4, 0xD4, 0xD4) },
                    }
                },
                {
                    MONOKAI, new Dictionary<string, ThemeColor>
                    {
                        { ClassificationTypes.COMMENT,     new ThemeColor(0x75, 0x71, 0x5E) },
                        { ClassificationTypes.INSTRUCTION, new ThemeColor(0xF9, 0x26, 0x72, true) },
                        { ClassificationTypes.REGISTER,    new ThemeColor(0x66, 0xD9, 0xEF) },
                        { ClassificationTypes.DIRECTIVE,   new ThemeColor(0xAE, 0x81, 0xFF) },
                        { ClassificationTypes.NUMBER,      new ThemeColor(0xAE, 0x81, 0xFF) },
                        { ClassificationTypes.STRING,      new ThemeColor(0xE6, 0xDB, 0x74) },
                        { ClassificationTypes.LABEL,       new ThemeColor(0xA6, 0xE2, 0x2E) },
                        { ClassificationTypes.STRUCTURE,   new ThemeColor(0xFD, 0x97, 0x1F) },
                        { ClassificationTypes.MEMBER,      new ThemeColor(0xF8, 0xF8, 0xF2) },
                        { ClassificationTypes.FUNCTION,    new ThemeColor(0xA6, 0xE2, 0x2E, true) },
                        { ClassificationTypes.GLOBAL,      new ThemeColor(0x66, 0xD9, 0xEF) },
                        { ClassificationTypes.OPERATOR,    new ThemeColor(0xF8, 0xF8, 0xF2) },
                    }
                },
                {
                    SOLARIZED_DARK, new Dictionary<string, ThemeColor>
                    {
                        { ClassificationTypes.COMMENT,     new ThemeColor(0x58, 0x6E, 0x75) },
                        { ClassificationTypes.INSTRUCTION, new ThemeColor(0x26, 0x8B, 0xD2, true) },
                        { ClassificationTypes.REGISTER,    new ThemeColor(0x2A, 0xA1, 0x98) },
                        { ClassificationTypes.DIRECTIVE,   new ThemeColor(0xD3, 0x36, 0x82) },
                        { ClassificationTypes.NUMBER,      new ThemeColor(0xCB, 0x4B, 0x16) },
                        { ClassificationTypes.STRING,      new ThemeColor(0x2A, 0xA1, 0x98) },
                        { ClassificationTypes.LABEL,       new ThemeColor(0xB5, 0x89, 0x00) },
                        { ClassificationTypes.STRUCTURE,   new ThemeColor(0xCB, 0x4B, 0x16) },
                        { ClassificationTypes.MEMBER,      new ThemeColor(0x83, 0x94, 0x96) },
                        { ClassificationTypes.FUNCTION,    new ThemeColor(0x85, 0x99, 0x00, true) },
                        { ClassificationTypes.GLOBAL,      new ThemeColor(0x6C, 0x71, 0xC4) },
                        { ClassificationTypes.OPERATOR,    new ThemeColor(0x83, 0x94, 0x96) },
                    }
                },
            };
    }
}
