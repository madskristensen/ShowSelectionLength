using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;

namespace ShowSelectionLength
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
    [Guid("0631cfcf-d450-4bc4-be4e-bfad0101f9c2")]
    public sealed class ShowSelectionLengthPackage : AsyncPackage
    {
    }
}
