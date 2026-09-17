using System;
using VContainer.Unity;

namespace Scripts.Features.GameScene.Header
{
    public class HeaderPresenter : IStartable, IDisposable
    {
        private readonly HeaderModel _model;
        private readonly HeaderView _view;

        public HeaderPresenter(HeaderModel model, HeaderView view)
        {
            _model = model;
            _view = view;
        }

        public void Start()
        {
            _model.Changed += Render;
            _view.BackClicked += OnBackClicked;
            Render();
        }

        public void Dispose()
        {
            _model.Changed -= Render;
            _view.BackClicked -= OnBackClicked;
        }

        private void Render()
        {
            _view.Render(_model.Title, _model.BackVisible, _model.InputEnabled);
        }

        private void OnBackClicked()
        {
            _model.RequestBack();
        }
    }
}
