using System;
using UnityEngine;

namespace Scripts.Common.Features.Config
{
    public class ConfigEnviourment : MonoBehaviour, IConfigEnviourment
    {
        public BootType BootType;
        public Enviourment[] Enviourments;
        public Enviourment GetEnviourment()
        {
            foreach (var item in Enviourments)
            {
                if (item.ID == BootType.ToString()) return item;
            }
            return null;
        }
    }

    public enum BootType
    {
        Release,
        Develop
    }

    [Serializable]
    public class Enviourment
    {
        public string ID;
        public APIConfig APIConfig;
        public DBConfig DBConfig;
        public LogConfig LogConfig;
    }

    [Serializable]
    public class APIConfig
    {
        public string BaseUrl;
        public int ReteryTimes;
        public int TimeoutMS;
        public string proxyAddress;
    }

    [Serializable]
    public class DBConfig
    {
        public int ReteryTimes;
        public string FilePath;
        public string FileName;
        public int TransactionWaitTime = 5000;// 排他制御のセマフォ取得の待機時間(ミリ秒)
    }

    [Serializable]
    public class LogConfig
    {
        public string FilePath;
        public string FileName;
        // public LogLevel LogLevel = LogLevel.DEBUG;
        // public RollingInterval LogRotation = RollingInterval.Day;
    }
}
