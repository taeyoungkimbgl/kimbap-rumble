using System;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Scripts.Features.GameScene.InventoryScreen.Editor
{
    // Authoring only. All UI is saved in the scene; no runtime generation is required.
    public static class InventoryScreenFactory
    {
        private const string ScenePath = "Assets/Scenes/GameScene.unity";
        private static readonly Vector2 ReferenceSize = new Vector2(1920, 1080);

        [MenuItem("Kimbap Rumble/Inventory/Create Greybox")]
        public static void Create()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            var scene = EditorSceneManager.OpenScene(ScenePath);
            var canvases = GameObject.Find("Canvases").transform;
            if (canvases.Find("CanvasInventory") != null)
                throw new InvalidOperationException("CanvasInventory already exists. Edit the saved layout in the Inspector.");

            var canvasRect = Rect("CanvasInventory", canvases, Vector2.zero, ReferenceSize);
            var canvas = canvasRect.gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasRect.gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = ReferenceSize;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            canvasRect.gameObject.AddComponent<GraphicRaycaster>();

            var background = Panel("Background", canvasRect, Vector2.zero, ReferenceSize, 0.16f);
            background.anchorMin = Vector2.zero;
            background.anchorMax = Vector2.one;
            background.sizeDelta = Vector2.zero;
            var root = Rect("PanelInventory", canvasRect, Vector2.zero, ReferenceSize);

            var tabs = Rect("CategoryTabs", root, new Vector2(-504, 444), new Vector2(752, 64));
            var weaponTab = Button("WeaponTab", tabs, "WEAPON", new Vector2(-256, 0), new Vector2(240, 64));
            Border(weaponTab, 8, 0.86f);
            Button("ShieldTab", tabs, "SHIELD", Vector2.zero, new Vector2(240, 64));
            Button("AccessoryTab", tabs, "ACCESSORY", new Vector2(256, 0), new Vector2(240, 64));

            var list = Panel("EquipmentListPanel", root, new Vector2(-336, 8), new Vector2(1088, 760), 0.22f);
            var gridRect = Rect("EquipmentGrid", list, Vector2.zero, new Vector2(1008, 680));
            var grid = gridRect.gameObject.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(240, 240);
            grid.spacing = new Vector2(16, 24);
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 4;
            grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
            grid.startAxis = GridLayoutGroup.Axis.Horizontal;
            grid.childAlignment = TextAnchor.UpperLeft;
            // MVP has two items per category. Unowned/empty cards are not shown.
            Card(gridRect, "WeaponA", "Weapon A", true, false);
            Card(gridRect, "WeaponB", "Weapon B", false, true);

            var preview = Panel("PreviewPanel", root, new Vector2(568, 8), new Vector2(624, 760), 0.22f);
            var character = Panel("CharacterPlaceholder", preview, new Vector2(0, 64), new Vector2(512, 512), 0.25f);
            Label("Label", character, "KIMBAP", Vector2.zero, new Vector2(384, 72), 40);
            Label("EquipmentName", preview, "Weapon B", new Vector2(0, -240), new Vector2(512, 48), 32);
            Label("SelectionLabel", preview, "Selected", new Vector2(0, -304), new Vector2(512, 32), 24);

            var actions = Rect("Actions", root, new Vector2(568, -440), new Vector2(504, 72));
            Button("BackButton", actions, "BACK", new Vector2(-132, 0), new Vector2(240, 72));
            Button("EquipButton", actions, "EQUIP", new Vector2(132, 0), new Vector2(240, 72));

            // Screen switching is connected later; preserve the existing initial screen.
            canvas.gameObject.SetActive(false);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("Inventory greybox saved: Canvases/CanvasInventory/PanelInventory (inactive).");
        }

        public static void CreateBatch()
        {
            Create();
            VerifyBatch();
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

        private static RectTransform Button(string name, Transform parent, string label, Vector2 position, Vector2 size)
        {
            var rect = Panel(name, parent, position, size, 0.36f);
            var image = rect.GetComponent<Image>();
            image.raycastTarget = true;
            var button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.transition = Selectable.Transition.None;
            button.navigation = new Navigation { mode = Navigation.Mode.None };
            if (label.Length > 0) Label("Label", rect, label, Vector2.zero, size, 32);
            return rect;
        }

        private static void Border(RectTransform parent, float width, float grey)
        {
            var size = parent.sizeDelta;
            Panel("BorderTop", parent, new Vector2(0, (size.y - width) / 2), new Vector2(size.x, width), grey);
            Panel("BorderBottom", parent, new Vector2(0, -(size.y - width) / 2), new Vector2(size.x, width), grey);
            Panel("BorderLeft", parent, new Vector2(-(size.x - width) / 2, 0), new Vector2(width, size.y - 2 * width), grey);
            Panel("BorderRight", parent, new Vector2((size.x - width) / 2, 0), new Vector2(width, size.y - 2 * width), grey);
        }

        private static void Card(Transform parent, string name, string label, bool equipped, bool selected)
        {
            var card = Button(name, parent, "", Vector2.zero, new Vector2(240, 240));
            Border(card, selected ? 8 : 4, selected ? 0.86f : 0.48f);
            var icon = Panel("IconPlaceholder", card, new Vector2(0, 8), new Vector2(160, 160), 0.25f);
            Label("Label", icon, "WEAPON", Vector2.zero, new Vector2(160, 32), 24);
            Label("EquipmentName", card, label, new Vector2(0, -96), new Vector2(192, 32), 24);
            if (equipped)
            {
                var badge = Panel("EquippedBadge", card, new Vector2(-40, 96), new Vector2(128, 32), 0.18f);
                Label("Label", badge, "Equipped", Vector2.zero, new Vector2(128, 32), 24);
            }
            if (selected)
            {
                var check = Panel("SelectedCheck", card, new Vector2(88, 88), new Vector2(40, 40), 0.86f);
                var iconRect = Rect("CheckIcon", check, Vector2.zero, new Vector2(32, 32));
                var shortStroke = Panel("ShortStroke", iconRect, new Vector2(-8, -4), new Vector2(8, 16), 0.16f);
                shortStroke.localRotation = Quaternion.Euler(0, 0, 45);
                var longStroke = Panel("LongStroke", iconRect, new Vector2(4, 0), new Vector2(8, 24), 0.16f);
                longStroke.localRotation = Quaternion.Euler(0, 0, -45);
            }
        }

        public static void VerifyBatch()
        {
            EditorSceneManager.OpenScene(ScenePath);
            Verify();
        }

        // Static geometry preview only; normal URP Game View and input are checked separately.
        [MenuItem("Kimbap Rumble/Inventory/Verify Greybox")]
        public static void Verify()
        {
            var source = GameObject.Find("Canvases").transform.Find("CanvasInventory");
            var canvas = UnityEngine.Object.Instantiate(source.gameObject).GetComponent<Canvas>();
            canvas.gameObject.hideFlags = HideFlags.HideAndDontSave;
            var cameraObject = new GameObject("InventoryPreviewCamera", typeof(Camera));
            cameraObject.hideFlags = HideFlags.HideAndDontSave;
            var camera = cameraObject.GetComponent<Camera>();
            var defaultPipeline = GraphicsSettings.defaultRenderPipeline;
            var qualityPipeline = QualitySettings.renderPipeline;
            try
            {
                GraphicsSettings.defaultRenderPipeline = null;
                QualitySettings.renderPipeline = null;
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = Color.black;
                camera.cullingMask = 1 << LayerMask.NameToLayer("UI");
                camera.orthographic = true;
                camera.transform.position = new Vector3(0, 0, -10);
                canvas.renderMode = RenderMode.WorldSpace;
                canvas.worldCamera = camera;
                canvas.gameObject.SetActive(true);
                canvas.GetComponent<CanvasScaler>().enabled = false;
                var canvasRect = canvas.GetComponent<RectTransform>();
                canvasRect.localScale = Vector3.one;
                canvasRect.pivot = new Vector2(0.5f, 0.5f);
                canvasRect.position = Vector3.zero;
                foreach (var size in new[] { new Vector2Int(1920, 1080), new Vector2Int(1280, 720), new Vector2Int(2560, 1080) })
                {
                    canvas.scaleFactor = Mathf.Min(size.x / ReferenceSize.x, size.y / ReferenceSize.y);
                    canvasRect.sizeDelta = new Vector2(size.x, size.y) / canvas.scaleFactor;
                    camera.orthographicSize = canvasRect.sizeDelta.y / 2;
                    camera.aspect = (float)size.x / size.y;
                    Canvas.ForceUpdateCanvases();
                    VerifyLayout(canvas, camera);
                    RenderPreview(camera, size);
                    Debug.Log("Inventory greybox verified: " + size);
                }
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(canvas.gameObject);
                UnityEngine.Object.DestroyImmediate(cameraObject);
                GraphicsSettings.defaultRenderPipeline = defaultPipeline;
                QualitySettings.renderPipeline = qualityPipeline;
            }
        }

        private static void VerifyLayout(Canvas canvas, Camera camera)
        {
            foreach (var text in canvas.GetComponentsInChildren<TextMeshProUGUI>())
                if (text.preferredWidth > text.rectTransform.rect.width + 1 || text.preferredHeight > text.rectTransform.rect.height + 1)
                    throw new InvalidOperationException("Text does not fit: " + text.transform.parent.name + "/" + text.name);
            var corners = new Vector3[4];
            foreach (var graphic in canvas.GetComponentsInChildren<Graphic>())
            {
                if (graphic.name == "Background") continue;
                graphic.rectTransform.GetWorldCorners(corners);
                foreach (var corner in corners)
                {
                    var point = camera.WorldToViewportPoint(corner);
                    if (point.x < 0 || point.x > 1 || point.y < 0 || point.y > 1)
                        throw new InvalidOperationException("UI outside viewport: " + graphic.name);
                }
            }
        }

        private static void RenderPreview(Camera camera, Vector2Int size)
        {
            var target = RenderTexture.GetTemporary(size.x, size.y, 24);
            var previous = RenderTexture.active;
            var image = new Texture2D(size.x, size.y, TextureFormat.RGB24, false);
            try
            {
                camera.targetTexture = target;
                camera.Render();
                RenderTexture.active = target;
                image.ReadPixels(new Rect(0, 0, size.x, size.y), 0, 0);
                image.Apply();
                if (image.GetPixel(size.x / 2, size.y / 2).maxColorComponent < 0.05f)
                    throw new InvalidOperationException("Preview did not render the Inventory UI.");
                File.WriteAllBytes(Path.Combine(Path.GetTempPath(), "inventory-greybox-" + size.x + "x" + size.y + ".png"), image.EncodeToPNG());
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(image);
                RenderTexture.active = previous;
                camera.targetTexture = null;
                RenderTexture.ReleaseTemporary(target);
            }
        }
    }
}
