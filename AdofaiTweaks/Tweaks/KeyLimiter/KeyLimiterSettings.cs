using System.Collections.Generic;
using System.Xml.Serialization;
using AdofaiTweaks.Core;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.KeyLimiter
{
    public class KeyLimiterSettings : TweakSettings
    {
        public List<KeyCode> ActiveKeys { get; set; } = new List<KeyCode>();
        public bool ShowKeyViewer { get; set; }
        public float KeyViewerSize { get; set; } = 100f;
        public float KeyViewerXPos { get; set; } = 0.89f;
        public float KeyViewerYPos { get; set; } = 0.03f;

        [XmlIgnore]
        public bool IsListening { get; set; }
    }
}
