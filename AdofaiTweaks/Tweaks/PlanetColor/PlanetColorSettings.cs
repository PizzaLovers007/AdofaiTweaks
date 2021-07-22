using AdofaiTweaks.Core;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.PlanetColor
{
    /// <summary>
    /// Settings for the Planet Color tweak.
    /// </summary>
    public class PlanetColorSettings : TweakSettings
    {
        /// <summary>
        /// Planet 1's body color data.
        /// </summary>
        public ObjectColor RedBody { get; set; } = new ObjectColor();

        /// <summary>
        /// Planet 1's tail color data.
        /// </summary>
        public ObjectColor RedTail { get; set; } = new ObjectColor();

        /// <summary>
        /// Planet 2's body color data.
        /// </summary>
        public ObjectColor BlueBody { get; set; } = new ObjectColor();

        /// <summary>
        /// Planet 2's tail color data.
        /// </summary>
        public ObjectColor BlueTail { get; set; } = new ObjectColor();

        //! Old code required to migrate data

        /// <summary>
        /// The color of planet 1's body.
        /// </summary>
        public Color Color1 { get; set; } = Color.white;

        /// <summary>
        /// The color of planet 2's body.
        /// </summary>
        public Color Color2 { get; set; } = Color.white;

        /// <summary>
        /// The color of planet 1's tail.
        /// </summary>
        public Color TailColor1 { get; set; } = Color.white;

        /// <summary>
        /// The color of planet 2's tail.
        /// </summary>
        public Color TailColor2 { get; set; } = Color.white;
    }
}
