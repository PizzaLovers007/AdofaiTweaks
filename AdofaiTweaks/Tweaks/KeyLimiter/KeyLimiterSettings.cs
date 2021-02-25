using System.Collections.Generic;
using System.Xml.Serialization;
using AdofaiTweaks.Core;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.KeyLimiter
{
    public class KeyLimiterSettings : TweakSettings
    {
        public List<KeyCode> ActiveKeys { get; set; } = new List<KeyCode>();

        [XmlIgnore]
        public bool IsListening { get; set; }
    }
}
