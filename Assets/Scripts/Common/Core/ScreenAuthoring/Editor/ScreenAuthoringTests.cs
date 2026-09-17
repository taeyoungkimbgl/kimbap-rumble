using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Scripts.Common.Core.ScreenAuthoring.Editor
{
    public class ScreenAuthoringTests
    {
        private Scene scene;
        private string scenePath;

        [SetUp]
        public void SetUp()
        {
            // The Test Runner saves/restores the user's scene setup and supplies
            // an unsaved bootstrap scene with default camera objects for EditMode tests.
            scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scenePath = "Assets/ScreenAuthoringTest" + Guid.NewGuid().ToString("N") + ".unity";
            EditorSceneManager.SaveScene(scene, scenePath);
            Apply(@"[
                { 'op':'create', 'kind':'Canvas', 'name':'Canvas', 'properties':{'active':false} },
                { 'op':'create', 'kind':'Button', 'parent':'Canvas', 'name':'Button' },
                { 'op':'create', 'kind':'Text', 'parent':'Canvas/Button', 'name':'Label', 'properties':{'text':'ORIGINAL'} }
            ]");
            Execute("save");
        }

        [TearDown]
        public void TearDown()
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            AssetDatabase.DeleteAsset(scenePath);
        }

        private JObject Execute(string action, params JProperty[] properties)
        {
            var request = new JObject { ["scene"] = scenePath, ["action"] = action };
            foreach (var property in properties) request.Add(property);
            return ScreenAuthoringCoordinator.Execute(request);
        }

        private JObject Apply(string operations, bool save = false)
        {
            return Execute("apply", new JProperty("operations", JArray.Parse(operations)), new JProperty("save", save));
        }

        private GameObject Find(string path) => ScreenAuthoringReader.Find(scene, path);

        [Test]
        public void InspectIncludesInactiveChildrenAndLeavesSceneUnchanged()
        {
            var before = File.ReadAllText(scenePath);
            var result = Execute("inspect", new JProperty("path", "Canvas"));
            Assert.AreEqual(3, ((JArray)result["data"]["nodes"]).Count);
            Assert.IsFalse(result["data"]["nodes"][1].Value<bool>("activeInHierarchy"));
            Assert.IsFalse(scene.isDirty);
            Assert.AreEqual(before, File.ReadAllText(scenePath));
            var label = ScreenAuthoringReader.Describe(Find("Canvas/Button/Label"));
            CollectionAssert.Contains(label["components"].Values<string>().ToArray(), typeof(TextMeshProUGUI).FullName);
            Assert.AreEqual("ORIGINAL", label["properties"].Value<string>("text"));
            Assert.AreEqual(32f, label["properties"].Value<float>("fontSize"));
            Assert.AreEqual("Center", label["properties"].Value<string>("alignment"));
            Assert.AreEqual(AssetDatabase.GetAssetPath(TMP_Settings.defaultFontAsset), label["properties"].Value<string>("font"));
        }

        [Test]
        public void PartialUpdatePreservesReferencesAndAcceptsZeroFalseAndEmpty()
        {
            var button = Find("Canvas/Button").GetComponent<Button>();
            var image = button.targetGraphic;
            var id = GlobalObjectId.GetGlobalObjectIdSlow(button).ToString();
            Apply(@"[
                {'op':'update','path':'Canvas/Button','properties':{'position':[0,0], 'interactable':false}},
                {'op':'update','path':'Canvas/Button/Label','properties':{'text':''}}
            ]");
            Assert.AreSame(image, button.targetGraphic);
            Assert.AreEqual(id, GlobalObjectId.GetGlobalObjectIdSlow(button).ToString());
            Assert.IsFalse(button.interactable);
            Assert.AreEqual("", Find("Canvas/Button/Label").GetComponent<TextMeshProUGUI>().text);
            Assert.IsFalse(Find("Canvas").activeSelf);
            Undo.PerformUndo();
            Assert.IsTrue(button.interactable);
            Assert.AreEqual("ORIGINAL", Find("Canvas/Button/Label").GetComponent<TextMeshProUGUI>().text);
            Undo.PerformRedo();
            Assert.IsFalse(button.interactable);
        }

        [Test]
        public void NewScreenCreationSupportsUndoRedoAndSaveReload()
        {
            Apply(@"[
                {'op':'create','kind':'Canvas','name':'SecondCanvas'},
                {'op':'create','kind':'Panel','parent':'SecondCanvas','name':'Panel','properties':{'size':[640,480]}},
                {'op':'create','kind':'Button','parent':'SecondCanvas/Panel','name':'Confirm'},
                {'op':'create','kind':'Text','parent':'SecondCanvas/Panel/Confirm','name':'Label','properties':{'text':'CONFIRM','fontSize':24.5,'alignment':'BottomRight','font':'Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset'}}
            ]");
            Undo.PerformUndo();
            Assert.Throws<ArgumentException>(() => Find("SecondCanvas"));
            Undo.PerformRedo();
            var button = Find("SecondCanvas/Panel/Confirm").GetComponent<Button>();
            Assert.AreEqual(button.GetComponent<Image>(), button.targetGraphic);
            Assert.AreEqual("CONFIRM", Find("SecondCanvas/Panel/Confirm/Label").GetComponent<TextMeshProUGUI>().text);
            Assert.AreEqual(24.5f, Find("SecondCanvas/Panel/Confirm/Label").GetComponent<TextMeshProUGUI>().fontSize);
            Assert.AreEqual(TextAlignmentOptions.BottomRight, Find("SecondCanvas/Panel/Confirm/Label").GetComponent<TextMeshProUGUI>().alignment);
            Assert.AreEqual(TMP_Settings.defaultFontAsset, Find("SecondCanvas/Panel/Confirm/Label").GetComponent<TextMeshProUGUI>().font);
            Execute("save");
            EditorSceneManager.CloseScene(scene, true);
            scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            Assert.AreEqual(new Vector2(640, 480), Find("SecondCanvas/Panel").GetComponent<RectTransform>().sizeDelta);
            Assert.AreEqual("CONFIRM", Find("SecondCanvas/Panel/Confirm/Label").GetComponent<TextMeshProUGUI>().text);
            Assert.AreEqual(24.5f, Find("SecondCanvas/Panel/Confirm/Label").GetComponent<TextMeshProUGUI>().fontSize);
            Assert.AreEqual(TextAlignmentOptions.BottomRight, Find("SecondCanvas/Panel/Confirm/Label").GetComponent<TextMeshProUGUI>().alignment);
            Assert.AreEqual(TMP_Settings.defaultFontAsset, Find("SecondCanvas/Panel/Confirm/Label").GetComponent<TextMeshProUGUI>().font);
        }

        [Test]
        public void FailedBatchRollsBackEarlierEditsAndCreationsWithoutSaving()
        {
            var before = File.ReadAllText(scenePath);
            Assert.Throws<ArgumentException>(() => Apply(@"[
                {'op':'update','path':'Canvas/Button/Label','properties':{'text':'CHANGED'}},
                {'op':'create','kind':'Panel','parent':'Canvas','name':'NewPanel'},
                {'op':'update','path':'Canvas/Button','properties':{'typo':42}}
            ]", true));
            Assert.AreEqual("ORIGINAL", Find("Canvas/Button/Label").GetComponent<TextMeshProUGUI>().text);
            Assert.Throws<ArgumentException>(() => Find("Canvas/NewPanel"));
            Assert.AreEqual(before, File.ReadAllText(scenePath));
        }

        [Test]
        public void DeleteIsExplicitAndUndoRestoresHierarchy()
        {
            Assert.Throws<ArgumentException>(() => Apply(@"[{'op':'delete','path':'Canvas/Button'}]"));
            Apply(@"[{'op':'delete','path':'Canvas/Button','confirmDelete':true}]");
            Assert.Throws<ArgumentException>(() => Find("Canvas/Button"));
            Undo.PerformUndo();
            Assert.AreEqual("ORIGINAL", Find("Canvas/Button/Label").GetComponent<TextMeshProUGUI>().text);
        }

        [Test]
        public void DuplicateNamesAndMissingPathsFailInsteadOfEditingAnotherObject()
        {
            Assert.Throws<ArgumentException>(() => Apply(@"[{'op':'create','kind':'Text','name':'Label','parent':'Canvas/Button'}]"));
            Assert.Throws<ArgumentException>(() => Execute("inspect", new JProperty("path", "Canvas/Missing")));
            var duplicate = new GameObject("Button", typeof(RectTransform));
            duplicate.transform.SetParent(Find("Canvas").transform, false);
            Assert.Throws<ArgumentException>(() => Apply(@"[{'op':'update','path':'Canvas/Button','properties':{'active':true}}]"));
        }

        [TestCase("{'fontSize':0}")]
        [TestCase("{'color':[1,1,1]}")]
        [TestCase("{'active':'false'}")]
        [TestCase("{'position':[0]}")]
        [TestCase("{'alignment':'999'}")]
        public void InvalidPropertiesFailAtInputBoundary(string properties)
        {
            Assert.Throws<ArgumentException>(() => Apply("[{'op':'update','path':'Canvas/Button/Label','properties':" + properties + "}]"));
        }

        [Test]
        public void EditingDoesNotSaveUntilRequested()
        {
            var before = File.ReadAllText(scenePath);
            Apply(@"[{'op':'update','path':'Canvas/Button/Label','properties':{'text':'SAVE ME'}}]");
            Assert.AreEqual(before, File.ReadAllText(scenePath));
            Execute("save");
            Assert.AreNotEqual(before, File.ReadAllText(scenePath));
        }

        [Test]
        public void FileProtocolReturnsErrorsForMalformedAndDuplicateJsonFields()
        {
            var directory = Path.Combine(Path.GetTempPath(), "screen-authoring-test-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);
            var input = Path.Combine(directory, "request.json");
            var output = Path.Combine(directory, "result.json");
            try
            {
                foreach (var json in new[] { "{", "{'action':'inspect','action':'apply'}" })
                {
                    File.WriteAllText(input, json);
                    Assert.IsFalse(ScreenAuthoringCoordinator.ProcessFile(input, output));
                    Assert.IsFalse(JObject.Parse(File.ReadAllText(output)).Value<bool>("success"));
                }
            }
            finally { Directory.Delete(directory, true); }
        }

        [Test]
        public void PreviewRendersInactiveCanvasInIsolationAndRestoresSourceAndPipeline()
        {
            Apply(@"[{'op':'update','path':'Canvas/Button','properties':{'color':[1,0,0,1]}},
                {'op':'update','path':'Canvas/Button/Label','properties':{'text':'TMP','position':[0,120],'size':[480,120],'fontSize':72}}]");
            var before = ScreenAuthoringReader.Inspect(scene, "Canvas").ToString();
            var pipeline = GraphicsSettings.defaultRenderPipeline;
            var quality = QualitySettings.renderPipeline;
            var result = Execute("preview", new JProperty("path", "Canvas"),
                new JProperty("resolutions", new JArray(new JArray(320, 180), new JArray(640, 360))));
            var imagePath = result["data"]["images"][0].Value<string>("path");
            var image = new Texture2D(2, 2);
            try
            {
                Assert.IsTrue(image.LoadImage(File.ReadAllBytes(imagePath)));
                Assert.AreEqual(320, image.width);
                var pixel = image.GetPixel(160, 90);
                Assert.Greater(pixel.r, 0.8f);
                Assert.Less(pixel.g, 0.1f);
                Assert.IsTrue(image.GetPixels().Any(x => x.r > 0.7f && x.g > 0.7f && x.b > 0.7f),
                    "The isolated preview must render the TextMeshPro label.");
                Assert.AreEqual(before, ScreenAuthoringReader.Inspect(scene, "Canvas").ToString());
                Assert.AreSame(pipeline, GraphicsSettings.defaultRenderPipeline);
                Assert.AreSame(quality, QualitySettings.renderPipeline);
                Assert.IsFalse(Find("Canvas").activeSelf);
            }
            finally
            {
                Object.DestroyImmediate(image);
                Directory.Delete(Path.GetDirectoryName(imagePath), true);
            }
        }
    }
}
