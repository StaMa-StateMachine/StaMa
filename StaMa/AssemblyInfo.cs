#region AssemblyInfo.cs file
//
// StaMa state machine controller library, http://stama.codeplex.com/
//
// Copyright (c) 2005-2014, Roland Schneider. All rights reserved.
//
#endregion

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
//using System.Security.Permissions;
using System.Runtime.InteropServices;
#if MF_FRAMEWORK
using Microsoft.SPOT;
#endif


//
// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
//

//[assembly: SecurityPermissionAttribute(SecurityAction.RequestMinimum, Flags = SecurityPermissionFlag.Execution)]
//[assembly: SecurityPermissionAttribute(SecurityAction.RequestMinimum, Flags = SecurityPermissionFlag.NoFlags)]

[assembly: AssemblyTitle("StaMa")]
[assembly: AssemblyDescription("StaMa state machine controller library")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("http://stama.codeplex.com/")]
[assembly: AssemblyProduct("StaMa")]
[assembly: AssemblyCopyright("Copyright (c) 2005-2015, Roland Schneider. All rights reserved.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]		

//
// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers 
// by using the '*' as shown below:

[assembly: AssemblyVersion("2.3.0.2")]

//
// In order to sign your assembly you must specify a key to use. Refer to the 
// Microsoft .NET Framework documentation for more information on assembly signing.
//
// Use the attributes below to control which key is used for signing. 
//
// Notes: 
//   (*) If no key is specified, the assembly is not signed.
//   (*) KeyName refers to a key that has been installed in the Crypto Service
//       Provider (CSP) on your machine. KeyFile refers to a file which contains
//       a key.
//   (*) If the KeyFile and the KeyName values are both specified, the 
//       following processing occurs:
//       (1) If the KeyName can be found in the CSP, that key is used.
//       (2) If the KeyName does not exist and the KeyFile does exist, the key 
//           in the KeyFile is installed into the CSP and used.
//   (*) In order to create a KeyFile, you can use the sn.exe (Strong Name) utility.
//       When specifying the KeyFile, the location of the KeyFile should be
//       relative to the project output directory which is
//       %Project Directory%\obj\<configuration>. For example, if your KeyFile is
//       located in the project directory, you would specify the AssemblyKeyFile 
//       attribute as [assembly: AssemblyKeyFile("..\\..\\mykey.snk")]
//   (*) Delay Signing is an advanced option - see the Microsoft .NET Framework
//       documentation for more information on this.
//
[assembly: AssemblyDelaySign(false)]
//[assembly: AssemblyKeyFile("..\\..\\StaMa.snk")]
[assembly: AssemblyKeyName("")]

#if !MF_FRAMEWORK
[assembly: CLSCompliant(true)]
#endif

#if STAMA_COMPATIBLE21
[assembly: ComVisible(false)]
#endif

namespace StaMa
{
    /// <summary>
    /// <para>
    /// Contains all classes, interfaces and delegate types to implement state machines.
    /// </para>
    /// <para>
    /// See class <see cref="StateMachineTemplate"/> to create the structure of a state machine.
    /// </para>
    /// <para>
    /// Use method <see cref="StateMachineTemplate.CreateStateMachine()"/> or <see cref="StateMachineTemplate.CreateStateMachine(Object)"/> to create StateMachine instances.
    /// </para>
    /// <para>
    /// Send events to a state machine to trigger transitions through method <see cref="StateMachine.SendTriggerEvent(Object)"/> or <see cref="StateMachine.SendTriggerEvent(Object,EventArgs)"/>.
    /// </para>
    /// </summary>
#if !MF_FRAMEWORK
    [System.Runtime.CompilerServices.CompilerGenerated]
#endif
    class NamespaceDoc
    {
    }
}
