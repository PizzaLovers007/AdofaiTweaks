using System;
using System.Xml.Serialization;
using AdofaiTweaks.Core;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.PlanetColor;

/// <summary>
/// Settings for the Planet Color tweak.
/// </summary>
public class PlanetColorSettings : TweakSettings {
    /// <summary>
    /// Color profiles for all planets.
    /// </summary>
    public PlanetColor[] ColorProfiles { get; set; }

    /// <summary>
    /// Whether the custom coloring for planet 1 is enabled.
    /// </summary>
    [XmlIgnore]
    [Obsolete("This option only exists to ensure backwards compatibility and will not be up to date.")]
    public bool Color1Enabled { get; set; }

    /// <summary>
    /// Whether the custom coloring for planet 2 is enabled.
    /// </summary>
    [XmlIgnore]
    [Obsolete("This option only exists to ensure backwards compatibility and will not be up to date.")]
    public bool Color2Enabled { get; set; }

    private Color _color1;

    /// <summary>
    /// The color of planet 1's body.
    /// </summary>
    [XmlIgnore]
    [Obsolete("This option only exists to ensure backwards compatibility and will not be up to date.")]
    public Color Color1 {
        get => _color1;
        set {
            _color1 = value;
            Color1Hex = ColorUtility.ToHtmlStringRGB(value);
        }
    }

    private Color _color2;

    /// <summary>
    /// The color of planet 2's body.
    /// </summary>
    [XmlIgnore]
    [Obsolete("This option only exists to ensure backwards compatibility and will not be up to date.")]
    public Color Color2 {
        get => _color2;
        set {
            _color2 = value;
            Color2Hex = ColorUtility.ToHtmlStringRGB(value);
        }
    }

    private Color _tailColor1;

    /// <summary>
    /// The color of planet 1's tail.
    /// </summary>
    [XmlIgnore]
    [Obsolete("This option only exists to ensure backwards compatibility and will not be up to date.")]
    public Color TailColor1 {
        get => _tailColor1;
        set {
            _tailColor1 = value;
            TailColor1Hex = ColorUtility.ToHtmlStringRGB(value);
        }
    }

    private Color _tailColor2;

    /// <summary>
    /// The color of planet 2's tail.
    /// </summary>
    [XmlIgnore]
    [Obsolete("This option only exists to ensure backwards compatibility and will not be up to date.")]
    public Color TailColor2 {
        get => _tailColor2;
        set {
            _tailColor2 = value;
            TailColor2Hex = ColorUtility.ToHtmlStringRGB(value);
        }
    }

    /// <summary>
    /// The hex for the color of planet 1's body.
    /// </summary>
    [XmlIgnore]
    [Obsolete("This option only exists to ensure backwards compatibility and will not be up to date.")]
    public string Color1Hex { get; set; }

    /// <summary>
    /// The hex for the color of planet 2's body.
    /// </summary>
    [XmlIgnore]
    [Obsolete("This option only exists to ensure backwards compatibility and will not be up to date.")]
    public string Color2Hex { get; set; }

    /// <summary>
    /// The hex for the color of planet 1's tail.
    /// </summary>
    [XmlIgnore]
    [Obsolete("This option only exists to ensure backwards compatibility and will not be up to date.")]
    public string TailColor1Hex { get; set; }

    /// <summary>
    /// The hex for the color of planet 2's tail.
    /// </summary>
    [XmlIgnore]
    [Obsolete("This option only exists to ensure backwards compatibility and will not be up to date.")]
    public string TailColor2Hex { get; set; }

    private static PlanetColor[] DefaultColorProfiles {
        get {
            const int profileCount = 3;
            string[] names = [ "Red", "Blue", "Green", "Yellow", "Purple", "Pink", "Orange", "Cyan" ];
            Color[] colors = [
                Color.red, Color.blue, new(.3f, .7f, 0), new(1f, .8f, 0),
                new(.7f, .1f, 1f), new(1f, .1f, .7f), new(1f, .4f, .1f), new(.1f, .8f, .9f)
            ];

            var result = new PlanetColor[profileCount];

            for (var i = 0; i < profileCount; i++) {
                result[i] = new(
                    names[i] + " Planet",
                    new(PlanetColorType.Solid, colors[i]),
                    new(PlanetColorType.Solid, colors[i]));
            }

            return result;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlanetColorSettings"/>
    /// class with some default colors.
    /// </summary>
    public PlanetColorSettings() {
        ColorProfiles = DefaultColorProfiles;
    }

    #pragma warning disable CS0618 // Type or member is obsolete

    /// <summary>
    /// Migrate the old settings to new settings.
    /// </summary>
    public void Migrate() {
        if (Color1Hex == null && Color2Hex == null)
        {
            return;
        }

        var red = new PlanetColor(
            "Red Planet",
            new(PlanetColorType.Solid, Color1),
            new(PlanetColorType.Solid, TailColor1));

        var blue = new PlanetColor(
            "Blue Planet",
            new(PlanetColorType.Solid, Color2),
            new(PlanetColorType.Solid, TailColor2));

        red.Enabled = Color1Enabled;
        blue.Enabled = Color2Enabled;

        Color1Hex = null;
        Color2Hex = null;
    }

    #pragma warning restore CS0618 // Type or member is obsolete
}