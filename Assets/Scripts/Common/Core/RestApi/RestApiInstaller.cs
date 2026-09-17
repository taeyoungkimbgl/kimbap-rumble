using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Scripts.Common.Core.RestApi
{
    public static class RestApiInstaller
    {
        public static void Register(IContainerBuilder builder, RestApiView view)
        {
            builder.RegisterComponent(view);
            builder.Register<RestApiModel>(Lifetime.Singleton);
            builder.Register<IRestApiService, RestApiServiceImpl>(Lifetime.Singleton);

            builder.RegisterEntryPoint<RestApiPresenter>();
        }
    }
}
