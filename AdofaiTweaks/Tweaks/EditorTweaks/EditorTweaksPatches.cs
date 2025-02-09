using System;
using System.Linq;
using System.Text;
using ADOFAI;
using ADOFAI.Editor.Actions;
using AdofaiTweaks.Core.Attributes;
using AdofaiTweaks.Utils;
using HarmonyLib;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.EditorTweaks;

/// <summary>
/// Patches for the Editor Tweaks tweak.
/// </summary>
internal static class EditorTweaksPatches
{
    [SyncTweakSettings]
    private static EditorTweaksSettings Settings { get; set; }

    /// <summary>
    /// Flip & Rotation syncing level event types.
    /// </summary>
    internal static readonly LevelEventType[] WhitelistedLevelEvents = [
        LevelEventType.MoveTrack, LevelEventType.MoveCamera, LevelEventType.PositionTrack
    ];

    private static scrFloor lastDisplayedFloor;
    private static int updateDisplayFloorFrame = -1;

    private static void ForceUpdateFloorDisplay(scnEditor editor, scrFloor displayFloor) {
        var sb = new StringBuilder();

        var totalAngle = 0d;

        ADOBase.lm.CalculateFloorAngleLengths();

        // Calculate total angle
        for (var i = 0; i < editor.selectedFloors.Count; i++) {
            var floor = editor.selectedFloors[i];
            if (floor.seqID == editor.floors.Count - 1) {
                continue;
            }

            var speedFactor = i == 0 ? 1 : editor.selectedFloors[0].speed / floor.speed;

            totalAngle += floor.angleLength * speedFactor * Mathf.Rad2Deg;

            var events = editor.events.Where(e =>
                BeatsAlteringLevelEvents.Contains(e.eventType) && e.active && e.floor == floor.seqID);

            foreach (var levelEvent in events) {
                totalAngle += levelEvent.eventType switch {
                    LevelEventType.Pause => levelEvent.GetFloat("duration") * 180d,
                    LevelEventType.FreeRoam => levelEvent.GetInt("duration") * 180d,
                    _ => 0
                } * speedFactor;
            }
        }

        // Display in a way player wants
        if (totalAngle != 0) {
            var addLine = false;
            if (Settings.ShowFloorAngle) {
                sb.Append($"<color=#ff5252>{totalAngle:#.####}\u00b0</color>");
                addLine = true;
            }

            if (Settings.ShowFloorBeats) {
                if (addLine) {
                    sb.Append("\n");
                }

                sb.Append($"<color=#52a9ff>{totalAngle / 180:#.####}\u2669</color>");
                addLine = true;
            }

            if (Settings.ShowFloorCount) {
                if (addLine) {
                    sb.Append("\n");
                }

                sb.Append($"<color=#8a8a8a>{editor.selectedFloors.Count}#</color>");
                addLine = true;
            }

            if (Settings.ShowFloorDuration) {
                if (addLine) {
                    sb.Append("\n");
                }

                sb.Append($"<color=#ffffff>{totalAngle / (editor.selectedFloors[0].speed * editor.levelData.bpm * 3):0.######}s</color>");
            }
        }

        var text = sb.ToString();
        if (text.Length == 0) {
            return;
        }

        if (!editor.selectedFloors.Contains(displayFloor)) {
            displayFloor = editor.selectedFloors[0];
        }

        displayFloor ??= editor.selectedFloors[^1];

        displayFloor.editorNumText.gameObject.SetActive(true);
        displayFloor.editorNumText.letterText.text = text;

        // Cache the last displayed floor
        lastDisplayedFloor = displayFloor;
    }

    // Twirls and Holds are already handled by
    // vanilla CalculateFloorAngleLengths() method
    private static readonly LevelEventType[] BeatsAlteringLevelEvents =
        [LevelEventType.FreeRoam, LevelEventType.Pause];

    private static void ResetLastDisplay(bool deactivateText = true) {
        // Hide the text from the last displayed floor
        if (!lastDisplayedFloor) {
            return;
        }

        if (!lastDisplayedFloor.editorNumText) {
            return;
        }

        if (deactivateText) {
            lastDisplayedFloor.editorNumText.gameObject.SetActive(false);
        }

        lastDisplayedFloor.editorNumText.letterText.text = lastDisplayedFloor.seqID.ToString();
        lastDisplayedFloor = null;
    }

    private static void UpdateFloorDisplay(scrFloor displayFloor = null) {
        var editor = ADOBase.editor;
        if (!editor) {
            return;
        }

        if (editor.SelectionIsEmpty()) {
            ResetLastDisplay();
            return;
        }

        if (editor.showFloorNums) {
            ResetLastDisplay(false);
        } else {
            ResetLastDisplay();

            if (displayFloor != null && !displayFloor.enabled) {
                displayFloor = editor.selectedFloors[0];
            }

            if (displayFloor != null && !displayFloor.enabled) {
                displayFloor = editor.selectedFloors[^1];
            }

            if (displayFloor != null && !displayFloor.enabled) {
                var camera = scrCamera.instance;
                var cameraPos = camera
                    ? camera.transform.position
                    : Vector3.zero;

                var closestFloorDistance = float.PositiveInfinity;

                foreach (var floor in editor.selectedFloors) {
                    if (!floor.enabled) {
                        continue;
                    }

                    var dist = Vector3.Distance(floor.transform.position.WithZ(0), cameraPos.WithZ(0));

                    if (dist < closestFloorDistance) {
                        closestFloorDistance = dist;
                        displayFloor = floor;
                    }
                }
            }

            if (displayFloor != null && displayFloor.enabled) {
                ForceUpdateFloorDisplay(editor, displayFloor);
            }
        }
    }

    private static void UpdateFloorDisplayAfterAFrame() {
        updateDisplayFloorFrame = Time.frameCount + 1;
    }

    [HarmonyPatch(typeof(scnEditor), "Update")]
    private static class RunPerFramePatch {
        private static void Postfix(scnEditor __instance) {
            PerformFineTunedRotation(__instance);
            CheckFloorDisplay();
        }

        private static void CheckFloorDisplay() {
            if (updateDisplayFloorFrame != Time.frameCount) {
                return;
            }

            updateDisplayFloorFrame = -1;

            UpdateFloorDisplay();
        }

        private static void PerformFineTunedRotation(scnEditor editor) {
            if (!Settings.FineTuneFloorRotations) {
                return;
            }

            if (RDInput.holdingControl && RDInput.holdingAlt) {
                var clockwiseRotationCommand = Input.GetKeyDown(KeyCode.Period);
                var counterClockwiseRotationCommand = Input.GetKeyDown(KeyCode.Comma);

                if (!clockwiseRotationCommand && !counterClockwiseRotationCommand) {
                    return;
                }

                if (editor.SelectionIsEmpty()) {
                    return;
                }

                var levelData = editor.levelData;
                var isOldLevel = levelData.isOldLevel;

                float rotationAngle;

                if (isOldLevel) {
                    levelData.pathData =
                        levelData.pathData.Remove(
                            editor.selectedFloors[0].seqID - 1,
                            editor.selectedFloors.Count);
                    rotationAngle = 15;
                } else {
                    levelData.angleData.RemoveRange(
                        editor.selectedFloors[0].seqID - 1,
                        editor.selectedFloors.Count);
                    rotationAngle = Settings.FloorRotationStep;
                }

                foreach (var floor in editor.selectedFloors) {
                    if (isOldLevel) {
                        levelData.pathData = editor.levelData.pathData.Insert(
                            floor.seqID - 1,
                            PathDataUtils.GetRotatedPath(floor.stringDirection, clockwiseRotationCommand).ToString());
                    } else {
                        // ReSharper disable once CompareOfFloatsByEqualityOperator
                        var angle = floor.floatDirection == 999
                            ? floor.floatDirection
                            : floor.floatDirection +
                              (clockwiseRotationCommand ? -1 : 1) * Settings.FloorRotationStep;
                        levelData.angleData.Insert(floor.seqID - 1, angle);
                    }
                }

                editor.RemakePath();
                Sync(
                    editor,
                    FloorMutationType.Rotation,
                    (clockwiseRotationCommand ? -1 : 1) * rotationAngle * Mathf.Deg2Rad);
            }
        }
    }

    [HarmonyPatch(typeof(scnEditor), "OnSelectedFloorChange")]
    private static class FloorTextDisplayPatch {
        private static void Postfix(scrFloor ___lastSelectedFloor) => UpdateFloorDisplay(___lastSelectedFloor);
    }

    [HarmonyPatch(typeof(scnEditor), "RemakePath")]
    private static class FloorTextDisplayAfterPathRemakePatch {
        private static void Postfix(scnEditor __instance) {
            UpdateFloorDisplay(lastDisplayedFloor);
        }
    }

    [HarmonyPatch(typeof(scrFloor), "OnBecameVisible")]
    [HarmonyPatch(typeof(scrFloor), "OnBecameInvisible")]
    private static class FloorTextDisplayCameraFollowPatch {
        private static void Prefix() {
            var editor = ADOBase.editor;
            if (!editor) {
                return;
            }

            if (editor.showFloorNums) {
                return;
            }

            if (editor.playMode) {
                return;
            }

            if (editor.SelectionIsEmpty()) {
                return;
            }

            if (lastDisplayedFloor == null || lastDisplayedFloor.enabled) {
                return;
            }

            UpdateFloorDisplayAfterAFrame();
        }
    }

    [HarmonyPatch(typeof(ToggleFloorNumsEditorAction), "Execute")]
    private static class OnToggleFloorNumsEditorActionPatch {
        private static void Prefix() {
            var editor = ADOBase.editor;
            if (!editor) {
                return;
            }

            if (editor.showFloorNums) {
                // Now turning OFF
                ResetLastDisplay();
            } else {
                // Now turning ON
                ForceUpdateFloorDisplay(editor, lastDisplayedFloor);
            }
        }
    }

    [HarmonyPatch(typeof(scnEditor), "FlipFloor")]
    [HarmonyPatch(typeof(scnEditor), "FlipSelection")]
    private static class EditorFlipPatch {
        private static void Postfix(scnEditor __instance, bool horizontal) {
            var mutateType = horizontal ? FloorMutationType.HorizontalFlip : FloorMutationType.VerticalFlip;
            Sync(__instance, mutateType);
        }
    }

    [HarmonyPatch(typeof(scnEditor), "RotateFloor")]
    [HarmonyPatch(typeof(scnEditor), "RotateSelection")]
    private static class EditorRotate90Patch {
        private static void Postfix(scnEditor __instance, bool CW) {
            var rotationRadian = Mathf.PI / 2 * (CW ? -1 : 1);

            Sync(__instance, FloorMutationType.Rotation, rotationRadian);
        }
    }

    [HarmonyPatch(typeof(scnEditor), "RotateFloor180")]
    [HarmonyPatch(typeof(scnEditor), "RotateSelection180")]
    private static class EditorRotate180Patch {
        private static void Postfix(scnEditor __instance) => Sync(__instance, FloorMutationType.BiDirectionFlip);
    }

    private enum FloorMutationType {
        HorizontalFlip,
        VerticalFlip,
        BiDirectionFlip,
        Rotation,
    }

    private static Vector2 MutateVector(Vector2 vector, FloorMutationType mutateType, float rotationRadians = 0) {
        return mutateType switch {
            FloorMutationType.HorizontalFlip => vector.WithX(-vector.x),
            FloorMutationType.VerticalFlip => vector.WithY(-vector.y),
            FloorMutationType.BiDirectionFlip => -vector,
            FloorMutationType.Rotation => vector.GetRotatedVector(rotationRadians),
            _ => throw new ArgumentOutOfRangeException(nameof(mutateType), mutateType, null)
        };
    }

    private static void Sync(scnEditor editor, FloorMutationType mutateType, float rotationRadians = 0) {
        if (!Settings.SyncLevelEventValuesWithFloorFlipsAndRotations) {
            return;
        }

        var floors = editor.selectedFloors.Select(f => f.seqID).ToList();
        if (floors.Count == 0) {
            return;
        }

        var events = editor.events.Where(e =>
            floors.Contains(e.floor) && WhitelistedLevelEvents.Contains(e.eventType));

        // Undo() completely overwrites LevelData, so we're safe to
        // just edit the values without tampering save state here.
        foreach (var e in events) {
            switch (e.eventType) {
                case LevelEventType.PositionTrack:
                case LevelEventType.MoveTrack:
                    e["positionOffset"] = MutateVector((Vector2)e["positionOffset"], mutateType, rotationRadians);
                    break;
                case LevelEventType.MoveCamera:
                    // Move regardless of relativeTo option
                    e["position"] = MutateVector((Vector2)e["position"], mutateType, rotationRadians);
                    break;
            }
        }

        editor.ApplyEventsToFloors();
    }
}