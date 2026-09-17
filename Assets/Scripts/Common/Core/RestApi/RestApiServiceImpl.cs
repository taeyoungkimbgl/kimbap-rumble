using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Scripts.Common.Log;
using VContainer;

namespace Scripts.Common.Core.RestApi
{
    public class RestApiServiceImpl : IRestApiService
    {
        [Inject] RestApiModel _model;
        [Inject] ILogService _log;

        public async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken,
            int timeoutMs)
        {
            using var linkedCancellationTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            if (timeoutMs > 0)
            {
                linkedCancellationTokenSource.CancelAfter(timeoutMs);
            }

            try
            {
                _log.Write($"StartSendAsync {request.Method} {request.RequestUri}");
                return await _model.Client.SendAsync(
                    request,
                    HttpCompletionOption.ResponseHeadersRead,
                    linkedCancellationTokenSource.Token);
            }
            catch (OperationCanceledException e) when (!cancellationToken.IsCancellationRequested)
            {
                _log.Write($"HTTP通信タイムアウトエラー timeoutMs={timeoutMs}");
                throw new TimeoutException($"Request timed out after {timeoutMs}ms.", e);
            }
            catch (Exception e)
            {
                _log.Write($"HTTP通信エラー {e}");
                throw;
            }
            finally
            {
                _log.Write("Finish Task");
            }
        }

        public async Task<string> GetAsync(string url)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            try
            {
                using var httpResponse = await SendAsync(
                    request,
                    CancellationToken.None,
                    _model.APIConfig.TimeoutMS);

                // ステータスコードが200以外の場合、業務エラー
                if (httpResponse.StatusCode != HttpStatusCode.OK)
                {
                    _log.Write("通信エラー　コード：" + (int)httpResponse.StatusCode);
                    return null;
                }

                // 結果を返却
                return await httpResponse.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                _log.Write($"GET通信エラー {e}");
                return null;
            }
        }
    }
}
