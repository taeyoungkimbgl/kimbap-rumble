using VContainer;
using VContainer.Unity;

namespace Scripts.Features.GameScene.StateMachine
{
    public class StateMachineUpdater : ITickable
    {
        [Inject] StateMachineModel _model;

        public void Tick()
        {
            _model.CurrentGameState?.Update();
            _model.CurrentScreenState?.Update();
            _model.CurrentDetectionState?.Update();
        }
    }
}
