using System;
using System.Collections.Generic;
using System.Linq;
using AdofaiTweaks.Core;
using AdofaiTweaks.Core.Attributes;
using AdofaiTweaks.Strings;
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
        public static readonly ISet<KeyCode> ALWAYS_BOUND_KEYS = new HashSet<KeyCode>() {
            KeyCode.Mouse0,
            KeyCode.Mouse1,
            KeyCode.Mouse2,
            KeyCode.Mouse3,
            KeyCode.Mouse4,
            KeyCode.Mouse5,
            KeyCode.Mouse6,
        };

        /// <summary>
        /// Always bound keys but async. Initialized at static constructor.
        /// </summary>
        public static readonly ISet<ushort> ALWAYS_BOUND_ASYNC_KEYS;

        private static bool GetAsyncInputEnabled() {
            return AsyncInputManager.isActive;
        }

        static KeyLimiterTweak()
        {
            // Do not process if types aren't there
            if (!GameVersionState.AsyncInputAvailable)
            {
                return;
            }

            IDictionary<KeyCode, ushort> unityNativeKeymap = KeyCodeConverter.UnityNativeKeyCodeList
                .GroupBy(x => x)
                .ToDictionary(g => g.Key.Item1, g => g.First().Item2);

            ALWAYS_BOUND_ASYNC_KEYS = ALWAYS_BOUND_KEYS
                .Select(k => unityNativeKeymap.TryGetValue(k, out ushort a) ? a : MutualKeyCode.AsyncNullKeyCode)
                .ToHashSet();

            ALWAYS_BOUND_ASYNC_KEYS.Remove(MutualKeyCode.AsyncNullKeyCode);
        }

        [SyncTweakSettings]
        private KeyLimiterSettings Settings { get; set; }

        /// <inheritdoc/>
        public override void OnUpdate(float deltaTime) {
            UpdateRegisteredKeys();
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
            foreach (var code in AsyncInputManager.frameDependentKeyDownMask.Except(ALWAYS_BOUND_ASYNC_KEYS)) {
                // Register/unregister the key
                if (Settings.ActiveAsyncKeys.Contains(code)) {
                    Settings.ActiveAsyncKeys.Remove(code);
                } else {
                    Settings.ActiveAsyncKeys.Add(code);
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
            // Not added string: "Currently selected input system: {AsyncInputManager.isActive ? 'async':'sync'}\n" +
            // "Note that the async key and sync keycodes are not shared nor migratable. " +
            // "You need to configure both sync and async input independently."
            GUILayout.Label(TweakStrings.Get(TranslationKeys.Global.TEST_KEY));
        }

        // Separated to another method to avoid type not found exception in older game versions
        private void LabelActiveAsyncKeys()
        {
            foreach (var code in Settings.ActiveAsyncKeys) {
                GUILayout.Label(((SharpHook.Native.KeyCode)code).ToString().Replace("Vc", ""));
                GUILayout.Space(8f);
            }
        }
    }
}
