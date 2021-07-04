using AdofaiTweaks.Core;

namespace AdofaiTweaks.Tweaks.RestrictJudgments
{
    /// <summary>
    /// Settings for the Restrict Judgments tweak.
    /// </summary>
    public class RestrictJudgmentsSettings : TweakSettings
    {
        /// <summary>
        /// Whether to restrict the judgment. If <c>RestrictJudgments[i]</c> is
        /// <c>true</c>, then punish the player on judgment <c>i</c>.
        /// </summary>
        public bool[] RestrictJudgments { get; set; }

        /// <summary>
        /// The string to display when the tweak kills the player. If the string
        /// contains <c>{judgment}</c>, it will be replaced by the judgment name
        /// that killed the player.
        /// </summary>
        public string CustomDeathString { get; set; } = "No {judgment}s allowed!";

        /// <summary>
        /// An action to restrict judgment.
        /// </summary>
        public RestrictJudgmentAction RestrictJudgmentAction { get; set; }
    }
}
