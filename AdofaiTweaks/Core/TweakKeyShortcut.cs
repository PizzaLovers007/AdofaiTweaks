﻿using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using AdofaiTweaks.Strings;
using UnityEngine;

namespace AdofaiTweaks.Core;

/// <summary>
/// A key shortcut you can check every frame.
/// </summary>
public class TweakKeyShortcut
{
    [NonSerialized]
    private readonly ISet<KeyCode> BLACKLISTED_KEYS = new HashSet<KeyCode>
    {
        KeyCode.LeftControl,
        KeyCode.LeftCommand,
        KeyCode.LeftApple,
        KeyCode.LeftShift,
        KeyCode.LeftAlt,
        KeyCode.RightControl,
        KeyCode.RightCommand,
        KeyCode.RightApple,
        KeyCode.RightShift,
        KeyCode.RightAlt,
        KeyCode.Mouse0,
        KeyCode.Mouse1,
        KeyCode.Mouse2,
        KeyCode.Mouse3,
        KeyCode.Mouse4,
        KeyCode.Mouse5,
        KeyCode.Mouse6,
    };

    /// <summary>
    /// Whether ctrl should be pressed to trigger this shortcut.
    /// </summary>
    public bool PressCtrl { get; set; }

    /// <summary>
    /// Whether shift should be pressed to trigger this shortcut.
    /// </summary>
    public bool PressShift { get; set; }

    /// <summary>
    /// Whether alt should be pressed to trigger this shortcut.
    /// </summary>
    public bool PressAlt { get; set; }

    /// <summary>
    /// Key that should be pressed to trigger this shortcut.
    /// </summary>
    public KeyCode PressKey { get; set; }

    /// <summary>
    /// Whether AdofaiTweaks is listening for key changes to the shortcut.
    /// </summary>
    [XmlIgnore]
    public bool IsListening { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TweakKeyShortcut"/>
    /// class.
    /// </summary>
    public TweakKeyShortcut() { }

    /// <summary>
    /// Check if this shortcut is pressed in current frame. SHOULD BE ONLY
    /// CALLED IN <c>Update()</c>.
    /// </summary>
    /// <returns>Whether the shortcut is precisely triggered.</returns>
    public bool CheckShortcut() {
        bool holdingCtrl =
            Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)
                                              || Input.GetKey(KeyCode.LeftMeta) || Input.GetKey(KeyCode.RightMeta);
        bool holdingShift =
            Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        bool holdingAlt =
            Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);

        return PressCtrl == holdingCtrl
               && PressShift == holdingShift
               && PressAlt == holdingAlt
               && Input.GetKeyDown(PressKey);
    }

    /// <summary>
    /// Visualize shortcut options. SHOULD BE ONLY CALLED IN <c>OnGUI()</c>.
    /// </summary>
    /// <param name="labelText">
    /// The label text that is shown before the shortcut GUI.
    /// </param>
    public void DrawShortcut(string labelText = null) {
        GUILayout.BeginHorizontal();

        if (!string.IsNullOrEmpty(labelText)) {
            GUILayout.Label(labelText);
            GUILayout.Space(8f);
        }

        // Control
        PressCtrl = GUILayout.Toggle(PressCtrl, "Control");
        GUILayout.Label(" + ");

        // Shift
        PressShift = GUILayout.Toggle(PressShift, "Shift");
        GUILayout.Label(" + ");

        // Alt
        PressAlt = GUILayout.Toggle(PressAlt, "Alt");
        GUILayout.Label(" + ");

        // Keycode
        var oldColor = GUI.color;
        if (IsListening) {
            GUI.color = new Color(1f, 0.5f, 0f); // orange
        }
        GUILayout.Box(PressKey.ToString());
        GUI.color = oldColor;
        string changeOrDoneText =
            TweakStrings.Get(IsListening
                ? TranslationKeys.Global.SHORTCUT_DONE
                : TranslationKeys.Global.SHORTCUT_CHANGE_KEY);
        GUILayout.Space(8f);
        if (GUILayout.Button(changeOrDoneText)) {
            IsListening = !IsListening;
        }

        if (IsListening) {
            foreach (KeyCode code in Enum.GetValues(typeof(KeyCode))) {
                // Skip key if not pressed or blacklisted
                if (!Input.GetKeyDown(code) || BLACKLISTED_KEYS.Contains(code)) {
                    continue;
                }

                // Change key
                PressKey = code;
                IsListening = false;
            }
        }

        GUILayout.FlexibleSpace();

        // Exit scope
        GUILayout.EndHorizontal();
    }
}