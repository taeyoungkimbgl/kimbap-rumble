namespace Scripts.Features.SystemScene.DataGateway
{
    public sealed class SaveMetadataData
    {
        public int SchemaVersion { get; }
        public int BaseContentVersion { get; }
        public long CreatedUtcSeconds { get; }
        public long UpdatedUtcSeconds { get; }
        public long LogicalUtcSeconds { get; }

        public SaveMetadataData(
            int schemaVersion,
            int baseContentVersion,
            long createdUtcSeconds,
            long updatedUtcSeconds,
            long logicalUtcSeconds)
        {
            SchemaVersion = schemaVersion;
            BaseContentVersion = baseContentVersion;
            CreatedUtcSeconds = createdUtcSeconds;
            UpdatedUtcSeconds = updatedUtcSeconds;
            LogicalUtcSeconds = logicalUtcSeconds;
        }
    }
}
