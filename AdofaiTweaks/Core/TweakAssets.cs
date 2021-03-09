using System.IO;
using UnityEngine;

namespace AdofaiTweaks.Core
{
    public static class TweakAssets
    {
        public static Font KoreanNormalFont { get; private set; }
        public static Font KoreanBoldFont { get; private set; }
        public static Sprite HandSprite { get; private set; }
        public static Sprite MeterSprite { get; private set; }
        public static Sprite TickSprite { get; private set; }
        public static Sprite KeyOutlineSprite { get; private set; }
        public static Sprite KeyBackgroundSprite { get; private set; }

        private static readonly AssetBundle assets;

        static TweakAssets() {
            assets =
                AssetBundle.LoadFromFile(
                    Path.Combine("Mods", "AdofaiTweaks", "adofai_tweaks.assets"));
            KoreanNormalFont = assets.LoadAsset<Font>("Assets/NanumGothic-Regular.ttf");
            KoreanBoldFont = assets.LoadAsset<Font>("Assets/NanumGothic-Bold.ttf");
            HandSprite = assets.LoadAsset<Sprite>("Assets/Hand.png");
            MeterSprite = assets.LoadAsset<Sprite>("Assets/Meter.png");
            TickSprite = assets.LoadAsset<Sprite>("Assets/Tick.png");
            KeyOutlineSprite = assets.LoadAsset<Sprite>("Assets/KeyOutline.png");
            KeyBackgroundSprite = assets.LoadAsset<Sprite>("Assets/KeyBackground.png");
        }
    }
}
