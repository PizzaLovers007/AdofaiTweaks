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

        public override void OnUpdate(float deltaTime) {
            UpdateRegisteredKeys();
        }

        private void UpdateRegisteredKeys() {
            if (!Settings.IsListening) {
                return;
            }

            foreach (KeyCode code in Enum.GetValues(typeof(KeyCode))) {
                // Skip key if not pressed or should always be bound
                if (!Input.GetKeyDown(code) || ALWAYS_BOUND_KEYS.Contains(code)) {
                    continue;
                }

                // Register/unregister the key
                if (Settings.ActiveKeys.Contains(code)) {
                    Settings.ActiveKeys.Remove(code);
                } else {
                    Settings.ActiveKeys.Add(code);
                }
            }
        }

        public override void OnHideGUI() {
            Settings.IsListening = false;
        }

        public override void OnSettingsGUI() {
            DrawKeyRegisterSettingsGUI();
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
    }
}
