using System;
using System.Collections.Generic;
using AdofaiTweaks.Core;
using AdofaiTweaks.Core.Attributes;
using AdofaiTweaks.Strings;
using AdofaiTweaks.Tweaks.KeyLimiter;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.KeyViewer
{
    /// <summary>
    /// A tweak for showing which keys are being pressed.
    /// </summary>
    [RegisterTweak(
        id: "key_viewer",
        settingsType: typeof(KeyViewerSettings),
        patchesType: typeof(KeyViewerPatches))]
    public class KeyViewerTweak : Tweak
    {
        /// <inheritdoc/>
        public override string Name =>
            TweakStrings.Get(TranslationKeys.KeyViewer.NAME);

        /// <inheritdoc/>
        public override string Description =>
            TweakStrings.Get(TranslationKeys.KeyViewer.DESCRIPTION);

        /// <summary>
        /// Keys that should not be listened to.
        /// </summary>
        public static readonly ISet<KeyCode> SKIPPED_KEYS = new HashSet<KeyCode>() {
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
        private KeyViewerSettings Settings { get; set; }

        [SyncTweakSettings]
        private KeyLimiterSettings LimiterSettings { get; set; }

        private KeyViewerProfile CurrentProfile { get => Settings.CurrentProfile; }

        private Dictionary<KeyCode, bool> keyState;
        private KeyViewer keyViewer;

        /// <inheritdoc/>
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
                // Skip key if not pressed or should be skipped
                if (!Input.GetKeyDown(code) || SKIPPED_KEYS.Contains(code)) {
                    continue;
                }

                // Register/unregister the key
                if (CurrentProfile.ActiveKeys.Contains(code)) {
                    CurrentProfile.ActiveKeys.Remove(code);
                    changed = true;
                } else {
                    CurrentProfile.ActiveKeys.Add(code);
                    changed = true;
                }
            }
            if (changed) {
                keyViewer.UpdateKeys();
            }
        }

        private void UpdateKeyState() {
            UpdateViewerVisibility();
            foreach (KeyCode code in CurrentProfile.ActiveKeys) {
                keyState[code] = Input.GetKey(code);
            }
            keyViewer.UpdateState(keyState);
        }

        /// <inheritdoc/>
        public override void OnHideGUI() {
            Settings.IsListening = false;
        }

        /// <inheritdoc/>
        public override void OnSettingsGUI() {
            DrawProfileSettingsGUI();
            GUILayout.Space(12f);
            MoreGUILayout.HorizontalLine(1f, 400f);
            GUILayout.Space(8f);
            DrawKeyRegisterSettingsGUI();
            GUILayout.Space(8f);
            DrawKeyViewerSettingsGUI();
        }

        private void DrawProfileSettingsGUI() {
            GUILayout.Space(4f);

            // New, Duplicate, Delete buttons
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(
                TweakStrings.Get(TranslationKeys.KeyViewer.NEW))) {
                Settings.Profiles.Add(new KeyViewerProfile());
                Settings.ProfileIndex = Settings.Profiles.Count - 1;
                Settings.CurrentProfile.Name += "Profile " + Settings.Profiles.Count;
                keyViewer.Profile = Settings.CurrentProfile;
            }
            if (GUILayout.Button(
                TweakStrings.Get(TranslationKeys.KeyViewer.DUPLICATE))) {
                Settings.Profiles.Add(Settings.CurrentProfile.Copy());
                Settings.ProfileIndex = Settings.Profiles.Count - 1;
                Settings.CurrentProfile.Name += " Copy";
                keyViewer.Profile = Settings.CurrentProfile;
            }

            if (Settings.Profiles.Count > 1
                && GUILayout.Button(
                    TweakStrings.Get(TranslationKeys.KeyViewer.DELETE))) {
                Settings.Profiles.RemoveAt(Settings.ProfileIndex);
                Settings.ProfileIndex =
                    Math.Min(Settings.ProfileIndex, Settings.Profiles.Count - 1);
                keyViewer.Profile = Settings.CurrentProfile;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(4f);

            // Profile name
            Settings.CurrentProfile.Name =
                MoreGUILayout.NamedTextField(
                    TweakStrings.Get(TranslationKeys.KeyViewer.PROFILE_NAME),
                    Settings.CurrentProfile.Name,
                    400f);

            // Profile list
            GUILayout.Label(TweakStrings.Get(TranslationKeys.KeyViewer.PROFILES));
            int selected = Settings.ProfileIndex;
            if (MoreGUILayout.ToggleList(Settings.Profiles, ref selected, p => p.Name)) {
                Settings.ProfileIndex = selected;
                keyViewer.Profile = Settings.CurrentProfile;
            }
        }

        private void DrawKeyRegisterSettingsGUI() {
            // List of registered keys
            GUILayout.Label(TweakStrings.Get(TranslationKeys.KeyViewer.REGISTERED_KEYS));
            GUILayout.BeginHorizontal();
            GUILayout.Space(20f);
            GUILayout.BeginVertical();
            GUILayout.Space(8f);
            GUILayout.EndVertical();
            foreach (KeyCode code in CurrentProfile.ActiveKeys) {
                GUILayout.Label(code.ToString());
                GUILayout.Space(8f);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(12f);

            // Record keys toggle
            GUILayout.BeginHorizontal();
            if (Settings.IsListening) {
                if (GUILayout.Button(TweakStrings.Get(TranslationKeys.KeyViewer.DONE))) {
                    Settings.IsListening = false;
                }
                GUILayout.Label(TweakStrings.Get(TranslationKeys.KeyViewer.PRESS_KEY_REGISTER));
            } else {
                if (GUILayout.Button(TweakStrings.Get(TranslationKeys.KeyViewer.CHANGE_KEYS))) {
                    Settings.IsListening = true;
                }
            }

            if (GUILayout.Button(
                    TweakStrings.Get(TranslationKeys.KeyViewer.CLEAR_KEY_COUNT)))
            {
                keyViewer.ClearCounts();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void DrawKeyViewerSettingsGUI() {
            MoreGUILayout.BeginIndent();

            // Show only in gameplay toggle
            CurrentProfile.ViewerOnlyGameplay =
                GUILayout.Toggle(
                    CurrentProfile.ViewerOnlyGameplay,
                    TweakStrings.Get(TranslationKeys.KeyViewer.VIEWER_ONLY_GAMEPLAY));

            // Animate keys toggle
            CurrentProfile.AnimateKeys =
                GUILayout.Toggle(
                    CurrentProfile.AnimateKeys,
                    TweakStrings.Get(TranslationKeys.KeyViewer.ANIMATE_KEYS));

            // Key press total toggle
            bool newShowTotal =
                GUILayout.Toggle(
                    CurrentProfile.ShowKeyPressTotal,
                    TweakStrings.Get(TranslationKeys.KeyViewer.SHOW_KEY_PRESS_TOTAL));
            if (newShowTotal != CurrentProfile.ShowKeyPressTotal) {
                CurrentProfile.ShowKeyPressTotal = newShowTotal;
                keyViewer.UpdateLayout();
            }

            // Size slider
            float newSize =
                MoreGUILayout.NamedSlider(
                    TweakStrings.Get(TranslationKeys.KeyViewer.KEY_VIEWER_SIZE),
                    CurrentProfile.KeyViewerSize,
                    10f,
                    200f,
                    300f,
                    roundNearest: 1f);
            if (newSize != CurrentProfile.KeyViewerSize) {
                CurrentProfile.KeyViewerSize = newSize;
                keyViewer.UpdateLayout();
            }

            // X position slider
            float newX =
                MoreGUILayout.NamedSlider(
                    TweakStrings.Get(TranslationKeys.KeyViewer.KEY_VIEWER_X_POS),
                    CurrentProfile.KeyViewerXPos,
                    0f,
                    1f,
                    300f,
                    roundNearest: 0.01f,
                    valueFormat: "{0:0.##}");
            if (newX != CurrentProfile.KeyViewerXPos) {
                CurrentProfile.KeyViewerXPos = newX;
                keyViewer.UpdateLayout();
            }

            // Y position slider
            float newY =
                MoreGUILayout.NamedSlider(
                    TweakStrings.Get(TranslationKeys.KeyViewer.KEY_VIEWER_Y_POS),
                    CurrentProfile.KeyViewerYPos,
                    0f,
                    1f,
                    300f,
                    roundNearest: 0.01f,
                    valueFormat: "{0:0.##}");
            if (newY != CurrentProfile.KeyViewerYPos) {
                CurrentProfile.KeyViewerYPos = newY;
                keyViewer.UpdateLayout();
            }

            GUILayout.Space(8f);

            Color newPressed, newReleased;
            string newPressedHex, newReleasedHex;

            // Outline color header
            GUILayout.BeginHorizontal();
            GUILayout.Label(
                TweakStrings.Get(TranslationKeys.KeyViewer.PRESSED_OUTLINE_COLOR),
                GUILayout.Width(200f));
            GUILayout.FlexibleSpace();
            GUILayout.Space(8f);
            GUILayout.Label(
                TweakStrings.Get(TranslationKeys.KeyViewer.RELEASED_OUTLINE_COLOR),
                GUILayout.Width(200f));
            GUILayout.FlexibleSpace();
            GUILayout.Space(20f);
            GUILayout.EndHorizontal();
            MoreGUILayout.BeginIndent();

            // Outline color RGBA sliders
            (newPressed, newReleased) =
                MoreGUILayout.ColorRgbaSlidersPair(
                    CurrentProfile.PressedOutlineColor, CurrentProfile.ReleasedOutlineColor);
            if (newPressed != CurrentProfile.PressedOutlineColor) {
                CurrentProfile.PressedOutlineColor = newPressed;
                keyViewer.UpdateLayout();
            }
            if (newReleased != CurrentProfile.ReleasedOutlineColor) {
                CurrentProfile.ReleasedOutlineColor = newReleased;
                keyViewer.UpdateLayout();
            }

            // Outline color hex
            (newPressedHex, newReleasedHex) =
                MoreGUILayout.NamedTextFieldPair(
                    "Hex:",
                    "Hex:",
                    CurrentProfile.PressedOutlineColorHex,
                    CurrentProfile.ReleasedOutlineColorHex,
                    100f,
                    40f);
            if (newPressedHex != CurrentProfile.PressedOutlineColorHex
                && ColorUtility.TryParseHtmlString($"#{newPressedHex}", out newPressed)) {
                CurrentProfile.PressedOutlineColor = newPressed;
                keyViewer.UpdateLayout();
            }
            if (newReleasedHex != CurrentProfile.ReleasedOutlineColorHex
                && ColorUtility.TryParseHtmlString($"#{newReleasedHex}", out newReleased)) {
                CurrentProfile.ReleasedOutlineColor = newReleased;
                keyViewer.UpdateLayout();
            }
            CurrentProfile.PressedOutlineColorHex = newPressedHex;
            CurrentProfile.ReleasedOutlineColorHex = newReleasedHex;

            MoreGUILayout.EndIndent();

            GUILayout.Space(8f);

            // Background color header
            GUILayout.BeginHorizontal();
            GUILayout.Label(
                TweakStrings.Get(TranslationKeys.KeyViewer.PRESSED_BACKGROUND_COLOR),
                GUILayout.Width(200f));
            GUILayout.FlexibleSpace();
            GUILayout.Space(8f);
            GUILayout.Label(
                TweakStrings.Get(TranslationKeys.KeyViewer.RELEASED_BACKGROUND_COLOR),
                GUILayout.Width(200f));
            GUILayout.FlexibleSpace();
            GUILayout.Space(20f);
            GUILayout.EndHorizontal();
            MoreGUILayout.BeginIndent();

            // Background color RGBA sliders
            (newPressed, newReleased) =
                MoreGUILayout.ColorRgbaSlidersPair(
                    CurrentProfile.PressedBackgroundColor, CurrentProfile.ReleasedBackgroundColor);
            if (newPressed != CurrentProfile.PressedBackgroundColor) {
                CurrentProfile.PressedBackgroundColor = newPressed;
                keyViewer.UpdateLayout();
            }
            if (newReleased != CurrentProfile.ReleasedBackgroundColor) {
                CurrentProfile.ReleasedBackgroundColor = newReleased;
                keyViewer.UpdateLayout();
            }

            // Background color hex
            (newPressedHex, newReleasedHex) =
                MoreGUILayout.NamedTextFieldPair(
                    "Hex:",
                    "Hex:",
                    CurrentProfile.PressedBackgroundColorHex,
                    CurrentProfile.ReleasedBackgroundColorHex,
                    100f,
                    40f);
            if (newPressedHex != CurrentProfile.PressedBackgroundColorHex
                && ColorUtility.TryParseHtmlString($"#{newPressedHex}", out newPressed)) {
                CurrentProfile.PressedBackgroundColor = newPressed;
                keyViewer.UpdateLayout();
            }
            if (newReleasedHex != CurrentProfile.ReleasedBackgroundColorHex
                && ColorUtility.TryParseHtmlString($"#{newReleasedHex}", out newReleased)) {
                CurrentProfile.ReleasedBackgroundColor = newReleased;
                keyViewer.UpdateLayout();
            }
            CurrentProfile.PressedBackgroundColorHex = newPressedHex;
            CurrentProfile.ReleasedBackgroundColorHex = newReleasedHex;

            MoreGUILayout.EndIndent();

            GUILayout.Space(8f);

            // Text color header
            GUILayout.BeginHorizontal();
            GUILayout.Label(
                TweakStrings.Get(TranslationKeys.KeyViewer.PRESSED_TEXT_COLOR),
                GUILayout.Width(200f));
            GUILayout.FlexibleSpace();
            GUILayout.Space(8f);
            GUILayout.Label(
                TweakStrings.Get(TranslationKeys.KeyViewer.RELEASED_TEXT_COLOR),
                GUILayout.Width(200f));
            GUILayout.FlexibleSpace();
            GUILayout.Space(20f);
            GUILayout.EndHorizontal();
            MoreGUILayout.BeginIndent();

            // Text color RGBA sliders
            (newPressed, newReleased) =
                MoreGUILayout.ColorRgbaSlidersPair(
                    CurrentProfile.PressedTextColor, CurrentProfile.ReleasedTextColor);
            if (newPressed != CurrentProfile.PressedTextColor) {
                CurrentProfile.PressedTextColor = newPressed;
                keyViewer.UpdateLayout();
            }
            if (newReleased != CurrentProfile.ReleasedTextColor) {
                CurrentProfile.ReleasedTextColor = newReleased;
                keyViewer.UpdateLayout();
            }

            // Text color hex
            (newPressedHex, newReleasedHex) =
                MoreGUILayout.NamedTextFieldPair(
                    "Hex:",
                    "Hex:",
                    CurrentProfile.PressedTextColorHex,
                    CurrentProfile.ReleasedTextColorHex,
                    100f,
                    40f);
            if (newPressedHex != CurrentProfile.PressedTextColorHex
                && ColorUtility.TryParseHtmlString($"#{newPressedHex}", out newPressed)) {
                CurrentProfile.PressedTextColor = newPressed;
                keyViewer.UpdateLayout();
            }
            if (newReleasedHex != CurrentProfile.ReleasedTextColorHex
                && ColorUtility.TryParseHtmlString($"#{newReleasedHex}", out newReleased)) {
                CurrentProfile.ReleasedTextColor = newReleased;
                keyViewer.UpdateLayout();
            }
            CurrentProfile.PressedTextColorHex = newPressedHex;
            CurrentProfile.ReleasedTextColorHex = newReleasedHex;

            MoreGUILayout.EndIndent();

            MoreGUILayout.EndIndent();
        }

        /// <inheritdoc/>
        public override void OnEnable() {
            if (Settings.Profiles.Count == 0) {
                Settings.Profiles.Add(new KeyViewerProfile() { Name = "Default Profile" });
            }
            if (Settings.ProfileIndex < 0 || Settings.ProfileIndex >= Settings.Profiles.Count) {
                Settings.ProfileIndex = 0;
            }

            MigrateOldSettings();

            GameObject keyViewerObj = new GameObject();
            GameObject.DontDestroyOnLoad(keyViewerObj);
            keyViewer = keyViewerObj.AddComponent<KeyViewer>();
            keyViewer.Profile = CurrentProfile;

            UpdateViewerVisibility();

            keyState = new Dictionary<KeyCode, bool>();
        }

        /// <summary>
        /// Migrates old KeyLimiter settings to a KeyViewer profile if there are
        /// settings to migrate.
        /// TODO: Delete this after a few releases.
        /// </summary>
        private void MigrateOldSettings() {
            // Check if there are settings to migrate
            if (LimiterSettings.PressedBackgroundColor == Color.black
                && LimiterSettings.ReleasedBackgroundColor == Color.black
                && LimiterSettings.PressedOutlineColor == Color.black
                && LimiterSettings.ReleasedOutlineColor == Color.black
                && LimiterSettings.PressedTextColor == Color.black
                && LimiterSettings.ReleasedTextColor == Color.black) {
                return;
            }

            // Copy into new profile
            KeyViewerProfile profile = new KeyViewerProfile {
                Name = "Old Profile",
                ActiveKeys = new List<KeyCode>(LimiterSettings.ActiveKeys),
                ViewerOnlyGameplay = LimiterSettings.ViewerOnlyGameplay,
                AnimateKeys = LimiterSettings.AnimateKeys,
                KeyViewerSize = LimiterSettings.KeyViewerSize,
                KeyViewerXPos = LimiterSettings.KeyViewerXPos,
                KeyViewerYPos = LimiterSettings.KeyViewerYPos,
                PressedOutlineColor = LimiterSettings.PressedOutlineColor,
                ReleasedOutlineColor = LimiterSettings.ReleasedOutlineColor,
                PressedBackgroundColor = LimiterSettings.PressedBackgroundColor,
                ReleasedBackgroundColor = LimiterSettings.ReleasedBackgroundColor,
                PressedTextColor = LimiterSettings.PressedTextColor,
                ReleasedTextColor = LimiterSettings.ReleasedTextColor,
            };

            // Set current to migrated profile
            Settings.Profiles.Insert(0, profile);
            Settings.ProfileIndex = 0;

            // Clear old settings
            LimiterSettings.PressedBackgroundColor = Color.black;
            LimiterSettings.ReleasedBackgroundColor = Color.black;
            LimiterSettings.PressedOutlineColor = Color.black;
            LimiterSettings.ReleasedOutlineColor = Color.black;
            LimiterSettings.PressedTextColor = Color.black;
            LimiterSettings.ReleasedTextColor = Color.black;
        }

        /// <inheritdoc/>
        public override void OnDisable() {
            GameObject.Destroy(keyViewer.gameObject);
        }

        private void UpdateViewerVisibility() {
            bool showViewer = true;
            if (CurrentProfile.ViewerOnlyGameplay
                && scrController.instance
                && scrConductor.instance) {
                bool playing = !scrController.instance.paused && scrConductor.instance.isGameWorld;
                showViewer &= playing;
            }
            if (showViewer != keyViewer.gameObject.activeSelf) {
                keyViewer.gameObject.SetActive(showViewer);
            }
        }
    }
}
