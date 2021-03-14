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
        /// Handler for the tweak's settings GUI.
        /// </summary>
        public virtual void OnSettingsGUI() { }

        /// <summary>
        /// Handler for when UMM's setttngs GUI is hidden.
        /// </summary>
        public virtual void OnHideGUI() { }

        /// <summary>
        /// Handler for when the tweak is enabled.
        /// </summary>
        public virtual void OnEnable() { }

        /// <summary>
        /// Handler for when the tweak is disabled.
        /// </summary>
        public virtual void OnDisable() { }

        /// <summary>
        /// Handler for UMM's update event.
        /// </summary>
        /// <param name="deltaTime">
        /// The amount of time that has passed since the previous frame in
        /// seconds.
        /// </param>
        public virtual void OnUpdate(float deltaTime) { }

        /// <summary>
        /// Handler for changing the language of AdofaiTweaks.
        /// </summary>
        public virtual void OnLanguageChange() { }
    }
}
