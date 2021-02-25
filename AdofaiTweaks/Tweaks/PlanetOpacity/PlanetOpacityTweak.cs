using AdofaiTweaks.Core;
using AdofaiTweaks.Core.Attributes;
using AdofaiTweaks.Strings;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.PlanetOpacity
{
    [RegisterTweak(
        id: "planet_opacity",
        settingsType: typeof(PlanetOpacitySettings),
        patchesType: typeof(PlanetOpacityPatches))]
    public class PlanetOpacityTweak : Tweak
    {
        public override string Name =>
            TweakStrings.Get(TranslationKeys.PlanetOpacity.NAME);

        public override string Description =>
            TweakStrings.Get(TranslationKeys.PlanetOpacity.DESCRIPTION);

        private scrPlanet RedPlanet { get => Object.FindObjectOfType<scrController>()?.redPlanet; }
        private scrPlanet BluePlanet { get => Object.FindObjectOfType<scrController>()?.bluePlanet; }

        [SyncTweakSettings]
        private PlanetOpacitySettings Settings { get; set; }

        public override void OnSettingsGUI() {
            float newOpacity;

            GUILayout.BeginHorizontal();
            GUILayout.Label(
                TweakStrings.Get(TranslationKeys.PlanetOpacity.PLANET_ONE), GUILayout.Width(110f));
            newOpacity =
                GUILayout.HorizontalSlider(
                    Settings.SettingsOpacity1, 0, 100, GUILayout.Width(200f));
            newOpacity = Mathf.Round(newOpacity);
            if (newOpacity != Settings.SettingsOpacity1) {
                Settings.SettingsOpacity1 = newOpacity;
                UpdatePlanetColors();
            }
            GUILayout.Label(Mathf.RoundToInt(Settings.SettingsOpacity1) + "%");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(
                TweakStrings.Get(TranslationKeys.PlanetOpacity.PLANET_TWO), GUILayout.Width(110f));
            newOpacity =
                GUILayout.HorizontalSlider(
                    Settings.SettingsOpacity2, 0, 100, GUILayout.Width(200f));
            newOpacity = Mathf.Round(newOpacity);
            if (newOpacity != Settings.SettingsOpacity2) {
                Settings.SettingsOpacity2 = newOpacity;
                UpdatePlanetColors();
            }
            GUILayout.Label(Mathf.RoundToInt(Settings.SettingsOpacity2) + "%");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        public override void OnEnable() {
            UpdatePlanetColors();
        }

        public override void OnDisable() {
            UpdatePlanetColors();
        }

        private void UpdatePlanetColors() {
            if (RedPlanet != null) {
                RedPlanet.LoadPlanetColor();
            }
            if (BluePlanet != null) {
                BluePlanet.LoadPlanetColor();
            }
        }
    }
}
