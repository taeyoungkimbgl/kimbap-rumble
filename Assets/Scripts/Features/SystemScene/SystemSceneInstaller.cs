using Scripts.Common.Core.RestApi;
using Scripts.Common.Features.Config;
using Scripts.Common.Log;
using Scripts.Features.SystemScene.DataGateway;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Scripts.Features.SystemScene
{
    public class SystemSceneInstaller : LifetimeScope
    {
        [SerializeField] SystemSceneView _view;
        [SerializeField] ConfigView _configView;
        [SerializeField] RestApiView _restApiView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<ISystemSceneService, SystemSceneServiceImpl>(Lifetime.Singleton);
            builder.Register<SystemSceneProvider>(Lifetime.Singleton);

            LogInstaller.Register(builder);
            ConfigInstaller.Register(builder, _configView);
            DataGatewayInstaller.Register(builder);
            RestApiInstaller.Register(builder, _restApiView);

            builder.RegisterComponent(_view);
            builder.Register<SystemSceneModel>(Lifetime.Singleton);
            builder.RegisterEntryPoint<SystemScenePresenter>();

            DontDestroyOnLoad(this);
        }
    }
}
