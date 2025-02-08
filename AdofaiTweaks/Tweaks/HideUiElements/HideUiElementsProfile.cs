using AdofaiTweaks.Core;

namespace AdofaiTweaks.Tweaks.HideUiElements;

/// <summary>
/// UI hiding options.
/// </summary>
public class HideUiElementsProfile : TweakSettingsProfile
{
    /// <summary>
    /// Hides all UI elements.
    /// </summary>
    public bool HideEverything { get; set; }

    /// <summary>
    /// Hides judgments (Perfect, EPerfect, etc.).
    /// </summary>
    public bool HideJudgment { get; set; }

    /// <summary>
    /// Hides miss indicators (the circled X icons).
    /// </summary>
    public bool HideMissIndicators { get; set; }

    /// <summary>
    /// Hides the song title and artist.
    /// </summary>
    public bool HideTitle { get; set; }

    /// <summary>
    /// Hides Otto.
    /// </summary>
    public bool HideOtto { get; set; }

    /// <summary>
    /// Hides the timing target icon in the bottom right corner.
    /// </summary>
    public bool HideTimingTarget { get; set; }

    /// <summary>
    /// Hides the no fail indicator in the bottom right corner.
    /// </summary>
    public bool HideNoFailIcon { get; set; }

    /// <summary>
    /// Hides the "Beta Build" text.
    /// </summary>
    public bool HideBeta { get; set; }

    /// <summary>
    /// Hides "Congratulations!"/"Pure Perfect!" text and detailed results.
    /// </summary>
    public bool HideResult { get; set; }

    /// <summary>
    /// Hides the hit error meter.
    /// </summary>
    public bool HideHitErrorMeter { get; set; }

    /// <summary>
    /// Hides the flash on landing last floor.
    /// </summary>
    public bool HideLastFloorFlash { get; set; }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="HideUiElementsProfile"/> class.
    /// </summary>
    public HideUiElementsProfile() { }
}