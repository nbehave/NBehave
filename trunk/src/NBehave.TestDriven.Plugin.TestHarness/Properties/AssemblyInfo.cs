using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using TestDriven.Framework;
using NBehave.TestDriven.Plugin;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("NBehave.TestDriven.Plugin.Tests")]
//[assembly: AssemblyDescription("")]
//[assembly: AssemblyTrademark("")]
//[assembly: AssemblyCulture("")]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("b661d510-596f-4fca-9a8d-b28ea5c52c0a")]

[assembly: CustomTestRunner(typeof(NBehaveStoryRunner))]
