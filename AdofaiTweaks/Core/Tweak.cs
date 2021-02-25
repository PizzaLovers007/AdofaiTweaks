namespace AdofaiTweaks.Core
{
    public abstract class Tweak
    {
        public abstract string Name { get; }
        public abstract string Description { get; }

        public virtual void OnSettingsGUI() { }

        public virtual void OnHideGUI() { }

        public virtual void OnEnable() { }

        public virtual void OnDisable() { }

        public virtual void OnUpdate(float deltaTime) { }

        public virtual void OnLanguageChange() { }
    }
}
