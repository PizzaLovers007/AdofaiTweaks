using System.Collections.Generic;
using System.Xml.Serialization;
using AdofaiTweaks.Core;

namespace AdofaiTweaks.Tweaks.KeyViewer
{
    public class KeyViewerSettings : TweakSettings
    {
        public List<KeyViewerProfile> Profiles { get; set; }
        public int ProfileIndex { get; set; }

        [XmlIgnore]
        public KeyViewerProfile CurrentProfile { get => Profiles[ProfileIndex]; }

        [XmlIgnore]
        public bool IsListening { get; set; }

        public KeyViewerSettings() {
            Profiles = new List<KeyViewerProfile>();
            ProfileIndex = 0;
        }
    }
}
