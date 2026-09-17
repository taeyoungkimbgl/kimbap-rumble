using System.Linq;
using Scripts.Common.Core.StateMachine;
using UnityEngine;
using VContainer;

namespace Scripts.Features.GameScene.StateMachine
{
    public class StateMachineServiceImpl : IStateMachineService
    {
        [Inject] StateMachineModel _model;

        public void RegisterGameState(IState<GameStateType> state)
        {
            if (_model.GameStates.Any(x => x.StateId == state.StateId))
            {
                return;
            }

            _model.GameStates.Add(state);
        }

        public void RegisterScreenState(IState<ScreenStateType> state)
        {
            if (_model.ScreenStates.Any(x => x.StateId == state.StateId))
            {
                return;
            }

            _model.ScreenStates.Add(state);
        }

        public void RegisterDetectionState(IState<DetectionStateType> state)
        {
            if (_model.DetectionStates.Any(x => x.StateId == state.StateId))
            {
                return;
            }

            _model.DetectionStates.Add(state);
        }

        public void ChangeGameState(GameStateType type, bool allowReentry = false)
        {
            if (_model.CurrentGameState != null
                && _model.CurrentGameState.StateId == type
                && !allowReentry)
            {
                return;
            }

            var nextState = _model.GameStates.FirstOrDefault(x => x.StateId == type);
            if (nextState == null)
            {
                Debug.LogError($"GameScene game state is not registered. state={type}");
                return;
            }

            _model.CurrentGameState?.Exit();
            _model.CurrentGameState = nextState;
            _model.CurrentGameState.Enter();
        }

        public void ChangeScreenState(ScreenStateType type, bool allowReentry = false)
        {
            if (_model.CurrentScreenState != null
                && _model.CurrentScreenState.StateId == type
                && !allowReentry)
            {
                return;
            }

            var nextState = _model.ScreenStates.FirstOrDefault(x => x.StateId == type);
            if (nextState == null)
            {
                Debug.LogError($"GameScene screen state is not registered. state={type}");
                return;
            }

            _model.CurrentScreenState?.Exit();
            _model.CurrentScreenState = nextState;
            _model.CurrentScreenState.Enter();
        }

        public void ChangeDetectionState(DetectionStateType type)
        {
            if (_model.CurrentDetectionState != null
                && _model.CurrentDetectionState.StateId == type)
            {
                return;
            }

            var nextState = _model.DetectionStates.FirstOrDefault(x => x.StateId == type);
            if (nextState == null)
            {
                Debug.LogError($"GameScene detection state is not registered. state={type}");
                return;
            }

            _model.CurrentDetectionState?.Exit();
            _model.CurrentDetectionState = nextState;
            _model.CurrentDetectionState.Enter();
        }
    }
}
