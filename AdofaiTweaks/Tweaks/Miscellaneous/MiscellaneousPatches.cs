using System.Collections.Generic;
using ADOFAI;
using AdofaiTweaks.Core.Attributes;
using HarmonyLib;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.Miscellaneous
{
    /// <summary>
    /// Patches for the Miscellaneous tweak.
    /// </summary>
    internal static class MiscellaneousPatches
    {
        [SyncTweakSettings]
        private static MiscellaneousSettings Settings { get; set; }

        [HarmonyPatch(typeof(CameraFilterPack_FX_Glitch1), "OnRenderImage")]
        private static class GlitchOnRenderImagePatch
        {
            // Flip happens in these time ranges:
            private static readonly float[,] RANGES = {
                { 9f, 9.125f },
                { 11.125f, 11.25f },
                { 23.25f, 23.735f },
                { 39f, 39.125f },
                { 45.25f, 45.375f },
                { 78.625f, 78.75f },
                { 78.875f, 79f },
                { 87.75f, 87.875f },
            };

            public static void Prefix(ref float ___TimeX) {
                if (!Settings.DisableGlitchFlip) {
                    return;
                }
                ___TimeX += Time.deltaTime;
                for (int i = 0; i < RANGES.GetLength(0); i++) {
                    if (RANGES[i, 0] - 0.01 <= ___TimeX && ___TimeX < RANGES[i, 1] + 0.01f) {
                        ___TimeX += 0.15f;
                    }
                }
                ___TimeX -= Time.deltaTime;
            }
        }

        [HarmonyPatch(typeof(scnEditor), "Update")]
        private static class EditorUpdatePatch
        {
            private static float originalScrollValue;

            public static void Prefix() {
                if (!Settings.DisableEditorZoom) {
                    return;
                }
                if (!scnEditor.instance.playMode) {
                    return;
                }
                var mouseScrollDelta = Input.mouseScrollDelta;
                if (Mathf.Abs(mouseScrollDelta.y) > 0.05f) {
                    originalScrollValue = scrCamera.instance.userSizeMultiplier;
                }
            }

            public static void Postfix() {
                if (!Settings.DisableEditorZoom) {
                    return;
                }
                if (!scnEditor.instance.playMode) {
                    return;
                }
                var mouseScrollDelta = Input.mouseScrollDelta;
                if (Mathf.Abs(mouseScrollDelta.y) > 0.05f) {
                    scrCamera.instance.userSizeMultiplier = originalScrollValue;
                }
            }
        }

        [TweakPatch(
            "Miscellaneous.ForceHitsoundVolumePreScnGamePatch",
            "CustomLevel",
            "ApplyEvent",
            maxVersion: 107)]
        private static class ForceHitsoundVolumePreScnGamePatch
        {
            public static void Postfix(ref LevelEvent evnt, ref List<scrFloor> floors) {
                if (evnt.eventType == LevelEventType.SetHitsound && Settings.IsEnabled && Settings.SetHitsoundVolume) {
                    int floor = evnt.floor;
                    GameObject gameObject = floors[floor].gameObject;

                    ffxSetHitsound[] ffxSetHitsounds = gameObject.GetComponentsInChildren<ffxSetHitsound>();

                    if (ffxSetHitsounds != null) {
                        foreach (ffxSetHitsound ffxSetHitsound in ffxSetHitsounds) {
                            Settings.UpdateVolume(ffxSetHitsound);
                        }
                    }
                }
            }
        }

        [TweakPatch(
            "Miscellaneous.ForceHitsoundVolumePostScnGamePatch",
            "scnGame",
            "ApplyEvent",
            minVersion: 108)]
        private static class ForceHitsoundVolumePostScnGamePatch
        {
            public static void Postfix(ref LevelEvent evnt, ref List<scrFloor> floors) {
                if (evnt.eventType == LevelEventType.SetHitsound && Settings.IsEnabled && Settings.SetHitsoundVolume) {
                    int floor = evnt.floor;
                    GameObject gameObject = floors[floor].gameObject;

                    ffxSetHitsound[] ffxSetHitsounds = gameObject.GetComponentsInChildren<ffxSetHitsound>();

                    if (ffxSetHitsounds != null) {
                        foreach (ffxSetHitsound ffxSetHitsound in ffxSetHitsounds) {
                            Settings.UpdateVolume(ffxSetHitsound);
                        }
                    }
                }
            }
        }

        [TweakPatch(
            "Miscellaneous.EnforceAsyncInputStateAtControllerStart",
            "scrController",
            "Start",
            minVersion: 97)]
        private static class EnforceAsyncInputStateAtControllerStartPatch
        {
            public static void Postfix() {
                if (Settings.SyncInputStateToInputOptions) {
                    var desiredState = Persistence.GetChosenAsynchronousInput();
                    AsyncInputManager.ToggleHook(desiredState);
                }
            }
        }
    }
}
