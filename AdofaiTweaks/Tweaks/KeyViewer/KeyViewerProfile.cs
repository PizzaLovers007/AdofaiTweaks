﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.KeyViewer
{
    public class KeyViewerProfile
    {
        public string Name { get; set; }
        public List<KeyCode> ActiveKeys { get; set; } = new List<KeyCode>();
        public bool ViewerOnlyGameplay { get; set; }
        public bool AnimateKeys { get; set; } = true;
        public bool ShowKeyPressTotal { get; set; } = true;
        public float KeyViewerSize { get; set; } = 100f;
        public float KeyViewerXPos { get; set; } = 0.89f;
        public float KeyViewerYPos { get; set; } = 0.03f;

        private Color _pressedOutlineColor;
        public Color PressedOutlineColor {
            get => _pressedOutlineColor;
            set {
                _pressedOutlineColor = value;
                PressedOutlineColorHex = ColorUtility.ToHtmlStringRGBA(value);
            }
        }

        private Color _releasedOutlineColor;
        public Color ReleasedOutlineColor {
            get => _releasedOutlineColor;
            set {
                _releasedOutlineColor = value;
                ReleasedOutlineColorHex = ColorUtility.ToHtmlStringRGBA(value);
            }
        }

        private Color _pressedBackgroundColor;
        public Color PressedBackgroundColor {
            get => _pressedBackgroundColor;
            set {
                _pressedBackgroundColor = value;
                PressedBackgroundColorHex = ColorUtility.ToHtmlStringRGBA(value);
            }
        }

        private Color _releasedBackgroundColor;
        public Color ReleasedBackgroundColor {
            get => _releasedBackgroundColor;
            set {
                _releasedBackgroundColor = value;
                ReleasedBackgroundColorHex = ColorUtility.ToHtmlStringRGBA(value);
            }
        }

        private Color _pressedTextColor;
        public Color PressedTextColor {
            get => _pressedTextColor;
            set {
                _pressedTextColor = value;
                PressedTextColorHex = ColorUtility.ToHtmlStringRGBA(value);
            }
        }

        private Color _releasedTextColor;
        public Color ReleasedTextColor {
            get => _releasedTextColor;
            set {
                _releasedTextColor = value;
                ReleasedTextColorHex = ColorUtility.ToHtmlStringRGBA(value);
            }
        }

        [XmlIgnore]
        public string PressedOutlineColorHex { get; set; }

        [XmlIgnore]
        public string ReleasedOutlineColorHex { get; set; }

        [XmlIgnore]
        public string PressedBackgroundColorHex { get; set; }

        [XmlIgnore]
        public string ReleasedBackgroundColorHex { get; set; }

        [XmlIgnore]
        public string PressedTextColorHex { get; set; }

        [XmlIgnore]
        public string ReleasedTextColorHex { get; set; }

        public KeyViewerProfile() {
            PressedOutlineColor = Color.white;
            ReleasedOutlineColor = Color.white;
            PressedBackgroundColor = Color.white;
            ReleasedBackgroundColor = Color.black.WithAlpha(0.4f);
            PressedTextColor = Color.black;
            ReleasedTextColor = Color.white;
        }

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
