using System;
using Scripts.Features.SystemScene.DataGateway;
using seiko.framework.bases.utils;
using seiko.framework.gyomu.constant;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace Scripts.Features.SystemScene
{
    public class SystemScenePresenter : IStartable, IDisposable
    {
        [Inject] IDataGatewayService _dataGatewayService;
        [Inject] ISystemSceneService _systemSceneService;
        [Inject] SystemSceneView _view;

        public void Start()
        {
            MessageUtil.InstallMessageFile();

            try
            {
                _dataGatewayService.Initialize(
                    Application.streamingAssetsPath,
                    Application.persistentDataPath);
                _systemSceneService.Initialize();

                _view.Updated += OnUpdated;
                _view.ApplicationPaused += OnApplicationPaused;
                _view.ApplicationQuitting += OnApplicationQuitting;

                if (_systemSceneService.TryConsumeClockRollbackNotice())
                {
                    Debug.Log($"time is wrong:");
                    return;
                }

                SceneManager.LoadScene(AppConstants.GAME_SCENE);
            }
            catch (Exception e)
            {
                Debug.LogError($"System scene initialization failed: {e}");
            }
        }

        public void Dispose()
        {
            _view.Updated -= OnUpdated;
            _view.ApplicationPaused -= OnApplicationPaused;
            _view.ApplicationQuitting -= OnApplicationQuitting;
        }

        void OnUpdated()
        {
            _systemSceneService.Update();
        }

        void OnApplicationPaused()
        {
            _systemSceneService.SaveLogicalUtc();
        }

        void OnApplicationQuitting()
        {
            _systemSceneService.SaveLogicalUtc();
        }
    }
}
