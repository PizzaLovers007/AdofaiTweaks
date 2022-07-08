using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AdofaiTweaks.Core;
using AdofaiTweaks.Core.Attributes;
using AdofaiTweaks.Strings;
using AdofaiTweaks.Translation;
using HarmonyLib;
using UnityEngine;
using UnityModManagerNet;

namespace AdofaiTweaks
{
    /// <summary>
    /// The main runner of the AdofaiTweaks mod.
    /// </summary>
    public static class AdofaiTweaks
    {
        /// <summary>
        /// UMM's logger instance.
        /// </summary>
        public static UnityModManager.ModEntry.ModLogger Logger { get; private set; }

        /// <summary>
        /// Whether the tweak is enabled.
        /// </summary>
        public static bool IsEnabled { get; private set; }

        /// <summary>
        /// The game's release number.
        /// </summary>
        public static readonly int ReleaseNumber = (int)AccessTools.Field(typeof(GCNS), "releaseNumber").GetValue(null);

        /// <summary>
        /// GlobalSettings instance.
        /// </summary>
        [SyncTweakSettings]
        public static GlobalSettings GlobalSettings { get; set; }

        private static List<Type> allTweakTypes;
        private static readonly List<TweakRunner> tweakRunners = new List<TweakRunner>();

        private static SettingsSynchronizer synchronizer;

        /// <summary>
        /// Runs the initial setup of AdofaiTweaks.
        /// </summary>
        /// <param name="modEntry">UMM's mod entry for AdofaiTweaks.</param>
        internal static void Setup(UnityModManager.ModEntry modEntry) {
            allTweakTypes =
                AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .Where(t => t.GetCustomAttribute<RegisterTweakAttribute>() != null)
                    .OrderBy(t => t.Name)
                    .ThenBy(t => t.GetCustomAttribute<RegisterTweakAttribute>().Priority)
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

#if DEBUG
            modEntry.HasUpdate = false;
            modEntry.Info.DisplayName += " <color=#a7a7a7><i>[Debug Build]</i></color>";
#endif
        }

        /// <summary>
        /// Handler for UMM's OnToggle event.
        /// </summary>
        /// <param name="modEntry">UMM's mod entry for AdofaiTweaks.</param>
        /// <param name="value">
        /// <c>true</c> if the mod is enabled, or <c>false</c> if the mod is
        /// disabled.
        /// </param>
        /// <returns><c>true</c>.</returns>
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

        /// <summary>
        /// Starts all tweak runners.
        /// </summary>
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

        /// <summary>
        /// Stops all tweak runners.
        /// </summary>
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

        /// <summary>
        /// Handler for UMM's OnGUI event. Displays the language chooser and
        /// every tweak's settings GUI.
        /// </summary>
        /// <param name="modEntry">UMM's mod entry for AdofaiTweaks.</param>
        private static void OnGUI(UnityModManager.ModEntry modEntry) {
            // Set some default GUI settings for better layouts
            if (GlobalSettings.Language.IsSymbolLanguage()) {
                GUI.skin.button.font = TweakAssets.SymbolLangNormalFont;
                GUI.skin.label.font = TweakAssets.SymbolLangNormalFont;
                GUI.skin.textArea.font = TweakAssets.SymbolLangNormalFont;
                GUI.skin.textField.font = TweakAssets.SymbolLangNormalFont;
                GUI.skin.toggle.font = TweakAssets.SymbolLangNormalFont;
                GUI.skin.button.fontSize = 15;
                GUI.skin.label.fontSize = 15;
                GUI.skin.textArea.fontSize = 15;
                GUI.skin.textField.fontSize = 15;
                GUI.skin.toggle.fontSize = 15;
            }
            GUI.skin.toggle = new GUIStyle(GUI.skin.toggle) {
                margin = new RectOffset(0, 4, 6, 6),
            };
            GUI.skin.label.wordWrap = false;

            GUILayout.Space(4);

            // Language chooser
            GUILayout.BeginHorizontal();
            GUILayout.Space(4);
            GUILayout.Label(
                TweakStrings.Get(TranslationKeys.Global.GLOBAL_LANGUAGE),
                new GUIStyle(GUI.skin.label) {
                    fontStyle = GlobalSettings.Language.IsSymbolLanguage()
                        ? FontStyle.Normal
                        : FontStyle.Bold,
                    font = GlobalSettings.Language.IsSymbolLanguage()
                        ? TweakAssets.KoreanBoldFont
                        : null,
                });
            foreach (LanguageEnum language in Enum.GetValues(typeof(LanguageEnum))) {
                string langString =
                    TweakStrings.GetForLanguage(TranslationKeys.Global.LANGUAGE_NAME, language);

                // Set special styles for selected and Korean language
                GUIStyle style = new GUIStyle(GUI.skin.button);
                if (language == GlobalSettings.Language) {
                    if (language.IsSymbolLanguage()) {
                        style.font = TweakAssets.KoreanBoldFont;
                        style.fontSize = 15;
                    } else {
                        style.fontStyle = FontStyle.Bold;
                    }
                } else if (language.IsSymbolLanguage()) {
                    style.font = TweakAssets.SymbolLangNormalFont;
                    style.fontSize = 15;
                }

                bool click = GUILayout.Button(langString, style);
                if (click) {
                    GlobalSettings.Language = language;
                    foreach (TweakRunner runner in tweakRunners) {
                        runner.OnLanguageChange();
                    }
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            // Show each tweak's GUI
            GUILayout.Space(4);
            foreach (TweakRunner runner in tweakRunners) {
                runner.OnGUI();
            }

            // Reset GUI settings to defaults
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

#if DEBUG
            GUILayout.Label($"<color=#a7a7a7><i>This build is a debug build.\nGame Version: r{ReleaseNumber}\nBuild Date: {GCNS.buildDate}\nCurrent Scene: {ADOBase.sceneName}</i></color>");
#endif
        }

        /// <summary>
        /// Handler for UMM's OnHideGUI event.
        /// </summary>
        /// <param name="modEntry">UMM's mod entry for AdofaiTweaks.</param>
        private static void OnHideGUI(UnityModManager.ModEntry modEntry) {
            foreach (TweakRunner runner in tweakRunners) {
                runner.OnHideGUI();
            }
            synchronizer.Save(modEntry);
        }

        /// <summary>
        /// Handler for UMM's OnSaveGUI event.
        /// </summary>
        /// <param name="modEntry">UMM's mod entry for AdofaiTweaks.</param>
        private static void OnSaveGUI(UnityModManager.ModEntry modEntry) {
            synchronizer.Save(modEntry);
        }

        /// <summary>
        /// Handler for UMM's OnUpdate event.
        /// </summary>
        /// <param name="modEntry">UMM's mod entry for AdofaiTweaks.</param>
        /// <param name="deltaTime">
        /// The amount of time that has passed since the previous frame in
        /// seconds.
        /// </param>
        private static void OnUpdate(UnityModManager.ModEntry modEntry, float deltaTime) {
            foreach (TweakRunner runner in tweakRunners) {
                runner.OnUpdate(deltaTime);
            }
        }
    }
}
