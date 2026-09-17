using System;
using Scripts.Features.GameScene.StateMachine.GameState;
using Scripts.Features.GameScene.StateMachine.ScreenState;
using VContainer;
using VContainer.Unity;

namespace Scripts.Features.GameScene.StateMachine
{
    public class StateMachinePresenter : IStartable, IDisposable
    {
        [Inject] IStateMachineService _service;
        [Inject] StateMachineModel _model;

        [Inject] RunInitialize _runInitialize;
        [Inject] Idle _idle;
        [Inject] ScreenOpen _screenOpen;
        [Inject] Result _result;

        [Inject] HomeScreenState _homeScreenState;
        [Inject] InventoryScreenState _inventoryScreenState;
        [Inject] AlbumScreenState _albumScreenState;
        [Inject] PhotoDetailScreenState _photoDetailScreenState;
        [Inject] ResultScreenState _resultScreenState;

        public void Start()
        {
            Initialize();
        }

        public void Dispose()
        {
            _model.CurrentScreenState?.Exit();
            _model.CurrentScreenState = null;
        }

        public void Initialize()
        {
            _service.RegisterGameState(_runInitialize);
            _service.RegisterGameState(_idle);
            _service.RegisterGameState(_screenOpen);
            _service.RegisterGameState(_result);

            _service.RegisterScreenState(_homeScreenState);
            _service.RegisterScreenState(_inventoryScreenState);
            _service.RegisterScreenState(_albumScreenState);
            _service.RegisterScreenState(_photoDetailScreenState);
            _service.RegisterScreenState(_resultScreenState);
        }
    }
}
