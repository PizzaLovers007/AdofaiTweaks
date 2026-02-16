using AdofaiTweaks.Core.Attributes;
using HarmonyLib;

namespace AdofaiTweaks.Tweaks.JudgmentVisuals;

/// <summary>
/// Patches for the Judgment Visuals tweak.
/// </summary>
internal static class JudgmentVisualsPatches
{
    [SyncTweakSettings]
    private static JudgmentVisualsSettings Settings { get; set; }

    [HarmonyPatch(typeof(scrPlanet), "SwitchChosen")]
    private static class PlanetSwitchChosenPatch
    {
        public static void Prefix(scrPlanet __instance) {
            var errorMeter = HitErrorMeter.Instance;

            if (!errorMeter)
            {
                return;
            }

            float angleDiff = (float)(__instance.angle - __instance.targetExitAngle);
            if (!__instance.controller.isCW) {
                angleDiff *= -1;
            }
            if (RDC.auto) {
                errorMeter.AddHit(0);
            } else {
                errorMeter.AddHit(angleDiff);
            }
        }
    }

    [HarmonyPatch(typeof(scrController), "Awake_Rewind")]
    private static class ControllerAwakeRewindPatch
    {
        public static void Prefix() {
            HitErrorMeter.Instance?.Reset();
        }
    }

    [HarmonyPatch(typeof(scrController), "ShowHitText")]
    private static class ControllerShowHitTextPatch
    {
        public static bool Prefix(HitMargin hitMargin) {
            if (!Settings.HidePerfects) {
                return true;
            }
            return hitMargin != HitMargin.Perfect;
        }
    }
}