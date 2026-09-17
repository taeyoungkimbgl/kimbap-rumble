using System.Collections.Generic;
using Scripts.Common.Core.StateMachine;

namespace Scripts.Features.GameScene.StateMachine
{
    public class StateMachineModel
    {
        public List<IState<GameStateType>> GameStates { get; set; } = new();
        public List<IState<ScreenStateType>> ScreenStates { get; set; } = new();
        public List<IState<DetectionStateType>> DetectionStates { get; set; } = new();
        public IState<GameStateType> CurrentGameState { get; set; }
        public IState<ScreenStateType> CurrentScreenState { get; set; }
        public IState<DetectionStateType> CurrentDetectionState { get; set; }
        public ScreenStateType PhotoDetailReturnScreen { get; set; }
    }

    public enum GameStateType
    {
        RunInitialize,
        Idle,
        ScreenOpen,
        Result,
    }

    public enum ScreenStateType
    {
        Home,
        Inventory,
        Album,
        PhotoDetail,
        Result,
    }
    public enum DetectionStateType
    {
        DetectionStandby,
        StandbyQRCode,
        DetectingQRCode,
        DetectedQRCode,
    }
}
