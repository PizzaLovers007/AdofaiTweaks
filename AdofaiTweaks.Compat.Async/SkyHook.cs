#if SKY_HOOK

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        public static bool IsAsyncInputEnabled => IsAsyncEnabled_Hook();
        public static bool IsAsyncEnabled_Hook()
        {

            var bindingFlags = System.Reflection.BindingFlags.IgnoreReturn;
            unchecked
            {
                bindingFlags = (System.Reflection.BindingFlags)0xffffffffffffffff;
            }
            
            Object instance = SkyHookManager.Instance;
            MethodInfo[] meths = instance.GetType().GetMethods();


            foreach (MethodInfo item in meths)
            {
                if(item.Name == "get_isHookActive")
                {
                    return (bool)item.Invoke(instance, null);
                }
            }
            throw new System.Exception("wtf dude...");
             
            
        }

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
        /// <returns>
        /// Human-readable string of the given <paramref name="code"/>.
        /// </returns>
        public static string GetLabel(ushort code) {
            if (codeToLabelDict.TryGetValue(code, out KeyLabel label)) {
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
                if (ALWAYS_BOUND_ASYNC_KEYS.Contains(code.label)) {
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
            return ALWAYS_BOUND_ASYNC_KEYS.Count(key => AsyncInput.GetKeyDown(key, false));
        }

        /// <summary>
        /// Updates the internal cache based on current <see
        /// cref="AsyncKeyCode"/> inputs this frame. Can be removed once the
        /// <see cref="UnityEngine.KeyCode"/> to <see cref="KeyLabel"/> mapping
        /// can be accessed statically.
        /// </summary>
        public static void UpdateAsyncKeyCache() {
            foreach (var code in AsyncInputManager.frameDependentKeyDownMask) {
                codeToLabelDict[code.key] = code.label;
            }
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