using System.Reflection;
using AdofaiTweaks.Core.Attributes;
using DG.Tweening;
using HarmonyLib;
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

        private static readonly MethodInfo setParticleSystemColorMethod =
            AccessTools.Method(typeof(scrPlanet), "SetParticleSystemColor");

        private static readonly MethodInfo setRingColorMethod =
            AccessTools.Method(typeof(scrPlanet), "SetRingColor");

        private static float CalculateBodyOpacity(scrPlanet planet) {
            if (planet == scrController.instance.redPlanet) {
                return Settings.PlanetOpacity1.Body;
            } else if (planet == scrController.instance.bluePlanet) {
                return Settings.PlanetOpacity2.Body;
            } else {
                return (Settings.PlanetOpacity1.Body + Settings.PlanetOpacity2.Body) / 2;
            }
        }

        private static float CalculateTailOpacity(scrPlanet planet) {
            if (planet == scrController.instance.redPlanet) {
                return Settings.PlanetOpacity1.Tail;
            } else if (planet == scrController.instance.bluePlanet) {
                return Settings.PlanetOpacity2.Tail;
            } else {
                return (Settings.PlanetOpacity1.Tail + Settings.PlanetOpacity2.Tail) / 2;
            }
        }

        private static float CalculateRingOpacity(scrPlanet planet) {
            if (planet == scrController.instance.redPlanet) {
                return Settings.PlanetOpacity1.Ring;
            } else if (planet == scrController.instance.bluePlanet) {
                return Settings.PlanetOpacity2.Ring;
            } else {
                return (Settings.PlanetOpacity1.Ring + Settings.PlanetOpacity2.Ring) / 2;
            }
        }

        private static Color ApplyOpacity(Color color, float opacity) {
            float alpha = color.a * opacity / 100;
            return color.WithAlpha(alpha);
        }

        [HarmonyPatch(typeof(scrPlanet), "SetPlanetColor")]
        private static class SetPlanetColorPatch
        {
            public static void Postfix(scrPlanet __instance, Color color) {
                float opacity = CalculateBodyOpacity(__instance);
                ParticleSystem.MainModule psmain = __instance.sparks.main;
                psmain.startColor = ApplyOpacity(psmain.startColor.color, opacity);
                __instance.sprite.color = ApplyOpacity(color, opacity);
            }
        }

        [HarmonyPatch(typeof(scrPlanet), "SetCoreColor")]
        private static class SetCoreColorPatch
        {
            public static void Postfix(scrPlanet __instance, Color color) {
                float opacity = CalculateBodyOpacity(__instance);
                __instance.glow.color = ApplyOpacity(__instance.glow.color, opacity);
                ParticleSystem.MainModule psmain = __instance.coreParticles.main;
                psmain.startColor = new ParticleSystem.MinMaxGradient(ApplyOpacity(color, opacity));
                setParticleSystemColorMethod.Invoke(
                    __instance,
                    new object[] {
                        __instance.coreParticles,
                        ApplyOpacity(color, opacity),
                        ApplyOpacity(color, opacity),
                    });
            }
        }

        [HarmonyPatch(typeof(scrPlanet), "SetTailColor")]
        private static class SetTailColorPatch
        {
            public static void Postfix(scrPlanet __instance, Color color) {
                float opacity = CalculateTailOpacity(__instance);
                setParticleSystemColorMethod.Invoke(
                    __instance,
                    new object[] {
                        __instance.tailParticles,
                        ApplyOpacity(color, opacity),
                        ApplyOpacity(color, opacity) * new Color(0.5f, 0.5f, 0.5f),
                    });
                __instance.SetExplosionColor(ApplyOpacity(color, opacity));
            }
        }

        [HarmonyPatch(typeof(scrPlanet), "SetRingColor")]
        private static class SetRingColorPatch
        {
            public static void Postfix(scrPlanet __instance) {
                float opacity = CalculateRingOpacity(__instance);
                __instance.ring.color = ApplyOpacity(__instance.ring.color, opacity);
            }
        }

        [HarmonyPatch(typeof(scrPlanet), "SetFaceColor")]
        private static class SetFaceColorPatch
        {
            public static void Postfix(scrPlanet __instance) {
                float opacity = CalculateBodyOpacity(__instance);
                __instance.faceSprite.color = ApplyOpacity(__instance.faceSprite.color, opacity);
            }
        }

        [HarmonyPatch(typeof(scrPlanet), "DisableCustomColor")]
        private static class DisableCustomColorPatch
        {
            public static void Postfix(scrPlanet __instance) {
                float opacity = CalculateBodyOpacity(__instance);
                __instance.sprite.color = ApplyOpacity(Color.white, opacity);
            }
        }

        [HarmonyPatch(typeof(scrPlanet), "ToggleSpecialPlanet")]
        private static class ToggleSpecialPlanetPatch
        {
            public static void Postfix(scrPlanet __instance, bool on) {
                if (!on) {
                    return;
                }
                float bodyAlpha = CalculateBodyOpacity(__instance) / 100;
                float tailAlpha = CalculateTailOpacity(__instance) / 100;
                float ringAlpha = CalculateRingOpacity(__instance) / 100;
                ParticleSystem.MainModule psmain;
                psmain = __instance.tailParticles.main;
                psmain.startColor = psmain.startColor.color.WithAlpha(tailAlpha);
                psmain = __instance.sparks.main;
                psmain.startColor = psmain.startColor.color.WithAlpha(bodyAlpha);
                psmain = __instance.coreParticles.main;
                psmain.startColor = psmain.startColor.color.WithAlpha(bodyAlpha);
                __instance.ring.color = __instance.ring.color.WithAlpha(ringAlpha * 0.4f);
                __instance.glow.color = __instance.glow.color.WithAlpha(bodyAlpha * 0.5f);
                __instance.sprite.color = __instance.sprite.color.WithAlpha(bodyAlpha);
            }
        }

        [HarmonyPatch(typeof(scrPlanet), "SetRainbow")]
        private static class SetRainbowPatch
        {
            [HarmonyPriority(Priority.LowerThanNormal)]
            public static bool Prefix(
                scrPlanet __instance, bool enabled, ref Sequence ___rainbowSeq) {
                float bodyOpacity = CalculateBodyOpacity(__instance);
                float tailOpacity = CalculateTailOpacity(__instance);
                float ringOpacity = CalculateRingOpacity(__instance);
                if (!enabled) {
                    if (___rainbowSeq != null) {
                        ___rainbowSeq.Kill(false);
                    }
                    return false;
                }
                if (___rainbowSeq != null) {
                    ___rainbowSeq.Kill(false);
                }
                __instance.sprite.color = Color.red;
                Color.RGBToHSV(Color.red, out float _, out float s, out float v);
                Tween[] array = new Tween[10];
                ___rainbowSeq = DOTween.Sequence();
                for (int i = 0; i < array.Length; i++) {
                    Color col = ApplyOpacity(Color.HSVToRGB(0.1f + 0.1f * i, s, v), bodyOpacity);
                    array[i] = __instance.sprite.DOColor(col, 0.5f);
                    ___rainbowSeq.Append(array[i]);
                }
                ___rainbowSeq.SetLoops(-1, LoopType.Restart).SetUpdate(true);
                Sequence tempSequence = ___rainbowSeq;
                ___rainbowSeq.OnUpdate(() => {
                    if (__instance.ring != null) {
                        setRingColorMethod.Invoke(
                            __instance,
                            new object[] { ApplyOpacity(__instance.sprite.color, ringOpacity) });
                        __instance.SetTailColor(ApplyOpacity(__instance.sprite.color, tailOpacity));
                        __instance.SetCoreColor(__instance.sprite.color);
                        return;
                    }
                    tempSequence.Kill(false);
                });
                return false;
            }
        }
    }
}
