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
                if (!Settings.DisableFilter) {
                    return;
                }
                fEnable = false;
            }
        }

        [HarmonyPatch(typeof(scrController), "WaitForStartCo")]
        private static class ControllerDisableStartVfxPatch
        {
            public static void Postfix(scrController __instance) {
                if (!Settings.DisableFilter) {
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
                if (!Settings.DisableBloom) {
                    return;
                }
                bEnable = false;
            }
        }

        [HarmonyPatch(typeof(ffxFlashPlus), "StartEffect")]
        private static class FlashStartEffectAlwaysClearPatch
        {
            public static void Prefix(ffxFlashPlus __instance) {
                if (!Settings.DisableFlash) {
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
                if (!Settings.DisableFlash) {
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
                if (!Settings.DisableHallOfMirrors) {
                    return;
                }
                __instance.cam.Bgcamstatic.enabled = true;
            }
        }

        [HarmonyPatch(typeof(ffxHallOfMirrorsPlus), "ScrubToTime")]
        private static class HomScrubToTimeAlwaysDisabledPatch
        {
            public static void Postfix(ffxHallOfMirrorsPlus __instance) {
                if (!Settings.DisableHallOfMirrors) {
                    return;
                }
                __instance.cam.Bgcamstatic.enabled = true;
            }
        }

        [HarmonyPatch(typeof(ffxShakeScreenPlus), "StartEffect")]
        private static class ShakeScreenStartEffectAlwaysDisabledPatch
        {
            public static bool Prefix() {
                return !Settings.DisableScreenShake;
            }
        }

        [HarmonyPatch(typeof(ffxShakeScreenPlus), "ScrubToTime")]
        private static class ShakeScreenScrubToTimeAlwaysDisabledPatch
        {
            public static bool Prefix() {
                return !Settings.DisableScreenShake;
            }
        }

        [HarmonyPatch(typeof(ffxMoveFloorPlus), "StartEffect")]
        private static class MoveFloorStartEffectLimitRangePatch
        {
            private static int origStart;
            private static int origEnd;

            public static void Prefix(ffxMoveFloorPlus __instance) {
                if (Settings.MoveTrackMax > DisableEffectsSettings.MOVE_TRACK_UPPER_BOUND) {
                    return;
                }
                int index = scrController.instance.currFloor.seqID;
                origStart = __instance.start;
                origEnd = __instance.end;
                if (origEnd < index + Settings.MoveTrackMax / 2) {
                    __instance.start = Math.Max(origEnd - Settings.MoveTrackMax - 1, origStart);
                } else if (origStart > index - Settings.MoveTrackMax / 2) {
                    __instance.end = Math.Min(origStart + Settings.MoveTrackMax - 1, origEnd);
                } else {
                    __instance.start = Math.Max(index - Settings.MoveTrackMax / 2, origStart);
                    __instance.end = Math.Min(index + Settings.MoveTrackMax / 2, origEnd);
                }
            }

            public static void Postfix(ffxMoveFloorPlus __instance) {
                if (Settings.MoveTrackMax > DisableEffectsSettings.MOVE_TRACK_UPPER_BOUND) {
                    return;
                }
                __instance.start = origStart;
                __instance.end = origEnd;
            }
        }

        [HarmonyPatch(typeof(ffxMoveFloorPlus), "ScrubToTime")]
        private static class MoveFloorScrubToTimeLimitRangePatch
        {
            private static int origStart;
            private static int origEnd;

            public static void Prefix(ffxMoveFloorPlus __instance) {
                if (Settings.MoveTrackMax > DisableEffectsSettings.MOVE_TRACK_UPPER_BOUND) {
                    return;
                }
                int index = scrController.instance.currFloor.seqID;
                origStart = __instance.start;
                origEnd = __instance.end;
                if (origEnd < index + Settings.MoveTrackMax / 2) {
                    __instance.start = Math.Max(origEnd - Settings.MoveTrackMax - 1, origStart);
                } else if (origStart > index - Settings.MoveTrackMax / 2) {
                    __instance.end = Math.Min(origStart + Settings.MoveTrackMax - 1, origEnd);
                } else {
                    __instance.start = Math.Max(index - Settings.MoveTrackMax / 2, origStart);
                    __instance.end = Math.Min(index + Settings.MoveTrackMax / 2, origEnd);
                }
            }

            public static void Postfix(ffxMoveFloorPlus __instance) {
                if (Settings.MoveTrackMax > DisableEffectsSettings.MOVE_TRACK_UPPER_BOUND) {
                    return;
                }
                __instance.start = origStart;
                __instance.end = origEnd;
            }
        }
    }
}
