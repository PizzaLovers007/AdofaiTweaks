using AdofaiTweaks.Core;
using AdofaiTweaks.Core.Attributes;
using AdofaiTweaks.Strings;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.JudgmentVisuals
{
    /// <summary>
    /// A tweak for adjusting the visuals for judgments.
    /// </summary>
    [RegisterTweak(
        id: "judgment_visuals",
        settingsType: typeof(JudgmentVisualsSettings),
        patchesType: typeof(JudgmentVisualsPatches))]
    public class JudgmentVisualsTweak : Tweak
    {
        /// <inheritdoc/>
        public override string Name =>
            TweakStrings.Get(TranslationKeys.JudgmentVisuals.NAME);

        /// <inheritdoc/>
        public override string Description =>
            TweakStrings.Get(TranslationKeys.JudgmentVisuals.DESCRIPTION);

        [SyncTweakSettings]
        private JudgmentVisualsSettings Settings { get; set; }

        private GameObject errorMeterObj;

        /// <inheritdoc/>
        public override void OnSettingsGUI() {
            Settings.ShowHitErrorMeter =
                GUILayout.Toggle(
                    Settings.ShowHitErrorMeter,
                    TweakStrings.Get(TranslationKeys.JudgmentVisuals.SHOW_HIT_ERROR_METER));

            if (Settings.ShowHitErrorMeter) {
                MoreGUILayout.BeginIndent();

                // Scale slider
                float newScale =
                    MoreGUILayout.NamedSlider(
                        TweakStrings.Get(TranslationKeys.JudgmentVisuals.ERROR_METER_SCALE),
                        Settings.ErrorMeterScale,
                        0.25f,
                        4f,
                        200f,
                        roundNearest: 0.125f,
                        valueFormat: "{0}x");
                if (newScale != Settings.ErrorMeterScale) {
                    Settings.ErrorMeterScale = newScale;
                    HitErrorMeter.Instance.Scale = newScale;
                }

                // Tick life slider
                Settings.ErrorMeterTickLife =
                    MoreGUILayout.NamedSlider(
                        TweakStrings.Get(TranslationKeys.JudgmentVisuals.ERROR_METER_TICK_LIFE),
                        Settings.ErrorMeterTickLife,
                        1f,
                        10f,
                        200f,
                        roundNearest: 1f,
                        valueFormat: TweakStrings.Get(
                            TranslationKeys.JudgmentVisuals.ERROR_METER_TICK_SECONDS));

                // Sensitivity slider
                GUILayout.BeginHorizontal();
                GUILayout.Label(
                    TweakStrings.Get(TranslationKeys.JudgmentVisuals.ERROR_METER_SENSITIVITY));
                GUILayout.Space(16f);
                GUILayout.Label(
                    TweakStrings.Get(TranslationKeys.JudgmentVisuals.ERROR_METER_MORE_STABLE));
                GUILayout.Space(8f);
                float newSens =
                    GUILayout.HorizontalSlider(
                        Settings.ErrorMeterSensitivity, 0.05f, 0.5f, GUILayout.Width(200f));
                newSens *= 20;
                newSens = Mathf.Round(newSens);
                newSens /= 20;
                if (Settings.ErrorMeterSensitivity != newSens) {
                    Settings.ErrorMeterSensitivity = newSens;
                }
                GUILayout.Space(8f);
                GUILayout.Label(
                    TweakStrings.Get(TranslationKeys.JudgmentVisuals.ERROR_METER_LESS_STABLE));
                GUILayout.Space(8f);
                GUILayout.Label($"({Settings.ErrorMeterSensitivity})");
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                MoreGUILayout.EndIndent();
            }

            Settings.HidePerfects =
                GUILayout.Toggle(
                    Settings.HidePerfects,
                    TweakStrings.Get(TranslationKeys.JudgmentVisuals.HIDE_PERFECTS));
        }

        /// <inheritdoc/>
        public override void OnUpdate(float deltaTime) {
            UpdateErrorMeter();
        }

        private void UpdateErrorMeter() {
            if (!scrConductor.instance || !scrController.instance || !errorMeterObj) {
                return;
            }
            bool playing = !scrController.instance.paused && scrConductor.instance.isGameWorld;
            bool shouldShowMeter = Settings.ShowHitErrorMeter && playing;
            if (shouldShowMeter != errorMeterObj.activeSelf) {
                errorMeterObj.SetActive(shouldShowMeter);
            }
        }

        /// <inheritdoc/>
        public override void OnEnable() {
            errorMeterObj = new GameObject();
            GameObject.DontDestroyOnLoad(errorMeterObj);
            HitErrorMeter errorMeter = errorMeterObj.AddComponent<HitErrorMeter>();
            errorMeter.Settings = Settings;
        }

        /// <inheritdoc/>
        public override void OnUnpatch() {
            GameObject.Destroy(errorMeterObj);
        }
    }
}
