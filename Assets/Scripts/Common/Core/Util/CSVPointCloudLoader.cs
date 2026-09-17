using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// x,y,z 形式（カンマ2個＝通常）
/// x,y,z,(…以降) 形式（カンマ3個以上＝LAS）
/// のどちらにも対応した PointCloud CSV ローダー。
/// </summary>
public static class CSVPointCloudLoader
{
    static readonly NumberStyles Style = NumberStyles.Float | NumberStyles.AllowLeadingSign;
    static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

    public static Task<Vector3[]> LoadAsync2(string absolutePath, bool mirrorX = false)
    => Task.Run(() => ParseSync2(absolutePath, mirrorX));

    static Vector3[] ParseSync2(string path, bool mirrorX = false)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException(path);

        var points = new List<Vector3>(capacity: 500_000);
        using var sr = new StreamReader(path);

        // 1 行目（ヘッダ判定）
        string? line = ReadNonEmptyLine(sr);
        if (line is null) return Array.Empty<Vector3>();

        if (IsHeader(line))
            line = ReadNonEmptyLine(sr); // ヘッダをスキップ

        if (line is null) return Array.Empty<Vector3>();

        // 先頭行をパース
        points.Add(ParseLine2(line.AsSpan()));

        // 残りの行をパース
        while ((line = ReadNonEmptyLine(sr)) is not null)
        {
            points.Add(ParseLine2(line.AsSpan(), mirrorX));
        }

        return points.ToArray();
    }
    const float ScaleFactor = 45.01f / 14.7f;
    static Vector3 ParseLine2(ReadOnlySpan<char> s, bool mirrorX = false)
    {
        /* 1 個目・2 個目のカンマ位置 */
        int c1 = s.IndexOf(',');
        if (c1 < 0) throw new FormatException($"Invalid CSV line: {s.ToString()}");

        int c2Rel = s[(c1 + 1)..].IndexOf(',');
        if (c2Rel < 0) throw new FormatException($"Invalid CSV line: {s.ToString()}");
        int c2 = c2Rel + c1 + 1;

        /* Z 成分の終端位置（3 個目のカンマ or 行末） */
        int startZ = c2 + 1;
        int endZ = s.Length;
        int c3Rel = s[startZ..].IndexOf(',');
        if (c3Rel >= 0) endZ = startZ + c3Rel;

        float x = float.Parse(s[..c1], Style, Culture);
        float y = float.Parse(s[(c1 + 1)..c2], Style, Culture);
        float z = float.Parse(s[startZ..endZ], Style, Culture);
        float fx = (float)(mirrorX ? -x : x);
        // return new Vector3(fx, y, z) * ScaleFactor;
        return new Vector3(fx, y, z);
    }


    public static Task<Vector3[]> LoadAsync(string absolutePath, CancellationToken ct = default)
        => Task.Run(() => ParseSync(absolutePath, ct), ct);

    /*──────────────────── private ────────────────────*/

    static Vector3[] ParseSync(string path, CancellationToken ct)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException(path);

        var points = new List<Vector3>(capacity: 500_000);
        using var sr = new StreamReader(path);

        // 1 行目（ヘッダ判定）
        string? line = ReadNonEmptyLine(sr);
        if (line is null) return Array.Empty<Vector3>();

        if (IsHeader(line))
            line = ReadNonEmptyLine(sr);             // ヘッダをスキップ

        if (line is null) return Array.Empty<Vector3>();

        // 形式判定：カンマ数が 3 個以上なら LAS
        bool isLas = CountCommas(line.AsSpan()) > 2;
        points.Add(ParseLine(line.AsSpan(), isLas));

        // 残りの行をパース
        while ((line = ReadNonEmptyLine(sr)) is not null)
        {
            ct.ThrowIfCancellationRequested();
            points.Add(ParseLine(line.AsSpan(), isLas));
        }

        return points.ToArray();
    }

    static Vector3 ParseLine(ReadOnlySpan<char> s, bool isLas)
    {
        /* 1 個目・2 個目のカンマ位置 */
        int c1 = s.IndexOf(',');
        if (c1 < 0) throw new FormatException($"Invalid CSV line: {s.ToString()}");

        int c2Rel = s[(c1 + 1)..].IndexOf(',');
        if (c2Rel < 0) throw new FormatException($"Invalid CSV line: {s.ToString()}");
        int c2 = c2Rel + c1 + 1;

        /* Z 成分の終端位置 */
        int startZ = c2 + 1;
        int endZ = s.Length;          // 通常形式は行末まで
        if (isLas)                      // LAS は 3 個目のカンマまで
        {
            int c3Rel = s[startZ..].IndexOf(',');
            if (c3Rel < 0) throw new FormatException($"Invalid CSV line: {s.ToString()}");
            endZ = startZ + c3Rel;
        }

        float x = float.Parse(s[..c1], Style, Culture);
        float y = float.Parse(s[(c1 + 1)..c2], Style, Culture);
        float z = float.Parse(s[startZ..endZ], Style, Culture);

        return isLas ? new Vector3(x, z, y)       // LAS: y と z を交換
                     : new Vector3(x, y, z);      // 通常
    }

    /*─────────────────── helpers ───────────────────*/

    static string? ReadNonEmptyLine(StreamReader sr)
    {
        string? ln;
        while ((ln = sr.ReadLine()) is not null)
            if (!string.IsNullOrWhiteSpace(ln))
                return ln;
        return null;
    }

    static bool IsHeader(string line)
        => line.Length != 0 && !char.IsDigit(line[0]) && line[0] != '-' && line[0] != '+';

    static int CountCommas(ReadOnlySpan<char> s)
    {
        int cnt = 0;
        foreach (char ch in s)
            if (ch == ',') cnt++;
        return cnt;
    }
}
