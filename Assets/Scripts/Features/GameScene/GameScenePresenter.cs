using Scripts.Features.GameScene.StateMachine;
using Scripts.Features.SystemScene.DataGateway;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Scripts.Features.GameScene
{
    public class GameScenePresenter : IStartable
    {
        [Inject] GameSceneModel _model;
        [Inject] GameSceneView _view;
        [Inject] IGameSceneService _service;
        [Inject] IDataGatewayService _dataGatewayService;
        [Inject] IStateMachineService _stateMachineService;

        public void Start()
        {
            Debug.Log(GetType().Name + "Started");
            LogMasterRecordCounts();
            _stateMachineService.ChangeGameState(GameStateType.RunInitialize);

        }

        void LogMasterRecordCounts()
        {
            _dataGatewayService.ReadMasterMetadata();
            Debug.Log("[MasterData] master_metadata: 1");
            Debug.Log($"[MasterData] equipment: {_dataGatewayService.ReadEquipment().Count}");
            Debug.Log($"[MasterData] locations: {_dataGatewayService.ReadLocations().Count}");
            Debug.Log($"[MasterData] photo_variants: {_dataGatewayService.ReadPhotoVariants().Count}");
            Debug.Log($"[MasterData] photo_layer_layouts: {_dataGatewayService.ReadPhotoLayerLayouts().Count}");
        }
    }
}
