using Scripts.Common.Core.StateMachine;
using Scripts.Features.GameScene.Header;

namespace Scripts.Features.GameScene.StateMachine.ScreenState
{
    public class HomeScreenState : IState<ScreenStateType>
    {
        private readonly IHeaderService _headerService;

        public HomeScreenState(IHeaderService headerService)
        {
            _headerService = headerService;
        }

        public ScreenStateType StateId => ScreenStateType.Home;

        public void Enter()
        {
            _headerService.SetInputEnabled(false);
            _headerService.Apply("Kimbap Rumble", false);
        }

        public void Update() { }

        public void Exit()
        {
            _headerService.SetInputEnabled(false);
        }
    }
}
