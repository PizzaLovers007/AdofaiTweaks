using System.Reflection;
using AdofaiTweaks.Core.Attributes;
using AdofaiTweaks.Utils;
using DG.Tweening;
using HarmonyLib;

using UnityEngine;

namespace AdofaiTweaks.Tweaks.PlanetOpacity;

/// <summary>
/// Patches for the Planet Opacity tweak.
/// </summary>
internal static class PlanetOpacityPatches
{
    [SyncTweakSettings]
    private static PlanetOpacitySettings Settings { get; set; }

    private static readonly MethodInfo SetParticleSystemColorMethod;
    private static readonly MethodInfo SetRingColorMethod;
    private static readonly FieldInfo SpriteField;
    private static readonly PropertyInfo SpriteColorProperty;

    private static readonly bool CanLoadPlanetRenderer = AdofaiTweaks.ReleaseNumber >= 128;

    static PlanetOpacityPatches() {
        if (!CanLoadPlanetRenderer) {
            SetParticleSystemColorMethod = AccessTools.Method(typeof(scrPlanet), "SetParticleSystemColor");
            SetRingColorMethod = AccessTools.Method(typeof(scrPlanet), "SetRingColor");
            SpriteField = AccessTools.Field(typeof(scrPlanet), "sprite");
            SpriteColorProperty = AccessTools.Property(SpriteField.FieldType, "color");
            return;
        }

        SetParticleSystemColorMethod = AccessTools.Method(typeof(PlanetRenderer), "SetParticleSystemColor");
        SetRingColorMethod = AccessTools.Method(typeof(PlanetRenderer), "SetRingColor");
        SpriteField = AccessTools.Field(typeof(PlanetRenderer), "sprite");
        SpriteColorProperty = AccessTools.Property(SpriteField.FieldType, "color");
    }

    private static float CalculateBodyOpacityPost127(PlanetRenderer planet) {
        if (planet.IsRedPlanet()) {
            return Settings.PlanetOpacity1.Body;
        }

        if (planet.IsBluePlanet()) {
            return Settings.PlanetOpacity2.Body;
        }

        return Settings.PlanetOpacity3.Body;
    }

    private static float CalculateTailOpacityPost127(PlanetRenderer planet) {
        if (planet.IsRedPlanet()) {
            return Settings.PlanetOpacity1.Tail;
        }

        if (planet.IsBluePlanet()) {
            return Settings.PlanetOpacity2.Tail;
        }

        return Settings.PlanetOpacity3.Tail;
    }

    private static float CalculateRingOpacityPost127(PlanetRenderer planet) {
        if (planet.IsRedPlanet()) {
            return Settings.PlanetOpacity1.Ring;
        }

        if (planet.IsBluePlanet()) {
            return Settings.PlanetOpacity2.Ring;
        }

        return Settings.PlanetOpacity3.Ring;
    }

    private static float CalculateBodyOpacityPre128(scrPlanet planet) {
        if (planet.IsRedPlanetLegacy()) {
            return Settings.PlanetOpacity1.Body;
        }

        if (planet.IsBluePlanetLegacy()) {
            return Settings.PlanetOpacity2.Body;
        }

        return Settings.PlanetOpacity3.Body;
    }

    private static float CalculateTailOpacityPre128(scrPlanet planet) {
        if (planet.IsRedPlanetLegacy()) {
            return Settings.PlanetOpacity1.Tail;
        }

        if (planet.IsBluePlanetLegacy()) {
            return Settings.PlanetOpacity2.Tail;
        }

        return Settings.PlanetOpacity3.Tail;
    }

    private static float CalculateRingOpacityPre128(scrPlanet planet) {
        if (planet.IsRedPlanetLegacy()) {
            return Settings.PlanetOpacity1.Ring;
        }

        if (planet.IsBluePlanetLegacy()) {
            return Settings.PlanetOpacity2.Ring;
        }

        return Settings.PlanetOpacity3.Ring;
    }

    private static Color ApplyOpacity(Color color, float opacity) {
        float alpha = color.a * opacity / 100;
        return color.WithAlpha(alpha);
    }

    private static void SetSpriteColor(object planet, Color color) {
        SpriteColorProperty.SetValue(SpriteField.GetValue(planet), color);
    }

    private static Color GetSpriteColor(object planet) {
        return (Color)SpriteColorProperty.GetValue(SpriteField.GetValue(planet));
    }

    [TweakPatch("PlanetOpacityPatches.PlanetRendererSetPlanetColorPatch", "PlanetRenderer", "SetPlanetColor")]
    private static class PlanetRendererSetPlanetColorPatch
    {
        public static void Postfix(PlanetRenderer __instance, Color color) {
            float opacity = CalculateBodyOpacityPost127(__instance);
            ParticleSystem.MainModule psmain = __instance.sparks.main;
            psmain.startColor = ApplyOpacity(psmain.startColor.color, opacity);
            SetSpriteColor(__instance, ApplyOpacity(color, opacity));
        }
    }

    [TweakPatch("PlanetOpacityPatches.PlanetRendererSetCoreColorPatch", "PlanetRenderer", "SetCoreColor")]
    private static class PlanetRendererSetCoreColorPatch
    {
        public static void Postfix(PlanetRenderer __instance, Color color) {
            float opacity = CalculateBodyOpacityPost127(__instance);
            __instance.glow.color = ApplyOpacity(__instance.glow.color, opacity);
            ParticleSystem.MainModule psmain = __instance.coreParticles.main;
            psmain.startColor = new ParticleSystem.MinMaxGradient(ApplyOpacity(color, opacity));
            SetParticleSystemColorMethod.Invoke(
                __instance,
                new object[] {
                    __instance.coreParticles,
                    ApplyOpacity(color, opacity),
                    ApplyOpacity(color, opacity),
                });
        }
    }

    [TweakPatch("PlanetOpacityPatches.PlanetRendererSetTailColorPatch", "PlanetRenderer", "SetTailColor")]
    private static class PlanetRendererSetTailColorPatch
    {
        public static void Postfix(PlanetRenderer __instance, Color color) {
            float opacity = CalculateTailOpacityPost127(__instance);
            SetParticleSystemColorMethod.Invoke(
                __instance,
                new object[] {
                    __instance.tailParticles,
                    ApplyOpacity(color, opacity),
                    ApplyOpacity(color, opacity) * new Color(0.5f, 0.5f, 0.5f),
                });

            var main = __instance.deathExplosion.main;
            var gradient = new Gradient();
            gradient.colorKeys = [
                new (color, 0.0f),
                new (color, 0.5f),
                new (color, 1f)
            ];
            gradient.alphaKeys = [
                new (opacity, 0.0f),
                new (opacity, 1f)
            ];
            gradient.mode = GradientMode.Fixed;
            main.startColor = new ParticleSystem.MinMaxGradient(gradient) {
                mode = ParticleSystemGradientMode.RandomColor,
            };
        }
    }

    [TweakPatch("PlanetOpacityPatches.PlanetRendererSetRingColorPatch", "PlanetRenderer", "SetRingColor")]
    private static class PlanetRendererSetRingColorPatch
    {
        public static void Postfix(PlanetRenderer __instance) {
            float opacity = CalculateRingOpacityPost127(__instance);
            __instance.ringComp.color = ApplyOpacity(__instance.ringComp.color, opacity);
        }
    }

    [TweakPatch("PlanetOpacityPatches.PlanetRendererSetFaceColorPatch", "PlanetRenderer", "SetFaceColor")]
    private static class PlanetRendererSetFaceColorPatch
    {
        public static void Postfix(PlanetRenderer __instance) {
            float opacity = CalculateBodyOpacityPost127(__instance);
            __instance.faceSprite.color = ApplyOpacity(__instance.faceSprite.color, opacity);
        }
    }

    [TweakPatch("PlanetOpacityPatches.PlanetRendererDisableCustomColorPatch", "PlanetRenderer", "DisableCustomColor")]
    private static class PlanetRendererDisableCustomColorPatch
    {
        public static void Postfix(PlanetRenderer __instance) {
            float opacity = CalculateBodyOpacityPost127(__instance);
            SetSpriteColor(__instance, ApplyOpacity(Color.white, opacity));
        }
    }

    [TweakPatch("PlanetOpacityPatches.PlanetRendererToggleSpecialPlanetPatch", "PlanetRenderer", "ToggleSpecialPlanet")]
    private static class PlanetRendererToggleSpecialPlanetPatch
    {
        public static void Postfix(PlanetRenderer __instance, bool on) {
            if (!on) {
                return;
            }
            float bodyAlpha = CalculateBodyOpacityPost127(__instance) / 100;
            float tailAlpha = CalculateTailOpacityPost127(__instance) / 100;
            float ringAlpha = CalculateRingOpacityPost127(__instance) / 100;
            ParticleSystem.MainModule psmain;
            psmain = __instance.tailParticles.main;
            psmain.startColor = psmain.startColor.color.WithAlpha(tailAlpha);
            psmain = __instance.sparks.main;
            psmain.startColor = psmain.startColor.color.WithAlpha(bodyAlpha);
            psmain = __instance.coreParticles.main;
            psmain.startColor = psmain.startColor.color.WithAlpha(bodyAlpha);
            __instance.ringComp.color = __instance.ringComp.color.WithAlpha(ringAlpha * 0.4f);
            __instance.glow.color = __instance.glow.color.WithAlpha(bodyAlpha * 0.5f);
            SetSpriteColor(__instance, GetSpriteColor(__instance).WithAlpha(bodyAlpha));
        }
    }

    [TweakPatch(
        "PlanetOpacityPatches.SetRainbowPatchPost127",
        "PlanetRenderer",
        "LateUpdate")]
    private static class SetRainbowPatchPost127
    {
        public static void Postfix(PlanetRenderer __instance) {
            if (__instance.rainbow && __instance.ring != null) {
                Color color = Color.HSVToRGB(PlanetRenderer.rainbowHue, 1f, 1f);
                float opacity = CalculateBodyOpacityPost127(__instance);
                SetSpriteColor(__instance, ApplyOpacity(color, opacity));
            }
        }
    }

    [TweakPatch(
        "PlanetOpacityPatches.SwitchToOverseerPost114",
        "scrPlanet",
        "SwitchToOverseer")]
    private static class SwitchToOverseerPost114
    {
        public static void Postfix(scrPlanet __instance) {
            float opacity = CalculateBodyOpacityPre128(__instance);
            SetSpriteColor(__instance, ApplyOpacity(GetSpriteColor(__instance), opacity));
        }
    }

    [TweakPatch(
        "PlanetOpacityPatches.SwitchToOverseerPost127",
        "PlanetRenderer",
        "SwitchToOverseer")]
    private static class SwitchToOverseerPost127
    {
        public static void Postfix(PlanetRenderer __instance) {
            float opacity = CalculateBodyOpacityPost127(__instance);
            SetSpriteColor(__instance, ApplyOpacity(GetSpriteColor(__instance), opacity));
        }
    }
}
