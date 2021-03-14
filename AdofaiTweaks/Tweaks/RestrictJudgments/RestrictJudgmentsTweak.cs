﻿using System;
using AdofaiTweaks.Core;
using AdofaiTweaks.Core.Attributes;
using AdofaiTweaks.Strings;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.RestrictJudgments
{
    /// <summary>
    /// A tweak for killing the player on certain judgments.
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

        /// <inheritdoc/>
        public override void OnSettingsGUI() {
            for (int i = 0; i < TOTAL_JUDGMENTS; i++) {
                Settings.RestrictJudgments[i] = GUILayout.Toggle(
                    Settings.RestrictJudgments[i],
                    TweakStrings.Get(
                        TranslationKeys.RestrictJudgments.RESTRICT,
                        RDString.Get("HitMargin." + (HitMargin)i)));
            }

            GUILayout.Label(TweakStrings.Get(TranslationKeys.RestrictJudgments.CUSTOM_DEATH));
            Settings.CustomDeathString = GUILayout.TextField(Settings.CustomDeathString);
        }

        /// <inheritdoc/>
        public override void OnEnable() {
            if (Settings.RestrictJudgments == null) {
                Settings.RestrictJudgments = new bool[TOTAL_JUDGMENTS];
                for (int i = 0; i < TOTAL_JUDGMENTS; i++) {
                    Settings.RestrictJudgments[i] = false;
                }
            }
        }
    }
}
