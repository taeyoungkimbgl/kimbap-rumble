#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class FeatureScriptsGeneratorWindow : EditorWindow
{
    // ベース名: Detector, Camera, UserSelection など
    private string baseName = "";

    // 保存先のルートパス（Assets からの相対パス）
    private string rootFolder = "Assets/Scripts/Features";

    // 追加（ServiceImpl）サフィックス（改行 or カンマ区切り）
    private string implServiceSuffixesRaw = "";

    // 追加（Component）サフィックス（改行 or カンマ区切り）
    private string componentSuffixesRaw = "";

    // 既存ファイルを自動パッチするか
    private bool patchInstaller = true;
    private bool patchView = true;

    [MenuItem("Tools/Feature Scripts Generator")]
    private static void OpenWindow()
    {
        GetWindow<FeatureScriptsGeneratorWindow>("Feature Scripts Generator");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("MVP + VContainer スクリプト自動生成（ServiceImpl / Component 両対応）", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        baseName = EditorGUILayout.TextField("Base Class Name", baseName);
        rootFolder = EditorGUILayout.TextField("Root Folder", rootFolder);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Add ServiceImpl Services (Suffix)", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "改行またはカンマ区切りでサフィックスを複数指定できます。\n" +
            "生成:\n" +
            "  I{Base}{Suffix}Service.cs\n" +
            "  {Base}{Suffix}ServiceImpl.cs\n" +
            "Installer 追記:\n" +
            "  builder.Register<I{Base}{Suffix}Service, {Base}{Suffix}ServiceImpl>(Lifetime.Singleton);",
            MessageType.Info);
        implServiceSuffixesRaw = EditorGUILayout.TextArea(implServiceSuffixesRaw, GUILayout.Height(70));

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Add Component Services (Suffix)", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "改行またはカンマ区切りでサフィックスを複数指定できます。\n" +
            "生成:\n" +
            "  I{Base}{Suffix}Service.cs\n" +
            "  {Base}{Suffix}.cs（MonoBehaviour, I{Base}{Suffix}Service）\n" +
            "View 追記:\n" +
            "  public {Base}{Suffix} {Suffix};\n" +
            "Installer 追記:\n" +
            "  builder.RegisterComponent(view.{Suffix}).As<I{Base}{Suffix}Service>();\n\n" +
            "注意: view.{Suffix} は Inspector で割り当てる前提です。",
            MessageType.Info);
        componentSuffixesRaw = EditorGUILayout.TextArea(componentSuffixesRaw, GUILayout.Height(70));

        EditorGUILayout.Space();
        patchInstaller = EditorGUILayout.ToggleLeft("Patch Installer", patchInstaller);
        patchView = EditorGUILayout.ToggleLeft("Patch View (Component fields)", patchView);

        EditorGUILayout.Space();
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Generate MVP Scripts + Add Extensions"))
            {
                GenerateMvpScriptsAndExtensions();
            }

            if (GUILayout.Button("Add Extensions Only"))
            {
                AddExtensionsOnly();
            }
        }
    }

    private void GenerateMvpScriptsAndExtensions()
    {
        if (!ValidateBaseName()) return;

        baseName = NormalizePascal(baseName);

        var featureFolder = Path.Combine(rootFolder, baseName).Replace("\\", "/");
        EnsureDirectory(featureFolder);

        var namespaceName = ToNamespace(featureFolder);

        // 従来 MVP 生成（現状維持）
        CreateInstaller(featureFolder, namespaceName);
        CreateModel(featureFolder, namespaceName);
        CreatePresenter(featureFolder, namespaceName);
        CreateServiceImpl(featureFolder, namespaceName);
        CreateView(featureFolder, namespaceName);
        CreateServiceInterface(featureFolder, namespaceName);

        // 拡張（ServiceImpl / Component 両方）
        GenerateExtensions(featureFolder, namespaceName);

        AssetDatabase.Refresh();
        Debug.Log($"[{baseName}] のスクリプト生成が完了しました。namespace: {namespaceName}");
    }

    private void AddExtensionsOnly()
    {
        if (!ValidateBaseName()) return;

        baseName = NormalizePascal(baseName);

        var featureFolder = Path.Combine(rootFolder, baseName).Replace("\\", "/");
        EnsureDirectory(featureFolder);

        var namespaceName = ToNamespace(featureFolder);

        GenerateExtensions(featureFolder, namespaceName);

        AssetDatabase.Refresh();
        Debug.Log($"[{baseName}] の拡張生成が完了しました。namespace: {namespaceName}");
    }

    private void GenerateExtensions(string featureFolder, string namespaceName)
    {
        var implSuffixes = ParseSuffixes(implServiceSuffixesRaw);
        var compSuffixes = ParseSuffixes(componentSuffixesRaw);

        if (implSuffixes.Count == 0 && compSuffixes.Count == 0)
        {
            Debug.Log("拡張（ServiceImpl / Component）の指定が無いため、拡張生成はスキップしました。");
            return;
        }

        // 同一 suffix が両方にある場合は Component 優先
        var overlap = implSuffixes.Intersect(compSuffixes).ToList();
        if (overlap.Count > 0)
        {
            foreach (var s in overlap)
            {
                Debug.LogWarning($"Suffix '{s}' は ServiceImpl/Component 両方に指定されています。Component を優先し、ServiceImpl 側はスキップします。");
            }
            implSuffixes = implSuffixes.Except(overlap).ToList();
        }

        // 生成
        CreateAdditionalImplServices(featureFolder, namespaceName, implSuffixes);
        CreateAdditionalComponentServices(featureFolder, namespaceName, compSuffixes);

        // パッチ
        var installerPath = Path.Combine(featureFolder, $"{baseName}Installer.cs");
        var viewPath = Path.Combine(featureFolder, $"{baseName}View.cs");

        if (patchView && compSuffixes.Count > 0)
        {
            PatchViewIfExists(viewPath, compSuffixes);
        }

        if (patchInstaller && (implSuffixes.Count > 0 || compSuffixes.Count > 0))
        {
            PatchInstallerIfExists(installerPath, implSuffixes, compSuffixes);
        }
    }

    private bool ValidateBaseName()
    {
        if (string.IsNullOrWhiteSpace(baseName))
        {
            Debug.LogError("Base Class Name が空です。Detector や UserSelection などを入力してください。");
            return false;
        }
        return true;
    }

    private static void EnsureDirectory(string folder)
    {
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
            Debug.Log($"フォルダ作成: {folder}");
        }
    }

    private static string NormalizePascal(string raw)
    {
        raw = raw.Trim();
        if (string.IsNullOrEmpty(raw)) return raw;
        return char.ToUpper(raw[0]) + raw.Substring(1);
    }

    private static List<string> ParseSuffixes(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return new List<string>();

        return raw
            .Split(new[] { '\n', '\r', ',', ';', '\t' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(t => NormalizeSuffix(t))
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Distinct()
            .ToList();
    }

    private static string NormalizeSuffix(string raw)
    {
        var s = raw.Trim();
        if (string.IsNullOrEmpty(s)) return "";

        s = s.Replace(" ", "").Replace("_", "");

        // "WriteService" と入力されても "Write" 扱い
        if (s.EndsWith("ServiceImpl", StringComparison.OrdinalIgnoreCase))
            s = s.Substring(0, s.Length - "ServiceImpl".Length);
        else if (s.EndsWith("Service", StringComparison.OrdinalIgnoreCase))
            s = s.Substring(0, s.Length - "Service".Length);

        if (string.IsNullOrEmpty(s)) return "";

        return char.ToUpper(s[0]) + s.Substring(1);
    }

    /// <summary>
    /// フォルダパスから namespace を生成する
    /// 例:
    ///   "Assets/SeikoElectric/SMP/Scripts/Features/UserSelection"
    ///   -> "SeikoElectric.SMP.Scripts.Features.UserSelection"
    /// </summary>
    private string ToNamespace(string featureFolderPath)
    {
        if (string.IsNullOrEmpty(featureFolderPath))
        {
            return "DefaultNamespace";
        }

        var path = featureFolderPath.Replace("\\", "/");

        var assetsPrefix = "Assets/";
        var index = path.IndexOf(assetsPrefix, StringComparison.Ordinal);
        if (index >= 0)
        {
            path = path.Substring(index + assetsPrefix.Length);
        }

        if (path.EndsWith("/"))
        {
            path = path.Substring(0, path.Length - 1);
        }

        path = path.Replace("/", ".");

        if (string.IsNullOrEmpty(path))
        {
            return "DefaultNamespace";
        }

        return path;
    }

    #region Existing MVP Scripts

    private void CreateInstaller(string featureFolder, string namespaceName)
    {
        string filePath = Path.Combine(featureFolder, $"{baseName}Installer.cs");
        if (File.Exists(filePath))
        {
            Debug.LogWarning($"{filePath} はすでに存在します。スキップします。");
            return;
        }

        string content = $@"using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace {namespaceName}
{{
    public static class {baseName}Installer
    {{
        public static void Register(IContainerBuilder builder, {baseName}View view)
        {{
            builder.Register<I{baseName}Service, {baseName}ServiceImpl>(Lifetime.Singleton);

            builder.RegisterComponent(view);
            builder.Register<{baseName}Model>(Lifetime.Singleton);
            builder.RegisterEntryPoint<{baseName}Presenter>();
        }}
    }}
}}
";
        File.WriteAllText(filePath, content);
    }

    private void CreateModel(string featureFolder, string namespaceName)
    {
        string filePath = Path.Combine(featureFolder, $"{baseName}Model.cs");
        if (File.Exists(filePath))
        {
            Debug.LogWarning($"{filePath} はすでに存在します。スキップします。");
            return;
        }

        string content = $@"namespace {namespaceName}
{{
    /// <summary>
    /// POCO Model
    /// </summary>
    public class {baseName}Model
    {{
        public string Id {{ get; set; }}
    }}
}}
";
        File.WriteAllText(filePath, content);
    }

    private void CreatePresenter(string featureFolder, string namespaceName)
    {
        string filePath = Path.Combine(featureFolder, $"{baseName}Presenter.cs");
        if (File.Exists(filePath))
        {
            Debug.LogWarning($"{filePath} はすでに存在します。スキップします。");
            return;
        }

        string content = $@"using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace {namespaceName}
{{
    public class {baseName}Presenter : IStartable
    {{
        [Inject] private {baseName}Model _model;
        [Inject] private {baseName}View _view;
        [Inject] private I{baseName}Service _service;

        public void Start()
        {{
            // TODO: 初期化処理
        }}
    }}
}}
";
        File.WriteAllText(filePath, content);
    }

    private void CreateServiceImpl(string featureFolder, string namespaceName)
    {
        string filePath = Path.Combine(featureFolder, $"{baseName}ServiceImpl.cs");
        if (File.Exists(filePath))
        {
            Debug.LogWarning($"{filePath} はすでに存在します。スキップします。");
            return;
        }

        string content = $@"namespace {namespaceName}
{{
    public class {baseName}ServiceImpl : I{baseName}Service
    {{
        // TODO: 実装
    }}
}}
";
        File.WriteAllText(filePath, content);
    }

    private void CreateView(string featureFolder, string namespaceName)
    {
        string filePath = Path.Combine(featureFolder, $"{baseName}View.cs");
        if (File.Exists(filePath))
        {
            Debug.LogWarning($"{filePath} はすでに存在します。スキップします。");
            return;
        }

        string content = $@"using UnityEngine;

namespace {namespaceName}
{{
    public class {baseName}View : MonoBehaviour
    {{
        // TODO: View ロジック
    }}
}}
";
        File.WriteAllText(filePath, content);
    }

    private void CreateServiceInterface(string featureFolder, string namespaceName)
    {
        string filePath = Path.Combine(featureFolder, $"I{baseName}Service.cs");
        if (File.Exists(filePath))
        {
            Debug.LogWarning($"{filePath} はすでに存在します。スキップします。");
            return;
        }

        string content = $@"namespace {namespaceName}
{{
    public interface I{baseName}Service
    {{
        // TODO: サービスインターフェイス
    }}
}}
";
        File.WriteAllText(filePath, content);
    }

    #endregion

    #region Additional - ServiceImpl Pattern

    private void CreateAdditionalImplServices(string featureFolder, string namespaceName, List<string> suffixes)
    {
        foreach (var suffix in suffixes)
        {
            CreateSuffixServiceInterface(featureFolder, namespaceName, suffix); // 共通インターフェイス
            CreateSuffixServiceImpl(featureFolder, namespaceName, suffix);
        }
    }

    // {baseName}{Suffix}ServiceImpl.cs
    private void CreateSuffixServiceImpl(string featureFolder, string namespaceName, string suffix)
    {
        string filePath = Path.Combine(featureFolder, $"{baseName}{suffix}ServiceImpl.cs");
        if (File.Exists(filePath))
        {
            Debug.LogWarning($"{filePath} はすでに存在します。スキップします。");
            return;
        }

        string content = $@"namespace {namespaceName}
{{
    public class {baseName}{suffix}ServiceImpl : I{baseName}{suffix}Service
    {{
        // TODO: 実装
    }}
}}
";
        File.WriteAllText(filePath, content);
    }

    #endregion

    #region Additional - Component Pattern

    private void CreateAdditionalComponentServices(string featureFolder, string namespaceName, List<string> suffixes)
    {
        foreach (var suffix in suffixes)
        {
            CreateSuffixServiceInterface(featureFolder, namespaceName, suffix); // 共通インターフェイス
            CreateSuffixComponent(featureFolder, namespaceName, suffix);
        }
    }

    // {baseName}{Suffix}.cs
    private void CreateSuffixComponent(string featureFolder, string namespaceName, string suffix)
    {
        string filePath = Path.Combine(featureFolder, $"{baseName}{suffix}.cs");
        if (File.Exists(filePath))
        {
            Debug.LogWarning($"{filePath} はすでに存在します。スキップします。");
            return;
        }

        string content = $@"using UnityEngine;

namespace {namespaceName}
{{
    public class {baseName}{suffix} : MonoBehaviour, I{baseName}{suffix}Service
    {{
        // TODO: Component 実装
    }}
}}
";
        File.WriteAllText(filePath, content);
    }

    #endregion

    #region Additional - Shared Interface

    // I{baseName}{Suffix}Service.cs
    private void CreateSuffixServiceInterface(string featureFolder, string namespaceName, string suffix)
    {
        string filePath = Path.Combine(featureFolder, $"I{baseName}{suffix}Service.cs");
        if (File.Exists(filePath))
        {
            // どちらの方式でも共通なので、存在していれば OK
            return;
        }

        string content = $@"namespace {namespaceName}
{{
    public interface I{baseName}{suffix}Service
    {{
        // TODO: サービスインターフェイス
    }}
}}
";
        File.WriteAllText(filePath, content);
    }

    #endregion

    #region Patching (View / Installer)

    // View に: public {baseName}{Suffix} {Suffix};
    private void PatchViewIfExists(string viewPath, List<string> componentSuffixes)
    {
        if (!File.Exists(viewPath))
        {
            Debug.LogWarning($"View が見つからないためパッチできません: {viewPath}");
            return;
        }

        var lines = File.ReadAllLines(viewPath).ToList();
        var text = string.Join("\n", lines);

        var classDecl = $"public class {baseName}View";
        var classLineIndex = lines.FindIndex(l => l.Contains(classDecl));
        if (classLineIndex < 0)
        {
            Debug.LogWarning($"View のクラス宣言が見つからないためパッチできません: {viewPath}");
            return;
        }

        // 挿入位置：クラスの最初の "{" の次
        int insertIndex = -1;
        for (int i = classLineIndex; i < lines.Count; i++)
        {
            if (lines[i].Contains("{"))
            {
                insertIndex = i + 1;
                break;
            }
        }
        if (insertIndex < 0)
        {
            Debug.LogWarning($"View の挿入位置が見つからないためパッチできません: {viewPath}");
            return;
        }

        // インデント推定：次行のインデント or 8 spaces
        string indent = "        ";
        if (insertIndex < lines.Count)
        {
            var nextIndent = GetLeadingWhitespace(lines[insertIndex]);
            if (!string.IsNullOrEmpty(nextIndent)) indent = nextIndent;
        }

        bool modified = false;

        foreach (var suffix in componentSuffixes)
        {
            var fieldLine = $"public {baseName}{suffix} {suffix};";
            if (text.Contains(fieldLine))
                continue;

            lines.Insert(insertIndex, $"{indent}{fieldLine}");
            insertIndex++;
            modified = true;
        }

        if (modified)
        {
            File.WriteAllLines(viewPath, lines);
            Debug.Log($"View をパッチしました: {viewPath}");
        }
        else
        {
            Debug.Log($"View パッチ不要（既にメンバーあり）: {viewPath}");
        }
    }

    // Installer に:
    // - ServiceImpl: builder.Register<I{Base}{Suffix}Service, {Base}{Suffix}ServiceImpl>(Lifetime.Singleton);
    // - Component : builder.RegisterComponent(view.{Suffix}).As<I{Base}{Suffix}Service>();
    private void PatchInstallerIfExists(string installerPath, List<string> implSuffixes, List<string> componentSuffixes)
    {
        if (!File.Exists(installerPath))
        {
            Debug.LogWarning($"Installer が見つからないためパッチできません: {installerPath}");
            return;
        }

        var lines = File.ReadAllLines(installerPath).ToList();
        var originalText = string.Join("\n", lines);

        // 挿入位置：builder.RegisterComponent(view); の直前を優先
        int anchorIndex = lines.FindIndex(l => l.Contains("builder.RegisterComponent(view);"));
        if (anchorIndex < 0)
        {
            anchorIndex = lines.FindIndex(l => l.Contains("builder.RegisterEntryPoint"));
        }
        if (anchorIndex < 0)
        {
            anchorIndex = FindRegisterMethodClosingBraceIndex(lines);
        }
        if (anchorIndex < 0)
        {
            Debug.LogWarning($"Installer のパッチ挿入位置が見つかりませんでした: {installerPath}");
            return;
        }

        var indent = GetLeadingWhitespace(lines[Mathf.Clamp(anchorIndex, 0, lines.Count - 1)]);
        bool modified = false;

        // 1) ServiceImpl register
        foreach (var suffix in implSuffixes)
        {
            var iface = $"I{baseName}{suffix}Service";
            var impl = $"{baseName}{suffix}ServiceImpl";
            var marker = $"Register<{iface}, {impl}>";

            if (originalText.Contains(marker))
                continue;

            var line = $"{indent}builder.Register<{iface}, {impl}>(Lifetime.Singleton);";
            lines.Insert(anchorIndex, line);
            anchorIndex++;
            modified = true;
        }

        // 2) Component register
        foreach (var suffix in componentSuffixes)
        {
            var iface = $"I{baseName}{suffix}Service";
            var marker = $"RegisterComponent(view.{suffix}).As<{iface}>";

            if (originalText.Contains(marker))
                continue;

            var line = $"{indent}builder.RegisterComponent(view.{suffix}).As<{iface}>();";
            lines.Insert(anchorIndex, line);
            anchorIndex++;
            modified = true;
        }

        if (modified)
        {
            File.WriteAllLines(installerPath, lines);
            Debug.Log($"Installer をパッチしました: {installerPath}");
        }
        else
        {
            Debug.Log($"Installer パッチ不要（既に登録済み）: {installerPath}");
        }
    }

    private static int FindRegisterMethodClosingBraceIndex(List<string> lines)
    {
        int start = lines.FindIndex(l => l.Contains("public static void Register"));
        if (start < 0) return -1;

        for (int i = start; i < lines.Count; i++)
        {
            var lineTrim = lines[i].Trim();
            if (lineTrim == "}" || lineTrim == "};")
            {
                return i;
            }
        }
        return -1;
    }

    private static string GetLeadingWhitespace(string line)
    {
        if (string.IsNullOrEmpty(line)) return "";
        int i = 0;
        while (i < line.Length && char.IsWhiteSpace(line[i])) i++;
        return line.Substring(0, i);
    }

    #endregion
}
#endif
