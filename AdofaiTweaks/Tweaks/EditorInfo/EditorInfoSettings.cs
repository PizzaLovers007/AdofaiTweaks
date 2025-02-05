using System.Xml.Serialization;
using AdofaiTweaks.Core;

namespace AdofaiTweaks.Tweaks.EditorInfo
{
    /// <summary>
    /// Settings for the Restrict Judgments tweak.
    /// </summary>
    public class EditorInfoSettings : TweakSettings
    {
        /// <summary>
        /// Displays angle for selected floor if this option is on.
        /// </summary>
        public bool ShowFloorAngle { get; set; }

        /// <summary>
        /// Displays beats for selected floor if this option is on.
        /// </summary>
        public bool ShowFloorBeats { get; set; }

        /// <summary>
        /// Whether the patch should display anything.
        /// </summary>
        [XmlIgnore]
        public bool ShouldShowAny => ShowFloorAngle || ShowFloorBeats;
    }
}
