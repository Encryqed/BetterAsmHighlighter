namespace BetterAsmHighlighter.Core
{
    public enum TokenType
    {
        Comment,
        Instruction,
        Register,
        Directive,
        Number,
        Label,
        Function,
        Global,
        Operator,
        Unknown
    }

    public struct Token
    {
        public TokenType Type;
        public int Start;
        public int Length;
        public string Text;

        public Token(TokenType Type, int Start, int Length, string Text)
        {
            this.Type = Type;
            this.Start = Start;
            this.Length = Length;
            this.Text = Text;
        }
    }
}
