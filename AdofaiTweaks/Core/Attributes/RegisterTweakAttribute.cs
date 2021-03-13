using System;

namespace AdofaiTweaks.Core.Attributes
{
    /// <summary>
    /// Registers a <see cref="Tweak"/> to be by the AdofaiTweaks framework.
    /// </summary>
    public class RegisterTweakAttribute : Attribute
    {
        /// <summary>
        /// The ID of the tweak. This will be used by Harmony to identify the
        /// patches in the provided <see cref="PatchesType"/>.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// The priority that determines where in the tweak list it shows up.
        /// Lower priority tweaks will appear first, and ties are broken by
        /// alphabetical order.
        /// </summary>
        public int Priority { get; private set; }

        /// <summary>
        /// The <see cref="Type"/> of the <see cref="TweakSettings"/> object
        /// used.
        /// </summary>
        public Type SettingsType { get; private set; }

        /// <summary>
        /// The <see cref="Type"/> of the class storing all patches for the
        /// tweak.
        /// </summary>
        public Type PatchesType { get; private set; }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="RegisterTweakAttribute"/> class.
        /// </summary>
        /// <param name="id">The ID of the tweak.</param>
        /// <param name="settingsType">
        /// The <see cref="TweakSettings"/> type that the tweak uses.
        /// </param>
        /// <param name="patchesType">
        /// The type that holds all the <see cref="HarmonyLib.Harmony"/>
        /// patches.
        /// </param>
        /// <param name="priority">
        /// Determines the ordering of the tweaks in the settings GUI. Lower
        /// numbers indicate an earlier position.
        /// </param>
        public RegisterTweakAttribute(
            string id, Type settingsType, Type patchesType, int priority = 0) {
            Id = id;
            Priority = priority;
            SettingsType = settingsType;
            PatchesType = patchesType;
        }
    }
}
