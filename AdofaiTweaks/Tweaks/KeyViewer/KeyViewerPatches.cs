using AdofaiTweaks.Core.Attributes;
using HarmonyLib;
using static RDInputType;

namespace AdofaiTweaks.Tweaks.KeyViewer;

/// <summary>
/// Patches for the Key Viewer tweak.
/// </summary>
internal static class KeyViewerPatches
{
    [SyncTweakSettings]
    private static KeyViewerSettings Settings { get; set; }

    [TweakPatch(
        "KeyViewer.CountValidKeysPressedPatch",
        "scrController",
        "CountValidKeysPressed",
        maxVersion: 119)]
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

    [TweakPatch(
        "KeyViewer.KeyboardMainPatch",
        "RDInputType_Keyboard",
        "Main",
        minVersion: 120)]
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

    [TweakPatch(
        "KeyViewer.AsyncKeyboardMainPatch",
        "RDInputType_AsyncKeyboard",
        "Main",
        minVersion: 120)]
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