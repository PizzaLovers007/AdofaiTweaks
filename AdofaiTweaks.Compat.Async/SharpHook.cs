#if SHARP_HOOK

using System.Collections.Generic;
using System.Linq;

namespace AdofaiTweaks.Compat.Async
{
    /// <summary>
    /// Wrapper around the SharpHook version of <see cref="AsyncInputManager"/>.
    /// </summary>
    public static class AsyncInputManagerCompat
    {
        /// <summary>
        /// Whether async input is enabled.
        /// </summary>
        public static bool IsAsyncInputEnabled => AsyncInputManager.isActive;

        /// <summary>
        /// Whether async input is available in this version of the game.
        /// </summary>
        public static bool IsAsyncAvailable => true;

        /// <summary>
        /// A set of keys that will always be counted as input.
        /// </summary>
        private static readonly ISet<UnityEngine.KeyCode> ALWAYS_BOUND_KEYS =
            new HashSet<UnityEngine.KeyCode> {
                UnityEngine.KeyCode.Mouse0,
                UnityEngine.KeyCode.Mouse1,
                UnityEngine.KeyCode.Mouse2,
                UnityEngine.KeyCode.Mouse3,
                UnityEngine.KeyCode.Mouse4,
                UnityEngine.KeyCode.Mouse5,
                UnityEngine.KeyCode.Mouse6,
            };

        private static readonly IDictionary<ushort, UnityEngine.KeyCode> codeToKeyCodeDict =
            KeyCodeConverter.UnityNativeKeyCodeList
                .GroupBy(x => x.Item2)
                .ToDictionary(t => t.Key, t => t.First().Item1);

        /// <summary>
        /// Always bound keys but for async input. Initialized at static
        /// constructor. Unused after r97.
        /// </summary>
        public static readonly ISet<ushort> ALWAYS_BOUND_ASYNC_KEYS;

        static AsyncInputManagerCompat() {
            ALWAYS_BOUND_ASYNC_KEYS = SetupOldAsyncKeyData();
        }

        private static ISet<ushort> SetupOldAsyncKeyData() {
            IDictionary<UnityEngine.KeyCode, ushort> unityNativeKeymap =
                KeyCodeConverter.UnityNativeKeyCodeList
                    .GroupBy(x => x.Item1)
                    .ToDictionary(g => g.Key, g => g.First().Item2);

            ISet<ushort> alwaysBoundAsyncKeys = ALWAYS_BOUND_KEYS
                .Select(k => unityNativeKeymap.TryGetValue(k, out ushort a) ? a : (ushort)0)
                .ToHashSet();

            alwaysBoundAsyncKeys.Remove(0); // ushort 0

            return alwaysBoundAsyncKeys;
        }

        /// <summary>
        /// Gets the human-readable string of the given <paramref name="code"/>.
        /// </summary>
        /// <param name="code">The raw keycode value.</param>
        /// <returns>
        /// Human-readable string of the given <paramref name="code"/>.
        /// </returns>
        public static string GetLabel(ushort code) {
            if (codeToKeyCodeDict.TryGetValue(code, out UnityEngine.KeyCode keyCode)) {
                return keyCode.ToString();
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
            foreach (var key in AsyncInputManager.frameDependentKeyDownMask) {
                if (ALWAYS_BOUND_ASYNC_KEYS.Contains(key.key)) {
                    continue;
                }
                yield return key.key;
            }
        }

        /// <summary>
        /// Gets the number of always-bound keys pressed down on this frame.
        /// </summary>
        /// <returns>
        /// The number of always-bound keys pressed down on this frame.
        /// </returns>
        public static int GetKeyDownCountForAlwaysBoundKeys() {
            return ALWAYS_BOUND_ASYNC_KEYS.Count(key => AsyncInput.GetKeyDown(key, false));
        }

        /// <summary>
        /// Does nothing in this version. Meant for AsyncKeyCode caching.
        /// </summary>
        public static void UpdateAsyncKeyCache() {
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

    /// <summary>
    /// Wrapper around the SharpHook version of <see cref="AsyncInput"/>.
    /// </summary>
    public static class AsyncInputCompat
    {
        public static bool GetKeyDown(ushort key) {
            return AsyncInput.GetKeyDown(key, false);
        }
    }
}

#endif
