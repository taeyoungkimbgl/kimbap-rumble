using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Scripts.Common.Core.RestApi
{
    public interface IRestApiService
    {
        Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken,
            int timeoutMs);

        Task<string> GetAsync(string url);
    }
}
