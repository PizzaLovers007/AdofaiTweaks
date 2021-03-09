using System.Collections.Generic;
using AdofaiTweaks.Core;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace AdofaiTweaks.Tweaks.KeyLimiter
{
    internal class KeyViewer : MonoBehaviour
    {
        public static readonly Dictionary<KeyCode, string> KEY_TO_STRING =
            new Dictionary<KeyCode, string>() {
                { KeyCode.Alpha0, "0" },
                { KeyCode.Alpha1, "1" },
                { KeyCode.Alpha2, "2" },
                { KeyCode.Alpha3, "3" },
                { KeyCode.Alpha4, "4" },
                { KeyCode.Alpha5, "5" },
                { KeyCode.Alpha6, "6" },
                { KeyCode.Alpha7, "7" },
                { KeyCode.Alpha8, "8" },
                { KeyCode.Alpha9, "9" },
                { KeyCode.Keypad0, "0" },
                { KeyCode.Keypad1, "1" },
                { KeyCode.Keypad2, "2" },
                { KeyCode.Keypad3, "3" },
                { KeyCode.Keypad4, "4" },
                { KeyCode.Keypad5, "5" },
                { KeyCode.Keypad6, "6" },
                { KeyCode.Keypad7, "7" },
                { KeyCode.Keypad8, "8" },
                { KeyCode.Keypad9, "9" },
                { KeyCode.KeypadPlus, "+" },
                { KeyCode.KeypadMinus, "-" },
                { KeyCode.KeypadMultiply, "*" },
                { KeyCode.KeypadDivide, "/" },
                { KeyCode.KeypadEnter, "↵" },
                { KeyCode.KeypadEquals, "=" },
                { KeyCode.KeypadPeriod, "." },
                { KeyCode.Return, "↵" },
                { KeyCode.None, " " },
                { KeyCode.Tab, "⇥" },
                { KeyCode.Backslash, "\\" },
                { KeyCode.Slash, "/" },
                { KeyCode.Minus, "-" },
                { KeyCode.Equals, "=" },
                { KeyCode.LeftBracket, "[" },
                { KeyCode.RightBracket, "]" },
                { KeyCode.Semicolon, ";" },
                { KeyCode.Comma, "," },
                { KeyCode.Period, "." },
                { KeyCode.Quote, "'" },
                { KeyCode.UpArrow, "↑" },
                { KeyCode.DownArrow, "↓" },
                { KeyCode.LeftArrow, "←" },
                { KeyCode.RightArrow, "→" },
                { KeyCode.Space, "␣" },
                { KeyCode.BackQuote, "`" },
                { KeyCode.LeftShift, "L⇧" },
                { KeyCode.RightShift, "R⇧" },
                { KeyCode.LeftControl, "LCtrl" },
                { KeyCode.RightControl, "RCtrl" },
                { KeyCode.LeftAlt, "LAlt" },
                { KeyCode.RightAlt, "AAlt" },
                { KeyCode.Delete, "Del" },
                { KeyCode.PageDown, "Pg↓" },
                { KeyCode.PageUp, "Pg↑" },
                { KeyCode.Insert, "Ins" },
            };
        private const float EASE_DURATION = 0.1f;
        private const float SHRINK_FACTOR = 0.9f;

        private GameObject keysObject;
        private Dictionary<KeyCode, Image> keyBgImages;
        private Dictionary<KeyCode, Image> keyOutlineImages;
        private Dictionary<KeyCode, Text> keyTexts;
        private Dictionary<KeyCode, bool> keyPrevStates;
        private RectTransform keysRectTransform;

        private KeyLimiterSettings _settings = new KeyLimiterSettings();
        public KeyLimiterSettings Settings {
            get => _settings;
            set {
                _settings = value;
                UpdateKeys();
            }
        }

        protected void Awake() {
            Canvas mainCanvas = gameObject.AddComponent<Canvas>();
            mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            mainCanvas.sortingOrder = 10001;
            CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.referenceResolution = new Vector2(1920, 1080);

            UpdateKeys();
        }

        public void UpdateKeys() {
            if (keysObject) {
                Destroy(keysObject);
            }

            keysObject = new GameObject();
            keysObject.transform.SetParent(transform);
            keysObject.AddComponent<Canvas>();
            keysRectTransform = keysObject.GetComponent<RectTransform>();

            keyBgImages = new Dictionary<KeyCode, Image>();
            keyOutlineImages = new Dictionary<KeyCode, Image>();
            keyTexts = new Dictionary<KeyCode, Text>();
            keyPrevStates = new Dictionary<KeyCode, bool>();
            foreach (KeyCode code in Settings.ActiveKeys) {
                GameObject keyBgObj = new GameObject();
                keyBgObj.transform.SetParent(keysObject.transform);
                Image bgImage = keyBgObj.AddComponent<Image>();
                bgImage.sprite = TweakAssets.KeyBackgroundSprite;
                bgImage.color = Settings.ReleasedBackgroundColor;
                keyBgImages[code] = bgImage;

                GameObject keyOutlineObj = new GameObject();
                keyOutlineObj.transform.SetParent(keysObject.transform);
                Image outlineImage = keyOutlineObj.AddComponent<Image>();
                outlineImage.sprite = TweakAssets.KeyOutlineSprite;
                outlineImage.color = Settings.ReleasedOutlineColor;
                keyOutlineImages[code] = outlineImage;

                GameObject keyTextObj = new GameObject();
                keyTextObj.transform.SetParent(keysObject.transform);
                Text text = keyTextObj.AddComponent<Text>();
                text.font = RDString.GetFontDataForLanguage(RDString.language).font;
                text.color = Settings.ReleasedTextColor;
                text.alignment = TextAnchor.MiddleCenter;
                if (!KEY_TO_STRING.TryGetValue(code, out string codeString)) {
                    codeString = code.ToString();
                }
                text.text = codeString;
                keyTexts[code] = text;

                keyPrevStates[code] = false;
            }

            UpdateLayout();
        }

        public void UpdateLayout() {
            int count = keyOutlineImages.Keys.Count;
            float keySize = Settings.KeyViewerSize;
            float spacing = Settings.KeyViewerSize / 10;
            float width = count * keySize + (count - 1) * spacing;
            Vector2 pos = new Vector2(Settings.KeyViewerXPos, Settings.KeyViewerYPos);

            keysRectTransform.anchorMin = pos;
            keysRectTransform.anchorMax = pos;
            keysRectTransform.pivot = pos;
            keysRectTransform.sizeDelta = new Vector2(width, keySize);
            keysRectTransform.anchoredPosition = Vector2.zero;

            float x = 0;
            foreach (KeyCode code in keyOutlineImages.Keys) {
                DOTween.Kill(TweenIdForKeyCode(code));

                Image bgImage = keyBgImages[code];
                bgImage.rectTransform.anchorMin = Vector2.zero;
                bgImage.rectTransform.anchorMax = Vector2.zero;
                bgImage.rectTransform.pivot = new Vector2(0.5f, 0.5f);
                bgImage.rectTransform.sizeDelta = new Vector2(keySize, keySize);
                bgImage.rectTransform.anchoredPosition =
                    new Vector2(x + keySize / 2, keySize / 2);

                Image outlineImage = keyOutlineImages[code];
                outlineImage.rectTransform.anchorMin = Vector2.zero;
                outlineImage.rectTransform.anchorMax = Vector2.zero;
                outlineImage.rectTransform.pivot = new Vector2(0.5f, 0.5f);
                outlineImage.rectTransform.sizeDelta = new Vector2(keySize, keySize);
                outlineImage.rectTransform.anchoredPosition =
                    new Vector2(x + keySize / 2, keySize / 2);

                Text text = keyTexts[code];
                text.rectTransform.anchorMin = Vector2.zero;
                text.rectTransform.anchorMax = Vector2.zero;
                text.rectTransform.pivot = new Vector2(0.5f, 0.5f);
                text.rectTransform.sizeDelta = new Vector2(keySize, keySize);
                text.rectTransform.anchoredPosition =
                    new Vector2(x + keySize / 2, keySize / 2 + keySize / 20);
                text.fontSize = Mathf.RoundToInt(keySize * 3 / 4);

                Vector3 scale = new Vector3(1, 1, 1);
                if (keyPrevStates[code]) {
                    bgImage.color = Settings.PressedBackgroundColor;
                    outlineImage.color = Settings.PressedOutlineColor;
                    text.color = Settings.PressedTextColor;
                    scale *= SHRINK_FACTOR;
                } else {
                    bgImage.color = Settings.ReleasedBackgroundColor;
                    outlineImage.color = Settings.ReleasedOutlineColor;
                    text.color = Settings.ReleasedTextColor;
                }
                bgImage.rectTransform.localScale = scale;
                outlineImage.rectTransform.localScale = scale;
                text.rectTransform.localScale = scale;

                x += keySize + spacing;
            }
        }

        public void UpdateState(Dictionary<KeyCode, bool> state) {
            foreach (KeyCode code in keyOutlineImages.Keys) {
                // Only change if the state changed
                if (state[code] == keyPrevStates[code]) {
                    continue;
                }
                keyPrevStates[code] = state[code];

                string id = TweenIdForKeyCode(code);
                if (DOTween.IsTweening(id)) {
                    DOTween.Kill(id);
                }

                Image bgImage = keyBgImages[code];
                Image outlineImage = keyOutlineImages[code];
                Text text = keyTexts[code];

                // Calculate the new color/size
                Color bgColor, outlineColor, textColor;
                Vector3 scale = new Vector3(1, 1, 1);
                if (state[code]) {
                    bgColor = Settings.PressedBackgroundColor;
                    outlineColor = Settings.PressedOutlineColor;
                    textColor = Settings.PressedTextColor;
                    if (Settings.AnimateKeys) {
                        scale *= SHRINK_FACTOR;
                    }
                } else {
                    bgColor = Settings.ReleasedBackgroundColor;
                    outlineColor = Settings.ReleasedOutlineColor;
                    textColor = Settings.ReleasedTextColor;
                }

                // Apply the new color/size
                bgImage.color = bgColor;
                outlineImage.color = outlineColor;
                text.color = textColor;
                if (Settings.AnimateKeys) {
                    bgImage.rectTransform.DOScale(scale, EASE_DURATION)
                        .SetId(id)
                        .SetEase(Ease.OutExpo)
                        .SetUpdate(true)
                        .OnKill(() => bgImage.rectTransform.localScale = scale);
                    outlineImage.rectTransform.DOScale(scale, EASE_DURATION)
                        .SetId(id)
                        .SetEase(Ease.OutExpo)
                        .SetUpdate(true)
                        .OnKill(() => outlineImage.rectTransform.localScale = scale);
                    text.rectTransform.DOScale(scale, EASE_DURATION)
                        .SetId(id)
                        .SetEase(Ease.OutExpo)
                        .SetUpdate(true)
                        .OnKill(() => text.rectTransform.localScale = scale);
                } else {
                    bgImage.rectTransform.localScale = scale;
                    outlineImage.rectTransform.localScale = scale;
                    text.rectTransform.localScale = scale;
                }
            }
        }

        private static string TweenIdForKeyCode(KeyCode code) {
            return $"adofai_tweaks.key_viewer.{code}";
        }
    }
}
