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
        /// Whether to force hitsound's volume to be static.
        /// </summary>
        public bool HitsoundForceVolume { get; set; }

        /// <summary>
        /// Whether to ignore the hitsound volume in the level's settings and
        /// overwrite hitsound volume.
        /// </summary>
        public bool HitsoundIgnoreStartingValue { get; set; }

        /// <summary>
        /// Set the bpm when starting the level.
        /// </summary>
        public bool SetBpmOnStart { get; set; }

        /// <summary>
        /// Starting tile's bpm.
        /// </summary>
        public float Bpm { get; set; } = 100;

        /// <summary>
        /// Whether to force sync the <see cref="AsyncInputManager"/> with
        /// <c>ChosenAsynchronousInput</c> option in the game.
        /// </summary>
        public bool SyncInputStateToInputOptions { get; set; } = true;

        /// <summary>
        /// Updates volume, should be called every map loads.
        /// </summary>
        public void UpdateVolume() {
            if (!HitsoundIgnoreStartingValue) {
                return;
            }

            scrConductor instance = scrConductor.instance;
            if (instance) {
                if (HitsoundForceVolume) {
                    instance.hitSoundVolume = HitsoundVolumeScale;
                } else {
                    instance.hitSoundVolume *= HitsoundVolumeScale;
                }
            }
        }

        /// <summary>
        /// Updates volume in event.
        /// </summary>
        /// <param name="ffxSetHitsound">
        /// hitsound variable to change volume.
        /// </param>
        public void UpdateVolume(ffxSetHitsound ffxSetHitsound) {
            if (HitsoundForceVolume) {
                ffxSetHitsound.volume = HitsoundVolumeScale;
            } else {
                ffxSetHitsound.volume *= HitsoundVolumeScale;
            }
        }
    }
}
