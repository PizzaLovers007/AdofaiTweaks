using System;
using System.IO;
using System.Xml.Serialization;
using UnityModManagerNet;

namespace AdofaiTweaks.Core;

/// <summary>
/// The base settings object used to store data related to the tweak.
/// </summary>
public class TweakSettings : UnityModManager.ModSettings
{
    /// <summary>
    /// Whether the tweak is enabled.
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// Whether the tweak's settings are expanded in the UMM in-game menu.
    /// </summary>
    public bool IsExpanded { get; set; }

    /// <summary>
    /// Gets the path to the file that holds this object's data.
    /// </summary>
    /// <param name="modEntry">The UMM mod entry for AdofaiTweaks.</param>
    /// <returns>
    /// The path to the file that holds this object's data.
    /// </returns>
    public override string GetPath(UnityModManager.ModEntry modEntry) {
        return Path.Combine(modEntry.Path, GetType().Name + ".xml");
    }

    /// <summary>
    /// Saves the settings data to the file at the path returned from
    /// <see cref="GetPath(UnityModManager.ModEntry)"/>.
    /// </summary>
    /// <param name="modEntry">The UMM mod entry for AdofaiTweaks.</param>
    public override void Save(UnityModManager.ModEntry modEntry) {
        var filepath = GetPath(modEntry);
        try {
            using var writer = new StreamWriter(filepath);
            var serializer = new XmlSerializer(GetType());
            serializer.Serialize(writer, this);
        } catch (Exception e) {
            modEntry.Logger.Error($"Can't save {filepath}.");
            modEntry.Logger.LogException(e);
        }
    }
}