using System.Linq;
using System.Reflection;
using AdofaiTweaks.Compat.Async;
using AdofaiTweaks.Core.Attributes;
using HarmonyLib;
using UnityEngine;
using static RDInputType;

namespace AdofaiTweaks.Tweaks.KeyLimiter;

/// <summary>
/// Patches for the Key Limiter tweak.
/// </summary>
internal static class KeyLimiterPatches
{
    [SyncTweakSettings]
    private static KeyLimiterSettings Settings { get; set; }

    [TweakPatch(
        "KeyLimiter.KeyboardMainWithLimit",
        "RDInputType_Keyboard",
        "Main")]
    private static class KeyboardMainWithLimit
    {
        private static readonly MethodInfo GetStateCountMethod =
            AccessTools.Method(typeof(RDInputType_Keyboard), "GetStateCount");

        public static void Postfix(
            RDInputType_Keyboard __instance, ref int __result, ButtonState state) {
            // Stop player inputs while we're editing the keys
            if (Settings.IsListening) {
                __result = 0;
                return;
            }

            // Do not limit keys if player is in CLS and has disabled key
            // limiting there
            if (!Settings.LimitKeyOnCLS && ADOBase.isCLS) {
                return;
            }

            // Do not limit keys if player is in main screen and has
            // disabled key limiting there
            if (!Settings.LimitKeyOnMainScreen
                && !ADOBase.controller.gameworld
                && !ADOBase.isCLS) {
                return;
            }

            // Do not limit keys in the pause menu
            if (scrController.instance.pauseMenu.isActiveAndEnabled) {
                return;
            }

            // Only count the limited keys
            MainStateCount stateCount =
                (MainStateCount)GetStateCountMethod.Invoke(__instance, new object[] { state });
            __result =
                stateCount.keys
                    .Count(k => Settings.ActiveKeys.Contains((KeyCode)k.value));
        }
    }

    [TweakPatch(
        "KeyLimiter.AsyncKeyboardMainWithLimit",
        "RDInputType_AsyncKeyboard",
        "Main")]
    private static class AsyncKeyboardMainWithLimit
    {
        private static readonly MethodInfo GetStateCountMethod =
            AccessTools.Method(typeof(RDInputType_AsyncKeyboard), "GetStateCount");

        public static void Postfix(
            RDInputType_AsyncKeyboard __instance, ref int __result, ButtonState state) {
            // Stop player inputs while we're editing the keys
            if (Settings.IsListening) {
                __result = 0;
                return;
            }

            // Do not limit keys if player is in CLS and has disabled key
            // limiting there
            if (!Settings.LimitKeyOnCLS && ADOBase.isCLS) {
                return;
            }

            // Do not limit keys if player is in main screen and has
            // disabled key limiting there
            if (!Settings.LimitKeyOnMainScreen
                && !ADOBase.controller.gameworld
                && !ADOBase.isCLS) {
                return;
            }

            // Do not limit keys in the pause menu
            if (scrController.instance.pauseMenu.isActiveAndEnabled) {
                return;
            }

            // Only count the limited keys
            MainStateCount stateCount =
                (MainStateCount)GetStateCountMethod.Invoke(__instance, new object[] { state });
            __result =
                stateCount.keys
                    .Select(AsyncInputManagerCompat.ConvertAnyKeyCodeToRaw)
                    .Count(kRaw => Settings.ActiveAsyncKeys.Contains(kRaw));
        }
    }
}