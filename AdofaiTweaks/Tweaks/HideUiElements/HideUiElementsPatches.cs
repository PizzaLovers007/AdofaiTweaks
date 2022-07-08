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

        private static bool CheckRecordingModeShortcut() =>
            Settings.UseRecordingModeShortcut && Settings.RecordingModeShortcut.CheckShortcut();

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

        [HarmonyPatch(typeof(scrController), "OnLandOnPortal")]
        private static class HideResultTextPatch
        {
            public static void Postfix(scrController __instance)
            {
                if (SelectedProfile.HideEverything || SelectedProfile.HideResult)
                {
                    __instance.txtCongrats.gameObject.SetActive(false);
                    __instance.txtResults.gameObject.SetActive(false);
                }
            }
        }

        private static class HideHitErrorMeterPatch
        {
            private static void HideErrorMeter()
            {
                var controller = ADOBase.controller;
                var errorMeter = controller.errorMeter;
                if ((SelectedProfile.HideEverything || SelectedProfile.HideHitErrorMeter) &&
                    errorMeter &&
                    controller.gameworld &&
                    errorMeter.gameObject.activeSelf)
                {
                    errorMeter.gameObject.SetActive(false);
                }
            }

            [HarmonyPatch(typeof(scrController), "paused", MethodType.Setter)]
            private static class scrControllerHideHitErrorMeterPatch
            {
                public static void Postfix() => HideErrorMeter();
            }

            [HarmonyPatch(typeof(scrPlanet), "MoveToNextFloor")]
            private static class scrPlanetHideHitErrorMeterPatch
            {
                public static void Postfix() => HideErrorMeter();
            }

            [HarmonyPatch(typeof(TaroCutsceneScript), "DisplayText")]
            private static class TaroCutsceneScriptHideHitErrorMeterPatch
            {
                public static void Postfix() => HideErrorMeter();
            }
        }

        [HarmonyPatch(typeof(scnEditor), "Update")]
        private static class scnEditorRecordingToggleShortcutPatch
        {
            public static void Postfix(scnEditor __instance)
            {
                if (CheckRecordingModeShortcut() && __instance.inStrictlyEditingMode)
                {
                    Settings.ToggleRecordingMode();
                }
            }
        }

        [HarmonyPatch(typeof(scnCLS), "Update")]
        private static class scnCLSRecordingToggleShortcutPatch
        {
            public static void Postfix()
            {
                if (CheckRecordingModeShortcut())
                {
                    Settings.ToggleRecordingMode();
                }
            }
        }
    }
}
