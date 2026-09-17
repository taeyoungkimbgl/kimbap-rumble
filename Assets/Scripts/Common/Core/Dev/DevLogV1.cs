using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DevLogV1 : MonoBehaviour
{
    DevButtonManagerV1 _devButtonManager;


    // public Button[] Buttons;
    [SerializeField] ScrollRect historyScroll;   // インスペクタでアサイン
    public GameObject TextLogParent;
    public GameObject PrefabTextLog;

    private StringBuilder sb = new StringBuilder();
    // private bool autoScroll = true;

    [SerializeField, Tooltip("テキストの先頭に時刻を表示する")]
    private bool useTimeStamp = true;

    [SerializeField, Tooltip("ログの種別に応じて色を付ける")]
    private bool coloredByLogType = true;

    [SerializeField, Tooltip("ログの詳細を表示")]
    private bool isShowStackTrace = false;

    [SerializeField, Tooltip("特定の文字列を含むログは表示しない")]
    private string[] ignore = new string[] { "[OVR" };

    TextMeshProUGUI textLog;
    public void Initialize()
    {
        Application.logMessageReceived += HandleLog;
        textLog = Instantiate(PrefabTextLog, TextLogParent.transform).GetComponent<TextMeshProUGUI>();
        sb = new StringBuilder();

        // _devButtonManager = GetComponent<DevButtonManagerV1>();
        // _devButtonManager.AddListener(DevButtonTypeV1.ClearDebugLog, CleanText);

        // Debug.Log("initializedChatchLog");
    }



    public void CleanText()
    {
        //自分の子供を全て調べる
        foreach (Transform child in TextLogParent.transform)
        {
            //自分の子供をDestroyする
            Destroy(child.gameObject);
        }
    }

    private void OnEnable()
    {

    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
        sb = null;
    }

    private void HandleLog(string logText, string stackTrace, LogType logType)
    {
        sb.Clear();

        if (0 < ignore.Length)
        {
            for (int i = 0; i < ignore.Length; i++)
            {
                if (ignore[i] != string.Empty && logText.Contains(ignore[i]))
                    return;
            }
        }

        if (useTimeStamp)
            // log.Append(string.Format("[{0}:{1:D3}] ", DateTime.Now.ToLongTimeString(), DateTime.Now.Millisecond));
            sb.Append(string.Format("[{0}] ", DateTime.Now.ToString("HH:mm:ss")));

        if (coloredByLogType)
        {
            switch (logType)
            {
                case LogType.Assert:
                case LogType.Warning:
                    // logText = GetColoredString(logText, "yellow");
                    break;
                case LogType.Error:
                case LogType.Exception:
                    logText = GetColoredString(logText, "red");
                    break;
                default:
                    break;
            }
        }
        switch (logType)
        {

            case LogType.Assert:
            case LogType.Warning:
                break;
            case LogType.Exception:
            case LogType.Error:
                sb.Append(logText);
                sb.AppendLine();
                if (isShowStackTrace)
                {
                    sb.Append("========================");
                    sb.AppendLine();
                    sb.Append(stackTrace);
                    sb.Append("========================");
                    sb.AppendLine();
                }
                AddLog(sb.ToString());
                break;
            case LogType.Log:
                sb.Append(logText);
                sb.AppendLine();
                AddLog(sb.ToString());
                break;
        }
    }

    void AddLog(string log)
    {
        // var textLog = Instantiate(PrefabTextLog, TextLogParent.transform).GetComponent<TextMeshProUGUI>();
        textLog.text += log;
        // textLog.gameObject.transform.SetSiblingIndex(0);
        historyScroll.verticalNormalizedPosition = 0f;
    }

    private string GetColoredString(string src, string color)
    {
        return string.Format("<color={0}>{1}</color>", color, src);
    }
}
