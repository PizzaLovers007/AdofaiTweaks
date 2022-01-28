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
        private static readonly HitMargin[] JUDGMENTS_TO_RESTRICT = {
            HitMargin.TooEarly,
            HitMargin.VeryEarly,
            HitMargin.EarlyPerfect,
            HitMargin.Perfect,
            HitMargin.LatePerfect,
            HitMargin.VeryLate,
            HitMargin.TooLate,
        };

        private static readonly RestrictJudgmentAction[] RESTRICT_ACTIONS = {
            RestrictJudgmentAction.KillPlayer,
            RestrictJudgmentAction.InstantRestart,
        };

        /// <inheritdoc/>
        public override string Name =>
            TweakStrings.Get(TranslationKeys.RestrictJudgments.NAME);

        /// <inheritdoc/>
        public override string Description =>
            TweakStrings.Get(TranslationKeys.RestrictJudgments.DESCRIPTION);

        [SyncTweakSettings]
        private RestrictJudgmentsSettings Settings { get; set; }

        /// <inheritdoc/>
        public override void OnSettingsGUI() {
            // select judgment
            GUILayout.Label(TweakStrings.Get(TranslationKeys.RestrictJudgments.RESTRICT_HEADER));
            MoreGUILayout.BeginIndent();
            foreach (HitMargin margin in JUDGMENTS_TO_RESTRICT) {
                Settings.RestrictJudgments[(int)margin] =
                    GUILayout.Toggle(
                        Settings.RestrictJudgments[(int)margin],
                        TweakStrings.Get(
                            TranslationKeys.RestrictJudgments.RESTRICT,
                            TweakStrings.GetRDString("HitMargin." + margin)));
            }
            MoreGUILayout.EndIndent();

            // select restriction method
            GUILayout.Label(TweakStrings.Get(TranslationKeys.RestrictJudgments.CUSTOM_HEADER));
            MoreGUILayout.BeginIndent();
            GUILayout.Label(TweakStrings.Get(TranslationKeys.RestrictJudgments.RESTRICT_ACTION));
            foreach (RestrictJudgmentAction action in RESTRICT_ACTIONS) {
                if (GUILayout.Toggle(
                        Settings.RestrictJudgmentAction == action,
                        TweakStrings.Get("RESTRICT_JUDGMENTS_I_RESTRICT_ACTION." + action))
                    && Settings.RestrictJudgmentAction != action) {
                    Settings.RestrictJudgmentAction = action;
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
            int numJudgmentTypes = Enum.GetValues(typeof(HitMargin)).Length;
            if (Settings.RestrictJudgments == null) {
                Settings.RestrictJudgments = new bool[numJudgmentTypes];
            } else if (Settings.RestrictJudgments.Length != numJudgmentTypes) {
                // Judgments were added/removed, migrate to new settings
                bool[] migratedJudgments = new bool[numJudgmentTypes];
                int len = Math.Min(numJudgmentTypes, Settings.RestrictJudgments.Length);
                for (int i = 0; i < len; i++) {
                    migratedJudgments[i] = Settings.RestrictJudgments[i];
                }
                Settings.RestrictJudgments = migratedJudgments;
            }
        }
    }
}
