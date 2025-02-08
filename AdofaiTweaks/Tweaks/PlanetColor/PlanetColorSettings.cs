using System.Xml.Serialization;
using AdofaiTweaks.Core;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.PlanetColor;

/// <summary>
/// Settings for the Planet Color tweak.
/// </summary>
public class PlanetColorSettings : TweakSettings
{
    /// <summary>
    /// Whether the custom coloring for planet 1 is enabled.
    /// </summary>
    public bool Color1Enabled { get; set; }

    /// <summary>
    /// Whether the custom coloring for planet 1 is enabled.
    /// </summary>
    public bool Color2Enabled { get; set; }

    private Color _color1;

    /// <summary>
    /// The color of planet 1's body.
    /// </summary>
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
    public string Color1Hex { get; set; }

    /// <summary>
    /// The hex for the color of planet 2's body.
    /// </summary>
    [XmlIgnore]
    public string Color2Hex { get; set; }

    /// <summary>
    /// The hex for the color of planet 1's tail.
    /// </summary>
    [XmlIgnore]
    public string TailColor1Hex { get; set; }

    /// <summary>
    /// The hex for the color of planet 2's tail.
    /// </summary>
    [XmlIgnore]
    public string TailColor2Hex { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlanetColorSettings"/>
    /// class with some default colors.
    /// </summary>
    public PlanetColorSettings() {
        Color1Enabled = true;
        Color2Enabled = true;
        Color1 = Color.white;
        Color2 = Color.white;
        TailColor1 = Color.white;
        TailColor2 = Color.white;
    }
}