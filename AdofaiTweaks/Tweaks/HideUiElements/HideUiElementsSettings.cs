using AdofaiTweaks.Core;

namespace AdofaiTweaks.Tweaks.HideUiElements
{
    /// <summary>
    /// Settings for the Hide UI Elements tweak.
    /// </summary>
    public class HideUiElementsSettings : TweakSettings
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
        /// Hides Otto and the timing target icon in the bottom right.
        /// </summary>
        public bool HideOtto { get; set; }

        /// <summary>
        /// Hides the "Beta Build" text.
        /// </summary>
        public bool HideBeta { get; set; }
    }
}
