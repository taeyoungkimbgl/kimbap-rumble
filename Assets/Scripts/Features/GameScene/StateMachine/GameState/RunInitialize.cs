using UnityEngine;
using Scripts.Common.Core.StateMachine;

namespace Scripts.Features.GameScene.StateMachine.GameState
{
    public class RunInitialize : IState<GameStateType>
    {
        public GameStateType StateId => GameStateType.RunInitialize;

        public void Enter()
        {
            Debug.Log(this.GetType().Name + "Enter");
        }

        public void Update() { }

        public void Exit() { }
    }
}
