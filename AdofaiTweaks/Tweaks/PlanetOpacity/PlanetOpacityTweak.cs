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

        private scrPlanet RedPlanet { get => scrController.instance?.planetRed; }
        private scrPlanet BluePlanet { get => scrController.instance?.planetBlue; }

        [SyncTweakSettings]
        private PlanetOpacitySettings Settings { get; set; }

        /// <inheritdoc/>
        public override void OnEnable() {
            MigrateOldSettings();
        }

        private void MigrateOldSettings() {
            if (Settings.SettingsOpacity1 != PlanetOpacitySettings.MIGRATED_VALUE) {
                Settings.PlanetOpacity1.Body = Settings.SettingsOpacity1;
                Settings.PlanetOpacity1.Tail = Settings.SettingsOpacity1;
                Settings.PlanetOpacity1.Ring = Settings.SettingsOpacity1;
                Settings.SettingsOpacity1 = PlanetOpacitySettings.MIGRATED_VALUE;
            }
            if (Settings.SettingsOpacity2 != PlanetOpacitySettings.MIGRATED_VALUE) {
                Settings.PlanetOpacity2.Body = Settings.SettingsOpacity2;
                Settings.PlanetOpacity2.Tail = Settings.SettingsOpacity2;
                Settings.PlanetOpacity2.Ring = Settings.SettingsOpacity2;
                Settings.SettingsOpacity2 = PlanetOpacitySettings.MIGRATED_VALUE;
            }
        }

        /// <inheritdoc/>
        public override void OnSettingsGUI() {
            GUILayout.BeginHorizontal();
            MoreGUILayout.LabelPair(
                TweakStrings.Get(TranslationKeys.PlanetOpacity.PLANET_ONE),
                TweakStrings.Get(TranslationKeys.PlanetOpacity.PLANET_TWO),
                200f);
            GUILayout.EndHorizontal();

            MoreGUILayout.BeginIndent();

            float newOpacity1, newOpacity2;
            (newOpacity1, newOpacity2) =
                MoreGUILayout.NamedSliderPair(
                    TweakStrings.Get(TranslationKeys.PlanetOpacity.BODY),
                    TweakStrings.Get(TranslationKeys.PlanetOpacity.BODY),
                    Settings.PlanetOpacity1.Body,
                    Settings.PlanetOpacity2.Body,
                    0f,
                    100f,
                    150f,
                    roundNearest: 1,
                    labelWidth: 80f,
                    valueFormat: "{0}%");
            if (newOpacity1 != Settings.PlanetOpacity1.Body
                || newOpacity2 != Settings.PlanetOpacity2.Body) {
                Settings.PlanetOpacity1.Body = newOpacity1;
                Settings.PlanetOpacity2.Body = newOpacity2;
                UpdatePlanetColors();
            }

            (newOpacity1, newOpacity2) =
                MoreGUILayout.NamedSliderPair(
                    TweakStrings.Get(TranslationKeys.PlanetOpacity.TAIL),
                    TweakStrings.Get(TranslationKeys.PlanetOpacity.TAIL),
                    Settings.PlanetOpacity1.Tail,
                    Settings.PlanetOpacity2.Tail,
                    0f,
                    100f,
                    150f,
                    roundNearest: 1,
                    labelWidth: 80f,
                    valueFormat: "{0}%");
            if (newOpacity1 != Settings.PlanetOpacity1.Tail
                || newOpacity2 != Settings.PlanetOpacity2.Tail) {
                Settings.PlanetOpacity1.Tail = newOpacity1;
                Settings.PlanetOpacity2.Tail = newOpacity2;
                UpdatePlanetColors();
            }

            (newOpacity1, newOpacity2) =
                MoreGUILayout.NamedSliderPair(
                    TweakStrings.Get(TranslationKeys.PlanetOpacity.RING),
                    TweakStrings.Get(TranslationKeys.PlanetOpacity.RING),
                    Settings.PlanetOpacity1.Ring,
                    Settings.PlanetOpacity2.Ring,
                    0f,
                    100f,
                    150f,
                    roundNearest: 1,
                    labelWidth: 80f,
                    valueFormat: "{0}%");
            if (newOpacity1 != Settings.PlanetOpacity1.Ring
                || newOpacity2 != Settings.PlanetOpacity2.Ring) {
                Settings.PlanetOpacity1.Ring = newOpacity1;
                Settings.PlanetOpacity2.Ring = newOpacity2;
                UpdatePlanetColors();
            }

            MoreGUILayout.EndIndent();
        }

        /// <inheritdoc/>
        public override void OnPatch() {
            UpdatePlanetColors();
        }

        /// <inheritdoc/>
        public override void OnUnpatch() {
            UpdatePlanetColors();
        }

        private void UpdatePlanetColors() {
            if (RedPlanet != null) {

                RedPlanet.planetRenderer.LoadPlanetColor(true);
            }
            if (BluePlanet != null) {
                BluePlanet.planetRenderer.LoadPlanetColor(false);
            }
        }
    }
}
