using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class UnityWebRequestExtensions
{
    /// <summary>
    /// UnityWebRequest.SendWebRequest() を Task 化して await 可能にする。
    /// 必ずメインスレッドから呼び出してください。
    /// </summary>
    public static Task<UnityWebRequest> SendWebRequestAsync(
        this UnityWebRequest request,
        CancellationToken cancellationToken = default)
    {
        var tcs = new TaskCompletionSource<UnityWebRequest>();

        var op = request.SendWebRequest();

        void OnCompleted(AsyncOperation _)
        {
            op.completed -= OnCompleted;
            tcs.TrySetResult(request);
        }

        op.completed += OnCompleted;

        if (cancellationToken.CanBeCanceled)
        {
            cancellationToken.Register(() =>
            {
                try { request.Abort(); } catch { /* no-op */ }
                op.completed -= OnCompleted;
                tcs.TrySetCanceled(cancellationToken);
            });
        }

        return tcs.Task;
    }
}
