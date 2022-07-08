using System;

namespace AdofaiTweaks;

/// <summary>
/// AdofaiTweaks' mod events for other mods to use.
/// Check the example use case at (link here later).
/// </summary>
public static class ModEvents
{
    /// <summary>
    /// Event that is fired when recording mode from <see cref="Tweaks.HideUiElements.HideUiElementsTweak"/> is enabled.
    /// </summary>
    public static EventHandler OnRecordingModeEnabled;

    /// <summary>
    /// Event that is fired when recording mode from <see cref="Tweaks.HideUiElements.HideUiElementsTweak"/> is disabled.
    /// </summary>
    public static EventHandler OnRecordingModeDisabled;
}