using Scripts.Features.GameScene.Header;
using Scripts.Features.GameScene.HomeScreen;
using Scripts.Features.GameScene.StateMachine;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Scripts.Features.GameScene
{
    public class GameSceneInstaller : LifetimeScope
    {
        [SerializeField] GameSceneView _view;
        [SerializeField] HomeScreenView _homeScreenView;
        [SerializeField] HeaderView _headerView;

        protected override void Configure(IContainerBuilder builder)
        {
            StateMachineInstaller.Register(builder);
            HeaderInstaller.Register(builder, _headerView);
            HomeScreenInstaller.Register(builder, _homeScreenView);

            builder.Register<IGameSceneService, GameSceneServiceImpl>(Lifetime.Singleton);

            builder.RegisterComponent(_view);
            builder.Register<GameSceneModel>(Lifetime.Singleton);
            builder.RegisterEntryPoint<GameScenePresenter>();
        }
    }
}
