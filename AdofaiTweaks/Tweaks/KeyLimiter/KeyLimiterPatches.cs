using System.Linq;
using System.Reflection;
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

        private static readonly PropertyInfo _scrControllerCLSModeProperty =
            AccessTools.Property(typeof(scrController), "CLSMode");

        [TweakPatch(
            "KeyLimiter.CountValidKeysPressedBeforeMultipressPatch",
            "scrController",
            "CountValidKeysPressed",
            MaxVersion = 71)]
        private static class CountValidKeysPressedBeforeMultipressPatch
        {
            private static readonly FieldInfo _scrControllerPseudoMultipressField =
                AccessTools.Field(typeof(scrController), "pseudoMultipress");

            public static bool Prefix(ref int __result, scrController __instance) {
                // Do not limit keys if current scene is CLS and player has
                // disabled key limiting in CLS
                if (!Settings.LimitKeyOnCLS && (bool)_scrControllerCLSModeProperty.GetValue(__instance)) {
                    return true;
                }

                // Do not limit keys if player is in main screen and has
                // disabled key limiting in there
                if (!Settings.LimitKeyOnMainScreen
                    && !__instance.gameworld
                    && !(bool)_scrControllerCLSModeProperty.GetValue(__instance)) {
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
                if (_scrControllerPseudoMultipressField.GetValue(__instance) is bool castedPseudoMultipress) {
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
            MinVersion = 72,
            MaxVersion = 96)]
        private static class CountValidKeysPressedAfterMultipressPatch
        {
            private static readonly PropertyInfo _ADOBaseIsCLSProperty =
                AccessTools.Property(typeof(ADOBase), "isCLS");

            private static readonly bool ReleaseNumberIsBelow94 = AdofaiTweaks.ReleaseNumber < 94;

            private static bool GetCLSMode(scrController controller = null)
            {
                if (ReleaseNumberIsBelow94)
                {
                    return (bool)_scrControllerCLSModeProperty.GetValue(controller);
                }

                return (bool)_ADOBaseIsCLSProperty.GetValue(null);
            }

            public static bool Prefix(ref int __result, scrController __instance) {
                // Do not limit keys if current scene is CLS and player has
                // disabled key limiting in CLS
                if (!Settings.LimitKeyOnCLS && GetCLSMode(__instance)) {
                    return true;
                }

                // Do not limit keys if player is in main screen and has
                // disabled key limiting in there
                if (!Settings.LimitKeyOnMainScreen
                    && !__instance.gameworld
                    && !GetCLSMode(__instance)) {
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

        [TweakPatch(
            "KeyLimiter.CountValidKeysAfterAsyncInputRefactorPatch",
            "scrController",
            "CountValidKeysPressed",
            MinVersion = 97)]
        private static class CountValidKeysAfterAsyncInputRefactorPatch
        {
            public static bool Prefix(ref int __result, scrController __instance)
            {
                // Do not limit keys if current scene is CLS and player has
                // disabled key limiting in CLS
                if (!Settings.LimitKeyOnCLS && ADOBase.isCLS) {
                    return true;
                }

                // Do not limit keys if player is in main screen and has
                // disabled key limiting in there
                if (!Settings.LimitKeyOnMainScreen
                    && !__instance.gameworld
                    && !ADOBase.isCLS) {
                    return true;
                }

                // Stop player inputs while we're editing the keys
                if (Settings.IsListening) {
                    __result = 0;
                    return false;
                }

                int keysPressed = 0;

                if (AsyncInputManager.isActive) {
                    // Check registered keys
                    keysPressed += Settings.ActiveAsyncKeys.Count(k => AsyncInput.GetKeyDown(k, false))
                                   // Always account for certain keys
                                   + KeyLimiterTweak.ALWAYS_BOUND_ASYNC_KEYS.Count(k => AsyncInput.GetKeyDown(k, false));
                }
                else {
                    // Check registered keys
                    keysPressed += Settings.ActiveKeys.Count(Input.GetKeyDown)
                                   // Always account for certain keys
                                   + KeyLimiterTweak.ALWAYS_BOUND_KEYS.Count(Input.GetKeyDown);
                }

                // Limit keys pressed
                __result = Mathf.Min(4, keysPressed);

                return false;
            }
        }

        // TODO: Find a corresponding method on versions after r94
        [TweakPatch(
            "KeyLimiter.CheckForSpecialInputKeysOrPausePatch",
            "scrController",
            "CheckForSpecialInputKeysOrPause",
            MaxVersion = 93)]
        private static class ControllerCheckForSpecialInputKeysOrPausePatch
        {
            public static void Postfix(ref bool __result, scrController __instance) {
                // Stop player inputs while we're editing the keys
                if (Settings.IsListening) {
                    return;
                }

                // Don't force keys if it's paused
                if (__instance?.paused ?? true) {
                    return;
                }

                // Do not override special keys in CLS
                if ((bool)_scrControllerCLSModeProperty.GetValue(__instance)) {
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
