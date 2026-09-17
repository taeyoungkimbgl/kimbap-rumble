using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using seiko.framework.gyomu.constant;
using UnityEngine;
using UnityEngine.Networking;

public class WebPosterV2
{
    /// <summary>
    /// 配列 data を { "dataList": [...] } 形式で JSON 化して POST。
    /// 成功時: onSuccess を呼ぶ（任意）
    /// 失敗時: 例外を投げる
    /// </summary>
    public async Task PostDataAsync<T>(T[] data, Action onSuccess = null, CancellationToken ct = default)
    {
        Debug.Log("StartPostData");

        var wrapper = new Wrapper<T> { dataList = data };  // 元の JSON 形式を維持
        var jsonData = JsonUtility.ToJson(wrapper);
        var jsonBytes = Encoding.UTF8.GetBytes(jsonData);

        using (var req = new UnityWebRequest(AppConstants.SPREAD_SHEET_URL, UnityWebRequest.kHttpVerbPOST))
        {
            req.uploadHandler = new UploadHandlerRaw(jsonBytes);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            // 任意: タイムアウト秒
            // req.timeout = 15;

            await req.SendWebRequestAsync(ct);

            if (IsWebRequestSuccessful(req))
            {
                onSuccess?.Invoke();
                Debug.Log("success");
            }
            else
            {
                Debug.LogError(req.error);
                throw new Exception(req.error ?? "Web post failed.");
            }
        }
    }

    private static bool IsWebRequestSuccessful(UnityWebRequest req)
    {
        return req.result != UnityWebRequest.Result.ProtocolError &&
               req.result != UnityWebRequest.Result.ConnectionError;
        // Unity 2021 以降なら: return req.result == UnityWebRequest.Result.Success;
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] dataList; // JsonUtility 対応のため配列のまま維持
    }
}
