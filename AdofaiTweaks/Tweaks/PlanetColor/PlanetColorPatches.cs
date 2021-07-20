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

        [HarmonyPatch(typeof(scrPlanet), "SetPlanetColor")]
        private static class SetPlanetColorPatch
        {
            public static void Prefix(scrPlanet __instance, ref Color color) {
                if (__instance.isRed) {
                    color = Settings.RedBody.PlainColor;
                } else {
                    color = Settings.BlueBody.PlainColor;
                }
            }
        }

        [HarmonyPatch(typeof(scrPlanet), "SetCoreColor")]
        private static class SetCoreColorPatch
        {
            public static void Prefix(scrPlanet __instance, ref Color color) {
                if (__instance.isRed) {
                    color = Settings.RedBody.PlainColor;
                } else {
                    color = Settings.BlueBody.PlainColor;
                }
            }
        }

        [HarmonyPatch(typeof(scrPlanet), "SetTailColor")]
        private static class SetTailColorPatch
        {
            public static void Prefix(scrPlanet __instance, ref Color color) {
                if (__instance.isRed) {
                    color = Settings.RedTail.PlainColor;
                } else {
                    color = Settings.BlueTail.PlainColor;
                }
            }
        }

        [HarmonyPatch(typeof(scrPlanet), "SetRingColor")]
        private static class SetRingColorPatch
        {
            public static void Prefix(scrPlanet __instance, ref Color color) {
                if (__instance.isRed) {
                    color = Settings.RedBody.PlainColor;
                } else {
                    color = Settings.BlueBody.PlainColor;
                }
            }
        }

        [HarmonyPatch(typeof(scrPlanet), "SetColor")]
        private static class SetColorPatch
        {
            public static void Postfix(scrPlanet __instance, AdofaiPlanetColor planetColor) {
                __instance.RemoveGold();
                __instance.SetRainbow(false);
                Color color = planetColor.GetColor();
                __instance.EnableCustomColor();
                __instance.SetPlanetColor(color);
                __instance.SetTailColor(color);
            }
        }

        [HarmonyPatch(typeof(scrPlanet), "LoadPlanetColor")]
        private static class LoadPlanetColorPatch
        {
            private static Color originalColor1;
            private static Color originalColor2;

            public static void Prefix() {
                originalColor1 = Persistence.GetPlayerColor(true);
                originalColor2 = Persistence.GetPlayerColor(false);
                Persistence.SetPlayerColor(Settings.RedBody.PlainColor, true);
                Persistence.SetPlayerColor(Settings.BlueBody.PlainColor, false);
            }

            public static void Postfix() {
                Persistence.SetPlayerColor(originalColor1, true);
                Persistence.SetPlayerColor(originalColor2, false);
            }
        }

        [TweakPatch(
            "PlanetColorPatches.RainbowModeBeforeControllerRefactor",
            "scrController",
            "RainbowMode",
            maxVersion: 74)]
        private static class RainbowModeBeforeControllerRefactorPatch
        {
            public static void Postfix(scrController __instance) {
                __instance.redPlanet.SetRainbow(false);
                __instance.redPlanet.LoadPlanetColor();
                __instance.bluePlanet.SetRainbow(false);
                __instance.bluePlanet.LoadPlanetColor();
            }
        }

        [TweakPatch(
            "PlanetColorPatches.RainbowModeAfterControllerRefactor",
            "scnLevelSelect",
            "RainbowMode",
            minVersion: 75)]
        private static class RainbowModeAfterControllerRefactorPatch
        {
            public static void Postfix(scnLevelSelect __instance) {
                __instance.controller.redPlanet.SetRainbow(false);
                __instance.controller.redPlanet.LoadPlanetColor();
                __instance.controller.bluePlanet.SetRainbow(false);
                __instance.controller.bluePlanet.LoadPlanetColor();
            }
        }

        [TweakPatch(
            "PlanetColorPatches.EnbyModeBeforeControllerRefactor",
            "scrController",
            "EnbyMode",
            maxVersion: 74)]
        private static class EnbyModeBeforeControllerRefactorPatch
        {
            public static void Postfix(scrController __instance) {
                __instance.redPlanet.LoadPlanetColor();
                __instance.bluePlanet.LoadPlanetColor();
            }
        }

        [TweakPatch(
            "PlanetColorPatches.EnbyModeAfterControllerRefactor",
            "scnLevelSelect",
            "EnbyMode",
            minVersion: 75)]
        private static class EnbyModeAfterControllerRefactorPatch
        {
            public static void Postfix(scnLevelSelect __instance) {
                __instance.controller.redPlanet.LoadPlanetColor();
                __instance.controller.bluePlanet.LoadPlanetColor();
            }
        }

        [TweakPatch(
            "PlanetColorPatches.TransModeBeforeControllerRefactor",
            "scrController",
            "TransMode",
            maxVersion: 74)]
        private static class TransModeBeforeControllerRefactorPatch
        {
            public static void Postfix(scrController __instance) {
                __instance.redPlanet.LoadPlanetColor();
                __instance.bluePlanet.LoadPlanetColor();
            }
        }

        [TweakPatch(
            "PlanetColorPatches.TransModeAfterControllerRefactor",
            "scnLevelSelect",
            "TransMode",
            minVersion: 75)]
        private static class TransModeAfterControllerRefactorPatch
        {
            public static void Postfix(scnLevelSelect __instance) {
                __instance.controller.redPlanet.LoadPlanetColor();
                __instance.controller.bluePlanet.LoadPlanetColor();
            }
        }
    }
}
