#if SKY_HOOK

using System.Collections.Generic;
using System.Linq;
using SkyHook;

namespace AdofaiTweaks.Compat.Async
{
    /// <summary>
    /// Wrapper around the SkyHook version of <see cref="AsyncInputManager"/>.
    /// </summary>
    public static class AsyncInputManagerCompat
    {
        /// <summary>
        /// Whether async input is enabled.
        /// </summary>
        public static bool IsAsyncInputEnabled => SkyHookManager.Instance.isHookActive;

        /// <summary>
        /// Whether async input is available in this version of the game.
        /// </summary>
        public static bool IsAsyncAvailable => true;

        private static readonly ISet<KeyLabel> ALWAYS_BOUND_ASYNC_KEYS = new HashSet<KeyLabel> {
            KeyLabel.MouseLeft,
            KeyLabel.MouseMiddle,
            KeyLabel.MouseRight,
            KeyLabel.MouseX1,
            KeyLabel.MouseX2,
        };

        private static readonly Dictionary<ushort, KeyLabel> codeToLabelDict = new Dictionary<ushort, KeyLabel>();

        /// <summary>
        /// Gets the human-readable string of the given <paramref name="code"/>.
        /// </summary>
        /// <param name="code">The raw keycode value.</param>
        /// <returns>Human-readable string of the given <paramref name="code"/>.</returns>
        public static string GetLabel(ushort code) {
            if (codeToLabelDict.TryGetValue(code, out KeyLabel label)) {
                return label.ToString();
            }
            return "Not cached yet";
        }

        /// <summary>
        /// Gets the keys that have been pressed down on this frame.
        /// </summary>
        /// <returns>The raw keycodes for the keys that have been pressed down on this frame.</returns>
        public static IEnumerable<ushort> GetKeysDownThisFrame() {
            foreach (var code in AsyncInputManager.frameDependentKeyDownMask) {
                yield return code.key;
                codeToLabelDict[code.key] = code.label;
            }
        }

        /// <summary>
        /// Gets the number of always-bound keys pressed down on this frame.
        /// </summary>
        /// <returns>The number of always-bound keys pressed down on this frame.</returns>
        public static int GetKeyDownCountForAlwaysBoundKeys() {
            return ALWAYS_BOUND_ASYNC_KEYS.Count(key => AsyncInput.GetKeyDown(key, false));
        }
    }

    /// <summary>
    /// Wrapper around the SkyHook version of <see cref="AsyncInput"/>.
    /// </summary>
    public static class AsyncInputCompat
    {
        public static bool GetKeyDown(ushort key) {
            return AsyncInput.GetKeyDown(key, false);
        }
    }
}

#endif