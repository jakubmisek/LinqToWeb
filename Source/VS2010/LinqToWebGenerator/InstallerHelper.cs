using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;

namespace YUICustomTool
{
    [RunInstaller(true)]
    public partial class InstallerHelper : Installer
    {
        public InstallerHelper()
        {
            //InitializeComponent();
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Commit(System.Collections.IDictionary savedState)
        {
            base.Commit(savedState);

            // Get the location of regasm
            string regasmPath = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory() + @"regasm.exe";
            // Get the location of our DLL
            string componentPath = typeof(InstallerHelper).Assembly.Location;
            // Execute regasm
            System.Diagnostics.Process.Start(regasmPath, "/codebase \"" + componentPath + "\"");
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Install(System.Collections.IDictionary stateSaver)
        {
            base.Install(stateSaver);
        }
    }
}