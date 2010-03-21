

To compilie the project without errors, you have to register Microsoft.VisualStudio.Shell.Interop as a COM object:

1.	Start the Visual Studio .NET Command Prompt
2.  Change the directorz to "C:\Windows\Microsoft.NET\Framework\v2.0.50727" (even on the x64 system)
3.	Register the assembly with regasm /tlb, you should see following output and huge warning messages then

C:\Windows\Microsoft.NET\Framework\v2.0.50727>regasm /tlb "C:\Program Files (x86
)\Microsoft Visual Studio 2008 SDK\VisualStudioIntegration\Common\Assemblies\Mic
rosoft.VisualStudio.Shell.Interop.dll"
Microsoft (R) .NET Framework Assembly Registration Utility 2.0.50727.4927
Copyright (C) Microsoft Corporation 1998-2004.  All rights reserved.

Types registered successfully