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

    [HarmonyPatch(typeof(scrHitTextManager), "ShowHitText")]
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