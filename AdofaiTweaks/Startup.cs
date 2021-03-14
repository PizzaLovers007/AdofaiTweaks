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
            AdofaiTweaks.Setup(modEntry);
        }
    }
}
