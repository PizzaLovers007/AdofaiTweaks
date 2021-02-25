using System;

namespace AdofaiTweaks.Core.Attributes
{
    /// <summary>
    /// Attribute that signals the <see cref="SettingsSynchronizer"/> to keep
    /// this property set to the latest tweak settings.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    internal class SyncTweakSettingsAttribute : Attribute
    {
    }
}
