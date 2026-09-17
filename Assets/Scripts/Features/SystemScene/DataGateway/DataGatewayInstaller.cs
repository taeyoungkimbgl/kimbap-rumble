using VContainer;

namespace Scripts.Features.SystemScene.DataGateway
{
    public static class DataGatewayInstaller
    {
        public static void Register(IContainerBuilder builder)
        {
            builder.Register<IDataGatewayService, DataGatewayServiceImpl>(Lifetime.Singleton);
            builder.Register<DataGatewayModel>(Lifetime.Singleton);
            builder.Register<DataGatewayView>(Lifetime.Singleton);
            builder.Register<DataGatewayPresenter>(Lifetime.Singleton);
            builder.Register<DataGatewaySqlReader>(Lifetime.Singleton);
            builder.Register<DataGatewayReader>(Lifetime.Singleton);
            builder.Register<DataGatewayWriter>(Lifetime.Singleton);
            builder.Register<DataGatewayCoordinator>(Lifetime.Singleton);

            builder.Register<DataGatewayMasterMetadataReader>(Lifetime.Singleton);
            builder.Register<DataGatewayEquipmentReader>(Lifetime.Singleton);
            builder.Register<DataGatewayLocationReader>(Lifetime.Singleton);
            builder.Register<DataGatewayPhotoVariantReader>(Lifetime.Singleton);
            builder.Register<DataGatewayPhotoLayerLayoutReader>(Lifetime.Singleton);
            builder.Register<DataGatewaySaveMetadataReader>(Lifetime.Singleton);
            builder.Register<DataGatewayOwnedEquipmentReader>(Lifetime.Singleton);
            builder.Register<DataGatewayEquippedItemReader>(Lifetime.Singleton);
            builder.Register<DataGatewayActiveRumbleReader>(Lifetime.Singleton);
            builder.Register<DataGatewayAlbumPhotoReader>(Lifetime.Singleton);
            builder.Register<DataGatewayDiscoveredLocationReader>(Lifetime.Singleton);

            builder.Register<DataGatewaySaveMetadataWriter>(Lifetime.Singleton);
            builder.Register<DataGatewayOwnedEquipmentWriter>(Lifetime.Singleton);
            builder.Register<DataGatewayEquippedItemWriter>(Lifetime.Singleton);
            builder.Register<DataGatewayActiveRumbleWriter>(Lifetime.Singleton);
            builder.Register<DataGatewayAlbumPhotoWriter>(Lifetime.Singleton);
            builder.Register<DataGatewayDiscoveredLocationWriter>(Lifetime.Singleton);
        }
    }
}
