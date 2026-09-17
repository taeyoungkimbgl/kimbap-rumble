using Scripts.Common.Core.StateMachine;

namespace Scripts.Features.GameScene.StateMachine.GameState
{
    public class ScreenOpen : IState<GameStateType>
    {
        public GameStateType StateId => GameStateType.ScreenOpen;

        public void Enter() { }

        public void Update() { }

        public void Exit() { }
    }
}
