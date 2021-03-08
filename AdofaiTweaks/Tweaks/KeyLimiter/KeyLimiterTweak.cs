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
            // Key viewer toggle
            bool newShow =
                GUILayout.Toggle(
                    Settings.ShowKeyViewer,
                    TweakStrings.Get(TranslationKeys.KeyLimiter.SHOW_KEY_VIEWER));
            if (newShow != Settings.ShowKeyViewer) {
                Settings.ShowKeyViewer = newShow;
                keyViewer.gameObject.SetActive(Settings.ShowKeyViewer);
            }

            // Key viewer settings
            if (Settings.ShowKeyViewer) {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20f);
                GUILayout.BeginVertical();

                // Size slider
                GUILayout.BeginHorizontal();
                GUILayout.Label(TweakStrings.Get(TranslationKeys.KeyLimiter.KEY_VIEWER_SIZE));
                GUILayout.Space(4f);
                float newSize =
                    GUILayout.HorizontalSlider(
                        Settings.KeyViewerSize, 10f, 200f, GUILayout.Width(300f));
                GUILayout.Space(4f);
                GUILayout.Label(Settings.KeyViewerSize + "", GUILayout.Width(50f));
                newSize = Mathf.Round(newSize);
                if (newSize != Settings.KeyViewerSize) {
                    Settings.KeyViewerSize = newSize;
                    keyViewer.UpdateLayout();
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label(TweakStrings.Get(TranslationKeys.KeyLimiter.KEY_VIEWER_X_POS));
                GUILayout.Space(4f);
                float newX =
                    GUILayout.HorizontalSlider(
                        Settings.KeyViewerXPos, 0f, 1f, GUILayout.Width(300f));
                GUILayout.Space(4f);
                GUILayout.Label(Settings.KeyViewerXPos + "", GUILayout.Width(50f));
                newX = Mathf.Round(newX * 100) / 100;
                if (newX != Settings.KeyViewerXPos) {
                    Settings.KeyViewerXPos = newX;
                    keyViewer.UpdateLayout();
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label(TweakStrings.Get(TranslationKeys.KeyLimiter.KEY_VIEWER_Y_POS));
                GUILayout.Space(4f);
                float newY =
                    GUILayout.HorizontalSlider(
                        Settings.KeyViewerYPos, 0f, 1f, GUILayout.Width(300f));
                GUILayout.Space(4f);
                GUILayout.Label(Settings.KeyViewerYPos + "", GUILayout.Width(50f));
                newY = Mathf.Round(newY * 100) / 100;
                if (newY != Settings.KeyViewerYPos) {
                    Settings.KeyViewerYPos = newY;
                    keyViewer.UpdateLayout();
                }
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

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
