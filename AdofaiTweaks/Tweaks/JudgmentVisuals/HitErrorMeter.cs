using AdofaiTweaks.Core;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace AdofaiTweaks.Tweaks.JudgmentVisuals
{
    /// <summary>
    /// A curved meter that shows the user how off their hits are from the
    /// actual tile timing.
    /// </summary>
    internal class HitErrorMeter : MonoBehaviour
    {
        private static HitErrorMeter _instance;

        /// <summary>
        /// A singleton instance of <see cref="HitErrorMeter"/>.
        /// </summary>
        public static HitErrorMeter Instance { get => _instance; }

        private readonly Color MISS_COLOR = new Color32(0xff, 0x00, 0x00, 0xff);
        private readonly Color COUNTED_COLOR = new Color32(0xff, 0x6f, 0x4d, 0xff);
        private readonly Color NEAR_COLOR = new Color32(0xfc, 0xff, 0x4d, 0xff);
        private readonly Color PERFECT_COLOR = new Color32(0x5f, 0xff, 0x4e, 0xff);
        private readonly string TWEEN_ID = "adofai_tweaks.hit_error_meter";
        private const int TICK_CACHE_SIZE = 60;

        private CanvasScaler scaler;

        private GameObject wrapperObj;
        private Canvas wrapperCanvas;
        private RectTransform wrapperRectTransform;

        private Image handImage;

        private float averageAngle;

        private GameObject[] cachedTicks;
        private string[] cachedTweenIds;
        private int tickIndex;

        private JudgmentVisualsSettings _settings = new JudgmentVisualsSettings();

        /// <summary>
        /// A singleton instance of <see cref="JudgmentVisualsSettings"/>.
        /// </summary>
        public JudgmentVisualsSettings Settings {
            get => _settings;
            set {
                _settings = value;
                UpdateLayout();
            }
        }

        /// <summary>
        /// Unity's Awake lifecycle event handler. Creates the meter, hand, and
        /// cached ticks.
        /// </summary>
        protected void Awake() {
            _instance = this;

            Canvas rootCanvas = gameObject.AddComponent<Canvas>();
            rootCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            rootCanvas.sortingOrder = 10000;
            scaler = gameObject.AddComponent<CanvasScaler>();

            wrapperObj = new GameObject();
            wrapperObj.transform.SetParent(transform);
            wrapperCanvas = wrapperObj.AddComponent<Canvas>();
            wrapperRectTransform = wrapperObj.GetComponent<RectTransform>();
            wrapperRectTransform.anchoredPosition = new Vector2(0, -48);
            wrapperRectTransform.sizeDelta = new Vector2(334, 135);

            GenerateMeterPng();

            cachedTicks = new GameObject[TICK_CACHE_SIZE];
            cachedTweenIds = new string[TICK_CACHE_SIZE];
            for (int i = 0; i < TICK_CACHE_SIZE; i++) {
                cachedTicks[i] = new GameObject();
                cachedTicks[i].transform.SetParent(wrapperObj.transform);
                Image tickImage = cachedTicks[i].AddComponent<Image>();
                tickImage.sprite = TweakAssets.TickSprite;
                tickImage.rectTransform.anchorMin = new Vector2(0.5f, 0f);
                tickImage.rectTransform.anchorMax = new Vector2(0.5f, 0f);
                tickImage.rectTransform.pivot = new Vector2(0.5f, 0f);
                tickImage.rectTransform.anchoredPosition = Vector2.zero;
                tickImage.rectTransform.sizeDelta = new Vector2(8, 182);
                tickImage.color = Color.clear;
                cachedTweenIds[i] = TWEEN_ID + "_tick_" + i;
            }

            UpdateLayout();

            gameObject.SetActive(false);
        }

        /// <summary>
        /// Unity's OnDestroy lifecycle event handler. Deletes the singleton
        /// instance.
        /// </summary>
        protected void OnDestroy() {
            _instance = null;
        }

        private void GenerateMeterPng() {
            GameObject meterObj = new GameObject();
            meterObj.transform.SetParent(wrapperObj.transform);
            Image image = meterObj.AddComponent<Image>();
            image.sprite = TweakAssets.MeterSprite;
            image.rectTransform.anchorMin = new Vector2(0.5f, 0f);
            image.rectTransform.anchorMax = new Vector2(0.5f, 0f);
            image.rectTransform.pivot = new Vector2(0.5f, 0f);
            image.rectTransform.anchoredPosition = Vector2.zero;
            image.rectTransform.sizeDelta = new Vector2(400, 200);

            GameObject handObj = new GameObject();
            handObj.transform.SetParent(wrapperObj.transform);
            handImage = handObj.AddComponent<Image>();
            handImage.sprite = TweakAssets.HandSprite;
            handImage.rectTransform.anchorMin = new Vector2(0.5f, 0f);
            handImage.rectTransform.anchorMax = new Vector2(0.5f, 0f);
            handImage.rectTransform.pivot = new Vector2(0.5f, 0f);
            handImage.rectTransform.anchoredPosition = Vector2.zero;
            handImage.rectTransform.sizeDelta = new Vector2(30, 140);
        }

        /// <summary>
        /// Updates the size and position of the hit error meter.
        /// </summary>
        public void UpdateLayout() {
            Vector2 pos = new Vector2(Settings.ErrorMeterXPos, Settings.ErrorMeterYPos);
            wrapperRectTransform.anchorMin = pos;
            wrapperRectTransform.anchorMax = pos;
            wrapperRectTransform.pivot = pos;
            scaler.scaleFactor = Settings.ErrorMeterScale;
        }

        /// <summary>
        /// Displays a tick with the given angle difference and moves the hand
        /// towards it.
        /// </summary>
        /// <param name="angleDiff">
        /// The angle difference between the hit and the tile.
        /// </param>
        public void AddHit(float angleDiff) {
            angleDiff *= -Mathf.Rad2Deg;

            // Scale with BPM
            double bpmTimesSpeed = scrConductor.instance.bpm * scrController.instance.speed;
            double conductorPitch = scrConductor.instance.song.pitch;
            double counted =
                scrMisc.GetAdjustedAngleBoundaryInDeg(
                    HitMarginGeneral.Counted, bpmTimesSpeed, conductorPitch);
            angleDiff *= (float)(60 / counted);

            // Clamp to meter range
            if (angleDiff < -60) {
                angleDiff = -60.0001f - Random.value * 3;
            }
            if (angleDiff > 60) {
                angleDiff = 60.0001f + Random.value * 3;
            }

            // Update average
            if (angleDiff >= -60 && angleDiff <= 60) {
                averageAngle = Mathf.Lerp(averageAngle, angleDiff, Settings.ErrorMeterSensitivity);
            }

            // Move hand
            handImage.rectTransform
                .DORotateQuaternion(Quaternion.Euler(0, 0, averageAngle), 0.25f)
                .SetEase(Ease.OutCubic)
                .SetId(TWEEN_ID);

            // Draw tick
            DrawTick(angleDiff);
        }

        /// <summary>
        /// Clears all ticks and moves the hand back to the center.
        /// </summary>
        public void Reset() {
            DOTween.Kill(TWEEN_ID);
            foreach (string id in cachedTweenIds) {
                DOTween.Kill(id);
            }
            averageAngle = 0;
            if (handImage) {
                handImage.rectTransform.rotation = Quaternion.identity;
            }
        }

        private void DrawTick(float angle) {
            // Determine the color
            double bpmTimesSpeed = scrConductor.instance.bpm * scrController.instance.speed;
            double conductorPitch = scrConductor.instance.song.pitch;
            double counted =
                scrMisc.GetAdjustedAngleBoundaryInDeg(
                    HitMarginGeneral.Counted, bpmTimesSpeed, conductorPitch);
            double perfect =
                scrMisc.GetAdjustedAngleBoundaryInDeg(
                    HitMarginGeneral.Perfect, bpmTimesSpeed, conductorPitch);
            double pure =
                scrMisc.GetAdjustedAngleBoundaryInDeg(
                    HitMarginGeneral.Pure, bpmTimesSpeed, conductorPitch);
            double scale = 60 / counted;
            counted *= scale;
            perfect *= scale;
            pure *= scale;
            Color color;
            if (angle < -counted) {
                color = MISS_COLOR;
            } else if (angle < -perfect) {
                color = COUNTED_COLOR;
            } else if (angle < -pure) {
                color = NEAR_COLOR;
            } else if (angle <= pure) {
                color = PERFECT_COLOR;
            } else if (angle <= perfect) {
                color = NEAR_COLOR;
            } else if (angle <= counted) {
                color = COUNTED_COLOR;
            } else {
                color = MISS_COLOR;
            }

            // Draw the tick, fade after 4 seconds
            GameObject tickObj = cachedTicks[tickIndex];
            string id = cachedTweenIds[tickIndex];
            tickIndex = (tickIndex + 1) % TICK_CACHE_SIZE;
            Image tickImage = tickObj.GetComponent<Image>();
            tickImage.rectTransform.rotation = Quaternion.Euler(0, 0, angle);
            DOTween.Kill(id);
            tickImage.color = color;
            tickImage
                .DOColor(color.WithAlpha(0), Settings.ErrorMeterTickLife)
                .SetEase(Ease.InQuad)
                .SetId(id)
                .OnKill(() => tickImage.color = Color.clear);
        }
    }
}
