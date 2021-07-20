using System.Collections;
using System.Collections.Generic;
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

        private readonly List<PlanetColorType> ALL_PLANET_COLOR_TYPES = new List<PlanetColorType>((IEnumerable<PlanetColorType>)System.Enum.GetValues(typeof(PlanetColorType)));

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

            GUILayout.Label("[CHANGE ME] Coloring Type:");
            MoreGUILayout.BeginIndent();
            for (int i = 0; i < ALL_PLANET_COLOR_TYPES.Count; i++) {
                bool colorEquals = Settings.Red.Body.ColorType.Equals(ALL_PLANET_COLOR_TYPES[i]);
                if (colorEquals != GUILayout.Toggle(colorEquals, $"{ALL_PLANET_COLOR_TYPES[i]}", GUILayout.MaxWidth(100f))) {
                    Settings.Red.Body.ColorType = ALL_PLANET_COLOR_TYPES[i];
                }
            }
            MoreGUILayout.EndIndent();

            switch (Settings.Red.Body.ColorType)
            {
                case PlanetColorType.Plain:
                    break;
                case PlanetColorType.Gradient:
                    break;
                case PlanetColorType.RandomGradient:
                    break;
                case PlanetColorType.Random:
                    break;
            }

            // Planet 1 RGB sliders
            (newBody, newTail) =
                MoreGUILayout.ColorRgbSlidersPair(Settings.Red.Body.PlainColor, Settings.Red.Tail.PlainColor);
            if (Settings.Red.Body.PlainColor != newBody)
            {
                Settings.Red.Body.PlainColor = newBody;
                UpdatePlanetColors();
            }
            if (Settings.Red.Tail.PlainColor != newTail)
            {
                Settings.Red.Tail.PlainColor = newTail;
                UpdatePlanetColors();
            }

            // Planet 1 Hex
            (newHex, newTailHex) =
                MoreGUILayout.NamedTextFieldPair(
                    "Hex:", "Hex:", Settings.Red.Body.PlainColorHex, Settings.Red.Tail.PlainColorHex, 100f, 40f);
            if (newHex != Settings.Red.Body.PlainColorHex
                && ColorUtility.TryParseHtmlString($"#{newHex}", out newBody))
            {
                Settings.Red.Body.PlainColor = newBody;
                UpdatePlanetColors();
            }
            if (newTailHex != Settings.Red.Tail.PlainColorHex
                && ColorUtility.TryParseHtmlString($"#{newTailHex}", out newTail))
            {
                Settings.Red.Tail.PlainColor = newTail;
                UpdatePlanetColors();
            }
            Settings.Red.Body.PlainColorHex = newHex;
            Settings.Red.Tail.PlainColorHex = newTailHex;

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
                MoreGUILayout.ColorRgbSlidersPair(Settings.Blue.Body.PlainColor, Settings.Blue.Tail.PlainColor);
            if (Settings.Blue.Body.PlainColor != newBody) {
                Settings.Blue.Body.PlainColor = newBody;
                UpdatePlanetColors();
            }
            if (Settings.Blue.Tail.PlainColor != newTail) {
                Settings.Blue.Tail.PlainColor = newTail;
                UpdatePlanetColors();
            }

            // Planet 2 Hex
            (newHex, newTailHex) =
                MoreGUILayout.NamedTextFieldPair(
                    "Hex:", "Hex:", Settings.Blue.Body.PlainColorHex, Settings.Blue.Tail.PlainColorHex, 100f, 40f);
            if (newHex != Settings.Blue.Body.PlainColorHex
                && ColorUtility.TryParseHtmlString($"#{newHex}", out newBody)) {
                Settings.Blue.Body.PlainColor = newBody;
                UpdatePlanetColors();
            }
            if (newTailHex != Settings.Blue.Tail.PlainColorHex
                && ColorUtility.TryParseHtmlString($"#{newTailHex}", out newTail)) {
                Settings.Blue.Tail.PlainColor = newTail;
                UpdatePlanetColors();
            }
            Settings.Blue.Body.PlainColorHex = newHex;
            Settings.Blue.Tail.PlainColorHex = newTailHex;

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

        /*private IEnumerator UpdateGradientPlanetColors()
        {
            //
        }*/

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
