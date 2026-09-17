using System;
using UnityEngine;

namespace Scripts.Features.SystemScene
{
    public class SystemSceneView : MonoBehaviour
    {
        public event Action Updated;
        public event Action ApplicationPaused;
        public event Action ApplicationQuitting;

        void Update()
        {
            Updated?.Invoke();
        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                ApplicationPaused?.Invoke();
            }
        }

        void OnApplicationQuit()
        {
            ApplicationQuitting?.Invoke();
        }
    }
}
