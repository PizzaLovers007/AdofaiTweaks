using ADOFAI;
using AdofaiTweaks.Core;
using AdofaiTweaks.Core.Attributes;
using AdofaiTweaks.Strings;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.DisableEffects
{
    /// <summary>
    /// A tweak for disabling certain effects to improve performance.
    /// </summary>
    [RegisterTweak(
        id: "disable_effects",
        settingsType: typeof(DisableEffectsSettings),
        patchesType: typeof(DisableEffectsPatches))]
    public class DisableEffectsTweak : Tweak
    {
        /// <inheritdoc/>
        public override string Name => TweakStrings.Get(TranslationKeys.DisableEffects.NAME);

        /// <inheritdoc/>
        public override string Description =>
            TweakStrings.Get(TranslationKeys.DisableEffects.DESCRIPTION);

        [SyncTweakSettings]
        private DisableEffectsSettings Settings { get; set; }

        /// <inheritdoc/>
        public override void OnSettingsGUI() {
            // Filter
            if (Settings.DisableFilter =
                GUILayout.Toggle(
                    Settings.DisableFilter,
                    TweakStrings.Get(
                        TranslationKeys.DisableEffects.FILTER,
                        RDString.GetEnumValue(Filter.Grayscale),
                        RDString.GetEnumValue(Filter.Arcade))))
            {
                /*
                // Enable specific filter
                if (GUILayout.Button(TweakStrings.Get("")))
                {
                    //
                }

                // Disable specific filter
                if (GUILayout.Button(TweakStrings.Get("")))
                {
                    //
                }
                */
            }

            // Bloom
            Settings.DisableBloom =
                GUILayout.Toggle(
                    Settings.DisableBloom,
                    TweakStrings.Get(TranslationKeys.DisableEffects.BLOOM));

            // Flash
            Settings.DisableFlash =
                GUILayout.Toggle(
                    Settings.DisableFlash,
                    TweakStrings.Get(TranslationKeys.DisableEffects.FLASH));

            // Hall of mirrors
            Settings.DisableHallOfMirrors =
                GUILayout.Toggle(
                    Settings.DisableHallOfMirrors,
                    TweakStrings.Get(
                        TranslationKeys.DisableEffects.HALL_OF_MIRRORS,
                        RDString.Get("editor." + LevelEventType.HallOfMirrors)));

            // Screen shake
            Settings.DisableScreenShake =
                GUILayout.Toggle(
                    Settings.DisableScreenShake,
                    TweakStrings.Get(TranslationKeys.DisableEffects.SCREEN_SHAKE));

            // Move track
            string trackMaxFormat =
                Settings.MoveTrackMax > DisableEffectsSettings.MOVE_TRACK_UPPER_BOUND
                    ? TweakStrings.Get(TranslationKeys.DisableEffects.TILE_MOVEMENT_UNLIMITED)
                    : "{0}";
            float newTrackMax =
                MoreGUILayout.NamedSlider(
                    TweakStrings.Get(TranslationKeys.DisableEffects.TILE_MOVEMENT_MAX),
                    Settings.MoveTrackMax,
                    5,
                    DisableEffectsSettings.MOVE_TRACK_UPPER_BOUND + 5,
                    300f,
                    roundNearest: 4,
                    valueFormat: trackMaxFormat);
            Settings.MoveTrackMax = Mathf.RoundToInt(newTrackMax);
        }
    }
}
