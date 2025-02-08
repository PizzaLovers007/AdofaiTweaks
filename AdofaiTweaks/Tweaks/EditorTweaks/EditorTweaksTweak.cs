using System;
using System.Globalization;
using AdofaiTweaks.Core;
using AdofaiTweaks.Core.Attributes;
using AdofaiTweaks.Strings;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.EditorTweaks;

/// <summary>
/// A tweak for various level editor features.
/// </summary>
[RegisterTweak(
    id: "editor_tweaks",
    settingsType: typeof(EditorTweaksSettings),
    patchesType: typeof(EditorTweaksPatches))]
internal class EditorTweaksTweak : Tweak
{
    /// <inheritdoc/>
    public override string Name =>
        TweakStrings.Get(TranslationKeys.EditorTweaks.NAME);

    /// <inheritdoc/>
    public override string Description =>
        TweakStrings.Get(TranslationKeys.EditorTweaks.DESCRIPTION);

    [SyncTweakSettings]
    private EditorTweaksSettings Settings { get; set; }

    /// <inheritdoc/>
    public override void OnSettingsGUI() {
        MoreGUILayout.BeginIndent();

        Settings.ShowFloorAngle = GUILayout.Toggle(
            Settings.ShowFloorAngle,
            TweakStrings.Get(TranslationKeys.EditorTweaks.SHOW_FLOOR_ANGLE));
        Settings.ShowFloorBeats = GUILayout.Toggle(
            Settings.ShowFloorBeats,
            TweakStrings.Get(TranslationKeys.EditorTweaks.SHOW_FLOOR_BEATS));
        Settings.ShowFloorCount = GUILayout.Toggle(
            Settings.ShowFloorCount,
            TweakStrings.Get(TranslationKeys.EditorTweaks.SHOW_FLOOR_COUNT));
        Settings.ShowFloorDuration = GUILayout.Toggle(
            Settings.ShowFloorDuration,
            TweakStrings.Get(TranslationKeys.EditorTweaks.SHOW_FLOOR_DURATION));

        GUILayout.Space(8f);

        Settings.FineTuneFloorRotations = GUILayout.Toggle(
            Settings.FineTuneFloorRotations,
            TweakStrings.Get(TranslationKeys.EditorTweaks.FINE_TUNED_ROTATIONS));

        GUILayout.BeginHorizontal();

        var rotationStepString = Settings.FloorRotationStep.ToString(CultureInfo.InvariantCulture);
        if (!rotationStepString.Contains(".")) {
            rotationStepString += ".0";
        }

        try {
            Settings.FloorRotationStep =
                Convert.ToSingle(
                    GUILayout.TextField(
                        rotationStepString,
                        GUILayout.MaxWidth(12f * Math.Max(1, rotationStepString.Length))));
        } catch {
            // Do nothing
        }

        GUILayout.Label("\u00b0");
        GUILayout.EndHorizontal();

        GUILayout.Space(8f);

        Settings.SyncLevelEventValuesWithFloorFlipsAndRotations =
            GUILayout.Toggle(
                Settings.SyncLevelEventValuesWithFloorFlipsAndRotations,
                TweakStrings.Get(TranslationKeys.EditorTweaks.SYNC_FLIPS_AND_ROTATIONS_WITH_LEVEL_EVENTS));
        GUILayout.Label(TweakStrings.Get(TranslationKeys.EditorTweaks.SYNC_WARNING));
        MoreGUILayout.BeginIndent();
        foreach (var whitelistedLevelEvent in EditorTweaksPatches.WhitelistedLevelEvents) {
            GUILayout.Label($" · {RDString.Get($"editor.{whitelistedLevelEvent}")} ({whitelistedLevelEvent})");
        }
        MoreGUILayout.EndIndent();

        MoreGUILayout.EndIndent();
    }
}