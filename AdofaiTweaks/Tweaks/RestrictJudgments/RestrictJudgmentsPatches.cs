using System;
using System.Collections;
using System.Reflection;
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
        private static bool dontMoveToNextFloor = false;

        private static readonly MethodInfo FAIL_ACTION_METHOD =
                AccessTools.Method(typeof(scrController), "FailAction");
        private static readonly MethodInfo RESET_CUSTOM_LEVEL_METHOD =
                AccessTools.Method(typeof(scrController), "ResetCustomLevel");

        private static scrController Controller {
            get {
                return scrController.instance;
            }
        }

        [TweakPatch(
            "RestrictJudgments.KillPlayerBeforeMultipress",
            "scrMistakesManager",
            "AddHit",
            maxVersion: 71)]
        private static class KillPlayerBeforeMultipressPatch
        {
            public static void Postfix(scrMistakesManager __instance, ref HitMargin hit) {
                if (Settings.RestrictJudgments[(int)hit]) {
                    switch (Settings.RestrictJudgmentAction) {
                        case RestrictJudgmentAction.KillPlayer:
                            if (!invokedFailAction) {
                                invokedFailAction = true;
                                latestHitMargin = hit;

                                // AdofaiTweaks.Logger.Log("FailAction 1 argument");
                                FAIL_ACTION_METHOD.Invoke(__instance.controller, new object[] { true });
                            }
                            break;
                        case RestrictJudgmentAction.NoRegister:
                            scrController.instance.chosenplanet.SwitchChosen();
                            break;
                        case RestrictJudgmentAction.InstantRestart:
                            break;
                    }
                }
            }
        }

        [TweakPatch(
            "RestrictJudgments.KillPlayerAfterMultipress",
            "scrMistakesManager",
            "AddHit",
            minVersion: 72)]
        private static class KillPlayerAfterMultipressPatch
        {
            public static void Postfix(ref HitMargin hit) {
                if (Settings.RestrictJudgments[(int)hit]) {
                    latestHitMargin = hit;

                    switch (Settings.RestrictJudgmentAction) {
                        case RestrictJudgmentAction.KillPlayer:
                            if (!invokedFailAction) {
                                invokedFailAction = true;

                                // AdofaiTweaks.Logger.Log("FailAction 2 argument");
                                FAIL_ACTION_METHOD.Invoke(Controller, new object[] { true, false });
                            }
                            break;
                        case RestrictJudgmentAction.NoRegister:
                            switch (hit) {
                                case HitMargin.TooEarly:
                                case HitMargin.TooLate:
                                    break;
                                default:
                                    void CancelRegister() {
                                        scrFloor previousFloor =
                                            Controller.customLevel.levelMaker.listFloors[Controller.currentSeqID];
                                        previousFloor.nextfloor.bottomglow.enabled = false;
                                        previousFloor.nextfloor.topglow.enabled = false;
                                        // Controller.OnDamage();
                                        dontMoveToNextFloor = true;

                                        /*
                                        Vector3 position = Controller.chosenplanet.other.transform.position;
                                        position.y += 1f;

                                        Controller.ShowHitText(
                                            latestHitMargin,
                                            position,
                                            (float)(Controller.chosenplanet.targetExitAngle - Controller.chosenplanet.angle));
                                        */

                                        // force stop while in scrPlanet.SwitchChosen method
                                        throw new Exception("Intentional Exception!");
                                    }

                                    if (GCS.perfectOnlyMode) {
                                        switch (hit) {
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
                            Controller.printe("killing all tweens (adofaitweaks invoked)");
                            DOTween.KillAll();

                            if (Controller.isEditingLevel) {
                                hideMarginText = true;
                                Controller.StartCoroutine(
                                    (IEnumerator)RESET_CUSTOM_LEVEL_METHOD.Invoke(Controller, null));
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
                            "{judgment}", RDString.Get("HitMargin." + latestHitMargin.ToString()));
                    field.SetValue(__instance, text);
                }
            }
        }
        /*
        [HarmonyPatch(typeof(scrPlanet), "MoveToNextFloor")]
        private static class MoveToNextFloorPatch
        {
            public static void Prefix() {
                if (dontMoveToNextFloor) {
                    dontMoveToNextFloor = false;

                    Vector3 position = Controller.chosenplanet.other.transform.position;
                    position.y += 1f;

                    Controller.ShowHitText(
                        latestHitMargin,
                        position,
                        (float)(Controller.chosenplanet.targetExitAngle - Controller.chosenplanet.angle));

                    // force stop while in scrPlanet.SwitchChosen method
                    throw new Exception("Intentional Exception!");
                }
            }

            /*public static bool Prefix() {
                if (dontMoveToNextFloor) {
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(scrPlanet), "SwitchChosen")]
        private static class SwitchChosenPatch
        {
            public static void Postfix(ref scrPlanet __instance, ref scrPlanet __result) {
                if (dontMoveToNextFloor) {
                    dontMoveToNextFloor = false;
                    __result = __instance;
                }
            }
        }
        */

        [HarmonyPatch(typeof(scrHitTextMesh), "Show")]
        private static class JudgmentTextShowPatch
        {
            public static void Prefix(ref Vector3 position) {
                if (hideMarginText) {
                    position = new Vector3(123456f, 123456f, 123456f);
                    hideMarginText = false;
                }
            }
        }
    }
}
