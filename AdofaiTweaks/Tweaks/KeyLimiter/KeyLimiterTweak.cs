using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using AdofaiTweaks.Core;
using AdofaiTweaks.Core.Attributes;
using AdofaiTweaks.Strings;
using HarmonyLib;
using SkyHook;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.KeyLimiter
{
    /// <summary>
    /// A tweak for restricting which keys are counted as input.
    /// </summary>
    [RegisterTweak(
        id: "key_limiter",
        settingsType: typeof(KeyLimiterSettings),
        patchesType: typeof(KeyLimiterPatches))]
    public class KeyLimiterTweak : Tweak
    {
        /// <inheritdoc/>
        public override string Name =>
            TweakStrings.Get(TranslationKeys.KeyLimiter.NAME);

        /// <inheritdoc/>
        public override string Description =>
            TweakStrings.Get(TranslationKeys.KeyLimiter.DESCRIPTION);

        /// <summary>
        /// A set of keys that will always be counted as input.
        /// </summary>
        public static readonly ISet<KeyCode> ALWAYS_BOUND_KEYS = new HashSet<KeyCode> {
            KeyCode.Mouse0,
            KeyCode.Mouse1,
            KeyCode.Mouse2,
            KeyCode.Mouse3,
            KeyCode.Mouse4,
            KeyCode.Mouse5,
            KeyCode.Mouse6,
        };

        /// <summary>
        /// Always bound keys but for async input. Initialized at static constructor.
        /// </summary>
        public static readonly ISet<KeyLabel> ALWAYS_BOUND_ASYNC_KEYS = new HashSet<KeyLabel> {
            KeyLabel.MouseLeft,
            KeyLabel.MouseMiddle,
            KeyLabel.MouseRight,
            KeyLabel.MouseX1,
            KeyLabel.MouseX2,
        };

        /// <summary>
        /// Dictionary of skyhook's ushort keycodes matched to key labels.
        /// <br/>
        /// This is only cached in runtime and will always not be a full list of all key codes.
        /// </summary>
        public static ReadOnlyDictionary<ushort, KeyLabel> CODE_TO_LABEL_DICT => new ReadOnlyDictionary<ushort, KeyLabel>(codeToLabelDict);
        private static Dictionary<ushort, KeyLabel> codeToLabelDict = new Dictionary<ushort, KeyLabel>();

        private static bool GetAsyncInputEnabled() {
            return AsyncInputManager.isActive;
        }

        [SyncTweakSettings]
        private KeyLimiterSettings Settings { get; set; }

        /// <inheritdoc/>
        public override void OnUpdate(float deltaTime) {
            UpdateRegisteredKeys();

            // Key up events are always called regardless of application's focused state
            foreach (var code in AsyncInputManager.frameDependentKeyUpMask) {
                if (!codeToLabelDict.ContainsKey(code.key)) {
                    codeToLabelDict.Add(code.key, code.label);
                }
            }
        }

        private void UpdateRegisteredKeys() {
            if (!Settings.IsListening) {
                return;
            }

            bool isAsyncInputEnabled = false;
            if (GameVersionState.AsyncInputAvailable)
            {
                // Calling as a method to avoid exception
                isAsyncInputEnabled = GetAsyncInputEnabled();
            }

            if (isAsyncInputEnabled) {
                IterateAndUpdateRegisteredAsyncKeys();
            }
            else
            {
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
        }

        // Separated to another method to avoid type not found exception in older game versions
        private void IterateAndUpdateRegisteredAsyncKeys() {
            foreach (var code in AsyncInputManager.frameDependentKeyDownMask) {
                // Register/unregister the key
                var key = code.key;
                if (Settings.ActiveAsyncKeys.Contains(key)) {
                    Settings.ActiveAsyncKeys.Remove(key);
                } else {
                    Settings.ActiveAsyncKeys.Add(key);
                }
            }
        }

        /// <inheritdoc/>
        public override void OnHideGUI() {
            Settings.IsListening = false;
        }

        /// <inheritdoc/>
        public override void OnSettingsGUI() {
            DrawKeyRegisterSettingsGUI();
        }

        private void DrawKeyRegisterSettingsGUI() {
            bool isAsyncInputEnabled = false;
            if (GameVersionState.AsyncInputAvailable)
            {
                DisplaySelectedInputSystemGUI();
                GUILayout.Space(12f);

                // Calling as a method to avoid exception
                isAsyncInputEnabled = GetAsyncInputEnabled();
            }

            // List of registered keys
            GUILayout.Label(TweakStrings.Get(TranslationKeys.KeyLimiter.REGISTERED_KEYS));
            GUILayout.BeginHorizontal();
            GUILayout.Space(20f);
            GUILayout.BeginVertical();
            GUILayout.Space(8f);
            GUILayout.EndVertical();
            if (isAsyncInputEnabled)
            {
                LabelActiveAsyncKeys();
            }
            else
            {
                foreach (KeyCode code in Settings.ActiveKeys) {
                    GUILayout.Label(code.ToString());
                    GUILayout.Space(8f);
                }
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

            // Limit CLS toggle
            Settings.LimitKeyOnCLS =
                GUILayout.Toggle(
                    Settings.LimitKeyOnCLS, TweakStrings.Get(TranslationKeys.KeyLimiter.LIMIT_CLS));

            // Limit main menu toggle
            Settings.LimitKeyOnMainScreen =
                GUILayout.Toggle(
                    Settings.LimitKeyOnMainScreen,
                    TweakStrings.Get(TranslationKeys.KeyLimiter.LIMIT_MAIN_MENU));
        }

        // Separated to another method to avoid type not found exception in older game versions
        private void DisplaySelectedInputSystemGUI()
        {
            GUILayout.Label(TweakStrings.Get(
                TranslationKeys.KeyLimiter.SELECTED_INPUT_SYSTEM,
                AsyncInputManager.isActive ?
                    TweakStrings.Get(TranslationKeys.KeyLimiter.ASYNCHRONOUS_INPUT_SYSTEM) :
                    TweakStrings.Get(TranslationKeys.KeyLimiter.SYNCHRONOUS_INPUT_SYSTEM)));
        }

        // Separated to another method to avoid type not found exception in older game versions
        private void LabelActiveAsyncKeys()
        {
            foreach (var code in Settings.ActiveAsyncKeys) {
                var label = new StringBuilder($"{code}(");

                if (codeToLabelDict.TryGetValue(code, out KeyLabel keyLabel)) {
                    label.Append(keyLabel);
                } else {
                    label.Append("Label Not Cached");
                }

                label.Append(')');
                GUILayout.Label(label.ToString());
                GUILayout.Space(8f);
            }
        }
    }
}
