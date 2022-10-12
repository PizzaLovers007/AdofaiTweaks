using System.Collections.Generic;
using System.Xml.Serialization;
using AdofaiTweaks.Core;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.KeyLimiter
{
    /// <summary>
    /// Settings for the Key Limiter tweak.
    /// </summary>
    public class KeyLimiterSettings : TweakSettings
    {
        /// <summary>
        /// The synchronous input keys that are counted as input.
        /// </summary>
        public List<KeyCode> ActiveKeys { get; set; } = new ();

        /// <summary>
        /// The asynchronous input keys that are counted as input.
        /// </summary>
        public List<object> ActiveAsyncKeys { get; set; } = new ();

        /// <summary>
        /// Old setting for showing the key viewer.
        /// </summary>
        public bool ShowKeyViewer { get; set; }

        /// <summary>
        /// Old setting for showing the key viewer only in gameplay.
        /// </summary>
        public bool ViewerOnlyGameplay { get; set; }

        /// <summary>
        /// Old setting for animating key presses.
        /// </summary>
        public bool AnimateKeys { get; set; } = true;

        /// <summary>
        /// Old setting for the key viewer size.
        /// </summary>
        public float KeyViewerSize { get; set; } = 100f;

        /// <summary>
        /// Old setting for the horizontal position of the key viewer.
        /// </summary>
        public float KeyViewerXPos { get; set; } = 0.89f;

        /// <summary>
        /// Old setting for the vertical position of the key viewer.
        /// </summary>
        public float KeyViewerYPos { get; set; } = 0.03f;

        /// <summary>
        /// Limit key on custom level selection scene.
        /// </summary>
        public bool LimitKeyOnCLS { get; set; } = true;

        /// <summary>
        /// Limit key on main screen.
        /// </summary>
        public bool LimitKeyOnMainScreen { get; set; } = true;

        /// <summary>
        /// Whether the user has migrated their KeyLimiter settings to async input version.
        /// </summary>
        public bool MigratedToAsyncKeys { get; set; }

        private Color _pressedOutlineColor;

        /// <summary>
        /// Old setting for the outline color of pressed keys.
        /// </summary>
        public Color PressedOutlineColor {
            get => _pressedOutlineColor;
            set {
                _pressedOutlineColor = value;
                PressedOutlineColorHex = ColorUtility.ToHtmlStringRGBA(value);
            }
        }

        private Color _releasedOutlineColor;

        /// <summary>
        /// Old setting for the outline color of released keys.
        /// </summary>
        public Color ReleasedOutlineColor {
            get => _releasedOutlineColor;
            set {
                _releasedOutlineColor = value;
                ReleasedOutlineColorHex = ColorUtility.ToHtmlStringRGBA(value);
            }
        }

        private Color _pressedBackgroundColor;

        /// <summary>
        /// Old setting for the background/fill color of pressed keys.
        /// </summary>
        public Color PressedBackgroundColor {
            get => _pressedBackgroundColor;
            set {
                _pressedBackgroundColor = value;
                PressedBackgroundColorHex = ColorUtility.ToHtmlStringRGBA(value);
            }
        }

        private Color _releasedBackgroundColor;

        /// <summary>
        /// Old setting for the background/fill color of released keys.
        /// </summary>
        public Color ReleasedBackgroundColor {
            get => _releasedBackgroundColor;
            set {
                _releasedBackgroundColor = value;
                ReleasedBackgroundColorHex = ColorUtility.ToHtmlStringRGBA(value);
            }
        }

        private Color _pressedTextColor;

        /// <summary>
        /// Old setting for the text color of pressed keys.
        /// </summary>
        public Color PressedTextColor {
            get => _pressedTextColor;
            set {
                _pressedTextColor = value;
                PressedTextColorHex = ColorUtility.ToHtmlStringRGBA(value);
            }
        }

        private Color _releasedTextColor;

        /// <summary>
        /// Old setting for the text color of released keys.
        /// </summary>
        public Color ReleasedTextColor {
            get => _releasedTextColor;
            set {
                _releasedTextColor = value;
                ReleasedTextColorHex = ColorUtility.ToHtmlStringRGBA(value);
            }
        }

        /// <summary>
        /// Whether the tweak is listening for keystrokes to change what keys
        /// are active.
        /// </summary>
        [XmlIgnore]
        public bool IsListening { get; set; }

        /// <summary>
        /// Old setting for the hex code for the pressed outline color.
        /// </summary>
        [XmlIgnore]
        private string PressedOutlineColorHex { get; set; }

        /// <summary>
        /// Old setting for the hex code for the released outline color.
        /// </summary>
        [XmlIgnore]
        private string ReleasedOutlineColorHex { get; set; }

        /// <summary>
        /// Old setting for the hex code for the pressed background/fill color.
        /// </summary>
        [XmlIgnore]
        private string PressedBackgroundColorHex { get; set; }

        /// <summary>
        /// Old setting for the hex code for the released background/fill color.
        /// </summary>
        [XmlIgnore]
        private string ReleasedBackgroundColorHex { get; set; }

        /// <summary>
        /// Old setting for the hex code for the pressed text color.
        /// </summary>
        [XmlIgnore]
        private string PressedTextColorHex { get; set; }

        /// <summary>
        /// Old setting for the hex code for the released text color.
        /// </summary>
        [XmlIgnore]
        private string ReleasedTextColorHex { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyLimiterSettings"/>
        /// class with some default values set.
        /// </summary>
        public KeyLimiterSettings() {
            // Settings no one would ever use
            PressedOutlineColor = Color.black;
            ReleasedOutlineColor = Color.black;
            PressedBackgroundColor = Color.black;
            ReleasedBackgroundColor = Color.black;
            PressedTextColor = Color.black;
            ReleasedTextColor = Color.black;
        }
    }
}
