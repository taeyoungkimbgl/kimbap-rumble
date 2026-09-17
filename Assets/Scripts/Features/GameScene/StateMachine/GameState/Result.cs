using Scripts.Common.Core.StateMachine;

namespace Scripts.Features.GameScene.StateMachine.GameState
{
    public class Result : IState<GameStateType>
    {
        public GameStateType StateId => GameStateType.Result;

        public void Enter() { }

        public void Update() { }

        public void Exit() { }
    }
}
