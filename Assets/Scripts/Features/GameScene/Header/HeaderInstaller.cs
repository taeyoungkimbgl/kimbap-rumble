using VContainer;
using VContainer.Unity;

namespace Scripts.Features.GameScene.Header
{
    public static class HeaderInstaller
    {
        public static void Register(IContainerBuilder builder, HeaderView view)
        {
            builder.Register<HeaderModel>(Lifetime.Singleton);
            builder.Register<IHeaderService, HeaderServiceImpl>(Lifetime.Singleton);
            builder.RegisterComponent(view);
            builder.RegisterEntryPoint<HeaderPresenter>();
        }
    }
}
