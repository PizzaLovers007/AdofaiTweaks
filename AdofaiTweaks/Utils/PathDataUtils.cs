using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AdofaiTweaks.Utils;

/// <summary>
/// Utilities related to handling path data.
/// </summary>
public static class PathDataUtils {
    private static readonly ReadOnlyDictionary<char, int> PathTable = new (new Dictionary<char, int> {
            { 'W', 195 },
            { 'H', 210 },
            { 'Q', 225 },
            { 'G', 240 },
            { 'q', 255 },
            { 'U', 270 },
            { 'o', 285 },
            { 'T', 300 },
            { 'E', 315 },
            { 'J', 330 },
            { 'p', 345 },
            { 'R', 0 },
            { 'A', 15 },
            { 'M', 30 },
            { 'C', 45 },
            { 'B', 60 },
            { 'Y', 75 },
            { 'D', 90 },
            { 'V', 105 },
            { 'F', 120 },
            { 'Z', 135 },
            { 'N', 150 },
            { 'x', 165 },
            { 'L', 180 },
    });

    /// <summary>
    /// Returns a 15° rotated path code in either one of the direction specified with <see cref="pathCode"/>.
    /// </summary>
    /// <param name="pathCode">Existing floor's string path.</param>
    /// <param name="clockwise">Rotation direction. Clockwise if <c>true</c>.</param>
    /// <returns>Rotated floor's path code.</returns>
    public static char GetRotatedPath(char pathCode, bool clockwise) {
        if (!PathTable.TryGetValue(pathCode, out var angle)) {
            return pathCode;
        }

        angle = (angle + (clockwise ? 15 : -15) + 360) % 360;

        return PathTable.FirstOrDefault(p => p.Value == angle).Key;
    }
}