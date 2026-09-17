using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;

namespace Scripts.Common.Log
{
    internal static class UnityErrorLogWriter
    {
        private const string FileNameFormat = "yyyyMMddHHmm'-unity-editor.log'";

        private static readonly Assembly UnityEditorAssembly = typeof(EditorWindow).Assembly;
        private static readonly Type LogEntriesType = UnityEditorAssembly.GetType("UnityEditor.LogEntries");
        private static readonly Type LogEntryType = UnityEditorAssembly.GetType("UnityEditor.LogEntry");
        private static readonly Type LogMessageFlagsType = UnityEditorAssembly.GetType("UnityEditor.LogMessageFlags");
        private static readonly Type LogMessageFlagsExtensionsType = UnityEditorAssembly.GetType("UnityEditor.LogMessageFlagsExtensions");

        private static readonly MethodInfo StartGettingEntriesMethod = LogEntriesType.GetMethod(
            "StartGettingEntries",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        private static readonly MethodInfo EndGettingEntriesMethod = LogEntriesType.GetMethod(
            "EndGettingEntries",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        private static readonly MethodInfo GetEntryInternalMethod = LogEntriesType.GetMethod(
            "GetEntryInternal",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        private static readonly MethodInfo IsErrorMethod = LogMessageFlagsExtensionsType.GetMethod(
            "IsError",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

        private static readonly FieldInfo MessageField = LogEntryType.GetField("message");
        private static readonly FieldInfo ModeField = LogEntryType.GetField("mode");

        [MenuItem(UnityErrorLogConfig.MenuRoot + "Export Console Errors")]
        private static void ExportFromMenu()
        {
            var outputDirectory = UnityErrorLogConfig.OutputDirectory;
            if (string.IsNullOrEmpty(outputDirectory))
            {
                UnityErrorLogConfig.Open();
                EditorUtility.DisplayDialog(
                    "Unity Error Log",
                    "Select an output directory before exporting.",
                    "OK");
                return;
            }

            Export(outputDirectory);
        }

        internal static void Export(string outputDirectory)
        {
            var messages = ReadErrorMessages();
            var exportedAt = DateTime.Now;
            var filePath = Path.Combine(outputDirectory, exportedAt.ToString(FileNameFormat));

            Directory.CreateDirectory(outputDirectory);
            File.WriteAllText(filePath, FormatLog(exportedAt, messages), new UTF8Encoding(false));

            EditorUtility.DisplayDialog(
                "Unity Error Log",
                $"Exported {messages.Count} error(s).\n\n{filePath}",
                "OK");
        }

        private static List<string> ReadErrorMessages()
        {
            var messages = new List<string>();
            var entry = Activator.CreateInstance(LogEntryType, true);
            var count = (int)StartGettingEntriesMethod.Invoke(null, null);

            try
            {
                for (var row = 0; row < count; row++)
                {
                    var arguments = new[] { (object)row, entry };
                    if (!(bool)GetEntryInternalMethod.Invoke(null, arguments))
                    {
                        continue;
                    }

                    var mode = (int)ModeField.GetValue(entry);
                    var flags = Enum.ToObject(LogMessageFlagsType, mode);
                    if ((bool)IsErrorMethod.Invoke(null, new[] { flags }))
                    {
                        messages.Add((string)MessageField.GetValue(entry));
                    }
                }
            }
            finally
            {
                EndGettingEntriesMethod.Invoke(null, null);
            }

            return messages;
        }

        private static string FormatLog(DateTime exportedAt, IReadOnlyList<string> messages)
        {
            var builder = new StringBuilder();
            builder.AppendLine("Unity Console Error Log");
            builder.AppendLine($"ExportedAt: {exportedAt:yyyy-MM-dd HH:mm:ss}");
            builder.AppendLine($"ErrorCount: {messages.Count}");

            for (var index = 0; index < messages.Count; index++)
            {
                builder.AppendLine();
                builder.AppendLine($"--- Error {index + 1} ---");
                builder.AppendLine(messages[index]);
            }

            return builder.ToString();
        }
    }
}
