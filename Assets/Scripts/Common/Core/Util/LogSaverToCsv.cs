using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

public static class LogSaverToCsv
{
    /*========= 内部状態 =========*/
    private static string[] _headers;
    private static string _filePath;
    private static bool _initialized;
    private static readonly object _lock = new();   // スレッド／コルーチン同時書き込み対策

    /*========= 初期化 =========*/
    /// <summary>
    /// CSV ファイル設定。最初に 1 回だけ呼び出してください。
    /// </summary>
    /// <param name="directory">保存先ディレクトリ。存在しない場合は生成</param>
    /// <param name="fileName">拡張子込みファイル名（例: log.csv）</param>
    /// <param name="headers">列ヘッダー（キー）。可変長</param>
    public static void Init(string directory, string fileName, params string[] headers)
    {
        if (_initialized)
        {
            _headers = null;
            _filePath = null;
            _initialized = false;
        }

        if (headers is null || headers.Length == 0)
            throw new ArgumentException("headers は 1 列以上指定してください。");

        _filePath = Path.Combine(directory, fileName);
        _headers = headers;
        _initialized = true;

        /*=== ヘッダー行を書き込む（上書き）===*/
        var sb = new StringBuilder();
        sb.AppendLine(string.Join(",", EscapeAll(headers)));
        File.WriteAllText(_filePath, sb.ToString(), Encoding.UTF8);
    }

    /*========= 行追加（配列版） =========*/
    public static void AddRow(params object[] values)
    {
        EnsureReady();

        if (values.Length != _headers.Length)
            throw new ArgumentException(
                $"列数が一致しません。ヘッダー {_headers.Length} 列に対し {values.Length} 列です。");

        WriteLineInternal(values);
    }

    /*========= 行追加（Dictionary 版） =========*/
    public static void AddRow(IDictionary<string, object> row)
    {
        EnsureReady();

        var values = new object[_headers.Length];
        for (int i = 0; i < _headers.Length; i++)
        {
            if (!row.TryGetValue(_headers[i], out var v))
                throw new KeyNotFoundException($"キー \"{_headers[i]}\" が row に存在しません。");
            values[i] = v;
        }

        WriteLineInternal(values);
    }

    /*========= ユーティリティ =========*/
    private static void WriteLineInternal(object[] values)
    {
        lock (_lock) // 同時書き込み防止
        {
            var line = string.Join(",", EscapeAll(values));
            File.AppendAllText(_filePath, line + Environment.NewLine, Encoding.UTF8);
        }
    }

    private static IEnumerable<string> EscapeAll(IEnumerable<object> objs)
    {
        foreach (var o in objs)
        {
            if (o == null) yield return "";
            else
            {
                // 数値はインバリアントカルチャで整形、文字列は ToString()
                var s = o is IFormattable f
                    ? f.ToString(null, CultureInfo.InvariantCulture)
                    : o.ToString();

                // カンマ・改行・ダブルクォートを含む場合は RFC4180 準拠で括る
                if (s.Contains(',') || s.Contains('"') || s.Contains('\n') || s.Contains('\r'))
                    s = $"\"{s.Replace("\"", "\"\"")}\"";

                yield return s;
            }
        }
    }

    private static void EnsureReady()
    {
        if (!_initialized)
            throw new InvalidOperationException("CsvUtil.Init() を先に呼んでください。");
    }
}
