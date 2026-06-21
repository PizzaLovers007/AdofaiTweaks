using AdofaiTweaks.Core;
using AdofaiTweaks.Core.Attributes;
using AdofaiTweaks.Strings;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.JudgmentVisuals;

/// <summary>
/// A tweak for adjusting the visuals for judgments.
/// </summary>
[RegisterTweak(
    id: "judgment_visuals",
    settingsType: typeof(JudgmentVisualsSettings),
    patchesType: typeof(JudgmentVisualsPatches))]
public class JudgmentVisualsTweak : Tweak {
    /// <inheritdoc/>
    public override string Name =>
        TweakStrings.Get(TranslationKeys.JudgmentVisuals.NAME);

    /// <inheritdoc/>
    public override string Description =>
        TweakStrings.Get(TranslationKeys.JudgmentVisuals.DESCRIPTION);

    [SyncTweakSettings]
    private JudgmentVisualsSettings Settings { get; set; }

    /// <inheritdoc/>
    public override void OnSettingsGUI() {
        // Hide perfects
        Settings.HidePerfects =
            GUILayout.Toggle(
                Settings.HidePerfects,
                TweakStrings.Get(
                    TranslationKeys.JudgmentVisuals.HIDE_PERFECTS,
                    TweakStrings.GetRDString("HitMargin." + HitMargin.Perfect)));
    }
}