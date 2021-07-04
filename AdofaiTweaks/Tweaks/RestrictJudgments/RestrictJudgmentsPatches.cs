using System.Reflection;
using AdofaiTweaks.Core.Attributes;
using HarmonyLib;
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
        private static HitMargin FAMargin;

        [TweakPatch(
            "RestrictJudgments.KillPlayerBeforeMultipress",
            "scrMistakesManager",
            "AddHit",
            maxVersion: 71)]
        private static class KillPlayerBeforeMultipressPatch
        {
            private static readonly MethodInfo FAIL_ACTION_METHOD =
                AccessTools.Method(typeof(scrController), "FailAction");

            public static void Postfix(scrMistakesManager __instance, ref HitMargin hit)
            {
                if (Settings.RestrictJudgments[(int)hit])
                {
                    switch (Settings.RestrictJudgmentAction)
                    {
                        case RestrictJudgmentAction.KillPlayer:
                            if (!invokedFailAction) {
                                invokedFailAction = true;
                                FAMargin = hit;

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
            private static readonly MethodInfo FAIL_ACTION_METHOD =
                AccessTools.Method(typeof(scrController), "FailAction");
            private static readonly MethodInfo MOVE_TO_NEXT_FLOOR_METHOD =
                AccessTools.Method(typeof(scrController), "MoveToNextFloor");

            private static scrController Controller {
                get {
                    return scrController.instance;
                }
            }

            public static void Postfix(scrMistakesManager __instance, ref HitMargin hit) {
                if (Settings.RestrictJudgments[(int)hit])
                {
                    switch (Settings.RestrictJudgmentAction)
                    {
                        case RestrictJudgmentAction.KillPlayer:
                            if (!invokedFailAction) {
                                invokedFailAction = true;
                                FAMargin = hit;

                                // AdofaiTweaks.Logger.Log("FailAction 1 argument");
                                FAIL_ACTION_METHOD.Invoke(__instance.controller, new object[] { true, false });
                            }
                            break;
                        case RestrictJudgmentAction.NoRegister:
                            // Controller.currentSeqID--;
                            switch (hit) {
                                case HitMargin.TooEarly:
                                case HitMargin.TooLate:
                                    break;
                                default:
                                    if (GCS.perfectOnlyMode) {
                                        switch (hit) {
                                            case HitMargin.VeryEarly:
                                            case HitMargin.VeryLate:
                                                break;
                                            default:
                                                Controller.currentSeqID -= 2;
                                                scrFloor previousFloor =
                                                    Controller.customLevel.levelMaker.listFloors[0];
                                                previousFloor.nextfloor.bottomglow.enabled = false;
                                                previousFloor.nextfloor.topglow.enabled = false;
                                                MOVE_TO_NEXT_FLOOR_METHOD.Invoke(Controller.chosenplanet, new object[] {
                                                    previousFloor, Controller.chosenplanet.angle, hit, });
                                                break;
                                        }
                                    } else {
                                        Controller.currentSeqID--;
                                        scrFloor previousFloor =
                                            Controller.customLevel.levelMaker.listFloors[Controller.currentSeqID];
                                        previousFloor.nextfloor.bottomglow.enabled = false;
                                        previousFloor.nextfloor.topglow.enabled = false;
                                        MOVE_TO_NEXT_FLOOR_METHOD.Invoke(Controller.chosenplanet, new object[] {
                                            previousFloor, Controller.chosenplanet.angle, hit, });
                                    }
                                    break;
                            }
                            break;
                        case RestrictJudgmentAction.InstantRestart:
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
                            "{judgment}", RDString.Get("HitMargin." + FAMargin.ToString()));
                    field.SetValue(__instance, text);
                }
            }
        }
    }
}
