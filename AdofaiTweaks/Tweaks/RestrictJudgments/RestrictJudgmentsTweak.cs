using System;
using AdofaiTweaks.Core;
using AdofaiTweaks.Core.Attributes;
using AdofaiTweaks.Strings;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.RestrictJudgments
{
    /// <summary>
    /// A tweak for punishing the player on certain judgments.
    /// </summary>
    [RegisterTweak(
        id: "restrict_judgments",
        settingsType: typeof(RestrictJudgmentsSettings),
        patchesType: typeof(RestrictJudgmentsPatches))]
    internal class RestrictJudgmentsTweak : Tweak
    {
        /// <inheritdoc/>
        public override string Name =>
            TweakStrings.Get(TranslationKeys.RestrictJudgments.NAME);

        /// <inheritdoc/>
        public override string Description =>
            TweakStrings.Get(TranslationKeys.RestrictJudgments.DESCRIPTION);

        [SyncTweakSettings]
        private RestrictJudgmentsSettings Settings { get; set; }

        private readonly int TOTAL_JUDGMENTS = Enum.GetNames(typeof(HitMargin)).Length;
        private readonly int TOTAL_ACTIONS = Enum.GetNames(typeof(RestrictJudgmentAction)).Length;

        /// <inheritdoc/>
        public override void OnSettingsGUI() {
            // select judgment
            GUILayout.Label(TweakStrings.Get(TranslationKeys.RestrictJudgments.RESTRICT_HEADER));
            MoreGUILayout.BeginIndent();
            for (int i = 0; i < TOTAL_JUDGMENTS; i++) {
                Settings.RestrictJudgments[i] = GUILayout.Toggle(
                    Settings.RestrictJudgments[i],
                    TweakStrings.Get(
                        TranslationKeys.RestrictJudgments.RESTRICT,
                        RDString.Get("HitMargin." + (HitMargin)i)));
            }
            MoreGUILayout.EndIndent();

            // select restriction method
            GUILayout.Label(TweakStrings.Get(TranslationKeys.RestrictJudgments.CUSTOM_HEADER));
            MoreGUILayout.BeginIndent();
            GUILayout.Label(TweakStrings.Get(TranslationKeys.RestrictJudgments.RESTRICT_ACTION));
            for (int i = 0; i < TOTAL_ACTIONS; i++) {
                if (GUILayout.Toggle(
                    Settings.RestrictJudgmentAction == (RestrictJudgmentAction)i,
                    ((RestrictJudgmentAction)i).ToString()) &&
                        Settings.RestrictJudgmentAction != (RestrictJudgmentAction)i) {
                    Settings.RestrictJudgmentAction = (RestrictJudgmentAction)i;
                }
            }

            // set custom death message
            if (Settings.RestrictJudgmentAction == RestrictJudgmentAction.KillPlayer) {
                GUILayout.Label(TweakStrings.Get(TranslationKeys.RestrictJudgments.CUSTOM_DEATH));
                Settings.CustomDeathString = GUILayout.TextField(Settings.CustomDeathString);
            }
            MoreGUILayout.EndIndent();
        }

        /// <inheritdoc/>
        public override void OnEnable() {
            if (Settings.RestrictJudgments == null) {
                Settings.RestrictJudgments = new bool[TOTAL_JUDGMENTS];
            } else if (Settings.RestrictJudgments.Length != TOTAL_JUDGMENTS) {
                // Judgments were added/removed, migrate to new settings
                bool[] migratedJudgments = new bool[TOTAL_JUDGMENTS];
                int len = Math.Min(TOTAL_JUDGMENTS, Settings.RestrictJudgments.Length);
                for (int i = 0; i < len; i++) {
                    migratedJudgments[i] = Settings.RestrictJudgments[i];
                }
                Settings.RestrictJudgments = migratedJudgments;
            }
        }
    }
}
