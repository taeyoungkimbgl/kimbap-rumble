using Scripts.Features.SystemScene.DataGateway;

namespace Scripts.Features.SystemScene
{
    public class SystemSceneServiceImpl : ISystemSceneService
    {
        const long ClockRollbackToleranceSeconds = 300L;
        const double SaveIntervalSeconds = 60d;

        readonly IDataGatewayService _dataGatewayService;
        readonly SystemSceneProvider _provider;

        long _initialLogicalUtcSeconds;
        double _initialMonotonicSeconds;
        double _lastSaveMonotonicSeconds;
        bool _clockRollbackNoticePending;

        public SystemSceneServiceImpl(
            IDataGatewayService dataGatewayService,
            SystemSceneProvider provider)
        {
            _dataGatewayService = dataGatewayService;
            _provider = provider;
        }

        public void Initialize()
        {
            var savedLogicalUtcSeconds = _dataGatewayService.ReadSaveMetadata().LogicalUtcSeconds;
            var deviceUtcSeconds = _provider.GetDeviceUtcSeconds();

            _initialLogicalUtcSeconds = savedLogicalUtcSeconds >= deviceUtcSeconds
                ? savedLogicalUtcSeconds
                : deviceUtcSeconds;
            _initialMonotonicSeconds = _provider.GetMonotonicSeconds();
            _lastSaveMonotonicSeconds = _initialMonotonicSeconds;
            _clockRollbackNoticePending =
                savedLogicalUtcSeconds - deviceUtcSeconds > ClockRollbackToleranceSeconds;

            SaveLogicalUtc(_initialMonotonicSeconds);
        }

        public long GetCurrentLogicalUtcSeconds()
        {
            return GetCurrentLogicalUtcSeconds(_provider.GetMonotonicSeconds());
        }

        public void Update()
        {
            var monotonicSeconds = _provider.GetMonotonicSeconds();
            if (monotonicSeconds - _lastSaveMonotonicSeconds < SaveIntervalSeconds)
            {
                return;
            }

            SaveLogicalUtc(monotonicSeconds);
        }

        public void SaveLogicalUtc()
        {
            SaveLogicalUtc(_provider.GetMonotonicSeconds());
        }

        public bool TryConsumeClockRollbackNotice()
        {
            if (!_clockRollbackNoticePending)
            {
                return false;
            }

            _clockRollbackNoticePending = false;
            return true;
        }

        long GetCurrentLogicalUtcSeconds(double monotonicSeconds)
        {
            var elapsedSeconds = monotonicSeconds - _initialMonotonicSeconds;
            return _initialLogicalUtcSeconds + (long)elapsedSeconds;
        }

        void SaveLogicalUtc(double monotonicSeconds)
        {
            var logicalUtcSeconds = GetCurrentLogicalUtcSeconds(monotonicSeconds);
            _lastSaveMonotonicSeconds = monotonicSeconds;
            _dataGatewayService.SaveLogicalUtc(logicalUtcSeconds, logicalUtcSeconds);
        }
    }
}
