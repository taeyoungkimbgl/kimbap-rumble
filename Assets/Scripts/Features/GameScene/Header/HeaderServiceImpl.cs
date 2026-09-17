using System;

namespace Scripts.Features.GameScene.Header
{
    public class HeaderServiceImpl : IHeaderService
    {
        private readonly HeaderModel _model;

        public HeaderServiceImpl(HeaderModel model)
        {
            _model = model;
        }

        public event Action BackRequested
        {
            add => _model.BackRequested += value;
            remove => _model.BackRequested -= value;
        }

        public void Apply(string title, bool backVisible)
        {
            _model.Apply(title, backVisible);
        }

        public void SetInputEnabled(bool enabled)
        {
            _model.SetInputEnabled(enabled);
        }
    }
}
