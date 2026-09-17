using Scripts.Common.Core.StateMachine;
using Scripts.Features.GameScene.Header;

namespace Scripts.Features.GameScene.StateMachine.ScreenState
{
    public class AlbumScreenState : IState<ScreenStateType>
    {
        private readonly IHeaderService _headerService;
        private readonly IStateMachineService _stateMachineService;

        public AlbumScreenState(IHeaderService headerService, IStateMachineService stateMachineService)
        {
            _headerService = headerService;
            _stateMachineService = stateMachineService;
        }

        public ScreenStateType StateId => ScreenStateType.Album;

        public void Enter()
        {
            _headerService.SetInputEnabled(false);
            _headerService.Apply("PHOTO ALBUM", true);
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
