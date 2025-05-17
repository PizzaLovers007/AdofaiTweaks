using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using ADOFAI;
using AdofaiTweaks.Core;
using AdofaiTweaks.Core.Attributes;
using AdofaiTweaks.Strings;
using HarmonyLib;
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
        Settings.UseTulttakModBehavior = GUILayout.Toggle(
            Settings.UseTulttakModBehavior,
            TweakStrings.Get(TranslationKeys.EditorTweaks.USE_TULTTAK_MOD_BEHAVIOR));

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

        #if DEBUG
        var editor = scnEditor.instance;
        if (editor && editor.selectedFloors.Count > 0) {
            if (GUILayout.Button($"Create {editor.selectedFloors.Count} track decoration(s) from track selection")) {
                var events = new LevelEvent[editor.selectedFloors.Count];
                for (var i = 0; i < events.Length; i++) {
                    var floor = editor.selectedFloors[i];
                    const double anglePrecision = 1.0E-6;

                    var e = new LevelEvent(-1, LevelEventType.AddObject);
                    e["position"] = floor.transform.position.xy() / 1.5f;
                    e["trackType"] = floor.midSpin ? FloorDecorationType.Midspin : FloorDecorationType.Normal;
                    e["trackAngle"] = Mathf.Rad2Deg * Math.Abs(floor.exitangle - floor.entryangle);
                    e["rotation"] = (float)((floor.exitangle - Math.PI * 1.5) * Mathf.Rad2Deg * (floor.exitangle < floor.entryangle ? -1 : 1));
                    e["scale"] = floor.transform.localScale.xy() * 100;
                    // e["trackColorType"] = floor.;
                    // e["trackColor"] = floor.;
                    // e["secondaryTrackColor"] = floor.;
                    // e["trackColorAnimDuration"] = floor.;
                    // e["trackOpacity"] = floor.;
                    // e["trackStyle"] = floor.;
                    // e["trackIcon"] = floor.;
                    // e["trackIconAngle"] = floor.;
                    // e["trackIconFlipped"] = floor.;
                    // e["trackRedSwirl"] = floor.;
                    // e["trackGraySetSpeedIcon"] = floor.;
                    // e["trackSetSpeedIconBpm"] = floor.;
                    // e["trackGlowEnabled"] = floor.;
                    // e["trackGlowColor"] = floor.;
                    // e["trackIconOutlines"] = floor.;

                    events[i] = e;
                }

                editor.levelData.decorations.AddRange(events);
                foreach (var levelEvent in events) {
                    scrDecorationManager.instance.CreateDecoration(levelEvent, out _);
                }
            }
        }
        #endif

        MoreGUILayout.EndIndent();
    }
}