namespace Scripts.Features.SystemScene.DataGateway
{
    public sealed class EquippedItemData
    {
        public string SlotType { get; }
        public string EquipmentId { get; }

        public EquippedItemData(string slotType, string equipmentId)
        {
            SlotType = slotType;
            EquipmentId = equipmentId;
        }
    }
}
