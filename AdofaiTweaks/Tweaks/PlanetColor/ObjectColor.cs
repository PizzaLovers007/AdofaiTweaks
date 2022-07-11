using System.Xml.Serialization;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.PlanetColor
{
    /// <summary>
    /// Class for storing specific color data.
    /// </summary>
    public class ObjectColor
    {
        /// <summary>
        /// The coloring type for planet's body.
        /// </summary>
        public PlanetColorType ColorType { get; set; }

        private Color _plainColor;

        /// <summary>
        /// The plain color.
        /// </summary>
        public Color PlainColor {
            get => _plainColor;
            set {
                _plainColor = value;
                PlainColorHex = ColorUtility.ToHtmlStringRGB(value);
            }
        }

        /// <summary>
        /// The hex for the color.
        /// </summary>
        [XmlIgnore]
        public string PlainColorHex { get; set; }

        /// <summary>
        /// The gradient.
        /// </summary>
        public Gradient Gradient { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectColor"/> class
        /// with a default color.
        /// </summary>
        public ObjectColor() {
            PlainColor = Color.white;
        }
    }
}
