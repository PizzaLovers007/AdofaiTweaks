using System.Xml.Serialization;
using AdofaiTweaks.Core;

namespace AdofaiTweaks.Tweaks.PlanetOpacity
{
    /// <summary>
    /// Settings for the Planet Opacity tweak.
    /// </summary>
    public class PlanetOpacitySettings : TweakSettings
    {
        /// <summary>
        /// Getter for the actual opacity of planet 1, which depends on whether
        /// the tweak is enabled.
        /// </summary>
        [XmlIgnore]
        public float ActualOpacity1 {
            get => IsEnabled && AdofaiTweaks.IsEnabled ? SettingsOpacity1 : 100f;
        }

        /// <summary>
        /// Getter for the actual opacity of planet 2, which depends on whether
        /// the tweak is enabled.
        /// </summary>
        [XmlIgnore]
        public float ActualOpacity2 {
            get => IsEnabled && AdofaiTweaks.IsEnabled ? SettingsOpacity2 : 100f;
        }

        /// <summary>
        /// The set opacity for planet 1.
        /// </summary>
        public float SettingsOpacity1 { get; set; } = 50f;

        /// <summary>
        /// The set opacity for planet 2.
        /// </summary>
        public float SettingsOpacity2 { get; set; } = 50f;
    }
}
