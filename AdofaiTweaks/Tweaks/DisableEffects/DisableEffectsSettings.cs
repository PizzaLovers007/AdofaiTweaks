using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdofaiTweaks.Core;

namespace AdofaiTweaks.Tweaks.DisableEffects
{
    /// <summary>
    /// Settings for the Disable Effects tweak.
    /// </summary>
    public class DisableEffectsSettings : TweakSettings
    {
        /// <summary>
        /// The upper bound for the tile movement max slider.
        /// </summary>
        internal const int MOVE_TRACK_UPPER_BOUND = 100;

        /// <summary>
        /// Disables VFX filters.
        /// </summary>
        public bool DisableFilter { get; set; }

        /// <summary>
        /// Disables bloom.
        /// </summary>
        public bool DisableBloom { get; set; }

        /// <summary>
        /// Disables screen flashes.
        /// </summary>
        public bool DisableFlash { get; set; }

        /// <summary>
        /// Disables the "Hall of Mirrors" effect.
        /// </summary>
        public bool DisableHallOfMirrors { get; set; }

        /// <summary>
        /// Disables screen shake.
        /// </summary>
        public bool DisableScreenShake { get; set; }

        /// <summary>
        /// Limits the max number of tile movements allowed for a "Move Track"
        /// event.
        /// </summary>
        public int MoveTrackMax { get; set; } = 30;
    }
}
