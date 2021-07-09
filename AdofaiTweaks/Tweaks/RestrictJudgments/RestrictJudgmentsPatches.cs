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
        // private static double lastAngle;
        private static bool hideMarginText = false;

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
            public static void Postfix(ref HitMargin hit)
            {
                if (Settings.RestrictJudgments[(int)hit])
                {
                    latestHitMargin = hit;

                    switch (Settings.RestrictJudgmentAction)
                    {
                        case RestrictJudgmentAction.KillPlayer:
                            if (!invokedFailAction)
                            {
                                invokedFailAction = true;

                                // 72+ => 2 arguments, 71- => 1 argument
                                FAIL_ACTION_METHOD.Invoke(
                                    Controller,
                                    AdofaiTweaks.ReleaseNumber > 71 ? new object[] { true, false } : new object[] { true });
                            }
                            break;
                        case RestrictJudgmentAction.NoRegister:
                            switch (hit)
                            {
                                case HitMargin.TooEarly:
                                case HitMargin.TooLate:
                                    break;
                                default:
                                    void CancelRegister()
                                    {
                                        if (Controller.currFloor)
                                        {
                                            Controller.currFloor.bottomglow.enabled = false;
                                            Controller.currFloor.topglow.enabled = false;
                                            Controller.OnDamage();

                                            Vector3 position = Controller.chosenplanet.other.transform.position;
                                            position.y += 1f;

                                            Controller.ShowHitText(
                                                latestHitMargin,
                                                position,
                                                (float)(Controller.chosenplanet.targetExitAngle - Controller.chosenplanet.angle));
                                            // hideMarginText = true;

                                            /* Hint: ScrubAdjacent?
                                             * TODO: Find a different way to do this instead of throwing an exception.
                                             */

                                            /*
                                            int seqID = Controller.currFloor.seqID - 1;
                                            scrFloor scrFloor = scrLevelMaker.instance.listFloors[seqID];
                                            Controller.chosenplanet.other.currfloor = scrFloor;
                                            Controller.currentSeqID = seqID;
                                            Controller.chosenplanet.other.transform.position = scrFloor.transform.position;
                                            AdofaiTweaks.Logger.Log($"Last angle applied: {Controller.chosenplanet.angle} -> {lastAngle}");
                                            AdofaiTweaks.Logger.Log($"Last snapped: {AccessTools.Field(typeof(scrPlanet), "snappedLastAngle").GetValue(Controller.chosenplanet)}");
                                            Controller.chosenplanet.angle = (double)AccessTools.Field(typeof(scrPlanet), "snappedLastAngle").GetValue(Controller.chosenplanet);
                                            */

                                            // force stop while in scrPlanet.SwitchChosen method
                                            throw new Exception("Intentional Exception fired by RestrictJudgment Tweak (don't use noregister if you don't want this error log spam)");
                                        }
                                    }

                                    if (GCS.perfectOnlyMode)
                                    {
                                        switch (hit)
                                        {
                                            case HitMargin.VeryEarly:
                                            case HitMargin.VeryLate:
                                                break;
                                            default:
                                                CancelRegister();
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        CancelRegister();
                                    }
                                    break;
                            }
                            break;
                        case RestrictJudgmentAction.InstantRestart:
                            Controller.printe("killing all tweens (adofaitweaks invoked)");
                            DOTween.KillAll();

                            if (Controller.isEditingLevel)
                            {
                                hideMarginText = true;
                                Controller.StartCoroutine("ResetCustomLevel");
                            }
                            else
                            {
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

        /*[HarmonyPatch(typeof(scrController), "Hit")]
        private static class ControllerHitPatch
        {
            public static void Prefix(scrController __instance)
            {
                lastAngle = __instance.chosenplanet.angle;
                AdofaiTweaks.Logger.Log($"Last angle set: {lastAngle}");
            }
        }*/

        [HarmonyPatch(typeof(scrController), "ShowHitText")]
        private static class ControllerShowHitTextPatch
        {
            public static bool Prefix()
            {
                if (hideMarginText)
                {
                    return hideMarginText = false;
                }
                return true;
            }
        }
    }
}
