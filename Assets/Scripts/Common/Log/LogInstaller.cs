using VContainer;
using VContainer.Unity;

namespace Scripts.Common.Log
{
    public static class LogInstaller
    {
        public static void Register(IContainerBuilder builder)
        {
            builder.Register<ILogService, LogServiceImpl>(Lifetime.Singleton);
            builder.Register<LogModel>(Lifetime.Singleton);

            builder.RegisterEntryPoint<LogPresenter>();
        }
    }
}
