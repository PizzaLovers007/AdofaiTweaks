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
                GUILayout.Toggle(Settings.ShowHitErrorMeter, "Show hit error meter");
        }

        public override void OnUpdate(float deltaTime) {
            UpdateErrorMeter();
        }

        private void UpdateErrorMeter() {
            if (!Settings.ShowHitErrorMeter || !scrConductor.instance || !scrController.instance) {
                return;
            }
            bool playing = !scrController.instance.paused && scrConductor.instance.isGameWorld;
            if (playing != errorMeterObj.activeSelf) {
                errorMeterObj.SetActive(playing);
            }
            if (Input.GetKeyDown(KeyCode.Backspace)) {
                Settings.ErrorMeter.Reset();
            }
        }

        public override void OnEnable() {
            errorMeterObj = new GameObject();
            GameObject.DontDestroyOnLoad(errorMeterObj);
            Settings.ErrorMeter = errorMeterObj.AddComponent<HitErrorMeter>();
        }

        public override void OnDisable() {
            GameObject.Destroy(errorMeterObj);
        }
    }
}
