using AdofaiTweaks.Core;

namespace AdofaiTweaks.Tweaks.Miscellaneous
{
    /// <summary>
    /// Settings for the Miscellaneous tweak.
    /// </summary>
    public class MiscellaneousSettings : TweakSettings
    {
        /// <summary>
        /// Disables the screen flip in the "Glitch" filter.
        /// </summary>
        public bool DisableGlitchFlip { get; set; }

        /// <summary>
        /// Disables editor zoom in/out when in gameplay.
        /// </summary>
        public bool DisableEditorZoom { get; set; }

        /// <summary>
        /// Overrides the hitsound volume.
        /// </summary>
        public bool SetHitsoundVolume { get; set; }

        /// <summary>
        /// The hitsound volume value.
        /// </summary>
        public float HitsoundVolumeScale { get; set; } = 1f;

        /// <summary>
        /// Updates volume, should be called every map loads.
        /// </summary>
        public void UpdateVolume()
        {
            scrConductor instance = scrConductor.instance;
            if (instance)
            {
                instance.hitSoundVolume *= HitsoundVolumeScale;
            }
        }
    }
}
