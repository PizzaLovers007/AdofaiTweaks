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

        [TweakPatch(
            "KeyLimiter.CountValidKeysPressedBeforeMultipressPatch",
            "scrController",
            "CountValidKeysPressed",
            MaxVersion = 71)]
        private static class CountValidKeysPressedBeforeMultipressPatch
        {
            public static bool Prefix(ref int __result, scrController __instance) {
                // Do not limit keys if current scene is CLS and player has
                // disabled key limiting in CLS
                if (!Settings.LimitKeyOnCLS && __instance.CLSMode) {
                    return true;
                }

                // Do not limit keys if player is in main screen and has
                // disabled key limiting in there
                if (!Settings.LimitKeyOnMainScreen
                    && !__instance.gameworld
                    && !__instance.CLSMode) {
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
                bool pseudoMultipress = false;
                if (typeof(scrController).GetField("pseudoMultipress").GetValue(__instance) is bool castedPseudoMultipress)
                {
                    pseudoMultipress = castedPseudoMultipress;
                }

                __result = Mathf.Min(pseudoMultipress ? 3 : 1, keysPressed);

                return false;
            }
        }

        [TweakPatch(
            "KeyLimiter.CountValidKeysPressedAfterMultipressPatch",
            "scrController",
            "CountValidKeysPressed",
            MinVersion = 72)]
        private static class CountValidKeysPressedAfterMultipressPatch
        {
            public static bool Prefix(ref int __result, scrController __instance) {
                // Do not limit keys if current scene is CLS and player has
                // disabled key limiting in CLS
                if (!Settings.LimitKeyOnCLS && __instance.CLSMode) {
                    return true;
                }

                // Do not limit keys if player is in main screen and has
                // disabled key limiting in there
                if (!Settings.LimitKeyOnMainScreen
                    && !__instance.gameworld
                    && !__instance.CLSMode) {
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
                __result = Mathf.Min(4, keysPressed);

                return false;
            }
        }

        [HarmonyPatch(typeof(scrController), "CheckForSpecialInputKeysOrPause")]
        private static class ControllerCheckForSpecialInputKeysOrPausePatch
        {
            public static void Postfix(ref bool __result, scrController __instance) {
                // Stop player inputs while we're editing the keys
                if (Settings.IsListening) {
                    return;
                }

                // Don't force keys if it's paused
                if (scrController.instance?.paused ?? true) {
                    return;
                }

                // Do not override special keys in CLS
                if (__instance.CLSMode) {
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
