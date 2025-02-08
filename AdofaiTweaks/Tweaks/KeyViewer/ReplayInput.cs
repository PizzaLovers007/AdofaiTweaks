using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.KeyViewer;

/// <summary>
/// Entry point for ReplayMod to call into to update the key viewer.
/// </summary>
public static class ReplayInput
{
    /// <summary>
    /// The AdofaiTweaks key viewer instance. Can be null.
    /// </summary>
    internal static KeyViewer KeyViewer { get; set; }

    /// <summary>
    /// The current key state.
    /// </summary>
    internal static Dictionary<KeyCode, bool> KeyState { get; set; }

    /// <summary>
    /// Whether inputs are currently being replayed.
    /// </summary>
    internal static bool IsReplayingInputs { get; set; }

    /// <summary>
    /// Tells the key viewer that replay inputs have started.
    /// </summary>
    public static void OnStartInputs() {
        IsReplayingInputs = true;
        ResetKeys();
    }

    /// <summary>
    /// Updates the key viewer with a pressed key.
    /// </summary>
    /// <param name="keyCode">The key that is pressed.</param>
    public static void OnKeyPressed(KeyCode keyCode) {
        if (KeyState == null || !KeyState.ContainsKey(keyCode) || !IsReplayingInputs) {
            return;
        }
        KeyState[keyCode] = true;
        KeyViewer?.UpdateState(KeyState);
    }

    /// <summary>
    /// Updates the key viewer with a released key.
    /// </summary>
    /// <param name="keyCode">The key that is released.</param>
    public static void OnKeyReleased(KeyCode keyCode) {
        if (KeyState == null || !KeyState.ContainsKey(keyCode) || !IsReplayingInputs) {
            return;
        }
        KeyState[keyCode] = false;
        KeyViewer?.UpdateState(KeyState);
    }

    /// <summary>
    /// Tells the key viewer that replay inputs have ended.
    /// </summary>
    public static void OnEndInputs() {
        IsReplayingInputs = false;
        ResetKeys();
    }

    private static void ResetKeys() {
        if (KeyState != null) {
            foreach (KeyCode code in KeyState.Keys.ToList()) {
                KeyState[code] = false;
            }
        }
    }
}