using System;
using HarmonyLib;

namespace AdofaiTweaks.Core.Attributes
{
    /// <summary>
    /// Replaces <see cref="HarmonyPatch"/> and prevents mod crashing from having no class specified in the game's code.
    /// </summary>
    public class TweakPatchAttribute : Attribute
    {
        /// <summary>
        /// Id of patch, it should <i>not</i> be identical to other patches' id.
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
    }
}
