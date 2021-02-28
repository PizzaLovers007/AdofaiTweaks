using AdofaiTweaks.Core.Attributes;
using HarmonyLib;

namespace AdofaiTweaks.Tweaks.JudgmentVisuals
{
    internal static class JudgmentVisualsPatches
    {
        [SyncTweakSettings]
        private static JudgmentVisualsSettings Settings { get; set; }

        [HarmonyPatch(typeof(scrPlanet), "SwitchChosen")]
        private static class PlanetSwitchChosenPatch
        {
            public static void Prefix(scrPlanet __instance) {
                if (!AdofaiTweaks.IsEnabled || !Settings.IsEnabled) {
                    return;
                }
                float angleDiff = (float)(__instance.angle - __instance.targetExitAngle);
                if (!__instance.controller.isCW) {
                    angleDiff *= -1;
                }
                Settings.ErrorMeter.AddHit(angleDiff);
            }
        }

        [HarmonyPatch(typeof(scrController), "Awake_Rewind")]
        private static class ControllerAwakeRewindPatch
        {
            public static void Prefix() {
                if (!AdofaiTweaks.IsEnabled || !Settings.IsEnabled) {
                    return;
                }
                Settings.ErrorMeter.Reset();
            }
        }
    }
}
