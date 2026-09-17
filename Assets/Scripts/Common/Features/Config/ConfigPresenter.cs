using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Scripts.Common.Features.Config
{
    public class ConfigPresenter : IStartable
    {
        [Inject] ConfigModel _model;
        [Inject] ConfigView _view;
        [Inject] IConfigService _service;
        [Inject] IConfigEnviourment _configEnviourment;

        public void Start()
        {
            _model.Enviourment = _configEnviourment.GetEnviourment();
        }
    }
}
