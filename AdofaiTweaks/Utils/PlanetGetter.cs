using System;
using System.Reflection;
using HarmonyLib;
using Rewired.Demos;

namespace AdofaiTweaks.Utils;

/// <summary>
/// Compatible getters for <see cref="scrPlanet"/> instances.
/// </summary>
public static class PlanetGetter {
    private enum PlanetType {
        Red,
        Blue,
        Green,
    }

    private static readonly FieldInfo LegacyRedPlanetField = AccessTools.Field(typeof(scrController), "redPlanet");
    private static readonly FieldInfo LegacyBluePlanetField = AccessTools.Field(typeof(scrController), "bluePlanet");
    private static readonly FieldInfo LegacyGreenPlanetField = AccessTools.Field(typeof(scrController), "greenPlanet");

    /// <summary>
    /// Red planet from <see cref="scrController"/>.
    /// </summary>
    public static scrPlanet RedPlanet {
        get {
            return AdofaiTweaks.ReleaseNumber switch {
                <= 127 => scrController.instance == null ? null : (scrPlanet)LegacyRedPlanetField.GetValue(scrController.instance),
                _ => GetNewPlanet(PlanetType.Red)
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
                _ => GetNewPlanet(PlanetType.Blue)
            };
        }
    }

    /// <summary>
    /// Green planet from <see cref="scrController"/>.
    /// </summary>
    public static scrPlanet GreenPlanet {
        get {
            return AdofaiTweaks.ReleaseNumber switch {
                <= 127 => scrController.instance == null ? null : (scrPlanet)LegacyGreenPlanetField?.GetValue(scrController.instance),
                _ => GetNewPlanet(PlanetType.Green)
            };
        }
    }

    private static scrPlanet GetNewPlanet(PlanetType planet) {
        return planet switch {
            PlanetType.Red => scrController.instance?.planetRed,
            PlanetType.Blue => scrController.instance?.planetBlue,
            PlanetType.Green => scrController.instance?.planetGreen,
            _ => null
        };
    }
}