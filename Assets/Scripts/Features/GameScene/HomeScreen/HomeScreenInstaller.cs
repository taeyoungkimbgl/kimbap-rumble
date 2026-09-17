using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Scripts.Features.GameScene.HomeScreen
{
    public static class HomeScreenInstaller
    {
        public static void Register(IContainerBuilder builder, HomeScreenView view)
        {
            builder.Register<IHomeScreenService, HomeScreenServiceImpl>(Lifetime.Singleton);

            builder.RegisterComponent(view);
            builder.Register<HomeScreenModel>(Lifetime.Singleton);
            builder.RegisterEntryPoint<HomeScreenPresenter>();
        }
    }
}
