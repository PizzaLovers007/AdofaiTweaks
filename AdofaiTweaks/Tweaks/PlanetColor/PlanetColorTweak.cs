using AdofaiTweaks.Core;
using AdofaiTweaks.Core.Attributes;
using AdofaiTweaks.Strings;
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

        private scrPlanet RedPlanet {
            get => Object.FindObjectOfType<scrController>()?.redPlanet;
        }

        private scrPlanet BluePlanet {
            get => Object.FindObjectOfType<scrController>()?.bluePlanet;
        }

        [SyncTweakSettings]
        private PlanetColorSettings Settings { get; set; }

        /// <inheritdoc/>
        public override void OnSettingsGUI() {
            Color newBody, newTail;
            string newHex, newTailHex;

            GUILayout.Label(TweakStrings.Get(TranslationKeys.PlanetColor.PLANET_ONE));
            MoreGUILayout.BeginIndent();

            GUILayout.BeginHorizontal();
            GUILayout.Label(
                TweakStrings.Get(TranslationKeys.PlanetColor.BODY), GUILayout.Width(200f));
            GUILayout.FlexibleSpace();
            GUILayout.Space(8f);
            GUILayout.Label(
                TweakStrings.Get(TranslationKeys.PlanetColor.TAIL), GUILayout.Width(200f));
            GUILayout.FlexibleSpace();
            GUILayout.Space(20f);
            GUILayout.EndHorizontal();
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

            GUILayout.Space(8f);

            GUILayout.Label(TweakStrings.Get(TranslationKeys.PlanetColor.PLANET_TWO));
            MoreGUILayout.BeginIndent();

            GUILayout.BeginHorizontal();
            GUILayout.Label(
                TweakStrings.Get(TranslationKeys.PlanetColor.BODY), GUILayout.Width(200f));
            GUILayout.FlexibleSpace();
            GUILayout.Space(8f);
            GUILayout.Label(
                TweakStrings.Get(TranslationKeys.PlanetColor.TAIL), GUILayout.Width(200f));
            GUILayout.FlexibleSpace();
            GUILayout.Space(20f);
            GUILayout.EndHorizontal();
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
                RedPlanet.LoadPlanetColor();
            }
            if (BluePlanet != null) {
                BluePlanet.LoadPlanetColor();
            }
        }
    }
}
