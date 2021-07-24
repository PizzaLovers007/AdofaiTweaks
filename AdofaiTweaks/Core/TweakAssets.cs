using System.IO;
using MelonLoader;
using UnityEngine;

namespace AdofaiTweaks.Core
{
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

        private static readonly AssetBundle assets;

        static TweakAssets() {
            assets =
                AssetBundle.LoadFromFile(
                    Path.Combine(
                        MelonHandler.ModsDirectory, "AdofaiTweaks", "adofai_tweaks.assets"));
            SymbolLangNormalFont = assets.LoadAsset<Font>("Assets/NanumGothic-Regular.ttf");
            KoreanBoldFont = assets.LoadAsset<Font>("Assets/NanumGothic-Bold.ttf");
            HandSprite = assets.LoadAsset<Sprite>("Assets/Hand.png");
            MeterSprite = assets.LoadAsset<Sprite>("Assets/Meter.png");
            TickSprite = assets.LoadAsset<Sprite>("Assets/Tick.png");
            KeyOutlineSprite = assets.LoadAsset<Sprite>("Assets/KeyOutline.png");
            KeyBackgroundSprite = assets.LoadAsset<Sprite>("Assets/KeyBackground.png");
        }
    }
}
