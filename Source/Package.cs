using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using BetterAsmHighlighter.Data;
using BetterAsmHighlighter.Editor;
using BetterAsmHighlighter.Options;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace BetterAsmHighlighter
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid("f47ac10b-58cc-4372-a567-0e02b2c3d479")]
    [ProvideOptionPage(typeof(ThemeOptionsPage), "BetterAsmHighlighter", "Theme", 0, 0, true)]
    internal sealed class AsmHighlighterPackage : AsyncPackage
    {
        protected override async Task InitializeAsync(CancellationToken CancellationToken, IProgress<ServiceProgressData> Progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(CancellationToken);

            ThemeOptionsPage Options = (ThemeOptionsPage)GetDialogPage(typeof(ThemeOptionsPage));
            if (Options == null)
                return;

            IComponentModel ComponentModel = (IComponentModel)await GetServiceAsync(typeof(SComponentModel));
            if (ComponentModel == null)
                return;

            ThemeManager Manager = ComponentModel.DefaultExportProvider.GetExportedValue<ThemeManager>();

            // If user has custom colors saved, apply those; otherwise apply theme preset
            if (!string.IsNullOrEmpty(Options.SerializedColors))
            {
                Dictionary<string, ThemeColor> Colors = Themes.Deserialize(Options.SerializedColors);

                if (Colors.Count > 0)
                {
                    Manager.ApplyColors(Colors);
                    return;
                }
            }

            if (Options.Theme != Themes.DARK)
            {
                Manager.ApplyTheme(Options.Theme);
            }
        }
    }
}
