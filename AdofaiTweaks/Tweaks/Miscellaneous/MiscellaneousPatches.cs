using AdofaiTweaks.Core.Attributes;
using HarmonyLib;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.Miscellaneous
{
    internal static class MiscellaneousPatches
    {
        [SyncTweakSettings]
        private static MiscellaneousSettings Settings { get; set; }

        [HarmonyPatch(typeof(CameraFilterPack_FX_Glitch1), "OnRenderImage")]
        private static class GlitchOnRenderImagePatch
        {
            // Flip happens in these time ranges:
            private static readonly float[,] RANGES = {
                { 9f, 9.125f },
                { 11.125f, 11.25f },
                { 23.25f, 23.735f },
                { 39f, 39.125f },
                { 45.25f, 45.375f },
                { 78.625f, 78.75f },
                { 78.875f, 79f },
                { 87.75f, 87.875f },
            };

            public static void Prefix(ref float ___TimeX) {
                if (!AdofaiTweaks.IsEnabled || !Settings.IsEnabled || !Settings.DisableGlitchFlip) {
                    return;
                }
                ___TimeX += Time.deltaTime;
                for (int i = 0; i < RANGES.GetLength(0); i++) {
                    if (RANGES[i, 0] - 0.01 <= ___TimeX && ___TimeX < RANGES[i, 1] + 0.01f) {
                        ___TimeX += 0.15f;
                    }
                }
                ___TimeX -= Time.deltaTime;
            }
        }

        [HarmonyPatch(typeof(scnEditor), "Update")]
        private static class EditorUpdatePatch
        {
            private static bool scrollEventInside;

            public static void Prefix(scnEditor __instance) {
                scrollEventInside = ScrollEvent.inside;
                if (!AdofaiTweaks.IsEnabled || !Settings.IsEnabled || !Settings.DisableEditorZoom) {
                    return;
                }
                if (!__instance.isLevelEditor || __instance.controller.paused) {
                    return;
                }
                ScrollEvent.inside = true;
            }

            public static void Postfix() {
                ScrollEvent.inside = scrollEventInside;
            }
        }
    }
}
