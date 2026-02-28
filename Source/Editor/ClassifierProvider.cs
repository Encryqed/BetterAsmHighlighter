using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace BetterAsmHighlighter.Editor
{
    [Export(typeof(IClassifierProvider))]
    [ContentType(AsmContentTypeDefinition.CONTENT_TYPE_NAME)]
    internal sealed class ClassifierProvider : IClassifierProvider
    {
        [Import]
        internal IClassificationTypeRegistryService ClassificationRegistry { get; set; } = null!;

        public IClassifier? GetClassifier(ITextBuffer Buffer)
        {
            return Buffer.Properties.GetOrCreateSingletonProperty(() => new Classifier(Buffer, ClassificationRegistry));
        }
    }
}
