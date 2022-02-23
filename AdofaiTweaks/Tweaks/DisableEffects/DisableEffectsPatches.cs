using System;
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

        [TweakPatch(
            "DisableEffects.SetFilterAlwaysFalse",
            "ffxSetFilterPlus",
            "SetFilter")]
        private static class SetFilterAlwaysFalsePatch
        {
            public static void Prefix(ffxSetFilterPlus __instance, ref bool fEnable) {
                if (!Settings.DisableFilter) {
                    return;
                }
                if (!Settings.FilterExcludeList.Contains(__instance.filter)) {
                    fEnable = false;
                }
            }
        }

        [TweakPatch(
            "DisableEffects.ControllerDisableStartVfx",
            "scrController",
            "WaitForStartCo")]
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

        [TweakPatch(
            "DisableEffects.SetBloomAlwaysFalse",
            "ffxBloomPlus",
            "SetBloom")]
        private static class SetBloomAlwaysFalsePatch
        {
            public static void Prefix(ref bool bEnable) {
                if (!Settings.DisableBloom) {
                    return;
                }
                bEnable = false;
            }
        }

        [TweakPatch(
            "DisableEffects.FlashStartEffectAlwaysClear",
            "ffxFlashPlus",
            "StartEffect")]
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

        [TweakPatch(
            "DisableEffects.FlashScrubToTimeAlwaysClear",
            "ffxFlashPlus",
            "ScrubToTime",
            MaxVersion = 82)]
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

        [TweakPatch(
            "DisableEffects.HomStartEffectAlwaysDisabled",
            "ffxHallOfMirrorsPlus",
            "StartEffect")]
        private static class HomStartEffectAlwaysDisabledPatch
        {
            public static void Postfix(ffxHallOfMirrorsPlus __instance) {
                if (!Settings.DisableHallOfMirrors) {
                    return;
                }
                __instance.cam.Bgcamstatic.enabled = true;
            }
        }

        [TweakPatch(
            "DisableEffects.HomScrubToTimeAlwaysDisabled",
            "ffxHallOfMirrorsPlus",
            "ScrubToTime",
            MaxVersion = 82)]
        private static class HomScrubToTimeAlwaysDisabledPatch
        {
            public static void Postfix(ffxHallOfMirrorsPlus __instance) {
                if (!Settings.DisableHallOfMirrors) {
                    return;
                }
                __instance.cam.Bgcamstatic.enabled = true;
            }
        }

        [TweakPatch(
            "DisableEffects.ShakeScreenStartEffectAlwaysDisabled",
            "ffxShakeScreenPlus",
            "StartEffect")]
        private static class ShakeScreenStartEffectAlwaysDisabledPatch
        {
            public static bool Prefix() {
                return !Settings.DisableScreenShake;
            }
        }

        [TweakPatch(
            "DisableEffects.ShakeScreenScrubToTimeAlwaysDisabled",
            "ffxShakeScreenPlus",
            "ScrubToTime",
            MaxVersion = 82)]
        private static class ShakeScreenScrubToTimeAlwaysDisabledPatch
        {
            public static bool Prefix() {
                return !Settings.DisableScreenShake;
            }
        }

        [TweakPatch(
            "DisableEffects.MoveFloorStartEffectLimitRange",
            "ffxMoveFloorPlus",
            "StartEffect")]
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

        [TweakPatch(
            "DisableEffects.MoveFloorScrubToTimeLimitRange",
            "ffxMoveFloorPlus",
            "ScrubToTime",
            MaxVersion = 82)]
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
