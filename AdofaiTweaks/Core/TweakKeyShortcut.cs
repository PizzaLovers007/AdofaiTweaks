using System;
using System.Collections.Generic;
using AdofaiTweaks.Strings;
using UnityEngine;

namespace AdofaiTweaks.Core
{
    /// <summary>
    /// A key shortcut you can check every frame.
    /// </summary>
    public class TweakKeyShortcut
    {
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
        /// Initializes a new instance of the <see cref="TweakKeyShortcut"/> class.
        /// </summary>
        public TweakKeyShortcut() { }

        [NonSerialized]
        private bool _isListening;

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
        /// Check if this shortcut is pressed in current frame.
        /// SHOULD BE ONLY CALLED IN <c>Update()</c>.
        /// </summary>
        /// <returns>Whether the shortcut is precisely triggered.</returns>
        public bool CheckShortcut() =>
            PressCtrl == RDInput.holdingControl && PressShift == RDInput.holdingShift && PressAlt == RDInput.holdingAlt && Input.GetKeyDown(PressKey);

        /// <summary>
        /// Visualize shortcut options.
        /// SHOULD BE ONLY CALLED IN <c>OnGUI()</c>.
        /// </summary>
        /// <param name="labelText">The label text that is shown before the shortcut GUI.</param>
        public void OnGUI(string labelText = null)
        {
            GUILayout.BeginHorizontal();

            if (!string.IsNullOrEmpty(labelText))
            {
                GUILayout.Label(labelText);
            }

            // Control
            PressCtrl = GUILayout.Toggle(PressCtrl, "Control");
            GUILayout.Label("+");

            // Shift
            PressShift = GUILayout.Toggle(PressShift, "Shift");
            GUILayout.Label("+");

            // Alt
            PressAlt = GUILayout.Toggle(PressAlt, "Alt");
            GUILayout.Label("+");

            // Keycode
            GUILayout.TextField(PressKey.ToString());
            if (GUILayout.Button(TweakStrings.Get(_isListening ? TranslationKeys.Global.SHORTCUT_CHANGE_KEY : TranslationKeys.Global.SHORTCUT_DONE)))
            {
                _isListening = !_isListening;
            }

            if (_isListening)
            {
                foreach (KeyCode code in Enum.GetValues(typeof(KeyCode))) {
                    // Skip key if not pressed or blacklisted
                    if (!Input.GetKeyDown(code) || BLACKLISTED_KEYS.Contains(code)) {
                        continue;
                    }

                    // Change key
                    PressKey = code;
                }
            }

            // Exit scope
            GUILayout.EndHorizontal();
        }
    }
}