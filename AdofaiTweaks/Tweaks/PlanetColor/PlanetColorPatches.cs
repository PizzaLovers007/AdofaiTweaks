using System.Reflection;
using AdofaiTweaks.Core.Attributes;
using AdofaiTweaks.Utils;
using HarmonyLib;
using UnityEngine;

using AdofaiPlanetColor = PlanetColor;

namespace AdofaiTweaks.Tweaks.PlanetColor;

/// <summary>
/// Patches for the Planet Color tweak.
/// </summary>
internal static class PlanetColorPatches
{
    [SyncTweakSettings]
    private static PlanetColorSettings Settings { get; set; }

    private static PlanetColor Red => Settings.ColorProfiles[0];
    private static PlanetColor Blue => Settings.ColorProfiles[1];
    private static PlanetColor Green => Settings.ColorProfiles[2];

    [TweakPatch("PlanetColorPatches.PlanetRendererSetPlanetColorPatch", "PlanetRenderer", "SetPlanetColor")]
    private static class PlanetRendererSetPlanetColorPatch
    {
        public static void Prefix(PlanetRenderer __instance, ref Color color) {
            if (PlanetComparison.IsFake(__instance))
            {
                return;
            }

            if (__instance.IsRedPlanet() && Red.Enabled) {
                color = Red.Body.SolidColor;
            } else if (__instance.IsBluePlanet() && Blue.Enabled) {
                color = Blue.Body.SolidColor;
            } else if (__instance.IsGreenPlanet() && Green.Enabled) {
                color = Green.Body.SolidColor;
            }
        }
    }

    [TweakPatch("PlanetColorPatches.PlanetRendererSetCoreColorPatch", "PlanetRenderer", "SetCoreColor")]
    private static class PlanetRendererSetCoreColorPatch
    {
        public static void Prefix(PlanetRenderer __instance, ref Color color) {
            if (PlanetComparison.IsFake(__instance))
            {
                return;
            }

            if (__instance.IsRedPlanet() && Red.Enabled) {
                color = Red.Body.SolidColor;
            } else if (__instance.IsBluePlanet() && Blue.Enabled) {
                color = Blue.Body.SolidColor;
            } else if (__instance.IsGreenPlanet() && Green.Enabled) {
                color = Green.Body.SolidColor;
            }
        }
    }

    [TweakPatch("PlanetColorPatches.PlanetRendererSetTailColorPatch", "PlanetRenderer", "SetTailColor")]
    private static class PlanetRendererSetTailColorPatch
    {
        public static void Prefix(PlanetRenderer __instance, ref Color color) {
            if (PlanetComparison.IsFake(__instance))
            {
                return;
            }

            if (__instance.IsRedPlanet() && Red.Enabled) {
                color = Red.Tail.SolidColor;
            } else if (__instance.IsBluePlanet() && Blue.Enabled) {
                color = Blue.Tail.SolidColor;
            } else if (__instance.IsGreenPlanet() && Green.Enabled) {
                color = Green.Tail.SolidColor;
            }
        }
    }

    [TweakPatch("PlanetColorPatches.PlanetRendererSetRingColorPatch", "PlanetRenderer", "SetRingColor")]
    private static class PlanetRendererSetRingColorPatch
    {
        public static void Prefix(PlanetRenderer __instance, ref Color color) {
            if (PlanetComparison.IsFake(__instance))
            {
                return;
            }

            if (__instance.IsRedPlanet() && Red.Enabled) {
                color = Red.Body.SolidColor;
            } else if (__instance.IsBluePlanet() && Blue.Enabled) {
                color = Blue.Body.SolidColor;
            } else if (__instance.IsGreenPlanet() && Green.Enabled) {
                color = Green.Body.SolidColor;
            }
        }
    }

    [TweakPatch("PlanetColorPatches.PlanetRendererSetFaceColorPatch", "PlanetRenderer", "SetFaceColor")]
    private static class PlanetRendererSetFaceColorPatch
    {
        public static void Prefix(PlanetRenderer __instance, ref Color color) {
            if (PlanetComparison.IsFake(__instance))
            {
                return;
            }

            if (__instance.IsRedPlanet() && Red.Enabled) {
                color = Red.Body.SolidColor;
            } else if (__instance.IsBluePlanet() && Blue.Enabled) {
                color = Blue.Body.SolidColor;
            } else if (__instance.IsGreenPlanet() && Green.Enabled) {
                color = Green.Body.SolidColor;
            }
        }
    }

    [TweakPatch("PlanetColorPatches.PlanetRendererSetColorPatch", "PlanetRenderer", "SetColor")]
    private static class PlanetRendererSetColorPatch
    {
        public static void Postfix(PlanetRenderer __instance, AdofaiPlanetColor planetColor) {
            if (PlanetComparison.IsFake(__instance))
            {
                return;
            }

            __instance.SetRainbow(false);
            Color color = planetColor.ToRealColor();
            __instance.EnableCustomColor();
            __instance.SetPlanetColor(color);
            __instance.SetTailColor(color);
        }
    }

    [TweakPatch(
        "PlanetColorPatches.GetTweakedPlayerColor",
        "Persistence",
        "GetPlayerColor")]
    private static class GetTweakedPlayerColorPatch
    {
        public static void Postfix(bool red, ref AdofaiPlanetColor __result) {
            if (red && Red.Enabled) {
                __result = new (Red.Body.SolidColor);
            } else if (!red && Blue.Enabled) {
                __result = new (Blue.Body.SolidColor);
            }
        }
    }

    [TweakPatch(
        "PlanetColorPatches.DoNotColorMultiplanet",
        "PlanetarySystem",
        "ApplyMultiplanetColors")]
    private static class DoNotColorMultiplanetPatch {
        public static bool Prefix() => !Green.Enabled;
    }

    [TweakPatch(
        "PlanetColorPatches.RainbowModeAfterControllerRefactor",
        "scnLevelSelect",
        "RainbowMode")]
    private static class RainbowModeNearControllerRefactorPatch
    {
        private static readonly FieldInfo RainbowColorField = AccessTools.Field(typeof(scrPlanet), "rainbowColor");

        public static bool Prefix() {
            var color = (Color)RainbowColorField.GetValue(null);
            Persistence.SetPlayerColor(color, true);
            Persistence.SetPlayerColor(color, false);
            return false;
        }
    }


    [TweakPatch(
        "PlanetColorPatches.EnbyModeAfterControllerRefactor",
        "scnLevelSelect",
        "EnbyMode")]
    private static class EnbyModeNearControllerRefactorPatch
    {
        private static readonly FieldInfo NbYellowColorField = AccessTools.Field(typeof(scrPlanet), "nbYellowColor");
        private static readonly FieldInfo NbPurpleColorField = AccessTools.Field(typeof(scrPlanet), "nbPurpleColor");

        public static bool Prefix() {
            Persistence.SetPlayerColor((Color)NbYellowColorField.GetValue(null), true);
            Persistence.SetPlayerColor((Color)NbPurpleColorField.GetValue(null), false);
            return false;
        }
    }

    [TweakPatch(
        "PlanetColorPatches.TransModeAfterControllerRefactor",
        "scnLevelSelect",
        "TransMode")]
    private static class TransModeNearControllerRefactorPatch
    {
        private static readonly FieldInfo TransBlueColorField = AccessTools.Field(typeof(scrPlanet), "transBlueColor");
        private static readonly FieldInfo TransPinkColorField = AccessTools.Field(typeof(scrPlanet), "transPinkColor");

        public static bool Prefix() {
            Persistence.SetPlayerColor((Color)TransBlueColorField.GetValue(null), true);
            Persistence.SetPlayerColor((Color)TransPinkColorField.GetValue(null), false);
            return false;
        }
    }
}