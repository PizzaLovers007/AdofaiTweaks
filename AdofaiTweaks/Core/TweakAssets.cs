using System.IO;
using System.Linq;
using UnityEngine;

namespace AdofaiTweaks.Core;

/// <summary>
/// Holds references to all Unity assets for AdofaiTweaks. These assets are
/// all loaded on first use of any of them.
/// </summary>
public static class TweakAssets
{
    /// <summary>
    /// The normal font to use for text for symbol languages.
    /// </summary>
    public static Font SymbolLangNormalFont { get; private set; }

    /// <summary>
    /// The bold font to use for Korean text.
    /// </summary>
    public static Font KoreanBoldFont { get; private set; }

    /// <summary>
    /// The sprite for the bottom arrow on the hit error meter.
    /// </summary>
    public static Sprite HandSprite { get; private set; }

    /// <summary>
    /// The sprite for the colored part of the hit error meter.
    /// </summary>
    public static Sprite MeterSprite { get; private set; }

    /// <summary>
    /// The sprite for the colored ticks on the hit error meter.
    /// </summary>
    public static Sprite TickSprite { get; private set; }

    /// <summary>
    /// The sprite for the key's outline in the key viewer.
    /// </summary>
    public static Sprite KeyOutlineSprite { get; private set; }

    /// <summary>
    /// The sprite for the key's background fill in the key viewer.
    /// </summary>
    public static Sprite KeyBackgroundSprite { get; private set; }

    private static readonly AssetBundle Assets;

    static TweakAssets() {
        Assets = LoadAssetBundle();
        SymbolLangNormalFont = LoadAsset<Font>("Assets/NanumGothic-Regular.ttf");
        KoreanBoldFont = LoadAsset<Font>("Assets/NanumGothic-Bold.ttf");
        HandSprite = LoadAsset<Sprite>("Assets/Hand.png");
        MeterSprite = LoadAsset<Sprite>("Assets/Meter.png");
        TickSprite = LoadAsset<Sprite>("Assets/Tick.png");
        KeyOutlineSprite = LoadAsset<Sprite>("Assets/KeyOutline.png");
        KeyBackgroundSprite = LoadAsset<Sprite>("Assets/KeyBackground.png");
    }

    private static AssetBundle LoadAssetBundle() {
        string[] paths = new[] {
            Path.Combine(AdofaiTweaks.ModPath ?? string.Empty, "adofai_tweaks.assets"),
            Path.Combine("Mods", "AdofaiTweaks", "adofai_tweaks.assets"),
        };

        foreach (string path in paths.Where(File.Exists).Distinct()) {
            AssetBundle bundle = AssetBundle.LoadFromFile(path);
            if (bundle != null) {
                return bundle;
            }

            AdofaiTweaks.Logger?.Error($"Failed to load asset bundle: {path}");
        }

        AdofaiTweaks.Logger?.Error("Failed to find asset bundle: adofai_tweaks.assets");
        return null;
    }

    private static T LoadAsset<T>(string name)
        where T : Object {
        T asset = Assets?.LoadAsset<T>(name);
        if (asset == null) {
            AdofaiTweaks.Logger?.Error($"Failed to load asset: {name}");
        }

        return asset;
    }
}