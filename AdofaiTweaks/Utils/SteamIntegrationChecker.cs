using System.Reflection;
using ADOFAI.SteamIntegration;
using HarmonyLib;

namespace AdofaiTweaks.Utils;

/// <summary>
/// Wrapper class for backwards compatibility with Steam Integration detection.
/// </summary>
public static class SteamIntegrationChecker {
    /// <summary>
    /// Returns true if Steam Integration is available.
    /// </summary>
    /// <returns><c>true</c> if Steam Integration is available.</returns>
    public static bool Check() {
        if (AdofaiTweaks.ReleaseNumber < 147) {
            return OldSteamIntegrationCheck();
        }

        if (!ADOBase.appIsInSteamLibrary)
        {
            return false;
        }

        return SteamController.initialized;
    }

    private static readonly Assembly ExecutingAssembly = typeof(ADOStartup).Assembly;
    private static readonly FieldInfo SteamIntegrationInstanceField =
        AccessTools.Field(ExecutingAssembly.GetType("SteamIntegration"), "Instance");
    private static readonly FieldInfo SteamIntegrationInitializedField =
        AccessTools.Field(ExecutingAssembly.GetType("SteamIntegration"), "initialized");

    private static bool OldSteamIntegrationCheck() {
        var integration = SteamIntegrationInstanceField.GetValue(null);
        if (integration == null) {
            return false;
        }

        return (bool)SteamIntegrationInitializedField.GetValue(integration);
    }
}