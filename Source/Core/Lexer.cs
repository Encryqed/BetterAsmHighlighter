using System;
using System.Collections.Generic;

namespace BetterAsmHighlighter.Core
{
    public sealed class Lexer
    {
        private readonly HashSet<string> Instructions;
        private readonly HashSet<string> Registers;
        private readonly HashSet<string> Directives;

        private static readonly HashSet<string> NameDefDirectives = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "proc", "endp"
        };

        private static readonly HashSet<string> StructDefDirectives = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "struct", "ends"
        };

        public Lexer(HashSet<string> Instructions, HashSet<string> Registers, HashSet<string> Directives)
        {
            this.Instructions = Instructions;
            this.Registers = Registers;
            this.Directives = Directives;
        }

        public List<Token> TokenizeLine(string Line, int Offset)
        {
            List<Token> Tokens = new List<Token>();
            int Pos = 0;

            while (Pos < Line.Length)
            {
                if (char.IsWhiteSpace(Line[Pos]))           { Pos++; continue; }
                if (Line[Pos] == ';')                       { ReadComment(Tokens, Line, Offset, Pos); break; }
                if (Line[Pos] == '"' || Line[Pos] == '\'')  { ReadString(Tokens, Line, Offset, ref Pos); continue; }
                if (Line[Pos] == '$')                       { Tokens.Add(new Token(TokenType.Number, Offset + Pos, 1, "$")); Pos++; continue; }
                if (IsOperator(Line[Pos]))                  { ReadOperator(Tokens, Line, Offset, ref Pos); continue; }
                if (char.IsDigit(Line[Pos]))                { ReadNumber(Tokens, Line, Offset, ref Pos); continue; }
                if (IsDotDirectiveStart(Line, Pos))         { ReadDotDirective(Tokens, Line, Offset, ref Pos); continue; }
                if (IsIdentifierStart(Line[Pos]))           { ReadIdentifier(Tokens, Line, Offset, ref Pos); continue; }

                Pos++;
            }

            PostProcess(Tokens);
            return Tokens;
        }

        private static void ReadComment(List<Token> Tokens, string Line, int Offset, int Pos)
        {
            Tokens.Add(new Token(TokenType.Comment, Offset + Pos, Line.Length - Pos, Line.Substring(Pos)));
        }

        private static void ReadString(List<Token> Tokens, string Line, int Offset, ref int Pos)
        {
            int Start = Pos;
            char Quote = Line[Pos];
            Pos++;

            while (Pos < Line.Length && Line[Pos] != Quote)
                Pos++;

            if (Pos < Line.Length)
                Pos++;

            Tokens.Add(new Token(TokenType.String, Offset + Start, Pos - Start, Line.Substring(Start, Pos - Start)));
        }

        private static void ReadOperator(List<Token> Tokens, string Line, int Offset, ref int Pos)
        {
            Tokens.Add(new Token(TokenType.Operator, Offset + Pos, 1, Line[Pos].ToString()));
            Pos++;
        }

        private void ReadNumber(List<Token> Tokens, string Line, int Offset, ref int Pos)
        {
            int Start = Pos;
            AdvanceNumber(Line, ref Pos);
            Tokens.Add(new Token(TokenType.Number, Offset + Start, Pos - Start, Line.Substring(Start, Pos - Start)));
        }

        private void ReadDotDirective(List<Token> Tokens, string Line, int Offset, ref int Pos)
        {
            int Start = Pos;
            Pos++;

            while (Pos < Line.Length && IsIdentifierChar(Line[Pos])) Pos++;

            string Word = Line.Substring(Start, Pos - Start);
            Tokens.Add(new Token(ClassifyWord(Word), Offset + Start, Pos - Start, Word));
        }

        private void ReadIdentifier(List<Token> Tokens, string Line, int Offset, ref int Pos)
        {
            int Start = Pos;
            while (Pos < Line.Length && IsIdentifierChar(Line[Pos]))
                Pos++;

            string Word = Line.Substring(Start, Pos - Start);

            // Segment override (register followed by ':'), e.g. gs:[rcx]
            if (Pos < Line.Length && Line[Pos] == ':' && Registers.Contains(Word))
            {
                Tokens.Add(new Token(TokenType.Register, Offset + Start, Pos - Start, Word));
                Tokens.Add(new Token(TokenType.Operator, Offset + Pos, 1, ":"));
                Pos++;
                return;
            }

            // EXTERN/EXTRN name -> global declaration
            if (IsExternContext(Tokens))
            {
                Tokens.Add(new Token(TokenType.Global, Offset + Start, Pos - Start, Word));

                if (Pos < Line.Length && Line[Pos] == ':')
                {
                    Tokens.Add(new Token(TokenType.Operator, Offset + Pos, 1, ":"));
                    Pos++;
                }

                return;
            }

            // Label definition (identifier followed by ':')
            if (Pos < Line.Length && Line[Pos] == ':')
            {
                Pos++;
                Tokens.Add(new Token(TokenType.Label, Offset + Start, Pos - Start, Word + ":"));
                return;
            }

            Tokens.Add(new Token(ClassifyWord(Word), Offset + Start, Pos - Start, Word));
        }

        private TokenType ClassifyWord(string Word)
        {
            if (Instructions.Contains(Word))
                return TokenType.Instruction;
            if (Registers.Contains(Word))
                return TokenType.Register;
            if (Directives.Contains(Word))
                return TokenType.Directive;

            return TokenType.Unknown;
        }

        private static bool IsExternContext(List<Token> Tokens)
        {
            bool bHasExtern = false;

            for (int i = 0; i < Tokens.Count; i++)
            {
                if (Tokens[i].Type == TokenType.Global)
                    return false;

                if (Tokens[i].Type == TokenType.Directive)
                {
                    string Text = Tokens[i].Text;
                    if (Text.Equals("extern", StringComparison.OrdinalIgnoreCase) || Text.Equals("extrn", StringComparison.OrdinalIgnoreCase))
                        bHasExtern = true;
                }
            }

            return bHasExtern;
        }

        private static void PostProcess(List<Token> Tokens)
        {
            if (Tokens.Count < 2)
                return;

            int First = -1;
            int Second = -1;

            for (int i = 0; i < Tokens.Count; i++)
            {
                if (Tokens[i].Type == TokenType.Comment) break;
                if (First == -1) { First = i; }
                else if (Second == -1) { Second = i; break; }
            }

            if (First == -1 || Second == -1)
                return;

            // Name PROC/ENDP -> name is a function
            if (Tokens[First].Type == TokenType.Unknown && Tokens[Second].Type == TokenType.Directive && NameDefDirectives.Contains(Tokens[Second].Text))
            {
                Tokens[First] = new Token(TokenType.Function, Tokens[First].Start, Tokens[First].Length, Tokens[First].Text);
            }

            // Name STRUCT/ENDS -> name is a structure
            if (Tokens[First].Type == TokenType.Unknown && Tokens[Second].Type == TokenType.Directive && StructDefDirectives.Contains(Tokens[Second].Text))
            {
                Tokens[First] = new Token(TokenType.Structure, Tokens[First].Start, Tokens[First].Length, Tokens[First].Text);
            }
        }

        private static bool IsDotDirectiveStart(string Line, int Pos)
        {
            return Line[Pos] == '.' && Pos + 1 < Line.Length && char.IsLetter(Line[Pos + 1]);
        }

        private static bool IsOperator(char C)
        {
            return C == '+' || C == '-' || C == '*' || C == '/' ||
                   C == '[' || C == ']' || C == '(' || C == ')' ||
                   C == '<' || C == '>' || C == ':' || C == ',';
        }

        private static bool IsIdentifierStart(char C)
        {
            return char.IsLetter(C) || C == '_' || C == '@';
        }

        private static bool IsIdentifierChar(char C)
        {
            return char.IsLetterOrDigit(C) || C == '_' || C == '@' || C == '$' || C == '?';
        }

        private static void AdvanceNumber(string Line, ref int Pos)
        {
            // 0x hex prefix
            if (Pos + 1 < Line.Length && Line[Pos] == '0' && (Line[Pos + 1] == 'x' || Line[Pos + 1] == 'X'))
            {
                Pos += 2;
                while (Pos < Line.Length && IsHexDigit(Line[Pos])) Pos++;
                return;
            }

            // 0b binary prefix
            if (Pos + 1 < Line.Length && Line[Pos] == '0' && (Line[Pos + 1] == 'b' || Line[Pos + 1] == 'B'))
            {
                Pos += 2;
                while (Pos < Line.Length && (Line[Pos] == '0' || Line[Pos] == '1')) Pos++;
                return;
            }

            // Decimal or MASM hex (0FFh)
            while (Pos < Line.Length && IsHexDigit(Line[Pos])) Pos++;

            if (Pos < Line.Length && (Line[Pos] == 'h' || Line[Pos] == 'H')) Pos++;
        }

        private static bool IsHexDigit(char C)
        {
            return (C >= '0' && C <= '9') || (C >= 'a' && C <= 'f') || (C >= 'A' && C <= 'F');
        }
    }
}
