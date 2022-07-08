using AdofaiTweaks.Core;
using AdofaiTweaks.Core.Attributes;
using AdofaiTweaks.Strings;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AdofaiTweaks.Tweaks.HideUiElements
{
    /// <summary>
    /// A tweak for hiding certain elements of the UI.
    /// </summary>
    [RegisterTweak(
        id: "hide_ui_elements",
        settingsType: typeof(HideUiElementsSettings),
        patchesType: typeof(HideUiElementsPatches))]
    public class HideUiElementsTweak : Tweak
    {
        /// <inheritdoc/>
        public override string Name =>
            TweakStrings.Get(TranslationKeys.HideUiElements.NAME);

        /// <inheritdoc/>
        public override string Description =>
            TweakStrings.Get(TranslationKeys.HideUiElements.DESCRIPTION);

        [SyncTweakSettings]
        private HideUiElementsSettings Settings { get; set; }

        private HideUiElementsProfile SelectedProfile =>
            Settings.RecordingMode ? Settings.RecordingProfile : Settings.PlayingProfile;

        /// <inheritdoc/>
        public override void OnSettingsGUI() {
            bool newVal;

            newVal = GUILayout.Toggle(
                Settings.RecordingMode,
                TweakStrings.Get(TranslationKeys.HideUiElements.RECORDING_MODE));
            if (newVal != Settings.RecordingMode)
            {
                Settings.ToggleRecordingMode();
            }

            newVal = GUILayout.Toggle(
                Settings.UseRecordingModeShortcut,
                TweakStrings.Get(TranslationKeys.HideUiElements.USE_RECORDING_MODE_SHORTCUT));
            if (newVal != Settings.UseRecordingModeShortcut) {
                Settings.UseRecordingModeShortcut = newVal;
            }

            if (newVal)
            {
                Settings.RecordingModeShortcut.OnGUI(TweakStrings.Get(TranslationKeys.HideUiElements.RECORDING_MODE_SHORTCUT));
            }

            GUILayout.Space(8f);

            newVal = GUILayout.Toggle(
                SelectedProfile.HideEverything,
                TweakStrings.Get(TranslationKeys.HideUiElements.EVERYTHING));
            if (newVal != SelectedProfile.HideEverything) {
                SelectedProfile.HideEverything = newVal;
                Settings.ShowOrHideElements();
            }
            if (SelectedProfile.HideEverything) {
                return;
            }

            GUILayout.Space(8f);

            SelectedProfile.HideJudgment =
                GUILayout.Toggle(
                    SelectedProfile.HideJudgment,
                    TweakStrings.Get(
                        TranslationKeys.HideUiElements.JUDGE_TEXT,
                        TweakStrings.GetRDString("HitMargin." + HitMargin.Perfect),
                        TweakStrings.GetRDString("HitMargin." + HitMargin.EarlyPerfect)));

            SelectedProfile.HideMissIndicators =
                GUILayout.Toggle(
                    SelectedProfile.HideMissIndicators,
                    TweakStrings.Get(TranslationKeys.HideUiElements.MISSES));

            newVal = GUILayout.Toggle(
                SelectedProfile.HideTitle, TweakStrings.Get(TranslationKeys.HideUiElements.SONG_TITLE));
            if (newVal != SelectedProfile.HideTitle) {
                SelectedProfile.HideTitle = newVal;
                Settings.ShowOrHideElements();
            }

            newVal = GUILayout.Toggle(
                SelectedProfile.HideOtto, TweakStrings.Get(TranslationKeys.HideUiElements.AUTO));
            if (newVal != SelectedProfile.HideOtto) {
                SelectedProfile.HideOtto = newVal;
                Settings.ShowOrHideElements();
            }

            newVal = GUILayout.Toggle(
                SelectedProfile.HideBeta, TweakStrings.Get(TranslationKeys.HideUiElements.BETA_BUILD));
            if (newVal != SelectedProfile.HideBeta) {
                SelectedProfile.HideBeta = newVal;
                Settings.ShowOrHideElements();
            }

            newVal = GUILayout.Toggle(
                SelectedProfile.HideResult, TweakStrings.Get(TranslationKeys.HideUiElements.RESULT_TEXT));
            if (newVal != SelectedProfile.HideResult) {
                SelectedProfile.HideResult = newVal;
                Settings.ShowOrHideElements();
            }

            newVal = GUILayout.Toggle(
                SelectedProfile.HideHitErrorMeter, TweakStrings.Get(TranslationKeys.HideUiElements.HIT_ERROR_METER));
            if (newVal != SelectedProfile.HideHitErrorMeter) {
                SelectedProfile.HideHitErrorMeter = newVal;
                Settings.ShowOrHideElements();
            }

            newVal = GUILayout.Toggle(
                SelectedProfile.HideLastFloorFlash, TweakStrings.Get(TranslationKeys.HideUiElements.LAST_FLOOR_FLASH));
            if (newVal != SelectedProfile.HideLastFloorFlash) {
                SelectedProfile.HideLastFloorFlash = newVal;
                Settings.ShowOrHideElements();
            }
        }

        /// <inheritdoc/>
        public override void OnEnable() {
            SceneManager.activeSceneChanged += ChangedActiveScene;
            MigrateOldSettings();
            Settings.ShowOrHideElements();
        }

        /// <summary>
        /// Migrates old KeyLimiter settings to a KeyViewer profile if there are
        /// settings to migrate.
        /// TODO: Delete this after a few releases.
        /// </summary>
        private void MigrateOldSettings() {
            // Check if there are settings to migrate
            if (!Settings.HideEverything &&
                !Settings.HideJudgment &&
                !Settings.HideMissIndicators &&
                !Settings.HideTitle &&
                !Settings.HideOtto &&
                !Settings.HideBeta) {
                return;
            }

            // Copy into new profile
            var profile = new HideUiElementsProfile {
                HideEverything = Settings.HideEverything,
                HideJudgment = Settings.HideJudgment,
                HideMissIndicators = Settings.HideMissIndicators,
                HideTitle = Settings.HideTitle,
                HideOtto = Settings.HideOtto,
                HideBeta = Settings.HideBeta,
            };

            // Set playing profile to migrated profile
            Settings.PlayingProfile = profile;

            // Clear old settings
            Settings.HideEverything =
                Settings.HideJudgment =
                    Settings.HideMissIndicators =
                        Settings.HideTitle =
                            Settings.HideOtto =
                                Settings.HideBeta = false;
        }

        /// <inheritdoc/>
        public override void OnDisable() {
            SceneManager.activeSceneChanged -= ChangedActiveScene;
            Settings.ShowOrHideElements();
        }

        private void ChangedActiveScene(Scene current, Scene next) {
            Settings.ShowOrHideElements();
        }
    }
}
