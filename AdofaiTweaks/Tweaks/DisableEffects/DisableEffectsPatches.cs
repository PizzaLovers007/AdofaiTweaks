using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdofaiTweaks.Core.Attributes;
using HarmonyLib;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.DisableEffects
{
    /// <summary>
    /// Patches for the Disable Effects tweak.
    /// </summary>
    internal static class DisableEffectsPatches
    {
        [SyncTweakSettings]
        private static DisableEffectsSettings Settings { get; set; }

        [HarmonyPatch(typeof(ffxSetFilterPlus), "SetFilter")]
        private static class SetFilterAlwaysFalsePatch
        {
            public static void Prefix(ref bool fEnable) {
                if (!AdofaiTweaks.IsEnabled || !Settings.IsEnabled || !Settings.DisableFilter) {
                    return;
                }
                fEnable = false;
            }
        }

        [HarmonyPatch(typeof(scrController), "WaitForStartCo")]
        private static class ControllerDisableStartVfxPatch
        {
            public static void Postfix(scrController __instance) {
                if (!AdofaiTweaks.IsEnabled || !Settings.IsEnabled || !Settings.DisableFilter) {
                    return;
                }
                foreach (MonoBehaviour behavior in __instance.filterToComp.Values) {
                    behavior.enabled = false;
                }
            }
        }

        [HarmonyPatch(typeof(ffxBloomPlus), "SetBloom")]
        private static class SetBloomAlwaysFalsePatch
        {
            public static void Prefix(ref bool bEnable) {
                if (!AdofaiTweaks.IsEnabled || !Settings.IsEnabled || !Settings.DisableBloom) {
                    return;
                }
                bEnable = false;
            }
        }

        [HarmonyPatch(typeof(ffxFlashPlus), "StartEffect")]
        private static class FlashStartEffectAlwaysClearPatch
        {
            public static void Prefix(ffxFlashPlus __instance) {
                if (!AdofaiTweaks.IsEnabled || !Settings.IsEnabled || !Settings.DisableFlash) {
                    return;
                }
                __instance.startColor = Color.clear;
                __instance.endColor = Color.clear;
            }
        }

        [HarmonyPatch(typeof(ffxFlashPlus), "ScrubToTime")]
        private static class FlashScrubToTimeAlwaysClearPatch
        {
            public static void Prefix(ffxFlashPlus __instance) {
                if (!AdofaiTweaks.IsEnabled || !Settings.IsEnabled || !Settings.DisableFlash) {
                    return;
                }
                __instance.startColor = Color.clear;
                __instance.endColor = Color.clear;
            }
        }

        [HarmonyPatch(typeof(ffxHallOfMirrorsPlus), "StartEffect")]
        private static class HomStartEffectAlwaysDisabledPatch
        {
            public static void Postfix(ffxHallOfMirrorsPlus __instance) {
                if (!AdofaiTweaks.IsEnabled
                    || !Settings.IsEnabled
                    || !Settings.DisableHallOfMirrors) {
                    return;
                }
                __instance.cam.Bgcamstatic.enabled = true;
            }
        }

        [HarmonyPatch(typeof(ffxHallOfMirrorsPlus), "ScrubToTime")]
        private static class HomScrubToTimeAlwaysDisabledPatch
        {
            public static void Postfix(ffxHallOfMirrorsPlus __instance) {
                if (!AdofaiTweaks.IsEnabled
                    || !Settings.IsEnabled
                    || !Settings.DisableHallOfMirrors) {
                    return;
                }
                __instance.cam.Bgcamstatic.enabled = true;
            }
        }

        [HarmonyPatch(typeof(ffxShakeScreenPlus), "StartEffect")]
        private static class ShakeScreenStartEffectAlwaysDisabledPatch
        {
            public static bool Prefix() {
                if (!AdofaiTweaks.IsEnabled
                    || !Settings.IsEnabled
                    || !Settings.DisableScreenShake) {
                    return true;
                }
                return false;
            }
        }

        [HarmonyPatch(typeof(ffxShakeScreenPlus), "ScrubToTime")]
        private static class ShakeScreenScrubToTimeAlwaysDisabledPatch
        {
            public static bool Prefix() {
                if (!AdofaiTweaks.IsEnabled
                    || !Settings.IsEnabled
                    || !Settings.DisableScreenShake) {
                    return true;
                }
                return false;
            }
        }
    }
}
