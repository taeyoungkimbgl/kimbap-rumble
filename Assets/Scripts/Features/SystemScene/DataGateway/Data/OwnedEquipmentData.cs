namespace Scripts.Features.SystemScene.DataGateway
{
    public sealed class OwnedEquipmentData
    {
        public string EquipmentId { get; }
        public long AcquiredUtcSeconds { get; }

        public OwnedEquipmentData(string equipmentId, long acquiredUtcSeconds)
        {
            EquipmentId = equipmentId;
            AcquiredUtcSeconds = acquiredUtcSeconds;
        }
    }
}
