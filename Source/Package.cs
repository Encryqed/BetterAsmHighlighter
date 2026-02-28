using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace BetterAsmHighlighter
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid("f47ac10b-58cc-4372-a567-0e02b2c3d479")]
    internal sealed class AsmHighlighterPackage : AsyncPackage
    {
        protected override async Task InitializeAsync(CancellationToken CancellationToken, IProgress<ServiceProgressData> Progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(CancellationToken);
        }
    }
}
