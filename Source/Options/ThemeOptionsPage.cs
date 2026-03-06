using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using BetterAsmHighlighter.Data;
using BetterAsmHighlighter.Editor;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;

namespace BetterAsmHighlighter.Options
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [Guid("b2c3d4e5-f6a7-8901-bcde-f12345678901")]
    internal class ThemeOptionsPage : UIElementDialogPage
    {
        public string Theme { get; set; } = Themes.DARK;
        public string SerializedColors { get; set; } = "";

        private ColorSettingsControl? Control;

        protected override UIElement Child
        {
            get
            {
                if (Control == null)
                {
                    Control = new ColorSettingsControl();
                }

                return Control;
            }
        }

        protected override void OnActivate(System.ComponentModel.CancelEventArgs E)
        {
            base.OnActivate(E);

            // Child getter guarantees Control is initialized
            Dictionary<string, ThemeColor> Colors = Themes.Deserialize(SerializedColors);
            Control!.LoadFromSettings(Theme, Colors);
        }

        protected override void OnApply(PageApplyEventArgs E)
        {
            Dictionary<string, ThemeColor> Colors = Control!.GetColors();
            Theme = Control.GetThemeName();
            SerializedColors = Themes.Serialize(Colors);

            base.OnApply(E);

            IComponentModel ComponentModel = (IComponentModel)GetService(typeof(SComponentModel));
            if (ComponentModel == null)
                return;

            ThemeManager Manager = ComponentModel.DefaultExportProvider.GetExportedValue<ThemeManager>();
            Manager.ApplyColors(Colors);
        }
    }
}
