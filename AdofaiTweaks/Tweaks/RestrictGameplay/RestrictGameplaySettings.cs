using AdofaiTweaks.Core;

namespace AdofaiTweaks.Tweaks.RestrictGameplay;

/// <summary>
/// Settings for the Restrict Gameplay tweak.
/// </summary>
public class RestrictGameplaySettings : TweakSettings
{
    /// <summary>
    /// Whether to restrict gameplay by judgment.
    /// </summary>
    public bool RestrictJudgment { get; set; }

    /// <summary>
    /// Whether to restrict the judgment. If <c>RestrictGameplay[i]</c> is
    /// <c>true</c>, then punish the player on judgment <c>i</c>.
    /// </summary>
    public bool[] RestrictedJudgments { get; set; }

    /// <summary>
    /// The string to display when the tweak kills the player. If the string
    /// contains <c>{judgment}</c>, it will be replaced by the judgment name
    /// that killed the player.
    /// </summary>
    public string CustomDeathStringForJudgment { get; set; } = "No {judgment}s allowed!";

    /// <summary>
    /// The string to display when the tweak kills the player. If the string
    /// contains <c>{averageAngle}</c>, it will be replaced by player's
    /// <see cref="AllowedAverageAngleThreshold"/> value.
    /// </summary>
    public string CustomDeathStringForAverageAngle { get; set; } = "Average angle left the ±{averageAngle}° range!";

    /// <summary>
    /// An action to restrict gameplay on judgment restriction.
    /// </summary>
    public RestrictGameplayAction RestrictGameplayActionForJudgment { get; set; }

    /// <summary>
    /// An action to restrict gameplay on average angle restriction.
    /// </summary>
    public RestrictGameplayAction RestrictGameplayActionForAverageAngle { get; set; }

    /// <summary>
    /// Whether to restrict the gameplay by average angle of a hit error meter.
    /// </summary>
    public bool RestrictAverageAngle { get; set; }

    /// <summary>
    /// Allowed range of the average angle.
    /// </summary>
    public float AllowedAverageAngleThreshold { get; set; } = 60;
}