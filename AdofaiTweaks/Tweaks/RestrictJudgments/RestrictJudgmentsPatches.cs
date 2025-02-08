using System.Collections;
using System.Reflection;
using AdofaiTweaks.Core;
using AdofaiTweaks.Core.Attributes;
using HarmonyLib;
using UnityEngine.UI;

namespace AdofaiTweaks.Tweaks.RestrictJudgments;

/// <summary>
/// Patches for the Restrict Judgments tweak.
/// </summary>
internal static class RestrictJudgmentsPatches
{
    [SyncTweakSettings]
    private static RestrictJudgmentsSettings Settings { get; set; }

    private static readonly MethodInfo resetCustomLevelMethod =
        AccessTools.Method(typeof(scrController), "ResetCustomLevel");

    private static readonly MethodInfo restartMethod =
        AccessTools.Method(typeof(scrController), "Restart");

    private static bool invokedFailAction = false;
    private static HitMargin latestHitMargin;
    private static bool hideMarginText = false;
    private static bool shouldFailPlayer = false;
    private static bool shouldInstantRestart = false;

    private static scrController Controller {
        get {
            return scrController.instance;
        }
    }

    [TweakPatch(
        "RestrictJudgments.IsValidHitWithRestrictions",
        "scrMisc",
        "IsValidHit",
        MinVersion = 80)]
    private static class IsValidHitWithRestrictionsPatch
    {
        public static void Postfix(ref bool __result, HitMargin margin) {
            // Skip extra checks if the hit margin isn't restricted or if
            // not in gameplay
            if (!Settings.RestrictJudgments[(int)margin] || !Controller.gameworld) {
                return;
            }

            __result = false;

            switch (Settings.RestrictJudgmentAction) {
                case RestrictJudgmentAction.KillPlayer:
                    shouldFailPlayer = true;
                    latestHitMargin = margin;
                    break;

                case RestrictJudgmentAction.InstantRestart:
                    shouldFailPlayer = true;
                    shouldInstantRestart = true;
                    break;

                case RestrictJudgmentAction.NoRegister:
                    // Just don't do anything
                    break;
            }
        }
    }

    [TweakPatch(
        "RestrictJudgments.SwitchChosenInstantRestartTrigger",
        "scrPlanet",
        "SwitchChosen",
        MinVersion = 80)]
    private static class SwitchChosenInstantRestartTriggerPatch
    {
        public static void Postfix() {
            if (!shouldFailPlayer) {
                return;
            }
            shouldFailPlayer = false;

            if (shouldInstantRestart) {
                // Force instantExplode fast fail even if no-fail is on
                bool origNoFail = Controller.noFail;
                Controller.noFail = false;
                Controller.instantExplode = true;
                Controller.FailAction();
                Controller.noFail = origNoFail;
            } else {
                // Fail with an "overload", text is changed later
                invokedFailAction = true;
                Controller.FailAction(true);
            }
        }
    }

    [TweakPatch(
        "RestrictJudgments.Fail2_UpdateFast",
        "scrController",
        "Fail2_Update",
        MinVersion = 80)]
    private static class Fail2_UpdateFastPatch
    {
        public static bool Prefix(scrController __instance) {
            if (!shouldInstantRestart) {
                return true;
            }

            shouldInstantRestart = false;
            Controller.instantExplode = false;
            if (scnEditor.instance != null) {
                object[] resetParams;
                if (AdofaiTweaks.ReleaseNumber >= 110) {
                    resetParams = new object[] { true };
                } else {
                    resetParams = new object[] { };
                }
                __instance.StartCoroutine(
                    (IEnumerator)resetCustomLevelMethod.Invoke(
                        __instance,
                        resetParams));
            } else {
                object[] resetParams;
                if (AdofaiTweaks.ReleaseNumber >= 110) {
                    resetParams = new object[] { false };
                } else {
                    resetParams = new object[] { };
                }
                restartMethod.Invoke(__instance, resetParams);
            }
            return false;
        }
    }

    [TweakPatch(
        "RestrictJudgments.CountdownShowOverload",
        "scrCountdown",
        "ShowOverload",
        MinVersion = 80)]
    private static class CountdownShowOverloadPatch
    {
        public static void Postfix(scrCountdown __instance, Text ___text) {
            AdofaiTweaks.Logger.Log("ShowOverload!");
            if (invokedFailAction) {
                invokedFailAction = false;
                ___text.text =
                    Settings.CustomDeathString.Replace(
                        "{judgment}",
                        TweakStrings.GetRDString("HitMargin." + latestHitMargin.ToString()));
            }
        }
    }

    [TweakPatch(
        "RestrictJudgments.ControllerShowHitText",
        "scrController",
        "ShowHitText",
        MinVersion = 80)]
    private static class ControllerShowHitTextPatch
    {
        public static bool Prefix() {
            if (hideMarginText) {
                return hideMarginText = false;
            }
            return true;
        }
    }
}