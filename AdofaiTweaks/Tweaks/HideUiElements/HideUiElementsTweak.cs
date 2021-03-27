using System.Linq;
using AdofaiTweaks.Core;
using AdofaiTweaks.Core.Attributes;
using AdofaiTweaks.Strings;
using Steamworks;
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

        /// <inheritdoc/>
        public override void OnSettingsGUI() {
            bool newVal;

            newVal = GUILayout.Toggle(
                Settings.HideEverything,
                TweakStrings.Get(TranslationKeys.HideUiElements.EVERYTHING));
            if (newVal != Settings.HideEverything) {
                Settings.HideEverything = newVal;
                ShowOrHideElements();
            }
            if (Settings.HideEverything) {
                return;
            }

            GUILayout.Space(8f);

            Settings.HideJudgment =
                GUILayout.Toggle(
                    Settings.HideJudgment,
                    TweakStrings.Get(
                        TranslationKeys.HideUiElements.JUDGE_TEXT,
                        RDString.Get("HitMargin." + HitMargin.Perfect),
                        RDString.Get("HitMargin." + HitMargin.EarlyPerfect)));

            Settings.HideMissIndicators =
                GUILayout.Toggle(
                    Settings.HideMissIndicators,
                    TweakStrings.Get(TranslationKeys.HideUiElements.MISSES));

            newVal = GUILayout.Toggle(
                Settings.HideTitle, TweakStrings.Get(TranslationKeys.HideUiElements.SONG_TITLE));
            if (newVal != Settings.HideTitle) {
                Settings.HideTitle = newVal;
                ShowOrHideElements();
            }

            newVal = GUILayout.Toggle(
                Settings.HideOtto, TweakStrings.Get(TranslationKeys.HideUiElements.AUTO));
            if (newVal != Settings.HideOtto) {
                Settings.HideOtto = newVal;
                ShowOrHideElements();
            }

            newVal = GUILayout.Toggle(
                Settings.HideBeta, TweakStrings.Get(TranslationKeys.HideUiElements.BETA_BUILD));
            if (newVal != Settings.HideBeta) {
                Settings.HideBeta = newVal;
                ShowOrHideElements();
            }
        }

        /// <inheritdoc/>
        public override void OnEnable() {
            SceneManager.activeSceneChanged += ChangedActiveScene;
            ShowOrHideElements();
        }

        /// <inheritdoc/>
        public override void OnDisable() {
            SceneManager.activeSceneChanged -= ChangedActiveScene;
            ShowOrHideElements();
        }

        private void ChangedActiveScene(Scene current, Scene next) {
            ShowOrHideElements();
        }

        private void ShowOrHideElements() {
            if (scrUIController.instance == null) {
                return;
            }
            scrUIController uiController = scrUIController.instance;

            bool hideEverything = Settings.HideEverything;
            bool hideOtto = hideEverything || Settings.HideOtto;
            bool hideBeta = hideEverything || Settings.HideBeta;
            bool hideTitle = hideEverything || Settings.HideTitle;
            hideEverything &= AdofaiTweaks.IsEnabled && Settings.IsEnabled;
            hideOtto &= AdofaiTweaks.IsEnabled && Settings.IsEnabled;
            hideBeta &= AdofaiTweaks.IsEnabled && Settings.IsEnabled;
            hideTitle &= AdofaiTweaks.IsEnabled && Settings.IsEnabled;

            RDC.noHud = hideEverything;

            scnEditor editor = Object.FindObjectOfType<scnEditor>();
            if (scrController.instance.isEditingLevel) {
                if (editor?.ottoCanvas.gameObject.activeSelf == hideOtto) {
                    editor.ottoCanvas.gameObject.SetActive(!hideOtto);
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
    }
}
