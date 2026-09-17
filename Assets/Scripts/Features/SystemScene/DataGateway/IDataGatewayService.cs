using System;
using System.Collections.Generic;

namespace Scripts.Features.SystemScene.DataGateway
{
    public interface IDataGatewayService : IDisposable
    {
        void Initialize(string streamingAssetsPath, string persistentDataPath);

        MasterMetadataData ReadMasterMetadata();
        IReadOnlyList<EquipmentData> ReadEquipment();
        EquipmentData ReadEquipment(string equipmentId);
        IReadOnlyList<LocationData> ReadLocations();
        LocationData ReadLocation(string locationId);
        IReadOnlyList<PhotoVariantData> ReadPhotoVariants();
        IReadOnlyList<PhotoVariantData> ReadPhotoVariants(string locationId);
        IReadOnlyList<PhotoLayerLayoutData> ReadPhotoLayerLayouts();
        IReadOnlyList<PhotoLayerLayoutData> ReadPhotoLayerLayouts(string photoVariantId);

        SaveMetadataData ReadSaveMetadata();
        IReadOnlyList<OwnedEquipmentData> ReadOwnedEquipment();
        IReadOnlyList<EquippedItemData> ReadEquippedItems();
        ActiveRumbleData ReadActiveRumble();
        IReadOnlyList<AlbumPhotoData> ReadAlbumPhotos();
        IReadOnlyList<DiscoveredLocationData> ReadDiscoveredLocations();

        void SaveEquippedItem(EquippedItemData item, long logicalUtcSeconds);
        void SaveActiveRumble(ActiveRumbleData rumble, long logicalUtcSeconds);
        long SaveRumbleResult(RumbleResultSaveData result);
        void DeleteAlbumPhoto(long photoId, long logicalUtcSeconds);
        void SaveLogicalUtc(long logicalUtcSeconds, long updatedUtcSeconds);

        byte[] ReadImageBytes(string relativePath);
    }
}
