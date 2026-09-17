using Scripts.Common.Core.StateMachine;
using Scripts.Features.GameScene.Header;

namespace Scripts.Features.GameScene.StateMachine.ScreenState
{
    public class ResultScreenState : IState<ScreenStateType>
    {
        private readonly IHeaderService _headerService;

        public ResultScreenState(IHeaderService headerService)
        {
            _headerService = headerService;
        }

        public ScreenStateType StateId => ScreenStateType.Result;

        public void Enter()
        {
            _headerService.SetInputEnabled(false);
            _headerService.Apply("RUMBLE RESULT", false);
        }

        public void Update() { }

        public void Exit()
        {
            _headerService.SetInputEnabled(false);
        }
    }
}
