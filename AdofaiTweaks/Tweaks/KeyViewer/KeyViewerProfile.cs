using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.KeyViewer
{
    /// <summary>
    /// Settings for a key viewer profile that controls properties such as
    /// colors and position/size.
    /// </summary>
    public class KeyViewerProfile
    {
        /// <summary>
        /// The user-defined name for the profile.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The list of keys to show in the key viewer.
        /// </summary>
        public List<KeyCode> ActiveKeys { get; set; } = new List<KeyCode>();

        /// <summary>
        /// Whether the key viewer should only be shown in gameplay.
        /// </summary>
        public bool ViewerOnlyGameplay { get; set; }

        /// <summary>
        /// Whether the key presses should be animated.
        /// </summary>
        public bool AnimateKeys { get; set; } = true;

        /// <summary>
        /// The total number of presses since the game started.
        /// </summary>
        public bool ShowKeyPressTotal { get; set; } = true;

        /// <summary>
        /// The size of the key viewer.
        /// </summary>
        public float KeyViewerSize { get; set; } = 100f;

        /// <summary>
        /// The horizontal position of the key viewer. Should be bound to the
        /// range <c>[0, 1]</c>.
        /// </summary>
        public float KeyViewerXPos { get; set; } = 0.89f;

        /// <summary>
        /// The vertical position of the key viewer. Should be bound to the
        /// range <c>[0, 1]</c>.
        /// </summary>
        public float KeyViewerYPos { get; set; } = 0.03f;

        private Color _pressedOutlineColor;

        /// <summary>
        /// The outline color of pressed keys.
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
        /// The outline color of released keys.
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
        /// The background/fill color of pressed keys.
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
        /// The background/fill color of released keys.
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
        /// The text color of pressed keys.
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
        /// The text color of released keys.
        /// </summary>
        public Color ReleasedTextColor {
            get => _releasedTextColor;
            set {
                _releasedTextColor = value;
                ReleasedTextColorHex = ColorUtility.ToHtmlStringRGBA(value);
            }
        }

        /// <summary>
        /// The hex code for the pressed outline color.
        /// </summary>
        [XmlIgnore]
        public string PressedOutlineColorHex { get; set; }

        /// <summary>
        /// The hex code for the released outline color.
        /// </summary>
        [XmlIgnore]
        public string ReleasedOutlineColorHex { get; set; }

        /// <summary>
        /// The hex code for the pressed background/fill color.
        /// </summary>
        [XmlIgnore]
        public string PressedBackgroundColorHex { get; set; }

        /// <summary>
        /// The hex code for the released background/fill color.
        /// </summary>
        [XmlIgnore]
        public string ReleasedBackgroundColorHex { get; set; }

        /// <summary>
        /// The hex code for the pressed text color.
        /// </summary>
        [XmlIgnore]
        public string PressedTextColorHex { get; set; }

        /// <summary>
        /// The hex code for the released text color.
        /// </summary>
        [XmlIgnore]
        public string ReleasedTextColorHex { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyViewerProfile"/>
        /// class with some default colors.
        /// </summary>
        public KeyViewerProfile() {
            PressedOutlineColor = Color.white;
            ReleasedOutlineColor = Color.white;
            PressedBackgroundColor = Color.white;
            ReleasedBackgroundColor = Color.black.WithAlpha(0.4f);
            PressedTextColor = Color.black;
            ReleasedTextColor = Color.white;
        }

        /// <summary>
        /// Creates a copy of <c>this</c>.
        /// </summary>
        /// <returns>A copy of <c>this</c>.</returns>
        public KeyViewerProfile Copy() {
            using (var ms = new MemoryStream()) {
                var serializer = new XmlSerializer(GetType());
                serializer.Serialize(ms, this);
                ms.Position = 0;
                return (KeyViewerProfile)serializer.Deserialize(ms);
            }
        }
    }
}
