﻿using System;
using System.Collections.Generic;
using System.Text;
using AdofaiTweaks.Compat.Async;
using AdofaiTweaks.Core;
using AdofaiTweaks.Core.Attributes;
using AdofaiTweaks.Strings;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.KeyLimiter;

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

        if (AsyncInputManagerCompat.IsAsyncInputEnabled) {
            foreach (var key in AsyncInputManagerCompat.GetKeysDownThisFrame()) {
                // Register/unregister the key
                if (Settings.ActiveAsyncKeys.Contains(key)) {
                    Settings.ActiveAsyncKeys.Remove(key);
                } else {
                    Settings.ActiveAsyncKeys.Add(key);
                }
            }
        } else {
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

    /// <inheritdoc/>
    public override void OnHideGUI() {
        Settings.IsListening = false;
    }

    /// <inheritdoc/>
    public override void OnSettingsGUI() {
        AsyncInputManagerCompat.UpdateAsyncKeyCache();
        DrawKeyRegisterSettingsGUI();
    }

    private void DrawKeyRegisterSettingsGUI() {
        // Input type header
        if (AsyncInputManagerCompat.IsAsyncAvailable) {
            GUILayout.Label(TweakStrings.Get(
                TranslationKeys.KeyLimiter.SELECTED_INPUT_SYSTEM,
                AsyncInputManagerCompat.IsAsyncInputEnabled ?
                    TweakStrings.Get(TranslationKeys.KeyLimiter.ASYNCHRONOUS_INPUT_SYSTEM) :
                    TweakStrings.Get(TranslationKeys.KeyLimiter.SYNCHRONOUS_INPUT_SYSTEM)));
            GUILayout.Space(12f);
        }

        // List of registered keys
        GUILayout.Label(TweakStrings.Get(TranslationKeys.KeyLimiter.REGISTERED_KEYS));
        GUILayout.BeginHorizontal();
        GUILayout.Space(20f);
        GUILayout.BeginVertical();
        GUILayout.Space(8f);
        GUILayout.EndVertical();
        if (AsyncInputManagerCompat.IsAsyncInputEnabled) {
            foreach (var code in Settings.ActiveAsyncKeys) {
                var label = new StringBuilder();

                label.Append(code);
                label.Append('(');
                label.Append(AsyncInputManagerCompat.GetLabel(code));
                label.Append(')');

                GUILayout.Label(label.ToString());
                GUILayout.Space(8f);
            }
        } else {
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
            if (GUILayout.Button(TweakStrings.Get(TranslationKeys.KeyLimiter.CLEAR_ALL_KEYS))) {
                if (AsyncInputManagerCompat.IsAsyncInputEnabled) {
                    Settings.ActiveAsyncKeys.Clear();
                } else {
                    Settings.ActiveKeys.Clear();
                }
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
}