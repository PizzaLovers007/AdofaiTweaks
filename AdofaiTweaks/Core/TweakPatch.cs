using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AdofaiTweaks.Core.Attributes;
using HarmonyLib;
using MelonLoader;

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
        internal TweakPatch(Type patchType, TweakPatchAttribute metadata, HarmonyLib.Harmony harmony, Assembly assembly = null) {
            PatchType = patchType;
            PatchMetadata = metadata;
            HarmonyInstance = harmony;
            ClassType = (assembly ?? typeof(ADOBase).Assembly).GetType(PatchMetadata.ClassName);
            PatchTargetMethods = ClassType?.GetMethods(AccessTools.all).Where(m => m.Name.Equals(PatchMetadata.MethodName));
        }

        private HarmonyLib.Harmony HarmonyInstance { get; set; }
        private Type ClassType { get; set; }
        private Type PatchType { get; set; }
        private IEnumerable<MethodInfo> PatchTargetMethods { get; set; }
        private readonly string[] HardcodedMethodNames = new[] { "Prefix", "Postfix", "Transpiler", "Finalizer" };

        /// <summary>
        /// Tweak's patch metadata (attribute data).
        /// </summary>
        internal TweakPatchAttribute PatchMetadata { get; set; }

        /// <summary>
        /// Whether the patch is patched (enabled).
        /// </summary>
        internal bool IsEnabled { get; private set; }

        /// <summary>
        /// Checks whether the patch is valid for current game's version.
        /// </summary>
        /// <returns>
        /// Patch's current availability in <see cref="bool"/>.
        /// </returns>
        internal bool IsValidPatch() {
            if ((PatchMetadata.MinVersion <= AdofaiTweaks.ReleaseNumber || PatchMetadata.MinVersion == -1) &&
                (PatchMetadata.MaxVersion >= AdofaiTweaks.ReleaseNumber || PatchMetadata.MaxVersion == -1) &&
                ClassType != null &&
                PatchType != null &&
                (PatchTargetMethods?.Count() ?? 0) != 0) {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Patch contents in this patch instance.
        /// </summary>
        internal void Patch() {
            if (!IsEnabled) {
                foreach (MethodInfo method in PatchTargetMethods) {
                    List<HarmonyMethod> hardcodedMethods = new List<HarmonyMethod>();

                    foreach (string methodName in HardcodedMethodNames) {
                        MethodInfo patchMethod = AccessTools.Method(PatchType, methodName);
                        hardcodedMethods.Add(patchMethod == null ? null : new HarmonyMethod(patchMethod));
                    }

                    HarmonyInstance.Patch(
                        method,
                        hardcodedMethods[0],
                        hardcodedMethods[1],
                        hardcodedMethods[2],
                        hardcodedMethods[3],
                        null);
                }

                IsEnabled = true;
            }
        }

        /// <summary>
        /// Unpatch contents in this patch instance.
        /// </summary>
        internal void Unpatch() {
            if (IsEnabled) {
                foreach (MethodInfo original in PatchTargetMethods) {
                    foreach (MethodInfo patch in PatchType.GetMethods()) {
                        HarmonyInstance.Unpatch(original, patch);
                    }
                }

                IsEnabled = false;
            }
        }

#if DEBUG
        private void PrintPatchDetails()
        {
            MelonLogger.Msg(
                    string.Format(
                    "Patch {0} details:\n" +
                    "Metadata.MinVersion <= GCNS.releaseNumber ({1} <= {2}) is {3}{4}\n" +
                    "Metadata.MaxVersion <= GCNS.releaseNumber ({5} <= {2}) is {6}{7}\n" +
                    "ClassType is {8}\n" +
                    "PatchType is {9}\n" +
                    "PatchTargetMethods count is {10}{11}\n" +
                    "Patch target method name is {12}",
                    // Parameters
                    PatchMetadata.PatchId,
                    PatchMetadata.MinVersion,
                    AdofaiTweaks.ReleaseNumber,
                    PatchMetadata.MinVersion <= AdofaiTweaks.ReleaseNumber || PatchMetadata.MinVersion == -1,
                    PatchMetadata.MinVersion == -1 ? " (-1)" : "",
                    PatchMetadata.MaxVersion,
                    PatchMetadata.MaxVersion >= AdofaiTweaks.ReleaseNumber || PatchMetadata.MaxVersion == -1,
                    PatchMetadata.MaxVersion == -1 ? " (-1)" : "",
                    ClassType,
                    PatchType,
                    PatchTargetMethods?.Count() ?? 0,
                    PatchTargetMethods == null ? " (null)" : "",
                    PatchMetadata.MethodName));
        }
    }
#endif
}
