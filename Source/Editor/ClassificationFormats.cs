using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace BetterAsmHighlighter.Editor
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationTypes.COMMENT)]
    [Name("BetterAsmHighlighter.Comment")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class CommentFormat : ClassificationFormatDefinition
    {
        public CommentFormat()
        {
            DisplayName = "ASM - Comment";
            ForegroundColor = Color.FromRgb(0x6A, 0x99, 0x55);
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationTypes.INSTRUCTION)]
    [Name("BetterAsmHighlighter.Instruction")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class InstructionFormat : ClassificationFormatDefinition
    {
        public InstructionFormat()
        {
            DisplayName = "ASM - Instruction";
            ForegroundColor = Color.FromRgb(0x56, 0x9C, 0xD6);
            IsBold = true;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationTypes.REGISTER)]
    [Name("BetterAsmHighlighter.Register")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class RegisterFormat : ClassificationFormatDefinition
    {
        public RegisterFormat()
        {
            DisplayName = "ASM - Register";
            ForegroundColor = Color.FromRgb(0x4E, 0xC9, 0xB0);
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationTypes.DIRECTIVE)]
    [Name("BetterAsmHighlighter.Directive")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class DirectiveFormat : ClassificationFormatDefinition
    {
        public DirectiveFormat()
        {
            DisplayName = "ASM - Directive";
            ForegroundColor = Color.FromRgb(0xC5, 0x86, 0xC0);
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationTypes.NUMBER)]
    [Name("BetterAsmHighlighter.Number")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class NumberFormat : ClassificationFormatDefinition
    {
        public NumberFormat()
        {
            DisplayName = "ASM - Number";
            ForegroundColor = Color.FromRgb(0xB5, 0xCE, 0xA8);
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationTypes.STRING)]
    [Name("BetterAsmHighlighter.String")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class StringFormat : ClassificationFormatDefinition
    {
        public StringFormat()
        {
            DisplayName = "ASM - String";
            ForegroundColor = Color.FromRgb(0xCE, 0x91, 0x78);
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationTypes.LABEL)]
    [Name("BetterAsmHighlighter.Label")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class LabelFormat : ClassificationFormatDefinition
    {
        public LabelFormat()
        {
            DisplayName = "ASM - Label";
            ForegroundColor = Color.FromRgb(0xDC, 0xDC, 0xAA);
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationTypes.OPERATOR)]
    [Name("BetterAsmHighlighter.Operator")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class OperatorFormat : ClassificationFormatDefinition
    {
        public OperatorFormat()
        {
            DisplayName = "ASM - Operator";
            ForegroundColor = Color.FromRgb(0xD4, 0xD4, 0xD4);
        }
    }
}
