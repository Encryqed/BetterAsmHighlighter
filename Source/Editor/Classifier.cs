using System;
using System.Collections.Generic;
using BetterAsmHighlighter.Core;
using BetterAsmHighlighter.Data;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace BetterAsmHighlighter.Editor
{
    internal sealed class Classifier : IClassifier
    {
        private readonly ITextBuffer Buffer;
        private readonly Lexer Lexer;
        private readonly Dictionary<TokenType, IClassificationType> ClassificationMap;

        private static readonly HashSet<string> DataDefDirectives = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "db", "dw", "dd", "dq", "byte", "word", "dword", "qword"
        };

        private HashSet<string> KnownGlobals = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private HashSet<string> KnownFunctions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private HashSet<string> KnownLabels = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private HashSet<string> KnownStructures = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private HashSet<int> StructBlockLines = new HashSet<int>();
        private int CachedVersion = -1;

        public Classifier(ITextBuffer Buffer, IClassificationTypeRegistryService Registry)
        {
            this.Buffer = Buffer;
            this.Buffer.Changed += OnBufferChanged;

            Lexer = new Lexer(Instructions.All, Registers.All, Directives.All);

            ClassificationMap = new Dictionary<TokenType, IClassificationType>
            {
                [TokenType.Comment]     = Registry.GetClassificationType(ClassificationTypes.COMMENT),
                [TokenType.Instruction] = Registry.GetClassificationType(ClassificationTypes.INSTRUCTION),
                [TokenType.Register]    = Registry.GetClassificationType(ClassificationTypes.REGISTER),
                [TokenType.Directive]   = Registry.GetClassificationType(ClassificationTypes.DIRECTIVE),
                [TokenType.Number]      = Registry.GetClassificationType(ClassificationTypes.NUMBER),
                [TokenType.String]      = Registry.GetClassificationType(ClassificationTypes.STRING),
                [TokenType.Label]       = Registry.GetClassificationType(ClassificationTypes.LABEL),
                [TokenType.Function]    = Registry.GetClassificationType(ClassificationTypes.FUNCTION),
                [TokenType.Structure]   = Registry.GetClassificationType(ClassificationTypes.STRUCTURE),
                [TokenType.Member]      = Registry.GetClassificationType(ClassificationTypes.MEMBER),
                [TokenType.Global]      = Registry.GetClassificationType(ClassificationTypes.GLOBAL),
                [TokenType.Operator]    = Registry.GetClassificationType(ClassificationTypes.OPERATOR),
            };
        }

        public event EventHandler<ClassificationChangedEventArgs>? ClassificationChanged;

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan Span)
        {
            List<ClassificationSpan> Result = new List<ClassificationSpan>();
            ITextSnapshot Snapshot = Span.Snapshot;
            CollectSymbols(Snapshot);

            int StartLine = Span.Start.GetContainingLine().LineNumber;
            int EndLine = Span.End.GetContainingLine().LineNumber;

            for (int i = StartLine; i <= EndLine; i++)
            {
                ITextSnapshotLine Line = Snapshot.GetLineFromLineNumber(i);
                string LineText = Line.GetText();
                int LineStart = Line.Start.Position;

                List<Token> Tokens = Lexer.TokenizeLine(LineText, LineStart);

                TokenType PrevType = TokenType.Unknown;

                foreach (Token Tok in Tokens)
                {
                    TokenType Type = Tok.Type;

                    if (Type == TokenType.Unknown)
                    {
                        if (KnownGlobals.Contains(Tok.Text))
                            Type = TokenType.Global;
                        else if (KnownFunctions.Contains(Tok.Text))
                            Type = TokenType.Function;
                        else if (KnownLabels.Contains(Tok.Text))
                            Type = TokenType.Label;
                        else if (KnownStructures.Contains(Tok.Text))
                            Type = TokenType.Structure;
                        else
                            continue;
                    }

                    PrevType = Type;

                    if (ClassificationMap.TryGetValue(Type, out IClassificationType? Classification) && Classification != null)
                    {
                        int SpanStart = Tok.Start;
                        int SpanLength = Tok.Length;

                        // Skip the leading '.' for struct member access
                        if (Type == TokenType.Member && Tok.Text.StartsWith("."))
                        {
                            SpanStart++;
                            SpanLength--;
                        }

                        SnapshotSpan TokenSpan = new SnapshotSpan(Snapshot, SpanStart, SpanLength);
                        Result.Add(new ClassificationSpan(TokenSpan, Classification));
                    }
                }
            }

            return Result;
        }

        private void CollectSymbols(ITextSnapshot Snapshot)
        {
            int Version = Snapshot.Version.VersionNumber;
            if (Version == CachedVersion)
                return;

            KnownGlobals = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            KnownFunctions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            KnownLabels = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            KnownStructures = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            StructBlockLines = new HashSet<int>();

            bool bInStructBlock = false;

            for (int i = 0; i < Snapshot.LineCount; i++)
            {
                List<Token> Tokens = Lexer.TokenizeLine(Snapshot.GetLineFromLineNumber(i).GetText(), 0);

                if (Tokens.Count < 1)
                    continue;

                // Name STRUCT -> start of struct block
                if (Tokens.Count >= 2
                    && Tokens[0].Type == TokenType.Structure
                    && Tokens[1].Type == TokenType.Directive
                    && Tokens[1].Text.Equals("struct", StringComparison.OrdinalIgnoreCase))
                {
                    KnownStructures.Add(Tokens[0].Text);
                    bInStructBlock = true;
                    continue;
                }

                // Name ENDS -> end of struct block
                if (Tokens.Count >= 2
                    && Tokens[0].Type == TokenType.Structure
                    && Tokens[1].Type == TokenType.Directive
                    && Tokens[1].Text.Equals("ends", StringComparison.OrdinalIgnoreCase))
                {
                    bInStructBlock = false;
                    continue;
                }

                // Lines inside struct body
                if (bInStructBlock)
                {
                    StructBlockLines.Add(i);
                    continue;
                }

                // EXTERN/EXTRN name:TYPE -> global
                if (Tokens.Count >= 2 && Tokens[0].Type == TokenType.Directive && Tokens[1].Type == TokenType.Global)
                {
                    KnownGlobals.Add(Tokens[1].Text);
                    continue;
                }

                // identifier: -> label definition
                if (Tokens[0].Type == TokenType.Label)
                {
                    string Name = Tokens[0].Text;

                    if (Name.EndsWith(":"))
                        Name = Name.Substring(0, Name.Length - 1);
                    if (Name.Length > 0)
                        KnownLabels.Add(Name);

                    continue;
                }

                // Name PROC/ENDP -> function
                if (Tokens.Count >= 2 && Tokens[0].Type == TokenType.Function && Tokens[1].Type == TokenType.Directive)
                {
                    KnownFunctions.Add(Tokens[0].Text);
                    continue;
                }

                // Name EQU ... -> constant (treat as global)
                if (Tokens.Count >= 2
                    && Tokens[0].Type == TokenType.Unknown
                    && Tokens[1].Type == TokenType.Directive
                    && Tokens[1].Text.Equals("equ", StringComparison.OrdinalIgnoreCase))
                {
                    KnownGlobals.Add(Tokens[0].Text);
                    continue;
                }

                // Name db/dw/dd/dq -> data variable (treat as global)
                if (Tokens.Count >= 2
                    && Tokens[0].Type == TokenType.Unknown
                    && Tokens[1].Type == TokenType.Directive
                    && DataDefDirectives.Contains(Tokens[1].Text))
                {
                    KnownGlobals.Add(Tokens[0].Text);
                    continue;
                }

                // Name StructType <> -> struct instance (treat as global)
                if (Tokens.Count >= 2
                    && Tokens[0].Type == TokenType.Unknown
                    && Tokens[1].Type == TokenType.Unknown
                    && KnownStructures.Contains(Tokens[1].Text))
                {
                    KnownGlobals.Add(Tokens[0].Text);
                }
            }

            CachedVersion = Version;
        }

        private void OnBufferChanged(object Sender, TextContentChangedEventArgs E)
        {
            foreach (ITextChange Change in E.Changes)
            {
                SnapshotSpan ChangedSpan = new SnapshotSpan(E.After, Change.NewSpan);
                ClassificationChanged?.Invoke(this, new ClassificationChangedEventArgs(ChangedSpan));
            }
        }
    }
}
