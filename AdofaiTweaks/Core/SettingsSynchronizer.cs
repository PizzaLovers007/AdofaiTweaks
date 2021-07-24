using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using AdofaiTweaks.Core.Attributes;
using HarmonyLib;
using MelonLoader;

namespace AdofaiTweaks.Core
{
    /// <summary>
    /// This class helps synchronize <see cref="TweakSettings"/> properties
    /// across the entire codebase. Two things must be done for the synchronizer
    /// to inject the settings:
    /// <list type="number">
    /// <item>
    /// The class type (or an instance of it) that has the
    /// <see cref="TweakSettings"/> property must be registered to the
    /// synchronizer via <see cref="Register(Type)"/> (or
    /// <see cref="Register(object)"/> for the instance).
    /// </item>
    /// <item>
    /// The <see cref="TweakSettings"/> property in the class must have the
    /// <see cref="SyncTweakSettingsAttribute"/> attribute.
    /// </item>
    /// </list>
    /// </summary>
    internal class SettingsSynchronizer
    {
        private readonly IDictionary<Type, TweakSettings> tweakSettingsDictionary =
            new Dictionary<Type, TweakSettings>();

        private readonly IDictionary<Type, object> registeredObjects =
            new Dictionary<Type, object>();

        /// <summary>
        /// Loads an instance of every <see cref="TweakSettings"/> type from the
        /// saved settings files.
        /// </summary>
        /// <param name="modEntry">The UMM mod entry for AdofaiTweaks.</param>
        public void Load() {
            tweakSettingsDictionary.Clear();
            MethodInfo loadMethod =
                typeof(UnityModManager.ModSettings).GetMethod(
                    nameof(UnityModManager.ModSettings.Load),
                    AccessTools.all,
                    null,
                    new Type[] { typeof(UnityModManager.ModEntry) },
                    null);
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes()) {
                if (!type.IsSubclassOf(typeof(TweakSettings))) {
                    continue;
                }
                modEntry.Logger.Log(string.Format("Loading: {0}", type.FullName));
                MethodInfo genericLoadMethod = loadMethod.MakeGenericMethod(type);
                try {
                    tweakSettingsDictionary[type] =
                        (TweakSettings)genericLoadMethod.Invoke(
                            null, new object[] { modEntry });
                } catch (Exception e) {
                    AdofaiTweaks.Logger.Error(
                        string.Format(
                            "Failed to read settings for {0}: {1}.", type.FullName, e));
                    ConstructorInfo constructor = type.GetConstructor(null);
                    tweakSettingsDictionary[type] =
                        (TweakSettings)constructor.Invoke(null);
                }
            }
        }

        /// <summary>
        /// Saves every <see cref="TweakSettings"/> instance to their respective
        /// files.
        /// </summary>
        /// <param name="modEntry">The UMM mod entry for AdofaiTweaks.</param>
        public void Save(UnityModManager.ModEntry modEntry) {
            foreach (Type type in tweakSettingsDictionary.Keys) {
                modEntry.Logger.Log("Saving: " + type.FullName);
                tweakSettingsDictionary[type].Save(modEntry);
            }
        }

        /// <summary>
        /// Injects the loaded <see cref="TweakSettings"/> instances into all
        /// the registered types/objects.
        /// </summary>
        public void Sync() {
            foreach (Type type in registeredObjects.Keys) {
                ApplySettingsTo(type, registeredObjects[type]);
            }
        }

        /// <summary>
        /// Gets the <see cref="TweakSettings"/> instance for the given
        /// <see cref="Tweak"/> type.
        /// </summary>
        /// <param name="tweakType">The type of the tweak.</param>
        /// <returns>
        /// The <see cref="TweakSettings"/> instance for the given
        /// <see cref="Tweak"/> type.
        /// </returns>
        public TweakSettings GetSettingsForType(Type tweakType) {
            return tweakSettingsDictionary[tweakType];
        }

        /// <summary>
        /// Registers the given type to the synchronizer.
        /// </summary>
        /// <param name="type">The type to register.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the type is already registered.
        /// </exception>
        public void Register(Type type) {
            Register(type, null);
        }

        /// <summary>
        /// Registers the given object to the synchronizer.
        /// </summary>
        /// <param name="obj">The object to register.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when another object of this type is already registered.
        /// </exception>
        public void Register(object obj) {
            Register(obj.GetType(), obj);
        }

        private void Register(Type type, object obj) {
            if (registeredObjects.ContainsKey(type)) {
                throw new ArgumentException(
                    string.Format(
                        "An object of type {0} has already been registered to " +
                        "SettingsSynchronizer. Please only register one object of every type to " +
                        "the synchronizer.",
                        type.FullName));
            }
            registeredObjects[type] = obj;
        }

        /// <summary>
        /// Unregisters the given type from the synchronizer.
        /// </summary>
        /// <param name="type">The type to unregister.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the type is not registered.
        /// </exception>
        public void Unregister(Type type) {
            Unregister(type, null);
        }

        /// <summary>
        /// Unregisters the given object from the synchronizer.
        /// </summary>
        /// <param name="obj">The object to unregister.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the object is not registered.
        /// </exception>
        public void Unregister(object obj) {
            Unregister(obj.GetType(), obj);
        }

        private void Unregister(Type type, object obj) {
            if (!registeredObjects.ContainsKey(type)) {
                throw new ArgumentException(
                    string.Format(
                        "No object of type {0} is registered in SettingsSynchronizer. This is " +
                        "most likely due to a misconfiguration. Please ensure you are " +
                        "registering the object correctly.",
                        type.FullName));
            }
            if (registeredObjects[type] != obj) {
                throw new ArgumentException(
                    string.Format(
                        "The registered object of type {0} differs from the object trying to be " +
                        "unregistered. Please ensure you are unregistering the correct object.",
                        type.FullName));
            }
            registeredObjects.Remove(type);
        }

        private void ApplySettingsTo(Type type, object obj = null) {
            foreach (PropertyInfo prop in type.GetProperties(AccessTools.all)) {
                if (prop.GetCustomAttribute<SyncTweakSettingsAttribute>() == null) {
                    continue;
                }
                try {
                    prop.SetValue(obj, tweakSettingsDictionary[prop.GetUnderlyingType()]);
                } catch (Exception e) {
                    AdofaiTweaks.Logger.Error(GenerateMessage(type, obj, prop, e));
                }
            }
        }

        private string GenerateMessage(
            Type patchType, object obj, PropertyInfo tweakProp, Exception e) {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(
                "Unable to update property {0} in object {1} (type is {2}).\n",
                tweakProp.Name,
                obj,
                patchType.FullName);
            sb.AppendFormat("Exception: {0}", e);
            return sb.ToString();
        }
    }
}
