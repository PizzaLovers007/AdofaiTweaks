using System;
using System.Linq;
using System.Reflection;
using AdofaiTweaks.Core;
using HarmonyLib;
using Steamworks;
using UnityEngine;

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
            bool hideBeta = hideEverything || selectedProfile.HideBeta;
            bool hideTitle = hideEverything || selectedProfile.HideTitle;

            var tweakEnabled = AdofaiTweaks.IsEnabled && IsEnabled;
            hideEverything &= tweakEnabled;
            hideOtto &= tweakEnabled;
            hideBeta &= tweakEnabled;
            hideTitle &= tweakEnabled;

            RDC.noHud = hideEverything;

            bool isEditingLevel = (bool)_isEditingLevelProperty.GetValue(
                AdofaiTweaks.ReleaseNumber >= 94 ? null : scnEditor.instance);

            if (isEditingLevel) {
                if (scnEditor.instance?.ottoCanvas.gameObject.activeSelf == hideOtto) {
                    scnEditor.instance.ottoCanvas.gameObject.SetActive(!hideOtto);
                }
            } else {
                uiController.difficultyImage.enabled = !hideOtto;
                if (uiController.difficultyContainer.gameObject.activeSelf == hideOtto) {
                    uiController.difficultyContainer.gameObject.SetActive(!hideOtto);
                }
                if (uiController.difficultyFadeContainer.gameObject.activeSelf == hideOtto) {
                    uiController.difficultyFadeContainer.gameObject.SetActive(!hideOtto);
                }
            }

            if (SteamAPI.Init()) {
                bool isBeta = SteamApps.GetCurrentBetaName(out _, 20);
                scrEnableIfBeta enableIfBeta =
                    Resources.FindObjectsOfTypeAll<scrEnableIfBeta>().FirstOrDefault();
                if (isBeta && enableIfBeta && enableIfBeta.gameObject.activeSelf == hideBeta) {
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
