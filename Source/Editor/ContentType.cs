using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;

namespace BetterAsmHighlighter.Editor
{
    internal static class AsmContentTypeDefinition
    {
        public const string CONTENT_TYPE_NAME = "asm";

        [Export]
        [Name(CONTENT_TYPE_NAME)]
        [BaseDefinition("code")]
        internal static ContentTypeDefinition? AsmContentType;

        [Export]
        [FileExtension(".asm")]
        [ContentType(CONTENT_TYPE_NAME)]
        internal static FileExtensionToContentTypeDefinition? AsmFileExtension;

        [Export]
        [FileExtension(".inc")]
        [ContentType(CONTENT_TYPE_NAME)]
        internal static FileExtensionToContentTypeDefinition? IncFileExtension;

        [Export]
        [FileExtension(".masm")]
        [ContentType(CONTENT_TYPE_NAME)]
        internal static FileExtensionToContentTypeDefinition? MasmFileExtension;
    }
}
