using System.Linq;
using System.Reflection;
using System.Text;
using ADOFAI;
using ADOFAI.Editor.Actions;
using AdofaiTweaks.Core.Attributes;
using HarmonyLib;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.EditorInfo
{
    /// <summary>
    /// Patches for the Restrict Judgments tweak.
    /// </summary>
    internal static class EditorInfoPatches
    {
        [SyncTweakSettings]
        private static EditorInfoSettings Settings { get; set; }

        private static scrFloor lastDisplayedFloor;

        private static int updateDisplayFloorFrame = -1;

        private static readonly FieldInfo EditorLastSelectedFloorField =
            AccessTools.Field(typeof(scnEditor), "lastSelectedFloor");

        // Twirls and Holds are already handled by
        // vanilla CalculateFloorAngleLengths() method
        private static readonly LevelEventType[] BeatsAlteringLevelEvents =
            [LevelEventType.FreeRoam, LevelEventType.Pause];

        private static void ResetLastDisplay(bool deactivateText = true) {
            // Hide the text from the last displayed floor
            if (!lastDisplayedFloor) return;
            if (!lastDisplayedFloor.editorNumText) return;

            if (deactivateText)
                lastDisplayedFloor.editorNumText.gameObject.SetActive(false);

            lastDisplayedFloor.editorNumText.letterText.text = lastDisplayedFloor.seqID.ToString();
            lastDisplayedFloor = null;
        }

        private static void ForceUpdateFloorDisplay(scnEditor editor, scrFloor displayFloor) {
            var sb = new StringBuilder();

            var totalAngle = 0d;

            ADOBase.lm.CalculateFloorAngleLengths();

            // Calculate total angle
            for (var i = 0; i < editor.selectedFloors.Count; i++) {
                var floor = editor.selectedFloors[i];
                if (floor.seqID == editor.floors.Count - 1) continue;

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
            if (Settings.ShowFloorAngle && totalAngle != 0)
                sb.AppendLine($"{totalAngle:#.####}\u00b0");

            if (Settings.ShowFloorBeats && totalAngle != 0) {
                sb.Append($"{totalAngle / 180:#.####}");
            }

            var text = sb.ToString();
            if (text.Length == 0) return;

            if (!editor.selectedFloors.Contains(displayFloor))
                displayFloor = editor.selectedFloors[0];

            displayFloor ??= editor.selectedFloors[^1];

            displayFloor.editorNumText.gameObject.SetActive(true);
            displayFloor.editorNumText.letterText.text = text;

            // Cache the last displayed floor
            lastDisplayedFloor = displayFloor;
        }

        private static void UpdateFloorDisplay(scrFloor displayFloor = null) {
            var editor = ADOBase.editor;
            if (!editor) return;

            if (editor.showFloorNums) {
                ResetLastDisplay(false);
            } else {
                ResetLastDisplay();

                if (!displayFloor || !displayFloor.enabled)
                    displayFloor = editor.selectedFloors[0];

                if (!displayFloor.enabled)
                    displayFloor = editor.selectedFloors[^1];

                if (!displayFloor.enabled) {
                    var enabledFloors = editor.selectedFloors.Where(f => f.enabled).ToArray();
                    if (!enabledFloors.Any()) return;

                    var camera = scrCamera.instance;
                    var cameraPos = camera
                        ? camera.transform.position
                        : Vector3.zero;

                    displayFloor = enabledFloors
                        .OrderBy(f => Vector3.Distance(f.transform.position.WithZ(0), cameraPos.WithZ(0)))
                        .First();
                }

                if (displayFloor.enabled)
                    ForceUpdateFloorDisplay(editor, displayFloor);
            }
        }

        private static void UpdateFloorDisplayAfterAFrame() {
            updateDisplayFloorFrame = Time.frameCount + 1;
        }

        [HarmonyPatch(typeof(scnEditor), "Update")]
        private static class RunPerFramePatch {
            private static void Postfix() {
                if (updateDisplayFloorFrame != Time.frameCount) return;
                updateDisplayFloorFrame = -1;

                UpdateFloorDisplay();
            }
        }

        [HarmonyPatch(typeof(scnEditor), "OnSelectedFloorChange")]
        private static class FloorTextDisplayPatch {
            private static void Postfix(scrFloor ___lastSelectedFloor) => UpdateFloorDisplay(___lastSelectedFloor);
        }

        [HarmonyPatch(typeof(scrFloor), "OnBecameVisible")]
        [HarmonyPatch(typeof(scrFloor), "OnBecameInvisible")]
        private static class FloorTextDisplayCameraFollowPatch {
            private static void Prefix() {
                var editor = ADOBase.editor;
                if (!editor) return;
                if (editor.showFloorNums) return;
                if (editor.playMode) return;
                if (editor.SelectionIsEmpty()) return;

                UpdateFloorDisplayAfterAFrame();
            }
        }
        
        [HarmonyPatch(typeof(ToggleFloorNumsEditorAction), "Execute")]
        private static class OnToggleFloorNumsEditorActionPatch {
            private static void Prefix() {
                var editor = ADOBase.editor;
                if (!editor) return;

                if (editor.showFloorNums) {
                    // Now turning OFF
                    ResetLastDisplay();
                } else {
                    // Now turning ON
                    ForceUpdateFloorDisplay(editor, lastDisplayedFloor);
                }
            }
        }
    }
}
