using System;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using AdofaiTweaks.Core;
using JetBrains.Annotations;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.PlanetColor;

/// <summary>
/// Color options for components of planet that can be colored.
/// </summary>
public class PlanetComponentColor {
    /// <summary>
    /// The coloring type for planet's body.
    /// </summary>
    public PlanetColorType ColorType { get; set; }

    /// <summary>
    /// Solid color.
    /// </summary>
    public Color SolidColor { get; set; }

    /// <summary>
    /// Gradient.
    /// </summary>
    public Gradient Gradient { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlanetComponentColor"/> class
    /// with a default color.
    /// </summary>
    /// <param name="colorType">Color type for the planet.</param>
    /// <param name="solidColor">Solid color to use if <see cref="colorType"/> is <see cref="PlanetColorType.Solid"/>.</param>
    /// <param name="gradient">Gradient to use if <see cref="colorType"/> is <see cref="PlanetColorType.Gradient"/>.</param>
    public PlanetComponentColor(PlanetColorType colorType, Color? solidColor = null, [CanBeNull] Gradient gradient = null) {
        ColorType = colorType;
        SolidColor = solidColor ?? Color.white;
        Gradient = gradient;
    }

    private bool _openToolbar;
    private string _solidColorHex = string.Empty;

    /// <summary>
    /// Settings GUI Content.
    /// </summary>
    /// <returns><c>True</c> if anything changed.</returns>
    public bool OnGUI() {
        var result = false;

        MoreGUILayout.BeginIndent();
        // GUILayout.BeginHorizontal();
        // // TODO translate
        // GUILayout.Label("Color Type: ");
        // if (_openToolbar) {
        //     ColorType = (PlanetColorType)GUILayout.Toolbar((int)ColorType, Enum.GetNames(typeof(PlanetColorType)));
        // } else {
        //     if (GUILayout.Button(nameof(ColorType))) {
        //         _openToolbar = true;
        //     }
        // }
        // GUILayout.EndHorizontal();

        switch (ColorType) {
            case PlanetColorType.Solid:
                var lastSolidColor = SolidColor;

                SolidColor = MoreGUILayout.ColorRgbaSliders(SolidColor);
                if ((result |= lastSolidColor != SolidColor) || _solidColorHex.Length < 4) {
                    _solidColorHex = SolidColor.ToHex(true);
                }

                _solidColorHex = MoreGUILayout.NamedTextField("Hex:", _solidColorHex, 108f);
                _solidColorHex = "#" + Regex.Replace(_solidColorHex, "[^a-fA-F\\d]", string.Empty);

                if (result |= ColorUtility.TryParseHtmlString(_solidColorHex, out var newColor)) {
                    SolidColor = newColor;
                }
                break;

            case PlanetColorType.Gradient:
                break;
        }
        MoreGUILayout.EndIndent();

        return result;
    }
}