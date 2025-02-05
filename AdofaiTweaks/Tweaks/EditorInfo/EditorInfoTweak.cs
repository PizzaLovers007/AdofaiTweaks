using AdofaiTweaks.Core;
using AdofaiTweaks.Core.Attributes;
using AdofaiTweaks.Strings;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.EditorInfo
{
    /// <summary>
    /// A tweak for various level editor features.
    /// </summary>
    [RegisterTweak(
        id: "editor_info",
        settingsType: typeof(EditorInfoSettings),
        patchesType: typeof(EditorInfoPatches))]
    internal class EditorInfoTweak : Tweak
    {
        /// <inheritdoc/>
        public override string Name =>
            TweakStrings.Get(TranslationKeys.EditorInfo.NAME);

        /// <inheritdoc/>
        public override string Description =>
            TweakStrings.Get(TranslationKeys.EditorInfo.DESCRIPTION);

        [SyncTweakSettings]
        private EditorInfoSettings Settings { get; set; }

        /// <inheritdoc/>
        public override void OnSettingsGUI() {
            MoreGUILayout.BeginIndent();
            Settings.ShowFloorAngle = GUILayout.Toggle(Settings.ShowFloorAngle, TweakStrings.Get(TranslationKeys.EditorInfo.SHOW_FLOOR_ANGLE));
            Settings.ShowFloorBeats = GUILayout.Toggle(Settings.ShowFloorBeats, TweakStrings.Get(TranslationKeys.EditorInfo.SHOW_FLOOR_BEATS));
            MoreGUILayout.EndIndent();
        }
    }
}
