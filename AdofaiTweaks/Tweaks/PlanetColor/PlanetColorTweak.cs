using System.Reflection;
using AdofaiTweaks.Core;
using AdofaiTweaks.Core.Attributes;
using AdofaiTweaks.Strings;
using AdofaiTweaks.Utils;
using HarmonyLib;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.PlanetColor
{
    /// <summary>
    /// A tweak for changing the planet colors to a custom color.
    /// </summary>
    [RegisterTweak(
        id: "planet_color",
        settingsType: typeof(PlanetColorSettings),
        patchesType: typeof(PlanetColorPatches))]
    public class PlanetColorTweak : Tweak
    {
        /// <inheritdoc/>
        public override string Name =>
            TweakStrings.Get(TranslationKeys.PlanetColor.NAME);

        /// <inheritdoc/>
        public override string Description =>
            TweakStrings.Get(TranslationKeys.PlanetColor.DESCRIPTION);

        [SyncTweakSettings]
        private PlanetColorSettings Settings { get; set; }

        /// <inheritdoc/>
        public override void OnSettingsGUI() {
            Color newBody, newTail;
            string newHex, newTailHex;

            bool color1Enabled =
                GUILayout.Toggle(
                    Settings.Color1Enabled,
                    TweakStrings.Get(TranslationKeys.PlanetColor.PLANET_ONE));
            if (color1Enabled != Settings.Color1Enabled) {
                Settings.Color1Enabled = color1Enabled;
                UpdatePlanetColors();
            }

            if (Settings.Color1Enabled) {
                MoreGUILayout.BeginIndent();

                MoreGUILayout.LabelPair(
                    TweakStrings.Get(TranslationKeys.PlanetColor.BODY),
                    TweakStrings.Get(TranslationKeys.PlanetColor.TAIL),
                    200f);
                MoreGUILayout.BeginIndent();

                // Planet 1 RGB sliders
                (newBody, newTail) =
                    MoreGUILayout.ColorRgbSlidersPair(Settings.Color1, Settings.TailColor1);
                if (Settings.Color1 != newBody) {
                    Settings.Color1 = newBody;
                    UpdatePlanetColors();
                }
                if (Settings.TailColor1 != newTail) {
                    Settings.TailColor1 = newTail;
                    UpdatePlanetColors();
                }

                // Planet 1 Hex
                (newHex, newTailHex) =
                    MoreGUILayout.NamedTextFieldPair(
                        "Hex:", "Hex:", Settings.Color1Hex, Settings.TailColor1Hex, 100f, 40f);
                if (newHex != Settings.Color1Hex
                    && ColorUtility.TryParseHtmlString($"#{newHex}", out newBody)) {
                    Settings.Color1 = newBody;
                    UpdatePlanetColors();
                }
                if (newTailHex != Settings.TailColor1Hex
                    && ColorUtility.TryParseHtmlString($"#{newTailHex}", out newTail)) {
                    Settings.TailColor1 = newTail;
                    UpdatePlanetColors();
                }
                Settings.Color1Hex = newHex;
                Settings.TailColor1Hex = newTailHex;

                MoreGUILayout.EndIndent();

                MoreGUILayout.EndIndent();
            }

            GUILayout.Space(8f);

            bool color2Enabled =
                GUILayout.Toggle(
                    Settings.Color2Enabled,
                    TweakStrings.Get(TranslationKeys.PlanetColor.PLANET_TWO));
            if (color2Enabled != Settings.Color2Enabled) {
                Settings.Color2Enabled = color2Enabled;
                UpdatePlanetColors();
            }

            if (Settings.Color2Enabled) {
                MoreGUILayout.BeginIndent();

                MoreGUILayout.LabelPair(
                    TweakStrings.Get(TranslationKeys.PlanetColor.BODY),
                    TweakStrings.Get(TranslationKeys.PlanetColor.TAIL),
                    200f);
                MoreGUILayout.BeginIndent();

                // Planet 2 RGB sliders
                (newBody, newTail) =
                    MoreGUILayout.ColorRgbSlidersPair(Settings.Color2, Settings.TailColor2);
                if (Settings.Color2 != newBody) {
                    Settings.Color2 = newBody;
                    UpdatePlanetColors();
                }
                if (Settings.TailColor2 != newTail) {
                    Settings.TailColor2 = newTail;
                    UpdatePlanetColors();
                }

                // Planet 2 Hex
                (newHex, newTailHex) =
                    MoreGUILayout.NamedTextFieldPair(
                        "Hex:", "Hex:", Settings.Color2Hex, Settings.TailColor2Hex, 100f, 40f);
                if (newHex != Settings.Color2Hex
                    && ColorUtility.TryParseHtmlString($"#{newHex}", out newBody)) {
                    Settings.Color2 = newBody;
                    UpdatePlanetColors();
                }
                if (newTailHex != Settings.TailColor2Hex
                    && ColorUtility.TryParseHtmlString($"#{newTailHex}", out newTail)) {
                    Settings.TailColor2 = newTail;
                    UpdatePlanetColors();
                }
                Settings.Color2Hex = newHex;
                Settings.TailColor2Hex = newTailHex;

                MoreGUILayout.EndIndent();

                MoreGUILayout.EndIndent();
            }
        }

        /// <inheritdoc/>
        public override void OnPatch() {
            UpdatePlanetColors();
        }

        /// <inheritdoc/>
        public override void OnUnpatch() {
            UpdatePlanetColors();
        }


        private static void LoadPlanetColorWithRenderer(scrPlanet planet) {
            planet.planetRenderer.LoadPlanetColor(planet == PlanetGetter.RedPlanet);
        }

        private static readonly MethodInfo ScrPlanetLoadPlanetColorMethod =
            AccessTools.Method(typeof(scrPlanet), "LoadPlanetColor");

        private static void LoadPlanetColor(scrPlanet planet) {
            ScrPlanetLoadPlanetColorMethod.Invoke(planet, []);
        }

        private static void UpdatePlanetColors() {
            var redPlanet = PlanetGetter.RedPlanet;
            var bluePlanet = PlanetGetter.BluePlanet;

            if (redPlanet != null) {
                if (AdofaiTweaks.ReleaseNumber >= 128) {
                    LoadPlanetColorWithRenderer(redPlanet);
                }
                else {
                    LoadPlanetColor(redPlanet);
                }
            }

            if (bluePlanet != null) {
                if (AdofaiTweaks.ReleaseNumber >= 128) {
                    LoadPlanetColorWithRenderer(bluePlanet);
                }
                else {
                    LoadPlanetColor(bluePlanet);
                }
            }
        }
    }
}
