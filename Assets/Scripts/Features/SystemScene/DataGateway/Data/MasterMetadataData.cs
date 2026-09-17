namespace Scripts.Features.SystemScene.DataGateway
{
    public sealed class MasterMetadataData
    {
        public string PackId { get; }
        public int SchemaVersion { get; }
        public int ContentVersion { get; }

        public MasterMetadataData(string packId, int schemaVersion, int contentVersion)
        {
            PackId = packId;
            SchemaVersion = schemaVersion;
            ContentVersion = contentVersion;
        }
    }
}
