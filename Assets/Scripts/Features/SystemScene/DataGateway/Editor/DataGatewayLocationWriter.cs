using System.Collections.Generic;
using Mono.Data.Sqlite;

namespace Scripts.Features.SystemScene.DataGateway
{
    public sealed class DataGatewayLocationWriter
    {
        readonly DataGatewayWriter _writer;

        public DataGatewayLocationWriter(DataGatewayWriter writer)
        {
            _writer = writer;
        }

        public void Upsert(LocationData data, SqliteConnection connection, SqliteTransaction transaction)
        {
            _writer.Write(connection, "locations_sql.xml", "insert", "upsert", new Dictionary<string, object>
            {
                ["@location_id"] = data.LocationId,
                ["@internal_name"] = data.InternalName,
                ["@sort_order"] = data.SortOrder
            }, transaction);
        }

        public void Delete(string locationId, SqliteConnection connection, SqliteTransaction transaction)
        {
            _writer.Write(connection, "locations_sql.xml", "delete", "delete_by_id", new Dictionary<string, object>
            {
                ["@location_id"] = locationId
            }, transaction);
        }
    }
}
