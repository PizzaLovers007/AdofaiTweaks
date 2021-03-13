using System;
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

        /// <summary>
        /// Starts up the runner.
        /// </summary>
        public void Start() {
            foreach (Type type in TweakMetadata.PatchesType.GetNestedTypes(AccessTools.all)) {
                harmony.CreateClassProcessor(type).Patch();
            }
            if (Settings.IsEnabled) {
                Tweak.OnEnable();
            }
        }

        /// <summary>
        /// Stops the runner.
        /// </summary>
        public void Stop() {
            if (Settings.IsEnabled) {
                Tweak.OnDisable();
            }
            harmony.UnpatchAll(harmony.Id);
        }

        /// <summary>
        /// Handler for adding this tweak's settings GUI to UMM's settings GUI.
        /// </summary>
        public void OnGUI() {
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
                    Tweak.OnEnable();
                    newIsExpanded = true;
                } else {
                    Tweak.OnDisable();
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
        public void OnHideGUI() {
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
        public void OnUpdate(float deltaTime) {
            if (Settings.IsEnabled) {
                Tweak.OnUpdate(deltaTime);
            }
        }

        /// <summary>
        /// Handler for changing the language of AdofaiTweaks.
        /// </summary>
        public void OnLanguageChange() {
            if (Settings.IsEnabled) {
                Tweak.OnLanguageChange();
            }
        }
    }
}
