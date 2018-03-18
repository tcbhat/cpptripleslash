namespace CppTripleSlash
{
    using Microsoft.VisualStudio.Shell;
    using System;
    using System.Runtime.InteropServices;

    [PackageRegistration(UseManagedResourcesOnly = true)]
    [Guid(GuidList.guidCppTripleSlashPkgString)]
    public sealed class CppTripleSlashPackage : Package
    {
        protected override void Initialize()
        {
            base.Initialize();
        }
    }
}