#if !SHARP_HOOK && !SKY_HOOK
#pragma warning disable IDE0060 // Remove unused parameter

using System.Collections.Generic;

namespace AdofaiTweaks.Compat.Async
{
    /// <summary>
    /// Class that "polyfills" <c>AsyncInputManager</c> when no async assembly exists.
    /// </summary>
    public static class AsyncInputManagerCompat
    {
        /// <summary>
        /// Whether async input is enabled.
        /// </summary>
        public static bool IsAsyncInputEnabled => false;

        /// <summary>
        /// Whether async input is available in this version of the game.
        /// </summary>
        public static bool IsAsyncAvailable => false;

        /// <summary>
        /// Always bound keys but for async input.
        /// </summary>
        public static readonly ISet<ushort> ALWAYS_BOUND_ASYNC_KEYS = new HashSet<ushort>();

        /// <summary>
        /// Gets the human-readable string of the given <paramref name="code"/>.
        /// </summary>
        /// <param name="code">The raw keycode value.</param>
        /// <returns>Human-readable string of the given <paramref name="code"/>.</returns>

        public static string GetLabel(ushort code) {
            return "Unknown";
        }

        /// <summary>
        /// Gets the keys that have been pressed down on this frame.
        /// </summary>
        /// <returns>The raw keycodes for the keys that have been pressed down on this frame.</returns>
        public static IEnumerable<ushort> GetKeysDownThisFrame() {
            yield break;
        }

        /// <summary>
        /// Gets the number of always-bound keys pressed down on this frame.
        /// </summary>
        /// <returns>The number of always-bound keys pressed down on this frame.</returns>
        public static int GetKeyDownCountForAlwaysBoundKeys() {
            return 0;
        }
    }

    /// <summary>
    /// Class that "polyfills" <c>AsyncInput</c> when no async assembly exists.
    /// </summary>
    public static class AsyncInputCompat
    {
        public static bool GetKeyDown(ushort key) {
            return false;
        }
    }
}

#pragma warning restore IDE0060 // Remove unused parameter
#endif