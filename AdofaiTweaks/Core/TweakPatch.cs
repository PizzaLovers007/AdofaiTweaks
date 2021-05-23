using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AdofaiTweaks.Core.Attributes;
using HarmonyLib;

namespace AdofaiTweaks.Core
{
    /// <summary>
    /// Tweak's patch system specifically designed to work at all versions of the game to avoid mod from crashing.
    /// </summary>
    internal class TweakPatch
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TweakPatch"/> class.
        /// </summary>
        /// <param name="PatchType">Type of the patching method.</param>
        /// <param name="attr">Attribute of the tweak's patch.</param>
        /// <param name="harmony">Harmony class to apply patch.</param>
        /// <param name="assembly">Assembly to find class from.</param>
        internal TweakPatch(Type PatchType, TweakPatchAttribute attr, Harmony harmony, Assembly assembly = null)
        {
            this.PatchType = PatchType;
            Metadata = attr;
            Harmony = harmony;
            ClassType = (assembly ?? typeof(ADOBase).Assembly).GetType(Metadata.ClassName);
            PatchTargetMethods = ClassType?.GetMethods().Where(m => m.Name.Equals(Metadata.MethodName));
        }

        private Harmony Harmony { get; set; }
        private Type ClassType { get; set; }
        private Type PatchType { get; set; }
        private IEnumerable<MethodInfo> PatchTargetMethods { get; set; }

        /// <summary>
        /// Tweak's patch metadata (attribute data).
        /// </summary>
        internal TweakPatchAttribute Metadata { get; set; }

        /// <summary>
        /// Whether the patch is patched (enabled).
        /// </summary>
        internal bool IsEnabled { get; private set; }

        /// <summary>
        /// Checks whether the patch is valid for current game's version.
        /// </summary>
        /// <param name="showDebuggingMessage">Whether to show debugging message in logs.</param>
        /// <returns>Patch's current availability in <see cref="bool"/>.</returns>
        internal bool IsValidPatch(bool showDebuggingMessage = false)
        {
            if ((Metadata.MinVersion <= GCNS.releaseNumber || Metadata.MinVersion == -1) &&
                (Metadata.MaxVersion >= GCNS.releaseNumber || Metadata.MaxVersion == -1) &&
                ClassType != null &&
                PatchType != null &&
                (PatchTargetMethods?.Count() ?? 0) != 0)
            {
                return true;
            }

#if DEBUG
            if (showDebuggingMessage)
            {
                AdofaiTweaks.Logger.Log($"Patch {Metadata.PatchId} is invalid! - Specific criteria check:\n" +
                $"Metadata.MinVersion <= GCNS.releaseNumber ({Metadata.MinVersion} <= {GCNS.releaseNumber}) is {Metadata.MinVersion <= GCNS.releaseNumber}\n" +
                $"Metadata.MinVersion <= GCNS.releaseNumber ({Metadata.MaxVersion} >= {GCNS.releaseNumber}) is {Metadata.MaxVersion >= GCNS.releaseNumber}\n" +
                $"ClassType is {ClassType}\n" +
                $"PatchType is {PatchType}\n" +
                $"PatchTargetMethods count is {PatchTargetMethods?.Count() ?? 0}");
            }
#endif
            return false;
        }

        /// <summary>
        /// Patches this patch.
        /// </summary>
        internal void Patch()
        {
            if (!IsEnabled)
            {
                foreach (MethodInfo method in PatchTargetMethods)
                {
                    MethodInfo prefixMethodInfo = PatchType.GetMethod("Prefix", AccessTools.all),
                        postfixMethodInfo = PatchType.GetMethod("Postfix", AccessTools.all);

                    HarmonyMethod prefixMethod = null,
                        postfixMethod = null;

                    if (prefixMethodInfo != null)
                    {
                        prefixMethod = new HarmonyMethod(prefixMethodInfo);
                    }

                    if (postfixMethodInfo != null)
                    {
                        postfixMethod = new HarmonyMethod(postfixMethodInfo);
                    }

                    Harmony.Patch(
                        method,
                        prefixMethod,
                        postfixMethod);
                }

                IsEnabled = true;
            }
        }

        /// <summary>
        /// Unpatches this patch.
        /// </summary>
        internal void Unpatch()
        {
            if (IsEnabled)
            {
                foreach (MethodInfo original in PatchTargetMethods)
                {
                    foreach (MethodInfo patch in PatchType.GetMethods())
                    {
                        Harmony.Unpatch(original, patch);
                    }
                }

                IsEnabled = false;
            }
        }
    }
}
