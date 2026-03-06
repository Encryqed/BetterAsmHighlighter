using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Media;
using BetterAsmHighlighter.Data;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Formatting;
using Microsoft.VisualStudio.Utilities;

namespace BetterAsmHighlighter.Editor
{
    [Export(typeof(ThemeManager))]
    internal sealed class ThemeManager
    {
        [Import]
        internal IClassificationFormatMapService FormatMapService = null!;

        [Import]
        internal IClassificationTypeRegistryService TypeRegistry = null!;

        public void ApplyTheme(string ThemeName)
        {
            if (!Themes.Palettes.TryGetValue(ThemeName, out Dictionary<string, ThemeColor> Palette))
                return;

            ApplyColors(Palette);
        }

        public void ApplyColors(Dictionary<string, ThemeColor> Colors)
        {
            IClassificationFormatMap FormatMap = FormatMapService.GetClassificationFormatMap("text");

            FormatMap.BeginBatchUpdate();

            try
            {
                foreach (KeyValuePair<string, ThemeColor> Entry in Colors)
                {
                    IClassificationType Type = TypeRegistry.GetClassificationType(Entry.Key);

                    if (Type == null)
                        continue;

                    TextFormattingRunProperties Properties = FormatMap.GetTextProperties(Type);
                    Properties = Properties.SetForeground(Entry.Value.Foreground);
                    Properties = Properties.SetBold(Entry.Value.bIsBold);

                    FormatMap.SetTextProperties(Type, Properties);
                }
            }
            finally
            {
                FormatMap.EndBatchUpdate();
            }
        }
    }
}
