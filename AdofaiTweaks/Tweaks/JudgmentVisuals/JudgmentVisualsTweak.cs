using AdofaiTweaks.Core;
using AdofaiTweaks.Core.Attributes;
using AdofaiTweaks.Strings;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.JudgmentVisuals
{
    [RegisterTweak(
        id: "judgment_visuals",
        settingsType: typeof(JudgmentVisualsSettings),
        patchesType: typeof(JudgmentVisualsPatches))]
    public class JudgmentVisualsTweak : Tweak
    {
        public override string Name =>
            TweakStrings.Get(TranslationKeys.JudgmentVisuals.NAME);

        public override string Description =>
            TweakStrings.Get(TranslationKeys.JudgmentVisuals.DESCRIPTION);

        [SyncTweakSettings]
        private JudgmentVisualsSettings Settings { get; set; }

        private GameObject errorMeterObj;

        public override void OnSettingsGUI() {
            Settings.ShowHitErrorMeter =
                GUILayout.Toggle(
                    Settings.ShowHitErrorMeter,
                    TweakStrings.Get(TranslationKeys.JudgmentVisuals.SHOW_HIT_ERROR_METER));

            if (Settings.ShowHitErrorMeter) {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20f);
                GUILayout.BeginVertical();

                // Scale slider
                GUILayout.BeginHorizontal();
                GUILayout.Label(
                    TweakStrings.Get(TranslationKeys.JudgmentVisuals.ERROR_METER_SCALE));
                GUILayout.Space(4f);
                float newScale =
                    GUILayout.HorizontalSlider(
                        Settings.ErrorMeterScale, 0.25f, 4f, GUILayout.Width(200f));
                newScale *= 4;
                newScale = Mathf.Round(newScale);
                newScale /= 4;
                if (Settings.ErrorMeterScale != newScale) {
                    Settings.ErrorMeterScale = newScale;
                    HitErrorMeter.Instance.Scale = newScale;
                }
                GUILayout.Space(8f);
                GUILayout.Label(Settings.ErrorMeterScale + "x");
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                // Tick life slider
                GUILayout.BeginHorizontal();
                GUILayout.Label(
                    TweakStrings.Get(TranslationKeys.JudgmentVisuals.ERROR_METER_TICK_LIFE));
                GUILayout.Space(4f);
                float newLife =
                    GUILayout.HorizontalSlider(
                        Settings.ErrorMeterTickLife, 1f, 10f, GUILayout.Width(200f));
                newLife = Mathf.Round(newLife);
                if (Settings.ErrorMeterTickLife != newLife) {
                    Settings.ErrorMeterTickLife = newLife;
                }
                GUILayout.Space(8f);
                GUILayout.Label(
                    TweakStrings.Get(
                        TranslationKeys.JudgmentVisuals.ERROR_METER_TICK_SECONDS,
                        Settings.ErrorMeterTickLife));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

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

                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }

            Settings.HidePerfects =
                GUILayout.Toggle(
                    Settings.HidePerfects,
                    TweakStrings.Get(TranslationKeys.JudgmentVisuals.HIDE_PERFECTS));
        }

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

        public override void OnEnable() {
            errorMeterObj = new GameObject();
            GameObject.DontDestroyOnLoad(errorMeterObj);
            HitErrorMeter errorMeter = errorMeterObj.AddComponent<HitErrorMeter>();
            errorMeter.Settings = Settings;
        }

        public override void OnDisable() {
            GameObject.Destroy(errorMeterObj);
        }
    }
}
