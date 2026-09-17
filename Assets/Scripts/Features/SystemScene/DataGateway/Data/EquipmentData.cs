namespace Scripts.Features.SystemScene.DataGateway
{
    public sealed class EquipmentData
    {
        public string EquipmentId { get; }
        public string SlotType { get; }
        public string NameKey { get; }
        public string HomeImagePath { get; }
        public string PhotoImagePath { get; }
        public string ListIconImagePath { get; }
        public string DetailIconImagePath { get; }
        public bool IsInitialOwned { get; }
        public int SortOrder { get; }

        public EquipmentData(
            string equipmentId,
            string slotType,
            string nameKey,
            string homeImagePath,
            string photoImagePath,
            string listIconImagePath,
            string detailIconImagePath,
            bool isInitialOwned,
            int sortOrder)
        {
            EquipmentId = equipmentId;
            SlotType = slotType;
            NameKey = nameKey;
            HomeImagePath = homeImagePath;
            PhotoImagePath = photoImagePath;
            ListIconImagePath = listIconImagePath;
            DetailIconImagePath = detailIconImagePath;
            IsInitialOwned = isInitialOwned;
            SortOrder = sortOrder;
        }
    }
}
