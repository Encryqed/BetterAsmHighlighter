using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace BetterAsmHighlighter.Editor
{
    internal static class ClassificationTypes
    {
        public const string COMMENT     = "asm.comment";
        public const string INSTRUCTION = "asm.instruction";
        public const string REGISTER    = "asm.register";
        public const string DIRECTIVE   = "asm.directive";
        public const string NUMBER      = "asm.number";
        public const string STRING      = "asm.string";
        public const string LABEL       = "asm.label";
        public const string FUNCTION    = "asm.function";
        public const string GLOBAL      = "asm.global";
        public const string OPERATOR    = "asm.operator";

        [Export] [Name(COMMENT)]     [BaseDefinition("comment")]    internal static ClassificationTypeDefinition? CommentType;
        [Export] [Name(INSTRUCTION)] [BaseDefinition("keyword")]    internal static ClassificationTypeDefinition? InstructionType;
        [Export] [Name(REGISTER)]    [BaseDefinition("identifier")] internal static ClassificationTypeDefinition? RegisterType;
        [Export] [Name(DIRECTIVE)]   [BaseDefinition("keyword")]    internal static ClassificationTypeDefinition? DirectiveType;
        [Export] [Name(NUMBER)]      [BaseDefinition("number")]     internal static ClassificationTypeDefinition? NumberType;
        [Export] [Name(STRING)]      [BaseDefinition("string")]     internal static ClassificationTypeDefinition? StringType;
        [Export] [Name(LABEL)]       [BaseDefinition("identifier")] internal static ClassificationTypeDefinition? LabelType;
        [Export] [Name(FUNCTION)]    [BaseDefinition("identifier")] internal static ClassificationTypeDefinition? FunctionType;
        [Export] [Name(GLOBAL)]      [BaseDefinition("identifier")] internal static ClassificationTypeDefinition? GlobalType;
        [Export] [Name(OPERATOR)]    [BaseDefinition("operator")]   internal static ClassificationTypeDefinition? OperatorType;
    }
}
