using System;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

namespace Scripts.Features.GameScene.HomeScreen.Editor
{
    // Authoring only: the saved scene has no runtime UI generation dependency.
    public static class HomeScreenFactory
    {
        private const string ScenePath = "Assets/Scenes/GameScene.unity";

        [MenuItem("Kimbap Rumble/Home/Create Greybox")]
        public static void Create()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            var scene = EditorSceneManager.OpenScene(ScenePath);
            var canvas = GameObject.Find("CanvasHome").GetComponent<Canvas>();
            if (canvas.transform.childCount != 0)
                throw new InvalidOperationException("CanvasHome already contains UI. Edit the existing layout in the Inspector.");

            var scaler = canvas.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            canvas.transform.localScale = Vector3.one;

            var background = Panel("Background", canvas.transform, Vector2.zero, new Vector2(1920, 1080), 0.16f);
            background.anchorMin = Vector2.zero;
            background.anchorMax = Vector2.one;
            background.sizeDelta = Vector2.zero;

            var root = Rect("HomeReady", canvas.transform, Vector2.zero, new Vector2(1920, 1080));
            Label("Title", root, "HOME", new Vector2(-720, 424), new Vector2(256, 72), 40);
            Button("AlbumButton", root, "ALBUM", new Vector2(464, 424), new Vector2(240, 72), 32);
            Button("SettingsButton", root, "SETTINGS", new Vector2(736, 424), new Vector2(240, 72), 32);

            var character = Panel("CharacterPlaceholder", root, new Vector2(0, 64), new Vector2(640, 640), 0.25f);
            Label("Label", character, "KIMBAP", Vector2.zero, new Vector2(480, 72), 40);
            Slot(root, "Weapon", new Vector2(-720, 208));
            Slot(root, "Shield", new Vector2(-720, -144));
            Slot(root, "Accessory", new Vector2(720, 32));
            Button("RumbleButton", root, "RUMBLE!", new Vector2(0, -408), new Vector2(480, 120), 56);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Verify();
        }

        private static RectTransform Rect(string name, Transform parent, Vector2 position, Vector2 size)
        {
            var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rect.gameObject.layer = LayerMask.NameToLayer("UI");
            rect.SetParent(parent, false);
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            return rect;
        }

        private static RectTransform Panel(string name, Transform parent, Vector2 position, Vector2 size, float grey)
        {
            var rect = Rect(name, parent, position, size);
            var image = rect.gameObject.AddComponent<Image>();
            image.color = new Color(grey, grey, grey, 1);
            image.raycastTarget = false;
            return rect;
        }

        private static void Label(string name, Transform parent, string value, Vector2 position, Vector2 size, int fontSize)
        {
            var rect = Rect(name, parent, position, size);
            var text = rect.gameObject.AddComponent<TextMeshProUGUI>();
            text.font = TMP_Settings.defaultFontAsset;
            text.text = value;
            text.fontSize = fontSize;
            text.alignment = TextAlignmentOptions.Center;
            text.textWrappingMode = TextWrappingModes.Normal;
            text.overflowMode = TextOverflowModes.Truncate;
            text.color = new Color(0.94f, 0.94f, 0.94f, 1);
            text.raycastTarget = false;
        }

        private static RectTransform Button(string name, Transform parent, string label, Vector2 position, Vector2 size, int fontSize)
        {
            var rect = Panel(name, parent, position, size, 0.36f);
            var image = rect.GetComponent<Image>();
            image.raycastTarget = true;
            var button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.transition = Selectable.Transition.None;
            button.navigation = new Navigation { mode = Navigation.Mode.None };
            if (label.Length > 0) Label("Label", rect, label, Vector2.zero, size, fontSize);
            return rect;
        }

        private static void Slot(Transform parent, string category, Vector2 position)
        {
            var slot = Button(category + "Slot", parent, "", position, new Vector2(256, 320), 32);
            Label("Heading", slot, category.ToUpperInvariant(), new Vector2(0, 112), new Vector2(256, 48), 32);
            var icon = Panel("IconPlaceholder", slot, new Vector2(0, -16), new Vector2(192, 192), 0.25f);
            Label("Label", icon, category.ToUpperInvariant(), Vector2.zero, new Vector2(192, 48), 24);
        }

        // Preview static UI geometry with a temporary built-in camera in batchmode.
        // This does not exercise the game's URP rendering or screen transitions.
        public static void VerifyBatch()
        {
            EditorSceneManager.OpenScene(ScenePath);
            Verify();
        }

        [MenuItem("Kimbap Rumble/Home/Verify Greybox")]
        public static void Verify()
        {
            var canvas = UnityEngine.Object.Instantiate(GameObject.Find("CanvasHome")).GetComponent<Canvas>();
            canvas.gameObject.hideFlags = HideFlags.HideAndDontSave;
            canvas.gameObject.layer = LayerMask.NameToLayer("UI");
            var defaultPipeline = GraphicsSettings.defaultRenderPipeline;
            var qualityPipeline = QualitySettings.renderPipeline;
            GraphicsSettings.defaultRenderPipeline = null;
            QualitySettings.renderPipeline = null;
            var cameraObject = new GameObject("HomePreviewCamera", typeof(Camera));
            var camera = cameraObject.GetComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Color.black;
            camera.cullingMask = 1 << LayerMask.NameToLayer("UI");
            camera.orthographic = true;
            camera.transform.position = new Vector3(0, 0, -10);
            canvas.renderMode = RenderMode.WorldSpace;
            var canvasRect = canvas.GetComponent<RectTransform>();
            canvasRect.localScale = Vector3.one;
            canvasRect.position = Vector3.zero;
            canvasRect.pivot = new Vector2(0.5f, 0.5f);
            canvas.worldCamera = camera;
            canvas.planeDistance = 1;
            try
            {
                foreach (var size in new[] { new Vector2Int(1920, 1080), new Vector2Int(1280, 720), new Vector2Int(2560, 1080) })
                {
                    var target = RenderTexture.GetTemporary(size.x, size.y, 24);
                    camera.targetTexture = target;
                    var scaler = canvas.GetComponent<CanvasScaler>();
                    scaler.enabled = false;
                    canvas.scaleFactor = Mathf.Min(size.x / 1920f, size.y / 1080f);
                    canvasRect.sizeDelta = new Vector2(size.x, size.y) / canvas.scaleFactor;
                    camera.orthographicSize = canvasRect.sizeDelta.y / 2;
                    camera.aspect = (float)size.x / size.y;
                    Canvas.ForceUpdateCanvases();
                    foreach (var text in canvas.GetComponentsInChildren<TextMeshProUGUI>())
                        if (text.preferredWidth > text.rectTransform.rect.width + 1 || text.preferredHeight > text.rectTransform.rect.height + 1)
                            throw new InvalidOperationException("Text does not fit: " + text.transform.parent.name);
                    foreach (var button in canvas.GetComponentsInChildren<Button>())
                    {
                        var corners = new Vector3[4];
                        button.GetComponent<RectTransform>().GetWorldCorners(corners);
                        foreach (var corner in corners)
                        {
                            var point = camera.WorldToViewportPoint(corner);
                            if (point.x < 0 || point.x > 1 || point.y < 0 || point.y > 1)
                                throw new InvalidOperationException("Button outside viewport: " + button.name);
                        }
                    }
                    camera.Render();
                    var previous = RenderTexture.active;
                    RenderTexture.active = target;
                    var image = new Texture2D(size.x, size.y, TextureFormat.RGB24, false);
                    image.ReadPixels(new Rect(0, 0, size.x, size.y), 0, 0);
                    image.Apply();
                    if (image.GetPixel(size.x / 2, size.y / 2).maxColorComponent < 0.05f)
                        throw new InvalidOperationException("Preview did not render the Home UI.");
                    File.WriteAllBytes("/private/tmp/home-greybox-" + size.x + "x" + size.y + ".png", image.EncodeToPNG());
                    UnityEngine.Object.DestroyImmediate(image);
                    RenderTexture.active = previous;
                    camera.targetTexture = null;
                    RenderTexture.ReleaseTemporary(target);
                    scaler.enabled = true;
                    Debug.Log("Home greybox verified: " + size);
                }
            }
            finally
            {
                RenderTexture.active = null;
                camera.targetTexture = null;
                UnityEngine.Object.DestroyImmediate(canvas.gameObject);
                UnityEngine.Object.DestroyImmediate(cameraObject);
                GraphicsSettings.defaultRenderPipeline = defaultPipeline;
                QualitySettings.renderPipeline = qualityPipeline;
            }
        }
    }
}
