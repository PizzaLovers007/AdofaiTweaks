using System;
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
            Settings.DisableFilter =
                GUILayout.Toggle(
                    Settings.DisableFilter,
                    TweakStrings.Get(TranslationKeys.DisableEffects.FILTER));

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
                    TweakStrings.Get(TranslationKeys.DisableEffects.HALL_OF_MIRRORS));

            // Screen shake
            Settings.DisableScreenShake =
                GUILayout.Toggle(
                    Settings.DisableScreenShake,
                    TweakStrings.Get(TranslationKeys.DisableEffects.SCREEN_SHAKE));
        }
    }
}
