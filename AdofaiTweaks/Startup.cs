using UnityModManagerNet;

namespace AdofaiTweaks
{
    internal static class Startup
    {
        internal static void Load(UnityModManager.ModEntry modEntry) {
            AdofaiTweaks.Setup(modEntry);
        }
    }
}
