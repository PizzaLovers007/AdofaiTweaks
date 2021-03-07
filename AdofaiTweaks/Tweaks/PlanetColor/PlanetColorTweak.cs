using AdofaiTweaks.Core;
using AdofaiTweaks.Core.Attributes;
using AdofaiTweaks.Strings;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.PlanetColor
{
    [RegisterTweak(
        id: "planet_color",
        settingsType: typeof(PlanetColorSettings),
        patchesType: typeof(PlanetColorPatches))]
    public class PlanetColorTweak : Tweak
    {
        public override string Name =>
            TweakStrings.Get(TranslationKeys.PlanetColor.NAME);

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

        public override void OnSettingsGUI() {
            float newR, newG, newB, newTailR, newTailG, newTailB;
            float oldR, oldG, oldB, oldTailR, oldTailG, oldTailB;
            string newHex, newTailHex;
            string oldHex, oldTailHex;

            GUILayout.Label(TweakStrings.Get(TranslationKeys.PlanetColor.PLANET_ONE));
            GUILayout.BeginHorizontal();
            GUILayout.Space(20f);
            GUILayout.BeginVertical();

            oldR = Mathf.Round(Settings.Color1.r * 255);
            oldTailR = Mathf.Round(Settings.TailColor1.r * 255);
            (newR, newTailR) =
                DrawColorSliderPair(
                    TweakStrings.Get(TranslationKeys.PlanetColor.BODY_R),
                    TweakStrings.Get(TranslationKeys.PlanetColor.TAIL_R),
                    oldR,
                    oldTailR);
            oldG = Mathf.Round(Settings.Color1.g * 255);
            oldTailG = Mathf.Round(Settings.TailColor1.g * 255);
            (newG, newTailG) =
                DrawColorSliderPair(
                    TweakStrings.Get(TranslationKeys.PlanetColor.BODY_G),
                    TweakStrings.Get(TranslationKeys.PlanetColor.TAIL_G),
                    oldG,
                    oldTailG);
            oldB = Mathf.Round(Settings.Color1.b * 255);
            oldTailB = Mathf.Round(Settings.TailColor1.b * 255);
            (newB, newTailB) =
                DrawColorSliderPair(
                    TweakStrings.Get(TranslationKeys.PlanetColor.BODY_B),
                    TweakStrings.Get(TranslationKeys.PlanetColor.TAIL_B),
                    oldB,
                    oldTailB);
            if (oldR != newR || oldG != newG || oldB != newB) {
                Settings.Color1 = new Color(newR / 255, newG / 255, newB / 255);
                UpdatePlanetColors();
            }
            if (oldTailR != newTailR || oldTailG != newTailG || oldTailB != newTailB) {
                Settings.TailColor1 = new Color(newTailR / 255, newTailG / 255, newTailB / 255);
                UpdatePlanetColors();
            }

            oldHex = Settings.Color1Hex;
            oldTailHex = Settings.TailColor1Hex;
            GUILayout.BeginHorizontal();
            GUILayout.Label(
                TweakStrings.Get(TranslationKeys.PlanetColor.BODY_HEX), GUILayout.Width(90f));
            GUILayout.FlexibleSpace();
            newHex = GUILayout.TextField(oldHex, GUILayout.Width(100f));
            GUILayout.Space(50f);
            GUILayout.Label(
                TweakStrings.Get(TranslationKeys.PlanetColor.TAIL_HEX), GUILayout.Width(90f));
            GUILayout.FlexibleSpace();
            newTailHex = GUILayout.TextField(oldTailHex, GUILayout.Width(100f));
            GUILayout.Space(50f);
            GUILayout.EndHorizontal();
            if (oldHex != newHex && ColorUtility.TryParseHtmlString(newHex, out Color newColor)) {
                Settings.Color1 = newColor;
                UpdatePlanetColors();
            }
            if (oldTailHex != newTailHex
                && ColorUtility.TryParseHtmlString(newTailHex, out newColor)) {
                Settings.TailColor1 = newColor;
                UpdatePlanetColors();
            }
            Settings.Color1Hex = newHex;
            Settings.TailColor1Hex = newTailHex;

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.Space(8f);

            GUILayout.Label(TweakStrings.Get(TranslationKeys.PlanetColor.PLANET_TWO));
            GUILayout.BeginHorizontal();
            GUILayout.Space(20f);
            GUILayout.BeginVertical();

            oldR = Mathf.Round(Settings.Color2.r * 255);
            oldTailR = Mathf.Round(Settings.TailColor2.r * 255);
            (newR, newTailR) =
                DrawColorSliderPair(
                    TweakStrings.Get(TranslationKeys.PlanetColor.BODY_R),
                    TweakStrings.Get(TranslationKeys.PlanetColor.TAIL_R),
                    oldR,
                    oldTailR);
            oldG = Mathf.Round(Settings.Color2.g * 255);
            oldTailG = Mathf.Round(Settings.TailColor2.g * 255);
            (newG, newTailG) =
                DrawColorSliderPair(
                    TweakStrings.Get(TranslationKeys.PlanetColor.BODY_G),
                    TweakStrings.Get(TranslationKeys.PlanetColor.TAIL_G),
                    oldG,
                    oldTailG);
            oldB = Mathf.Round(Settings.Color2.b * 255);
            oldTailB = Mathf.Round(Settings.TailColor2.b * 255);
            (newB, newTailB) =
                DrawColorSliderPair(
                    TweakStrings.Get(TranslationKeys.PlanetColor.BODY_B),
                    TweakStrings.Get(TranslationKeys.PlanetColor.TAIL_B),
                    oldB,
                    oldTailB);
            if (oldR != newR || oldG != newG || oldB != newB) {
                Settings.Color2 = new Color(newR / 255, newG / 255, newB / 255);
                UpdatePlanetColors();
            }
            if (oldTailR != newTailR || oldTailG != newTailG || oldTailB != newTailB) {
                Settings.TailColor2 = new Color(newTailR / 255, newTailG / 255, newTailB / 255);
                UpdatePlanetColors();
            }

            oldHex = Settings.Color2Hex;
            oldTailHex = Settings.TailColor2Hex;
            GUILayout.BeginHorizontal();
            GUILayout.Label(
                TweakStrings.Get(TranslationKeys.PlanetColor.BODY_HEX), GUILayout.Width(90f));
            GUILayout.FlexibleSpace();
            newHex = GUILayout.TextField(oldHex, GUILayout.Width(100f));
            GUILayout.Space(50f);
            GUILayout.Label(
                TweakStrings.Get(TranslationKeys.PlanetColor.TAIL_HEX), GUILayout.Width(90f));
            GUILayout.FlexibleSpace();
            newTailHex = GUILayout.TextField(oldTailHex, GUILayout.Width(100f));
            GUILayout.Space(50f);
            GUILayout.EndHorizontal();
            if (oldHex != newHex && ColorUtility.TryParseHtmlString(newHex, out newColor)) {
                Settings.Color2 = newColor;
                UpdatePlanetColors();
            }
            if (oldTailHex != newTailHex
                && ColorUtility.TryParseHtmlString(newTailHex, out newColor)) {
                Settings.TailColor2 = newColor;
                UpdatePlanetColors();
            }
            Settings.Color2Hex = newHex;
            Settings.TailColor2Hex = newTailHex;

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private (float, float) DrawColorSliderPair(
            string bodyLabel, string tailLabel, float oldBodyCol, float oldTailCol) {
            GUILayout.BeginHorizontal();
            float newBodyCol = DrawColorSliders(bodyLabel, oldBodyCol);
            GUILayout.FlexibleSpace();
            float newTailCol = DrawColorSliders(tailLabel, oldTailCol);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            return (newBodyCol, newTailCol);
        }

        private float DrawColorSliders(string label, float oldCol) {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(90f));
            float newCol = GUILayout.HorizontalSlider(oldCol, 0, 255, GUILayout.Width(300f));
            newCol = Mathf.Round(newCol);
            GUILayout.Label("" + Mathf.RoundToInt(newCol), GUILayout.Width(40f));
            GUILayout.EndHorizontal();
            return newCol;
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
