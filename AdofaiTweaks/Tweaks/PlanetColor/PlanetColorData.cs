namespace AdofaiTweaks.Tweaks.PlanetColor
{
    /// <summary>
    /// Class for storing planet data for one planet.
    /// </summary>
    public class PlanetColorData
    {
        /// <summary>
        /// The color value for planet's body.
        /// </summary>
        public ObjectColor Body { get; set; }

        /// <summary>
        /// The color value for planet's tail.
        /// </summary>
        public ObjectColor Tail { get; set; }
    }
}
