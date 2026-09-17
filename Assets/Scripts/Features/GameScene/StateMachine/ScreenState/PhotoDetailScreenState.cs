using Scripts.Common.Core.StateMachine;
using Scripts.Features.GameScene.Header;

namespace Scripts.Features.GameScene.StateMachine.ScreenState
{
    public class PhotoDetailScreenState : IState<ScreenStateType>
    {
        private readonly IHeaderService _headerService;
        private readonly IStateMachineService _stateMachineService;
        private readonly StateMachineModel _model;

        public PhotoDetailScreenState(IHeaderService headerService, IStateMachineService stateMachineService, StateMachineModel model)
        {
            _headerService = headerService;
            _stateMachineService = stateMachineService;
            _model = model;
        }

        public ScreenStateType StateId => ScreenStateType.PhotoDetail;

        public void Enter()
        {
            _headerService.SetInputEnabled(false);
            _headerService.Apply("PHOTO DETAIL", true);
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
            _stateMachineService.ChangeScreenState(_model.PhotoDetailReturnScreen);
        }
    }
}
