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

    [HarmonyPatch(typeof(RDInputType_Keyboard), "Main")]
    private static class KeyboardMainPatch
    {
        [HarmonyBefore("adofai_tweaks.key_limiter")]
        public static bool Prefix(ref int __result, ButtonState state) {
            if (!Settings.IsListening) {
                return true;
            }

            __result = 0;
            return false;
        }
    }

    [HarmonyPatch(typeof(RDInputType_AsyncKeyboard), "Main")]
    private static class AsyncKeyboardMainPatch
    {
        [HarmonyBefore("adofai_tweaks.key_limiter")]
        public static bool Prefix(ref int __result, ButtonState state) {
            if (!Settings.IsListening) {
                return true;
            }

            __result = 0;
            return false;
        }
    }
}