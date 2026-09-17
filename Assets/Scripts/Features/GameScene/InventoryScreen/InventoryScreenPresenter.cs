using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Scripts.Features.GameScene.InventoryScreen
{
    public class InventoryScreenPresenter : IStartable
    {
        [Inject] private InventoryScreenModel _model;
        [Inject] private InventoryScreenView _view;
        [Inject] private IInventoryScreenService _service;

        public void Start()
        {
            // TODO: 初期化処理
        }
    }
}
