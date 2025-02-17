using System.Reflection;
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
        if (AdofaiTweaks.ReleaseNumber < 131) {
            return OldSteamIntegrationCheck();
        }

        return SteamIntegration.initialized;
    }

    private static readonly FieldInfo SteamIntegrationInstanceField =
        AccessTools.Field(typeof(SteamIntegration), "Instance");
    private static readonly FieldInfo SteamIntegrationInitializedField =
        AccessTools.Field(typeof(SteamIntegration), "initialized");

    private static bool OldSteamIntegrationCheck() {
        var integration = (SteamIntegration)SteamIntegrationInstanceField.GetValue(null);
        if (integration == null) {
            return false;
        }

        return (bool)SteamIntegrationInitializedField.GetValue(integration);
    }
}