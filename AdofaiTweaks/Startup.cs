using System;
using System.IO;
using UnityModManagerNet;

namespace AdofaiTweaks;

/// <summary>
/// Entry point for AdofaiTweaks.
/// </summary>
internal static class Startup
{
    /// <summary>
    /// Entry point for AdofaiTweaks.
    /// </summary>
    /// <param name="modEntry">UMM's mod entry for AdofaiTweaks.</param>
    internal static void Load(UnityModManager.ModEntry modEntry) {
        LoadAssembly("Mods/AdofaiTweaks/AdofaiTweaks.Strings.dll");
        LoadAssembly("Mods/AdofaiTweaks/AdofaiTweaks.Translation.dll");
        LoadAssembly("Mods/AdofaiTweaks/LiteDB.dll");
        LoadAssembly("Mods/AdofaiTweaks/System.Buffers.dll");
        LoadAssembly("Mods/AdofaiTweaks/IndexRange.dll");

        if (TryLoadAssembly("A Dance of Fire and Ice_Data/Managed/SkyHook.Unity.dll")) {
            LoadAssembly("Mods/AdofaiTweaks/AdofaiTweaks.Compat.AsyncSkyHook.dll");
            modEntry.Logger.Log("Async assembly: SkyHook");
        } else if (TryLoadAssembly("A Dance of Fire and Ice_Data/Managed/SharpHook.dll")) {
            LoadAssembly("Mods/AdofaiTweaks/AdofaiTweaks.Compat.AsyncSharpHook.dll");
            modEntry.Logger.Log("Async assembly: SharpHook");
        } else {
            LoadAssembly("Mods/AdofaiTweaks/AdofaiTweaks.Compat.AsyncPolyfill.dll");
            modEntry.Logger.Log("Async assembly: Polyfill");
        }

        AdofaiTweaks.Setup(modEntry);
    }

    /// <summary>
    /// Loads an assembly at the given file path.
    /// </summary>
    /// <param name="path">The path to the assembly.</param>
    private static void LoadAssembly(string path) {
        // Load other assemblies
        using FileStream stream = new FileStream(path, FileMode.Open);
        byte[] data = new byte[stream.Length];
        stream.Read(data, 0, data.Length);
        AppDomain.CurrentDomain.Load(data);
    }

    /// <summary>
    /// Attempts to load an assembly at the given file path.
    /// </summary>
    /// <param name="path">The path to the assembly.</param>
    /// <returns>
    /// <c>true</c> if the assembly was successfully loaded, <c>false</c>
    /// otherwise.
    /// </returns>
    private static bool TryLoadAssembly(string path) {
        try {
            LoadAssembly(path);
            return true;
        } catch (Exception) {
            return false;
        }
    }
}