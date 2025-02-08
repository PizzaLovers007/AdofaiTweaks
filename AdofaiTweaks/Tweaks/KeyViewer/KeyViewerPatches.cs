using AdofaiTweaks.Core.Attributes;
using HarmonyLib;

namespace AdofaiTweaks.Tweaks.KeyViewer;

/// <summary>
/// Patches for the Key Viewer tweak.
/// </summary>
internal static class KeyViewerPatches
{
    [SyncTweakSettings]
    private static KeyViewerSettings Settings { get; set; }

    [HarmonyPatch(typeof(scrController), "CountValidKeysPressed")]
    private static class CountValidKeysPressedPatch
    {
        [HarmonyBefore("adofai_tweaks.key_limiter")]
        public static bool Prefix(ref int __result) {
            // Stop player inputs while we're editing the keys
            if (Settings.IsListening) {
                __result = 0;
                return false;
            }

            return true;
        }
    }
}