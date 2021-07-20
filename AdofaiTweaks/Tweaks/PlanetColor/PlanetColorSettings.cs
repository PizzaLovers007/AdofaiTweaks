using AdofaiTweaks.Core;

namespace AdofaiTweaks.Tweaks.PlanetColor
{
    /// <summary>
    /// Settings for the Planet Color tweak.
    /// </summary>
    public class PlanetColorSettings : TweakSettings
    {
        /// <summary>
        /// Red planet's color data.
        /// </summary>
        public PlanetColorData Red { get; set; }

        /// <summary>
        /// Blue planet's color data.
        /// </summary>
        public PlanetColorData Blue { get; set; }
    }
}
