using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Scripts.Features.GameScene.InventoryScreen
{
    public static class InventoryScreenInstaller
    {
        public static void Register(IContainerBuilder builder, InventoryScreenView view)
        {
            builder.Register<IInventoryScreenService, InventoryScreenServiceImpl>(Lifetime.Singleton);

            builder.RegisterComponent(view);
            builder.Register<InventoryScreenModel>(Lifetime.Singleton);
            builder.RegisterEntryPoint<InventoryScreenPresenter>();
        }
    }
}
