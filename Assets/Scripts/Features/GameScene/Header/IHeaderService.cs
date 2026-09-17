using System;

namespace Scripts.Features.GameScene.Header
{
    public interface IHeaderService
    {
        void Apply(string title, bool backVisible);
        void SetInputEnabled(bool enabled);
        event Action BackRequested;
    }
}
