using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

/// <summary>
/// 点群を CSV で保存するユーティリティ。保存前に既存ファイル／フォルダを整理する。
/// </summary>
public static class CSVSaver
{
    /// <param name="fileName">保存する CSV ファイル名（拡張子込み）</param>
    /// <param name="uniquePoints">重複がない点群データ</param>
    public static void SavePointsToCSV(string fileName, HashSet<Vector3> uniquePoints)
    {
        string directory = Application.persistentDataPath;

        // 必要に応じてクリーンアップ
        DeleteFiles(
            directory: directory,
            extension: "*.csv",
            expiredDays: 1,          // 例：3日以上前の MMdd フォルダを削除
            deleteAll: true,      // 全削除したい場合は true
            ignores: new[] {     // フォルダ／ファイル名をここに列挙（不要なら空配列可）
                "file"              // フォルダ名を無視したい場合
            });

        // 新規 CSV を書き出し
        string path = Path.Combine(directory, fileName);
        using (var writer = new StreamWriter(path, false))
        {
            writer.WriteLine("x,y,z");
            foreach (var pt in uniquePoints)
                writer.WriteLine($"{pt.x},{pt.y},{pt.z}");
        }

        Debug.Log($"Saved to: {path}");
#if UNITY_EDITOR
        UnityEditor.EditorUtility.RevealInFinder(path);
#endif
    }

    /// <summary>
    /// directory 直下を整理するユーティリティ
    /// </summary>
    /// <param name="directory">対象ディレクトリ</param>
    /// <param name="extension">削除対象 CSV の検索パターン</param>
    /// <param name="expiredDays">0 以上で MMdd フォルダの有効期限（日数）を指定</param>
    /// <param name="deleteAll">true なら ignores 以外のすべてを削除する</param>
    /// <param name="ignores">削除対象から除外したいファイル／フォルダ名</param>
    public static void DeleteFiles(
        string directory,
        string extension = "*.csv",
        int expiredDays = 0,
        bool deleteAll = false,
        string[] ignores = null)
    {
        ignores ??= Array.Empty<string>();

        // ----------------------------------------
        // ① CSV ファイル削除
        // ----------------------------------------
        TryDeleteFiles(Directory.GetFiles(directory, extension), ignores, deleteAll);

        // deleteAll 指定時は期限によるフォルダ削除をスキップし、
        // ignores 以外のフォルダをすべて削除する
        if (deleteAll)
        {
            var dirs = Directory.GetDirectories(directory);
            foreach (var dir in dirs)
                TryDeleteDirectory(dir, ignores);
            return;
        }

        // ----------------------------------------
        // ② 期限切れ MMdd フォルダ削除
        // ----------------------------------------
        if (expiredDays <= 0) return;

        var subDirs = Directory.GetDirectories(directory);
        foreach (var subDir in subDirs)
        {
            var folderName = Path.GetFileName(subDir);

            // 無視対象か？
            if (IsIgnored(folderName, ignores)) continue;

            // MMdd 形式でなければ無視
            if (folderName.Length != 4 || !int.TryParse(folderName, out _)) continue;

            if (!DateTime.TryParseExact(
                    folderName, "MMdd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime folderDate))
                continue;

            // 今年のその日付とする（未来なら前年補正）
            var baseDate = new DateTime(DateTime.Now.Year, folderDate.Month, folderDate.Day);
            if (baseDate > DateTime.Now) baseDate = baseDate.AddYears(-1);

            if ((DateTime.Now - baseDate).TotalDays > expiredDays)
                TryDeleteDirectory(subDir, ignores);
        }
    }

    // ---------- ヘルパー ----------

    static bool IsIgnored(string name, string[] ignores)
        => Array.Exists(ignores, ig => string.Equals(ig, name, StringComparison.OrdinalIgnoreCase));

    static void TryDeleteFiles(IEnumerable<string> files, string[] ignores, bool deleteAll)
    {
        int deleted = 0;
        foreach (var file in files)
        {
            if (!deleteAll && IsIgnored(Path.GetFileName(file), ignores)) continue;

            try
            {
                File.Delete(file);
                ++deleted;
            }
            catch (IOException ex)
            {
                Debug.LogWarning($"Could not delete file {file}: {ex.Message}");
            }
        }
        if (deleted > 0) Debug.Log($"Deleted {deleted} file(s).");
    }

    static void TryDeleteDirectory(string dirPath, string[] ignores)
    {
        if (IsIgnored(Path.GetFileName(dirPath), ignores)) return;

        try
        {
            Directory.Delete(dirPath, true);
            Debug.Log($"Deleted folder: {dirPath}");
        }
        catch (IOException ex)
        {
            Debug.LogWarning($"Could not delete folder {dirPath}: {ex.Message}");
        }
    }
}
