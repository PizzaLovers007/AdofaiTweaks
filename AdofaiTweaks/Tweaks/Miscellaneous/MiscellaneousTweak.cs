using AdofaiTweaks.Core;
using AdofaiTweaks.Core.Attributes;
using AdofaiTweaks.Strings;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.Miscellaneous
{
    [RegisterTweak(
        id: "miscellaneous",
        settingsType: typeof(MiscellaneousSettings),
        patchesType: typeof(MiscellaneousPatches),
        priority: 1000)]
    public class MiscellaneousTweak : Tweak
    {
        public override string Name =>
            TweakStrings.Get(TranslationKeys.Miscellaneous.NAME);

        public override string Description =>
            TweakStrings.Get(TranslationKeys.Miscellaneous.DESCRIPTION);

        [SyncTweakSettings]
        private MiscellaneousSettings Settings { get; set; }

        public override void OnSettingsGUI() {
            // Glitch flip
            Settings.DisableGlitchFlip =
                GUILayout.Toggle(
                    Settings.DisableGlitchFlip,
                    TweakStrings.Get(TranslationKeys.Miscellaneous.GLITCH_FLIP));

            // Editor zoom
            Settings.DisableEditorZoom =
                GUILayout.Toggle(
                    Settings.DisableEditorZoom,
                    TweakStrings.Get(TranslationKeys.Miscellaneous.EDITOR_ZOOM));
        }
    }
}
