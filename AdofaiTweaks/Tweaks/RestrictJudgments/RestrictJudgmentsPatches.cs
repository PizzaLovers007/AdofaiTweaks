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

            public static void Postfix(scrMistakesManager __instance, ref HitMargin hit) {
                if (Settings.RestrictJudgments[(int)hit] && !invokedFailAction) {
                    invokedFailAction = true;
                    FAMargin = hit;

                    AdofaiTweaks.Logger.Log("FailAction 1 argument");
                    FAIL_ACTION_METHOD.Invoke(__instance.controller, new object[] { true });
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

            public static void Postfix(scrMistakesManager __instance, ref HitMargin hit) {
                if (Settings.RestrictJudgments[(int)hit] && !invokedFailAction) {
                    invokedFailAction = true;
                    FAMargin = hit;

                    AdofaiTweaks.Logger.Log("FailAction 2 arguments");
                    FAIL_ACTION_METHOD.Invoke(__instance.controller, new object[] { true, false });
                }
            }
        }

        [HarmonyPatch(typeof(scrCountdown), "ShowOverload")]
        private static class CountdownShowOverloadPatch
        {
            public static void Postfix(scrCountdown __instance) {
                if (invokedFailAction) {
                    invokedFailAction = false;

                    FieldInfo field = typeof(scrCountdown).GetField("text", AccessTools.all);
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
