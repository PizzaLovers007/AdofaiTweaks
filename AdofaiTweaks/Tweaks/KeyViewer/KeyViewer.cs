using System.Collections.Generic;
using System.Linq;
using AdofaiTweaks.Core;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace AdofaiTweaks.Tweaks.KeyViewer
{
    /// <summary>
    /// A key viewer that shows if a list of given keys are currently being
    /// pressed.
    /// </summary>
    internal class KeyViewer : MonoBehaviour
    {
        private static readonly Dictionary<KeyCode, string> KEY_TO_STRING =
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

        private static readonly Dictionary<KeyCode, int> keyCounts = new Dictionary<KeyCode, int>();

        private GameObject keysObject;
        private Dictionary<KeyCode, Image> keyBgImages;
        private Dictionary<KeyCode, Image> keyOutlineImages;
        private Dictionary<KeyCode, Text> keyTexts;
        private Dictionary<KeyCode, Text> keyCountTexts;
        private Dictionary<KeyCode, bool> keyPrevStates;
        private RectTransform keysRectTransform;

        private KeyViewerProfile _profile = new KeyViewerProfile();

        /// <summary>
        /// The current profile that this key viewer is using.
        /// </summary>
        public KeyViewerProfile Profile {
            get => _profile;
            set {
                _profile = value;
                UpdateKeys();
            }
        }

        /// <summary>
        /// Unity's Awake lifecycle event handler. Creates the key viewer.
        /// </summary>
        protected void Awake() {
            Canvas mainCanvas = gameObject.AddComponent<Canvas>();
            mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            mainCanvas.sortingOrder = 10001;
            CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.referenceResolution = new Vector2(1920, 1080);

            UpdateKeys();
        }

        /// <summary>
        /// Updates what keys are displayed on the key viewer.
        /// </summary>
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
            keyCountTexts = new Dictionary<KeyCode, Text>();
            keyPrevStates = new Dictionary<KeyCode, bool>();
            foreach (KeyCode code in Profile.ActiveKeys) {
                if (!keyCounts.ContainsKey(code)) {
                    keyCounts[code] = 0;
                }

                GameObject keyBgObj = new GameObject();
                keyBgObj.transform.SetParent(keysObject.transform);
                Image bgImage = keyBgObj.AddComponent<Image>();
                bgImage.sprite = TweakAssets.KeyBackgroundSprite;
                bgImage.color = Profile.ReleasedBackgroundColor;
                keyBgImages[code] = bgImage;
                bgImage.type = Image.Type.Sliced;

                GameObject keyOutlineObj = new GameObject();
                keyOutlineObj.transform.SetParent(keysObject.transform);
                Image outlineImage = keyOutlineObj.AddComponent<Image>();
                outlineImage.sprite = TweakAssets.KeyOutlineSprite;
                outlineImage.color = Profile.ReleasedOutlineColor;
                keyOutlineImages[code] = outlineImage;
                outlineImage.type = Image.Type.Sliced;

                GameObject keyTextObj = new GameObject();
                keyTextObj.transform.SetParent(keysObject.transform);
                Text text = keyTextObj.AddComponent<Text>();
                text.font = RDString.GetFontDataForLanguage(SystemLanguage.English).font;
                text.color = Profile.ReleasedTextColor;
                text.alignment = TextAnchor.UpperCenter;
                if (!KEY_TO_STRING.TryGetValue(code, out string codeString)) {
                    codeString = code.ToString();
                }
                text.text = codeString;
                keyTexts[code] = text;

                GameObject keyCountTextObj = new GameObject();
                keyCountTextObj.transform.SetParent(keysObject.transform);
                Text countText = keyCountTextObj.AddComponent<Text>();
                countText.font = RDString.GetFontDataForLanguage(SystemLanguage.English).font;
                countText.color = Profile.ReleasedTextColor;
                countText.alignment = TextAnchor.LowerCenter;
                countText.text = keyCounts[code] + "";
                keyCountTexts[code] = countText;

                keyPrevStates[code] = false;
            }

            UpdateLayout();
        }

        /// <summary>
        /// Updates the position, size, and color of the displayed keys.
        /// </summary>
        public void UpdateLayout() {
            int count = keyOutlineImages.Keys.Count;
            float keyWidth = 100;
            float keyHeight = Profile.ShowKeyPressTotal ? 150 : 100;
            float spacing = 10;
            float width = count * keyWidth + (count - 1) * spacing;
            Vector2 pos = new Vector2(Profile.KeyViewerXPos, Profile.KeyViewerYPos);

            keysRectTransform.anchorMin = pos;
            keysRectTransform.anchorMax = pos;
            keysRectTransform.pivot = pos;
            keysRectTransform.sizeDelta = new Vector2(width, keyHeight);
            keysRectTransform.anchoredPosition = Vector2.zero;
            keysRectTransform.localScale = new Vector3(1, 1, 1) * Profile.KeyViewerSize / 100f;

            float x = 0;
            foreach (KeyCode code in keyOutlineImages.Keys) {
                DOTween.Kill(TweenIdForKeyCode(code));

                Image bgImage = keyBgImages[code];
                bgImage.rectTransform.anchorMin = Vector2.zero;
                bgImage.rectTransform.anchorMax = Vector2.zero;
                bgImage.rectTransform.pivot = new Vector2(0.5f, 0.5f);
                bgImage.rectTransform.sizeDelta = new Vector2(keyWidth, keyHeight);
                bgImage.rectTransform.anchoredPosition =
                    new Vector2(x + keyWidth / 2, keyHeight / 2);

                Image outlineImage = keyOutlineImages[code];
                outlineImage.rectTransform.anchorMin = Vector2.zero;
                outlineImage.rectTransform.anchorMax = Vector2.zero;
                outlineImage.rectTransform.pivot = new Vector2(0.5f, 0.5f);
                outlineImage.rectTransform.sizeDelta = new Vector2(keyWidth, keyHeight);
                outlineImage.rectTransform.anchoredPosition =
                    new Vector2(x + keyWidth / 2, keyHeight / 2);

                float heightOffset = Profile.ShowKeyPressTotal ? 0 : keyHeight / 20f;
                Text text = keyTexts[code];
                text.rectTransform.anchorMin = Vector2.zero;
                text.rectTransform.anchorMax = Vector2.zero;
                text.rectTransform.pivot = new Vector2(0.5f, 0.5f);
                text.rectTransform.sizeDelta = new Vector2(keyWidth, keyHeight * 1.03f);
                text.rectTransform.anchoredPosition =
                    new Vector2(
                        x + keyWidth / 2,
                        keyHeight / 2 + heightOffset);
                text.fontSize = Mathf.RoundToInt(keyWidth * 3 / 4);
                text.alignment =
                    Profile.ShowKeyPressTotal ? TextAnchor.UpperCenter : TextAnchor.MiddleCenter;

                Text countText = keyCountTexts[code];
                countText.rectTransform.anchorMin = Vector2.zero;
                countText.rectTransform.anchorMax = Vector2.zero;
                countText.rectTransform.pivot = new Vector2(0.5f, 0.5f);
                countText.rectTransform.sizeDelta = new Vector2(keyWidth, keyHeight * 0.8f);
                countText.rectTransform.anchoredPosition =
                    new Vector2(
                        x + keyWidth / 2,
                        keyHeight / 2);
                countText.fontSize = Mathf.RoundToInt(keyWidth / 2);
                countText.gameObject.SetActive(Profile.ShowKeyPressTotal);

                // Press/release state
                Vector3 scale = new Vector3(1, 1, 1);
                if (keyPrevStates[code]) {
                    bgImage.color = Profile.PressedBackgroundColor;
                    outlineImage.color = Profile.PressedOutlineColor;
                    text.color = Profile.PressedTextColor;
                    countText.color = Profile.PressedTextColor;
                    scale *= SHRINK_FACTOR;
                } else {
                    bgImage.color = Profile.ReleasedBackgroundColor;
                    outlineImage.color = Profile.ReleasedOutlineColor;
                    text.color = Profile.ReleasedTextColor;
                    countText.color = Profile.ReleasedTextColor;
                }
                bgImage.rectTransform.localScale = scale;
                outlineImage.rectTransform.localScale = scale;
                text.rectTransform.localScale = scale;
                countText.rectTransform.localScale = scale;

                x += keyWidth + spacing;
            }
        }

        /// <summary>
        /// Updates the current state of the keys.
        /// </summary>
        /// <param name="state">The current state of the keys.</param>
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
                Text countText = keyCountTexts[code];

                // Handle key count increment
                if (state[code]) {
                    keyCounts[code]++;
                    countText.text = keyCounts[code] + "";
                }

                // Calculate the new color/size
                Color bgColor, outlineColor, textColor;
                Vector3 scale = new Vector3(1, 1, 1);
                if (state[code]) {
                    bgColor = Profile.PressedBackgroundColor;
                    outlineColor = Profile.PressedOutlineColor;
                    textColor = Profile.PressedTextColor;
                    if (Profile.AnimateKeys) {
                        scale *= SHRINK_FACTOR;
                    }
                } else {
                    bgColor = Profile.ReleasedBackgroundColor;
                    outlineColor = Profile.ReleasedOutlineColor;
                    textColor = Profile.ReleasedTextColor;
                }

                // Apply the new color/size
                bgImage.color = bgColor;
                outlineImage.color = outlineColor;
                text.color = textColor;
                countText.color = textColor;
                if (Profile.AnimateKeys) {
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
                    countText.rectTransform.DOScale(scale, EASE_DURATION)
                        .SetId(id)
                        .SetEase(Ease.OutExpo)
                        .SetUpdate(true)
                        .OnKill(() => countText.rectTransform.localScale = scale);
                } else {
                    bgImage.rectTransform.localScale = scale;
                    outlineImage.rectTransform.localScale = scale;
                    text.rectTransform.localScale = scale;
                    countText.rectTransform.localScale = scale;
                }
            }
        }

        private static string TweenIdForKeyCode(KeyCode code) {
            return $"adofai_tweaks.key_viewer.{code}";
        }

        /// <summary>
        /// Clears the current key counts
        /// </summary>
        public void ClearCounts()
        {
            foreach (KeyCode key in keyCounts.Keys.ToList())
            {
                keyCounts[key] = 0;
                keyCountTexts[key].text = "0";
            }
        }
    }
}
