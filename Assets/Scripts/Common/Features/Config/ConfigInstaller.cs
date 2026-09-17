using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Scripts.Common.Features.Config
{
    public static class ConfigInstaller
    {
        public static void Register(IContainerBuilder builder, ConfigView view)
        {
            builder.Register<IConfigService, ConfigServiceImpl>(Lifetime.Singleton);

            builder.RegisterComponent(view.Enviourment).As<IConfigEnviourment>();
            builder.RegisterComponent(view);
            builder.Register<ConfigModel>(Lifetime.Singleton);
            builder.RegisterEntryPoint<ConfigPresenter>();
        }
    }
}
