using System;
using System.IO;
using System.Xml.Serialization;
using MelonLoader;

namespace AdofaiTweaks.Core
{
    /// <summary>
    /// The base settings object used to store data related to the tweak.
    /// </summary>
    public class TweakSettings
    {
        /// <summary>
        /// Whether the tweak is enabled.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Whether the tweak's settings are expanded in the in-game menu.
        /// </summary>
        public bool IsExpanded { get; set; }

        /// <summary>
        /// Gets the path to the file that holds this object's data.
        /// </summary>
        /// <returns>
        /// The path to the file that holds this object's data.
        /// </returns>
        public string GetPath() {
            return Path.Combine(
                MelonHandler.ModsDirectory,
                "AdofaiTweaks",
                GetType().Name + ".xml");
        }

        /// <summary>
        /// Saves the settings data to the file at the path returned from
        /// <see cref="GetPath()"/>.
        /// </summary>
        public void Save() {
            var filepath = GetPath();
            try {
                using (var writer = new StreamWriter(filepath)) {
                    var serializer = new XmlSerializer(GetType());
                    serializer.Serialize(writer, this);
                }
            } catch (Exception e) {
                MelonLogger.Error($"Can't save {filepath}.");
                MelonLogger.Error(e.ToString());
            }
        }

        /// <summary>
        /// Loads the settings data from the file at the path returned from
        /// <see cref="GetPath()"/>.
        /// </summary>
        /// <typeparam name="T">The settings type to cast to.</typeparam>
        /// <returns>
        /// An instance of <c>T</c> based on the settings data.
        /// </returns>
        public static T Load<T>() where T : TweakSettings, new() {
            var t = new T();
            var filepath = t.GetPath();
            if (File.Exists(filepath)) {
                try {
                    using (var stream = File.OpenRead(filepath)) {
                        var serializer = new XmlSerializer(typeof(T));
                        var result = (T)serializer.Deserialize(stream);

                        return result;
                    }
                } catch (Exception e) {
                    MelonLogger.Error($"Can't read {filepath}.");
                    MelonLogger.Error(e.ToString());
                }
            }

            return t;
        }
    }
}
