using System.Reflection;
using System.Runtime.InteropServices;
using MelonLoader;

// General Information about an assembly is controlled through the following set
// of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("AdofaiTweaks")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("AdofaiTweaks")]
[assembly: AssemblyCopyright("Copyright ©  2021")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible to
// COM components. If you need to access a type in this assembly from COM, set
// the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("285d8437-6a8e-4abe-ab99-9936852430f7")]

// Version information for an assembly consists of the following four values:
//
// Major Version Minor Version Build Number Revision
//
// You can specify all the values or you can default the Build and Revision
// Numbers by using the '*' as shown below: [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

// MelonLoader Mod Informations
[assembly: MelonInfo(typeof(AdofaiTweaks.AdofaiTweaks), "AdofaiTweaks", "2.4.0", "PizzaLovers007 & CrackThrough")]
[assembly: MelonGame("7th Beat Games", "A Dance of Fire and Ice")]

// Do not patch everything but allow me to safely choose which patch to patch
[assembly: HarmonyDontPatchAll]