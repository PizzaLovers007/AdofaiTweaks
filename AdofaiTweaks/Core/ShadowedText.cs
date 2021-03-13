using UnityEngine;
using UnityEngine.UI;

namespace AdofaiTweaks.Core
{
    /// <summary>
    /// A simple <see cref="UnityEngine.UI.Text"/> wrapper that has a drop
    /// shadow.
    /// </summary>
    public class ShadowedText : MonoBehaviour
    {
        private Text mainText;
        private Text shadowText;

        /// <summary>
        /// The text that this object displays.
        /// </summary>
        public string Text {
            get {
                return mainText.text;
            }
            set {
                mainText.text = value;
                shadowText.text = value;
            }
        }

        /// <summary>
        /// The alignment of the text.
        /// </summary>
        public TextAnchor Alignment {
            get {
                return mainText.alignment;
            }
            set {
                mainText.alignment = value;
                shadowText.alignment = value;
            }
        }

        /// <summary>
        /// The font size of the text.
        /// </summary>
        public int FontSize {
            get {
                return mainText.fontSize;
            }
            set {
                mainText.fontSize = value;
                shadowText.fontSize = value;
                shadowText.rectTransform.anchoredPosition =
                    Position + new Vector2(value / 20f, -value / 20f);
            }
        }

        /// <summary>
        /// The color of the text.
        /// </summary>
        public Color Color {
            get {
                return mainText.color;
            }
            set {
                mainText.color = value;
            }
        }

        /// <summary>
        /// The normalized center position within the bounding box of the text.
        /// </summary>
        public Vector2 Center {
            get {
                return mainText.rectTransform.anchorMin;
            }
            set {
                mainText.rectTransform.anchorMin = value;
                mainText.rectTransform.anchorMax = value;
                mainText.rectTransform.pivot = value;
                shadowText.rectTransform.anchorMin = value;
                shadowText.rectTransform.anchorMax = value;
                shadowText.rectTransform.pivot = value;
            }
        }

        /// <summary>
        /// The position within the <see cref="Canvas"/> that the text is at.
        /// </summary>
        public Vector2 Position {
            get {
                return mainText.rectTransform.anchoredPosition;
            }
            set {
                mainText.rectTransform.anchoredPosition = value;
                shadowText.rectTransform.anchoredPosition =
                    value + new Vector2(FontSize / 20f, -FontSize / 20f);
            }
        }

        /// <summary>
        /// Creates the text with a drop shadow.
        /// </summary>
        protected void Awake() {
            Canvas canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            ContentSizeFitter fitter;

            GameObject shadowObject = new GameObject();
            fitter = shadowObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            shadowObject.transform.SetParent(transform);
            shadowText = shadowObject.AddComponent<Text>();
            shadowText.font = RDString.GetFontDataForLanguage(RDString.language).font;
            shadowText.color = Color.black.WithAlpha(0.4f);

            GameObject mainObject = new GameObject();
            mainObject.transform.SetParent(transform);
            fitter = mainObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            mainText = mainObject.AddComponent<Text>();
            mainText.font = RDString.GetFontDataForLanguage(RDString.language).font;
        }
    }
}
