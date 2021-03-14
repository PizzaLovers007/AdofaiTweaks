using AdofaiTweaks.Core;
using AdofaiTweaks.Core.Attributes;
using AdofaiTweaks.Strings;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.PlanetOpacity
{
    /// <summary>
    /// A tweak for changing the opacities of the planets.
    /// </summary>
    [RegisterTweak(
        id: "planet_opacity",
        settingsType: typeof(PlanetOpacitySettings),
        patchesType: typeof(PlanetOpacityPatches))]
    public class PlanetOpacityTweak : Tweak
    {
        /// <inheritdoc/>
        public override string Name =>
            TweakStrings.Get(TranslationKeys.PlanetOpacity.NAME);

        /// <inheritdoc/>
        public override string Description =>
            TweakStrings.Get(TranslationKeys.PlanetOpacity.DESCRIPTION);

        private scrPlanet RedPlanet { get => Object.FindObjectOfType<scrController>()?.redPlanet; }
        private scrPlanet BluePlanet { get => Object.FindObjectOfType<scrController>()?.bluePlanet; }

        [SyncTweakSettings]
        private PlanetOpacitySettings Settings { get; set; }

        /// <inheritdoc/>
        public override void OnSettingsGUI() {
            float newOpacity;

            newOpacity =
                MoreGUILayout.NamedSlider(
                    TweakStrings.Get(TranslationKeys.PlanetOpacity.PLANET_ONE),
                    Settings.SettingsOpacity1,
                    0f,
                    100f,
                    200f,
                    roundNearest: 1,
                    valueFormat: "{0}%");
            if (newOpacity != Settings.SettingsOpacity1) {
                Settings.SettingsOpacity1 = newOpacity;
                UpdatePlanetColors();
            }

            newOpacity =
                MoreGUILayout.NamedSlider(
                    TweakStrings.Get(TranslationKeys.PlanetOpacity.PLANET_TWO),
                    Settings.SettingsOpacity2,
                    0f,
                    100f,
                    200f,
                    roundNearest: 1,
                    valueFormat: "{0}%");
            if (newOpacity != Settings.SettingsOpacity2) {
                Settings.SettingsOpacity2 = newOpacity;
                UpdatePlanetColors();
            }
        }

        /// <inheritdoc/>
        public override void OnEnable() {
            UpdatePlanetColors();
        }

        /// <inheritdoc/>
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
