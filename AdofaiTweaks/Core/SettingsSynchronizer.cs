using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using AdofaiTweaks.Core.Attributes;
using HarmonyLib;
using UnityModManagerNet;

namespace AdofaiTweaks.Core
{
    internal class SettingsSynchronizer
    {
        private readonly IDictionary<Type, TweakSettings> tweakSettingsDictionary =
            new Dictionary<Type, TweakSettings>();

        private readonly IDictionary<Type, object> registeredObjects =
            new Dictionary<Type, object>();

        public void Load(UnityModManager.ModEntry modEntry) {
            tweakSettingsDictionary.Clear();
            MethodInfo loadMethod =
                typeof(UnityModManager.ModSettings).GetMethod(
                    nameof(UnityModManager.ModSettings.Load),
                    AccessTools.all,
                    null,
                    new Type[] { typeof(UnityModManager.ModEntry) },
                    null);
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach (Type type in assembly.GetTypes()) {
                    if (type.IsSubclassOf(typeof(TweakSettings))) {
                        modEntry.Logger.Log("Loading: " + type.FullName);
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
            }
        }

        public void Save(UnityModManager.ModEntry modEntry) {
            foreach (Type type in tweakSettingsDictionary.Keys) {
                modEntry.Logger.Log("Saving: " + type.FullName);
                tweakSettingsDictionary[type].Save(modEntry);
            }
        }

        public void Sync() {
            foreach (Type type in registeredObjects.Keys) {
                ApplySettingsTo(type, registeredObjects[type]);
            }
        }

        public TweakSettings GetSettingsForType(Type tweakType) {
            return tweakSettingsDictionary[tweakType];
        }

        public void Register(Type type) {
            Register(type, null);
        }

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

        public void Unregister(Type type) {
            Unregister(type, null);
        }

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
