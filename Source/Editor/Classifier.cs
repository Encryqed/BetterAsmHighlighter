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

        private HashSet<string> KnownGlobals = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private HashSet<string> KnownFunctions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private HashSet<string> KnownLabels = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
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
                [TokenType.Label]       = Registry.GetClassificationType(ClassificationTypes.LABEL),
                [TokenType.Function]    = Registry.GetClassificationType(ClassificationTypes.FUNCTION),
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
                        else
                            continue;
                    }

                    if (ClassificationMap.TryGetValue(Type, out IClassificationType? Classification) && Classification != null)
                    {
                        SnapshotSpan TokenSpan = new SnapshotSpan(Snapshot, Tok.Start, Tok.Length);
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

            for (int i = 0; i < Snapshot.LineCount; i++)
            {
                List<Token> Tokens = Lexer.TokenizeLine(Snapshot.GetLineFromLineNumber(i).GetText(), 0);

                if (Tokens.Count < 1)
                    continue;

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
