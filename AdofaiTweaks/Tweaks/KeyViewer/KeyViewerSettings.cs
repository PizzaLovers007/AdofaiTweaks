using System.Collections.Generic;
using System.Xml.Serialization;
using AdofaiTweaks.Core;

namespace AdofaiTweaks.Tweaks.KeyViewer
{
    /// <summary>
    /// Settings for the Key Viewer tweak.
    /// </summary>
    public class KeyViewerSettings : TweakSettings
    {
        /// <summary>
        /// A list of profiles that the user has saved.
        /// </summary>
        public List<KeyViewerProfile> Profiles { get; set; }

        /// <summary>
        /// The index of the current profile being used.
        /// </summary>
        public int ProfileIndex { get; set; }

        /// <summary>
        /// A reference to the current profile being used.
        /// </summary>
        [XmlIgnore]
        public KeyViewerProfile CurrentProfile { get => Profiles[ProfileIndex]; }

        /// <summary>
        /// Whether the tweak is listening for keystrokes to change what keys
        /// are active.
        /// </summary>
        [XmlIgnore]
        public bool IsListening { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyViewerSettings"/>
        /// class with some default values.
        /// </summary>
        public KeyViewerSettings() {
            Profiles = new List<KeyViewerProfile>();
            ProfileIndex = 0;
        }
    }
}
