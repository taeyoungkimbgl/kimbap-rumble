using System.Collections.Generic;
using Mono.Data.Sqlite;

namespace Scripts.Features.SystemScene.DataGateway
{
    public sealed class DataGatewayMasterMetadataWriter
    {
        readonly DataGatewayWriter _writer;

        public DataGatewayMasterMetadataWriter(DataGatewayWriter writer)
        {
            _writer = writer;
        }

        public void Upsert(MasterMetadataData data, SqliteConnection connection, SqliteTransaction transaction)
        {
            _writer.Write(connection, "master_metadata_sql.xml", "insert", "upsert", new Dictionary<string, object>
            {
                ["@pack_id"] = data.PackId,
                ["@schema_version"] = data.SchemaVersion,
                ["@content_version"] = data.ContentVersion
            }, transaction);
        }

        public void Delete(string packId, SqliteConnection connection, SqliteTransaction transaction)
        {
            _writer.Write(connection, "master_metadata_sql.xml", "delete", "delete_by_id", new Dictionary<string, object>
            {
                ["@pack_id"] = packId
            }, transaction);
        }
    }
}
