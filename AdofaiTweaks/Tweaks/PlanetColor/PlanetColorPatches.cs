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

    private static bool IsFake(PlanetRenderer planetRenderer) {
        return planetRenderer != PlanetGetter.RedPlanet.planetRenderer && planetRenderer != PlanetGetter.BluePlanet.planetRenderer &&
               planetRenderer != PlanetGetter.GreenPlanet.planetRenderer;
    }
    private static bool IsFake(scrPlanet planet) {
        return planet != PlanetGetter.RedPlanet && planet != PlanetGetter.BluePlanet &&
               planet != PlanetGetter.GreenPlanet;
    }

    [TweakPatch("PlanetColorPatches.PlanetRendererSetPlanetColorPatch", "PlanetRenderer", "SetPlanetColor", minVersion: 128)]
    private static class PlanetRendererSetPlanetColorPatch
    {
        public static void Prefix(PlanetRenderer __instance, ref Color color) {
            if (IsFake(__instance))
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

    [TweakPatch("PlanetColorPatches.PlanetRendererSetCoreColorPatch", "PlanetRenderer", "SetCoreColor", minVersion: 128)]
    private static class PlanetRendererSetCoreColorPatch
    {
        public static void Prefix(PlanetRenderer __instance, ref Color color) {
            if (IsFake(__instance))
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

    [TweakPatch("PlanetColorPatches.PlanetRendererSetTailColorPatch", "PlanetRenderer", "SetTailColor", minVersion: 128)]
    private static class PlanetRendererSetTailColorPatch
    {
        public static void Prefix(PlanetRenderer __instance, ref Color color) {
            if (IsFake(__instance))
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

    [TweakPatch("PlanetColorPatches.PlanetRendererSetRingColorPatch", "PlanetRenderer", "SetRingColor", minVersion: 128)]
    private static class PlanetRendererSetRingColorPatch
    {
        public static void Prefix(PlanetRenderer __instance, ref Color color) {
            if (IsFake(__instance))
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

    [TweakPatch("PlanetColorPatches.PlanetRendererSetFaceColorPatch", "PlanetRenderer", "SetFaceColor", minVersion: 128)]
    private static class PlanetRendererSetFaceColorPatch
    {
        public static void Prefix(PlanetRenderer __instance, ref Color color) {
            if (IsFake(__instance))
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

    [TweakPatch("PlanetColorPatches.PlanetRendererSetColorPatch", "PlanetRenderer", "SetColor", minVersion: 128)]
    private static class PlanetRendererSetColorPatch
    {
        public static void Postfix(PlanetRenderer __instance, AdofaiPlanetColor planetColor) {
            if (IsFake(__instance))
            {
                return;
            }

            __instance.SetRainbow(false);
            Color color = planetColor.GetColor();
            __instance.EnableCustomColor();
            __instance.SetPlanetColor(color);
            __instance.SetTailColor(color);
        }
    }

    [TweakPatch("PlanetColorPatches.PlanetClassSetPlanetColorPatch", "scrPlanet", "SetPlanetColor", maxVersion: 127)]
    private static class PlanetClassSetPlanetColorPatch
    {
        public static void Prefix(scrPlanet __instance, ref Color color) {
            if (IsFake(__instance))
            {
                return;
            }

            if (__instance.IsRedPlanetLegacy() && Red.Enabled) {
                color = Red.Body.SolidColor;
            } else if (__instance.IsBluePlanetLegacy() && Blue.Enabled) {
                color = Blue.Body.SolidColor;
            } else if (__instance.IsGreenPlanetLegacy() && Green.Enabled) {
                color = Green.Body.SolidColor;
            }
        }
    }

    [TweakPatch("PlanetColorPatches.PlanetClassSetCoreColorPatch", "scrPlanet", "SetCoreColor", maxVersion: 127)]
    private static class PlanetClassSetCoreColorPatch
    {
        public static void Prefix(scrPlanet __instance, ref Color color) {
            if (IsFake(__instance))
            {
                return;
            }

            if (__instance.IsRedPlanetLegacy() && Red.Enabled) {
                color = Red.Body.SolidColor;
            } else if (__instance.IsBluePlanetLegacy() && Blue.Enabled) {
                color = Blue.Body.SolidColor;
            } else if (__instance.IsGreenPlanetLegacy() && Green.Enabled) {
                color = Green.Body.SolidColor;
            }
        }
    }

    [TweakPatch("PlanetColorPatches.PlanetClassSetTailColorPatch", "scrPlanet", "SetTailColor", maxVersion: 127)]
    private static class PlanetClassSetTailColorPatch
    {
        public static void Prefix(scrPlanet __instance, ref Color color) {
            if (IsFake(__instance))
            {
                return;
            }

            if (__instance.IsRedPlanetLegacy() && Red.Enabled) {
                color = Red.Tail.SolidColor;
            } else if (__instance.IsBluePlanetLegacy() && Blue.Enabled) {
                color = Blue.Tail.SolidColor;
            } else if (__instance.IsGreenPlanetLegacy() && Green.Enabled) {
                color = Green.Tail.SolidColor;
            }
        }
    }


    [TweakPatch("PlanetColorPatches.PlanetClassSetRingColorPatch", "scrPlanet", "SetRingColor", maxVersion: 127)]
    private static class PlanetClassSetRingColorPatch
    {
        public static void Prefix(scrPlanet __instance, ref Color color) {
            if (IsFake(__instance))
            {
                return;
            }

            if (__instance.IsRedPlanetLegacy() && Red.Enabled) {
                color = Red.Body.SolidColor;
            } else if (__instance.IsBluePlanetLegacy() && Blue.Enabled) {
                color = Blue.Body.SolidColor;
            } else if (__instance.IsGreenPlanetLegacy() && Green.Enabled) {
                color = Green.Body.SolidColor;
            }
        }
    }

    [TweakPatch("PlanetColorPatches.PlanetClassSetFaceColorPatch", "scrPlanet", "SetFaceColor", maxVersion: 127)]
    private static class PlanetClassSetFaceColorPatch
    {
        public static void Prefix(scrPlanet __instance, ref Color color) {
            if (IsFake(__instance))
            {
                return;
            }

            if (__instance.IsRedPlanetLegacy() && Red.Enabled) {
                color = Red.Body.SolidColor;
            } else if (__instance.IsBluePlanetLegacy() && Blue.Enabled) {
                color = Blue.Body.SolidColor;
            } else if (__instance.IsGreenPlanetLegacy() && Green.Enabled) {
                color = Green.Body.SolidColor;
            }
        }
    }

    [TweakPatch("PlanetColorPatches.PlanetClassSetColorPatch", "scrPlanet", "SetColor", maxVersion: 127)]
    private static class PlanetClassSetColorPatch {
        private static readonly MethodInfo SetRainbow = AccessTools.Method(typeof(scrPlanet), "SetRainbow");
        private static readonly MethodInfo EnableCustomColor = AccessTools.Method(typeof(scrPlanet), "EnableCustomColor");
        private static readonly MethodInfo SetPlanetColor = AccessTools.Method(typeof(scrPlanet), "SetPlanetColor");
        private static readonly MethodInfo SetTailColor = AccessTools.Method(typeof(scrPlanet), "SetTailColor");

        public static void Postfix(scrPlanet __instance, AdofaiPlanetColor planetColor) {
            if (IsFake(__instance))
            {
                return;
            }

            SetRainbow.Invoke(__instance, [false]);
            Color color = planetColor.GetColor();
            EnableCustomColor.Invoke(__instance, []);
            SetPlanetColor.Invoke(__instance, [color]);
            SetTailColor.Invoke(__instance, [color]);
        }
    }

    [TweakPatch(
        "PlanetColorPatches.GetTweakedPlayerColor",
        "Persistence",
        "GetPlayerColor")]
    private static class GetTweakedPlayerColorPatch
    {
        public static void Postfix(bool red, ref Color __result) {
            if (red && Red.Enabled) {
                __result = Red.Body.SolidColor;
            } else if (!red && Blue.Enabled) {
                __result = Blue.Body.SolidColor;
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
        "PlanetColorPatches.RainbowModeBeforeControllerRefactor",
        "scrController",
        "RainbowMode",
        maxVersion: 74)]

    [TweakPatch(
        "PlanetColorPatches.RainbowModeAfterControllerRefactor",
        "scnLevelSelect",
        "RainbowMode",
        minVersion: 75)]
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
        "PlanetColorPatches.EnbyModeBeforeControllerRefactor",
        "scrController",
        "EnbyMode",
        maxVersion: 74)]

    [TweakPatch(
        "PlanetColorPatches.EnbyModeAfterControllerRefactor",
        "scnLevelSelect",
        "EnbyMode",
        minVersion: 75)]
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
        "PlanetColorPatches.TransModeBeforeControllerRefactor",
        "scrController",
        "TransMode",
        maxVersion: 74)]

    [TweakPatch(
        "PlanetColorPatches.TransModeAfterControllerRefactor",
        "scnLevelSelect",
        "TransMode",
        minVersion: 75)]
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