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
    /// The sprite for the key's outline in the key viewer.
    /// </summary>
    public static Sprite KeyOutlineSprite { get; private set; }

    /// <summary>
    /// The sprite for the key's background fill in the key viewer.
    /// </summary>
    public static Sprite KeyBackgroundSprite { get; private set; }

    private static readonly AssetBundle Assets;

    static TweakAssets() {
        Assets = AssetBundle.LoadFromFile(Path.GetFullPath(Path.Combine(
            "Mods", "AdofaiTweaks", "adofaitweaks.assets")));

        if (Assets == null) {
            AdofaiTweaks.Logger.Error("Could not open adofaitweaks.assets");
            return;
        }

        SymbolLangNormalFont = LoadAsset<Font>("Assets/NanumGothic-Regular.ttf");
        KoreanBoldFont = LoadAsset<Font>("Assets/NanumGothic-Bold.ttf");
        KeyOutlineSprite = LoadAsset<Sprite>("Assets/KeyOutline.png");
        KeyBackgroundSprite = LoadAsset<Sprite>("Assets/KeyBackground.png");
    }

    private static T LoadAsset<T>(string name)
        where T : Object
    {
        var asset = Assets!.LoadAsset<T>(name);

        if (asset == null) {
            AdofaiTweaks.Logger.Error($"Asset '{name}' is invalid or unassigned.");
        }

        return asset;
    }
}