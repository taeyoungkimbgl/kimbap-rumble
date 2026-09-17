using Scripts.Common.Core.StateMachine;

namespace Scripts.Features.GameScene.StateMachine.GameState
{
    public class Idle : IState<GameStateType>
    {
        public GameStateType StateId => GameStateType.Idle;

        public void Enter() { }

        public void Update() { }

        public void Exit() { }
    }
}
