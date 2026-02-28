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
                [TokenType.Operator]    = Registry.GetClassificationType(ClassificationTypes.OPERATOR),
            };
        }

        public event EventHandler<ClassificationChangedEventArgs>? ClassificationChanged;

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan Span)
        {
            List<ClassificationSpan> Result = new List<ClassificationSpan>();
            ITextSnapshot Snapshot = Span.Snapshot;

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
                    if (Tok.Type == TokenType.Unknown)
                        continue;

                    if (ClassificationMap.TryGetValue(Tok.Type, out IClassificationType? Classification)
                        && Classification != null)
                    {
                        SnapshotSpan TokenSpan = new SnapshotSpan(Snapshot, Tok.Start, Tok.Length);
                        Result.Add(new ClassificationSpan(TokenSpan, Classification));
                    }
                }
            }

            return Result;
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
