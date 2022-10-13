using System;
using System.Collections.Generic;
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

        [SyncTweakSettings]
        private KeyLimiterSettings Settings { get; set; }

        private static readonly bool IsAsyncInputAvailable = AdofaiTweaks.ReleaseNumber >= 97;
        private static bool displayMigrationWarning;

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public override void OnHideGUI() {
            Settings.IsListening = false;
        }

        /// <inheritdoc/>
        public override void OnSettingsGUI() {
            DrawKeyRegisterSettingsGUI();
        }

        private void DrawKeyRegisterSettingsGUI() {
            if (IsAsyncInputAvailable)
            {
                // TODO: Invoke through reflection to avoid type not found exception
                MigrationReminderGUI();
                GUILayout.Space(12f);
            }
            else if (Settings.MigratedToAsyncKeys)
            {
                // Not added string:
                // "<b>Warning!</b> Your settings were meant for asynchronous input system, " +
                // "but the this version of the game does not have asynchronous input system.\n" +
                // "If you want to use KeyLimiter again, go back to any version that is above " +
                // "{version r97} and migrate your settings back to synchronous input only."
                GUILayout.Label(TweakStrings.Get(TranslationKeys.Global.TEST_KEY));
                GUILayout.Space(12f);
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

            // Limit CLS toggle
            Settings.LimitKeyOnCLS =
                GUILayout.Toggle(
                    Settings.LimitKeyOnCLS, TweakStrings.Get(TranslationKeys.KeyLimiter.LIMIT_CLS));

            // Limit main menu toggle
            Settings.LimitKeyOnMainScreen =
                GUILayout.Toggle(
                    Settings.LimitKeyOnMainScreen,
                    TweakStrings.Get(TranslationKeys.KeyLimiter.LIMIT_MAIN_MENU));

            // Display options to migrate their settings to sync/async input if types are there
            if (IsAsyncInputAvailable)
            {
                // TODO: Invoke through reflection to avoid type not found exception
                DrawActiveKeysMigrationGUI();
            }
        }

        // TODO: Invoke through reflection to avoid type not found exception
        private void DrawActiveKeysMigrationGUI()
        {
            // Not added string: "Your settings are for {async?a:''}synchronous input system. Do you want to migrate your settings to __ input?"
            GUILayout.Label(TweakStrings.Get(TranslationKeys.Global.TEST_KEY));

            // Not added string: "Convert the settings to {async?a:''}synchronous input system"
            displayMigrationWarning |= GUILayout.Button(TweakStrings.Get(TranslationKeys.Global.TEST_KEY));

            if (displayMigrationWarning)
            {
                // TODO: Invoke through reflection to avoid type not found exception
                MigrationWarningPromptGUI();
            }

            // Not added string: "Your settings are for {async?a:''}synchronous input system. Do you want to switch to __ input?"
            GUILayout.Label(TweakStrings.Get(TranslationKeys.Global.TEST_KEY));
        }

        // TODO: Invoke through reflection to avoid type not found exception
        private void MigrationReminderGUI()
        {
            if (Settings.MigratedToAsyncKeys != Persistence.GetChosenAsynchronousInput())
            {
                // Not added string: "Warning: Your settings are for {async?a:''}synchronous input system, " +
                // "but the current input system is {}. Consider migrating the mod settings or toggling async input."
                GUILayout.Label(TweakStrings.Get(TranslationKeys.Global.TEST_KEY));
            }
        }

        // TODO: Invoke through reflection to avoid type not found exception
        private void MigrationWarningPromptGUI()
        {
            MoreGUILayout.BeginIndent();
            // Not added string: "You are about to change the settings. Are you sure? The migration is only possible in versions above r97"
            GUILayout.Label(TweakStrings.Get(TranslationKeys.Global.TEST_KEY));
            GUILayout.BeginHorizontal();
            // Not added string: "Yes"
            if (GUILayout.Button(TweakStrings.Get(TranslationKeys.Global.TEST_KEY)))
            {
                MigrateActiveKeys();
                displayMigrationWarning = false;
            }
            // Not added string: "No"
            if (GUILayout.Button(TweakStrings.Get(TranslationKeys.Global.TEST_KEY)))
            {
                displayMigrationWarning = false;
            }
            GUILayout.EndHorizontal();
            MoreGUILayout.EndIndent();
        }

        private void MigrateActiveKeys()
        {
            if (Settings.MigratedToAsyncKeys)
            {
                // TODO: Migrate back to sync keys
                AdofaiTweaks.Logger.Log("[KeyLimiterTweak] Test async -> sync migration");
                Settings.MigratedToAsyncKeys = false;
            }
            else
            {
                // TODO: Migrate to async keys
                AdofaiTweaks.Logger.Log("[KeyLimiterTweak] Test sync -> async migration");
                Settings.MigratedToAsyncKeys = true;
            }
        }
    }
}
