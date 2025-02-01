using AdofaiTweaks.Core.Attributes;
using HarmonyLib;
using UnityEngine;

using AdofaiPlanetColor = PlanetColor;

namespace AdofaiTweaks.Tweaks.PlanetColor
{
    /// <summary>
    /// Patches for the Planet Color tweak.
    /// </summary>
    internal static class PlanetColorPatches
    {
        [SyncTweakSettings]
        private static PlanetColorSettings Settings { get; set; }


        // When you complain about how hideous this is just know that I hate it as much as you do
        private static bool IsRedPlanet(PlanetRenderer planet) {
            return planet == scrController.instance.planetRed.planetRenderer;
        }

        private static bool IsBluePlanet(PlanetRenderer planet) {
            return planet == scrController.instance.planetBlue.planetRenderer;
        }

        [HarmonyPatch(typeof(PlanetRenderer), "SetPlanetColor")]
        private static class SetPlanetColorPatch
        {
            public static void Prefix(PlanetRenderer __instance, ref Color color) {
                if (IsRedPlanet(__instance) && Settings.Color1Enabled) {
                    color = Settings.Color1;
                } else if (IsBluePlanet(__instance) && Settings.Color2Enabled) {
                    color = Settings.Color2;
                }
            }
        }

        [HarmonyPatch(typeof(PlanetRenderer), "SetCoreColor")]
        private static class SetCoreColorPatch
        {
            public static void Prefix(PlanetRenderer __instance, ref Color color) {
                if (IsRedPlanet(__instance) && Settings.Color1Enabled) {
                    color = Settings.Color1;
                } else if (IsBluePlanet(__instance) && Settings.Color2Enabled) {
                    color = Settings.Color2;
                }
            }
        }

        [HarmonyPatch(typeof(PlanetRenderer), "SetTailColor")]
        private static class SetTailColorPatch
        {
            public static void Prefix(PlanetRenderer __instance, ref Color color) {
                if (IsRedPlanet(__instance) && Settings.Color1Enabled) {
                    color = Settings.TailColor1;
                } else if (IsBluePlanet(__instance) && Settings.Color2Enabled) {
                    color = Settings.TailColor2;
                }
            }
        }

        [HarmonyPatch(typeof(PlanetRenderer), "SetRingColor")]
        private static class SetRingColorPatch
        {
            public static void Prefix(PlanetRenderer __instance, ref Color color) {
                if (IsRedPlanet(__instance) && Settings.Color1Enabled) {
                    color = Settings.Color1;
                } else if (IsBluePlanet(__instance) && Settings.Color2Enabled) {
                    color = Settings.Color2;
                }
            }
        }

        [HarmonyPatch(typeof(PlanetRenderer), "SetFaceColor")]
        private static class SetFaceColorPatch
        {
            public static void Prefix(PlanetRenderer __instance, ref Color color) {
                if (IsRedPlanet(__instance) && Settings.Color1Enabled) {
                    color = Settings.Color1;
                } else if (IsBluePlanet(__instance) && Settings.Color2Enabled) {
                    color = Settings.Color2;
                }
            }
        }

        [HarmonyPatch(typeof(PlanetRenderer), "SetColor")]
        private static class SetColorPatch
        {
            public static void Postfix(PlanetRenderer __instance, AdofaiPlanetColor planetColor) {
                if (__instance != scrController.instance.planetRed
                    && __instance != scrController.instance.planetBlue) {
                    return;
                }
                __instance.SetRainbow(false);
                Color color = planetColor.GetColor();
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
            public static void Postfix(bool red, ref Color __result) {
                if (red && Settings.Color1Enabled) {
                    __result = Settings.Color1;
                } else if (!red && Settings.Color2Enabled) {
                    __result = Settings.Color2;
                }
            }
        }

        [TweakPatch(
            "PlanetColorPatches.RainbowModeBeforeControllerRefactor",
            "scrController",
            "RainbowMode",
            maxVersion: 74)]
        private static class RainbowModeBeforeControllerRefactorPatch
        {
            public static bool Prefix() {
                Persistence.SetPlayerColor(PlanetRenderer.rainbowColor, true);
                Persistence.SetPlayerColor(PlanetRenderer.rainbowColor, false);
                return false;
            }
        }

        [TweakPatch(
            "PlanetColorPatches.RainbowModeAfterControllerRefactor",
            "scnLevelSelect",
            "RainbowMode",
            minVersion: 75)]
        private static class RainbowModeAfterControllerRefactorPatch
        {
            public static bool Prefix() {
                Persistence.SetPlayerColor(PlanetRenderer.rainbowColor, true);
                Persistence.SetPlayerColor(PlanetRenderer.rainbowColor, false);
                return false;
            }
        }

        [TweakPatch(
            "PlanetColorPatches.EnbyModeBeforeControllerRefactor",
            "scrController",
            "EnbyMode",
            maxVersion: 74)]
        private static class EnbyModeBeforeControllerRefactorPatch
        {
            public static bool Prefix() {
                Persistence.SetPlayerColor(PlanetRenderer.nbYellowColor, true);
                Persistence.SetPlayerColor(PlanetRenderer.nbPurpleColor, false);
                return false;
            }
        }

        [TweakPatch(
            "PlanetColorPatches.EnbyModeAfterControllerRefactor",
            "scnLevelSelect",
            "EnbyMode",
            minVersion: 75)]
        private static class EnbyModeAfterControllerRefactorPatch
        {
            public static bool Prefix() {
                Persistence.SetPlayerColor(PlanetRenderer.nbYellowColor, true);
                Persistence.SetPlayerColor(PlanetRenderer.nbPurpleColor, false);
                return false;
            }
        }

        [TweakPatch(
            "PlanetColorPatches.TransModeBeforeControllerRefactor",
            "scrController",
            "TransMode",
            maxVersion: 74)]
        private static class TransModeBeforeControllerRefactorPatch
        {
            public static bool Prefix() {
                Persistence.SetPlayerColor(PlanetRenderer.transBlueColor, true);
                Persistence.SetPlayerColor(PlanetRenderer.transPinkColor, false);
                return false;
            }
        }

        [TweakPatch(
            "PlanetColorPatches.TransModeAfterControllerRefactor",
            "scnLevelSelect",
            "TransMode",
            minVersion: 75)]
        private static class TransModeAfterControllerRefactorPatch
        {
            public static bool Prefix() {
                Persistence.SetPlayerColor(PlanetRenderer.transBlueColor, true);
                Persistence.SetPlayerColor(PlanetRenderer.transPinkColor, false);
                return false;
            }
        }
    }
}
