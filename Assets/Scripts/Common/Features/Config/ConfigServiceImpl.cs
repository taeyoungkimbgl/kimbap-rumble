using VContainer;

namespace Scripts.Common.Features.Config
{
    public class ConfigServiceImpl : IConfigService
    {
        [Inject] ConfigModel _model;
        public Enviourment GetConfigs() => _model.Enviourment;
    }
}
