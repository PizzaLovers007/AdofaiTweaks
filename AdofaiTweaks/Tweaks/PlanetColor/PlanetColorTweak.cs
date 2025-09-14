using System.Reflection;
using AdofaiTweaks.Core;
using AdofaiTweaks.Core.Attributes;
using AdofaiTweaks.Strings;
using AdofaiTweaks.Utils;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.PlanetColor;

/// <summary>
/// A tweak for changing the planet colors to a custom color.
/// </summary>
[UsedImplicitly]
[RegisterTweak(
    id: "planet_color",
    settingsType: typeof(PlanetColorSettings),
    patchesType: typeof(PlanetColorPatches))]
public class PlanetColorTweak : Tweak
{
    /// <inheritdoc/>
    public override string Name =>
        TweakStrings.Get(TranslationKeys.PlanetColor.NAME);

    /// <inheritdoc/>
    public override string Description =>
        TweakStrings.Get(TranslationKeys.PlanetColor.DESCRIPTION);

    [SyncTweakSettings]
    private PlanetColorSettings Settings { get; set; }

    /// <inheritdoc/>
    public override void OnSettingsGUI() {
        foreach (var colorProfile in Settings.ColorProfiles) {
            colorProfile.ShowGUISettings(UpdatePlanetColors);
        }
    }

    /// <inheritdoc/>
    public override void OnPatch() {
        UpdatePlanetColors();
    }

    /// <inheritdoc/>
    public override void OnUnpatch() {
        UpdatePlanetColors();
    }

    /// <inheritdoc/>
    public override void OnEnable() {
        Settings.Migrate();
    }

    private static void LoadPlanetColorWithRenderer(scrPlanet planet) {
        planet.planetarySystem.ColorPlanets();
    }

    private static readonly MethodInfo ScrPlanetLoadPlanetColorMethod =
        AccessTools.Method(typeof(scrPlanet), "LoadPlanetColor");

    private static void LoadPlanetColor(scrPlanet planet) {
        ScrPlanetLoadPlanetColorMethod.Invoke(planet, []);
    }

    private static void UpdatePlanetColors() {
        var redPlanet = PlanetGetter.RedPlanet;
        var bluePlanet = PlanetGetter.BluePlanet;
        var greenPlanet = PlanetGetter.GreenPlanet;

        if (redPlanet != null) {
            if (AdofaiTweaks.ReleaseNumber >= 128) {
                LoadPlanetColorWithRenderer(redPlanet);
            } else {
                LoadPlanetColor(redPlanet);
            }
        }

        if (bluePlanet != null) {
            if (AdofaiTweaks.ReleaseNumber >= 128) {
                LoadPlanetColorWithRenderer(bluePlanet);
            } else {
                LoadPlanetColor(bluePlanet);
            }
        }

        if (greenPlanet != null) {
            if (AdofaiTweaks.ReleaseNumber >= 128) {
                LoadPlanetColorWithRenderer(greenPlanet);
            } else {
                LoadPlanetColor(greenPlanet);
            }
        }
    }
}
