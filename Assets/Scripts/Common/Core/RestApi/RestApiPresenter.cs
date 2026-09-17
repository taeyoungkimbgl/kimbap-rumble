using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using Scripts.Common.Features.Config;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Scripts.Common.Core.RestApi
{
    public class RestApiPresenter : IStartable
    {
        [Inject] RestApiModel _model;
        [Inject] IConfigService _configService;

        public void Start()
        {
            // Debug.Log(GetType().Name + "Started");
            // _model.APIConfig = _configService.GetConfigs().APIConfig;
            // _model.BaseUrl = _configService.GetConfigs().APIConfig.BaseUrl;
            // Debug.Log("_model.BaseUrl: " + _model.BaseUrl);
            // InstanceClient(_model.APIConfig);
        }

        void InstanceClient(APIConfig config)
        {
            if (_model.Client == null)
            {
                // ハンドラの作成
                HttpClientHandler handler = new HttpClientHandler();

                if (_model.APIConfig.proxyAddress != null && _model.APIConfig.proxyAddress != "")
                {
                    handler.Proxy = new WebProxy(_model.APIConfig.proxyAddress);
                    handler.UseProxy = true;
                    // プロキシのログ出力
                    Debug.Log("GetProxy：" + handler.Proxy.GetProxy(new Uri(_model.APIConfig.BaseUrl)).ToString());
                }

                // HTTPCLIENTの作成
                // _log.Write("BaseAddress: " + config.BaseUrl);
                _model.UploadTimeoutMS = Math.Max(config.TimeoutMS, _model.UploadTimeoutMS);
                _model.Client = new HttpClient(handler);
                _model.Client.Timeout = Timeout.InfiniteTimeSpan;
                _model.Client.BaseAddress = new Uri(config.BaseUrl);
            }
        }
    }
}
