using System.Xml.Serialization;
using AdofaiTweaks.Core;

namespace AdofaiTweaks.Tweaks.PlanetOpacity
{
    public class PlanetOpacitySettings : TweakSettings
    {
        [XmlIgnore]
        public float ActualOpacity1 {
            get => IsEnabled && AdofaiTweaks.IsEnabled ? SettingsOpacity1 : 100f;
        }

        [XmlIgnore]
        public float ActualOpacity2 {
            get => IsEnabled && AdofaiTweaks.IsEnabled ? SettingsOpacity2 : 100f;
        }

        public float SettingsOpacity1 { get; set; } = 50f;
        public float SettingsOpacity2 { get; set; } = 50f;
    }
}
