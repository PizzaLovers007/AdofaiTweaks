using System.Collections.Generic;
using System.Reflection;
using AdofaiTweaks.Core;
using AdofaiTweaks.Core.Attributes;
using DG.Tweening;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AdofaiTweaks.Tweaks.RestrictJudgments
{
    /// <summary>
    /// Patches for the Restrict Judgments tweak.
    /// </summary>
    internal static class RestrictJudgmentsPatches
    {
        [SyncTweakSettings]
        private static RestrictJudgmentsSettings Settings { get; set; }

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
                    __instance.StartCoroutine(__instance.ResetCustomLevel());
                } else {
                    __instance.Restart();
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
            public static void Postfix(scrCountdown __instance) {
                AdofaiTweaks.Logger.Log("ShowOverload!");
                if (invokedFailAction) {
                    invokedFailAction = false;

                    FieldInfo field = AccessTools.Field(typeof(scrCountdown), "text");
                    Text text = (Text)field.GetValue(__instance);
                    text.text =
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
}
