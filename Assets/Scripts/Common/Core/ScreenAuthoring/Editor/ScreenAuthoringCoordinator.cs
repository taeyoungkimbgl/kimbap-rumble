using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Scripts.Common.Core.ScreenAuthoring.Editor.ScreenAuthoringReader;

namespace Scripts.Common.Core.ScreenAuthoring.Editor
{
    [InitializeOnLoad]
    public static class ScreenAuthoringCoordinator
    {
        private const string Root = "Library/ScreenAuthoring";
        private static double nextPoll;

        static ScreenAuthoringCoordinator()
        {
            // JSON files are delivered atomically by Tools/screen_authoring.py.
            if (!Application.isBatchMode) EditorApplication.update += Poll;
        }

        private static void Poll()
        {
            if (EditorApplication.timeSinceStartup < nextPoll || EditorApplication.isCompiling || EditorApplication.isUpdating)
                return;
            nextPoll = EditorApplication.timeSinceStartup + 0.5;
            var inbox = Root + "/Inbox";
            if (!Directory.Exists(inbox)) return;
            var requestPath = Directory.GetFiles(inbox, "*.json").OrderBy(x => x, StringComparer.Ordinal).FirstOrDefault();
            if (requestPath == null) return;
            var name = Path.GetFileName(requestPath);
            Directory.CreateDirectory(Root + "/Processing");
            var processingPath = Root + "/Processing/" + name;
            try
            {
                File.Move(requestPath, processingPath);
            }
            catch (FileNotFoundException) { return; } // The client cancelled a still-queued request.
            ProcessFile(processingPath, Root + "/Results/" + name);
            File.Delete(processingPath);
        }

        [MenuItem("Tools/Screen Authoring/Process Pending Request")]
        public static void ProcessPending()
        {
            nextPoll = 0;
            Poll();
        }

        [MenuItem("Tools/Screen Authoring/Run Request File")]
        public static void RunRequestFile()
        {
            var path = EditorUtility.OpenFilePanel("Screen authoring request", "", "json");
            if (path.Length == 0) return;
            var resultPath = Path.GetFullPath(Root + "/Results/" + Guid.NewGuid().ToString("N") + ".json");
            ProcessFile(path, resultPath);
            Debug.Log("Screen authoring result: " + resultPath);
        }

        public static void ExecuteBatch()
        {
            var arguments = Environment.GetCommandLineArgs();
            string Argument(string key)
            {
                var index = Array.IndexOf(arguments, key);
                if (index < 0 || index + 1 == arguments.Length)
                    throw new ArgumentException("Missing argument: " + key);
                return arguments[index + 1];
            }
            var success = ProcessFile(Argument("-screenRequest"), Argument("-screenResult"));
            if (!success) throw new InvalidOperationException("Screen authoring failed. See -screenResult JSON.");
        }

        public static bool ProcessFile(string requestPath, string resultPath)
        {
            JObject result;
            try
            {
                var request = JObject.Parse(File.ReadAllText(requestPath), new JsonLoadSettings
                {
                    DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Error
                });
                result = Execute(request);
            }
            catch (Exception exception)
            {
                result = new JObject { ["success"] = false, ["error"] = exception.Message };
            }
            var fullPath = Path.GetFullPath(resultPath);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            // Consumers only observe complete JSON. Each request has its own result filename.
            var temporaryPath = fullPath + ".tmp";
            File.WriteAllText(temporaryPath, result.ToString(Formatting.Indented));
            if (File.Exists(fullPath)) File.Delete(fullPath);
            File.Move(temporaryPath, fullPath);
            return result.Value<bool>("success");
        }

        public static JObject Execute(JObject request)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                throw new InvalidOperationException("Screen authoring requires Edit Mode.");
            var action = String(request["action"]);
            switch (action)
            {
                case "inspect": Keys(request, "action", "scene", "path"); break;
                case "apply": Keys(request, "action", "scene", "operations", "save"); break;
                case "preview": Keys(request, "action", "scene", "path", "resolutions"); break;
                case "save": Keys(request, "action", "scene"); break;
                default: throw new ArgumentException("Unsupported action: " + action);
            }
            var scenePath = String(request["scene"]);
            var segments = Segments(scenePath);
            if (segments[0] != "Assets" || !scenePath.EndsWith(".unity", StringComparison.Ordinal) || !File.Exists(scenePath))
                throw new ArgumentException("scene must name an existing Assets/*.unity file.");
            var scene = SceneManager.GetSceneByPath(scenePath);
            var opened = !scene.IsValid() || !scene.isLoaded;
            var previousActive = SceneManager.GetActiveScene();
            if (opened)
            {
                if (action == "save") throw new ArgumentException("Open the scene before saving it.");
                var emptyStartupScene = SceneManager.sceneCount == 1 && previousActive.path.Length == 0 &&
                    !previousActive.isDirty && previousActive.rootCount == 0;
                scene = EditorSceneManager.OpenScene(scenePath, emptyStartupScene ? OpenSceneMode.Single : OpenSceneMode.Additive);
                if (previousActive.IsValid()) SceneManager.SetActiveScene(previousActive);
            }
            try
            {
                JObject data;
                var saved = false;
                switch (action)
                {
                    case "inspect":
                        data = Inspect(scene, request["path"] == null ? null : String(request["path"]));
                        break;
                    case "apply":
                        saved = request["save"] != null && Boolean(request["save"]);
                        data = Apply(scene, request["operations"] as JArray, saved);
                        break;
                    case "preview":
                        var resolutions = request["resolutions"] == null
                            ? new JArray(new JArray(1920, 1080), new JArray(1280, 720))
                            : request["resolutions"] as JArray;
                        if (resolutions == null || resolutions.Count == 0)
                            throw new ArgumentException("resolutions must be a nonempty array.");
                        data = ScreenAuthoringPreviewWriter.Write(Find(scene, String(request["path"])), resolutions);
                        break;
                    default:
                        Save(scene);
                        saved = true;
                        data = new JObject();
                        break;
                }
                return new JObject { ["success"] = true, ["action"] = action, ["scene"] = scenePath, ["saved"] = saved, ["data"] = data };
            }
            finally
            {
                if (opened && action != "apply" && SceneManager.sceneCount > 1) EditorSceneManager.CloseScene(scene, true);
            }
        }

        private static JObject Apply(Scene scene, JArray operations, bool save)
        {
            if (operations == null || operations.Count == 0)
                throw new ArgumentException("operations must be a nonempty array.");
            var touched = new JArray();
            Undo.IncrementCurrentGroup();
            var group = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName(ScreenAuthoringFactory.UndoName);
            try
            {
                foreach (var item in operations)
                {
                    if (!(item is JObject operation)) throw new ArgumentException("Each operation must be an object.");
                    var op = String(operation["op"]);
                    GameObject target;
                    switch (op)
                    {
                        case "create":
                            Keys(operation, "op", "kind", "parent", "name", "properties");
                            target = ScreenAuthoringFactory.Create(scene, operation);
                            break;
                        case "update":
                            Keys(operation, "op", "path", "properties");
                            target = Find(scene, String(operation["path"]));
                            break;
                        case "addComponent":
                            Keys(operation, "op", "path", "component");
                            target = Find(scene, String(operation["path"]));
                            ScreenAuthoringFactory.AddComponent(target, String(operation["component"]));
                            break;
                        case "setReferences":
                            Keys(operation, "op", "path", "component", "references");
                            target = Find(scene, String(operation["path"]));
                            if (!(operation["references"] is JObject references) || !references.HasValues)
                                throw new ArgumentException("setReferences requires a nonempty references object.");
                            ScreenAuthoringFactory.SetReferences(target, String(operation["component"]), references);
                            break;
                        case "delete":
                            Keys(operation, "op", "path", "confirmDelete");
                            if (operation["confirmDelete"] == null || !Boolean(operation["confirmDelete"]))
                                throw new ArgumentException("delete requires confirmDelete: true.");
                            target = Find(scene, String(operation["path"]));
                            Component<RectTransform>(target);
                            touched.Add(PathOf(target.transform));
                            Undo.DestroyObjectImmediate(target);
                            continue;
                        default: throw new ArgumentException("Unsupported operation: " + op);
                    }
                    if (operation["properties"] != null)
                    {
                        if (!(operation["properties"] is JObject properties)) throw new ArgumentException("properties must be an object.");
                        ScreenAuthoringFactory.Update(target, properties);
                    }
                    else if (op == "update") throw new ArgumentException("update requires properties.");
                    touched.Add(PathOf(target.transform));
                }
                Undo.FlushUndoRecordObjects();
                Undo.CollapseUndoOperations(group);
                EditorSceneManager.MarkSceneDirty(scene);
                if (save) Save(scene);
                SceneView.RepaintAll();
                EditorApplication.QueuePlayerLoopUpdate();
                return new JObject { ["paths"] = touched, ["dirty"] = scene.isDirty };
            }
            catch
            {
                Undo.FlushUndoRecordObjects();
                Undo.RevertAllDownToGroup(group);
                throw;
            }
            finally { Undo.IncrementCurrentGroup(); }
        }

        private static void Save(Scene scene)
        {
            if (!EditorSceneManager.SaveScene(scene))
                throw new IOException("Could not save scene: " + scene.path);
        }
    }
}
