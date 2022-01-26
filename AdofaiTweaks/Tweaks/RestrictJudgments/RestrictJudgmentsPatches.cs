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
        private static bool skipSwitchChosen = false;

        private static scrController Controller {
            get {
                return scrController.instance;
            }
        }

        [HarmonyPatch(typeof(scrMistakesManager), "AddHit")]
        private static class MistakesManagerRestrictPlayerPatch
        {
            private static readonly MethodInfo FAIL_ACTION_METHOD =
                AccessTools.Method(typeof(scrController), "FailAction");

            // Cancel hit registering
            private static void CancelRegister() {
                var nextfloor = Controller.currFloor?.nextfloor;
                if (nextfloor) {
                    // Disable glow for sprite floors
                    if (nextfloor.bottomglow) {
                        nextfloor.bottomglow.enabled = false;
                    }

                    if (nextfloor.topglow) {
                        nextfloor.topglow.enabled = false;
                    }

                    Controller.OnDamage();

                    Vector3 position = Controller.chosenplanet.other.transform.position;
                    position.y += 1f;

                    Controller.ShowHitText(
                        latestHitMargin,
                        position,
                        (float)(Controller.chosenplanet.targetExitAngle - Controller.chosenplanet.angle));

                    skipSwitchChosen = true;
                }
            }

            public static void Postfix(ref HitMargin hit) {
                if (Settings.RestrictJudgments[(int)hit]) {
                    latestHitMargin = hit;

                    switch (Settings.RestrictJudgmentAction) {
                        case RestrictJudgmentAction.KillPlayer:
                        if (!invokedFailAction) {
                            invokedFailAction = true;

                            // 72+ => 2 arguments, 71- => 1 argument
                            FAIL_ACTION_METHOD.Invoke(
                                Controller,
                                AdofaiTweaks.ReleaseNumber > 71 ? new object[] { true, false } : new object[] { true });
                        }
                        break;
                        case RestrictJudgmentAction.NoRegister:
                        switch (hit) {
                            // Those judgements are not registered as a successful hit
                            case HitMargin.TooEarly:
                            case HitMargin.TooLate:
                            break;
                            default:
                            if (GCS.perfectOnlyMode) {
                                switch (hit) {
                                    // Those judgements are also not registered as a successful hit on perfectOnlyMode
                                    case HitMargin.VeryEarly:
                                    case HitMargin.VeryLate:
                                    break;
                                    default:
                                    CancelRegister();
                                    break;
                                }
                            } else {
                                CancelRegister();
                            }
                            break;
                        }
                        break;
                        case RestrictJudgmentAction.InstantRestart:
                        Controller.printe("AdofaiTweaks MOD> killing all tweens");
                        DOTween.KillAll();

                        if (Controller.isEditingLevel) {
                            hideMarginText = true;
                            Controller.StartCoroutine("ResetCustomLevel");
                        } else {
                            SceneManager.LoadScene(ADOBase.sceneName);
                        }
                        break;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(scrCountdown), "ShowOverload")]
        private static class CountdownShowOverloadPatch
        {
            public static void Postfix(scrCountdown __instance) {
                if (invokedFailAction) {
                    invokedFailAction = false;

                    FieldInfo field = AccessTools.Field(typeof(scrCountdown), "text");
                    Text text = (Text)field.GetValue(__instance);
                    text.text =
                        Settings.CustomDeathString.Replace(
                            "{judgment}", TweakStrings.GetRDString("HitMargin." + latestHitMargin.ToString()));
                    field.SetValue(__instance, text);
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

        private static class SwitchChosenSkipPatches
        {
            private static bool floorHasConditionalChange = false;
            private static List<ffxPlusBase> floorPerfectEffects = new List<ffxPlusBase>();
            private static List<ffxPlusBase> floorHitEffects = new List<ffxPlusBase>();
            private static List<ffxPlusBase> floorBarelyEffects = new List<ffxPlusBase>();
            private static List<ffxPlusBase> floorMissEffects = new List<ffxPlusBase>();
            private static List<ffxPlusBase> floorLossEffects = new List<ffxPlusBase>();

            [HarmonyPatch(typeof(scrController), "ClearMisses")]
            private static class ControllerClearMissesPatch
            {
                public static bool Prefix(scrController __instance) {
                    AdofaiTweaks.Logger.Log($"{(skipSwitchChosen ? "" : "not ")}skipping the ClearMisses method!");
                    if (skipSwitchChosen) {
                        scrFloor floor = __instance.currFloor.nextfloor;
                        floorHasConditionalChange = floor.hasConditionalChange;

                        floorPerfectEffects = floor.perfectEffects;
                        floorHitEffects = floor.hitEffects;
                        floorBarelyEffects = floor.barelyEffects;
                        floorMissEffects = floor.missEffects;
                        floorLossEffects = floor.lossEffects;

                        floor.perfectEffects = new List<ffxPlusBase>();
                        floor.hitEffects = new List<ffxPlusBase>();
                        floor.barelyEffects = new List<ffxPlusBase>();
                        floor.missEffects = new List<ffxPlusBase>();
                        floor.lossEffects = new List<ffxPlusBase>();

                        hideMarginText = true;

                        return floor.hasConditionalChange = false;
                    }

                    return true;
                }
            }

            [HarmonyPatch(typeof(scrPlanet), "MoveToNextFloor")]
            private static class PlanetMoveToNextFloorPatch
            {
                public static bool Prefix() {
                    AdofaiTweaks.Logger.Log($"{(skipSwitchChosen ? "" : "not ")}skipping the MoveToNextFloor method!");
                    return !skipSwitchChosen;
                }
            }

            [HarmonyPatch(typeof(scrPlanet), "SwitchChosen")]
            private static class PlanetSwitchChosenPatch
            {
                public static void Postfix(ref scrPlanet __result) {
                    AdofaiTweaks.Logger.Log($"{(skipSwitchChosen ? "" : "not ")}skipping the SwitchChosen method!");
                    if (skipSwitchChosen) {
                        __result = __result.other;

                        scrFloor floor = __result.controller.currFloor.nextfloor;
                        floor.hasConditionalChange = floorHasConditionalChange;

                        floor.perfectEffects = floorPerfectEffects;
                        floor.hitEffects = floorHitEffects;
                        floor.barelyEffects = floorBarelyEffects;
                        floor.missEffects = floorMissEffects;
                        floor.lossEffects = floorLossEffects;

                        skipSwitchChosen = false;
                    }
                }
            }
        }
    }
}
