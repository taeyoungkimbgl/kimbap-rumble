using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using seiko.framework.gyomu.constant;
using UnityEngine;
using UnityEngine.Networking;

public class WebRequesterV2
{
    public WebRequesterV2() { }

    /// <summary>
    /// シートから JSON を取得して T にデシリアライズ。
    /// 例外を投げるので呼び出し側で try/catch 推奨。
    /// </summary>
    public async Task GetDataAsync<T>(string sheetName, Action<T> action)
    {
        Debug.Log("StartGetData");
        using (var request = CreateRequestGetSheetJson(sheetName))
        {
            // 任意: リクエストタイムアウト秒（必要に応じて）
            // request.timeout = 15;

            await request.SendWebRequestAsync();

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
                throw new Exception(request.error ?? "Web request failed.");
            }

            Debug.Log("SuccessGetData");
            action(JsonConvert.DeserializeObject<T>(request.downloadHandler.text));
        }
    }

    private UnityWebRequest CreateRequestGetSheetJson(string sheetName)
    {
        var url = string.Format("{0}?{1}&{2}",
            AppConstants.SPREAD_SHEET_URL,
            Param("sheetId", AppConstants.SPREAD_SHEET_ID),
            Param("sheetName", sheetName));
        return UnityWebRequest.Get(url);
    }

    private string Param(string key, string param) => $"{key}={param}";
}
