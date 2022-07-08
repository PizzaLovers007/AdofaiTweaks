using System;
using System.IO;
using UnityModManagerNet;

namespace AdofaiTweaks
{
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

            AdofaiTweaks.Setup(modEntry);
        }

        /// <summary>
        /// Loads an assembly at the given file path.
        /// </summary>
        /// <param name="path">The path to the assembly.</param>
        private static void LoadAssembly(string path)
        {
            // Load other assemblies
            using FileStream stream = new FileStream(path, FileMode.Open);
            byte[] data = new byte[stream.Length];
            stream.Read(data, 0, data.Length);
            AppDomain.CurrentDomain.Load(data);
        }
    }
}
