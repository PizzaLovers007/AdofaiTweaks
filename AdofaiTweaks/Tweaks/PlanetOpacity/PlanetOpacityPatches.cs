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

        private static Color ApplyOpacity(Color color, float opacity) {
            float alpha = color.a * opacity / 100;
            return color.WithAlpha(alpha);
        }

        [HarmonyPatch(typeof(scrPlanet), "SetPlanetColor")]
        private static class SetPlanetColorPatch
        {
            public static void Postfix(scrPlanet __instance, Color color) {
                if (!Settings.IsEnabled || !AdofaiTweaks.IsEnabled) {
                    return;
                }
                float opacity =
                    __instance.isRed ? Settings.ActualOpacity1 : Settings.ActualOpacity2;
                ParticleSystem.MainModule psmain = __instance.sparks.main;
                psmain.startColor = ApplyOpacity(psmain.startColor.color, opacity);
                __instance.sprite.color = ApplyOpacity(color, opacity);
            }
        }

        [HarmonyPatch(typeof(scrPlanet), "SetCoreColor")]
        private static class SetCoreColorPatch
        {
            public static void Postfix(scrPlanet __instance, Color color) {
                if (!Settings.IsEnabled || !AdofaiTweaks.IsEnabled) {
                    return;
                }
                float opacity =
                    __instance.isRed ? Settings.ActualOpacity1 : Settings.ActualOpacity2;
                __instance.glow.color = ApplyOpacity(__instance.glow.color, opacity);
                ParticleSystem.MainModule psmain = __instance.tailParticles.main;
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
                if (!Settings.IsEnabled || !AdofaiTweaks.IsEnabled) {
                    return;
                }
                float opacity =
                    __instance.isRed ? Settings.ActualOpacity1 : Settings.ActualOpacity2;
                setParticleSystemColorMethod.Invoke(
                    __instance,
                    new object[] {
                        __instance.tailParticles,
                        ApplyOpacity(color, opacity),
                        ApplyOpacity(color, opacity) * new Color(0.5f, 0.5f, 0.5f),
                    });
                ParticleSystem.MainModule psmain = __instance.deathExplosion.main;
                psmain.startColor = ApplyOpacity(color, opacity);
            }
        }

        [HarmonyPatch(typeof(scrPlanet), "SetRingColor")]
        private static class SetRingColorPatch
        {
            public static void Postfix(scrPlanet __instance) {
                if (!Settings.IsEnabled || !AdofaiTweaks.IsEnabled) {
                    return;
                }
                float opacity =
                    __instance.isRed ? Settings.ActualOpacity1 : Settings.ActualOpacity2;
                __instance.ring.color = ApplyOpacity(__instance.ring.color, opacity);
            }
        }

        [HarmonyPatch(typeof(scrPlanet), "DisableCustomColor")]
        private static class DisableCustomColorPatch
        {
            public static void Postfix(scrPlanet __instance) {
                if (!Settings.IsEnabled || !AdofaiTweaks.IsEnabled) {
                    return;
                }
                float opacity =
                    __instance.isRed ? Settings.ActualOpacity1 : Settings.ActualOpacity2;
                __instance.sprite.color = ApplyOpacity(Color.white, opacity);
            }
        }

        [HarmonyPatch(typeof(scrPlanet), "SwitchToGold")]
        private static class SwitchToGoldPatch
        {
            public static void Postfix(scrPlanet __instance) {
                if (!Settings.IsEnabled || !AdofaiTweaks.IsEnabled) {
                    return;
                }
                float alpha =
                    __instance.isRed ? Settings.ActualOpacity1 : Settings.ActualOpacity2;
                alpha /= 100;
                ParticleSystem.MainModule psmain;
                psmain = __instance.tailParticles.main;
                psmain.startColor = psmain.startColor.color.WithAlpha(alpha);
                psmain = __instance.sparks.main;
                psmain.startColor = psmain.startColor.color.WithAlpha(alpha);
                __instance.ring.color = __instance.ring.color.WithAlpha(alpha * 0.4f);
                __instance.glow.color = __instance.glow.color.WithAlpha(alpha * 0.5f);
                __instance.sprite.color = __instance.sprite.color.WithAlpha(alpha);
            }
        }

        [HarmonyPatch(typeof(scrPlanet), "SetRainbow")]
        private static class SetRainbowPatch
        {
            [HarmonyPriority(Priority.LowerThanNormal)]
            public static bool Prefix(
                scrPlanet __instance, bool enabled, ref Sequence ___rainbowSeq) {
                if (!Settings.IsEnabled || !AdofaiTweaks.IsEnabled) {
                    return true;
                }
                float opacity =
                    __instance.isRed ? Settings.ActualOpacity1 : Settings.ActualOpacity2;
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
                    Color col = ApplyOpacity(Color.HSVToRGB(0.1f + 0.1f * i, s, v), opacity);
                    array[i] = __instance.sprite.DOColor(col, 0.5f);
                    ___rainbowSeq.Append(array[i]);
                }
                ___rainbowSeq.SetLoops(-1, LoopType.Restart).SetUpdate(true);
                Sequence tempSequence = ___rainbowSeq;
                ___rainbowSeq.OnUpdate(() => {
                    if (__instance.ring != null) {
                        setRingColorMethod.Invoke(
                            __instance, new object[] { __instance.sprite.color });
                        __instance.SetTailColor(__instance.sprite.color);
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
