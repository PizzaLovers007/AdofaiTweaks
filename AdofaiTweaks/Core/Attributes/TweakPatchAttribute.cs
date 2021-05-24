using System;
using HarmonyLib;

namespace AdofaiTweaks.Core.Attributes
{
    /// <summary>
    /// Replaces <see cref="HarmonyPatch"/> and prevents mod crashing from
    /// having no class specified in the game's code.
    /// </summary>
    public class TweakPatchAttribute : Attribute
    {
        /// <summary>
        /// ID of patch, it should <i>not</i> be identical to other patches' ID.
        /// </summary>
        public string PatchId { get; set; }

        /// <summary>
        /// Name of the class to find specific method from.
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// Name of the method in the class to patch.
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// Minimum ADOFAI's version of this patch working.
        /// </summary>
        public int MinVersion { get; set; }

        /// <summary>
        /// Maximum ADOFAI's version of this patch working.
        /// </summary>
        public int MaxVersion { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TweakPatchAttribute"/>
        /// class.
        /// </summary>
        /// <param name="patchId">
        /// The ID for the patch. No duplicates IDs are allowed.
        /// </param>
        /// <param name="className">The class name to patch.</param>
        /// <param name="methodName">The method name to patch.</param>
        /// <param name="minVersion">
        /// Minimum ADOFAI version required (defaults to -1).
        /// </param>
        /// <param name="maxVersion">
        /// Maximum ADOFAI version required (defaults to -1).
        /// </param>
        public TweakPatchAttribute(
            string patchId,
            string className,
            string methodName,
            int minVersion = -1,
            int maxVersion = -1) {
            PatchId = patchId;
            ClassName = className;
            MethodName = methodName;
            MinVersion = minVersion;
            MaxVersion = maxVersion;
        }
    }
}
