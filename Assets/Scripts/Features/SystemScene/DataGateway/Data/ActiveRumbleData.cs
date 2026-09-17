namespace Scripts.Features.SystemScene.DataGateway
{
    public sealed class ActiveRumbleData
    {
        public long StartedUtcSeconds { get; }
        public long CompletesUtcSeconds { get; }
        public string WeaponId { get; }
        public string ShieldId { get; }
        public string AccessoryId { get; }
        public string LocationId { get; }
        public string PhotoVariantId { get; }
        public string RewardEquipmentId { get; }

        public ActiveRumbleData(
            long startedUtcSeconds,
            long completesUtcSeconds,
            string weaponId,
            string shieldId,
            string accessoryId,
            string locationId,
            string photoVariantId,
            string rewardEquipmentId)
        {
            StartedUtcSeconds = startedUtcSeconds;
            CompletesUtcSeconds = completesUtcSeconds;
            WeaponId = weaponId;
            ShieldId = shieldId;
            AccessoryId = accessoryId;
            LocationId = locationId;
            PhotoVariantId = photoVariantId;
            RewardEquipmentId = rewardEquipmentId;
        }
    }
}
