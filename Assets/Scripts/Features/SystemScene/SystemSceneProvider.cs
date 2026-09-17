using System;
using UnityEngine;

namespace Scripts.Features.SystemScene
{
    public class SystemSceneProvider
    {
        public long GetDeviceUtcSeconds()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        public double GetMonotonicSeconds()
        {
            return Time.realtimeSinceStartupAsDouble;
        }
    }
}
