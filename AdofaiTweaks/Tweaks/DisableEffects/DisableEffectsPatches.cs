using System;
using System.Collections.Generic;
using System.Reflection;
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
        private static FieldInfo filterToCompFieldInfo;

        [SyncTweakSettings]
        private static DisableEffectsSettings Settings { get; set; }

        static DisableEffectsPatches() {
            if (AdofaiTweaks.ReleaseNumber <= 82) {
                filterToCompFieldInfo = AccessTools.Field(typeof(scrController), "filterToComp");
            } else {
                filterToCompFieldInfo = AccessTools.Field(typeof(scrVfxPlus), "filterToComp");
            }
        }

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
                Dictionary<Filter, MonoBehaviour> filterToComp =
                    (AdofaiTweaks.ReleaseNumber <= 82
                        ? filterToCompFieldInfo.GetValue(__instance)
                        : filterToCompFieldInfo.GetValue(scrVfxPlus.instance))
                    as Dictionary<Filter, MonoBehaviour>;
                foreach (MonoBehaviour behavior in filterToComp.Values) {
                    behavior.enabled = false;
                }
            }
        }

        [TweakPatch(
            "DisableEffects.SetBloomAlwaysFalse",
            "ffxBloomPlus",
            "StartEffect")]
        private static class BloomStartEffectDoNothingPatch
        {
            public static bool Prefix() {
                return !Settings.DisableBloom;
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
            "DisableEffects.HomStartEffectAlwaysDisabledPre105",
            "ffxHallOfMirrorsPlus",
            "StartEffect",
            MaxVersion = 104)]
        private static class HomStartEffectAlwaysDisabledPre105Patch
        {
            public static void Postfix(ffxHallOfMirrorsPlus __instance) {
                if (!Settings.DisableHallOfMirrors) {
                    return;
                }
                __instance.cam.Bgcamstatic.enabled = true;
            }
        }

        [TweakPatch(
            "DisableEffects.HomStartEffectAlwaysDisabledPost105",
            "ffxHallOfMirrorsPlus",
            "StartEffect",
            MinVersion = 105)]
        private static class HomStartEffectAlwaysDisabledPost105Patch
        {
            public static void Postfix(ffxHallOfMirrorsPlus __instance) {
                if (!Settings.DisableHallOfMirrors) {
                    return;
                }
                __instance.cam.Bgcamstatic.clearFlags = CameraClearFlags.Color;
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
