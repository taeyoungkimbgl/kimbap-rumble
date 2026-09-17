namespace Scripts.Features.SystemScene.DataGateway
{
    public sealed class RumbleResultSaveData
    {
        public OwnedEquipmentData RewardEquipment { get; }
        public AlbumPhotoData Photo { get; }
        public DiscoveredLocationData DiscoveredLocation { get; }
        public long LogicalUtcSeconds { get; }

        public RumbleResultSaveData(
            OwnedEquipmentData rewardEquipment,
            AlbumPhotoData photo,
            DiscoveredLocationData discoveredLocation,
            long logicalUtcSeconds)
        {
            RewardEquipment = rewardEquipment;
            Photo = photo;
            DiscoveredLocation = discoveredLocation;
            LogicalUtcSeconds = logicalUtcSeconds;
        }
    }
}
