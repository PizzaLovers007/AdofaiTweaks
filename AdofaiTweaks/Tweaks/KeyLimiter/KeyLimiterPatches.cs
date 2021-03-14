using AdofaiTweaks.Core.Attributes;
using HarmonyLib;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.KeyLimiter
{
    /// <summary>
    /// Patches for the Key Limiter tweak.
    /// </summary>
    internal static class KeyLimiterPatches
    {
        [SyncTweakSettings]
        private static KeyLimiterSettings Settings { get; set; }

        [HarmonyPatch(typeof(scrController), "CountValidKeysPressed")]
        private static class CountValidKeysPressedPatch
        {
            public static bool Prefix(ref int __result, scrController __instance) {
                // Don't make changes if the tweak is diabled
                if (!Settings.IsEnabled) {
                    return true;
                }

                // Stop player inputs while we're editing the keys
                if (Settings.IsListening) {
                    __result = 0;
                    return false;
                }

                int keysPressed = 0;

                // Check registered keys
                foreach (KeyCode code in Settings.ActiveKeys) {
                    if (Input.GetKeyDown(code)) {
                        keysPressed++;
                    }
                }

                // Always account for certain keys
                foreach (KeyCode code in KeyLimiterTweak.ALWAYS_BOUND_KEYS) {
                    if (Input.GetKeyDown(code)) {
                        keysPressed++;
                    }
                }

                // Limit keys pressed
                __result = Mathf.Min(__instance.pseudoMultipress ? 3 : 1, keysPressed);

                return false;
            }
        }

        [HarmonyPatch(typeof(scrController), "CheckForSpecialInputKeysOrPause")]
        private static class ControllerCheckForSpecialInputKeysOrPausePatch
        {
            public static void Postfix(ref bool __result) {
                if (!AdofaiTweaks.IsEnabled || !Settings.IsEnabled) {
                    return;
                }

                // Stop player inputs while we're editing the keys
                if (Settings.IsListening) {
                    return;
                }

                // Don't force keys if it's paused
                if (scrController.instance?.paused ?? true) {
                    return;
                }

                // Force active keys to not be special
                foreach (KeyCode code in Settings.ActiveKeys) {
                    if (Input.GetKeyDown(code)) {
                        __result = false;
                        return;
                    }
                }
            }
        }
    }
}
