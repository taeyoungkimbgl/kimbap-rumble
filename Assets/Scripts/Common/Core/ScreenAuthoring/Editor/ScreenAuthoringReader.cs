using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Scripts.Common.Core.ScreenAuthoring.Editor
{
    public static class ScreenAuthoringReader
    {
        public static void Keys(JObject data, params string[] allowed)
        {
            foreach (var property in data.Properties())
                if (!allowed.Contains(property.Name))
                    throw new ArgumentException("Unknown field: " + property.Name);
        }

        public static string String(JToken token)
        {
            if (token == null || token.Type != JTokenType.String)
                throw new ArgumentException("Expected a string.");
            return token.Value<string>();
        }

        public static bool Boolean(JToken token)
        {
            if (token == null || token.Type != JTokenType.Boolean)
                throw new ArgumentException("Expected a boolean.");
            return token.Value<bool>();
        }

        public static float Number(JToken token)
        {
            if (token == null || (token.Type != JTokenType.Integer && token.Type != JTokenType.Float))
                throw new ArgumentException("Expected a finite number.");
            var value = token.Value<float>();
            if (float.IsNaN(value) || float.IsInfinity(value))
                throw new ArgumentException("Expected a finite number.");
            return value;
        }

        public static int Integer(JToken token)
        {
            if (token == null || token.Type != JTokenType.Integer)
                throw new ArgumentException("Expected an integer.");
            return token.Value<int>();
        }

        public static float[] Numbers(JToken token, int length)
        {
            if (!(token is JArray values) || values.Count != length)
                throw new ArgumentException("Expected an array of " + length + " numbers.");
            return values.Select(Number).ToArray();
        }

        public static Vector2 Vector(JToken token)
        {
            var values = Numbers(token, 2);
            return new Vector2(values[0], values[1]);
        }

        public static T EnumValue<T>(JToken token) where T : struct, Enum
        {
            var name = String(token);
            if (!Enum.GetNames(typeof(T)).Contains(name))
                throw new ArgumentException("Unsupported " + typeof(T).Name + ": " + name);
            return (T)Enum.Parse(typeof(T), name);
        }

        public static T Component<T>(GameObject target) where T : Component
        {
            var component = target.GetComponent<T>();
            if (component == null)
                throw new ArgumentException(target.name + " needs " + typeof(T).Name + ".");
            return component;
        }

        public static string[] Segments(string path)
        {
            var segments = path.Split('/');
            if (segments.Any(x => string.IsNullOrWhiteSpace(x) || x == "." || x == ".." || x.Contains("\\")))
                throw new ArgumentException("Use an exact hierarchy path without empty segments: " + path);
            return segments;
        }

        public static GameObject Find(Scene scene, string path)
        {
            IEnumerable<Transform> candidates = scene.GetRootGameObjects().Select(x => x.transform);
            Transform found = null;
            foreach (var segment in Segments(path))
            {
                var matches = candidates.Where(x => x.name == segment).ToArray();
                if (matches.Length != 1)
                    throw new ArgumentException("Path must resolve exactly once (missing or duplicate name): " + path);
                found = matches[0];
                candidates = found.Cast<Transform>();
            }
            return found.gameObject;
        }

        public static string PathOf(Transform target)
        {
            return target.parent == null ? target.name : PathOf(target.parent) + "/" + target.name;
        }

        public static JObject Inspect(Scene scene, string path)
        {
            var roots = path == null ? scene.GetRootGameObjects() : new[] { Find(scene, path) };
            var nodes = new JArray();
            foreach (var root in roots)
                foreach (var transform in root.GetComponentsInChildren<Transform>(true))
                    nodes.Add(Describe(transform.gameObject));
            return new JObject { ["nodes"] = nodes, ["dirty"] = scene.isDirty };
        }

        private static JArray Pair(Vector2 value) => new JArray(value.x, value.y);

        public static JObject Describe(GameObject target)
        {
            var properties = new JObject { ["active"] = target.activeSelf };
            if (target.TryGetComponent<RectTransform>(out var rect))
            {
                properties["position"] = Pair(rect.anchoredPosition);
                properties["size"] = Pair(rect.sizeDelta);
                properties["anchorMin"] = Pair(rect.anchorMin);
                properties["anchorMax"] = Pair(rect.anchorMax);
                properties["pivot"] = Pair(rect.pivot);
                properties["scale"] = new JArray(rect.localScale.x, rect.localScale.y, rect.localScale.z);
                properties["rotation"] = rect.localEulerAngles.z;
            }
            if (target.TryGetComponent<Graphic>(out var graphic))
            {
                properties["color"] = new JArray(graphic.color.r, graphic.color.g, graphic.color.b, graphic.color.a);
                properties["raycastTarget"] = graphic.raycastTarget;
            }
            if (target.TryGetComponent<TextMeshProUGUI>(out var text))
            {
                properties["text"] = text.text;
                properties["fontSize"] = text.fontSize;
                properties["alignment"] = text.alignment.ToString();
                properties["autoSize"] = text.enableAutoSizing;
                properties["font"] = AssetDatabase.GetAssetPath(text.font);
            }
            if (target.TryGetComponent<Image>(out var image))
            {
                properties["sprite"] = image.sprite == null ? null : AssetDatabase.GetAssetPath(image.sprite);
                properties["preserveAspect"] = image.preserveAspect;
            }
            if (target.TryGetComponent<Button>(out var button))
                properties["interactable"] = button.interactable;
            if (target.TryGetComponent<Canvas>(out var canvas))
                properties["sortingOrder"] = canvas.sortingOrder;
            if (target.TryGetComponent<CanvasScaler>(out var scaler))
            {
                properties["referenceResolution"] = Pair(scaler.referenceResolution);
                properties["screenMatchMode"] = scaler.screenMatchMode.ToString();
                properties["matchWidthOrHeight"] = scaler.matchWidthOrHeight;
            }
            return new JObject
            {
                ["path"] = PathOf(target.transform),
                ["globalObjectId"] = GlobalObjectId.GetGlobalObjectIdSlow(target).ToString(),
                ["activeInHierarchy"] = target.activeInHierarchy,
                ["components"] = new JArray(target.GetComponents<Component>().Select(x => x == null ? "MissingScript" : x.GetType().FullName)),
                ["properties"] = properties
            };
        }
    }
}
