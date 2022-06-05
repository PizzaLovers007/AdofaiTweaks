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

        private static bool IsRedPlanet(scrPlanet planet) {
            return planet == scrController.instance.redPlanet;
        }

        private static bool IsBluePlanet(scrPlanet planet) {
            return planet == scrController.instance.bluePlanet;
        }

        [HarmonyPatch(typeof(scrPlanet), "SetPlanetColor")]
        private static class SetPlanetColorPatch
        {
            public static void Prefix(scrPlanet __instance, ref Color color) {
                if (IsRedPlanet(__instance) && Settings.Color1Enabled) {
                    color = Settings.Color1;
                } else if (IsBluePlanet(__instance) && Settings.Color2Enabled) {
                    color = Settings.Color2;
                }
            }
        }

        [HarmonyPatch(typeof(scrPlanet), "SetCoreColor")]
        private static class SetCoreColorPatch
        {
            public static void Prefix(scrPlanet __instance, ref Color color) {
                if (IsRedPlanet(__instance) && Settings.Color1Enabled) {
                    color = Settings.Color1;
                } else if (IsBluePlanet(__instance) && Settings.Color2Enabled) {
                    color = Settings.Color2;
                }
            }
        }

        [HarmonyPatch(typeof(scrPlanet), "SetTailColor")]
        private static class SetTailColorPatch
        {
            public static void Prefix(scrPlanet __instance, ref Color color) {
                if (IsRedPlanet(__instance) && Settings.Color1Enabled) {
                    color = Settings.TailColor1;
                } else if (IsBluePlanet(__instance) && Settings.Color2Enabled) {
                    color = Settings.TailColor2;
                }
            }
        }

        [HarmonyPatch(typeof(scrPlanet), "SetRingColor")]
        private static class SetRingColorPatch
        {
            public static void Prefix(scrPlanet __instance, ref Color color) {
                if (IsRedPlanet(__instance) && Settings.Color1Enabled) {
                    color = Settings.Color1;
                } else if (IsBluePlanet(__instance) && Settings.Color2Enabled) {
                    color = Settings.Color2;
                }
            }
        }

        [HarmonyPatch(typeof(scrPlanet), "SetColor")]
        private static class SetColorPatch
        {
            public static void Postfix(scrPlanet __instance, AdofaiPlanetColor planetColor) {
                if (__instance != scrController.instance.redPlanet
                    && __instance != scrController.instance.bluePlanet) {
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
                Persistence.SetPlayerColor(scrPlanet.rainbowColor, true);
                Persistence.SetPlayerColor(scrPlanet.rainbowColor, false);
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
                Persistence.SetPlayerColor(scrPlanet.rainbowColor, true);
                Persistence.SetPlayerColor(scrPlanet.rainbowColor, false);
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
                Persistence.SetPlayerColor(scrPlanet.nbYellowColor, true);
                Persistence.SetPlayerColor(scrPlanet.nbPurpleColor, false);
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
                Persistence.SetPlayerColor(scrPlanet.nbYellowColor, true);
                Persistence.SetPlayerColor(scrPlanet.nbPurpleColor, false);
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
                Persistence.SetPlayerColor(scrPlanet.transBlueColor, true);
                Persistence.SetPlayerColor(scrPlanet.transPinkColor, false);
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
                Persistence.SetPlayerColor(scrPlanet.transBlueColor, true);
                Persistence.SetPlayerColor(scrPlanet.transPinkColor, false);
                return false;
            }
        }
    }
}
