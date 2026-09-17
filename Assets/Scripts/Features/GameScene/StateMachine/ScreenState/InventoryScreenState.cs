using Scripts.Common.Core.StateMachine;
using Scripts.Features.GameScene.Header;

namespace Scripts.Features.GameScene.StateMachine.ScreenState
{
    public class InventoryScreenState : IState<ScreenStateType>
    {
        private readonly IHeaderService _headerService;
        private readonly IStateMachineService _stateMachineService;

        public InventoryScreenState(IHeaderService headerService, IStateMachineService stateMachineService)
        {
            _headerService = headerService;
            _stateMachineService = stateMachineService;
        }

        public ScreenStateType StateId => ScreenStateType.Inventory;

        public void Enter()
        {
            _headerService.SetInputEnabled(false);
            _headerService.Apply("INVENTORY", true);
            _headerService.BackRequested += OnBackRequested;
        }

        public void Update() { }

        public void Exit()
        {
            _headerService.SetInputEnabled(false);
            _headerService.BackRequested -= OnBackRequested;
        }

        private void OnBackRequested()
        {
            _stateMachineService.ChangeScreenState(ScreenStateType.Home);
        }
    }
}
