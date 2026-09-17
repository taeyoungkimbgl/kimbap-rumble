using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class BaseAnimator : MonoBehaviour
{
    [SerializeField] SpriteRenderer _spriteRenderer;
    CancellationTokenSource _animationCancellation;
    List<Sprite> _animationSprites = new();

    void Awake()
    {
        if (_spriteRenderer == null)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    public void Play(List<Sprite> sprites, float fps = 8f)
    {
        PlayInternal(sprites, fps, true, null);
    }

    public void PlayOnce(List<Sprite> sprites, Action onComplete = null, float fps = 8f)
    {
        PlayInternal(sprites, fps, false, onComplete);
    }

    void PlayInternal(List<Sprite> sprites, float fps, bool loop, Action onComplete)
    {
        StopAnimation();

        if (sprites == null || sprites.Count == 0 || _spriteRenderer == null)
        {
            return;
        }

        _animationSprites = sprites;

        _animationCancellation = new CancellationTokenSource();
        PlayAnimation(_animationSprites, fps, loop, onComplete, _animationCancellation);
    }

    async void PlayAnimation(
        List<Sprite> sprites,
        float fps,
        bool loop,
        Action onComplete,
        CancellationTokenSource animationCancellation)
    {
        var cancellationToken = animationCancellation.Token;
        var safeFps = Mathf.Max(1f, fps);
        var frameDelayMilliseconds = Mathf.RoundToInt(1000f / safeFps);

        try
        {
            var frameIndex = 0;

            while (!cancellationToken.IsCancellationRequested)
            {
                _spriteRenderer.sprite = sprites[frameIndex];
                await Task.Delay(frameDelayMilliseconds, cancellationToken);

                frameIndex++;

                if (frameIndex < sprites.Count)
                {
                    continue;
                }

                if (!loop)
                {
                    break;
                }

                frameIndex = 0;
            }

            if (!loop && !cancellationToken.IsCancellationRequested)
            {
                onComplete?.Invoke();
            }
        }
        catch (Exception exception)
        {
            if (exception is not TaskCanceledException && exception is not OperationCanceledException)
            {
                Debug.LogException(exception);
            }
        }
        finally
        {
            if (!loop && ReferenceEquals(_animationCancellation, animationCancellation))
            {
                _animationCancellation.Dispose();
                _animationCancellation = null;
            }
        }
    }


    void StopAnimation()
    {
        if (_animationCancellation == null)
        {
            return;
        }

        _animationCancellation.Cancel();
        _animationCancellation.Dispose();
        _animationCancellation = null;
    }

    public void Dispose()
    {
        StopAnimation();
    }
}
