using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AdofaiTweaks.Core;
using AdofaiTweaks.Core.Attributes;
using AdofaiTweaks.Strings;
using AdofaiTweaks.Translation;
using UnityEngine;
using UnityModManagerNet;

namespace AdofaiTweaks
{
    public static class AdofaiTweaks
    {
        public static UnityModManager.ModEntry.ModLogger Logger { get; private set; }
        public static bool IsEnabled { get; private set; }

        private static List<Type> allTweakTypes;
        private static readonly List<TweakRunner> tweakRunners = new List<TweakRunner>();

        private static SettingsSynchronizer synchronizer;

        [SyncTweakSettings]
        private static GlobalSettings GlobalSettings { get; set; }

        internal static void Setup(UnityModManager.ModEntry modEntry) {
            allTweakTypes =
                AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .Where(t => t.GetCustomAttribute<RegisterTweakAttribute>() != null)
                    .OrderBy(t => t.Name)
                    .OrderBy(t => t.GetCustomAttribute<RegisterTweakAttribute>().Priority)
                    .ToList();

            Logger = modEntry.Logger;
            synchronizer = new SettingsSynchronizer();

            synchronizer.Load(modEntry);

            // Register global settings
            synchronizer.Register(typeof(TweakStrings));
            synchronizer.Register(typeof(AdofaiTweaks));
            synchronizer.Register(typeof(TweakRunner));

            modEntry.OnToggle = OnToggle;
            modEntry.OnGUI = OnGUI;
            modEntry.OnHideGUI = OnHideGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            modEntry.OnUpdate = OnUpdate;
        }

        private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value) {
            IsEnabled = value;
            if (value) {
                StartTweaks();
            } else {
                StopTweaks();

                // Save all settings
                synchronizer.Save(modEntry);
            }
            return true;
        }

        private static void StartTweaks() {
            HashSet<string> tweakIds = new HashSet<string>();

            // Create runners for all the tweaks
            foreach (Type tweakType in allTweakTypes) {
                // Verify tweaks do not have conflicting IDs
                RegisterTweakAttribute registerAttribute =
                    tweakType.GetCustomAttribute<RegisterTweakAttribute>();
                if (tweakIds.Contains(registerAttribute.Id)) {
                    throw new InvalidOperationException(
                        "Found conflicting tweaks with the ID '{0}', ");
                }

                // Create the tweak
                ConstructorInfo constructor = tweakType.GetConstructor(new Type[] { });
                Tweak tweak = (Tweak)constructor.Invoke(null);
                TweakSettings settings =
                    synchronizer.GetSettingsForType(registerAttribute.SettingsType);

                // Create and register the runner
                TweakRunner runner = new TweakRunner(tweak, settings);
                tweakRunners.Add(runner);
                synchronizer.Register(runner.Tweak);
                synchronizer.Register(runner.TweakMetadata.PatchesType);
            }

            // Sync patch/tweak references with their respective settings
            synchronizer.Sync();

            // Start all runners
            foreach (TweakRunner runner in tweakRunners) {
                runner.Start();
            }
        }

        private static void StopTweaks() {
            // Stop all runners
            foreach (TweakRunner runner in tweakRunners) {
                runner.Stop();
                synchronizer.Unregister(runner.Tweak);
                synchronizer.Unregister(runner.TweakMetadata.PatchesType);
            }

            // Clear out all runners
            tweakRunners.Clear();
        }

        private static void OnGUI(UnityModManager.ModEntry modEntry) {
            if (GlobalSettings.Language == LanguageEnum.KOREAN) {
                GUI.skin.button.font = TweakAssets.KoreanNormalFont;
                GUI.skin.label.font = TweakAssets.KoreanNormalFont;
                GUI.skin.textArea.font = TweakAssets.KoreanNormalFont;
                GUI.skin.textField.font = TweakAssets.KoreanNormalFont;
                GUI.skin.toggle.font = TweakAssets.KoreanNormalFont;
                GUI.skin.button.fontSize = 15;
                GUI.skin.label.fontSize = 15;
                GUI.skin.textArea.fontSize = 15;
                GUI.skin.textField.fontSize = 15;
                GUI.skin.toggle.fontSize = 15;
            }
            GUI.skin.toggle = new GUIStyle(GUI.skin.toggle) {
                margin = new RectOffset(0, 4, 6, 6),
            };

            GUILayout.Space(4);

            // Language chooser
            GUILayout.BeginHorizontal();
            GUILayout.Space(4);
            GUILayout.Label(
                TweakStrings.Get(TranslationKeys.Global.GLOBAL_LANGUAGE),
                new GUIStyle(GUI.skin.label) {
                    fontStyle = GlobalSettings.Language == LanguageEnum.KOREAN
                        ? FontStyle.Normal
                        : FontStyle.Bold,
                    font = GlobalSettings.Language == LanguageEnum.KOREAN
                        ? TweakAssets.KoreanBoldFont
                        : null,
                });
            foreach (LanguageEnum language in Enum.GetValues(typeof(LanguageEnum))) {
                string langString =
                    TweakStrings.GetForLanguage(TranslationKeys.Global.LANGUAGE_NAME, language);
                if (GUILayout.Button(langString)) {
                    GlobalSettings.Language = language;
                    foreach (TweakRunner runner in tweakRunners) {
                        runner.OnLanguageChange();
                    }
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(4);
            foreach (TweakRunner runner in tweakRunners) {
                runner.OnGUI();
            }

            GUI.skin.button.font = null;
            GUI.skin.label.font = null;
            GUI.skin.textArea.font = null;
            GUI.skin.textField.font = null;
            GUI.skin.toggle.font = null;
            GUI.skin.button.fontSize = 0;
            GUI.skin.label.fontSize = 0;
            GUI.skin.textArea.fontSize = 0;
            GUI.skin.textField.fontSize = 0;
            GUI.skin.toggle.fontSize = 0;
        }

        private static void OnHideGUI(UnityModManager.ModEntry modEntry) {
            foreach (TweakRunner runner in tweakRunners) {
                runner.OnHideGUI();
            }
            synchronizer.Save(modEntry);
        }

        private static void OnSaveGUI(UnityModManager.ModEntry modEntry) {
            synchronizer.Save(modEntry);
        }

        private static void OnUpdate(UnityModManager.ModEntry modEntry, float deltaTime) {
            foreach (TweakRunner runner in tweakRunners) {
                runner.OnUpdate(deltaTime);
            }
        }
    }
}
