using System.Xml.Serialization;
using AdofaiTweaks.Core;

namespace AdofaiTweaks.Tweaks.JudgmentVisuals
{
    public class JudgmentVisualsSettings : TweakSettings
    {
        public bool ShowHitErrorMeter { get; set; }

        [XmlIgnore]
        internal HitErrorMeter ErrorMeter { get; set; }
    }
}
