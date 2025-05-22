using System;
using System.Globalization;
using AdofaiTweaks.Core;
using AdofaiTweaks.Core.Attributes;
using AdofaiTweaks.Strings;
using JetBrains.Annotations;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.RestrictGameplay;

/// <summary>
/// A tweak for punishing the player on certain criteria.
/// </summary>
[UsedImplicitly]
[RegisterTweak(
    id: "restrict_gameplay",
    settingsType: typeof(RestrictGameplaySettings),
    patchesType: typeof(RestrictGameplayPatches))]
internal class RestrictGameplayTweak : Tweak {
    private static readonly HitMargin[] JUDGMENTS_TO_RESTRICT = [
        HitMargin.TooEarly,
        HitMargin.VeryEarly,
        HitMargin.EarlyPerfect,
        HitMargin.Perfect,
        HitMargin.LatePerfect,
        HitMargin.VeryLate,
        HitMargin.TooLate
    ];

    private static readonly RestrictGameplayAction[] RESTRICT_ACTIONS = [
        RestrictGameplayAction.KillPlayer,
        RestrictGameplayAction.InstantRestart,
        RestrictGameplayAction.NoRegister
    ];

    /// <inheritdoc/>
    public override string Name =>
        TweakStrings.Get(TranslationKeys.RestrictGameplay.NAME);

    /// <inheritdoc/>
    public override string Description =>
        TweakStrings.Get(TranslationKeys.RestrictGameplay.DESCRIPTION);

    [SyncTweakSettings]
    private RestrictGameplaySettings Settings { get; set; }

    /// <inheritdoc/>
    public override void OnSettingsGUI() {
        if (Settings.RestrictJudgment = GUILayout.Toggle(
            Settings.RestrictJudgment,
            TweakStrings.Get(TranslationKeys.RestrictGameplay.RESTRICT_JUDGMENT))) {
            MoreGUILayout.BeginIndent();

            // Select judgment to restrict
            GUILayout.Label(TweakStrings.Get(TranslationKeys.RestrictGameplay.SELECT_JUDGMENT));
            MoreGUILayout.BeginIndent();
            foreach (var hitMargin in JUDGMENTS_TO_RESTRICT) {
                Settings.RestrictedJudgments[(int)hitMargin] = GUILayout.Toggle(
                    Settings.RestrictedJudgments[(int)hitMargin],
                    TweakStrings.Get(
                        TranslationKeys.RestrictGameplay.RESTRICT_JUDGMENT_ITEM,
                        [TweakStrings.GetRDString($"HitMargin.{hitMargin}")]));
            }
            MoreGUILayout.EndIndent();

            // Select penalty for this restriction
            GUILayout.Label(TweakStrings.Get(TranslationKeys.RestrictGameplay.SELECT_PENALTY));
            foreach (var action in RESTRICT_ACTIONS) {
                if (GUILayout.Toggle(
                        Settings.RestrictGameplayActionForJudgment == action,
                        TweakStrings.Get($"RESTRICT_GAMEPLAY_I_RESTRICT_ACTION.{action}"))
                    && Settings.RestrictGameplayActionForJudgment != action) {
                    Settings.RestrictGameplayActionForJudgment = action;
                }
            }

            // Custom death message for this restriction
            GUILayout.Label(TweakStrings.Get(TranslationKeys.RestrictGameplay.CUSTOM_DEATH_JUDGMENT));
            Settings.CustomDeathStringForJudgment = GUILayout.TextField(Settings.CustomDeathStringForJudgment);

            MoreGUILayout.EndIndent();
        }

        if (Settings.RestrictAverageAngle = GUILayout.Toggle(
            Settings.RestrictAverageAngle,
            TweakStrings.Get(TranslationKeys.RestrictGameplay.RESTRICT_AVERAGE_ANGLE))) {
            MoreGUILayout.BeginIndent();

            // Set threshold for average angle
            GUILayout.BeginHorizontal();
            GUILayout.Label(TweakStrings.Get(TranslationKeys.RestrictGameplay.RESTRICT_AVERAGE_ANGLE_THRESHOLD));

            GUILayout.Label(" ±");
            var textFieldValue =
                Convert.ToSingle(
                    GUILayout.TextField(Settings.AllowedAverageAngleThreshold.ToString(CultureInfo.InvariantCulture)));
            GUILayout.Label("°");

            if (textFieldValue > 0) {
                Settings.AllowedAverageAngleThreshold = textFieldValue;
            }
            GUILayout.EndHorizontal();

            // Select penalty for this restriction
            GUILayout.Label(TweakStrings.Get(TranslationKeys.RestrictGameplay.SELECT_PENALTY));
            foreach (var action in RESTRICT_ACTIONS) {
                if (GUILayout.Toggle(
                        Settings.RestrictGameplayActionForAverageAngle == action,
                        TweakStrings.Get($"RESTRICT_GAMEPLAY_I_RESTRICT_ACTION.{action}"))
                    && Settings.RestrictGameplayActionForAverageAngle != action) {
                    Settings.RestrictGameplayActionForAverageAngle = action;
                }
            }

            // Custom death message for this restriction
            GUILayout.Label(TweakStrings.Get(TranslationKeys.RestrictGameplay.CUSTOM_DEATH_AVERAGE_ANGLE));
            Settings.CustomDeathStringForAverageAngle = GUILayout.TextField(Settings.CustomDeathStringForAverageAngle);

            MoreGUILayout.EndIndent();
        }
    }

    /// <inheritdoc/>
    public override void OnEnable() {
        int numJudgmentTypes = Enum.GetValues(typeof(HitMargin)).Length;
        if (Settings.RestrictedJudgments == null) {
            Settings.RestrictedJudgments = new bool[numJudgmentTypes];
        } else if (Settings.RestrictedJudgments.Length != numJudgmentTypes) {
            // Judgments were added/removed, migrate to new settings
            bool[] migratedJudgments = new bool[numJudgmentTypes];
            int len = Math.Min(numJudgmentTypes, Settings.RestrictedJudgments.Length);
            for (int i = 0; i < len; i++) {
                migratedJudgments[i] = Settings.RestrictedJudgments[i];
            }
            Settings.RestrictedJudgments = migratedJudgments;
        }
    }
}