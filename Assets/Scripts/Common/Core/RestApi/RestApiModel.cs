using System;
using System.IO;
using System.Net.Http;
using Scripts.Common.Features.Config;
using UnityEngine;

namespace Scripts.Common.Core.RestApi
{
    public class RestApiModel
    {
        public HttpClient Client { get; set; }
        public APIConfig APIConfig { get; set; }
        public int UploadTimeoutMS { get; set; } = 5 * 60 * 1000;

        public string BaseUrl { get; set; }
        public string LogFileName { get; set; }

        public string HealthEndpoint => string.Concat(BaseUrl, "/api/Health");
        public string UploadEndpoint => string.Concat(BaseUrl, "/api/Upload");
        public string DownloadEndpoint => string.Concat(BaseUrl, "/api/Download");
        public string GetMasterDataEndpoint => string.Concat(BaseUrl, "/api/GetMasterData");
        public string GetInspectionEndpoint => string.Concat(BaseUrl, "/api/GetInspection");
        public string PostInspectionEndpoint => string.Concat(BaseUrl, "/api/PostInspection");
        public string DownloadMinimapEndpoint => string.Concat(BaseUrl, "/api/DownloadMinimap");
        public string LogDirectoryPath => Path.Combine(Application.persistentDataPath, "M3Logs");
        public string LogFilePath => Path.Combine(LogDirectoryPath, LogFileName);
    }
}
