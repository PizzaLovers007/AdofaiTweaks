using System.Xml.Serialization;
using AdofaiTweaks.Core;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.PlanetColor
{
    public class PlanetColorSettings : TweakSettings
    {
        private Color _color1;

        public Color Color1 {
            get => _color1;
            set {
                _color1 = value;
                Color1Hex = ColorUtility.ToHtmlStringRGB(value);
            }
        }

        private Color _color2;

        public Color Color2 {
            get => _color2;
            set {
                _color2 = value;
                Color2Hex = ColorUtility.ToHtmlStringRGB(value);
            }
        }

        private Color _tailColor1;

        public Color TailColor1 {
            get => _tailColor1;
            set {
                _tailColor1 = value;
                TailColor1Hex = ColorUtility.ToHtmlStringRGB(value);
            }
        }

        private Color _tailColor2;

        public Color TailColor2 {
            get => _tailColor2;
            set {
                _tailColor2 = value;
                TailColor2Hex = ColorUtility.ToHtmlStringRGB(value);
            }
        }

        [XmlIgnore]
        public string Color1Hex { get; set; }

        [XmlIgnore]
        public string Color2Hex { get; set; }

        [XmlIgnore]
        public string TailColor1Hex { get; set; }

        [XmlIgnore]
        public string TailColor2Hex { get; set; }

        [XmlIgnore]
        public Color OriginalColor1 { get; set; }

        [XmlIgnore]
        public Color OriginalColor2 { get; set; }

        public PlanetColorSettings() {
            Color1 = Color.white;
            Color2 = Color.white;
            TailColor1 = Color.white;
            TailColor2 = Color.white;
        }
    }
}
