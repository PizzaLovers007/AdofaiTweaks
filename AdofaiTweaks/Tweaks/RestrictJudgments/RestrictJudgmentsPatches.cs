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
        private static bool shouldFailFast = false;

        private static scrController Controller {
            get {
                return scrController.instance;
            }
        }

        [TweakPatch(
            "RestrictJudgments.GetHitMarginWithRestrictions",
            "scrMisc",
            "GetHitMargin",
            MinVersion = 80)]
        private static class GetHitMarginWithRestrictions
        {
            public static void Postfix(ref HitMargin __result) {
                // Skip extra checks if the hit margin isn't restricted or if
                // not in gameplay
                if (!Settings.RestrictJudgments[(int)__result] || !Controller.gameworld) {
                    return;
                }

                switch (Settings.RestrictJudgmentAction) {
                    case RestrictJudgmentAction.KillPlayer:
                    if (!invokedFailAction) {
                        invokedFailAction = !Controller.noFail;
                        latestHitMargin = __result;

                        // Fail with an "overload", text is changed later
                        Controller.FailAction(true);
                    }
                    break;

                    case RestrictJudgmentAction.NoRegister:
                    break;

                    case RestrictJudgmentAction.InstantRestart:
                    __result = HitMargin.TooEarly;
                    // Force instantExplode fast fail even if no-fail is on
                    bool origNoFail = Controller.noFail;
                    Controller.noFail = false;
                    shouldFailFast = true;
                    Controller.instantExplode = true;
                    Controller.FailAction();
                    Controller.noFail = origNoFail;
                    break;
                }
            }
        }

        [TweakPatch(
            "RestrictJudgments.Fail2_UpdateFast",
            "scrController",
            "Fail2_Update",
            MinVersion = 80)]
        private static class Fail2_UpdateFast
        {
            public static bool Prefix(scrController __instance) {
                if (!shouldFailFast) {
                    return true;
                }

                shouldFailFast = false;
                Controller.instantExplode = false;
                if (scnEditor.instance != null) {
                    __instance.StartCoroutine(__instance.ResetCustomLevel());
                } else {
                    __instance.Restart();
                }
                return false;
            }
        }

        [HarmonyPatch(typeof(scrCountdown), "ShowOverload")]
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

        [HarmonyPatch(typeof(scrController), "ShowHitText")]
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
