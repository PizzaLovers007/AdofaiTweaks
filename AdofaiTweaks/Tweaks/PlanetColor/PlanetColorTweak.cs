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
                bool colorEquals = Settings.RedBody.ColorType.Equals(ALL_PLANET_COLOR_TYPES[i]);
                if (colorEquals != GUILayout.Toggle(colorEquals, $"{ALL_PLANET_COLOR_TYPES[i]}", GUILayout.MaxWidth(100f))) {
                    Settings.RedBody.ColorType = ALL_PLANET_COLOR_TYPES[i];
                }
            }
            MoreGUILayout.EndIndent();

            switch (Settings.RedBody.ColorType)
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
                MoreGUILayout.ColorRgbSlidersPair(Settings.RedBody.PlainColor, Settings.RedTail.PlainColor);
            if (Settings.RedBody.PlainColor != newBody)
            {
                Settings.RedBody.PlainColor = newBody;
                UpdatePlanetColors();
            }
            if (Settings.RedTail.PlainColor != newTail)
            {
                Settings.RedTail.PlainColor = newTail;
                UpdatePlanetColors();
            }

            // Planet 1 Hex
            (newHex, newTailHex) =
                MoreGUILayout.NamedTextFieldPair(
                    "Hex:", "Hex:", Settings.RedBody.PlainColorHex, Settings.RedTail.PlainColorHex, 100f, 40f);
            if (newHex != Settings.RedBody.PlainColorHex
                && ColorUtility.TryParseHtmlString($"#{newHex}", out newBody))
            {
                Settings.RedBody.PlainColor = newBody;
                UpdatePlanetColors();
            }
            if (newTailHex != Settings.RedTail.PlainColorHex
                && ColorUtility.TryParseHtmlString($"#{newTailHex}", out newTail))
            {
                Settings.RedTail.PlainColor = newTail;
                UpdatePlanetColors();
            }
            Settings.RedBody.PlainColorHex = newHex;
            Settings.RedTail.PlainColorHex = newTailHex;

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
                MoreGUILayout.ColorRgbSlidersPair(Settings.BlueBody.PlainColor, Settings.BlueTail.PlainColor);
            if (Settings.BlueBody.PlainColor != newBody) {
                Settings.BlueBody.PlainColor = newBody;
                UpdatePlanetColors();
            }
            if (Settings.BlueTail.PlainColor != newTail) {
                Settings.BlueTail.PlainColor = newTail;
                UpdatePlanetColors();
            }

            // Planet 2 Hex
            (newHex, newTailHex) =
                MoreGUILayout.NamedTextFieldPair(
                    "Hex:", "Hex:", Settings.BlueBody.PlainColorHex, Settings.BlueTail.PlainColorHex, 100f, 40f);
            if (newHex != Settings.BlueBody.PlainColorHex
                && ColorUtility.TryParseHtmlString($"#{newHex}", out newBody)) {
                Settings.BlueBody.PlainColor = newBody;
                UpdatePlanetColors();
            }
            if (newTailHex != Settings.BlueTail.PlainColorHex
                && ColorUtility.TryParseHtmlString($"#{newTailHex}", out newTail)) {
                Settings.BlueTail.PlainColor = newTail;
                UpdatePlanetColors();
            }
            Settings.BlueBody.PlainColorHex = newHex;
            Settings.BlueTail.PlainColorHex = newTailHex;

            MoreGUILayout.EndIndent();

            MoreGUILayout.EndIndent();
        }

        /// <inheritdoc/>
        public override void OnEnable()
        {
            MigrateOldSettings();
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

        /// <summary>
        /// Migrates old PlanetColor settings to a new structured settings if there are
        /// settings to migrate.
        /// TODO: Delete this after a few releases.
        /// </summary>
        private void MigrateOldSettings()
        {
            // Check if there's anything to migrate
            if (Settings.Color1 == Color.black &&
                Settings.Color2 == Color.black &&
                Settings.TailColor1 == Color.black &&
                Settings.TailColor2 == Color.black) {
                return;
            }

            // Migrate the old values
            Settings.RedBody.PlainColor = Settings.Color1;
            Settings.RedTail.PlainColor = Settings.TailColor1;
            Settings.BlueBody.PlainColor = Settings.Color2;
            Settings.BlueTail.PlainColor = Settings.TailColor2;

            // Reset to value
            Settings.Color1 =
                Settings.Color2 =
                Settings.TailColor1 =
                Settings.TailColor2 = Color.black;
        }

        /*private IEnumerator UpdateGradientPlanetColors()
        {
            //
        }*/
    }
}
