using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AdofaiTweaks.Core.Attributes;
using HarmonyLib;

namespace AdofaiTweaks.Core
{
    /// <summary>
    /// Tweak's patch system specifically designed to work at all versions of
    /// the game to avoid mod from crashing.
    /// </summary>
    internal class TweakPatch
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TweakPatch"/> class.
        /// </summary>
        /// <param name="patchType">Type of the patching method.</param>
        /// <param name="metadata">Attribute of the tweak's patch.</param>
        /// <param name="harmony">Harmony class to apply patch.</param>
        /// <param name="assembly">Assembly to find class from.</param>
        internal TweakPatch(Type patchType, TweakPatchAttribute metadata, Harmony harmony, Assembly assembly = null) {
            PatchType = patchType;
            Metadata = metadata;
            Harmony = harmony;
            ClassType = (assembly ?? typeof(ADOBase).Assembly).GetType(Metadata.ClassName);
            PatchTargetMethods = ClassType?.GetMethods(AccessTools.all).Where(m => m.Name.Equals(Metadata.MethodName));
        }

        private Harmony Harmony { get; }
        private Type ClassType { get; }
        private Type PatchType { get; }
        private IEnumerable<MethodInfo> PatchTargetMethods { get; }
        private readonly string[] _hardcodedMethodNames = ["Prefix", "Postfix", "Transpiler", "Finalizer"];

        /// <summary>
        /// Tweak's patch metadata (attribute data).
        /// </summary>
        internal TweakPatchAttribute Metadata { get; }

        /// <summary>
        /// Whether the patch is patched (enabled).
        /// </summary>
        internal bool IsEnabled { get; private set; }

        /// <summary>
        /// Checks whether the patch is valid for current game's version.
        /// </summary>
        /// <param name="showDebuggingMessage">
        /// Whether to show debugging message in logs.
        /// </param>
        /// <returns>
        /// Patch's current availability in <see cref="bool"/>.
        /// </returns>
        internal bool IsValidPatch(bool showDebuggingMessage = false) {
            if ((Metadata.MinVersion <= AdofaiTweaks.ReleaseNumber || Metadata.MinVersion == -1) &&
                (Metadata.MaxVersion >= AdofaiTweaks.ReleaseNumber || Metadata.MaxVersion == -1) &&
                ClassType != null &&
                PatchType != null &&
                (PatchTargetMethods?.Count() ?? 0) != 0) {
                return true;
            }

#if DEBUG
            if (showDebuggingMessage)
            {
                AdofaiTweaks.Logger.Log(
                    string.Format(
                    "Patch {0} is inapplicable!\n" +
                    " ├ Specific criteria check:\n" +
                    " ├ Metadata.MinVersion <= GCNS.releaseNumber ({1} <= {2}) is {3}{4} - Expected True\n" +
                    " ├ Metadata.MaxVersion <= GCNS.releaseNumber ({5} <= {2}) is {6}{7} - Expected True\n" +
                    " ├ ClassType is {8} - Expected Not Null\n" +
                    " ├ PatchType is {9} - Expected Not Null\n" +
                    " ├ PatchTargetMethods count is {10}{11} - Expected Not Null, Integer above 0\n" +
                    " └ Patch target method name is {12} - Expected Not Null, String\n",
                    // Parameters
                    Metadata.PatchId,
                    Metadata.MinVersion,
                    AdofaiTweaks.ReleaseNumber,
                    Metadata.MinVersion <= AdofaiTweaks.ReleaseNumber || Metadata.MinVersion == -1,
                    Metadata.MinVersion == -1 ? " (-1)" : "",
                    Metadata.MaxVersion,
                    Metadata.MaxVersion >= AdofaiTweaks.ReleaseNumber || Metadata.MaxVersion == -1,
                    Metadata.MaxVersion == -1 ? " (-1)" : "",
                    ClassType,
                    PatchType,
                    PatchTargetMethods?.Count() ?? 0,
                    PatchTargetMethods == null ? " (null)" : "",
                    Metadata.MethodName));
            }
#endif
            return false;
        }

        /// <summary>
        /// Patch contents in this patch instance.
        /// </summary>
        internal void Patch() {
            if (!IsEnabled) {
#if DEBUG
                AdofaiTweaks.Logger.Log($"Applying patch {Metadata.PatchId}");
#endif

                foreach (MethodInfo method in PatchTargetMethods) {
                    List<HarmonyMethod> hardcodedMethods = new List<HarmonyMethod>();

                    foreach (string methodName in _hardcodedMethodNames) {
                        MethodInfo patchMethod = AccessTools.Method(PatchType, methodName);
                        hardcodedMethods.Add(patchMethod == null ? null : new HarmonyMethod(patchMethod));
                    }

                    Harmony.Patch(
                        method,
                        hardcodedMethods[0],
                        hardcodedMethods[1],
                        hardcodedMethods[2],
                        hardcodedMethods[3]);
                }

                IsEnabled = true;
            }
        }

        /// <summary>
        /// Unpatch contents in this patch instance.
        /// </summary>
        internal void Unpatch() {
            if (IsEnabled) {
#if DEBUG
                AdofaiTweaks.Logger.Log($"Cancelling patch {Metadata.PatchId}");
#endif
                foreach (MethodInfo original in PatchTargetMethods) {
                    foreach (MethodInfo patch in PatchType.GetMethods()) {
                        Harmony.Unpatch(original, patch);
                    }
                }

                IsEnabled = false;
            }
        }
    }
}
