using System.Reflection;
using AdofaiTweaks.Core.Attributes;
using AdofaiTweaks.Utils;
using DG.Tweening;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.PlanetOpacity
{
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

            return (Settings.PlanetOpacity1.Body + Settings.PlanetOpacity2.Body) / 2;
        }

        private static float CalculateTailOpacityPost127(PlanetRenderer planet) {
            if (planet.IsRedPlanet()) {
                return Settings.PlanetOpacity1.Tail;
            }

            if (planet.IsBluePlanet()) {
                return Settings.PlanetOpacity2.Tail;
            }

            return (Settings.PlanetOpacity1.Tail + Settings.PlanetOpacity2.Tail) / 2;
        }

        private static float CalculateRingOpacityPost127(PlanetRenderer planet) {
            if (planet.IsRedPlanet())
            {
                return Settings.PlanetOpacity1.Ring;
            }

            if (planet.IsBluePlanet())
            {
                return Settings.PlanetOpacity2.Ring;
            }

            return (Settings.PlanetOpacity1.Ring + Settings.PlanetOpacity2.Ring) / 2;
        }

        private static float CalculateBodyOpacityPre128(scrPlanet planet) {
            if (planet.IsRedPlanetLegacy()) {
                return Settings.PlanetOpacity1.Body;
            }

            if (planet.IsBluePlanetLegacy()) {
                return Settings.PlanetOpacity2.Body;
            }

            return (Settings.PlanetOpacity1.Body + Settings.PlanetOpacity2.Body) / 2;
        }

        private static float CalculateTailOpacityPre128(scrPlanet planet) {
            if (planet.IsRedPlanetLegacy()) {
                return Settings.PlanetOpacity1.Tail;
            }

            if (planet.IsBluePlanetLegacy()) {
                return Settings.PlanetOpacity2.Tail;
            }

            return (Settings.PlanetOpacity1.Tail + Settings.PlanetOpacity2.Tail) / 2;
        }

        private static float CalculateRingOpacityPre128(scrPlanet planet) {
            if (planet.IsRedPlanetLegacy())
            {
                return Settings.PlanetOpacity1.Ring;
            }

            if (planet.IsBluePlanetLegacy())
            {
                return Settings.PlanetOpacity2.Ring;
            }

            return (Settings.PlanetOpacity1.Ring + Settings.PlanetOpacity2.Ring) / 2;
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

        [TweakPatch("PlanetOpacityPatches.PlanetRendererSetPlanetColorPatch", "PlanetRenderer", "SetPlanetColor", minVersion: 128)]
        private static class PlanetRendererSetPlanetColorPatch
        {
            public static void Postfix(PlanetRenderer __instance, Color color) {
                float opacity = CalculateBodyOpacityPost127(__instance);
                ParticleSystem.MainModule psmain = __instance.sparks.main;
                psmain.startColor = ApplyOpacity(psmain.startColor.color, opacity);
                SetSpriteColor(__instance, ApplyOpacity(color, opacity));
            }
        }

        [TweakPatch("PlanetOpacityPatches.ScrPlanetSetPlanetColorPatch", "scrPlanet", "SetPlanetColor", maxVersion: 127)]
        private static class ScrPlanetSetPlanetColorPatch {
            private static readonly FieldInfo SparksField = AccessTools.Field(typeof(scrPlanet), "sparks");

            public static void Postfix(scrPlanet __instance, Color color) {
                float opacity = CalculateBodyOpacityPre128(__instance);
                ParticleSystem.MainModule psmain = ((ParticleSystem)SparksField.GetValue(__instance)).main;
                psmain.startColor = ApplyOpacity(psmain.startColor.color, opacity);
                SetSpriteColor(__instance, ApplyOpacity(color, opacity));
            }
        }

        [TweakPatch("PlanetOpacityPatches.PlanetRendererSetCoreColorPatch", "PlanetRenderer", "SetCoreColor", minVersion: 128)]
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

        [TweakPatch("PlanetOpacityPatches.ScrPlanetSetCoreColorPatch", "scrPlanet", "SetCoreColor", maxVersion: 127)]
        private static class ScrPlanetSetCoreColorPatch
        {
            private static readonly FieldInfo GlowField = AccessTools.Field(typeof(scrPlanet), "glow");
            private static readonly FieldInfo CoreParticlesField = AccessTools.Field(typeof(scrPlanet), "coreParticles");

            public static void Postfix(scrPlanet __instance, Color color) {
                float opacity = CalculateBodyOpacityPre128(__instance);
                var glow = (SpriteRenderer)GlowField.GetValue(__instance);
                glow.color = ApplyOpacity(glow.color, opacity);
                var coreParticles = (ParticleSystem)CoreParticlesField.GetValue(__instance);
                ParticleSystem.MainModule psmain = coreParticles.main;
                psmain.startColor = new ParticleSystem.MinMaxGradient(ApplyOpacity(color, opacity));
                SetParticleSystemColorMethod.Invoke(
                    __instance,
                    new object[] {
                        coreParticles,
                        ApplyOpacity(color, opacity),
                        ApplyOpacity(color, opacity),
                    });
            }
        }

        [TweakPatch("PlanetOpacityPatches.PlanetRendererSetTailColorPatch", "PlanetRenderer", "SetTailColor", minVersion: 128)]
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
                gradient.SetKeys(
                [
                    new (color, 0.0f),
                    new (color, 0.5f),
                    new (color, 1f)
                ], [
                    new (opacity, 0.0f),
                    new (opacity, 1f)
                ]);
                gradient.mode = GradientMode.Fixed;
                main.startColor = new ParticleSystem.MinMaxGradient(gradient)
                {
                    mode = ParticleSystemGradientMode.RandomColor,
                };
            }
        }

        [TweakPatch("PlanetOpacityPatches.ScrPlanetSetTailColorPatch", "scrPlanet", "SetTailColor", maxVersion: 127)]
        private static class ScrPlanetSetTailColorPatch
        {
            private static readonly FieldInfo TailParticlesField = AccessTools.Field(typeof(scrPlanet), "tailParticles");

            // private static readonly MethodInfo SetExplosionColorMethod =
            //     AccessTools.Method(typeof(scrPlanet), "SetExplosionColor");

            private static readonly FieldInfo DeathExplosionField =
                AccessTools.Field(typeof(scrPlanet), "deathExplosion");

            public static void Postfix(scrPlanet __instance, Color color) {
                float opacity = CalculateTailOpacityPre128(__instance);
                SetParticleSystemColorMethod.Invoke(
                    __instance,
                    [
                        TailParticlesField.GetValue(__instance),
                        ApplyOpacity(color, opacity),
                        ApplyOpacity(color, opacity) * new Color(0.5f, 0.5f, 0.5f)
                    ]);

                var main = ((ParticleSystem)DeathExplosionField.GetValue(__instance)).main;
                var gradient = new Gradient();
                gradient.SetKeys(
                [
                    new (color, 0.0f),
                    new (color, 0.5f),
                    new (color, 1f)
                ], [
                    new (opacity, 0.0f),
                    new (opacity, 1f)
                ]);
                gradient.mode = GradientMode.Fixed;
                main.startColor = new ParticleSystem.MinMaxGradient(gradient)
                {
                    mode = ParticleSystemGradientMode.RandomColor,
                };

                // SetExplosionColorMethod.Invoke(__instance, [ApplyOpacity(color, opacity), default(Color)]);
            }
        }

        [TweakPatch("PlanetOpacityPatches.PlanetRendererSetRingColorPatch", "PlanetRenderer", "SetRingColor", minVersion: 128)]
        private static class PlanetRendererSetRingColorPatch
        {
            public static void Postfix(PlanetRenderer __instance) {
                float opacity = CalculateRingOpacityPost127(__instance);
                __instance.ring.color = ApplyOpacity(__instance.ring.color, opacity);
            }
        }

        [TweakPatch("PlanetOpacityPatches.ScrPlanetSetRingColorPatch", "scrPlanet", "SetRingColor", maxVersion: 127)]
        private static class ScrPlanetSetRingColorPatch
        {
            private static readonly FieldInfo RingField = AccessTools.Field(typeof(scrPlanet), "ring");

            public static void Postfix(scrPlanet __instance) {
                float opacity = CalculateRingOpacityPre128(__instance);
                var ring = (SpriteRenderer)RingField.GetValue(__instance);
                ring.color = ApplyOpacity(ring.color, opacity);
            }
        }

        [TweakPatch("PlanetOpacityPatches.PlanetRendererSetFaceColorPatch", "PlanetRenderer", "SetFaceColor", minVersion: 128)]
        private static class PlanetRendererSetFaceColorPatch
        {
            public static void Postfix(PlanetRenderer __instance) {
                float opacity = CalculateBodyOpacityPost127(__instance);
                __instance.faceSprite.color = ApplyOpacity(__instance.faceSprite.color, opacity);
            }
        }

        [TweakPatch("PlanetOpacityPatches.ScrPlanetSetFaceColorPatch", "scrPlanet", "SetFaceColor", maxVersion: 127)]
        private static class ScrPlanetSetFaceColorPatch
        {
            private static readonly FieldInfo FaceSpriteField = AccessTools.Field(typeof(scrPlanet), "faceSprite");

            public static void Postfix(scrPlanet __instance) {
                float opacity = CalculateBodyOpacityPre128(__instance);
                var faceSprite = (SpriteRenderer)FaceSpriteField.GetValue(__instance);
                faceSprite.color = ApplyOpacity(faceSprite.color, opacity);
            }
        }

        [TweakPatch("PlanetOpacityPatches.PlanetRendererDisableCustomColorPatch", "PlanetRenderer", "DisableCustomColor", minVersion: 128)]
        private static class PlanetRendererDisableCustomColorPatch
        {
            public static void Postfix(PlanetRenderer __instance) {
                float opacity = CalculateBodyOpacityPost127(__instance);
                SetSpriteColor(__instance, ApplyOpacity(Color.white, opacity));
            }
        }

        [TweakPatch("PlanetOpacityPatches.ScrPlanetDisableCustomColorPatch", "scrPlanet", "DisableCustomColor", maxVersion: 127)]
        private static class ScrPlanetDisableCustomColorPatch
        {
            public static void Postfix(scrPlanet __instance) {
                float opacity = CalculateBodyOpacityPre128(__instance);
                SetSpriteColor(__instance, ApplyOpacity(Color.white, opacity));
            }
        }

        [TweakPatch("PlanetOpacityPatches.PlanetRendererToggleSpecialPlanetPatch", "PlanetRenderer", "ToggleSpecialPlanet", minVersion: 128)]
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
                __instance.ring.color = __instance.ring.color.WithAlpha(ringAlpha * 0.4f);
                __instance.glow.color = __instance.glow.color.WithAlpha(bodyAlpha * 0.5f);
                SetSpriteColor(__instance, GetSpriteColor(__instance).WithAlpha(bodyAlpha));
            }
        }

        [TweakPatch("PlanetOpacityPatches.ScrPlanetToggleSpecialPlanetPatch", "scrPlanet", "ToggleSpecialPlanet", maxVersion: 127)]
        private static class ScrPlanetToggleSpecialPlanetPatch
        {
            private static readonly FieldInfo TailParticlesField = AccessTools.Field(typeof(scrPlanet), "tailParticles");
            private static readonly FieldInfo SparksField = AccessTools.Field(typeof(scrPlanet), "sparks");
            private static readonly FieldInfo CoreParticlesField = AccessTools.Field(typeof(scrPlanet), "coreParticles");
            private static readonly FieldInfo RingField = AccessTools.Field(typeof(scrPlanet), "ring");
            private static readonly FieldInfo GlowField = AccessTools.Field(typeof(scrPlanet), "glow");

            public static void Postfix(scrPlanet __instance, bool on) {
                if (!on) {
                    return;
                }
                float bodyAlpha = CalculateBodyOpacityPre128(__instance) / 100;
                float tailAlpha = CalculateTailOpacityPre128(__instance) / 100;
                float ringAlpha = CalculateRingOpacityPre128(__instance) / 100;
                ParticleSystem.MainModule psmain;
                psmain = ((ParticleSystem)TailParticlesField.GetValue(__instance)).main;
                psmain.startColor = psmain.startColor.color.WithAlpha(tailAlpha);
                psmain = ((ParticleSystem)SparksField.GetValue(__instance)).main;
                psmain.startColor = psmain.startColor.color.WithAlpha(bodyAlpha);
                psmain = ((ParticleSystem)CoreParticlesField.GetValue(__instance)).main;
                psmain.startColor = psmain.startColor.color.WithAlpha(bodyAlpha);
                var ring = (SpriteRenderer)RingField.GetValue(__instance);
                var glow = (SpriteRenderer)GlowField.GetValue(__instance);
                ring.color = ring.color.WithAlpha(ringAlpha * 0.4f);
                glow.color = glow.color.WithAlpha(bodyAlpha * 0.5f);
                SetSpriteColor(__instance, GetSpriteColor(__instance).WithAlpha(bodyAlpha));
            }
        }

        [TweakPatch(
            "PlanetOpacityPatches.SetRainbowPatchPre110",
            "scrPlanet",
            "SetRainbow",
            MaxVersion = 109)]
        private static class SetRainbowPatchPre110
        {
            private static readonly FieldInfo PlanetRingField =
                AccessTools.Field(typeof(scrPlanet), "ring");

            private static readonly MethodInfo PlanetSetTailColorMethod =
                AccessTools.Method(typeof(scrPlanet), "SetTailColor");
            private static readonly MethodInfo PlanetSetCoreColorMethod =
                AccessTools.Method(typeof(scrPlanet), "SetCoreColor");

            [HarmonyPriority(Priority.LowerThanNormal)]
            public static bool Prefix(
                scrPlanet __instance, bool enabled, ref Sequence ___rainbowSeq) {
                SpriteRenderer sprite = (SpriteRenderer)SpriteField.GetValue(__instance);
                float bodyOpacity = CalculateBodyOpacityPre128(__instance);
                float tailOpacity = CalculateBodyOpacityPre128(__instance);
                float ringOpacity = CalculateBodyOpacityPre128(__instance);
                if (!enabled) {
                    if (___rainbowSeq != null) {
                        ___rainbowSeq.Kill(false);
                    }
                    return false;
                }
                if (___rainbowSeq != null) {
                    ___rainbowSeq.Kill(false);
                }
                sprite.color = Color.red;
                Color.RGBToHSV(Color.red, out float _, out float s, out float v);
                Tween[] array = new Tween[10];
                ___rainbowSeq = DOTween.Sequence();
                for (int i = 0; i < array.Length; i++) {
                    Color col = ApplyOpacity(Color.HSVToRGB(0.1f + 0.1f * i, s, v), bodyOpacity);
                    array[i] = sprite.DOColor(col, 0.5f);
                    ___rainbowSeq.Append(array[i]);
                }
                ___rainbowSeq.SetLoops(-1, LoopType.Restart).SetUpdate(true);
                Sequence tempSequence = ___rainbowSeq;
                ___rainbowSeq.OnUpdate(() => {
                    if (PlanetRingField.GetValue(__instance) != null) {
                        SetRingColorMethod.Invoke(
                            __instance,
                            new object[] { ApplyOpacity(sprite.color, ringOpacity) });
                        PlanetSetTailColorMethod.Invoke(__instance, [ApplyOpacity(sprite.color, tailOpacity)]);
                        PlanetSetCoreColorMethod.Invoke(__instance, [sprite.color]);
                        return;
                    }
                    tempSequence.Kill(false);
                });
                return false;
            }
        }

        [TweakPatch(
            "PlanetOpacityPatches.SetRainbowPatchPost110",
            "scrPlanet",
            "LateUpdate",
            MinVersion = 110,
            MaxVersion = 127)]
        private static class SetRainbowPatchPost110 {
            private static readonly FieldInfo PlanetRingField =
                AccessTools.Field(typeof(scrPlanet), "ring");

            private static readonly PropertyInfo PlanetRainbowHueProperty =
                AccessTools.Property(typeof(scrPlanet), "rainbowHue");

            public static void Postfix(scrPlanet __instance, bool ___rainbow) {
                if (___rainbow && PlanetRingField.GetValue(__instance) != null) {
                    Color color = Color.HSVToRGB((float)PlanetRainbowHueProperty.GetValue(__instance), 1f, 1f);
                    float opacity = CalculateBodyOpacityPre128(__instance);
                    SetSpriteColor(__instance, ApplyOpacity(color, opacity));
                }
            }
        }

        [TweakPatch(
            "PlanetOpacityPatches.SetRainbowPatchPost127",
            "PlanetRenderer",
            "LateUpdate",
            MinVersion = 128)]
        private static class SetRainbowPatchPost127
        {
            public static void Postfix(PlanetRenderer __instance, bool ___rainbow) {
                if (___rainbow && __instance.ring != null) {
                    Color color = Color.HSVToRGB(PlanetRenderer.rainbowHue, 1f, 1f);
                    float opacity = CalculateBodyOpacityPost127(__instance);
                    SetSpriteColor(__instance, ApplyOpacity(color, opacity));
                }
            }
        }

        [TweakPatch(
            "PlanetOpacityPatches.SwitchToOverseerPost114",
            "scrPlanet",
            "SwitchToOverseer",
            MinVersion = 114)]
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
            "SwitchToOverseer",
            MinVersion = 127)]
        private static class SwitchToOverseerPost127
        {
            public static void Postfix(PlanetRenderer __instance) {
                float opacity = CalculateBodyOpacityPost127(__instance);
                SetSpriteColor(__instance, ApplyOpacity(GetSpriteColor(__instance), opacity));
            }
        }
    }
}
