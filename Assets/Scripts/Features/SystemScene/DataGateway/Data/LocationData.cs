namespace Scripts.Features.SystemScene.DataGateway
{
    public sealed class LocationData
    {
        public string LocationId { get; }
        public string InternalName { get; }
        public int SortOrder { get; }

        public LocationData(string locationId, string internalName, int sortOrder)
        {
            LocationId = locationId;
            InternalName = internalName;
            SortOrder = sortOrder;
        }
    }
}
