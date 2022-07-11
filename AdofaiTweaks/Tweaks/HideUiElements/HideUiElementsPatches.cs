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

        private static HideUiElementsProfile SelectedProfile =>
            Settings.RecordingMode ? Settings.RecordingProfile : Settings.PlayingProfile;

        [HarmonyPatch(typeof(scrHitTextMesh), "Show")]
        private static class JudgmentTextShowPatch
        {
            public static void Prefix(ref Vector3 position) {
                if (SelectedProfile.HideEverything || SelectedProfile.HideJudgment) {
                    position = new Vector3(123456f, 123456f, 123456f);
                }
            }
        }

        [HarmonyPatch(typeof(scrMissIndicator), "Awake")]
        private static class MissIndicatorPatch
        {
            public static void Postfix(scrMissIndicator __instance) {
                if (SelectedProfile.HideEverything || SelectedProfile.HideMissIndicators) {
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
                if (SelectedProfile.HideEverything || SelectedProfile.HideOtto) {
                    RDC.auto = false;
                }
            }

            public static void Postfix() {
                RDC.auto = prevVal;
            }
        }

        private static class HideResultTextAndFlashPatches
        {
            private static bool shouldIgnoreFlashOnce;

            [HarmonyPatch(typeof(scrController), "OnLandOnPortal")]
            private static class HideResultTextPatch
            {
                public static void Prefix() {
                    if (SelectedProfile.HideEverything || SelectedProfile.HideLastFloorFlash) {
                        shouldIgnoreFlashOnce = true;
                    }
                }

                public static void Postfix(scrController __instance) {
                    if (SelectedProfile.HideEverything || SelectedProfile.HideResult) {
                        __instance.txtCongrats.gameObject.SetActive(false);
                        __instance.txtResults.gameObject.SetActive(false);
                    }
                }
            }

            [HarmonyPatch(typeof(scrFlash), "Flash")]
            private static class HideLastFloorFlashPatch
            {
                public static bool Prefix(ref Color _colorStart) {
                    if (shouldIgnoreFlashOnce && _colorStart == Color.white.WithAlpha(.4f)) {
                        return shouldIgnoreFlashOnce = false;
                    }

                    return true;
                }
            }
        }

        private static class HideHitErrorMeterPatches
        {
            private static void HideErrorMeter() {
                var controller = ADOBase.controller;
                var errorMeter = controller.errorMeter;
                if ((SelectedProfile.HideEverything || SelectedProfile.HideHitErrorMeter) &&
                    errorMeter &&
                    controller.gameworld &&
                    errorMeter.gameObject.activeSelf) {
                    errorMeter.gameObject.SetActive(false);
                }
            }

            [HarmonyPatch(typeof(scrController), "paused", MethodType.Setter)]
            private static class ScrControllerHideHitErrorMeterPatch
            {
                public static void Postfix() => HideErrorMeter();
            }

            [HarmonyPatch(typeof(scrPlanet), "MoveToNextFloor")]
            private static class ScrPlanetHideHitErrorMeterPatch
            {
                public static void Postfix() => HideErrorMeter();
            }

            [HarmonyPatch(typeof(TaroCutsceneScript), "DisplayText")]
            private static class TaroCutsceneScriptHideHitErrorMeterPatch
            {
                public static void Postfix() => HideErrorMeter();
            }
        }
    }
}
