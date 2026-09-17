namespace Scripts.Features.SystemScene.DataGateway
{
    public sealed class DiscoveredLocationData
    {
        public string LocationId { get; }
        public long FirstDiscoveredUtcSeconds { get; }

        public DiscoveredLocationData(string locationId, long firstDiscoveredUtcSeconds)
        {
            LocationId = locationId;
            FirstDiscoveredUtcSeconds = firstDiscoveredUtcSeconds;
        }
    }
}
