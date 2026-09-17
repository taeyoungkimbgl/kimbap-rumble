using System;
using System.IO;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Scripts.Common.Core.ScreenAuthoring.Editor.ScreenAuthoringReader;
using Object = UnityEngine.Object;

namespace Scripts.Common.Core.ScreenAuthoring.Editor
{
    public static class ScreenAuthoringPreviewWriter
    {
        public static JObject Write(GameObject source, JArray resolutions)
        {
            Component<Canvas>(source);
            var sourceScaler = Component<CanvasScaler>(source);
            if (sourceScaler.uiScaleMode != CanvasScaler.ScaleMode.ScaleWithScreenSize)
                throw new ArgumentException("Static preview currently supports ScaleWithScreenSize canvases.");
            foreach (var resolution in resolutions)
            {
                var values = Numbers(resolution, 2);
                if (values[0] < 16 || values[1] < 16 || values[0] > 4096 || values[1] > 4096 ||
                    values[0] != (int)values[0] || values[1] != (int)values[1])
                    throw new ArgumentException("Preview dimensions must be integers between 16 and 4096.");
            }

            var previewScene = EditorSceneManager.NewPreviewScene();
            var defaultPipeline = GraphicsSettings.defaultRenderPipeline;
            var qualityPipeline = QualitySettings.renderPipeline;
            try
            {
                // Keep copies isolated from other canvases and from gameplay OnEnable calls.
                var host = new GameObject("ScreenAuthoringPreview");
                host.SetActive(false);
                SceneManager.MoveGameObjectToScene(host, previewScene);
                var copy = Object.Instantiate(source, host.transform);
                foreach (var behaviour in copy.GetComponentsInChildren<MonoBehaviour>(true))
                    if (behaviour != null && behaviour.GetType().Assembly != typeof(Graphic).Assembly &&
                        behaviour.GetType().Assembly != typeof(TextMeshProUGUI).Assembly)
                        behaviour.enabled = false;
                foreach (var child in copy.GetComponentsInChildren<Transform>(true))
                    child.gameObject.layer = LayerMask.NameToLayer("UI");
                var canvas = copy.GetComponent<Canvas>();
                copy.GetComponent<CanvasScaler>().enabled = false;
                canvas.renderMode = RenderMode.WorldSpace;
                var rect = copy.GetComponent<RectTransform>();
                rect.localPosition = Vector3.zero;
                rect.localRotation = Quaternion.identity;
                rect.localScale = Vector3.one;
                rect.pivot = new Vector2(0.5f, 0.5f);
                copy.SetActive(true);
                host.SetActive(true);

                var cameraObject = new GameObject("ScreenAuthoringCamera", typeof(Camera));
                SceneManager.MoveGameObjectToScene(cameraObject, previewScene);
                var camera = cameraObject.GetComponent<Camera>();
                camera.scene = previewScene;
                camera.enabled = false;
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = Color.black;
                camera.cullingMask = 1 << LayerMask.NameToLayer("UI");
                camera.orthographic = true;
                camera.transform.position = new Vector3(0, 0, -10);
                canvas.worldCamera = camera;
                // Static UI geometry only. This preview does not exercise URP or gameplay.
                GraphicsSettings.defaultRenderPipeline = null;
                QualitySettings.renderPipeline = null;
                var outputDirectory = Path.GetFullPath("Library/ScreenAuthoring/Previews/" + Guid.NewGuid().ToString("N"));
                Directory.CreateDirectory(outputDirectory);
                var images = new JArray();
                foreach (var resolution in resolutions)
                {
                    var dimensions = Numbers(resolution, 2);
                    var width = (int)dimensions[0];
                    var height = (int)dimensions[1];
                    var ratios = new Vector2(width / sourceScaler.referenceResolution.x, height / sourceScaler.referenceResolution.y);
                    float scale;
                    switch (sourceScaler.screenMatchMode)
                    {
                        case CanvasScaler.ScreenMatchMode.Expand: scale = Mathf.Min(ratios.x, ratios.y); break;
                        case CanvasScaler.ScreenMatchMode.Shrink: scale = Mathf.Max(ratios.x, ratios.y); break;
                        default:
                            scale = Mathf.Pow(2, Mathf.Lerp(Mathf.Log(ratios.x, 2), Mathf.Log(ratios.y, 2), sourceScaler.matchWidthOrHeight));
                            break;
                    }
                    canvas.scaleFactor = scale;
                    rect.sizeDelta = new Vector2(width, height) / scale;
                    camera.aspect = (float)width / height;
                    camera.orthographicSize = rect.sizeDelta.y / 2;
                    Canvas.ForceUpdateCanvases();
                    var warnings = LayoutWarnings(canvas, camera);
                    var path = Path.Combine(outputDirectory, width + "x" + height + ".png");
                    Render(camera, width, height, path);
                    images.Add(new JObject { ["path"] = path, ["width"] = width, ["height"] = height, ["warnings"] = warnings });
                }
                return new JObject { ["mode"] = "static-layout", ["images"] = images };
            }
            finally
            {
                GraphicsSettings.defaultRenderPipeline = defaultPipeline;
                QualitySettings.renderPipeline = qualityPipeline;
                EditorSceneManager.ClosePreviewScene(previewScene);
            }
        }

        private static JArray LayoutWarnings(Canvas canvas, Camera camera)
        {
            var warnings = new JArray();
            foreach (var text in canvas.GetComponentsInChildren<TextMeshProUGUI>())
                if (text.preferredWidth > text.rectTransform.rect.width + 1 || text.preferredHeight > text.rectTransform.rect.height + 1)
                    warnings.Add("Text may overflow: " + PathOf(text.transform));
            var corners = new Vector3[4];
            foreach (var button in canvas.GetComponentsInChildren<Button>())
            {
                button.GetComponent<RectTransform>().GetWorldCorners(corners);
                foreach (var corner in corners)
                {
                    var point = camera.WorldToViewportPoint(corner);
                    if (point.x < -0.001f || point.x > 1.001f || point.y < -0.001f || point.y > 1.001f)
                    {
                        warnings.Add("Button outside preview: " + PathOf(button.transform));
                        break;
                    }
                }
            }
            return warnings;
        }

        private static void Render(Camera camera, int width, int height, string path)
        {
            var previous = RenderTexture.active;
            var target = RenderTexture.GetTemporary(width, height, 24);
            var image = new Texture2D(width, height, TextureFormat.RGB24, false);
            try
            {
                camera.targetTexture = target;
                camera.Render();
                RenderTexture.active = target;
                image.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                image.Apply();
                File.WriteAllBytes(path, image.EncodeToPNG());
            }
            finally
            {
                RenderTexture.active = previous;
                camera.targetTexture = null;
                Object.DestroyImmediate(image);
                RenderTexture.ReleaseTemporary(target);
            }
        }
    }
}
