using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Scripts.Common.Core.ScreenAuthoring.Editor.ScreenAuthoringReader;

namespace Scripts.Common.Core.ScreenAuthoring.Editor
{
    public static class ScreenAuthoringFactory
    {
        internal const string UndoName = "Screen Authoring";

        public static GameObject Create(Scene scene, JObject operation)
        {
            var kind = String(operation["kind"]);
            if (!new[] { "GameObject", "Canvas", "Rect", "Panel", "Image", "Text", "Button" }.Contains(kind))
                throw new ArgumentException("Unsupported kind: " + kind);
            var name = String(operation["name"]);
            if (Segments(name).Length != 1)
                throw new ArgumentException("name must be one hierarchy segment.");
            var parentPath = (string)operation["parent"];
            var parent = string.IsNullOrEmpty(parentPath) ? null : Find(scene, parentPath).transform;
            var siblings = parent == null ? scene.GetRootGameObjects().Select(x => x.transform) : parent.Cast<Transform>();
            if (siblings.Any(x => x.name == name))
                throw new ArgumentException("Object already exists. Use update: " + name);
            if (kind != "GameObject" && kind != "Canvas" && (parent == null || parent.GetComponentInParent<Canvas>(true) == null))
                throw new ArgumentException("UI elements must be created below a Canvas.");

            var target = kind == "GameObject" ? new GameObject(name) : new GameObject(name, typeof(RectTransform));
            SceneManager.MoveGameObjectToScene(target, scene);
            Undo.RegisterCreatedObjectUndo(target, UndoName);
            Undo.SetTransformParent(target.transform, parent, UndoName);
            if (kind == "GameObject")
            {
                Undo.RecordObject(target.transform, UndoName);
                target.transform.localPosition = Vector3.zero;
                target.transform.localRotation = Quaternion.identity;
                target.transform.localScale = Vector3.one;
                return target;
            }
            var rect = target.GetComponent<RectTransform>();
            Undo.RecordObject(rect, UndoName);
            target.layer = LayerMask.NameToLayer("UI");
            rect.localScale = Vector3.one;
            rect.localRotation = Quaternion.identity;
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition3D = Vector3.zero;
            rect.sizeDelta = new Vector2(240, 72);

            if (kind == "Canvas")
            {
                var canvas = Undo.AddComponent<Canvas>(target);
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                var scaler = Undo.AddComponent<CanvasScaler>(target);
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
                Undo.AddComponent<GraphicRaycaster>(target);
                rect.sizeDelta = scaler.referenceResolution;
            }
            if (kind == "Panel" || kind == "Image" || kind == "Button")
            {
                var image = Undo.AddComponent<Image>(target);
                image.color = kind == "Button" ? new Color(0.36f, 0.36f, 0.36f, 1) : new Color(0.22f, 0.22f, 0.22f, 1);
                image.raycastTarget = kind == "Button";
            }
            if (kind == "Text")
            {
                var text = Undo.AddComponent<TextMeshProUGUI>(target);
                text.font = TMP_Settings.defaultFontAsset;
                text.text = name;
                text.fontSize = 32;
                text.alignment = TextAlignmentOptions.Center;
                text.textWrappingMode = TextWrappingModes.Normal;
                text.overflowMode = TextOverflowModes.Truncate;
                text.color = new Color(0.94f, 0.94f, 0.94f, 1);
                text.raycastTarget = false;
            }
            if (kind == "Button")
            {
                var button = Undo.AddComponent<Button>(target);
                button.targetGraphic = target.GetComponent<Image>();
                button.transition = Selectable.Transition.None;
                button.navigation = new Navigation { mode = Navigation.Mode.None };
            }
            // Capture initialized component values for redo, including references on new buttons.
            Undo.RegisterCompleteObjectUndo(target.GetComponents<Component>(), UndoName);
            return target;
        }

        public static void Update(GameObject target, JObject properties)
        {
            foreach (var property in properties.Properties())
            {
                var value = property.Value;
                switch (property.Name)
                {
                    case "name":
                        var name = String(value);
                        if (Segments(name).Length != 1)
                            throw new ArgumentException("name must be one hierarchy segment.");
                        var siblings = target.transform.parent == null
                            ? target.scene.GetRootGameObjects().Select(x => x.transform)
                            : target.transform.parent.Cast<Transform>();
                        if (siblings.Any(x => x.gameObject != target && x.name == name))
                            throw new ArgumentException("Object already exists: " + name);
                        Undo.RecordObject(target, UndoName);
                        target.name = name;
                        PrefabUtility.RecordPrefabInstancePropertyModifications(target);
                        break;
                    case "active":
                        Undo.RecordObject(target, UndoName);
                        target.SetActive(Boolean(value));
                        PrefabUtility.RecordPrefabInstancePropertyModifications(target);
                        break;
                    case "position": Change<RectTransform>(target, x => x.anchoredPosition = Vector(value)); break;
                    case "size": Change<RectTransform>(target, x => x.sizeDelta = Vector(value)); break;
                    case "anchorMin": Change<RectTransform>(target, x => x.anchorMin = Vector(value)); break;
                    case "anchorMax": Change<RectTransform>(target, x => x.anchorMax = Vector(value)); break;
                    case "pivot": Change<RectTransform>(target, x => x.pivot = Vector(value)); break;
                    case "scale":
                        var scale = Numbers(value, 3);
                        Change<RectTransform>(target, x => x.localScale = new Vector3(scale[0], scale[1], scale[2]));
                        break;
                    case "rotation": Change<RectTransform>(target, x => x.localRotation = Quaternion.Euler(0, 0, Number(value))); break;
                    case "color":
                        var rgba = Numbers(value, 4);
                        Change<Graphic>(target, x => x.color = new Color(rgba[0], rgba[1], rgba[2], rgba[3]));
                        break;
                    case "raycastTarget": Change<Graphic>(target, x => x.raycastTarget = Boolean(value)); break;
                    case "text": Change<TextMeshProUGUI>(target, x => x.text = String(value)); break;
                    case "fontSize":
                        var fontSize = Number(value);
                        if (fontSize <= 0) throw new ArgumentException("fontSize must be positive.");
                        Change<TextMeshProUGUI>(target, x => x.fontSize = fontSize);
                        break;
                    case "alignment": Change<TextMeshProUGUI>(target, x => x.alignment = EnumValue<TextAlignmentOptions>(value)); break;
                    case "autoSize": Change<TextMeshProUGUI>(target, x => x.enableAutoSizing = Boolean(value)); break;
                    case "font": Change<TextMeshProUGUI>(target, x => x.font = Asset<TMP_FontAsset>(String(value))); break;
                    case "sprite": Change<Image>(target, x => x.sprite = value.Type == JTokenType.Null ? null : Asset<Sprite>(String(value))); break;
                    case "preserveAspect": Change<Image>(target, x => x.preserveAspect = Boolean(value)); break;
                    case "interactable": Change<Button>(target, x => x.interactable = Boolean(value)); break;
                    case "sortingOrder": Change<Canvas>(target, x => x.sortingOrder = Integer(value)); break;
                    case "referenceResolution":
                        var resolution = Vector(value);
                        if (resolution.x <= 0 || resolution.y <= 0) throw new ArgumentException("referenceResolution must be positive.");
                        Change<CanvasScaler>(target, x => x.referenceResolution = resolution);
                        break;
                    case "screenMatchMode": Change<CanvasScaler>(target, x => x.screenMatchMode = EnumValue<CanvasScaler.ScreenMatchMode>(value)); break;
                    case "matchWidthOrHeight":
                        var match = Number(value);
                        if (match < 0 || match > 1) throw new ArgumentException("matchWidthOrHeight must be between 0 and 1.");
                        Change<CanvasScaler>(target, x => x.matchWidthOrHeight = match);
                        break;
                    default: throw new ArgumentException("Unsupported property: " + property.Name);
                }
            }
        }

        public static void AddComponent(GameObject target, string typeName)
        {
            var type = ComponentType(typeName);
            if (target.GetComponent(type) != null)
                throw new ArgumentException("Component already exists: " + typeName);
            if (Undo.AddComponent(target, type) == null)
                throw new ArgumentException("Cannot add component: " + typeName);
        }

        public static void SetReferences(GameObject target, string typeName, JObject references)
        {
            var component = FindComponent(target, typeName);
            using var serialized = new SerializedObject(component);
            foreach (var reference in references.Properties())
            {
                var property = serialized.FindProperty(reference.Name);
                if (property == null || property.propertyType != SerializedPropertyType.ObjectReference || reference.Name == "m_Script")
                    throw new ArgumentException("Expected an object reference field: " + reference.Name);
                UnityEngine.Object value = null;
                if (reference.Value.Type != JTokenType.Null)
                {
                    if (!(reference.Value is JObject locator))
                        throw new ArgumentException("Reference must be a scene locator or null.");
                    Keys(locator, "path", "component");
                    var referencedObject = Find(target.scene, String(locator["path"]));
                    value = locator["component"] == null
                        ? (UnityEngine.Object)referencedObject
                        : FindComponent(referencedObject, String(locator["component"]));
                }
                property.objectReferenceValue = value;
                if (property.objectReferenceValue != value)
                    throw new ArgumentException("Reference type mismatch: " + reference.Name);
            }
            serialized.ApplyModifiedProperties();
            PrefabUtility.RecordPrefabInstancePropertyModifications(component);
        }

        private static Type ComponentType(string name)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies().Select(x => x.GetType(name, false))
                .Where(x => x != null).Distinct().ToArray();
            if (types.Length != 1 || !typeof(Component).IsAssignableFrom(types[0]) ||
                types[0].IsAbstract || types[0].ContainsGenericParameters)
                throw new ArgumentException("Expected one concrete Component type by full name: " + name);
            return types[0];
        }

        private static Component FindComponent(GameObject target, string typeName)
        {
            var components = target.GetComponents(ComponentType(typeName));
            if (components.Length != 1)
                throw new ArgumentException("Component must resolve exactly once: " + typeName + " on " + PathOf(target.transform));
            return components[0];
        }

        private static void Change<T>(GameObject target, Action<T> change) where T : Component
        {
            var component = Component<T>(target);
            Undo.RecordObject(component, UndoName);
            change(component);
            PrefabUtility.RecordPrefabInstancePropertyModifications(component);
        }

        private static T Asset<T>(string path) where T : UnityEngine.Object
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset == null) throw new ArgumentException("Asset missing or wrong type: " + path);
            return asset;
        }
    }
}
