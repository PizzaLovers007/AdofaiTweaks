using AdofaiTweaks.Core.Attributes;
using HarmonyLib;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.HideUiElements
{
    /// <summary>
    /// Patches for the Hide UI Elements tweak.
    /// </summary>
    internal static class HideUiElementsPatches
    {
        [SyncTweakSettings]
        private static HideUiElementsSettings Settings { get; set; }

        [HarmonyPatch(typeof(scrHitTextMesh), "Show")]
        private static class JudgmentTextShowPatch
        {
            public static void Prefix(ref Vector3 position) {
                if (Settings.HideEverything || Settings.HideJudgment) {
                    position = new Vector3(123456f, 123456f, 123456f);
                }
            }
        }

        [HarmonyPatch(typeof(scrMissIndicator), "Awake")]
        private static class MissIndicatorPatch
        {
            public static void Postfix(scrMissIndicator __instance) {
                if (Settings.HideEverything || Settings.HideMissIndicators) {
                    __instance.transform.position = new Vector3(123456f, 123456f, 123456f);
                }
            }
        }

        [HarmonyPatch(typeof(scrShowIfDebug), "Update")]
        private static class HideAutoplayTextPatch
        {
            private static bool prevVal;

            public static void Prefix() {
                prevVal = RDC.auto;
                if (Settings.HideEverything || Settings.HideOtto) {
                    RDC.auto = false;
                }
            }

            public static void Postfix() {
                RDC.auto = prevVal;
            }
        }
    }
}
