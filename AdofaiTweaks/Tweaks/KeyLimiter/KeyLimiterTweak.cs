using System;
using System.Collections.Generic;
using AdofaiTweaks.Core;
using AdofaiTweaks.Core.Attributes;
using AdofaiTweaks.Strings;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.KeyLimiter
{
    /// <summary>
    /// Restricts which keys can be counted as input.
    /// </summary>
    [RegisterTweak(
        id: "key_limiter",
        settingsType: typeof(KeyLimiterSettings),
        patchesType: typeof(KeyLimiterPatches))]
    public class KeyLimiterTweak : Tweak
    {
        public override string Name =>
            TweakStrings.Get(TranslationKeys.KeyLimiter.NAME);

        public override string Description =>
            TweakStrings.Get(TranslationKeys.KeyLimiter.DESCRIPTION);

        public static readonly ISet<KeyCode> ALWAYS_BOUND_KEYS = new HashSet<KeyCode>() {
            KeyCode.Mouse0,
            KeyCode.Mouse1,
            KeyCode.Mouse2,
            KeyCode.Mouse3,
            KeyCode.Mouse4,
            KeyCode.Mouse5,
            KeyCode.Mouse6,
            KeyCode.Escape,
        };

        [SyncTweakSettings]
        private KeyLimiterSettings Settings { get; set; }

        private Dictionary<KeyCode, bool> keyState;
        private KeyViewer keyViewer;

        public override void OnUpdate(float deltaTime) {
            UpdateRegisteredKeys();
            UpdateKeyState();
        }

        private void UpdateRegisteredKeys() {
            if (!Settings.IsListening) {
                return;
            }

            bool changed = false;
            foreach (KeyCode code in Enum.GetValues(typeof(KeyCode))) {
                // Skip key if not pressed or should always be bound
                if (!Input.GetKeyDown(code) || ALWAYS_BOUND_KEYS.Contains(code)) {
                    continue;
                }

                // Register/unregister the key
                if (Settings.ActiveKeys.Contains(code)) {
                    Settings.ActiveKeys.Remove(code);
                    changed = true;
                } else {
                    Settings.ActiveKeys.Add(code);
                    changed = true;
                }
            }
            if (changed) {
                keyViewer.UpdateKeys();
            }
        }

        private void UpdateKeyState() {
            foreach (KeyCode code in Settings.ActiveKeys) {
                keyState[code] = Input.GetKey(code);
            }
            keyViewer.UpdateState(keyState);
        }

        public override void OnHideGUI() {
            Settings.IsListening = false;
        }

        public override void OnSettingsGUI() {
            DrawKeyRegisterSettingsGUI();
            GUILayout.Space(8f);
            DrawKeyViewerSettingsGUI();
        }

        private void DrawKeyRegisterSettingsGUI() {
            // List of registered keys
            GUILayout.Label(TweakStrings.Get(TranslationKeys.KeyLimiter.REGISTERED_KEYS));
            GUILayout.BeginHorizontal();
            GUILayout.Space(20f);
            GUILayout.BeginVertical();
            GUILayout.Space(8f);
            GUILayout.EndVertical();
            foreach (KeyCode code in Settings.ActiveKeys) {
                GUILayout.Label(code.ToString());
                GUILayout.Space(8f);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(12f);

            // Record keys toggle
            GUILayout.BeginHorizontal();
            if (Settings.IsListening) {
                if (GUILayout.Button(TweakStrings.Get(TranslationKeys.KeyLimiter.DONE))) {
                    Settings.IsListening = false;
                }
                GUILayout.Label(TweakStrings.Get(TranslationKeys.KeyLimiter.PRESS_KEY_REGISTER));
            } else {
                if (GUILayout.Button(TweakStrings.Get(TranslationKeys.KeyLimiter.CHANGE_KEYS))) {
                    Settings.IsListening = true;
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void DrawKeyViewerSettingsGUI() {
            // Key viewer toggle
            bool newShow =
                GUILayout.Toggle(
                    Settings.ShowKeyViewer,
                    TweakStrings.Get(TranslationKeys.KeyLimiter.SHOW_KEY_VIEWER));
            if (newShow != Settings.ShowKeyViewer) {
                Settings.ShowKeyViewer = newShow;
                keyViewer.gameObject.SetActive(Settings.ShowKeyViewer);
            }

            // Don't draw settings if key viewer is hidden
            if (!Settings.ShowKeyViewer) {
                return;
            }

            MoreGUILayout.BeginIndent();

            Settings.AnimateKeys =
                GUILayout.Toggle(
                    Settings.AnimateKeys,
                    TweakStrings.Get(TranslationKeys.KeyLimiter.ANIMATE_KEYS));

            // Size slider
            float newSize =
                MoreGUILayout.NamedSlider(
                    TweakStrings.Get(TranslationKeys.KeyLimiter.KEY_VIEWER_SIZE),
                    Settings.KeyViewerSize,
                    10f,
                    200f,
                    300f,
                    roundNearest: 1f);
            if (newSize != Settings.KeyViewerSize) {
                Settings.KeyViewerSize = newSize;
                keyViewer.UpdateLayout();
            }

            // X position slider
            float newX =
                MoreGUILayout.NamedSlider(
                    TweakStrings.Get(TranslationKeys.KeyLimiter.KEY_VIEWER_X_POS),
                    Settings.KeyViewerXPos,
                    0f,
                    1f,
                    300f,
                    roundNearest: 0.01f);
            if (newX != Settings.KeyViewerXPos) {
                Settings.KeyViewerXPos = newX;
                keyViewer.UpdateLayout();
            }

            // Y position slider
            float newY =
                MoreGUILayout.NamedSlider(
                    TweakStrings.Get(TranslationKeys.KeyLimiter.KEY_VIEWER_Y_POS),
                    Settings.KeyViewerYPos,
                    0f,
                    1f,
                    300f,
                    roundNearest: 0.01f);
            if (newY != Settings.KeyViewerYPos) {
                Settings.KeyViewerYPos = newY;
                keyViewer.UpdateLayout();
            }

            GUILayout.Space(8f);

            Color newPressed, newReleased;
            string newPressedHex, newReleasedHex;

            // Outline color header
            GUILayout.BeginHorizontal();
            GUILayout.Label(
                TweakStrings.Get(TranslationKeys.KeyLimiter.PRESSED_OUTLINE_COLOR),
                GUILayout.Width(200f));
            GUILayout.FlexibleSpace();
            GUILayout.Space(8f);
            GUILayout.Label(
                TweakStrings.Get(TranslationKeys.KeyLimiter.RELEASED_OUTLINE_COLOR),
                GUILayout.Width(200f));
            GUILayout.FlexibleSpace();
            GUILayout.Space(20f);
            GUILayout.EndHorizontal();
            MoreGUILayout.BeginIndent();

            // Outline color RGBA sliders
            (newPressed, newReleased) =
                MoreGUILayout.ColorRgbaSlidersPair(
                    Settings.PressedOutlineColor, Settings.ReleasedOutlineColor);
            if (newPressed != Settings.PressedOutlineColor) {
                Settings.PressedOutlineColor = newPressed;
                keyViewer.UpdateLayout();
            }
            if (newReleased != Settings.ReleasedOutlineColor) {
                Settings.ReleasedOutlineColor = newReleased;
                keyViewer.UpdateLayout();
            }

            // Outline color hex
            (newPressedHex, newReleasedHex) =
                MoreGUILayout.NamedTextFieldPair(
                    "Hex:",
                    "Hex:",
                    Settings.PressedOutlineColorHex,
                    Settings.ReleasedOutlineColorHex,
                    100f,
                    40f);
            if (newPressedHex != Settings.PressedOutlineColorHex
                && ColorUtility.TryParseHtmlString(newPressedHex, out newPressed)) {
                Settings.PressedOutlineColor = newPressed;
            }
            if (newReleasedHex != Settings.ReleasedOutlineColorHex
                && ColorUtility.TryParseHtmlString(newReleasedHex, out newReleased)) {
                Settings.ReleasedOutlineColor = newReleased;
            }
            Settings.PressedOutlineColorHex = newPressedHex;
            Settings.ReleasedOutlineColorHex = newReleasedHex;

            MoreGUILayout.EndIndent();

            GUILayout.Space(8f);

            // Background color header
            GUILayout.BeginHorizontal();
            GUILayout.Label(
                TweakStrings.Get(TranslationKeys.KeyLimiter.PRESSED_BACKGROUND_COLOR),
                GUILayout.Width(200f));
            GUILayout.FlexibleSpace();
            GUILayout.Space(8f);
            GUILayout.Label(
                TweakStrings.Get(TranslationKeys.KeyLimiter.RELEASED_BACKGROUND_COLOR),
                GUILayout.Width(200f));
            GUILayout.FlexibleSpace();
            GUILayout.Space(20f);
            GUILayout.EndHorizontal();
            MoreGUILayout.BeginIndent();

            // Background color RGBA sliders
            (newPressed, newReleased) =
                MoreGUILayout.ColorRgbaSlidersPair(
                    Settings.PressedBackgroundColor, Settings.ReleasedBackgroundColor);
            if (newPressed != Settings.PressedBackgroundColor) {
                Settings.PressedBackgroundColor = newPressed;
                keyViewer.UpdateLayout();
            }
            if (newReleased != Settings.ReleasedBackgroundColor) {
                Settings.ReleasedBackgroundColor = newReleased;
                keyViewer.UpdateLayout();
            }

            // Background color hex
            (newPressedHex, newReleasedHex) =
                MoreGUILayout.NamedTextFieldPair(
                    "Hex:",
                    "Hex:",
                    Settings.PressedBackgroundColorHex,
                    Settings.ReleasedBackgroundColorHex,
                    100f,
                    40f);
            if (newPressedHex != Settings.PressedBackgroundColorHex
                && ColorUtility.TryParseHtmlString(newPressedHex, out newPressed)) {
                Settings.PressedBackgroundColor = newPressed;
            }
            if (newReleasedHex != Settings.ReleasedBackgroundColorHex
                && ColorUtility.TryParseHtmlString(newReleasedHex, out newReleased)) {
                Settings.ReleasedBackgroundColor = newReleased;
            }
            Settings.PressedBackgroundColorHex = newPressedHex;
            Settings.ReleasedBackgroundColorHex = newReleasedHex;

            MoreGUILayout.EndIndent();

            GUILayout.Space(8f);

            // Text color header
            GUILayout.BeginHorizontal();
            GUILayout.Label(
                TweakStrings.Get(TranslationKeys.KeyLimiter.PRESSED_TEXT_COLOR),
                GUILayout.Width(200f));
            GUILayout.FlexibleSpace();
            GUILayout.Space(8f);
            GUILayout.Label(
                TweakStrings.Get(TranslationKeys.KeyLimiter.RELEASED_TEXT_COLOR),
                GUILayout.Width(200f));
            GUILayout.FlexibleSpace();
            GUILayout.Space(20f);
            GUILayout.EndHorizontal();
            MoreGUILayout.BeginIndent();

            // Text color RGBA sliders
            (newPressed, newReleased) =
                MoreGUILayout.ColorRgbaSlidersPair(
                    Settings.PressedTextColor, Settings.ReleasedTextColor);
            if (newPressed != Settings.PressedTextColor) {
                Settings.PressedTextColor = newPressed;
                keyViewer.UpdateLayout();
            }
            if (newReleased != Settings.ReleasedTextColor) {
                Settings.ReleasedTextColor = newReleased;
                keyViewer.UpdateLayout();
            }

            // Text color hex
            (newPressedHex, newReleasedHex) =
                MoreGUILayout.NamedTextFieldPair(
                    "Hex:",
                    "Hex:",
                    Settings.PressedTextColorHex,
                    Settings.ReleasedTextColorHex,
                    100f,
                    40f);
            if (newPressedHex != Settings.PressedTextColorHex
                && ColorUtility.TryParseHtmlString(newPressedHex, out newPressed)) {
                Settings.PressedTextColor = newPressed;
            }
            if (newReleasedHex != Settings.ReleasedTextColorHex
                && ColorUtility.TryParseHtmlString(newReleasedHex, out newReleased)) {
                Settings.ReleasedTextColor = newReleased;
            }
            Settings.PressedTextColorHex = newPressedHex;
            Settings.ReleasedTextColorHex = newReleasedHex;

            MoreGUILayout.EndIndent();

            MoreGUILayout.EndIndent();
        }

        public override void OnEnable() {
            GameObject keyViewerObj = new GameObject();
            GameObject.DontDestroyOnLoad(keyViewerObj);
            keyViewer = keyViewerObj.AddComponent<KeyViewer>();
            keyViewerObj.SetActive(Settings.ShowKeyViewer);
            keyViewer.Settings = Settings;

            keyState = new Dictionary<KeyCode, bool>();
        }

        public override void OnDisable() {
            GameObject.Destroy(keyViewer.gameObject);
        }
    }
}
