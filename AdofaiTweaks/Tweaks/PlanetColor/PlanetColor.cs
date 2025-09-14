using System;
using AdofaiTweaks.Core;
using AdofaiTweaks.Strings;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.PlanetColor;

/// <summary>
/// Map of <see cref="PlanetComponentColor"/>.
/// </summary>
public class PlanetColor {
    /// <summary>
    /// Name of planet to color.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Whether to use this settings value.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Planet body color.
    /// </summary>
    public PlanetComponentColor Body { get; set; }

    /// <summary>
    /// Planet tail color.
    /// </summary>
    public PlanetComponentColor Tail { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlanetColor"/> class.
    /// </summary>
    /// <param name="name">Name of the planet to color.</param>
    /// <param name="body">Body color.</param>
    /// <param name="tail">Tail color.</param>
    public PlanetColor(string name, PlanetComponentColor body, PlanetComponentColor tail) {
        Name = name;
        Body = body;
        Tail = tail;
    }

    /// <summary>
    /// Display GUI Settings of this instance.
    /// </summary>
    /// <param name="onChanged">Callback that is called when this instance is mutated.</param>
    public void ShowGUISettings(Action onChanged) {
        var callOnChanged = false;

        var lastEnabled = Enabled;
        if (Enabled = GUILayout.Toggle(Enabled, Name)) {
            MoreGUILayout.BeginIndent();

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            GUILayout.Label(TweakStrings.Get(TranslationKeys.PlanetColor.BODY));

            callOnChanged |= Body.OnGUI();

            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.Label(TweakStrings.Get(TranslationKeys.PlanetColor.TAIL));

            callOnChanged |= Tail.OnGUI();

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            MoreGUILayout.EndIndent();
        }

        callOnChanged |= lastEnabled != Enabled;

        if (callOnChanged) {
            onChanged();
        }
    }
}