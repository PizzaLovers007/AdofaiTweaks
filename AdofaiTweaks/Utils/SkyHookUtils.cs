using System;
using System.Collections.Generic;
using System.Linq;
using SkyHook;

namespace AdofaiTweaks.Utils;

/// <summary>
/// A utility class for SkyHook.
/// </summary>
public static class SkyHookUtils {
    private static readonly ISet<KeyLabel> AlwaysBoundAsyncKeys = new HashSet<KeyLabel> {
        KeyLabel.MouseLeft,
        KeyLabel.MouseMiddle,
        KeyLabel.MouseRight,
        KeyLabel.MouseX1,
        KeyLabel.MouseX2,
    };

    private static readonly Dictionary<ushort, KeyLabel> CodeToLabelDict = new ();

    /// <summary>
    /// Gets the human-readable string of the given <paramref name="code"/>.
    /// </summary>
    /// <param name="code">The raw keycode value.</param>
    /// <returns>
    /// Human-readable string of the given <paramref name="code"/>.
    /// </returns>
    public static string GetLabel(ushort code) {
        if (CodeToLabelDict.TryGetValue(code, out var label)) {
            return label.ToString();
        }

        return "Unknown";
    }

    /// <summary>
    /// Gets the keys that have been pressed down on this frame.
    /// </summary>
    /// <returns>
    /// The raw keycodes for the keys that have been pressed down on this
    /// frame.
    /// </returns>
    public static IEnumerable<ushort> GetKeysDownThisFrame() {
        foreach (var code in AsyncInputManager.frameDependentKeyDownMask) {
            if (AlwaysBoundAsyncKeys.Contains(code.label)) {
                continue;
            }

            yield return code.key;
        }
    }

    /// <summary>
    /// Gets the number of always-bound keys pressed down on this frame.
    /// </summary>
    /// <returns>
    /// The number of always-bound keys pressed down on this frame.
    /// </returns>
    public static int GetKeyDownCountForAlwaysBoundKeys() {
        return AlwaysBoundAsyncKeys.Count(key => AsyncInput.GetKeyDown(key, false));
    }

    /// <summary>
    /// Updates the internal cache based on current
    /// <see cref="AsyncKeyCode"/> inputs this frame.
    /// </summary>
    public static void UpdateAsyncKeyCache() {
        foreach (var code in AsyncInputManager.frameDependentKeyDownMask) {
            CodeToLabelDict[code.key] = code.label;
        }
    }

    /// <summary>
    /// Converts an <see cref="AnyKeyCode"/> to a raw async keycode.
    /// </summary>
    /// <param name="key">
    /// The <see cref="AnyKeyCode"/> keycode to convert.
    /// </param>
    /// <returns>The converted raw value.</returns>
    public static ushort ConvertAnyKeyCodeToRaw(AnyKeyCode key) {
        return ((AsyncKeyCode)key.value).key;
    }
}
