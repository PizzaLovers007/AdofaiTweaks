using System.Reflection;
using AdofaiTweaks.Core;
using AdofaiTweaks.Core.Attributes;
using AdofaiTweaks.Strings;
using AdofaiTweaks.Utils;
using HarmonyLib;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.PlanetOpacity;

/// <summary>
/// A tweak for changing the opacities of the planets.
/// </summary>
[RegisterTweak(
    id: "planet_opacity",
    settingsType: typeof(PlanetOpacitySettings),
    patchesType: typeof(PlanetOpacityPatches))]
public class PlanetOpacityTweak : Tweak
{
    /// <inheritdoc/>
    public override string Name =>
        TweakStrings.Get(TranslationKeys.PlanetOpacity.NAME);

    /// <inheritdoc/>
    public override string Description =>
        TweakStrings.Get(TranslationKeys.PlanetOpacity.DESCRIPTION);


    [SyncTweakSettings]
    private PlanetOpacitySettings Settings { get; set; }

    /// <inheritdoc/>
    public override void OnEnable() {
        MigrateOldSettings();
    }

    private void MigrateOldSettings() {
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (Settings.SettingsOpacity1 != PlanetOpacitySettings.MIGRATED_VALUE) {
            Settings.PlanetOpacity1.Body = Settings.SettingsOpacity1;
            Settings.PlanetOpacity1.Tail = Settings.SettingsOpacity1;
            Settings.PlanetOpacity1.Ring = Settings.SettingsOpacity1;
            Settings.SettingsOpacity1 = PlanetOpacitySettings.MIGRATED_VALUE;
        }

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (Settings.SettingsOpacity2 != PlanetOpacitySettings.MIGRATED_VALUE) {
            Settings.PlanetOpacity2.Body = Settings.SettingsOpacity2;
            Settings.PlanetOpacity2.Tail = Settings.SettingsOpacity2;
            Settings.PlanetOpacity2.Ring = Settings.SettingsOpacity2;
            Settings.SettingsOpacity2 = PlanetOpacitySettings.MIGRATED_VALUE;
        }
    }

    /// <inheritdoc/>
    public override void OnSettingsGUI() {
        GUILayout.BeginHorizontal();
        MoreGUILayout.LabelPair(
            TweakStrings.Get(TranslationKeys.PlanetOpacity.PLANET_ONE),
            TweakStrings.Get(TranslationKeys.PlanetOpacity.PLANET_TWO),
            200f);
        GUILayout.EndHorizontal();

        MoreGUILayout.BeginIndent();

        float newOpacity1, newOpacity2;
        (newOpacity1, newOpacity2) =
            MoreGUILayout.NamedSliderPair(
                TweakStrings.Get(TranslationKeys.PlanetOpacity.BODY),
                TweakStrings.Get(TranslationKeys.PlanetOpacity.BODY),
                Settings.PlanetOpacity1.Body,
                Settings.PlanetOpacity2.Body,
                0f,
                100f,
                150f,
                roundNearest: 1,
                labelWidth: 80f,
                valueFormat: "{0}%");
        // ReSharper disable CompareOfFloatsByEqualityOperator
        if (newOpacity1 != Settings.PlanetOpacity1.Body
            || newOpacity2 != Settings.PlanetOpacity2.Body) {
            // ReSharper restore CompareOfFloatsByEqualityOperator
            Settings.PlanetOpacity1.Body = newOpacity1;
            Settings.PlanetOpacity2.Body = newOpacity2;
            UpdatePlanetColors();
        }

        (newOpacity1, newOpacity2) =
            MoreGUILayout.NamedSliderPair(
                TweakStrings.Get(TranslationKeys.PlanetOpacity.TAIL),
                TweakStrings.Get(TranslationKeys.PlanetOpacity.TAIL),
                Settings.PlanetOpacity1.Tail,
                Settings.PlanetOpacity2.Tail,
                0f,
                100f,
                150f,
                roundNearest: 1,
                labelWidth: 80f,
                valueFormat: "{0}%");
        // ReSharper disable CompareOfFloatsByEqualityOperator
        if (newOpacity1 != Settings.PlanetOpacity1.Tail
            || newOpacity2 != Settings.PlanetOpacity2.Tail) {
            // ReSharper restore CompareOfFloatsByEqualityOperator
            Settings.PlanetOpacity1.Tail = newOpacity1;
            Settings.PlanetOpacity2.Tail = newOpacity2;
            UpdatePlanetColors();
        }

        (newOpacity1, newOpacity2) =
            MoreGUILayout.NamedSliderPair(
                TweakStrings.Get(TranslationKeys.PlanetOpacity.RING),
                TweakStrings.Get(TranslationKeys.PlanetOpacity.RING),
                Settings.PlanetOpacity1.Ring,
                Settings.PlanetOpacity2.Ring,
                0f,
                100f,
                150f,
                roundNearest: 1,
                labelWidth: 80f,
                valueFormat: "{0}%");
        // ReSharper disable CompareOfFloatsByEqualityOperator
        if (newOpacity1 != Settings.PlanetOpacity1.Ring
            || newOpacity2 != Settings.PlanetOpacity2.Ring) {
            // ReSharper restore CompareOfFloatsByEqualityOperator
            Settings.PlanetOpacity1.Ring = newOpacity1;
            Settings.PlanetOpacity2.Ring = newOpacity2;
            UpdatePlanetColors();
        }

        MoreGUILayout.EndIndent();
    }

    /// <inheritdoc/>
    public override void OnPatch() {
        UpdatePlanetColors();
    }

    /// <inheritdoc/>
    public override void OnUnpatch() {
        UpdatePlanetColors();
    }

    private static void LoadPlanetColorWithRenderer(scrPlanet planet) {
        planet.planetRenderer.LoadPlanetColor(planet == PlanetGetter.RedPlanet);
    }

    private static readonly MethodInfo ScrPlanetLoadPlanetColorMethod =
        AccessTools.Method(typeof(scrPlanet), "LoadPlanetColor");

    private static void LoadPlanetColor(scrPlanet planet) {
        ScrPlanetLoadPlanetColorMethod.Invoke(planet, []);
    }

    private static void UpdatePlanetColors() {
        var redPlanet = PlanetGetter.RedPlanet;
        var bluePlanet = PlanetGetter.BluePlanet;

        if (redPlanet != null) {
            if (AdofaiTweaks.ReleaseNumber >= 128) {
                LoadPlanetColorWithRenderer(redPlanet);
            }
            else {
                LoadPlanetColor(redPlanet);
            }
        }

        if (bluePlanet != null) {
            if (AdofaiTweaks.ReleaseNumber >= 128) {
                LoadPlanetColorWithRenderer(bluePlanet);
            }
            else {
                LoadPlanetColor(bluePlanet);
            }
        }
    }
}