using System.Collections.Generic;
using Mono.Data.Sqlite;

namespace Scripts.Features.SystemScene.DataGateway
{
    public sealed class DataGatewayDiscoveredLocationWriter
    {
        readonly DataGatewayWriter _writer;

        public DataGatewayDiscoveredLocationWriter(DataGatewayWriter writer)
        {
            _writer = writer;
        }

        public void Insert(DiscoveredLocationData data, SqliteTransaction transaction)
        {
            _writer.WriteSave("discovered_locations_sql.xml", "insert", "insert_or_ignore", new Dictionary<string, object>
            {
                ["@location_id"] = data.LocationId,
                ["@first_discovered_utc_seconds"] = data.FirstDiscoveredUtcSeconds
            }, transaction);
        }
    }
}
