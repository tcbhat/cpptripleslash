using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using System.Reflection;

namespace PopDragos.CppDoxyComplete
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [Guid(GuidList.guidCppDoxyCompletePkgString)]
    public sealed class CppDoxyCompletePackage : Package
    {
		/// <summary>
		/// Default constructor of the package.
		/// Inside this method you can place any initialization code that does not require 
		/// any Visual Studio service because at this point the package object is created but 
		/// not sited yet inside Visual Studio environment. The place to do all the other 
		/// initialization is the Initialize method.
		/// </summary>
		public CppDoxyCompletePackage()
		{
			AppDomain currentDomain = AppDomain.CurrentDomain;
			currentDomain.AssemblyResolve += new ResolveEventHandler(LoadFromSameFolder);
			//base();
		}

		static Assembly LoadFromSameFolder(object sender, ResolveEventArgs args)
		{
			if (args.Name.StartsWith("VCCodeModel"))
			{
				//string folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
				//string assemblyPath = Path.Combine(folderPath, new AssemblyName(args.Name).Name + ".dll");
				//if (!File.Exists(assemblyPath)) return null;
				var name = args.Name;
				Assembly assembly = null ;
				try
				{
					assembly = Assembly.Load(name);
					if (assembly == null)
					{
						name.Replace("Version=12", "Version=14");
						assembly = Assembly.Load(name);
					}

					if (assembly == null)
					{
						name.Replace("Version=14", "Version=15");
						assembly = Assembly.Load(name);
					}


					return assembly;
				}
				catch
				{
					return null;
				}
				
				return assembly;
			}
			else
			{
				return null;
			}
		}



		/////////////////////////////////////////////////////////////////////////////
		// Overridden Package Implementation
		#region Package Members

		/// <summary>
		/// Initialization of the package; this method is called right after the package is sited, so this is the place
		/// where you can put all the initialization code that rely on services provided by VisualStudio.
		/// </summary>
		protected override void Initialize()
        {

			base.Initialize();

        }
		#endregion

	}
}
