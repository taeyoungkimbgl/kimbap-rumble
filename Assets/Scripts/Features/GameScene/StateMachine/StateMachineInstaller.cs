using Scripts.Features.GameScene.StateMachine.GameState;
using Scripts.Features.GameScene.StateMachine.ScreenState;
using VContainer;
using VContainer.Unity;

namespace Scripts.Features.GameScene.StateMachine
{
    public static class StateMachineInstaller
    {
        public static void Register(IContainerBuilder builder)
        {
            builder.Register<IStateMachineService, StateMachineServiceImpl>(Lifetime.Singleton);
            builder.Register<StateMachineModel>(Lifetime.Singleton);
            builder.RegisterEntryPoint<StateMachinePresenter>();
            builder.RegisterEntryPoint<StateMachineUpdater>();

            builder.Register<RunInitialize>(Lifetime.Singleton);
            builder.Register<Idle>(Lifetime.Singleton);
            builder.Register<ScreenOpen>(Lifetime.Singleton);
            builder.Register<Result>(Lifetime.Singleton);

            builder.Register<HomeScreenState>(Lifetime.Singleton);
            builder.Register<InventoryScreenState>(Lifetime.Singleton);
            builder.Register<AlbumScreenState>(Lifetime.Singleton);
            builder.Register<PhotoDetailScreenState>(Lifetime.Singleton);
            builder.Register<ResultScreenState>(Lifetime.Singleton);
        }
    }
}
