using System.Reflection;
using HarmonyLib;

namespace AdofaiTweaks.Utils;

/// <summary>
/// Compatible getters for <see cref="scrPlanet"/> instances.
/// </summary>
public static class PlanetGetter {
    private static readonly FieldInfo LegacyRedPlanetField = AccessTools.Field(typeof(scrController), "redPlanet");
    private static readonly FieldInfo LegacyBluePlanetField = AccessTools.Field(typeof(scrController), "bluePlanet");

    /// <summary>
    /// Red planet from <see cref="scrController"/>.
    /// </summary>
    public static scrPlanet RedPlanet {
        get {
            return AdofaiTweaks.ReleaseNumber switch {
                <= 127 => scrController.instance == null ? null : (scrPlanet)LegacyRedPlanetField.GetValue(scrController.instance),
                _ => GetNewPlanet(true)
            };
        }
    }

    /// <summary>
    /// Blue planet from <see cref="scrController"/>.
    /// </summary>
    public static scrPlanet BluePlanet {
        get {
            return AdofaiTweaks.ReleaseNumber switch {
                <= 127 => scrController.instance == null ? null : (scrPlanet)LegacyBluePlanetField.GetValue(scrController.instance),
                _ => GetNewPlanet(false)
            };
        }
    }

    private static scrPlanet GetNewPlanet(bool isRed) {
        return isRed ? scrController.instance?.planetRed : scrController.instance?.planetBlue;
    }
}