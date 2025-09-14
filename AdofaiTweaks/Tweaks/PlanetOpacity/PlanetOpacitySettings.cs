using AdofaiTweaks.Core;

namespace AdofaiTweaks.Tweaks.PlanetOpacity;

/// <summary>
/// Settings for the Planet Opacity tweak.
/// </summary>
public class PlanetOpacitySettings : TweakSettings
{
    /// <summary>
    /// Value that is used to designate that the field is migrated.
    /// </summary>
    public const float MIGRATED_VALUE = -12f;

    /// <summary>
    /// Older field for opacity for planet 1. Use <see cref="PlanetOpacity1"/>
    /// instead.
    /// </summary>
    public float SettingsOpacity1 { get; set; }

    /// <summary>
    /// Older field for opacity for planet 2. Use <see cref="PlanetOpacity2"/>
    /// instead.
    /// </summary>
    public float SettingsOpacity2 { get; set; }

    /// <summary>
    /// The opacity settings for planet 1.
    /// </summary>
    public PlanetOpacityProfile PlanetOpacity1 { get; set; }

    /// <summary>
    /// The opacity settings for planet 2.
    /// </summary>
    public PlanetOpacityProfile PlanetOpacity2 { get; set; }

    /// <summary>
    /// The opacity settings for planet 3.
    /// </summary>
    public PlanetOpacityProfile PlanetOpacity3 { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlanetOpacitySettings"/>
    /// class with some default colors.
    /// </summary>
    public PlanetOpacitySettings() {
        // Treat as migrated by default
        SettingsOpacity1 = MIGRATED_VALUE;
        SettingsOpacity2 = MIGRATED_VALUE;

        PlanetOpacity1 = new PlanetOpacityProfile() {
            Body = 50f,
            Tail = 50f,
            Ring = 50f,
        };
        PlanetOpacity2 = new PlanetOpacityProfile() {
            Body = 50f,
            Tail = 50f,
            Ring = 50f,
        };
        PlanetOpacity3 = new PlanetOpacityProfile() {
            Body = 50f,
            Tail = 50f,
            Ring = 50f,
        };
    }
}
