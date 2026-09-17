using System.Collections.Generic;
using Mono.Data.Sqlite;

namespace Scripts.Features.SystemScene.DataGateway
{
    public sealed class DataGatewaySaveMetadataWriter
    {
        readonly DataGatewayWriter _writer;

        public DataGatewaySaveMetadataWriter(DataGatewayWriter writer)
        {
            _writer = writer;
        }

        public void Insert(SaveMetadataData data, SqliteTransaction transaction)
        {
            _writer.WriteSave("save_metadata_sql.xml", "insert", "insert", new Dictionary<string, object>
            {
                ["@schema_version"] = data.SchemaVersion,
                ["@base_content_version"] = data.BaseContentVersion,
                ["@created_utc_seconds"] = data.CreatedUtcSeconds,
                ["@updated_utc_seconds"] = data.UpdatedUtcSeconds,
                ["@logical_utc_seconds"] = data.LogicalUtcSeconds
            }, transaction);
        }

        public void UpdateTime(long logicalUtcSeconds, long updatedUtcSeconds, SqliteTransaction transaction)
        {
            _writer.WriteSave("save_metadata_sql.xml", "update", "update_time", new Dictionary<string, object>
            {
                ["@logical_utc_seconds"] = logicalUtcSeconds,
                ["@updated_utc_seconds"] = updatedUtcSeconds
            }, transaction);
        }

        public void UpdateBaseContentVersion(int contentVersion, SqliteTransaction transaction)
        {
            _writer.WriteSave("save_metadata_sql.xml", "update", "update_base_content_version", new Dictionary<string, object>
            {
                ["@base_content_version"] = contentVersion
            }, transaction);
        }
    }
}
