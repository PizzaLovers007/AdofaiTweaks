using AdofaiTweaks.Core;
using AdofaiTweaks.Core.Attributes;
using AdofaiTweaks.Strings;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.Miscellaneous
{
    /// <summary>
    /// A tweak for holding miscellaneous toggles.
    /// </summary>
    [RegisterTweak(
        id: "miscellaneous",
        settingsType: typeof(MiscellaneousSettings),
        patchesType: typeof(MiscellaneousPatches),
        priority: 1000)]
    public class MiscellaneousTweak : Tweak
    {
        /// <inheritdoc/>
        public override string Name =>
            TweakStrings.Get(TranslationKeys.Miscellaneous.NAME);

        /// <inheritdoc/>
        public override string Description =>
            TweakStrings.Get(TranslationKeys.Miscellaneous.DESCRIPTION);

        [SyncTweakSettings]
        private MiscellaneousSettings Settings { get; set; }

        private string bpmString = "";

        /// <inheritdoc/>
        public override void OnSettingsGUI() {
            // Glitch flip
            Settings.DisableGlitchFlip =
                GUILayout.Toggle(
                    Settings.DisableGlitchFlip,
                    TweakStrings.Get(
                        TranslationKeys.Miscellaneous.GLITCH_FLIP,
                        RDString.GetEnumValue(Filter.Glitch)));

            // Editor zoom
            Settings.DisableEditorZoom =
                GUILayout.Toggle(
                    Settings.DisableEditorZoom,
                    TweakStrings.Get(TranslationKeys.Miscellaneous.EDITOR_ZOOM));

            // Hitsound volume
            if (Settings.SetHitsoundVolume =
                GUILayout.Toggle(
                    Settings.SetHitsoundVolume,
                    TweakStrings.Get(TranslationKeys.Miscellaneous.SET_HITSOUND_VOLUME)))
            {
                bool valueChanged = Settings.HitsoundVolumeScale == (
                    Settings.HitsoundVolumeScale =
                        Mathf.Min(
                            MoreGUILayout.NamedSlider(
                                TweakStrings.Get(TranslationKeys.Miscellaneous.CURRENT_HITSOUND_VOLUME),
                                Settings.HitsoundVolumeScale * 100,
                                0,
                                100f,
                                200f,
                                valueFormat: "{0:0.#}%") / 100, 1));
                if (valueChanged)
                {
                    Settings.UpdateVolume();
                }
            }

            if (Settings.SetBpmInFirstTile =
                GUILayout.Toggle(
                    Settings.SetBpmInFirstTile,
                    TweakStrings.Get(TranslationKeys.Global.TEST_KEY)))
            {
                bpmString = MoreGUILayout.NamedTextField(TranslationKeys.Global.TEST_KEY, bpmString, 200f);

                if (float.TryParse(bpmString, out float bpm))
                {
                    Settings.Bpm = bpm;
                }
            }
        }
    }
}
