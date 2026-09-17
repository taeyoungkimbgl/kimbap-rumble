using System;
using UnityEngine;

namespace Scripts.Common.Log
{
    public static class LogStartupContext
    {
        static DateTime _startupTime = DateTime.Now;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void CaptureStartupTime()
        {
            _startupTime = DateTime.Now;
        }

        public static string StartupTimestamp => _startupTime.ToString("yyMMddHHmmss");
    }
}
