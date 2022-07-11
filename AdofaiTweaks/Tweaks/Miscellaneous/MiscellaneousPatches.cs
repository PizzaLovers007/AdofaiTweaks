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

        [HarmonyPatch(typeof(CustomLevel), "Play")]
        private static class CustomLevelPlayPatch
        {
            public static void IncreaseInputOffset(int offset) {
                scrConductor.currentPreset.inputOffset += offset;
            }

            public static void IncreaseVisualOffset(int offset) {
                scrConductor.visualOffset += offset;
            }

            public static void Postfix(CustomLevel __instance, ref int seqID) {
                if (Settings.IsEnabled && Settings.SetHitsoundVolume) {
                    Settings.UpdateVolume();
                }

#if DEBUG
                // TODO Finish working on start set bpm
                if (Settings.IsEnabled && Settings.SetBpmOnStart) {
                    float oldBpm = scrConductor.instance.bpm, // original bpm
                        newBpm = Settings.Bpm; // new bpm to replace

                    // floor the player is currently on
                    scrFloor floor = scrLevelMaker.instance.listFloors[seqID];

                    // current floor's angle
                    double angle = (floor.exitangle - floor.entryangle + 360) % 360;
                    if (angle == 0) {
                        angle = 360;
                    }

                    // (bpm, angle) => (1000 * angle) / (3 * bpm)

                    // bpm has to be bpmConstant when floor.speed is multiplied,
                    // so dividing floor.speed here (this code does not work
                    // properly) conductor.controller.speed = newBpm /
                    // conductor.controller.speed;

                    // floor's speed should be changed, to set the bpm right
                    float timeCalcBase = (float)angle * 1000,
                        oldTime = timeCalcBase / (oldBpm * 3),
                        newTime = timeCalcBase / (Settings.Bpm * 3);
                    int timeDiff = Mathf.RoundToInt(oldTime - newTime);

                    // add the time difference to sync the music
                    // scrConductor.currentPreset.inputOffset += timeDiff;
                    // scrConductor.visualOffset += timeDiff;

                    /* old code:
                    // add the time difference between the old and new time, to
                    // sync the music
                    conductor.song.time += timeDiff;
                    conductor.song2.time += timeDiff;
                    */

                    scrConductor.instance.StartCoroutine("DesyncFix");
                }
#endif
            }
        }

        [HarmonyPatch(typeof(CustomLevel), "ApplyEvent")]
        private static class CustomLevelApplyEventPatch
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
    }
}
