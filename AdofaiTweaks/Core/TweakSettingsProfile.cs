using System.IO;
using System.Xml.Serialization;

namespace AdofaiTweaks.Core;

/// <summary>
/// Abstraction of multiple states of a settings.
/// </summary>
public abstract class TweakSettingsProfile
{
    /// <summary>
    /// Creates a copy of <c>this</c>.
    /// </summary>
    /// <typeparam name="T">Type of the profile instance.</typeparam>
    /// <returns>A copy of <c>this</c>.</returns>
    protected T Copy<T>()
        where T : TweakSettingsProfile {
        using var ms = new MemoryStream();
        var serializer = new XmlSerializer(GetType());
        serializer.Serialize(ms, this);
        ms.Position = 0;
        return (T)serializer.Deserialize(ms);
    }
}