using System.IO;
using Ionic.Zip;
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
            Stream uabStream = OpenFile("adofai_tweaks.assets");
            if (uabStream == null) {
                MelonLogger.Error("Could not read UAB!");
                return;
            }
            using (uabStream) {
                assets = AssetBundle.LoadFromStream(OpenFile("adofai_tweaks.assets"));
                SymbolLangNormalFont = assets.LoadAsset<Font>("Assets/NanumGothic-Regular.ttf");
                KoreanBoldFont = assets.LoadAsset<Font>("Assets/NanumGothic-Bold.ttf");
                HandSprite = assets.LoadAsset<Sprite>("Assets/Hand.png");
                MeterSprite = assets.LoadAsset<Sprite>("Assets/Meter.png");
                TickSprite = assets.LoadAsset<Sprite>("Assets/Tick.png");
                KeyOutlineSprite = assets.LoadAsset<Sprite>("Assets/KeyOutline.png");
                KeyBackgroundSprite = assets.LoadAsset<Sprite>("Assets/KeyBackground.png");
            }
        }

        /// <summary>
        /// Opens a <see cref="Stream"/> to an asset file in the mod zip file.
        /// Logs an error if there was an issue finding/opening the asset.
        /// </summary>
        /// <param name="assetFilename">
        /// The filename of the asset to open.
        /// </param>
        /// <returns>
        /// The <see cref="Stream"/> to the asset file, or <c>null</c> if there
        /// was an error opening the file.
        /// </returns>
        public static Stream OpenFile(string assetFilename) {
            string[] matchedZips =
                Directory.GetFiles(
                    MelonHandler.ModsDirectory, $"AdofaiTweaks*.zip");
            if (matchedZips.Length == 0) {
                MelonLogger.Error("Unable to find mod zip file.");
                return null;
            } else if (matchedZips.Length > 1) {
                MelonLogger.Error("Multiple copies of the mod zip file were found. Please only have one downloaded version at a time.");
                return null;
            }

            using (ZipFile zipFile = ZipFile.Read(matchedZips[0])) {
                if (zipFile.ContainsEntry(assetFilename)) {
                    using (Stream zipStream = zipFile[assetFilename].OpenReader()) {
                        MemoryStream byteStream = new MemoryStream();
                        zipStream.CopyTo(byteStream);
                        return byteStream;
                    }
                }
            }

            MelonLogger.Error($"Unable to find asset '{assetFilename}' in mod zip file.");
            return null;
        }
    }
}
