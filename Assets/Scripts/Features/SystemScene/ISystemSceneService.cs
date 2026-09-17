namespace Scripts.Features.SystemScene
{
    public interface ISystemSceneService
    {
        void Initialize();
        long GetCurrentLogicalUtcSeconds();
        void Update();
        void SaveLogicalUtc();
        bool TryConsumeClockRollbackNotice();
    }
}
