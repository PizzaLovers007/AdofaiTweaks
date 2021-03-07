using System;
using AdofaiTweaks.Core.Attributes;
using AdofaiTweaks.Translation;
using HarmonyLib;
using UnityEngine;

namespace AdofaiTweaks.Core
{
    internal class TweakRunner
    {
        public Tweak Tweak { get; private set; }
        public TweakSettings Settings { get; private set; }
        public RegisterTweakAttribute TweakMetadata { get; private set; }

        private readonly Harmony harmony;

        [SyncTweakSettings]
        private static GlobalSettings GlobalSettings { get; set; }

        public TweakRunner(Tweak tweak, TweakSettings settings) {
            Tweak = tweak;
            Settings = settings;
            TweakMetadata =
                Attribute.GetCustomAttribute(tweak.GetType(), typeof(RegisterTweakAttribute))
                    as RegisterTweakAttribute;
            harmony = new Harmony("adofai_tweaks." + TweakMetadata.Id);
        }

        public void Start() {
            foreach (Type type in TweakMetadata.PatchesType.GetNestedTypes(AccessTools.all)) {
                harmony.CreateClassProcessor(type).Patch();
            }
            if (Settings.IsEnabled) {
                Tweak.OnEnable();
            }
        }

        public void Stop() {
            if (Settings.IsEnabled) {
                Tweak.OnDisable();
            }
            harmony.UnpatchAll(harmony.Id);
        }

        public void OnGUI() {
            // Draw header
            GUILayout.BeginHorizontal();
            Settings.IsExpanded = GUILayout.Toggle(
                Settings.IsExpanded,
                Settings.IsEnabled ? (Settings.IsExpanded ? "◢" : "▶") : "",
                new GUIStyle() {
                    fixedWidth = 10,
                    normal = new GUIStyleState() { textColor = Color.white },
                    fontSize = 16,
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
                    Settings.IsExpanded = true;
                } else {
                    Tweak.OnDisable();
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

        public void OnHideGUI() {
            Tweak.OnHideGUI();
        }

        public void OnUpdate(float deltaTime) {
            Tweak.OnUpdate(deltaTime);
        }

        public void OnLanguageChange() {
            Tweak.OnLanguageChange();
        }
    }
}
