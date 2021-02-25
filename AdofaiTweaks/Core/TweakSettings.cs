using System;
using System.IO;
using System.Xml.Serialization;
using UnityModManagerNet;

namespace AdofaiTweaks.Core
{
    public class TweakSettings : UnityModManager.ModSettings
    {
        public bool IsEnabled { get; set; }
        public bool IsExpanded { get; set; }

        public override string GetPath(UnityModManager.ModEntry modEntry) {
            return Path.Combine(modEntry.Path, GetType().Name + ".xml");
        }

        public override void Save(UnityModManager.ModEntry modEntry) {
            var filepath = GetPath(modEntry);
            try {
                using (var writer = new StreamWriter(filepath)) {
                    var serializer = new XmlSerializer(GetType());
                    serializer.Serialize(writer, this);
                }
            } catch (Exception e) {
                modEntry.Logger.Error($"Can't save {filepath}.");
                modEntry.Logger.LogException(e);
            }
        }
    }
}
