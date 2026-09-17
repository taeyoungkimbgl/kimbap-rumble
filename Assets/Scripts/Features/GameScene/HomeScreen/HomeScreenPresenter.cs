using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Scripts.Features.GameScene.HomeScreen
{
    public class HomeScreenPresenter : IStartable
    {
        [Inject] private HomeScreenModel _model;
        [Inject] private HomeScreenView _view;
        [Inject] private IHomeScreenService _service;

        public void Start()
        {
            // TODO: 初期化処理
        }
    }
}
