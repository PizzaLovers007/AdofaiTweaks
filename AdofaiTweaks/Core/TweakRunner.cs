using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AdofaiTweaks.Core.Attributes;
using AdofaiTweaks.Translation;
using HarmonyLib;
using UnityEngine;

namespace AdofaiTweaks.Core
{
    /// <summary>
    /// Internal runner for every tweak. Handles the following:
    /// <list type="bullet">
    /// <item>Patching/unpatching <see cref="Harmony"/> patches.</item>
    /// <item>
    /// Displaying the title, description, and toggle for the settings GUI.
    /// </item>
    /// <item>
    /// Calling each tweak's lifecycle methods (OnUpdate, OnSettingsGUI, etc.).
    /// </item>
    /// </list>
    /// </summary>
    internal class TweakRunner
    {
        /// <summary>
        /// The <see cref="Tweak"/> instance the runner is operating on.
        /// </summary>
        internal Tweak Tweak { get; private set; }

        /// <summary>
        /// The metadata associated with this tweak.
        /// </summary>
        internal RegisterTweakAttribute TweakMetadata { get; private set; }

        private TweakSettings Settings { get; set; }

        private IList<TweakPatch> TweakPatches { get; set; } = new List<TweakPatch>();

        private readonly Harmony harmony;

        [SyncTweakSettings]
        private static GlobalSettings GlobalSettings { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TweakRunner"/> class
        /// for the given tweak instance and its settings.
        /// </summary>
        /// <param name="tweak">The tweak instance to run.</param>
        /// <param name="settings">The settings for the tweak.</param>
        public TweakRunner(Tweak tweak, TweakSettings settings) {
            Tweak = tweak;
            Settings = settings;
            TweakMetadata =
                Attribute.GetCustomAttribute(tweak.GetType(), typeof(RegisterTweakAttribute))
                    as RegisterTweakAttribute;
            harmony = new Harmony("adofai_tweaks." + TweakMetadata.Id);
        }

        private void EnableTweak() {
            Tweak.OnEnable();
            foreach (Type type in TweakMetadata.PatchesType.GetNestedTypes(AccessTools.all)) {
                TweakPatchAttribute attr = type.GetCustomAttributes(false).OfType<TweakPatchAttribute>()?.SingleOrDefault();
                if (attr != null)
                {
                    TweakPatch tweakPatch = new TweakPatch(type, attr, harmony);
                    if (tweakPatch?.IsValidPatch() ?? false)
                    {
                        TweakPatch duplicatePatch = TweakPatches.SingleOrDefault(p => p.Metadata.PatchId.Equals(attr.PatchId));
                        if (duplicatePatch != null)
                        {
                            AdofaiTweaks.Logger.Log($"Patch with the ID of '{duplicatePatch.Metadata.PatchId}' is already registered. Please check if you have two patches with the same ID.");
                        }
                        else
                        {
                            TweakPatches.Add(tweakPatch);
                            tweakPatch.Patch();
                            // tweakPatch.Unpatch();
                        }
                    }
                }
                else
                {
                    // Old patch code
                    harmony.CreateClassProcessor(type).Patch();
                }
            }
            Tweak.OnPatch();
        }

        private void DisableTweak() {
            Tweak.OnDisable();
            harmony.UnpatchAll(harmony.Id);
            Tweak.OnUnpatch();
        }

        /// <summary>
        /// Starts up the runner.
        /// </summary>
        internal void Start() {
            if (Settings.IsEnabled) {
                EnableTweak();
            }
        }

        /// <summary>
        /// Stops the runner.
        /// </summary>
        internal void Stop() {
            if (Settings.IsEnabled) {
                DisableTweak();
            }
        }

        /// <summary>
        /// Handler for adding this tweak's settings GUI to UMM's settings GUI.
        /// </summary>
        internal void OnGUI() {
            // Draw header
            GUILayout.BeginHorizontal();
            bool newIsExpanded = GUILayout.Toggle(
                Settings.IsExpanded,
                Settings.IsEnabled ? (Settings.IsExpanded ? "◢" : "▶") : "",
                new GUIStyle() {
                    fixedWidth = 10,
                    normal = new GUIStyleState() { textColor = Color.white },
                    fontSize = 15,
                    margin = new RectOffset(4, 2, 6, 6),
                });
            bool newIsEnabled = GUILayout.Toggle(
                Settings.IsEnabled,
                Tweak.Name,
                new GUIStyle(GUI.skin.toggle) {
                    fontStyle = GlobalSettings.Language == LanguageEnum.KOREAN
                        ? FontStyle.Normal
                        : FontStyle.Bold,
                    font = GlobalSettings.Language == LanguageEnum.KOREAN
                        ? TweakAssets.KoreanBoldFont
                        : null,
                    margin = new RectOffset(0, 4, 4, 4),
                });
            GUILayout.Label("-");
            GUILayout.Label(
                Tweak.Description,
                new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Italic });
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            // Handle enable/disable change
            if (newIsEnabled != Settings.IsEnabled) {
                Settings.IsEnabled = newIsEnabled;
                if (newIsEnabled) {
                    EnableTweak();
                    newIsExpanded = true;
                } else {
                    DisableTweak();
                }
            }

            // Handle expand/collapse change
            if (newIsExpanded != Settings.IsExpanded) {
                Settings.IsExpanded = newIsExpanded;
                if (!newIsExpanded) {
                    Tweak.OnHideGUI();
                }
            }

            // Draw custom options
            if (Settings.IsExpanded && Settings.IsEnabled) {
                GUILayout.BeginHorizontal();
                GUILayout.Space(24f);
                GUILayout.BeginVertical();
                Tweak.OnSettingsGUI();
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                GUILayout.Space(12f);
            }
        }

        /// <summary>
        /// Handler for when UMM's settings GUI is hidden.
        /// </summary>
        internal void OnHideGUI() {
            if (Settings.IsEnabled) {
                Tweak.OnHideGUI();
            }
        }

        /// <summary>
        /// Handler for UMM's update event.
        /// </summary>
        /// <param name="deltaTime">
        /// The amount of time that has passed since the previous frame in
        /// seconds.
        /// </param>
        internal void OnUpdate(float deltaTime) {
            if (Settings.IsEnabled) {
                Tweak.OnUpdate(deltaTime);
            }
        }

        /// <summary>
        /// Handler for changing the language of AdofaiTweaks.
        /// </summary>
        internal void OnLanguageChange() {
            if (Settings.IsEnabled) {
                Tweak.OnLanguageChange();
            }
        }
    }
}
