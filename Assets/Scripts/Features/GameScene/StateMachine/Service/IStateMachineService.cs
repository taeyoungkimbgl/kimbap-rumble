using Scripts.Common.Core.StateMachine;

namespace Scripts.Features.GameScene.StateMachine
{
    public interface IStateMachineService
    {
        void RegisterGameState(IState<GameStateType> state);
        void RegisterScreenState(IState<ScreenStateType> state);
        void RegisterDetectionState(IState<DetectionStateType> state);
        void ChangeGameState(GameStateType type, bool allowReentry = false);
        void ChangeScreenState(ScreenStateType type, bool allowReentry = false);
        void ChangeDetectionState(DetectionStateType type);
    }
}
