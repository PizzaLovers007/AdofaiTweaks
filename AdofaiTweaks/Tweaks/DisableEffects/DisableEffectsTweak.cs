using System;
using System.Collections.Generic;
using System.Linq;
using ADOFAI;
using AdofaiTweaks.Core;
using AdofaiTweaks.Core.Attributes;
using AdofaiTweaks.Strings;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.DisableEffects
{
    /// <summary>
    /// A tweak for disabling certain effects to improve performance.
    /// </summary>
    [RegisterTweak(
        id: "disable_effects",
        settingsType: typeof(DisableEffectsSettings),
        patchesType: typeof(DisableEffectsPatches))]
    public class DisableEffectsTweak : Tweak
    {
        /// <inheritdoc/>
        public override string Name => TweakStrings.Get(TranslationKeys.DisableEffects.NAME);

        /// <inheritdoc/>
        public override string Description =>
            TweakStrings.Get(TranslationKeys.DisableEffects.DESCRIPTION);

        [SyncTweakSettings]
        private DisableEffectsSettings Settings { get; set; }

        private IDictionary<Filter, bool> FilterExcludeDict;
        private bool DisplayList = false;

        private Vector2 scrollPosition;

        /// <inheritdoc/>
        public override void OnSettingsGUI() {
            // Filter
            GUILayout.BeginHorizontal();
            if (Settings.DisableFilter =
                GUILayout.Toggle(
                    Settings.DisableFilter,
                    TweakStrings.Get(
                        TranslationKeys.DisableEffects.FILTER,
                        RDString.GetEnumValue(Filter.Grayscale),
                        RDString.GetEnumValue(Filter.Arcade)))) {
                // Exclude specific filter from being disabled
                if (GUILayout.Button(TweakStrings.Get(TranslationKeys.DisableEffects.EXCLUDE_FILTER_LIST, Settings.FilterExcludeList.Count))) {
                    DisplayList = !DisplayList;
                }

                if (DisplayList) {
                    GUILayout.EndHorizontal();

                    GUILayout.Space(10f);
                    GUILayout.Label(TweakStrings.Get(TranslationKeys.DisableEffects.EXCLUDE_FILTER_START_LIST));

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20f);

                    scrollPosition = GUILayout.BeginScrollView(
                        scrollPosition, GUILayout.Height(140f));
                    foreach (Filter f in Enum.GetValues(typeof(Filter))) {
                        bool flag = GUILayout.Toggle(FilterExcludeDict[f], RDString.GetEnumValue(f));
                        if (FilterExcludeDict[f] != flag) {
                            FilterExcludeDict[f] = flag;
                            UpdateExcludeList();
                        }
                    }
                    GUILayout.EndScrollView();
                }
            }

            GUILayout.EndHorizontal();

            // Bloom
            Settings.DisableBloom =
                GUILayout.Toggle(
                    Settings.DisableBloom,
                    TweakStrings.Get(TranslationKeys.DisableEffects.BLOOM));

            // Flash
            Settings.DisableFlash =
                GUILayout.Toggle(
                    Settings.DisableFlash,
                    TweakStrings.Get(TranslationKeys.DisableEffects.FLASH));

            // Hall of mirrors
            Settings.DisableHallOfMirrors =
                GUILayout.Toggle(
                    Settings.DisableHallOfMirrors,
                    TweakStrings.Get(
                        TranslationKeys.DisableEffects.HALL_OF_MIRRORS,
                        TweakStrings.GetRDString("editor." + LevelEventType.HallOfMirrors)));

            // Screen shake
            Settings.DisableScreenShake =
                GUILayout.Toggle(
                    Settings.DisableScreenShake,
                    TweakStrings.Get(TranslationKeys.DisableEffects.SCREEN_SHAKE));

            // Move track
            string trackMaxFormat =
                Settings.MoveTrackMax > DisableEffectsSettings.MOVE_TRACK_UPPER_BOUND
                    ? TweakStrings.Get(TranslationKeys.DisableEffects.TILE_MOVEMENT_UNLIMITED)
                    : "{0}";
            float newTrackMax =
                MoreGUILayout.NamedSlider(
                    TweakStrings.Get(TranslationKeys.DisableEffects.TILE_MOVEMENT_MAX),
                    Settings.MoveTrackMax,
                    5,
                    DisableEffectsSettings.MOVE_TRACK_UPPER_BOUND + 5,
                    300f,
                    roundNearest: 5,
                    valueFormat: trackMaxFormat);
            Settings.MoveTrackMax = Mathf.RoundToInt(newTrackMax);
        }

        /// <inheritdoc/>
        public override void OnEnable() {
            FilterExcludeDict = new Dictionary<Filter, bool>();

            foreach (Filter f in Enum.GetValues(typeof(Filter))) {
                FilterExcludeDict.Add(f, Settings.FilterExcludeList.Contains(f));
            }
        }

        private void UpdateExcludeList() {
            List<Filter> result = new List<Filter>();
            foreach (KeyValuePair<Filter, bool> p in FilterExcludeDict.Where(p => p.Value)) {
                result.Add(p.Key);
            }
            Settings.FilterExcludeList = result;
        }
    }
}
