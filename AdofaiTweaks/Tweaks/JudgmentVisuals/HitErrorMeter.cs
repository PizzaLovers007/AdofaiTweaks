using System.IO;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace AdofaiTweaks.Tweaks.JudgmentVisuals
{
    internal class HitErrorMeter : MonoBehaviour
    {
        private static HitErrorMeter _instance;
        public static HitErrorMeter Instance { get => _instance; }

        private readonly Color MISS_COLOR = new Color32(0xff, 0x00, 0x00, 0xff);
        private readonly Color COUNTED_COLOR = new Color32(0xff, 0x6f, 0x4d, 0xff);
        private readonly Color NEAR_COLOR = new Color32(0xfc, 0xff, 0x4d, 0xff);
        private readonly Color PERFECT_COLOR = new Color32(0x5f, 0xff, 0x4e, 0xff);
        private readonly string TWEEN_ID = "adofai_tweaks.hit_error_meter";

        private CanvasScaler scalar;

        private Image handImage;
        private Sprite tickSprite;

        private float averageAngle;

        public float Scale {
            get {
                return scalar.scaleFactor;
            }
            set {
                scalar.scaleFactor = value;
            }
        }

        public JudgmentVisualsSettings Settings { get; set; }

        protected void Awake() {
            _instance = this;
        }

        protected void OnDestroy() {
            _instance = null;
        }

        protected void Start() {
            Canvas canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10000;
            scalar = gameObject.AddComponent<CanvasScaler>();

            GenerateMeterPng();
            gameObject.SetActive(false);
        }

        private void GenerateMeterPng() {
            Sprite meterSprite =
                scrExtImgHolder.LoadNewSprite(Path.Combine("Mods", "AdofaiTweaks", "Meter.png"));
            Sprite handSprite =
                scrExtImgHolder.LoadNewSprite(Path.Combine("Mods", "AdofaiTweaks", "Hand.png"));
            tickSprite =
                scrExtImgHolder.LoadNewSprite(Path.Combine("Mods", "AdofaiTweaks", "Tick.png"));

            GameObject meterObj = new GameObject();
            meterObj.transform.SetParent(transform);
            Image image = meterObj.AddComponent<Image>();
            image.sprite = meterSprite;
            image.rectTransform.anchorMin = new Vector2(0.5f, 0f);
            image.rectTransform.anchorMax = new Vector2(0.5f, 0f);
            image.rectTransform.pivot = new Vector2(0.5f, 0f);
            image.rectTransform.sizeDelta = new Vector2(400, 200);
            image.rectTransform.anchoredPosition = new Vector2(0, -10);

            GameObject handObj = new GameObject();
            handObj.transform.SetParent(transform);
            handImage = handObj.AddComponent<Image>();
            handImage.sprite = handSprite;
            handImage.rectTransform.anchorMin = new Vector2(0.5f, 0f);
            handImage.rectTransform.anchorMax = new Vector2(0.5f, 0f);
            handImage.rectTransform.pivot = new Vector2(0.5f, 0f);
            handImage.rectTransform.sizeDelta = new Vector2(30, 140);
            handImage.rectTransform.anchoredPosition = new Vector2(0, -10);
        }

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
                averageAngle = Mathf.Lerp(averageAngle, angleDiff, 0.2f);
            }

            // Move hand
            handImage.rectTransform
                .DORotateQuaternion(Quaternion.Euler(0, 0, averageAngle), 0.25f)
                .SetEase(Ease.OutCubic)
                .SetId(TWEEN_ID);

            // Draw tick
            DrawTick(angleDiff);
        }

        public void Reset() {
            DOTween.Kill(TWEEN_ID);
            averageAngle = 0;
            handImage.rectTransform.rotation = Quaternion.identity;
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
            GameObject tickObj = new GameObject();
            tickObj.transform.SetParent(transform);
            Image tickImage = tickObj.AddComponent<Image>();
            tickImage.sprite = tickSprite;
            tickImage.rectTransform.anchorMin = new Vector2(0.5f, 0f);
            tickImage.rectTransform.anchorMax = new Vector2(0.5f, 0f);
            tickImage.rectTransform.pivot = new Vector2(0.5f, 0f);
            tickImage.rectTransform.sizeDelta = new Vector2(8, 182);
            tickImage.rectTransform.anchoredPosition = new Vector2(0, -10);
            tickImage.rectTransform.rotation = Quaternion.Euler(0, 0, angle);
            tickImage.color = color;
            tickImage
                .DOColor(color.WithAlpha(0), 4f)
                .SetEase(Ease.InQuad)
                .SetId(TWEEN_ID)
                .OnKill(() => Destroy(tickObj));
        }
    }
}
