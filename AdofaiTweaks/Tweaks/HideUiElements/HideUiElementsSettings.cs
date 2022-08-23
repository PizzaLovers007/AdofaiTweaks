using System;
using System.Linq;
using System.Reflection;
using AdofaiTweaks.Core;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace AdofaiTweaks.Tweaks.HideUiElements
{
    /// <summary>
    /// Settings for the Hide UI Elements tweak.
    /// </summary>
    public class HideUiElementsSettings : TweakSettings
    {
        /// <summary>
        /// UI hide options for non-recording mode.
        /// </summary>
        public HideUiElementsProfile PlayingProfile { get; set; } = new ();

        /// <summary>
        /// UI hide options for recording mode.
        /// </summary>
        public HideUiElementsProfile RecordingProfile { get; set; } = new ();

        /// <summary>
        /// Whether recording mode is enabled.
        /// </summary>
        public bool RecordingMode { get; set; }

        /// <summary>
        /// Whether to use the recording mode shortcut.
        /// </summary>
        public bool UseRecordingModeShortcut { get; set; }

        /// <summary>
        /// Key shortcut for toggling recording mode.
        /// </summary>
        public TweakKeyShortcut RecordingModeShortcut { get; set; } = new () {
            PressCtrl = true,
            PressShift = false,
            PressAlt = false,
            PressKey = KeyCode.F8,
        };

        /// <summary>
        /// Toggle recording mode.
        /// </summary>
        public void ToggleRecordingMode() {
            RecordingMode = !RecordingMode;

            // TODO: Fire an event here for other mods to be able to listen for

            // Update the visibility
            ShowOrHideElements();
        }

        [NonSerialized]
        private readonly PropertyInfo _isEditingLevelProperty =
            AccessTools.Property(typeof(ADOBase), "isEditingLevel");

        /// <summary>
        /// Update elements' visibility.
        /// </summary>
        public void ShowOrHideElements() {
            if (scrUIController.instance == null) {
                return;
            }

            var uiController = scrUIController.instance;
            var selectedProfile = RecordingMode ? RecordingProfile : PlayingProfile;

            bool hideEverything = selectedProfile.HideEverything;
            bool hideOtto = hideEverything || selectedProfile.HideOtto;
            bool hideTimingTarget = hideEverything || selectedProfile.HideTimingTarget;
            bool hideNoFail = hideEverything || selectedProfile.HideNoFailIcon;
            bool hideBeta = hideEverything || selectedProfile.HideBeta;
            bool hideTitle = hideEverything || selectedProfile.HideTitle;

            var tweakEnabled = AdofaiTweaks.IsEnabled && IsEnabled;
            hideEverything &= tweakEnabled;
            hideOtto &= tweakEnabled;
            hideTimingTarget &= tweakEnabled;
            hideNoFail &= tweakEnabled;
            hideBeta &= tweakEnabled;
            hideTitle &= tweakEnabled;

            RDC.noHud = hideEverything;

            bool isEditingLevel = (bool)_isEditingLevelProperty.GetValue(
                AdofaiTweaks.ReleaseNumber >= 94 ? null : scnEditor.instance);

            if (isEditingLevel) {
                scnEditor.instance.autoImage.enabled = !hideOtto;
                scnEditor.instance.buttonAuto.enabled = !hideOtto;
                if (scnEditor.instance?.editorDifficultySelector.gameObject.activeSelf == hideTimingTarget) {
                    scnEditor.instance.editorDifficultySelector.gameObject.SetActive(!hideTimingTarget);
                }
                if (scnEditor.instance?.buttonNoFail.gameObject.activeSelf == hideNoFail) {
                    scnEditor.instance.buttonNoFail.gameObject.SetActive(!hideNoFail);
                }
            } else {
                uiController.noFailImage.enabled = !hideNoFail;
                uiController.difficultyImage.enabled = !hideTimingTarget;
                if (uiController.difficultyContainer.gameObject.activeSelf == hideTimingTarget) {
                    uiController.difficultyContainer.gameObject.SetActive(!hideTimingTarget);
                }
                if (uiController.difficultyFadeContainer.gameObject.activeSelf == hideTimingTarget) {
                    uiController.difficultyFadeContainer.gameObject.SetActive(!hideTimingTarget);
                }
            }

            if ((SteamIntegration.Instance?.initialized ?? false) && !string.IsNullOrEmpty(GCS.steamBranchName))
            {
                scrEnableIfBeta enableIfBeta =
                    Resources.FindObjectsOfTypeAll<scrEnableIfBeta>().FirstOrDefault();
                if (enableIfBeta && enableIfBeta.gameObject.activeSelf == hideBeta) {
                    enableIfBeta.gameObject.SetActive(!hideBeta);
                }
            }

            if (uiController.txtLevelName.gameObject.activeSelf == hideTitle) {
                uiController.txtLevelName.gameObject.SetActive(!hideTitle);
            }
        }

        /// <summary>
        /// Hides all UI elements. OLD SETTINGS. ONLY FOR MIGRATING.
        /// </summary>
        public bool HideEverything { get; set; }

        /// <summary>
        /// Hides judgments (Perfect, EPerfect, etc.). OLD SETTINGS. ONLY FOR
        /// MIGRATING.
        /// </summary>
        public bool HideJudgment { get; set; }

        /// <summary>
        /// Hides miss indicators (the circled X icons). OLD SETTINGS. ONLY FOR
        /// MIGRATING.
        /// </summary>
        public bool HideMissIndicators { get; set; }

        /// <summary>
        /// Hides the song title and artist. OLD SETTINGS. ONLY FOR MIGRATING.
        /// </summary>
        public bool HideTitle { get; set; }

        /// <summary>
        /// Hides Otto and the timing target icon in the bottom right. OLD
        /// SETTINGS. ONLY FOR MIGRATING.
        /// </summary>
        public bool HideOtto { get; set; }

        /// <summary>
        /// Hides the "Beta Build" text. OLD SETTINGS. ONLY FOR MIGRATING.
        /// </summary>
        public bool HideBeta { get; set; }
    }
}
