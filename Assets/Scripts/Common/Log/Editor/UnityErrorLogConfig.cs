using UnityEditor;
using UnityEngine;

namespace Scripts.Common.Log
{
    internal sealed class UnityErrorLogConfig : EditorWindow
    {
        private const string OutputDirectoryKey = "Scripts.Common.Log.UnityErrorLog.OutputDirectory";
        internal const string MenuRoot = "Tools/Unity Error Log/";

        internal static string OutputDirectory
        {
            get => EditorPrefs.GetString(OutputDirectoryKey);
            private set => EditorPrefs.SetString(OutputDirectoryKey, value);
        }

        [MenuItem(MenuRoot + "Settings")]
        internal static void Open()
        {
            GetWindow<UnityErrorLogConfig>("Unity Error Log");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Output Directory", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            var outputDirectory = EditorGUILayout.TextField(OutputDirectory);
            if (EditorGUI.EndChangeCheck())
            {
                OutputDirectory = outputDirectory;
            }

            if (GUILayout.Button("Browse"))
            {
                var selectedDirectory = EditorUtility.OpenFolderPanel(
                    "Select Unity Error Log Output Directory",
                    OutputDirectory,
                    string.Empty);

                if (!string.IsNullOrEmpty(selectedDirectory))
                {
                    OutputDirectory = selectedDirectory;
                }
            }

            EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(OutputDirectory));
            if (GUILayout.Button("Export Console Errors"))
            {
                UnityErrorLogWriter.Export(OutputDirectory);
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}
