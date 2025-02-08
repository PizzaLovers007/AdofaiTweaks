using AdofaiTweaks.Core;
using AdofaiTweaks.Translation;

namespace AdofaiTweaks;

/// <summary>
/// The global settings for AdofaiTweaks.
/// </summary>
public class GlobalSettings : TweakSettings
{
    /// <summary>
    /// The current language for the user.
    /// </summary>
    public LanguageEnum Language { get; set; } = LanguageEnum.ENGLISH;
}