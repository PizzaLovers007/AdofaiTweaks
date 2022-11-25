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
    }
}
