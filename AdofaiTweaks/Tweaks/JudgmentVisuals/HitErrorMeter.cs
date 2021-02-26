using System.IO;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace AdofaiTweaks.Tweaks.JudgmentVisuals
{
    internal class HitErrorMeter : MonoBehaviour
    {
        private const float INNER_RADIUS = 100;
        private const float OUTER_RADIUS = 110;
        private const int NUM_SECTIONS = 36;

        private readonly Vector2 MISS_UV = new Vector2(0, 0);
        private readonly Vector2 COUNTED_UV = new Vector2(1, 0);
        private readonly Vector2 PERFECT_UV = new Vector2(0, 1);
        private readonly Vector2 PURE_UV = new Vector2(1, 1);

        private Material material;
        private Mesh arc;
        private CanvasRenderer renderer;
        private readonly Vector2[] uvs = new Vector2[NUM_SECTIONS * 2 + 2];

        private Image handImage;

        public void Start() {
            Canvas canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scalar = gameObject.AddComponent<CanvasScaler>();
            scalar.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scalar.referenceResolution = new Vector2(1920, 1080);
            gameObject.AddComponent<GraphicRaycaster>();

            //GenerateMeterMesh();
            GenerateMeterPng();
        }

        private void GenerateMeterPng() {
            Sprite meterSprite =
                scrExtImgHolder.LoadNewSprite(Path.Combine("Mods", "AdofaiTweaks", "Meter.png"));
            GameObject meterObj = new GameObject();
            meterObj.transform.SetParent(transform);
            Image image = meterObj.AddComponent<Image>();
            image.sprite = meterSprite;
            image.rectTransform.anchoredPosition = new Vector2(0, -400);
            image.rectTransform.sizeDelta = new Vector2(300, 200);
            Sprite handSprite =
                scrExtImgHolder.LoadNewSprite(Path.Combine("Mods", "AdofaiTweaks", "Hand.png"));
            GameObject handObj = new GameObject();
            handObj.transform.SetParent(transform);
            handImage = handObj.AddComponent<Image>();
            handImage.sprite = handSprite;
            handImage.rectTransform.pivot = new Vector2(0.5f, 0.21f);
            handImage.rectTransform.anchoredPosition = new Vector2(0, -460);
            handImage.rectTransform.sizeDelta = new Vector2(30, 150);
        }

        private void GenerateMeterMesh() {
            GameObject meshObj = new GameObject();
            meshObj.transform.SetParent(transform);
            RectTransform rectTransform = meshObj.AddComponent<RectTransform>();
            renderer = meshObj.AddComponent<CanvasRenderer>();
            rectTransform.anchoredPosition = new Vector2(0, -500);
            rectTransform.localScale = new Vector3(1, 1, 1);

            arc = new Mesh();
            Vector3[] verts = new Vector3[NUM_SECTIONS * 2 + 2];
            for (int i = 0; i <= NUM_SECTIONS; i++) {
                float angle =
                    Mathf.Lerp(-Mathf.PI / 3 - 0.1f, Mathf.PI / 3 + 0.1f, (float)i / NUM_SECTIONS);
                verts[i * 2] =
                    new Vector3(
                        OUTER_RADIUS * Mathf.Sin(angle), OUTER_RADIUS * Mathf.Cos(angle), 100);
                verts[i * 2 + 1] =
                    new Vector3(
                        INNER_RADIUS * Mathf.Sin(angle), INNER_RADIUS * Mathf.Cos(angle), 100);
            }
            arc.vertices = verts;
            for (int i = 0; i <= NUM_SECTIONS; i++) {
                uvs[i * 2] = PURE_UV;
                uvs[i * 2 + 1] = PURE_UV;
            }
            arc.uv = uvs;
            int[] tris = new int[NUM_SECTIONS * 6];
            for (int i = 0; i < NUM_SECTIONS; i++) {
                int p1 = i * 6;
                int p2 = i * 6 + 3;
                tris[p1] = i * 2;
                tris[p1 + 1] = i * 2 + 2;
                tris[p1 + 2] = i * 2 + 1;
                tris[p2] = i * 2 + 2;
                tris[p2 + 1] = i * 2 + 3;
                tris[p2 + 2] = i * 2 + 1;
            }
            arc.triangles = tris;

            material = new Material(Shader.Find("ADOFAI/ScrollingSprite"));
            Texture2D texture = new Texture2D(2, 2, TextureFormat.ARGB32, false) {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Point,
            };
            ColorUtility.TryParseHtmlString("#FF0000", out Color color);
            texture.SetPixel(0, 0, color);
            ColorUtility.TryParseHtmlString("#FF6F4D", out color);
            texture.SetPixel(1, 0, color);
            ColorUtility.TryParseHtmlString("#A0FF4D", out color);
            texture.SetPixel(0, 1, color);
            ColorUtility.TryParseHtmlString("#5FFF4E", out color);
            texture.SetPixel(1, 1, color);
            texture.Apply();
            material.mainTexture = texture;
            material.SetVector("_ScrollSpeed", new Vector2(0, 0));
            material.SetFloat("_Time0", 0);

            renderer.SetMaterial(material, null);
            renderer.SetMesh(arc);
        }

        public void Update() {
            //UpdateMeterMesh();
        }

        private void UpdateMeterMesh() {
            if (!scrConductor.instance || !scrController.instance) {
                return;
            }
            double bpmTimesSpeed = scrConductor.instance.bpm * scrController.instance.speed;
            double conductorPitch = scrConductor.instance.song.pitch;
            double counted =
                scrMisc.GetAdjustedAngleBoundaryInDeg(
                    HitMarginGeneral.Counted, bpmTimesSpeed, conductorPitch) * Mathf.PI / 180;
            double perfect =
                scrMisc.GetAdjustedAngleBoundaryInDeg(
                    HitMarginGeneral.Perfect, bpmTimesSpeed, conductorPitch) * Mathf.PI / 180;
            double pure =
                scrMisc.GetAdjustedAngleBoundaryInDeg(
                    HitMarginGeneral.Pure, bpmTimesSpeed, conductorPitch) * Mathf.PI / 180;
            double scale = Mathf.PI / 3 / counted;
            counted *= scale;
            perfect *= scale;
            pure *= scale;
            for (int i = 0; i <= NUM_SECTIONS; i++) {
                float angle =
                    Mathf.Lerp(-Mathf.PI / 3 - 0.1f, Mathf.PI / 3 + 0.1f, (float)i / NUM_SECTIONS);
                if (angle < -counted) {
                    uvs[i * 2] = MISS_UV;
                    uvs[i * 2 + 1] = MISS_UV;
                } else if (angle < -perfect) {
                    uvs[i * 2] = COUNTED_UV;
                    uvs[i * 2 + 1] = COUNTED_UV;
                } else if (angle < -pure) {
                    uvs[i * 2] = PERFECT_UV;
                    uvs[i * 2 + 1] = PERFECT_UV;
                } else if (angle <= pure) {
                    uvs[i * 2] = PURE_UV;
                    uvs[i * 2 + 1] = PURE_UV;
                } else if (angle <= perfect) {
                    uvs[i * 2] = PERFECT_UV;
                    uvs[i * 2 + 1] = PERFECT_UV;
                } else if (angle <= counted) {
                    uvs[i * 2] = COUNTED_UV;
                    uvs[i * 2 + 1] = COUNTED_UV;
                } else {
                    uvs[i * 2] = MISS_UV;
                    uvs[i * 2 + 1] = MISS_UV;
                }
            }
            arc.uv = uvs;
            renderer.SetMaterial(material, null);
            renderer.SetMesh(arc);
        }
    }
}
