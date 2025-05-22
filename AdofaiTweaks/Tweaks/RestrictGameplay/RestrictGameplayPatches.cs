using System.Collections;
using System.Globalization;
using System.Linq;
using System.Reflection;
using AdofaiTweaks.Core;
using AdofaiTweaks.Core.Attributes;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine.UI;

namespace AdofaiTweaks.Tweaks.RestrictGameplay;

/// <summary>
/// Patches for the Restrict Gameplay tweak.
/// </summary>
internal static class RestrictGameplayPatches
{
    [SyncTweakSettings]
    private static RestrictGameplaySettings Settings { get; set; }

    private static readonly MethodInfo ResetCustomLevelMethod =
        AccessTools.Method(typeof(scrController), "ResetCustomLevel");

    private static readonly MethodInfo RestartMethod =
        AccessTools.Method(typeof(scrController), "Restart");

    private static readonly FieldInfo HitErrorMeterAverageAngleField =
        AccessTools.Field(typeof(scrHitErrorMeter), "averageAngle");

    private enum FailReason {
        None,
        Judgment,
        AverageAngle,
    }

    private static string GetCustomFailActionMessage(this FailReason failReason) {
        return failReason switch {
            FailReason.Judgment => Settings.CustomDeathStringForJudgment.Replace(
                "{judgment}",
                TweakStrings.GetRDString("HitMargin." + latestHitMargin)),
            FailReason.AverageAngle => Settings.CustomDeathStringForAverageAngle.Replace(
                "{averageAngle}",
                Settings.AllowedAverageAngleThreshold.ToString(CultureInfo.InvariantCulture)),
            _ => ""
        };
    }

    private static FailReason failActionInvokeReason;
    private static HitMargin latestHitMargin;
    private static bool hideMarginText;
    private static bool shouldFailPlayer;
    private static bool shouldInstantRestart;

    private static scrController Controller => scrController.instance;

    [UsedImplicitly]
    [TweakPatch(
        "RestrictGameplay.IsValidHitWithRestrictions",
        "scrMisc",
        "IsValidHit",
        MinVersion = 80)]
    private static class IsValidHitWithRestrictionsPatch
    {
        [UsedImplicitly]
        public static void Postfix(ref bool __result, HitMargin margin) {
            // Not in gameplay
            if (!Controller.gameworld)
            {
                return;
            }

            var restrictJudgment = (CheckJudgment(margin), FailReason.Judgment);
            var restrictAverageAngle = (CheckAverageAngle(), FailReason.AverageAngle);

            var (restrictAction, failReason) = SelectRestrictAction(restrictJudgment, restrictAverageAngle);
            if (restrictAction == null) {
                return;
            }

            __result = false;

            switch (restrictAction) {
                case RestrictGameplayAction.KillPlayer:
                    latestHitMargin = margin;
                    shouldFailPlayer = true;
                    failActionInvokeReason = failReason;
                    break;

                case RestrictGameplayAction.InstantRestart:
                    shouldInstantRestart = true;
                    shouldFailPlayer = true;
                    failActionInvokeReason = failReason;
                    break;

                case RestrictGameplayAction.NoRegister:
                    // Just don't do anything
                    break;
            }
        }

        private static RestrictGameplayAction? CheckJudgment(HitMargin margin) {
            if (!Settings.RestrictJudgment) {
                return null;
            }

            if (!Settings.RestrictedJudgments[(int)margin]) {
                return null;
            }

            return Settings.RestrictGameplayActionForJudgment;
        }

        private static RestrictGameplayAction? CheckAverageAngle() {
            if (!Settings.RestrictAverageAngle) {
                return null;
            }

            var errorMeter = Controller.errorMeter;
            float averageAngle;

            if (errorMeter) {
                averageAngle = (float)HitErrorMeterAverageAngleField.GetValue(errorMeter);
            } else {
                averageAngle = 0;
            }

            if (averageAngle >= -Settings.AllowedAverageAngleThreshold &&
                averageAngle <= Settings.AllowedAverageAngleThreshold)
            {
                return null;
            }

            return Settings.RestrictGameplayActionForAverageAngle;
        }

        private static (RestrictGameplayAction?, FailReason) SelectRestrictAction(params (RestrictGameplayAction?, FailReason)[] actions) {
            return actions.Select(x => x.Item1 switch {
                RestrictGameplayAction.NoRegister => (x, 1),
                RestrictGameplayAction.KillPlayer => (x, 2),
                RestrictGameplayAction.InstantRestart => (x, 3),
                _ => (x, 0),
            }).Aggregate((a, b) => a.Item2 > b.Item2 ? a : b).x;
        }
    }

    [UsedImplicitly]
    [TweakPatch(
        "RestrictGameplay.SwitchChosenInstantRestartTrigger",
        "scrPlanet",
        "SwitchChosen",
        MinVersion = 80)]
    private static class SwitchChosenInstantRestartTriggerPatch
    {
        [UsedImplicitly]
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

    [UsedImplicitly]
    [TweakPatch(
        "RestrictGameplay.Fail2_UpdateFast",
        "scrController",
        "Fail2_Update",
        MinVersion = 80)]
    private static class Fail2_UpdateFastPatch
    {
        [UsedImplicitly]
        public static bool Prefix(scrController __instance) {
            if (!shouldInstantRestart) {
                return true;
            }

            shouldInstantRestart = false;
            Controller.instantExplode = false;

            if (scnEditor.instance != null) {
                object[] resetParams = AdofaiTweaks.ReleaseNumber >= 110 ? [true] : [];
                __instance.StartCoroutine(
                    (IEnumerator)ResetCustomLevelMethod.Invoke(
                        __instance,
                        resetParams));
            } else {
                object[] resetParams = AdofaiTweaks.ReleaseNumber >= 110 ? [false] : [];
                RestartMethod.Invoke(__instance, resetParams);
            }
            return false;
        }
    }

    [UsedImplicitly]
    [TweakPatch(
        "RestrictGameplay.CountdownShowOverload",
        "scrCountdown",
        "ShowOverload",
        MinVersion = 80)]
    private static class CountdownShowOverloadPatch
    {
        [UsedImplicitly]
        public static void Postfix(scrCountdown __instance, Text ___text) {
            AdofaiTweaks.Logger.Log("ShowOverload!");
            if (failActionInvokeReason != FailReason.None) {
                failActionInvokeReason = FailReason.None;
                ___text.text = failActionInvokeReason.GetCustomFailActionMessage();
            }
        }
    }

    [UsedImplicitly]
    [TweakPatch(
        "RestrictGameplay.ControllerShowHitText",
        "scrController",
        "ShowHitText",
        MinVersion = 80)]
    private static class ControllerShowHitTextPatch
    {
        [UsedImplicitly]
        public static bool Prefix() {
            if (hideMarginText) {
                return hideMarginText = false;
            }
            return true;
        }
    }
}