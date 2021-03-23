namespace AdofaiTweaks.Core
{
    /// <summary>
    /// The base class for all tweaks. This class has several lifecycle events
    /// that can be overridden.
    /// </summary>
    public abstract class Tweak
    {
        /// <summary>
        /// The name of the tweak, as shown in the settings GUI.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// The description of the tweak, as shown in the settings GUI.
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// Called when tweak's settings GUI is updated per frame.
        /// </summary>
        public virtual void OnSettingsGUI() { }

        /// <summary>
        /// Called when UMM's settings GUI is hidden.
        /// </summary>
        public virtual void OnHideGUI() { }

        /// <summary>
        /// Called when the tweak is enabled. Runs before Harmony patches the
        /// methods for this tweak.
        /// </summary>
        public virtual void OnEnable() { }

        /// <summary>
        /// Called after Harmony patches the methods for this tweak when the
        /// tweak is enabled.
        /// </summary>
        public virtual void OnPatch() { }

        /// <summary>
        /// Called when the tweak is disabled. Runs before Harmony unpatches the
        /// methods for this tweak.
        /// </summary>
        public virtual void OnDisable() { }

        /// <summary>
        /// Called after Harmony unpatches the methods for this tweak when the
        /// tweak is disabled.
        /// </summary>
        public virtual void OnUnpatch() { }

        /// <summary>
        /// Called when the game updates per frame.
        /// </summary>
        /// <param name="deltaTime">
        /// The amount of time that has passed since the previous frame in
        /// seconds.
        /// </param>
        public virtual void OnUpdate(float deltaTime) { }

        /// <summary>
        /// Called after the language for AdofaiTweaks is changed.
        /// </summary>
        public virtual void OnLanguageChange() { }
    }
}
